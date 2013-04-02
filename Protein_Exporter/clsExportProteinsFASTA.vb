Option Strict On

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

		Dim errorMessage As String = String.Empty

		If Not PRISM.Files.clsFileTools.ValidateFreeDiskSpace(destinationPath, 150, errorMessage) Then
			If String.IsNullOrEmpty(errorMessage) Then errorMessage = "clsFileTools.ValidateFreeDiskSpace returned a blank error message"
			Throw New System.IO.IOException("Unable to save FASTA file at " & destinationPath & ". " & errorMessage)
		End If

		'Dim dr As DataRow
		Dim sw As System.IO.StreamWriter = New System.IO.StreamWriter(destinationPath)

		Dim nameList As ArrayList

		'Dim e As IEnumerator = Proteins.GetEnumerator
		Dim proteinPosition As Integer
		Dim proteinLength As Integer

		Dim tmpSeq As String
		Dim tmpName As String
		Dim tmpDesc As String
		Dim seqLine As String
		Dim tmpPC As Protein_Storage.IProteinStorageEntry
		Dim tmpAltNames As String = String.Empty

		If Not PRISM.Files.clsFileTools.ValidateFreeDiskSpace(destinationPath, 150, errorMessage) Then
			If String.IsNullOrEmpty(errorMessage) Then errorMessage = "clsFileTools.ValidateFreeDiskSpace returned a blank error message"
			Throw New System.IO.IOException("Unable to create FASTA file at " & destinationPath & ". " & errorMessage)
		End If

		Me.OnExportStart("Writing to FASTA File")

		Dim counterMax As Integer = Proteins.ProteinCount
		Dim counter As Integer
		Dim cntrlFinder As System.Text.RegularExpressions.Regex = _
		 New System.Text.RegularExpressions.Regex("[\x00-\x1F\x7F-\xFF]", Text.RegularExpressions.RegexOptions.Compiled)


		Dim EventTriggerThresh As Integer
		If counterMax <= 25 Then
			EventTriggerThresh = 1
		Else
			EventTriggerThresh = CInt(counterMax / 25)
		End If

		nameList = Proteins.GetSortedProteinNames

		For Each tmpName In nameList

			Me.OnExportStart("Writing: " + tmpName)

			tmpPC = DirectCast(Proteins.GetProtein(tmpName), Protein_Storage.IProteinStorageEntry)
			tmpSeq = tmpPC.Sequence

			counter += 1

			If (counter Mod EventTriggerThresh) = 0 Then
				Me.OnProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 3))
			End If

			proteinLength = tmpSeq.Length
			tmpDesc = cntrlFinder.Replace(tmpPC.Description, " ")


			sw.WriteLine((">" & tmpPC.Reference & " " & tmpDesc & tmpAltNames).Trim())

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

		Dim targetFI As System.IO.FileInfo = New System.IO.FileInfo(newDestinationPath)

		If fi.Exists Then
			If targetFI.Exists Then
				targetFI.Delete()
			End If
			fi.MoveTo(newDestinationPath)
			destinationPath = newDestinationPath
		End If
		fi = Nothing
		targetFI = Nothing

		Me.OnExportEnd()

		Return fingerprint

	End Function


    Protected Overloads Overrides Function Export( _
        ByRef ProteinTables As DataSet, _
        ByRef destinationPath As String) As String

        Dim ProteinTable As DataTable
        Dim writtenProteinCount As Integer

        For Each ProteinTable In ProteinTables.Tables
            writtenProteinCount = WriteFromDatatable(ProteinTable, destinationPath)
        Next


        Return Me.FinalizeFile(destinationPath)

    End Function

    Protected Overloads Overrides Function Export( _
        ByRef ProteinTable As DataTable, _
        ByRef destinationPath As String) As String

        Dim writtenProteinCount As Integer

        If ProteinTable.Rows.Count > 0 Then
            writtenProteinCount = WriteFromDatatable(ProteinTable, destinationPath)
        Else
            Return Me.FinalizeFile(destinationPath)
        End If

        Return destinationPath

    End Function

    Function WriteFromDatatable(ByRef proteinTable As DataTable, ByRef destinationPath As String) As Integer

        Dim counterMax As Integer ' = ProteinTable.Rows.Count
        Dim counter As Integer
        Dim cntrlFinder As System.Text.RegularExpressions.Regex = _
            New System.Text.RegularExpressions.Regex("[\x00-\x1F\x7F-\xFF]", Text.RegularExpressions.RegexOptions.Compiled)
        Dim dr As DataRow
        Dim foundRows() As DataRow
        Dim tmpSeq As String
        Dim tmpName As String = String.Empty
        Dim tmpDesc As String

        Dim seqLinePortion As String
        Dim proteinPosition As Integer
        Dim proteinLength As Integer

        Dim tmpAltNames As String = String.Empty
        Dim EventTriggerThresh As Integer
        Dim sw As System.IO.StreamWriter

		Dim errorMessage As String = String.Empty
		If Not PRISM.Files.clsFileTools.ValidateFreeDiskSpace(destinationPath, 150, errorMessage) Then
			If String.IsNullOrEmpty(errorMessage) Then errorMessage = "clsFileTools.ValidateFreeDiskSpace returned a blank error message"
			Throw New System.IO.IOException("Unable to append to FASTA file at " & destinationPath & ". " & errorMessage)
		End If

        ' Open the output file for append
        sw = New System.IO.StreamWriter(New System.IO.FileStream(destinationPath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.Read))

		'Me.OnDetailedExportStart("Writing: " + proteinTable.TableName)

        counterMax = proteinTable.Rows.Count
		If counterMax <= 25 Then
			EventTriggerThresh = 1
		Else
			EventTriggerThresh = CInt(counterMax / 25)
		End If

        foundRows = proteinTable.Select("")

        For Each dr In foundRows
            tmpSeq = Me.m_ExportComponent.SequenceExtender(dr.Item("Sequence").ToString, proteinTable.Rows.Count)


            counter += 1

            If (counter Mod EventTriggerThresh) = 0 Then
				'Me.OnDetailedProgressUpdate("Processing: " + tmpName, Math.Round(CDbl(counter / counterMax), 3))
            End If

            proteinLength = tmpSeq.Length
            tmpDesc = cntrlFinder.Replace(dr.Item("Description").ToString, " ")
            tmpName = Me.m_ExportComponent.ReferenceExtender(dr.Item("Name").ToString)

            sw.WriteLine((">" & tmpName & " " & tmpDesc & tmpAltNames).Trim())

            For proteinPosition = 1 To proteinLength Step Me.m_seqLineLength
                seqLinePortion = Mid(tmpSeq, proteinPosition, Me.m_seqLineLength)
                sw.WriteLine(seqLinePortion)
            Next
        Next
        counter = 0
        sw.Flush()
        sw.Close()

        sw = Nothing
    End Function

    Function FinalizeFile(ByRef destinationPath As String) As String
        Dim fingerprint As String = Me.GetFileHash(destinationPath)

        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(destinationPath)

        Dim newDestinationPath As String
        newDestinationPath = System.IO.Path.Combine( _
                System.IO.Path.GetDirectoryName(destinationPath), _
                fingerprint + System.IO.Path.GetExtension(destinationPath))

        Dim targetFI As System.IO.FileInfo = New System.IO.FileInfo(newDestinationPath)

        If fi.Exists Then
            If targetFI.Exists Then
                targetFI.Delete()
            End If
            fi.MoveTo(newDestinationPath)
            destinationPath = newDestinationPath
        End If
        fi = Nothing

        Me.OnExportEnd()

        Return fingerprint
    End Function


End Class
