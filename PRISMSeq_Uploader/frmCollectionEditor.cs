﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using PRISMSeq_Uploader.Properties;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.ProteinUpload;
using PRISM;
using PRISMDatabaseUtils;
using ValidateFastaFile;

namespace PRISMSeq_Uploader
{
    public partial class frmCollectionEditor : Form
    {
        // Ignore Spelling: frm, prismdb2, proteinseqs

        public frmCollectionEditor()
        {
            ProgramDate = ThisAssembly.GitCommitDate.ToLocalTime().ToString("MMMM dd, yyyy");

            // Initialize Dictionaries
            mFileErrorList = new Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>>();
            mCachedFileDescriptions = new Dictionary<string, KeyValuePair<string, string>>();
            mValidUploadsList = new Dictionary<string, PSUploadHandler.UploadInfo>();
            mSummarizedFileErrorList = new Dictionary<string, Dictionary<string, int>>();
            mFileWarningList = new Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>>();
            mSummarizedFileWarningList = new Dictionary<string, Dictionary<string, int>>();

            mSearchTimer = new System.Timers.Timer(2000d);
            mSearchTimer.Elapsed += SearchTimerHandler;

            Load += frmCollectionEditor_Load;

            InitializeComponent();

            CheckTransferButtonsEnabledStatus();

            ReadSettings();
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new frmCollectionEditor());
        }

        private readonly string ProgramDate;

        private DataTable mOrganisms;
        private DataTable mProteinCollections;
        private DataTable mProteinCollectionNames;
        private DataTable mAnnotationTypes;
        private DataTable mCollectionMembers;
        private int mSelectedOrganismId;
        private int mSelectedAnnotationTypeId;
        private string mSelectedFilePath;
        private int mSelectedCollectionId;
        private string mLastBatchUploadDirectoryPath;

        /// <summary>
        /// Protein sequences database connection string
        /// </summary>
        private string mPsConnectionString = "Host=prismdb2.emsl.pnl.gov;Port=5432;Database=dms;UserId=pceditor";

        private string mLastSelectedOrganism = string.Empty;
        private string mLastSelectedAnnotationType = string.Empty;
        private bool mLastValueForAllowAsterisks;
        private bool mLastValueForAllowDash;
        private int mLastValueForMaxProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
        private ImportHandler mImportHandler;
        private PSUploadHandler mUploadHandler;
        private DataListViewHandler mSourceListViewHandler;
        // Unused: private BatchUploadFromFileList mFileBatcher;

        private bool mLocalFileLoaded;

        private bool mSearchActive;

        private int mBatchLoadTotalCount;
        private int mBatchLoadCurrentCount;

        /// <summary>
        /// Keys are FASTA file names, values are lists of errors
        /// </summary>
        private readonly Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> mFileErrorList;

        /// <summary>
        /// Keys are FASTA file names, values are dictionaries of error messages, tracking the count of each error
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, int>> mSummarizedFileErrorList;

        /// <summary>
        /// Keys are FASTA file names, values are lists of warnings
        /// </summary>
        private readonly Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> mFileWarningList;

        /// <summary>
        /// Keys are FASTA file names, values are dictionaries of warning messages, tracking the count of each warning
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, int>> mSummarizedFileWarningList;

        /// <summary>
        /// Keys are FASTA file paths
        /// Values are upload info
        /// </summary>
        private readonly Dictionary<string, PSUploadHandler.UploadInfo> mValidUploadsList;

#pragma warning disable CS0618 // Type or member is obsolete
        private SyncFASTAFileArchive mSyncer;
#pragma warning restore CS0618 // Type or member is obsolete

        private readonly System.Timers.Timer mSearchTimer;

        /// <summary>
        /// Tracks the description and source that the user has entered for each FASTA file
        /// Key: FASTA file name
        /// Value: KeyValuePair of Description and Source
        /// </summary>
        /// <remarks>Useful in case validation fails and the uploader needs to try again to upload a FASTA file</remarks>
        private readonly Dictionary<string, KeyValuePair<string, string>> mCachedFileDescriptions;

        private void frmCollectionEditor_Load(object sender, EventArgs e)
        {
            // Get initial info - organism list, full collections list

            // Prior to July 2024:      Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI
            // Starting in July 2024:   host=prismdb2;Port=5432;Database=dms;UserId=svc-dms
            // Starting in August 2024: Host=prismdb2.emsl.pnl.gov;Port=5432;Database=dms;UserId=pceditor

            var connectionString = Settings.Default.ProteinSeqsDBConnectStr;

            // Uncomment to upload to the test database
            // connectionString = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences_T3;Integrated Security=SSPI";

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                mPsConnectionString = connectionString;
            }

            UpdateServerNameLabel();

            mImportHandler = new ImportHandler(mPsConnectionString);
            mImportHandler.LoadStart += ImportStartHandler;
            mImportHandler.LoadProgress += ImportProgressHandler;
            mImportHandler.LoadEnd += ImportEndHandler;
            mImportHandler.CollectionLoadComplete += CollectionLoadHandler;
            //mnuToolsFBatchUpload.Enabled = false;

            lblBatchProgress.Text = "Fetching Organism and Collection Lists...";

            cboCollectionPicker.Enabled = false;
            cboOrganismFilter.Enabled = false;

            TriggerCollectionTableUpdate(false);

            mSourceListViewHandler = new DataListViewHandler(lvwSource);

            cmdLoadProteins.Enabled = false;
            txtLiveSearch.Visible = false;
            pbxLiveSearchBkg.Visible = false;
            pbxLiveSearchCancel.Visible = false;
            lblSearchCount.Text = string.Empty;
            cmdExportToFile.Enabled = false;
            cmdSaveDestCollection.Enabled = false;

            cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
            lblBatchProgress.Text = string.Empty;

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
            if (mSelectedOrganismId != -1 && mSelectedCollectionId != -1)
            {
                cboAnnotationTypePicker.SelectedIndexChanged -= cboAnnotationTypePicker_SelectedIndexChanged;
                cboCollectionPicker.SelectedIndexChanged -= cboCollectionPicker_SelectedIndexChanged;
                cboOrganismFilter.SelectedItem = mSelectedOrganismId;
                cboOrganismList_SelectedIndexChanged(this, null);

                cboCollectionPicker.SelectedItem = mSelectedCollectionId;
                cboAnnotationTypePicker.SelectedItem = mSelectedAnnotationTypeId;
                cboCollectionPicker.Select();
                cboCollectionPicker.SelectedIndexChanged += cboCollectionPicker_SelectedIndexChanged;
                cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
            }
        }

        private void TriggerCollectionTableUpdate(bool refreshTable)
        {
            if (refreshTable)
            {
                mImportHandler.TriggerProteinCollectionTableUpdate();
            }
            // CollectionLoadThread = New System.Threading.Thread(AddressOf mImportHandler.TriggerProteinCollectionsLoad)
            // CollectionLoadThread.Start()
            if (mSelectedOrganismId > 0)
            {
                mImportHandler.TriggerProteinCollectionsLoad(mSelectedOrganismId);
            }
            else
            {
                mImportHandler.TriggerProteinCollectionsLoad();
            }
        }

        private void BindOrganismListToControl(DataTable organismList)
        {
            cboOrganismFilter.BeginUpdate();
            cboOrganismFilter.DataSource = organismList;
            cboOrganismFilter.DisplayMember = "display_name";
            cboOrganismFilter.ValueMember = "id";

            cboOrganismFilter.EndUpdate();
        }

        private void BindAnnotationTypeListToControl(DataTable annotationTypeList)
        {
            cboAnnotationTypePicker.BeginUpdate();
            cboAnnotationTypePicker.DisplayMember = "display_name";
            // cboAnnotationTypePicker.DisplayMember = "name";
            cboAnnotationTypePicker.ValueMember = "id";
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
                cboCollectionPicker.DisplayMember = "display";
                cboCollectionPicker.ValueMember = "protein_collection_id";

                cmdLoadProteins.Enabled = true;
            }

            cboCollectionPicker.EndUpdate();
        }

        private void BatchLoadController()
        {
            mProteinCollectionNames = mImportHandler.LoadProteinCollectionNames();
            mFileErrorList?.Clear();
            mFileWarningList?.Clear();
            mValidUploadsList?.Clear();
            mSummarizedFileErrorList?.Clear();
            mSummarizedFileWarningList?.Clear();

            var frmBatchUpload = new frmBatchAddNewCollection(
                mImportHandler,
                mOrganisms,
                mAnnotationTypes,
                mProteinCollectionNames,
                mPsConnectionString,
                mLastBatchUploadDirectoryPath,
                mCachedFileDescriptions);

            lblBatchProgress.Text = string.Empty;

            if (!string.IsNullOrEmpty(mLastSelectedOrganism))
            {
                frmBatchUpload.SelectedOrganismName = mLastSelectedOrganism;
            }

            if (!string.IsNullOrEmpty(mLastSelectedAnnotationType))
            {
                frmBatchUpload.SelectedAnnotationType = mLastSelectedAnnotationType;
            }

            frmBatchUpload.ValidationAllowAsterisks = mLastValueForAllowAsterisks;
            frmBatchUpload.ValidationAllowDash = mLastValueForAllowDash;
            frmBatchUpload.ValidationMaxProteinNameLength = mLastValueForMaxProteinNameLength;

            // Show the window
            var resultReturn = frmBatchUpload.ShowDialog();

            // Save the selected organism and annotation type
            mLastSelectedOrganism = frmBatchUpload.SelectedOrganismName;
            mLastSelectedAnnotationType = frmBatchUpload.SelectedAnnotationType;
            mLastValueForAllowAsterisks = frmBatchUpload.ValidationAllowAsterisks;
            mLastValueForAllowDash = frmBatchUpload.ValidationAllowDash;
            mLastValueForMaxProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength;

            mLastBatchUploadDirectoryPath = frmBatchUpload.CurrentDirectory;

            try
            {
                // Save these settings to the registry
                if (!string.IsNullOrEmpty(mLastSelectedOrganism) && !mLastSelectedOrganism.Equals(" - "))
                {
                    //Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", mLastSelectedOrganism);
                    Settings.Default.LastSelectedOrganism = mLastSelectedOrganism;
                }

                if (!string.IsNullOrEmpty(mLastSelectedAnnotationType) && !mLastSelectedAnnotationType.StartsWith(" -- None"))
                {
                    //Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", mLastSelectedAnnotationType);
                    Settings.Default.LastSelectedAnnotationType = mLastSelectedAnnotationType;
                }

                if (!string.IsNullOrEmpty(mLastBatchUploadDirectoryPath))
                {
                    //Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", mLastBatchULDirectoryPath);
                    Settings.Default.LastBatchULDirectoryPath = mLastBatchUploadDirectoryPath;
                }

                Settings.Default.Save();
            }
            catch (Exception)
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

            var selectedFileList = frmBatchUpload.FileList;

            mBatchLoadTotalCount = selectedFileList.Count;

            if (mUploadHandler != null)
            {
                mUploadHandler.BatchProgress -= BatchImportProgressHandler;
                mUploadHandler.LoadProgress -= ImportProgressHandler;
                mUploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                mUploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                mUploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                mUploadHandler.ValidationProgress -= ValidationProgressHandler;
                mUploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
            }

            mUploadHandler = new PSUploadHandler(mPsConnectionString);
            RegisterEvents(mUploadHandler);

            mUploadHandler.BatchProgress += BatchImportProgressHandler;
            mUploadHandler.LoadProgress += ImportProgressHandler;
            mUploadHandler.ValidFASTAFileLoaded += ValidFASTAUploadHandler;
            mUploadHandler.InvalidFASTAFile += InvalidFASTAFileHandler;
            mUploadHandler.FASTAFileWarnings += FASTAFileWarningsHandler;
            mUploadHandler.ValidationProgress += ValidationProgressHandler;
            mUploadHandler.WroteLineEndNormalizedFASTA += NormalizedFASTAFileGenerationHandler;
            mUploadHandler.LoadStart += LoadStartHandler;

            pgbMain.Value = 0;
            lblCurrentTask.Text = string.Empty;
            pnlProgBar.Visible = true;

            try
            {
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAllSymbolsInProteinNames, frmBatchUpload.ValidationAllowAllSymbolsInProteinNames);
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAsterisksInResidues, frmBatchUpload.ValidationAllowAsterisks);
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowDashInResidues, frmBatchUpload.ValidationAllowDash);

                mUploadHandler.MaximumProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength;

                mUploadHandler.BatchUpload(selectedFileList);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error uploading collection: " + ex.Message, "Error");
            }

            pnlProgBar.Visible = false;

            // Display any errors that occurred
            var errorDisplay = new frmValidationReport
            {
                FileErrorList = mFileErrorList,
                FileWarningList = mFileWarningList,
                FileValidList = mValidUploadsList,
                ErrorSummaryList = mSummarizedFileErrorList,
                WarningSummaryList = mSummarizedFileWarningList,
                OrganismList = mOrganisms
            };
            errorDisplay.ShowDialog();

            lblBatchProgress.Text = "Updating Protein Collections List...";
            Application.DoEvents();

            TriggerCollectionTableUpdate(true);

            RefreshCollectionList();
            mUploadHandler.ResetErrorList();

            lblBatchProgress.Text = string.Empty;
            gbxSourceCollection.Enabled = true;
            gbxDestinationCollection.Enabled = true;
            cmdDestAdd.Enabled = true;
            cmdDestAddAll.Enabled = true;
            cmdDestRemove.Enabled = true;
            cmdDestRemoveAll.Enabled = true;

            mBatchLoadCurrentCount = 0;
        }

        private void ReadSettings()
        {
            try
            {
                //mLastSelectedOrganism = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", "");
                //mLastSelectedAnnotationType = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", "");
                //mLastBatchULDirectoryPath = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", "");
                mLastSelectedOrganism = Settings.Default.LastSelectedOrganism ?? "";
                mLastSelectedAnnotationType = Settings.Default.LastSelectedAnnotationType ?? "";
                mLastBatchUploadDirectoryPath = Settings.Default.LastBatchULDirectoryPath ?? "";
            }
            catch (Exception)
            {
                // Ignore errors here
            }
        }

        private void ShowAboutBox()
        {
            //var AboutBox = new frmAboutBox;

            //AboutBox.Location = mMainProcess.myAppSettings.AboutBoxLocation;
            //AboutBox.ShowDialog();

            var strMessage = "This is version " + Application.ProductVersion + ", " + ProgramDate;

            MessageBox.Show(strMessage, "About Protein Collection Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateServerNameLabel()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mPsConnectionString))
                {
                    lblTargetDatabase.Text = "ERROR determining target database: mPSConnectionString is empty";
                    return;
                }

                var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mPsConnectionString, "PRISMSeq_Uploader");

                var dbTools = DbToolsFactory.GetDBTools(connectionStringToUse);

                lblTargetDatabase.Text = string.Format("Target database: {0} (on {1})", dbTools.DatabaseName, dbTools.ServerName);
            }
            catch (Exception ex)
            {
                lblTargetDatabase.Text = "ERROR determining target database: " + ex.Message;
            }
        }

        #region "ComboBox handlers"

        private void cboOrganismList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cboOrganismFilter.SelectedValue) != 0)
            {
                mProteinCollections.DefaultView.RowFilter = "[Organism_ID] = " + cboOrganismFilter.SelectedValue;
            }
            else
            {
                mProteinCollections.DefaultView.RowFilter = string.Empty;
            }

            mSelectedOrganismId = Convert.ToInt32(cboOrganismFilter.SelectedValue);

            BindCollectionListToControl(mProteinCollections.DefaultView);

            if (lvwSource.Items.Count == 0)
            {
                cboCollectionPicker_SelectedIndexChanged(this, null);
            }
        }

        private void cboCollectionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvwSource.Items.Clear();
            mImportHandler.ClearProteinCollection();
            mSelectedCollectionId = Convert.ToInt32(cboCollectionPicker.SelectedValue);

            if (mSelectedCollectionId > 0)
            {
                var foundRows = mProteinCollections.Select("[Protein_Collection_ID] = " + mSelectedCollectionId);
                mSelectedAnnotationTypeId = Convert.ToInt32(foundRows[0]["Authority_ID"]);
                //mAnnotationTypes = mImportHandler.LoadAnnotationTypes(mSelectedCollectionID);
                //mAnnotationTypes = mImportHandler.LoadAnnotationTypes();
                cmdLoadProteins.Enabled = true;
            }
            else
            {
                mAnnotationTypes = mImportHandler.LoadAnnotationTypes();
                cmdLoadProteins.Enabled = false;
            }

            BindAnnotationTypeListToControl(mAnnotationTypes);
        }

        private void cboAnnotationTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwSource.Items.Count > 0)
            {
                lvwSource.Items.Clear();
                mImportHandler.ClearProteinCollection();
            }

            if (ReferenceEquals(cboAnnotationTypePicker.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                mSelectedAnnotationTypeId = Convert.ToInt32(cboAnnotationTypePicker.SelectedValue);
            }

            if (mSelectedCollectionId > 0)
            {
                var foundRows = mProteinCollections.Select("[Protein_Collection_ID] = " + mSelectedCollectionId);
                mSelectedAnnotationTypeId = Convert.ToInt32(foundRows[0]["Authority_ID"]);
            }
        }

        #endregion

        #region "Action Button Handlers"

        private void cmdLoadProteins_Click(object sender, EventArgs e)
        {
            ImportStartHandler("Retrieving Protein Entries..");
            var foundRows = mProteinCollections.Select("Protein_Collection_ID = " + cboCollectionPicker.SelectedValue);

            ImportProgressHandler(0.5d);
            mSelectedFilePath = foundRows[0]["Collection_Name"].ToString();
            UpdateCachedInfoAfterLoadingProteins();

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
            if (lvwDestination.Items.Count <= 0)
            {
                if (mUploadHandler != null)
                {
                    mUploadHandler.BatchProgress -= BatchImportProgressHandler;
                    mUploadHandler.LoadProgress -= ImportProgressHandler;
                    mUploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                    mUploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                    mUploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                    mUploadHandler.ValidationProgress -= ValidationProgressHandler;
                    mUploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
                }

                mUploadHandler = null;
                return;
            }

            var frmAddCollection = new frmAddNewCollection
            {
                CollectionName = Path.GetFileNameWithoutExtension(mSelectedFilePath),
                IsLocalFile = mLocalFileLoaded,
                AnnotationTypes = mAnnotationTypes,
                OrganismList = mOrganisms,
                OrganismId = mSelectedOrganismId,
                AnnotationTypeId = mSelectedAnnotationTypeId
            };

            var eResult = frmAddCollection.ShowDialog();

            if (eResult == DialogResult.OK)
            {
                cboCollectionPicker.Enabled = true;
                cboOrganismFilter.Enabled = true;

                var organismId = frmAddCollection.OrganismId;
                var annotationTypeId = frmAddCollection.AnnotationTypeId;

                var selectedProteins = ScanDestinationCollectionWindow(lvwDestination);

                if (mUploadHandler == null)
                {
                    mUploadHandler = new PSUploadHandler(mPsConnectionString);
                    RegisterEvents(mUploadHandler);

                    mUploadHandler.BatchProgress += BatchImportProgressHandler;
                    mUploadHandler.LoadProgress += ImportProgressHandler;
                    mUploadHandler.ValidFASTAFileLoaded += ValidFASTAUploadHandler;
                    mUploadHandler.InvalidFASTAFile += InvalidFASTAFileHandler;
                    mUploadHandler.FASTAFileWarnings += FASTAFileWarningsHandler;
                    mUploadHandler.ValidationProgress += ValidationProgressHandler;
                    mUploadHandler.WroteLineEndNormalizedFASTA += NormalizedFASTAFileGenerationHandler;
                }

                mUploadHandler.UploadCollection(mImportHandler.CollectionMembers, selectedProteins,
                                                 frmAddCollection.CollectionName, frmAddCollection.CollectionDescription,
                                                 frmAddCollection.CollectionSource,
                                                 AddUpdateEntries.CollectionTypes.ProtOriginalSource, organismId,
                                                 annotationTypeId);

                RefreshCollectionList();

                ClearFromDestinationCollectionWindow(lvwDestination, true);

                cboOrganismFilter.Enabled = true;
                cboCollectionPicker.Enabled = true;
                cboOrganismFilter.SelectedValue = organismId;
            }

            if (mUploadHandler != null)
            {
                mUploadHandler.BatchProgress -= BatchImportProgressHandler;
                mUploadHandler.LoadProgress -= ImportProgressHandler;
                mUploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                mUploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                mUploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                mUploadHandler.ValidationProgress -= ValidationProgressHandler;
                mUploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
            }

            mUploadHandler = null;
        }

        #endregion

        #region "LiveSearch Handlers"

        private void txtLiveSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length > 0 && txtLiveSearch.ForeColor.ToString() != "Color [InactiveCaption]")
            {
                mSearchTimer.Start();
            }
            else if (string.IsNullOrEmpty(txtLiveSearch.Text) && !mSearchActive)
            {
                pbxLiveSearchCancel_Click(this, null);
            }
            else
            {
                mSearchActive = false;
                mSearchTimer.Stop();
            }
        }

        private void txtLiveSearch_Click(object sender, EventArgs e)
        {
            if (mSearchActive)
            {
                return;
            }

            txtLiveSearch.TextChanged -= txtLiveSearch_TextChanged;
            txtLiveSearch.Text = null;
            txtLiveSearch.ForeColor = SystemColors.ControlText;
            mSearchActive = true;
            pbxLiveSearchCancel.Visible = true;
            txtLiveSearch.TextChanged += txtLiveSearch_TextChanged;
        }

        private void txtLiveSearch_Leave(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length == 0)
            {
                txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
                txtLiveSearch.Text = "Search";
                mSearchActive = false;
                mSearchTimer.Stop();
                mSourceListViewHandler.Load(mCollectionMembers);
            }
        }

        private void pbxLiveSearchCancel_Click(object sender, EventArgs e)
        {
            txtLiveSearch.Text = string.Empty;
            txtLiveSearch_Leave(this, null);
            lvwSource.Focus();
            pbxLiveSearchCancel.Visible = false;
        }

        internal void SearchTimerHandler(
            object sender,
            ElapsedEventArgs e)
        {
            if (mSearchActive)
            {
                mSourceListViewHandler.Load(mCollectionMembers, txtLiveSearch.Text);
                mSearchActive = false;
                mSearchTimer.Stop();
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

        // Double-click to remove selected member from the destination collection
        private void lvwDestination_DoubleClick(
            object sender,
            EventArgs e)
        {
            ClearFromDestinationCollectionWindow(lvwDestination, false);
        }

        internal void UpdateCachedInfoAfterLoadingProteins()
        {
            mSelectedCollectionId = Convert.ToInt32(cboCollectionPicker.SelectedValue);
            mSelectedAnnotationTypeId = Convert.ToInt32(cboAnnotationTypePicker.SelectedValue);

            mCollectionMembers = mImportHandler.LoadCollectionMembersById(mSelectedCollectionId, mSelectedAnnotationTypeId);
            mLocalFileLoaded = false;

            mSourceListViewHandler.Load(mCollectionMembers);
            lvwSource.Focus();
            lvwSource.Enabled = true;
        }

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

        private void ScanSourceCollectionWindow(ListView lvwSrc, ListView lvwDest, bool selectAll)
        {
            ListViewItem entry;

            if (selectAll)
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
            {
                selectedList.Add(li.Text);
            }

            return selectedList;
        }

        private void ClearFromDestinationCollectionWindow(ListView lvwDest, bool selectAll)
        {
            if (selectAll)
            {
                lvwDest.Items.Clear();
                cmdSaveDestCollection.Enabled = false;
                cmdExportToFile.Enabled = false;
            }
            else
            {
                foreach (ListViewItem entry in lvwDest.SelectedItems)
                {
                    entry.Remove();
                }
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
        //    mFileBatcher = new BatchUploadFromFileList(mPSConnectionString);
        //    mFileBatcher.UploadBatch();
        //}

        [Obsolete("The tools menu is hidden")]
        private void mnuToolsCollectionEdit_Click(object sender, EventArgs e)
        {
            var cse = new frmCollectionStateEditor(mPsConnectionString);
            cse.ShowDialog();
        }

        // Unused
        //private void mnuToolsExtractFromFile_Click(object sender, EventArgs e)
        //{
        //    var f = new frmExtractFromFlatFile(mImportHandler.Authorities, mPsConnectionString);
        //    f.ShowDialog();
        //}

        // Unused
        //private void mnuToolsUpdateArchives_Click(object sender, EventArgs e)
        //{
        //    var f = new FolderBrowserDialog();
        //    string outputPath = string.Empty;

        //    if (mSyncer == null)
        //        mSyncer = new SyncFASTAFileArchive(mPSConnectionString);

        //    f.RootFolder = Environment.SpecialFolder.MyComputer;
        //    f.ShowNewFolderButton = true;

        //    var r = f.ShowDialog();

        //    if (r == DialogResult.OK)
        //    {
        //        outputPath = f.SelectedPath;
        //        int errorCode = mSyncer.SyncCollectionsAndArchiveTables(outputPath);
        //    }
        //}

        #endregion

        #region "Progress Event Handlers"

        private void ImportStartHandler(string taskTitle)
        {
            //mFileBatcher.LoadStart();

            pgbMain.Value = 0;
            lblCurrentTask.Text = taskTitle;
            pnlProgBar.Visible = true;
            Application.DoEvents();
        }

        private void ImportProgressHandler(double fractionDone)
        {
            //mFileBatcher.ProgressUpdate()
            pgbMain.Value = (int)Math.Round(fractionDone * 100d);
            Application.DoEvents();
        }

        private void LoadStartHandler(string taskTitle)
        {
            pgbMain.Value = 0;
            lblCurrentTask.Text = taskTitle;
            pnlProgBar.Visible = true;
            Application.DoEvents();
        }

        private void SyncProgressHandler(string statusMsg, double fractionDone)
        {
            lblBatchProgress.Text = statusMsg;

            if (fractionDone > 1.0d)
            {
                fractionDone = 1.0d;
            }

            ImportProgressHandler(fractionDone);
        }

        private void ImportEndHandler()
        {
            //mFileBatcher.LoadEnd()

            lblCurrentTask.Text = "Complete: " + lblCurrentTask.Text;
            Invalidate();
            gbxDestinationCollection.Invalidate();
            gbxSourceCollection.Invalidate();
            Application.DoEvents();
        }

        private void CollectionLoadHandler(DataTable collectionTable)
        {
            mProteinCollections = collectionTable;
            mOrganisms ??= mImportHandler.LoadOrganisms();

            mAnnotationTypes ??= mImportHandler.LoadAnnotationTypes();

            BindOrganismListToControl(mOrganisms);
            BindAnnotationTypeListToControl(mAnnotationTypes);
            mProteinCollections.DefaultView.RowFilter = string.Empty;
            BindCollectionListToControl(mProteinCollections.DefaultView);
            cboCollectionPicker.Enabled = true;
            cboOrganismFilter.Enabled = true;
            lblBatchProgress.Text = string.Empty;
            //mnuToolsFBatchUpload.Enabled = true;

            cboOrganismFilter.SelectedIndexChanged += cboOrganismList_SelectedIndexChanged;
            cboCollectionPicker.SelectedIndexChanged += cboCollectionPicker_SelectedIndexChanged;
        }

        private void BatchImportProgressHandler(string status)
        {
            mBatchLoadCurrentCount++;
            lblBatchProgress.Text = status + " (File " + mBatchLoadCurrentCount + " of " + mBatchLoadTotalCount + ")";
            Application.DoEvents();
        }

        private void ValidFASTAUploadHandler(
            string fastaFilePath,
            PSUploadHandler.UploadInfo uploadInfo)
        {
            mValidUploadsList.Add(fastaFilePath, uploadInfo);
        }

        private void InvalidFASTAFileHandler(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> errorCollection)
        {
            mFileErrorList.Add(Path.GetFileName(fastaFilePath), errorCollection);
            mSummarizedFileErrorList.Add(Path.GetFileName(fastaFilePath), SummarizeErrors(errorCollection));
        }

        private void FASTAFileWarningsHandler(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> warningCollection)
        {
            mFileWarningList.Add(Path.GetFileName(fastaFilePath), warningCollection);

            mSummarizedFileWarningList.Add(Path.GetFileName(fastaFilePath), SummarizeErrors(warningCollection));
        }

        private Dictionary<string, int> SummarizeErrors(IReadOnlyCollection<CustomFastaValidator.ErrorInfoExtended> errorCollection)
        {
            // Keys are error messages, values are the number of times the error was reported
            var errorSummary = new Dictionary<string, int>();

            if (errorCollection?.Count > 0)
            {
                foreach (var errorEntry in errorCollection)
                {
                    var message = errorEntry.MessageText;

                    if (errorSummary.TryGetValue(message, out var currentCount))
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

            ImportProgressHandler(fractionDone);
        }

        private void NormalizedFASTAFileGenerationHandler(string newFilePath)
        {
        }

        /// <summary>
        /// Use this method to chain events between classes
        /// </summary>
        /// <param name="sourceClass"></param>
        protected void RegisterEvents(EventNotifier sourceClass)
        {
            // sourceClass.DebugEvent += OnDebugEvent;
            // sourceClass.StatusEvent += OnStatusEvent;
            sourceClass.ErrorEvent += OnErrorEvent;
            sourceClass.WarningEvent += OnWarningEvent;
            // sourceClass.ProgressUpdate += OnProgressUpdate;
        }

        private void OnWarningEvent(string message)
        {
            MessageBox.Show("Warning: " + message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OnErrorEvent(string message, Exception ex)
        {
            MessageBox.Show("Error: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        #endregion

        /// <summary>
        /// Valid, but could take a very long time
        /// Thus, the menu item is disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete("Referenced by hidden menu")]
        private void mnuAdminNameHashRefresh_Click(object sender, EventArgs e)
        {
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(mPsConnectionString);
                mSyncer.SyncProgress += SyncProgressHandler;
            }

            mSyncer.RefreshNameHashes();
        }

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            var frmTesting = new frmTestingInterface();
            frmTesting.Show();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }
    }
}