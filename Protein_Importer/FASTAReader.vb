Option Strict Off

Imports System
Imports System.IO
Imports SequenceInfoCalculator

Public Class FASTAReaderNotInitializedException
    Inherits System.ApplicationException
    Public Overrides ReadOnly Property Message() As String
        Get
            Return "The FASTAReader instance has not been properly initialized."
        End Get
    End Property
End Class

Public Class FASTAReader
    Implements IReadProteinImportFile

    Private m_FASTAFilePath As String
    Private m_initialized As Boolean = False

    Private m_LastError As String
    Private ReadOnly m_DescLineRegEx As System.Text.RegularExpressions.Regex
    Private ReadOnly m_NoDescLineRegEx As System.Text.RegularExpressions.Regex
    Private ReadOnly m_DescLineMatcher As System.Text.RegularExpressions.Regex

    Private m_DefaultNameField As String = "Name"
    Private m_DefaultDescField As String = "Description"
    Private m_DefaultSeqField As String = "Sequence"
    Private m_DefaultLenField As String = "Length"
    Private m_MolFormField As String = "Molecular_Formula"
    Private m_MonoMassField As String = "Monoisotopic_Mass"
    Private m_AvgMassField As String = "Average_Mass"

#Region " Events "

    Protected Event LoadStart(taskTitle As String) Implements IReadProteinImportFile.LoadStart
    Protected Event LoadEnd() Implements IReadProteinImportFile.LoadEnd
    Protected Event LoadProgress(fractionDone As Double) Implements IReadProteinImportFile.LoadProgress

#End Region

    Public Sub New()
        m_DescLineMatcher = New System.Text.RegularExpressions.Regex("^\>.+$")
        m_DescLineRegEx = New System.Text.RegularExpressions.Regex("^\>(?<name>\S+)\s+(?<description>.*)$")
        m_NoDescLineRegEx = New System.Text.RegularExpressions.Regex("^\>(?<name>\S+)$")

    End Sub

    Protected ReadOnly Property LastErrorMessage() As String Implements IReadProteinImportFile.LastErrorMessage
        Get
            Return m_LastError
        End Get
    End Property

    Protected Function LoadFASTAFile(FilePath As String) As Protein_Storage.IProteinStorage Implements IReadProteinImportFile.GetProteinEntries
        Return LoadFASTAFile(FilePath, -1)
    End Function

    Protected Function LoadFASTAFile(FilePath As String, NumRecordsToLoad As Integer) As Protein_Storage.IProteinStorage Implements IReadProteinImportFile.GetProteinEntries

        Dim fileLength As Integer
        Dim currPos As Integer = 0

        Dim fastaContents As Protein_Storage.IProteinStorage = New Protein_Storage.clsProteinStorage(FilePath)

        Dim strORFTemp As String = String.Empty
        Dim strDescTemp As String = String.Empty
        Dim strSeqTemp As String = String.Empty
        Dim descMatch As System.Text.RegularExpressions.Match

        Dim seqInfo As ICalculateSeqInfo = New SequenceInfoCalculator.SequenceInfoCalculator

        Dim recordCount As Integer

        m_FASTAFilePath = FilePath

        Dim lineEndCharCount As Integer = LineEndCharacterCount(FilePath)

        Try

            Dim fi = New FileInfo(m_FASTAFilePath)
            fileLength = fi.Length
            If (fi.Exists And fileLength > 0) Then

                RaiseEvent LoadStart("Reading Source File...") 'Trigger the setup of the pgb

                Using fileReader = New StreamReader(New FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))

                    Dim s = fileReader.ReadLine.Trim

                    Do While Not s Is Nothing
                        If m_DescLineMatcher.IsMatch(s) Then
                            'DescriptionLine, new record
                            If currPos > 0 Then 'dump current record
                                seqInfo.CalculateSequenceInfo(strSeqTemp)
                                recordCount += 1
                                If recordCount Mod 100 = 0 Then
                                    RaiseEvent LoadProgress(CSng(currPos / fileLength))     'trigger pgb update every 10th record
                                End If
                                fastaContents.AddProtein(New Protein_Storage.clsProteinStorageEntry(
                                 strORFTemp, strDescTemp, strSeqTemp, seqInfo.SequenceLength,
                                 seqInfo.MonoIsotopicMass, seqInfo.AverageMass,
                                 seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount))
                            End If

                            strORFTemp = ""
                            strDescTemp = ""
                            strSeqTemp = ""

                            If m_DescLineRegEx.IsMatch(s) Then
                                descMatch = m_DescLineRegEx.Match(s)
                                strORFTemp = descMatch.Groups("name").Value
                                strDescTemp = descMatch.Groups("description").Value
                            ElseIf m_NoDescLineRegEx.IsMatch(s) Then
                                descMatch = m_NoDescLineRegEx.Match(s)
                                strORFTemp = descMatch.Groups(1).Value
                                strDescTemp = ""
                            End If
                        Else
                            strSeqTemp &= s
                        End If
                        If NumRecordsToLoad > 0 And recordCount >= NumRecordsToLoad - 1 Then
                            Exit Do
                        End If
                        currPos += s.Length + lineEndCharCount

                        If fileReader.EndOfStream Then
                            Exit Do
                        End If

                        s = fileReader.ReadLine.Trim
                    Loop

                    'dump the last record
                    seqInfo.CalculateSequenceInfo(strSeqTemp)
                    recordCount += 1

                    fastaContents.AddProtein(New Protein_Storage.clsProteinStorageEntry(
                     strORFTemp, strDescTemp, strSeqTemp, seqInfo.SequenceLength,
                     seqInfo.MonoIsotopicMass, seqInfo.AverageMass,
                     seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount))

                    RaiseEvent LoadEnd()

                End Using

            End If

        Catch e As Exception
            ' Exception occurred
            ' For safety, we will clear fastaContents
            m_LastError = e.Message
        End Try

        Return fastaContents

    End Function

    Protected Function LineEndCharacterCount(FilePath As String) As Integer

        Dim fi = New FileInfo(m_FASTAFilePath)
        If (fi.Exists) Then
            Using fileReader = New StreamReader(New FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                While Not fileReader.EndOfStream
                    Dim testcode = fileReader.Read()
                    If testcode = 10 OrElse testcode = 13 Then
                        If fileReader.EndOfStream Then
                            Return 1
                        End If

                        Dim testcode2 = fileReader.Read()
                        If testcode2 = 10 Or testcode2 = 13 Then
                            Return 2
                        Else
                            Return 1
                        End If
                    End If
                End While
            End Using

        End If

        Return 2

    End Function
End Class
