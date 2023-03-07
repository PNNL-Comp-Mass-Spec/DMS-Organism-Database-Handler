namespace AppUI_OrfDBHandler
{
    partial class frmValidationReport
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
            this.cboFileListErrors = new System.Windows.Forms.ComboBox();
            this.cmdExportErrorDetails = new System.Windows.Forms.Button();
            this.lblErrorList = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.lvwErrorList = new System.Windows.Forms.ListView();
            this.colNumOccurences = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colErrorDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbxValidFileList = new System.Windows.Forms.GroupBox();
            this.lvwValidList = new System.Windows.Forms.ListView();
            this.colFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOrganism = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colActualCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbxInvalidFileList = new System.Windows.Forms.GroupBox();
            this.pgbListViewLoad = new System.Windows.Forms.ProgressBar();
            this.fraFastaFileWarnings = new System.Windows.Forms.GroupBox();
            this.lvwWarningList = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColumnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmdExportWarningDetails = new System.Windows.Forms.Button();
            this.cboFileListWarnings = new System.Windows.Forms.ComboBox();
            this.lblWarning = new System.Windows.Forms.Label();
            this.gbxValidFileList.SuspendLayout();
            this.gbxInvalidFileList.SuspendLayout();
            this.fraFastaFileWarnings.SuspendLayout();
            this.SuspendLayout();
            //
            // cboFileListErrors
            //
            this.cboFileListErrors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFileListErrors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFileListErrors.Location = new System.Drawing.Point(12, 28);
            this.cboFileListErrors.Name = "cboFileListErrors";
            this.cboFileListErrors.Size = new System.Drawing.Size(448, 21);
            this.cboFileListErrors.TabIndex = 1;
            this.cboFileListErrors.SelectedIndexChanged += new System.EventHandler(this.cboFileListErrors_SelectedIndexChanged);
            //
            // cmdExportErrorDetails
            //
            this.cmdExportErrorDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExportErrorDetails.Location = new System.Drawing.Point(468, 28);
            this.cmdExportErrorDetails.Name = "cmdExportErrorDetails";
            this.cmdExportErrorDetails.Size = new System.Drawing.Size(114, 20);
            this.cmdExportErrorDetails.TabIndex = 3;
            this.cmdExportErrorDetails.Text = "Export Detailed List";
            this.cmdExportErrorDetails.Click += new System.EventHandler(this.cmdExportErrorDetails_Click);
            //
            // lblErrorList
            //
            this.lblErrorList.Location = new System.Drawing.Point(12, 56);
            this.lblErrorList.Name = "lblErrorList";
            this.lblErrorList.Size = new System.Drawing.Size(406, 16);
            this.lblErrorList.TabIndex = 4;
            this.lblErrorList.Text = "Recorded Validation Errors";
            //
            // cmdClose
            //
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdClose.Location = new System.Drawing.Point(520, 626);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(84, 24);
            this.cmdClose.TabIndex = 5;
            this.cmdClose.Text = "Close";
            //
            // lvwErrorList
            //
            this.lvwErrorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwErrorList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colNumOccurences,
            this.colErrorDescription});
            this.lvwErrorList.FullRowSelect = true;
            this.lvwErrorList.GridLines = true;
            this.lvwErrorList.HideSelection = false;
            this.lvwErrorList.Location = new System.Drawing.Point(12, 76);
            this.lvwErrorList.MultiSelect = false;
            this.lvwErrorList.Name = "lvwErrorList";
            this.lvwErrorList.Size = new System.Drawing.Size(572, 127);
            this.lvwErrorList.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lvwErrorList.TabIndex = 6;
            this.lvwErrorList.UseCompatibleStateImageBehavior = false;
            this.lvwErrorList.View = System.Windows.Forms.View.Details;
            //
            // colNumOccurences
            //
            this.colNumOccurences.Text = "Error Count";
            this.colNumOccurences.Width = 80;
            //
            // colErrorDescription
            //
            this.colErrorDescription.Text = "Error Description";
            this.colErrorDescription.Width = 488;
            //
            // gbxValidFileList
            //
            this.gbxValidFileList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxValidFileList.Controls.Add(this.lvwValidList);
            this.gbxValidFileList.Location = new System.Drawing.Point(10, 10);
            this.gbxValidFileList.Name = "gbxValidFileList";
            this.gbxValidFileList.Size = new System.Drawing.Size(596, 146);
            this.gbxValidFileList.TabIndex = 7;
            this.gbxValidFileList.TabStop = false;
            this.gbxValidFileList.Text = "FASTA Files Successfully Uploaded";
            //
            // lvwValidList
            //
            this.lvwValidList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwValidList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFileName,
            this.colOrganism,
            this.colCount,
            this.colActualCount});
            this.lvwValidList.FullRowSelect = true;
            this.lvwValidList.GridLines = true;
            this.lvwValidList.HideSelection = false;
            this.lvwValidList.Location = new System.Drawing.Point(12, 30);
            this.lvwValidList.MultiSelect = false;
            this.lvwValidList.Name = "lvwValidList";
            this.lvwValidList.Size = new System.Drawing.Size(572, 101);
            this.lvwValidList.TabIndex = 7;
            this.lvwValidList.UseCompatibleStateImageBehavior = false;
            this.lvwValidList.View = System.Windows.Forms.View.Details;
            //
            // colFileName
            //
            this.colFileName.Text = "File Name";
            this.colFileName.Width = 313;
            //
            // colOrganism
            //
            this.colOrganism.Text = "Organism";
            this.colOrganism.Width = 100;
            //
            // colCount
            //
            this.colCount.Text = "Protein Count";
            this.colCount.Width = 80;
            //
            // colActualCount
            //
            this.colActualCount.Text = "Actual Count";
            this.colActualCount.Width = 75;
            //
            // gbxInvalidFileList
            //
            this.gbxInvalidFileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxInvalidFileList.Controls.Add(this.lvwErrorList);
            this.gbxInvalidFileList.Controls.Add(this.cmdExportErrorDetails);
            this.gbxInvalidFileList.Controls.Add(this.cboFileListErrors);
            this.gbxInvalidFileList.Controls.Add(this.lblErrorList);
            this.gbxInvalidFileList.Location = new System.Drawing.Point(10, 400);
            this.gbxInvalidFileList.Name = "gbxInvalidFileList";
            this.gbxInvalidFileList.Size = new System.Drawing.Size(596, 216);
            this.gbxInvalidFileList.TabIndex = 8;
            this.gbxInvalidFileList.TabStop = false;
            this.gbxInvalidFileList.Text = "FASTA Files Not Uploaded Due to Errors";
            //
            // pgbListViewLoad
            //
            this.pgbListViewLoad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgbListViewLoad.Location = new System.Drawing.Point(10, 630);
            this.pgbListViewLoad.Name = "pgbListViewLoad";
            this.pgbListViewLoad.Size = new System.Drawing.Size(496, 18);
            this.pgbListViewLoad.TabIndex = 9;
            this.pgbListViewLoad.Visible = false;
            //
            // fraFastaFileWarnings
            //
            this.fraFastaFileWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fraFastaFileWarnings.Controls.Add(this.lvwWarningList);
            this.fraFastaFileWarnings.Controls.Add(this.cmdExportWarningDetails);
            this.fraFastaFileWarnings.Controls.Add(this.cboFileListWarnings);
            this.fraFastaFileWarnings.Controls.Add(this.lblWarning);
            this.fraFastaFileWarnings.Location = new System.Drawing.Point(8, 166);
            this.fraFastaFileWarnings.Name = "fraFastaFileWarnings";
            this.fraFastaFileWarnings.Size = new System.Drawing.Size(596, 224);
            this.fraFastaFileWarnings.TabIndex = 10;
            this.fraFastaFileWarnings.TabStop = false;
            this.fraFastaFileWarnings.Text = "Fasta File Warnings";
            //
            // lvwWarningList
            //
            this.lvwWarningList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwWarningList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1,
            this.ColumnHeader2});
            this.lvwWarningList.FullRowSelect = true;
            this.lvwWarningList.GridLines = true;
            this.lvwWarningList.HideSelection = false;
            this.lvwWarningList.Location = new System.Drawing.Point(12, 77);
            this.lvwWarningList.MultiSelect = false;
            this.lvwWarningList.Name = "lvwWarningList";
            this.lvwWarningList.Size = new System.Drawing.Size(572, 136);
            this.lvwWarningList.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.lvwWarningList.TabIndex = 6;
            this.lvwWarningList.UseCompatibleStateImageBehavior = false;
            this.lvwWarningList.View = System.Windows.Forms.View.Details;
            //
            // ColumnHeader1
            //
            this.ColumnHeader1.Text = "Warning Count";
            this.ColumnHeader1.Width = 80;
            //
            // ColumnHeader2
            //
            this.ColumnHeader2.Text = "Warning Description";
            this.ColumnHeader2.Width = 488;
            //
            // cmdExportWarningDetails
            //
            this.cmdExportWarningDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExportWarningDetails.Location = new System.Drawing.Point(468, 29);
            this.cmdExportWarningDetails.Name = "cmdExportWarningDetails";
            this.cmdExportWarningDetails.Size = new System.Drawing.Size(114, 20);
            this.cmdExportWarningDetails.TabIndex = 3;
            this.cmdExportWarningDetails.Text = "Export Detailed List";
            this.cmdExportWarningDetails.Click += new System.EventHandler(this.cmdExportWarningDetails_Click);
            //
            // cboFileListWarnings
            //
            this.cboFileListWarnings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboFileListWarnings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFileListWarnings.Location = new System.Drawing.Point(12, 29);
            this.cboFileListWarnings.Name = "cboFileListWarnings";
            this.cboFileListWarnings.Size = new System.Drawing.Size(448, 21);
            this.cboFileListWarnings.TabIndex = 1;
            this.cboFileListWarnings.SelectedIndexChanged += new System.EventHandler(this.cboFileListWarnings_SelectedIndexChanged);
            //
            // lblWarning
            //
            this.lblWarning.Location = new System.Drawing.Point(12, 56);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(406, 16);
            this.lblWarning.TabIndex = 4;
            this.lblWarning.Text = "Recorded Validation Warnings";
            //
            // frmValidationReport
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 658);
            this.Controls.Add(this.fraFastaFileWarnings);
            this.Controls.Add(this.pgbListViewLoad);
            this.Controls.Add(this.gbxInvalidFileList);
            this.Controls.Add(this.gbxValidFileList);
            this.Controls.Add(this.cmdClose);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(362, 416);
            this.Name = "frmValidationReport";
            this.Text = "FASTA File Validation Failure Report";
            this.gbxValidFileList.ResumeLayout(false);
            this.gbxInvalidFileList.ResumeLayout(false);
            this.fraFastaFileWarnings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.ColumnHeader colErrorDescription;
        private System.Windows.Forms.ListView lvwErrorList;
        private System.Windows.Forms.Label lblErrorList;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.GroupBox gbxValidFileList;
        private System.Windows.Forms.GroupBox gbxInvalidFileList;
        private System.Windows.Forms.ListView lvwValidList;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colOrganism;
        private System.Windows.Forms.ColumnHeader colCount;
        private System.Windows.Forms.ProgressBar pgbListViewLoad;
        private System.Windows.Forms.ColumnHeader colNumOccurences;
        private System.Windows.Forms.Button cmdExportErrorDetails;
        private System.Windows.Forms.ColumnHeader colActualCount;
        private System.Windows.Forms.GroupBox fraFastaFileWarnings;
        private System.Windows.Forms.ListView lvwWarningList;
        private System.Windows.Forms.Button cmdExportWarningDetails;
        private System.Windows.Forms.ComboBox cboFileListWarnings;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.ColumnHeader ColumnHeader1;
        private System.Windows.Forms.ColumnHeader ColumnHeader2;
        private System.Windows.Forms.ComboBox cboFileListErrors;
    }
}