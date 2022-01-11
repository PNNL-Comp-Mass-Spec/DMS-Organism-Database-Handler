using AppUI_OrfDBHandler.Controls;

namespace AppUI_OrfDBHandler
{
    partial class frmBatchAddNewCollection
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
            if (disposing)
            {
                mStatusResetTimer?.Stop();
                components?.Dispose();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBatchAddNewCollection));
            this.ctlTreeViewFolderBrowser = new Raccoom.Windows.Forms.TreeViewFolderBrowser();
            this.lblBatchUploadTree = new System.Windows.Forms.Label();
            this.lblOrganismSelect = new System.Windows.Forms.Label();
            this.lblFolderContents = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdUploadChecked = new System.Windows.Forms.Button();
            this.lvwFolderContents = new System.Windows.Forms.ListView();
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileModDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFileSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCollectionExists = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cboAnnotationTypePicker = new System.Windows.Forms.ComboBox();
            this.lblAnnAuth = new System.Windows.Forms.Label();
            this.lvwSelectedFiles = new System.Windows.Forms.ListView();
            this.colUpFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSelOrganism = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAnnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSelectedFiles = new System.Windows.Forms.Label();
            this.cmdPreviewFile = new System.Windows.Forms.Button();
            this.fraValidationOptions = new System.Windows.Forms.GroupBox();
            this.chkValidationAllowAllSymbolsInProteinNames = new System.Windows.Forms.CheckBox();
            this.txtMaximumProteinNameLength = new System.Windows.Forms.TextBox();
            this.lblMaximumProteinNameLength = new System.Windows.Forms.Label();
            this.chkValidationAllowAsterisks = new System.Windows.Forms.CheckBox();
            this.chkValidationAllowDash = new System.Windows.Forms.CheckBox();
            this.cmdRefreshFiles = new System.Windows.Forms.Button();
            this.cmdUpdateDescription = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cboOrganismSelect = new System.Windows.Forms.ComboBox();
            this.cmdRemoveFile = new AppUI_OrfDBHandler.Controls.ImageButton();
            this.cmdAddFile = new AppUI_OrfDBHandler.Controls.ImageButton();
            this.cmdUpdateOrganisms = new System.Windows.Forms.Button();
            this.fraValidationOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctlTreeViewFolderBrowser
            // 
            this.ctlTreeViewFolderBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ctlTreeViewFolderBrowser.DataSource = null;
            this.ctlTreeViewFolderBrowser.HideSelection = false;
            this.ctlTreeViewFolderBrowser.Location = new System.Drawing.Point(7, 29);
            this.ctlTreeViewFolderBrowser.Name = "_treeViewFolderBrowser1";
            this.ctlTreeViewFolderBrowser.ShowLines = false;
            this.ctlTreeViewFolderBrowser.ShowRootLines = false;
            this.ctlTreeViewFolderBrowser.Size = new System.Drawing.Size(233, 505);
            this.ctlTreeViewFolderBrowser.TabIndex = 0;
            this.ctlTreeViewFolderBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ctlTreeViewFolderBrowser_AfterSelect);
            this.ctlTreeViewFolderBrowser.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ctlTreeViewFolderBrowser_KeyUp);
            this.ctlTreeViewFolderBrowser.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ctlTreeViewFolderBrowser_MouseUp);
            // 
            // lblBatchUploadTree
            // 
            this.lblBatchUploadTree.Location = new System.Drawing.Point(10, 10);
            this.lblBatchUploadTree.Name = "lblBatchUploadTree";
            this.lblBatchUploadTree.Size = new System.Drawing.Size(229, 16);
            this.lblBatchUploadTree.TabIndex = 0;
            this.lblBatchUploadTree.Text = "Select source folder for upload (F5 to refresh)";
            // 
            // lblOrganismSelect
            // 
            this.lblOrganismSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblOrganismSelect.Location = new System.Drawing.Point(6, 545);
            this.lblOrganismSelect.Name = "lblOrganismSelect";
            this.lblOrganismSelect.Size = new System.Drawing.Size(186, 20);
            this.lblOrganismSelect.TabIndex = 10;
            this.lblOrganismSelect.Text = "Select destination &organism";
            // 
            // lblFolderContents
            // 
            this.lblFolderContents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFolderContents.Location = new System.Drawing.Point(244, 10);
            this.lblFolderContents.Name = "lblFolderContents";
            this.lblFolderContents.Size = new System.Drawing.Size(940, 16);
            this.lblFolderContents.TabIndex = 2;
            this.lblFolderContents.Text = "Selected folder contents";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(1129, 621);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(60, 29);
            this.cmdCancel.TabIndex = 21;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdUploadChecked
            // 
            this.cmdUploadChecked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUploadChecked.Location = new System.Drawing.Point(999, 621);
            this.cmdUploadChecked.Name = "cmdUploadChecked";
            this.cmdUploadChecked.Size = new System.Drawing.Size(120, 29);
            this.cmdUploadChecked.TabIndex = 20;
            this.cmdUploadChecked.Text = "&Upload new FASTAs";
            this.cmdUploadChecked.Click += new System.EventHandler(this.cmdUploadChecked_Click);
            // 
            // lvwFolderContents
            // 
            this.lvwFolderContents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwFolderContents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFileName,
            this.colFileModDate,
            this.colFileSize,
            this.colCollectionExists});
            this.lvwFolderContents.FullRowSelect = true;
            this.lvwFolderContents.GridLines = true;
            this.lvwFolderContents.HideSelection = false;
            this.lvwFolderContents.Location = new System.Drawing.Point(244, 26);
            this.lvwFolderContents.Name = "lvwFolderContents";
            this.lvwFolderContents.Size = new System.Drawing.Size(945, 248);
            this.lvwFolderContents.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwFolderContents.TabIndex = 3;
            this.lvwFolderContents.UseCompatibleStateImageBehavior = false;
            this.lvwFolderContents.View = System.Windows.Forms.View.Details;
            this.lvwFolderContents.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwFolderContents_ColumnClick);
            this.lvwFolderContents.Click += new System.EventHandler(this.lvwFolderContents_Click);
            this.lvwFolderContents.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvwFolderContents_KeyDown);
            this.lvwFolderContents.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lvwFolderContents_KeyUp);
            this.lvwFolderContents.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvwFolderContents_MouseUp);
            // 
            // colFileName
            // 
            this.colFileName.Text = "Name";
            this.colFileName.Width = 450;
            // 
            // colFileModDate
            // 
            this.colFileModDate.Text = "Date Modified";
            this.colFileModDate.Width = 140;
            // 
            // colFileSize
            // 
            this.colFileSize.Text = "Size";
            this.colFileSize.Width = 67;
            // 
            // colCollectionExists
            // 
            this.colCollectionExists.Text = "Existing Collection?";
            this.colCollectionExists.Width = 150;
            // 
            // cboAnnotationTypePicker
            // 
            this.cboAnnotationTypePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAnnotationTypePicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnnotationTypePicker.Location = new System.Drawing.Point(801, 566);
            this.cboAnnotationTypePicker.Name = "cboAnnotationTypePicker";
            this.cboAnnotationTypePicker.Size = new System.Drawing.Size(276, 25);
            this.cboAnnotationTypePicker.TabIndex = 13;
            this.cboAnnotationTypePicker.SelectedIndexChanged += new System.EventHandler(this.cboAnnotationTypePicker_SelectedIndexChanged);
            // 
            // lblAnnAuth
            // 
            this.lblAnnAuth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnnAuth.Location = new System.Drawing.Point(801, 545);
            this.lblAnnAuth.Name = "lblAnnAuth";
            this.lblAnnAuth.Size = new System.Drawing.Size(203, 20);
            this.lblAnnAuth.TabIndex = 12;
            this.lblAnnAuth.Text = "Select Annotation &Type";
            // 
            // lvwSelectedFiles
            // 
            this.lvwSelectedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwSelectedFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colUpFileName,
            this.colSelOrganism,
            this.colDescription,
            this.colSource,
            this.colAnnType});
            this.lvwSelectedFiles.FullRowSelect = true;
            this.lvwSelectedFiles.GridLines = true;
            this.lvwSelectedFiles.HideSelection = false;
            this.lvwSelectedFiles.Location = new System.Drawing.Point(244, 345);
            this.lvwSelectedFiles.Name = "lvwSelectedFiles";
            this.lvwSelectedFiles.Size = new System.Drawing.Size(945, 189);
            this.lvwSelectedFiles.TabIndex = 9;
            this.lvwSelectedFiles.UseCompatibleStateImageBehavior = false;
            this.lvwSelectedFiles.View = System.Windows.Forms.View.Details;
            this.lvwSelectedFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwSelectedFiles_ColumnClick);
            this.lvwSelectedFiles.Click += new System.EventHandler(this.lvwSelectedFiles_Click);
            this.lvwSelectedFiles.DoubleClick += new System.EventHandler(this.lvwSelectedFiles_DoubleClick);
            this.lvwSelectedFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvwSelectedFiles_KeyDown);
            // 
            // colUpFileName
            // 
            this.colUpFileName.Text = "Name";
            this.colUpFileName.Width = 251;
            // 
            // colSelOrganism
            // 
            this.colSelOrganism.Text = "Selected Organism";
            this.colSelOrganism.Width = 141;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 150;
            // 
            // colSource
            // 
            this.colSource.Text = "Source (Person, URL, FTP site)";
            this.colSource.Width = 150;
            // 
            // colAnnType
            // 
            this.colAnnType.Text = "Annotation Type";
            this.colAnnType.Width = 105;
            // 
            // lblSelectedFiles
            // 
            this.lblSelectedFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSelectedFiles.Location = new System.Drawing.Point(244, 319);
            this.lblSelectedFiles.Name = "lblSelectedFiles";
            this.lblSelectedFiles.Size = new System.Drawing.Size(620, 16);
            this.lblSelectedFiles.TabIndex = 8;
            this.lblSelectedFiles.Text = "FASTA files selected for upload";
            // 
            // cmdPreviewFile
            // 
            this.cmdPreviewFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdPreviewFile.Enabled = false;
            this.cmdPreviewFile.Location = new System.Drawing.Point(1059, 314);
            this.cmdPreviewFile.Name = "cmdPreviewFile";
            this.cmdPreviewFile.Size = new System.Drawing.Size(130, 24);
            this.cmdPreviewFile.TabIndex = 5;
            this.cmdPreviewFile.Text = "&Preview Selected File";
            this.cmdPreviewFile.Click += new System.EventHandler(this.cmdPreviewFile_Click);
            // 
            // fraValidationOptions
            // 
            this.fraValidationOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fraValidationOptions.Controls.Add(this.chkValidationAllowAllSymbolsInProteinNames);
            this.fraValidationOptions.Controls.Add(this.txtMaximumProteinNameLength);
            this.fraValidationOptions.Controls.Add(this.lblMaximumProteinNameLength);
            this.fraValidationOptions.Controls.Add(this.chkValidationAllowAsterisks);
            this.fraValidationOptions.Controls.Add(this.chkValidationAllowDash);
            this.fraValidationOptions.Location = new System.Drawing.Point(4, 595);
            this.fraValidationOptions.Name = "fraValidationOptions";
            this.fraValidationOptions.Size = new System.Drawing.Size(581, 62);
            this.fraValidationOptions.TabIndex = 15;
            this.fraValidationOptions.TabStop = false;
            this.fraValidationOptions.Text = "Fasta Validation Options";
            // 
            // chkValidationAllowAllSymbolsInProteinNames
            // 
            this.chkValidationAllowAllSymbolsInProteinNames.Location = new System.Drawing.Point(8, 39);
            this.chkValidationAllowAllSymbolsInProteinNames.Name = "chkValidationAllowAllSymbolsInProteinNames";
            this.chkValidationAllowAllSymbolsInProteinNames.Size = new System.Drawing.Size(250, 20);
            this.chkValidationAllowAllSymbolsInProteinNames.TabIndex = 1;
            this.chkValidationAllowAllSymbolsInProteinNames.Text = "Allow all symbols in protein names";
            // 
            // txtMaximumProteinNameLength
            // 
            this.txtMaximumProteinNameLength.Location = new System.Drawing.Point(495, 17);
            this.txtMaximumProteinNameLength.Name = "txtMaximumProteinNameLength";
            this.txtMaximumProteinNameLength.Size = new System.Drawing.Size(60, 24);
            this.txtMaximumProteinNameLength.TabIndex = 4;
            this.txtMaximumProteinNameLength.Text = "80";
            this.txtMaximumProteinNameLength.Validating += new System.ComponentModel.CancelEventHandler(this.txtMaximumProteinNameLength_Validating);
            // 
            // lblMaximumProteinNameLength
            // 
            this.lblMaximumProteinNameLength.Location = new System.Drawing.Point(397, 13);
            this.lblMaximumProteinNameLength.Name = "lblMaximumProteinNameLength";
            this.lblMaximumProteinNameLength.Size = new System.Drawing.Size(103, 42);
            this.lblMaximumProteinNameLength.TabIndex = 3;
            this.lblMaximumProteinNameLength.Text = "Max Protein Name Length";
            // 
            // chkValidationAllowAsterisks
            // 
            this.chkValidationAllowAsterisks.Location = new System.Drawing.Point(8, 16);
            this.chkValidationAllowAsterisks.Name = "chkValidationAllowAsterisks";
            this.chkValidationAllowAsterisks.Size = new System.Drawing.Size(190, 20);
            this.chkValidationAllowAsterisks.TabIndex = 0;
            this.chkValidationAllowAsterisks.Text = "Allow asterisks in residues";
            // 
            // chkValidationAllowDash
            // 
            this.chkValidationAllowDash.Location = new System.Drawing.Point(209, 16);
            this.chkValidationAllowDash.Name = "chkValidationAllowDash";
            this.chkValidationAllowDash.Size = new System.Drawing.Size(165, 20);
            this.chkValidationAllowDash.TabIndex = 2;
            this.chkValidationAllowDash.Text = "Allow dash in residues";
            // 
            // cmdRefreshFiles
            // 
            this.cmdRefreshFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRefreshFiles.Location = new System.Drawing.Point(1059, 281);
            this.cmdRefreshFiles.Name = "cmdRefreshFiles";
            this.cmdRefreshFiles.Size = new System.Drawing.Size(130, 25);
            this.cmdRefreshFiles.TabIndex = 4;
            this.cmdRefreshFiles.Text = "&Refresh Files";
            this.cmdRefreshFiles.Click += new System.EventHandler(this.cmdRefreshFiles_Click);
            // 
            // cmdUpdateDescription
            // 
            this.cmdUpdateDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUpdateDescription.Location = new System.Drawing.Point(1098, 545);
            this.cmdUpdateDescription.Name = "cmdUpdateDescription";
            this.cmdUpdateDescription.Size = new System.Drawing.Size(91, 51);
            this.cmdUpdateDescription.TabIndex = 14;
            this.cmdUpdateDescription.Text = "Update &Description";
            this.cmdUpdateDescription.Click += new System.EventHandler(this.cmdUpdateDescription_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.Location = new System.Drawing.Point(244, 281);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(211, 38);
            this.lblStatus.TabIndex = 22;
            this.lblStatus.Text = "Status";
            // 
            // cboOrganismSelect
            // 
            this.cboOrganismSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOrganismSelect.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cboOrganismSelect.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboOrganismSelect.FormattingEnabled = true;
            this.cboOrganismSelect.Location = new System.Drawing.Point(7, 565);
            this.cboOrganismSelect.Name = "cboOrganismSelect";
            this.cboOrganismSelect.Size = new System.Drawing.Size(777, 25);
            this.cboOrganismSelect.TabIndex = 11;
            this.cboOrganismSelect.SelectedIndexChanged += new System.EventHandler(this.cboOrganismSelect_SelectedIndexChanged);
            // 
            // cmdRemoveFile
            // 
            this.cmdRemoveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdRemoveFile.GenerateDisabledImage = true;
            this.cmdRemoveFile.Location = new System.Drawing.Point(879, 293);
            this.cmdRemoveFile.Name = "cmdRemoveFile";
            this.cmdRemoveFile.Size = new System.Drawing.Size(35, 36);
            this.cmdRemoveFile.TabIndex = 7;
            this.cmdRemoveFile.ThemedImage = ((System.Drawing.Bitmap)(resources.GetObject("cmdRemoveFile.ThemedImage")));
            this.cmdRemoveFile.Click += new System.EventHandler(this.cmdRemoveFile_Click);
            // 
            // cmdAddFile
            // 
            this.cmdAddFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdAddFile.GenerateDisabledImage = true;
            this.cmdAddFile.Location = new System.Drawing.Point(829, 293);
            this.cmdAddFile.Name = "cmdAddFile";
            this.cmdAddFile.Size = new System.Drawing.Size(35, 36);
            this.cmdAddFile.TabIndex = 6;
            this.cmdAddFile.ThemedImage = ((System.Drawing.Bitmap)(resources.GetObject("cmdAddFile.ThemedImage")));
            this.cmdAddFile.Click += new System.EventHandler(this.cmdAddFile_Click);
            // 
            // cmdUpdateOrganisms
            // 
            this.cmdUpdateOrganisms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUpdateOrganisms.Location = new System.Drawing.Point(591, 602);
            this.cmdUpdateOrganisms.Name = "cmdUpdateOrganisms";
            this.cmdUpdateOrganisms.Size = new System.Drawing.Size(91, 51);
            this.cmdUpdateOrganisms.TabIndex = 19;
            this.cmdUpdateOrganisms.Text = "Update &Organisms";
            this.cmdUpdateOrganisms.Click += new System.EventHandler(this.cmdUpdateOrganisms_Click);
            // 
            // frmBatchAddNewCollection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(1199, 665);
            this.Controls.Add(this.cmdUpdateOrganisms);
            this.Controls.Add(this.cboOrganismSelect);
            this.Controls.Add(this.cmdRemoveFile);
            this.Controls.Add(this.cmdAddFile);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdUploadChecked);
            this.Controls.Add(this.cmdUpdateDescription);
            this.Controls.Add(this.cmdRefreshFiles);
            this.Controls.Add(this.fraValidationOptions);
            this.Controls.Add(this.cmdPreviewFile);
            this.Controls.Add(this.lvwSelectedFiles);
            this.Controls.Add(this.lvwFolderContents);
            this.Controls.Add(this.lblOrganismSelect);
            this.Controls.Add(this.ctlTreeViewFolderBrowser);
            this.Controls.Add(this.lblBatchUploadTree);
            this.Controls.Add(this.lblFolderContents);
            this.Controls.Add(this.cboAnnotationTypePicker);
            this.Controls.Add(this.lblAnnAuth);
            this.Controls.Add(this.lblSelectedFiles);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(800, 474);
            this.Name = "frmBatchAddNewCollection";
            this.Text = "Batch Upload FASTA Files";
            this.fraValidationOptions.ResumeLayout(false);
            this.fraValidationOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Raccoom.Windows.Forms.TreeViewFolderBrowser ctlTreeViewFolderBrowser;
        private System.Windows.Forms.Label lblBatchUploadTree;
        private System.Windows.Forms.Label lblOrganismSelect;
        private System.Windows.Forms.Label lblFolderContents;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdUploadChecked;
        private System.Windows.Forms.ListView lvwFolderContents;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colFileSize;
        private System.Windows.Forms.ColumnHeader colFileModDate;
        private System.Windows.Forms.ComboBox cboAnnotationTypePicker;
        private System.Windows.Forms.ColumnHeader colCollectionExists;
        private System.Windows.Forms.ColumnHeader colUpFileName;
        private System.Windows.Forms.ColumnHeader colSelOrganism;
        private System.Windows.Forms.Label lblSelectedFiles;
        private System.Windows.Forms.ListView lvwSelectedFiles;
        private ImageButton cmdAddFile;
        private ImageButton cmdRemoveFile;
        private System.Windows.Forms.Label lblAnnAuth;
        private System.Windows.Forms.ColumnHeader colAnnType;
        private System.Windows.Forms.Button cmdPreviewFile;
        private System.Windows.Forms.GroupBox fraValidationOptions;
        private System.Windows.Forms.CheckBox chkValidationAllowAsterisks;
        private System.Windows.Forms.Button cmdRefreshFiles;
        private System.Windows.Forms.TextBox txtMaximumProteinNameLength;
        private System.Windows.Forms.Label lblMaximumProteinNameLength;
        private System.Windows.Forms.CheckBox chkValidationAllowAllSymbolsInProteinNames;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ColumnHeader colSource;
        private System.Windows.Forms.Button cmdUpdateDescription;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox chkValidationAllowDash;
        private System.Windows.Forms.ComboBox cboOrganismSelect;
        private System.Windows.Forms.Button cmdUpdateOrganisms;
    }
}