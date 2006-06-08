Imports Protein_Exporter.ExportProteinCollectionsIFC

Public MustInherit Class clsExportProteins
    Implements ExportProteinCollectionsIFC.IExportProteins

    Public Sub New()

    End Sub

    Protected Event ExportStart(ByVal taskTitle As String) Implements IExportProteins.ExportStart
    Protected Event ExportProgress(ByVal statusMsg As String, ByVal fractionDone As Double) Implements IExportProteins.ExportProgress
    Protected Event ExportEnd() Implements IExportProteins.ExportEnd

    Protected Function Export( _
        ByRef Proteins As Protein_Storage.IProteinStorage, _
        ByRef destinationPath As String, _
        ByVal SelectedProteinList As ArrayList) As String Implements IExportProteins.Export

        Dim tmpProteinsList As Protein_Storage.IProteinStorage

        tmpProteinsList = New Protein_Storage.clsProteinStorage(System.IO.Path.GetFileNameWithoutExtension(destinationPath))

        Dim Reference As String

        For Each Reference In SelectedProteinList
            tmpProteinsList.AddProtein(Proteins.GetProtein(Reference))
        Next

        Return Export(tmpProteinsList, destinationPath)

    End Function

    Protected MustOverride Function Export( _
        ByRef Proteins As Protein_Storage.IProteinStorage, _
        ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected MustOverride Function Export( _
        ByRef ProteinTables As DataSet, _
        ByRef destinationPath As String) As String Implements IExportProteins.Export


    Protected Sub OnExportStart(ByVal taskTitle As String)
        RaiseEvent ExportStart(taskTitle)
    End Sub

    Protected Sub OnProgressUpdate(ByVal statusMsg As String, ByVal fractionDone As Double)
        RaiseEvent ExportProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportEnd()
        RaiseEvent ExportEnd()
    End Sub

    Protected Function GetFileHash(ByVal FullFilePath As String) As String _
        Implements ExportProteinCollectionsIFC.IExportProteins.GenerateFileAuthenticationHash
        'Dim shaGen As System.Security.Cryptography.MD5CryptoServiceProvider
        'shaGen = New System.Security.Cryptography.MD5CryptoServiceProvider

        Dim crcGen As CRC32
        crcGen = New CRC32
        Dim crc As Integer = 0

        Dim tmpSHA() As Byte
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



        Public Shared Function ToHexString(ByVal bytes() As Byte) As String

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


            Dim hexStr As String = ""
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
