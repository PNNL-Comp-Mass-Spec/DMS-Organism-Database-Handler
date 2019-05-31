Public Class frmAddNewCollection
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

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
    Friend WithEvents gbxMetaData As GroupBox
    Friend WithEvents txtCollectionName As TextBox
    Friend WithEvents lblCollectionName As Label
    Friend WithEvents cboOrganismPicker As ComboBox
    Friend WithEvents lblOrganismPicker As Label
    Friend WithEvents lblAuthorityPicker As Label
    Friend WithEvents cboAuthorityPicker As ComboBox
    Friend WithEvents cmdAddOrganism As Button
    Friend WithEvents cmdAddAuthority As Button
    Friend WithEvents cmdCancel As Button
    Friend WithEvents cmdOK As Button
    Friend WithEvents lblProteinCount As Label
    Friend WithEvents lblResidueCount As Label
    Friend WithEvents lblDescription As Label
    Friend WithEvents lblSource As Label
    Friend WithEvents txtSource As TextBox
    Friend WithEvents txtDescription As TextBox
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.txtCollectionName = New TextBox()
        Me.lblCollectionName = New Label()
        Me.cboOrganismPicker = New ComboBox()
        Me.lblOrganismPicker = New Label()
        Me.lblAuthorityPicker = New Label()
        Me.cboAuthorityPicker = New ComboBox()
        Me.cmdAddOrganism = New Button()
        Me.cmdAddAuthority = New Button()
        Me.gbxMetaData = New GroupBox()
        Me.lblDescription = New Label()
        Me.txtDescription = New TextBox()
        Me.cmdCancel = New Button()
        Me.cmdOK = New Button()
        Me.lblProteinCount = New Label()
        Me.lblResidueCount = New Label()
        Me.lblSource = New Label()
        Me.txtSource = New TextBox()
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
        Me.txtCollectionName.Size = New System.Drawing.Size(454, 24)
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
        Me.cboOrganismPicker.Location = New System.Drawing.Point(6, 262)
        Me.cboOrganismPicker.Name = "cboOrganismPicker"
        Me.cboOrganismPicker.Size = New System.Drawing.Size(418, 25)
        Me.cboOrganismPicker.TabIndex = 2
        '
        'lblOrganismPicker
        '
        Me.lblOrganismPicker.Location = New System.Drawing.Point(3, 241)
        Me.lblOrganismPicker.Name = "lblOrganismPicker"
        Me.lblOrganismPicker.Size = New System.Drawing.Size(140, 20)
        Me.lblOrganismPicker.TabIndex = 3
        Me.lblOrganismPicker.Text = "Organism"
        '
        'lblAuthorityPicker
        '
        Me.lblAuthorityPicker.Location = New System.Drawing.Point(3, 292)
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
        Me.cboAuthorityPicker.Location = New System.Drawing.Point(6, 313)
        Me.cboAuthorityPicker.Name = "cboAuthorityPicker"
        Me.cboAuthorityPicker.Size = New System.Drawing.Size(418, 25)
        Me.cboAuthorityPicker.TabIndex = 5
        '
        'cmdAddOrganism
        '
        Me.cmdAddOrganism.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdAddOrganism.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdAddOrganism.Enabled = False
        Me.cmdAddOrganism.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAddOrganism.Location = New System.Drawing.Point(432, 262)
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
        Me.cmdAddAuthority.Location = New System.Drawing.Point(432, 313)
        Me.cmdAddAuthority.Name = "cmdAddAuthority"
        Me.cmdAddAuthority.Size = New System.Drawing.Size(28, 25)
        Me.cmdAddAuthority.TabIndex = 11
        Me.cmdAddAuthority.Text = "+"
        '
        'gbxMetaData
        '
        Me.gbxMetaData.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbxMetaData.Controls.Add(Me.lblSource)
        Me.gbxMetaData.Controls.Add(Me.txtSource)
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
        Me.gbxMetaData.Size = New System.Drawing.Size(488, 354)
        Me.gbxMetaData.TabIndex = 13
        Me.gbxMetaData.TabStop = False
        Me.gbxMetaData.Text = "Collection Information"
        '
        'lblDescription
        '
        Me.lblDescription.Location = New System.Drawing.Point(14, 68)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(140, 20)
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
        Me.txtDescription.Size = New System.Drawing.Size(454, 56)
        Me.txtDescription.TabIndex = 0
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Location = New System.Drawing.Point(393, 391)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(105, 28)
        Me.cmdCancel.TabIndex = 14
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Location = New System.Drawing.Point(275, 391)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(105, 28)
        Me.cmdOK.TabIndex = 15
        Me.cmdOK.Text = "OK"
        '
        'lblProteinCount
        '
        Me.lblProteinCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblProteinCount.Location = New System.Drawing.Point(11, 386)
        Me.lblProteinCount.Name = "lblProteinCount"
        Me.lblProteinCount.Size = New System.Drawing.Size(140, 14)
        Me.lblProteinCount.TabIndex = 17
        Me.lblProteinCount.Text = "Protein Count: -"
        '
        'lblResidueCount
        '
        Me.lblResidueCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblResidueCount.Location = New System.Drawing.Point(11, 405)
        Me.lblResidueCount.Name = "lblResidueCount"
        Me.lblResidueCount.Size = New System.Drawing.Size(140, 15)
        Me.lblResidueCount.TabIndex = 16
        Me.lblResidueCount.Text = "Residue Count: -"
        '
        'lblSource
        '
        Me.lblSource.Location = New System.Drawing.Point(14, 152)
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Size = New System.Drawing.Size(300, 20)
        Me.lblSource.TabIndex = 13
        Me.lblSource.Text = "Source (person, url, ftp site, etc.)"
        '
        'txtSource
        '
        Me.txtSource.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtSource.BackColor = System.Drawing.SystemColors.Window
        Me.txtSource.Location = New System.Drawing.Point(17, 173)
        Me.txtSource.MaxLength = 256
        Me.txtSource.Multiline = True
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New System.Drawing.Size(454, 56)
        Me.txtSource.TabIndex = 12
        '
        'frmAddNewCollection
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(7, 17)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(510, 430)
        Me.Controls.Add(Me.lblProteinCount)
        Me.Controls.Add(Me.lblResidueCount)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.gbxMetaData)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MaximumSize = New System.Drawing.Size(896, 475)
        Me.MinimumSize = New System.Drawing.Size(448, 150)
        Me.Name = "frmAddNewCollection"
        Me.Text = "Upload a Protein Collection"
        Me.gbxMetaData.ResumeLayout(False)
        Me.gbxMetaData.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Protected m_CollectionName As String
    Protected m_Description As String
    Protected m_CollectionSource As String

    Protected m_OrganismID As Integer
    Protected m_AnnotationTypeID As Integer

    Protected m_AnnotationTypes As DataTable
    Protected m_Organisms As DataTable

    Protected m_Local_File As Boolean

    Private Sub frmAddNewCollection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If IsLocalFile Then
            cboAuthorityPicker.Enabled = True
            cboOrganismPicker.Enabled = True
            'txtCollectionName.Visible = False
            txtCollectionName.Text = m_CollectionName

            m_Organisms.Rows(0).Item("Display_Name") = " -- Select an Organism --"
            m_Organisms.Rows(0).Item("ID") = 0
            m_Organisms.AcceptChanges()

            BindToCombo(cboAuthorityPicker, m_AnnotationTypes, "Display_Name", "ID")
            BindToCombo(cboOrganismPicker, m_Organisms, "Display_Name", "ID")
            'cboOrganismPicker.Items.RemoveAt(0)
            cboOrganismPicker.SelectedValue = 1
            AddHandler cboOrganismPicker.SelectedIndexChanged, AddressOf cboOrganismPicker_SelectedIndexChanged
            AddHandler cboAuthorityPicker.SelectedIndexChanged, AddressOf cboAuthorityPicker_SelectedIndexChanged

        Else
            txtCollectionName.Visible = True
            txtCollectionName.Text = m_CollectionName
            BindToCombo(cboOrganismPicker, m_Organisms, "Display_Name", "ID")
            cboOrganismPicker.SelectedValue = m_OrganismID
            BindToCombo(cboAuthorityPicker, m_AnnotationTypes, "Display_Name", "ID")
            cboAuthorityPicker.SelectedValue = m_AnnotationTypeID
            cboAuthorityPicker.Enabled = False
            cboOrganismPicker.Enabled = False

        End If



    End Sub

    Friend Property IsLocalFile As Boolean
        Get
            Return m_Local_File
        End Get
        Set
            m_Local_File = Value
        End Set
    End Property

    Friend Property CollectionName As String
        Get
            Return m_CollectionName
        End Get
        Set
            m_CollectionName = Value
        End Set
    End Property

    Friend Property CollectionDescription As String
        Get
            Return m_Description
        End Get
        Set
            m_Description = Value
        End Set
    End Property

    Friend Property CollectionSource As String
        Get
            Return m_CollectionSource
        End Get
        Set
            m_CollectionSource = Value
        End Set
    End Property

    Friend WriteOnly Property OrganismList As DataTable
        Set
            m_Organisms = Value
        End Set
    End Property

    Friend WriteOnly Property AnnotationTypes As DataTable
        Set
            m_AnnotationTypes = Value
        End Set
    End Property

    Friend Property OrganismID As Integer
        Get
            Return m_OrganismID
        End Get
        Set
            m_OrganismID = Value
        End Set
    End Property

    Friend Property AnnotationTypeID As Integer
        Get
            Return m_AnnotationTypeID
        End Get
        Set
            m_AnnotationTypeID = Value
        End Set
    End Property


    Protected Sub BindToCombo(
        cbo As ComboBox,
        list As DataTable,
        DisplayMember As String,
        ValueMember As String)

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
            .Add("Display_Name", Type.GetType("System.String"))
            .Add("ID", Type.GetType("System.Int32"))
        End With

        dr = tmpTable.NewRow
        dr.Item("Display_Name") = " -- Determined from Existing Collection(s) --"
        dr.Item("ID") = 0
        tmpTable.Rows.Add(dr)

        Return tmpTable
    End Function



#Region " Event Handlers "

    Private Sub txtCollectionName_Leave(sender As Object, e As EventArgs) Handles txtCollectionName.Leave
        m_CollectionName = txtCollectionName.Text
    End Sub

    Private Sub cboOrganismPicker_SelectedIndexChanged(sender As Object, e As EventArgs)
        m_OrganismID = CInt(cboOrganismPicker.SelectedValue)
        If m_OrganismID = 0 Then
            cmdOK.Enabled = False
        Else
            cmdOK.Enabled = True
        End If

    End Sub

    Private Sub cboAuthorityPicker_SelectedIndexChanged(sender As Object, e As EventArgs)
        m_AnnotationTypeID = CInt(cboAuthorityPicker.SelectedValue)
    End Sub

    Private Sub cmdAddOrganism_Click(sender As Object, e As EventArgs) Handles cmdAddOrganism.Click

    End Sub

    Private Sub cmdAddAuthority_Click(sender As Object, e As EventArgs) Handles cmdAddAuthority.Click

    End Sub

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        m_CollectionName = txtCollectionName.Text
        m_Description = txtDescription.Text
        m_CollectionSource = txtSource.Text
        m_OrganismID = CInt(cboOrganismPicker.SelectedValue)
        m_AnnotationTypeID = CInt(cboAuthorityPicker.SelectedValue)
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        m_CollectionName = Nothing
        m_OrganismID = Nothing
        m_AnnotationTypeID = Nothing
    End Sub


#End Region

End Class
