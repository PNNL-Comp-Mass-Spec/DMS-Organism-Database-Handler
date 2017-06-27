Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms
Imports TableManipulationBase

Public Class clsBatchUploadFromFileList

    Protected WithEvents m_Uploader As IUploadProteins
    Protected m_TableGetter As IGetSQLData
    Protected m_CurrentFileList As Hashtable

    Protected m_PSConnectionString As String

    Protected m_AuthorityTable As DataTable
    Protected m_AnnotationTypeTable As DataTable
    Protected m_OrganismTable As DataTable

    Protected m_BatchForm As frmBatchUploadFromFileList

    Const DMS_Org_DB_Table_Name As String = "V_Legacy_Static_File_Locations"
    Const Protein_Collections_Table_Name As String = "T_Protein_Collections"

    Sub New(PSConnectionString As String)

        m_PSConnectionString = PSConnectionString
        m_Uploader = New clsPSUploadHandler(PSConnectionString)
        m_TableGetter = New clsDBTask(PSConnectionString)
    End Sub

    Public Event ProgressUpdate(fractionDone As Double)
    Public Event TaskChange(currentTaskTitle As String)
    Public Event LoadStart(taskTitle As String)
    Public Event LoadEnd()

    Private Sub OnTaskChange(currentTaskTitle As String) Handles m_Uploader.BatchProgress
        RaiseEvent TaskChange(currentTaskTitle)
    End Sub

    Private Sub OnProgressUpdate(fractionDone As Double) Handles m_Uploader.LoadProgress
        RaiseEvent ProgressUpdate(fractionDone)
    End Sub

    Private Sub OnLoadStart(taskTitle As String) Handles m_Uploader.LoadStart
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Private Sub OnLoadEnd() Handles m_Uploader.LoadEnd
        RaiseEvent LoadEnd()
    End Sub

    Sub UploadBatch()

        Dim fileCollection As Hashtable
        Dim fce As FileListInfo

        Dim ui As IUploadProteins.UploadInfo
        Dim uiList = New List(Of IUploadProteins.UploadInfo)

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
            If m_Uploader Is Nothing Then
                m_Uploader = New clsPSUploadHandler(m_PSConnectionString)
            End If
            For Each fce In fileCollection.Values
                ui = TransformToUploadInfo(fce)
                uiList.Add(ui)
            Next

            m_Uploader.InitialSetup()
            m_Uploader.BatchUpload(uiList)


        End If

    End Sub

    Protected Function GetAuthorityTable() As DataTable
        Const authSQL = "SELECT ID, Display_Name, Details FROM V_Authority_Picker"
        Return m_TableGetter.GetTable(authSQL)
    End Function

    Protected Function GetAnnotationTypeTable() As DataTable
        Const annoSQL = "SELECT ID, Display_Name, Details FROM V_Annotation_Type_Picker"
        Return m_TableGetter.GetTable(annoSQL)
    End Function

    Protected Function GetOrganismsTable() As DataTable
        Const orgSQL = "SELECT ID, Short_Name, Display_Name, Organism_Name FROM V_Organism_Picker"
        Return m_TableGetter.GetTable(orgSQL)
    End Function

    Private Function TransformToUploadInfo(fli As FileListInfo) As IUploadProteins.UploadInfo

        Dim fi = New FileInfo(fli.FullFilePath)
        Dim ui As New IUploadProteins.UploadInfo(fi, fli.OrganismID, fli.AnnotationTypeID)

        Return ui

    End Function

    'Protected Function GetDMSFileEntities() As Hashtable
    '    Dim fileList As New Hashtable
    '    Dim collectionList As New ArrayList
    '    Dim fileTable As DataTable
    '    Dim collectionTable As DataTable
    '    Dim DMSOrgDBSQL As String
    '    Dim dr As DataRow

    '    Dim LoadedCollectionsSQL As String

    '    Dim tmpFileName As String
    '    Dim tmpOrganismName As String
    '    Dim tmpOrganismID As Integer
    '    Dim tmpFullPath As String

    '    DMSOrgDBSQL = "SELECT ID, FileName, Full_Path, Organism_Name, Organism_ID FROM " & DMS_Org_DB_Table_Name
    '    fileTable = Me.m_TableGetter.GetTable(DMSOrgDBSQL)

    '    LoadedCollectionsSQL = "SELECT FileName FROM " & Protein_Collections_Table_Name
    '    collectionTable = Me.m_TableGetter.GetTable(LoadedCollectionsSQL)

    '    For Each dr In collectionTable.Rows
    '        collectionList.Add(dr.Item("FileName").ToString)
    '    Next

    '    If Me.m_CurrentFileList Is Nothing Then
    '        Me.m_CurrentFileList = New Hashtable
    '    Else
    '        Me.m_CurrentFileList.Clear()
    '    End If

    '    For Each dr In fileTable.Rows
    '        tmpFileName = dr.Item("FileName").ToString
    '        tmpOrganismName = dr.Item("Organism_Name").ToString
    '        tmpOrganismID = CInt(dr.Item("Organism_ID"))
    '        tmpFullPath = dr.Item("Full_Path").ToString
    '        If Not fileList.ContainsKey(tmpFileName) And Not collectionList.Contains(System.IO.Path.GetFileNameWithoutExtension(tmpFileName)) Then
    '            fileList.Add(tmpFileName, New FileListInfo(tmpFileName, tmpFullPath, tmpOrganismName, tmpOrganismID))
    '        End If
    '    Next

    '    fileTable.Clear()
    '    fileTable = Nothing

    '    Return fileList

    'End Function

    Protected Function GetDMSFileEntities() As Hashtable
        Dim fileList As New Hashtable
        Dim collectionList As New ArrayList
        Dim fileTable As DataTable

        Dim dr As DataRow

        Dim LoadedCollectionsSQL As String

        Dim tmpFileName As String
        Dim tmpOrganismName As String
        Dim tmpOrganismID As Integer
        Dim tmpFullPath As String
        Dim tmpAnnTypeID As Integer
        Dim tmpAuthTypeID As Integer

        'DMSOrgDBSQL = "SELECT ID, FileName, Full_Path, Organism_Name, Organism_ID FROM " & DMS_Org_DB_Table_Name
        'fileTable = Me.m_TableGetter.GetTable(DMSOrgDBSQL)

        'LoadedCollectionsSQL = "SELECT FileName FROM " & Protein_Collections_Table_Name
        'collectionTable = Me.m_TableGetter.GetTable(LoadedCollectionsSQL)

        'For Each dr In collectionTable.Rows
        '    collectionList.Add(dr.Item("FileName").ToString)
        'Next

        LoadedCollectionsSQL = "SELECT FileName, Full_Path, Organism_Name, Organism_ID, Annotation_Type_ID, Authority_ID FROM V_Collections_Reload_Filtered"

        If m_TableGetter Is Nothing Then
            m_TableGetter = New clsDBTask(m_PSConnectionString)
        End If
        fileTable = m_TableGetter.GetTable(LoadedCollectionsSQL)

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
        fileTable = Nothing

        Return fileList

    End Function

    Protected Function UploadSelectedFiles(fileNameList As Hashtable) As Integer

        Dim upInfoContainer As IUploadProteins.UploadInfo
        Dim fli As FileListInfo
        Dim selectedFileList = New List(Of IUploadProteins.UploadInfo)

        For Each fli In fileNameList.Values
            upInfoContainer = New IUploadProteins.UploadInfo(
                New FileInfo(fli.FullFilePath),
                fli.OrganismID, fli.NamingAuthorityID)
            selectedFileList.Add(upInfoContainer)
        Next

        m_Uploader.BatchUpload(selectedFileList)

    End Function


    Friend Structure FileListInfo
        Private m_FileName As String
        Private m_FullFilePath As String
        Private m_Organism As String
        Private m_OrganismID As Integer
        Private m_NamingAuthorityID As Integer
        Private m_AnnotationType As String
        Private m_AnnotationTypeID As Integer

        Sub New(
            FileName As String,
            FullFilePath As String,
            OrganismName As String,
            OrganismID As Integer)

            m_FileName = FileName
            m_FullFilePath = FullFilePath
            m_Organism = OrganismName
            m_OrganismID = OrganismID

        End Sub

        Sub New(
            FileName As String,
            FullFilePath As String,
            OrganismName As String,
            OrganismID As Integer,
            AnnotationTypeID As Integer,
            NamingAuthorityID As Integer)

            m_FileName = FileName
            m_FullFilePath = FullFilePath
            m_Organism = OrganismName
            m_OrganismID = OrganismID
            m_AnnotationTypeID = AnnotationTypeID
            m_NamingAuthorityID = NamingAuthorityID

        End Sub

        Property FileName() As String
            Get
                Return m_FileName
            End Get
            Set(Value As String)
                m_FileName = Value
            End Set
        End Property

        Property FullFilePath() As String
            Get
                Return m_FullFilePath
            End Get
            Set(Value As String)
                m_FullFilePath = Value
            End Set
        End Property

        Property OrganismName() As String
            Get
                Return m_Organism
            End Get
            Set(Value As String)
                m_Organism = Value
            End Set
        End Property

        Property OrganismID() As Integer
            Get
                Return m_OrganismID
            End Get
            Set(Value As Integer)
                m_OrganismID = Value
            End Set
        End Property

        Property NamingAuthorityID() As Integer
            Get
                Return m_NamingAuthorityID
            End Get
            Set(Value As Integer)
                m_NamingAuthorityID = Value
            End Set
        End Property

        Property AnnotationTypeID() As Integer
            Get
                Return m_AnnotationTypeID
            End Get
            Set(Value As Integer)
                m_AnnotationTypeID = Value
            End Set
        End Property

        Property AnnotationType() As String
            Get
                Return m_AnnotationType
            End Get
            Set(Value As String)
                m_AnnotationType = Value
            End Set
        End Property
    End Structure

End Class
