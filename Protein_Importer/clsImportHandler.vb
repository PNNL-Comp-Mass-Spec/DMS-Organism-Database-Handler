Option Strict On
Public Interface IImportProteins

    Function LoadProteins(ByVal filePath As String, _
        ByVal fileType As IImportProteins.ProteinImportFileTypes) As DataTable
    Function LoadProteinsForBatch( _
        ByVal filePath As String, _
        ByVal SelectedOrganismID As Integer, _
        ByVal SelectedAuthorityID As Integer) As Protein_Storage.IProteinStorage

    Function LoadOrganisms() As DataTable
    Function LoadAuthorities() As DataTable
    'Function LoadAuthorities(ByVal proteinCollectionID As Integer) As DataTable
    Function LoadAnnotationTypes() As DataTable
    Function LoadAnnotationTypes(ByVal proteinCollectionID As Integer) As DataTable

    Function LoadProteinCollections() As DataTable
    Function LoadProteinCollectionNames() As DataTable
    Sub ClearProteinCollection()
    Sub TriggerProteinCollectionsLoad()
    Sub TriggerProteinCollectionsLoad(ByVal organism_ID As Integer)
    Sub TriggerProteinCollectionTableUpdate()
    Function LoadCollectionMembersByID( _
        ByVal collectionID As Integer, _
        ByVal namingAuthorityID As Integer) As DataTable
    Function LoadCollectionMembersByName( _
        ByVal collectionName As String, _
        ByVal namingAuthorityID As Integer) As DataTable

    ReadOnly Property CollectionMembers() As Protein_Storage.IProteinStorage
    ReadOnly Property Authorities() As Hashtable

    Event LoadStart(ByVal taskTitle As String)
    Event LoadProgress(ByVal fractionDone As Double)
    'Event ValidationProgress(ByVal taskTitle As String, ByVal fractionDone As Double)
    Event LoadEnd()
    Event CollectionLoadComplete(ByVal CollectionsTable As DataTable)
    'Event InvalidFASTAFile(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList)

    Enum ProteinImportFileTypes
        FASTA
        Access
    End Enum

End Interface

Public Class clsImportHandler
    Implements IImportProteins

    Protected m_SQLAccess As TableManipulationBase.IGetSQLData
    Protected WithEvents m_Importer As IReadProteinImportFile

    Protected m_PISConnectionString As String
    'Protected m_ucProgress As ProgressBar.ucProgress
    Protected m_PersistentTaskNum As Integer

    Protected m_SPError As String

    Const ProteinCollectionsTable As String = "T_Protein_Collections"
    Const ProteinsTable As String = "T_Proteins"
    Const MembersTable As String = "T_Protein_Collection_Members"
    Const NamesTable As String = "T_Protein_Names"
    Const PositionTable As String = "T_Position_Info"
    Const CollectionProteinMap As String = "V_Protein_Collections_By_Organism"

    Protected m_FileContents As Protein_Storage.IProteinStorage
    Protected m_CollectionsList As DataTable

    Protected m_AuthoritiesList As Hashtable
    Protected m_AuthoritiesTable As DataTable

    Protected Event LoadStart(ByVal taskTitle As String) Implements IImportProteins.LoadStart
    Protected Event LoadProgress(ByVal fractionDone As Double) Implements IImportProteins.LoadProgress
    Protected Event LoadEnd() Implements IImportProteins.LoadEnd
    'Protected Event ValidationProgress(ByVal taskTitle As String, ByVal fractionDone As Double) Implements IImportProteins.ValidationProgress
    Protected Event CollectionLoadComplete(ByVal CollectionsTable As DataTable) Implements IImportProteins.CollectionLoadComplete
    'Protected Event InvalidFASTAFile(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList) Implements IImportProteins.InvalidFASTAFile


    Public Sub New(ByVal PISConnectionString As String)
        Me.m_SQLAccess = New TableManipulationBase.clsDBTask(PISConnectionString, True)
        Me.m_PISConnectionString = PISConnectionString
        Me.m_Importer = New FASTAReader
        Me.m_CollectionsList = Me.LoadProteinCollectionNames
    End Sub

    Protected ReadOnly Property CollectionMembers() As Protein_Storage.IProteinStorage Implements IImportProteins.CollectionMembers
        Get
            Return Me.m_FileContents
        End Get
    End Property

    Protected ReadOnly Property Authorities() As Hashtable Implements IImportProteins.Authorities
        Get
            Return Me.m_AuthoritiesList
        End Get
    End Property

    Protected Function GetCollectionNameFromID(ByVal ProteinCollectionID As Integer) As String
        Dim foundrows() As DataRow = Me.m_CollectionsList.Select("Protein_Collection_ID = " & CStr(ProteinCollectionID))
        Dim dr As DataRow = foundrows(0)
        Dim collectionName As String = CStr(dr.Item("FileName"))

        Return collectionName
    End Function

    Protected Function LoadFASTA(ByVal filePath As String) As Protein_Storage.IProteinStorage

        'check for existence of current file
        Dim UpdateMode As Boolean

        Dim fastaContents As Protein_Storage.IProteinStorage
        fastaContents = Me.m_Importer.GetProteinEntries(filePath)

        Dim strErrorMessage As String = Me.m_Importer.LastErrorMessage()

        If Not strErrorMessage Is Nothing AndAlso strErrorMessage.Length > 0 Then
            Dim intProteinsLoaded As Integer

            Try
                If Not fastaContents Is Nothing Then
                    intProteinsLoaded = fastaContents.ProteinCount
                End If
            Catch ex As Exception
                ' Ignore errors here
            End Try
            Windows.Forms.MessageBox.Show("GetProteinEntries returned an error after loading " & intProteinsLoaded.ToString & " proteins: " & strErrorMessage, "Error", Windows.Forms.MessageBoxButtons.OK, Windows.Forms.MessageBoxIcon.Error)

            fastaContents.ClearProteinEntries()
        End If

        Return fastaContents

    End Function

    Protected Function LoadOrganisms() As DataTable Implements IImportProteins.LoadOrganisms
        Dim orgSQL As String = "SELECT * FROM V_Organism_Picker ORDER BY Short_Name"
        Dim tmpOrgTable As DataTable = Me.m_SQLAccess.GetTable(orgSQL)

        Dim dr As DataRow = tmpOrgTable.NewRow

        With dr
            .Item("ID") = 0
            .Item("Short_Name") = "None"
            .Item("Display_Name") = " -- None Selected -- "
        End With

        tmpOrgTable.Rows.InsertAt(dr, 0)


        tmpOrgTable.AcceptChanges()

        Dim pk1(0) As DataColumn

        pk1(0) = tmpOrgTable.Columns("ID")
        tmpOrgTable.PrimaryKey = pk1


        Return tmpOrgTable

    End Function

    Protected Function LoadAnnotationTypes( _
        ByVal proteinCollectionID As Integer) As DataTable Implements IImportProteins.LoadAnnotationTypes

        Dim AnnTypeIDSQL As String
        AnnTypeIDSQL = _
            "SELECT Annotation_Type_ID " & _
            "FROM V_Protein_Collection_Authority " & _
            "WHERE Protein_Collection_ID = " & proteinCollectionID.ToString

        Dim tmpAnnTypeIDTable As DataTable
        tmpAnnTypeIDTable = Me.m_SQLAccess.GetTable(AnnTypeIDSQL)

        Dim dr As DataRow
        Dim authIDSB As New System.Text.StringBuilder
        For Each dr In tmpAnnTypeIDTable.Rows
            With authIDSB
                .Append(dr.Item("Annotation_Type_ID").ToString)
                .Append(", ")
            End With
        Next

        tmpAnnTypeIDTable = Nothing

        authIDSB.Remove(authIDSB.Length - 2, 2)

        Dim AuthSQL As String = _
            "SELECT * FROM V_Annotation_Type_Picker " & _
            "WHERE ID IN (" & authIDSB.ToString & ") " & _
            "ORDER BY Display_Name"

        Dim tmpAuthTable As DataTable = Me.m_SQLAccess.GetTable(AuthSQL)

        dr = tmpAuthTable.NewRow

        With dr
            .Item("ID") = 0
            .Item("Display_Name") = " -- None Selected -- "
            .Item("Details") = "None Selected"
        End With

        tmpAuthTable.Rows.InsertAt(dr, 0)

        tmpAuthTable.AcceptChanges()

        Dim pk1(0) As DataColumn

        pk1(0) = tmpAuthTable.Columns("ID")
        tmpAuthTable.PrimaryKey = pk1


        Return tmpAuthTable

    End Function

    Protected Function LoadAnnotationTypes() As DataTable Implements IImportProteins.LoadAnnotationTypes
        Dim AuthSQL As String = "SELECT * FROM V_Annotation_Type_Picker ORDER BY Display_Name"
        Dim tmpAnnTypeTable As DataTable = Me.m_SQLAccess.GetTable(AuthSQL)

        Dim dr As DataRow = tmpAnnTypeTable.NewRow

        With dr
            .Item("ID") = 0
            .Item("Display_Name") = " -- None Selected -- "
            '.Item("name") = " -- None Selected -- "
            .Item("Details") = "None Selected"
        End With

        tmpAnnTypeTable.Rows.InsertAt(dr, 0)

        tmpAnnTypeTable.AcceptChanges()
        Me.m_AuthoritiesList = Me.m_SQLAccess.DataTableToHashtable(tmpAnnTypeTable, "ID", "Display_Name")
        Me.m_AuthoritiesTable = tmpAnnTypeTable.Copy

        Return tmpAnnTypeTable
    End Function

    Protected Function LoadAuthorities() As DataTable Implements IImportProteins.LoadAuthorities
        Dim AuthSQL As String = "SELECT * FROM V_Authority_Picker ORDER BY Display_Name"
        Dim tmpAuthTable As DataTable = Me.m_SQLAccess.GetTable(AuthSQL)

        Dim dr As DataRow = tmpAuthTable.NewRow

        With dr
            .Item("ID") = 0
            .Item("Display_Name") = " -- None Selected -- "
            .Item("Details") = "None Selected"
        End With

        tmpAuthTable.Rows.InsertAt(dr, 0)

        tmpAuthTable.AcceptChanges()
        Me.m_AuthoritiesList = Me.m_SQLAccess.DataTableToHashtable(tmpAuthTable, "ID", "Display_Name")

        Return tmpAuthTable
    End Function


    Protected Sub ClearProteinCollection() Implements IImportProteins.ClearProteinCollection
        If Not Me.m_FileContents Is Nothing Then
            Me.m_FileContents.ClearProteinEntries()
        End If
    End Sub

    Protected Sub TriggerProteinCollectionsLoad() Implements IImportProteins.TriggerProteinCollectionsLoad
        Me.OnCollectionLoadComplete(Me.LoadProteinCollections)
    End Sub

    Protected Sub TriggerProteinCollectionsLoad(ByVal Organism_ID As Integer) Implements IImportProteins.TriggerProteinCollectionsLoad
        Me.OnCollectionLoadComplete(Me.LoadProteinCollections(Organism_ID))
    End Sub

    Protected Sub TriggerProteinCollectionTableUpdate() Implements IImportProteins.TriggerProteinCollectionTableUpdate
        'Dim errCode As Integer = Me.RunSP_UpdateProteinCollectionsByOrganism
        Me.OnCollectionLoadComplete(Me.LoadProteinCollections)
    End Sub

    Protected Function LoadProteinCollections() As DataTable Implements IImportProteins.LoadProteinCollections
        Dim PCSQL As String

        PCSQL = "SELECT MIN(FileName) AS FileName, Protein_Collection_ID, " & _
                    "MIN(Organism_ID) AS Organism_ID, MIN(Authority_ID) AS Authority_ID, " & _
                    "MIN(Display) AS Display, MIN(Authentication_Hash) AS Authentication_Hash " & _
                "FROM V_Protein_Collections_By_Organism " & _
                "GROUP BY Protein_Collection_ID " & _
                "ORDER BY MIN(FileName)"

        Dim tmpPCTable As DataTable = Me.m_SQLAccess.GetTable(PCSQL)

        Dim dr As DataRow = tmpPCTable.NewRow

        With dr
            .Item("Protein_Collection_ID") = 0
            .Item("Display") = " -- None Selected -- "
        End With

        tmpPCTable.Rows.InsertAt(dr, 0)
        tmpPCTable.AcceptChanges()

        Return tmpPCTable
    End Function

    Protected Function LoadProteinCollections(ByVal Organism_ID As Integer) As DataTable
        Dim PCSQL As String

        PCSQL = "SELECT FileName, Protein_Collection_ID, Organism_ID, Authority_ID, Display, Authentication_Hash" & _
                              " FROM V_Protein_Collections_By_Organism" & _
                              " WHERE Organism_ID = " & Organism_ID & _
                              " ORDER BY [FileName]"
        Dim tmpPCTable As DataTable = Me.m_SQLAccess.GetTable(PCSQL)

        Dim dr As DataRow = tmpPCTable.NewRow

        With dr
            .Item("Protein_Collection_ID") = 0
            .Item("Display") = " -- None Selected -- "
        End With

        tmpPCTable.Rows.InsertAt(dr, 0)
        tmpPCTable.AcceptChanges()

        Return tmpPCTable
    End Function

    Protected Function LoadProteinCollectionNames() As DataTable Implements IImportProteins.LoadProteinCollectionNames
        Dim PCSQL As String = _
            "SELECT Protein_Collection_ID, FileName, Authority_ID " & _
            "FROM V_Protein_Collections_By_Organism " & _
            "ORDER BY FileName"
        Dim tmpPCTable As DataTable = Me.m_SQLAccess.GetTable(PCSQL)

        Dim dr As DataRow = tmpPCTable.NewRow

        With dr
            .Item("Protein_Collection_ID") = 0
            .Item("FileName") = " -- None Selected -- "
        End With

        tmpPCTable.Rows.InsertAt(dr, 0)
        tmpPCTable.AcceptChanges()

        Return tmpPCTable
    End Function

    Protected Function LoadCollectionMembersByID( _
        ByVal collectionID As Integer, _
        ByVal authorityID As Integer) As DataTable Implements IImportProteins.LoadCollectionMembersByID

        Me.m_CollectionsList = Me.LoadProteinCollections

        If authorityID <= 0 Then
            Dim foundrows() As DataRow
            foundrows = Me.m_CollectionsList.Select("Protein_Collection_ID = " & collectionID)
            authorityID = CInt(foundrows(0).Item("Authority_ID"))

        End If

        Dim MemberSQL As String = _
            "SELECT * From V_Protein_Storage_Entry_Import " & _
            "WHERE [Protein_Collection_ID] = " & collectionID.ToString & " " & _
                "AND Annotation_Type_ID = " & authorityID.ToString & " " & _
                "ORDER BY [Name]"
        Return Me.LoadCollectionMembers(MemberSQL)
    End Function

    Protected Function LoadCollectionMembersByName( _
        ByVal collectionName As String, _
        ByVal authorityID As Integer) As DataTable Implements IImportProteins.LoadCollectionMembersByName

        Dim GetIDSQL As String = "SELECT Protein_Collection_ID, Primary_Annotation_Type_ID " & _
            "FROM T_Protein_Collections " & _
            "WHERE [FileName] = " & collectionName & " ORDER BY [Name]"

        Dim tmpTable As DataTable = Me.m_SQLAccess.GetTable(GetIDSQL)
        Dim foundRow As DataRow = tmpTable.Rows(0)
        Dim collectionID As Integer = DirectCast(foundRow.Item("Protein_Collection_ID"), Int32)
        'Dim authorityID As Integer = DirectCast(foundRow.Item("Primary_Authority_ID"), Int32)

        Return LoadCollectionMembersByID(collectionID, authorityID)

    End Function

    Private Function LoadCollectionMembers(ByVal SelectStatement As String) As DataTable
        Dim tmpMemberTable As DataTable = Me.m_SQLAccess.GetTable(SelectStatement)

        'Dim ProteinNames As New ArrayList

        Me.m_FileContents = Me.LoadProteinInfo(tmpMemberTable.Select(""))

        Return tmpMemberTable

    End Function

    'Protected Function LoadSelectedProteinInfo(ByRef CollectionMemberTable As DataTable, _
    '    ByVal SelectedProteinNames As ArrayList) As Protein_Storage.IProteinStorage

    '    Dim ReferenceList As String
    '    Dim Reference As String

    '    For Each Reference In SelectedProteinNames
    '        ReferenceList &= Reference.ToString & ", "
    '    Next

    '    ReferenceList = Left(ReferenceList, ReferenceList.Length - 2)

    '    Dim foundrows() As DataRow = CollectionMemberTable.Select("IN " & ReferenceList)

    '    Dim tmpPS As Protein_Storage.IProteinStorage = Me.LoadProteinInfo(foundrows)

    '    Return tmpPS

    'End Function

    'Protected Function LoadAllProteinInfo(ByRef CollectionMemberTable As DataTable) As Protein_Storage.IProteinStorage
    '    Dim tmpPS As Protein_Storage.IProteinStorage = Me.LoadProteinInfo(CollectionMemberTable.Select(""))

    '    Return tmpPS
    'End Function

    Protected Function LoadProteinInfo(ByRef CollectionMembers() As DataRow) As Protein_Storage.IProteinStorage
        Dim dr As DataRow

        Dim ce As Protein_Storage.IProteinStorageEntry
        Dim tmpPS As Protein_Storage.IProteinStorage
        tmpPS = New Protein_Storage.clsProteinStorageDMS("")
        Dim proteinCount As Integer
        Dim triggerCount As Integer
        Dim counter As Integer

        RaiseEvent LoadStart("Retrieving Protein Entries...")

        proteinCount = CollectionMembers.Length

        If proteinCount > 20 Then
            triggerCount = CInt(proteinCount / 20)
        Else
            triggerCount = 1
        End If


        For Each dr In CollectionMembers
            ce = New Protein_Storage.clsProteinStorageEntry( _
                dr.Item("Name").ToString, _
                dr.Item("Description").ToString, _
                dr.Item("Sequence").ToString, _
                DirectCast(dr.Item("Length"), Int32), _
                DirectCast(dr.Item("Monoisotopic_Mass"), Double), _
                DirectCast(dr.Item("Average_Mass"), Double), _
                dr.Item("Molecular_Formula").ToString, _
                dr.Item("SHA1_Hash").ToString, counter)

            If counter Mod triggerCount > 0 Then
                Me.Task_LoadProgress(CSng(counter / proteinCount))
            End If

            ce.Protein_ID = DirectCast(dr.Item("Protein_ID"), Int32)
            tmpPS.AddProtein(ce)
            counter += 1
        Next

        Return tmpPS

    End Function

    'Function to load fasta file contents with no checking against the existing database entries
    'used to load up the source collection listview
    Protected Function LoadProteinsRaw( _
        ByVal filePath As String, _
        ByVal fileType As IImportProteins.ProteinImportFileTypes) As DataTable Implements IImportProteins.LoadProteins

        Dim tmpProteinTable As DataTable = Me.m_SQLAccess.GetTableTemplate("V_Protein_Database_Export")
        Dim counter As Integer
        Dim triggerCount As Integer
        Dim proteinCount As Integer

        Select Case fileType
            Case IImportProteins.ProteinImportFileTypes.FASTA
                Me.m_FileContents = Me.LoadFASTA(filePath)
            Case Else
                Return Nothing
        End Select

        If Me.m_FileContents Is Nothing Then
            Return Nothing
        End If

        proteinCount = CInt(Me.m_FileContents.ProteinCount)
        If proteinCount > 20 Then
            triggerCount = CInt(proteinCount / 20)
        Else
            triggerCount = 1
        End If

        Dim contentsEnum As IDictionaryEnumerator = Me.m_FileContents.GetEnumerator
        Dim entry As Protein_Storage.IProteinStorageEntry
        Dim dr As DataRow

        'Move certain elements of the protein record to a datatable for display in the source window
        Me.Task_LoadStart("Updating Display List...")
        Do While contentsEnum.MoveNext = True
            entry = DirectCast(contentsEnum.Value, Protein_Storage.clsProteinStorageEntry)
            dr = tmpProteinTable.NewRow
            dr.Item("Name") = entry.Reference
            dr.Item("Description") = entry.Description
            dr.Item("Sequence") = entry.Sequence
            tmpProteinTable.Rows.Add(dr)
            If counter Mod triggerCount > 0 Then
                Me.Task_LoadProgress(CSng(counter / proteinCount))
            End If
            counter += 1
        Loop
        Me.Task_LoadEnd()

        Return tmpProteinTable

    End Function

    Protected Function LoadProteinsForBatch( _
        ByVal FullFilePath As String, _
        ByVal SelectedOrganismID As Integer, _
        ByVal SelectedAuthorityID As Integer) As Protein_Storage.IProteinStorage Implements IImportProteins.LoadProteinsForBatch

        Dim strErrorMessage As String

        Dim ps As Protein_Storage.IProteinStorage = Me.LoadFASTA(FullFilePath)

        Return ps
    End Function


#Region " Event Handlers "

    'handles loadstart event for fasta importer module
    Protected Sub Task_LoadStart(ByVal taskTitle As String) Handles m_Importer.LoadStart
        'Me.m_PersistentTaskNum += 1
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Protected Sub Task_LoadProgress(ByVal fractionDone As Double) Handles m_Importer.LoadProgress
        RaiseEvent LoadProgress(fractionDone)
    End Sub

    'Private Sub Task_LoadProgress(ByVal taskTitle As String, ByVal fractionDone As Double) Handles m_Importer.ValidationProgress
    '    RaiseEvent ValidationProgress(taskTitle, fractionDone)
    'End Sub

    Protected Sub Task_LoadEnd() Handles m_Importer.LoadEnd
        RaiseEvent LoadEnd()
    End Sub

    'Private Sub OnInvalidFASTAFile(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList) Handles m_Importer.InvalidFASTAFile
    '    RaiseEvent InvalidFASTAFile(FASTAFilePath, errorCollection)
    'End Sub

    Protected Sub OnCollectionLoadComplete(ByVal CollectionsList As DataTable)
        RaiseEvent CollectionLoadComplete(CollectionsList)
    End Sub
#End Region

#Region " Stored Procedure Access "
    Protected Function RunSP_UpdateProteinCollectionsByOrganism() As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionsByOrganism", _
            Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

#End Region

End Class
