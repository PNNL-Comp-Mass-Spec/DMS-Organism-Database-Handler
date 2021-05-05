using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using AppUI_OrfDBHandler.Properties;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.ProteinUpload;
using ValidateFastaFile;

namespace AppUI_OrfDBHandler
{
    public partial class frmCollectionEditor : Form
    {
        public frmCollectionEditor()
        {
            // Initialize Dictionaries
            mFileErrorList = new Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>>();
            mCachedFileDescriptions = new Dictionary<string, KeyValuePair<string, string>>();
            mValidUploadsList = new Dictionary<string, PSUploadHandler.UploadInfo>();
            mSummarizedFileErrorList = new Dictionary<string, Dictionary<string, int>>();
            mFileWarningList = new Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>>();
            mSummarizedFileWarningList = new Dictionary<string, Dictionary<string, int>>();

            searchTimer = new System.Timers.Timer(2000d);
            searchTimer.Elapsed += SearchTimerHandler;
            memberLoadTimer = new System.Timers.Timer(2000d);
            memberLoadTimer.Elapsed += MemberLoadTimerHandler;
            base.Load += frmCollectionEditor_Load;

            InitializeComponent();

            CheckTransferButtonsEnabledStatus();

            ReadSettings();
        }

        [STAThread()]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new frmCollectionEditor());
        }

        private const string ProgramDate = "May 5, 2021";

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
        private string mPsConnectionString = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;";

        private string mLastSelectedOrganism = "";
        private string mLastSelectedAnnotationType = "";
        private bool mLastValueForAllowAsterisks = false;
        private bool mLastValueForAllowDash = false;
        private int mLastValueForMaxProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
        private ImportHandler mImportHandler;
        private PSUploadHandler mUploadHandler;
        private DataListViewHandler mSourceListViewHandler;
        // Unused: private BatchUploadFromFileList mFileBatcher;

        private bool mLocalFileLoaded;

        private bool mSearchActive = false;

        private int mBatchLoadTotalCount;
        private int mBatchLoadCurrentCount;

        /// <summary>
        /// Keys are fasta file names, values are lists of errors
        /// </summary>
        private readonly Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> mFileErrorList;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of error messages, tracking the count of each error
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, int>> mSummarizedFileErrorList;

        /// <summary>
        /// Keys are fasta file names, values are lists of warnings
        /// </summary>
        private readonly Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> mFileWarningList;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of warning messages, tracking the count of each warning
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, int>> mSummarizedFileWarningList;

        /// <summary>
        /// Keys are FASTA file paths
        /// Values are upload info
        /// </summary>
        private readonly Dictionary<string, PSUploadHandler.UploadInfo> mValidUploadsList;

        private SyncFASTAFileArchive mSyncer;

        private readonly System.Timers.Timer searchTimer;
        private readonly System.Timers.Timer memberLoadTimer;

        /// <summary>
        /// Tracks the description and source that the user has entered for each FASTA file
        /// Key: fasta file name
        /// Value: KeyValuePair of Description and Source
        /// </summary>
        /// <remarks>Useful in case validation fails and the uploader needs to try again to upload a FASTA file</remarks>
        private readonly Dictionary<string, KeyValuePair<string, string>> mCachedFileDescriptions;

        private void frmCollectionEditor_Load(object sender, EventArgs e)
        {
            // Get initial info - organism list, full collections list

            // Data Source=proteinseqs;Initial Catalog=Protein_Sequences
            var connectionString = Settings.Default.ProteinSeqsDBConnectStr;

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
            mProteinCollectionNames = mImportHandler.LoadProteinCollectionNames();
            mFileErrorList?.Clear();
            mFileWarningList?.Clear();
            mValidUploadsList?.Clear();
            mSummarizedFileErrorList?.Clear();
            mSummarizedFileWarningList?.Clear();

            var frmBatchUpload = new frmBatchAddNewCollection(
                mOrganisms,
                mAnnotationTypes,
                mProteinCollectionNames,
                mPsConnectionString,
                mLastBatchUploadDirectoryPath,
                mCachedFileDescriptions);

            lblBatchProgress.Text = "";

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

            var tmpSelectedFileList = frmBatchUpload.FileList;

            mBatchLoadTotalCount = tmpSelectedFileList.Count;

            if (mUploadHandler != null)
            {
                mUploadHandler.BatchProgress -= BatchImportProgressHandler;
                mUploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                mUploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                mUploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                mUploadHandler.ValidationProgress -= ValidationProgressHandler;
                mUploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
            }

            mUploadHandler = new PSUploadHandler(mPsConnectionString);

            mUploadHandler.BatchProgress += BatchImportProgressHandler;
            mUploadHandler.ValidFASTAFileLoaded += ValidFASTAUploadHandler;
            mUploadHandler.InvalidFASTAFile += InvalidFASTAFileHandler;
            mUploadHandler.FASTAFileWarnings += FASTAFileWarningsHandler;
            mUploadHandler.ValidationProgress += ValidationProgressHandler;
            mUploadHandler.WroteLineEndNormalizedFASTA += NormalizedFASTAFileGenerationHandler;

            pgbMain.Value = 0;
            lblCurrentTask.Text = "";
            pnlProgBar.Visible = true;

            try
            {
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAllSymbolsInProteinNames, frmBatchUpload.ValidationAllowAllSymbolsInProteinNames);
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAsterisksInResidues, frmBatchUpload.ValidationAllowAsterisks);
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowDashInResidues, frmBatchUpload.ValidationAllowDash);

                mUploadHandler.MaximumProteinNameLength = frmBatchUpload.ValidationMaxProteinNameLength;

                mUploadHandler.BatchUpload(tmpSelectedFileList);
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

            lblBatchProgress.Text = "";
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
                    lblTargetServer.Text = "ERROR determining target server: mPSConnectionString is empty";
                    return;
                }

                var reExtractServerName = new Regex(@"Data Source\s*=\s*([^\s;]+)", RegexOptions.IgnoreCase);
                var reMatch = reExtractServerName.Match(mPsConnectionString);

                if (reMatch.Success)
                {
                    lblTargetServer.Text = "Target server: " + reMatch.Groups[1].Value;
                }
                else
                {
                    lblTargetServer.Text = "Target server: UNKNOWN -- name not found in " + mPsConnectionString;
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
            if (Convert.ToInt32(cboOrganismFilter.SelectedValue) != 0)
            {
                mProteinCollections.DefaultView.RowFilter = "[Organism_ID] = " + cboOrganismFilter.SelectedValue;
            }
            else
            {
                mProteinCollections.DefaultView.RowFilter = "";
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
            else
            {
                //mSelectedAuthorityID = 0;
            }

            if (mSelectedCollectionId > 0)
            {
                var foundRows = mProteinCollections.Select("[Protein_Collection_ID] = " + mSelectedCollectionId);
                mSelectedAnnotationTypeId = Convert.ToInt32(foundRows[0]["Authority_ID"]);
            }
            //else if (mSelectedAuthorityID == -2)
            //{
            //    //Bring up addition dialog
            //    var AuthAdd = new AddNamingAuthority(mPSConnectionString);
            //    tmpAuthID = AuthAdd.AddNamingAuthority;
            //    mAuthorities = mImportHandler.LoadAuthorities();
            //    BindAuthorityListToControl(mAuthorities);
            //    mSelectedAuthorityID = tmpAuthID;
            //    cboAuthorityPicker.SelectedValue = tmpAuthID;
            //}
        }

        #endregion

        #region "Action Button Handlers"

        private void cmdLoadProteins_Click(object sender, EventArgs e)
        {
            ImportStartHandler("Retrieving Protein Entries..");
            var foundRows =
                mProteinCollections.Select("Protein_Collection_ID = " + cboCollectionPicker.SelectedValue);
            ImportProgressHandler(0.5d);
            mSelectedFilePath = foundRows[0]["FileName"].ToString();
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
                if (mUploadHandler != null)
                {
                    mUploadHandler.BatchProgress -= BatchImportProgressHandler;
                    mUploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                    mUploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                    mUploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                    mUploadHandler.ValidationProgress -= ValidationProgressHandler;
                    mUploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
                }

                mUploadHandler = null;
                return;
            }

            var frmAddCollection = new frmAddNewCollection()
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

                var tmpOrganismId = frmAddCollection.OrganismId;
                var tmpAnnotationTypeId = frmAddCollection.AnnotationTypeId;

                var tmpSelectedProteinList = ScanDestinationCollectionWindow(lvwDestination);

                if (mUploadHandler == null)
                {
                    mUploadHandler = new PSUploadHandler(mPsConnectionString);
                    mUploadHandler.BatchProgress += BatchImportProgressHandler;
                    mUploadHandler.ValidFASTAFileLoaded += ValidFASTAUploadHandler;
                    mUploadHandler.InvalidFASTAFile += InvalidFASTAFileHandler;
                    mUploadHandler.FASTAFileWarnings += FASTAFileWarningsHandler;
                    mUploadHandler.ValidationProgress += ValidationProgressHandler;
                    mUploadHandler.WroteLineEndNormalizedFASTA += NormalizedFASTAFileGenerationHandler;
                }

                mUploadHandler.UploadCollection(mImportHandler.CollectionMembers, tmpSelectedProteinList,
                                                 frmAddCollection.CollectionName, frmAddCollection.CollectionDescription,
                                                 frmAddCollection.CollectionSource,
                                                 AddUpdateEntries.CollectionTypes.ProtOriginalSource, tmpOrganismId,
                                                 tmpAnnotationTypeId);

                RefreshCollectionList();

                ClearFromDestinationCollectionWindow(lvwDestination, true);

                cboOrganismFilter.Enabled = true;
                cboCollectionPicker.Enabled = true;
                cboOrganismFilter.SelectedValue = tmpOrganismId;
            }

            if (mUploadHandler != null)
            {
                mUploadHandler.BatchProgress -= BatchImportProgressHandler;
                mUploadHandler.ValidFASTAFileLoaded -= ValidFASTAUploadHandler;
                mUploadHandler.InvalidFASTAFile -= InvalidFASTAFileHandler;
                mUploadHandler.FASTAFileWarnings -= FASTAFileWarningsHandler;
                mUploadHandler.ValidationProgress -= ValidationProgressHandler;
                mUploadHandler.WroteLineEndNormalizedFASTA -= NormalizedFASTAFileGenerationHandler;
            }

            mUploadHandler = null;
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
        //    //SaveDialog.FileName = mImportHandler.CollectionMembers.FileName;

        //    if (SaveDialog.ShowDialog == DialogResult.OK)
        //        SelectedSavePath = SaveDialog.FileName;
        //    else
        //        return;

        //    if (System.IO.Path.GetExtension(mSelectedFilePath) == ".fasta")
        //        fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA;
        //    else if (System.IO.Path.GetExtension(mSelectedFilePath) == ".mdb")
        //        fileType = Protein_Importer.IImportProteins.ProteinImportFileTypes.Access;

        //    if (fileType == Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA)
        //        mProteinExporter = new Protein_Exporter.ExportProteinsFASTA();
        //    else
        //        return;

        //    tmpProteinCollection = new Protein_Storage.ProteinStorage(SelectedSavePath);

        //    tmpSelectedProteinList = ScanDestinationCollectionWindow(lvwDestination);

        //    foreach (var tmpProteinReference in tmpSelectedProteinList)
        //        tmpProteinCollection.AddProtein(mImportHandler.CollectionMembers.GetProtein(tmpProteinReference));

        //    mProteinExporter.Export(mImportHandler.CollectionMembers, SelectedSavePath);
        //}

        #endregion

        #region "LiveSearch Handlers"

        private void txtLiveSearch_TextChanged(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length > 0 && txtLiveSearch.ForeColor.ToString() != "Color [InactiveCaption]")
            {
                searchTimer.Start();
            }
            else if (string.IsNullOrEmpty(txtLiveSearch.Text) && !mSearchActive)
            {
                pbxLiveSearchCancel_Click(this, null);
            }
            else
            {
                mSearchActive = false;
                searchTimer.Stop();
                //txtLiveSearch.Text = "Search";
                //txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption;
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
                searchTimer.Stop();
                mSourceListViewHandler.Load(mCollectionMembers);
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
            if (mSearchActive)
            {
                //Debug.WriteLine("SearchTimer.active.kick");

                mSourceListViewHandler.Load(mCollectionMembers, txtLiveSearch.Text);
                mSearchActive = false;
                searchTimer.Stop();
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
            mSelectedCollectionId = Convert.ToInt32(cboCollectionPicker.SelectedValue);
            mSelectedAnnotationTypeId = Convert.ToInt32(cboAnnotationTypePicker.SelectedValue);

            mCollectionMembers = mImportHandler.LoadCollectionMembersById(mSelectedCollectionId, mSelectedAnnotationTypeId);
            mLocalFileLoaded = false;

            //mSelectedAuthorityID = mImportHandler.

            mSourceListViewHandler.Load(mCollectionMembers);
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
                selectedList.Add(li.Text);

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
        //    mFileBatcher = new BatchUploadFromFileList(mPSConnectionString);
        //    mFileBatcher.UploadBatch();
        //}

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
        //    string outputPath = "";

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
            if (mOrganisms == null)
            {
                mOrganisms = mImportHandler.LoadOrganisms();
            }

            if (mAnnotationTypes == null)
            {
                mAnnotationTypes = mImportHandler.LoadAnnotationTypes();
            }

            BindOrganismListToControl(mOrganisms);
            BindAnnotationTypeListToControl(mAnnotationTypes);
            mProteinCollections.DefaultView.RowFilter = "";
            BindCollectionListToControl(mProteinCollections.DefaultView);
            cboCollectionPicker.Enabled = true;
            cboOrganismFilter.Enabled = true;
            lblBatchProgress.Text = "";
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

            pgbMain.Value = (int)Math.Round(fractionDone * 100d);
            Application.DoEvents();
        }

        private void NormalizedFASTAFileGenerationHandler(string newFilePath)
        {
        }

        #endregion

        [Obsolete("Unused")]
        private void mnuAdminUpdateZeroedMasses_Click(object sender, EventArgs e)
        {
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(mPsConnectionString);
                mSyncer.SyncProgress += SyncProgressHandler;
            }

            mSyncer.CorrectMasses();
        }

        /// <summary>
        /// Valid, but could take a very long time
        /// Thus, the menu item is disabled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuAdminNameHashRefresh_Click(object sender, EventArgs e)
        {
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(mPsConnectionString);
                mSyncer.SyncProgress += SyncProgressHandler;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            mSyncer.RefreshNameHashes();
#pragma warning restore CS0618 // Type or member is obsolete
        }

        private void MenuItem5_Click(object sender, EventArgs e)
        {
            var frmTesting = new frmTestingInterface();
            frmTesting.Show();
        }

        [Obsolete("Uses old table")]
        private void MenuItem6_Click(object sender, EventArgs e)
        {
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(mPsConnectionString);
                mSyncer.SyncProgress += SyncProgressHandler;
            }

            mSyncer.FixArchivedFilePaths();
        }

        [Obsolete("Unused: AddSortingIndices uses an old view")]
        private void MenuItem8_Click(object sender, EventArgs e)
        {
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(mPsConnectionString);
                mSyncer.SyncProgress += SyncProgressHandler;
            }

            mSyncer.AddSortingIndices();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }
    }
}