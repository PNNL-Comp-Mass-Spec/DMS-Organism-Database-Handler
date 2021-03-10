using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using AppUI_OrfDBHandler.Properties;
using ExtractAnnotationFromDescription;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Protein_Importer;
using Protein_Uploader;
using ValidateFastaFile;

namespace AppUI_OrfDBHandler
{
    public class frmCollectionEditor : Form
    {
        #region "Windows Form Designer generated code"

        public frmCollectionEditor() : base()
        {
            SearchTimer = new System.Timers.Timer(2000d);
            SearchTimer.Elapsed += SearchTimerHandler;
            MemberLoadTimer = new System.Timers.Timer(2000d);
            MemberLoadTimer.Elapsed += MemberLoadTimerHandler;
            base.Load += frmCollectionEditor_Load;

            // This call is required by the Windows Form Designer.
            InitializeComponent();
            CheckTransferButtonsEnabledStatus();

            // Add any initialization after the InitializeComponent() call

            m_CachedFileDescriptions = new Dictionary<string, KeyValuePair<string, string>>();

            ReadSettings();
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
        internal Panel pnlProgBar;
        internal GroupBox gbxSourceCollection;
        internal ComboBox cboAnnotationTypePicker;
        internal Label lblAnnotationTypeFilter;
        internal PictureBox pbxLiveSearchCancel;
        internal Label lblSearchCount;
        internal Button cmdLoadProteins;
        internal Button cmdLoadFile;
        internal TextBox txtLiveSearch;
        internal ComboBox cboCollectionPicker;
        internal ComboBox cboOrganismFilter;
        internal Label lblOrganismFilter;
        internal Label lblCollectionPicker;
        internal PictureBox pbxLiveSearchBkg;
        internal ListView lvwSource;
        internal Label lblSourceMembers;
        internal ColumnHeader colSrcName;
        internal ColumnHeader colSrcDesc;
        internal UIControls.ImageButton cmdDestAdd;
        internal UIControls.ImageButton cmdDestRemove;
        internal UIControls.ImageButton cmdDestAddAll;
        internal UIControls.ImageButton cmdDestRemoveAll;
        internal GroupBox gbxDestinationCollection;
        internal Label lblCurrProteinCount;
        internal ListView lvwDestination;
        internal ColumnHeader colName;
        internal Panel pnlProgBarLower;
        internal ProgressBar pgbMain;
        internal Panel pnlSource;
        internal Splitter SourceDestSplit;
        internal Panel pnlDest;
        internal Panel pnlProgBarUpper;
        internal Label lblCurrentTask;
        internal MainMenu mnuMainGUI;
        internal MenuItem mnuFile;
        internal MenuItem mnuHelp;
        internal MenuItem mnuTools;
        internal MenuItem mnuAdmin;
        internal MenuItem mnuFileExit;
        internal MenuItem mnuToolsNucToProt;
        internal MenuItem mnuToolsConvert;
        internal MenuItem mnuToolsConvertF2A;
        internal MenuItem mnuToolsConvertA2F;
        internal MenuItem mnuToolsFCheckup;
        internal MenuItem mnuToolsOptions;
        internal MenuItem mnuToolsCollectionEdit;
        internal MenuItem mnuToolsCompareDBs;
        internal MenuItem mnuToolsExtractFromFile;
        internal MenuItem mnuHelpAbout;
        internal MenuItem mnuAdminNameHashRefresh;
        internal MenuItem mnuToolsSep1;
        internal MenuItem mnuAdminBatchUploadFiles;
        internal MenuItem mnuAdminUpdateSHA;
        internal MenuItem mnuAdminUpdateCollectionsArchive;
        internal MenuItem mnuAdminUpdateZeroedMasses;
        internal MenuItem mnuAdminTestingInterface;
        internal MenuItem mnuAdminFixArchivePaths;
        internal MenuItem mnuAdminAddSortingIndexes;
        internal Label lblBatchProgress;
        internal Button cmdExportToFile;
        internal Label lblTargetServer;
        internal Button cmdSaveDestCollection;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCollectionEditor));
            pnlProgBar = new Panel();
            pnlProgBarUpper = new Panel();
            lblBatchProgress = new Label();
            lblCurrentTask = new Label();
            pnlProgBarLower = new Panel();
            pgbMain = new ProgressBar();
            pnlSource = new Panel();
            lblTargetServer = new Label();
            cmdDestAdd = new UIControls.ImageButton();
            cmdDestAdd.Click += new EventHandler(cmdDestAdd_Click);
            cmdDestRemove = new UIControls.ImageButton();
            cmdDestRemove.Click += new EventHandler(cmdDestRemove_Click);
            cmdDestAddAll = new UIControls.ImageButton();
            cmdDestAddAll.Click += new EventHandler(cmdDestAddAll_Click);
            cmdDestRemoveAll = new UIControls.ImageButton();
            cmdDestRemoveAll.Click += new EventHandler(cmdDestRemoveAll_Click);
            gbxSourceCollection = new GroupBox();
            cboAnnotationTypePicker = new ComboBox();
            lblAnnotationTypeFilter = new Label();
            pbxLiveSearchCancel = new PictureBox();
            pbxLiveSearchCancel.Click += new EventHandler(pbxLiveSearchCancel_Click);
            lblSearchCount = new Label();
            cmdLoadProteins = new Button();
            cmdLoadProteins.Click += new EventHandler(cmdLoadProteins_Click);
            cmdLoadFile = new Button();
            cmdLoadFile.Click += new EventHandler(cmdLoadFile_Click);
            txtLiveSearch = new TextBox();
            txtLiveSearch.TextChanged += new EventHandler(txtLiveSearch_TextChanged);
            txtLiveSearch.Click += new EventHandler(txtLiveSearch_Click);
            txtLiveSearch.Leave += new EventHandler(txtLiveSearch_Leave);
            cboCollectionPicker = new ComboBox();
            cboOrganismFilter = new ComboBox();
            lblOrganismFilter = new Label();
            lblCollectionPicker = new Label();
            pbxLiveSearchBkg = new PictureBox();
            lvwSource = new ListView();
            lvwSource.DoubleClick += new EventHandler(lvwSource_DoubleClick);
            colSrcName = new ColumnHeader();
            colSrcDesc = new ColumnHeader();
            lblSourceMembers = new Label();
            SourceDestSplit = new Splitter();
            pnlDest = new Panel();
            gbxDestinationCollection = new GroupBox();
            cmdSaveDestCollection = new Button();
            cmdSaveDestCollection.Click += new EventHandler(cmdSaveDestCollection_Click);
            cmdExportToFile = new Button();
            lblCurrProteinCount = new Label();
            lvwDestination = new ListView();
            lvwDestination.DoubleClick += new EventHandler(lvwDestination_DoubleClick);
            colName = new ColumnHeader();
            mnuMainGUI = new MainMenu(components);
            mnuFile = new MenuItem();
            mnuFileExit = new MenuItem();
            mnuFileExit.Click += new EventHandler(mnuFileExit_Click);
            mnuTools = new MenuItem();
            mnuToolsCollectionEdit = new MenuItem();
            mnuToolsCollectionEdit.Click += new EventHandler(mnuToolsCollectionEdit_Click);
            mnuToolsNucToProt = new MenuItem();
            mnuToolsNucToProt.Click += new EventHandler(mnuToolsNucToProt_Click);
            mnuToolsConvert = new MenuItem();
            mnuToolsConvert.Click += new EventHandler(mnuToolsConvert_Click);
            mnuToolsConvertF2A = new MenuItem();
            mnuToolsConvertF2A.Click += new EventHandler(mnuToolsConvertF2A_Click);
            mnuToolsConvertA2F = new MenuItem();
            mnuToolsConvertA2F.Click += new EventHandler(mnuToolsConvertA2F_Click);
            mnuToolsFCheckup = new MenuItem();
            mnuToolsFCheckup.Click += new EventHandler(mnuToolsFCheckup_Click);
            mnuToolsCompareDBs = new MenuItem();
            mnuToolsExtractFromFile = new MenuItem();
            mnuToolsExtractFromFile.Click += new EventHandler(mnuToolsExtractFromFile_Click);
            mnuToolsSep1 = new MenuItem();
            mnuToolsOptions = new MenuItem();
            mnuToolsOptions.Click += new EventHandler(mnuToolsOptions_Click);
            mnuAdmin = new MenuItem();
            mnuAdminBatchUploadFiles = new MenuItem();
            mnuAdminNameHashRefresh = new MenuItem();
            mnuAdminNameHashRefresh.Click += new EventHandler(mnuAdminNameHashRefresh_Click);
            mnuAdminUpdateSHA = new MenuItem();
            mnuAdminUpdateCollectionsArchive = new MenuItem();
            mnuAdminUpdateZeroedMasses = new MenuItem();
            mnuAdminUpdateZeroedMasses.Click += new EventHandler(mnuAdminUpdateZeroedMasses_Click);
            mnuAdminTestingInterface = new MenuItem();
            mnuAdminTestingInterface.Click += new EventHandler(MenuItem5_Click);
            mnuAdminFixArchivePaths = new MenuItem();
            mnuAdminFixArchivePaths.Click += new EventHandler(MenuItem6_Click);
            mnuAdminAddSortingIndexes = new MenuItem();
            mnuAdminAddSortingIndexes.Click += new EventHandler(MenuItem8_Click);
            mnuHelp = new MenuItem();
            mnuHelpAbout = new MenuItem();
            mnuHelpAbout.Click += new EventHandler(mnuHelpAbout_Click);
            pnlProgBar.SuspendLayout();
            pnlProgBarUpper.SuspendLayout();
            pnlProgBarLower.SuspendLayout();
            pnlSource.SuspendLayout();
            gbxSourceCollection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbxLiveSearchCancel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbxLiveSearchBkg).BeginInit();
            pnlDest.SuspendLayout();
            gbxDestinationCollection.SuspendLayout();
            SuspendLayout();
            //
            // pnlProgBar
            //
            pnlProgBar.Controls.Add(pnlProgBarUpper);
            pnlProgBar.Controls.Add(pnlProgBarLower);
            pnlProgBar.Dock = DockStyle.Bottom;
            pnlProgBar.Location = new Point(0, 432);
            pnlProgBar.Name = "pnlProgBar";
            pnlProgBar.Size = new Size(1130, 92);
            pnlProgBar.TabIndex = 0;
            pnlProgBar.Visible = false;
            //
            // pnlProgBarUpper
            //
            pnlProgBarUpper.Controls.Add(lblBatchProgress);
            pnlProgBarUpper.Controls.Add(lblCurrentTask);
            pnlProgBarUpper.Dock = DockStyle.Fill;
            pnlProgBarUpper.Location = new Point(0, 0);
            pnlProgBarUpper.Name = "pnlProgBarUpper";
            pnlProgBarUpper.Padding = new Padding(6);
            pnlProgBarUpper.Size = new Size(1130, 51);
            pnlProgBarUpper.TabIndex = 2;
            //
            // lblBatchProgress
            //
            lblBatchProgress.Dock = DockStyle.Fill;
            lblBatchProgress.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            lblBatchProgress.Location = new Point(6, 23);
            lblBatchProgress.Name = "lblBatchProgress";
            lblBatchProgress.Size = new Size(1118, 22);
            lblBatchProgress.TabIndex = 16;
            //
            // lblCurrentTask
            //
            lblCurrentTask.Dock = DockStyle.Top;
            lblCurrentTask.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            lblCurrentTask.Location = new Point(6, 6);
            lblCurrentTask.Name = "lblCurrentTask";
            lblCurrentTask.Size = new Size(1118, 17);
            lblCurrentTask.TabIndex = 0;
            lblCurrentTask.Text = "Reading Source File...";
            lblCurrentTask.TextAlign = ContentAlignment.BottomLeft;
            lblCurrentTask.Visible = false;
            //
            // pnlProgBarLower
            //
            pnlProgBarLower.Controls.Add(pgbMain);
            pnlProgBarLower.Dock = DockStyle.Bottom;
            pnlProgBarLower.Location = new Point(0, 51);
            pnlProgBarLower.Name = "pnlProgBarLower";
            pnlProgBarLower.Padding = new Padding(6);
            pnlProgBarLower.Size = new Size(1130, 41);
            pnlProgBarLower.TabIndex = 0;
            //
            // pgbMain
            //
            pgbMain.Dock = DockStyle.Fill;
            pgbMain.Location = new Point(6, 6);
            pgbMain.Name = "pgbMain";
            pgbMain.Size = new Size(1118, 29);
            pgbMain.TabIndex = 14;
            pgbMain.Visible = false;
            //
            // pnlSource
            //
            pnlSource.Controls.Add(lblTargetServer);
            pnlSource.Controls.Add(cmdDestAdd);
            pnlSource.Controls.Add(cmdDestRemove);
            pnlSource.Controls.Add(cmdDestAddAll);
            pnlSource.Controls.Add(cmdDestRemoveAll);
            pnlSource.Controls.Add(gbxSourceCollection);
            pnlSource.Dock = DockStyle.Left;
            pnlSource.Location = new Point(0, 0);
            pnlSource.Name = "pnlSource";
            pnlSource.Padding = new Padding(8, 8, 8, 10);
            pnlSource.Size = new Size(762, 432);
            pnlSource.TabIndex = 0;
            //
            // lblTargetServer
            //
            lblTargetServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblTargetServer.Location = new Point(13, 388);
            lblTargetServer.Name = "lblTargetServer";
            lblTargetServer.Size = new Size(300, 19);
            lblTargetServer.TabIndex = 21;
            lblTargetServer.Text = "Target server: ";
            //
            // cmdDestAdd
            //
            cmdDestAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdDestAdd.Enabled = false;
            cmdDestAdd.FlatStyle = FlatStyle.System;
            cmdDestAdd.Font = new Font("Tahoma", 12.0f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdDestAdd.ForeColor = SystemColors.Highlight;
            cmdDestAdd.Location = new Point(688, 186);
            cmdDestAdd.Name = "cmdDestAdd";
            cmdDestAdd.Size = new Size(54, 38);
            cmdDestAdd.TabIndex = 5;
            cmdDestAdd.ThemedImage = (Bitmap)resources.GetObject("cmdDestAdd.ThemedImage");
            //
            // cmdDestRemove
            //
            cmdDestRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdDestRemove.Enabled = false;
            cmdDestRemove.FlatStyle = FlatStyle.System;
            cmdDestRemove.Font = new Font("Tahoma", 12.0f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdDestRemove.ForeColor = SystemColors.Highlight;
            cmdDestRemove.Location = new Point(688, 245);
            cmdDestRemove.Name = "cmdDestRemove";
            cmdDestRemove.Size = new Size(54, 40);
            cmdDestRemove.TabIndex = 6;
            cmdDestRemove.ThemedImage = (Bitmap)resources.GetObject("cmdDestRemove.ThemedImage");
            //
            // cmdDestAddAll
            //
            cmdDestAddAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdDestAddAll.Enabled = false;
            cmdDestAddAll.FlatStyle = FlatStyle.System;
            cmdDestAddAll.Font = new Font("Tahoma", 12.0f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdDestAddAll.ForeColor = SystemColors.Highlight;
            cmdDestAddAll.Location = new Point(688, 125);
            cmdDestAddAll.Name = "cmdDestAddAll";
            cmdDestAddAll.Size = new Size(54, 40);
            cmdDestAddAll.TabIndex = 3;
            cmdDestAddAll.ThemedImage = (Bitmap)resources.GetObject("cmdDestAddAll.ThemedImage");
            //
            // cmdDestRemoveAll
            //
            cmdDestRemoveAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdDestRemoveAll.Enabled = false;
            cmdDestRemoveAll.FlatStyle = FlatStyle.System;
            cmdDestRemoveAll.Font = new Font("Tahoma", 12.0f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdDestRemoveAll.ForeColor = SystemColors.Highlight;
            cmdDestRemoveAll.Location = new Point(688, 306);
            cmdDestRemoveAll.Name = "cmdDestRemoveAll";
            cmdDestRemoveAll.Size = new Size(54, 39);
            cmdDestRemoveAll.TabIndex = 4;
            cmdDestRemoveAll.ThemedImage = (Bitmap)resources.GetObject("cmdDestRemoveAll.ThemedImage");
            //
            // gbxSourceCollection
            //
            gbxSourceCollection.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbxSourceCollection.Controls.Add(cboAnnotationTypePicker);
            gbxSourceCollection.Controls.Add(lblAnnotationTypeFilter);
            gbxSourceCollection.Controls.Add(pbxLiveSearchCancel);
            gbxSourceCollection.Controls.Add(lblSearchCount);
            gbxSourceCollection.Controls.Add(cmdLoadProteins);
            gbxSourceCollection.Controls.Add(cmdLoadFile);
            gbxSourceCollection.Controls.Add(txtLiveSearch);
            gbxSourceCollection.Controls.Add(cboCollectionPicker);
            gbxSourceCollection.Controls.Add(cboOrganismFilter);
            gbxSourceCollection.Controls.Add(lblOrganismFilter);
            gbxSourceCollection.Controls.Add(lblCollectionPicker);
            gbxSourceCollection.Controls.Add(pbxLiveSearchBkg);
            gbxSourceCollection.Controls.Add(lvwSource);
            gbxSourceCollection.Controls.Add(lblSourceMembers);
            gbxSourceCollection.Location = new Point(11, 10);
            gbxSourceCollection.Name = "gbxSourceCollection";
            gbxSourceCollection.Size = new Size(661, 360);
            gbxSourceCollection.TabIndex = 1;
            gbxSourceCollection.TabStop = false;
            gbxSourceCollection.Text = "Source Collection";
            //
            // cboAnnotationTypePicker
            //
            cboAnnotationTypePicker.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboAnnotationTypePicker.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAnnotationTypePicker.Location = new Point(336, 56);
            cboAnnotationTypePicker.Name = "cboAnnotationTypePicker";
            cboAnnotationTypePicker.Size = new Size(302, 25);
            cboAnnotationTypePicker.TabIndex = 17;
            //
            // lblAnnotationTypeFilter
            //
            lblAnnotationTypeFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAnnotationTypeFilter.Location = new Point(336, 38);
            lblAnnotationTypeFilter.Name = "lblAnnotationTypeFilter";
            lblAnnotationTypeFilter.Size = new Size(297, 20);
            lblAnnotationTypeFilter.TabIndex = 18;
            lblAnnotationTypeFilter.Text = "Naming Authority Filter";
            //
            // pbxLiveSearchCancel
            //
            pbxLiveSearchCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbxLiveSearchCancel.BackColor = SystemColors.ActiveCaptionText;
            pbxLiveSearchCancel.Image = (Image)resources.GetObject("pbxLiveSearchCancel.Image");
            pbxLiveSearchCancel.Location = new Point(272, 324);
            pbxLiveSearchCancel.Name = "pbxLiveSearchCancel";
            pbxLiveSearchCancel.Size = new Size(22, 20);
            pbxLiveSearchCancel.TabIndex = 16;
            pbxLiveSearchCancel.TabStop = false;
            //
            // lblSearchCount
            //
            lblSearchCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblSearchCount.Location = new Point(314, 329);
            lblSearchCount.Name = "lblSearchCount";
            lblSearchCount.Size = new Size(123, 19);
            lblSearchCount.TabIndex = 15;
            lblSearchCount.Text = "30000/30000";
            lblSearchCount.TextAlign = ContentAlignment.TopCenter;
            //
            // cmdLoadProteins
            //
            cmdLoadProteins.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdLoadProteins.Location = new Point(493, 103);
            cmdLoadProteins.Name = "cmdLoadProteins";
            cmdLoadProteins.Size = new Size(143, 29);
            cmdLoadProteins.TabIndex = 14;
            cmdLoadProteins.Text = "Load Proteins";
            //
            // cmdLoadFile
            //
            cmdLoadFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdLoadFile.Location = new Point(442, 320);
            cmdLoadFile.Name = "cmdLoadFile";
            cmdLoadFile.Size = new Size(196, 29);
            cmdLoadFile.TabIndex = 10;
            cmdLoadFile.Text = "&Import New Collection...";
            //
            // txtLiveSearch
            //
            txtLiveSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLiveSearch.BorderStyle = BorderStyle.None;
            txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
            txtLiveSearch.Location = new Point(53, 326);
            txtLiveSearch.Name = "txtLiveSearch";
            txtLiveSearch.Size = new Size(150, 17);
            txtLiveSearch.TabIndex = 8;
            txtLiveSearch.Text = "Search";
            //
            // cboCollectionPicker
            //
            cboCollectionPicker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboCollectionPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCollectionPicker.Location = new Point(20, 103);
            cboCollectionPicker.Name = "cboCollectionPicker";
            cboCollectionPicker.Size = new Size(459, 25);
            cboCollectionPicker.TabIndex = 1;
            //
            // cboOrganismFilter
            //
            cboOrganismFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboOrganismFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOrganismFilter.Location = new Point(20, 56);
            cboOrganismFilter.Name = "cboOrganismFilter";
            cboOrganismFilter.Size = new Size(302, 25);
            cboOrganismFilter.TabIndex = 0;
            //
            // lblOrganismFilter
            //
            lblOrganismFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblOrganismFilter.Location = new Point(20, 38);
            lblOrganismFilter.Name = "lblOrganismFilter";
            lblOrganismFilter.Size = new Size(296, 20);
            lblOrganismFilter.TabIndex = 3;
            lblOrganismFilter.Text = "Organism Selector";
            //
            // lblCollectionPicker
            //
            lblCollectionPicker.Location = new Point(20, 82);
            lblCollectionPicker.Name = "lblCollectionPicker";
            lblCollectionPicker.Size = new Size(140, 19);
            lblCollectionPicker.TabIndex = 4;
            lblCollectionPicker.Text = "Protein Collection";
            //
            // pbxLiveSearchBkg
            //
            pbxLiveSearchBkg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbxLiveSearchBkg.Image = (Image)resources.GetObject("pbxLiveSearchBkg.Image");
            pbxLiveSearchBkg.Location = new Point(22, 319);
            pbxLiveSearchBkg.Name = "pbxLiveSearchBkg";
            pbxLiveSearchBkg.Size = new Size(280, 32);
            pbxLiveSearchBkg.TabIndex = 9;
            pbxLiveSearchBkg.TabStop = false;
            //
            // lvwSource
            //
            lvwSource.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwSource.Columns.AddRange(new ColumnHeader[] { colSrcName, colSrcDesc });
            lvwSource.FullRowSelect = true;
            lvwSource.GridLines = true;
            lvwSource.Location = new Point(20, 158);
            lvwSource.Name = "lvwSource";
            lvwSource.Size = new Size(618, 152);
            lvwSource.TabIndex = 2;
            lvwSource.UseCompatibleStateImageBehavior = false;
            lvwSource.View = View.Details;
            //
            // colSrcName
            //
            colSrcName.Text = "Name";
            colSrcName.Width = 117;
            //
            // colSrcDesc
            //
            colSrcDesc.Text = "Description";
            colSrcDesc.Width = 320;
            //
            // lblSourceMembers
            //
            lblSourceMembers.Location = new Point(20, 135);
            lblSourceMembers.Name = "lblSourceMembers";
            lblSourceMembers.Size = new Size(179, 20);
            lblSourceMembers.TabIndex = 5;
            lblSourceMembers.Text = "Collection Members";
            //
            // SourceDestSplit
            //
            SourceDestSplit.BackColor = SystemColors.ActiveBorder;
            SourceDestSplit.Location = new Point(762, 0);
            SourceDestSplit.MinExtra = 265;
            SourceDestSplit.MinSize = 450;
            SourceDestSplit.Name = "SourceDestSplit";
            SourceDestSplit.Size = new Size(4, 432);
            SourceDestSplit.TabIndex = 2;
            SourceDestSplit.TabStop = false;
            //
            // pnlDest
            //
            pnlDest.Controls.Add(gbxDestinationCollection);
            pnlDest.Dock = DockStyle.Fill;
            pnlDest.Location = new Point(766, 0);
            pnlDest.Name = "pnlDest";
            pnlDest.Padding = new Padding(8, 8, 8, 10);
            pnlDest.Size = new Size(364, 432);
            pnlDest.TabIndex = 1;
            //
            // gbxDestinationCollection
            //
            gbxDestinationCollection.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbxDestinationCollection.Controls.Add(cmdSaveDestCollection);
            gbxDestinationCollection.Controls.Add(cmdExportToFile);
            gbxDestinationCollection.Controls.Add(lblCurrProteinCount);
            gbxDestinationCollection.Controls.Add(lvwDestination);
            gbxDestinationCollection.Location = new Point(11, 10);
            gbxDestinationCollection.Name = "gbxDestinationCollection";
            gbxDestinationCollection.Size = new Size(342, 410);
            gbxDestinationCollection.TabIndex = 2;
            gbxDestinationCollection.TabStop = false;
            gbxDestinationCollection.Text = "Destination Collection";
            //
            // cmdSaveDestCollection
            //
            cmdSaveDestCollection.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdSaveDestCollection.Location = new Point(167, 368);
            cmdSaveDestCollection.Name = "cmdSaveDestCollection";
            cmdSaveDestCollection.Size = new Size(159, 29);
            cmdSaveDestCollection.TabIndex = 4;
            cmdSaveDestCollection.Text = "&Upload Collection...";
            //
            // cmdExportToFile
            //
            cmdExportToFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmdExportToFile.Enabled = false;
            cmdExportToFile.Location = new Point(20, 368);
            cmdExportToFile.Name = "cmdExportToFile";
            cmdExportToFile.Size = new Size(142, 29);
            cmdExportToFile.TabIndex = 3;
            cmdExportToFile.Text = "Export to File...";
            //
            // lblCurrProteinCount
            //
            lblCurrProteinCount.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            lblCurrProteinCount.Location = new Point(20, 23);
            lblCurrProteinCount.Name = "lblCurrProteinCount";
            lblCurrProteinCount.Size = new Size(229, 18);
            lblCurrProteinCount.TabIndex = 2;
            lblCurrProteinCount.Text = "Protein Count: 0";
            //
            // lvwDestination
            //
            lvwDestination.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwDestination.Columns.AddRange(new ColumnHeader[] { colName });
            lvwDestination.FullRowSelect = true;
            lvwDestination.GridLines = true;
            lvwDestination.Location = new Point(20, 64);
            lvwDestination.Name = "lvwDestination";
            lvwDestination.Size = new Size(302, 296);
            lvwDestination.TabIndex = 0;
            lvwDestination.UseCompatibleStateImageBehavior = false;
            lvwDestination.View = View.Details;
            //
            // colName
            //
            colName.Text = "Name";
            colName.Width = 228;
            //
            // mnuMainGUI
            //
            mnuMainGUI.MenuItems.AddRange(new MenuItem[] { mnuFile, mnuTools, mnuAdmin, mnuHelp });
            //
            // mnuFile
            //
            mnuFile.Index = 0;
            mnuFile.MenuItems.AddRange(new MenuItem[] { mnuFileExit });
            mnuFile.Text = "&File";
            //
            // mnuFileExit
            //
            mnuFileExit.Index = 0;
            mnuFileExit.Text = "E&xit";
            //
            // mnuTools
            //
            mnuTools.Index = 1;
            mnuTools.MenuItems.AddRange(new MenuItem[] { mnuToolsCollectionEdit, mnuToolsNucToProt, mnuToolsConvert, mnuToolsFCheckup, mnuToolsCompareDBs, mnuToolsExtractFromFile, mnuToolsSep1, mnuToolsOptions });
            mnuTools.Text = "&Tools";
            //
            // mnuToolsCollectionEdit
            //
            mnuToolsCollectionEdit.Index = 0;
            mnuToolsCollectionEdit.Text = "&Edit Collection States...";
            //
            // mnuToolsNucToProt
            //
            mnuToolsNucToProt.Index = 1;
            mnuToolsNucToProt.Text = "Translate Nucleotides to Proteins...";
            mnuToolsNucToProt.Visible = false;
            //
            // mnuToolsConvert
            //
            mnuToolsConvert.Index = 2;
            mnuToolsConvert.MenuItems.AddRange(new MenuItem[] { mnuToolsConvertF2A, mnuToolsConvertA2F });
            mnuToolsConvert.Text = "Convert Database Format...";
            mnuToolsConvert.Visible = false;
            //
            // mnuToolsConvertF2A
            //
            mnuToolsConvertF2A.Index = 0;
            mnuToolsConvertF2A.Text = "FASTA to Access...";
            //
            // mnuToolsConvertA2F
            //
            mnuToolsConvertA2F.Index = 1;
            mnuToolsConvertA2F.Text = "Access to FASTA...";
            //
            // mnuToolsFCheckup
            //
            mnuToolsFCheckup.Index = 3;
            mnuToolsFCheckup.Text = "Perform FASTA File Checkup...";
            mnuToolsFCheckup.Visible = false;
            //
            // mnuToolsCompareDBs
            //
            mnuToolsCompareDBs.Index = 4;
            mnuToolsCompareDBs.Text = "Compare Databases...";
            mnuToolsCompareDBs.Visible = false;
            //
            // mnuToolsExtractFromFile
            //
            mnuToolsExtractFromFile.Enabled = false;
            mnuToolsExtractFromFile.Index = 5;
            mnuToolsExtractFromFile.Text = "Extract Annotations from Text File...";
            //
            // mnuToolsSep1
            //
            mnuToolsSep1.Index = 6;
            mnuToolsSep1.Text = "-";
            //
            // mnuToolsOptions
            //
            mnuToolsOptions.Enabled = false;
            mnuToolsOptions.Index = 7;
            mnuToolsOptions.Text = "Options...";
            //
            // mnuAdmin
            //
            mnuAdmin.Index = 2;
            mnuAdmin.MenuItems.AddRange(new MenuItem[] { mnuAdminBatchUploadFiles, mnuAdminNameHashRefresh, mnuAdminUpdateSHA, mnuAdminUpdateCollectionsArchive, mnuAdminUpdateZeroedMasses, mnuAdminTestingInterface, mnuAdminFixArchivePaths, mnuAdminAddSortingIndexes });
            mnuAdmin.Text = "Admin";
            //
            // mnuAdminBatchUploadFiles
            //
            mnuAdminBatchUploadFiles.Index = 0;
            mnuAdminBatchUploadFiles.Text = "Batch Upload FASTA Files Using DMS...";
            mnuAdminBatchUploadFiles.Visible = false;
            //
            // mnuAdminNameHashRefresh
            //
            mnuAdminNameHashRefresh.Index = 1;
            mnuAdminNameHashRefresh.Text = "Refresh Protein Name Hashes";
            //
            // mnuAdminUpdateSHA
            //
            mnuAdminUpdateSHA.Enabled = false;
            mnuAdminUpdateSHA.Index = 2;
            mnuAdminUpdateSHA.Text = "Update File Authentication Hashes";
            mnuAdminUpdateSHA.Visible = false;
            //
            // mnuAdminUpdateCollectionsArchive
            //
            mnuAdminUpdateCollectionsArchive.Enabled = false;
            mnuAdminUpdateCollectionsArchive.Index = 3;
            mnuAdminUpdateCollectionsArchive.Text = "Update Collections Archive";
            mnuAdminUpdateCollectionsArchive.Visible = false;
            //
            // mnuAdminUpdateZeroedMasses
            //
            mnuAdminUpdateZeroedMasses.Index = 4;
            mnuAdminUpdateZeroedMasses.Text = "Update Zeroed Masses";
            mnuAdminUpdateZeroedMasses.Visible = false;
            //
            // mnuAdminTestingInterface
            //
            mnuAdminTestingInterface.Index = 5;
            mnuAdminTestingInterface.Text = "Testing Interface Window...";
            //
            // mnuAdminFixArchivePaths
            //
            mnuAdminFixArchivePaths.Enabled = false;
            mnuAdminFixArchivePaths.Index = 6;
            mnuAdminFixArchivePaths.Text = "Fix Archive Path Names";
            mnuAdminFixArchivePaths.Visible = false;
            //
            // mnuAdminAddSortingIndexes
            //
            mnuAdminAddSortingIndexes.Index = 7;
            mnuAdminAddSortingIndexes.Text = "Add Sorting Indexes";
            mnuAdminAddSortingIndexes.Visible = false;
            //
            // mnuHelp
            //
            mnuHelp.Index = 3;
            mnuHelp.MenuItems.AddRange(new MenuItem[] { mnuHelpAbout });
            mnuHelp.Text = "&Help";
            //
            // mnuHelpAbout
            //
            mnuHelpAbout.Index = 0;
            mnuHelpAbout.Text = "&About Protein Collection Editor";
            //
            // frmCollectionEditor
            //
            AutoScaleBaseSize = new Size(7, 17);
            ClientSize = new Size(1130, 524);
            Controls.Add(pnlDest);
            Controls.Add(SourceDestSplit);
            Controls.Add(pnlSource);
            Controls.Add(pnlProgBar);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            Icon = (Icon)resources.GetObject("$this.Icon");
            Menu = mnuMainGUI;
            MinimumSize = new Size(1148, 550);
            Name = "frmCollectionEditor";
            Text = "Protein Collection Editor";
            pnlProgBar.ResumeLayout(false);
            pnlProgBarUpper.ResumeLayout(false);
            pnlProgBarLower.ResumeLayout(false);
            pnlSource.ResumeLayout(false);
            gbxSourceCollection.ResumeLayout(false);
            gbxSourceCollection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbxLiveSearchCancel).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbxLiveSearchBkg).EndInit();
            pnlDest.ResumeLayout(false);
            gbxDestinationCollection.ResumeLayout(false);
            ResumeLayout(false);
        }

        [STAThread()]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new frmCollectionEditor());
        }


        #endregion

        private const string PROGRAM_DATE = "February 18, 2020";

        private DataTable m_Organisms;
        private DataTable m_ProteinCollections;
        private DataTable m_ProteinCollectionNames;
        private DataTable m_AnnotationTypes;
        private DataTable m_CollectionMembers;
        private int m_SelectedOrganismID;
        private int m_SelectedAnnotationTypeID;
        private string m_SelectedFilePath;
        private int m_SelectedCollectionID;
        private string m_LastBatchULDirectoryPath;

        /// <summary>
        /// Protein sequences database connection string
        /// </summary>
        private string m_PSConnectionString = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;";

        private string m_LastSelectedOrganism = "";
        private string m_LastSelectedAnnotationType = "";
        private bool m_LastValueForAllowAsterisks = false;
        private bool m_LastValueForAllowDash = false;
        private int m_LastValueForMaxProteinNameLength = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
        private ImportHandler m_ImportHandler;
        private PSUploadHandler m_UploadHandler;
        private DataListViewHandler m_SourceListViewHandler;
        // Unused: private BatchUploadFromFileList m_fileBatcher;

        private bool m_LocalFileLoaded;

        private bool m_SearchActive = false;

        private int m_BatchLoadTotalCount;
        private int m_BatchLoadCurrentCount;

        /// <summary>
        /// Keys are fasta file names, values are lists of errors
        /// </summary>
        private Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> m_FileErrorList;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of error messages, tracking the count of each error
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> m_SummarizedFileErrorList;

        /// <summary>
        /// Keys are fasta file names, values are lists of warnings
        /// </summary>
        private Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> m_FileWarningList;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of warning messages, tracking the count of each warning
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> m_SummarizedFileWarningList;

        /// <summary>
        /// Keys are FASTA file paths
        /// Values are upload info
        /// </summary>
        private Dictionary<string, PSUploadHandler.UploadInfo> m_ValidUploadsList;

        private SyncFASTAFileArchive m_Syncer;
        private readonly bool m_EncryptSequences = false;
        internal readonly System.Timers.Timer SearchTimer;
        internal readonly System.Timers.Timer MemberLoadTimer;

        /// <summary>
        /// Tracks the description and source that the user has entered for each FASTA file
        /// Key: fasta file name
        /// Value: KeyValuePair of Description and Source
        /// </summary>
        /// <remarks>Useful in case validation fails and the uploader needs to try again to upload a FASTA file</remarks>
        private readonly Dictionary<string, KeyValuePair<string, string>> m_CachedFileDescriptions;

        private void frmCollectionEditor_Load(object sender, EventArgs e)
        {
            // Get initial info - organism list, full collections list

            // Data Source=proteinseqs;Initial Catalog=Protein_Sequences
            string connectionString = Settings.Default.ProteinSeqsDBConnectStr;

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                m_PSConnectionString = connectionString;
            }

            UpdateServerNameLabel();

            m_ImportHandler = new ImportHandler(m_PSConnectionString);
            m_ImportHandler.LoadStart += ImportStartHandler;
            m_ImportHandler.LoadProgress += ImportProgressHandler;
            m_ImportHandler.LoadEnd += ImportEndHandler;
            m_ImportHandler.CollectionLoadComplete += CollectionLoadHandler;
            //mnuToolsFBatchUpload.Enabled = false;

            lblBatchProgress.Text = "Fetching Organism and Collection Lists...";

            cboCollectionPicker.Enabled = false;
            cboOrganismFilter.Enabled = false;

            TriggerCollectionTableUpdate(false);

            m_SourceListViewHandler = new DataListViewHandler(lvwSource);

            cmdLoadProteins.Enabled = false;
            txtLiveSearch.Visible = false;
            pbxLiveSearchBkg.Visible = false;
            pbxLiveSearchCancel.Visible = false;
            lblSearchCount.Text = "";
            cmdExportToFile.Enabled = false;
            cmdSaveDestCollection.Enabled = false;

            cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
            lblBatchProgress.Text = "";

            CheckTransferButtonsEnabledStatus();

            // Setup collections for selected organism

            // Use 2-3 second delay after collection change before refreshing member list
        }

        private void CheckTransferButtonsEnabledStatus()
        {
            if (lvwSource.Items.Count > 0)
            {
                cmdDestAdd.Enabled = true;
                cmdDestAddAll.Enabled = true;
            }
            else
            {
                cmdDestAdd.Enabled = false;
                cmdDestAddAll.Enabled = false;
            }

            if (lvwDestination.Items.Count > 0)
            {
                cmdDestRemove.Enabled = true;
                cmdDestRemoveAll.Enabled = true;
            }
            else
            {
                cmdDestRemove.Enabled = false;
                cmdDestRemoveAll.Enabled = false;
            }
        }

        private void RefreshCollectionList()
        {
            if (m_SelectedOrganismID != -1 & m_SelectedCollectionID != -1)
            {
                cboAnnotationTypePicker.SelectedIndexChanged -= cboAnnotationTypePicker_SelectedIndexChanged;
                cboCollectionPicker.SelectedIndexChanged -= cboCollectionPicker_SelectedIndexChanged;
                cboOrganismFilter.SelectedItem = m_SelectedOrganismID;
                cboOrganismList_SelectedIndexChanged(this, null);

                cboCollectionPicker.SelectedItem = m_SelectedCollectionID;
                cboAnnotationTypePicker.SelectedItem = m_SelectedAnnotationTypeID;
                cboCollectionPicker.Select();
                cboCollectionPicker.SelectedIndexChanged += cboCollectionPicker_SelectedIndexChanged;
                cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
            }
        }

        private void TriggerCollectionTableUpdate(bool RefreshTable)
        {
            if (RefreshTable)
            {
                m_ImportHandler.TriggerProteinCollectionTableUpdate();
            }
            // CollectionLoadThread = New System.Threading.Thread(AddressOf m_ImportHandler.TriggerProteinCollectionsLoad)
            // CollectionLoadThread.Start()
            if (m_SelectedOrganismID > 0)
            {
                m_ImportHandler.TriggerProteinCollectionsLoad(m_SelectedOrganismID);
            }
            else
            {
                m_ImportHandler.TriggerProteinCollectionsLoad();
            }
        }

        private void BindOrganismListToControl(DataTable organismList)
        {
            cboOrganismFilter.BeginUpdate();
            cboOrganismFilter.DataSource = organismList;
            cboOrganismFilter.DisplayMember = "Display_Name";
            cboOrganismFilter.ValueMember = "ID";

            cboOrganismFilter.EndUpdate();
        }

        private void BindAnnotationTypeListToControl(DataTable annotationTypeList)
        {
            cboAnnotationTypePicker.BeginUpdate();
            cboAnnotationTypePicker.DisplayMember = "Display_Name";
            // cboAnnotationTypePicker.DisplayMember = "name";
            cboAnnotationTypePicker.ValueMember = "ID";
            cboAnnotationTypePicker.DataSource = annotationTypeList;
            cboAnnotationTypePicker.Refresh();

            cboAnnotationTypePicker.EndUpdate();
        }

        private void BindCollectionListToControl(ICollection collectionList)
        {
            cboCollectionPicker.BeginUpdate();
            if (collectionList.Count == 0)
            {
                cboCollectionPicker.DataSource = null;
                cboCollectionPicker.Items.Add(" -- No Collections for this Organism -- ");
                cboCollectionPicker.SelectedIndex = 0;
                cboCollectionPicker.Enabled = false;

                cmdLoadProteins.Enabled = false;
                txtLiveSearch.Visible = false;
                pbxLiveSearchBkg.Visible = false;
                pbxLiveSearchCancel.Visible = false;
            }
            else
            {
                cboCollectionPicker.Enabled = true;
                cboCollectionPicker.DataSource = collectionList;
                cboCollectionPicker.DisplayMember = "Display";
                cboCollectionPicker.ValueMember = "Protein_Collection_ID";

                cmdLoadProteins.Enabled = true;
            }

            cboCollectionPicker.EndUpdate();
        }

        private void BatchLoadController()
        {
            DialogResult resultReturn;

            m_ProteinCollectionNames = m_ImportHandler.LoadProteinCollectionNames();
            if (m_FileErrorList != null)
            {
                m_FileErrorList.Clear();
            }

            if (m_FileWarningList != null)
            {
                m_FileWarningList.Clear();
            }

            if (m_ValidUploadsList != null)
            {
                m_ValidUploadsList.Clear();
            }

            if (m_SummarizedFileErrorList != null)
            {
                m_SummarizedFileErrorList.Clear();
            }

            if (m_SummarizedFileWarningList != null)
            {
                m_SummarizedFileWarningList.Clear();
            }

            var frmBatchUpload = new frmBatchAddNewCollection(
                m_Organisms,
                m_AnnotationTypes,
                m_ProteinCollectionNames,
                m_PSConnectionString,
                m_LastBatchULDirectoryPath,
                m_CachedFileDescriptions);

            List<PSUploadHandler.UploadInfo> tmpSelectedFileList;

            lblBatchProgress.Text = "";

            if (m_LastSelectedOrganism != null && m_LastSelectedOrganism.Length > 0)
            {
                frmBatchUpload.SelectedOrganismName = m_LastSelectedOrganism;
            }

            if (m_LastSelectedAnnotationType != null && m_LastSelectedAnnotationType.Length > 0)
            {
                frmBatchUpload.SelectedAnnotationType = m_LastSelectedAnnotationType;
            }

            frmBatchUpload.ValidationAllowAsterisks = m_LastValueForAllowAsterisks;
            frmBatchUpload.ValidationAllowDash = m_LastValueForAllowDash;
            frmBatchUpload.ValidationMaxProteinNameLength = m_LastValueForMaxProteinNameLength;

            // Show the window
            resultReturn = frmBatchUpload.ShowDialog();

            // Save the selected organism and annotation type
            m_LastSelectedOrganism = frmBatchUpload.SelectedOrganismName;
            m_LastSelectedAnnotationType = frmBatchUpload.SelectedAnnotationType;
            m_LastValueForAllowAsterisks = frmBatchUpload.ValidationAllowAsterisks;
            m_LastValueForAllowDash = frmBatchUpload.ValidationAllowDash;
            m_LastValueForMaxProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength;

            m_LastBatchULDirectoryPath = frmBatchUpload.CurrentDirectory;

            try
            {
                // Save these settings to the registry
                if (!string.IsNullOrEmpty(m_LastSelectedOrganism))
                {
                    Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", m_LastSelectedOrganism);
                }

                if (!string.IsNullOrEmpty(m_LastSelectedAnnotationType))
                {
                    Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", m_LastSelectedAnnotationType);
                }

                if (!string.IsNullOrEmpty(m_LastBatchULDirectoryPath))
                {
                    Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", m_LastBatchULDirectoryPath);
                }
            }
            catch (Exception ex)
            {
                // Ignore errors here
            }

            if (resultReturn != DialogResult.OK)
                return;

            gbxSourceCollection.Enabled = false;
            gbxDestinationCollection.Enabled = false;
            cmdDestAdd.Enabled = false;
            cmdDestAddAll.Enabled = false;
            cmdDestRemove.Enabled = false;
            cmdDestRemoveAll.Enabled = false;

            tmpSelectedFileList = frmBatchUpload.FileList;

            m_BatchLoadTotalCount = tmpSelectedFileList.Count;

            if (m_UploadHandler != null)
            {
                m_UploadHandler.BatchProgress -= BatchImportProgressHandler;
                m_UploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                m_UploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                m_UploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                m_UploadHandler.ValidationProgress -= ValidationProgressHandler;
                m_UploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
            }

            if (m_EncryptSequences)
            {
                m_UploadHandler = new PSUploadHandler(m_PSConnectionString);
            }
            else
            {
                m_UploadHandler = new PSUploadHandler(m_PSConnectionString);
            }

            m_UploadHandler.BatchProgress += BatchImportProgressHandler;
            m_UploadHandler.ValidFASTAFileLoaded += ValidFASTAUploadHandler;
            m_UploadHandler.InvalidFASTAFile += InvalidFASTAFileHandler;
            m_UploadHandler.FASTAFileWarnings += FASTAFileWarningsHandler;
            m_UploadHandler.ValidationProgress += ValidationProgressHandler;
            m_UploadHandler.WroteLineEndNormalizedFASTA += NormalizedFASTAFileGenerationHandler;

            pnlProgBar.Visible = true;
            try
            {
                m_UploadHandler.SetValidationOptions(PSUploadHandler.eValidationOptionConstants.AllowAllSymbolsInProteinNames, frmBatchUpload.ValidationAllowAllSymbolsInProteinNames);
                m_UploadHandler.SetValidationOptions(PSUploadHandler.eValidationOptionConstants.AllowAsterisksInResidues, frmBatchUpload.ValidationAllowAsterisks);
                m_UploadHandler.SetValidationOptions(PSUploadHandler.eValidationOptionConstants.AllowDashInResidues, frmBatchUpload.ValidationAllowDash);

                m_UploadHandler.MaximumProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength;

                m_UploadHandler.BatchUpload(tmpSelectedFileList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error uploading collection: " + ex.Message, "Error");
            }

            pnlProgBar.Visible = false;

            // Display any errors that occurred
            var errorDisplay = new frmValidationReport();
            errorDisplay.FileErrorList = m_FileErrorList;
            errorDisplay.FileWarningList = m_FileWarningList;
            errorDisplay.FileValidList = m_ValidUploadsList;
            errorDisplay.ErrorSummaryList = m_SummarizedFileErrorList;
            errorDisplay.WarningSummaryList = m_SummarizedFileWarningList;
            errorDisplay.OrganismList = m_Organisms;
            errorDisplay.ShowDialog();

            lblBatchProgress.Text = "Updating Protein Collections List...";
            Application.DoEvents();

            TriggerCollectionTableUpdate(true);

            RefreshCollectionList();
            m_UploadHandler.ResetErrorList();

            lblBatchProgress.Text = "";
            gbxSourceCollection.Enabled = true;
            gbxDestinationCollection.Enabled = true;
            cmdDestAdd.Enabled = true;
            cmdDestAddAll.Enabled = true;
            cmdDestRemove.Enabled = true;
            cmdDestRemoveAll.Enabled = true;

            m_BatchLoadCurrentCount = 0;
        }

        private void ReadSettings()
        {
            try
            {
                m_LastSelectedOrganism = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", "");
                m_LastSelectedAnnotationType = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", "");
                m_LastBatchULDirectoryPath = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", "");
            }
            catch (Exception ex)
            {
                // Ignore errors here
            }
        }

        private void ShowAboutBox()
        {
            // Dim AboutBox As New frmAboutBox

            // AboutBox.Location = m_MainProcess.myAppSettings.AboutBoxLocation
            // AboutBox.ShowDialog()

            string strMessage;

            strMessage = "This is version " + Application.ProductVersion + ", " + PROGRAM_DATE;

            MessageBox.Show(strMessage, "About Protein Collection Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateServerNameLabel()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(m_PSConnectionString))
                {
                    lblTargetServer.Text = "ERROR determining target server: m_PSConnectionString is empty";
                    return;
                }

                var reExtractServerName = new Regex(@"Data Source\s*=\s*([^\s;]+)", RegexOptions.IgnoreCase);
                var reMatch = reExtractServerName.Match(m_PSConnectionString);

                if (reMatch.Success)
                {
                    lblTargetServer.Text = "Target server: " + reMatch.Groups[1].Value;
                }
                else
                {
                    lblTargetServer.Text = "Target server: UNKNOWN -- name not found in " + m_PSConnectionString;
                }
            }
            catch (Exception ex)
            {
                lblTargetServer.Text = "ERROR determining target server: " + ex.Message;
            }
        }

        #region "Combobox handlers"

        private void cboOrganismList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Conversions.ToInteger(cboOrganismFilter.SelectedValue) != 0)
            {
                m_ProteinCollections.DefaultView.RowFilter = "[Organism_ID] = " + cboOrganismFilter.SelectedValue.ToString();
            }
            else
            {
                m_ProteinCollections.DefaultView.RowFilter = "";
            }

            m_SelectedOrganismID = Conversions.ToInteger(cboOrganismFilter.SelectedValue);

            BindCollectionListToControl(m_ProteinCollections.DefaultView);

            if (lvwSource.Items.Count == 0)
            {
                cboCollectionPicker_SelectedIndexChanged(this, null);
            }
        }

        private void cboCollectionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvwSource.Items.Clear();
            m_ImportHandler.ClearProteinCollection();
            m_SelectedCollectionID = Conversions.ToInteger(cboCollectionPicker.SelectedValue);

            if (m_SelectedCollectionID > 0)
            {
                var foundRows = m_ProteinCollections.Select("[Protein_Collection_ID] = " + m_SelectedCollectionID.ToString());
                m_SelectedAnnotationTypeID = Conversions.ToInteger(foundRows[0]["Authority_ID"]);
                //m_AnnotationTypes = m_ImportHandler.LoadAnnotationTypes(m_SelectedCollectionID);
                //m_AnnotationTypes = m_ImportHandler.LoadAnnotationTypes();
                cmdLoadProteins.Enabled = true;
            }
            else
            {
                m_AnnotationTypes = m_ImportHandler.LoadAnnotationTypes();
                cmdLoadProteins.Enabled = false;
            }

            BindAnnotationTypeListToControl(m_AnnotationTypes);
        }

        private void cboAnnotationTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSource.Items.Count > 0)
            {
                lvwSource.Items.Clear();
                m_ImportHandler.ClearProteinCollection();
            }

            if (ReferenceEquals(cboAnnotationTypePicker.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                m_SelectedAnnotationTypeID = Conversions.ToInteger(cboAnnotationTypePicker.SelectedValue);
            }
            else
            {
                //m_SelectedAuthorityID = 0;
            }

            if (m_SelectedCollectionID > 0)
            {
                var foundRows = m_ProteinCollections.Select("[Protein_Collection_ID] = " + m_SelectedCollectionID.ToString());
                m_SelectedAnnotationTypeID = Conversions.ToInteger(foundRows[0]["Authority_ID"]);
            }
            //else if (m_SelectedAuthorityID == -2)
            //{
            //    //Bring up addition dialog
            //    var AuthAdd = new AddNamingAuthority(m_PSConnectionString);
            //    tmpAuthID = AuthAdd.AddNamingAuthority;
            //    m_Authorities = m_ImportHandler.LoadAuthorities();
            //    BindAuthorityListToControl(m_Authorities);
            //    m_SelectedAuthorityID = tmpAuthID;
            //    cboAuthorityPicker.SelectedValue = tmpAuthID;
            //}
        }

        #endregion

        #region "Action Button Handlers"

        private void cmdLoadProteins_Click(object sender, EventArgs e)
        {
            ImportStartHandler("Retrieving Protein Entries..");
            var foundRows =
                m_ProteinCollections.Select("Protein_Collection_ID = " + cboCollectionPicker.SelectedValue.ToString());
            ImportProgressHandler(0.5d);
            m_SelectedFilePath = foundRows[0]["FileName"].ToString();
            MemberLoadTimerHandler(this, null);
            ImportProgressHandler(1.0d);
            txtLiveSearch.Visible = true;
            pbxLiveSearchBkg.Visible = true;
            ImportEndHandler();
        }

        private void cmdLoadFile_Click(object sender, EventArgs e)
        {
            BatchLoadController();
        }

        private void cmdSaveDestCollection_Click(object sender, EventArgs e)
        {
            //DialogResult resultReturn

            //var frmAddCollection = new frmAddNewCollection();
            //int tmpOrganismID;
            //int tmpAnnotationTypeID;
            //List<string> tmpSelectedProteinList;

            if (lvwDestination.Items.Count <= 0)
            {
                if (m_UploadHandler != null)
                {
                    m_UploadHandler.BatchProgress -= BatchImportProgressHandler;
                    m_UploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                    m_UploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                    m_UploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                    m_UploadHandler.ValidationProgress -= ValidationProgressHandler;
                    m_UploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
                }

                m_UploadHandler = null;
                return;
            }

            var frmAddCollection = new frmAddNewCollection()
            {
                CollectionName = Path.GetFileNameWithoutExtension(m_SelectedFilePath),
                IsLocalFile = m_LocalFileLoaded,
                AnnotationTypes = m_AnnotationTypes,
                OrganismList = m_Organisms,
                OrganismID = m_SelectedOrganismID,
                AnnotationTypeID = m_SelectedAnnotationTypeID
            };

            var eResult = frmAddCollection.ShowDialog();

            if (eResult == DialogResult.OK)
            {
                cboCollectionPicker.Enabled = true;
                cboOrganismFilter.Enabled = true;

                int tmpOrganismID = frmAddCollection.OrganismID;
                int tmpAnnotationTypeID = frmAddCollection.AnnotationTypeID;

                var tmpSelectedProteinList = ScanDestinationCollectionWindow(lvwDestination);

                if (m_UploadHandler == null)
                {
                    m_UploadHandler = new PSUploadHandler(m_PSConnectionString);
                    m_UploadHandler.BatchProgress += BatchImportProgressHandler;
                    m_UploadHandler.ValidFASTAFileLoaded += ValidFASTAUploadHandler;
                    m_UploadHandler.InvalidFASTAFile += InvalidFASTAFileHandler;
                    m_UploadHandler.FASTAFileWarnings += FASTAFileWarningsHandler;
                    m_UploadHandler.ValidationProgress += ValidationProgressHandler;
                    m_UploadHandler.WroteLineEndNormalizedFASTA += NormalizedFASTAFileGenerationHandler;
                }

                m_UploadHandler.UploadCollection(m_ImportHandler.CollectionMembers, tmpSelectedProteinList,
                                                 frmAddCollection.CollectionName, frmAddCollection.CollectionDescription,
                                                 frmAddCollection.CollectionSource,
                                                 AddUpdateEntries.CollectionTypes.prot_original_source, tmpOrganismID,
                                                 tmpAnnotationTypeID);

                RefreshCollectionList();

                ClearFromDestinationCollectionWindow(lvwDestination, true);

                cboOrganismFilter.Enabled = true;
                cboCollectionPicker.Enabled = true;
                cboOrganismFilter.SelectedValue = tmpOrganismID;
            }

            if (m_UploadHandler != null)
            {
                m_UploadHandler.BatchProgress -= BatchImportProgressHandler;
                m_UploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                m_UploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                m_UploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                m_UploadHandler.ValidationProgress -= ValidationProgressHandler;
                m_UploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
            }

            m_UploadHandler = null;
        }

        //private void cmdExportToFile_Click(System.Object sender, System.EventArgs e)
        //{
        //    SaveFileDialog SaveDialog = new SaveFileDialog();
        //    Protein_Importer.IImportProteins.ProteinImportFileTypes fileType;
        //    string SelectedSavePath;
        //    ArrayList tmpSelectedProteinList;
        //    Protein_Storage.ProteinStorage tmpProteinCollection;

        //    SaveDialog.Title = "Save Protein Database File";
        //    SaveDialog.DereferenceLinks = true;
        //    SaveDialog.Filter = "FASTA Files (*.fasta)|*.fasta|Microsoft Access Databases (*.mdb)|*.mdb|All Files (*.*)|*.*";
        //    SaveDialog.FilterIndex = 1;
        //    SaveDialog.RestoreDirectory = true;
        //    SaveDialog.OverwritePrompt = true;e
        //    //SaveDialog.FileName = m_ImportHandler.CollectionMembers.FileName;

        //    if (SaveDialog.ShowDialog == DialogResult.OK)
        //        SelectedSavePath = SaveDialog.FileName;
        //    else
        //        return;

        //    if (System.IO.Path.GetExtension(m_SelectedFilePath) == ".fasta")
        //        fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA;
        //    else if (System.IO.Path.GetExtension(m_SelectedFilePath) == ".mdb")
        //        fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.Access;

        //    if (fileType == Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA)
        //        m_ProteinExporter = new Protein_Exporter.ExportProteinsFASTA();
        //    else
        //        return;

        //    tmpProteinCollection = new Protein_Storage.ProteinStorage(SelectedSavePath);

        //    tmpSelectedProteinList = ScanDestinationCollectionWindow(lvwDestination);

        //    foreach (var tmpProteinReference in tmpSelectedProteinList)
        //        tmpProteinCollection.AddProtein(m_ImportHandler.CollectionMembers.GetProtein(tmpProteinReference));

        //    m_ProteinExporter.Export(m_ImportHandler.CollectionMembers, SelectedSavePath);
        //}

        #endregion

        #region "LiveSearch Handlers"

        private void txtLiveSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length > 0 & txtLiveSearch.ForeColor.ToString() != "Color [InactiveCaption]")
            {
                SearchTimer.Start();
            }
            else if (string.IsNullOrEmpty(txtLiveSearch.Text) & m_SearchActive == false)
            {
                pbxLiveSearchCancel_Click(this, null);
            }
            else
            {
                m_SearchActive = false;
                SearchTimer.Stop();
                //txtLiveSearch.Text = "Search";
                //txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            }
        }

        private void txtLiveSearch_Click(object sender, EventArgs e)
        {
            if (m_SearchActive)
            {
            }
            else
            {
                txtLiveSearch.TextChanged -= txtLiveSearch_TextChanged;
                txtLiveSearch.Text = null;
                txtLiveSearch.ForeColor = SystemColors.ControlText;
                m_SearchActive = true;
                pbxLiveSearchCancel.Visible = true;
                txtLiveSearch.TextChanged += txtLiveSearch_TextChanged;
                //Debug.WriteLine("inactive.click");
            }
        }

        private void txtLiveSearch_Leave(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length == 0)
            {
                txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
                txtLiveSearch.Text = "Search";
                m_SearchActive = false;
                SearchTimer.Stop();
                m_SourceListViewHandler.Load(m_CollectionMembers);
            }
        }

        private void pbxLiveSearchCancel_Click(object sender, EventArgs e)
        {
            txtLiveSearch.Text = "";
            txtLiveSearch_Leave(this, null);
            lvwSource.Focus();
            pbxLiveSearchCancel.Visible = false;
        }

        internal void SearchTimerHandler(
            object sender,
            ElapsedEventArgs e)
        {
            if (m_SearchActive == true)
            {
                //Debug.WriteLine("SearchTimer.active.kick");

                m_SourceListViewHandler.Load(m_CollectionMembers, txtLiveSearch.Text);
                m_SearchActive = false;
                SearchTimer.Stop();
            }
            else
            {
                //Debug.WriteLine("SearchTimer.inactive.kick");

            }
        }


        #endregion

        #region "ListView Event Handlers"

        private void lvwSource_DoubleClick(
            object sender,
            EventArgs e)
        {
            ScanSourceCollectionWindow(lvwSource, lvwDestination, false);
        }

        // Double click to remove selected member from the destination collection
        private void lvwDestination_DoubleClick(
            object sender,
            EventArgs e)
        {
            ClearFromDestinationCollectionWindow(lvwDestination, false);
        }

        internal void MemberLoadTimerHandler(
            object sender,
            ElapsedEventArgs e)
        {
            m_SelectedCollectionID = Conversions.ToInteger(cboCollectionPicker.SelectedValue);
            m_SelectedAnnotationTypeID = Conversions.ToInteger(cboAnnotationTypePicker.SelectedValue);

            m_CollectionMembers = m_ImportHandler.LoadCollectionMembersByID(m_SelectedCollectionID, m_SelectedAnnotationTypeID);
            m_LocalFileLoaded = false;

            //m_SelectedAuthorityID = m_ImportHandler.

            m_SourceListViewHandler.Load(m_CollectionMembers);
            lvwSource.Focus();
            lvwSource.Enabled = true;

            //MemberLoadTimer.Stop();

        }

        //Update the selected collection
        //private void lvwSource_SelectedIndexChanged(
        //    object sender,
        //    EventArgs e)
        //{
        //}

        //private void lvwDestination_SelectedIndexChanged(
        //    object sender,
        //    EventArgs e)
        //{
        //}

        //DoubleClick to Move the selected member to the destination collection

        private void cmdDestAddAll_Click(object sender, EventArgs e)
        {
            ScanSourceCollectionWindow(lvwSource, lvwDestination, true);
            CheckTransferButtonsEnabledStatus();
        }

        private void cmdDestAdd_Click(object sender, EventArgs e)
        {
            ScanSourceCollectionWindow(lvwSource, lvwDestination, false);
            CheckTransferButtonsEnabledStatus();
        }

        private void cmdDestRemove_Click(object sender, EventArgs e)
        {
            ClearFromDestinationCollectionWindow(lvwDestination, false);
            CheckTransferButtonsEnabledStatus();
        }

        private void cmdDestRemoveAll_Click(object sender, EventArgs e)
        {
            ClearFromDestinationCollectionWindow(lvwDestination, true);
            CheckTransferButtonsEnabledStatus();
        }

        private void ScanSourceCollectionWindow(ListView lvwSrc, ListView lvwDest, bool SelectAll)
        {
            ListViewItem entry;

            if (SelectAll)
            {
                foreach (ListViewItem currentEntry in lvwSrc.Items)
                {
                    entry = currentEntry;
                    // Need to figure out some way to check for duplicates (maybe just at upload time)
                    lvwDest.Items.Add(entry.Text);
                }
            }
            else
            {
                foreach (ListViewItem currentEntry1 in lvwSrc.SelectedItems)
                {
                    entry = currentEntry1;
                    // Need to figure out some way to check for duplicates (maybe just at upload time)
                    lvwDest.Items.Add(entry.Text);
                }
            }

            lblCurrProteinCount.Text = "Protein Count: " + lvwDest.Items.Count;
            cmdExportToFile.Enabled = true;
            cmdSaveDestCollection.Enabled = true;
        }

        private List<string> ScanDestinationCollectionWindow(ListView lvwDest)
        {
            var selectedList = new List<string>();

            foreach (ListViewItem li in lvwDest.Items)
                selectedList.Add(li.Text);

            return selectedList;
        }

        private void ClearFromDestinationCollectionWindow(ListView lvwDest, bool SelectAll)
        {
            if (SelectAll)
            {
                lvwDest.Items.Clear();
                cmdSaveDestCollection.Enabled = false;
                cmdExportToFile.Enabled = false;
            }
            else
            {
                foreach (ListViewItem entry in lvwDest.SelectedItems)
                    entry.Remove();
            }

            lblCurrProteinCount.Text = "Protein Count: " + lvwDest.Items.Count;
        }

        #endregion

        #region "Menu Option Handlers"

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Unused
        //private void mnuToolsFBatchUpload_Click(object sender, EventArgs e)
        //{
        //    //Steal this to use with file-directed loading
        //    m_fileBatcher = new BatchUploadFromFileList(m_PSConnectionString);
        //    m_fileBatcher.UploadBatch();
        //}

        private void mnuToolsCollectionEdit_Click(object sender, EventArgs e)
        {
            var cse = new frmCollectionStateEditor(m_PSConnectionString);
            cse.ShowDialog();
        }

        private void mnuToolsExtractFromFile_Click(object sender, EventArgs e)
        {
            var f = new frmExtractFromFlatfile(m_ImportHandler.Authorities, m_PSConnectionString);
            f.ShowDialog();
        }

        // Unused
        //private void mnuToolsUpdateArchives_Click(object sender, EventArgs e)
        //{
        //    var f = new FolderBrowserDialog();
        //    string outputPath = "";

        //    if (m_Syncer == null)
        //        m_Syncer = new SyncFASTAFileArchive(m_PSConnectionString);

        //    f.RootFolder = Environment.SpecialFolder.MyComputer;
        //    f.ShowNewFolderButton = true;

        //    var r = f.ShowDialog();

        //    if (r == DialogResult.OK)
        //    {
        //        outputPath = f.SelectedPath;
        //        int errorCode = m_Syncer.SyncCollectionsAndArchiveTables(outputPath);
        //    }
        //}

        #endregion

        #region "Progress Event Handlers"
        private void ImportStartHandler(string taskTitle)
        {
            //m_fileBatcher.LoadStart();

            pnlProgBar.Visible = true;
            pgbMain.Visible = true;
            pgbMain.Value = 0;
            lblCurrentTask.Text = taskTitle;
            lblCurrentTask.Visible = true;
            Application.DoEvents();
        }

        private void ImportProgressHandler(double fractionDone)
        {
            //m_fileBatcher.ProgressUpdate()

            pgbMain.Value = (int)Math.Round(fractionDone * 100d);
            Application.DoEvents();
        }

        private void SyncProgressHandler(string statusMsg, double fractionDone)
        {
            lblBatchProgress.Text = statusMsg;
            if (fractionDone > 1.0d)
            {
                fractionDone = 1.0d;
            }

            pgbMain.Value = (int)Math.Round(fractionDone * 100d);
            Application.DoEvents();
        }

        private void ImportEndHandler()
        {
            //m_fileBatcher.LoadEnd()

            lblCurrentTask.Text = "Complete: " + lblCurrentTask.Text;
            Invalidate();
            gbxDestinationCollection.Invalidate();
            gbxSourceCollection.Invalidate();
            Application.DoEvents();
        }

        private void CollectionLoadHandler(DataTable collectionTable)
        {
            m_ProteinCollections = collectionTable;
            if (m_Organisms == null)
            {
                m_Organisms = m_ImportHandler.LoadOrganisms();
            }

            if (m_AnnotationTypes == null)
            {
                m_AnnotationTypes = m_ImportHandler.LoadAnnotationTypes();
            }

            BindOrganismListToControl(m_Organisms);
            BindAnnotationTypeListToControl(m_AnnotationTypes);
            m_ProteinCollections.DefaultView.RowFilter = "";
            BindCollectionListToControl(m_ProteinCollections.DefaultView);
            cboCollectionPicker.Enabled = true;
            cboOrganismFilter.Enabled = true;
            lblBatchProgress.Text = "";
            //mnuToolsFBatchUpload.Enabled = true;

            cboOrganismFilter.SelectedIndexChanged += cboOrganismList_SelectedIndexChanged;
            cboCollectionPicker.SelectedIndexChanged += cboCollectionPicker_SelectedIndexChanged;
        }

        private void BatchImportProgressHandler(string status)
        {
            m_BatchLoadCurrentCount += 1;
            lblBatchProgress.Text = status + " (File " + m_BatchLoadCurrentCount.ToString() + " of " + m_BatchLoadTotalCount + ")";
            Application.DoEvents();
        }

        private void ValidFASTAUploadHandler(
            string FASTAFilePath,
            PSUploadHandler.UploadInfo UploadInfo)
        {
            if (m_ValidUploadsList == null)
            {
                m_ValidUploadsList = new Dictionary<string, PSUploadHandler.UploadInfo>();
            }

            m_ValidUploadsList.Add(FASTAFilePath, UploadInfo);
        }

        private void InvalidFASTAFileHandler(string FASTAFilePath, List<clsCustomValidateFastaFiles.udtErrorInfoExtended> errorCollection)
        {
            if (m_FileErrorList == null)
            {
                m_FileErrorList = new Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>>();
            }

            m_FileErrorList.Add(Path.GetFileName(FASTAFilePath), errorCollection);

            if (m_SummarizedFileErrorList == null)
            {
                m_SummarizedFileErrorList = new Dictionary<string, Dictionary<string, int>>();
            }

            m_SummarizedFileErrorList.Add(Path.GetFileName(FASTAFilePath), SummarizeErrors(errorCollection));
        }

        private void FASTAFileWarningsHandler(string FASTAFilePath, List<clsCustomValidateFastaFiles.udtErrorInfoExtended> warningCollection)
        {
            if (m_FileWarningList == null)
            {
                m_FileWarningList = new Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>>();
            }

            m_FileWarningList.Add(Path.GetFileName(FASTAFilePath), warningCollection);

            if (m_SummarizedFileWarningList == null)
            {
                m_SummarizedFileWarningList = new Dictionary<string, Dictionary<string, int>>();
            }

            m_SummarizedFileWarningList.Add(Path.GetFileName(FASTAFilePath), SummarizeErrors(warningCollection));
        }

        private Dictionary<string, int> SummarizeErrors(IReadOnlyCollection<clsCustomValidateFastaFiles.udtErrorInfoExtended> errorCollection)
        {
            // Keys are error messages, values are the number of times the error was reported
            var errorSummary = new Dictionary<string, int>();

            if (errorCollection != null && errorCollection.Count > 0)
            {
                foreach (var errorEntry in errorCollection)
                {
                    string message = errorEntry.MessageText;
                    int currentCount;

                    if (errorSummary.TryGetValue(message, out currentCount))
                    {
                        errorSummary[message] = currentCount + 1;
                    }
                    else
                    {
                        errorSummary.Add(message, 1);
                    }
                }
            }

            return errorSummary;
        }

        private void ValidationProgressHandler(string taskTitle, double fractionDone)
        {
            if (taskTitle != null)
            {
                lblCurrentTask.Text = taskTitle;
            }

            pgbMain.Value = (int)Math.Round(fractionDone * 100d);
            Application.DoEvents();
        }

        private void NormalizedFASTAFileGenerationHandler(string newFilePath)
        {
        }

        #endregion

        private void mnuAdminUpdateZeroedMasses_Click(object sender, EventArgs e)
        {
            if (m_Syncer == null)
            {
                m_Syncer = new SyncFASTAFileArchive(m_PSConnectionString);
                m_Syncer.SyncProgress += SyncProgressHandler;
            }

            m_Syncer.CorrectMasses();
        }

        private void mnuAdminNameHashRefresh_Click(object sender, EventArgs e)
        {
            if (m_Syncer == null)
            {
                m_Syncer = new SyncFASTAFileArchive(m_PSConnectionString);
                m_Syncer.SyncProgress += SyncProgressHandler;
            }

            m_Syncer.RefreshNameHashes();
        }

        private void mnuToolsNucToProt_Click(object sender, EventArgs e)
        {
        }

        private void mnuToolsConvert_Click(object sender, EventArgs e)
        {
        }

        private void mnuToolsConvertF2A_Click(object sender, EventArgs e)
        {
        }

        private void mnuToolsConvertA2F_Click(object sender, EventArgs e)
        {
        }

        private void mnuToolsFCheckup_Click(object sender, EventArgs e)
        {
        }

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            var frmTesting = new frmTestingInterface();
            frmTesting.Show();
        }

        private void MenuItem6_Click(object sender, EventArgs e)
        {
            if (m_Syncer == null)
            {
                m_Syncer = new SyncFASTAFileArchive(m_PSConnectionString);
                m_Syncer.SyncProgress += SyncProgressHandler;
            }

            m_Syncer.FixArchivedFilePaths();
        }

        private void MenuItem8_Click(object sender, EventArgs e)
        {
            if (m_Syncer == null)
            {
                m_Syncer = new SyncFASTAFileArchive(m_PSConnectionString);
                m_Syncer.SyncProgress += SyncProgressHandler;
            }

            m_Syncer.AddSortingIndices();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }

        private void mnuToolsOptions_Click(object sender, EventArgs e)
        {
        }

        //private void mnuAdminUpdateZeroedMasses_Click(object sender, EventArgs e)
        //{
        //}
    }
}