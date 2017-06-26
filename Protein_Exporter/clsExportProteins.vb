Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports System.Security.Cryptography
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports Protein_Storage

Public MustInherit Class clsExportProteins
    Implements IExportProteins

    Protected m_ExportComponent As clsGetFASTAFromDMSForward

    Public Sub New(ByRef ExportComponent As clsGetFASTAFromDMSForward)
        Me.m_ExportComponent = ExportComponent
    End Sub

    Protected Event ExportStart(taskTitle As String) Implements IExportProteins.ExportStart
    Protected Event ExportProgress(statusMsg As String, fractionDone As Double) Implements IExportProteins.ExportProgress
    Protected Event ExportEnd() Implements IExportProteins.ExportEnd

    Protected Function Export(
      Proteins As IProteinStorage,
      ByRef destinationPath As String,
      SelectedProteinList As List(Of String)) As String Implements IExportProteins.Export

        Dim tmpProteinsList As IProteinStorage

        tmpProteinsList = New clsProteinStorage(Path.GetFileNameWithoutExtension(destinationPath))

        Dim Reference As String

        For Each Reference In SelectedProteinList
            tmpProteinsList.AddProtein(Proteins.GetProtein(Reference))
        Next

        Return Export(tmpProteinsList, destinationPath)

    End Function

    Protected MustOverride Function Export(
      Proteins As IProteinStorage,
      ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected MustOverride Function Export(
      ProteinTables As DataSet,
      ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected MustOverride Function Export(
      ProteinTable As DataTable,
      ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected Sub OnExportStart(taskTitle As String)
        RaiseEvent ExportStart(taskTitle)
    End Sub

    Protected Sub OnProgressUpdate(statusMsg As String, fractionDone As Double)
        RaiseEvent ExportProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportEnd()
        RaiseEvent ExportEnd()
    End Sub

    Protected Function GetFileHash(FullFilePath As String) As String Implements IExportProteins.GenerateFileAuthenticationHash

        'Dim shaGen As System.Security.Cryptography.MD5CryptoServiceProvider
        'shaGen = New System.Security.Cryptography.MD5CryptoServiceProvider

        Dim crcGen As CRC32
        crcGen = New CRC32
        Dim crc = 0

        Dim fi As New System.IO.FileInfo(FullFilePath)
        Dim f As System.IO.Stream

        If fi.Exists Then
            f = fi.OpenRead()

            'tmpSHA = shaGen.ComputeHash(f)
            crc = crcGen.GetCrc32(f)
            Dim crcString As String = String.Format("{0:X8}", crc)

            'Dim SHA1string As String = HexConverter.ToHexString(tmpSHA)

            f.Close()
            f = Nothing
            fi = Nothing
            'Return SHA1string
            Return crcString
        Else
            Return Nothing
        End If
    End Function

    Class HexConverter

        ' Private Shared hexDigits As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c}



        Public Shared Function ToHexString(bytes() As Byte) As String

            'hash = sha1.Hash;
            '   buff = new StringBuilder();
            '   foreach (byte hashByte in hash)
            '   {
            '      buff.Append(String.Format("{0:X1}", hashByte));
            '   }
            '   Console.WriteLine("Hash: {0}", buff.ToString());





            'Dim hexStr As String = ""
            'Dim i As Integer = 0

            'Dim sb As New System.Text.StringBuilder

            'For i = 0 To bytes.Length - 1

            '    sb.Append(bytes(i).ToString("X").PadLeft(2, "0"c))

            'Next

            'hexStr = sb.ToString

            'Return hexStr


            Dim hexStr = ""
            'Dim i As Integer = 0
            Dim b As Byte

            Dim sb As New System.Text.StringBuilder

            'For i = 0 To bytes.Length - 1
            For Each b In bytes

                'sb.Append(bytes(i).ToString("X").PadLeft(2, "0"c))
                sb.Append(String.Format("{0:X1}", b))

            Next

            hexStr = sb.ToString

            Return hexStr


        End Function 'ToHexString

    End Class 'HexConverter

End Class
