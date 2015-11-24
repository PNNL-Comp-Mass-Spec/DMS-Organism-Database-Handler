Public Class frmFilePreview
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Me.validationRegex = New System.Text.RegularExpressions.Regex("^(\d+)$")
        'Add any initialization after the InitializeComponent() call

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
    Friend WithEvents lblLineCount As System.Windows.Forms.Label
    Friend WithEvents txtLineCount As System.Windows.Forms.TextBox
    Friend WithEvents cmdRefresh As System.Windows.Forms.Button
    Friend WithEvents lblPreviewTitle As System.Windows.Forms.Label
    Friend WithEvents colName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colDescription As System.Windows.Forms.ColumnHeader
    Friend WithEvents lvwPreview As System.Windows.Forms.ListView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lvwPreview = New System.Windows.Forms.ListView
        Me.colName = New System.Windows.Forms.ColumnHeader
        Me.colDescription = New System.Windows.Forms.ColumnHeader
        Me.txtLineCount = New System.Windows.Forms.TextBox
        Me.cmdRefresh = New System.Windows.Forms.Button
        Me.lblLineCount = New System.Windows.Forms.Label
        Me.lblPreviewTitle = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'lvwPreview
        '
        Me.lvwPreview.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwPreview.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colName, Me.colDescription})
        Me.lvwPreview.FullRowSelect = True
        Me.lvwPreview.GridLines = True
        Me.lvwPreview.Location = New System.Drawing.Point(-2, 48)
        Me.lvwPreview.MultiSelect = False
        Me.lvwPreview.Name = "lvwPreview"
        Me.lvwPreview.Size = New System.Drawing.Size(476, 548)
        Me.lvwPreview.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwPreview.TabIndex = 0
        Me.lvwPreview.View = System.Windows.Forms.View.Details
        '
        'colName
        '
        Me.colName.Text = "Protein Name"
        Me.colName.Width = 120
        '
        'colDescription
        '
        Me.colDescription.Text = "Description Line"
        Me.colDescription.Width = 352
        '
        'txtLineCount
        '
        Me.txtLineCount.Location = New System.Drawing.Point(366, 22)
        Me.txtLineCount.Name = "txtLineCount"
        Me.txtLineCount.TabIndex = 1
        Me.txtLineCount.Text = ""
        Me.txtLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'cmdRefresh
        '
        Me.cmdRefresh.Location = New System.Drawing.Point(284, 22)
        Me.cmdRefresh.Name = "cmdRefresh"
        Me.cmdRefresh.Size = New System.Drawing.Size(76, 20)
        Me.cmdRefresh.TabIndex = 2
        Me.cmdRefresh.Text = "Refresh List"
        '
        'lblLineCount
        '
        Me.lblLineCount.Location = New System.Drawing.Point(364, 6)
        Me.lblLineCount.Name = "lblLineCount"
        Me.lblLineCount.Size = New System.Drawing.Size(98, 16)
        Me.lblLineCount.TabIndex = 3
        Me.lblLineCount.Text = "# Lines to Preview"
        '
        'lblPreviewTitle
        '
        Me.lblPreviewTitle.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPreviewTitle.Location = New System.Drawing.Point(2, 28)
        Me.lblPreviewTitle.Name = "lblPreviewTitle"
        Me.lblPreviewTitle.Size = New System.Drawing.Size(184, 16)
        Me.lblPreviewTitle.TabIndex = 4
        Me.lblPreviewTitle.Text = "Preview of File Contents"
        '
        'frmFilePreview
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(472, 596)
        Me.Controls.Add(Me.lblLineCount)
        Me.Controls.Add(Me.cmdRefresh)
        Me.Controls.Add(Me.txtLineCount)
        Me.Controls.Add(Me.lvwPreview)
        Me.Controls.Add(Me.lblPreviewTitle)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(244, 360)
        Me.Name = "frmFilePreview"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Preview of: "
        Me.ResumeLayout(False)

    End Sub

#End Region

    Event RefreshRequest(lineCount As Integer)
    Shadows Event FormClosing()

    Private validationRegex As System.Text.RegularExpressions.Regex
    Private m_currentLineCount As Integer = 100

    WriteOnly Property WindowName() As String
        Set(Value As String)
            Me.Text = Value
        End Set
    End Property

    ReadOnly Property FormVisibility() As Boolean
        Get
            Return Me.Visible
        End Get
    End Property

    Private Sub cmdRefresh_Click(sender As System.Object, e As System.EventArgs) Handles cmdRefresh.Click
        RaiseEvent RefreshRequest(Me.m_currentLineCount)
        If Me.cmdRefresh.Enabled = True Then
            Me.cmdRefresh.Enabled = False
        End If
    End Sub

    Private Sub txtLineCount_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles txtLineCount.Validating
        Dim value As Integer
        Dim m As System.Text.RegularExpressions.Match
        Dim countText As String = Me.txtLineCount.Text

        If validationRegex.IsMatch(CInt(countText).ToString) Then
            m = validationRegex.Match(CInt(countText).ToString)
            value = CInt(m.Groups(0).Value)
            Me.txtLineCount.Text = value.ToString
            Me.m_currentLineCount = value
            If Me.cmdRefresh.Enabled = False Then
                Me.cmdRefresh.Enabled = True
            End If
        Else
            Me.txtLineCount.Text = Me.m_currentLineCount.ToString
            e.Cancel = True
        End If

    End Sub

    Private Sub frmFilePreview_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.txtLineCount.Text = Me.m_currentLineCount.ToString
        RaiseEvent RefreshRequest(Me.m_currentLineCount)
    End Sub

    Private Sub txtLineCount_TextChanged(sender As System.Object, e As System.EventArgs) Handles txtLineCount.TextChanged
        Dim countText As String = Me.txtLineCount.Text
        If validationRegex.IsMatch(countText) Then
            Me.cmdRefresh.Enabled = True
        Else
            Me.cmdRefresh.Enabled = False
        End If
    End Sub

    Private Sub frmFilePreview_Closed(sender As Object, e As System.EventArgs) Handles MyBase.Closed
        RaiseEvent FormClosing()
    End Sub
End Class
