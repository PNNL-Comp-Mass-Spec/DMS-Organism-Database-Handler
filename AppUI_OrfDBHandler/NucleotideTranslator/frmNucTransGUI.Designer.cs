namespace AppUI_OrfDBHandler.NucleotideTranslator
{
    partial class frmNucTransGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbxNucSeqSelect = new System.Windows.Forms.GroupBox();
            this.lblInputPath = new System.Windows.Forms.Label();
            this.cmdBrowseInput = new System.Windows.Forms.Button();
            this.txtInputPath = new System.Windows.Forms.TextBox();
            this.gbxTransOptions = new System.Windows.Forms.GroupBox();
            this.cboFrameSelect = new System.Windows.Forms.ComboBox();
            this.chkCircular = new System.Windows.Forms.CheckBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.cboTranslationTableSelect = new System.Windows.Forms.ComboBox();
            this.lblTranslationTableSelect = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.lblFrameSelect = new System.Windows.Forms.Label();
            this.lblMinProteinSize = new System.Windows.Forms.Label();
            this.gbxDestinationSelect = new System.Windows.Forms.GroupBox();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.cmdBrowseOutput = new System.Windows.Forms.Button();
            this.txtOutputPath = new System.Windows.Forms.TextBox();
            this.cmdStart = new System.Windows.Forms.Button();
            this.gbxNucSeqSelect.SuspendLayout();
            this.gbxTransOptions.SuspendLayout();
            this.gbxDestinationSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxNucSeqSelect
            // 
            this.gbxNucSeqSelect.Controls.Add(this.lblInputPath);
            this.gbxNucSeqSelect.Controls.Add(this.cmdBrowseInput);
            this.gbxNucSeqSelect.Controls.Add(this.txtInputPath);
            this.gbxNucSeqSelect.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbxNucSeqSelect.Location = new System.Drawing.Point(8, 6);
            this.gbxNucSeqSelect.Name = "gbxNucSeqSelect";
            this.gbxNucSeqSelect.Size = new System.Drawing.Size(406, 68);
            this.gbxNucSeqSelect.TabIndex = 0;
            this.gbxNucSeqSelect.TabStop = false;
            this.gbxNucSeqSelect.Text = "Select Nucleotide Sequence File";
            // 
            // lblInputPath
            // 
            this.lblInputPath.Location = new System.Drawing.Point(8, 16);
            this.lblInputPath.Name = "lblInputPath";
            this.lblInputPath.Size = new System.Drawing.Size(100, 14);
            this.lblInputPath.TabIndex = 2;
            this.lblInputPath.Text = "Input Path";
            // 
            // cmdBrowseInput
            // 
            this.cmdBrowseInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseInput.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdBrowseInput.Location = new System.Drawing.Point(334, 32);
            this.cmdBrowseInput.Name = "cmdBrowseInput";
            this.cmdBrowseInput.Size = new System.Drawing.Size(62, 21);
            this.cmdBrowseInput.TabIndex = 1;
            this.cmdBrowseInput.Text = "Browse...";
            // 
            // txtInputPath
            // 
            this.txtInputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInputPath.Location = new System.Drawing.Point(10, 32);
            this.txtInputPath.Name = "txtInputPath";
            this.txtInputPath.Size = new System.Drawing.Size(316, 21);
            this.txtInputPath.TabIndex = 0;
            // 
            // gbxTransOptions
            // 
            this.gbxTransOptions.Controls.Add(this.cboFrameSelect);
            this.gbxTransOptions.Controls.Add(this.chkCircular);
            this.gbxTransOptions.Controls.Add(this.TextBox1);
            this.gbxTransOptions.Controls.Add(this.cboTranslationTableSelect);
            this.gbxTransOptions.Controls.Add(this.lblTranslationTableSelect);
            this.gbxTransOptions.Controls.Add(this.Label1);
            this.gbxTransOptions.Controls.Add(this.TextBox2);
            this.gbxTransOptions.Controls.Add(this.lblFrameSelect);
            this.gbxTransOptions.Controls.Add(this.lblMinProteinSize);
            this.gbxTransOptions.Location = new System.Drawing.Point(8, 80);
            this.gbxTransOptions.Name = "gbxTransOptions";
            this.gbxTransOptions.Size = new System.Drawing.Size(406, 114);
            this.gbxTransOptions.TabIndex = 1;
            this.gbxTransOptions.TabStop = false;
            this.gbxTransOptions.Text = "Translation Options";
            // 
            // cboFrameSelect
            // 
            this.cboFrameSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFrameSelect.Items.AddRange(new object[] {
            "All",
            "3 Forward",
            "3 Reverse",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6"});
            this.cboFrameSelect.Location = new System.Drawing.Point(226, 78);
            this.cboFrameSelect.Name = "cboFrameSelect";
            this.cboFrameSelect.Size = new System.Drawing.Size(94, 21);
            this.cboFrameSelect.TabIndex = 4;
            // 
            // chkCircular
            // 
            this.chkCircular.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.chkCircular.Location = new System.Drawing.Point(330, 78);
            this.chkCircular.Name = "chkCircular";
            this.chkCircular.Size = new System.Drawing.Size(68, 24);
            this.chkCircular.TabIndex = 3;
            this.chkCircular.Text = "Circular?";
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(10, 78);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(96, 21);
            this.TextBox1.TabIndex = 2;
            this.TextBox1.Text = "30";
            this.TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cboTranslationTableSelect
            // 
            this.cboTranslationTableSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTranslationTableSelect.Location = new System.Drawing.Point(10, 32);
            this.cboTranslationTableSelect.Name = "cboTranslationTableSelect";
            this.cboTranslationTableSelect.Size = new System.Drawing.Size(386, 21);
            this.cboTranslationTableSelect.TabIndex = 0;
            // 
            // lblTranslationTableSelect
            // 
            this.lblTranslationTableSelect.Location = new System.Drawing.Point(8, 16);
            this.lblTranslationTableSelect.Name = "lblTranslationTableSelect";
            this.lblTranslationTableSelect.Size = new System.Drawing.Size(132, 14);
            this.lblTranslationTableSelect.TabIndex = 1;
            this.lblTranslationTableSelect.Text = "Translation Table to Use";
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(116, 62);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(102, 14);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Max Protein Length";
            // 
            // TextBox2
            // 
            this.TextBox2.Location = new System.Drawing.Point(118, 78);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(96, 21);
            this.TextBox2.TabIndex = 2;
            this.TextBox2.Text = "1000000";
            this.TextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblFrameSelect
            // 
            this.lblFrameSelect.Location = new System.Drawing.Point(224, 62);
            this.lblFrameSelect.Name = "lblFrameSelect";
            this.lblFrameSelect.Size = new System.Drawing.Size(90, 12);
            this.lblFrameSelect.TabIndex = 1;
            this.lblFrameSelect.Text = "Frame";
            // 
            // lblMinProteinSize
            // 
            this.lblMinProteinSize.Location = new System.Drawing.Point(8, 62);
            this.lblMinProteinSize.Name = "lblMinProteinSize";
            this.lblMinProteinSize.Size = new System.Drawing.Size(98, 14);
            this.lblMinProteinSize.TabIndex = 1;
            this.lblMinProteinSize.Text = "Min Protein Length";
            // 
            // gbxDestinationSelect
            // 
            this.gbxDestinationSelect.Controls.Add(this.lblOutputPath);
            this.gbxDestinationSelect.Controls.Add(this.cmdBrowseOutput);
            this.gbxDestinationSelect.Controls.Add(this.txtOutputPath);
            this.gbxDestinationSelect.Location = new System.Drawing.Point(8, 200);
            this.gbxDestinationSelect.Name = "gbxDestinationSelect";
            this.gbxDestinationSelect.Size = new System.Drawing.Size(406, 68);
            this.gbxDestinationSelect.TabIndex = 2;
            this.gbxDestinationSelect.TabStop = false;
            this.gbxDestinationSelect.Text = "Select Destination";
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.Location = new System.Drawing.Point(10, 16);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(100, 14);
            this.lblOutputPath.TabIndex = 5;
            this.lblOutputPath.Text = "Output Path";
            // 
            // cmdBrowseOutput
            // 
            this.cmdBrowseOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseOutput.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdBrowseOutput.Location = new System.Drawing.Point(334, 32);
            this.cmdBrowseOutput.Name = "cmdBrowseOutput";
            this.cmdBrowseOutput.Size = new System.Drawing.Size(62, 21);
            this.cmdBrowseOutput.TabIndex = 4;
            this.cmdBrowseOutput.Text = "Browse...";
            // 
            // txtOutputPath
            // 
            this.txtOutputPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutputPath.Location = new System.Drawing.Point(10, 32);
            this.txtOutputPath.Name = "txtOutputPath";
            this.txtOutputPath.Size = new System.Drawing.Size(316, 21);
            this.txtOutputPath.TabIndex = 3;
            // 
            // cmdStart
            // 
            this.cmdStart.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdStart.Location = new System.Drawing.Point(338, 278);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(75, 22);
            this.cmdStart.TabIndex = 3;
            this.cmdStart.Text = "Translate...";
            // 
            // frmNucTransGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 308);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.gbxDestinationSelect);
            this.Controls.Add(this.gbxTransOptions);
            this.Controls.Add(this.gbxNucSeqSelect);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmNucTransGUI";
            this.Text = "Translate Nucelotide Sequences";
            this.gbxNucSeqSelect.ResumeLayout(false);
            this.gbxNucSeqSelect.PerformLayout();
            this.gbxTransOptions.ResumeLayout(false);
            this.gbxTransOptions.PerformLayout();
            this.gbxDestinationSelect.ResumeLayout(false);
            this.gbxDestinationSelect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxNucSeqSelect;
        private System.Windows.Forms.TextBox txtInputPath;
        private System.Windows.Forms.Button cmdBrowseInput;
        private System.Windows.Forms.Label lblInputPath;
        private System.Windows.Forms.GroupBox gbxTransOptions;
        private System.Windows.Forms.GroupBox gbxDestinationSelect;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.Button cmdBrowseOutput;
        private System.Windows.Forms.TextBox txtOutputPath;
        private System.Windows.Forms.ComboBox cboTranslationTableSelect;
        private System.Windows.Forms.Label lblTranslationTableSelect;
        private System.Windows.Forms.Label lblMinProteinSize;
        private System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.TextBox TextBox2;
        private System.Windows.Forms.CheckBox chkCircular;
        private System.Windows.Forms.ComboBox cboFrameSelect;
        private System.Windows.Forms.Label lblFrameSelect;
        private System.Windows.Forms.Button cmdStart;
    }
}