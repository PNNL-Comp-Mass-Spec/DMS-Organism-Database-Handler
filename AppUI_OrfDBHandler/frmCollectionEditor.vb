Public Class frmCollectionEditor
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.CheckTransferButtonsEnabledStatus()
        'Add any initialization after the InitializeComponent() call

        ReadSettings()

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
    Friend WithEvents pnlProgBar As System.Windows.Forms.Panel
    Friend WithEvents gbxSourceCollection As System.Windows.Forms.GroupBox
    Friend WithEvents cboAuthorityPicker As System.Windows.Forms.ComboBox
    Friend WithEvents lblAuthorityFilter As System.Windows.Forms.Label
    Friend WithEvents pbxLiveSearchCancel As System.Windows.Forms.PictureBox
    Friend WithEvents lblSearchCount As System.Windows.Forms.Label
    Friend WithEvents cmdLoadProteins As System.Windows.Forms.Button
    Friend WithEvents cmdLoadFile As System.Windows.Forms.Button
    Friend WithEvents txtLiveSearch As System.Windows.Forms.TextBox
    Friend WithEvents cboCollectionPicker As System.Windows.Forms.ComboBox
    Friend WithEvents cboOrganismFilter As System.Windows.Forms.ComboBox
    Friend WithEvents lblOrganismFilter As System.Windows.Forms.Label
    Friend WithEvents lblCollectionPicker As System.Windows.Forms.Label
    Friend WithEvents pbxLiveSearchBkg As System.Windows.Forms.PictureBox
    Friend WithEvents lvwSource As System.Windows.Forms.ListView
    Friend WithEvents lblSourceMembers As System.Windows.Forms.Label
    Friend WithEvents colSrcName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colSrcDesc As System.Windows.Forms.ColumnHeader
    Friend WithEvents cmdDestAdd As UIControls.ImageButton
    Friend WithEvents cmdDestRemove As UIControls.ImageButton
    Friend WithEvents cmdDestAddAll As UIControls.ImageButton
    Friend WithEvents cmdDestRemoveAll As UIControls.ImageButton
    Friend WithEvents gbxDestinationCollection As System.Windows.Forms.GroupBox
    Friend WithEvents lblCurrProteinCount As System.Windows.Forms.Label
    Friend WithEvents lvwDestination As System.Windows.Forms.ListView
    Friend WithEvents colName As System.Windows.Forms.ColumnHeader
    Friend WithEvents pnlProgBarLower As System.Windows.Forms.Panel
    Friend WithEvents pgbMain As System.Windows.Forms.ProgressBar
    Friend WithEvents pnlSource As System.Windows.Forms.Panel
    Friend WithEvents SourceDestSplit As System.Windows.Forms.Splitter
    Friend WithEvents pnlDest As System.Windows.Forms.Panel
    Friend WithEvents pnlProgBarUpper As System.Windows.Forms.Panel
    Friend WithEvents lblCurrentTask As System.Windows.Forms.Label
    Friend WithEvents mnuMainGUI As System.Windows.Forms.MainMenu
    Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuHelp As System.Windows.Forms.MenuItem
    Friend WithEvents mnuTools As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdmin As System.Windows.Forms.MenuItem
    Friend WithEvents mnuFileExit As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsNucToProt As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsConvert As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsConvertF2A As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsConvertA2F As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsFCheckup As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsOptions As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsCollectionEdit As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsCompareDBs As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsExtractFromFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuHelpAbout As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminNameHashRefresh As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsSep1 As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminBatchUploadFiles As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminUpdateSHA As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminUpdateCollectionsArchive As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminUpdateZeroedMasses As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminTestingInterface As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminFixArchivePaths As System.Windows.Forms.MenuItem
    Friend WithEvents mnuAdminAddSortingIndexes As System.Windows.Forms.MenuItem
    Friend WithEvents lblBatchProgress As System.Windows.Forms.Label
    Friend WithEvents cmdExportToFile As System.Windows.Forms.Button
    Friend WithEvents cmdSaveDestCollection As System.Windows.Forms.Button
    Friend WithEvents VisualStyleProvider2 As Skybound.VisualStyles.VisualStyleProvider


    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCollectionEditor))
        Me.pnlProgBar = New System.Windows.Forms.Panel
        Me.pnlProgBarUpper = New System.Windows.Forms.Panel
        Me.lblBatchProgress = New System.Windows.Forms.Label
        Me.lblCurrentTask = New System.Windows.Forms.Label
        Me.pnlProgBarLower = New System.Windows.Forms.Panel
        Me.pgbMain = New System.Windows.Forms.ProgressBar
        Me.pnlSource = New System.Windows.Forms.Panel
        Me.cmdDestAdd = New UIControls.ImageButton
        Me.cmdDestRemove = New UIControls.ImageButton
        Me.cmdDestAddAll = New UIControls.ImageButton
        Me.cmdDestRemoveAll = New UIControls.ImageButton
        Me.gbxSourceCollection = New System.Windows.Forms.GroupBox
        Me.cboAuthorityPicker = New System.Windows.Forms.ComboBox
        Me.lblAuthorityFilter = New System.Windows.Forms.Label
        Me.pbxLiveSearchCancel = New System.Windows.Forms.PictureBox
        Me.lblSearchCount = New System.Windows.Forms.Label
        Me.cmdLoadProteins = New System.Windows.Forms.Button
        Me.cmdLoadFile = New System.Windows.Forms.Button
        Me.txtLiveSearch = New System.Windows.Forms.TextBox
        Me.cboCollectionPicker = New System.Windows.Forms.ComboBox
        Me.cboOrganismFilter = New System.Windows.Forms.ComboBox
        Me.lblOrganismFilter = New System.Windows.Forms.Label
        Me.lblCollectionPicker = New System.Windows.Forms.Label
        Me.pbxLiveSearchBkg = New System.Windows.Forms.PictureBox
        Me.lvwSource = New System.Windows.Forms.ListView
        Me.colSrcName = New System.Windows.Forms.ColumnHeader
        Me.colSrcDesc = New System.Windows.Forms.ColumnHeader
        Me.lblSourceMembers = New System.Windows.Forms.Label
        Me.SourceDestSplit = New System.Windows.Forms.Splitter
        Me.pnlDest = New System.Windows.Forms.Panel
        Me.gbxDestinationCollection = New System.Windows.Forms.GroupBox
        Me.cmdSaveDestCollection = New System.Windows.Forms.Button
        Me.cmdExportToFile = New System.Windows.Forms.Button
        Me.lblCurrProteinCount = New System.Windows.Forms.Label
        Me.lvwDestination = New System.Windows.Forms.ListView
        Me.colName = New System.Windows.Forms.ColumnHeader
        Me.mnuMainGUI = New System.Windows.Forms.MainMenu(Me.components)
        Me.mnuFile = New System.Windows.Forms.MenuItem
        Me.mnuFileExit = New System.Windows.Forms.MenuItem
        Me.mnuTools = New System.Windows.Forms.MenuItem
        Me.mnuToolsCollectionEdit = New System.Windows.Forms.MenuItem
        Me.mnuToolsNucToProt = New System.Windows.Forms.MenuItem
        Me.mnuToolsConvert = New System.Windows.Forms.MenuItem
        Me.mnuToolsConvertF2A = New System.Windows.Forms.MenuItem
        Me.mnuToolsConvertA2F = New System.Windows.Forms.MenuItem
        Me.mnuToolsFCheckup = New System.Windows.Forms.MenuItem
        Me.mnuToolsCompareDBs = New System.Windows.Forms.MenuItem
        Me.mnuToolsExtractFromFile = New System.Windows.Forms.MenuItem
        Me.mnuToolsSep1 = New System.Windows.Forms.MenuItem
        Me.mnuToolsOptions = New System.Windows.Forms.MenuItem
        Me.mnuAdmin = New System.Windows.Forms.MenuItem
        Me.mnuAdminBatchUploadFiles = New System.Windows.Forms.MenuItem
        Me.mnuAdminNameHashRefresh = New System.Windows.Forms.MenuItem
        Me.mnuAdminUpdateSHA = New System.Windows.Forms.MenuItem
        Me.mnuAdminUpdateCollectionsArchive = New System.Windows.Forms.MenuItem
        Me.mnuAdminUpdateZeroedMasses = New System.Windows.Forms.MenuItem
        Me.mnuAdminTestingInterface = New System.Windows.Forms.MenuItem
        Me.mnuAdminFixArchivePaths = New System.Windows.Forms.MenuItem
        Me.mnuAdminAddSortingIndexes = New System.Windows.Forms.MenuItem
        Me.mnuHelp = New System.Windows.Forms.MenuItem
        Me.mnuHelpAbout = New System.Windows.Forms.MenuItem
        Me.VisualStyleProvider2 = New Skybound.VisualStyles.VisualStyleProvider
        Me.pnlProgBar.SuspendLayout()
        Me.pnlProgBarUpper.SuspendLayout()
        Me.pnlProgBarLower.SuspendLayout()
        Me.pnlSource.SuspendLayout()
        Me.gbxSourceCollection.SuspendLayout()
        CType(Me.pbxLiveSearchCancel, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbxLiveSearchBkg, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlDest.SuspendLayout()
        Me.gbxDestinationCollection.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlProgBar
        '
        Me.pnlProgBar.Controls.Add(Me.pnlProgBarUpper)
        Me.pnlProgBar.Controls.Add(Me.pnlProgBarLower)
        Me.pnlProgBar.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlProgBar.Location = New System.Drawing.Point(0, 565)
        Me.pnlProgBar.Name = "pnlProgBar"
        Me.pnlProgBar.Size = New System.Drawing.Size(812, 76)
        Me.pnlProgBar.TabIndex = 0
        Me.pnlProgBar.Visible = False
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pnlProgBar, True)
        '
        'pnlProgBarUpper
        '
        Me.pnlProgBarUpper.Controls.Add(Me.lblBatchProgress)
        Me.pnlProgBarUpper.Controls.Add(Me.lblCurrentTask)
        Me.pnlProgBarUpper.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlProgBarUpper.Location = New System.Drawing.Point(0, 0)
        Me.pnlProgBarUpper.Name = "pnlProgBarUpper"
        Me.pnlProgBarUpper.Padding = New System.Windows.Forms.Padding(6)
        Me.pnlProgBarUpper.Size = New System.Drawing.Size(812, 42)
        Me.pnlProgBarUpper.TabIndex = 2
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pnlProgBarUpper, True)
        '
        'lblBatchProgress
        '
        Me.lblBatchProgress.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBatchProgress.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblBatchProgress.Location = New System.Drawing.Point(6, 20)
        Me.lblBatchProgress.Name = "lblBatchProgress"
        Me.lblBatchProgress.Size = New System.Drawing.Size(800, 16)
        Me.lblBatchProgress.TabIndex = 16
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblBatchProgress, True)
        '
        'lblCurrentTask
        '
        Me.lblCurrentTask.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblCurrentTask.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrentTask.Location = New System.Drawing.Point(6, 6)
        Me.lblCurrentTask.Name = "lblCurrentTask"
        Me.lblCurrentTask.Size = New System.Drawing.Size(800, 14)
        Me.lblCurrentTask.TabIndex = 15
        Me.lblCurrentTask.Text = "Reading Source File..."
        Me.lblCurrentTask.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        Me.lblCurrentTask.Visible = False
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblCurrentTask, True)
        '
        'pnlProgBarLower
        '
        Me.pnlProgBarLower.Controls.Add(Me.pgbMain)
        Me.pnlProgBarLower.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlProgBarLower.Location = New System.Drawing.Point(0, 42)
        Me.pnlProgBarLower.Name = "pnlProgBarLower"
        Me.pnlProgBarLower.Padding = New System.Windows.Forms.Padding(6)
        Me.pnlProgBarLower.Size = New System.Drawing.Size(812, 34)
        Me.pnlProgBarLower.TabIndex = 1
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pnlProgBarLower, True)
        '
        'pgbMain
        '
        Me.pgbMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pgbMain.Location = New System.Drawing.Point(6, 6)
        Me.pgbMain.Name = "pgbMain"
        Me.pgbMain.Size = New System.Drawing.Size(800, 22)
        Me.pgbMain.TabIndex = 14
        Me.pgbMain.Visible = False
        '
        'pnlSource
        '
        Me.pnlSource.Controls.Add(Me.cmdDestAdd)
        Me.pnlSource.Controls.Add(Me.cmdDestRemove)
        Me.pnlSource.Controls.Add(Me.cmdDestAddAll)
        Me.pnlSource.Controls.Add(Me.cmdDestRemoveAll)
        Me.pnlSource.Controls.Add(Me.gbxSourceCollection)
        Me.pnlSource.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlSource.Location = New System.Drawing.Point(0, 0)
        Me.pnlSource.Name = "pnlSource"
        Me.pnlSource.Padding = New System.Windows.Forms.Padding(8, 8, 8, 10)
        Me.pnlSource.Size = New System.Drawing.Size(544, 565)
        Me.pnlSource.TabIndex = 1
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pnlSource, True)
        '
        'cmdDestAdd
        '
        Me.cmdDestAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdDestAdd.Enabled = False
        Me.cmdDestAdd.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdDestAdd.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDestAdd.ForeColor = System.Drawing.SystemColors.Highlight
        Me.cmdDestAdd.Location = New System.Drawing.Point(496, 278)
        Me.cmdDestAdd.Name = "cmdDestAdd"
        Me.cmdDestAdd.Size = New System.Drawing.Size(38, 32)
        Me.cmdDestAdd.TabIndex = 5
        Me.cmdDestAdd.ThemedImage = CType(resources.GetObject("cmdDestAdd.ThemedImage"), System.Drawing.Bitmap)
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdDestAdd, False)
        '
        'cmdDestRemove
        '
        Me.cmdDestRemove.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdDestRemove.Enabled = False
        Me.cmdDestRemove.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdDestRemove.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDestRemove.ForeColor = System.Drawing.SystemColors.Highlight
        Me.cmdDestRemove.Location = New System.Drawing.Point(496, 327)
        Me.cmdDestRemove.Name = "cmdDestRemove"
        Me.cmdDestRemove.Size = New System.Drawing.Size(38, 33)
        Me.cmdDestRemove.TabIndex = 6
        Me.cmdDestRemove.ThemedImage = CType(resources.GetObject("cmdDestRemove.ThemedImage"), System.Drawing.Bitmap)
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdDestRemove, False)
        '
        'cmdDestAddAll
        '
        Me.cmdDestAddAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdDestAddAll.Enabled = False
        Me.cmdDestAddAll.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdDestAddAll.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDestAddAll.ForeColor = System.Drawing.SystemColors.Highlight
        Me.cmdDestAddAll.Location = New System.Drawing.Point(496, 228)
        Me.cmdDestAddAll.Name = "cmdDestAddAll"
        Me.cmdDestAddAll.Size = New System.Drawing.Size(38, 33)
        Me.cmdDestAddAll.TabIndex = 3
        Me.cmdDestAddAll.ThemedImage = CType(resources.GetObject("cmdDestAddAll.ThemedImage"), System.Drawing.Bitmap)
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdDestAddAll, False)
        '
        'cmdDestRemoveAll
        '
        Me.cmdDestRemoveAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdDestRemoveAll.Enabled = False
        Me.cmdDestRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdDestRemoveAll.Font = New System.Drawing.Font("Tahoma", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdDestRemoveAll.ForeColor = System.Drawing.SystemColors.Highlight
        Me.cmdDestRemoveAll.Location = New System.Drawing.Point(496, 377)
        Me.cmdDestRemoveAll.Name = "cmdDestRemoveAll"
        Me.cmdDestRemoveAll.Size = New System.Drawing.Size(38, 32)
        Me.cmdDestRemoveAll.TabIndex = 4
        Me.cmdDestRemoveAll.ThemedImage = CType(resources.GetObject("cmdDestRemoveAll.ThemedImage"), System.Drawing.Bitmap)
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdDestRemoveAll, False)
        '
        'gbxSourceCollection
        '
        Me.gbxSourceCollection.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxSourceCollection.Controls.Add(Me.cboAuthorityPicker)
        Me.gbxSourceCollection.Controls.Add(Me.lblAuthorityFilter)
        Me.gbxSourceCollection.Controls.Add(Me.pbxLiveSearchCancel)
        Me.gbxSourceCollection.Controls.Add(Me.lblSearchCount)
        Me.gbxSourceCollection.Controls.Add(Me.cmdLoadProteins)
        Me.gbxSourceCollection.Controls.Add(Me.cmdLoadFile)
        Me.gbxSourceCollection.Controls.Add(Me.txtLiveSearch)
        Me.gbxSourceCollection.Controls.Add(Me.cboCollectionPicker)
        Me.gbxSourceCollection.Controls.Add(Me.cboOrganismFilter)
        Me.gbxSourceCollection.Controls.Add(Me.lblOrganismFilter)
        Me.gbxSourceCollection.Controls.Add(Me.lblCollectionPicker)
        Me.gbxSourceCollection.Controls.Add(Me.pbxLiveSearchBkg)
        Me.gbxSourceCollection.Controls.Add(Me.lvwSource)
        Me.gbxSourceCollection.Controls.Add(Me.lblSourceMembers)
        Me.gbxSourceCollection.Location = New System.Drawing.Point(8, 8)
        Me.gbxSourceCollection.Name = "gbxSourceCollection"
        Me.gbxSourceCollection.Size = New System.Drawing.Size(472, 547)
        Me.gbxSourceCollection.TabIndex = 1
        Me.gbxSourceCollection.TabStop = False
        Me.gbxSourceCollection.Text = "Source Collection"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.gbxSourceCollection, True)
        '
        'cboAuthorityPicker
        '
        Me.cboAuthorityPicker.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboAuthorityPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAuthorityPicker.Location = New System.Drawing.Point(240, 46)
        Me.cboAuthorityPicker.Name = "cboAuthorityPicker"
        Me.cboAuthorityPicker.Size = New System.Drawing.Size(216, 21)
        Me.cboAuthorityPicker.TabIndex = 17
        '
        'lblAuthorityFilter
        '
        Me.lblAuthorityFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblAuthorityFilter.Location = New System.Drawing.Point(240, 31)
        Me.lblAuthorityFilter.Name = "lblAuthorityFilter"
        Me.lblAuthorityFilter.Size = New System.Drawing.Size(212, 17)
        Me.lblAuthorityFilter.TabIndex = 18
        Me.lblAuthorityFilter.Text = "Naming Authority Filter"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblAuthorityFilter, True)
        '
        'pbxLiveSearchCancel
        '
        Me.pbxLiveSearchCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pbxLiveSearchCancel.BackColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.pbxLiveSearchCancel.Image = CType(resources.GetObject("pbxLiveSearchCancel.Image"), System.Drawing.Image)
        Me.pbxLiveSearchCancel.Location = New System.Drawing.Point(194, 517)
        Me.pbxLiveSearchCancel.Name = "pbxLiveSearchCancel"
        Me.pbxLiveSearchCancel.Size = New System.Drawing.Size(16, 17)
        Me.pbxLiveSearchCancel.TabIndex = 16
        Me.pbxLiveSearchCancel.TabStop = False
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pbxLiveSearchCancel, True)
        '
        'lblSearchCount
        '
        Me.lblSearchCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSearchCount.Location = New System.Drawing.Point(224, 521)
        Me.lblSearchCount.Name = "lblSearchCount"
        Me.lblSearchCount.Size = New System.Drawing.Size(88, 16)
        Me.lblSearchCount.TabIndex = 15
        Me.lblSearchCount.Text = "30000/30000"
        Me.lblSearchCount.TextAlign = System.Drawing.ContentAlignment.TopCenter
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblSearchCount, True)
        '
        'cmdLoadProteins
        '
        Me.cmdLoadProteins.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdLoadProteins.Location = New System.Drawing.Point(352, 85)
        Me.cmdLoadProteins.Name = "cmdLoadProteins"
        Me.cmdLoadProteins.Size = New System.Drawing.Size(102, 24)
        Me.cmdLoadProteins.TabIndex = 14
        Me.cmdLoadProteins.Text = "Load Proteins"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdLoadProteins, True)
        '
        'cmdLoadFile
        '
        Me.cmdLoadFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdLoadFile.Location = New System.Drawing.Point(316, 514)
        Me.cmdLoadFile.Name = "cmdLoadFile"
        Me.cmdLoadFile.Size = New System.Drawing.Size(140, 24)
        Me.cmdLoadFile.TabIndex = 10
        Me.cmdLoadFile.Text = "&Import New Collection..."
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdLoadFile, True)
        '
        'txtLiveSearch
        '
        Me.txtLiveSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtLiveSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
        Me.txtLiveSearch.Location = New System.Drawing.Point(38, 519)
        Me.txtLiveSearch.Name = "txtLiveSearch"
        Me.txtLiveSearch.Size = New System.Drawing.Size(154, 14)
        Me.txtLiveSearch.TabIndex = 8
        Me.txtLiveSearch.Text = "Search"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.txtLiveSearch, True)
        '
        'cboCollectionPicker
        '
        Me.cboCollectionPicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboCollectionPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCollectionPicker.Location = New System.Drawing.Point(14, 85)
        Me.cboCollectionPicker.Name = "cboCollectionPicker"
        Me.cboCollectionPicker.Size = New System.Drawing.Size(328, 21)
        Me.cboCollectionPicker.TabIndex = 1
        '
        'cboOrganismFilter
        '
        Me.cboOrganismFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboOrganismFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOrganismFilter.Location = New System.Drawing.Point(14, 46)
        Me.cboOrganismFilter.Name = "cboOrganismFilter"
        Me.cboOrganismFilter.Size = New System.Drawing.Size(216, 21)
        Me.cboOrganismFilter.TabIndex = 0
        '
        'lblOrganismFilter
        '
        Me.lblOrganismFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblOrganismFilter.Location = New System.Drawing.Point(14, 31)
        Me.lblOrganismFilter.Name = "lblOrganismFilter"
        Me.lblOrganismFilter.Size = New System.Drawing.Size(212, 17)
        Me.lblOrganismFilter.TabIndex = 3
        Me.lblOrganismFilter.Text = "Organism Selector"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblOrganismFilter, True)
        '
        'lblCollectionPicker
        '
        Me.lblCollectionPicker.Location = New System.Drawing.Point(14, 64)
        Me.lblCollectionPicker.Name = "lblCollectionPicker"
        Me.lblCollectionPicker.Size = New System.Drawing.Size(100, 16)
        Me.lblCollectionPicker.TabIndex = 4
        Me.lblCollectionPicker.Text = "Protein Collection"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblCollectionPicker, True)
        '
        'pbxLiveSearchBkg
        '
        Me.pbxLiveSearchBkg.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pbxLiveSearchBkg.Image = CType(resources.GetObject("pbxLiveSearchBkg.Image"), System.Drawing.Image)
        Me.pbxLiveSearchBkg.Location = New System.Drawing.Point(16, 513)
        Me.pbxLiveSearchBkg.Name = "pbxLiveSearchBkg"
        Me.pbxLiveSearchBkg.Size = New System.Drawing.Size(200, 26)
        Me.pbxLiveSearchBkg.TabIndex = 9
        Me.pbxLiveSearchBkg.TabStop = False
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pbxLiveSearchBkg, True)
        '
        'lvwSource
        '
        Me.lvwSource.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwSource.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colSrcName, Me.colSrcDesc})
        Me.lvwSource.FullRowSelect = True
        Me.lvwSource.GridLines = True
        Me.lvwSource.Location = New System.Drawing.Point(14, 130)
        Me.lvwSource.Name = "lvwSource"
        Me.lvwSource.Size = New System.Drawing.Size(442, 376)
        Me.lvwSource.TabIndex = 2
        Me.lvwSource.UseCompatibleStateImageBehavior = False
        Me.lvwSource.View = System.Windows.Forms.View.Details
        '
        'colSrcName
        '
        Me.colSrcName.Text = "Name"
        Me.colSrcName.Width = 117
        '
        'colSrcDesc
        '
        Me.colSrcDesc.Text = "Description"
        Me.colSrcDesc.Width = 320
        '
        'lblSourceMembers
        '
        Me.lblSourceMembers.Location = New System.Drawing.Point(14, 108)
        Me.lblSourceMembers.Name = "lblSourceMembers"
        Me.lblSourceMembers.Size = New System.Drawing.Size(128, 16)
        Me.lblSourceMembers.TabIndex = 5
        Me.lblSourceMembers.Text = "Collection Members"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblSourceMembers, True)
        '
        'SourceDestSplit
        '
        Me.SourceDestSplit.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.SourceDestSplit.Location = New System.Drawing.Point(544, 0)
        Me.SourceDestSplit.MinExtra = 265
        Me.SourceDestSplit.MinSize = 450
        Me.SourceDestSplit.Name = "SourceDestSplit"
        Me.SourceDestSplit.Size = New System.Drawing.Size(3, 565)
        Me.SourceDestSplit.TabIndex = 2
        Me.SourceDestSplit.TabStop = False
        '
        'pnlDest
        '
        Me.pnlDest.Controls.Add(Me.gbxDestinationCollection)
        Me.pnlDest.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlDest.Location = New System.Drawing.Point(547, 0)
        Me.pnlDest.Name = "pnlDest"
        Me.pnlDest.Padding = New System.Windows.Forms.Padding(8, 8, 8, 10)
        Me.pnlDest.Size = New System.Drawing.Size(265, 565)
        Me.pnlDest.TabIndex = 3
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.pnlDest, True)
        '
        'gbxDestinationCollection
        '
        Me.gbxDestinationCollection.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxDestinationCollection.Controls.Add(Me.cmdSaveDestCollection)
        Me.gbxDestinationCollection.Controls.Add(Me.cmdExportToFile)
        Me.gbxDestinationCollection.Controls.Add(Me.lblCurrProteinCount)
        Me.gbxDestinationCollection.Controls.Add(Me.lvwDestination)
        Me.gbxDestinationCollection.Location = New System.Drawing.Point(8, 8)
        Me.gbxDestinationCollection.Name = "gbxDestinationCollection"
        Me.gbxDestinationCollection.Size = New System.Drawing.Size(249, 547)
        Me.gbxDestinationCollection.TabIndex = 2
        Me.gbxDestinationCollection.TabStop = False
        Me.gbxDestinationCollection.Text = "Destination Collection"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.gbxDestinationCollection, True)
        '
        'cmdSaveDestCollection
        '
        Me.cmdSaveDestCollection.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSaveDestCollection.Location = New System.Drawing.Point(124, 512)
        Me.cmdSaveDestCollection.Name = "cmdSaveDestCollection"
        Me.cmdSaveDestCollection.Size = New System.Drawing.Size(114, 24)
        Me.cmdSaveDestCollection.TabIndex = 4
        Me.cmdSaveDestCollection.Text = "&Upload Collection..."
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdSaveDestCollection, True)
        '
        'cmdExportToFile
        '
        Me.cmdExportToFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdExportToFile.Enabled = False
        Me.cmdExportToFile.Location = New System.Drawing.Point(14, 512)
        Me.cmdExportToFile.Name = "cmdExportToFile"
        Me.cmdExportToFile.Size = New System.Drawing.Size(102, 24)
        Me.cmdExportToFile.TabIndex = 3
        Me.cmdExportToFile.Text = "Export to File..."
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.cmdExportToFile, True)
        '
        'lblCurrProteinCount
        '
        Me.lblCurrProteinCount.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrProteinCount.Location = New System.Drawing.Point(14, 19)
        Me.lblCurrProteinCount.Name = "lblCurrProteinCount"
        Me.lblCurrProteinCount.Size = New System.Drawing.Size(164, 15)
        Me.lblCurrProteinCount.TabIndex = 2
        Me.lblCurrProteinCount.Text = "Protein Count: 0"
        Me.VisualStyleProvider2.SetVisualStyleSupport(Me.lblCurrProteinCount, True)
        '
        'lvwDestination
        '
        Me.lvwDestination.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwDestination.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colName})
        Me.lvwDestination.FullRowSelect = True
        Me.lvwDestination.GridLines = True
        Me.lvwDestination.Location = New System.Drawing.Point(14, 53)
        Me.lvwDestination.Name = "lvwDestination"
        Me.lvwDestination.Size = New System.Drawing.Size(221, 453)
        Me.lvwDestination.TabIndex = 0
        Me.lvwDestination.UseCompatibleStateImageBehavior = False
        Me.lvwDestination.View = System.Windows.Forms.View.Details
        '
        'colName
        '
        Me.colName.Text = "Name"
        Me.colName.Width = 228
        '
        'mnuMainGUI
        '
        Me.mnuMainGUI.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFile, Me.mnuTools, Me.mnuAdmin, Me.mnuHelp})
        '
        'mnuFile
        '
        Me.mnuFile.Index = 0
        Me.mnuFile.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFileExit})
        Me.mnuFile.Text = "&File"
        '
        'mnuFileExit
        '
        Me.mnuFileExit.Index = 0
        Me.mnuFileExit.Text = "E&xit"
        '
        'mnuTools
        '
        Me.mnuTools.Index = 1
        Me.mnuTools.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuToolsCollectionEdit, Me.mnuToolsNucToProt, Me.mnuToolsConvert, Me.mnuToolsFCheckup, Me.mnuToolsCompareDBs, Me.mnuToolsExtractFromFile, Me.mnuToolsSep1, Me.mnuToolsOptions})
        Me.mnuTools.Text = "&Tools"
        '
        'mnuToolsCollectionEdit
        '
        Me.mnuToolsCollectionEdit.Index = 0
        Me.mnuToolsCollectionEdit.Text = "&Edit Collection States..."
        '
        'mnuToolsNucToProt
        '
        Me.mnuToolsNucToProt.Index = 1
        Me.mnuToolsNucToProt.Text = "Translate Nucleotides to Proteins..."
        Me.mnuToolsNucToProt.Visible = False
        '
        'mnuToolsConvert
        '
        Me.mnuToolsConvert.Index = 2
        Me.mnuToolsConvert.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuToolsConvertF2A, Me.mnuToolsConvertA2F})
        Me.mnuToolsConvert.Text = "Convert Database Format..."
        Me.mnuToolsConvert.Visible = False
        '
        'mnuToolsConvertF2A
        '
        Me.mnuToolsConvertF2A.Index = 0
        Me.mnuToolsConvertF2A.Text = "FASTA to Access..."
        '
        'mnuToolsConvertA2F
        '
        Me.mnuToolsConvertA2F.Index = 1
        Me.mnuToolsConvertA2F.Text = "Access to FASTA..."
        '
        'mnuToolsFCheckup
        '
        Me.mnuToolsFCheckup.Index = 3
        Me.mnuToolsFCheckup.Text = "Perform FASTA File Checkup..."
        Me.mnuToolsFCheckup.Visible = False
        '
        'mnuToolsCompareDBs
        '
        Me.mnuToolsCompareDBs.Index = 4
        Me.mnuToolsCompareDBs.Text = "Compare Databases..."
        Me.mnuToolsCompareDBs.Visible = False
        '
        'mnuToolsExtractFromFile
        '
        Me.mnuToolsExtractFromFile.Enabled = False
        Me.mnuToolsExtractFromFile.Index = 5
        Me.mnuToolsExtractFromFile.Text = "Extract Annotations from Text File..."
        '
        'mnuToolsSep1
        '
        Me.mnuToolsSep1.Index = 6
        Me.mnuToolsSep1.Text = "-"
        '
        'mnuToolsOptions
        '
        Me.mnuToolsOptions.Enabled = False
        Me.mnuToolsOptions.Index = 7
        Me.mnuToolsOptions.Text = "Options..."
        '
        'mnuAdmin
        '
        Me.mnuAdmin.Index = 2
        Me.mnuAdmin.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuAdminBatchUploadFiles, Me.mnuAdminNameHashRefresh, Me.mnuAdminUpdateSHA, Me.mnuAdminUpdateCollectionsArchive, Me.mnuAdminUpdateZeroedMasses, Me.mnuAdminTestingInterface, Me.mnuAdminFixArchivePaths, Me.mnuAdminAddSortingIndexes})
        Me.mnuAdmin.Text = "Admin"
        '
        'mnuAdminBatchUploadFiles
        '
        Me.mnuAdminBatchUploadFiles.Index = 0
        Me.mnuAdminBatchUploadFiles.Text = "Batch Upload FASTA Files Using DMS..."
        Me.mnuAdminBatchUploadFiles.Visible = False
        '
        'mnuAdminNameHashRefresh
        '
        Me.mnuAdminNameHashRefresh.Index = 1
        Me.mnuAdminNameHashRefresh.Text = "Refresh Protein Name Hashes"
        '
        'mnuAdminUpdateSHA
        '
        Me.mnuAdminUpdateSHA.Enabled = False
        Me.mnuAdminUpdateSHA.Index = 2
        Me.mnuAdminUpdateSHA.Text = "Update File Authentication Hashes"
        Me.mnuAdminUpdateSHA.Visible = False
        '
        'mnuAdminUpdateCollectionsArchive
        '
        Me.mnuAdminUpdateCollectionsArchive.Enabled = False
        Me.mnuAdminUpdateCollectionsArchive.Index = 3
        Me.mnuAdminUpdateCollectionsArchive.Text = "Update Collections Archive"
        Me.mnuAdminUpdateCollectionsArchive.Visible = False
        '
        'mnuAdminUpdateZeroedMasses
        '
        Me.mnuAdminUpdateZeroedMasses.Index = 4
        Me.mnuAdminUpdateZeroedMasses.Text = "Update Zeroed Masses"
        Me.mnuAdminUpdateZeroedMasses.Visible = False
        '
        'mnuAdminTestingInterface
        '
        Me.mnuAdminTestingInterface.Index = 5
        Me.mnuAdminTestingInterface.Text = "Testing Interface Window..."
        '
        'mnuAdminFixArchivePaths
        '
        Me.mnuAdminFixArchivePaths.Enabled = False
        Me.mnuAdminFixArchivePaths.Index = 6
        Me.mnuAdminFixArchivePaths.Text = "Fix Archive Path Names"
        Me.mnuAdminFixArchivePaths.Visible = False
        '
        'mnuAdminAddSortingIndexes
        '
        Me.mnuAdminAddSortingIndexes.Index = 7
        Me.mnuAdminAddSortingIndexes.Text = "Add Sorting Indexes"
        Me.mnuAdminAddSortingIndexes.Visible = False
        '
        'mnuHelp
        '
        Me.mnuHelp.Index = 3
        Me.mnuHelp.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuHelpAbout})
        Me.mnuHelp.Text = "&Help"
        '
        'mnuHelpAbout
        '
        Me.mnuHelpAbout.Index = 0
        Me.mnuHelpAbout.Text = "&About Protein Collection Editor"
        '
        'frmCollectionEditor
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(812, 641)
        Me.Controls.Add(Me.pnlDest)
        Me.Controls.Add(Me.SourceDestSplit)
        Me.Controls.Add(Me.pnlSource)
        Me.Controls.Add(Me.pnlProgBar)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Menu = Me.mnuMainGUI
        Me.MinimumSize = New System.Drawing.Size(820, 675)
        Me.Name = "frmCollectionEditor"
        Me.Text = "Protein Collection Editor"
        Me.pnlProgBar.ResumeLayout(False)
        Me.pnlProgBarUpper.ResumeLayout(False)
        Me.pnlProgBarLower.ResumeLayout(False)
        Me.pnlSource.ResumeLayout(False)
        Me.gbxSourceCollection.ResumeLayout(False)
        Me.gbxSourceCollection.PerformLayout()
        CType(Me.pbxLiveSearchCancel, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbxLiveSearchBkg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlDest.ResumeLayout(False)
        Me.gbxDestinationCollection.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    <System.STAThread()> Public Shared Sub Main()
        'System.Windows.Forms.Application.EnableVisualStyles()
        Skybound.VisualStyles.VisualStyleProvider.EnableVisualStyles()
        System.Windows.Forms.Application.DoEvents()
        System.Windows.Forms.Application.Run(New frmCollectionEditor)  ' replace frmDecode by the name of your form!!!
    End Sub


#End Region

	Protected Const PROGRAM_DATE As String = "February 2, 2012"

    Protected m_Organisms As DataTable
    Protected m_ProteinCollections As DataTable
    Protected m_ProteinCollectionNames As DataTable
    Protected m_SelectedOrgCollections As DataTable
    Protected m_AnnotationTypes As DataTable
    Protected m_CollectionMembers As DataTable
    Protected m_SelectedOrganismID As Integer
    Protected m_SelectedAnnotationTypeID As Integer
    Protected m_SelectedFilePath As String
    Protected m_SelectedCollectionID As Integer
    Protected m_LastBatchULDirectoryPath As String
    'Protected m_PSConnectionString As String = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences_Test;Integrated Security=SSPI;"
    Protected m_PSConnectionString As String = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;"
    'Protected m_PSConnectionString As String = "Data Source=we10125;Initial Catalog=Protein_Sequences_T3;Integrated Security=SSPI;"

    Protected m_LastSelectedOrganism As String = ""
    Protected m_LastSelectedAnnotationType As String = ""
    Protected m_LastValueForAllowAsterisks As Boolean = False
	Protected m_LastValueForAllowDash As Boolean = False
	Protected m_LastValueForMaxProteinNameLength As Integer = ValidateFastaFile.clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH

    Protected WithEvents m_ImportHandler As Protein_Importer.IImportProteins
    Protected WithEvents m_UploadHandler As Protein_Uploader.IUploadProteins
    Protected WithEvents m_SourceListViewHandler As DataListViewHandler
    Protected WithEvents m_ProteinExporter As Protein_Exporter.ExportProteinCollectionsIFC.IExportProteins
    'Protected WithEvents m_Validator As ValidateFastaFile.ICustomValidation
    Protected WithEvents m_fileBatcher As Protein_Uploader.clsBatchUploadFromFileList

    Protected CollectionLoadThread As System.Threading.Thread

    Protected m_LocalFileLoaded As Boolean

    Private m_SearchActive As Boolean = False
    Private m_LoadMembers As Boolean = False

    Private m_BatchLoadTotalCount As Integer
    Private m_BatchLoadCurrentCount As Integer

    Private m_FileErrorList As Hashtable
    Private m_SummarizedFileErrorList As Hashtable

    Private m_FileWarningList As Hashtable
    Private m_SummarizedFileWarningList As Hashtable

    Private m_ValidUploadsList As Hashtable

    Protected WithEvents m_Syncer As clsSyncFASTAFileArchive

    Protected m_EncryptSequences As Boolean = False


    Friend WithEvents SearchTimer As New System.Timers.Timer(2000)
    Friend WithEvents MemberLoadTimer As New System.Timers.Timer(2000)

    Private Sub frmCollectionEditor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Get initial info - organism list, full collections list

        'TODO Need to get this from prefs file, not hard code
        Me.m_ImportHandler = New Protein_Importer.clsImportHandler(m_PSConnectionString)
        'Me.mnuToolsFBatchUpload.Enabled = False

        Me.lblBatchProgress.Text = "Fetching Organism and Collection Lists..."

        Me.cboCollectionPicker.Enabled = False
        Me.cboOrganismFilter.Enabled = False

        Me.TriggerCollectionTableUpdate(False)

        Me.m_SourceListViewHandler = New DataListViewHandler(Me.lvwSource)

        Me.cmdLoadProteins.Enabled = False
        Me.txtLiveSearch.Visible = False
        Me.pbxLiveSearchBkg.Visible = False
        Me.pbxLiveSearchCancel.Visible = False
        Me.lblSearchCount.Text = ""
        Me.cmdExportToFile.Enabled = False
        Me.cmdSaveDestCollection.Enabled = False

        AddHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged
        Me.lblBatchProgress.Text = ""

        Me.CheckTransferButtonsEnabledStatus()

        'Setup collections for selected organism

        'Use 2-3 second delay after collection change before refreshing member list


    End Sub

    Private Sub CheckTransferButtonsEnabledStatus()
        If Me.lvwSource.Items.Count > 0 Then
            Me.cmdDestAdd.Enabled = True
            Me.cmdDestAddAll.Enabled = True
        Else
            Me.cmdDestAdd.Enabled = False
            Me.cmdDestAddAll.Enabled = False
        End If

        If Me.lvwDestination.Items.Count > 0 Then
            Me.cmdDestRemove.Enabled = True
            Me.cmdDestRemoveAll.Enabled = True
        Else
            Me.cmdDestRemove.Enabled = False
            Me.cmdDestRemoveAll.Enabled = False
        End If
    End Sub

    Private Sub RefreshCollectionList()

        If Me.m_SelectedOrganismID <> -1 And Me.m_SelectedCollectionID <> -1 Then
            RemoveHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged
            RemoveHandler cboCollectionPicker.SelectedIndexChanged, AddressOf cboCollectionPicker_SelectedIndexChanged
            Me.cboOrganismFilter.SelectedItem = Me.m_SelectedOrganismID
            Me.cboOrganismList_SelectedIndexChanged(Me, Nothing)

            Me.cboCollectionPicker.SelectedItem = Me.m_SelectedCollectionID
            Me.cboAuthorityPicker.SelectedItem = Me.m_SelectedAnnotationTypeID
            Me.cboCollectionPicker.Select()
            AddHandler cboCollectionPicker.SelectedIndexChanged, AddressOf cboCollectionPicker_SelectedIndexChanged
            AddHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged
        End If

    End Sub

    Private Sub TriggerCollectionTableUpdate(ByVal RefreshTable As Boolean)
        If RefreshTable Then
            Me.m_ImportHandler.TriggerProteinCollectionTableUpdate()
        End If
        'Me.CollectionLoadThread = New System.Threading.Thread(AddressOf m_ImportHandler.TriggerProteinCollectionsLoad)
        'Me.CollectionLoadThread.Start()
        If Me.m_SelectedOrganismID > 0 Then
            Me.m_ImportHandler.TriggerProteinCollectionsLoad(Me.m_SelectedOrganismID)
        Else
            Me.m_ImportHandler.TriggerProteinCollectionsLoad()
        End If
    End Sub

    Protected Sub BindOrganismListToControl(ByVal organismList As DataTable)

        Me.cboOrganismFilter.BeginUpdate()
        With Me.cboOrganismFilter
            .DataSource = organismList
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
        End With
        Me.cboOrganismFilter.EndUpdate()

    End Sub

    Protected Sub BindAuthorityListToControl(ByVal authorityList As DataTable)
        Me.cboAuthorityPicker.BeginUpdate()

        With Me.cboAuthorityPicker
            .DisplayMember = "Display_Name"
            '.DisplayMember = "name"
            .ValueMember = "ID"
            .DataSource = authorityList
            .Refresh()

        End With
        Me.cboAuthorityPicker.EndUpdate()
    End Sub

    Protected Sub BindCollectionListToControl(ByVal collectionList As DataView)

        Me.cboCollectionPicker.BeginUpdate()
        If collectionList.Count = 0 Then
            With Me.cboCollectionPicker
                .DataSource = Nothing
                .Items.Add(" -- No Collections for this Organism -- ")
                .SelectedIndex = 0
                .Enabled = False
            End With
            Me.cmdLoadProteins.Enabled = False
            Me.txtLiveSearch.Visible = False
            Me.pbxLiveSearchBkg.Visible = False
            Me.pbxLiveSearchCancel.Visible = False
        Else
            With Me.cboCollectionPicker
                .Enabled = True
                .DataSource = collectionList
                .DisplayMember = "Display"
                .ValueMember = "Protein_Collection_ID"
            End With
            Me.cmdLoadProteins.Enabled = True
        End If
        Me.cboCollectionPicker.EndUpdate()

    End Sub

    Protected Sub BatchLoadController()
        Dim resultReturn As DialogResult

        Me.m_ProteinCollectionNames = Me.m_ImportHandler.LoadProteinCollectionNames

        If Not Me.m_FileErrorList Is Nothing Then
            Me.m_FileErrorList.Clear()
        End If

        If Not Me.m_FileWarningList Is Nothing Then
            Me.m_FileWarningList.Clear()
        End If

        If Not Me.m_ValidUploadsList Is Nothing Then
            Me.m_ValidUploadsList.Clear()
        End If

        If Not Me.m_SummarizedFileErrorList Is Nothing Then
            Me.m_SummarizedFileErrorList.Clear()
        End If

        If Not Me.m_SummarizedFileWarningList Is Nothing Then
            Me.m_SummarizedFileWarningList.Clear()
        End If

        Dim frmBatchUpload As New frmBatchAddNewCollectionTest( _
            Me.m_Organisms, _
            m_AnnotationTypes, _
            Me.m_ProteinCollectionNames, _
            Me.m_PSConnectionString)

        Dim tmpSelectedFileList As Hashtable

        Me.lblBatchProgress.Text = ""

        If Not m_LastSelectedOrganism Is Nothing AndAlso m_LastSelectedOrganism.Length > 0 Then
            frmBatchUpload.SelectedOrganismName = m_LastSelectedOrganism
        End If

        If Not m_LastSelectedAnnotationType Is Nothing AndAlso m_LastSelectedAnnotationType.Length > 0 Then
            frmBatchUpload.SelectedAnnotationType = m_LastSelectedAnnotationType
        End If

        frmBatchUpload.ValidationAllowAsterisks = m_LastValueForAllowAsterisks
		frmBatchUpload.ValidationAllowDash = m_LastValueForAllowDash
		frmBatchUpload.ValidationMaxProteinNameLength = m_LastValueForMaxProteinNameLength

        ' Set the last directory used
        frmBatchUpload.CurrentDirectory = Me.m_LastBatchULDirectoryPath

        ' Show the window
        resultReturn = frmBatchUpload.ShowDialog

        ' Save the selected organism and annotation type
        m_LastSelectedOrganism = frmBatchUpload.SelectedOrganismName
        m_LastSelectedAnnotationType = frmBatchUpload.SelectedAnnotationType
        m_LastValueForAllowAsterisks = frmBatchUpload.ValidationAllowAsterisks
		m_LastValueForAllowDash = frmBatchUpload.ValidationAllowDash
		m_LastValueForMaxProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength

		m_LastBatchULDirectoryPath = frmBatchUpload.CurrentDirectory

        Try
            ' Save these settings to the registry
			If Not String.IsNullOrEmpty(m_LastSelectedOrganism) Then
				SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", m_LastSelectedOrganism)
			End If

			If Not String.IsNullOrEmpty(m_LastSelectedAnnotationType) Then
				SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", m_LastSelectedAnnotationType)
			End If

			If Not String.IsNullOrEmpty(m_LastBatchULDirectoryPath) Then
				SaveSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", m_LastBatchULDirectoryPath)
			End If

        Catch ex As Exception
            ' Ignore errors here
        End Try



        If resultReturn = DialogResult.OK Then

            Me.gbxSourceCollection.Enabled = False
            Me.gbxDestinationCollection.Enabled = False
            Me.cmdDestAdd.Enabled = False
            Me.cmdDestAddAll.Enabled = False
            Me.cmdDestRemove.Enabled = False
            Me.cmdDestRemoveAll.Enabled = False

            tmpSelectedFileList = frmBatchUpload.FileList

            Me.m_BatchLoadTotalCount = tmpSelectedFileList.Count

            If Me.m_EncryptSequences Then
                Me.m_UploadHandler = New Protein_Uploader.clsPSUploadHandler(m_PSConnectionString)
            Else
                Me.m_UploadHandler = New Protein_Uploader.clsPSUploadHandler(m_PSConnectionString)
            End If
            Me.m_UploadHandler.InitialSetup()

            Me.pnlProgBar.Visible = True

            Try
                Me.m_UploadHandler.SetValidationOptions(Protein_Uploader.IUploadProteins.eValidationOptionConstants.AllowAsterisksInResidues, frmBatchUpload.ValidationAllowAsterisks)
                Me.m_UploadHandler.SetValidationOptions(Protein_Uploader.IUploadProteins.eValidationOptionConstants.AllowDashInResidues, frmBatchUpload.ValidationAllowDash)

				Me.m_UploadHandler.MaximumProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength

				Me.m_UploadHandler.BatchUpload(tmpSelectedFileList)

            Catch ex As Exception
                System.Windows.Forms.MessageBox.Show("Error uploading collection: " & ex.Message, "Error")
            End Try

            Me.pnlProgBar.Visible = False

            If Not Me.m_UploadHandler.ImportExportCountsMatched Then
                ' Number of files to load didn't match the number of files that succeeded
            End If

            ' Display any errors that occurred
            Dim errorDisplay As New frmValidationReport
            errorDisplay.FileErrorList = Me.m_FileErrorList
            errorDisplay.FileWarningList = Me.m_FileWarningList
            errorDisplay.FileValidList = Me.m_ValidUploadsList
            errorDisplay.ErrorSummaryList = Me.m_SummarizedFileErrorList
            errorDisplay.WarningSummaryList = Me.m_SummarizedFileWarningList
            errorDisplay.OrganismList = Me.m_Organisms
            errorDisplay.ShowDialog()

            Me.lblBatchProgress.Text = "Updating Protein Collections List..."
            System.Windows.Forms.Application.DoEvents()

            Me.TriggerCollectionTableUpdate(True)

            Me.RefreshCollectionList()
            Me.m_UploadHandler.ResetErrorList()

            Me.lblBatchProgress.Text = ""
            Me.gbxSourceCollection.Enabled = True
            Me.gbxDestinationCollection.Enabled = True
            Me.cmdDestAdd.Enabled = True
            Me.cmdDestAddAll.Enabled = True
            Me.cmdDestRemove.Enabled = True
            Me.cmdDestRemoveAll.Enabled = True

            Me.m_BatchLoadCurrentCount = 0
        End If
    End Sub

    Private Sub ReadSettings()
        Try
            m_LastSelectedOrganism = GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", "")
			m_LastSelectedAnnotationType = GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", "")
			m_LastBatchULDirectoryPath = GetSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", "")
        Catch ex As Exception
            ' Ignore errors here
        End Try
    End Sub

    Private Sub ShowAboutBox()
        '    Dim AboutBox As New frmAboutBox

        '    AboutBox.Location = Me.m_MainProcess.myAppSettings.AboutBoxLocation
        '    AboutBox.ShowDialog()

        Dim strMessage As String

        strMessage = "This is version " & Application.ProductVersion & ", " & PROGRAM_DATE

        System.Windows.Forms.MessageBox.Show(strMessage, "About Protein Collection Editor", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub

#Region " Combobox handlers "

    Private Sub cboOrganismList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If CInt(Me.cboOrganismFilter.SelectedValue) <> 0 Then
            Me.m_ProteinCollections.DefaultView.RowFilter = "[Organism_ID] = " & Me.cboOrganismFilter.SelectedValue.ToString
            Me.m_LoadMembers = True
        Else
            Me.m_ProteinCollections.DefaultView.RowFilter = ""
            Me.m_LoadMembers = False
        End If

        Me.m_SelectedOrganismID = CInt(Me.cboOrganismFilter.SelectedValue)

        Me.BindCollectionListToControl(Me.m_ProteinCollections.DefaultView)

        If Me.lvwSource.Items.Count = 0 Then
            Me.cboCollectionPicker_SelectedIndexChanged(Me, Nothing)

        End If
    End Sub

    Private Sub cboCollectionPicker_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.lvwSource.Items.Clear()
        Me.m_ImportHandler.ClearProteinCollection()
        Me.m_SelectedCollectionID = CInt(Me.cboCollectionPicker.SelectedValue)

        If Me.m_SelectedCollectionID > 0 Then
            Dim foundRows() As DataRow = Me.m_ProteinCollections.Select("[Protein_Collection_ID] = " & Me.m_SelectedCollectionID.ToString)
            Me.m_SelectedAnnotationTypeID = CInt(foundRows(0).Item("Authority_ID"))
            'Me.m_AnnotationTypes = Me.m_ImportHandler.LoadAnnotationTypes(Me.m_SelectedCollectionID)
            'Me.m_AnnotationTypes = Me.m_ImportHandler.LoadAnnotationTypes()
            Me.cmdLoadProteins.Enabled = True
        Else
            Me.m_AnnotationTypes = Me.m_ImportHandler.LoadAnnotationTypes
            Me.cmdLoadProteins.Enabled = False
        End If
        Me.BindAuthorityListToControl(Me.m_AnnotationTypes)
    End Sub

    Private Sub cboAuthorityPicker_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Me.lvwSource.Items.Count > 0 Then
            Me.lvwSource.Items.Clear()
            Me.m_ImportHandler.ClearProteinCollection()
        End If

        If Me.cboAuthorityPicker.SelectedValue.GetType Is System.Type.GetType("System.Int32") Then
            Me.m_SelectedAnnotationTypeID = CInt(Me.cboAuthorityPicker.SelectedValue)
        Else
            'Me.m_SelectedAuthorityID = 0
        End If

        If Me.m_SelectedCollectionID > 0 Then
            Dim foundRows() As DataRow = Me.m_ProteinCollections.Select("[Protein_Collection_ID] = " & Me.m_SelectedCollectionID.ToString)
            Me.m_SelectedAnnotationTypeID = CInt(foundRows(0).Item("Authority_ID"))
            'ElseIf Me.m_SelectedAuthorityID = -2 Then
            '    'Bring up addition dialog
            '    Dim AuthAdd As New clsAddNamingAuthority(Me.m_PSConnectionString)
            '    tmpAuthID = AuthAdd.AddNamingAuthority
            '    Me.m_Authorities = Me.m_ImportHandler.LoadAuthorities()
            '    Me.BindAuthorityListToControl(Me.m_Authorities)
            '    Me.m_SelectedAuthorityID = tmpAuthID
            '    Me.cboAuthorityPicker.SelectedValue = tmpAuthID
        End If
    End Sub
#End Region

#Region " Action Button Handlers "

    Private Sub cmdLoadProteins_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdLoadProteins.Click
        Me.ImportStartHandler("Retrieving Protein Entries..")
        Dim foundRows() As DataRow = _
            Me.m_ProteinCollections.Select("Protein_Collection_ID = " & Me.cboCollectionPicker.SelectedValue.ToString)
        Me.ImportProgressHandler(0.5)
        Me.m_SelectedFilePath = foundRows(0).Item("FileName").ToString
        Me.MemberLoadTimerHandler(Me, Nothing)
        Me.ImportProgressHandler(1.0)
        Me.txtLiveSearch.Visible = True
        Me.pbxLiveSearchBkg.Visible = True
        Me.ImportEndHandler()

    End Sub

    Private Sub cmdLoadFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadFile.Click

        Me.BatchLoadController()

    End Sub

    Private Sub cmdSaveDestCollection_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSaveDestCollection.Click
        Dim resultReturn As DialogResult

        Dim frmAddCollection As New frmAddNewCollection
        Dim tmpOrganismID As Integer
        Dim tmpAnnotationTypeID As Integer
        Dim tmpSelectedProteinList As ArrayList

        If Me.lvwDestination.Items.Count > 0 Then

            With frmAddCollection
                .CollectionName = System.IO.Path.GetFileNameWithoutExtension(Me.m_SelectedFilePath)
                .IsLocalFile = Me.m_LocalFileLoaded
                .AnnotationTypes = Me.m_AnnotationTypes
                .OrganismList = Me.m_Organisms
                .OrganismID = Me.m_SelectedOrganismID
                .AnnotationTypeID = Me.m_SelectedAnnotationTypeID
            End With

            resultReturn = frmAddCollection.ShowDialog

            If resultReturn = DialogResult.OK Then
                Me.cboCollectionPicker.Enabled = True
                Me.cboOrganismFilter.Enabled = True

                tmpOrganismID = frmAddCollection.OrganismID
                tmpAnnotationTypeID = frmAddCollection.AnnotationTypeID

                tmpSelectedProteinList = Me.ScanDestinationCollectionWindow(Me.lvwDestination)

                If Me.m_UploadHandler Is Nothing Then
                    Me.m_UploadHandler = New Protein_Uploader.clsPSUploadHandler(m_PSConnectionString)
                    Me.m_UploadHandler.InitialSetup()
                End If

                Me.m_UploadHandler.UploadCollection(Me.m_ImportHandler.CollectionMembers, _
                    tmpSelectedProteinList, frmAddCollection.CollectionName, _
                    frmAddCollection.CollectionDescription, Protein_Importer.IAddUpdateEntries.CollectionTypes.prot_original_source, _
                    tmpOrganismID, tmpAnnotationTypeID)

                Me.RefreshCollectionList()

                Me.ClearFromDestinationCollectionWindow(Me.lvwDestination, True)

                Me.cboOrganismFilter.Enabled = True
                Me.cboCollectionPicker.Enabled = True
                Me.cboOrganismFilter.SelectedValue = tmpOrganismID
            End If

        End If
        Me.m_UploadHandler = Nothing
    End Sub

    'Private Sub cmdExportToFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdExportToFile.Click

    '    Dim SaveDialog As New SaveFileDialog
    '    Dim fileType As Protein_Importer.IImportProteins.ProteinImportFileTypes
    '    Dim SelectedSavePath As String
    '    Dim tmpSelectedProteinList As ArrayList
    '    Dim tmpProteinCollection As Protein_Storage.IProteinStorage
    '    Dim tmpProteinReference As String

    '    With SaveDialog
    '        .Title = "Save Protein Database File"
    '        .DereferenceLinks = True
    '        .Filter = "FASTA Files (*.fasta)|*.fasta|Microsoft Access Databases (*.mdb)|*.mdb|All Files (*.*)|*.*"
    '        .FilterIndex = 1
    '        .RestoreDirectory = True
    '        .OverwritePrompt = True
    '        '.FileName = Me.m_ImportHandler.CollectionMembers.FileName
    '    End With

    '    If SaveDialog.ShowDialog = DialogResult.OK Then
    '        SelectedSavePath = SaveDialog.FileName
    '    Else
    '        Exit Sub
    '    End If

    '    If System.IO.Path.GetExtension(Me.m_SelectedFilePath) = ".fasta" Then
    '        fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA
    '    ElseIf System.IO.Path.GetExtension(Me.m_SelectedFilePath) = ".mdb" Then
    '        fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.Access
    '    End If

    '    If fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA Then
    '        Me.m_ProteinExporter = New Protein_Exporter.clsExportProteinsFASTA
    '    Else
    '        Exit Sub
    '    End If

    '    tmpProteinCollection = New Protein_Storage.clsProteinStorage(SelectedSavePath)

    '    tmpSelectedProteinList = Me.ScanDestinationCollectionWindow(Me.lvwDestination)

    '    For Each tmpProteinReference In tmpSelectedProteinList
    '        tmpProteinCollection.AddProtein( _
    '            Me.m_ImportHandler.CollectionMembers.GetProtein(tmpProteinReference))
    '    Next


    '    Me.m_ProteinExporter.Export( _
    '        Me.m_ImportHandler.CollectionMembers, _
    '        SelectedSavePath)


    'End Sub

#End Region

#Region " LiveSearch Handlers "

    Private Sub txtLiveSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLiveSearch.TextChanged

        If Me.txtLiveSearch.Text.Length > 0 And txtLiveSearch.ForeColor.ToString <> "Color [InactiveCaption]" Then
            Me.SearchTimer.Start()
        ElseIf Me.txtLiveSearch.Text = "" And Me.m_SearchActive = False Then
            Me.pbxLiveSearchCancel_Click(Me, Nothing)
        Else
            Me.m_SearchActive = False
            Me.SearchTimer.Stop()
            'Me.txtLiveSearch.Text = "Search"
            'Me.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
        End If

    End Sub

    Private Sub txtLiveSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLiveSearch.Click
        If m_SearchActive Then
        Else
            RemoveHandler txtLiveSearch.TextChanged, AddressOf txtLiveSearch_TextChanged
            txtLiveSearch.Text = Nothing
            txtLiveSearch.ForeColor = System.Drawing.SystemColors.ControlText
            m_SearchActive = True
            Me.pbxLiveSearchCancel.Visible = True
            AddHandler txtLiveSearch.TextChanged, AddressOf txtLiveSearch_TextChanged
            'Debug.WriteLine("inactive.click")
        End If
    End Sub

    Private Sub txtLiveSearch_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLiveSearch.Leave
        If txtLiveSearch.Text.Length = 0 Then
            txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
            txtLiveSearch.Text = "Search"
            Me.m_SearchActive = False
            SearchTimer.Stop()
            Me.m_SourceListViewHandler.Load(Me.m_CollectionMembers)
        End If
    End Sub

    Private Sub pbxLiveSearchCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbxLiveSearchCancel.Click
        Me.txtLiveSearch.Text = ""
        Me.txtLiveSearch_Leave(Me, Nothing)
        Me.lvwSource.Focus()
        Me.pbxLiveSearchCancel.Visible = False
    End Sub

    Friend Sub SearchTimerHandler( _
        ByVal sender As Object, _
        ByVal e As System.Timers.ElapsedEventArgs) Handles SearchTimer.Elapsed

        If Me.m_SearchActive = True Then
            'Debug.WriteLine("Searchtimer.active.kick")

            Me.m_SourceListViewHandler.Load(Me.m_CollectionMembers, Me.txtLiveSearch.Text)
            Me.m_SearchActive = False
            Me.SearchTimer.Stop()
        Else
            'Debug.WriteLine("Searchtimer.inactive.kick")

        End If
    End Sub


#End Region

#Region " ListView Event Handlers "


    Private Sub lvwSource_DoubleClick( _
        ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles lvwSource.DoubleClick

        ScanSourceCollectionWindow(Me.lvwSource, Me.lvwDestination, False)

    End Sub

    'Doubleclick to remove selected member from the destination collection
    Private Sub lvwDestination_DoubleClick( _
        ByVal sender As Object, _
        ByVal e As System.EventArgs) Handles lvwDestination.DoubleClick

        Me.ClearFromDestinationCollectionWindow(Me.lvwDestination, False)

    End Sub

    Friend Sub MemberLoadTimerHandler( _
        ByVal sender As Object, _
        ByVal e As System.Timers.ElapsedEventArgs) Handles MemberLoadTimer.Elapsed

        Me.m_SelectedCollectionID = CInt(Me.cboCollectionPicker.SelectedValue)
        Me.m_SelectedAnnotationTypeID = CInt(Me.cboAuthorityPicker.SelectedValue)

        Me.m_CollectionMembers = Me.m_ImportHandler.LoadCollectionMembersByID(Me.m_SelectedCollectionID, Me.m_SelectedAnnotationTypeID)
        Me.m_LocalFileLoaded = False

        'me.m_SelectedAuthorityID = me.m_ImportHandler.

        Me.m_SourceListViewHandler.Load(Me.m_CollectionMembers)
        Me.lvwSource.Focus()
        Me.lvwSource.Enabled = True


        'Me.MemberLoadTimer.Stop()

    End Sub

    'Update the selected collection
    'Private Sub lvwSource_SelectedIndexChanged( _
    '    ByVal sender As System.Object, _
    '    ByVal e As System.EventArgs) Handles lvwSource.SelectedIndexChanged

    'End Sub

    'Private Sub lvwDestination_SelectedIndexChanged( _
    '    ByVal sender As System.Object, _
    '    ByVal e As System.EventArgs) Handles lvwDestination.SelectedIndexChanged

    'End Sub

    'DoubleClick to Move the selected member to the destination collection


    Private Sub cmdDestAddAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDestAddAll.Click
        Me.ScanSourceCollectionWindow(Me.lvwSource, Me.lvwDestination, True)
        Me.CheckTransferButtonsEnabledStatus()
    End Sub

    Private Sub cmdDestAdd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDestAdd.Click
        Me.ScanSourceCollectionWindow(Me.lvwSource, Me.lvwDestination, False)
        Me.CheckTransferButtonsEnabledStatus()
    End Sub

    Private Sub cmdDestRemove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDestRemove.Click
        Me.ClearFromDestinationCollectionWindow(Me.lvwDestination, False)
        Me.CheckTransferButtonsEnabledStatus()
    End Sub

    Private Sub cmdDestRemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDestRemoveAll.Click
        Me.ClearFromDestinationCollectionWindow(Me.lvwDestination, True)
        Me.CheckTransferButtonsEnabledStatus()
    End Sub

    Protected Sub ScanSourceCollectionWindow( _
        ByVal lvwSrc As ListView, ByVal lvwDest As ListView, ByVal SelectAll As Boolean)

        Dim entry As ListViewItem
    

        If SelectAll Then
            For Each entry In lvwSrc.Items
                'Need to figure out some way to check for duplicates (maybe just at upload time)
                lvwDest.Items.Add(entry.Text)
            Next
        Else
            For Each entry In lvwSrc.SelectedItems
                'Need to figure out some way to check for duplicates (maybe just at upload time)
                lvwDest.Items.Add(entry.Text)
            Next
        End If

        Me.lblCurrProteinCount.Text = "Protein Count: " & lvwDest.Items.Count
        Me.cmdExportToFile.Enabled = True
        Me.cmdSaveDestCollection.Enabled = True

    End Sub

    Protected Function ScanDestinationCollectionWindow(ByVal lvwDest As ListView) As ArrayList
        Dim selectedList As New ArrayList
        Dim li As ListViewItem

        For Each li In lvwDest.Items
            selectedList.Add(li.Text)
        Next

        Return selectedList

    End Function

    Protected Sub ClearFromDestinationCollectionWindow(ByVal lvwDest As ListView, ByVal SelectAll As Boolean)
        Dim entry As ListViewItem

        If SelectAll Then
            lvwDest.Items.Clear()
            Me.cmdSaveDestCollection.Enabled = False
            Me.cmdExportToFile.Enabled = False
        Else
            For Each entry In lvwDest.SelectedItems
                entry.Remove()
            Next
        End If

        Me.lblCurrProteinCount.Text = "Protein Count: " & lvwDest.Items.Count

    End Sub

#End Region

#Region " Menu Option Handlers"

    Private Sub mnuFileExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuFileExit.Click
        Application.Exit()
    End Sub

    Private Sub mnuToolsFBatchUpload_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'Steal this to use with file-directed loading
        Me.m_fileBatcher = New Protein_Uploader.clsBatchUploadFromFileList(Me.m_PSConnectionString)
        Me.m_fileBatcher.UploadBatch()
    End Sub
    Private Sub mnuToolsNucTrans_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub mnuToolsConvertFASTA2Access_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub mnuToolsConvertAccess2FASTA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub mnuToolsCheckup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub mnuToolsCollectionEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsCollectionEdit.Click
        Dim cse As New frmCollectionStateEditor(Me.m_PSConnectionString)
        Dim r As DialogResult = cse.ShowDialog

    End Sub

    Private Sub mnuToolsExtractFromFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsExtractFromFile.Click
        Dim f As New ExtractAnnotationFromDescription.frmExtractFromFlatfile(Me.m_ImportHandler.Authorities, Me.m_PSConnectionString)
        f.ShowDialog()
    End Sub

    Private Sub mnuToolsUpdateArchives_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim f As FolderBrowserDialog = New FolderBrowserDialog
        Dim r As DialogResult
        Dim outputPath As String

        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.m_PSConnectionString)
        End If



        With f
            .RootFolder = Environment.SpecialFolder.MyComputer
            .ShowNewFolderButton = True

            r = .ShowDialog()
        End With

        If r = DialogResult.OK Then
            outputPath = f.SelectedPath

            Dim errorCode As Integer
            errorCode = Me.m_Syncer.SyncCollectionsAndArchiveTables(outputPath)
        End If

    End Sub

#End Region

#Region " Progress Event Handlers "
    Private Sub ImportStartHandler(ByVal taskTitle As String) Handles _
        m_ImportHandler.LoadStart, _
        m_SourceListViewHandler.LoadStart, _
        m_UploadHandler.LoadStart, _
        m_fileBatcher.LoadStart, _
        m_Syncer.SyncStart

        Me.pnlProgBar.Visible = True
        Me.pgbMain.Visible = True
        Me.pgbMain.Value = 0
        Me.lblCurrentTask.Text = taskTitle
        Me.lblCurrentTask.Visible = True
        System.Windows.Forms.Application.DoEvents()

    End Sub

    Private Sub ImportProgressHandler(ByVal fractionDone As Double) Handles _
        m_ImportHandler.LoadProgress, _
        m_SourceListViewHandler.LoadProgress, _
        m_UploadHandler.LoadProgress, _
        m_fileBatcher.ProgressUpdate

        Me.pgbMain.Value = CInt(fractionDone * 100)
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub SyncProgressHandler(ByVal statusmsg As String, ByVal fractionDone As Double) Handles _
        m_Syncer.SyncProgress
        Me.lblBatchProgress.Text = statusmsg
        If fractionDone > 1.0 Then
            fractionDone = 1.0
        End If
        Me.pgbMain.Value = CInt(fractionDone * 100)
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub ImportEndHandler() Handles _
        m_ImportHandler.LoadEnd, _
        m_SourceListViewHandler.LoadEnd, _
        m_UploadHandler.LoadEnd, m_fileBatcher.LoadEnd, _
        m_Syncer.SyncComplete

        Me.pnlProgBar.Visible = False
        Me.lblCurrentTask.Text = ""
        Me.lblCurrentTask.Visible = False
        Me.Invalidate()
        Me.gbxDestinationCollection.Invalidate()
        Me.gbxSourceCollection.Invalidate()
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub CollectionLoadHandler(ByVal CollectionTable As DataTable) Handles m_ImportHandler.CollectionLoadComplete
        Me.m_ProteinCollections = CollectionTable
        If Me.m_Organisms Is Nothing Then
            Me.m_Organisms = Me.m_ImportHandler.LoadOrganisms
        End If
        If Me.m_AnnotationTypes Is Nothing Then
            Me.m_AnnotationTypes = Me.m_ImportHandler.LoadAnnotationTypes
        End If
        Me.BindOrganismListToControl(Me.m_Organisms)
        Me.BindAuthorityListToControl(Me.m_AnnotationTypes)
        Me.m_ProteinCollections.DefaultView.RowFilter = ""
        Me.BindCollectionListToControl(Me.m_ProteinCollections.DefaultView)
        Me.cboCollectionPicker.Enabled = True
        Me.cboOrganismFilter.Enabled = True
        Me.lblBatchProgress.Text = ""
        'Me.mnuToolsFBatchUpload.Enabled = True

        AddHandler cboOrganismFilter.SelectedIndexChanged, AddressOf cboOrganismList_SelectedIndexChanged
        AddHandler cboCollectionPicker.SelectedIndexChanged, AddressOf cboCollectionPicker_SelectedIndexChanged

    End Sub

    Private Sub BatchImportProgressHandler(ByVal Status As String) Handles m_UploadHandler.BatchProgress, m_fileBatcher.TaskChange
        Me.m_BatchLoadCurrentCount += 1
        Me.lblBatchProgress.Text = Status & " (File " & Me.m_BatchLoadCurrentCount.ToString & " of " & Me.m_BatchLoadTotalCount & ")"
        Application.DoEvents()
    End Sub

    Private Sub FilteredLoadCountHandler(ByVal FilteredCount As Integer, ByVal TotalCount As Integer) Handles m_SourceListViewHandler.NumberLoadedStatus
        Me.lblSearchCount.Text = FilteredCount.ToString & " / " & TotalCount.ToString
    End Sub

    Private Sub ValidFASTAUploadHandler( _
        ByVal FASTAFilePath As String, _
        ByVal UploadInfo As Protein_Uploader.IUploadProteins.UploadInfo) _
            Handles m_UploadHandler.ValidFASTAFileLoaded

        If Me.m_ValidUploadsList Is Nothing Then
            Me.m_ValidUploadsList = New Hashtable
        End If

        Me.m_ValidUploadsList.Add(FASTAFilePath, UploadInfo)

    End Sub

    Private Sub InvalidFASTAFileHandler(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList) _
        Handles m_UploadHandler.InvalidFASTAFile

        If Me.m_FileErrorList Is Nothing Then
            Me.m_FileErrorList = New Hashtable
        End If

        Me.m_FileErrorList.Add(System.IO.Path.GetFileName(FASTAFilePath), errorCollection)

        If Me.m_SummarizedFileErrorList Is Nothing Then
            Me.m_SummarizedFileErrorList = New Hashtable
        End If

        Me.m_SummarizedFileErrorList.Add(System.IO.Path.GetFileName(FASTAFilePath), Me.SummarizeErrors(errorCollection))

    End Sub

    Private Sub FASTAFileWarningsHandler(ByVal FASTAFilePath As String, ByVal warningCollection As ArrayList) _
    Handles m_UploadHandler.FASTAFileWarnings

        If Me.m_FileWarningList Is Nothing Then
            Me.m_FileWarningList = New Hashtable
        End If

        Me.m_FileWarningList.Add(System.IO.Path.GetFileName(FASTAFilePath), warningCollection)

        If Me.m_SummarizedFileWarningList Is Nothing Then
            Me.m_SummarizedFileWarningList = New Hashtable
        End If

        Me.m_SummarizedFileWarningList.Add(System.IO.Path.GetFileName(FASTAFilePath), Me.SummarizeErrors(warningCollection))

    End Sub

    Private Function SummarizeErrors(ByRef errorCollection As ArrayList) As Hashtable
        Dim errorSummary As New Hashtable
        Dim errorEntry As ValidateFastaFile.ICustomValidation.udtErrorInfoExtended
        Dim currentCount As Integer

        If Not errorCollection Is Nothing AndAlso errorCollection.Count > 0 Then
            For Each errorEntry In errorCollection
                If Not errorSummary.Contains(errorEntry.MessageText.ToString) Then
                    errorSummary.Add(errorEntry.MessageText.ToString, "1")
                Else
                    currentCount = CInt(errorSummary.Item(errorEntry.MessageText.ToString))
                    errorSummary.Item(errorEntry.MessageText.ToString) = (currentCount + 1).ToString
                End If
            Next
        End If

        Return errorSummary

    End Function

    Private Sub ValidationProgressHandler(ByVal taskTitle As String, ByVal fractionDone As Double) _
        Handles m_UploadHandler.ValidationProgress
        'Handles m_ImportHandler.ValidationProgress, m_UploadHandler.ValidationProgress
        If Not taskTitle Is Nothing Then
            Me.lblCurrentTask.Text = taskTitle
        End If
        Me.pgbMain.Value = CInt(fractionDone * 100)
        System.Windows.Forms.Application.DoEvents()
    End Sub

    Private Sub NormalizedFASTAFileGenerationHandler(ByVal newFilePath As String) _
        Handles m_UploadHandler.WroteLineEndNormalizedFASTA



    End Sub

#End Region


    Private Sub mnuToolsUpdateSHA_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.m_PSConnectionString)
        End If

        Me.m_Syncer.UpdateSHA1Hashes()
    End Sub

    Private Sub mnuAdminUpdateZeroedMasses_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAdminUpdateZeroedMasses.Click
        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.m_PSConnectionString)
        End If

        Me.m_Syncer.CorrectMasses()
    End Sub

    Private Sub mnuAdminNameHashRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAdminNameHashRefresh.Click
        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.m_PSConnectionString)
        End If

        Me.m_Syncer.RefreshNameHashes()
    End Sub

    Private Sub mnuToolsNucToProt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsNucToProt.Click

    End Sub

    Private Sub mnuToolsConvert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsConvert.Click

    End Sub

    Private Sub mnuToolsConvertF2A_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsConvertF2A.Click

    End Sub

    Private Sub mnuToolsConvertA2F_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsConvertA2F.Click

    End Sub

    Private Sub mnuToolsFCheckup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsFCheckup.Click

    End Sub

    Private Sub MenuItem5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAdminTestingInterface.Click
        Dim frmTesting As New frmTestingInterface
        frmTesting.Show()
    End Sub

    Private Sub MenuItem6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAdminFixArchivePaths.Click
        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.m_PSConnectionString)
        End If

        Me.m_Syncer.FixArchivedFilePaths()

    End Sub
    Private Sub MenuItem8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAdminAddSortingIndexes.Click
        If Me.m_Syncer Is Nothing Then
            Me.m_Syncer = New clsSyncFASTAFileArchive(Me.m_PSConnectionString)
        End If
        Me.m_Syncer.AddSortingIndices()
    End Sub

    Private Sub mnuHelpAbout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuHelpAbout.Click
        ShowAboutBox()
    End Sub

    Private Sub mnuToolsOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuToolsOptions.Click

    End Sub

    'Private Sub mnuAdminUpdateZeroedMasses_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAdminUpdateZeroedMasses.Click

    'End Sub


End Class
