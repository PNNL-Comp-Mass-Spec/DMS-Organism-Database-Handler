Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms
Imports TableManipulationBase

Public Class clsBatchUploadFromFileList

    Protected ReadOnly m_Uploader As clsPSUploadHandler
    Protected ReadOnly m_DatabaseAccessor As clsDBTask
    Protected m_CurrentFileList As Hashtable


    Protected m_AuthorityTable As DataTable
    Protected m_AnnotationTypeTable As DataTable
    Protected m_OrganismTable As DataTable

    Protected m_BatchForm As frmBatchUploadFromFileList

    Const DMS_Org_DB_Table_Name As String = "V_Legacy_Static_File_Locations"
    Const Protein_Collections_Table_Name As String = "T_Protein_Collections"

    Sub New(psConnectionString As String)

        m_Uploader = New clsPSUploadHandler(psConnectionString)
        AddHandler m_Uploader.BatchProgress, AddressOf OnTaskChange
        AddHandler m_Uploader.LoadProgress, AddressOf OnProgressUpdate
        AddHandler m_Uploader.LoadStart, AddressOf OnLoadStart
        AddHandler m_Uploader.LoadEnd, AddressOf OnLoadEnd
        AddHandler m_Uploader.LoadStart, AddressOf OnLoadStart

        m_DatabaseAccessor = New clsDBTask(psConnectionString)
    End Sub

    Public Event ProgressUpdate(fractionDone As Double)
    Public Event TaskChange(currentTaskTitle As String)
    Public Event LoadStart(taskTitle As String)
    Public Event LoadEnd()

    Private Sub OnTaskChange(currentTaskTitle As String)
        RaiseEvent TaskChange(currentTaskTitle)
    End Sub

    Private Sub OnProgressUpdate(fractionDone As Double)
        RaiseEvent ProgressUpdate(fractionDone)
    End Sub

    Private Sub OnLoadStart(taskTitle As String)
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Private Sub OnLoadEnd()
        RaiseEvent LoadEnd()
    End Sub

    Sub UploadBatch()

        Dim fileCollection As Hashtable
        Dim fce As FileListInfo

        Dim ui As clsPSUploadHandler.UploadInfo
        Dim uiList = New List(Of clsPSUploadHandler.UploadInfo)

        m_AnnotationTypeTable = GetAnnotationTypeTable()
        m_AuthorityTable = GetAuthorityTable()
        m_OrganismTable = GetOrganismsTable()

        m_BatchForm = New frmBatchUploadFromFileList(m_AuthorityTable, m_AnnotationTypeTable, m_OrganismTable)

        m_CurrentFileList = GetDMSFileEntities()

        m_BatchForm.FileCollection = m_CurrentFileList

        Dim r As DialogResult

        r = m_BatchForm.ShowDialog()

        If r = DialogResult.OK Then
            fileCollection = m_BatchForm.SelectedFilesCollection

            For Each fce In fileCollection.Values
                ui = TransformToUploadInfo(fce)
                uiList.Add(ui)
            Next

            m_Uploader.BatchUpload(uiList)

        End If

    End Sub

    Protected Function GetAuthorityTable() As DataTable
        Const authSQL = "SELECT ID, Display_Name, Details FROM V_Authority_Picker"
        Return m_DatabaseAccessor.GetTable(authSQL)
    End Function

    Protected Function GetAnnotationTypeTable() As DataTable
        Const annoSQL = "SELECT ID, Display_Name, Details FROM V_Annotation_Type_Picker"
        Return m_DatabaseAccessor.GetTable(annoSQL)
    End Function

    Protected Function GetOrganismsTable() As DataTable
        Const orgSQL = "SELECT ID, Short_Name, Display_Name, Organism_Name FROM V_Organism_Picker"
        Return m_DatabaseAccessor.GetTable(orgSQL)
    End Function

    Private Function TransformToUploadInfo(fli As FileListInfo) As clsPSUploadHandler.UploadInfo

        Dim fi = New FileInfo(fli.FullFilePath)
        Dim ui As New clsPSUploadHandler.UploadInfo(fi, fli.OrganismID, fli.AnnotationTypeID)

        Return ui

    End Function

    Protected Function GetDMSFileEntities() As Hashtable
        Dim fileList As New Hashtable
        Dim collectionList As New ArrayList

        Dim dr As DataRow

        Dim LoadedCollectionsSQL As String

        Dim tmpFileName As String
        Dim tmpOrganismName As String
        Dim tmpOrganismID As Integer
        Dim tmpFullPath As String
        Dim tmpAnnTypeID As Integer
        Dim tmpAuthTypeID As Integer

        LoadedCollectionsSQL = "SELECT FileName, Full_Path, Organism_Name, Organism_ID, Annotation_Type_ID, Authority_ID FROM V_Collections_Reload_Filtered"

        Using fileTable As DataTable = m_DatabaseAccessor.GetTable(LoadedCollectionsSQL)

            If m_CurrentFileList Is Nothing Then
                m_CurrentFileList = New Hashtable
            Else
                m_CurrentFileList.Clear()
            End If

            For Each dr In fileTable.Rows
                tmpFileName = dr.Item("FileName").ToString
                tmpOrganismName = dr.Item("Organism_Name").ToString
                tmpOrganismID = CInt(dr.Item("Organism_ID"))
                tmpFullPath = dr.Item("Full_Path").ToString
                tmpAnnTypeID = CInt(dr.Item("Annotation_Type_ID"))
                tmpAuthTypeID = CInt(dr.Item("Authority_ID"))
                If Not fileList.ContainsKey(tmpFileName) And Not collectionList.Contains(Path.GetFileNameWithoutExtension(tmpFileName)) Then
                    fileList.Add(tmpFileName, New FileListInfo(tmpFileName, tmpFullPath, tmpOrganismName, tmpOrganismID, tmpAnnTypeID, tmpAuthTypeID))
                End If
            Next

            fileTable.Clear()
        End Using

        Return fileList

    End Function

    Protected Function UploadSelectedFiles(fileNameList As Hashtable) As Integer

        Dim upInfoContainer As clsPSUploadHandler.UploadInfo
        Dim fli As FileListInfo
        Dim selectedFileList = New List(Of clsPSUploadHandler.UploadInfo)

        For Each fli In fileNameList.Values
            upInfoContainer = New clsPSUploadHandler.UploadInfo(
                New FileInfo(fli.FullFilePath),
                fli.OrganismID, fli.NamingAuthorityID)
            selectedFileList.Add(upInfoContainer)
        Next

        m_Uploader.BatchUpload(selectedFileList)

    End Function


    Friend Structure FileListInfo
        Sub New(
            FileName As String,
            FullFilePath As String,
            OrganismName As String,
            OrganismID As Integer)

            Me.FileName = FileName
            Me.FullFilePath = FullFilePath
            Me.OrganismName = OrganismName
            Me.OrganismID = OrganismID

        End Sub

        Sub New(
            FileName As String,
            FullFilePath As String,
            OrganismName As String,
            OrganismID As Integer,
            AnnotationTypeID As Integer,
            NamingAuthorityID As Integer)

            Me.FileName = FileName
            Me.FullFilePath = FullFilePath
            Me.OrganismName = OrganismName
            Me.OrganismID = OrganismID
            Me.AnnotationTypeID = AnnotationTypeID
            Me.NamingAuthorityID = NamingAuthorityID

        End Sub

        Property FileName As String

        Property FullFilePath As String

        Property OrganismName As String

        Property OrganismID As Integer

        Property NamingAuthorityID As Integer

        Property AnnotationTypeID As Integer

        Public Property AnnotationType As String

    End Structure

End Class
