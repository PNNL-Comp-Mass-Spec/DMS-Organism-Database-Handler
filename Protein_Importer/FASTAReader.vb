Option Strict Off

Imports System
Imports System.IO
Imports System.Text.RegularExpressions
Imports Protein_Storage
Imports SequenceInfoCalculator

Public Class FASTAReaderNotInitializedException
    Inherits ApplicationException
    Public Overrides ReadOnly Property Message As String
        Get
            Return "The FASTAReader instance has not been properly initialized."
        End Get
    End Property
End Class

Public Class FASTAReader

    Private m_FASTAFilePath As String
    Private m_initialized As Boolean = False

    Private m_LastError As String
    Private m_DefaultNameField As String = "Name"
    Private m_DefaultDescField As String = "Description"
    Private m_DefaultSeqField As String = "Sequence"
    Private m_DefaultLenField As String = "Length"
    Private m_MolFormField As String = "Molecular_Formula"
    Private m_MonoMassField As String = "Monoisotopic_Mass"
    Private m_AvgMassField As String = "Average_Mass"
    Private ReadOnly m_DescLineRegEx As Regex
    Private ReadOnly m_NoDescLineRegEx As Regex
    Private ReadOnly m_DescLineMatcher As Regex

#Region " Events "

    Public Event LoadStart(taskTitle As String)
    Public Event LoadEnd()
    Public Event LoadProgress(fractionDone As Double)

#End Region

    Public Sub New()
        m_DescLineMatcher = New Regex("^\>.+$")
        m_DescLineRegEx = New Regex("^\>(?<name>\S+)\s+(?<description>.*)$")
        m_NoDescLineRegEx = New Regex("^\>(?<name>\S+)$")

    End Sub

    Public ReadOnly Property LastErrorMessage As String
        Get
            Return m_LastError
        End Get
    End Property

    Public Function GetProteinEntries(filePath As String) As clsProteinStorage
        Return LoadFASTAFile(filePath, -1)
    End Function

    Public Function GetProteinEntries(filePath As String, numRecordsToLoad As Integer) As clsProteinStorage
        Return LoadFASTAFile(filePath, numRecordsToLoad)
    End Function

    Public Function LoadFASTAFile(filePath As String) As clsProteinStorage
        Return LoadFASTAFile(filePath, -1)
    End Function

    Public Function LoadFASTAFile(filePath As String, numRecordsToLoad As Integer) As clsProteinStorage

        Dim fileLength As Integer
        Dim currPos As Integer = 0

        Dim fastaContents As clsProteinStorage = New clsProteinStorage(filePath)

        Dim strORFTemp As String = String.Empty
        Dim strDescTemp As String = String.Empty
        Dim strSeqTemp As String = String.Empty
        Dim descMatch As Match

        Dim seqInfo = New SequenceInfoCalculator.SequenceInfoCalculator

        Dim recordCount As Integer

        m_FASTAFilePath = filePath

        Dim lineEndCharCount As Integer = LineEndCharacterCount(filePath)

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
                                fastaContents.AddProtein(New clsProteinStorageEntry(
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
                        If numRecordsToLoad > 0 And recordCount >= numRecordsToLoad - 1 Then
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

                    fastaContents.AddProtein(New clsProteinStorageEntry(
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

    Protected Function LineEndCharacterCount(filePath As String) As Integer

        Dim fi = New FileInfo(m_FASTAFilePath)
        If (fi.Exists) Then
            Using fileReader = New StreamReader(New FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                While Not fileReader.EndOfStream
                    Dim testCode = fileReader.Read()
                    If testCode = 10 OrElse testCode = 13 Then
                        If fileReader.EndOfStream Then
                            Return 1
                        End If

                        Dim testCode2 = fileReader.Read()
                        If testCode2 = 10 Or testCode2 = 13 Then
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
