Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms
Imports TableManipulationBase

Public Class BatchUploadFromFileList

    Protected ReadOnly m_Uploader As PSUploadHandler
    Protected ReadOnly m_DatabaseAccessor As DBTask
    Protected m_CurrentFileList As Dictionary(Of String, FileListInfo)

    Protected m_AuthorityTable As DataTable
    Protected m_AnnotationTypeTable As DataTable
    Protected m_OrganismTable As DataTable

    Protected m_BatchForm As frmBatchUploadFromFileList

    Const DMS_Org_DB_Table_Name As String = "V_Legacy_Static_File_Locations"
    Const Protein_Collections_Table_Name As String = "T_Protein_Collections"

    Sub New(psConnectionString As String)

        m_Uploader = New PSUploadHandler(psConnectionString)
        AddHandler m_Uploader.BatchProgress, AddressOf OnTaskChange
        AddHandler m_Uploader.LoadProgress, AddressOf OnProgressUpdate
        AddHandler m_Uploader.LoadStart, AddressOf OnLoadStart
        AddHandler m_Uploader.LoadEnd, AddressOf OnLoadEnd
        AddHandler m_Uploader.LoadStart, AddressOf OnLoadStart

        m_DatabaseAccessor = New DBTask(psConnectionString)
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

        Dim uiList = New List(Of PSUploadHandler.UploadInfo)

        m_AnnotationTypeTable = GetAnnotationTypeTable()
        m_AuthorityTable = GetAuthorityTable()
        m_OrganismTable = GetOrganismsTable()

        m_BatchForm = New frmBatchUploadFromFileList(m_AuthorityTable, m_AnnotationTypeTable, m_OrganismTable)

        m_CurrentFileList = GetDMSFileEntities()

        m_BatchForm.FileCollection = m_CurrentFileList

        Dim r As DialogResult

        r = m_BatchForm.ShowDialog()

        If r = DialogResult.OK Then
            Dim fileCollection = m_BatchForm.SelectedFilesCollection

            For Each fce In fileCollection.Values
                Dim ui = TransformToUploadInfo(fce)
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

    Private Function TransformToUploadInfo(fli As FileListInfo) As PSUploadHandler.UploadInfo

        Dim fi = New FileInfo(fli.FullFilePath)
        Dim ui = New PSUploadHandler.UploadInfo(fi, fli.OrganismID, fli.AnnotationTypeID)

        Return ui

    End Function

    Protected Function GetDMSFileEntities() As Dictionary(Of String, FileListInfo)
        Dim fileList = New Dictionary(Of String, FileListInfo)(StringComparer.OrdinalIgnoreCase)

        Dim collectionList = New SortedSet(Of String)(StringComparer.OrdinalIgnoreCase)

        'Dim dr As DataRow

        'Dim LoadedCollectionsSQL As String

        'Dim tmpFileName As String
        'Dim tmpOrganismName As String
        'Dim tmpOrganismID As Integer
        'Dim tmpFullPath As String
        'Dim tmpAnnTypeID As Integer
        'Dim tmpAuthTypeID As Integer

        Dim loadedCollectionsSQL = "SELECT FileName, Full_Path, Organism_Name, Organism_ID, Annotation_Type_ID, Authority_ID FROM V_Collections_Reload_Filtered"

        Using fileTable As DataTable = m_DatabaseAccessor.GetTable(loadedCollectionsSQL)

            If m_CurrentFileList Is Nothing Then
                m_CurrentFileList = New Dictionary(Of String, FileListInfo)
            Else
                m_CurrentFileList.Clear()
            End If

            For Each dr As DataRow In fileTable.Rows
                Dim fileName = dr.Item("FileName").ToString
                Dim organismName = dr.Item("Organism_Name").ToString
                Dim organismID = CInt(dr.Item("Organism_ID"))
                Dim fullPath = dr.Item("Full_Path").ToString
                Dim annotationTypeID = CInt(dr.Item("Annotation_Type_ID"))
                Dim authorityTypeID = CInt(dr.Item("Authority_ID"))

                Dim baseName = Path.GetFileNameWithoutExtension(fileName)

                If Not fileList.ContainsKey(fileName) And Not collectionList.Contains(baseName) Then
                    fileList.Add(fileName,
                                 New FileListInfo(fileName, fullPath, organismName, organismID, annotationTypeID, authorityTypeID))
                End If
            Next

            fileTable.Clear()
        End Using

        Return fileList

    End Function

    Protected Function UploadSelectedFiles(fileNameList As Dictionary(Of String, FileListInfo)) As Integer

        Dim selectedFileList = New List(Of PSUploadHandler.UploadInfo)

        For Each fli In fileNameList.Values
            Dim upInfoContainer = New PSUploadHandler.UploadInfo(
                New FileInfo(fli.FullFilePath), fli.OrganismID, fli.NamingAuthorityID)
            selectedFileList.Add(upInfoContainer)
        Next

        m_Uploader.BatchUpload(selectedFileList)

    End Function

    Public Structure FileListInfo
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
