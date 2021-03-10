﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PRISMDatabaseUtils;
using Protein_Uploader;
using Raccoom.Windows.Forms;
using ValidateFastaFile;

namespace AppUI_OrfDBHandler
{
    public class frmBatchAddNewCollection : Form
    {
        #region "Windows Form Designer generated code"

        public frmBatchAddNewCollection(
            DataTable organismList,
            DataTable annotationTypeList,
            DataTable existingCollectionsList,
            string psConnectionString,
            string selectedFolderPath,
            Dictionary<string, KeyValuePair<string, string>> cachedFileDescriptions)
            : base()
        {
            base.Load += frmBatchAddNewCollection_Load;
            base.Closing += frmBatchAddNewCollection_Closing;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            m_StatusResetTimer = new Timer() { Interval = 1000 };
            m_StatusResetTimer.Tick += m_StatusResetTimer_Tick;

            m_StatusResetTimer.Start();

            ClearStatus();

            // Add any initialization after the InitializeComponent() call
            m_OrganismList = organismList;
            m_OrganismListSorted = new DataView(m_OrganismList) { Sort = "Display_Name" };

            m_AnnotationTypeList = annotationTypeList;
            m_CollectionsTable = existingCollectionsList;
            m_PSConnectionString = psConnectionString;

            m_CachedFileDescriptions = cachedFileDescriptions;

            ctlTreeViewFolderBrowser.DataSource = new TreeStrategyFolderBrowserProvider();
            ctlTreeViewFolderBrowser.CheckBoxBehaviorMode = CheckBoxBehaviorMode.None;

            InitializeTreeView(selectedFolderPath);
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
        private IContainer components;
        internal TreeViewFolderBrowser ctlTreeViewFolderBrowser;
        internal ComboBox cboOrganismSelect;
        internal Label lblBatchUploadTree;
        internal Label lblOrganismSelect;
        internal Label lblFolderContents;
        internal Button cmdCancel;
        internal Button cmdUploadChecked;
        internal ListView lvwFolderContents;
        internal ColumnHeader colFileName;
        internal ColumnHeader colFileSize;
        internal ColumnHeader colFileModDate;
        internal ComboBox cboAnnotationTypePicker;
        internal ColumnHeader colCollectionExists;
        internal ColumnHeader colUpFileName;
        internal ColumnHeader colSelOrganism;
        internal Label lblSelectedFiles;
        internal ListView lvwSelectedFiles;
        internal UIControls.ImageButton cmdAddFile;
        internal UIControls.ImageButton cmdRemoveFile;
        internal Label lblAnnAuth;
        internal ColumnHeader colAnnType;
        internal Button cmdPreviewFile;
        internal CheckBox chkEncryptionEnable;
        internal Label lblPassphrase;
        internal TextBox txtPassphrase;
        internal GroupBox fraValidationOptions;
        internal CheckBox chkValidationAllowAsterisks;
        internal Button cmdRefreshFiles;
        internal TextBox txtMaximumProteinNameLength;
        internal Label lblMaximumProteinNameLength;
        internal CheckBox chkValidationAllowAllSymbolsInProteinNames;
        internal ColumnHeader colDescription;
        internal ColumnHeader colSource;
        internal Button cmdUpdateDescription;
        internal Label lblStatus;
        internal CheckBox chkValidationAllowDash;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new Container();
            var resources = new ComponentResourceManager(typeof(frmBatchAddNewCollection));
            ctlTreeViewFolderBrowser = new TreeViewFolderBrowser();
            ctlTreeViewFolderBrowser.AfterSelect += new TreeViewEventHandler(ctlTreeViewFolderBrowser_AfterSelect);
            ctlTreeViewFolderBrowser.MouseUp += new MouseEventHandler(ctlTreeViewFolderBrowser_MouseUp);
            ctlTreeViewFolderBrowser.KeyUp += new KeyEventHandler(ctlTreeViewFolderBrowser_KeyUp);
            cboOrganismSelect = new ComboBox();
            cboOrganismSelect.SelectedIndexChanged += new EventHandler(cboOrganismSelect_SelectedIndexChanged);
            lblBatchUploadTree = new Label();
            lblOrganismSelect = new Label();
            lblFolderContents = new Label();
            cmdCancel = new Button();
            cmdCancel.Click += new EventHandler(cmdCancel_Click);
            cmdUploadChecked = new Button();
            cmdUploadChecked.Click += new EventHandler(cmdUploadChecked_Click);
            lvwFolderContents = new ListView();
            lvwFolderContents.Click += new EventHandler(lvwFolderContents_Click);
            lvwFolderContents.MouseUp += new MouseEventHandler(lvwFolderContents_MouseUp);
            lvwFolderContents.ColumnClick += new ColumnClickEventHandler(lvwFolderContents_ColumnClick);
            lvwFolderContents.KeyDown += new KeyEventHandler(lvwFolderContents_KeyDown);
            colFileName = new ColumnHeader();
            colFileModDate = new ColumnHeader();
            colFileSize = new ColumnHeader();
            colCollectionExists = new ColumnHeader();
            cboAnnotationTypePicker = new ComboBox();
            cboAnnotationTypePicker.SelectedIndexChanged += new EventHandler(cboAnnotationTypePicker_SelectedIndexChanged);
            lblAnnAuth = new Label();
            lvwSelectedFiles = new ListView();
            lvwSelectedFiles.Click += new EventHandler(lvwSelectedFiles_Click);
            lvwSelectedFiles.ColumnClick += new ColumnClickEventHandler(lvwSelectedFiles_ColumnClick);
            lvwSelectedFiles.DoubleClick += new EventHandler(lvwSelectedFiles_DoubleClick);
            lvwSelectedFiles.KeyDown += new KeyEventHandler(lvwSelectedFiles_KeyDown);
            colUpFileName = new ColumnHeader();
            colSelOrganism = new ColumnHeader();
            colDescription = new ColumnHeader();
            colSource = new ColumnHeader();
            colAnnType = new ColumnHeader();
            lblSelectedFiles = new Label();
            cmdAddFile = new UIControls.ImageButton();
            cmdAddFile.Click += new EventHandler(cmdAddFile_Click);
            cmdRemoveFile = new UIControls.ImageButton();
            cmdRemoveFile.Click += new EventHandler(cmdRemoveFile_Click);
            cmdPreviewFile = new Button();
            cmdPreviewFile.Click += new EventHandler(cmdPreviewFile_Click);
            chkEncryptionEnable = new CheckBox();
            chkEncryptionEnable.CheckedChanged += new EventHandler(chkEncryptionEnable_CheckedChanged);
            lblPassphrase = new Label();
            txtPassphrase = new TextBox();
            fraValidationOptions = new GroupBox();
            chkValidationAllowAllSymbolsInProteinNames = new CheckBox();
            txtMaximumProteinNameLength = new TextBox();
            txtMaximumProteinNameLength.Validating += new CancelEventHandler(txtMaximumProteinNameLength_Validating);
            lblMaximumProteinNameLength = new Label();
            chkValidationAllowAsterisks = new CheckBox();
            chkValidationAllowDash = new CheckBox();
            cmdRefreshFiles = new Button();
            cmdRefreshFiles.Click += new EventHandler(cmdRefreshFiles_Click);
            cmdUpdateDescription = new Button();
            cmdUpdateDescription.Click += new EventHandler(cmdUpdateDescription_Click);
            lblStatus = new Label();
            fraValidationOptions.SuspendLayout();
            SuspendLayout();
            //
            // ctlTreeViewFolderBrowser
            //
            ctlTreeViewFolderBrowser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            ctlTreeViewFolderBrowser.DataSource = null;
            ctlTreeViewFolderBrowser.HideSelection = false;
            ctlTreeViewFolderBrowser.Location = new Point(10, 35);
            ctlTreeViewFolderBrowser.Name = "_treeViewFolderBrowser1";
            ctlTreeViewFolderBrowser.ShowLines = false;
            ctlTreeViewFolderBrowser.ShowRootLines = false;
            ctlTreeViewFolderBrowser.Size = new Size(326, 524);
            ctlTreeViewFolderBrowser.TabIndex = 0;
            //
            // cboOrganismSelect
            //
            cboOrganismSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cboOrganismSelect.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOrganismSelect.Location = new Point(8, 587);
            cboOrganismSelect.Name = "cboOrganismSelect";
            cboOrganismSelect.Size = new Size(609, 25);
            cboOrganismSelect.TabIndex = 11;
            //
            // lblBatchUploadTree
            //
            lblBatchUploadTree.Location = new Point(14, 12);
            lblBatchUploadTree.Name = "lblBatchUploadTree";
            lblBatchUploadTree.Size = new Size(320, 20);
            lblBatchUploadTree.TabIndex = 0;
            lblBatchUploadTree.Text = "Select source folder for upload (F5 to refresh)";
            //
            // lblOrganismSelect
            //
            lblOrganismSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblOrganismSelect.Location = new Point(8, 567);
            lblOrganismSelect.Name = "lblOrganismSelect";
            lblOrganismSelect.Size = new Size(261, 20);
            lblOrganismSelect.TabIndex = 10;
            lblOrganismSelect.Text = "Select destination &organism";
            //
            // lblFolderContents
            //
            lblFolderContents.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblFolderContents.Location = new Point(342, 12);
            lblFolderContents.Name = "lblFolderContents";
            lblFolderContents.Size = new Size(835, 20);
            lblFolderContents.TabIndex = 2;
            lblFolderContents.Text = "Selected folder contents";
            //
            // cmdCancel
            //
            cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdCancel.DialogResult = DialogResult.Cancel;
            cmdCancel.Location = new Point(1101, 653);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(84, 36);
            cmdCancel.TabIndex = 20;
            cmdCancel.Text = "Cancel";
            //
            // cmdUploadChecked
            //
            cmdUploadChecked.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdUploadChecked.Location = new Point(918, 653);
            cmdUploadChecked.Name = "cmdUploadChecked";
            cmdUploadChecked.Size = new Size(168, 36);
            cmdUploadChecked.TabIndex = 19;
            cmdUploadChecked.Text = "&Upload new FASTAs";
            //
            // lvwFolderContents
            //
            lvwFolderContents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwFolderContents.Columns.AddRange(new ColumnHeader[] { colFileName, colFileModDate, colFileSize, colCollectionExists });
            lvwFolderContents.FullRowSelect = true;
            lvwFolderContents.GridLines = true;
            lvwFolderContents.HideSelection = false;
            lvwFolderContents.Location = new Point(342, 32);
            lvwFolderContents.Name = "lvwFolderContents";
            lvwFolderContents.Size = new Size(843, 218);
            lvwFolderContents.Sorting = SortOrder.Ascending;
            lvwFolderContents.TabIndex = 3;
            lvwFolderContents.UseCompatibleStateImageBehavior = false;
            lvwFolderContents.View = View.Details;
            //
            // colFileName
            //
            colFileName.Text = "Name";
            colFileName.Width = 450;
            //
            // colFileModDate
            //
            colFileModDate.Text = "Date Modified";
            colFileModDate.Width = 140;
            //
            // colFileSize
            //
            colFileSize.Text = "Size";
            colFileSize.Width = 67;
            //
            // colCollectionExists
            //
            colCollectionExists.Text = "Existing Collection?";
            colCollectionExists.Width = 150;
            //
            // cboAnnotationTypePicker
            //
            cboAnnotationTypePicker.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cboAnnotationTypePicker.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAnnotationTypePicker.Location = new Point(641, 587);
            cboAnnotationTypePicker.Name = "cboAnnotationTypePicker";
            cboAnnotationTypePicker.Size = new Size(364, 25);
            cboAnnotationTypePicker.TabIndex = 13;
            //
            // lblAnnAuth
            //
            lblAnnAuth.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblAnnAuth.Location = new Point(641, 567);
            lblAnnAuth.Name = "lblAnnAuth";
            lblAnnAuth.Size = new Size(285, 20);
            lblAnnAuth.TabIndex = 12;
            lblAnnAuth.Text = "Select Annotation &Type";
            //
            // lvwSelectedFiles
            //
            lvwSelectedFiles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwSelectedFiles.Columns.AddRange(new ColumnHeader[] { colUpFileName, colSelOrganism, colDescription, colSource, colAnnType });
            lvwSelectedFiles.FullRowSelect = true;
            lvwSelectedFiles.GridLines = true;
            lvwSelectedFiles.HideSelection = false;
            lvwSelectedFiles.Location = new Point(342, 335);
            lvwSelectedFiles.Name = "lvwSelectedFiles";
            lvwSelectedFiles.Size = new Size(843, 224);
            lvwSelectedFiles.TabIndex = 9;
            lvwSelectedFiles.UseCompatibleStateImageBehavior = false;
            lvwSelectedFiles.View = View.Details;
            //
            // colUpFileName
            //
            colUpFileName.Text = "Name";
            colUpFileName.Width = 251;
            //
            // colSelOrganism
            //
            colSelOrganism.Text = "Selected Organism";
            colSelOrganism.Width = 141;
            //
            // colDescription
            //
            colDescription.Text = "Description";
            colDescription.Width = 150;
            //
            // colSource
            //
            colSource.Text = "Source (person, URL, FTP)";
            colSource.Width = 150;
            //
            // colAnnType
            //
            colAnnType.Text = "Annotation Type";
            colAnnType.Width = 105;
            //
            // lblSelectedFiles
            //
            lblSelectedFiles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblSelectedFiles.Location = new Point(342, 304);
            lblSelectedFiles.Name = "lblSelectedFiles";
            lblSelectedFiles.Size = new Size(868, 19);
            lblSelectedFiles.TabIndex = 8;
            lblSelectedFiles.Text = "FASTA files selected for upload";
            //
            // cmdAddFile
            //
            cmdAddFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdAddFile.FlatStyle = FlatStyle.System;
            cmdAddFile.GenerateDisabledImage = true;
            cmdAddFile.Location = new Point(681, 272);
            cmdAddFile.Name = "cmdAddFile";
            cmdAddFile.Size = new Size(48, 44);
            cmdAddFile.TabIndex = 6;
            cmdAddFile.ThemedImage = (Bitmap)resources.GetObject("cmdAddFile.ThemedImage");
            //
            // cmdRemoveFile
            //
            cmdRemoveFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdRemoveFile.FlatStyle = FlatStyle.System;
            cmdRemoveFile.GenerateDisabledImage = true;
            cmdRemoveFile.Location = new Point(751, 272);
            cmdRemoveFile.Name = "cmdRemoveFile";
            cmdRemoveFile.Size = new Size(48, 44);
            cmdRemoveFile.TabIndex = 7;
            cmdRemoveFile.ThemedImage = (Bitmap)resources.GetObject("cmdRemoveFile.ThemedImage");
            //
            // cmdPreviewFile
            //
            cmdPreviewFile.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdPreviewFile.Enabled = false;
            cmdPreviewFile.Location = new Point(1003, 297);
            cmdPreviewFile.Name = "cmdPreviewFile";
            cmdPreviewFile.Size = new Size(182, 30);
            cmdPreviewFile.TabIndex = 5;
            cmdPreviewFile.Text = "&Preview Selected File";
            //
            // chkEncryptionEnable
            //
            chkEncryptionEnable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkEncryptionEnable.Location = new Point(707, 628);
            chkEncryptionEnable.Name = "chkEncryptionEnable";
            chkEncryptionEnable.Size = new Size(174, 23);
            chkEncryptionEnable.TabIndex = 16;
            chkEncryptionEnable.Text = "Encrypt Sequences?";
            chkEncryptionEnable.Visible = false;
            //
            // lblPassphrase
            //
            lblPassphrase.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblPassphrase.Location = new Point(703, 653);
            lblPassphrase.Name = "lblPassphrase";
            lblPassphrase.Size = new Size(178, 20);
            lblPassphrase.TabIndex = 17;
            lblPassphrase.Text = "Encryption Passphrase";
            lblPassphrase.Visible = false;
            //
            // txtPassphrase
            //
            txtPassphrase.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtPassphrase.Enabled = false;
            txtPassphrase.Location = new Point(707, 673);
            txtPassphrase.Name = "txtPassphrase";
            txtPassphrase.PasswordChar = '•';
            txtPassphrase.Size = new Size(154, 24);
            txtPassphrase.TabIndex = 18;
            txtPassphrase.Visible = false;
            //
            // fraValidationOptions
            //
            fraValidationOptions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            fraValidationOptions.Controls.Add(chkValidationAllowAllSymbolsInProteinNames);
            fraValidationOptions.Controls.Add(txtMaximumProteinNameLength);
            fraValidationOptions.Controls.Add(lblMaximumProteinNameLength);
            fraValidationOptions.Controls.Add(chkValidationAllowAsterisks);
            fraValidationOptions.Controls.Add(chkValidationAllowDash);
            fraValidationOptions.Location = new Point(6, 622);
            fraValidationOptions.Name = "fraValidationOptions";
            fraValidationOptions.Size = new Size(687, 75);
            fraValidationOptions.TabIndex = 15;
            fraValidationOptions.TabStop = false;
            fraValidationOptions.Text = "Fasta Validation Options";
            //
            // chkValidationAllowAllSymbolsInProteinNames
            //
            chkValidationAllowAllSymbolsInProteinNames.Location = new Point(11, 47);
            chkValidationAllowAllSymbolsInProteinNames.Name = "chkValidationAllowAllSymbolsInProteinNames";
            chkValidationAllowAllSymbolsInProteinNames.Size = new Size(292, 25);
            chkValidationAllowAllSymbolsInProteinNames.TabIndex = 1;
            chkValidationAllowAllSymbolsInProteinNames.Text = "Allow all symbols in protein names";
            //
            // txtMaximumProteinNameLength
            //
            txtMaximumProteinNameLength.Location = new Point(570, 21);
            txtMaximumProteinNameLength.Name = "txtMaximumProteinNameLength";
            txtMaximumProteinNameLength.Size = new Size(84, 24);
            txtMaximumProteinNameLength.TabIndex = 4;
            txtMaximumProteinNameLength.Text = "60";
            //
            // lblMaximumProteinNameLength
            //
            lblMaximumProteinNameLength.Location = new Point(454, 16);
            lblMaximumProteinNameLength.Name = "lblMaximumProteinNameLength";
            lblMaximumProteinNameLength.Size = new Size(128, 34);
            lblMaximumProteinNameLength.TabIndex = 3;
            lblMaximumProteinNameLength.Text = "Max Protein Name Length";
            //
            // chkValidationAllowAsterisks
            //
            chkValidationAllowAsterisks.Location = new Point(11, 19);
            chkValidationAllowAsterisks.Name = "chkValidationAllowAsterisks";
            chkValidationAllowAsterisks.Size = new Size(219, 25);
            chkValidationAllowAsterisks.TabIndex = 0;
            chkValidationAllowAsterisks.Text = "Allow asterisks in residues";
            //
            // chkValidationAllowDash
            //
            chkValidationAllowDash.Location = new Point(241, 19);
            chkValidationAllowDash.Name = "chkValidationAllowDash";
            chkValidationAllowDash.Size = new Size(218, 25);
            chkValidationAllowDash.TabIndex = 2;
            chkValidationAllowDash.Text = "Allow dash in residues";
            //
            // cmdRefreshFiles
            //
            cmdRefreshFiles.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdRefreshFiles.Location = new Point(1003, 258);
            cmdRefreshFiles.Name = "cmdRefreshFiles";
            cmdRefreshFiles.Size = new Size(182, 30);
            cmdRefreshFiles.TabIndex = 4;
            cmdRefreshFiles.Text = "&Refresh Files";
            //
            // cmdUpdateDescription
            //
            cmdUpdateDescription.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdUpdateDescription.Location = new Point(1068, 574);
            cmdUpdateDescription.Name = "cmdUpdateDescription";
            cmdUpdateDescription.Size = new Size(117, 49);
            cmdUpdateDescription.TabIndex = 14;
            cmdUpdateDescription.Text = "Update &Description";
            //
            // lblStatus
            //
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStatus.Location = new Point(342, 258);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(295, 46);
            lblStatus.TabIndex = 21;
            lblStatus.Text = "Status";
            //
            // frmBatchAddNewCollection
            //
            AcceptButton = cmdUploadChecked;
            AutoScaleBaseSize = new Size(7, 17);
            CancelButton = cmdCancel;
            ClientSize = new Size(1199, 707);
            Controls.Add(cmdRemoveFile);
            Controls.Add(cmdAddFile);
            Controls.Add(lblStatus);
            Controls.Add(cmdCancel);
            Controls.Add(cmdUploadChecked);
            Controls.Add(cmdUpdateDescription);
            Controls.Add(cmdRefreshFiles);
            Controls.Add(fraValidationOptions);
            Controls.Add(txtPassphrase);
            Controls.Add(lblPassphrase);
            Controls.Add(chkEncryptionEnable);
            Controls.Add(cmdPreviewFile);
            Controls.Add(lvwSelectedFiles);
            Controls.Add(lvwFolderContents);
            Controls.Add(lblOrganismSelect);
            Controls.Add(ctlTreeViewFolderBrowser);
            Controls.Add(cboOrganismSelect);
            Controls.Add(lblBatchUploadTree);
            Controls.Add(lblFolderContents);
            Controls.Add(cboAnnotationTypePicker);
            Controls.Add(lblAnnAuth);
            Controls.Add(lblSelectedFiles);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            MinimumSize = new Size(1120, 576);
            Name = "frmBatchAddNewCollection";
            Text = "Batch Upload FASTA Files";
            fraValidationOptions.ResumeLayout(false);
            fraValidationOptions.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        #region "Constants, enums, and member variables"

        private enum eFolderContentsColumn
        {
            FileName = 0,
            LastModified = 1,
            FileSize = 2,
            ExistingCollection = 3,
            FilePath = 4,
        }

        private enum eSelectedFileColumn
        {
            ProteinCollectionName = 0,
            Organism = 1,
            Description = 2,
            Source = 3,
            AnnotationType = 4,
            FilePath = 5,
        }

        private struct udtProteinCollectionMetadata
        {
            public string Description;
            public string Source;
        }

        private string m_LastUsedDirectory;
        private string m_LastSelectedOrganism = string.Empty;
        private string m_LastSelectedAnnotationType = string.Empty;

        /// <summary>
        /// Keys are full paths to the fasta file
        /// Values are FileInfo instances
        /// </summary>
        private Dictionary<string, FileInfo> m_FileList;

        private List<PSUploadHandler.UploadInfo> m_CheckedFileList;

        // Keys are file paths, values are UploadInfo objects
        private Dictionary<string, PSUploadHandler.UploadInfo> m_SelectedFileList;

        private readonly DataTable m_OrganismList;
        private readonly DataView m_OrganismListSorted;

        private readonly DataTable m_AnnotationTypeList;
        private readonly DataTable m_CollectionsTable;

        /// <summary>
        /// Keys are protein collection ID
        /// Values are Protein Collection name
        /// </summary>
        private Dictionary<int, string> m_CollectionsList;

        private int m_SelectedOrganismID;
        private int m_SelectedAnnotationTypeID;
        private readonly string m_PSConnectionString;
        private bool m_ReallyClose = false;
        private FilePreviewHandler m_FilePreviewer;

        private bool m_PreviewFormStatus;

        // private HashTable m_PassphraseList;
        // private string m_CachedPassphrase;

        private const string ADD_FILES_MESSAGE = "You must first select an organism and annotation type, and then select one or more protein collections.";
        private bool m_AllowAddFiles;
        private string m_AllowAddFilesMessage = ADD_FILES_MESSAGE;

        // Tracks the index of the last column clicked in lvwSelectedFiles
        private int mSortColumnSelectedItems = -1;

        // Tracks the index of the last column clicked in lvwFolderContents
        private int mSortColumnFolderContents = -1;

        /// <summary>
        /// Tracks Description and Source that the uploader has defined for each file (not persisted when the application closes)
        /// </summary>
        /// <remarks>Useful in case validation fails and the uploader needs to try again to upload a FASTA file</remarks>
        private readonly Dictionary<string, KeyValuePair<string, string>> m_CachedFileDescriptions;

        private bool m_StatusResetRequired;
        private DateTime m_StatusClearTime;
        private readonly Timer m_StatusResetTimer;

        #endregion

        #region "Properties"

        public List<PSUploadHandler.UploadInfo> FileList
        {
            get
            {
                return m_CheckedFileList;
            }
        }

        public int SelectedOrganismID
        {
            get
            {
                return m_SelectedOrganismID;
            }
        }

        public int SelectedAnnotationTypeID
        {
            get
            {
                return m_SelectedAnnotationTypeID;
            }
        }

        public string CurrentDirectory
        {
            get
            {
                return m_LastUsedDirectory;
            }

            set
            {
                m_LastUsedDirectory = value;
            }
        }

        private string SelectedNodeFolderPath
        {
            get
            {
                try
                {
                    TreeNodePath currentNode = ctlTreeViewFolderBrowser.SelectedNode as TreeNodePath;

                    if (currentNode != null && !string.IsNullOrWhiteSpace(currentNode.Path))
                    {
                        return currentNode.Path;
                    }
                }
                catch (Exception ex)
                {
                    // Ignore errors
                }

                return string.Empty;
            }
        }

        public string SelectedOrganismName
        {
            get
            {
                if (cboOrganismSelect.Items.Count > 0)
                {
                    return cboOrganismSelect.Text;
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                m_LastSelectedOrganism = value;
            }
        }

        public string SelectedAnnotationType
        {
            get
            {
                if (cboAnnotationTypePicker.Items.Count > 0)
                {
                    return cboAnnotationTypePicker.Text;
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                m_LastSelectedAnnotationType = value;
            }
        }

        public bool ValidationAllowAsterisks
        {
            get
            {
                return chkValidationAllowAsterisks.Checked;
            }

            set
            {
                chkValidationAllowAsterisks.Checked = value;
            }
        }

        public bool ValidationAllowDash
        {
            get
            {
                return chkValidationAllowDash.Checked;
            }

            set
            {
                chkValidationAllowDash.Checked = value;
            }
        }

        public bool ValidationAllowAllSymbolsInProteinNames
        {
            get
            {
                return chkValidationAllowAllSymbolsInProteinNames.Checked;
            }

            set
            {
                chkValidationAllowAllSymbolsInProteinNames.Checked = value;
            }
        }

        public int ValidationMaxProteinNameLength
        {
            get
            {
                int intValue;
                if (int.TryParse(txtMaximumProteinNameLength.Text, out intValue))
                {
                    return intValue;
                }
                else
                {
                    return clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
                }
            }

            set
            {
                if (value < 5)
                    value = 5;
                txtMaximumProteinNameLength.Text = value.ToString();
            }
        }

        #endregion

        private void frmBatchAddNewCollection_Load(object sender, EventArgs e)
        {
            m_CollectionsList = CollectionsTableToList(m_CollectionsTable);

            if (m_FileList == null)
            {
                m_FileList = new Dictionary<string, FileInfo>();
            }

            m_CheckedFileList = new List<PSUploadHandler.UploadInfo>();
            LoadOrganismPicker(cboOrganismSelect, m_OrganismListSorted);
            LoadAnnotationTypePicker(cboAnnotationTypePicker, m_AnnotationTypeList);
            cmdUploadChecked.Enabled = false;
            cmdAddFile.Enabled = true;
            cmdRemoveFile.Enabled = false;

            SelectComboBoxItemByName(cboOrganismSelect, m_LastSelectedOrganism, 2);
            SelectComboBoxItemByName(cboAnnotationTypePicker, m_LastSelectedAnnotationType, 1);
        }

        #region "Directory Loading"

        private Dictionary<int, string> CollectionsTableToList(DataTable dt)
        {
            var collectionInfo = new Dictionary<int, string>(dt.Rows.Count);
            var foundRows = dt.Select("", "Protein_Collection_ID");
            int tmpID;
            string tmpName;

            foreach (DataRow dr in foundRows)
            {
                tmpID = DatabaseUtilsExtensions.GetInteger(null, dr["Protein_Collection_ID"]);
                tmpName = dr["FileName"].ToString();
                if (!collectionInfo.ContainsKey(tmpID))
                {
                    collectionInfo.Add(tmpID, tmpName);
                }
            }

            return collectionInfo;
        }

        private void ScanDirectory(string directoryPath)
        {
            lblFolderContents.Text = "FASTA files in: '" + directoryPath + "'";

            var di = new DirectoryInfo(directoryPath);

            if (!di.Exists)
            {
                return;
            }

            m_LastUsedDirectory = directoryPath;

            var foundFASTAFiles = di.GetFiles();

            if (m_FileList != null)
            {
                m_FileList.Clear();
            }
            else
            {
                m_FileList = new Dictionary<string, FileInfo>();
            }

            foreach (var fi in foundFASTAFiles)
            {
                string fileExtension = Path.GetExtension(fi.Name);

                switch (fileExtension.ToLower() ?? "")
                {
                    case ".fasta":
                    case ".fst":
                    case ".fa":
                    case ".pep":
                    case ".faa":
                        m_FileList.Add(fi.FullName, fi);
                        break;
                }
            }

            LoadListView();
        }

        #endregion

        #region "UI Loading Functions"

        /// <summary>
        /// Populate the top ListView with fasta files in the selected folder
        /// </summary>
        /// <remarks></remarks>
        private void LoadListView()
        {
            lvwFolderContents.BeginUpdate();

            lvwFolderContents.Items.Clear();

            if (m_CollectionsList == null)
            {
                m_CollectionsList = new Dictionary<int, string>();
            }

            foreach (var fi in m_FileList.Values)
            {
                string proteinCollectionName = Path.GetFileNameWithoutExtension(fi.Name);

                var li = new ListViewItem();

                // Fasta file name (with the extension)
                li.Text = fi.Name;

                // Last Write Time
                li.SubItems.Add(fi.LastWriteTime.ToString("g"));

                // File Size
                li.SubItems.Add(Numeric2Bytes(fi.Length));

                // Whether or not the fasta file is already a protein collection
                if (m_CollectionsList.ContainsValue(proteinCollectionName))
                {
                    li.SubItems.Add("Yes");
                }
                else
                {
                    li.SubItems.Add("No");
                }

                // Full file path
                li.SubItems.Add(fi.FullName);

                lvwFolderContents.Items.Add(li);
            }

            lvwFolderContents.EndUpdate();
        }

        private void LoadOrganismPicker(ComboBox cbo, DataView orgList)
        {
            cboOrganismSelect.SelectedIndexChanged -= cboOrganismSelect_SelectedIndexChanged;
            cbo.DataSource = orgList;
            cbo.DisplayMember = "Display_Name";
            cbo.ValueMember = "ID";

            cboOrganismSelect.SelectedIndexChanged += cboOrganismSelect_SelectedIndexChanged;
        }

        private void LoadAnnotationTypePicker(ComboBox cbo, DataTable authList)
        {
            cboAnnotationTypePicker.SelectedIndexChanged -= cboAnnotationTypePicker_SelectedIndexChanged;

            cbo.BeginUpdate();
            var dr = authList.NewRow();

            dr["ID"] = -2;
            dr["Display_Name"] = "Add New Annotation Type...";
            dr["Details"] = "Brings up a dialog box to allow adding a naming authority to the list";

            var pk1 = new DataColumn[1];
            pk1[0] = authList.Columns["ID"];
            authList.PrimaryKey = pk1;

            if (authList.Rows.Contains(dr["ID"]))
            {
                var rdr = authList.Rows.Find(dr["ID"]);
                authList.Rows.Remove(rdr);
            }

            authList.Rows.Add(dr);
            authList.AcceptChanges();

            cbo.DataSource = authList;
            cbo.DisplayMember = "Display_Name";
            cbo.ValueMember = "ID";
            cbo.EndUpdate();

            cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
        }

        #endregion

        #region "Internal Service Functions"

        public string Numeric2Bytes(double b)
        {
            var bSize = new string[9];
            int i;

            bSize[0] = "Bytes";
            bSize[1] = "KB"; // Kilobytes
            bSize[2] = "MB"; // Megabytes
            bSize[3] = "GB"; // Gigabytes
            bSize[4] = "TB"; // Terabytes
            bSize[5] = "PB"; // Petabytes
            bSize[6] = "EB"; // Exabytes
            bSize[7] = "ZB"; // Zettabytes
            bSize[8] = "YB"; // Yottabytes

            for (i = bSize.Length; i >= 0; i -= 1)
            {
                if (b >= Math.Pow(1024d, i))
                {
                    return FormatDecimal(b / Math.Pow(1024d, i)) + " " + bSize[i];
                }
            }

            return b.ToString() + " Bytes";
        }

        /// <summary>
        /// Return the value formatted to include one or two digits after the decimal point
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Examples:
        /// 1
        /// 123
        /// 12.3
        /// 2.4
        /// 1.2
        /// 0.12
        /// </remarks>
        private string FormatDecimal(double value)
        {
            if (value >= 100d)
            {
                // No digits after the decimal
                return ((int)Math.Round(value)).ToString();
            }
            else if (value >= 1d)
            {
                // One digit after the decimal
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal
                return value.ToString("0.00");
            }
        }

        #endregion

        private void AddFileToSelectedList()
        {
            try
            {
                if (m_SelectedFileList == null)
                {
                    m_SelectedFileList = new Dictionary<string, PSUploadHandler.UploadInfo>(StringComparer.CurrentCultureIgnoreCase);
                }

                foreach (ListViewItem li in lvwFolderContents.SelectedItems)
                {
                    string fastaFilePath = GetFolderContentsColumn(li, eFolderContentsColumn.FilePath);

                    var upInfo = new PSUploadHandler.UploadInfo()
                    {
                        FileInformation = m_FileList[fastaFilePath],
                        OrganismID = (int)cboOrganismSelect.SelectedValue,
                        AnnotationTypeID = (int)cboAnnotationTypePicker.SelectedValue,
                        Description = string.Empty,
                        Source = string.Empty,
                        EncryptSequences = false,
                        EncryptionPassphrase = string.Empty
                    };

                    string proteinCollection = Path.GetFileNameWithoutExtension(upInfo.FileInformation.Name);

                    if (m_SelectedFileList.ContainsKey(upInfo.FileInformation.FullName))
                    {
                        m_SelectedFileList.Remove(upInfo.FileInformation.FullName);
                        foreach (ListViewItem si in lvwSelectedFiles.Items)
                        {
                            if ((si.Text ?? "") == (proteinCollection ?? ""))
                            {
                                lvwSelectedFiles.Items.Remove(si);
                            }
                        }
                    }

                    KeyValuePair<string, string> kvDescriptionSource = default;
                    string fileDescription;
                    string fileSource;

                    if (m_CachedFileDescriptions.TryGetValue(proteinCollection, out kvDescriptionSource))
                    {
                        fileDescription = kvDescriptionSource.Key;
                        fileSource = kvDescriptionSource.Value;
                    }
                    else
                    {
                        fileDescription = string.Empty;
                        fileSource = string.Empty;
                    }

                    var newLi = new ListViewItem(proteinCollection);
                    // Organism (Column 1)
                    newLi.SubItems.Add(cboOrganismSelect.Text);

                    // Description (Column 2)
                    newLi.SubItems.Add(fileDescription);

                    // Source (Column 3)
                    newLi.SubItems.Add(fileSource);

                    // Annotation Type (Column 4)
                    newLi.SubItems.Add(cboAnnotationTypePicker.Text);

                    // Full Path (ColIndex 5)
                    newLi.SubItems.Add(upInfo.FileInformation.FullName);

                    lvwSelectedFiles.Items.Add(newLi);
                    m_SelectedFileList.Add(upInfo.FileInformation.FullName, upInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in AddFileToSelectedList: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AddUpdateDictionaryItem(IDictionary<string, int> itemList, string newItem)
        {
            int itemCount;
            if (itemList.TryGetValue(newItem, out itemCount))
            {
                itemList[newItem] = itemCount + 1;
            }
            else
            {
                itemList.Add(newItem, 1);
            }
        }

        private void AfterNodeSelect()
        {
            string folderPath = SelectedNodeFolderPath;
            if (string.IsNullOrWhiteSpace(folderPath))
                return;
            ScanDirectory(folderPath);
        }

        private void ClearStatus()
        {
            lblStatus.Text = string.Empty;
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = message;

            m_StatusClearTime = DateTime.UtcNow.AddSeconds(5d);
            m_StatusResetRequired = true;
        }

        private string GetFolderContentsColumn(ListViewItem li, eFolderContentsColumn eColumn)
        {
            string value = li.SubItems[(int)eColumn].Text;
            return value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="li"></param>
        /// <param name="eColumn"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string GetSelectedFileColumn(ListViewItem li, eSelectedFileColumn eColumn)
        {
            string value = li.SubItems[(int)eColumn].Text;
            return value;
        }

        private void InitializeTreeView(string selectedDirectoryPath = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(selectedDirectoryPath))
                {
                    var currentFolder = new DirectoryInfo(selectedDirectoryPath);

                    while (!currentFolder.Exists && currentFolder.Parent != null)
                        currentFolder = currentFolder.Parent;

                    if (string.Equals(SelectedNodeFolderPath, currentFolder.FullName, StringComparison.OrdinalIgnoreCase))
                    {
                        ScanDirectory(currentFolder.FullName);
                        return;
                    }

                    ctlTreeViewFolderBrowser.Populate(currentFolder.FullName);
                    return;
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(selectedDirectoryPath))
                {
                    MessageBox.Show("Error refreshing folders and files for directory " + selectedDirectoryPath + ": " + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            ctlTreeViewFolderBrowser.Populate();
            if (!ctlTreeViewFolderBrowser.TopNode.IsExpanded)
            {
                ctlTreeViewFolderBrowser.TopNode.Expand();
            }
        }


        /// <summary>
        /// Populates m_CheckedFileList
        /// </summary>
        /// <returns>True if success, false if no protein collections are selected or if one or more is missing a description and/or source</returns>
        /// <remarks></remarks>
        private bool MakeCheckedFileList()
        {
            var tmpNameList = new Dictionary<string, udtProteinCollectionMetadata>();

            foreach (ListViewItem li in lvwSelectedFiles.Items)
            {
                string fastaFilePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath);

                if (!tmpNameList.ContainsKey(fastaFilePath))
                {
                    var udtMetadata = new udtProteinCollectionMetadata();
                    udtMetadata.Description = GetSelectedFileColumn(li, eSelectedFileColumn.Description);
                    udtMetadata.Source = GetSelectedFileColumn(li, eSelectedFileColumn.Source);
                    tmpNameList.Add(fastaFilePath, udtMetadata);
                }
            }

            foreach (var item in m_SelectedFileList)
            {
                var upInfo = item.Value;
                var fi = upInfo.FileInformation;

                udtProteinCollectionMetadata udtMetadata = default;
                if (tmpNameList.TryGetValue(fi.FullName, out udtMetadata))
                {
                    upInfo.Description = udtMetadata.Description;
                    upInfo.Source = udtMetadata.Source;

                    if (string.IsNullOrWhiteSpace(upInfo.Description) && string.IsNullOrWhiteSpace(upInfo.Source))
                    {
                        MessageBox.Show("Each new protein collection must have a description and/or source defined", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    m_CheckedFileList.Add(upInfo);
                }
            }

            if (m_CheckedFileList.Count == 0)
            {
                MessageBox.Show("No fasta files are selected", "Nothing to Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private string MostCommonItem(Dictionary<string, int> itemList)
        {
            if (itemList.Count == 0)
            {
                return string.Empty;
            }

            var query = from item in itemList orderby item.Key descending select item;

            return query.First().Key;
        }

        private void RemoveFileFromSelectedList()
        {
            foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
            {
                lvwSelectedFiles.Items.Remove(li);

                string filePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath);
                if (m_SelectedFileList.ContainsKey(filePath))
                {
                    m_SelectedFileList.Remove(filePath);
                }
            }
        }

        private void SelectAllRows(ListView lv)
        {
            foreach (ListViewItem li in lv.Items)
                li.Selected = true;
        }

        private void SelectComboBoxItemByName(ComboBox objComboBox, string strValue, int intDataColumnIndexToCheck)
        {
            // Look for strValue in a combobox that has a data table attached via the .DataSource property
            // If the value is found, then the given item in the combobox is selected

            int intIndex;
            DataRowView objRow;
            try
            {
                if (strValue != null && strValue.Length > 0)
                {
                    for (intIndex = 0; intIndex < objComboBox.Items.Count; intIndex++)
                    {
                        objRow = (DataRowView)objComboBox.Items[intIndex];

                        if (!DBNull.Value.Equals(objRow[intDataColumnIndexToCheck]))
                        {
                            if ((objRow[intDataColumnIndexToCheck]?.ToString() ?? "") == (strValue ?? ""))
                            {
                                objComboBox.SelectedIndex = intIndex;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore errors here
            }
        }

        private void SortListView(ListView lv, int lastClickedColIndex, int colIndex, bool isDateColumn)
        {
            // Determine whether the column is the same as the last column clicked.
            if (colIndex != lastClickedColIndex)
            {
                if (isDateColumn)
                {
                    // Sort date columns descending by default
                    lv.Sorting = SortOrder.Descending;
                }
                else
                {
                    // Sort text columns ascending by default
                    lv.Sorting = SortOrder.Ascending;
                }
            }
            // Determine what the last sort order was and change it
            else if (lv.Sorting == SortOrder.Ascending)
            {
                lv.Sorting = SortOrder.Descending;
            }
            else
            {
                lv.Sorting = SortOrder.Ascending;
            }

            // Set the ListViewItemSorter property to a new ListViewItemComparer object
            lv.ListViewItemSorter = new ListViewItemComparer(colIndex, lv.Sorting, isDateColumn);

            // Call the sort method to manually sort
            lv.Sort();
        }

        /// <summary>
        /// Remove leading and trailing whitespace.
        /// Replace tabs and carriage returns with semicolons
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string StripWhiteSpace(string value)
        {
            string updatedValue = value.Trim().Replace("\t", "; ").Replace(Environment.NewLine, "; ").Replace("\r", "; ").Replace("\n", "; ");

            return updatedValue;
        }

        private void UpdateProteinCollectionMetadata()
        {
            var descriptionList = new Dictionary<string, int>();
            var sourceList = new Dictionary<string, int>();

            var itemsToUpdate = new List<ListViewItem>();

            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    string descriptionCurrent = GetSelectedFileColumn(li, eSelectedFileColumn.Description);
                    string sourceCurrent = GetSelectedFileColumn(li, eSelectedFileColumn.Source);

                    if (!string.IsNullOrWhiteSpace(descriptionCurrent))
                    {
                        AddUpdateDictionaryItem(descriptionList, descriptionCurrent);
                    }

                    if (!string.IsNullOrWhiteSpace(sourceCurrent))
                    {
                        AddUpdateDictionaryItem(sourceList, sourceCurrent);
                    }

                    itemsToUpdate.Add(li);
                }
            }
            else
            {
                foreach (ListViewItem li in lvwSelectedFiles.Items)
                    itemsToUpdate.Add(li);
            }

            // Show a window with the most commonly used description and source

            var oMetadataWindow = new frmNewCollectionMetadataEditor()
            {
                Description = MostCommonItem(descriptionList),
                Source = MostCommonItem(sourceList)
            };

            var eDialogResult = oMetadataWindow.ShowDialog();

            if (eDialogResult == DialogResult.OK)
            {
                string updatedDescription = StripWhiteSpace(oMetadataWindow.Description);
                string updatedSource = StripWhiteSpace(oMetadataWindow.Source);

                foreach (var li in itemsToUpdate)
                {
                    li.SubItems[(int)eSelectedFileColumn.Description].Text = updatedDescription;
                    li.SubItems[(int)eSelectedFileColumn.Source].Text = updatedSource;

                    string proteinCollection = li.SubItems[(int)eSelectedFileColumn.ProteinCollectionName].Text;

                    var kvDescriptionSource = new KeyValuePair<string, string>(updatedDescription, updatedSource);

                    if (m_CachedFileDescriptions.ContainsKey(proteinCollection))
                    {
                        m_CachedFileDescriptions[proteinCollection] = kvDescriptionSource;
                    }
                    else
                    {
                        m_CachedFileDescriptions.Add(proteinCollection, kvDescriptionSource);
                    }
                }
            }
        }

        #region "Button and Combo Handlers"

        private void cboOrganismSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            m_SelectedOrganismID = (int)cbo.SelectedValue;
            CheckTransferEnable();
            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    string fastaFilePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath);

                    // Update organism in m_SelectedFileList
                    var tmpUpInfo = m_SelectedFileList[fastaFilePath];
                    tmpUpInfo.OrganismID = m_SelectedOrganismID;

                    m_SelectedFileList[fastaFilePath] = tmpUpInfo;

                    // Update organism in lvwSelectedFiles
                    li.SubItems[(int)eSelectedFileColumn.Organism].Text = cbo.Text;
                }
            }
        }

        private void cboAnnotationTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            PSUploadHandler.UploadInfo tmpUpInfo;

            if (ReferenceEquals(cboAnnotationTypePicker.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                m_SelectedAnnotationTypeID = (int)Math.Round(Convert.ToDouble(cboAnnotationTypePicker.SelectedValue));
            }
            else
            {
                // m_SelectedAuthorityID = 0
            }

            CheckTransferEnable();

            int tmpAnnTypeID;

            if (m_SelectedAnnotationTypeID == -2)
            {
                // Bring up an additional dialog
                var AnnTypeAdd = new AddAnnotationTypeType(m_PSConnectionString);
                AnnTypeAdd.FormLocation = new Point(Left + Width + 10, Top);
                tmpAnnTypeID = AnnTypeAdd.AddAnnotationType();
                // Dim AuthAdd As New AddNamingAuthority(m_PSConnectionString)
                // tmpAuthID = AuthAdd.AddNamingAuthority

                if (!AnnTypeAdd.EntryExists & tmpAnnTypeID > 0)
                {
                    DataRow dr;
                    dr = m_AnnotationTypeList.NewRow();

                    dr["ID"] = tmpAnnTypeID;
                    dr["Display_Name"] = AnnTypeAdd.DisplayName;
                    dr["Details"] = AnnTypeAdd.Description;

                    m_AnnotationTypeList.Rows.Add(dr);
                    m_AnnotationTypeList.AcceptChanges();
                    LoadAnnotationTypePicker(cboAnnotationTypePicker, m_AnnotationTypeList);
                    m_SelectedAnnotationTypeID = tmpAnnTypeID;

                    cboAnnotationTypePicker.SelectedValue = tmpAnnTypeID;
                }
            }

            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    // Update annotation type in m_SelectedFileList
                    string fastaFilePath = GetSelectedFileColumn(li, eSelectedFileColumn.FilePath);
                    tmpUpInfo = m_SelectedFileList[fastaFilePath];

                    m_SelectedFileList[fastaFilePath] =
                        new PSUploadHandler.UploadInfo(
                            tmpUpInfo.FileInformation,
                            m_SelectedOrganismID,
                            m_SelectedAnnotationTypeID);

                    // Update annotation type in lvwSelectedFiles
                    li.SubItems[(int)eSelectedFileColumn.AnnotationType].Text = cbo.Text;
                }
            }
        }

        private void chkEncryptionEnable_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            bool encryptSequences = false;

            if (chk.CheckState == CheckState.Checked)
            {
                txtPassphrase.Enabled = true;
            }
            else
            {
                txtPassphrase.Enabled = false;
            }

            CheckTransferEnable();

            //if (lvwSelectedFiles.SelectedItems.Count > 0)
            //{
            //    foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
            //    {
            //        var fastaFilePath = li.SubItems[(int)eSelectedFileColumn.FilePath].Text;
            //        var tmpUpInfo = (Protein_Uploader.PSUploadHandler.UploadInfo) m_SelectedFileList[fastaFilePath];
            //        if (encryptSequences)
            //        {
            //            tmpUpInfo.EncryptSequences = true;
            //            tmpUpInfo.EncryptionPassphrase = passPhraseForm.Passphrase;
            //            li.SubItems[(int) eSelectedFileColumn.Encrypt].Text = "Yes";
            //        }
            //        else
            //        {
            //            tmpUpInfo.EncryptSequences = false;
            //            tmpUpInfo.EncryptionPassphrase = null;
            //            li.SubItems[(int)eSelectedFileColumn.Encrypt].Text = "No";
            //        }

            //        m_SelectedFileList[fastaFilePath] = tmpUpInfo;
            //    }
            //}
        }

        private void CheckTransferEnable()
        {
            if (chkEncryptionEnable.Checked == true)
            {
                if (m_SelectedOrganismID > 0 &&
                    m_SelectedAnnotationTypeID > 0 &&
                    lvwFolderContents.SelectedItems.Count > 0 &&
                    txtPassphrase.Text.Length > 0)
                {
                    m_AllowAddFiles = true;
                    m_AllowAddFilesMessage = "";
                }
                else
                {
                    m_AllowAddFiles = false;
                    m_AllowAddFilesMessage = ADD_FILES_MESSAGE + "  You also must define a passphrase for encryption.";
                }
            }
            else if (m_SelectedOrganismID > 0 && m_SelectedAnnotationTypeID > 0 && lvwFolderContents.SelectedItems.Count > 0)
            {
                m_AllowAddFiles = true;
                m_AllowAddFilesMessage = "";
            }
            else
            {
                m_AllowAddFiles = false;
                m_AllowAddFilesMessage = ADD_FILES_MESSAGE;
            }

            if (lvwSelectedFiles.Items.Count > 0)
            {
                cmdRemoveFile.Enabled = true;
            }
            else
            {
                cmdRemoveFile.Enabled = false;
            }

            if (lvwSelectedFiles.Items.Count > 0)
            {
                cmdUploadChecked.Enabled = true;
            }
            else
            {
                cmdUploadChecked.Enabled = false;
            }

            if (lvwFolderContents.SelectedItems.Count > 0)
            {
                cmdPreviewFile.Enabled = true;
            }
            else
            {
                cmdPreviewFile.Enabled = false;
            }
        }

        private void cmdUploadChecked_Click(object sender, EventArgs e)
        {
            bool validInfo = MakeCheckedFileList();
            if (!validInfo)
                return;
            m_ReallyClose = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmdAddFile_Click(object sender, EventArgs e)
        {
            if (!m_AllowAddFiles)
            {
                MessageBox.Show(m_AllowAddFilesMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                AddFileToSelectedList();
                CheckTransferEnable();
            }
        }

        private void cmdRemoveFile_Click(object sender, EventArgs e)
        {
            RemoveFileFromSelectedList();
            CheckTransferEnable();
        }

        private void cmdPreviewFile_Click(object sender, EventArgs e)
        {
            if (m_FilePreviewer == null)
            {
                m_FilePreviewer = new FilePreviewHandler();
                m_FilePreviewer.FormStatus += OnPreviewFormStatusChange;
            }

            if (lvwFolderContents.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a file to preview", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var li = lvwFolderContents.SelectedItems[0];
            string fullName = GetFolderContentsColumn(li, eFolderContentsColumn.FilePath);

            m_FilePreviewer.ShowPreview(fullName, Left + Width + 10, Top, Height);
        }

        private void cmdRefreshFiles_Click(object sender, EventArgs e)
        {
            InitializeTreeView(m_LastUsedDirectory);
        }

        private void cmdUpdateDescription_Click(object sender, EventArgs e)
        {
            UpdateProteinCollectionMetadata();
        }

        #endregion

        private void lvwSelectedFiles_Click(object sender, EventArgs e)
        {
            if (lvwSelectedFiles.SelectedItems.Count == 0)
                return;
            var li = lvwSelectedFiles.SelectedItems[0];

            string selectedOrganism = GetSelectedFileColumn(li, eSelectedFileColumn.Organism);

            //if (li.SubItems[(int)eSelectedFileColumn.Encrypt].Text = "Yes")
            //{
            //    chkEncryptionEnable.CheckedChanged -= chkEncryptionEnable_CheckedChanged;
            //    chkEncryptionEnable.CheckState = CheckState.Checked;
            //    chkEncryptionEnable.CheckedChanged += chkEncryptionEnable_CheckedChanged;
            //}
            //else
            //{
            //    chkEncryptionEnable.CheckedChanged -= chkEncryptionEnable_CheckedChanged;
            //    chkEncryptionEnable.CheckState = CheckState.Unchecked;
            //    chkEncryptionEnable.CheckedChanged += chkEncryptionEnable_CheckedChanged;
            //}


            string selFileAnnotationType = GetSelectedFileColumn(li, eSelectedFileColumn.AnnotationType);

            cboAnnotationTypePicker.Text = selFileAnnotationType;
            cboOrganismSelect.Text = selectedOrganism;

            CheckTransferEnable();
        }

        private void lvwFolderContents_Click(object sender, EventArgs e)
        {
            lvwSelectedFiles.SelectedItems.Clear();
        }

        private void frmBatchAddNewCollection_Closing(object sender, CancelEventArgs e)
        {
            if (lvwSelectedFiles.Items.Count > 0 & !m_ReallyClose)
            {
                DialogResult r;

                r = MessageBox.Show("You have files selected for upload. Really close the form?",
                                    "Files selected for upload", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (r == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = false;
                }
            }

            if (m_FilePreviewer != null & m_PreviewFormStatus == true)
            {
                m_FilePreviewer.CloseForm();
                m_FilePreviewer.FormStatus -= OnPreviewFormStatusChange;
                m_FilePreviewer = null;
            }
        }

        private void lvwFolderContents_MouseUp(object sender, MouseEventArgs e)
        {
            CheckTransferEnable();
        }

        public void OnPreviewFormStatusChange(bool Visibility)
        {
            if (Visibility == true)
            {
                cmdPreviewFile.Enabled = false;
                m_PreviewFormStatus = true;
            }
            else
            {
                cmdPreviewFile.Enabled = true;
                m_PreviewFormStatus = false;
            }
        }

        //private void txtPassphrase_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    TextBox txt = (TextBox)sender;

        //    if (txt.Text.Length > 0)
        //    {
        //        if (lvwSelectedFiles.SelectedItems.Count == 0)
        //            m_CachedPassphrase = txt.Text;
        //        else if (lvwSelectedFiles.SelectedItems.Count > 0)
        //        {
        //            foreach (ListViewItem li in lvwSelectedFiles.Items)
        //            {
        //                li.Tag = txt.Text;
        //                li.SubItems[(int)eSelectedFileColumn.Encrypt].Text = "Yes";
        //            }
        //        }
        //        CheckTransferEnable();
        //    }
        //    else
        //    {
        //        CheckTransferEnable();
        //        return;
        //    }
        //}


        private void txtMaximumProteinNameLength_Validating(object sender, CancelEventArgs e)
        {
            if (txtMaximumProteinNameLength.TextLength == 0)
            {
                txtMaximumProteinNameLength.Text = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString();
            }
            else
            {
                int intValue = 0;
                if (int.TryParse(txtMaximumProteinNameLength.Text, out intValue))
                {
                    if (intValue < 30)
                    {
                        txtMaximumProteinNameLength.Text = "30";
                    }
                }
                else
                {
                    txtMaximumProteinNameLength.Text = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString();
                }
            }
        }

        private void lvwSelectedFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortListView(lvwSelectedFiles, mSortColumnSelectedItems, e.Column, isDateColumn: false);
            mSortColumnSelectedItems = e.Column;
        }

        private void lvwSelectedFiles_DoubleClick(object sender, EventArgs e)
        {
            UpdateProteinCollectionMetadata();
        }

        private void lvwSelectedFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                // Select all rows
                SelectAllRows(lvwSelectedFiles);
            }
        }

        private void lvwFolderContents_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            bool isDateColumn = e.Column == (int)eFolderContentsColumn.LastModified;
            SortListView(lvwFolderContents, mSortColumnFolderContents, e.Column, isDateColumn);
            mSortColumnFolderContents = e.Column;
        }

        private void lvwFolderContents_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                // Select all rows
                SelectAllRows(lvwFolderContents);
            }
        }

        private void ctlTreeViewFolderBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AfterNodeSelect();
        }

        private void ctlTreeViewFolderBrowser_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                string folderPath = SelectedNodeFolderPath;
                if (string.IsNullOrWhiteSpace(folderPath))
                    return;
                Clipboard.SetText(folderPath);
                UpdateStatus("Folder path copied to the clipboard");
            }
        }

        private void ctlTreeViewFolderBrowser_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                try
                {
                    string folderPath = SelectedNodeFolderPath;
                    if (string.IsNullOrWhiteSpace(folderPath))
                        return;
                    var currentFolder = new DirectoryInfo(folderPath);

                    while (!currentFolder.Exists && currentFolder.Parent != null)
                        currentFolder = currentFolder.Parent;

                    ctlTreeViewFolderBrowser.Populate(currentFolder.FullName);

                    InitializeTreeView(currentFolder.FullName);

                    if (!ctlTreeViewFolderBrowser.SelectedNode.IsExpanded)
                    {
                        ctlTreeViewFolderBrowser.SelectedNode.Expand();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in NodeMouseDoubleClick: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void m_StatusResetTimer_Tick(object sender, EventArgs e)
        {
            if (m_StatusResetRequired && DateTime.UtcNow > m_StatusClearTime)
            {
                m_StatusResetRequired = false;
                lblStatus.Text = string.Empty;
            }
        }

        private class ListViewItemComparer : IComparer
        {
            private readonly bool mSortingDates;
            private readonly int mColIndex;
            private readonly SortOrder mSortOrder;

            public ListViewItemComparer(int column, SortOrder order, bool sortingDates = false)
            {
                mSortingDates = sortingDates;
                mColIndex = column;
                mSortOrder = order;
            }

            public int Compare(object x, object y)
            {
                var returnVal = default(int);
                bool compared = false;

                if (mSortingDates)
                {
                    try
                    {
                        // Parse the two objects passed as a parameter as a DateTi
                        var dateA = DateTime.Parse(((ListViewItem)x).SubItems[mColIndex].Text);
                        var dateB = DateTime.Parse(((ListViewItem)y).SubItems[mColIndex].Text);

                        // Compare the two dates.
                        returnVal = DateTime.Compare(dateA, dateB);
                        compared = true;
                    }
                    catch
                    {
                        // Sort as strings
                    }
                }

                if (!compared)
                {
                    // Compare the two items as a string.
                    returnVal = string.Compare(((ListViewItem)x).SubItems[mColIndex].Text,
                                               ((ListViewItem)y).SubItems[mColIndex].Text);
                }

                if (mSortOrder == SortOrder.Descending)
                {
                    return returnVal * -1;
                }
                else
                {
                    return returnVal;
                }
            }
        }
    }
}