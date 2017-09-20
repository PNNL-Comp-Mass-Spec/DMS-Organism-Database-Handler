Imports System.ComponentModel
Imports System.Text.RegularExpressions

Public Class frmFilePreview
    Inherits Form

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
    Friend WithEvents lblLineCount As Label
    Friend WithEvents txtLineCount As TextBox
    Friend WithEvents cmdRefresh As Button
    Friend WithEvents lblPreviewTitle As Label
    Friend WithEvents colName As ColumnHeader
    Friend WithEvents colDescription As ColumnHeader
    Friend WithEvents cmdClose As Button
    Friend WithEvents lvwPreview As ListView
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lvwPreview = New ListView()
        Me.colName = CType(New ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colDescription = CType(New ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.txtLineCount = New TextBox()
        Me.cmdRefresh = New Button()
        Me.lblLineCount = New Label()
        Me.lblPreviewTitle = New Label()
        Me.cmdClose = New Button()
        Me.SuspendLayout()
        '
        'lvwPreview
        '
        Me.lvwPreview.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwPreview.Columns.AddRange(New ColumnHeader() {Me.colName, Me.colDescription})
        Me.lvwPreview.FullRowSelect = True
        Me.lvwPreview.GridLines = True
        Me.lvwPreview.Location = New System.Drawing.Point(-3, 58)
        Me.lvwPreview.MultiSelect = False
        Me.lvwPreview.Name = "lvwPreview"
        Me.lvwPreview.Size = New System.Drawing.Size(666, 452)
        Me.lvwPreview.Sorting = System.Windows.Forms.SortOrder.Ascending
        Me.lvwPreview.TabIndex = 0
        Me.lvwPreview.UseCompatibleStateImageBehavior = False
        Me.lvwPreview.View = System.Windows.Forms.View.Details
        '
        'colName
        '
        Me.colName.Text = "Protein Name"
        Me.colName.Width = 200
        '
        'colDescription
        '
        Me.colDescription.Text = "Description Line"
        Me.colDescription.Width = 352
        '
        'txtLineCount
        '
        Me.txtLineCount.Location = New System.Drawing.Point(277, 26)
        Me.txtLineCount.Name = "txtLineCount"
        Me.txtLineCount.Size = New System.Drawing.Size(140, 24)
        Me.txtLineCount.TabIndex = 1
        Me.txtLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'cmdRefresh
        '
        Me.cmdRefresh.Location = New System.Drawing.Point(430, 16)
        Me.cmdRefresh.Name = "cmdRefresh"
        Me.cmdRefresh.Size = New System.Drawing.Size(106, 36)
        Me.cmdRefresh.TabIndex = 2
        Me.cmdRefresh.Text = "&Refresh List"
        '
        'lblLineCount
        '
        Me.lblLineCount.Location = New System.Drawing.Point(275, 6)
        Me.lblLineCount.Name = "lblLineCount"
        Me.lblLineCount.Size = New System.Drawing.Size(137, 20)
        Me.lblLineCount.TabIndex = 3
        Me.lblLineCount.Text = "# Lines to Preview"
        '
        'lblPreviewTitle
        '
        Me.lblPreviewTitle.Font = New System.Drawing.Font("Tahoma", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPreviewTitle.Location = New System.Drawing.Point(3, 34)
        Me.lblPreviewTitle.Name = "lblPreviewTitle"
        Me.lblPreviewTitle.Size = New System.Drawing.Size(257, 19)
        Me.lblPreviewTitle.TabIndex = 4
        Me.lblPreviewTitle.Text = "Preview of File Contents"
        '
        'cmdClose
        '
        Me.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdClose.Location = New System.Drawing.Point(542, 16)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(106, 36)
        Me.cmdClose.TabIndex = 5
        Me.cmdClose.Text = "&Close"
        '
        'frmFilePreview
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.CancelButton = Me.cmdClose
        Me.ClientSize = New System.Drawing.Size(660, 510)
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.lblLineCount)
        Me.Controls.Add(Me.cmdRefresh)
        Me.Controls.Add(Me.txtLineCount)
        Me.Controls.Add(Me.lvwPreview)
        Me.Controls.Add(Me.lblPreviewTitle)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(342, 437)
        Me.Name = "frmFilePreview"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Preview of: "
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Event RefreshRequest(lineCount As Integer)
    Shadows Event FormClosing()

    Private ReadOnly validationRegex As Regex
    Private m_currentLineCount As Integer = 100

    WriteOnly Property WindowName As String
        Set
            Me.Text = Value
        End Set
    End Property

    ReadOnly Property FormVisibility As Boolean
        Get
            Return Me.Visible
        End Get
    End Property

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        RaiseEvent RefreshRequest(Me.m_currentLineCount)
        If Me.cmdRefresh.Enabled = True Then
            Me.cmdRefresh.Enabled = False
        End If
    End Sub

    Private Sub txtLineCount_Validating(sender As Object, e As CancelEventArgs) Handles txtLineCount.Validating
        Dim value As Integer
        Dim m As Match
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

    Private Sub frmFilePreview_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.txtLineCount.Text = Me.m_currentLineCount.ToString
        RaiseEvent RefreshRequest(Me.m_currentLineCount)
    End Sub

    Private Sub txtLineCount_TextChanged(sender As Object, e As EventArgs) Handles txtLineCount.TextChanged
        Dim countText As String = Me.txtLineCount.Text
        If validationRegex.IsMatch(countText) Then
            Me.cmdRefresh.Enabled = True
        Else
            Me.cmdRefresh.Enabled = False
        End If
    End Sub

    Private Sub frmFilePreview_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        RaiseEvent FormClosing()
    End Sub

    Private Sub cmdClose_Click(sender As Object, e As EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub
End Class
