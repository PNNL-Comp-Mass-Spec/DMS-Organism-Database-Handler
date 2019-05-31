Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports PRISM
Imports PRISMWin
Imports Protein_Storage

Public Class clsExportProteinsXTFASTA
    Inherits clsExportProteins

    Public Sub New(exportComponent As clsGetFASTAFromDMSForward)
        MyBase.New(exportComponent)
    End Sub

    Const HEADER_STRING As String = "xbang-pro-fasta-format"

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Protected Overloads Overrides Function Export(
      proteins As IProteinStorage,
      ByRef destinationPath As String) As String

        Const REQUIRED_SIZE_MB = 150

        Dim buffer = Encoding.Default.GetBytes(HEADER_STRING)

        ReDim Preserve buffer(255)

        Dim currentFreeSpaceBytes As Int64
        Dim errorMessage As String = String.Empty

        Dim success = DiskInfo.GetDiskFreeSpace(destinationPath, currentFreeSpaceBytes, errorMessage)
        If Not success Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message"
            Throw New IOException("Unable to create FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        If Not FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, errorMessage) Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message"
            Throw New IOException("Unable to create FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        Using bw = New BinaryWriter(File.OpenWrite(destinationPath))

            bw.BaseStream.Seek(0, SeekOrigin.Begin)

            Dim proteinLength As Integer

            Dim tmpSeq As String
            Dim tmpName As String
            Dim tmpPC As IProteinStorageEntry
            Dim tmpNum As Int32

            OnExportStart("Writing to X!Tandem formatted FASTA File")

            Dim counterMax As Integer = proteins.ProteinCount
            Dim counter As Integer

            Dim proteinArray As New SortedSet(Of String)

            Dim proteinEnum = proteins.GetEnumerator

            While proteinEnum.MoveNext()
                proteinArray.Add(proteinEnum.Current.Key)
            End While

            Dim encoding As New ASCIIEncoding

            Dim EventTriggerThresh As Integer
            If counterMax <= 25 Then
                EventTriggerThresh = 1
            Else
                EventTriggerThresh = CInt(counterMax / 25)
            End If

            bw.Write(buffer)

            For Each tmpName In proteinArray
                OnExportStart("Writing: " + tmpName)
                tmpPC = proteins.GetProtein(tmpName)
                tmpSeq = tmpPC.Sequence

                counter += 1

                If (counter Mod EventTriggerThresh) = 0 Then
                    OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 3))

                End If

                proteinLength = tmpSeq.Length

                Array.Clear(buffer, 0, 4)
                tmpNum = tmpName.Length + 1
                buffer = ConvIntegerToByteArray(tmpNum, 4)
                Array.Reverse(buffer)

                bw.Write(buffer)
                buffer = encoding.GetBytes(tmpName)
                bw.Write(buffer)
                bw.Write(ConvIntegerToByteArray(0, 1))

                Array.Clear(buffer, 0, 4)
                tmpNum = proteinLength + 1
                buffer = ConvIntegerToByteArray(tmpNum, 4)
                Array.Reverse(buffer)

                bw.Write(buffer)
                buffer = encoding.GetBytes(tmpSeq)
                bw.Write(buffer)
                bw.Write(ConvIntegerToByteArray(0, 1))


            Next

        End Using

        Dim fingerprint As String = GetFileHash(destinationPath)

        OnExportEnd()

        Return fingerprint

    End Function

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteinTables"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Protected Overloads Overrides Function Export(
      proteinTables As DataSet,
      ByRef destinationPath As String) As String

        Const REQUIRED_SIZE_MB = 150

        Dim buffer = Encoding.Default.GetBytes(HEADER_STRING)

        ReDim Preserve buffer(255)

        Dim currentFreeSpaceBytes As Int64
        Dim errorMessage As String = String.Empty

        Dim success = DiskInfo.GetDiskFreeSpace(destinationPath, currentFreeSpaceBytes, errorMessage)
        If Not success Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message"
            Throw New IOException("Unable to create FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        If Not FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, errorMessage) Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message"
            Throw New IOException("Unable to create FASTA file at " & destinationPath & ". " & errorMessage)
        End If

        Using bw = New BinaryWriter(File.OpenWrite(destinationPath))

            bw.BaseStream.Seek(0, SeekOrigin.Begin)

            'Dim e As IEnumerator = Proteins.GetEnumerator

            Dim proteinLength As Integer

            Dim proteinTable As DataTable

            Dim dr As DataRow
            Dim foundRows() As DataRow

            Dim tmpSeq As String
            Dim tmpName As String

            Dim tmpNum As Int32

            OnExportStart("Writing to X!Tandem formatted FASTA File")

            'Dim counterMax As Integer = Proteins.ProteinCount
            Dim counterMax As Integer ' = ProteinTable.Rows.Count
            Dim counter As Integer

            For Each proteinTable In proteinTables.Tables
                OnExportStart("Writing: " + proteinTable.TableName)
                counterMax = proteinTable.Rows.Count
                foundRows = proteinTable.Select("", "Name")

                Dim encoding As New ASCIIEncoding

                Dim EventTriggerThresh As Integer
                If counterMax <= 25 Then
                    EventTriggerThresh = 1
                Else
                    EventTriggerThresh = CInt(counterMax / 25)
                End If

                bw.Write(buffer)

                For Each dr In foundRows
                    'tmpPC = Proteins.GetProtein(tmpName)
                    'tmpSeq = tmpPC.Sequence
                    tmpSeq = dr.Item("Sequence").ToString
                    tmpName = dr.Item("Name").ToString
                    'tmpDesc = dr.Item("Description").ToString

                    counter += 1

                    If (counter Mod EventTriggerThresh) = 0 Then
                        OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 3))
                    End If

                    proteinLength = tmpSeq.Length

                    Array.Clear(buffer, 0, 4)
                    tmpNum = tmpName.Length + 1
                    buffer = ConvIntegerToByteArray(tmpNum, 4)
                    Array.Reverse(buffer)

                    bw.Write(buffer)
                    buffer = encoding.GetBytes(tmpName)
                    bw.Write(buffer)
                    bw.Write(ConvIntegerToByteArray(0, 1))

                    Array.Clear(buffer, 0, 4)
                    tmpNum = proteinLength + 1
                    buffer = ConvIntegerToByteArray(tmpNum, 4)
                    Array.Reverse(buffer)

                    bw.Write(buffer)
                    buffer = encoding.GetBytes(tmpSeq)
                    bw.Write(buffer)
                    bw.Write(ConvIntegerToByteArray(0, 1))


                Next
            Next

        End Using

        Dim fingerprint As String = GetFileHash(destinationPath)

        OnExportEnd()

        Return fingerprint

    End Function

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteinTable"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Protected Overloads Overrides Function Export(
      proteinTable As DataTable,
      ByRef destinationPath As String) As String

        ' Not implemented for this class
        Return String.Empty

    End Function

    Friend Function ConvIntegerToByteArray(n As Long, lg As Integer) As Byte()
        'converts an integer to a byte array of length lg
        Dim m = New Byte(lg - 1) {}
        Dim i, k As Integer
        Dim h As String
        h = Hex(n).PadLeft(16, "0"c)
        k = 16
        For i = lg - 1 To 0 Step -1
            k = k - 2
            m(i) = CByte("&H" & h.Substring(k, 2))
        Next
        Return m
    End Function

    Public Function ConvByteArrayToInteger(b As Byte(), Optional ln As Integer = 0, Optional sidx As Integer = 0) As Long
        Dim i As Integer
        Dim j, k As Long
        If ln = 0 Then ln = UBound(b) + 1
        ln = sidx + ln - 1
        k = 1
        j = CInt(b(ln))
        For i = ln - 1 To sidx Step -1
            k = 256 * k
            j = j + k * b(i)
        Next
        Return j
    End Function



End Class
