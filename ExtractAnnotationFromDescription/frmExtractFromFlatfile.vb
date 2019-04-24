Public Class frmExtractFromFlatfile
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New(AuthorityList As Hashtable, PSConnectionString As String)
        MyBase.New()
        Me.m_AuthorityList = AuthorityList
        Me.m_PSConnectionString = PSConnectionString
        'This call is required by the Windows Form Designer.
        InitializeComponent()

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
    Friend WithEvents lblNewNames As System.Windows.Forms.Label
    'Friend WithEvents lvwNewNames As System.Windows.Forms.ListView
    Friend WithEvents cmdUploadAnnotations As System.Windows.Forms.Button
    Friend WithEvents lblCurrentCollectionInfo As System.Windows.Forms.Label
    Friend WithEvents lblNamingAuthority As System.Windows.Forms.Label
    Friend WithEvents cboNamingAuthority As System.Windows.Forms.ComboBox
    Friend WithEvents lvwProteins As System.Windows.Forms.ListView
    Friend WithEvents lvwNewNames As System.Windows.Forms.ListView
    Friend WithEvents colAnnGroup As System.Windows.Forms.ColumnHeader
    Friend WithEvents colAnnGroupName As System.Windows.Forms.ColumnHeader
    Friend WithEvents colNamingAuth As System.Windows.Forms.ColumnHeader
    Friend WithEvents chkUseHeader As System.Windows.Forms.CheckBox
    Friend WithEvents colSplitChar As System.Windows.Forms.ColumnHeader
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lblNewNames = New System.Windows.Forms.Label()
        Me.lvwNewNames = New System.Windows.Forms.ListView()
        Me.colAnnGroup = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colAnnGroupName = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colNamingAuth = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSplitChar = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.cmdUploadAnnotations = New System.Windows.Forms.Button()
        Me.lblCurrentCollectionInfo = New System.Windows.Forms.Label()
        Me.lblNamingAuthority = New System.Windows.Forms.Label()
        Me.cboNamingAuthority = New System.Windows.Forms.ComboBox()
        Me.lvwProteins = New System.Windows.Forms.ListView()
        Me.chkUseHeader = New System.Windows.Forms.CheckBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'lblNewNames
        '
        Me.lblNewNames.Location = New System.Drawing.Point(11, 287)
        Me.lblNewNames.Name = "lblNewNames"
        Me.lblNewNames.Size = New System.Drawing.Size(482, 19)
        Me.lblNewNames.TabIndex = 6
        Me.lblNewNames.Text = "Annotations Extracted from Loaded Flat text file"
        '
        'lvwNewNames
        '
        Me.lvwNewNames.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwNewNames.CheckBoxes = True
        Me.lvwNewNames.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colAnnGroup, Me.colAnnGroupName, Me.colNamingAuth, Me.colSplitChar})
        Me.lvwNewNames.FullRowSelect = True
        Me.lvwNewNames.GridLines = True
        Me.lvwNewNames.HideSelection = False
        Me.lvwNewNames.Location = New System.Drawing.Point(11, 306)
        Me.lvwNewNames.MultiSelect = False
        Me.lvwNewNames.Name = "lvwNewNames"
        Me.lvwNewNames.Size = New System.Drawing.Size(797, 259)
        Me.lvwNewNames.TabIndex = 18
        Me.lvwNewNames.UseCompatibleStateImageBehavior = False
        Me.lvwNewNames.View = System.Windows.Forms.View.Details
        '
        'colAnnGroup
        '
        Me.colAnnGroup.Text = "Group ID"
        Me.colAnnGroup.Width = 59
        '
        'colAnnGroupName
        '
        Me.colAnnGroupName.Text = "Group Name"
        Me.colAnnGroupName.Width = 145
        '
        'colNamingAuth
        '
        Me.colNamingAuth.Text = "Naming Authority"
        Me.colNamingAuth.Width = 290
        '
        'colSplitChar
        '
        Me.colSplitChar.Text = "Delimiter"
        Me.colSplitChar.Width = 59
        '
        'cmdUploadAnnotations
        '
        Me.cmdUploadAnnotations.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdUploadAnnotations.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdUploadAnnotations.Location = New System.Drawing.Point(587, 631)
        Me.cmdUploadAnnotations.Name = "cmdUploadAnnotations"
        Me.cmdUploadAnnotations.Size = New System.Drawing.Size(221, 27)
        Me.cmdUploadAnnotations.TabIndex = 9
        Me.cmdUploadAnnotations.Text = "Upload Checked Groups"
        '
        'lblCurrentCollectionInfo
        '
        Me.lblCurrentCollectionInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCurrentCollectionInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCurrentCollectionInfo.Location = New System.Drawing.Point(0, 257)
        Me.lblCurrentCollectionInfo.Name = "lblCurrentCollectionInfo"
        Me.lblCurrentCollectionInfo.Size = New System.Drawing.Size(825, 20)
        Me.lblCurrentCollectionInfo.TabIndex = 12
        Me.lblCurrentCollectionInfo.TextAlign = System.Drawing.ContentAlignment.BottomCenter
        '
        'lblNamingAuthority
        '
        Me.lblNamingAuthority.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblNamingAuthority.Location = New System.Drawing.Point(4, 573)
        Me.lblNamingAuthority.Name = "lblNamingAuthority"
        Me.lblNamingAuthority.Size = New System.Drawing.Size(247, 21)
        Me.lblNamingAuthority.TabIndex = 16
        Me.lblNamingAuthority.Text = "Naming Authority"
        '
        'cboNamingAuthority
        '
        Me.cboNamingAuthority.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboNamingAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboNamingAuthority.Location = New System.Drawing.Point(14, 592)
        Me.cboNamingAuthority.Name = "cboNamingAuthority"
        Me.cboNamingAuthority.Size = New System.Drawing.Size(676, 25)
        Me.cboNamingAuthority.TabIndex = 17
        '
        'lvwProteins
        '
        Me.lvwProteins.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvwProteins.FullRowSelect = True
        Me.lvwProteins.GridLines = True
        Me.lvwProteins.Location = New System.Drawing.Point(1, 2)
        Me.lvwProteins.MultiSelect = False
        Me.lvwProteins.Name = "lvwProteins"
        Me.lvwProteins.Size = New System.Drawing.Size(819, 251)
        Me.lvwProteins.TabIndex = 19
        Me.lvwProteins.UseCompatibleStateImageBehavior = False
        Me.lvwProteins.View = System.Windows.Forms.View.Details
        '
        'chkUseHeader
        '
        Me.chkUseHeader.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkUseHeader.Location = New System.Drawing.Point(553, 284)
        Me.chkUseHeader.Name = "chkUseHeader"
        Me.chkUseHeader.Size = New System.Drawing.Size(258, 20)
        Me.chkUseHeader.TabIndex = 20
        Me.chkUseHeader.Text = "Use First Line as Group Names?"
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(701, 592)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(98, 24)
        Me.TextBox1.TabIndex = 23
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label1.Location = New System.Drawing.Point(699, 573)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 21)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Delimiter"
        '
        'frmExtractFromFlatfile
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.ClientSize = New System.Drawing.Size(822, 667)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.chkUseHeader)
        Me.Controls.Add(Me.lvwProteins)
        Me.Controls.Add(Me.cboNamingAuthority)
        Me.Controls.Add(Me.lblNamingAuthority)
        Me.Controls.Add(Me.cmdUploadAnnotations)
        Me.Controls.Add(Me.lvwNewNames)
        Me.Controls.Add(Me.lblNewNames)
        Me.Controls.Add(Me.lblCurrentCollectionInfo)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(840, 712)
        Me.Name = "frmExtractFromFlatfile"
        Me.Text = "Extract Annotations From Flatfile"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private m_UseHeaderInfo As Boolean = False
    Private m_Extract As clsExtractFromFlatfile
    Private m_AuthorityList As Hashtable
    Private m_CurrentAuthorityID As Integer
    Private m_CurrentGroupID As Integer
    Private m_PSConnectionString As String

    Private Sub frmExtractFromFlatfile_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.m_Extract = New clsExtractFromFlatfile(Me.m_AuthorityList, Me.m_PSConnectionString)
        Dim openFrm As New System.Windows.Forms.OpenFileDialog
        Dim r As System.Windows.Forms.DialogResult
        Dim filePath As String

        RemoveHandler chkUseHeader.CheckedChanged, AddressOf chkUseHeader_CheckedChanged

        If Me.m_UseHeaderInfo = True Then
            Me.chkUseHeader.CheckState = System.Windows.Forms.CheckState.Checked
        Else
            Me.chkUseHeader.CheckState = System.Windows.Forms.CheckState.Unchecked
        End If

        AddHandler chkUseHeader.CheckedChanged, AddressOf chkUseHeader_CheckedChanged

        Me.LoadAuthorityCombobox(Me.cboNamingAuthority, Me.m_AuthorityList)

        With openFrm
            .Filter = "tab-delimited text files (*.txt)|*.txt|All files (*.*)|*.*"
            .AddExtension = True
            .DefaultExt = "txt"
            .DereferenceLinks = True
            .FilterIndex = 0
            .Multiselect = False
            .Title = "Select a tab delimited text file to load"
            r = .ShowDialog()
        End With

        If r = System.Windows.Forms.DialogResult.OK Then
            filePath = openFrm.FileName
            Me.m_Extract.LoadFile(filePath, Chr(9), Me.m_UseHeaderInfo)
            Me.LoadRawFileListView()
            Me.LoadAnnotationGroupListView()
        Else
            Me.Close()
        End If

    End Sub

    Private Sub lvwNewNames_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lvwNewNames.SelectedIndexChanged
        Dim lvw As System.Windows.Forms.ListView = DirectCast(sender, System.Windows.Forms.ListView)
        If lvw.SelectedItems.Count > 0 Then
            Me.m_CurrentGroupID = CInt(lvw.SelectedItems(0).Text)
            Me.cboNamingAuthority.SelectedValue = CInt(lvw.SelectedItems(0).Tag)
            'Me.cboNamingAuthority.Select()
        End If

    End Sub

    Private Sub chkUseHeader_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkUseHeader.CheckedChanged
        Dim chk As System.Windows.Forms.CheckBox =
            DirectCast(sender, System.Windows.Forms.CheckBox)
        If chk.CheckState = System.Windows.Forms.CheckState.Checked Then
            Me.m_UseHeaderInfo = True
        Else
            Me.m_UseHeaderInfo = False
        End If
        Me.m_Extract.LoadGroups(Chr(9), Me.m_UseHeaderInfo)
        Me.RefreshRawFileListViewHeaders()
        Me.LoadAnnotationGroupListView()
    End Sub

    Private Sub RefreshRawFileListViewHeaders()

        Dim columnCollection As Hashtable = Me.m_Extract.ColumnNames

        Dim columncount As Integer = columnCollection.Count
        Dim columnNumber As Integer
        For columnNumber = 1 To columncount
            Me.lvwProteins.Columns.Item(columnNumber - 1).Text = columnCollection.Item(columnNumber).ToString
        Next

        If Me.m_UseHeaderInfo Then
            Me.lvwProteins.Items.RemoveAt(0)
        Else
            Me.lvwProteins.Items.Insert(0, Me.m_Extract.HashToListViewItem(DirectCast(Me.m_Extract.FileContents(0), Hashtable), 1))
        End If

    End Sub

    Private Sub LoadRawFileListView()

        Dim fc = Me.m_Extract.FileContents
        Dim maxIndex As Integer = fc.Count - 1
        Dim maxColumnCount As Integer = 0

        For Each item In fc
            If item.Count > maxColumnCount Then
                maxColumnCount = item.Count
            End If
        Next

        Me.lvwProteins.BeginUpdate()

        Me.lvwProteins.Clear()

        'Create Columns
        Dim columnCollection As Hashtable = Me.m_Extract.ColumnNames
        Dim columnCount As Integer = columnCollection.Count
        Dim columnNumber As Integer
        For columnNumber = 1 To columnCount
            Dim ch = New System.Windows.Forms.ColumnHeader
            ch.Text = columnCollection.Item(columnNumber).ToString
            ch.Width = 70
            Me.lvwProteins.Columns.Add(ch)
        Next

        For lineCount = 0 To maxIndex
            Dim lineHash = fc.Item(lineCount)
            Dim lvItem = Me.m_Extract.HashToListViewItem(lineHash, lineCount)

            'lvItem = New System.Windows.Forms.ListViewItem((lineCount + 1).ToString)
            'columnCount = lineHash.Count
            'For columnNumber = 1 To columnCount

            '    item = lineHash.Item(columnNumber).ToString
            '    If item.Length > 0 Then
            '        lvItem.SubItems.Add(lineHash.Item(columnNumber).ToString)
            '    Else
            '        lvItem.SubItems.Add("---")
            '    End If
            'Next
            'blankColumnCount = maxColumnCount - columnCount
            'If blankColumnCount > 0 Then
            '    For columnNumber = 1 To blankColumnCount
            '        lvItem.SubItems.Add("---")
            '    Next
            'End If

            Me.lvwProteins.Items.Add(lvItem)
        Next

        Me.lvwProteins.EndUpdate()

    End Sub

    Private Sub LoadAnnotationGroupListView()
        Dim maxIndex As Integer = Me.m_Extract.Annotations.GroupCount
        Dim groupID As Integer
        Dim lvItem As System.Windows.Forms.ListViewItem

        Me.lvwNewNames.BeginUpdate()
        Me.lvwNewNames.Items.Clear()
        For groupID = 1 To maxIndex
            lvItem = Me.m_Extract.GetListViewItemForGroup(groupID)
            Me.lvwNewNames.Items.Add(lvItem)
        Next
        Me.lvwNewNames.EndUpdate()

    End Sub

    Private Sub LoadAuthorityCombobox(
        cbo As System.Windows.Forms.ComboBox,
        authList As Hashtable)

        Dim a As New ArrayList
        Dim memberEnum = authList.GetEnumerator

        cbo.BeginUpdate()
        While memberEnum.MoveNext()
            a.Add(New AuthorityContainer(memberEnum.Value.ToString, CInt(memberEnum.Key)))
        End While

        a.Sort(New AuthorityContainerComparer)

        cbo.DataSource = a
        cbo.DisplayMember = "AuthorityName"
        cbo.ValueMember = "AuthorityID"

        cbo.EndUpdate()
    End Sub

    Private Sub cboNamingAuthority_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cboNamingAuthority.SelectedIndexChanged
        Dim cbo = DirectCast(sender, System.Windows.Forms.ComboBox)

        If Me.lvwNewNames.SelectedItems.Count > 0 Then
            Me.m_Extract.ChangeAuthorityIDforGroup(Me.m_CurrentGroupID, CInt(cbo.SelectedValue))
            Me.lvwNewNames.SelectedItems(0).SubItems(2).Text = cbo.Text
            Me.lvwNewNames.SelectedItems(0).Tag = CInt(cbo.SelectedValue)
        End If

    End Sub

    Private Class AuthorityContainer
        Sub New(AuthorityName As String, AuthorityID As Integer)

            Me.AuthorityID = AuthorityID
            Me.AuthorityName = AuthorityName
        End Sub

        Public ReadOnly Property AuthorityName As String

        Public ReadOnly Property AuthorityID As Integer

    End Class

    Private Class AuthorityContainerComparer
        Implements IComparer

        Function Compare(x As Object, y As Object) As Integer Implements IComparer.Compare
            Dim Auth_1 = DirectCast(x, AuthorityContainer)
            Dim Auth_2 = DirectCast(y, AuthorityContainer)

            Dim Reference_1 As String = Auth_1.AuthorityName
            Dim Reference_2 As String = Auth_2.AuthorityName

            If Reference_1 > Reference_2 Then
                Return 1
            ElseIf Reference_1 < Reference_2 Then
                Return -1
            Else
                Return 0
            End If
        End Function

    End Class


End Class
