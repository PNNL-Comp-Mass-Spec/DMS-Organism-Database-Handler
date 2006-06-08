Public Class clsExportProteinsXTFASTA
    Inherits clsExportProteins

    Public Sub New()
        MyBase.New()
    End Sub

    Const HEADER_STRING As String = "xbang-pro-fasta-format"


    Protected Overloads Overrides Function Export( _
        ByRef Proteins As Protein_Storage.IProteinStorage, _
        ByRef destinationPath As String) As String


        Dim buffer(HEADER_STRING.Length) As Byte

        buffer = System.Text.Encoding.Default.GetBytes(HEADER_STRING)

        ReDim Preserve buffer(255)

        Dim bw As System.IO.BinaryWriter = New System.IO.BinaryWriter(IO.File.OpenWrite(destinationPath))

        bw.BaseStream.Seek(0, IO.SeekOrigin.Begin)

        Dim e As IEnumerator = Proteins.GetEnumerator
        Dim descLine As String
        Dim seqLine As String
        Dim proteinPosition As Integer
        Dim proteinLength As Integer

        Dim tmpSeq As String
        Dim tmpName As String
        Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim tmpNum As Int32

        Me.OnExportStart("Writing to X!Tandem formatted FASTA File")

        Dim counterMax As Integer = Proteins.ProteinCount
        Dim counter As Integer

        Dim proteinArray As New ArrayList

        Dim ProteinEnum As IDictionaryEnumerator = Proteins.GetEnumerator

        While ProteinEnum.MoveNext = True
            proteinArray.Add(ProteinEnum.Key)
        End While

        proteinArray.Sort()

        Dim encoding As New System.Text.ASCIIEncoding

        Dim EventTriggerThresh As Integer
        If counterMax <= 10 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 10)
        End If

        bw.Write(buffer)

        For Each tmpName In proteinArray
            Me.OnExportStart("Writing: " + tmpName)
            tmpPC = Proteins.GetProtein(tmpName)
            tmpSeq = tmpPC.Sequence

            counter += 1

            If (counter Mod EventTriggerThresh) = 0 Then
                Me.OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 0))

            End If

            proteinLength = tmpSeq.Length

            Array.Clear(buffer, 0, 4)
            tmpNum = tmpName.Length + 1
            buffer = Me.ConvIntegertoByteArray(tmpNum, 4)
            Array.Reverse(buffer)

            bw.Write(buffer)
            buffer = encoding.GetBytes(tmpName)
            bw.Write(buffer)
            bw.Write(Me.ConvIntegertoByteArray(0, 1))

            Array.Clear(buffer, 0, 4)
            tmpNum = proteinLength + 1
            buffer = Me.ConvIntegertoByteArray(tmpNum, 4)
            Array.Reverse(buffer)

            bw.Write(buffer)
            buffer = encoding.GetBytes(tmpSeq)
            bw.Write(buffer)
            bw.Write(Me.ConvIntegertoByteArray(0, 1))


        Next
        bw.Flush()
        bw.Close()

        bw = Nothing

        Dim fingerprint As String = Me.GetFileHash(destinationPath)

        Me.OnExportEnd()

        Return fingerprint

    End Function

    Protected Overloads Overrides Function Export( _
    ByRef ProteinTables As DataSet, _
    ByRef destinationPath As String) As String


        Dim buffer(HEADER_STRING.Length) As Byte

        buffer = System.Text.Encoding.Default.GetBytes(HEADER_STRING)

        ReDim Preserve buffer(255)

        Dim bw As System.IO.BinaryWriter = New System.IO.BinaryWriter(IO.File.OpenWrite(destinationPath))

        bw.BaseStream.Seek(0, IO.SeekOrigin.Begin)

        'Dim e As IEnumerator = Proteins.GetEnumerator
        Dim descLine As String
        Dim seqLine As String
        Dim proteinPosition As Integer
        Dim proteinLength As Integer

        Dim proteinTable As DataTable

        Dim dr As DataRow
        Dim foundRows() As DataRow

        Dim tmpSeq As String
        Dim tmpName As String
        Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim tmpNum As Int32

        Me.OnExportStart("Writing to X!Tandem formatted FASTA File")

        'Dim counterMax As Integer = Proteins.ProteinCount
        Dim counterMax As Integer ' = ProteinTable.Rows.Count
        Dim counter As Integer

        'Dim proteinArray As New ArrayList

        'Dim ProteinEnum As IDictionaryEnumerator = Proteins.GetEnumerator

        'While ProteinEnum.MoveNext = True
        '    proteinArray.Add(ProteinEnum.Key)
        'End While

        'proteinArray.Sort()

        For Each proteinTable In ProteinTables.Tables
            Me.OnExportStart("Writing: " + proteinTable.TableName)
            counterMax = proteinTable.Rows.Count
            foundRows = proteinTable.Select("", "Name")

            Dim encoding As New System.Text.ASCIIEncoding

            Dim EventTriggerThresh As Integer
            If counterMax <= 10 Then
                EventTriggerThresh = 1
            Else
                EventTriggerThresh = CInt(counterMax / 10)
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
                    Me.OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 0))
                End If

                proteinLength = tmpSeq.Length

                Array.Clear(buffer, 0, 4)
                tmpNum = tmpName.Length + 1
                buffer = Me.ConvIntegertoByteArray(tmpNum, 4)
                Array.Reverse(buffer)

                bw.Write(buffer)
                buffer = encoding.GetBytes(tmpName)
                bw.Write(buffer)
                bw.Write(Me.ConvIntegertoByteArray(0, 1))

                Array.Clear(buffer, 0, 4)
                tmpNum = proteinLength + 1
                buffer = Me.ConvIntegertoByteArray(tmpNum, 4)
                Array.Reverse(buffer)

                bw.Write(buffer)
                buffer = encoding.GetBytes(tmpSeq)
                bw.Write(buffer)
                bw.Write(Me.ConvIntegertoByteArray(0, 1))


            Next
        Next
        bw.Flush()
        bw.Close()

        bw = Nothing

        Dim fingerprint As String = Me.GetFileHash(destinationPath)

        Me.OnExportEnd()

        Return fingerprint

    End Function

    Friend Function ConvIntegertoByteArray(ByVal n As Long, ByVal lg As Integer) As Byte()
        'converts an integer to a byte array of length lg
        Dim m() As Byte = New Byte(lg - 1) {}
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

    Public Function ConvByteArraytoInteger(ByVal b As Byte(), Optional ByVal ln As Integer = 0, Optional ByVal sidx As Integer = 0) As Long
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
