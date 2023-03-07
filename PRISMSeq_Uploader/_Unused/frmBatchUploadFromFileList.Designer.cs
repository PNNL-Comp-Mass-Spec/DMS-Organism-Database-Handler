namespace PRISMSeq_Uploader.ProteinUpload
{
    partial class frmBatchUploadFromFileList
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
            this.cmdUploadFiles = new System.Windows.Forms.Button();
            this.lblOrgansimPicker = new System.Windows.Forms.Label();
            this.cboOrganismPicker = new System.Windows.Forms.ComboBox();
            this.lvwFiles = new System.Windows.Forms.ListView();
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFilePath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOrganism = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAnnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboAnnotationType = new System.Windows.Forms.ComboBox();
            this.lblAnnotationType = new System.Windows.Forms.Label();
            this.cmdCheckAll = new System.Windows.Forms.Button();
            this.cmdUncheckAll = new System.Windows.Forms.Button();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // cmdUploadFiles
            //
            this.cmdUploadFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdUploadFiles.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdUploadFiles.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdUploadFiles.Location = new System.Drawing.Point(654, 638);
            this.cmdUploadFiles.Name = "cmdUploadFiles";
            this.cmdUploadFiles.Size = new System.Drawing.Size(158, 22);
            this.cmdUploadFiles.TabIndex = 9;
            this.cmdUploadFiles.Text = "Upload Checked Files";
            this.cmdUploadFiles.UseVisualStyleBackColor = true;
            this.cmdUploadFiles.Click += new System.EventHandler(this.cmdUploadFiles_Click);
            //
            // lblOrgansimPicker
            //
            this.lblOrgansimPicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOrgansimPicker.Location = new System.Drawing.Point(10, 588);
            this.lblOrgansimPicker.Name = "lblOrgansimPicker";
            this.lblOrgansimPicker.Size = new System.Drawing.Size(228, 18);
            this.lblOrgansimPicker.TabIndex = 16;
            this.lblOrgansimPicker.Text = "Organism";
            //
            // cboOrganismPicker
            //
            this.cboOrganismPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOrganismPicker.Location = new System.Drawing.Point(10, 604);
            this.cboOrganismPicker.Name = "cboOrganismPicker";
            this.cboOrganismPicker.Size = new System.Drawing.Size(510, 21);
            this.cboOrganismPicker.TabIndex = 17;
            this.cboOrganismPicker.SelectedIndexChanged += new System.EventHandler(this.cboOrganismPicker_SelectedIndexChanged);
            //
            // lvwFiles
            //
            this.lvwFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwFiles.CheckBoxes = true;
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFileName,
            this.colFilePath,
            this.colOrganism,
            this.colAnnType});
            this.lvwFiles.FullRowSelect = true;
            this.lvwFiles.GridLines = true;
            this.lvwFiles.HideSelection = false;
            this.lvwFiles.Location = new System.Drawing.Point(1, 2);
            this.lvwFiles.Name = "lvwFiles";
            this.lvwFiles.Size = new System.Drawing.Size(820, 576);
            this.lvwFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwFiles.TabIndex = 19;
            this.lvwFiles.UseCompatibleStateImageBehavior = false;
            this.lvwFiles.View = System.Windows.Forms.View.Details;
            this.lvwFiles.SelectedIndexChanged += new System.EventHandler(this.lvwFiles_SelectedIndexChanged);
            //
            // colFileName
            //
            this.colFileName.Text = "FileName";
            this.colFileName.Width = 215;
            //
            // colFilePath
            //
            this.colFilePath.Text = "Directory Path of File";
            this.colFilePath.Width = 247;
            //
            // colOrganism
            //
            this.colOrganism.Text = "Organism";
            this.colOrganism.Width = 125;
            //
            // colAnnType
            //
            this.colAnnType.Text = "Annotation Type";
            this.colAnnType.Width = 117;
            //
            // cboAnnotationType
            //
            this.cboAnnotationType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAnnotationType.Location = new System.Drawing.Point(534, 604);
            this.cboAnnotationType.Name = "cboAnnotationType";
            this.cboAnnotationType.Size = new System.Drawing.Size(280, 21);
            this.cboAnnotationType.TabIndex = 21;
            this.cboAnnotationType.SelectedIndexChanged += new System.EventHandler(this.cboAnnotationType_SelectedIndexChanged);
            //
            // lblAnnotationType
            //
            this.lblAnnotationType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnnotationType.Location = new System.Drawing.Point(536, 588);
            this.lblAnnotationType.Name = "lblAnnotationType";
            this.lblAnnotationType.Size = new System.Drawing.Size(210, 17);
            this.lblAnnotationType.TabIndex = 20;
            this.lblAnnotationType.Text = "Annotation Type";
            //
            // cmdCheckAll
            //
            this.cmdCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCheckAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdCheckAll.Location = new System.Drawing.Point(10, 638);
            this.cmdCheckAll.Name = "cmdCheckAll";
            this.cmdCheckAll.Size = new System.Drawing.Size(100, 22);
            this.cmdCheckAll.TabIndex = 22;
            this.cmdCheckAll.Text = "Check All";
            this.cmdCheckAll.UseVisualStyleBackColor = true;
            this.cmdCheckAll.Click += new System.EventHandler(this.cmdCheckAll_Click);
            //
            // cmdUncheckAll
            //
            this.cmdUncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdUncheckAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdUncheckAll.Location = new System.Drawing.Point(118, 638);
            this.cmdUncheckAll.Name = "cmdUncheckAll";
            this.cmdUncheckAll.Size = new System.Drawing.Size(100, 22);
            this.cmdUncheckAll.TabIndex = 23;
            this.cmdUncheckAll.Text = "Uncheck All";
            this.cmdUncheckAll.UseVisualStyleBackColor = true;
            this.cmdUncheckAll.Click += new System.EventHandler(this.cmdUncheckAll_Click);
            //
            // txtFilePath
            //
            this.txtFilePath.Location = new System.Drawing.Point(319, 641);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(431, 20);
            this.txtFilePath.TabIndex = 24;
            //
            // frmBatchUploadFromFileList1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 673);
            this.Controls.Add(this.cmdUploadFiles);
            this.Controls.Add(this.cmdUncheckAll);
            this.Controls.Add(this.cmdCheckAll);
            this.Controls.Add(this.cboAnnotationType);
            this.Controls.Add(this.cboOrganismPicker);
            this.Controls.Add(this.lblAnnotationType);
            this.Controls.Add(this.lblOrgansimPicker);
            this.Controls.Add(this.lvwFiles);
            this.Controls.Add(this.txtFilePath);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.MinimumSize = new System.Drawing.Size(600, 586);
            this.Name = "frmBatchUploadFromFileList1";
            this.Text = "Batch Upload FASTA Files from FileList";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdUploadFiles;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colFilePath;
        private System.Windows.Forms.ColumnHeader colAnnType;
        private System.Windows.Forms.Button cmdCheckAll;
        private System.Windows.Forms.ColumnHeader colOrganism;
        private System.Windows.Forms.ComboBox cboAnnotationType;
        private System.Windows.Forms.Button cmdUncheckAll;
        private System.Windows.Forms.Label lblAnnotationType;
        private System.Windows.Forms.ListView lvwFiles;
        private System.Windows.Forms.Label lblOrgansimPicker;
        private System.Windows.Forms.ComboBox cboOrganismPicker;
        private System.Windows.Forms.TextBox txtFilePath;
    }
}