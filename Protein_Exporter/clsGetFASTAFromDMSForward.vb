Option Strict On

Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsGetFASTAFromDMSForward

	Protected m_TableGrabber As TableManipulationBase.IGetSQLData
    Protected WithEvents m_fileDumper As IExportProteins
	Protected m_AllCollections As Hashtable
	Protected m_OrganismList As Hashtable

	Protected m_CurrentFullOutputPath As String
	Friend m_CurrentFileProteinCount As Integer
	Protected m_CurrentArchiveFileName As String

	Protected m_PSC As Protein_Storage.IProteinStorage
	Protected m_PSEntry As Protein_Storage.IProteinStorageEntry


	Protected m_CollectionsCache As DataTable
	Protected m_OrganismCache As DataTable
	Protected m_CollectionCountsCache As DataTable

	Protected m_Naming_Suffix As String = "_forward"
	Protected m_Extension As String = ""

	Protected m_RijndaelDecryption As clsRijndaelEncryptionHandler
	'Protected m_SHA1Provider As System.Security.Cryptography.SHA1Managed


    Public Sub New(
     ProteinStorageConnectionString As String,
     DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        'Me.m_SHA1Provider = New System.Security.Cryptography.SHA1Managed
        Me.m_TableGrabber = New TableManipulationBase.clsDBTask(ProteinStorageConnectionString, True)
        Me.m_AllCollections = Me.GetCollectionNameList()
        Me.m_OrganismList = Me.GetOrganismList()

        Select Case DatabaseFormatType
            Case IGetFASTAFromDMS.DatabaseFormatTypes.fasta
                Me.m_fileDumper = New clsExportProteinsFASTA(Me)
                Me.m_Extension = ".fasta"
            Case IGetFASTAFromDMS.DatabaseFormatTypes.fastapro
                Me.m_fileDumper = New clsExportProteinsXTFASTA(Me)
                Me.m_Extension = ".fasta.pro"
        End Select

    End Sub


    Event FileGenerationCompleted(FullOutputPath As String)
    Event FileGenerationProgress(statusMsg As String, fractionDone As Double)
    Event FileGenerationStarted(taskMsg As String)

    Property FullOutputPath() As String
        Get
            Return Me.m_CurrentFullOutputPath
        End Get
        Set(Value As String)
            Me.m_CurrentFullOutputPath = Value
        End Set
    End Property

    ReadOnly Property ArchivalName() As String
        Get
            Return Me.m_CurrentArchiveFileName
        End Get
    End Property

    Protected Overridable Function ExtendedExportPath(
     ExportPath As String,
     ProteinCollectionName As String) As String

        Return System.IO.Path.Combine(ExportPath, ProteinCollectionName + Me.m_Naming_Suffix + Me.m_Extension)

    End Function

    Overridable Function SequenceExtender(originalSequence As String, collectionCount As Integer) As String
        Return originalSequence
    End Function

    Overridable Function ReferenceExtender(originalReference As String) As String
        Return originalReference
    End Function

    Overridable Overloads Function ExportFASTAFile(
       ProteinCollectionNameList As ArrayList,
       ExportPath As String,
       AlternateAuthorityID As Integer,
       PadWithPrimaryAnnotation As Boolean) As String   'Implements IGetFASTAFromDMS.ExportFASTAFile

        Dim ProteinCollectionName As String
        Dim collectionSQL As String
        Dim collectionTable As DataTable

        Dim alternateNames As Hashtable

        Dim authorizationSQL As String
        Dim authorizationTable As DataTable

        Dim passPhraseSQL As String
        Dim passPhraseTable As DataTable

        Dim encCheckRows() As DataRow
        Dim authCheckRows() As DataRow

        Dim trueName As String = String.Empty

        Dim tmpID As Integer
        Dim tmpIDListSB As New System.Text.StringBuilder
        Dim cipherSeq As String
        Dim clearSeq As String

        Dim decryptionRow As DataRow

        Dim nameCheckRegex As New System.Text.RegularExpressions.Regex("(?<collectionname>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))")

        If Not CheckProteinCollectionNameValidity(ProteinCollectionNameList) Then
            Return ""
        End If

        Dim m As System.Text.RegularExpressions.Match

        Dim user As New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent)
        Dim user_ID As String = user.Identity.Name
        Dim collectionPassphrases As Hashtable = Nothing

        Dim collectionNameList As String = String.Empty

        ' Check each collectionname for encryption of contents
        For Each nameString As String In ProteinCollectionNameList
            encCheckRows = Me.m_CollectionsCache.Select("Filename = '" & nameString & "' AND Contents_Encrypted > 0")

            If encCheckRows.Length > 0 Then
                'Determine the encrypted collections to which this user has access
                authorizationSQL = "SELECT Protein_Collection_ID, Protein_Collection_Name " &
                  "FROM V_Encrypted_Collection_Authorizations " &
                  "WHERE Login_Name = '" & user_ID & "'"

                authorizationTable = Me.m_TableGrabber.GetTable(authorizationSQL)
                authCheckRows = authorizationTable.Select("Protein_Collection_Name = '" & nameString & "' OR Protein_Collection_Name = 'Administrator'")
                If authCheckRows.Length > 0 Then
                    tmpID = Me.FindIDByName(nameString)
                    passPhraseSQL = "SELECT Passphrase " &
                     "FROM T_Encrypted_Collection_Passphrases " &
                     "WHERE Protein_Collection_ID = " & tmpID.ToString
                    passPhraseTable = Me.m_TableGrabber.GetTable(passPhraseSQL)

                    If collectionPassphrases Is Nothing Then
                        collectionPassphrases = New Hashtable
                    End If
                    collectionPassphrases.Add(nameString, passPhraseTable.Rows(0).Item("Passphrase").ToString)
                Else
                    Throw New Exception("User " & user_ID & " does not have access to the encrypted collection '" & nameString & "'")
                End If
            End If

            If collectionNameList.Length > 0 Then collectionNameList &= ", "
            collectionNameList &= nameString
        Next

        Dim lengthCheckSQL As String
        Dim lengthCheckTable As DataTable
        Dim collectionLength As Integer
        Dim currentCollectionPos As Integer
        Dim foundrow As DataRow
        Dim sectionStart As Integer
        Dim sectionEnd As Integer
        Dim tableName As String

        Dim tmpOutputPath As String
        Dim tmpOutputPathCandidate As String

        Dim currentCollectionCount As Integer
        Dim proteinCollectionsExported As Integer

        ' Get a temp file name; however, create the file in the target folder path 
        ' (in case the system-defined temp directory is on a different drive than the target folder)

        Dim fiOutputPathCheck As System.IO.FileInfo
        Do
            tmpOutputPathCandidate = System.IO.Path.GetTempFileName
            Try
                ' The GetTempFileName function created a temp file that we don't need; delete it now (but use try/catch just in case the deletion fails for some freak reason)
                System.IO.File.Delete(tmpOutputPathCandidate)
            Catch ex As Exception
            End Try

            tmpOutputPath = System.IO.Path.Combine(ExportPath, System.IO.Path.GetFileName(tmpOutputPathCandidate))
            fiOutputPathCheck = New System.IO.FileInfo(tmpOutputPath)

        Loop While fiOutputPathCheck.Exists

        If ProteinCollectionNameList.Count = 1 Then
            OnExportStart("Exporting protein collection " & collectionNameList)
        Else
            OnExportStart("Exporting " & ProteinCollectionNameList.Count & "protein collections: " & collectionNameList)
        End If

        proteinCollectionsExported = 0
        For Each ProteinCollectionName In ProteinCollectionNameList
            currentCollectionPos = 0
            currentCollectionCount = 0
            sectionStart = currentCollectionPos
            sectionEnd = 0

            ' Make sure there are no leading or trailing spaces
            ProteinCollectionName = ProteinCollectionName.Trim()

            If nameCheckRegex.IsMatch(ProteinCollectionName) Then
                m = nameCheckRegex.Match(ProteinCollectionName)
                trueName = m.Groups("collectionname").Value
            Else
                trueName = ProteinCollectionName
            End If

            ' Lookup the number of proteins that should be in this protein collection
            lengthCheckSQL = "SELECT NumProteins FROM T_Protein_Collections " &
              "WHERE FileName = '" & ProteinCollectionName & "'"

            lengthCheckTable = Me.m_TableGrabber.GetTable(lengthCheckSQL)

            If lengthCheckTable.Rows.Count > 0 Then
                foundrow = lengthCheckTable.Rows(0)
                collectionLength = CType(foundrow.Item(0), Int32)
            Else
                collectionLength = -1
            End If

            Do
                sectionStart = currentCollectionPos
                sectionEnd = sectionStart + 10000

                If PadWithPrimaryAnnotation Then
                    tmpID = Me.FindIDByName(trueName)
                    collectionSQL =
                     "SELECT Name, Description, Sequence, Protein_ID " &
                     "FROM V_Protein_Database_Export " &
                     "WHERE " &
                      "Protein_Collection_ID = " & tmpID.ToString & " " &
                      "AND Sorting_Index BETWEEN " & sectionStart.ToString & " AND " & sectionEnd.ToString & " " &
                    "ORDER BY Sorting_Index"

                Else
                    collectionSQL =
                     "SELECT Name, Description, Sequence, Protein_ID " &
                     "FROM V_Protein_Database_Export " &
                     "WHERE Protein_Collection_ID = " & tmpID.ToString & ") " &
                      "AND Annotation_Type_ID = " & AlternateAuthorityID & " " &
                      "AND Sorting_Index BETWEEN " & sectionStart.ToString & " AND " & sectionEnd.ToString & " " &
                    "ORDER BY Sorting_Index"
                    alternateNames = New Hashtable

                End If


                collectionTable = Me.m_TableGrabber.GetTable(collectionSQL)

                If Not collectionPassphrases Is Nothing Then
                    If collectionPassphrases.ContainsKey(trueName) Then

                        Me.m_RijndaelDecryption = New clsRijndaelEncryptionHandler(collectionPassphrases.Item(trueName).ToString)
                        For Each decryptionRow In collectionTable.Rows
                            cipherSeq = decryptionRow.Item("Sequence").ToString
                            clearSeq = Me.m_RijndaelDecryption.Decrypt(cipherSeq)
                            decryptionRow.Item("Sequence") = clearSeq
                            decryptionRow.AcceptChanges()
                        Next
                    End If
                End If

                If collectionLength < 10000 Then
                    tableName = trueName
                Else
                    tableName = trueName + "_" + Format(sectionStart, "0000000000") + "-" + Format(sectionEnd, "0000000000")
                End If

                collectionTable.TableName = tableName

                Me.m_CurrentFileProteinCount = collectionTable.Rows.Count

                'collection.Tables.Add(collectionTable)
                Me.m_fileDumper.Export(collectionTable, tmpOutputPath)


                currentCollectionPos = sectionEnd + 1
                currentCollectionCount += collectionTable.Rows.Count

                Dim fractionDoneOverall As Double = 0
                If collectionLength > 0 Then
                    fractionDoneOverall = (proteinCollectionsExported / ProteinCollectionNameList.Count) + (currentCollectionCount / collectionLength) / ProteinCollectionNameList.Count
                End If

                OnExportProgressUpdate(currentCollectionCount & " entries exported, collection " & (proteinCollectionsExported + 1) & " of " & (ProteinCollectionNameList.Count), fractionDoneOverall)

            Loop Until collectionTable.Rows.Count = 0

            tmpIDListSB.Append(Format(tmpID, "000000"))
            tmpIDListSB.Append("+")
            If currentCollectionCount <> collectionLength Then
                Throw New Exception("The number of proteins exported for collection '" + ProteinCollectionName &
                 "' does not match the exected value:" &
                 currentCollectionCount & " exported from T_Protein_Collection_Members vs. " & collectionLength & " listed in T_Protein_Collections")
            End If

            proteinCollectionsExported += 1
        Next
        OnExportComplete(tmpOutputPath)

        Dim tmpFI = New System.IO.FileInfo(tmpOutputPath)

        tmpIDListSB.Remove(tmpIDListSB.Length - 1, 1)
        Dim name As String '= hash

        If ProteinCollectionNameList.Count > 1 Then
            name = tmpIDListSB.ToString
            If ExportPath.Length + name.Length > 225 Then
                ' If exporting a large number of protein collections, name can be very long
                ' This can lead to error: The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters
                ' Thus, truncate name
                Dim intMaxNameLength As Integer
                intMaxNameLength = 225 - ExportPath.Length
                If intMaxNameLength < 30 Then intMaxNameLength = 30

                name = name.Substring(0, intMaxNameLength)

                ' Find the last plus sign and truncate just before it
                Dim intLastPlusLocation As Integer
                intLastPlusLocation = name.LastIndexOf("+"c)
                If intLastPlusLocation > 30 Then
                    name = name.Substring(0, intLastPlusLocation)
                End If

            End If
        Else
            name = trueName
        End If

        Me.m_CurrentFullOutputPath = Me.ExtendedExportPath(ExportPath, name)
        Me.m_CurrentArchiveFileName = name

        ' Rename (move) the temporary file to the final, full name
        If System.IO.File.Exists(Me.m_CurrentFullOutputPath) Then
            System.IO.File.Delete(Me.m_CurrentFullOutputPath)
        End If
        tmpFI.MoveTo(Me.m_CurrentFullOutputPath)

        ' Assuming the final file now exists, delete the temporary file (if present)
        Dim outputfi = New System.IO.FileInfo(Me.m_CurrentFullOutputPath)
        If outputfi.Exists Then
            tmpFI = New System.IO.FileInfo(tmpOutputPath)
            If tmpFI.Exists Then
                tmpFI.Delete()
            End If
        End If

        ' Determine the SHA-hash of the output file
        ' This process will also rename the file, e.g. from "C:\Temp\SAR116_RBH_AA_012809_forward.fasta" to "C:\Temp\38FFACAC.fasta"
        Dim SHA1 As String
        SHA1 = Me.m_fileDumper.Export(New DataTable, Me.m_CurrentFullOutputPath)

        Me.OnExportComplete(m_CurrentFullOutputPath)

        Return SHA1


    End Function

    Overridable Overloads Function ExportFASTAFile(
       ProteinCollectionNameList As ArrayList,
       ExportPath As String) As String 'Implements IGetFASTAFromDMS.ExportFASTAFile

        Dim primaryAuthorityID = 1

        Return Me.ExportFASTAFile(ProteinCollectionNameList,
         ExportPath, primaryAuthorityID, True)


    End Function

    Protected Function CheckProteinCollectionNameValidity(proteinCollectionNameList As ArrayList) As Boolean
        Dim name As String
        Dim id As Integer
        For Each name In proteinCollectionNameList
            id = Me.FindIDByName(name)
            If id < 1 Then
                Throw New Exception("The collection named '" + name + "' does not exist in the system")
                Return False
            End If
        Next
        Return True
    End Function

    Protected Function GetPrimaryAuthorityID(proteinCollectionID As Integer) As Integer
        Dim foundrows() As DataRow

        foundrows = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & proteinCollectionID.ToString)

        Return CInt(foundrows(0).Item("Primary_Annotation_Type_ID"))
    End Function

    Protected Function GetPrimaryAuthorityID(proteinCollectionName As String) As Integer
        Dim proteinCollectionID As Integer = Me.FindIDByName(proteinCollectionName)
        Return Me.GetPrimaryAuthorityID(proteinCollectionID)
    End Function

    Function GetCollectionNameList() As System.Collections.Hashtable 'Implements IGetFASTAFromDMS.GetAllCollections
        If Me.m_CollectionsCache Is Nothing Then
            Me.RefreshCollectionCache()
        End If

        Return Me.m_TableGrabber.DataTableToHashtable(Me.m_CollectionsCache, "Protein_Collection_ID", "FileName")
    End Function

    Function GetCollectionsByOrganism(OrganismID As Integer) As System.Collections.Hashtable
        If Me.m_CollectionsCache Is Nothing Then
            Me.RefreshCollectionCache()
        End If

        Return Me.m_TableGrabber.DataTableToHashtable(Me.m_CollectionsCache, "Protein_Collection_ID", "FileName", "[Organism_ID] = " & OrganismID.ToString)
    End Function

    Function GetCollectionsByOrganismTable(OrganismID As Integer) As DataTable
        Dim tmpTable As DataTable = Me.m_CollectionsCache.Clone

        Dim dr As DataRow
        Dim foundRows() As DataRow = Me.m_CollectionsCache.Select("[Organism_ID] = " & OrganismID.ToString)

        For Each dr In foundRows
            tmpTable.ImportRow(dr)
        Next

        Return tmpTable
    End Function

    Function GetOrganismList() As System.Collections.Hashtable
        If Me.m_OrganismCache Is Nothing Then
            Me.RefreshOrganismCache()
        End If

        Return Me.m_TableGrabber.DataTableToHashtable(Me.m_OrganismCache, "Organism_ID", "Name")
    End Function

    Function GetOrganismListTable() As DataTable
        If Me.m_OrganismCache Is Nothing Then
            Me.RefreshOrganismCache()
        End If

        Return Me.m_OrganismCache
    End Function

    Protected Sub RefreshCollectionCache()
        Me.m_CollectionsCache = Me.m_TableGrabber.GetTable(
         "SELECT * FROM V_Protein_Collections_By_Organism ORDER BY Protein_Collection_ID")
    End Sub

    Protected Sub RefreshOrganismCache()
        Me.m_OrganismCache = Me.m_TableGrabber.GetTable(
         "SELECT ID as Organism_ID, Short_Name as Name FROM V_Organism_Picker ORDER BY Organism_ID")
    End Sub

    Function FindIDByName(CollectionName As String) As Integer
        If CollectionName.Length = 0 Then
            Return 0
        End If
        'Dim dr As DataRow
        Dim foundRows() As DataRow

        ' Make sure there are no leading or trailing spaces
        CollectionName = CollectionName.Trim()
        foundRows = Me.m_CollectionsCache.Select("[FileName] = '" & CollectionName & "'")
        If foundRows.Length = 0 Then
            Me.RefreshCollectionCache()
            foundRows = Me.m_CollectionsCache.Select("[FileName] = '" & CollectionName & "'")
        End If
        Dim id As Integer
        Try
            id = CInt(foundRows(0).Item("Protein_Collection_ID"))
        Catch ex As Exception
            id = -1
        End Try
        Return id
    End Function

    Function FindNameByID(CollectionID As Integer) As String
        Dim foundrows() As DataRow
        foundrows = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & CollectionID.ToString)
        If foundrows.Length = 0 Then
            Me.RefreshCollectionCache()
            foundrows = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & CollectionID.ToString)
        End If
        Return foundrows(0).Item("FileName").ToString
    End Function

    Protected Function FindPrimaryAnnotationID(collectionID As Integer) As Integer
        Dim foundRows() As DataRow = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & collectionID.ToString)
        Return CInt(foundRows(0).Item("Primary_Annotation_Type_ID"))
    End Function

    Function GetFileHash(FullFilePath As String) As String

        Return Me.m_fileDumper.GenerateFileAuthenticationHash(FullFilePath)

    End Function

    Function GetStoredHash(ProteinCollectionName As String) As String
        Dim foundRows() As DataRow = Me.m_CollectionsCache.Select("[FileName] = '" & ProteinCollectionName & "'")
        Return CStr(foundRows(0).Item("Authentication_Hash"))
    End Function

    Function GetStoredHash(ProteinCollectionID As Integer) As String
        Dim ProteinCollectionName = CStr(Me.m_AllCollections.Item(ProteinCollectionID))
        Return Me.GetStoredHash(ProteinCollectionName)
    End Function

    'Protected Function GenerateHash(SourceText As String) As String
    '    'Create an encoding object to ensure the encoding standard for the source text
    '    Dim Ue As New System.Text.ASCIIEncoding
    '    'Retrieve a byte array based on the source text
    '    Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)
    '    'Compute the hash value from the source
    '    Dim SHA1_hash() As Byte = Me.m_SHA1Provider.ComputeHash(ByteSourceText)
    '    'And convert it to String format for return
    '    Dim SHA1string As String = Convert.ToBase64String(SHA1_hash)

    '    Return SHA1string
    'End Function

    Protected Sub OnExportStart(taskMsg As String) Handles m_fileDumper.ExportStart
        RaiseEvent FileGenerationStarted(taskMsg)
    End Sub

    Protected Sub OnExportProgressUpdate(statusMsg As String, fractionDone As Double) Handles m_fileDumper.ExportProgress
        RaiseEvent FileGenerationProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportComplete(outputFilePath As String)
        RaiseEvent FileGenerationCompleted(outputFilePath)
    End Sub

End Class
