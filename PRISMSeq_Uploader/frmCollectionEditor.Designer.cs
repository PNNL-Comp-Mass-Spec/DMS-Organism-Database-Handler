using PRISMSeq_Uploader.Controls;

namespace PRISMSeq_Uploader
{
    partial class frmCollectionEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCollectionEditor));
            this.pnlProgBar = new System.Windows.Forms.Panel();
            this.pnlProgBarUpper = new System.Windows.Forms.Panel();
            this.pnlBatchProgress = new System.Windows.Forms.Panel();
            this.lblBatchProgress = new System.Windows.Forms.Label();
            this.pnlCurrentTask = new System.Windows.Forms.Panel();
            this.lblCurrentTask = new System.Windows.Forms.Label();
            this.pnlProgBarLower = new System.Windows.Forms.Panel();
            this.pgbMain = new System.Windows.Forms.ProgressBar();
            this.mnuMainGUI = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileExit = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.gbxSourceCollection = new System.Windows.Forms.GroupBox();
            this.cmdLoadFile = new System.Windows.Forms.Button();
            this.cboAnnotationTypePicker = new System.Windows.Forms.ComboBox();
            this.lblAnnotationTypeFilter = new System.Windows.Forms.Label();
            this.cmdLoadProteins = new System.Windows.Forms.Button();
            this.cboCollectionPicker = new System.Windows.Forms.ComboBox();
            this.cboOrganismFilter = new System.Windows.Forms.ComboBox();
            this.lblOrganismFilter = new System.Windows.Forms.Label();
            this.lblCollectionPicker = new System.Windows.Forms.Label();
            this.lvwSource = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSourceMembers = new System.Windows.Forms.Label();
            this.lblTargetDatabase = new System.Windows.Forms.Label();
            this.pnlProgBar.SuspendLayout();
            this.pnlProgBarUpper.SuspendLayout();
            this.pnlBatchProgress.SuspendLayout();
            this.pnlCurrentTask.SuspendLayout();
            this.pnlProgBarLower.SuspendLayout();
            this.gbxSourceCollection.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlProgBar
            // 
            this.pnlProgBar.Controls.Add(this.pnlProgBarUpper);
            this.pnlProgBar.Controls.Add(this.pnlProgBarLower);
            this.pnlProgBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProgBar.Location = new System.Drawing.Point(0, 461);
            this.pnlProgBar.Name = "pnlProgBar";
            this.pnlProgBar.Size = new System.Drawing.Size(1134, 55);
            this.pnlProgBar.TabIndex = 0;
            this.pnlProgBar.Visible = false;
            // 
            // pnlProgBarUpper
            // 
            this.pnlProgBarUpper.Controls.Add(this.pnlBatchProgress);
            this.pnlProgBarUpper.Controls.Add(this.pnlCurrentTask);
            this.pnlProgBarUpper.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProgBarUpper.Location = new System.Drawing.Point(0, 0);
            this.pnlProgBarUpper.Name = "pnlProgBarUpper";
            this.pnlProgBarUpper.Padding = new System.Windows.Forms.Padding(6);
            this.pnlProgBarUpper.Size = new System.Drawing.Size(1134, 22);
            this.pnlProgBarUpper.TabIndex = 2;
            // 
            // pnlBatchProgress
            // 
            this.pnlBatchProgress.Controls.Add(this.lblBatchProgress);
            this.pnlBatchProgress.Location = new System.Drawing.Point(585, 2);
            this.pnlBatchProgress.Name = "pnlBatchProgress";
            this.pnlBatchProgress.Size = new System.Drawing.Size(540, 20);
            this.pnlBatchProgress.TabIndex = 2;
            // 
            // lblBatchProgress
            // 
            this.lblBatchProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBatchProgress.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBatchProgress.Location = new System.Drawing.Point(0, 0);
            this.lblBatchProgress.Name = "lblBatchProgress";
            this.lblBatchProgress.Size = new System.Drawing.Size(540, 14);
            this.lblBatchProgress.TabIndex = 20;
            this.lblBatchProgress.Text = "Batch progress";
            // 
            // pnlCurrentTask
            // 
            this.pnlCurrentTask.Controls.Add(this.lblCurrentTask);
            this.pnlCurrentTask.Location = new System.Drawing.Point(3, 2);
            this.pnlCurrentTask.Name = "pnlCurrentTask";
            this.pnlCurrentTask.Size = new System.Drawing.Size(576, 20);
            this.pnlCurrentTask.TabIndex = 1;
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCurrentTask.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTask.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(576, 14);
            this.lblCurrentTask.TabIndex = 21;
            this.lblCurrentTask.Text = "Reading Source File...";
            // 
            // pnlProgBarLower
            // 
            this.pnlProgBarLower.Controls.Add(this.pgbMain);
            this.pnlProgBarLower.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProgBarLower.Location = new System.Drawing.Point(0, 21);
            this.pnlProgBarLower.Name = "pnlProgBarLower";
            this.pnlProgBarLower.Padding = new System.Windows.Forms.Padding(6);
            this.pnlProgBarLower.Size = new System.Drawing.Size(1134, 34);
            this.pnlProgBarLower.TabIndex = 0;
            // 
            // pgbMain
            // 
            this.pgbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgbMain.Location = new System.Drawing.Point(6, 6);
            this.pgbMain.Name = "pgbMain";
            this.pgbMain.Size = new System.Drawing.Size(1122, 22);
            this.pgbMain.TabIndex = 14;
            // 
            // mnuMainGUI
            // 
            this.mnuMainGUI.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuHelp});
            // 
            // mnuFile
            // 
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileExit});
            this.mnuFile.Text = "&File";
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Index = 0;
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 1;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Index = 0;
            this.mnuHelpAbout.Text = "&About Protein Collection Editor";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // gbxSourceCollection
            // 
            this.gbxSourceCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxSourceCollection.Controls.Add(this.cmdLoadFile);
            this.gbxSourceCollection.Controls.Add(this.cboAnnotationTypePicker);
            this.gbxSourceCollection.Controls.Add(this.lblAnnotationTypeFilter);
            this.gbxSourceCollection.Controls.Add(this.cmdLoadProteins);
            this.gbxSourceCollection.Controls.Add(this.cboCollectionPicker);
            this.gbxSourceCollection.Controls.Add(this.cboOrganismFilter);
            this.gbxSourceCollection.Controls.Add(this.lblOrganismFilter);
            this.gbxSourceCollection.Controls.Add(this.lblCollectionPicker);
            this.gbxSourceCollection.Controls.Add(this.lvwSource);
            this.gbxSourceCollection.Controls.Add(this.lblSourceMembers);
            this.gbxSourceCollection.Location = new System.Drawing.Point(12, 12);
            this.gbxSourceCollection.Name = "gbxSourceCollection";
            this.gbxSourceCollection.Size = new System.Drawing.Size(1110, 418);
            this.gbxSourceCollection.TabIndex = 3;
            this.gbxSourceCollection.TabStop = false;
            this.gbxSourceCollection.Text = "Source Collection";
            // 
            // 
            // cmdLoadFile
            // 
            this.cmdLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdLoadFile.Location = new System.Drawing.Point(178, 372);
            this.cmdLoadFile.Name = "cmdLoadFile";
            this.cmdLoadFile.Size = new System.Drawing.Size(298, 40);
            this.cmdLoadFile.TabIndex = 19;
            this.cmdLoadFile.Text = "&Import New Collection...";
            this.cmdLoadFile.Click += new System.EventHandler(this.cmdLoadFile_Click);
            // 
            // cboAnnotationTypePicker
            // 
            this.cboAnnotationTypePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAnnotationTypePicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnnotationTypePicker.Location = new System.Drawing.Point(878, 42);
            this.cboAnnotationTypePicker.Name = "cboAnnotationTypePicker";
            this.cboAnnotationTypePicker.Size = new System.Drawing.Size(216, 25);
            this.cboAnnotationTypePicker.TabIndex = 17;
            // 
            // lblAnnotationTypeFilter
            // 
            this.lblAnnotationTypeFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnnotationTypeFilter.Location = new System.Drawing.Point(878, 24);
            this.lblAnnotationTypeFilter.Name = "lblAnnotationTypeFilter";
            this.lblAnnotationTypeFilter.Size = new System.Drawing.Size(212, 17);
            this.lblAnnotationTypeFilter.TabIndex = 18;
            this.lblAnnotationTypeFilter.Text = "Naming Authority Filter";
            // 
            // cmdLoadProteins
            // 
            this.cmdLoadProteins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoadProteins.Location = new System.Drawing.Point(990, 85);
            this.cmdLoadProteins.Name = "cmdLoadProteins";
            this.cmdLoadProteins.Size = new System.Drawing.Size(102, 24);
            this.cmdLoadProteins.TabIndex = 14;
            this.cmdLoadProteins.Text = "Load Proteins";
            this.cmdLoadProteins.Click += new System.EventHandler(this.cmdLoadProteins_Click);
            // 
            // cboCollectionPicker
            // 
            this.cboCollectionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCollectionPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCollectionPicker.Location = new System.Drawing.Point(14, 85);
            this.cboCollectionPicker.Name = "cboCollectionPicker";
            this.cboCollectionPicker.Size = new System.Drawing.Size(966, 25);
            this.cboCollectionPicker.TabIndex = 1;
            // 
            // cboOrganismFilter
            // 
            this.cboOrganismFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOrganismFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrganismFilter.Location = new System.Drawing.Point(14, 42);
            this.cboOrganismFilter.Name = "cboOrganismFilter";
            this.cboOrganismFilter.Size = new System.Drawing.Size(854, 25);
            this.cboOrganismFilter.TabIndex = 0;
            // 
            // lblOrganismFilter
            // 
            this.lblOrganismFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOrganismFilter.Location = new System.Drawing.Point(14, 24);
            this.lblOrganismFilter.Name = "lblOrganismFilter";
            this.lblOrganismFilter.Size = new System.Drawing.Size(261, 17);
            this.lblOrganismFilter.TabIndex = 3;
            this.lblOrganismFilter.Text = "Organism Selector";
            // 
            // lblCollectionPicker
            // 
            this.lblCollectionPicker.Location = new System.Drawing.Point(14, 68);
            this.lblCollectionPicker.Name = "lblCollectionPicker";
            this.lblCollectionPicker.Size = new System.Drawing.Size(250, 17);
            this.lblCollectionPicker.TabIndex = 4;
            this.lblCollectionPicker.Text = "Protein Collection";
            // 
            // lvwSource
            // 
            this.lvwSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwSource.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSrcName,
            this.colSrcDesc});
            this.lvwSource.FullRowSelect = true;
            this.lvwSource.GridLines = true;
            this.lvwSource.HideSelection = false;
            this.lvwSource.Location = new System.Drawing.Point(14, 130);
            this.lvwSource.Name = "lvwSource";
            this.lvwSource.Size = new System.Drawing.Size(1080, 232);
            this.lvwSource.TabIndex = 2;
            this.lvwSource.UseCompatibleStateImageBehavior = false;
            this.lvwSource.View = System.Windows.Forms.View.Details;
            // 
            // colSrcName
            // 
            this.colSrcName.Text = "Name";
            this.colSrcName.Width = 117;
            // 
            // colSrcDesc
            // 
            this.colSrcDesc.Text = "Description";
            this.colSrcDesc.Width = 320;
            // 
            // lblSourceMembers
            // 
            this.lblSourceMembers.Location = new System.Drawing.Point(14, 111);
            this.lblSourceMembers.Name = "lblSourceMembers";
            this.lblSourceMembers.Size = new System.Drawing.Size(128, 17);
            this.lblSourceMembers.TabIndex = 5;
            this.lblSourceMembers.Text = "Collection Members";
            // 
            // lblTargetDatabase
            // 
            this.lblTargetDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTargetDatabase.Location = new System.Drawing.Point(18, 434);
            this.lblTargetDatabase.Name = "lblTargetDatabase";
            this.lblTargetDatabase.Size = new System.Drawing.Size(700, 20);
            this.lblTargetDatabase.TabIndex = 22;
            this.lblTargetDatabase.Text = "Target database: ";
            // 
            // frmCollectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 516);
            this.Controls.Add(this.lblTargetDatabase);
            this.Controls.Add(this.gbxSourceCollection);
            this.Controls.Add(this.pnlProgBar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mnuMainGUI;
            this.MinimumSize = new System.Drawing.Size(820, 453);
            this.Name = "frmCollectionEditor";
            this.Text = "Protein Collection Editor";
            this.pnlProgBar.ResumeLayout(false);
            this.pnlProgBarUpper.ResumeLayout(false);
            this.pnlBatchProgress.ResumeLayout(false);
            this.pnlCurrentTask.ResumeLayout(false);
            this.pnlProgBarLower.ResumeLayout(false);
            this.gbxSourceCollection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlProgBar;
        private System.Windows.Forms.Panel pnlProgBarLower;
        private System.Windows.Forms.ProgressBar pgbMain;
        private System.Windows.Forms.MainMenu mnuMainGUI;
        private System.Windows.Forms.MenuItem mnuFile;
        private System.Windows.Forms.MenuItem mnuHelp;
        private System.Windows.Forms.MenuItem mnuFileExit;
        private System.Windows.Forms.MenuItem mnuHelpAbout;
        private System.Windows.Forms.GroupBox gbxSourceCollection;
        private System.Windows.Forms.Button cmdLoadFile;
        private System.Windows.Forms.ComboBox cboAnnotationTypePicker;
        private System.Windows.Forms.Label lblAnnotationTypeFilter;
        private System.Windows.Forms.Button cmdLoadProteins;
        private System.Windows.Forms.ComboBox cboCollectionPicker;
        private System.Windows.Forms.ComboBox cboOrganismFilter;
        private System.Windows.Forms.Label lblOrganismFilter;
        private System.Windows.Forms.Label lblCollectionPicker;
        private System.Windows.Forms.ListView lvwSource;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcDesc;
        private System.Windows.Forms.Label lblSourceMembers;
        private System.Windows.Forms.Label lblTargetDatabase;
        private System.Windows.Forms.Label lblCurrentTask;
        private System.Windows.Forms.Panel pnlCurrentTask;
        private System.Windows.Forms.Panel pnlProgBarUpper;
        private System.Windows.Forms.Panel pnlBatchProgress;
        private System.Windows.Forms.Label lblBatchProgress;
    }
}