Imports ExpTreeLib
Imports ExpTreeLib.CShItem

Public Class frmBatchAddNewCollectionTest
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New( _
        ByVal OrganismList As DataTable, _
        ByVal AuthorityList As DataTable, _
        ByVal ExistingCollectionsList As DataTable, _
        ByVal PSConnectionString As String)

        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Me.m_OrganismList = OrganismList
        Me.m_OrganismListSorted = New DataView(Me.m_OrganismList)
        m_OrganismListSorted.Sort = "Display_Name"

        Me.m_AuthorityList = AuthorityList
        Me.m_CollectionsTable = ExistingCollectionsList
        Me.m_PSConnectionString = PSConnectionString

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents expUploadFolderSelect As ExpTreeLib.ExpTree
    Friend WithEvents cboOrganismSelect As System.Windows.Forms.ComboBox
    Friend WithEvents lblBatchUploadTree As System.Windows.Forms.Label
    Friend WithEvents lblOrganismSelect As System.Windows.Forms.Label
    Friend WithEvents lblFolderContents As System.Windows.Forms.Label
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdUploadChecked As System.Windows.Forms.Button
    Friend WithEvents lvwFolderContents As System.Windows.Forms.ListView
    Friend WithEvents colFileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colFileSize As System.Windows.Forms.ColumnHeader
    Friend WithEvents colFileModDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents cboAuthorityPicker As System.Windows.Forms.ComboBox
    Friend WithEvents colCollectionExists As System.Windows.Forms.ColumnHeader
    Friend WithEvents colUpFileName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colSelOrganism As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblSelectedFiles As System.Windows.Forms.Label
    Friend WithEvents lvwSelectedFiles As System.Windows.Forms.ListView
    Friend WithEvents cmdAddFile As UIControls.ImageButton
    Friend WithEvents cmdRemoveFile As UIControls.ImageButton
    Friend WithEvents VisualStyleProvider1 As Skybound.VisualStyles.VisualStyleProvider
    Friend WithEvents lblAnnAuth As System.Windows.Forms.Label
    Friend WithEvents colAnnType As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmdPreviewFile As System.Windows.Forms.Button
    Friend WithEvents chkEncryptionEnable As System.Windows.Forms.CheckBox
    Friend WithEvents colEncrypt As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblPassphrase As System.Windows.Forms.Label
    Friend WithEvents txtPassphrase As System.Windows.Forms.TextBox
    Friend WithEvents fraValidationOptions As System.Windows.Forms.GroupBox
    Friend WithEvents chkValidationAllowAsterisks As System.Windows.Forms.CheckBox
    Friend WithEvents chkValidationAllowDash As System.Windows.Forms.CheckBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmBatchAddNewCollectionTest))
        Me.expUploadFolderSelect = New ExpTreeLib.ExpTree
        Me.cboOrganismSelect = New System.Windows.Forms.ComboBox
        Me.lblBatchUploadTree = New System.Windows.Forms.Label
        Me.lblOrganismSelect = New System.Windows.Forms.Label
        Me.lblFolderContents = New System.Windows.Forms.Label
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdUploadChecked = New System.Windows.Forms.Button
        Me.lvwFolderContents = New System.Windows.Forms.ListView
        Me.colFileName = New System.Windows.Forms.ColumnHeader
        Me.colFileModDate = New System.Windows.Forms.ColumnHeader
        Me.colFileSize = New System.Windows.Forms.ColumnHeader
        Me.colCollectionExists = New System.Windows.Forms.ColumnHeader
        Me.cboAuthorityPicker = New System.Windows.Forms.ComboBox
        Me.lblAnnAuth = New System.Windows.Forms.Label
        Me.lvwSelectedFiles = New System.Windows.Forms.ListView
        Me.colUpFileName = New System.Windows.Forms.ColumnHeader
        Me.colSelOrganism = New System.Windows.Forms.ColumnHeader
        Me.colAnnType = New System.Windows.Forms.ColumnHeader
        Me.colEncrypt = New System.Windows.Forms.ColumnHeader
        Me.lblSelectedFiles = New System.Windows.Forms.Label
        Me.cmdAddFile = New UIControls.ImageButton
        Me.cmdRemoveFile = New UIControls.ImageButton
        Me.VisualStyleProvider1 = New Skybound.VisualStyles.VisualStyleProvider
        Me.cmdPreviewFile = New System.Windows.Forms.Button
        Me.chkEncryptionEnable = New System.Windows.Forms.CheckBox
        Me.lblPassphrase = New System.Windows.Forms.Label
        Me.txtPassphrase = New System.Windows.Forms.TextBox
        Me.fraValidationOptions = New System.Windows.Forms.GroupBox
        Me.chkValidationAllowAsterisks = New System.Windows.Forms.CheckBox
        Me.chkValidationAllowDash = New System.Windows.Forms.CheckBox
        Me.fraValidationOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'expUploadFolderSelect
        '
        Me.expUploadFolderSelect.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.expUploadFolderSelect.Location = New System.Drawing.Point(10, 26)
        Me.expUploadFolderSelect.Name = "expUploadFolderSelect"
        Me.expUploadFolderSelect.Size = New System.Drawing.Size(220, 388)
        Me.expUploadFolderSelect.StartUpDirectory = ExpTreeLib.ExpTree.StartDir.Desktop
        Me.expUploadFolderSelect.TabIndex = 1
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.expUploadFolderSelect, True)
        '
        'cboOrganismSelect
        '
        Me.cboOrganismSelect.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cboOrganismSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOrganismSelect.Location = New System.Drawing.Point(6, 442)
        Me.cboOrganismSelect.Name = "cboOrganismSelect"
        Me.cboOrganismSelect.Size = New System.Drawing.Size(260, 21)
        Me.cboOrganismSelect.TabIndex = 10
        '
        'lblBatchUploadTree
        '
        Me.lblBatchUploadTree.Location = New System.Drawing.Point(10, 10)
        Me.lblBatchUploadTree.Name = "lblBatchUploadTree"
        Me.lblBatchUploadTree.Size = New System.Drawing.Size(186, 16)
        Me.lblBatchUploadTree.TabIndex = 0
        Me.lblBatchUploadTree.Text = "Select Source Folder for Upload"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblBatchUploadTree, True)
        '
        'lblOrganismSelect
        '
        Me.lblOrganismSelect.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblOrganismSelect.Location = New System.Drawing.Point(6, 426)
        Me.lblOrganismSelect.Name = "lblOrganismSelect"
        Me.lblOrganismSelect.Size = New System.Drawing.Size(186, 16)
        Me.lblOrganismSelect.TabIndex = 9
        Me.lblOrganismSelect.Text = "Select Destination Organism"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblOrganismSelect, True)
        '
        'lblFolderContents
        '
        Me.lblFolderContents.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFolderContents.Location = New System.Drawing.Point(244, 10)
        Me.lblFolderContents.Name = "lblFolderContents"
        Me.lblFolderContents.Size = New System.Drawing.Size(596, 16)
        Me.lblFolderContents.TabIndex = 2
        Me.lblFolderContents.Text = "Selected Folder Contents"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblFolderContents, True)
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(780, 490)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(60, 22)
        Me.cmdCancel.TabIndex = 18
        Me.cmdCancel.Text = "Cancel"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdCancel, True)
        '
        'cmdUploadChecked
        '
        Me.cmdUploadChecked.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdUploadChecked.Location = New System.Drawing.Point(650, 490)
        Me.cmdUploadChecked.Name = "cmdUploadChecked"
        Me.cmdUploadChecked.Size = New System.Drawing.Size(120, 22)
        Me.cmdUploadChecked.TabIndex = 17
        Me.cmdUploadChecked.Text = "&Upload Selected List"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdUploadChecked, True)
        '
        'lvwFolderContents
        '
        Me.lvwFolderContents.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwFolderContents.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colFileName, Me.colFileModDate, Me.colFileSize, Me.colCollectionExists})
        Me.lvwFolderContents.FullRowSelect = True
        Me.lvwFolderContents.GridLines = True
        Me.lvwFolderContents.HideSelection = False
        Me.lvwFolderContents.Location = New System.Drawing.Point(244, 26)
        Me.lvwFolderContents.Name = "lvwFolderContents"
        Me.lvwFolderContents.Size = New System.Drawing.Size(602, 134)
        Me.lvwFolderContents.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwFolderContents.TabIndex = 3
        Me.lvwFolderContents.View = System.Windows.Forms.View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "Name"
        Me.colFileName.Width = 335
        '
        'colFileModDate
        '
        Me.colFileModDate.Text = "Date Modified"
        Me.colFileModDate.Width = 121
        '
        'colFileSize
        '
        Me.colFileSize.Text = "Size"
        Me.colFileSize.Width = 67
        '
        'colCollectionExists
        '
        Me.colCollectionExists.Text = "Uploaded?"
        Me.colCollectionExists.Width = 63
        '
        'cboAuthorityPicker
        '
        Me.cboAuthorityPicker.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cboAuthorityPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAuthorityPicker.Location = New System.Drawing.Point(278, 442)
        Me.cboAuthorityPicker.Name = "cboAuthorityPicker"
        Me.cboAuthorityPicker.Size = New System.Drawing.Size(260, 21)
        Me.cboAuthorityPicker.TabIndex = 12
        '
        'lblAnnAuth
        '
        Me.lblAnnAuth.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblAnnAuth.Location = New System.Drawing.Point(278, 426)
        Me.lblAnnAuth.Name = "lblAnnAuth"
        Me.lblAnnAuth.Size = New System.Drawing.Size(204, 16)
        Me.lblAnnAuth.TabIndex = 11
        Me.lblAnnAuth.Text = "Select Annotation Type"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblAnnAuth, True)
        '
        'lvwSelectedFiles
        '
        Me.lvwSelectedFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwSelectedFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colUpFileName, Me.colSelOrganism, Me.colAnnType, Me.colEncrypt})
        Me.lvwSelectedFiles.FullRowSelect = True
        Me.lvwSelectedFiles.GridLines = True
        Me.lvwSelectedFiles.HideSelection = False
        Me.lvwSelectedFiles.Location = New System.Drawing.Point(244, 230)
        Me.lvwSelectedFiles.Name = "lvwSelectedFiles"
        Me.lvwSelectedFiles.Size = New System.Drawing.Size(602, 180)
        Me.lvwSelectedFiles.TabIndex = 8
        Me.lvwSelectedFiles.View = System.Windows.Forms.View.Details
        '
        'colUpFileName
        '
        Me.colUpFileName.Text = "Name"
        Me.colUpFileName.Width = 251
        '
        'colSelOrganism
        '
        Me.colSelOrganism.Text = "Selected Organism"
        Me.colSelOrganism.Width = 141
        '
        'colAnnType
        '
        Me.colAnnType.Text = "Annotation Type"
        Me.colAnnType.Width = 105
        '
        'colEncrypt
        '
        Me.colEncrypt.Text = "Encrypt?"
        '
        'lblSelectedFiles
        '
        Me.lblSelectedFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblSelectedFiles.Location = New System.Drawing.Point(244, 204)
        Me.lblSelectedFiles.Name = "lblSelectedFiles"
        Me.lblSelectedFiles.Size = New System.Drawing.Size(620, 16)
        Me.lblSelectedFiles.TabIndex = 4
        Me.lblSelectedFiles.Text = "FASTA Files Selected for Upload"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblSelectedFiles, True)
        '
        'cmdAddFile
        '
        Me.cmdAddFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdAddFile.GenerateDisabledImage = True
        Me.cmdAddFile.Image = CType(resources.GetObject("cmdAddFile.Image"), System.Drawing.Image)
        Me.cmdAddFile.Location = New System.Drawing.Point(486, 178)
        Me.cmdAddFile.Name = "cmdAddFile"
        Me.cmdAddFile.Size = New System.Drawing.Size(34, 36)
        Me.cmdAddFile.TabIndex = 5
        Me.cmdAddFile.ThemedImage = CType(resources.GetObject("cmdAddFile.ThemedImage"), System.Drawing.Bitmap)
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdAddFile, False)
        '
        'cmdRemoveFile
        '
        Me.cmdRemoveFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdRemoveFile.GenerateDisabledImage = True
        Me.cmdRemoveFile.Image = CType(resources.GetObject("cmdRemoveFile.Image"), System.Drawing.Image)
        Me.cmdRemoveFile.Location = New System.Drawing.Point(536, 178)
        Me.cmdRemoveFile.Name = "cmdRemoveFile"
        Me.cmdRemoveFile.Size = New System.Drawing.Size(34, 36)
        Me.cmdRemoveFile.TabIndex = 6
        Me.cmdRemoveFile.ThemedImage = CType(resources.GetObject("cmdRemoveFile.ThemedImage"), System.Drawing.Bitmap)
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdRemoveFile, False)
        '
        'cmdPreviewFile
        '
        Me.cmdPreviewFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPreviewFile.Enabled = False
        Me.cmdPreviewFile.Location = New System.Drawing.Point(716, 184)
        Me.cmdPreviewFile.Name = "cmdPreviewFile"
        Me.cmdPreviewFile.Size = New System.Drawing.Size(130, 22)
        Me.cmdPreviewFile.TabIndex = 7
        Me.cmdPreviewFile.Text = "&Preview Selected File"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdPreviewFile, True)
        '
        'chkEncryptionEnable
        '
        Me.chkEncryptionEnable.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkEncryptionEnable.Location = New System.Drawing.Point(720, 440)
        Me.chkEncryptionEnable.Name = "chkEncryptionEnable"
        Me.chkEncryptionEnable.Size = New System.Drawing.Size(124, 26)
        Me.chkEncryptionEnable.TabIndex = 15
        Me.chkEncryptionEnable.Text = "Encrypt Sequences?"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.chkEncryptionEnable, True)
        '
        'lblPassphrase
        '
        Me.lblPassphrase.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblPassphrase.Location = New System.Drawing.Point(550, 426)
        Me.lblPassphrase.Name = "lblPassphrase"
        Me.lblPassphrase.Size = New System.Drawing.Size(204, 16)
        Me.lblPassphrase.TabIndex = 13
        Me.lblPassphrase.Text = "Encryption Passphrase"
        Me.lblPassphrase.Visible = False
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblPassphrase, True)
        '
        'txtPassphrase
        '
        Me.txtPassphrase.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPassphrase.Location = New System.Drawing.Point(550, 442)
        Me.txtPassphrase.Name = "txtPassphrase"
        Me.txtPassphrase.PasswordChar = Microsoft.VisualBasic.ChrW(8226)
        Me.txtPassphrase.Size = New System.Drawing.Size(158, 21)
        Me.txtPassphrase.TabIndex = 14
        Me.txtPassphrase.Text = ""
        Me.txtPassphrase.Visible = False
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtPassphrase, True)
        '
        'fraValidationOptions
        '
        Me.fraValidationOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.fraValidationOptions.Controls.Add(Me.chkValidationAllowAsterisks)
        Me.fraValidationOptions.Controls.Add(Me.chkValidationAllowDash)
        Me.fraValidationOptions.Location = New System.Drawing.Point(4, 474)
        Me.fraValidationOptions.Name = "fraValidationOptions"
        Me.fraValidationOptions.Size = New System.Drawing.Size(336, 48)
        Me.fraValidationOptions.TabIndex = 16
        Me.fraValidationOptions.TabStop = False
        Me.fraValidationOptions.Text = "Fasta Validation Options"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.fraValidationOptions, True)
        '
        'chkValidationAllowAsterisks
        '
        Me.chkValidationAllowAsterisks.Location = New System.Drawing.Point(8, 16)
        Me.chkValidationAllowAsterisks.Name = "chkValidationAllowAsterisks"
        Me.chkValidationAllowAsterisks.Size = New System.Drawing.Size(156, 20)
        Me.chkValidationAllowAsterisks.TabIndex = 2
        Me.chkValidationAllowAsterisks.Text = "Allow asterisks in residues"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.chkValidationAllowAsterisks, True)
        '
        'chkValidationAllowDash
        '
        Me.chkValidationAllowDash.Location = New System.Drawing.Point(172, 16)
        Me.chkValidationAllowDash.Name = "chkValidationAllowDash"
        Me.chkValidationAllowDash.Size = New System.Drawing.Size(156, 20)
        Me.chkValidationAllowDash.TabIndex = 3
        Me.chkValidationAllowDash.Text = "Allow dash in residues"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.chkValidationAllowDash, True)
        '
        'frmBatchAddNewCollectionTest
        '
        Me.AcceptButton = Me.cmdUploadChecked
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(856, 536)
        Me.Controls.Add(Me.fraValidationOptions)
        Me.Controls.Add(Me.txtPassphrase)
        Me.Controls.Add(Me.lblPassphrase)
        Me.Controls.Add(Me.chkEncryptionEnable)
        Me.Controls.Add(Me.cmdPreviewFile)
        Me.Controls.Add(Me.cmdRemoveFile)
        Me.Controls.Add(Me.cmdAddFile)
        Me.Controls.Add(Me.lvwSelectedFiles)
        Me.Controls.Add(Me.lvwFolderContents)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.lblOrganismSelect)
        Me.Controls.Add(Me.cboOrganismSelect)
        Me.Controls.Add(Me.expUploadFolderSelect)
        Me.Controls.Add(Me.lblBatchUploadTree)
        Me.Controls.Add(Me.lblFolderContents)
        Me.Controls.Add(Me.cmdUploadChecked)
        Me.Controls.Add(Me.cboAuthorityPicker)
        Me.Controls.Add(Me.lblAnnAuth)
        Me.Controls.Add(Me.lblSelectedFiles)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(800, 474)
        Me.Name = "frmBatchAddNewCollectionTest"
        Me.Text = "Batch Upload FASTA Files"
        Me.fraValidationOptions.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region
    Private m_CurrentDirectory As String
    Private m_LastUsedDirectory As String
    Private m_LastSelectedOrganism As String = ""
    Private m_LastSelectedAnnotationType As String = ""
    Private m_FileList As Hashtable
    Private m_CheckedFileList As Hashtable
    Private m_SelectedFileList As Hashtable
    Private m_OrganismList As DataTable
    Private m_OrganismListSorted As DataView

    Private m_AuthorityList As DataTable
    Private m_CollectionsTable As DataTable
    Private m_CollectionsList As Hashtable
    Private m_SelectedOrganismID As Integer
    Private m_SelectedAuthorityID As Integer
    Private m_SelectedFileForPreview As String
    Private m_PSConnectionString As String
    Private m_ReallyClose As Boolean = False

    Private WithEvents m_FilePreviewer As clsFilePreviewHandler
    Private m_PreviewFormStatus As Boolean

    Private m_PassphraseList As Hashtable
    Private m_CachedPassphrase As String

    Const ADD_FILES_MESSAGE As String = "You must first select an organism and annotation type, and then select one or more protein collections."
    Private m_AllowAddFiles As Boolean
    Private m_AllowAddFilesMessage As String = ADD_FILES_MESSAGE

#Region " Properties "
    ReadOnly Property FileList() As Hashtable
        Get
            Return Me.m_CheckedFileList
        End Get
    End Property

    ReadOnly Property SelectedOrganismID() As Integer
        Get
            Return Me.m_SelectedOrganismID
        End Get
    End Property

    ReadOnly Property SelectedAuthorityID() As Integer
        Get
            Return Me.m_SelectedAuthorityID
        End Get
    End Property

    Property CurrentDirectory() As String
        Get
            Return Me.m_LastUsedDirectory
        End Get
        Set(ByVal Value As String)
            Me.m_LastUsedDirectory = Value
        End Set
    End Property

    Property SelectedOrganismName() As String
        Get
            If Me.cboOrganismSelect.Items.Count > 0 Then
                Return Me.cboOrganismSelect.Text
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal Value As String)
            m_LastSelectedOrganism = Value
        End Set
    End Property

    Property SelectedAnnotationType() As String
        Get
            If Me.cboAuthorityPicker.Items.Count > 0 Then
                Return Me.cboAuthorityPicker.Text
            Else
                Return String.Empty
            End If
        End Get
        Set(ByVal Value As String)
            m_LastSelectedAnnotationType = Value
        End Set
    End Property

    Property ValidationAllowAsterisks() As Boolean
        Get
            Return chkValidationAllowAsterisks.Checked
        End Get
        Set(ByVal Value As Boolean)
            chkValidationAllowAsterisks.Checked = Value
        End Set
    End Property

    Property ValidationAllowDash() As Boolean
        Get
            Return chkValidationAllowDash.Checked
        End Get
        Set(ByVal Value As Boolean)
            chkValidationAllowDash.Checked = Value
        End Set
    End Property


#End Region

    Private Sub frmBatchAddNewCollection_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim di As System.IO.DirectoryInfo
        Me.expUploadFolderSelect.StartUpDirectory = ExpTree.StartDir.Desktop

        If Not Me.m_LastUsedDirectory Is Nothing Then
            di = New System.IO.DirectoryInfo(Me.m_LastUsedDirectory)
            If di.Exists Then
                Me.expUploadFolderSelect.ExpandANode(Me.m_LastUsedDirectory)
            End If
        End If

        If Me.m_FileList Is Nothing Then
            Me.m_FileList = New Hashtable
        End If

        Me.m_CheckedFileList = New Hashtable
        Me.LoadOrganismPicker(Me.cboOrganismSelect, Me.m_OrganismListSorted)
        Me.LoadAuthorityPicker(Me.cboAuthorityPicker, Me.m_AuthorityList)
        Me.cmdUploadChecked.Enabled = False
        Me.cmdAddFile.Enabled = True
        Me.cmdRemoveFile.Enabled = False

        Me.m_CollectionsList = Me.CollectionsTableToHash(Me.m_CollectionsTable)

        SelectComboBoxItemByName(cboOrganismSelect, m_LastSelectedOrganism, 2)
        SelectComboBoxItemByName(cboAuthorityPicker, m_LastSelectedAnnotationType, 1)

        SystemImageListManager.SetListViewImageList(Me.lvwFolderContents, True, False)
        SystemImageListManager.SetListViewImageList(Me.lvwFolderContents, False, False)

    End Sub

    Private Sub AfterNodeSelect( _
        ByVal pathName As String, _
        ByVal CSI As CShItem) Handles expUploadFolderSelect.ExpTreeNodeSelected

        Dim dirList As New ArrayList

        Dim totalItems As Integer

        dirList = CSI.GetDirectories

        totalItems = dirList.Count
        Me.m_LastUsedDirectory = pathName

        If totalItems > 0 Then         
            dirList.Sort()
        End If
        If pathName <> CShItem.strMyComputer And pathName <> CShItem.strSystemFolder And pathName <> "Desktop" Then
            Me.ScanDirectory(pathName, Me.lvwFolderContents)
            Me.lblFolderContents.Text = "Results Files In: '" & pathName & "'"
        Else
            Me.lblFolderContents.Text = "Results Files In..."
        End If


    End Sub

#Region " Directory Loading "

    Private Function CollectionsTableToHash(ByVal dt As DataTable) As Hashtable
        Dim ht As New Hashtable(dt.Rows.Count)
        Dim dr As DataRow
        Dim foundrows() As DataRow = dt.Select("", "Protein_Collection_ID")
        Dim tmpID As Integer
        Dim tmpName As String

        For Each dr In foundrows
            tmpID = DirectCast(dr.Item("Protein_Collection_ID"), Int32)
            tmpName = dr.Item("FileName").ToString
            If Not ht.Contains(tmpID) Then
                ht.Add(tmpID, tmpName)
            End If
            'ht.Add(CInt(dr.Item("Protein_Collection_ID")), dr.Item("FileName").ToString)
        Next

        Return ht

    End Function

    Private Sub ScanDirectory(ByVal DirectoryPath As String, ByVal lvw As System.Windows.Forms.ListView)

        Dim di As New System.IO.DirectoryInfo(DirectoryPath)
        Dim fi As System.IO.FileInfo
        Dim foundFASTAFiles() As System.IO.FileInfo

        Dim tmpParsedType As String


        'Dim outArrayList As New ArrayList

        If di.Exists Then
            foundFASTAFiles = di.GetFiles()
        Else
            Exit Sub
        End If

        If Not Me.m_FileList Is Nothing Then
            Me.m_FileList.Clear()
        Else
            Me.m_FileList = New Hashtable
        End If

        For Each fi In foundFASTAFiles
            tmpParsedType = System.IO.Path.GetExtension(fi.Name)

            Select Case tmpParsedType
                Case ".fasta", ".fst", ".fa", ".pep"
                    Me.m_FileList.Add(fi.FullName, fi)
            End Select
        Next

        Me.LoadListView(Me.lvwFolderContents)

    End Sub
#End Region

#Region " UI Loading Functions "

    Private Sub LoadListView(ByVal lvw As ListView)
        Dim fi As System.IO.FileInfo
        Dim li As ListViewItem
        Dim tmpName As String
        lvw.BeginUpdate()

        lvw.Items.Clear()

        If Me.m_CollectionsList Is Nothing Then
            Me.m_CollectionsList = New Hashtable
        End If

        For Each fi In Me.m_FileList.Values
            tmpName = System.IO.Path.GetFileNameWithoutExtension(fi.Name)
            li = New ListViewItem
            li.Text = fi.Name
            li.SubItems.Add(Format(fi.LastWriteTime, "g"))
            li.SubItems.Add(Numeric2Bytes(CDbl(fi.Length)))
            If Me.m_CollectionsList.ContainsValue(tmpName) Then
                li.SubItems.Add("Yes")
            Else
                li.SubItems.Add("No")
            End If
            li.SubItems.Add(fi.FullName)
            lvw.Items.Add(li)
        Next
        lvw.EndUpdate()
    End Sub

    Private Sub LoadOrganismPicker(ByVal cbo As ComboBox, ByVal orgList As DataView)
        RemoveHandler cboOrganismSelect.SelectedIndexChanged, AddressOf cboOrganismSelect_SelectedIndexChanged
        With cbo
            .DataSource = orgList
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
        End With

        AddHandler cboOrganismSelect.SelectedIndexChanged, AddressOf cboOrganismSelect_SelectedIndexChanged
    End Sub

    Private Sub LoadAuthorityPicker(ByVal cbo As ComboBox, ByVal authList As DataTable)
        RemoveHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged

        cbo.BeginUpdate()
        Dim dr As DataRow = authList.NewRow

        With dr
            .Item("ID") = -2
            .Item("Display_Name") = "Add New Annotation Type..."
            .Item("Details") = "Brings up a dialog box to allow adding a naming authority to the list"
        End With



        Dim pk1(0) As DataColumn
        pk1(0) = authList.Columns("ID")
        authList.PrimaryKey = pk1

        If authList.Rows.Contains(dr.Item("ID")) Then
            Dim rdr As DataRow = authList.Rows.Find(dr.Item("ID"))
            authList.Rows.Remove(rdr)
        End If

        authList.Rows.Add(dr)
        authList.AcceptChanges()

        With cbo
            .DataSource = authList
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
        End With
        cbo.EndUpdate()

        AddHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged
    End Sub

#End Region

#Region " Internal Service Functions "

    Function Numeric2Bytes(ByVal b As Double) As String
        Dim bSize(8) As String
        Dim i As Integer

        bSize(0) = "Bytes"
        bSize(1) = "KB" 'Kilobytes
        bSize(2) = "MB" 'Megabytes
        bSize(3) = "GB" 'Gigabytes
        bSize(4) = "TB" 'Terabytes
        bSize(5) = "PB" 'Petabytes
        bSize(6) = "EB" 'Exabytes
        bSize(7) = "ZB" 'Zettabytes
        bSize(8) = "YB" 'Yottabytes

        'b = CDbl(b) ' Make sure var is a Double (not just
        ' variant)
        For i = UBound(bSize) To 0 Step -1
            If b >= (1024 ^ i) Then
                Return ThreeNonZeroDigits(b / (1024 ^ i)) & " " & bSize(i)
                Exit For
            End If
        Next

        Return b.ToString() & " Bytes"

    End Function

    ' Return the value formatted to include at most three
    ' non-zero digits and at most two digits after the
    ' decimal point. Examples:
    '         1
    '       123
    '        12.3
    '         1.23
    '         0.12
    Private Function ThreeNonZeroDigits(ByVal value As Double) _
        As String
        If value >= 100 Then
            ' No digits after the decimal.
            Return Format$(CInt(value))
        ElseIf value >= 10 Then
            ' One digit after the decimal.
            Return Format$(value, "0.0")
        Else
            ' Two digits after the decimal.
            Return Format$(value, "0.00")
        End If
    End Function

#End Region

    Private Sub MakeCheckedFileList()
        Dim upInfo As Protein_Uploader.IUploadProteins.UploadInfo
        Dim fi As System.IO.FileInfo
        Dim li As ListViewItem

        Dim tmpNameList As New ArrayList


        For Each li In Me.lvwSelectedFiles.Items
            tmpNameList.Add(li.SubItems(4).Text)
        Next

        Dim counter As IDictionaryEnumerator = Me.m_SelectedFileList.GetEnumerator

        While counter.MoveNext = True
            upInfo = DirectCast(counter.Value, Protein_Uploader.IUploadProteins.UploadInfo)
            fi = upInfo.FileInformation

            If tmpNameList.Contains(fi.FullName) Then
                Me.m_CheckedFileList.Add(fi.FullName, upInfo)
            End If
        End While

    End Sub

    Private Sub AddFileToSelectedList()

        Dim li As ListViewItem
        Dim si As ListViewItem
        Dim newLi As ListViewItem
        Dim upInfo As Protein_Uploader.IUploadProteins.UploadInfo

        Try
            If Me.m_SelectedFileList Is Nothing Then
                Me.m_SelectedFileList = New Hashtable
            End If

            For Each li In Me.lvwFolderContents.SelectedItems
                upInfo.FileInformation = DirectCast(Me.m_FileList.Item(li.SubItems(4).Text), System.IO.FileInfo)
                upInfo.OrganismID = DirectCast(Me.cboOrganismSelect.SelectedValue, Int32)
                upInfo.AuthorityID = DirectCast(Me.cboAuthorityPicker.SelectedValue, Int32)
                upInfo.EncryptSequences = False
                upInfo.EncryptionPassphrase = ""
                If Me.m_SelectedFileList.Contains(upInfo.FileInformation.FullName) Then
                    Me.m_SelectedFileList.Remove(upInfo.FileInformation.FullName)
                    For Each si In Me.lvwSelectedFiles.Items
                        If si.Text = System.IO.Path.GetFileNameWithoutExtension(upInfo.FileInformation.Name) Then
                            Me.lvwSelectedFiles.Items.Remove(si)
                        End If
                    Next
                End If

                'Me.m_SelectedFileList.Add(upInfo.FileInformation.FullName, upInfo)
                newLi = New ListViewItem(System.IO.Path.GetFileNameWithoutExtension(upInfo.FileInformation.Name))
                With newLi
                    .SubItems.Add(Me.cboOrganismSelect.Text)
                    .SubItems.Add(Me.cboAuthorityPicker.Text)
                    If Me.chkEncryptionEnable.Checked Then
                        .SubItems.Add("Yes")
                        .Tag = Me.txtPassphrase.Text
                        upInfo.EncryptSequences = True
                        upInfo.EncryptionPassphrase = newLi.Tag.ToString
                    Else
                        .SubItems.Add("No")
                    End If
                    .SubItems.Add(upInfo.FileInformation.FullName)
                End With
                Me.lvwSelectedFiles.Items.Add(newLi)
                Me.m_SelectedFileList.Add(upInfo.FileInformation.FullName, upInfo)
            Next
        Catch ex As Exception
            System.Windows.Forms.MessageBox.Show("Error in AddFileToSelectedList: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try


    End Sub

    Private Sub RemoveFileFromSelectedList()
        Dim li As ListViewItem

        For Each li In Me.lvwSelectedFiles.SelectedItems
            Me.lvwSelectedFiles.Items.Remove(li)
            If Me.m_SelectedFileList.ContainsKey(li.SubItems(3).ToString) Then
                Me.m_SelectedFileList.Remove(li.SubItems(3).ToString)
            End If
        Next
    End Sub

    Private Sub SelectComboBoxItemByName(ByRef objComboBox As System.Windows.Forms.ComboBox, ByVal strValue As String, ByVal intDataColumnIndexToCheck As Integer)
        ' Look for strValue in a combobox that has a data table attached via the .DataSource property
        ' If the value is found, then the given item in the combobox is selected

        Dim intIndex As Integer
        Dim objRow As System.Data.DataRowView

        Try
            If Not strValue Is Nothing AndAlso strValue.Length > 0 Then
                For intIndex = 0 To objComboBox.Items.Count - 1
                    objRow = DirectCast(objComboBox.Items.Item(intIndex), System.Data.DataRowView)

                    If Not System.DBNull.Value.Equals(objRow.Item(intDataColumnIndexToCheck)) Then
                        If CStr(objRow.Item(intDataColumnIndexToCheck)) = strValue Then
                            objComboBox.SelectedIndex = intIndex
                            Exit For

                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            ' Ignore errors here
        End Try


    End Sub

#Region " Button and Combo Handlers "

    Private Sub cboOrganismSelect_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboOrganismSelect.SelectedIndexChanged
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)
        Dim tmpUpInfo As Protein_Uploader.IUploadProteins.UploadInfo

        Me.m_SelectedOrganismID = DirectCast(cbo.SelectedValue, Int32)
        CheckTransferEnable()
        If Me.lvwSelectedFiles.SelectedItems.Count > 0 Then
            Dim li As ListViewItem
            For Each li In Me.lvwSelectedFiles.SelectedItems
                tmpUpInfo = DirectCast(Me.m_SelectedFileList.Item(li.SubItems(4).Text), Protein_Uploader.IUploadProteins.UploadInfo)
                tmpUpInfo.OrganismID = Me.m_SelectedOrganismID
                Me.m_SelectedFileList.Item(li.SubItems(4).Text) = tmpUpInfo
                'New Protein_Uploader.IUploadProteins.UploadInfo(tmpUpInfo.FileInformation, Me.m_SelectedOrganismID, tmpUpInfo.AuthorityID)
                'Me.m_SelectedFileList.Item(li.SubItems(3).ToString) = _
                '    New Protein_Uploader.IUploadProteins.UploadInfo(tmpUpInfo.FileInformation, Me.m_SelectedOrganismID, tmpUpInfo.AuthorityID)
                li.SubItems(1).Text = cbo.Text
            Next
        End If

    End Sub

    Private Sub cboAuthorityPicker_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboAuthorityPicker.SelectedIndexChanged
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)
        Dim tmpUpInfo As Protein_Uploader.IUploadProteins.UploadInfo

        If Me.cboAuthorityPicker.SelectedValue.GetType Is System.Type.GetType("System.Int32") Then
            Me.m_SelectedAuthorityID = CInt(Me.cboAuthorityPicker.SelectedValue)
        Else
            'Me.m_SelectedAuthorityID = 0
        End If

        CheckTransferEnable()

        Dim tmpAnnTypeID As Integer

        If Me.m_SelectedAuthorityID = -2 Then
            'Bring up addition dialog
            Dim AnnTypeAdd As New clsAddAnnotationType(Me.m_PSConnectionString)
            AnnTypeAdd.FormLocation = New System.Drawing.Point(Me.Left + Me.Width + 10, Me.Top)
            tmpAnnTypeID = AnnTypeAdd.AddAnnotationType
            'Dim AuthAdd As New clsAddNamingAuthority(Me.m_PSConnectionString)
            'tmpAuthID = AuthAdd.AddNamingAuthority

            If Not AnnTypeAdd.EntryExists And tmpAnnTypeID > 0 Then

                Dim dr As DataRow
                dr = Me.m_AuthorityList.NewRow

                'Dim AuthName As String
                'AuthName = Me.m_AuthorityList

                With dr
                    .Item("ID") = tmpAnnTypeID
                    .Item("Display_Name") = AnnTypeAdd.DisplayName
                    .Item("Details") = AnnTypeAdd.Description
                End With

                Me.m_AuthorityList.Rows.Add(dr)
                Me.m_AuthorityList.AcceptChanges()
                Me.LoadAuthorityPicker(Me.cboAuthorityPicker, Me.m_AuthorityList)
                Me.m_SelectedAuthorityID = tmpAnnTypeID

                Me.cboAuthorityPicker.SelectedValue = tmpAnnTypeID
            End If
            AnnTypeAdd = Nothing

        End If


        If Me.lvwSelectedFiles.SelectedItems.Count > 0 Then
            Dim li As ListViewItem
            For Each li In Me.lvwSelectedFiles.SelectedItems
                tmpUpInfo = DirectCast(Me.m_SelectedFileList.Item(li.SubItems(4).Text), Protein_Uploader.IUploadProteins.UploadInfo)
                Me.m_SelectedFileList.Item(li.SubItems(4).Text) = _
                    New Protein_Uploader.IUploadProteins.UploadInfo(tmpUpInfo.FileInformation, Me.m_SelectedOrganismID, Me.m_SelectedAuthorityID) 'tmpUpInfo.AuthorityID)
                li.SubItems(2).Text = cbo.Text
            Next
        End If

    End Sub

    Private Sub chkEncryptionEnable_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkEncryptionEnable.CheckedChanged
        Dim chk As CheckBox = DirectCast(sender, CheckBox)
        Dim encryptSequences As Boolean = False

        If chk.CheckState = CheckState.Checked Then
            Me.txtPassphrase.Visible = True
        Else
            Me.txtPassphrase.Visible = False
        End If

        Me.CheckTransferEnable()

        'Dim tmpUpInfo As Protein_Uploader.IUploadProteins.UploadInfo
        'If Me.lvwSelectedFiles.SelectedItems.Count > 0 Then
        '    Dim li As ListViewItem
        '    For Each li In Me.lvwSelectedFiles.SelectedItems
        '        tmpUpInfo = DirectCast(Me.m_SelectedFileList.Item(li.SubItems(4).Text), Protein_Uploader.IUploadProteins.UploadInfo)
        '        If encryptSequences Then
        '            tmpUpInfo.EncryptSequences = True
        '            tmpUpInfo.EncryptionPassphrase = passPhraseForm.Passphrase
        '            li.SubItems(3).Text = "Yes"
        '        Else
        '            tmpUpInfo.EncryptSequences = False
        '            tmpUpInfo.EncryptionPassphrase = Nothing
        '            li.SubItems(3).Text = "No"
        '        End If
        '        Me.m_SelectedFileList.Item(li.SubItems(4).Text) = tmpUpInfo
        '    Next
        'End If

    End Sub


    Private Sub CheckTransferEnable()
        If Me.chkEncryptionEnable.Checked = True Then
            If Me.m_SelectedOrganismID > 0 And _
                Me.m_SelectedAuthorityID > 0 And _
                Me.lvwFolderContents.SelectedItems.Count > 0 And _
                Me.txtPassphrase.Text.Length > 0 Then

                m_AllowAddFiles = True
                m_AllowAddFilesMessage = ""
            Else
                m_AllowAddFiles = False
                m_AllowAddFilesMessage = ADD_FILES_MESSAGE & "  You also must define a passphrase for encryption."
            End If
        Else
            If Me.m_SelectedOrganismID > 0 And Me.m_SelectedAuthorityID > 0 And Me.lvwFolderContents.SelectedItems.Count > 0 Then
                m_AllowAddFiles = True
                m_AllowAddFilesMessage = ""
            Else
                m_AllowAddFiles = False
                m_AllowAddFilesMessage = ADD_FILES_MESSAGE
            End If
        End If

        If Me.lvwSelectedFiles.Items.Count > 0 Then
            Me.cmdRemoveFile.Enabled = True
        Else
            Me.cmdRemoveFile.Enabled = False
        End If

        If Me.lvwSelectedFiles.Items.Count > 0 Then
            Me.cmdUploadChecked.Enabled = True
        Else
            Me.cmdUploadChecked.Enabled = False
        End If

        If Me.lvwFolderContents.SelectedItems.Count > 0 Then
            Me.cmdPreviewFile.Enabled = True
        Else
            Me.cmdPreviewFile.Enabled = False
        End If

    End Sub

    Private Sub cmdUploadChecked_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUploadChecked.Click
        Me.MakeCheckedFileList()
        Me.m_ReallyClose = True
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub cmdAddFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddFile.Click
        If Not m_AllowAddFiles Then
            System.Windows.Forms.MessageBox.Show(m_AllowAddFilesMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            Me.AddFileToSelectedList()
            CheckTransferEnable()
        End If
    End Sub

    Private Sub cmdRemoveFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdRemoveFile.Click
        Me.RemoveFileFromSelectedList()
        CheckTransferEnable()
    End Sub

    Private Sub cmdPreviewFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPreviewFile.Click
        If Me.m_FilePreviewer Is Nothing Then
            Me.m_FilePreviewer = New clsFilePreviewHandler
        End If

        Dim fullName As String = Me.lvwFolderContents.SelectedItems(0).SubItems(4).Text


        Me.m_FilePreviewer.ShowPreview(fullName, Me.Left + Me.Width + 10, Me.Top, Me.Height)

    End Sub

#End Region

    Private Sub lvwSelectedFiles_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvwSelectedFiles.Click
        Dim lvw As ListView = DirectCast(sender, ListView)
        Dim selectedAuthority As String
        Dim selectedOrganism As String

        Dim encryptSeq As Boolean

        Dim li As ListViewItem = Me.lvwSelectedFiles.SelectedItems.Item(0)

        selectedOrganism = li.SubItems(1).Text
        selectedAuthority = li.SubItems(2).Text
        If li.SubItems(3).Text = "Yes" Then
            encryptSeq = True
            RemoveHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
            Me.chkEncryptionEnable.CheckState = CheckState.Checked
            AddHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
        Else
            encryptSeq = False
            RemoveHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
            Me.chkEncryptionEnable.CheckState = CheckState.Unchecked
            AddHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
        End If

        Me.cboAuthorityPicker.Text = selectedAuthority
        Me.cboOrganismSelect.Text = selectedOrganism

        CheckTransferEnable()

    End Sub

    Private Sub lvwFolderContents_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvwFolderContents.Click
        Me.lvwSelectedFiles.SelectedItems.Clear()
    End Sub

    Private Sub frmBatchAddNewCollectionTest_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If Me.lvwSelectedFiles.Items.Count > 0 And Not Me.m_ReallyClose Then
            Dim r As System.Windows.Forms.DialogResult

            r = MessageBox.Show("You have files selected for upload. Really close the form?", _
                "Files selected for upload", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
            If r = DialogResult.No Then
                e.Cancel = True
            Else
                e.Cancel = False
            End If

        End If
        If Not Me.m_FilePreviewer Is Nothing And Me.m_PreviewFormStatus = True Then
            Me.m_FilePreviewer.CloseForm()
            Me.m_FilePreviewer = Nothing
        End If
    End Sub


    Private Sub lvwFolderContents_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lvwFolderContents.MouseUp
        Me.CheckTransferEnable()
    End Sub

    Sub OnPreviewFormStatusChange(ByVal Visibility As Boolean) Handles m_FilePreviewer.FormStatus
        If Visibility = True Then
            Me.cmdPreviewFile.Enabled = False
            Me.m_PreviewFormStatus = True
        Else
            Me.cmdPreviewFile.Enabled = True
            Me.m_PreviewFormStatus = False
        End If
    End Sub

    Private Sub txtPassphrase_Validating(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtPassphrase.Validating
        Dim txt As TextBox = DirectCast(sender, TextBox)
        Dim li As ListViewItem

        If txt.Text.Length > 0 Then
            If Me.lvwSelectedFiles.SelectedItems.Count = 0 Then
                Me.m_CachedPassphrase = txt.Text
            ElseIf Me.lvwSelectedFiles.SelectedItems.Count > 0 Then
                For Each li In Me.lvwSelectedFiles.Items
                    li.Tag = txt.Text
                    li.SubItems(3).Text = "Yes"
                Next
            End If
            Me.CheckTransferEnable()
        Else
            Me.CheckTransferEnable()
            Exit Sub
        End If
    End Sub
End Class
