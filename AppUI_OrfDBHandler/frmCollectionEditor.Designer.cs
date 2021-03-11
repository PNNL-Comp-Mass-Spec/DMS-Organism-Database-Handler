using AppUI_OrfDBHandler.Controls;

namespace AppUI_OrfDBHandler
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
            this.lblBatchProgress = new System.Windows.Forms.Label();
            this.lblCurrentTask = new System.Windows.Forms.Label();
            this.pnlProgBarLower = new System.Windows.Forms.Panel();
            this.pgbMain = new System.Windows.Forms.ProgressBar();
            this.pnlSource = new System.Windows.Forms.Panel();
            this.lblTargetServer = new System.Windows.Forms.Label();
            this.cmdDestAdd = new AppUI_OrfDBHandler.Controls.ImageButton();
            this.cmdDestRemove = new AppUI_OrfDBHandler.Controls.ImageButton();
            this.cmdDestAddAll = new AppUI_OrfDBHandler.Controls.ImageButton();
            this.cmdDestRemoveAll = new AppUI_OrfDBHandler.Controls.ImageButton();
            this.gbxSourceCollection = new System.Windows.Forms.GroupBox();
            this.cboAnnotationTypePicker = new System.Windows.Forms.ComboBox();
            this.lblAnnotationTypeFilter = new System.Windows.Forms.Label();
            this.pbxLiveSearchCancel = new System.Windows.Forms.PictureBox();
            this.lblSearchCount = new System.Windows.Forms.Label();
            this.cmdLoadProteins = new System.Windows.Forms.Button();
            this.cmdLoadFile = new System.Windows.Forms.Button();
            this.txtLiveSearch = new System.Windows.Forms.TextBox();
            this.cboCollectionPicker = new System.Windows.Forms.ComboBox();
            this.cboOrganismFilter = new System.Windows.Forms.ComboBox();
            this.lblOrganismFilter = new System.Windows.Forms.Label();
            this.lblCollectionPicker = new System.Windows.Forms.Label();
            this.pbxLiveSearchBkg = new System.Windows.Forms.PictureBox();
            this.lvwSource = new System.Windows.Forms.ListView();
            this.colSrcName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSrcDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblSourceMembers = new System.Windows.Forms.Label();
            this.SourceDestSplit = new System.Windows.Forms.Splitter();
            this.pnlDest = new System.Windows.Forms.Panel();
            this.gbxDestinationCollection = new System.Windows.Forms.GroupBox();
            this.cmdSaveDestCollection = new System.Windows.Forms.Button();
            this.cmdExportToFile = new System.Windows.Forms.Button();
            this.lblCurrProteinCount = new System.Windows.Forms.Label();
            this.lvwDestination = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuMainGUI = new System.Windows.Forms.MainMenu(this.components);
            this.mnuFile = new System.Windows.Forms.MenuItem();
            this.mnuFileExit = new System.Windows.Forms.MenuItem();
            this.mnuTools = new System.Windows.Forms.MenuItem();
            this.mnuToolsCollectionEdit = new System.Windows.Forms.MenuItem();
            this.mnuToolsSep1 = new System.Windows.Forms.MenuItem();
            this.mnuAdmin = new System.Windows.Forms.MenuItem();
            this.mnuAdminBatchUploadFiles = new System.Windows.Forms.MenuItem();
            this.mnuAdminNameHashRefresh = new System.Windows.Forms.MenuItem();
            this.mnuAdminUpdateSHA = new System.Windows.Forms.MenuItem();
            this.mnuAdminUpdateCollectionsArchive = new System.Windows.Forms.MenuItem();
            this.mnuAdminTestingInterface = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
            this.pnlProgBar.SuspendLayout();
            this.pnlProgBarUpper.SuspendLayout();
            this.pnlProgBarLower.SuspendLayout();
            this.pnlSource.SuspendLayout();
            this.gbxSourceCollection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLiveSearchCancel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLiveSearchBkg)).BeginInit();
            this.pnlDest.SuspendLayout();
            this.gbxDestinationCollection.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlProgBar
            // 
            this.pnlProgBar.Controls.Add(this.pnlProgBarUpper);
            this.pnlProgBar.Controls.Add(this.pnlProgBarLower);
            this.pnlProgBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProgBar.Location = new System.Drawing.Point(0, 448);
            this.pnlProgBar.Name = "pnlProgBar";
            this.pnlProgBar.Size = new System.Drawing.Size(1132, 76);
            this.pnlProgBar.TabIndex = 0;
            this.pnlProgBar.Visible = false;
            // 
            // pnlProgBarUpper
            // 
            this.pnlProgBarUpper.Controls.Add(this.lblBatchProgress);
            this.pnlProgBarUpper.Controls.Add(this.lblCurrentTask);
            this.pnlProgBarUpper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProgBarUpper.Location = new System.Drawing.Point(0, 0);
            this.pnlProgBarUpper.Name = "pnlProgBarUpper";
            this.pnlProgBarUpper.Padding = new System.Windows.Forms.Padding(6);
            this.pnlProgBarUpper.Size = new System.Drawing.Size(1132, 42);
            this.pnlProgBarUpper.TabIndex = 2;
            // 
            // lblBatchProgress
            // 
            this.lblBatchProgress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBatchProgress.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBatchProgress.Location = new System.Drawing.Point(6, 20);
            this.lblBatchProgress.Name = "lblBatchProgress";
            this.lblBatchProgress.Size = new System.Drawing.Size(1120, 16);
            this.lblBatchProgress.TabIndex = 16;
            // 
            // lblCurrentTask
            // 
            this.lblCurrentTask.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCurrentTask.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentTask.Location = new System.Drawing.Point(6, 6);
            this.lblCurrentTask.Name = "lblCurrentTask";
            this.lblCurrentTask.Size = new System.Drawing.Size(1120, 14);
            this.lblCurrentTask.TabIndex = 0;
            this.lblCurrentTask.Text = "Reading Source File...";
            this.lblCurrentTask.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lblCurrentTask.Visible = false;
            // 
            // pnlProgBarLower
            // 
            this.pnlProgBarLower.Controls.Add(this.pgbMain);
            this.pnlProgBarLower.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlProgBarLower.Location = new System.Drawing.Point(0, 42);
            this.pnlProgBarLower.Name = "pnlProgBarLower";
            this.pnlProgBarLower.Padding = new System.Windows.Forms.Padding(6);
            this.pnlProgBarLower.Size = new System.Drawing.Size(1132, 34);
            this.pnlProgBarLower.TabIndex = 0;
            // 
            // pgbMain
            // 
            this.pgbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgbMain.Location = new System.Drawing.Point(6, 6);
            this.pgbMain.Name = "pgbMain";
            this.pgbMain.Size = new System.Drawing.Size(1120, 22);
            this.pgbMain.TabIndex = 14;
            this.pgbMain.Visible = false;
            // 
            // pnlSource
            // 
            this.pnlSource.Controls.Add(this.lblTargetServer);
            this.pnlSource.Controls.Add(this.cmdDestAdd);
            this.pnlSource.Controls.Add(this.cmdDestRemove);
            this.pnlSource.Controls.Add(this.cmdDestAddAll);
            this.pnlSource.Controls.Add(this.cmdDestRemoveAll);
            this.pnlSource.Controls.Add(this.gbxSourceCollection);
            this.pnlSource.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSource.Location = new System.Drawing.Point(0, 0);
            this.pnlSource.Name = "pnlSource";
            this.pnlSource.Padding = new System.Windows.Forms.Padding(8, 8, 8, 10);
            this.pnlSource.Size = new System.Drawing.Size(544, 448);
            this.pnlSource.TabIndex = 0;
            // 
            // lblTargetServer
            // 
            this.lblTargetServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTargetServer.Location = new System.Drawing.Point(9, 412);
            this.lblTargetServer.Name = "lblTargetServer";
            this.lblTargetServer.Size = new System.Drawing.Size(215, 15);
            this.lblTargetServer.TabIndex = 21;
            this.lblTargetServer.Text = "Target server: ";
            // 
            // cmdDestAdd
            // 
            this.cmdDestAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDestAdd.Enabled = false;
            this.cmdDestAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdDestAdd.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDestAdd.ForeColor = System.Drawing.SystemColors.Highlight;
            this.cmdDestAdd.Location = new System.Drawing.Point(491, 153);
            this.cmdDestAdd.Name = "cmdDestAdd";
            this.cmdDestAdd.Size = new System.Drawing.Size(39, 31);
            this.cmdDestAdd.TabIndex = 5;
            this.cmdDestAdd.ThemedImage = ((System.Drawing.Bitmap)(resources.GetObject("cmdDestAdd.ThemedImage")));
            this.cmdDestAdd.Click += new System.EventHandler(this.cmdDestAdd_Click);
            // 
            // cmdDestRemove
            // 
            this.cmdDestRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDestRemove.Enabled = false;
            this.cmdDestRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdDestRemove.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDestRemove.ForeColor = System.Drawing.SystemColors.Highlight;
            this.cmdDestRemove.Location = new System.Drawing.Point(491, 202);
            this.cmdDestRemove.Name = "cmdDestRemove";
            this.cmdDestRemove.Size = new System.Drawing.Size(39, 33);
            this.cmdDestRemove.TabIndex = 6;
            this.cmdDestRemove.ThemedImage = ((System.Drawing.Bitmap)(resources.GetObject("cmdDestRemove.ThemedImage")));
            this.cmdDestRemove.Click += new System.EventHandler(this.cmdDestRemove_Click);
            // 
            // cmdDestAddAll
            // 
            this.cmdDestAddAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDestAddAll.Enabled = false;
            this.cmdDestAddAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdDestAddAll.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDestAddAll.ForeColor = System.Drawing.SystemColors.Highlight;
            this.cmdDestAddAll.Location = new System.Drawing.Point(491, 103);
            this.cmdDestAddAll.Name = "cmdDestAddAll";
            this.cmdDestAddAll.Size = new System.Drawing.Size(39, 33);
            this.cmdDestAddAll.TabIndex = 3;
            this.cmdDestAddAll.ThemedImage = ((System.Drawing.Bitmap)(resources.GetObject("cmdDestAddAll.ThemedImage")));
            this.cmdDestAddAll.Click += new System.EventHandler(this.cmdDestAddAll_Click);
            // 
            // cmdDestRemoveAll
            // 
            this.cmdDestRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdDestRemoveAll.Enabled = false;
            this.cmdDestRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdDestRemoveAll.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdDestRemoveAll.ForeColor = System.Drawing.SystemColors.Highlight;
            this.cmdDestRemoveAll.Location = new System.Drawing.Point(491, 252);
            this.cmdDestRemoveAll.Name = "cmdDestRemoveAll";
            this.cmdDestRemoveAll.Size = new System.Drawing.Size(39, 32);
            this.cmdDestRemoveAll.TabIndex = 4;
            this.cmdDestRemoveAll.ThemedImage = ((System.Drawing.Bitmap)(resources.GetObject("cmdDestRemoveAll.ThemedImage")));
            this.cmdDestRemoveAll.Click += new System.EventHandler(this.cmdDestRemoveAll_Click);
            // 
            // gbxSourceCollection
            // 
            this.gbxSourceCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxSourceCollection.Controls.Add(this.cboAnnotationTypePicker);
            this.gbxSourceCollection.Controls.Add(this.lblAnnotationTypeFilter);
            this.gbxSourceCollection.Controls.Add(this.pbxLiveSearchCancel);
            this.gbxSourceCollection.Controls.Add(this.lblSearchCount);
            this.gbxSourceCollection.Controls.Add(this.cmdLoadProteins);
            this.gbxSourceCollection.Controls.Add(this.cmdLoadFile);
            this.gbxSourceCollection.Controls.Add(this.txtLiveSearch);
            this.gbxSourceCollection.Controls.Add(this.cboCollectionPicker);
            this.gbxSourceCollection.Controls.Add(this.cboOrganismFilter);
            this.gbxSourceCollection.Controls.Add(this.lblOrganismFilter);
            this.gbxSourceCollection.Controls.Add(this.lblCollectionPicker);
            this.gbxSourceCollection.Controls.Add(this.pbxLiveSearchBkg);
            this.gbxSourceCollection.Controls.Add(this.lvwSource);
            this.gbxSourceCollection.Controls.Add(this.lblSourceMembers);
            this.gbxSourceCollection.Location = new System.Drawing.Point(8, 8);
            this.gbxSourceCollection.Name = "gbxSourceCollection";
            this.gbxSourceCollection.Size = new System.Drawing.Size(472, 389);
            this.gbxSourceCollection.TabIndex = 1;
            this.gbxSourceCollection.TabStop = false;
            this.gbxSourceCollection.Text = "Source Collection";
            // 
            // cboAnnotationTypePicker
            // 
            this.cboAnnotationTypePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAnnotationTypePicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnnotationTypePicker.Location = new System.Drawing.Point(240, 46);
            this.cboAnnotationTypePicker.Name = "cboAnnotationTypePicker";
            this.cboAnnotationTypePicker.Size = new System.Drawing.Size(216, 25);
            this.cboAnnotationTypePicker.TabIndex = 17;
            // 
            // lblAnnotationTypeFilter
            // 
            this.lblAnnotationTypeFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAnnotationTypeFilter.Location = new System.Drawing.Point(240, 31);
            this.lblAnnotationTypeFilter.Name = "lblAnnotationTypeFilter";
            this.lblAnnotationTypeFilter.Size = new System.Drawing.Size(212, 17);
            this.lblAnnotationTypeFilter.TabIndex = 18;
            this.lblAnnotationTypeFilter.Text = "Naming Authority Filter";
            // 
            // pbxLiveSearchCancel
            // 
            this.pbxLiveSearchCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbxLiveSearchCancel.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pbxLiveSearchCancel.Image = ((System.Drawing.Image)(resources.GetObject("pbxLiveSearchCancel.Image")));
            this.pbxLiveSearchCancel.Location = new System.Drawing.Point(194, 359);
            this.pbxLiveSearchCancel.Name = "pbxLiveSearchCancel";
            this.pbxLiveSearchCancel.Size = new System.Drawing.Size(16, 16);
            this.pbxLiveSearchCancel.TabIndex = 16;
            this.pbxLiveSearchCancel.TabStop = false;
            this.pbxLiveSearchCancel.Click += new System.EventHandler(this.pbxLiveSearchCancel_Click);
            // 
            // lblSearchCount
            // 
            this.lblSearchCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSearchCount.Location = new System.Drawing.Point(224, 363);
            this.lblSearchCount.Name = "lblSearchCount";
            this.lblSearchCount.Size = new System.Drawing.Size(88, 16);
            this.lblSearchCount.TabIndex = 15;
            this.lblSearchCount.Text = "30000/30000";
            this.lblSearchCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cmdLoadProteins
            // 
            this.cmdLoadProteins.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoadProteins.Location = new System.Drawing.Point(352, 85);
            this.cmdLoadProteins.Name = "cmdLoadProteins";
            this.cmdLoadProteins.Size = new System.Drawing.Size(102, 24);
            this.cmdLoadProteins.TabIndex = 14;
            this.cmdLoadProteins.Text = "Load Proteins";
            this.cmdLoadProteins.Click += new System.EventHandler(this.cmdLoadProteins_Click);
            // 
            // cmdLoadFile
            // 
            this.cmdLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdLoadFile.Location = new System.Drawing.Point(316, 356);
            this.cmdLoadFile.Name = "cmdLoadFile";
            this.cmdLoadFile.Size = new System.Drawing.Size(140, 23);
            this.cmdLoadFile.TabIndex = 10;
            this.cmdLoadFile.Text = "&Import New Collection...";
            this.cmdLoadFile.Click += new System.EventHandler(this.cmdLoadFile_Click);
            // 
            // txtLiveSearch
            // 
            this.txtLiveSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLiveSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtLiveSearch.Location = new System.Drawing.Point(38, 360);
            this.txtLiveSearch.Name = "txtLiveSearch";
            this.txtLiveSearch.Size = new System.Drawing.Size(107, 17);
            this.txtLiveSearch.TabIndex = 8;
            this.txtLiveSearch.Text = "Search";
            this.txtLiveSearch.Click += new System.EventHandler(this.txtLiveSearch_Click);
            this.txtLiveSearch.TextChanged += new System.EventHandler(this.txtLiveSearch_TextChanged);
            this.txtLiveSearch.Leave += new System.EventHandler(this.txtLiveSearch_Leave);
            // 
            // cboCollectionPicker
            // 
            this.cboCollectionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboCollectionPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCollectionPicker.Location = new System.Drawing.Point(14, 85);
            this.cboCollectionPicker.Name = "cboCollectionPicker";
            this.cboCollectionPicker.Size = new System.Drawing.Size(328, 25);
            this.cboCollectionPicker.TabIndex = 1;
            // 
            // cboOrganismFilter
            // 
            this.cboOrganismFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOrganismFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrganismFilter.Location = new System.Drawing.Point(14, 46);
            this.cboOrganismFilter.Name = "cboOrganismFilter";
            this.cboOrganismFilter.Size = new System.Drawing.Size(216, 25);
            this.cboOrganismFilter.TabIndex = 0;
            // 
            // lblOrganismFilter
            // 
            this.lblOrganismFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOrganismFilter.Location = new System.Drawing.Point(14, 31);
            this.lblOrganismFilter.Name = "lblOrganismFilter";
            this.lblOrganismFilter.Size = new System.Drawing.Size(212, 17);
            this.lblOrganismFilter.TabIndex = 3;
            this.lblOrganismFilter.Text = "Organism Selector";
            // 
            // lblCollectionPicker
            // 
            this.lblCollectionPicker.Location = new System.Drawing.Point(14, 68);
            this.lblCollectionPicker.Name = "lblCollectionPicker";
            this.lblCollectionPicker.Size = new System.Drawing.Size(100, 15);
            this.lblCollectionPicker.TabIndex = 4;
            this.lblCollectionPicker.Text = "Protein Collection";
            // 
            // pbxLiveSearchBkg
            // 
            this.pbxLiveSearchBkg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbxLiveSearchBkg.Image = ((System.Drawing.Image)(resources.GetObject("pbxLiveSearchBkg.Image")));
            this.pbxLiveSearchBkg.Location = new System.Drawing.Point(16, 355);
            this.pbxLiveSearchBkg.Name = "pbxLiveSearchBkg";
            this.pbxLiveSearchBkg.Size = new System.Drawing.Size(200, 26);
            this.pbxLiveSearchBkg.TabIndex = 9;
            this.pbxLiveSearchBkg.TabStop = false;
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
            this.lvwSource.Size = new System.Drawing.Size(442, 217);
            this.lvwSource.TabIndex = 2;
            this.lvwSource.UseCompatibleStateImageBehavior = false;
            this.lvwSource.View = System.Windows.Forms.View.Details;
            this.lvwSource.DoubleClick += new System.EventHandler(this.lvwSource_DoubleClick);
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
            // SourceDestSplit
            // 
            this.SourceDestSplit.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.SourceDestSplit.Location = new System.Drawing.Point(544, 0);
            this.SourceDestSplit.MinExtra = 265;
            this.SourceDestSplit.MinSize = 450;
            this.SourceDestSplit.Name = "SourceDestSplit";
            this.SourceDestSplit.Size = new System.Drawing.Size(3, 448);
            this.SourceDestSplit.TabIndex = 2;
            this.SourceDestSplit.TabStop = false;
            // 
            // pnlDest
            // 
            this.pnlDest.Controls.Add(this.gbxDestinationCollection);
            this.pnlDest.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDest.Location = new System.Drawing.Point(547, 0);
            this.pnlDest.Name = "pnlDest";
            this.pnlDest.Padding = new System.Windows.Forms.Padding(8, 8, 8, 10);
            this.pnlDest.Size = new System.Drawing.Size(585, 448);
            this.pnlDest.TabIndex = 1;
            // 
            // gbxDestinationCollection
            // 
            this.gbxDestinationCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxDestinationCollection.Controls.Add(this.cmdSaveDestCollection);
            this.gbxDestinationCollection.Controls.Add(this.cmdExportToFile);
            this.gbxDestinationCollection.Controls.Add(this.lblCurrProteinCount);
            this.gbxDestinationCollection.Controls.Add(this.lvwDestination);
            this.gbxDestinationCollection.Location = new System.Drawing.Point(8, 8);
            this.gbxDestinationCollection.Name = "gbxDestinationCollection";
            this.gbxDestinationCollection.Size = new System.Drawing.Size(569, 430);
            this.gbxDestinationCollection.TabIndex = 2;
            this.gbxDestinationCollection.TabStop = false;
            this.gbxDestinationCollection.Text = "Destination Collection";
            // 
            // cmdSaveDestCollection
            // 
            this.cmdSaveDestCollection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSaveDestCollection.Location = new System.Drawing.Point(444, 395);
            this.cmdSaveDestCollection.Name = "cmdSaveDestCollection";
            this.cmdSaveDestCollection.Size = new System.Drawing.Size(113, 24);
            this.cmdSaveDestCollection.TabIndex = 4;
            this.cmdSaveDestCollection.Text = "&Upload Collection...";
            this.cmdSaveDestCollection.Click += new System.EventHandler(this.cmdSaveDestCollection_Click);
            // 
            // cmdExportToFile
            // 
            this.cmdExportToFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdExportToFile.Enabled = false;
            this.cmdExportToFile.Location = new System.Drawing.Point(14, 395);
            this.cmdExportToFile.Name = "cmdExportToFile";
            this.cmdExportToFile.Size = new System.Drawing.Size(102, 24);
            this.cmdExportToFile.TabIndex = 3;
            this.cmdExportToFile.Text = "Export to File...";
            // 
            // lblCurrProteinCount
            // 
            this.lblCurrProteinCount.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrProteinCount.Location = new System.Drawing.Point(14, 19);
            this.lblCurrProteinCount.Name = "lblCurrProteinCount";
            this.lblCurrProteinCount.Size = new System.Drawing.Size(164, 15);
            this.lblCurrProteinCount.TabIndex = 2;
            this.lblCurrProteinCount.Text = "Protein Count: 0";
            // 
            // lvwDestination
            // 
            this.lvwDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwDestination.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName});
            this.lvwDestination.FullRowSelect = true;
            this.lvwDestination.GridLines = true;
            this.lvwDestination.HideSelection = false;
            this.lvwDestination.Location = new System.Drawing.Point(14, 53);
            this.lvwDestination.Name = "lvwDestination";
            this.lvwDestination.Size = new System.Drawing.Size(540, 335);
            this.lvwDestination.TabIndex = 0;
            this.lvwDestination.UseCompatibleStateImageBehavior = false;
            this.lvwDestination.View = System.Windows.Forms.View.Details;
            this.lvwDestination.DoubleClick += new System.EventHandler(this.lvwDestination_DoubleClick);
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 228;
            // 
            // mnuMainGUI
            // 
            this.mnuMainGUI.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuTools,
            this.mnuAdmin,
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
            // mnuTools
            // 
            this.mnuTools.Index = 1;
            this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuToolsCollectionEdit,
            this.mnuToolsSep1});
            this.mnuTools.Text = "&Tools";
            this.mnuTools.Visible = false;
            // 
            // mnuToolsCollectionEdit
            // 
            this.mnuToolsCollectionEdit.Index = 0;
            this.mnuToolsCollectionEdit.Text = "&Edit Collection States...";
            this.mnuToolsCollectionEdit.Click += new System.EventHandler(this.mnuToolsCollectionEdit_Click);
            // 
            // mnuToolsSep1
            // 
            this.mnuToolsSep1.Index = 1;
            this.mnuToolsSep1.Text = "-";
            // 
            // mnuAdmin
            // 
            this.mnuAdmin.Index = 2;
            this.mnuAdmin.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAdminBatchUploadFiles,
            this.mnuAdminNameHashRefresh,
            this.mnuAdminUpdateSHA,
            this.mnuAdminUpdateCollectionsArchive,
            this.mnuAdminTestingInterface});
            this.mnuAdmin.Text = "Admin";
            this.mnuAdmin.Visible = false;
            // 
            // mnuAdminBatchUploadFiles
            // 
            this.mnuAdminBatchUploadFiles.Index = 0;
            this.mnuAdminBatchUploadFiles.Text = "Batch Upload FASTA Files Using DMS...";
            this.mnuAdminBatchUploadFiles.Visible = false;
            // 
            // mnuAdminNameHashRefresh
            // 
            this.mnuAdminNameHashRefresh.Index = 1;
            this.mnuAdminNameHashRefresh.Text = "Refresh Protein Name Hashes";
            this.mnuAdminNameHashRefresh.Visible = false;
            this.mnuAdminNameHashRefresh.Click += new System.EventHandler(this.mnuAdminNameHashRefresh_Click);
            // 
            // mnuAdminUpdateSHA
            // 
            this.mnuAdminUpdateSHA.Enabled = false;
            this.mnuAdminUpdateSHA.Index = 2;
            this.mnuAdminUpdateSHA.Text = "Update File Authentication Hashes";
            this.mnuAdminUpdateSHA.Visible = false;
            // 
            // mnuAdminUpdateCollectionsArchive
            // 
            this.mnuAdminUpdateCollectionsArchive.Enabled = false;
            this.mnuAdminUpdateCollectionsArchive.Index = 3;
            this.mnuAdminUpdateCollectionsArchive.Text = "Update Collections Archive";
            this.mnuAdminUpdateCollectionsArchive.Visible = false;
            // 
            // mnuAdminTestingInterface
            // 
            this.mnuAdminTestingInterface.Index = 4;
            this.mnuAdminTestingInterface.Text = "Testing Interface Window...";
            this.mnuAdminTestingInterface.Click += new System.EventHandler(this.MenuItem5_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 3;
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
            // frmCollectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 524);
            this.Controls.Add(this.pnlDest);
            this.Controls.Add(this.SourceDestSplit);
            this.Controls.Add(this.pnlSource);
            this.Controls.Add(this.pnlProgBar);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mnuMainGUI;
            this.MinimumSize = new System.Drawing.Size(820, 453);
            this.Name = "frmCollectionEditor";
            this.Text = "Protein Collection Editor";
            this.pnlProgBar.ResumeLayout(false);
            this.pnlProgBarUpper.ResumeLayout(false);
            this.pnlProgBarLower.ResumeLayout(false);
            this.pnlSource.ResumeLayout(false);
            this.gbxSourceCollection.ResumeLayout(false);
            this.gbxSourceCollection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLiveSearchCancel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLiveSearchBkg)).EndInit();
            this.pnlDest.ResumeLayout(false);
            this.gbxDestinationCollection.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlProgBar;
        private System.Windows.Forms.GroupBox gbxSourceCollection;
        private System.Windows.Forms.ComboBox cboAnnotationTypePicker;
        private System.Windows.Forms.Label lblAnnotationTypeFilter;
        private System.Windows.Forms.PictureBox pbxLiveSearchCancel;
        private System.Windows.Forms.Label lblSearchCount;
        private System.Windows.Forms.Button cmdLoadProteins;
        private System.Windows.Forms.Button cmdLoadFile;
        private System.Windows.Forms.TextBox txtLiveSearch;
        private System.Windows.Forms.ComboBox cboCollectionPicker;
        private System.Windows.Forms.ComboBox cboOrganismFilter;
        private System.Windows.Forms.Label lblOrganismFilter;
        private System.Windows.Forms.Label lblCollectionPicker;
        private System.Windows.Forms.PictureBox pbxLiveSearchBkg;
        private System.Windows.Forms.ListView lvwSource;
        private System.Windows.Forms.Label lblSourceMembers;
        private System.Windows.Forms.ColumnHeader colSrcName;
        private System.Windows.Forms.ColumnHeader colSrcDesc;
        private ImageButton cmdDestAdd;
        private ImageButton cmdDestRemove;
        private ImageButton cmdDestAddAll;
        private ImageButton cmdDestRemoveAll;
        private System.Windows.Forms.GroupBox gbxDestinationCollection;
        private System.Windows.Forms.Label lblCurrProteinCount;
        private System.Windows.Forms.ListView lvwDestination;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.Panel pnlProgBarLower;
        private System.Windows.Forms.ProgressBar pgbMain;
        private System.Windows.Forms.Panel pnlSource;
        private System.Windows.Forms.Splitter SourceDestSplit;
        private System.Windows.Forms.Panel pnlDest;
        private System.Windows.Forms.Panel pnlProgBarUpper;
        private System.Windows.Forms.Label lblCurrentTask;
        private System.Windows.Forms.MainMenu mnuMainGUI;
        private System.Windows.Forms.MenuItem mnuFile;
        private System.Windows.Forms.MenuItem mnuHelp;
        private System.Windows.Forms.MenuItem mnuTools;
        private System.Windows.Forms.MenuItem mnuAdmin;
        private System.Windows.Forms.MenuItem mnuFileExit;
        private System.Windows.Forms.MenuItem mnuToolsCollectionEdit;
        private System.Windows.Forms.MenuItem mnuHelpAbout;
        private System.Windows.Forms.MenuItem mnuAdminNameHashRefresh;
        private System.Windows.Forms.MenuItem mnuToolsSep1;
        private System.Windows.Forms.MenuItem mnuAdminBatchUploadFiles;
        private System.Windows.Forms.MenuItem mnuAdminUpdateSHA;
        private System.Windows.Forms.MenuItem mnuAdminUpdateCollectionsArchive;
        private System.Windows.Forms.MenuItem mnuAdminTestingInterface;
        private System.Windows.Forms.Label lblBatchProgress;
        private System.Windows.Forms.Button cmdExportToFile;
        private System.Windows.Forms.Label lblTargetServer;
        private System.Windows.Forms.Button cmdSaveDestCollection;
    }
}