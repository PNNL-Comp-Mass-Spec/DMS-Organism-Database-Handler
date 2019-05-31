Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports Protein_Uploader
Imports Raccoom.Windows.Forms
Imports ValidateFastaFile

Public Class frmBatchAddNewCollection
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New(
        organismList As DataTable,
        annotationTypeList As DataTable,
        existingCollectionsList As DataTable,
        psConnectionString As String,
        selectedFolderPath As String,
        cachedFileDescriptions As Dictionary(Of String, KeyValuePair(Of String, String)))

        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        m_StatusResetTimer = New Timer With {
            .Interval = 1000
        }

        m_StatusResetTimer.Start()

        ClearStatus()

        'Add any initialization after the InitializeComponent() call
        m_OrganismList = organismList
        m_OrganismListSorted = New DataView(m_OrganismList) With {
            .Sort = "Display_Name"
        }

        m_AnnotationTypeList = annotationTypeList
        m_CollectionsTable = existingCollectionsList
        m_PSConnectionString = psConnectionString

        m_CachedFileDescriptions = cachedFileDescriptions

        ctlTreeViewFolderBrowser.DataSource = New TreeStrategyFolderBrowserProvider()
        ctlTreeViewFolderBrowser.CheckBoxBehaviorMode = CheckBoxBehaviorMode.None

        InitializeTreeView(selectedFolderPath)

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Friend WithEvents ctlTreeViewFolderBrowser As TreeViewFolderBrowser
    Friend WithEvents cboOrganismSelect As ComboBox
    Friend WithEvents lblBatchUploadTree As Label
    Friend WithEvents lblOrganismSelect As Label
    Friend WithEvents lblFolderContents As Label
    Friend WithEvents cmdCancel As Button
    Friend WithEvents cmdUploadChecked As Button
    Friend WithEvents lvwFolderContents As ListView
    Friend WithEvents colFileName As ColumnHeader
    Friend WithEvents colFileSize As ColumnHeader
    Friend WithEvents colFileModDate As ColumnHeader
    Friend WithEvents cboAnnotationTypePicker As ComboBox
    Friend WithEvents colCollectionExists As ColumnHeader
    Friend WithEvents colUpFileName As ColumnHeader
    Friend WithEvents colSelOrganism As ColumnHeader
    Friend WithEvents lblSelectedFiles As Label
    Friend WithEvents lvwSelectedFiles As ListView
    Friend WithEvents cmdAddFile As UIControls.ImageButton
    Friend WithEvents cmdRemoveFile As UIControls.ImageButton
    Friend WithEvents lblAnnAuth As Label
    Friend WithEvents colAnnType As ColumnHeader
    Friend WithEvents cmdPreviewFile As Button
    Friend WithEvents chkEncryptionEnable As CheckBox
    Friend WithEvents lblPassphrase As Label
    Friend WithEvents txtPassphrase As TextBox
    Friend WithEvents fraValidationOptions As GroupBox
    Friend WithEvents chkValidationAllowAsterisks As CheckBox
    Friend WithEvents cmdRefreshFiles As Button
    Friend WithEvents txtMaximumProteinNameLength As TextBox
    Friend WithEvents lblMaximumProteinNameLength As Label
    Friend WithEvents chkValidationAllowAllSymbolsInProteinNames As CheckBox
    Friend WithEvents colDescription As ColumnHeader
    Friend WithEvents colSource As ColumnHeader
    Friend WithEvents cmdUpdateDescription As Button
    Friend WithEvents lblStatus As Label
    Friend WithEvents chkValidationAllowDash As CheckBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmBatchAddNewCollection))
        Me.ctlTreeViewFolderBrowser = New Raccoom.Windows.Forms.TreeViewFolderBrowser()
        Me.cboOrganismSelect = New System.Windows.Forms.ComboBox()
        Me.lblBatchUploadTree = New System.Windows.Forms.Label()
        Me.lblOrganismSelect = New System.Windows.Forms.Label()
        Me.lblFolderContents = New System.Windows.Forms.Label()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdUploadChecked = New System.Windows.Forms.Button()
        Me.lvwFolderContents = New System.Windows.Forms.ListView()
        Me.colFileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colFileModDate = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colFileSize = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCollectionExists = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.cboAnnotationTypePicker = New System.Windows.Forms.ComboBox()
        Me.lblAnnAuth = New System.Windows.Forms.Label()
        Me.lvwSelectedFiles = New System.Windows.Forms.ListView()
        Me.colUpFileName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSelOrganism = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDescription = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSource = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAnnType = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblSelectedFiles = New System.Windows.Forms.Label()
        Me.cmdAddFile = New UIControls.ImageButton()
        Me.cmdRemoveFile = New UIControls.ImageButton()
        Me.cmdPreviewFile = New System.Windows.Forms.Button()
        Me.chkEncryptionEnable = New System.Windows.Forms.CheckBox()
        Me.lblPassphrase = New System.Windows.Forms.Label()
        Me.txtPassphrase = New System.Windows.Forms.TextBox()
        Me.fraValidationOptions = New System.Windows.Forms.GroupBox()
        Me.chkValidationAllowAllSymbolsInProteinNames = New System.Windows.Forms.CheckBox()
        Me.txtMaximumProteinNameLength = New System.Windows.Forms.TextBox()
        Me.lblMaximumProteinNameLength = New System.Windows.Forms.Label()
        Me.chkValidationAllowAsterisks = New System.Windows.Forms.CheckBox()
        Me.chkValidationAllowDash = New System.Windows.Forms.CheckBox()
        Me.cmdRefreshFiles = New System.Windows.Forms.Button()
        Me.cmdUpdateDescription = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.fraValidationOptions.SuspendLayout()
        Me.SuspendLayout()
        '
        'ctlTreeViewFolderBrowser
        '
        Me.ctlTreeViewFolderBrowser.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ctlTreeViewFolderBrowser.DataSource = Nothing
        Me.ctlTreeViewFolderBrowser.HideSelection = False
        Me.ctlTreeViewFolderBrowser.Location = New System.Drawing.Point(10, 35)
        Me.ctlTreeViewFolderBrowser.Name = "treeViewFolderBrowser1"
        Me.ctlTreeViewFolderBrowser.ShowLines = False
        Me.ctlTreeViewFolderBrowser.ShowRootLines = False
        Me.ctlTreeViewFolderBrowser.Size = New System.Drawing.Size(326, 524)
        Me.ctlTreeViewFolderBrowser.TabIndex = 0
        '
        'cboOrganismSelect
        '
        Me.cboOrganismSelect.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboOrganismSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOrganismSelect.Location = New System.Drawing.Point(8, 587)
        Me.cboOrganismSelect.Name = "cboOrganismSelect"
        Me.cboOrganismSelect.Size = New System.Drawing.Size(609, 25)
        Me.cboOrganismSelect.TabIndex = 11
        '
        'lblBatchUploadTree
        '
        Me.lblBatchUploadTree.Location = New System.Drawing.Point(14, 12)
        Me.lblBatchUploadTree.Name = "lblBatchUploadTree"
        Me.lblBatchUploadTree.Size = New System.Drawing.Size(320, 20)
        Me.lblBatchUploadTree.TabIndex = 0
        Me.lblBatchUploadTree.Text = "Select source folder for upload (F5 to refresh)"
        '
        'lblOrganismSelect
        '
        Me.lblOrganismSelect.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblOrganismSelect.Location = New System.Drawing.Point(8, 567)
        Me.lblOrganismSelect.Name = "lblOrganismSelect"
        Me.lblOrganismSelect.Size = New System.Drawing.Size(261, 20)
        Me.lblOrganismSelect.TabIndex = 10
        Me.lblOrganismSelect.Text = "Select destination &organism"
        '
        'lblFolderContents
        '
        Me.lblFolderContents.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFolderContents.Location = New System.Drawing.Point(342, 12)
        Me.lblFolderContents.Name = "lblFolderContents"
        Me.lblFolderContents.Size = New System.Drawing.Size(835, 20)
        Me.lblFolderContents.TabIndex = 2
        Me.lblFolderContents.Text = "Selected folder contents"
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(1101, 653)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(84, 36)
        Me.cmdCancel.TabIndex = 20
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdUploadChecked
        '
        Me.cmdUploadChecked.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdUploadChecked.Location = New System.Drawing.Point(918, 653)
        Me.cmdUploadChecked.Name = "cmdUploadChecked"
        Me.cmdUploadChecked.Size = New System.Drawing.Size(168, 36)
        Me.cmdUploadChecked.TabIndex = 19
        Me.cmdUploadChecked.Text = "&Upload new FASTAs"
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
        Me.lvwFolderContents.Location = New System.Drawing.Point(342, 32)
        Me.lvwFolderContents.Name = "lvwFolderContents"
        Me.lvwFolderContents.Size = New System.Drawing.Size(843, 218)
        Me.lvwFolderContents.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwFolderContents.TabIndex = 3
        Me.lvwFolderContents.UseCompatibleStateImageBehavior = False
        Me.lvwFolderContents.View = System.Windows.Forms.View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "Name"
        Me.colFileName.Width = 450
        '
        'colFileModDate
        '
        Me.colFileModDate.Text = "Date Modified"
        Me.colFileModDate.Width = 140
        '
        'colFileSize
        '
        Me.colFileSize.Text = "Size"
        Me.colFileSize.Width = 67
        '
        'colCollectionExists
        '
        Me.colCollectionExists.Text = "Existing Collection?"
        Me.colCollectionExists.Width = 150
        '
        'cboAnnotationTypePicker
        '
        Me.cboAnnotationTypePicker.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboAnnotationTypePicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAnnotationTypePicker.Location = New System.Drawing.Point(641, 587)
        Me.cboAnnotationTypePicker.Name = "cboAnnotationTypePicker"
        Me.cboAnnotationTypePicker.Size = New System.Drawing.Size(364, 25)
        Me.cboAnnotationTypePicker.TabIndex = 13
        '
        'lblAnnAuth
        '
        Me.lblAnnAuth.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblAnnAuth.Location = New System.Drawing.Point(641, 567)
        Me.lblAnnAuth.Name = "lblAnnAuth"
        Me.lblAnnAuth.Size = New System.Drawing.Size(285, 20)
        Me.lblAnnAuth.TabIndex = 12
        Me.lblAnnAuth.Text = "Select Annotation &Type"
        '
        'lvwSelectedFiles
        '
        Me.lvwSelectedFiles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwSelectedFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colUpFileName, Me.colSelOrganism, Me.colDescription, Me.colSource, Me.colAnnType})
        Me.lvwSelectedFiles.FullRowSelect = True
        Me.lvwSelectedFiles.GridLines = True
        Me.lvwSelectedFiles.HideSelection = False
        Me.lvwSelectedFiles.Location = New System.Drawing.Point(342, 335)
        Me.lvwSelectedFiles.Name = "lvwSelectedFiles"
        Me.lvwSelectedFiles.Size = New System.Drawing.Size(843, 224)
        Me.lvwSelectedFiles.TabIndex = 9
        Me.lvwSelectedFiles.UseCompatibleStateImageBehavior = False
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
        'colDescription
        '
        Me.colDescription.Text = "Description"
        Me.colDescription.Width = 150
        '
        'colSource
        '
        Me.colSource.Text = "Source (person, URL, FTP)"
        Me.colSource.Width = 150
        '
        'colAnnType
        '
        Me.colAnnType.Text = "Annotation Type"
        Me.colAnnType.Width = 105
        '
        'lblSelectedFiles
        '
        Me.lblSelectedFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblSelectedFiles.Location = New System.Drawing.Point(342, 304)
        Me.lblSelectedFiles.Name = "lblSelectedFiles"
        Me.lblSelectedFiles.Size = New System.Drawing.Size(868, 19)
        Me.lblSelectedFiles.TabIndex = 8
        Me.lblSelectedFiles.Text = "FASTA files selected for upload"
        '
        'cmdAddFile
        '
        Me.cmdAddFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdAddFile.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdAddFile.GenerateDisabledImage = True
        Me.cmdAddFile.Location = New System.Drawing.Point(681, 272)
        Me.cmdAddFile.Name = "cmdAddFile"
        Me.cmdAddFile.Size = New System.Drawing.Size(48, 44)
        Me.cmdAddFile.TabIndex = 6
        Me.cmdAddFile.ThemedImage = CType(resources.GetObject("cmdAddFile.ThemedImage"), System.Drawing.Bitmap)
        '
        'cmdRemoveFile
        '
        Me.cmdRemoveFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdRemoveFile.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdRemoveFile.GenerateDisabledImage = True
        Me.cmdRemoveFile.Location = New System.Drawing.Point(751, 272)
        Me.cmdRemoveFile.Name = "cmdRemoveFile"
        Me.cmdRemoveFile.Size = New System.Drawing.Size(48, 44)
        Me.cmdRemoveFile.TabIndex = 7
        Me.cmdRemoveFile.ThemedImage = CType(resources.GetObject("cmdRemoveFile.ThemedImage"), System.Drawing.Bitmap)
        '
        'cmdPreviewFile
        '
        Me.cmdPreviewFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdPreviewFile.Enabled = False
        Me.cmdPreviewFile.Location = New System.Drawing.Point(1003, 297)
        Me.cmdPreviewFile.Name = "cmdPreviewFile"
        Me.cmdPreviewFile.Size = New System.Drawing.Size(182, 30)
        Me.cmdPreviewFile.TabIndex = 5
        Me.cmdPreviewFile.Text = "&Preview Selected File"
        '
        'chkEncryptionEnable
        '
        Me.chkEncryptionEnable.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkEncryptionEnable.Location = New System.Drawing.Point(707, 628)
        Me.chkEncryptionEnable.Name = "chkEncryptionEnable"
        Me.chkEncryptionEnable.Size = New System.Drawing.Size(174, 23)
        Me.chkEncryptionEnable.TabIndex = 16
        Me.chkEncryptionEnable.Text = "Encrypt Sequences?"
        Me.chkEncryptionEnable.Visible = False
        '
        'lblPassphrase
        '
        Me.lblPassphrase.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblPassphrase.Location = New System.Drawing.Point(703, 653)
        Me.lblPassphrase.Name = "lblPassphrase"
        Me.lblPassphrase.Size = New System.Drawing.Size(178, 20)
        Me.lblPassphrase.TabIndex = 17
        Me.lblPassphrase.Text = "Encryption Passphrase"
        Me.lblPassphrase.Visible = False
        '
        'txtPassphrase
        '
        Me.txtPassphrase.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtPassphrase.Enabled = False
        Me.txtPassphrase.Location = New System.Drawing.Point(707, 673)
        Me.txtPassphrase.Name = "txtPassphrase"
        Me.txtPassphrase.PasswordChar = Global.Microsoft.VisualBasic.ChrW(8226)
        Me.txtPassphrase.Size = New System.Drawing.Size(154, 24)
        Me.txtPassphrase.TabIndex = 18
        Me.txtPassphrase.Visible = False
        '
        'fraValidationOptions
        '
        Me.fraValidationOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.fraValidationOptions.Controls.Add(Me.chkValidationAllowAllSymbolsInProteinNames)
        Me.fraValidationOptions.Controls.Add(Me.txtMaximumProteinNameLength)
        Me.fraValidationOptions.Controls.Add(Me.lblMaximumProteinNameLength)
        Me.fraValidationOptions.Controls.Add(Me.chkValidationAllowAsterisks)
        Me.fraValidationOptions.Controls.Add(Me.chkValidationAllowDash)
        Me.fraValidationOptions.Location = New System.Drawing.Point(6, 622)
        Me.fraValidationOptions.Name = "fraValidationOptions"
        Me.fraValidationOptions.Size = New System.Drawing.Size(687, 75)
        Me.fraValidationOptions.TabIndex = 15
        Me.fraValidationOptions.TabStop = False
        Me.fraValidationOptions.Text = "Fasta Validation Options"
        '
        'chkValidationAllowAllSymbolsInProteinNames
        '
        Me.chkValidationAllowAllSymbolsInProteinNames.Location = New System.Drawing.Point(11, 47)
        Me.chkValidationAllowAllSymbolsInProteinNames.Name = "chkValidationAllowAllSymbolsInProteinNames"
        Me.chkValidationAllowAllSymbolsInProteinNames.Size = New System.Drawing.Size(292, 25)
        Me.chkValidationAllowAllSymbolsInProteinNames.TabIndex = 1
        Me.chkValidationAllowAllSymbolsInProteinNames.Text = "Allow all symbols in protein names"
        '
        'txtMaximumProteinNameLength
        '
        Me.txtMaximumProteinNameLength.Location = New System.Drawing.Point(570, 21)
        Me.txtMaximumProteinNameLength.Name = "txtMaximumProteinNameLength"
        Me.txtMaximumProteinNameLength.Size = New System.Drawing.Size(84, 24)
        Me.txtMaximumProteinNameLength.TabIndex = 4
        Me.txtMaximumProteinNameLength.Text = "60"
        '
        'lblMaximumProteinNameLength
        '
        Me.lblMaximumProteinNameLength.Location = New System.Drawing.Point(454, 16)
        Me.lblMaximumProteinNameLength.Name = "lblMaximumProteinNameLength"
        Me.lblMaximumProteinNameLength.Size = New System.Drawing.Size(128, 34)
        Me.lblMaximumProteinNameLength.TabIndex = 3
        Me.lblMaximumProteinNameLength.Text = "Max Protein Name Length"
        '
        'chkValidationAllowAsterisks
        '
        Me.chkValidationAllowAsterisks.Location = New System.Drawing.Point(11, 19)
        Me.chkValidationAllowAsterisks.Name = "chkValidationAllowAsterisks"
        Me.chkValidationAllowAsterisks.Size = New System.Drawing.Size(219, 25)
        Me.chkValidationAllowAsterisks.TabIndex = 0
        Me.chkValidationAllowAsterisks.Text = "Allow asterisks in residues"
        '
        'chkValidationAllowDash
        '
        Me.chkValidationAllowDash.Location = New System.Drawing.Point(241, 19)
        Me.chkValidationAllowDash.Name = "chkValidationAllowDash"
        Me.chkValidationAllowDash.Size = New System.Drawing.Size(218, 25)
        Me.chkValidationAllowDash.TabIndex = 2
        Me.chkValidationAllowDash.Text = "Allow dash in residues"
        '
        'cmdRefreshFiles
        '
        Me.cmdRefreshFiles.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdRefreshFiles.Location = New System.Drawing.Point(1003, 258)
        Me.cmdRefreshFiles.Name = "cmdRefreshFiles"
        Me.cmdRefreshFiles.Size = New System.Drawing.Size(182, 30)
        Me.cmdRefreshFiles.TabIndex = 4
        Me.cmdRefreshFiles.Text = "&Refresh Files"
        '
        'cmdUpdateDescription
        '
        Me.cmdUpdateDescription.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdUpdateDescription.Location = New System.Drawing.Point(1068, 574)
        Me.cmdUpdateDescription.Name = "cmdUpdateDescription"
        Me.cmdUpdateDescription.Size = New System.Drawing.Size(117, 49)
        Me.cmdUpdateDescription.TabIndex = 14
        Me.cmdUpdateDescription.Text = "Update &Description"
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.Location = New System.Drawing.Point(342, 258)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(295, 46)
        Me.lblStatus.TabIndex = 21
        Me.lblStatus.Text = "Status"
        '
        'frmBatchAddNewCollection
        '
        Me.AcceptButton = Me.cmdUploadChecked
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(1199, 707)
        Me.Controls.Add(Me.cmdRemoveFile)
        Me.Controls.Add(Me.cmdAddFile)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdUploadChecked)
        Me.Controls.Add(Me.cmdUpdateDescription)
        Me.Controls.Add(Me.cmdRefreshFiles)
        Me.Controls.Add(Me.fraValidationOptions)
        Me.Controls.Add(Me.txtPassphrase)
        Me.Controls.Add(Me.lblPassphrase)
        Me.Controls.Add(Me.chkEncryptionEnable)
        Me.Controls.Add(Me.cmdPreviewFile)
        Me.Controls.Add(Me.lvwSelectedFiles)
        Me.Controls.Add(Me.lvwFolderContents)
        Me.Controls.Add(Me.lblOrganismSelect)
        Me.Controls.Add(Me.ctlTreeViewFolderBrowser)
        Me.Controls.Add(Me.cboOrganismSelect)
        Me.Controls.Add(Me.lblBatchUploadTree)
        Me.Controls.Add(Me.lblFolderContents)
        Me.Controls.Add(Me.cboAnnotationTypePicker)
        Me.Controls.Add(Me.lblAnnAuth)
        Me.Controls.Add(Me.lblSelectedFiles)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(1120, 576)
        Me.Name = "frmBatchAddNewCollection"
        Me.Text = "Batch Upload FASTA Files"
        Me.fraValidationOptions.ResumeLayout(False)
        Me.fraValidationOptions.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

#Region "Constants, enums, and member variables"

    Private Enum eFolderContentsColumn
        FileName = 0
        LastModified = 1
        FileSize = 2
        ExistingCollection = 3
        FilePath = 4
    End Enum

    Private Enum eSelectedFileColumn
        ProteinCollectionName = 0
        Organism = 1
        Description = 2
        Source = 3
        AnnotationType = 4
        FilePath = 5
    End Enum

    Private Structure udtProteinCollectionMetadata
        Public Description As String
        Public Source As String
    End Structure

    Private m_LastUsedDirectory As String
    Private m_LastSelectedOrganism As String = String.Empty
    Private m_LastSelectedAnnotationType As String = String.Empty

    ''' <summary>
    ''' Keys are full paths to the fasta file
    ''' Values are FileInfo instances
    ''' </summary>
    Private m_FileList As Dictionary(Of String, FileInfo)

    Private m_CheckedFileList As List(Of Protein_Uploader.clsPSUploadHandler.UploadInfo)

    ' Keys are file paths, values are UploadInfo objects
    Private m_SelectedFileList As Dictionary(Of String, Protein_Uploader.clsPSUploadHandler.UploadInfo)

    Private ReadOnly m_OrganismList As DataTable
    Private ReadOnly m_OrganismListSorted As DataView

    Private ReadOnly m_AnnotationTypeList As DataTable
    Private ReadOnly m_CollectionsTable As DataTable

    ''' <summary>
    ''' Keys are protein collection ID
    ''' Values are Protein Collection name
    ''' </summary>
    Private m_CollectionsList As Dictionary(Of Integer, String)

    Private m_SelectedOrganismID As Integer
    Private m_SelectedAnnotationTypeID As Integer
    Private ReadOnly m_PSConnectionString As String
    Private m_ReallyClose As Boolean = False

    Private WithEvents m_FilePreviewer As clsFilePreviewHandler
    Private m_PreviewFormStatus As Boolean

    ' Private m_PassphraseList As Hashtable
    ' Private m_CachedPassphrase As String

    Const ADD_FILES_MESSAGE As String = "You must first select an organism and annotation type, and then select one or more protein collections."
    Private m_AllowAddFiles As Boolean
    Private m_AllowAddFilesMessage As String = ADD_FILES_MESSAGE

    ' Tracks the index of the last column clicked in lvwSelectedFiles
    Private mSortColumnSelectedItems As Integer = -1

    ' Tracks the index of the last column clicked in lvwFolderContents
    Private mSortColumnFolderContents As Integer = -1

    ''' <summary>
    ''' Tracks Description and Source that the uploader has defined for each file (not persisted when the application closes)
    ''' </summary>
    ''' <remarks>Useful in case validation fails and the uploader needs to try again to upload a FASTA file</remarks>
    Private ReadOnly m_CachedFileDescriptions As Dictionary(Of String, KeyValuePair(Of String, String))

    Private m_StatusResetRequired As Boolean
    Private m_StatusClearTime As DateTime

    Private WithEvents m_StatusResetTimer As Timer

#End Region

#Region " Properties "
    ReadOnly Property FileList As List(Of clsPSUploadHandler.UploadInfo)
        Get
            Return m_CheckedFileList
        End Get
    End Property

    ReadOnly Property SelectedOrganismID As Integer
        Get
            Return m_SelectedOrganismID
        End Get
    End Property

    ReadOnly Property SelectedAnnotationTypeID As Integer
        Get
            Return m_SelectedAnnotationTypeID
        End Get
    End Property

    Property CurrentDirectory As String
        Get
            Return m_LastUsedDirectory
        End Get
        Set
            m_LastUsedDirectory = Value
        End Set
    End Property

    Private ReadOnly Property SelectedNodeFolderPath As String
        Get
            Try
                Dim currentNode = TryCast(ctlTreeViewFolderBrowser.SelectedNode, TreeNodePath)

                If Not currentNode Is Nothing AndAlso Not String.IsNullOrWhiteSpace(currentNode.Path) Then
                    Return currentNode.Path
                End If

            Catch ex As Exception
                ' Ignore errors
            End Try

            Return String.Empty
        End Get
    End Property

    Property SelectedOrganismName As String
        Get
            If cboOrganismSelect.Items.Count > 0 Then
                Return cboOrganismSelect.Text
            Else
                Return String.Empty
            End If
        End Get
        Set
            m_LastSelectedOrganism = Value
        End Set
    End Property

    Property SelectedAnnotationType As String
        Get
            If cboAnnotationTypePicker.Items.Count > 0 Then
                Return cboAnnotationTypePicker.Text
            Else
                Return String.Empty
            End If
        End Get
        Set
            m_LastSelectedAnnotationType = Value
        End Set
    End Property

    Property ValidationAllowAsterisks As Boolean
        Get
            Return chkValidationAllowAsterisks.Checked
        End Get
        Set
            chkValidationAllowAsterisks.Checked = Value
        End Set
    End Property

    Property ValidationAllowDash As Boolean
        Get
            Return chkValidationAllowDash.Checked
        End Get
        Set
            chkValidationAllowDash.Checked = Value
        End Set
    End Property

    Property ValidationAllowAllSymbolsInProteinNames As Boolean
        Get
            Return chkValidationAllowAllSymbolsInProteinNames.Checked
        End Get
        Set
            chkValidationAllowAllSymbolsInProteinNames.Checked = Value
        End Set
    End Property

    Property ValidationMaxProteinNameLength As Integer
        Get
            Dim intValue As Integer
            If Integer.TryParse(txtMaximumProteinNameLength.Text, intValue) Then
                Return intValue
            Else
                Return clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH
            End If
        End Get
        Set
            If Value < 5 Then Value = 5
            txtMaximumProteinNameLength.Text = Value.ToString()
        End Set
    End Property

#End Region

    Private Sub frmBatchAddNewCollection_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        m_CollectionsList = CollectionsTableToList(m_CollectionsTable)

        If m_FileList Is Nothing Then
            m_FileList = New Dictionary(Of String, FileInfo)
        End If

        m_CheckedFileList = New List(Of clsPSUploadHandler.UploadInfo)
        LoadOrganismPicker(cboOrganismSelect, m_OrganismListSorted)
        LoadAnnotationTypePicker(cboAnnotationTypePicker, m_AnnotationTypeList)
        cmdUploadChecked.Enabled = False
        cmdAddFile.Enabled = True
        cmdRemoveFile.Enabled = False

        SelectComboBoxItemByName(cboOrganismSelect, m_LastSelectedOrganism, 2)
        SelectComboBoxItemByName(cboAnnotationTypePicker, m_LastSelectedAnnotationType, 1)

    End Sub

#Region " Directory Loading "

    Private Function CollectionsTableToList(dt As DataTable) As Dictionary(Of Integer, String)
        Dim collectionInfo As New Dictionary(Of Integer, String)(dt.Rows.Count)
        Dim dr As DataRow
        Dim foundRows() As DataRow = dt.Select("", "Protein_Collection_ID")
        Dim tmpID As Integer
        Dim tmpName As String

        For Each dr In foundRows
            tmpID = DirectCast(dr.Item("Protein_Collection_ID"), Integer)
            tmpName = dr.Item("FileName").ToString()
            If Not collectionInfo.ContainsKey(tmpID) Then
                collectionInfo.Add(tmpID, tmpName)
            End If
        Next

        Return collectionInfo

    End Function

    Private Sub ScanDirectory(directoryPath As String)

        lblFolderContents.Text = "FASTA files in: '" & directoryPath & "'"

        Dim di As New DirectoryInfo(directoryPath)

        If Not di.Exists Then
            Exit Sub
        End If

        m_LastUsedDirectory = directoryPath

        Dim foundFASTAFiles = di.GetFiles()

        If Not m_FileList Is Nothing Then
            m_FileList.Clear()
        Else
            m_FileList = New Dictionary(Of String, FileInfo)
        End If

        For Each fi In foundFASTAFiles
            Dim fileExtension = Path.GetExtension(fi.Name)

            Select Case fileExtension.ToLower()
                Case ".fasta", ".fst", ".fa", ".pep", ".faa"
                    m_FileList.Add(fi.FullName, fi)
            End Select
        Next

        LoadListView()

    End Sub

#End Region

#Region " UI Loading Functions "

    ''' <summary>
    ''' Populate the top ListView with fasta files in the selected folder
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadListView()
        lvwFolderContents.BeginUpdate()

        lvwFolderContents.Items.Clear()

        If m_CollectionsList Is Nothing Then
            m_CollectionsList = New Dictionary(Of Integer, String)
        End If

        For Each fi In m_FileList.Values

            Dim proteinCollectionName = Path.GetFileNameWithoutExtension(fi.Name)

            Dim li = New ListViewItem()

            ' Fasta file name (with the extension)
            li.Text = fi.Name

            ' Last Write Time
            li.SubItems.Add(Format(fi.LastWriteTime, "g"))

            ' File Size
            li.SubItems.Add(Numeric2Bytes(fi.Length))

            ' Whether or not the fasta file is already a protein collection
            If m_CollectionsList.ContainsValue(proteinCollectionName) Then
                li.SubItems.Add("Yes")
            Else
                li.SubItems.Add("No")
            End If

            ' Full file path
            li.SubItems.Add(fi.FullName)

            lvwFolderContents.Items.Add(li)
        Next

        lvwFolderContents.EndUpdate()
    End Sub

    Private Sub LoadOrganismPicker(cbo As ComboBox, orgList As DataView)
        RemoveHandler cboOrganismSelect.SelectedIndexChanged, AddressOf cboOrganismSelect_SelectedIndexChanged
        With cbo
            .DataSource = orgList
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
        End With

        AddHandler cboOrganismSelect.SelectedIndexChanged, AddressOf cboOrganismSelect_SelectedIndexChanged
    End Sub

    Private Sub LoadAnnotationTypePicker(cbo As ComboBox, authList As DataTable)
        RemoveHandler cboAnnotationTypePicker.SelectedIndexChanged, AddressOf cboAnnotationTypePicker_SelectedIndexChanged

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

        AddHandler cboAnnotationTypePicker.SelectedIndexChanged, AddressOf cboAnnotationTypePicker_SelectedIndexChanged
    End Sub

#End Region

#Region " Internal Service Functions "

    Function Numeric2Bytes(b As Double) As String
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

        For i = UBound(bSize) To 0 Step -1
            If b >= (1024 ^ i) Then
                Return FormatDecimal(b / (1024 ^ i)) & " " & bSize(i)
            End If
        Next

        Return b.ToString() & " Bytes"

    End Function

    ''' <summary>
    ''' Return the value formatted to include one or two digits after the decimal point
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Examples:
    '''      1
    '''    123
    '''     12.3
    '''      2.4
    '''      1.2
    '''      0.12
    ''' </remarks>
    Private Function FormatDecimal(value As Double) As String
        If value >= 100 Then
            ' No digits after the decimal
            Return Format$(CInt(value))
        ElseIf value >= 1 Then
            ' One digit after the decimal
            Return Format$(value, "0.0")
        Else
            ' Two digits after the decimal
            Return Format$(value, "0.00")
        End If
    End Function

#End Region

    Private Sub AddFileToSelectedList()

        Try
            If m_SelectedFileList Is Nothing Then
                m_SelectedFileList = New Dictionary(Of String, clsPSUploadHandler.UploadInfo)(StringComparer.CurrentCultureIgnoreCase)
            End If

            For Each li As ListViewItem In lvwFolderContents.SelectedItems
                Dim fastaFilePath = GetFolderContentsColumn(li, eFolderContentsColumn.FilePath)

                Dim upInfo = New clsPSUploadHandler.UploadInfo() With {
                    .FileInformation = m_FileList.Item(fastaFilePath),
                    .OrganismID = DirectCast(cboOrganismSelect.SelectedValue, Integer),
                    .AnnotationTypeID = DirectCast(cboAnnotationTypePicker.SelectedValue, Integer),
                    .Description = String.Empty,
                    .Source = String.Empty,
                    .EncryptSequences = False,
                    .EncryptionPassphrase = String.Empty
                }

                Dim proteinCollection = Path.GetFileNameWithoutExtension(upInfo.FileInformation.Name)

                If m_SelectedFileList.ContainsKey(upInfo.FileInformation.FullName) Then
                    m_SelectedFileList.Remove(upInfo.FileInformation.FullName)
                    For Each si As ListViewItem In lvwSelectedFiles.Items
                        If si.Text = proteinCollection Then
                            lvwSelectedFiles.Items.Remove(si)
                        End If
                    Next
                End If

                Dim kvDescriptionSource As KeyValuePair(Of String, String) = Nothing
                Dim fileDescription As String
                Dim fileSource As String

                If m_CachedFileDescriptions.TryGetValue(proteinCollection, kvDescriptionSource) Then
                    fileDescription = kvDescriptionSource.Key
                    fileSource = kvDescriptionSource.Value
                Else
                    fileDescription = String.Empty
                    fileSource = String.Empty
                End If

                Dim newLi = New ListViewItem(proteinCollection)
                With newLi
                    ' Organism (Column 1)
                    .SubItems.Add(cboOrganismSelect.Text)

                    ' Description (Column 2)
                    .SubItems.Add(fileDescription)

                    ' Source (Column 3)
                    .SubItems.Add(fileSource)

                    ' Annotation Type (Column 4)
                    .SubItems.Add(cboAnnotationTypePicker.Text)

                    ' Full Path (ColIndex 5)
                    .SubItems.Add(upInfo.FileInformation.FullName)

                End With

                lvwSelectedFiles.Items.Add(newLi)
                m_SelectedFileList.Add(upInfo.FileInformation.FullName, upInfo)


            Next
        Catch ex As Exception
            MessageBox.Show("Error in AddFileToSelectedList: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try


    End Sub

    Private Sub AddUpdateDictionaryItem(itemList As IDictionary(Of String, Integer), newItem As String)

        Dim itemCount As Integer
        If itemList.TryGetValue(newItem, itemCount) Then
            itemList(newItem) = itemCount + 1
        Else
            itemList.Add(newItem, 1)
        End If

    End Sub

    Private Sub AfterNodeSelect()
        Dim folderPath = Me.SelectedNodeFolderPath
        If String.IsNullOrWhiteSpace(folderPath) Then Return

        ScanDirectory(folderPath)
    End Sub

    Private Sub ClearStatus()
        lblStatus.Text = String.Empty
    End Sub

    Private Sub UpdateStatus(message As String)
        lblStatus.Text = message

        m_StatusClearTime = DateTime.UtcNow.AddSeconds(5)
        m_StatusResetRequired = True

    End Sub

    Private Function GetFolderContentsColumn(li As ListViewItem, eColumn As eFolderContentsColumn) As String
        Dim value = li.SubItems(eColumn).Text
        Return value
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="li"></param>
    ''' <param name="eColumn"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSelectedFileColumn(li As ListViewItem, eColumn As eSelectedFileColumn) As String
        Dim value = li.SubItems(eColumn).Text
        Return value
    End Function

    Private Sub InitializeTreeView(Optional selectedDirectoryPath As String = "")
        Try
            If Not String.IsNullOrWhiteSpace(selectedDirectoryPath) Then

                Dim currentFolder = New DirectoryInfo(selectedDirectoryPath)

                While Not currentFolder.Exists AndAlso Not currentFolder.Parent Is Nothing
                    currentFolder = currentFolder.Parent
                End While

                If String.Equals(Me.SelectedNodeFolderPath, currentFolder.FullName, StringComparison.OrdinalIgnoreCase) Then
                    ScanDirectory(currentFolder.FullName)
                    Return
                End If

                ctlTreeViewFolderBrowser.Populate(currentFolder.FullName)
                Return
            End If
        Catch ex As Exception
            If Not String.IsNullOrWhiteSpace(selectedDirectoryPath) Then
                MessageBox.Show("Error refreshing folders and files for directory " & selectedDirectoryPath & ": " & ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End If
        End Try

        ctlTreeViewFolderBrowser.Populate()
        If Not ctlTreeViewFolderBrowser.TopNode.IsExpanded Then
            ctlTreeViewFolderBrowser.TopNode.Expand()
        End If

    End Sub


    ''' <summary>
    ''' Populates m_CheckedFileList
    ''' </summary>
    ''' <returns>True if success, false if no protein collections are selected or if one or more is missing a description and/or source</returns>
    ''' <remarks></remarks>
    Private Function MakeCheckedFileList() As Boolean

        Dim tmpNameList As New Dictionary(Of String, udtProteinCollectionMetadata)

        For Each li As ListViewItem In lvwSelectedFiles.Items
            Dim fastaFilePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath)

            If Not tmpNameList.ContainsKey(fastaFilePath) Then
                Dim udtMetadata = New udtProteinCollectionMetadata()
                udtMetadata.Description = GetSelectedFileColumn(li, eSelectedFileColumn.Description)
                udtMetadata.Source = GetSelectedFileColumn(li, eSelectedFileColumn.Source)
                tmpNameList.Add(fastaFilePath, udtMetadata)
            End If

        Next

        For Each item In m_SelectedFileList
            Dim upInfo = item.Value
            Dim fi = upInfo.FileInformation

            Dim udtMetadata As udtProteinCollectionMetadata = Nothing
            If tmpNameList.TryGetValue(fi.FullName, udtMetadata) Then

                upInfo.Description = udtMetadata.Description
                upInfo.Source = udtMetadata.Source

                If String.IsNullOrWhiteSpace(upInfo.Description) AndAlso String.IsNullOrWhiteSpace(upInfo.Source) Then
                    MessageBox.Show("Each new protein collection must have a description and/or source defined", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                End If

                m_CheckedFileList.Add(upInfo)
            End If
        Next

        If m_CheckedFileList.Count = 0 Then
            MessageBox.Show("No fasta files are selected", "Nothing to Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return False
        End If

        Return True

    End Function

    Private Function MostCommonItem(itemList As Dictionary(Of String, Integer)) As String

        If itemList.Count = 0 Then
            Return String.Empty
        End If

        Dim query = (From item In itemList Order By item.Key Descending Select item)

        Return query.First.Key

    End Function

    Private Sub RemoveFileFromSelectedList()

        For Each li As ListViewItem In lvwSelectedFiles.SelectedItems
            lvwSelectedFiles.Items.Remove(li)

            Dim filePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath)
            If m_SelectedFileList.ContainsKey(filePath) Then
                m_SelectedFileList.Remove(filePath)
            End If
        Next
    End Sub

    Private Sub SelectAllRows(lv As ListView)
        For Each li As ListViewItem In lv.Items
            li.Selected = True
        Next
    End Sub

    Private Sub SelectComboBoxItemByName(objComboBox As ComboBox, strValue As String, intDataColumnIndexToCheck As Integer)
        ' Look for strValue in a combobox that has a data table attached via the .DataSource property
        ' If the value is found, then the given item in the combobox is selected

        Dim intIndex As Integer
        Dim objRow As DataRowView

        Try
            If Not strValue Is Nothing AndAlso strValue.Length > 0 Then
                For intIndex = 0 To objComboBox.Items.Count - 1
                    objRow = DirectCast(objComboBox.Items.Item(intIndex), DataRowView)

                    If Not DBNull.Value.Equals(objRow.Item(intDataColumnIndexToCheck)) Then
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

    Private Sub SortListView(lv As ListView, lastClickedColIndex As Integer, colIndex As Integer, isDateColumn As Boolean)

        ' Determine whether the column is the same as the last column clicked.
        If colIndex <> lastClickedColIndex Then
            If isDateColumn Then
                ' Sort date columns descending by default
                lv.Sorting = SortOrder.Descending
            Else
                ' Sort text columns ascending by default
                lv.Sorting = SortOrder.Ascending
            End If
        Else
            ' Determine what the last sort order was and change it
            If lv.Sorting = SortOrder.Ascending Then
                lv.Sorting = SortOrder.Descending
            Else
                lv.Sorting = SortOrder.Ascending
            End If
        End If

        ' Set the ListViewItemSorter property to a new ListViewItemComparer object
        lv.ListViewItemSorter = New ListViewItemComparer(colIndex, lv.Sorting, isDateColumn)

        ' Call the sort method to manually sort
        lv.Sort()


    End Sub

    ''' <summary>
    ''' Remove leading and trailing whitespace.
    ''' Replace tabs and carriage returns with semicolons
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function StripWhiteSpace(value As String) As String

        Dim updatedValue = value.Trim().Replace(ControlChars.Tab, "; ").Replace(ControlChars.NewLine, "; ").Replace(ControlChars.Cr, "; ").Replace(ControlChars.Lf, "; ")

        Return updatedValue

    End Function

    Private Sub UpdateProteinCollectionMetadata()
        Dim descriptionList = New Dictionary(Of String, Integer)
        Dim sourceList = New Dictionary(Of String, Integer)

        Dim itemsToUpdate = New List(Of ListViewItem)

        If lvwSelectedFiles.SelectedItems.Count > 0 Then
            For Each li As ListViewItem In lvwSelectedFiles.SelectedItems

                Dim descriptionCurrent = GetSelectedFileColumn(li, eSelectedFileColumn.Description)
                Dim sourceCurrent = GetSelectedFileColumn(li, eSelectedFileColumn.Source)

                If Not String.IsNullOrWhiteSpace(descriptionCurrent) Then
                    AddUpdateDictionaryItem(descriptionList, descriptionCurrent)
                End If

                If Not String.IsNullOrWhiteSpace(sourceCurrent) Then
                    AddUpdateDictionaryItem(sourceList, sourceCurrent)
                End If

                itemsToUpdate.Add(li)
            Next
        Else
            For Each li As ListViewItem In lvwSelectedFiles.Items
                itemsToUpdate.Add(li)
            Next
        End If


        ' Show a window with the most commonly used description and source

        Dim oMetadataWindow = New frmNewCollectionMetadataEditor With {
            .Description = MostCommonItem(descriptionList),
            .Source = MostCommonItem(sourceList)
        }

        Dim eDialogResult = oMetadataWindow.ShowDialog()

        If eDialogResult = DialogResult.OK Then
            Dim updatedDescription = StripWhiteSpace(oMetadataWindow.Description)
            Dim updatedSource = StripWhiteSpace(oMetadataWindow.Source)

            For Each li In itemsToUpdate
                li.SubItems(eSelectedFileColumn.Description).Text = updatedDescription
                li.SubItems(eSelectedFileColumn.Source).Text = updatedSource


                Dim proteinCollection = li.SubItems(eSelectedFileColumn.ProteinCollectionName).Text

                Dim kvDescriptionSource = New KeyValuePair(Of String, String)(updatedDescription, updatedSource)

                If m_CachedFileDescriptions.ContainsKey(proteinCollection) Then
                    m_CachedFileDescriptions(proteinCollection) = kvDescriptionSource
                Else
                    m_CachedFileDescriptions.Add(proteinCollection, kvDescriptionSource)
                End If
            Next
        End If

    End Sub

#Region "Button and Combo Handlers"

    Private Sub cboOrganismSelect_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboOrganismSelect.SelectedIndexChanged
        Dim cbo = DirectCast(sender, ComboBox)

        m_SelectedOrganismID = DirectCast(cbo.SelectedValue, Integer)
        CheckTransferEnable()
        If lvwSelectedFiles.SelectedItems.Count > 0 Then

            For Each li As ListViewItem In lvwSelectedFiles.SelectedItems
                Dim fastaFilePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath)

                ' Update organism in m_SelectedFileList
                Dim tmpUpInfo = m_SelectedFileList.Item(fastaFilePath)
                tmpUpInfo.OrganismID = m_SelectedOrganismID

                m_SelectedFileList.Item(fastaFilePath) = tmpUpInfo

                ' Update organism in lvwSelectedFiles
                li.SubItems(eSelectedFileColumn.Organism).Text = cbo.Text
            Next
        End If

    End Sub

    Private Sub cboAnnotationTypePicker_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboAnnotationTypePicker.SelectedIndexChanged
        Dim cbo = DirectCast(sender, ComboBox)
        Dim tmpUpInfo As clsPSUploadHandler.UploadInfo

        If cboAnnotationTypePicker.SelectedValue.GetType Is Type.GetType("System.Int32") Then
            m_SelectedAnnotationTypeID = CInt(cboAnnotationTypePicker.SelectedValue)
        Else
            'm_SelectedAuthorityID = 0
        End If

        CheckTransferEnable()

        Dim tmpAnnTypeID As Integer

        If m_SelectedAnnotationTypeID = -2 Then
            'Bring up an additional dialog
            Dim AnnTypeAdd As New clsAddAnnotationType(m_PSConnectionString)
            AnnTypeAdd.FormLocation = New Point(Left + Width + 10, Top)
            tmpAnnTypeID = AnnTypeAdd.AddAnnotationType
            'Dim AuthAdd As New clsAddNamingAuthority(m_PSConnectionString)
            'tmpAuthID = AuthAdd.AddNamingAuthority

            If Not AnnTypeAdd.EntryExists And tmpAnnTypeID > 0 Then

                Dim dr As DataRow
                dr = m_AnnotationTypeList.NewRow

                With dr
                    .Item("ID") = tmpAnnTypeID
                    .Item("Display_Name") = AnnTypeAdd.DisplayName
                    .Item("Details") = AnnTypeAdd.Description
                End With

                m_AnnotationTypeList.Rows.Add(dr)
                m_AnnotationTypeList.AcceptChanges()
                LoadAnnotationTypePicker(cboAnnotationTypePicker, m_AnnotationTypeList)
                m_SelectedAnnotationTypeID = tmpAnnTypeID

                cboAnnotationTypePicker.SelectedValue = tmpAnnTypeID
            End If

        End If

        If lvwSelectedFiles.SelectedItems.Count > 0 Then
            Dim li As ListViewItem
            For Each li In lvwSelectedFiles.SelectedItems

                ' Update annotation type in m_SelectedFileList
                Dim fastaFilePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath)
                tmpUpInfo = m_SelectedFileList.Item(fastaFilePath)

                m_SelectedFileList.Item(fastaFilePath) =
                    New clsPSUploadHandler.UploadInfo(
                        tmpUpInfo.FileInformation,
                        m_SelectedOrganismID,
                        m_SelectedAnnotationTypeID)

                ' Update annotation type in lvwSelectedFiles
                li.SubItems(eSelectedFileColumn.AnnotationType).Text = cbo.Text
            Next
        End If

    End Sub

    Private Sub chkEncryptionEnable_CheckedChanged(sender As Object, e As EventArgs) Handles chkEncryptionEnable.CheckedChanged
        Dim chk = DirectCast(sender, CheckBox)
        Dim encryptSequences = False

        If chk.CheckState = CheckState.Checked Then
            txtPassphrase.Enabled = True
        Else
            txtPassphrase.Enabled = False
        End If

        CheckTransferEnable()

        'Dim tmpUpInfo As Protein_Uploader.clsPSUploadHandler.UploadInfo
        'If lvwSelectedFiles.SelectedItems.Count > 0 Then
        '    Dim li As ListViewItem
        '    For Each li In lvwSelectedFiles.SelectedItems
        '        Dim fastaFilePath = li.SubItems(eSelectedFileColumn.FilePath).Text
        '        tmpUpInfo = DirectCast(m_SelectedFileList.Item(fastaFilePath), Protein_Uploader.clsPSUploadHandler.UploadInfo)
        '        If encryptSequences Then
        '            tmpUpInfo.EncryptSequences = True
        '            tmpUpInfo.EncryptionPassphrase = passPhraseForm.Passphrase
        '            li.SubItems(eSelectedFileColumn.Encrypt).Text = "Yes"
        '        Else
        '            tmpUpInfo.EncryptSequences = False
        '            tmpUpInfo.EncryptionPassphrase = Nothing
        '            li.SubItems(eSelectedFileColumn.Encrypt).Text = "No"
        '        End If
        '        m_SelectedFileList.Item(fastaFilePath) = tmpUpInfo
        '    Next
        'End If

    End Sub

    Private Sub CheckTransferEnable()
        If chkEncryptionEnable.Checked = True Then
            If m_SelectedOrganismID > 0 And
             m_SelectedAnnotationTypeID > 0 And
             lvwFolderContents.SelectedItems.Count > 0 And
             txtPassphrase.Text.Length > 0 Then

                m_AllowAddFiles = True
                m_AllowAddFilesMessage = ""
            Else
                m_AllowAddFiles = False
                m_AllowAddFilesMessage = ADD_FILES_MESSAGE & "  You also must define a passphrase for encryption."
            End If
        Else
            If m_SelectedOrganismID > 0 And m_SelectedAnnotationTypeID > 0 And lvwFolderContents.SelectedItems.Count > 0 Then
                m_AllowAddFiles = True
                m_AllowAddFilesMessage = ""
            Else
                m_AllowAddFiles = False
                m_AllowAddFilesMessage = ADD_FILES_MESSAGE
            End If
        End If

        If lvwSelectedFiles.Items.Count > 0 Then
            cmdRemoveFile.Enabled = True
        Else
            cmdRemoveFile.Enabled = False
        End If

        If lvwSelectedFiles.Items.Count > 0 Then
            cmdUploadChecked.Enabled = True
        Else
            cmdUploadChecked.Enabled = False
        End If

        If lvwFolderContents.SelectedItems.Count > 0 Then
            cmdPreviewFile.Enabled = True
        Else
            cmdPreviewFile.Enabled = False
        End If

    End Sub

    Private Sub cmdUploadChecked_Click(sender As Object, e As EventArgs) Handles cmdUploadChecked.Click
        Dim validInfo = MakeCheckedFileList()
        If Not validInfo Then Return

        m_ReallyClose = True
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub cmdAddFile_Click(sender As Object, e As EventArgs) Handles cmdAddFile.Click
        If Not m_AllowAddFiles Then
            MessageBox.Show(m_AllowAddFilesMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            AddFileToSelectedList()
            CheckTransferEnable()
        End If
    End Sub

    Private Sub cmdRemoveFile_Click(sender As Object, e As EventArgs) Handles cmdRemoveFile.Click
        RemoveFileFromSelectedList()
        CheckTransferEnable()
    End Sub

    Private Sub cmdPreviewFile_Click(sender As Object, e As EventArgs) Handles cmdPreviewFile.Click
        If m_FilePreviewer Is Nothing Then
            m_FilePreviewer = New clsFilePreviewHandler
        End If

        If lvwFolderContents.SelectedItems.Count = 0 Then
            MessageBox.Show("Please select a file to preview", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Dim li = lvwFolderContents.SelectedItems(0)
        Dim fullName As String = GetFolderContentsColumn(li, eFolderContentsColumn.FilePath)

        m_FilePreviewer.ShowPreview(fullName, Left + Width + 10, Top, Height)

    End Sub

    Private Sub cmdRefreshFiles_Click(sender As Object, e As EventArgs) Handles cmdRefreshFiles.Click
        InitializeTreeView(m_LastUsedDirectory)
    End Sub

    Private Sub cmdUpdateDescription_Click(sender As Object, e As EventArgs) Handles cmdUpdateDescription.Click
        UpdateProteinCollectionMetadata()
    End Sub

#End Region

    Private Sub lvwSelectedFiles_Click(sender As Object, e As EventArgs) Handles lvwSelectedFiles.Click

        If lvwSelectedFiles.SelectedItems.Count = 0 Then Return

        Dim li As ListViewItem = lvwSelectedFiles.SelectedItems.Item(0)

        Dim selectedOrganism = GetSelectedFileColumn(li, eSelectedFileColumn.Organism)

        'If li.SubItems(eSelectedFileColumn.Encrypt).Text = "Yes" Then
        '    RemoveHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
        '    chkEncryptionEnable.CheckState = CheckState.Checked
        '    AddHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
        'Else
        '    RemoveHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
        '    chkEncryptionEnable.CheckState = CheckState.Unchecked
        '    AddHandler chkEncryptionEnable.CheckedChanged, AddressOf chkEncryptionEnable_CheckedChanged
        'End If

        Dim selFileAnnotationType As String = GetSelectedFileColumn(li, eSelectedFileColumn.AnnotationType)

        cboAnnotationTypePicker.Text = selFileAnnotationType
        cboOrganismSelect.Text = selectedOrganism

        CheckTransferEnable()

    End Sub

    Private Sub lvwFolderContents_Click(sender As Object, e As EventArgs) Handles lvwFolderContents.Click
        lvwSelectedFiles.SelectedItems.Clear()
    End Sub

    Private Sub frmBatchAddNewCollection_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        If lvwSelectedFiles.Items.Count > 0 And Not m_ReallyClose Then
            Dim r As DialogResult

            r = MessageBox.Show("You have files selected for upload. Really close the form?",
                                "Files selected for upload", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
            If r = DialogResult.No Then
                e.Cancel = True
            Else
                e.Cancel = False
            End If

        End If
        If Not m_FilePreviewer Is Nothing And m_PreviewFormStatus = True Then
            m_FilePreviewer.CloseForm()
            m_FilePreviewer = Nothing
        End If
    End Sub

    Private Sub lvwFolderContents_MouseUp(sender As Object, e As MouseEventArgs) Handles lvwFolderContents.MouseUp
        CheckTransferEnable()
    End Sub

    Sub OnPreviewFormStatusChange(Visibility As Boolean) Handles m_FilePreviewer.FormStatus
        If Visibility = True Then
            cmdPreviewFile.Enabled = False
            m_PreviewFormStatus = True
        Else
            cmdPreviewFile.Enabled = True
            m_PreviewFormStatus = False
        End If
    End Sub

    'Private Sub txtPassphrase_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtPassphrase.Validating
    '    Dim txt As TextBox = DirectCast(sender, TextBox)
    '    Dim li As ListViewItem

    '    If txt.Text.Length > 0 Then
    '        If lvwSelectedFiles.SelectedItems.Count = 0 Then
    '            m_CachedPassphrase = txt.Text
    '        ElseIf lvwSelectedFiles.SelectedItems.Count > 0 Then
    '            For Each li In lvwSelectedFiles.Items
    '                li.Tag = txt.Text
    '                li.SubItems(eSelectedFileColumn.Encrypt).Text = "Yes"
    '            Next
    '        End If
    '        CheckTransferEnable()
    '    Else
    '        CheckTransferEnable()
    '        Exit Sub
    '    End If
    'End Sub

    Private Sub txtMaximumProteinNameLength_Validating(sender As Object, e As CancelEventArgs) Handles txtMaximumProteinNameLength.Validating
        If txtMaximumProteinNameLength.TextLength = 0 Then
            txtMaximumProteinNameLength.Text = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString()
        Else
            Dim intValue = 0
            If Integer.TryParse(txtMaximumProteinNameLength.Text, intValue) Then
                If intValue < 30 Then
                    txtMaximumProteinNameLength.Text = "30"
                End If
            Else
                txtMaximumProteinNameLength.Text = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString()
            End If
        End If

    End Sub

    Private Sub lvwSelectedFiles_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lvwSelectedFiles.ColumnClick
        SortListView(lvwSelectedFiles, mSortColumnSelectedItems, e.Column, isDateColumn:=False)
        mSortColumnSelectedItems = e.Column
    End Sub

    Private Sub lvwSelectedFiles_DoubleClick(sender As Object, e As EventArgs) Handles lvwSelectedFiles.DoubleClick
        UpdateProteinCollectionMetadata()
    End Sub

    Private Sub lvwSelectedFiles_KeyDown(sender As Object, e As KeyEventArgs) Handles lvwSelectedFiles.KeyDown

        If e.Control AndAlso e.KeyCode = Keys.A Then
            ' Select all rows
            SelectAllRows(lvwSelectedFiles)
        End If

    End Sub

    Private Sub lvwFolderContents_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lvwFolderContents.ColumnClick
        Dim isDateColumn = (e.Column = eFolderContentsColumn.LastModified)
        SortListView(lvwFolderContents, mSortColumnFolderContents, e.Column, isDateColumn)
        mSortColumnFolderContents = e.Column
    End Sub

    Private Sub lvwFolderContents_KeyDown(sender As Object, e As KeyEventArgs) Handles lvwFolderContents.KeyDown

        If e.Control AndAlso e.KeyCode = Keys.A Then
            ' Select all rows
            SelectAllRows(lvwFolderContents)
        End If

    End Sub

    Private Sub ctlTreeViewFolderBrowser_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles ctlTreeViewFolderBrowser.AfterSelect
        AfterNodeSelect()
    End Sub

    Private Sub ctlTreeViewFolderBrowser_MouseUp(sender As Object, e As MouseEventArgs) Handles ctlTreeViewFolderBrowser.MouseUp
        If e.Button = MouseButtons.Right Then
            Dim folderPath = SelectedNodeFolderPath
            If String.IsNullOrWhiteSpace(folderPath) Then Exit Sub
            Clipboard.SetText(folderPath)
            UpdateStatus("Folder path copied to the clipboard")
        End If
    End Sub

    Private Sub ctlTreeViewFolderBrowser_KeyUp(sender As Object, e As KeyEventArgs) Handles ctlTreeViewFolderBrowser.KeyUp
        If e.KeyCode = Keys.F5 Then

            Try
                Dim folderPath = SelectedNodeFolderPath
                If String.IsNullOrWhiteSpace(folderPath) Then Exit Sub

                Dim currentFolder = New DirectoryInfo(folderPath)

                While Not currentFolder.Exists AndAlso Not currentFolder.Parent Is Nothing
                    currentFolder = currentFolder.Parent
                End While

                ctlTreeViewFolderBrowser.Populate(currentFolder.FullName)

                InitializeTreeView(currentFolder.FullName)

                If Not ctlTreeViewFolderBrowser.SelectedNode.IsExpanded Then
                    ctlTreeViewFolderBrowser.SelectedNode.Expand()
                End If

            Catch ex As Exception
                MessageBox.Show("Error in NodeMouseDoubleClick: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try

        End If
    End Sub

    Private Sub m_StatusResetTimer_Tick(sender As Object, e As EventArgs) Handles m_StatusResetTimer.Tick
        If m_StatusResetRequired AndAlso DateTime.UtcNow > m_StatusClearTime Then
            m_StatusResetRequired = False
            lblStatus.Text = String.Empty
        End If
    End Sub

    Private Class ListViewItemComparer
        Implements IComparer

        Private ReadOnly mSortingDates As Boolean
        Private ReadOnly mColIndex As Integer
        Private ReadOnly mSortOrder As SortOrder

        Public Sub New(column As Integer, order As SortOrder, Optional sortingDates As Boolean = False)
            mSortingDates = sortingDates
            mColIndex = column
            mSortOrder = order
        End Sub

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Dim returnVal As Integer
            Dim compared = False

            If mSortingDates Then
                Try
                    ' Parse the two objects passed as a parameter as a DateTi
                    Dim dateA As DateTime = DateTime.Parse(CType(x, ListViewItem).SubItems(mColIndex).Text)
                    Dim dateB As DateTime = DateTime.Parse(CType(y, ListViewItem).SubItems(mColIndex).Text)

                    ' Compare the two dates.
                    returnVal = DateTime.Compare(dateA, dateB)
                    compared = True
                Catch
                    ' Sort as strings
                End Try
            End If

            If Not compared Then
                ' Compare the two items as a string.
                returnVal = String.Compare(CType(x, ListViewItem).SubItems(mColIndex).Text,
                                           CType(y, ListViewItem).SubItems(mColIndex).Text)
            End If

            If mSortOrder = SortOrder.Descending Then
                Return returnVal * -1
            Else
                Return returnVal
            End If

        End Function

    End Class

End Class
