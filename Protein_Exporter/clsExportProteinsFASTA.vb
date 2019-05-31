Option Strict On

Imports System.IO
Imports System.Text.RegularExpressions
Imports PRISM
Imports PRISMWin
Imports Protein_Storage

Public Class clsExportProteinsFASTA
    Inherits clsExportProteins

    Private ReadOnly m_seqLineLength As Integer = 60

    Public Sub New(exportComponent As clsGetFASTAFromDMSForward)
        MyBase.New(exportComponent)

    End Sub

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="Proteins"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overloads Overrides Function Export(
      proteins As clsProteinStorage,
      ByRef destinationPath As String) As String

        Const REQUIRED_SIZE_MB = 150

        Dim currentFreeSpaceBytes As Int64
        Dim errorMessage As String = String.Empty

        Dim success = DiskInfo.GetDiskFreeSpace(destinationPath, currentFreeSpaceBytes, errorMessage)
        If Not success Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message"
            Throw New IOException("Unable to save FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        If Not FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, errorMessage) Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message"
            Throw New IOException("Unable to save FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        Using writer = New StreamWriter(destinationPath)

            Dim proteinPosition As Integer
            Dim proteinLength As Integer

            Dim tmpSeq As String
            Dim tmpName As String
            Dim tmpDesc As String
            Dim seqLine As String
            Dim tmpPC As clsProteinStorageEntry
            Dim tmpAltNames As String = String.Empty

            OnExportStart("Writing to FASTA File")

            Dim counterMax As Integer = proteins.ProteinCount
            Dim counter As Integer
            Dim hexCodeFinder = New Regex("[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled)


            Dim EventTriggerThresh As Integer
            If counterMax <= 25 Then
                EventTriggerThresh = 1
            Else
                EventTriggerThresh = CInt(counterMax / 25)
            End If

            Dim nameList = proteins.GetSortedProteinNames

            For Each tmpName In nameList

                OnExportStart("Writing: " + tmpName)

                tmpPC = proteins.GetProtein(tmpName)
                tmpSeq = tmpPC.Sequence

                counter += 1

                If (counter Mod EventTriggerThresh) = 0 Then
                    OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 3))
                End If

                proteinLength = tmpSeq.Length
                tmpDesc = hexCodeFinder.Replace(tmpPC.Description, " ")


                writer.WriteLine((">" & tmpPC.Reference & " " & tmpDesc & tmpAltNames).Trim())

                For proteinPosition = 1 To proteinLength Step m_seqLineLength
                    seqLine = Mid(tmpSeq, proteinPosition, m_seqLineLength)
                    writer.WriteLine(seqLine)
                Next
            Next

        End Using

        Dim fingerprint As String = GenerateFileAuthenticationHash(destinationPath)

        Dim fi = New FileInfo(destinationPath)

        Dim newDestinationPath As String
        newDestinationPath = Path.Combine(
          Path.GetDirectoryName(destinationPath),
          fingerprint + Path.GetExtension(destinationPath))

        Dim targetFI = New FileInfo(newDestinationPath)

        If fi.Exists Then
            If targetFI.Exists Then
                targetFI.Delete()
            End If
            fi.MoveTo(newDestinationPath)
            destinationPath = newDestinationPath
        End If

        OnExportEnd()

        Return fingerprint

    End Function

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteinTables"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Public Overloads Overrides Function Export(
      proteinTables As DataSet,
      ByRef destinationPath As String) As String

        For Each proteinTable As DataTable In proteinTables.Tables
            WriteFromDataTable(proteinTable, destinationPath)
        Next

        Return FinalizeFile(destinationPath)

    End Function

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteinTable"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Public Overloads Overrides Function Export(
      proteinTable As DataTable,
      ByRef destinationPath As String) As String

        If proteinTable.Rows.Count > 0 Then
            WriteFromDataTable(proteinTable, destinationPath)
        Else
            Return FinalizeFile(destinationPath)
        End If

        Return destinationPath

    End Function

    Function WriteFromDataTable(proteinTable As DataTable, destinationPath As String) As Integer

        Const REQUIRED_SIZE_MB = 150

        Dim counterMax As Integer ' = ProteinTable.Rows.Count
        Dim counter As Integer
        Dim proteinsWritten = 0

        Dim hexCodeFinder = New Regex("[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled)

        Dim tmpAltNames As String = String.Empty
        Dim EventTriggerThresh As Integer

        Dim currentFreeSpaceBytes As Int64
        Dim errorMessage As String = String.Empty

        Dim success = DiskInfo.GetDiskFreeSpace(destinationPath, currentFreeSpaceBytes, errorMessage)
        If Not success Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message"
            Throw New IOException("Unable to append to FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        If Not FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, errorMessage) Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message"
            Throw New IOException("Unable to append to FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        ' Open the output file for append
        Using writer = New StreamWriter(New FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.Read))

            'OnDetailedExportStart("Writing: " + proteinTable.TableName)

            counterMax = proteinTable.Rows.Count
            If counterMax <= 25 Then
                EventTriggerThresh = 1
            Else
                EventTriggerThresh = CInt(counterMax / 25)
            End If

            Dim foundRows = proteinTable.Select("")

            For Each currentRow In foundRows
                Dim tmpSeq = m_ExportComponent.SequenceExtender(currentRow.Item("Sequence").ToString, proteinTable.Rows.Count)

                counter += 1

                If (counter Mod EventTriggerThresh) = 0 Then
                    'OnDetailedProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 3))
                End If

                Dim proteinLength = tmpSeq.Length
                Dim tmpDesc = hexCodeFinder.Replace(currentRow.Item("Description").ToString, " ")
                Dim tmpName = m_ExportComponent.ReferenceExtender(currentRow.Item("Name").ToString)

                writer.WriteLine((">" & tmpName & " " & tmpDesc & tmpAltNames).Trim())

                For proteinPosition = 1 To proteinLength Step m_seqLineLength
                    Dim seqLinePortion = Mid(tmpSeq, proteinPosition, m_seqLineLength)
                    writer.WriteLine(seqLinePortion)
                Next

                proteinsWritten += 1
            Next

        End Using


        Return proteinsWritten

    End Function

    ''' <summary>
    ''' Rename the file to include the fingerprint
    ''' </summary>
    ''' <param name="destinationPath">File path to finalize; will get updated with the new name that includes the fingerprint</param>
    ''' <returns>Fingerprint, e.g. 9B916A8B</returns>
    Function FinalizeFile(ByRef destinationPath As String) As String
        Dim fingerprint As String = GenerateFileAuthenticationHash(destinationPath)

        Dim fi = New FileInfo(destinationPath)

        Dim newDestinationPath As String
        newDestinationPath = Path.Combine(
                Path.GetDirectoryName(destinationPath),
                fingerprint + Path.GetExtension(destinationPath))

        Dim targetFI = New FileInfo(newDestinationPath)

        If fi.Exists Then
            If targetFI.Exists Then
                targetFI.Delete()
            End If
            fi.MoveTo(newDestinationPath)
            destinationPath = newDestinationPath
        End If

        OnExportEnd()

        Return fingerprint
    End Function


End Class
