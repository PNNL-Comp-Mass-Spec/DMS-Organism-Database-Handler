Option Strict Off

Imports System.IO
Imports System.Text.RegularExpressions
Imports Protein_Storage

Public Class FASTAReader

    Private m_FASTAFilePath As String

    Private m_LastError As String
    Private ReadOnly m_DescLineRegEx As Regex
    Private ReadOnly m_NoDescLineRegEx As Regex
    Private ReadOnly m_DescLineMatcher As Regex

#Region " Events "

    Public Event LoadStart(taskTitle As String)
    Public Event LoadEnd()
    Public Event LoadProgress(fractionDone As Double)

#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
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

    Public Function GetProteinEntries(filePath As String) As ProteinStorage
        Return LoadFASTAFile(filePath, -1)
    End Function

    Public Function GetProteinEntries(filePath As String, numRecordsToLoad As Integer) As ProteinStorage
        Return LoadFASTAFile(filePath, numRecordsToLoad)
    End Function

    Public Function LoadFASTAFile(filePath As String) As ProteinStorage
        Return LoadFASTAFile(filePath, -1)
    End Function

    Public Function LoadFASTAFile(filePath As String, numRecordsToLoad As Integer) As ProteinStorage

        Dim fileLength As Integer
        Dim currentPosition = 0

        Dim fastaContents = New ProteinStorage(filePath)

        Dim reference As String = String.Empty
        Dim description As String = String.Empty
        Dim sequence As String = String.Empty
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
                            If currentPosition > 0 Then 'dump current record
                                seqInfo.CalculateSequenceInfo(sequence)
                                recordCount += 1
                                If recordCount Mod 100 = 0 Then
                                    RaiseEvent LoadProgress(CSng(currentPosition / fileLength))     'trigger pgb update every 10th record
                                End If
                                fastaContents.AddProtein(New ProteinStorageEntry(
                                 reference, description, sequence, seqInfo.SequenceLength,
                                 seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                                 seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount))
                            End If

                            reference = String.Empty
                            description = String.Empty
                            sequence = String.Empty

                            If m_DescLineRegEx.IsMatch(s) Then
                                descMatch = m_DescLineRegEx.Match(s)
                                reference = descMatch.Groups("name").Value
                                description = descMatch.Groups("description").Value
                            ElseIf m_NoDescLineRegEx.IsMatch(s) Then
                                descMatch = m_NoDescLineRegEx.Match(s)
                                reference = descMatch.Groups(1).Value
                                description = String.Empty
                            End If
                        Else
                            sequence &= s
                        End If
                        If numRecordsToLoad > 0 And recordCount >= numRecordsToLoad - 1 Then
                            Exit Do
                        End If
                        currentPosition += s.Length + lineEndCharCount

                        If fileReader.EndOfStream Then
                            Exit Do
                        End If

                        s = fileReader.ReadLine.Trim
                    Loop

                    'dump the last record
                    seqInfo.CalculateSequenceInfo(sequence)
                    recordCount += 1

                    fastaContents.AddProtein(New ProteinStorageEntry(
                        reference, description, sequence, seqInfo.SequenceLength,
                        seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                        seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount))

                    RaiseEvent LoadEnd()

                End Using

            End If

        Catch ex As Exception
            Dim stackTrace = PRISM.StackTraceFormatter.GetExceptionStackTrace(ex)
            m_LastError = ex.Message & "; " & stackTrace
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
