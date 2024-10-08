﻿namespace PRISMSeq_Uploader
{
    partial class frmTestingInterface
    {
        // Ignore Spelling: proteinseqs, shewanella

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
            this.txtTransFilePath = new System.Windows.Forms.TextBox();
            this.cmdLoadTT = new System.Windows.Forms.Button();
            this.cmdBrowseTT = new System.Windows.Forms.Button();
            this.gbxTransTableImportTest = new System.Windows.Forms.GroupBox();
            this.gbxFASTAImportTest = new System.Windows.Forms.GroupBox();
            this.cmdBrowseFF = new System.Windows.Forms.Button();
            this.cmdLoadFF = new System.Windows.Forms.Button();
            this.txtFASTAFilePath = new System.Windows.Forms.TextBox();
            this.txtConnString = new System.Windows.Forms.TextBox();
            this.gbxConnectionString = new System.Windows.Forms.GroupBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.cmdBatchLoadDMS = new System.Windows.Forms.Button();
            this.cboCollectionsList = new System.Windows.Forms.ComboBox();
            this.cmdExportFASTA = new System.Windows.Forms.Button();
            this.gbxOtherStuff = new System.Windows.Forms.GroupBox();
            this.cmdUpdateArchiveTables = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pgbAdminConsole = new System.Windows.Forms.ProgressBar();
            this.gbxTransTableImportTest.SuspendLayout();
            this.gbxFASTAImportTest.SuspendLayout();
            this.gbxConnectionString.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.gbxOtherStuff.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTransFilePath
            // 
            this.txtTransFilePath.Location = new System.Drawing.Point(24, 42);
            this.txtTransFilePath.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtTransFilePath.Name = "txtTransFilePath";
            this.txtTransFilePath.Size = new System.Drawing.Size(1176, 31);
            this.txtTransFilePath.TabIndex = 0;
            this.txtTransFilePath.Text = "C:\\Documents and Settings\\d3k857\\My Documents\\Visual Studio Projects\\OrganismData" +
    "abase Handler\\Aux_Files\\gc.ptr";
            // 
            // cmdLoadTT
            // 
            this.cmdLoadTT.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadTT.Location = new System.Drawing.Point(1388, 38);
            this.cmdLoadTT.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdLoadTT.Name = "cmdLoadTT";
            this.cmdLoadTT.Size = new System.Drawing.Size(150, 44);
            this.cmdLoadTT.TabIndex = 1;
            this.cmdLoadTT.Text = "Load";
            this.cmdLoadTT.Click += new System.EventHandler(this.cmdLoadTT_Click);
            // 
            // cmdBrowseTT
            // 
            this.cmdBrowseTT.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdBrowseTT.Location = new System.Drawing.Point(1224, 38);
            this.cmdBrowseTT.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdBrowseTT.Name = "cmdBrowseTT";
            this.cmdBrowseTT.Size = new System.Drawing.Size(150, 44);
            this.cmdBrowseTT.TabIndex = 3;
            this.cmdBrowseTT.Text = "Browse...";
            this.cmdBrowseTT.Click += new System.EventHandler(this.cmdBrowseTT_Click);
            // 
            // gbxTransTableImportTest
            // 
            this.gbxTransTableImportTest.Controls.Add(this.cmdBrowseTT);
            this.gbxTransTableImportTest.Controls.Add(this.txtTransFilePath);
            this.gbxTransTableImportTest.Controls.Add(this.cmdLoadTT);
            this.gbxTransTableImportTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbxTransTableImportTest.Location = new System.Drawing.Point(16, 131);
            this.gbxTransTableImportTest.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxTransTableImportTest.Name = "gbxTransTableImportTest";
            this.gbxTransTableImportTest.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxTransTableImportTest.Size = new System.Drawing.Size(1564, 104);
            this.gbxTransTableImportTest.TabIndex = 5;
            this.gbxTransTableImportTest.TabStop = false;
            this.gbxTransTableImportTest.Text = "Translation Table Import Test";
            // 
            // gbxFASTAImportTest
            // 
            this.gbxFASTAImportTest.Controls.Add(this.cmdBrowseFF);
            this.gbxFASTAImportTest.Controls.Add(this.cmdLoadFF);
            this.gbxFASTAImportTest.Controls.Add(this.txtFASTAFilePath);
            this.gbxFASTAImportTest.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbxFASTAImportTest.Location = new System.Drawing.Point(16, 242);
            this.gbxFASTAImportTest.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxFASTAImportTest.Name = "gbxFASTAImportTest";
            this.gbxFASTAImportTest.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxFASTAImportTest.Size = new System.Drawing.Size(1560, 104);
            this.gbxFASTAImportTest.TabIndex = 6;
            this.gbxFASTAImportTest.TabStop = false;
            this.gbxFASTAImportTest.Text = "FASTA File Import Test";
            // 
            // cmdBrowseFF
            // 
            this.cmdBrowseFF.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdBrowseFF.Location = new System.Drawing.Point(1228, 38);
            this.cmdBrowseFF.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdBrowseFF.Name = "cmdBrowseFF";
            this.cmdBrowseFF.Size = new System.Drawing.Size(150, 44);
            this.cmdBrowseFF.TabIndex = 5;
            this.cmdBrowseFF.Text = "Browse...";
            this.cmdBrowseFF.Click += new System.EventHandler(this.cmdBrowseTT_Click);
            // 
            // cmdLoadFF
            // 
            this.cmdLoadFF.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdLoadFF.Location = new System.Drawing.Point(1392, 38);
            this.cmdLoadFF.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdLoadFF.Name = "cmdLoadFF";
            this.cmdLoadFF.Size = new System.Drawing.Size(150, 44);
            this.cmdLoadFF.TabIndex = 4;
            this.cmdLoadFF.Text = "Load";
            this.cmdLoadFF.Click += new System.EventHandler(this.cmdLoadFF_Click);
            // 
            // txtFASTAFilePath
            // 
            this.txtFASTAFilePath.Location = new System.Drawing.Point(24, 42);
            this.txtFASTAFilePath.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtFASTAFilePath.Name = "txtFASTAFilePath";
            this.txtFASTAFilePath.Size = new System.Drawing.Size(1176, 31);
            this.txtFASTAFilePath.TabIndex = 0;
            this.txtFASTAFilePath.Text = "D:\\Org_DB\\Shewanella\\FASTA\\Shewanella_Heme_proteins_2003-11-19.fasta";
            // 
            // txtConnString
            // 
            this.txtConnString.Location = new System.Drawing.Point(24, 42);
            this.txtConnString.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.txtConnString.Name = "txtConnString";
            this.txtConnString.Size = new System.Drawing.Size(1176, 31);
            this.txtConnString.TabIndex = 7;
            this.txtConnString.Text = "Host=prismdb2.emsl.pnl.gov;Port=5432;Database=dms;UserId=pceditor";
            // 
            // gbxConnectionString
            // 
            this.gbxConnectionString.Controls.Add(this.txtConnString);
            this.gbxConnectionString.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbxConnectionString.Location = new System.Drawing.Point(16, 19);
            this.gbxConnectionString.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxConnectionString.Name = "gbxConnectionString";
            this.gbxConnectionString.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxConnectionString.Size = new System.Drawing.Size(1560, 104);
            this.gbxConnectionString.TabIndex = 8;
            this.gbxConnectionString.TabStop = false;
            this.gbxConnectionString.Text = "Connection String";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.TextBox1);
            this.GroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GroupBox2.Location = new System.Drawing.Point(16, 362);
            this.GroupBox2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.GroupBox2.Size = new System.Drawing.Size(1560, 104);
            this.GroupBox2.TabIndex = 6;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Translation Table Import Test";
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(24, 42);
            this.TextBox1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(1176, 31);
            this.TextBox1.TabIndex = 0;
            this.TextBox1.Text = "1";
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.cmdBatchLoadDMS);
            this.GroupBox3.Controls.Add(this.cboCollectionsList);
            this.GroupBox3.Controls.Add(this.cmdExportFASTA);
            this.GroupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.GroupBox3.Location = new System.Drawing.Point(16, 477);
            this.GroupBox3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.GroupBox3.Size = new System.Drawing.Size(1560, 104);
            this.GroupBox3.TabIndex = 7;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "FASTA File Import Test";
            // 
            // cmdBatchLoadDMS
            // 
            this.cmdBatchLoadDMS.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdBatchLoadDMS.Location = new System.Drawing.Point(1224, 38);
            this.cmdBatchLoadDMS.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdBatchLoadDMS.Name = "cmdBatchLoadDMS";
            this.cmdBatchLoadDMS.Size = new System.Drawing.Size(150, 44);
            this.cmdBatchLoadDMS.TabIndex = 9;
            this.cmdBatchLoadDMS.Text = "Batch Load";
            this.cmdBatchLoadDMS.Click += new System.EventHandler(this.cmdBatchLoadDMS_Click_1);
            // 
            // cboCollectionsList
            // 
            this.cboCollectionsList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCollectionsList.Location = new System.Drawing.Point(24, 42);
            this.cboCollectionsList.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cboCollectionsList.Name = "cboCollectionsList";
            this.cboCollectionsList.Size = new System.Drawing.Size(1176, 33);
            this.cboCollectionsList.TabIndex = 5;
            // 
            // cmdExportFASTA
            // 
            this.cmdExportFASTA.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdExportFASTA.Location = new System.Drawing.Point(1392, 38);
            this.cmdExportFASTA.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdExportFASTA.Name = "cmdExportFASTA";
            this.cmdExportFASTA.Size = new System.Drawing.Size(150, 44);
            this.cmdExportFASTA.TabIndex = 4;
            this.cmdExportFASTA.Text = "Export";
            this.cmdExportFASTA.Click += new System.EventHandler(this.cmdExportFASTA_Click);
            // 
            // gbxOtherStuff
            // 
            this.gbxOtherStuff.Controls.Add(this.cmdUpdateArchiveTables);
            this.gbxOtherStuff.Controls.Add(this.Button3);
            this.gbxOtherStuff.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.gbxOtherStuff.Location = new System.Drawing.Point(16, 592);
            this.gbxOtherStuff.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxOtherStuff.Name = "gbxOtherStuff";
            this.gbxOtherStuff.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.gbxOtherStuff.Size = new System.Drawing.Size(1560, 104);
            this.gbxOtherStuff.TabIndex = 9;
            this.gbxOtherStuff.TabStop = false;
            this.gbxOtherStuff.Text = "Miscellaneous";
            // 
            // cmdUpdateArchiveTables
            // 
            this.cmdUpdateArchiveTables.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdUpdateArchiveTables.Location = new System.Drawing.Point(1132, 35);
            this.cmdUpdateArchiveTables.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cmdUpdateArchiveTables.Name = "cmdUpdateArchiveTables";
            this.cmdUpdateArchiveTables.Size = new System.Drawing.Size(240, 44);
            this.cmdUpdateArchiveTables.TabIndex = 9;
            this.cmdUpdateArchiveTables.Text = "Sync Archive Tables";
            this.cmdUpdateArchiveTables.Visible = false;
#pragma warning disable CS0618 // Type or member is obsolete
            this.cmdUpdateArchiveTables.Click += new System.EventHandler(this.cmdUpdateArchiveTables_Click);
#pragma warning restore CS0618 // Type or member is obsolete
                              // 
                              // Button3
                              // 
            this.Button3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Button3.Location = new System.Drawing.Point(1392, 35);
            this.Button3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(150, 44);
            this.Button3.TabIndex = 4;
            this.Button3.Text = "Export";
            // 
            // lblProgress
            // 
            this.lblProgress.Location = new System.Drawing.Point(16, 708);
            this.lblProgress.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(1560, 31);
            this.lblProgress.TabIndex = 11;
            this.lblProgress.Text = "No Progress Status Yet";
            // 
            // pgbAdminConsole
            // 
            this.pgbAdminConsole.Location = new System.Drawing.Point(16, 735);
            this.pgbAdminConsole.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pgbAdminConsole.Name = "pgbAdminConsole";
            this.pgbAdminConsole.Size = new System.Drawing.Size(1560, 38);
            this.pgbAdminConsole.TabIndex = 10;
            this.pgbAdminConsole.Visible = false;
            // 
            // frmTestingInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1592, 785);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.pgbAdminConsole);
            this.Controls.Add(this.gbxOtherStuff);
            this.Controls.Add(this.gbxConnectionString);
            this.Controls.Add(this.gbxFASTAImportTest);
            this.Controls.Add(this.gbxTransTableImportTest);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox3);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmTestingInterface";
            this.Text = "-";
            this.gbxTransTableImportTest.ResumeLayout(false);
            this.gbxTransTableImportTest.PerformLayout();
            this.gbxFASTAImportTest.ResumeLayout(false);
            this.gbxFASTAImportTest.PerformLayout();
            this.gbxConnectionString.ResumeLayout(false);
            this.gbxConnectionString.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.gbxOtherStuff.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtTransFilePath;
        private System.Windows.Forms.GroupBox gbxTransTableImportTest;
        private System.Windows.Forms.GroupBox gbxFASTAImportTest;
        private System.Windows.Forms.TextBox txtFASTAFilePath;
        private System.Windows.Forms.TextBox txtConnString;
        private System.Windows.Forms.Button cmdLoadTT;
        private System.Windows.Forms.Button cmdBrowseTT;
        private System.Windows.Forms.Button cmdBrowseFF;
        private System.Windows.Forms.Button cmdLoadFF;
        private System.Windows.Forms.GroupBox GroupBox2;
        private System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.GroupBox GroupBox3;
        private System.Windows.Forms.Button cmdExportFASTA;
        private System.Windows.Forms.ComboBox cboCollectionsList;
        private System.Windows.Forms.GroupBox gbxConnectionString;
        private System.Windows.Forms.Button cmdBatchLoadDMS;
        private System.Windows.Forms.GroupBox gbxOtherStuff;
        private System.Windows.Forms.Button cmdUpdateArchiveTables;
        private System.Windows.Forms.Button Button3;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar pgbAdminConsole;
    }
}