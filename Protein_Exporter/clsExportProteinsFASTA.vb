Public Class clsExportProteinsFASTA
    Inherits clsExportProteins

    Private m_seqLineLength As Integer = 60
    'Protected m_FileArchiver As Protein_Exporter.IArchiveOutputFiles

    Public Sub New(ByRef ExportComponent As Protein_Exporter.clsGetFASTAFromDMSForward)
        MyBase.New(ExportComponent)

    End Sub

    Protected Overloads Overrides Function Export( _
        ByRef Proteins As Protein_Storage.IProteinStorage, _
        ByRef destinationPath As String) As String

        'Dim dr As DataRow
        Dim sw As System.IO.StreamWriter = New System.IO.StreamWriter(destinationPath)

        Dim pe As Protein_Storage.IProteinStorageEntry

        Dim nameList As ArrayList

        'Dim e As IEnumerator = Proteins.GetEnumerator
        Dim descLine As String
        Dim seqLine As String
        Dim proteinPosition As Integer
        Dim proteinLength As Integer

        Dim tmpSeq As String
        Dim tmpName As String
        Dim tmpDesc As String
        Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim tmpAltNamesSB As System.Text.StringBuilder
        Dim tmpAltNames As String
        Dim s As String

        Me.OnExportStart("Writing to FASTA File")

        Dim counterMax As Integer = Proteins.ProteinCount
        Dim counter As Integer
        Dim cntrlFinder As System.Text.RegularExpressions.Regex = _
            New System.Text.RegularExpressions.Regex("[\x00-\x1F\x7F-\xFF]", Text.RegularExpressions.RegexOptions.Compiled)


        Dim EventTriggerThresh As Integer
        If counterMax <= 10 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 10)
        End If

        nameList = Proteins.GetSortedProteinNames

        For Each tmpName In nameList

            Me.OnExportStart("Writing: " + tmpName)

            tmpPC = DirectCast(Proteins.GetProtein(tmpName), Protein_Storage.IProteinStorageEntry)
            tmpSeq = tmpPC.Sequence

            counter += 1

            If (counter Mod EventTriggerThresh) = 0 Then
                Me.OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 0))
            End If

            proteinLength = tmpSeq.Length
            tmpDesc = cntrlFinder.Replace(tmpPC.Description, " ")


            sw.WriteLine(Trim(">" & tmpPC.Reference & " " & tmpDesc & tmpAltNames))

            For proteinPosition = 1 To proteinLength Step Me.m_seqLineLength
                seqLine = Mid(tmpSeq, proteinPosition, Me.m_seqLineLength)
                sw.WriteLine(seqLine)
            Next
        Next

        sw.Flush()
        sw.Close()

        sw = Nothing


        Dim fingerprint As String = Me.GetFileHash(destinationPath)

        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(destinationPath)

        Dim newDestinationPath As String
        newDestinationPath = System.IO.Path.Combine( _
                System.IO.Path.GetDirectoryName(destinationPath), _
                fingerprint + System.IO.Path.GetExtension(destinationPath))

        Dim copyFI As System.IO.FileInfo = New System.IO.FileInfo(newDestinationPath)

        If fi.Exists Then
            If copyFI.Exists Then
                copyFI.Delete()
            End If
            fi.CopyTo(newDestinationPath)
            fi.Delete()
            destinationPath = newDestinationPath
        End If
        fi = Nothing
        copyFI = Nothing

        Me.OnExportEnd()

        Return fingerprint

    End Function


    Protected Overloads Overrides Function Export( _
        ByRef ProteinTables As DataSet, _
        ByRef destinationPath As String) As String

        Dim sw As System.IO.StreamWriter = New System.IO.StreamWriter(destinationPath)

        Dim pe As Protein_Storage.IProteinStorageEntry

        Dim ProteinTable As DataTable

        Dim descLine As String
        Dim seqLine As String
        Dim proteinPosition As Integer
        Dim proteinLength As Integer

        Dim tmpSeq As String
        Dim tmpName As String
        Dim tmpDesc As String
        'Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim dr As DataRow
        Dim foundRows() As DataRow
        Dim tmpAltNamesSB As System.Text.StringBuilder
        Dim tmpAltNames As String
        Dim s As String

        'Me.OnExportStart("Writing to FASTA File")

        Dim counterMax As Integer ' = ProteinTable.Rows.Count
        Dim counter As Integer
        Dim cntrlFinder As System.Text.RegularExpressions.Regex = _
            New System.Text.RegularExpressions.Regex("[\x00-\x1F\x7F-\xFF]", Text.RegularExpressions.RegexOptions.Compiled)


        Dim EventTriggerThresh As Integer

        For Each ProteinTable In ProteinTables.Tables
            Me.OnExportStart("Writing: " + ProteinTable.TableName)
            counterMax = ProteinTable.Rows.Count
            If counterMax <= 10 Then
                EventTriggerThresh = 1
            Else
                EventTriggerThresh = CInt(counterMax / 10)
            End If

            foundRows = ProteinTable.Select("")

            For Each dr In foundRows
                tmpSeq = Me.m_ExportComponent.SequenceExtender(dr.Item("Sequence").ToString, ProteinTable.Rows.Count)


                counter += 1

                If (counter Mod EventTriggerThresh) = 0 Then
                    Me.OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 0))
                End If

                proteinLength = tmpSeq.Length
                tmpDesc = cntrlFinder.Replace(dr.Item("Description").ToString, " ")
                tmpName = Me.m_ExportComponent.ReferenceExtender(dr.Item("Name").ToString)

                sw.WriteLine(Trim(">" & tmpName & " " & tmpDesc & tmpAltNames))

                For proteinPosition = 1 To proteinLength Step Me.m_seqLineLength
                    seqLine = Mid(tmpSeq, proteinPosition, Me.m_seqLineLength)
                    sw.WriteLine(seqLine)
                Next
            Next
            counter = 0
        Next

        sw.Flush()
        sw.Close()

        sw = Nothing


        Dim fingerprint As String = Me.GetFileHash(destinationPath)

        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(destinationPath)

        Dim newDestinationPath As String
        newDestinationPath = System.IO.Path.Combine( _
                System.IO.Path.GetDirectoryName(destinationPath), _
                fingerprint + System.IO.Path.GetExtension(destinationPath))

        Dim copyFI As System.IO.FileInfo = New System.IO.FileInfo(newDestinationPath)

        If fi.Exists Then
            If copyFI.Exists Then
                copyFI.Delete()
            End If
            fi.CopyTo(newDestinationPath)
            fi.Delete()
            destinationPath = newDestinationPath
        End If
        fi = Nothing

        Me.OnExportEnd()

        Return fingerprint

    End Function



End Class
