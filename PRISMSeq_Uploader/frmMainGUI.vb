Public Class frmMainGUI
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        'Load program settings here to get connection strings etc.

        'HACK:  Hard coded connection string is temporary
        Me.m_ImportHandler = New Protein_Importer.clsImportHandler( _
            "Data Source=gigasax;Initial Catalog=Protein_Storage;Integrated Security=SSPI;")

        Me.m_Organisms = Me.m_ImportHandler.LoadOrganisms
        Me.m_ProteinCollections = Me.m_ImportHandler.LoadProteinCollections
        Me.BindOrganismListToControl(Me.m_Organisms)
        Me.m_ListViewHandler = New DataListViewHandler(Me.lvwAvailableCollections)

        'AddHandler cboOrganismList.SelectedIndexChanged, AddressOf cboOrganismList_SelectedIndexChanged
        'Me.cboOrganismList.SelectedIndex = 0
        'Me.cboOrganismList_SelectedIndexChanged(Me, Nothing)
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
    Friend WithEvents cboOrganismList As System.Windows.Forms.ComboBox
    Friend WithEvents lblOrganismList As System.Windows.Forms.Label
    Friend WithEvents lblAvailableCollections As System.Windows.Forms.Label
    Friend WithEvents cmdAddOrganism As System.Windows.Forms.Button
    Friend WithEvents lvwAvailableCollections As System.Windows.Forms.ListView
    Friend WithEvents colName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colID As System.Windows.Forms.ColumnHeader
    Friend WithEvents txtLiveSearch As System.Windows.Forms.TextBox
    Friend WithEvents colProteinCount As System.Windows.Forms.ColumnHeader
    Friend WithEvents mnuMainGUI As System.Windows.Forms.MainMenu
    Friend WithEvents mnuFile As System.Windows.Forms.MenuItem
    Friend WithEvents mnuFileExit As System.Windows.Forms.MenuItem
    Friend WithEvents mnuTools As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsOptions As System.Windows.Forms.MenuItem
    Friend WithEvents cmdAddAuthority As System.Windows.Forms.Button
    Friend WithEvents cboAuthorityPicker As System.Windows.Forms.ComboBox
    Friend WithEvents lblAuthorityPicker As System.Windows.Forms.Label
    Friend WithEvents MainTipProvider As System.Windows.Forms.ToolTip
    Friend WithEvents cmdAddCollection As System.Windows.Forms.Button
    Friend WithEvents pbxLiveSearchBkg As System.Windows.Forms.PictureBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmMainGUI))
        Me.cboOrganismList = New System.Windows.Forms.ComboBox
        Me.lblOrganismList = New System.Windows.Forms.Label
        Me.lblAvailableCollections = New System.Windows.Forms.Label
        Me.cmdAddOrganism = New System.Windows.Forms.Button
        'Me.mnuMainGUI = New System.Windows.Forms.MainMenu
        'Me.mnuFile = New System.Windows.Forms.MenuItem
        'Me.mnuFileExit = New System.Windows.Forms.MenuItem
        'Me.mnuTools = New System.Windows.Forms.MenuItem
        'Me.mnuToolsOptions = New System.Windows.Forms.MenuItem
        Me.lvwAvailableCollections = New System.Windows.Forms.ListView
        Me.colID = New System.Windows.Forms.ColumnHeader
        Me.colName = New System.Windows.Forms.ColumnHeader
        Me.colProteinCount = New System.Windows.Forms.ColumnHeader
        Me.txtLiveSearch = New System.Windows.Forms.TextBox
        Me.pbxLiveSearchBkg = New System.Windows.Forms.PictureBox
        Me.cmdAddAuthority = New System.Windows.Forms.Button
        Me.cboAuthorityPicker = New System.Windows.Forms.ComboBox
        Me.lblAuthorityPicker = New System.Windows.Forms.Label
        Me.cmdAddCollection = New System.Windows.Forms.Button
        Me.MainTipProvider = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'cboOrganismList
        '
        Me.cboOrganismList.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboOrganismList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOrganismList.Location = New System.Drawing.Point(16, 22)
        Me.cboOrganismList.Name = "cboOrganismList"
        Me.cboOrganismList.Size = New System.Drawing.Size(240, 21)
        Me.cboOrganismList.TabIndex = 1
        Me.MainTipProvider.SetToolTip(Me.cboOrganismList, "Pick an Organism to Filter On")
        '
        'lblOrganismList
        '
        Me.lblOrganismList.Location = New System.Drawing.Point(16, 6)
        Me.lblOrganismList.Name = "lblOrganismList"
        Me.lblOrganismList.Size = New System.Drawing.Size(208, 18)
        Me.lblOrganismList.TabIndex = 2
        Me.lblOrganismList.Text = "Organism"
        '
        'lblAvailableCollections
        '
        Me.lblAvailableCollections.Location = New System.Drawing.Point(16, 48)
        Me.lblAvailableCollections.Name = "lblAvailableCollections"
        Me.lblAvailableCollections.Size = New System.Drawing.Size(230, 12)
        Me.lblAvailableCollections.TabIndex = 3
        Me.lblAvailableCollections.Text = "Available Protein Collections"
        '
        'cmdAddOrganism
        '
        Me.cmdAddOrganism.Enabled = False
        Me.cmdAddOrganism.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdAddOrganism.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddOrganism.Location = New System.Drawing.Point(260, 22)
        Me.cmdAddOrganism.Name = "cmdAddOrganism"
        Me.cmdAddOrganism.Size = New System.Drawing.Size(20, 21)
        Me.cmdAddOrganism.TabIndex = 4
        Me.cmdAddOrganism.Text = "+"
        Me.MainTipProvider.SetToolTip(Me.cmdAddOrganism, "Add a New Organism")
        '
        'lvwAvailableCollections
        '
        Me.lvwAvailableCollections.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colID, Me.colName, Me.colProteinCount})
        Me.lvwAvailableCollections.Cursor = System.Windows.Forms.Cursors.Default
        Me.lvwAvailableCollections.FullRowSelect = True
        Me.lvwAvailableCollections.GridLines = True
        Me.lvwAvailableCollections.HideSelection = False
        Me.lvwAvailableCollections.Location = New System.Drawing.Point(16, 64)
        Me.lvwAvailableCollections.MultiSelect = False
        Me.lvwAvailableCollections.Name = "lvwAvailableCollections"
        Me.lvwAvailableCollections.Size = New System.Drawing.Size(494, 480)
        Me.lvwAvailableCollections.TabIndex = 5
        Me.MainTipProvider.SetToolTip(Me.lvwAvailableCollections, "Double-Click on a Collection to View")
        Me.lvwAvailableCollections.View = System.Windows.Forms.View.Details
        '
        'colID
        '
        Me.colID.Text = "ID"
        Me.colID.Width = 39
        '
        'colName
        '
        Me.colName.Text = "Collection Name"
        Me.colName.Width = 345
        '
        'colProteinCount
        '
        Me.colProteinCount.Text = "Number of Proteins"
        Me.colProteinCount.Width = 106
        '
        'txtLiveSearch
        '
        Me.txtLiveSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
        Me.txtLiveSearch.Location = New System.Drawing.Point(42, 558)
        Me.txtLiveSearch.Name = "txtLiveSearch"
        Me.txtLiveSearch.Size = New System.Drawing.Size(160, 14)
        Me.txtLiveSearch.TabIndex = 6
        Me.txtLiveSearch.Text = "Search"
        Me.MainTipProvider.SetToolTip(Me.txtLiveSearch, "Search within the Collections List")
        '
        'pbxLiveSearchBkg
        '
        Me.pbxLiveSearchBkg.Image = CType(resources.GetObject("pbxLiveSearchBkg.Image"), System.Drawing.Image)
        Me.pbxLiveSearchBkg.Location = New System.Drawing.Point(18, 552)
        Me.pbxLiveSearchBkg.Name = "pbxLiveSearchBkg"
        Me.pbxLiveSearchBkg.Size = New System.Drawing.Size(200, 24)
        Me.pbxLiveSearchBkg.TabIndex = 7
        Me.pbxLiveSearchBkg.TabStop = False
        Me.MainTipProvider.SetToolTip(Me.pbxLiveSearchBkg, "Search within the Collections List")
        '
        'cmdAddAuthority
        '
        Me.cmdAddAuthority.Enabled = False
        Me.cmdAddAuthority.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdAddAuthority.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddAuthority.Location = New System.Drawing.Point(490, 22)
        Me.cmdAddAuthority.Name = "cmdAddAuthority"
        Me.cmdAddAuthority.Size = New System.Drawing.Size(20, 21)
        Me.cmdAddAuthority.TabIndex = 9
        Me.cmdAddAuthority.Text = "+"
        Me.MainTipProvider.SetToolTip(Me.cmdAddAuthority, "Add a New Naming Authority")
        '
        'cboAuthorityPicker
        '
        Me.cboAuthorityPicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboAuthorityPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAuthorityPicker.Location = New System.Drawing.Point(288, 22)
        Me.cboAuthorityPicker.Name = "cboAuthorityPicker"
        Me.cboAuthorityPicker.Size = New System.Drawing.Size(198, 21)
        Me.cboAuthorityPicker.TabIndex = 8
        Me.MainTipProvider.SetToolTip(Me.cboAuthorityPicker, "Pick a Naming Authority to Filter On")
        '
        'lblAuthorityPicker
        '
        Me.lblAuthorityPicker.Location = New System.Drawing.Point(288, 6)
        Me.lblAuthorityPicker.Name = "lblAuthorityPicker"
        Me.lblAuthorityPicker.Size = New System.Drawing.Size(190, 18)
        Me.lblAuthorityPicker.TabIndex = 10
        Me.lblAuthorityPicker.Text = "Naming Authority"
        '
        'cmdAddCollection
        '
        Me.cmdAddCollection.Enabled = False
        Me.cmdAddCollection.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdAddCollection.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddCollection.Location = New System.Drawing.Point(490, 552)
        Me.cmdAddCollection.Name = "cmdAddCollection"
        Me.cmdAddCollection.Size = New System.Drawing.Size(20, 21)
        Me.cmdAddCollection.TabIndex = 11
        Me.cmdAddCollection.Text = "+"
        Me.MainTipProvider.SetToolTip(Me.cmdAddCollection, "Add a New Protein Collection")
        '
        'MainTipProvider
        '
        Me.MainTipProvider.AutomaticDelay = 1000
        '        '
        'frmMainGUI
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(526, 583)
        Me.Controls.Add(Me.cmdAddCollection)
        Me.Controls.Add(Me.cmdAddAuthority)
        Me.Controls.Add(Me.cboAuthorityPicker)
        Me.Controls.Add(Me.txtLiveSearch)
        Me.Controls.Add(Me.pbxLiveSearchBkg)
        Me.Controls.Add(Me.lvwAvailableCollections)
        Me.Controls.Add(Me.cmdAddOrganism)
        Me.Controls.Add(Me.cboOrganismList)
        Me.Controls.Add(Me.lblOrganismList)
        Me.Controls.Add(Me.lblAvailableCollections)
        Me.Controls.Add(Me.lblAuthorityPicker)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Menu = Me.mnuMainGUI
        Me.Name = "frmMainGUI"
        Me.Text = "Protein Collection Manager"
        Me.ResumeLayout(False)

    End Sub
    '<System.STAThread()> Public Shared Sub Main()
    '    System.Windows.Forms.Application.EnableVisualStyles()
    '    System.Windows.Forms.Application.DoEvents()
    '    System.Windows.Forms.Application.Run(New frmMainGUI)  ' replace frmDecode by the name of your form!!!
    'End Sub

#End Region

    Protected m_Organisms As DataTable
    Protected m_ProteinCollections As DataTable
    Protected m_ImportHandler As Protein_Importer.IImportProteins
    Protected m_SelectedOrganismID As Integer
    Protected m_ListViewHandler As DataListViewHandler

    Private m_SearchActive As Boolean = False
    Friend WithEvents SearchTimer As New System.Timers.Timer(2000)


    Private Sub frmMainGUI_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    End Sub

    Protected Sub BindOrganismListToControl(ByVal organismList As DataTable)
        With Me.cboOrganismList
            .DataSource = organismList
            .DisplayMember = "Display_Name"
            .ValueMember = "Organism_ID"
        End With

    End Sub



    'Private Sub cboOrganismList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    With Me.m_ListViewHandler
    '        .OrganismIDFilter = CInt(Me.cboOrganismList.SelectedValue)
    '        .Load(Me.m_ProteinCollections)
    '    End With
    'End Sub

    Private Sub cboAuthorityPicker_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboAuthorityPicker.SelectedIndexChanged

    End Sub



#Region " LiveSearch Handlers "

    Private Sub txtLiveSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLiveSearch.TextChanged
        If m_SearchActive Then
            SearchTimer.Start()
        End If
    End Sub

    Private Sub txtLiveSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLiveSearch.Click
        If m_SearchActive Then
        Else
            txtLiveSearch.Text = Nothing
            txtLiveSearch.ForeColor = System.Drawing.SystemColors.ControlText
            m_SearchActive = True
        End If
    End Sub

    Private Sub txtLiveSearch_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtLiveSearch.Leave
        If txtLiveSearch.Text.Length = 0 Then
            txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
            txtLiveSearch.Text = "Search"
            Me.m_SearchActive = False
            SearchTimer.Stop()
            Me.m_ListViewHandler.Load(Me.m_ProteinCollections)
        End If
    End Sub

    Friend Sub TimerHandler(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles SearchTimer.Elapsed
        Me.m_ListViewHandler.Load(Me.m_ProteinCollections, Me.txtLiveSearch.Text)
    End Sub

#End Region

#Region " ListView Event Handlers "

    'Update the selected collection
    Private Sub lvwAvailableCollections_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvwAvailableCollections.SelectedIndexChanged

    End Sub

    'DoubleClick to View the Members of a given collection
    Private Sub lvwAvailableCollections_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvwAvailableCollections.DoubleClick

    End Sub

#End Region
End Class
