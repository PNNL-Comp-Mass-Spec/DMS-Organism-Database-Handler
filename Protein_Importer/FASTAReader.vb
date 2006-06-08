Option Strict Off

Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text


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
    Private m_DescLineRegEx As System.Text.RegularExpressions.Regex
    Private m_NoDescLineRegEx As System.Text.RegularExpressions.Regex
    Private m_DescLineMatcher As System.Text.RegularExpressions.Regex

    Private m_DefaultNameField As String = "Name"
    Private m_DefaultDescField As String = "Description"
    Private m_DefaultSeqField As String = "Sequence"
    Private m_DefaultLenField As String = "Length"
    Private m_MolFormField As String = "Molecular_Formula"
    Private m_MonoMassField As String = "Monoisotopic_Mass"
    Private m_AvgMassField As String = "Average_Mass"


#Region " Events "

    Protected Event LoadStart(ByVal taskTitle As String) Implements IReadProteinImportFile.LoadStart
    Protected Event LoadEnd() Implements IReadProteinImportFile.LoadEnd
    Protected Event LoadProgress(ByVal fractionDone As Double) Implements IReadProteinImportFile.LoadProgress

#End Region

    Public Sub New()
        Me.m_DescLineMatcher = New System.Text.RegularExpressions.Regex("^\>.+$")
        Me.m_DescLineRegEx = New System.Text.RegularExpressions.Regex("^\>(?<name>\S+)\s+(?<description>.*)$")
        Me.m_NoDescLineRegEx = New System.Text.RegularExpressions.Regex("^\>(?<name>\S+)$")

        Dim rList() As System.Text.RegularExpressions.RegexCompilationInfo



    End Sub

    Private Sub InitFASTAReader(ByVal FASTAFilePath As String)
    End Sub

    Protected ReadOnly Property LastErrorMessage() As String Implements IReadProteinImportFile.LastErrorMessage
        Get
            Return Me.m_LastError
        End Get
    End Property

    Protected Function LoadFASTAFile(ByVal FilePath As String) As Protein_Storage.IProteinStorage Implements IReadProteinImportFile.GetProteinEntries
        Return Me.LoadFASTAFile(FilePath, -1)
    End Function

    Protected Function LoadFASTAFile(ByVal FilePath As String, ByVal NumRecordsToLoad As Integer) As Protein_Storage.IProteinStorage Implements IReadProteinImportFile.GetProteinEntries
        Dim fi As FileInfo
        Dim tr As TextReader
        Dim s As String
        Dim nextChar As String
        Dim fileLength As Integer
        Dim currPos As Integer = 0

        Dim fastaContents As Protein_Storage.IProteinStorage = New Protein_Storage.clsProteinStorage(FilePath)

        Dim strORFTemp As String
        Dim strDescTemp As String
        Dim strSeqTemp As String
        Dim newRecordFlag As Boolean
        Dim readDescLine As Boolean

        Dim checkRows() As DataRow

        Dim descMatch As System.Text.RegularExpressions.Match

        Dim descLineSplitPoint As Integer
        Dim seqInfo As SequenceInfoCalculator.ICalculateSeqInfo
        seqInfo = New SequenceInfoCalculator.SequenceInfoCalculator

        Dim recordCount As Integer
        Dim workrow As DataRow

        Me.m_FASTAFilePath = FilePath
        Dim fileName As String = System.IO.Path.GetFileNameWithoutExtension(FilePath)

        Dim lineEndCharCount As Integer = Me.LineEndCharacterCount(FilePath)

        Dim tmpPath As String = System.IO.Path.GetTempFileName

        Try

            fi = New FileInfo(m_FASTAFilePath)
            fileLength = fi.Length
            If (fi.Exists) Then


                RaiseEvent LoadStart("Reading Source File...") 'Trigger the setup of the pgb

                tr = fi.OpenText
                s = tr.ReadLine
                Do While Not s Is Nothing
                    If Me.m_DescLineMatcher.IsMatch(s) Then
                        'DescriptionLine, new record
                        If currPos > 0 Then 'dump current record
                            seqInfo.CalculateSequenceInfo(strSeqTemp)
                            recordCount += 1
                            If recordCount Mod 100 = 0 Then
                                RaiseEvent LoadProgress(CSng(currPos / fileLength))     'trigger pgb update every 10th record
                            End If

                            fastaContents.AddProtein(New Protein_Storage.clsProteinStorageEntry( _
                                strORFTemp, strDescTemp, strSeqTemp, seqInfo.SequenceLength, _
                                seqInfo.MonoIsotopicMass, seqInfo.AverageMass, _
                                seqInfo.MolecularFormula, seqInfo.SHA1Hash))
                        End If

                        strORFTemp = ""
                        strDescTemp = ""
                        strSeqTemp = ""

                        If Me.m_DescLineRegEx.IsMatch(s) Then
                            descMatch = Me.m_DescLineRegEx.Match(s)
                            strORFTemp = descMatch.Groups("name").Value
                            strDescTemp = descMatch.Groups("description").Value
                        ElseIf Me.m_NoDescLineRegEx.IsMatch(s) Then
                            descMatch = Me.m_NoDescLineRegEx.Match(s)
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
                    s = tr.ReadLine
                Loop

                'dump the last record
                seqInfo.CalculateSequenceInfo(strSeqTemp)
                recordCount += 1

                fastaContents.AddProtein(New Protein_Storage.clsProteinStorageEntry( _
                    strORFTemp, strDescTemp, strSeqTemp, seqInfo.SequenceLength, _
                    seqInfo.MonoIsotopicMass, seqInfo.AverageMass, _
                    seqInfo.MolecularFormula, seqInfo.SHA1Hash))


                RaiseEvent LoadEnd()

            End If
            tr.Close()


        Catch e As Exception
            Me.m_LastError = e.Message
        Finally
            If (Not tr Is Nothing) Then
                tr.Close()
            End If
        End Try

        Return fastaContents

    End Function

    Protected Function LineEndCharacterCount(ByVal FilePath As String) As Integer
        Dim fi As FileInfo
        Dim tr As TextReader
        Dim s As String
        Dim testchar() As Char
        Dim testcode As Integer
        Dim testcode2 As Integer
        Dim counter As Integer
        Dim endCount As Integer

        fi = New FileInfo(m_FASTAFilePath)
        If (fi.Exists) Then
            tr = fi.OpenText
            For counter = 1 To fi.Length
                testcode = tr.Read()
                If testcode = 10 Or testcode = 13 Then
                    testcode2 = tr.Read()
                    If testcode2 = 10 Or testcode2 = 13 Then
                        Return 2
                    Else
                        Return 1
                    End If
                End If
            Next

        End If

    End Function
End Class
