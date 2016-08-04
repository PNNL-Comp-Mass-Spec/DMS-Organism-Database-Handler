Imports System.Timers

Public Class frmCollectionStateEditor
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New(ProteinStorageConnectionString As String)
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Me.m_PSConnectionString = ProteinStorageConnectionString
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lvwCollections As ListView
    Friend WithEvents lblCollectionsListView As Label
    Friend WithEvents colCollectionName As ColumnHeader
    Friend WithEvents colOrganism As ColumnHeader
    Friend WithEvents colDateAdded As ColumnHeader
    Friend WithEvents colCurrState As ColumnHeader
    Friend WithEvents txtLiveSearch As TextBox
    Friend WithEvents pbxLiveSearchBkg As PictureBox
    Friend WithEvents lblStateChanger As Label
    Friend WithEvents cboStateChanger As ComboBox
    Friend WithEvents cmdStateChanger As Button
    Friend WithEvents MainMenu1 As MainMenu
    Friend WithEvents mnuTools As MenuItem
    Friend WithEvents mnuToolsDeleteSelected As MenuItem
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCollectionStateEditor))
        Me.lvwCollections = New System.Windows.Forms.ListView()
        Me.colCollectionName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colOrganism = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDateAdded = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colCurrState = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblCollectionsListView = New System.Windows.Forms.Label()
        Me.txtLiveSearch = New System.Windows.Forms.TextBox()
        Me.pbxLiveSearchBkg = New System.Windows.Forms.PictureBox()
        Me.lblStateChanger = New System.Windows.Forms.Label()
        Me.cboStateChanger = New System.Windows.Forms.ComboBox()
        Me.cmdStateChanger = New System.Windows.Forms.Button()
        Me.MainMenu1 = New System.Windows.Forms.MainMenu(Me.components)
        Me.mnuTools = New System.Windows.Forms.MenuItem()
        Me.mnuToolsDeleteSelected = New System.Windows.Forms.MenuItem()
        CType(Me.pbxLiveSearchBkg, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.lvwCollections.UseCompatibleStateImageBehavior = False
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
        Me.lblCollectionsListView.Location = New System.Drawing.Point(14, 17)
        Me.lblCollectionsListView.Name = "lblCollectionsListView"
        Me.lblCollectionsListView.Size = New System.Drawing.Size(826, 24)
        Me.lblCollectionsListView.TabIndex = 1
        Me.lblCollectionsListView.Text = "Available Collections"
        '
        'txtLiveSearch
        '
        Me.txtLiveSearch.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.txtLiveSearch.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption
        Me.txtLiveSearch.Location = New System.Drawing.Point(48, 696)
        Me.txtLiveSearch.Name = "txtLiveSearch"
        Me.txtLiveSearch.Size = New System.Drawing.Size(215, 17)
        Me.txtLiveSearch.TabIndex = 17
        Me.txtLiveSearch.Text = "Search"
        '
        'pbxLiveSearchBkg
        '
        Me.pbxLiveSearchBkg.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pbxLiveSearchBkg.Image = CType(resources.GetObject("pbxLiveSearchBkg.Image"), System.Drawing.Image)
        Me.pbxLiveSearchBkg.Location = New System.Drawing.Point(17, 689)
        Me.pbxLiveSearchBkg.Name = "pbxLiveSearchBkg"
        Me.pbxLiveSearchBkg.Size = New System.Drawing.Size(280, 29)
        Me.pbxLiveSearchBkg.TabIndex = 18
        Me.pbxLiveSearchBkg.TabStop = False
        '
        'lblStateChanger
        '
        Me.lblStateChanger.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblStateChanger.Location = New System.Drawing.Point(305, 696)
        Me.lblStateChanger.Name = "lblStateChanger"
        Me.lblStateChanger.Size = New System.Drawing.Size(255, 17)
        Me.lblStateChanger.TabIndex = 19
        Me.lblStateChanger.Text = "Change Selected Collections To..."
        '
        'cboStateChanger
        '
        Me.cboStateChanger.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboStateChanger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStateChanger.Location = New System.Drawing.Point(560, 691)
        Me.cboStateChanger.Name = "cboStateChanger"
        Me.cboStateChanger.Size = New System.Drawing.Size(0, 25)
        Me.cboStateChanger.TabIndex = 20
        '
        'cmdStateChanger
        '
        Me.cmdStateChanger.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdStateChanger.Location = New System.Drawing.Point(504, 691)
        Me.cmdStateChanger.Name = "cmdStateChanger"
        Me.cmdStateChanger.Size = New System.Drawing.Size(96, 24)
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
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.ClientSize = New System.Drawing.Size(614, 740)
        Me.Controls.Add(Me.cmdStateChanger)
        Me.Controls.Add(Me.cboStateChanger)
        Me.Controls.Add(Me.lblStateChanger)
        Me.Controls.Add(Me.txtLiveSearch)
        Me.Controls.Add(Me.pbxLiveSearchBkg)
        Me.Controls.Add(Me.lvwCollections)
        Me.Controls.Add(Me.lblCollectionsListView)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Menu = Me.MainMenu1
        Me.Name = "frmCollectionStateEditor"
        Me.Padding = New System.Windows.Forms.Padding(10, 30, 10, 50)
        Me.Text = "Collection State Editor"
        CType(Me.pbxLiveSearchBkg, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region
    Friend WithEvents SearchTimer As New Timer(2000)
    Private m_SearchActive As Boolean = False
    Private m_Handler As clsCollectionStatePickerHandler
    Private ReadOnly m_PSConnectionString As String
    Private m_StatesTable As DataTable
    Private m_SelectedNewStateID As Integer = 1
    Private m_SortOrderAsc As Boolean = True
    Private m_SelectedCol As Integer = 0

#Region " Live Search Handler "

    Private Sub txtLiveSearch_TextChanged(sender As Object, e As EventArgs) Handles txtLiveSearch.TextChanged
        If m_SearchActive Then
            SearchTimer.Start()
        End If
    End Sub

    Private Sub txtLiveSearch_Click(sender As Object, e As EventArgs) Handles txtLiveSearch.Click
        If m_SearchActive Then
        Else
            txtLiveSearch.Text = Nothing
            txtLiveSearch.ForeColor = SystemColors.ControlText
            m_SearchActive = True
        End If
    End Sub

    Private Sub txtLiveSearch_Leave(sender As Object, e As EventArgs) Handles txtLiveSearch.Leave
        If txtLiveSearch.Text.Length = 0 Then
            txtLiveSearch.ForeColor = SystemColors.InactiveCaption
            txtLiveSearch.Text = "Search"
            Me.m_SearchActive = False
            SearchTimer.Stop()
            Me.m_Handler.FillListView(Me.lvwCollections)
        End If
    End Sub

#End Region

    Friend Sub TimerHandler(sender As Object, e As ElapsedEventArgs) Handles SearchTimer.Elapsed
        Me.m_Handler.FillFilteredListView(Me.lvwCollections, Me.txtLiveSearch.Text)
    End Sub

#Region " Form Event Handlers"

    Private Sub frmCollectionStateEditor_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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

    Private Sub cboStateChanger_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboStateChanger.SelectedIndexChanged
        Dim cbo = DirectCast(sender, ComboBox)
        Me.m_SelectedNewStateID = CInt(cbo.SelectedValue)
    End Sub

    Private Sub cmdStateChanger_Click(sender As Object, e As EventArgs) Handles cmdStateChanger.Click
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

    Private Sub lvwSearchResults_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles lvwCollections.ColumnClick

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

    Private Class ListViewItemComparer
        Implements IComparer

        ' Implements the manual sorting of items by columns.
        ReadOnly m_SortAscending As Boolean = True

        Private ReadOnly colIndex As Integer

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="columnIndex"></param>
        ''' <param name="sortAscending"></param>
        Public Sub New(columnIndex As Integer, sortAscending As Boolean)
            colIndex = columnIndex
            m_SortAscending = sortAscending
        End Sub

        Public Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare

            Dim item1 = TryCast(x, ListViewItem)
            Dim item2 = TryCast(y, ListViewItem)

            If item1 Is Nothing AndAlso item2 Is Nothing Then
                Return 0
            End If

            Dim compareResult As Integer

            If item1 Is Nothing Then
                compareResult = 1
            ElseIf item2 Is Nothing Then
                compareResult = -1
            Else
                compareResult = String.Compare(item1.SubItems(colIndex).Text, item2.SubItems(colIndex).Text)
            End If

            If m_SortAscending Then
                Return compareResult
            Else
                Return -compareResult
            End If

        End Function

    End Class

End Class
