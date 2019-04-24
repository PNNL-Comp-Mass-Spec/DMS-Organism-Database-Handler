Imports System.Windows.Forms

Public Class frmBatchUploadFromFileList
    Inherits Form

    Private ReadOnly m_AnnotationTypeList As DataTable
    Private ReadOnly m_OrganismList As DataTable
    Private m_SelectedFilesCollection As Hashtable
    Private Const m_SaveFileName As String = "FASTAFile_NamingAuth_XRef.txt"
    Private ReadOnly m_SavePath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly.Location)


#Region " Windows Form Designer generated code "

    Public Sub New(
        AuthorityList As DataTable,
        AnnotationTypeList As DataTable,
        OrganismList As DataTable)

        MyBase.New()
        m_AnnotationTypeList = AnnotationTypeList
        m_OrganismList = OrganismList

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
    'Friend WithEvents lvwNewNames As ListView
    Friend WithEvents cmdUploadFiles As Button
    Friend WithEvents colFileName As ColumnHeader
    Friend WithEvents colFilePath As ColumnHeader
    Friend WithEvents colAnnType As ColumnHeader
    Friend WithEvents cmdCheckAll As Button
    Friend WithEvents colOrganism As ColumnHeader
    Friend WithEvents cboAnnotationType As ComboBox
    Friend WithEvents cmdUncheckAll As Button
    Friend WithEvents lblAnnotationType As Label
    Friend WithEvents lvwFiles As ListView
    Friend WithEvents lblOrganismPicker As Label
    Friend WithEvents cboOrganismPicker As ComboBox
    Friend WithEvents txtFilePath As TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.cmdUploadFiles = New Button()
        Me.lblOrganismPicker = New Label()
        Me.cboOrganismPicker = New ComboBox()
        Me.lvwFiles = New ListView()
        Me.colFileName = CType(New ColumnHeader(), ColumnHeader)
        Me.colFilePath = CType(New ColumnHeader(), ColumnHeader)
        Me.colOrganism = CType(New ColumnHeader(), ColumnHeader)
        Me.colAnnType = CType(New ColumnHeader(), ColumnHeader)
        Me.cboAnnotationType = New ComboBox()
        Me.lblAnnotationType = New Label()
        Me.cmdCheckAll = New Button()
        Me.cmdUncheckAll = New Button()
        Me.txtFilePath = New TextBox()
        Me.SuspendLayout()
        '
        'cmdUploadFiles
        '
        Me.cmdUploadFiles.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.cmdUploadFiles.DialogResult = DialogResult.OK
        Me.cmdUploadFiles.FlatStyle = FlatStyle.System
        Me.cmdUploadFiles.Location = New System.Drawing.Point(587, 630)
        Me.cmdUploadFiles.Name = "cmdUploadFiles"
        Me.cmdUploadFiles.Size = New System.Drawing.Size(221, 27)
        Me.cmdUploadFiles.TabIndex = 9
        Me.cmdUploadFiles.Text = "Upload Checked Files"
        '
        'lblOrganismPicker
        '
        Me.lblOrganismPicker.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.lblOrganismPicker.Location = New System.Drawing.Point(14, 569)
        Me.lblOrganismPicker.Name = "lblOrganismPicker"
        Me.lblOrganismPicker.Size = New System.Drawing.Size(319, 22)
        Me.lblOrganismPicker.TabIndex = 16
        Me.lblOrganismPicker.Text = "Organism"
        '
        'cboOrganismPicker
        '
        Me.cboOrganismPicker.Anchor = CType(((AnchorStyles.Bottom Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.cboOrganismPicker.Location = New System.Drawing.Point(14, 589)
        Me.cboOrganismPicker.Name = "cboOrganismPicker"
        Me.cboOrganismPicker.Size = New System.Drawing.Size(385, 25)
        Me.cboOrganismPicker.TabIndex = 17
        '
        'lvwFiles
        '
        Me.lvwFiles.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
            Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.lvwFiles.CheckBoxes = True
        Me.lvwFiles.Columns.AddRange(New ColumnHeader() {Me.colFileName, Me.colFilePath, Me.colOrganism, Me.colAnnType})
        Me.lvwFiles.FullRowSelect = True
        Me.lvwFiles.GridLines = True
        Me.lvwFiles.HideSelection = False
        Me.lvwFiles.Location = New System.Drawing.Point(1, 2)
        Me.lvwFiles.Name = "lvwFiles"
        Me.lvwFiles.Size = New System.Drawing.Size(820, 555)
        Me.lvwFiles.Sorting = SortOrder.Ascending
        Me.lvwFiles.TabIndex = 19
        Me.lvwFiles.UseCompatibleStateImageBehavior = False
        Me.lvwFiles.View = View.Details
        '
        'colFileName
        '
        Me.colFileName.Text = "FileName"
        Me.colFileName.Width = 215
        '
        'colFilePath
        '
        Me.colFilePath.Text = "Directory Path of File"
        Me.colFilePath.Width = 247
        '
        'colOrganism
        '
        Me.colOrganism.Text = "Organism"
        Me.colOrganism.Width = 125
        '
        'colAnnType
        '
        Me.colAnnType.Text = "Annotation Type"
        Me.colAnnType.Width = 117
        '
        'cboAnnotationType
        '
        Me.cboAnnotationType.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.cboAnnotationType.Location = New System.Drawing.Point(419, 589)
        Me.cboAnnotationType.Name = "cboAnnotationType"
        Me.cboAnnotationType.Size = New System.Drawing.Size(392, 25)
        Me.cboAnnotationType.TabIndex = 21
        '
        'lblAnnotationType
        '
        Me.lblAnnotationType.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.lblAnnotationType.Location = New System.Drawing.Point(422, 569)
        Me.lblAnnotationType.Name = "lblAnnotationType"
        Me.lblAnnotationType.Size = New System.Drawing.Size(294, 22)
        Me.lblAnnotationType.TabIndex = 20
        Me.lblAnnotationType.Text = "Annotation Type"
        '
        'cmdCheckAll
        '
        Me.cmdCheckAll.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.cmdCheckAll.FlatStyle = FlatStyle.System
        Me.cmdCheckAll.Location = New System.Drawing.Point(14, 630)
        Me.cmdCheckAll.Name = "cmdCheckAll"
        Me.cmdCheckAll.Size = New System.Drawing.Size(140, 27)
        Me.cmdCheckAll.TabIndex = 22
        Me.cmdCheckAll.Text = "Check All"
        '
        'cmdUncheckAll
        '
        Me.cmdUncheckAll.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
        Me.cmdUncheckAll.FlatStyle = FlatStyle.System
        Me.cmdUncheckAll.Location = New System.Drawing.Point(165, 630)
        Me.cmdUncheckAll.Name = "cmdUncheckAll"
        Me.cmdUncheckAll.Size = New System.Drawing.Size(140, 27)
        Me.cmdUncheckAll.TabIndex = 23
        Me.cmdUncheckAll.Text = "Uncheck All"
        '
        'txtFilePath
        '
        Me.txtFilePath.Location = New System.Drawing.Point(319, 641)
        Me.txtFilePath.Name = "txtFilePath"
        Me.txtFilePath.Size = New System.Drawing.Size(431, 24)
        Me.txtFilePath.TabIndex = 24
        '
        'frmBatchUploadFromFileList
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.ClientSize = New System.Drawing.Size(822, 667)
        Me.Controls.Add(Me.txtFilePath)
        Me.Controls.Add(Me.cmdUncheckAll)
        Me.Controls.Add(Me.cmdCheckAll)
        Me.Controls.Add(Me.cboAnnotationType)
        Me.Controls.Add(Me.lblAnnotationType)
        Me.Controls.Add(Me.lvwFiles)
        Me.Controls.Add(Me.cboOrganismPicker)
        Me.Controls.Add(Me.lblOrganismPicker)
        Me.Controls.Add(Me.cmdUploadFiles)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MinimumSize = New System.Drawing.Size(840, 712)
        Me.Name = "frmBatchUploadFromFileList"
        Me.Text = "Batch Upload FASTA Files from FileList"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Sub frmBatchUploadFromFileList_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.PopulateDropDowns()
        If Not Me.FileCollection Is Nothing Then
            'Me.LoadFileNamingAuthorities()
            Me.PopulateListView()
        End If
    End Sub

    Private Sub frmBatchUploadFromFileList_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        Me.SaveFileNamingAuthorities()

    End Sub

    Property FileCollection As Hashtable

    Private Sub SaveFileNamingAuthorities()
        Dim saveFilePath As String = System.IO.Path.Combine(Me.m_SavePath, m_SaveFileName)
        Dim sw As System.IO.StreamWriter
        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(saveFilePath)
        If fi.Exists Then
            fi.Delete()
        End If
        sw = New System.IO.StreamWriter(System.IO.Path.Combine(Me.m_SavePath, m_SaveFileName))

        Dim fli As clsBatchUploadFromFileList.FileListInfo

        For Each fli In Me.FileCollection.Values
            If fli.AnnotationTypeID > 0 Then
                sw.Write(fli.FileName)
                sw.Write(ControlChars.Tab)
                sw.Write(fli.AnnotationTypeID.ToString)
                sw.Flush()
            End If
        Next

        sw.Close()

    End Sub

    'Private Sub LoadFileNamingAuthorities()
    '    Dim loadFilePath As String = System.IO.Path.Combine(Me.m_SavePath, Me.m_SaveFileName)
    '    Dim fi As System.IO.FileInfo = New System.IO.FileInfo(loadFilePath)
    '    Dim tr As System.IO.TextReader
    '    Dim s As String

    '    Dim tmpFileName As String
    '    Dim tmpAnnotationID As Integer
    '    Dim fields() As String

    '    Dim fli As clsBatchUploadFromFileList.FileListInfo

    '    If fi.Exists And Not Me.m_FileCollection Is Nothing Then
    '        tr = fi.OpenText
    '        s = tr.ReadLine
    '        While Not s Is Nothing
    '            fields = s.Split(ControlChars.Tab)
    '            tmpFileName = fields(0)
    '            tmpAnnotationID = CInt(fields(1))
    '            If tmpAnnotationID > 0 Then
    '                fli = DirectCast(Me.m_FileCollection.Item(tmpFileName), clsBatchUploadFromFileList.FileListInfo)
    '                fli.AnnotationTypeID = tmpAnnotationID
    '                Me.m_FileCollection(tmpFileName) = fli
    '            End If
    '            s = tr.ReadLine
    '        End While
    '    Else
    '        Exit Sub
    '    End If

    'End Sub

    Private Sub LoadFileNamingAuthorities()
        Dim loadFilePath As String = System.IO.Path.Combine(Me.m_SavePath, m_SaveFileName)
        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(loadFilePath)
        'Dim tr As System.IO.TextReader
        'Dim s As String

        'Dim tmpFileName As String
        'Dim tmpAnnotationID As Integer
        'Dim fields() As String

        'Dim fli As clsBatchUploadFromFileList.FileListInfo

        'If fi.Exists And Not Me.m_FileCollection Is Nothing Then
        '    tr = fi.OpenText
        '    s = tr.ReadLine
        '    While Not s Is Nothing
        '        fields = s.Split(ControlChars.Tab)
        '        tmpFileName = fields(0)
        '        tmpAnnotationID = CInt(fields(1))
        '        If tmpAnnotationID > 0 Then
        '            fli = DirectCast(Me.m_FileCollection.Item(tmpFileName), clsBatchUploadFromFileList.FileListInfo)
        '            fli.AnnotationTypeID = tmpAnnotationID
        '            Me.m_FileCollection(tmpFileName) = fli
        '        End If
        '        s = tr.ReadLine
        '    End While
        'Else
        '    Exit Sub
        'End If



    End Sub
    
    ReadOnly Property SelectedFilesCollection As Hashtable
        Get
            Return Me.m_SelectedFilesCollection
        End Get
    End Property

    Private Sub PopulateDropDowns()

        Dim dr As DataRow

        dr = Me.m_AnnotationTypeList.NewRow
        With dr
            .Item("ID") = 0
            .Item("Display_Name") = "---------"
        End With
        Me.m_AnnotationTypeList.Rows.InsertAt(dr, 0)
        Me.m_AnnotationTypeList.AcceptChanges()

        dr = Me.m_OrganismList.NewRow
        With dr
            .Item("ID") = 0
            .Item("Display_Name") = "---------"
        End With
        Me.m_OrganismList.Rows.InsertAt(dr, 0)
        Me.m_OrganismList.AcceptChanges()

        RemoveHandler cboAnnotationType.SelectedIndexChanged, AddressOf cboAnnotationType_SelectedIndexChanged

        With Me.cboAnnotationType
            .BeginUpdate()
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
            .DataSource = Me.m_AnnotationTypeList
            .EndUpdate()
        End With
        Me.cboAnnotationType.Text = "---------"

        AddHandler cboAnnotationType.SelectedIndexChanged, AddressOf cboAnnotationType_SelectedIndexChanged


        RemoveHandler cboOrganismPicker.SelectedIndexChanged, AddressOf cboOrganismPicker_SelectedIndexChanged
        With Me.cboOrganismPicker
            .BeginUpdate()
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
            .DataSource = Me.m_OrganismList
            .EndUpdate()
        End With
        Me.cboOrganismPicker.Text = "---------"

        AddHandler cboOrganismPicker.SelectedIndexChanged, AddressOf cboOrganismPicker_SelectedIndexChanged

    End Sub

    Sub PopulateListView()
        Dim fli As clsBatchUploadFromFileList.FileListInfo
        Dim li As ListViewItem
        Dim foundRows() As DataRow

        RemoveHandler lvwFiles.SelectedIndexChanged, AddressOf lvwFiles_SelectedIndexChanged

        If Me.FileCollection.Count > 0 Then
            Me.lvwFiles.BeginUpdate()
            For Each fli In Me.FileCollection.Values
                li = New ListViewItem
                li.Text = fli.FileName
                li.SubItems.Add(fli.FullFilePath)
                li.SubItems.Add(fli.OrganismName)
                If fli.AnnotationTypeID > 0 Then
                    foundRows = Me.m_AnnotationTypeList.Select("ID = " & fli.AnnotationTypeID)
                    fli.AnnotationType = foundRows(0).Item(1).ToString
                    li.SubItems.Add(fli.AnnotationType)
                Else
                    li.SubItems.Add("---------")
                End If
                Me.lvwFiles.Items.Add(li)
            Next
            Me.lvwFiles.EndUpdate()

            AddHandler lvwFiles.SelectedIndexChanged, AddressOf lvwFiles_SelectedIndexChanged

        End If
    End Sub

    Private Function BuildSelectedFilesList() As Integer
        If Me.m_SelectedFilesCollection Is Nothing Then
            Me.m_SelectedFilesCollection = New Hashtable
        Else
            Me.m_SelectedFilesCollection.Clear()
        End If

        Dim li As ListViewItem

        For Each li In Me.lvwFiles.CheckedItems
            Me.m_SelectedFilesCollection.Add(
                li.Text,
                DirectCast(Me.FileCollection.Item(li.Text), clsBatchUploadFromFileList.FileListInfo))
        Next

        Return Me.lvwFiles.CheckedItems.Count

    End Function

    Private Sub cmdUploadFiles_Click(sender As System.Object, e As System.EventArgs) Handles cmdUploadFiles.Click

        Dim selectedCount As Integer = Me.BuildSelectedFilesList()
        If selectedCount > 0 Then
            Me.DialogResult = DialogResult.OK
        End If
    End Sub

    Private Sub cmdCheckAll_Click(sender As System.Object, e As System.EventArgs) Handles cmdCheckAll.Click
        Dim li As ListViewItem
        For Each li In Me.lvwFiles.Items
            li.Checked = True
        Next
    End Sub

    Private Sub cmdUncheckAll_Click(sender As System.Object, e As System.EventArgs) Handles cmdUncheckAll.Click
        Dim li As ListViewItem
        For Each li In Me.lvwFiles.Items
            li.Checked = False
        Next
    End Sub

    Private Sub cboOrganismPicker_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cboOrganismPicker.SelectedIndexChanged
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)
        Dim li As ListViewItem
        Dim fli As clsBatchUploadFromFileList.FileListInfo

        If Me.lvwFiles.SelectedItems.Count > 0 Then
            For Each li In Me.lvwFiles.SelectedItems
                li.SubItems(2).Text = cbo.Text
                fli = DirectCast(Me.FileCollection.Item(li.Text),
                    clsBatchUploadFromFileList.FileListInfo)
                fli.NamingAuthorityID = CInt(cbo.SelectedValue)
                Me.FileCollection.Item(li.Text) = fli
            Next
        End If
    End Sub

    Private Sub cboAnnotationType_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cboAnnotationType.SelectedIndexChanged
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)
        Dim li As ListViewItem
        Dim fli As clsBatchUploadFromFileList.FileListInfo

        If Me.lvwFiles.SelectedItems.Count > 0 Then
            For Each li In Me.lvwFiles.SelectedItems
                li.SubItems(3).Text = cbo.Text
                fli = DirectCast(Me.FileCollection.Item(li.Text),
                    clsBatchUploadFromFileList.FileListInfo)
                fli.AnnotationTypeID = CInt(cbo.SelectedValue)
                Me.FileCollection.Item(li.Text) = fli
            Next
        End If
    End Sub

    Private Sub lvwFiles_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lvwFiles.SelectedIndexChanged
        Dim fli As clsBatchUploadFromFileList.FileListInfo
        Dim li As ListViewItem

        RemoveHandler cboAnnotationType.SelectedIndexChanged, AddressOf cboAnnotationType_SelectedIndexChanged
        RemoveHandler cboOrganismPicker.SelectedIndexChanged, AddressOf cboOrganismPicker_SelectedIndexChanged
        If Me.lvwFiles.SelectedItems.Count = 0 Then
            Exit Sub
        ElseIf Me.lvwFiles.SelectedItems.Count = 1 Then
            li = Me.lvwFiles.SelectedItems(0)
            fli = DirectCast(Me.FileCollection.Item(li.Text), clsBatchUploadFromFileList.FileListInfo)
            If fli.AnnotationTypeID > 0 Then
                Me.cboAnnotationType.SelectedValue = fli.AnnotationTypeID
            End If
            If fli.OrganismID > 0 Then
                Me.cboOrganismPicker.SelectedValue = fli.OrganismID
            End If
            If fli.FullFilePath.Length > 0 Then
                Me.txtFilePath.Text = System.IO.Path.GetDirectoryName(fli.FullFilePath)
            End If
        ElseIf Me.lvwFiles.SelectedItems.Count > 1 Then
            Me.cboAnnotationType.SelectedValue = 0
            Me.cboOrganismPicker.SelectedValue = 0
        End If

        AddHandler cboAnnotationType.SelectedIndexChanged, AddressOf cboAnnotationType_SelectedIndexChanged
        AddHandler cboOrganismPicker.SelectedIndexChanged, AddressOf cboOrganismPicker_SelectedIndexChanged


    End Sub
End Class
