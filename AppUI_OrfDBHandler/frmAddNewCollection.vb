Public Class frmAddNewCollection
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
    Friend WithEvents gbxMetaData As System.Windows.Forms.GroupBox
    Friend WithEvents txtCollectionName As System.Windows.Forms.TextBox
    Friend WithEvents lblCollectionName As System.Windows.Forms.Label
    Friend WithEvents cboOrganismPicker As System.Windows.Forms.ComboBox
    Friend WithEvents lblOrganismPicker As System.Windows.Forms.Label
    Friend WithEvents lblAuthorityPicker As System.Windows.Forms.Label
    Friend WithEvents cboAuthorityPicker As System.Windows.Forms.ComboBox
    Friend WithEvents cmdAddOrganism As System.Windows.Forms.Button
    Friend WithEvents cmdAddAuthority As System.Windows.Forms.Button
    Friend WithEvents cmdCancel As System.Windows.Forms.Button
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents lblProteinCount As System.Windows.Forms.Label
    Friend WithEvents lblResidueCount As System.Windows.Forms.Label
    Friend WithEvents lblDescription As System.Windows.Forms.Label
    Friend WithEvents txtDescription As System.Windows.Forms.TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtCollectionName = New System.Windows.Forms.TextBox()
        Me.lblCollectionName = New System.Windows.Forms.Label()
        Me.cboOrganismPicker = New System.Windows.Forms.ComboBox()
        Me.lblOrganismPicker = New System.Windows.Forms.Label()
        Me.lblAuthorityPicker = New System.Windows.Forms.Label()
        Me.cboAuthorityPicker = New System.Windows.Forms.ComboBox()
        Me.cmdAddOrganism = New System.Windows.Forms.Button()
        Me.cmdAddAuthority = New System.Windows.Forms.Button()
        Me.gbxMetaData = New System.Windows.Forms.GroupBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.txtDescription = New System.Windows.Forms.TextBox()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdOK = New System.Windows.Forms.Button()
        Me.lblProteinCount = New System.Windows.Forms.Label()
        Me.lblResidueCount = New System.Windows.Forms.Label()
        Me.gbxMetaData.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtCollectionName
        '
        Me.txtCollectionName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtCollectionName.BackColor = System.Drawing.SystemColors.Window
        Me.txtCollectionName.Location = New System.Drawing.Point(17, 36)
        Me.txtCollectionName.Name = "txtCollectionName"
        Me.txtCollectionName.Size = New System.Drawing.Size(374, 24)
        Me.txtCollectionName.TabIndex = 0
        '
        'lblCollectionName
        '
        Me.lblCollectionName.Location = New System.Drawing.Point(14, 19)
        Me.lblCollectionName.Name = "lblCollectionName"
        Me.lblCollectionName.Size = New System.Drawing.Size(140, 15)
        Me.lblCollectionName.TabIndex = 1
        Me.lblCollectionName.Text = "Name"
        '
        'cboOrganismPicker
        '
        Me.cboOrganismPicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboOrganismPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOrganismPicker.Location = New System.Drawing.Point(17, 169)
        Me.cboOrganismPicker.Name = "cboOrganismPicker"
        Me.cboOrganismPicker.Size = New System.Drawing.Size(338, 25)
        Me.cboOrganismPicker.TabIndex = 2
        '
        'lblOrganismPicker
        '
        Me.lblOrganismPicker.Location = New System.Drawing.Point(14, 148)
        Me.lblOrganismPicker.Name = "lblOrganismPicker"
        Me.lblOrganismPicker.Size = New System.Drawing.Size(140, 20)
        Me.lblOrganismPicker.TabIndex = 3
        Me.lblOrganismPicker.Text = "Organism"
        '
        'lblAuthorityPicker
        '
        Me.lblAuthorityPicker.Location = New System.Drawing.Point(14, 199)
        Me.lblAuthorityPicker.Name = "lblAuthorityPicker"
        Me.lblAuthorityPicker.Size = New System.Drawing.Size(140, 20)
        Me.lblAuthorityPicker.TabIndex = 4
        Me.lblAuthorityPicker.Text = "Authority"
        '
        'cboAuthorityPicker
        '
        Me.cboAuthorityPicker.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboAuthorityPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboAuthorityPicker.Location = New System.Drawing.Point(17, 220)
        Me.cboAuthorityPicker.Name = "cboAuthorityPicker"
        Me.cboAuthorityPicker.Size = New System.Drawing.Size(338, 25)
        Me.cboAuthorityPicker.TabIndex = 5
        '
        'cmdAddOrganism
        '
        Me.cmdAddOrganism.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdAddOrganism.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdAddOrganism.Enabled = False
        Me.cmdAddOrganism.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddOrganism.Location = New System.Drawing.Point(363, 169)
        Me.cmdAddOrganism.Name = "cmdAddOrganism"
        Me.cmdAddOrganism.Size = New System.Drawing.Size(28, 25)
        Me.cmdAddOrganism.TabIndex = 10
        Me.cmdAddOrganism.Text = "+"
        '
        'cmdAddAuthority
        '
        Me.cmdAddAuthority.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdAddAuthority.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdAddAuthority.Enabled = False
        Me.cmdAddAuthority.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddAuthority.Location = New System.Drawing.Point(363, 220)
        Me.cmdAddAuthority.Name = "cmdAddAuthority"
        Me.cmdAddAuthority.Size = New System.Drawing.Size(28, 25)
        Me.cmdAddAuthority.TabIndex = 11
        Me.cmdAddAuthority.Text = "+"
        '
        'gbxMetaData
        '
        Me.gbxMetaData.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxMetaData.Controls.Add(Me.cmdAddOrganism)
        Me.gbxMetaData.Controls.Add(Me.cboAuthorityPicker)
        Me.gbxMetaData.Controls.Add(Me.cmdAddAuthority)
        Me.gbxMetaData.Controls.Add(Me.lblAuthorityPicker)
        Me.gbxMetaData.Controls.Add(Me.txtCollectionName)
        Me.gbxMetaData.Controls.Add(Me.cboOrganismPicker)
        Me.gbxMetaData.Controls.Add(Me.lblCollectionName)
        Me.gbxMetaData.Controls.Add(Me.lblOrganismPicker)
        Me.gbxMetaData.Controls.Add(Me.lblDescription)
        Me.gbxMetaData.Controls.Add(Me.txtDescription)
        Me.gbxMetaData.Location = New System.Drawing.Point(11, 7)
        Me.gbxMetaData.Name = "gbxMetaData"
        Me.gbxMetaData.Size = New System.Drawing.Size(408, 258)
        Me.gbxMetaData.TabIndex = 13
        Me.gbxMetaData.TabStop = False
        Me.gbxMetaData.Text = "Collection Information"
        '
        'lblDescription
        '
        Me.lblDescription.Location = New System.Drawing.Point(14, 68)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(140, 15)
        Me.lblDescription.TabIndex = 1
        Me.lblDescription.Text = "Description"
        '
        'txtDescription
        '
        Me.txtDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtDescription.BackColor = System.Drawing.SystemColors.Window
        Me.txtDescription.Location = New System.Drawing.Point(17, 89)
        Me.txtDescription.MaxLength = 256
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New System.Drawing.Size(374, 56)
        Me.txtDescription.TabIndex = 0
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(313, 292)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(105, 28)
        Me.cmdCancel.TabIndex = 14
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Location = New System.Drawing.Point(195, 292)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(105, 28)
        Me.cmdOK.TabIndex = 15
        Me.cmdOK.Text = "OK"
        '
        'lblProteinCount
        '
        Me.lblProteinCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblProteinCount.Location = New System.Drawing.Point(11, 287)
        Me.lblProteinCount.Name = "lblProteinCount"
        Me.lblProteinCount.Size = New System.Drawing.Size(140, 14)
        Me.lblProteinCount.TabIndex = 17
        Me.lblProteinCount.Text = "Protein Count: -"
        '
        'lblResidueCount
        '
        Me.lblResidueCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblResidueCount.Location = New System.Drawing.Point(11, 306)
        Me.lblResidueCount.Name = "lblResidueCount"
        Me.lblResidueCount.Size = New System.Drawing.Size(140, 15)
        Me.lblResidueCount.TabIndex = 16
        Me.lblResidueCount.Text = "Residue Count: -"
        '
        'frmAddNewCollection
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(430, 317)
        Me.Controls.Add(Me.lblProteinCount)
        Me.Controls.Add(Me.lblResidueCount)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.gbxMetaData)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MaximumSize = New System.Drawing.Size(896, 362)
        Me.MinimumSize = New System.Drawing.Size(448, 362)
        Me.Name = "frmAddNewCollection"
        Me.Text = "Upload a Protein Collection"
        Me.gbxMetaData.ResumeLayout(False)
        Me.gbxMetaData.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Protected m_CollectionName As String
    Protected m_Description As String
    Protected m_OrganismID As Integer
    Protected m_AnnotationTypeID As Integer

    Protected m_AnnotationTypes As DataTable
    Protected m_Organisms As DataTable
    Protected m_StandinTable As DataTable

    Protected m_Local_File As Boolean

    Private Sub frmAddNewCollection_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Me.IsLocalFile Then
            Me.cboAuthorityPicker.Enabled = True
            Me.cboOrganismPicker.Enabled = True
            'Me.txtCollectionName.Visible = False
            Me.txtCollectionName.Text = Me.m_CollectionName

            Me.m_Organisms.Rows(0).Item("Display_Name") = " -- Select an Organism --"
            Me.m_Organisms.Rows(0).Item("ID") = 0
            Me.m_Organisms.AcceptChanges()

            Me.BindToCombo(Me.cboAuthorityPicker, Me.m_AnnotationTypes, "Display_Name", "ID")
            Me.BindToCombo(Me.cboOrganismPicker, Me.m_Organisms, "Display_Name", "ID")
            'Me.cboOrganismPicker.Items.RemoveAt(0)
            Me.cboOrganismPicker.SelectedValue = 1
            AddHandler cboOrganismPicker.SelectedIndexChanged, AddressOf cboOrganismPicker_SelectedIndexChanged
            AddHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged

        Else
            'Me.m_StandinTable = Me.MakeStandinDatabase
            Me.txtCollectionName.Visible = True
            Me.txtCollectionName.Text = Me.m_CollectionName
            'Me.BindToCombo(Me.cboOrganismPicker, Me.m_StandinTable, "Display_Name", "ID")
            Me.BindToCombo(Me.cboOrganismPicker, Me.m_Organisms, "Display_Name", "ID")
            Me.cboOrganismPicker.SelectedValue = Me.m_OrganismID
            'Me.BindToCombo(Me.cboAuthorityPicker, Me.m_StandinTable, "Display_Name", "ID")
            Me.BindToCombo(Me.cboAuthorityPicker, Me.m_AnnotationTypes, "Display_Name", "ID")
            Me.cboAuthorityPicker.SelectedValue = Me.m_AnnotationTypeID
            Me.cboAuthorityPicker.Enabled = False
            Me.cboOrganismPicker.Enabled = False

        End If



    End Sub

    Friend Property IsLocalFile() As Boolean
        Get
            Return Me.m_Local_File
        End Get
        Set(ByVal Value As Boolean)
            Me.m_Local_File = Value
        End Set
    End Property

    Friend Property CollectionName() As String
        Get
            Return Me.m_CollectionName
        End Get
        Set(ByVal Value As String)
            Me.m_CollectionName = Value
        End Set
    End Property

    Friend Property CollectionDescription() As String
        Get
            Return Me.m_Description
        End Get
        Set(ByVal Value As String)
            Me.m_Description = Value
        End Set
    End Property

    Friend WriteOnly Property OrganismList() As DataTable
        Set(ByVal Value As DataTable)
            Me.m_Organisms = Value
        End Set
    End Property

    Friend WriteOnly Property AnnotationTypes() As DataTable
        Set(ByVal Value As DataTable)
            Me.m_AnnotationTypes = Value
        End Set
    End Property

    Friend Property OrganismID() As Integer
        Get
            Return Me.m_OrganismID
        End Get
        Set(ByVal Value As Integer)
            Me.m_OrganismID = Value
        End Set
    End Property

    Friend Property AnnotationTypeID() As Integer
        Get
            Return Me.m_AnnotationTypeID
        End Get
        Set(ByVal Value As Integer)
            Me.m_AnnotationTypeID = Value
        End Set
    End Property


    Protected Sub BindToCombo( _
        ByVal cbo As ComboBox, _
        ByVal list As DataTable, _
        ByVal DisplayMember As String, _
        ByVal ValueMember As String)

        'Dim dr As DataRow
        'For Each dr In list.Rows
        '    Debug.WriteLine(dr.Item(0).ToString & ", " & dr.Item(1).ToString & ", " & dr.Item(2).ToString & ", ")
        'Next

        With cbo
            .DataSource = list
            .DisplayMember = DisplayMember
            '.DisplayMember = list.Columns("Display_Name").ColumnName.ToString
            .ValueMember = ValueMember
            '.ValueMember = list.Columns("ID").ColumnName.ToString
        End With

    End Sub

    Protected Function MakeStandinDatabase() As DataTable
        Dim tmpTable As New DataTable
        Dim dr As DataRow
        With tmpTable.Columns
            .Add("Display_Name", System.Type.GetType("System.String"))
            .Add("ID", System.Type.GetType("System.Int32"))
        End With

        dr = tmpTable.NewRow
        dr.Item("Display_Name") = " -- Determined from Existing Collection(s) --"
        dr.Item("ID") = 0
        tmpTable.Rows.Add(dr)

        Return tmpTable
    End Function



#Region " Event Handlers "

    Private Sub txtCollectionName_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtCollectionName.Leave
        Me.m_CollectionName = Me.txtCollectionName.Text
    End Sub

    Private Sub cboOrganismPicker_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.m_OrganismID = CInt(Me.cboOrganismPicker.SelectedValue)
        If Me.m_OrganismID = 0 Then
            Me.cmdOK.Enabled = False
        Else
            Me.cmdOK.Enabled = True
        End If

    End Sub

    Private Sub cboAuthorityPicker_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.m_AnnotationTypeID = CInt(Me.cboAuthorityPicker.SelectedValue)
    End Sub

    Private Sub cmdAddOrganism_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddOrganism.Click

    End Sub

    Private Sub cmdAddAuthority_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddAuthority.Click

    End Sub

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        Me.m_CollectionName = Me.txtCollectionName.Text
        Me.m_Description = Me.txtDescription.Text
        Me.m_OrganismID = CInt(Me.cboOrganismPicker.SelectedValue)
        Me.m_AnnotationTypeID = CInt(Me.cboAuthorityPicker.SelectedValue)
    End Sub

    Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
        Me.m_CollectionName = Nothing
        Me.m_OrganismID = Nothing
        Me.m_AnnotationTypeID = Nothing
    End Sub


#End Region

End Class
