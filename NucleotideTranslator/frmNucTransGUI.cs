namespace NucleotideTranslator
{
    public class frmNucTransGUI : System.Windows.Forms.Form
    {
        #region "Windows Form Designer generated code"

        public frmNucTransGUI() : base()
        {

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call

        }

        // Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        internal System.Windows.Forms.GroupBox gbxNucSeqSelect;
        internal System.Windows.Forms.TextBox txtInputPath;
        internal System.Windows.Forms.Button cmdBrowseInput;
        internal System.Windows.Forms.Label lblInputPath;
        internal System.Windows.Forms.GroupBox gbxTransOptions;
        internal System.Windows.Forms.GroupBox gbxDestinationSelect;
        internal System.Windows.Forms.Label lblOutputPath;
        internal System.Windows.Forms.Button cmdBrowseOutput;
        internal System.Windows.Forms.TextBox txtOutputPath;
        internal System.Windows.Forms.ComboBox cboTranslationTableSelect;
        internal System.Windows.Forms.Label lblTranslationTableSelect;
        internal System.Windows.Forms.Label lblMinProteinSize;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox TextBox2;
        internal System.Windows.Forms.CheckBox chkCircular;
        internal System.Windows.Forms.ComboBox cboFrameSelect;
        internal System.Windows.Forms.Label lblFrameSelect;
        internal System.Windows.Forms.Button cmdStart;

        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            gbxNucSeqSelect = new System.Windows.Forms.GroupBox();
            lblInputPath = new System.Windows.Forms.Label();
            cmdBrowseInput = new System.Windows.Forms.Button();
            txtInputPath = new System.Windows.Forms.TextBox();
            gbxTransOptions = new System.Windows.Forms.GroupBox();
            cboFrameSelect = new System.Windows.Forms.ComboBox();
            chkCircular = new System.Windows.Forms.CheckBox();
            TextBox1 = new System.Windows.Forms.TextBox();
            cboTranslationTableSelect = new System.Windows.Forms.ComboBox();
            lblTranslationTableSelect = new System.Windows.Forms.Label();
            Label1 = new System.Windows.Forms.Label();
            TextBox2 = new System.Windows.Forms.TextBox();
            lblFrameSelect = new System.Windows.Forms.Label();
            lblMinProteinSize = new System.Windows.Forms.Label();
            gbxDestinationSelect = new System.Windows.Forms.GroupBox();
            lblOutputPath = new System.Windows.Forms.Label();
            cmdBrowseOutput = new System.Windows.Forms.Button();
            txtOutputPath = new System.Windows.Forms.TextBox();
            cmdStart = new System.Windows.Forms.Button();
            gbxNucSeqSelect.SuspendLayout();
            gbxTransOptions.SuspendLayout();
            gbxDestinationSelect.SuspendLayout();
            SuspendLayout();
            //
            // gbxNucSeqSelect
            //
            gbxNucSeqSelect.Controls.Add(lblInputPath);
            gbxNucSeqSelect.Controls.Add(cmdBrowseInput);
            gbxNucSeqSelect.Controls.Add(txtInputPath);
            gbxNucSeqSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            gbxNucSeqSelect.Location = new System.Drawing.Point(8, 6);
            gbxNucSeqSelect.Name = "gbxNucSeqSelect";
            gbxNucSeqSelect.Size = new System.Drawing.Size(406, 68);
            gbxNucSeqSelect.TabIndex = 0;
            gbxNucSeqSelect.TabStop = false;
            gbxNucSeqSelect.Text = "Select Nucleotide Sequence File";
            //
            // lblInputPath
            //
            lblInputPath.Location = new System.Drawing.Point(8, 16);
            lblInputPath.Name = "lblInputPath";
            lblInputPath.Size = new System.Drawing.Size(100, 14);
            lblInputPath.TabIndex = 2;
            lblInputPath.Text = "Input Path";
            //
            // cmdBrowseInput
            //
            cmdBrowseInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cmdBrowseInput.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdBrowseInput.Location = new System.Drawing.Point(334, 32);
            cmdBrowseInput.Name = "cmdBrowseInput";
            cmdBrowseInput.Size = new System.Drawing.Size(62, 21);
            cmdBrowseInput.TabIndex = 1;
            cmdBrowseInput.Text = "Browse...";
            //
            // txtInputPath
            //
            txtInputPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtInputPath.Location = new System.Drawing.Point(10, 32);
            txtInputPath.Name = "txtInputPath";
            txtInputPath.Size = new System.Drawing.Size(316, 21);
            txtInputPath.TabIndex = 0;
            txtInputPath.Text = "";
            //
            // gbxTransOptions
            //
            gbxTransOptions.Controls.Add(cboFrameSelect);
            gbxTransOptions.Controls.Add(chkCircular);
            gbxTransOptions.Controls.Add(TextBox1);
            gbxTransOptions.Controls.Add(cboTranslationTableSelect);
            gbxTransOptions.Controls.Add(lblTranslationTableSelect);
            gbxTransOptions.Controls.Add(Label1);
            gbxTransOptions.Controls.Add(TextBox2);
            gbxTransOptions.Controls.Add(lblFrameSelect);
            gbxTransOptions.Controls.Add(lblMinProteinSize);
            gbxTransOptions.Location = new System.Drawing.Point(8, 80);
            gbxTransOptions.Name = "gbxTransOptions";
            gbxTransOptions.Size = new System.Drawing.Size(406, 114);
            gbxTransOptions.TabIndex = 1;
            gbxTransOptions.TabStop = false;
            gbxTransOptions.Text = "Translation Options";
            //
            // cboFrameSelect
            //
            cboFrameSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboFrameSelect.Items.AddRange(new object[] { "All", "3 Forward", "3 Reverse", "1", "2", "3", "4", "5", "6" });
            cboFrameSelect.Location = new System.Drawing.Point(226, 78);
            cboFrameSelect.Name = "cboFrameSelect";
            cboFrameSelect.Size = new System.Drawing.Size(94, 21);
            cboFrameSelect.TabIndex = 4;
            //
            // chkCircular
            //
            chkCircular.FlatStyle = System.Windows.Forms.FlatStyle.System;
            chkCircular.Location = new System.Drawing.Point(330, 78);
            chkCircular.Name = "chkCircular";
            chkCircular.Size = new System.Drawing.Size(68, 24);
            chkCircular.TabIndex = 3;
            chkCircular.Text = "Circular?";
            //
            // TextBox1
            //
            TextBox1.Location = new System.Drawing.Point(10, 78);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new System.Drawing.Size(96, 21);
            TextBox1.TabIndex = 2;
            TextBox1.Text = "30";
            TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // cboTranslationTableSelect
            //
            cboTranslationTableSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboTranslationTableSelect.Location = new System.Drawing.Point(10, 32);
            cboTranslationTableSelect.Name = "cboTranslationTableSelect";
            cboTranslationTableSelect.Size = new System.Drawing.Size(386, 21);
            cboTranslationTableSelect.TabIndex = 0;
            //
            // lblTranslationTableSelect
            //
            lblTranslationTableSelect.Location = new System.Drawing.Point(8, 16);
            lblTranslationTableSelect.Name = "lblTranslationTableSelect";
            lblTranslationTableSelect.Size = new System.Drawing.Size(132, 14);
            lblTranslationTableSelect.TabIndex = 1;
            lblTranslationTableSelect.Text = "Translation Table to Use";
            //
            // Label1
            //
            Label1.Location = new System.Drawing.Point(116, 62);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(102, 14);
            Label1.TabIndex = 1;
            Label1.Text = "Max Protein Length";
            //
            // TextBox2
            //
            TextBox2.Location = new System.Drawing.Point(118, 78);
            TextBox2.Name = "TextBox2";
            TextBox2.Size = new System.Drawing.Size(96, 21);
            TextBox2.TabIndex = 2;
            TextBox2.Text = "1000000";
            TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // lblFrameSelect
            //
            lblFrameSelect.Location = new System.Drawing.Point(224, 62);
            lblFrameSelect.Name = "lblFrameSelect";
            lblFrameSelect.Size = new System.Drawing.Size(90, 12);
            lblFrameSelect.TabIndex = 1;
            lblFrameSelect.Text = "Frame";
            //
            // lblMinProteinSize
            //
            lblMinProteinSize.Location = new System.Drawing.Point(8, 62);
            lblMinProteinSize.Name = "lblMinProteinSize";
            lblMinProteinSize.Size = new System.Drawing.Size(98, 14);
            lblMinProteinSize.TabIndex = 1;
            lblMinProteinSize.Text = "Min Protein Length";
            //
            // gbxDestinationSelect
            //
            gbxDestinationSelect.Controls.Add(lblOutputPath);
            gbxDestinationSelect.Controls.Add(cmdBrowseOutput);
            gbxDestinationSelect.Controls.Add(txtOutputPath);
            gbxDestinationSelect.Location = new System.Drawing.Point(8, 200);
            gbxDestinationSelect.Name = "gbxDestinationSelect";
            gbxDestinationSelect.Size = new System.Drawing.Size(406, 68);
            gbxDestinationSelect.TabIndex = 2;
            gbxDestinationSelect.TabStop = false;
            gbxDestinationSelect.Text = "Select Destination";
            //
            // lblOutputPath
            //
            lblOutputPath.Location = new System.Drawing.Point(10, 16);
            lblOutputPath.Name = "lblOutputPath";
            lblOutputPath.Size = new System.Drawing.Size(100, 14);
            lblOutputPath.TabIndex = 5;
            lblOutputPath.Text = "Output Path";
            //
            // cmdBrowseOutput
            //
            cmdBrowseOutput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            cmdBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdBrowseOutput.Location = new System.Drawing.Point(334, 32);
            cmdBrowseOutput.Name = "cmdBrowseOutput";
            cmdBrowseOutput.Size = new System.Drawing.Size(62, 21);
            cmdBrowseOutput.TabIndex = 4;
            cmdBrowseOutput.Text = "Browse...";
            //
            // txtOutputPath
            //
            txtOutputPath.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtOutputPath.Location = new System.Drawing.Point(10, 32);
            txtOutputPath.Name = "txtOutputPath";
            txtOutputPath.Size = new System.Drawing.Size(316, 21);
            txtOutputPath.TabIndex = 3;
            txtOutputPath.Text = "";
            //
            // cmdStart
            //
            cmdStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdStart.Location = new System.Drawing.Point(338, 278);
            cmdStart.Name = "cmdStart";
            cmdStart.Size = new System.Drawing.Size(75, 22);
            cmdStart.TabIndex = 3;
            cmdStart.Text = "Translate...";
            //
            // frmNucTransGUI
            //
            AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            ClientSize = new System.Drawing.Size(422, 308);
            Controls.Add(cmdStart);
            Controls.Add(gbxDestinationSelect);
            Controls.Add(gbxTransOptions);
            Controls.Add(gbxNucSeqSelect);
            Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            Name = "frmNucTransGUI";
            Text = "Translate Nucelotide Sequences";
            gbxNucSeqSelect.ResumeLayout(false);
            gbxTransOptions.ResumeLayout(false);
            gbxDestinationSelect.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
    }
}