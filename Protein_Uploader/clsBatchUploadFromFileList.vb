Imports System.Collections.Generic

Public Class clsBatchUploadFromFileList

    Protected WithEvents m_Uploader As IUploadProteins
    Protected m_TableGetter As TableManipulationBase.IGetSQLData
    Protected m_CurrentFileList As Hashtable

    Protected m_PSConnectionString As String

    Protected m_AuthorityTable As DataTable
    Protected m_AnnotationTypeTable As DataTable
    Protected m_OrganismTable As DataTable

    Protected m_BatchForm As frmBatchUploadFromFileList

    Const DMS_Org_DB_Table_Name As String = "V_Legacy_Static_File_Locations"
    Const Protein_Collections_Table_Name As String = "T_Protein_Collections"

    Sub New(ByVal PSConnectionString As String)

        Me.m_PSConnectionString = PSConnectionString
        Me.m_Uploader = New clsPSUploadHandler(PSConnectionString)
        Me.m_TableGetter = New TableManipulationBase.clsDBTask(PSConnectionString)
    End Sub

    Public Event ProgressUpdate(ByVal fractionDone As Double)
    Public Event TaskChange(ByVal currentTaskTitle As String)
    Public Event LoadStart(ByVal taskTitle As String)
    Public Event LoadEnd()

    Private Sub OnTaskChange(ByVal currentTaskTitle As String) Handles m_Uploader.BatchProgress
        RaiseEvent TaskChange(currentTaskTitle)
    End Sub

    Private Sub OnProgressUpdate(ByVal fractionDone As Double) Handles m_Uploader.LoadProgress
        RaiseEvent ProgressUpdate(fractionDone)
    End Sub

    Private Sub OnLoadStart(ByVal taskTitle As String) Handles m_Uploader.LoadStart
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Private Sub OnLoadEnd() Handles m_Uploader.LoadEnd
        RaiseEvent LoadEnd()
    End Sub

    Sub UploadBatch()

        Dim fileCollection As Hashtable
        Dim fce As FileListInfo

        Dim ui As Protein_Uploader.IUploadProteins.UploadInfo
        Dim uiList As List(Of IUploadProteins.UploadInfo) = New List(Of IUploadProteins.UploadInfo)

        Me.m_AnnotationTypeTable = Me.GetAnnotationTypeTable()
        Me.m_AuthorityTable = Me.GetAuthorityTable
        Me.m_OrganismTable = Me.GetOrganismsTable

        Me.m_BatchForm = New frmBatchUploadFromFileList(Me.m_AuthorityTable, Me.m_AnnotationTypeTable, Me.m_OrganismTable)

        Me.m_CurrentFileList = Me.GetDMSFileEntities

        Me.m_BatchForm.FileCollection = Me.m_CurrentFileList

        Dim r As System.Windows.Forms.DialogResult

        r = Me.m_BatchForm.ShowDialog()

        If r = Windows.Forms.DialogResult.OK Then
            fileCollection = Me.m_BatchForm.SelectedFilesCollection
            If Me.m_Uploader Is Nothing Then
                Me.m_Uploader = New Protein_Uploader.clsPSUploadHandler(Me.m_PSConnectionString)
            End If
            For Each fce In fileCollection.Values
                ui = Me.TransformToUploadInfo(fce)
                uiList.Add(ui)
            Next

            Me.m_Uploader.InitialSetup()
            Me.m_Uploader.BatchUpload(uiList)


        End If

    End Sub

    Protected Function GetAuthorityTable() As DataTable
        Const authSQL As String = "SELECT ID, Display_Name, Details FROM V_Authority_Picker"
        Return Me.m_TableGetter.GetTable(authSQL)
    End Function

    Protected Function GetAnnotationTypeTable() As DataTable
        Const annoSQL As String = "SELECT ID, Display_Name, Details FROM V_Annotation_Type_Picker"
        Return Me.m_TableGetter.GetTable(annoSQL)
    End Function

    Protected Function GetOrganismsTable() As DataTable
        Const orgSQL As String = "SELECT ID, Short_Name, Display_Name, Organism_Name FROM V_Organism_Picker"
        Return Me.m_TableGetter.GetTable(orgSQL)
    End Function

    Private Function TransformToUploadInfo(ByVal fli As FileListInfo) As Protein_Uploader.IUploadProteins.UploadInfo

        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(fli.FullFilePath)
        Dim ui As New Protein_Uploader.IUploadProteins.UploadInfo(fi, fli.OrganismID, fli.AnnotationTypeID)

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

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If
        fileTable = Me.m_TableGetter.GetTable(LoadedCollectionsSQL)

        If Me.m_CurrentFileList Is Nothing Then
            Me.m_CurrentFileList = New Hashtable
        Else
            Me.m_CurrentFileList.Clear()
        End If

        For Each dr In fileTable.Rows
            tmpFileName = dr.Item("FileName").ToString
            tmpOrganismName = dr.Item("Organism_Name").ToString
            tmpOrganismID = CInt(dr.Item("Organism_ID"))
            tmpFullPath = dr.Item("Full_Path").ToString
            tmpAnnTypeID = CInt(dr.Item("Annotation_Type_ID"))
            tmpAuthTypeID = CInt(dr.Item("Authority_ID"))
            If Not fileList.ContainsKey(tmpFileName) And Not collectionList.Contains(System.IO.Path.GetFileNameWithoutExtension(tmpFileName)) Then
                fileList.Add(tmpFileName, New FileListInfo(tmpFileName, tmpFullPath, tmpOrganismName, tmpOrganismID, tmpAnnTypeID, tmpAuthTypeID))
            End If
        Next

        fileTable.Clear()
        fileTable = Nothing

        Return fileList

    End Function

    Protected Function UploadSelectedFiles(ByVal fileNameList As Hashtable) As Integer

        Dim upInfoContainer As IUploadProteins.UploadInfo
        Dim fli As FileListInfo
        Dim selectedFileList As List(Of IUploadProteins.UploadInfo) = New List(Of IUploadProteins.UploadInfo)

        For Each fli In fileNameList.Values
            upInfoContainer = New IUploadProteins.UploadInfo( _
                New System.IO.FileInfo(fli.FullFilePath), _
                fli.OrganismID, fli.NamingAuthorityID)
            selectedFileList.Add(upInfoContainer)
        Next

        Me.m_Uploader.BatchUpload(selectedFileList)

    End Function


    Friend Structure FileListInfo
        Private m_FileName As String
        Private m_FullFilePath As String
        Private m_Organism As String
        Private m_OrganismID As Integer
        Private m_NamingAuthorityID As Integer
        Private m_AnnotationType As String
        Private m_AnnotationTypeID As Integer

        Sub New( _
            ByVal FileName As String, _
            ByVal FullFilePath As String, _
            ByVal OrganismName As String, _
            ByVal OrganismID As Integer)

            m_FileName = FileName
            m_FullFilePath = FullFilePath
            m_Organism = OrganismName
            m_OrganismID = OrganismID

        End Sub

        Sub New( _
            ByVal FileName As String, _
            ByVal FullFilePath As String, _
            ByVal OrganismName As String, _
            ByVal OrganismID As Integer, _
            ByVal AnnotationTypeID As Integer, _
            ByVal NamingAuthorityID As Integer)

            m_FileName = FileName
            m_FullFilePath = FullFilePath
            m_Organism = OrganismName
            m_OrganismID = OrganismID
            m_AnnotationTypeID = AnnotationTypeID
            m_NamingAuthorityID = NamingAuthorityID

        End Sub

        Property FileName() As String
            Get
                Return Me.m_FileName
            End Get
            Set(ByVal Value As String)
                Me.m_FileName = Value
            End Set
        End Property

        Property FullFilePath() As String
            Get
                Return Me.m_FullFilePath
            End Get
            Set(ByVal Value As String)
                Me.m_FullFilePath = Value
            End Set
        End Property

        Property OrganismName() As String
            Get
                Return Me.m_Organism
            End Get
            Set(ByVal Value As String)
                Me.m_Organism = Value
            End Set
        End Property

        Property OrganismID() As Integer
            Get
                Return Me.m_OrganismID
            End Get
            Set(ByVal Value As Integer)
                Me.m_OrganismID = Value
            End Set
        End Property

        Property NamingAuthorityID() As Integer
            Get
                Return Me.m_NamingAuthorityID
            End Get
            Set(ByVal Value As Integer)
                Me.m_NamingAuthorityID = Value
            End Set
        End Property

        Property AnnotationTypeID() As Integer
            Get
                Return Me.m_AnnotationTypeID
            End Get
            Set(ByVal Value As Integer)
                Me.m_AnnotationTypeID = Value
            End Set
        End Property

        Property AnnotationType() As String
            Get
                Return Me.m_AnnotationType
            End Get
            Set(ByVal Value As String)
                Me.m_AnnotationType = Value
            End Set
        End Property
    End Structure

End Class
