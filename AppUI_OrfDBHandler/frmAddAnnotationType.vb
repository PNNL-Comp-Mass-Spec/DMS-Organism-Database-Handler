Public Class frmAddAnnotationType
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

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
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents VisualStyleProvider1 As Skybound.VisualStyles.VisualStyleProvider
    Friend WithEvents lblAnnTypeName As System.Windows.Forms.Label
    Friend WithEvents txtAnnTypeName As System.Windows.Forms.TextBox
    Friend WithEvents txtDescription As System.Windows.Forms.TextBox
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents txtTypeExample As System.Windows.Forms.TextBox
    Friend WithEvents lblTypeExample As System.Windows.Forms.Label
    Friend WithEvents lblAuthority As System.Windows.Forms.Label
    Friend WithEvents cboAuthorityName As System.Windows.Forms.ComboBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lblAnnTypeName = New System.Windows.Forms.Label
        Me.txtAnnTypeName = New System.Windows.Forms.TextBox
        Me.txtDescription = New System.Windows.Forms.TextBox
        Me.lblDescription = New System.Windows.Forms.Label
        Me.txtTypeExample = New System.Windows.Forms.TextBox
        Me.lblTypeExample = New System.Windows.Forms.Label
        Me.cmdCancel = New System.Windows.Forms.Button
        Me.cmdOK = New System.Windows.Forms.Button
        Me.VisualStyleProvider1 = New Skybound.VisualStyles.VisualStyleProvider
        Me.lblAuthority = New System.Windows.Forms.Label
        Me.cboAuthorityName = New System.Windows.Forms.ComboBox
        Me.SuspendLayout()
        '
        'lblAnnTypeName
        '
        Me.lblAnnTypeName.Location = New System.Drawing.Point(6, 8)
        Me.lblAnnTypeName.Name = "lblAnnTypeName"
        Me.lblAnnTypeName.Size = New System.Drawing.Size(266, 16)
        Me.lblAnnTypeName.TabIndex = 0
        Me.lblAnnTypeName.Text = "Annotation Type Name (64 char max)"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblAnnTypeName, True)
        '
        'txtAnnTypeName
        '
        Me.txtAnnTypeName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAnnTypeName.Location = New System.Drawing.Point(6, 24)
        Me.txtAnnTypeName.MaxLength = 64
        Me.txtAnnTypeName.Name = "txtAnnTypeName"
        Me.txtAnnTypeName.Size = New System.Drawing.Size(276, 21)
        Me.txtAnnTypeName.TabIndex = 1
        Me.txtAnnTypeName.Text = ""
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtAnnTypeName, True)
        '
        'txtDescription
        '
        Me.txtDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDescription.Location = New System.Drawing.Point(7, 66)
        Me.txtDescription.MaxLength = 128
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(276, 21)
        Me.txtDescription.TabIndex = 3
        Me.txtDescription.Text = ""
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtDescription, True)
        '
        'lblDescription
        '
        Me.lblDescription.Location = New System.Drawing.Point(7, 50)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(265, 16)
        Me.lblDescription.TabIndex = 2
        Me.lblDescription.Text = "Annotation Type Description (128 char max)"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblDescription, True)
        '
        'txtTypeExample
        '
        Me.txtTypeExample.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtTypeExample.Location = New System.Drawing.Point(7, 108)
        Me.txtTypeExample.MaxLength = 128
        Me.txtTypeExample.Name = "txtTypeExample"
        Me.txtTypeExample.Size = New System.Drawing.Size(276, 21)
        Me.txtTypeExample.TabIndex = 5
        Me.txtTypeExample.Text = ""
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.txtTypeExample, True)
        '
        'lblTypeExample
        '
        Me.lblTypeExample.Location = New System.Drawing.Point(7, 92)
        Me.lblTypeExample.Name = "lblTypeExample"
        Me.lblTypeExample.Size = New System.Drawing.Size(265, 16)
        Me.lblTypeExample.TabIndex = 4
        Me.lblTypeExample.Text = "Example of Annotation (optional, 128 char max)"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblTypeExample, True)
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel.Location = New System.Drawing.Point(208, 182)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.TabIndex = 6
        Me.cmdCancel.Text = "Cancel"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdCancel, True)
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdOK.Location = New System.Drawing.Point(124, 182)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.TabIndex = 7
        Me.cmdOK.Text = "OK"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.cmdOK, True)
        '
        'lblAuthority
        '
        Me.lblAuthority.Location = New System.Drawing.Point(7, 136)
        Me.lblAuthority.Name = "lblAuthority"
        Me.lblAuthority.Size = New System.Drawing.Size(265, 16)
        Me.lblAuthority.TabIndex = 8
        Me.lblAuthority.Text = "Naming Authority"
        Me.VisualStyleProvider1.SetVisualStyleSupport(Me.lblAuthority, True)
        '
        'cboAuthorityName
        '
        Me.cboAuthorityName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboAuthorityName.Location = New System.Drawing.Point(7, 152)
        Me.cboAuthorityName.Name = "cboAuthorityName"
        Me.cboAuthorityName.Size = New System.Drawing.Size(276, 21)
        Me.cboAuthorityName.TabIndex = 9
        '
        'frmAddAnnotationType
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(292, 216)
        Me.Controls.Add(Me.cboAuthorityName)
        Me.Controls.Add(Me.lblAuthority)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.txtTypeExample)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.txtAnnTypeName)
        Me.Controls.Add(Me.lblTypeExample)
        Me.Controls.Add(Me.lblDescription)
        Me.Controls.Add(Me.lblAnnTypeName)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximumSize = New System.Drawing.Size(298, 240)
        Me.MinimumSize = New System.Drawing.Size(298, 240)
        Me.Name = "frmAddAnnotationType"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Add Annotation Type"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private m_TypeName As String
    Private m_Description As String
    Private m_Example As String
    Private m_AuthID As Integer
    Private m_AuthoritiesTable As DataTable
    Private m_PSConnectionString As String


#Region " Return Properties "

    Property TypeName() As String
        Get
            Return Me.m_TypeName
        End Get
        Set(ByVal Value As String)
            Me.m_TypeName = Value
        End Set
    End Property

    Property Description() As String
        Get
            Return Me.m_Description
        End Get
        Set(ByVal Value As String)
            Me.m_Description = Value
        End Set
    End Property

    Property Example() As String
        Get
            Return Me.m_Example
        End Get
        Set(ByVal Value As String)
            Me.m_Example = Value
        End Set
    End Property

    Property AuthorityID() As Integer
        Get
            Return Me.m_AuthID
        End Get
        Set(ByVal Value As Integer)
            Me.m_AuthID = Value
        End Set
    End Property

    WriteOnly Property ConnectionString() As String
        Set(ByVal Value As String)
            Me.m_PSConnectionString = Value
        End Set
    End Property

    WriteOnly Property AuthorityTable() As DataTable
        Set(ByVal Value As DataTable)
            Me.m_AuthoritiesTable = Value
        End Set
    End Property

#End Region

    Private Sub frmAddAnnotationType_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not Me.m_TypeName Is Nothing Then
            Me.txtAnnTypeName.Text = Me.m_TypeName
        End If

        If Not Me.m_Description Is Nothing Then
            Me.txtDescription.Text = Me.m_Description
        End If

        If Not Me.m_Example Is Nothing Then
            Me.txtTypeExample.Text = Me.m_Example
        End If

        Me.LoadAuthoritiesList()

        If Me.m_AuthID > 0 Then
            Me.cboAuthorityName.SelectedValue = Me.m_AuthID
            Me.cboAuthorityName.Select()
        End If


    End Sub

    Private Sub LoadAuthoritiesList()

        'If Me.m_au Then
        RemoveHandler cboAuthorityName.SelectedIndexChanged, AddressOf cboAuthorityName_SelectedIndexChanged

        Dim dr As DataRow = Me.m_AuthoritiesTable.NewRow

        With dr
            .Item("ID") = -2
            .Item("Display_Name") = "Add New Naming Authority..."
            .Item("Details") = "Brings up a dialog box to allow adding a naming authority to the list"
        End With



        Dim pk1(0) As DataColumn
        pk1(0) = m_AuthoritiesTable.Columns("ID")
        m_AuthoritiesTable.PrimaryKey = pk1

        If m_AuthoritiesTable.Rows.Contains(dr.Item("ID")) Then
            Dim rdr As DataRow = m_AuthoritiesTable.Rows.Find(dr.Item("ID"))
            m_AuthoritiesTable.Rows.Remove(rdr)
        End If

        m_AuthoritiesTable.Rows.Add(dr)


        With Me.cboAuthorityName
            .DataSource = Me.m_AuthoritiesTable
            .DisplayMember = "Display_Name"
            .ValueMember = "ID"
        End With

        AddHandler cboAuthorityName.SelectedIndexChanged, AddressOf cboAuthorityName_SelectedIndexChanged
    End Sub

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        Me.m_TypeName = Me.txtAnnTypeName.Text
        Me.m_Description = Me.txtDescription.Text
        Me.m_Example = Me.txtTypeExample.Text
        Me.m_AuthID = CInt(Me.cboAuthorityName.SelectedValue)

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    'Private Sub txtAuthWeb_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txtTypeExample.Validating
    '    Dim txt As TextBox = DirectCast(sender, TextBox)
    '    Dim tmpAddress As String = Me.ValidateWebAddressFormat(txt.Text)
    '    txt.Text = tmpAddress
    'End Sub

    Private Sub cboAuthorityName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboAuthorityName.SelectedIndexChanged
        Dim cbo As ComboBox = DirectCast(sender, ComboBox)
        Dim tmpUpInfo As Protein_Uploader.IUploadProteins.UploadInfo

        If cbo.SelectedValue.GetType Is System.Type.GetType("System.Int32") Then
            Me.m_AuthID = CInt(cbo.SelectedValue)
        Else
            'Me.m_SelectedAuthorityID = 0
        End If

        Dim tmpAuthID As Integer
        Dim tmpIndex As Integer

        If Me.m_AuthID = -2 Then
            'Bring up addition dialog
            Dim AuthAdd As New clsAddNamingAuthority(Me.m_PSConnectionString)
            AuthAdd.FormLocation = New System.Drawing.Point(Me.Left + 20, Me.Top + 30)
            tmpAuthID = AuthAdd.AddNamingAuthority

            If Not AuthAdd.EntryExists And tmpAuthID > 0 Then

                Dim dr As DataRow
                dr = Me.m_AuthoritiesTable.NewRow

                With dr
                    .Item("ID") = tmpAuthID
                    .Item("Display_Name") = AuthAdd.ShortName
                    .Item("Details") = AuthAdd.FullName
                End With

                Me.m_AuthoritiesTable.Rows.Add(dr)
                Me.m_AuthoritiesTable.AcceptChanges()
                Me.LoadAuthoritiesList()
                Me.m_AuthID = tmpAuthID
            End If
            Me.cboAuthorityName.SelectedValue = tmpAuthID

            'Me.cboAuthorityName.SelectedIndex = 
            AuthAdd = Nothing

        End If


        'If Me.lvwSelectedFiles.SelectedItems.Count > 0 Then
        '    Dim li As ListViewItem
        '    For Each li In Me.lvwSelectedFiles.SelectedItems
        '        tmpUpInfo = DirectCast(Me.m_SelectedFileList.Item(li.SubItems(3).Text), Protein_Uploader.IUploadProteins.UploadInfo)
        '        Me.m_SelectedFileList.Item(li.SubItems(3).Text) = _
        '            New Protein_Uploader.IUploadProteins.UploadInfo(tmpUpInfo.FileInformation, Me.m_SelectedOrganismID, tmpUpInfo.AuthorityID)
        '        li.SubItems(2).Text = cbo.Text
        '    Next
        'End If

    End Sub
End Class
