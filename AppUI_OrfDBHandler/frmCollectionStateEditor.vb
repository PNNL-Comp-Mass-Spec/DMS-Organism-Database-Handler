Public Class frmCollectionStateEditor
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New(ByVal ProteinStorageConnectionString As String)
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Me.m_PSConnectionString = ProteinStorageConnectionString
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
    Friend WithEvents lvwCollections As System.Windows.Forms.ListView
    Friend WithEvents lblCollectionsListView As System.Windows.Forms.Label
    Friend WithEvents colCollectionName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colOrganism As System.Windows.Forms.ColumnHeader
    Friend WithEvents colDateAdded As System.Windows.Forms.ColumnHeader
    Friend WithEvents colCurrState As System.Windows.Forms.ColumnHeader
    Friend WithEvents txtLiveSearch As System.Windows.Forms.TextBox
    Friend WithEvents pbxLiveSearchBkg As System.Windows.Forms.PictureBox
    Friend WithEvents lblStateChanger As System.Windows.Forms.Label
    Friend WithEvents cboStateChanger As System.Windows.Forms.ComboBox
    Friend WithEvents cmdStateChanger As System.Windows.Forms.Button
    Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
    Friend WithEvents mnuTools As System.Windows.Forms.MenuItem
    Friend WithEvents mnuToolsDeleteSelected As System.Windows.Forms.MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(frmCollectionStateEditor))
        Me.lvwCollections = New System.Windows.Forms.ListView
        Me.colCollectionName = New System.Windows.Forms.ColumnHeader
        Me.colOrganism = New System.Windows.Forms.ColumnHeader
        Me.colDateAdded = New System.Windows.Forms.ColumnHeader
        Me.colCurrState = New System.Windows.Forms.ColumnHeader
        Me.lblCollectionsListView = New System.Windows.Forms.Label
        Me.txtLiveSearch = New System.Windows.Forms.TextBox
        Me.pbxLiveSearchBkg = New System.Windows.Forms.PictureBox
        Me.lblStateChanger = New System.Windows.Forms.Label
        Me.cboStateChanger = New System.Windows.Forms.ComboBox
        Me.cmdStateChanger = New System.Windows.Forms.Button
        Me.MainMenu1 = New System.Windows.Forms.MainMenu
        Me.mnuTools = New System.Windows.Forms.MenuItem
        Me.mnuToolsDeleteSelected = New System.Windows.Forms.MenuItem
        Me.SuspendLayout()
        '
        'lvwCollections
        '
        Me.lvwCollections.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colCollectionName, Me.colOrganism, Me.colDateAdded, Me.colCurrState})
        Me.lvwCollections.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvwCollections.FullRowSelect = True
        Me.lvwCollections.GridLines = True
        Me.lvwCollections.Location = New System.Drawing.Point(10, 30)
        Me.lvwCollections.Name = "lvwCollections"
        Me.lvwCollections.Size = New System.Drawing.Size(594, 660)
        Me.lvwCollections.TabIndex = 0
        Me.lvwCollections.View = System.Windows.Forms.View.Details
        '
        'colCollectionName
        '
        Me.colCollectionName.Text = "Collection Name"
        Me.colCollectionName.Width = 278
        '
        'colOrganism
        '
        Me.colOrganism.Text = "Organism"
        Me.colOrganism.Width = 110
        '
        'colDateAdded
        '
        Me.colDateAdded.Text = "Date Added"
        Me.colDateAdded.Width = 100
        '
        'colCurrState
        '
        Me.colCurrState.Text = "State"
        Me.colCurrState.Width = 80
        '
        'lblCollectionsListView
        '
        Me.lblCollectionsListView.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCollectionsListView.Location = New System.Drawing.Point(10, 14)
        Me.lblCollectionsListView.Name = "lblCollectionsListView"
        Me.lblCollectionsListView.Size = New System.Drawing.Size(590, 20)
        Me.lblCollectionsListView.TabIndex = 1
        Me.lblCollectionsListView.Text = "Available Collections"
        '
        'txtLiveSearch
        '
        Me.txtLiveSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtLiveSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
        Me.txtLiveSearch.Location = New System.Drawing.Point(34, 704)
        Me.txtLiveSearch.Name = "txtLiveSearch"
        Me.txtLiveSearch.Size = New System.Drawing.Size(154, 14)
        Me.txtLiveSearch.TabIndex = 17
        Me.txtLiveSearch.Text = "Search"
        '
        'pbxLiveSearchBkg
        '
        Me.pbxLiveSearchBkg.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pbxLiveSearchBkg.Image = CType(resources.GetObject("pbxLiveSearchBkg.Image"), System.Drawing.Image)
        Me.pbxLiveSearchBkg.Location = New System.Drawing.Point(12, 698)
        Me.pbxLiveSearchBkg.Name = "pbxLiveSearchBkg"
        Me.pbxLiveSearchBkg.Size = New System.Drawing.Size(200, 24)
        Me.pbxLiveSearchBkg.TabIndex = 18
        Me.pbxLiveSearchBkg.TabStop = False
        '
        'lblStateChanger
        '
        Me.lblStateChanger.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblStateChanger.Location = New System.Drawing.Point(218, 704)
        Me.lblStateChanger.Name = "lblStateChanger"
        Me.lblStateChanger.Size = New System.Drawing.Size(182, 14)
        Me.lblStateChanger.TabIndex = 19
        Me.lblStateChanger.Text = "Change Selected Collections To..."
        '
        'cboStateChanger
        '
        Me.cboStateChanger.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboStateChanger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStateChanger.Location = New System.Drawing.Point(400, 700)
        Me.cboStateChanger.Name = "cboStateChanger"
        Me.cboStateChanger.Size = New System.Drawing.Size(126, 21)
        Me.cboStateChanger.TabIndex = 20
        '
        'cmdStateChanger
        '
        Me.cmdStateChanger.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStateChanger.Location = New System.Drawing.Point(536, 700)
        Me.cmdStateChanger.Name = "cmdStateChanger"
        Me.cmdStateChanger.Size = New System.Drawing.Size(68, 20)
        Me.cmdStateChanger.TabIndex = 21
        Me.cmdStateChanger.Text = "Change"
        '
        'MainMenu1
        '
        Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuTools})
        '
        'mnuTools
        '
        Me.mnuTools.Index = 0
        Me.mnuTools.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuToolsDeleteSelected})
        Me.mnuTools.Text = "Tools"
        Me.mnuTools.Visible = False
        '
        'mnuToolsDeleteSelected
        '
        Me.mnuToolsDeleteSelected.Index = 0
        Me.mnuToolsDeleteSelected.Text = "Delete Selected Collections..."
        '
        'frmCollectionStateEditor
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(614, 740)
        Me.Controls.Add(Me.cmdStateChanger)
        Me.Controls.Add(Me.cboStateChanger)
        Me.Controls.Add(Me.lblStateChanger)
        Me.Controls.Add(Me.txtLiveSearch)
        Me.Controls.Add(Me.pbxLiveSearchBkg)
        Me.Controls.Add(Me.lvwCollections)
        Me.Controls.Add(Me.lblCollectionsListView)
        Me.DockPadding.Bottom = 50
        Me.DockPadding.Left = 10
        Me.DockPadding.Right = 10
        Me.DockPadding.Top = 30
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Menu = Me.MainMenu1
        Me.Name = "frmCollectionStateEditor"
        Me.Text = "Collection State Editor"
        Me.ResumeLayout(False)

    End Sub

#End Region
    Friend WithEvents SearchTimer As New System.Timers.Timer(2000)
    Private m_SearchActive As Boolean = False
    Private m_Handler As clsCollectionStatePickerHandler
    Private m_PSConnectionString As String
    Private m_StatesTable As DataTable
    Private m_SelectedNewStateID As Integer = 1
    Private m_SortingColumn As ColumnHeader
    Private m_SortOrderAsc As Boolean = True
    Private m_SelectedCol As Integer = 0

#Region " Live Search Handler "

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
            Me.m_Handler.FillListView(Me.lvwCollections)
        End If
    End Sub

#End Region

    Friend Sub TimerHandler(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles SearchTimer.Elapsed
        Me.m_Handler.FillFilteredListView(Me.lvwCollections, Me.txtLiveSearch.Text)
    End Sub

#Region " Form Event Handlers"

    Private Sub frmCollectionStateEditor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.m_Handler = New clsCollectionStatePickerHandler(Me.m_PSConnectionString)
        Me.m_StatesTable = Me.m_Handler.GetStates()

        RemoveHandler cboStateChanger.SelectedIndexChanged, AddressOf cboStateChanger_SelectedIndexChanged

        With Me.cboStateChanger
            .BeginUpdate()
            .DataSource = Me.m_StatesTable
            .DisplayMember = "State"
            .ValueMember = "ID"
            .EndUpdate()
        End With

        AddHandler cboStateChanger.SelectedIndexChanged, AddressOf cboStateChanger_SelectedIndexChanged

        Me.cboStateChanger.SelectedValue = Me.m_SelectedNewStateID

        Me.m_Handler.FillListView(Me.lvwCollections)
    End Sub

    Private Sub cboStateChanger_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboStateChanger.SelectedIndexChanged
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)
        Me.m_SelectedNewStateID = CInt(cbo.SelectedValue)
    End Sub

    Private Sub cmdStateChanger_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStateChanger.Click
        Dim al As New ArrayList
        Dim item As ListViewItem

        For Each item In Me.lvwCollections.SelectedItems
            al.Add(item.Tag)
        Next

        Me.m_Handler.ChangeSelectedCollectionStates(Me.m_SelectedNewStateID, al)

        Me.m_Handler.ForceIDTableReload = True

        If Me.m_SearchActive Then
            Me.m_Handler.FillFilteredListView(Me.lvwCollections, Me.txtLiveSearch.Text)
        Else
            Me.m_Handler.FillListView(Me.lvwCollections)
        End If

    End Sub

#End Region

    Private Sub lvwSearchResults_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lvwCollections.ColumnClick

        'If selected column is same as previously selected column, then reverse sort order. Otherwise,
        '	sort newly selected column in ascending order

        'Set up ascending/descending criteria
        If e.Column = m_SelectedCol Then
            m_SortOrderAsc = Not m_SortOrderAsc
        Else
            m_SortOrderAsc = True
            m_SelectedCol = e.Column
        End If

        'Perform sort
        Me.lvwCollections.ListViewItemSorter = New ListViewItemComparer(e.Column, m_SortOrderAsc)
    End Sub
End Class

Class ListViewItemComparer
    Implements IComparer

    ' Implements the manual sorting of items by columns.
    Dim m_SortOrderAsc As Boolean = True

    Private col As Integer

    Public Sub New()
        col = 0
    End Sub

    Public Sub New(ByVal column As Integer)
        col = column
    End Sub

    Public Sub New(ByVal column As Integer, ByVal SortOrderAsc As Boolean)
        col = column
        m_SortOrderAsc = SortOrderAsc
    End Sub

    Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
     Implements IComparer.Compare

        Dim TempResult As Integer

        TempResult = [String].Compare(CType(x, ListViewItem).SubItems(col).Text, CType(y, ListViewItem).SubItems(col).Text)
        If m_SortOrderAsc Then
            Return TempResult
        Else
            Return -TempResult
        End If

    End Function

    Public Property SortOrder() As Boolean
        Get
            Return m_SortOrderAsc
        End Get
        Set(ByVal Value As Boolean)
            m_SortOrderAsc = False
        End Set
    End Property

End Class