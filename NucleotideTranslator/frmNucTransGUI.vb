Public Class frmNucTransGUI
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
    Friend WithEvents gbxNucSeqSelect As System.Windows.Forms.GroupBox
    Friend WithEvents txtInputPath As System.Windows.Forms.TextBox
    Friend WithEvents cmdBrowseInput As System.Windows.Forms.Button
    Friend WithEvents lblInputPath As System.Windows.Forms.Label
    Friend WithEvents gbxTransOptions As System.Windows.Forms.GroupBox
    Friend WithEvents gbxDestinationSelect As System.Windows.Forms.GroupBox
    Friend WithEvents lblOutputPath As System.Windows.Forms.Label
    Friend WithEvents cmdBrowseOutput As System.Windows.Forms.Button
    Friend WithEvents txtOutputPath As System.Windows.Forms.TextBox
    Friend WithEvents cboTranslationTableSelect As System.Windows.Forms.ComboBox
    Friend WithEvents lblTranslationTableSelect As System.Windows.Forms.Label
    Friend WithEvents lblMinProteinSize As System.Windows.Forms.Label
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox2 As System.Windows.Forms.TextBox
    Friend WithEvents chkCircular As System.Windows.Forms.CheckBox
    Friend WithEvents cboFrameSelect As System.Windows.Forms.ComboBox
    Friend WithEvents lblFrameSelect As System.Windows.Forms.Label
    Friend WithEvents cmdStart As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.gbxNucSeqSelect = New System.Windows.Forms.GroupBox
        Me.lblInputPath = New System.Windows.Forms.Label
        Me.cmdBrowseInput = New System.Windows.Forms.Button
        Me.txtInputPath = New System.Windows.Forms.TextBox
        Me.gbxTransOptions = New System.Windows.Forms.GroupBox
        Me.cboFrameSelect = New System.Windows.Forms.ComboBox
        Me.chkCircular = New System.Windows.Forms.CheckBox
        Me.TextBox1 = New System.Windows.Forms.TextBox
        Me.cboTranslationTableSelect = New System.Windows.Forms.ComboBox
        Me.lblTranslationTableSelect = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.TextBox2 = New System.Windows.Forms.TextBox
        Me.lblFrameSelect = New System.Windows.Forms.Label
        Me.lblMinProteinSize = New System.Windows.Forms.Label
        Me.gbxDestinationSelect = New System.Windows.Forms.GroupBox
        Me.lblOutputPath = New System.Windows.Forms.Label
        Me.cmdBrowseOutput = New System.Windows.Forms.Button
        Me.txtOutputPath = New System.Windows.Forms.TextBox
        Me.cmdStart = New System.Windows.Forms.Button
        Me.gbxNucSeqSelect.SuspendLayout()
        Me.gbxTransOptions.SuspendLayout()
        Me.gbxDestinationSelect.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbxNucSeqSelect
        '
        Me.gbxNucSeqSelect.Controls.Add(Me.lblInputPath)
        Me.gbxNucSeqSelect.Controls.Add(Me.cmdBrowseInput)
        Me.gbxNucSeqSelect.Controls.Add(Me.txtInputPath)
        Me.gbxNucSeqSelect.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.gbxNucSeqSelect.Location = New System.Drawing.Point(8, 6)
        Me.gbxNucSeqSelect.Name = "gbxNucSeqSelect"
        Me.gbxNucSeqSelect.Size = New System.Drawing.Size(406, 68)
        Me.gbxNucSeqSelect.TabIndex = 0
        Me.gbxNucSeqSelect.TabStop = False
        Me.gbxNucSeqSelect.Text = "Select Nucleotide Sequence File"
        '
        'lblInputPath
        '
        Me.lblInputPath.Location = New System.Drawing.Point(8, 16)
        Me.lblInputPath.Name = "lblInputPath"
        Me.lblInputPath.Size = New System.Drawing.Size(100, 14)
        Me.lblInputPath.TabIndex = 2
        Me.lblInputPath.Text = "Input Path"
        '
        'cmdBrowseInput
        '
        Me.cmdBrowseInput.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowseInput.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBrowseInput.Location = New System.Drawing.Point(334, 32)
        Me.cmdBrowseInput.Name = "cmdBrowseInput"
        Me.cmdBrowseInput.Size = New System.Drawing.Size(62, 21)
        Me.cmdBrowseInput.TabIndex = 1
        Me.cmdBrowseInput.Text = "Browse..."
        '
        'txtInputPath
        '
        Me.txtInputPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtInputPath.Location = New System.Drawing.Point(10, 32)
        Me.txtInputPath.Name = "txtInputPath"
        Me.txtInputPath.Size = New System.Drawing.Size(316, 21)
        Me.txtInputPath.TabIndex = 0
        Me.txtInputPath.Text = ""
        '
        'gbxTransOptions
        '
        Me.gbxTransOptions.Controls.Add(Me.cboFrameSelect)
        Me.gbxTransOptions.Controls.Add(Me.chkCircular)
        Me.gbxTransOptions.Controls.Add(Me.TextBox1)
        Me.gbxTransOptions.Controls.Add(Me.cboTranslationTableSelect)
        Me.gbxTransOptions.Controls.Add(Me.lblTranslationTableSelect)
        Me.gbxTransOptions.Controls.Add(Me.Label1)
        Me.gbxTransOptions.Controls.Add(Me.TextBox2)
        Me.gbxTransOptions.Controls.Add(Me.lblFrameSelect)
        Me.gbxTransOptions.Controls.Add(Me.lblMinProteinSize)
        Me.gbxTransOptions.Location = New System.Drawing.Point(8, 80)
        Me.gbxTransOptions.Name = "gbxTransOptions"
        Me.gbxTransOptions.Size = New System.Drawing.Size(406, 114)
        Me.gbxTransOptions.TabIndex = 1
        Me.gbxTransOptions.TabStop = False
        Me.gbxTransOptions.Text = "Translation Options"
        '
        'cboFrameSelect
        '
        Me.cboFrameSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboFrameSelect.Items.AddRange(New Object() {"All", "3 Forward", "3 Reverse", "1", "2", "3", "4", "5", "6"})
        Me.cboFrameSelect.Location = New System.Drawing.Point(226, 78)
        Me.cboFrameSelect.Name = "cboFrameSelect"
        Me.cboFrameSelect.Size = New System.Drawing.Size(94, 21)
        Me.cboFrameSelect.TabIndex = 4
        '
        'chkCircular
        '
        Me.chkCircular.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.chkCircular.Location = New System.Drawing.Point(330, 78)
        Me.chkCircular.Name = "chkCircular"
        Me.chkCircular.Size = New System.Drawing.Size(68, 24)
        Me.chkCircular.TabIndex = 3
        Me.chkCircular.Text = "Circular?"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(10, 78)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(96, 21)
        Me.TextBox1.TabIndex = 2
        Me.TextBox1.Text = "30"
        Me.TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'cboTranslationTableSelect
        '
        Me.cboTranslationTableSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboTranslationTableSelect.Location = New System.Drawing.Point(10, 32)
        Me.cboTranslationTableSelect.Name = "cboTranslationTableSelect"
        Me.cboTranslationTableSelect.Size = New System.Drawing.Size(386, 21)
        Me.cboTranslationTableSelect.TabIndex = 0
        '
        'lblTranslationTableSelect
        '
        Me.lblTranslationTableSelect.Location = New System.Drawing.Point(8, 16)
        Me.lblTranslationTableSelect.Name = "lblTranslationTableSelect"
        Me.lblTranslationTableSelect.Size = New System.Drawing.Size(132, 14)
        Me.lblTranslationTableSelect.TabIndex = 1
        Me.lblTranslationTableSelect.Text = "Translation Table to Use"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(116, 62)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(102, 14)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Max Protein Length"
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(118, 78)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(96, 21)
        Me.TextBox2.TabIndex = 2
        Me.TextBox2.Text = "1000000"
        Me.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblFrameSelect
        '
        Me.lblFrameSelect.Location = New System.Drawing.Point(224, 62)
        Me.lblFrameSelect.Name = "lblFrameSelect"
        Me.lblFrameSelect.Size = New System.Drawing.Size(90, 12)
        Me.lblFrameSelect.TabIndex = 1
        Me.lblFrameSelect.Text = "Frame"
        '
        'lblMinProteinSize
        '
        Me.lblMinProteinSize.Location = New System.Drawing.Point(8, 62)
        Me.lblMinProteinSize.Name = "lblMinProteinSize"
        Me.lblMinProteinSize.Size = New System.Drawing.Size(98, 14)
        Me.lblMinProteinSize.TabIndex = 1
        Me.lblMinProteinSize.Text = "Min Protein Length"
        '
        'gbxDestinationSelect
        '
        Me.gbxDestinationSelect.Controls.Add(Me.lblOutputPath)
        Me.gbxDestinationSelect.Controls.Add(Me.cmdBrowseOutput)
        Me.gbxDestinationSelect.Controls.Add(Me.txtOutputPath)
        Me.gbxDestinationSelect.Location = New System.Drawing.Point(8, 200)
        Me.gbxDestinationSelect.Name = "gbxDestinationSelect"
        Me.gbxDestinationSelect.Size = New System.Drawing.Size(406, 68)
        Me.gbxDestinationSelect.TabIndex = 2
        Me.gbxDestinationSelect.TabStop = False
        Me.gbxDestinationSelect.Text = "Select Destination"
        '
        'lblOutputPath
        '
        Me.lblOutputPath.Location = New System.Drawing.Point(10, 16)
        Me.lblOutputPath.Name = "lblOutputPath"
        Me.lblOutputPath.Size = New System.Drawing.Size(100, 14)
        Me.lblOutputPath.TabIndex = 5
        Me.lblOutputPath.Text = "Output Path"
        '
        'cmdBrowseOutput
        '
        Me.cmdBrowseOutput.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdBrowseOutput.Location = New System.Drawing.Point(334, 32)
        Me.cmdBrowseOutput.Name = "cmdBrowseOutput"
        Me.cmdBrowseOutput.Size = New System.Drawing.Size(62, 21)
        Me.cmdBrowseOutput.TabIndex = 4
        Me.cmdBrowseOutput.Text = "Browse..."
        '
        'txtOutputPath
        '
        Me.txtOutputPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOutputPath.Location = New System.Drawing.Point(10, 32)
        Me.txtOutputPath.Name = "txtOutputPath"
        Me.txtOutputPath.Size = New System.Drawing.Size(316, 21)
        Me.txtOutputPath.TabIndex = 3
        Me.txtOutputPath.Text = ""
        '
        'cmdStart
        '
        Me.cmdStart.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.cmdStart.Location = New System.Drawing.Point(338, 278)
        Me.cmdStart.Name = "cmdStart"
        Me.cmdStart.Size = New System.Drawing.Size(75, 22)
        Me.cmdStart.TabIndex = 3
        Me.cmdStart.Text = "Translate..."
        '
        'frmNucTransGUI
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(422, 308)
        Me.Controls.Add(Me.cmdStart)
        Me.Controls.Add(Me.gbxDestinationSelect)
        Me.Controls.Add(Me.gbxTransOptions)
        Me.Controls.Add(Me.gbxNucSeqSelect)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmNucTransGUI"
        Me.Text = "Translate Nucelotide Sequences"
        Me.gbxNucSeqSelect.ResumeLayout(False)
        Me.gbxTransOptions.ResumeLayout(False)
        Me.gbxDestinationSelect.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
