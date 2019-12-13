Option Strict On

Imports System.Collections.Generic
Imports System.Windows.Forms
Imports Protein_Storage

Public Class clsImportHandler

    Public Enum ProteinImportFileTypes
        FASTA
        Access
    End Enum

    Protected m_SQLAccess As TableManipulationBase.clsDBTask
    Protected WithEvents m_Importer As FASTAReader

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

    Protected m_FileContents As clsProteinStorage
    Protected m_CollectionsList As DataTable

    Protected m_AuthoritiesList As Dictionary(Of String, String)
    Protected m_AuthoritiesTable As DataTable

    Public Event LoadStart(taskTitle As String)
    Public Event LoadProgress(fractionDone As Double)
    Public Event LoadEnd()
    Public Event CollectionLoadComplete(CollectionsTable As DataTable)


    Public Sub New(PISConnectionString As String)
        m_SQLAccess = New TableManipulationBase.clsDBTask(PISConnectionString, True)
        m_PISConnectionString = PISConnectionString
        m_Importer = New FASTAReader
        m_CollectionsList = LoadProteinCollectionNames()
    End Sub

    Public ReadOnly Property CollectionMembers As Protein_Storage.clsProteinStorage
        Get
            Return m_FileContents
        End Get
    End Property

    Public ReadOnly Property Authorities As Dictionary(Of String, String)
        Get
            Return m_AuthoritiesList
        End Get
    End Property

    Protected Function GetCollectionNameFromID(ProteinCollectionID As Integer) As String
        Dim foundRows() As DataRow = m_CollectionsList.Select("Protein_Collection_ID = " & CStr(ProteinCollectionID))
        Dim dr As DataRow = foundRows(0)
        Dim collectionName = CStr(dr.Item("FileName"))

        Return collectionName
    End Function

    Protected Function LoadFASTA(filePath As String) As Protein_Storage.clsProteinStorage

        'check for existence of current file
        Dim fastaContents As Protein_Storage.clsProteinStorage
        fastaContents = m_Importer.GetProteinEntries(filePath)

        Dim errorMessage As String = m_Importer.LastErrorMessage()

        If Not String.IsNullOrWhiteSpace(errorMessage) Then
            Dim proteinsLoaded As Integer

            Try
                If Not fastaContents Is Nothing Then
                    proteinsLoaded = fastaContents.ProteinCount
                End If
            Catch ex As Exception
                ' Ignore errors here
            End Try

            MessageBox.Show("GetProteinEntries returned an error after loading " & proteinsLoaded & " proteins: " & errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

            fastaContents.ClearProteinEntries()
        End If

        Return fastaContents

    End Function

    Public Function LoadOrganisms() As DataTable
        Dim orgSQL = "SELECT * FROM V_Organism_Picker ORDER BY Short_Name"
        Dim tmpOrgTable As DataTable = m_SQLAccess.GetTable(orgSQL)

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

    Public Function LoadAnnotationTypes(
        proteinCollectionID As Integer) As DataTable

        Dim AnnTypeIDSQL As String
        AnnTypeIDSQL =
            "SELECT Annotation_Type_ID " &
            "FROM V_Protein_Collection_Authority " &
            "WHERE Protein_Collection_ID = " & proteinCollectionID.ToString

        Dim tmpAnnTypeIDTable As DataTable
        tmpAnnTypeIDTable = m_SQLAccess.GetTable(AnnTypeIDSQL)

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

        Dim AuthSQL As String =
            "SELECT * FROM V_Annotation_Type_Picker " &
            "WHERE ID IN (" & authIDSB.ToString & ") " &
            "ORDER BY Display_Name"

        Dim tmpAuthTable As DataTable = m_SQLAccess.GetTable(AuthSQL)

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

    Public Function LoadAnnotationTypes() As DataTable
        Dim AuthSQL As String = "SELECT * FROM V_Annotation_Type_Picker ORDER BY Display_Name"
        Dim tmpAnnTypeTable As DataTable = m_SQLAccess.GetTable(AuthSQL)

        Dim dr As DataRow = tmpAnnTypeTable.NewRow

        With dr
            .Item("ID") = 0
            .Item("Display_Name") = " -- None Selected -- "
            '.Item("name") = " -- None Selected -- "
            .Item("Details") = "None Selected"
        End With

        tmpAnnTypeTable.Rows.InsertAt(dr, 0)

        tmpAnnTypeTable.AcceptChanges()
        m_AuthoritiesList = m_SQLAccess.DataTableToDictionary(tmpAnnTypeTable, "ID", "Display_Name")
        m_AuthoritiesTable = tmpAnnTypeTable.Copy

        Return tmpAnnTypeTable
    End Function

    Public Function LoadAuthorities() As DataTable
        Dim AuthSQL As String = "SELECT * FROM V_Authority_Picker ORDER BY Display_Name"
        Dim tmpAuthTable As DataTable = m_SQLAccess.GetTable(AuthSQL)

        Dim dr As DataRow = tmpAuthTable.NewRow

        With dr
            .Item("ID") = 0
            .Item("Display_Name") = " -- None Selected -- "
            .Item("Details") = "None Selected"
        End With

        tmpAuthTable.Rows.InsertAt(dr, 0)

        tmpAuthTable.AcceptChanges()
        m_AuthoritiesList = m_SQLAccess.DataTableToDictionary(tmpAuthTable, "ID", "Display_Name")

        Return tmpAuthTable
    End Function

    Public Sub ClearProteinCollection()
        If Not m_FileContents Is Nothing Then
            m_FileContents.ClearProteinEntries()
        End If
    End Sub

    Public Sub TriggerProteinCollectionsLoad()
        OnCollectionLoadComplete(LoadProteinCollections)
    End Sub

    Public Sub TriggerProteinCollectionsLoad(Organism_ID As Integer)
        OnCollectionLoadComplete(LoadProteinCollections(Organism_ID))
    End Sub

    Public Sub TriggerProteinCollectionTableUpdate()
        'Dim errCode As Integer = RunSP_UpdateProteinCollectionsByOrganism
        OnCollectionLoadComplete(LoadProteinCollections)
    End Sub

    Public Function LoadProteinCollections() As DataTable

        Dim PCSQL = "SELECT MIN(FileName) AS FileName, Protein_Collection_ID, " &
                    "MIN(Organism_ID) AS Organism_ID, MIN(Authority_ID) AS Authority_ID, " &
                    "MIN(Display) AS Display, MIN(Authentication_Hash) AS Authentication_Hash " &
                "FROM V_Protein_Collections_By_Organism " &
                "GROUP BY Protein_Collection_ID " &
                "ORDER BY MIN(FileName)"

        Dim tmpPCTable As DataTable = m_SQLAccess.GetTable(PCSQL)

        Dim dr As DataRow = tmpPCTable.NewRow

        With dr
            .Item("Protein_Collection_ID") = 0
            .Item("Display") = " -- None Selected -- "
        End With

        tmpPCTable.Rows.InsertAt(dr, 0)
        tmpPCTable.AcceptChanges()

        Return tmpPCTable
    End Function

    Protected Function LoadProteinCollections(Organism_ID As Integer) As DataTable

        Dim PCSQL = "SELECT FileName, Protein_Collection_ID, Organism_ID, Authority_ID, Display, Authentication_Hash" &
                              " FROM V_Protein_Collections_By_Organism" &
                              " WHERE Organism_ID = " & Organism_ID &
                              " ORDER BY [FileName]"
        Dim tmpPCTable As DataTable = m_SQLAccess.GetTable(PCSQL)

        Dim dr As DataRow = tmpPCTable.NewRow

        With dr
            .Item("Protein_Collection_ID") = 0
            .Item("Display") = " -- None Selected -- "
        End With

        tmpPCTable.Rows.InsertAt(dr, 0)
        tmpPCTable.AcceptChanges()

        Return tmpPCTable
    End Function

    Public Function LoadProteinCollectionNames() As DataTable
        Dim PCSQL As String =
            "SELECT Protein_Collection_ID, FileName, Authority_ID " &
            "FROM V_Protein_Collections_By_Organism " &
            "ORDER BY FileName"
        Dim tmpPCTable As DataTable = m_SQLAccess.GetTable(PCSQL)

        Dim dr As DataRow = tmpPCTable.NewRow

        With dr
            .Item("Protein_Collection_ID") = 0
            .Item("FileName") = " -- None Selected -- "
        End With

        tmpPCTable.Rows.InsertAt(dr, 0)
        tmpPCTable.AcceptChanges()

        Return tmpPCTable
    End Function

    Public Function LoadCollectionMembersByID(
        collectionID As Integer,
        authorityID As Integer) As DataTable

        m_CollectionsList = LoadProteinCollections()

        If authorityID <= 0 Then
            Dim foundRows = m_CollectionsList.Select("Protein_Collection_ID = " & collectionID)
            authorityID = CInt(foundRows(0).Item("Authority_ID"))

        End If

        Dim MemberSQL As String =
            "SELECT * From V_Protein_Storage_Entry_Import " &
            "WHERE [Protein_Collection_ID] = " & collectionID.ToString & " " &
                "AND Annotation_Type_ID = " & authorityID.ToString & " " &
                "ORDER BY [Name]"
        Return LoadCollectionMembers(MemberSQL)
    End Function

    Public Function LoadCollectionMembersByName(
        collectionName As String,
        authorityID As Integer) As DataTable

        Dim GetIDSQL As String = "SELECT Protein_Collection_ID, Primary_Annotation_Type_ID " &
            "FROM T_Protein_Collections " &
            "WHERE [FileName] = " & collectionName & " ORDER BY [Name]"

        Dim tmpTable As DataTable = m_SQLAccess.GetTable(GetIDSQL)
        Dim foundRow As DataRow = tmpTable.Rows(0)
        Dim collectionID As Integer = DirectCast(foundRow.Item("Protein_Collection_ID"), Int32)
        'Dim authorityID As Integer = DirectCast(foundRow.Item("Primary_Authority_ID"), Int32)

        Return LoadCollectionMembersByID(collectionID, authorityID)

    End Function

    Private Function LoadCollectionMembers(SelectStatement As String) As DataTable
        Dim tmpMemberTable As DataTable = m_SQLAccess.GetTable(SelectStatement)

        m_FileContents = LoadProteinInfo(tmpMemberTable.Select(""))

        Return tmpMemberTable

    End Function

    Protected Function LoadProteinInfo(proteinCollectionMembers() As DataRow) As Protein_Storage.clsProteinStorage
        Dim tmpPS = New Protein_Storage.clsProteinStorageDMS("")
        Dim proteinCount As Integer
        Dim triggerCount As Integer
        Dim counter As Integer

        RaiseEvent LoadStart("Retrieving Protein Entries...")

        proteinCount = proteinCollectionMembers.Length

        If proteinCount > 20 Then
            triggerCount = CInt(proteinCount / 20)
        Else
            triggerCount = 1
        End If

        For Each dr As DataRow In proteinCollectionMembers
            Dim ce = New Protein_Storage.clsProteinStorageEntry(
                dr.Item("Name").ToString,
                dr.Item("Description").ToString,
                dr.Item("Sequence").ToString,
                DirectCast(dr.Item("Length"), Int32),
                DirectCast(dr.Item("Monoisotopic_Mass"), Double),
                DirectCast(dr.Item("Average_Mass"), Double),
                dr.Item("Molecular_Formula").ToString,
                dr.Item("SHA1_Hash").ToString, counter)

            If counter Mod triggerCount > 0 Then
                Task_LoadProgress(CSng(counter / proteinCount))
            End If

            ce.Protein_ID = DirectCast(dr.Item("Protein_ID"), Int32)
            tmpPS.AddProtein(ce)
            counter += 1
        Next

        Return CType(tmpPS, clsProteinStorage)

    End Function

    'Function to load fasta file contents with no checking against the existing database entries
    'used to load up the source collection ListView
    Public Function LoadProteinsRaw(
        filePath As String,
        fileType As ProteinImportFileTypes) As DataTable

        Dim tmpProteinTable As DataTable = m_SQLAccess.GetTableTemplate("V_Protein_Database_Export")
        Dim counter As Integer
        Dim triggerCount As Integer
        Dim proteinCount As Integer

        Select Case fileType
            Case ProteinImportFileTypes.FASTA
                m_FileContents = LoadFASTA(filePath)
            Case Else
                Return Nothing
        End Select

        If m_FileContents Is Nothing Then
            Return Nothing
        End If

        proteinCount = CInt(m_FileContents.ProteinCount)
        If proteinCount > 20 Then
            triggerCount = CInt(proteinCount / 20)
        Else
            triggerCount = 1
        End If

        Dim contentsEnum = m_FileContents.GetEnumerator

        'Move certain elements of the protein record to a DataTable for display in the source window
        Task_LoadStart("Updating Display List...")
        Do While contentsEnum.MoveNext()
            Dim entry = contentsEnum.Current.Value
            Dim dr = tmpProteinTable.NewRow
            dr.Item("Name") = entry.Reference
            dr.Item("Description") = entry.Description
            dr.Item("Sequence") = entry.Sequence
            tmpProteinTable.Rows.Add(dr)
            If counter Mod triggerCount > 0 Then
                Task_LoadProgress(CSng(counter / proteinCount))
            End If
            counter += 1
        Loop
        Task_LoadEnd()

        Return tmpProteinTable

    End Function

    Public Function LoadProteinsForBatch(
        FullFilePath As String,
        SelectedOrganismID As Integer,
        SelectedAuthorityID As Integer) As Protein_Storage.clsProteinStorage

        Dim ps As Protein_Storage.clsProteinStorage = LoadFASTA(FullFilePath)

        Return ps
    End Function


#Region " Event Handlers "

    'handles loadstart event for fasta importer module
    Protected Sub Task_LoadStart(taskTitle As String) Handles m_Importer.LoadStart
        'm_PersistentTaskNum += 1
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Protected Sub Task_LoadProgress(fractionDone As Double) Handles m_Importer.LoadProgress
        RaiseEvent LoadProgress(fractionDone)
    End Sub

    'Private Sub Task_LoadProgress(taskTitle As String, fractionDone As Double) Handles m_Importer.ValidationProgress
    '    RaiseEvent ValidationProgress(taskTitle, fractionDone)
    'End Sub

    Protected Sub Task_LoadEnd() Handles m_Importer.LoadEnd
        RaiseEvent LoadEnd()
    End Sub

    'Private Sub OnInvalidFASTAFile(FASTAFilePath As String, errorCollection As ArrayList) Handles m_Importer.InvalidFASTAFile
    '    RaiseEvent InvalidFASTAFile(FASTAFilePath, errorCollection)
    'End Sub

    Protected Sub OnCollectionLoadComplete(CollectionsList As DataTable)
        RaiseEvent CollectionLoadComplete(CollectionsList)
    End Sub
#End Region

#Region " Stored Procedure Access "
    Protected Function RunSP_UpdateProteinCollectionsByOrganism() As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionsByOrganism",
            m_SQLAccess.Connection)

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
