Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsGetFASTAFromDMSForward
    'Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS

    Protected m_TableGrabber As TableManipulationBase.IGetSQLData
    Protected WithEvents m_fileDumper As Protein_Exporter.ExportProteinCollectionsIFC.IExportProteins
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


    Public Sub New( _
        ByVal ProteinStorageConnectionString As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        Me.m_TableGrabber = New TableManipulationBase.clsDBTask(ProteinStorageConnectionString, True)
        Me.m_AllCollections = Me.GetCollectionNameList()
        Me.m_OrganismList = Me.GetOrganismList

        Select Case DatabaseFormatType
            Case IGetFASTAFromDMS.DatabaseFormatTypes.FASTA
                Me.m_fileDumper = New clsExportProteinsFASTA(Me)
                Me.m_Extension = ".fasta"
            Case IGetFASTAFromDMS.DatabaseFormatTypes.FASTAPro
                Me.m_fileDumper = New clsExportProteinsXTFASTA(Me)
                Me.m_Extension = ".fasta.pro"
        End Select

    End Sub


    Event FileGenerationCompleted(ByVal FullOutputPath As String)
    Event FileGenerationProgress(ByVal statusMsg As String, ByVal fractionDone As Double)
    Event FileGenerationStarted(ByVal taskMsg As String)

    Property FullOutputPath() As String
        Get
            Return Me.m_CurrentFullOutputPath
        End Get
        Set(ByVal Value As String)
            Me.m_CurrentFullOutputPath = Value
        End Set
    End Property

    ReadOnly Property ArchivalName() As String
        Get
            Return Me.m_CurrentArchiveFileName
        End Get
    End Property

    Protected Overridable Function ExtendedExportPath( _
        ByVal ExportPath As String, _
        ByVal ProteinCollectionName As String) As String

        Return System.IO.Path.Combine(ExportPath, ProteinCollectionName + Me.m_Naming_Suffix + Me.m_Extension)

    End Function

    Overridable Function SequenceExtender(ByVal originalSequence As String, ByVal collectionCount As Integer) As String
        Return originalSequence
    End Function

    Overridable Function ReferenceExtender(ByVal originalReference As String) As String
        Return originalReference
    End Function

    Overridable Overloads Function ExportFASTAFile( _
       ByVal ProteinCollectionNameList As ArrayList, _
       ByVal ExportPath As String, _
       ByVal AlternateAuthorityID As Integer, _
       ByVal PadWithPrimaryAnnotation As Boolean) As String 'Implements IGetFASTAFromDMS.ExportFASTAFile

        Dim ProteinCollectionName As String
        Dim collectionSQL As String
        Dim collectionTable As DataTable
        Dim tmpCollectionTable As DataTable
        Dim alternateNames As Hashtable
        Dim PrimaryAnnotationID As Integer
        Dim ProteinCollectionID As Integer
        Dim appendMultiples As Boolean

        Dim authorizationSQL As String
        Dim authorizationTable As DataTable

        Dim passPhraseSQL As String
        Dim passPhraseTable As DataTable

        Dim encCheckRows() As DataRow
        Dim authCheckRows() As DataRow

        Dim trueName As String

        Dim tmpID As Integer
        Dim tmpIDListSB As New System.Text.StringBuilder
        Dim cipherSeq As String
        Dim clearSeq As String

        Dim decryptionRow As DataRow


        'Dim collection As Protein_Storage.IProteinStorage
        Dim collection As New DataSet
        appendMultiples = False
        Dim AddnXRefList As New ArrayList

        Dim nameCheckRegex As New System.Text.RegularExpressions.Regex("(?<collectionname>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))")

        Dim m As System.Text.RegularExpressions.Match

        Dim user As New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent)
        Dim user_ID As String = user.Identity.Name
        Dim collectionPassphrases As Hashtable

        'check each collectionname for encryption of contents
        Dim nameString As String
        For Each nameString In ProteinCollectionNameList
            encCheckRows = Me.m_CollectionsCache.Select("Filename = '" & nameString & "' AND Contents_Encrypted > 0")

            If encCheckRows.Length > 0 Then
                'Determine the encrypted collections to which this user has access
                authorizationSQL = "SELECT Protein_Collection_ID, Protein_Collection_Name " & _
                                    "FROM V_Encrypted_Collection_Authorizations " & _
                                    "WHERE Login_Name = '" & user_ID & "'"

                authorizationTable = Me.m_TableGrabber.GetTable(authorizationSQL)
                authCheckRows = authorizationTable.Select("Protein_Collection_Name = '" & nameString & "' OR Protein_Collection_Name = 'Administrator'")
                If authCheckRows.Length > 0 Then
                    tmpID = Me.FindIDByName(nameString)
                    passPhraseSQL = "SELECT Passphrase " & _
                                    "FROM T_Encrypted_Collection_Passphrases " & _
                                    "WHERE Protein_Collection_ID = " & tmpID.ToString
                    passPhraseTable = Me.m_TableGrabber.GetTable(passPhraseSQL)

                    If collectionPassphrases Is Nothing Then
                        collectionPassphrases = New Hashtable
                    End If
                    collectionPassphrases.Add(nameString, passPhraseTable.Rows(0).Item("Passphrase").ToString)
                Else
                    Throw New Exception("User " & user_ID & " does not have access to the encrypted collection '" & nameString & "'")
                    Me.OnExportComplete()
                    Exit Function
                End If
            End If

        Next


        For Each ProteinCollectionName In ProteinCollectionNameList
            If nameCheckRegex.IsMatch(ProteinCollectionName) Then
                m = nameCheckRegex.Match(ProteinCollectionName)
                trueName = m.Groups("collectionname").Value
            Else
                trueName = ProteinCollectionName
            End If

            If PadWithPrimaryAnnotation Then
                'Dim alternateNameXRefSQL As String = _
                '    "SELECT Protein_ID, Alternate_Name " & _
                '    "FROM V_Alternate_Name_Xref " & _
                '    "WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString & " AND " & _
                '    "Annotation_Type_ID <> " & PrimaryAnnotationID

                '"SELECT Primary_Name, Alternate_Name " & _
                '"FROM V_Alternate_Name_Xref " & _
                '"WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString
                '"WHERE (Protein_ID IN " & _
                '    "(SELECT Protein_ID " & _
                '    "FROM T_Protein_Collection_Members " & _
                '    "WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString & "))"

                'Dim alternateNameTable As DataTable = Me.m_TableGrabber.GetTable(alternateNameXRefSQL)

                'alternateNames = Me.m_TableGrabber.DataTableToComplexHashtable( _
                '    alternateNameTable, "Protein_ID", "Alternate_Name")

                'alternateNameTable = Nothing

                tmpID = Me.FindIDByName(trueName)

                'collectionSQL = _
                '    "SELECT Name, Description, Sequence, Protein_ID " & _
                '    "FROM V_Protein_Database_Export " & _
                '    "WHERE Protein_Collection_ID = " & tmpID.ToString & " " & _
                '        "AND Primary_Annotation_Type_ID = Annotation_Type_ID " & _
                '    "ORDER BY Name"
                collectionSQL = _
                    "SELECT Name, Description, Sequence, Protein_ID " & _
                    "FROM V_Protein_Database_Export " & _
                    "WHERE Protein_Collection_ID = " & tmpID.ToString ' & " " & _
                '"ORDER BY Name"


            Else
                collectionSQL = _
                    "SELECT Name, Description, Sequence, Protein_ID " & _
                    "FROM V_Protein_Database_Export " & _
                    "WHERE Protein_Collection_ID = " & tmpID.ToString & ") " & _
                        "AND Annotation_Type_ID = " & AlternateAuthorityID ' & _
                '"ORDER BY Name"
                alternateNames = New Hashtable

            End If

            'Me.OnExportStart("Retrieving: " + trueName)



            collectionTable = Me.m_TableGrabber.GetTable(collectionSQL)

            If Not collectionPassphrases Is Nothing Then
                If collectionPassphrases.ContainsKey(trueName) Then
                    '    If collectionPassphrases.ContainsKey(trueName) Then

                    Me.m_RijndaelDecryption = New clsRijndaelEncryptionHandler(collectionPassphrases.Item(trueName).ToString)
                    For Each decryptionRow In collectionTable.Rows
                        cipherSeq = decryptionRow.Item("Sequence").ToString
                        clearSeq = Me.m_RijndaelDecryption.Decrypt(cipherSeq)
                        decryptionRow.Item("Sequence") = clearSeq
                        decryptionRow.AcceptChanges()
                    Next
                End If
            End If

            collectionTable.TableName = trueName

            Me.m_CurrentFileProteinCount = collectionTable.Rows.Count

            collection.Tables.Add(collectionTable)

            'collection = Me.DataTableToCollection( _
            '            collectionTable, "Name", "Description", "Sequence", "Protein_ID", _
            '            Me.ExtendedExportPath(ExportPath, ProteinCollectionName), appendMultiples, _
            '            alternateNames)

            If appendMultiples Then
                AddnXRefList.Add(tmpID)
            End If

            appendMultiples = True

            tmpIDListSB.Append(Format(tmpID, "000000"))
            tmpIDListSB.Append("+")

        Next
        tmpIDListSB.Remove(tmpIDListSB.Length - 1, 1)
        Dim name As String

        If ProteinCollectionNameList.Count > 1 Then
            name = tmpIDListSB.ToString
        Else
            name = trueName
        End If

        Me.m_CurrentFullOutputPath = Me.ExtendedExportPath(ExportPath, name)
        Me.m_CurrentArchiveFileName = tmpIDListSB.ToString

        Dim fingerprint As String = Me.m_fileDumper.Export(collection, Me.m_CurrentFullOutputPath)


        Me.OnExportComplete()

        Return fingerprint


    End Function

    Overridable Overloads Function ExportFASTAFile( _
       ByVal ProteinCollectionNameList As ArrayList, _
       ByVal ExportPath As String) As String 'Implements IGetFASTAFromDMS.ExportFASTAFile

        Dim primaryAuthorityID As Integer = 1

        Return Me.ExportFASTAFile(ProteinCollectionNameList, _
            ExportPath, primaryAuthorityID, True)


    End Function

    Protected Function GetPrimaryAuthorityID(ByVal proteinCollectionID As Integer) As Integer
        Dim dr As DataRow
        Dim foundrows() As DataRow

        foundrows = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & proteinCollectionID.ToString)

        Return CInt(foundrows(0).Item("Primary_Annotation_Type_ID"))
    End Function

    Protected Function GetPrimaryAuthorityID(ByVal proteinCollectionName As String) As Integer
        Dim proteinCollectionID As Integer = Me.FindIDByName(proteinCollectionName)
        Return Me.GetPrimaryAuthorityID(proteinCollectionID)
    End Function

    Function GetCollectionNameList() As System.Collections.Hashtable 'Implements IGetFASTAFromDMS.GetAllCollections
        If Me.m_CollectionsCache Is Nothing Then
            Me.RefreshCollectionCache()
        End If

        Return Me.m_TableGrabber.DataTableToHashtable(Me.m_CollectionsCache, "Protein_Collection_ID", "FileName")
    End Function

    Function GetCollectionsByOrganism(ByVal OrganismID As Integer) As System.Collections.Hashtable
        If Me.m_CollectionsCache Is Nothing Then
            Me.RefreshCollectionCache()
        End If

        Return Me.m_TableGrabber.DataTableToHashtable(Me.m_CollectionsCache, "Protein_Collection_ID", "FileName", "[Organism_ID] = " & OrganismID.ToString)
    End Function

    Function GetCollectionsByOrganismTable(ByVal OrganismID As Integer) As DataTable
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
        Me.m_CollectionsCache = Me.m_TableGrabber.GetTable( _
            "SELECT * FROM V_Protein_Collections_By_Organism ORDER BY Protein_Collection_ID")
    End Sub

    Protected Sub RefreshOrganismCache()
        Me.m_OrganismCache = Me.m_TableGrabber.GetTable( _
            "SELECT ID as Organism_ID, Short_Name as Name FROM V_Organism_Picker ORDER BY Organism_ID")
    End Sub

    'Protected Function DataTableToCollection( _
    '    ByRef ProteinTable As DataTable, _
    '    ByVal NameField As String, _
    '    ByVal DescField As String, _
    '    ByVal SeqField As String, _
    '    ByVal ProteinIDField As String, _
    '    ByVal ExportFullPath As String, _
    '    ByVal Append As Boolean, _
    '    Optional ByVal AlternateNames As Hashtable = Nothing) As Protein_Storage.IProteinStorage

    '    Dim errorCode As Integer = 0

    '    'Try
    '    'Dim pc As Protein_Storage.IProteinStorage

    '    If Me.m_PSC Is Nothing Or Append = False Then
    '        Me.m_PSC = New Protein_Storage.clsProteinStorageDMS(ExportFullPath)
    '    End If
    '    Dim ce As Protein_Storage.IProteinStorageEntry

    '    If AlternateNames Is Nothing Then
    '        AlternateNames = New Hashtable
    '    End If

    '    Dim collectionCount As Integer = ProteinTable.Rows.Count


    '    Dim dr As DataRow
    '    Dim tmpSeq As String
    '    Dim tmpName As String
    '    Dim tmpDesc As String
    '    Dim tmpID As Integer
    '    Dim tmpNameList As ArrayList

    '    tmpNameList = New ArrayList

    '    Me.OnExportStart("Transferring entries to storage class")

    '    For Each dr In ProteinTable.Rows
    '        tmpName = Me.ReferenceExtender(dr.Item(NameField).ToString)
    '        tmpID = CInt(dr.Item(ProteinIDField))
    '        If AlternateNames.Contains(tmpID.ToString) Then
    '            tmpNameList = DirectCast(AlternateNames(tmpID.ToString), ArrayList) 'AlternateNames.Item(tmpName).ToString
    '        Else
    '            'tmpNameList = New ArrayList
    '            'tmpNameList.Add(tmpName)
    '        End If
    '        tmpDesc = dr.Item(DescField).ToString
    '        tmpSeq = dr.Item(SeqField).ToString

    '        ce = New Protein_Storage.clsProteinStorageEntry( _
    '            tmpName, tmpDesc, Me.SequenceExtender(tmpSeq, collectionCount), tmpID)

    '        For Each tmpName In tmpNameList
    '            ce.AddXRef(tmpName)
    '        Next
    '        If tmpNameList.Count > 0 Then
    '            tmpNameList.Clear()
    '        End If
    '        Me.m_PSC.AddProtein(ce)

    '    Next

    '    'Me.OnExportComplete()

    '    Return Me.m_PSC
    '    'Catch
    '    '    errorCode = Err.Number
    '    'End Try

    '    'Return Me.GetFileHash(ExportFullPath)

    'End Function

    Function FindIDByName(ByVal CollectionName As String) As Integer
        'Dim dr As DataRow
        Dim foundRows() As DataRow
        foundRows = Me.m_CollectionsCache.Select("[FileName] = '" & CollectionName & "'")
        If foundRows.Length = 0 Then
            Me.RefreshCollectionCache()
            foundRows = Me.m_CollectionsCache.Select("[FileName] = '" & CollectionName & "'")
        End If
        Return CInt(foundRows(0).Item("Protein_Collection_ID"))
    End Function

    Function FindNameByID(ByVal CollectionID As Integer) As String
        Dim foundrows() As DataRow
        foundrows = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & CollectionID.ToString)
        If foundrows.Length = 0 Then
            Me.RefreshCollectionCache()
            foundrows = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & CollectionID.ToString)
        End If
        Return foundrows(0).Item("FileName").ToString
    End Function

    Protected Function FindPrimaryAnnotationID(ByVal collectionID As Integer) As Integer
        Dim foundRows() As DataRow = Me.m_CollectionsCache.Select("Protein_Collection_ID = " & collectionID.ToString)
        Return CInt(foundRows(0).Item("Primary_Annotation_Type_ID"))
    End Function

    Function GetFileHash(ByVal FullFilePath As String) As String

        Return Me.m_fileDumper.GenerateFileAuthenticationHash(FullFilePath)

    End Function

    Function GetStoredHash(ByVal ProteinCollectionName As String) As String
        Dim foundRows() As DataRow = Me.m_CollectionsCache.Select("[FileName] = '" & ProteinCollectionName & "'")
        Return CStr(foundRows(0).Item("Authentication_Hash"))
    End Function

    Function GetStoredHash(ByVal ProteinCollectionID As Integer) As String
        Dim ProteinCollectionName As String = CStr(Me.m_AllCollections.Item(ProteinCollectionID))
        Return Me.GetStoredHash(ProteinCollectionName)
    End Function

    Protected Sub OnExportStart(ByVal taskMsg As String) Handles m_fileDumper.ExportStart
        RaiseEvent FileGenerationStarted(taskMsg)
    End Sub

    Protected Sub OnExportProgressUpdate(ByVal statusMsg As String, ByVal fractionDone As Double) Handles m_fileDumper.ExportProgress
        RaiseEvent FileGenerationProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportComplete()
        RaiseEvent FileGenerationCompleted(Me.FullOutputPath)
    End Sub

End Class
