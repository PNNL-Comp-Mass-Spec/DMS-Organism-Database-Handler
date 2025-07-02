using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.ProteinUpload;
using PRISMDatabaseUtils;
using PRISMSeq_Uploader.Properties;
using PRISM.Logging;
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

            Load += frmCollectionEditor_Load;

            InitializeComponent();

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
        private int mSelectedCollectionId;
        private string mLastBatchUploadDirectoryPath;

        /// <summary>
        /// Protein sequences database connection string
        /// </summary>
        private string mDbConnectionString = "Host=prismdb2.emsl.pnl.gov;Port=5432;Database=dms;UserId=pceditor";

        private string mLastSelectedOrganism = string.Empty;
        private string mLastSelectedAnnotationType = string.Empty;
        private bool mLastValueForAllowAsterisks;
        private bool mLastValueForAllowDash;
        private int mLastValueForMaxProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
        private ImportHandler mImportHandler;
        private PSUploadHandler mUploadHandler;
        private DataListViewHandler mSourceListViewHandler;

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
                mDbConnectionString = connectionString;
            }

            UpdateServerNameLabel();

            // Initialize mImportHandler if null
            mImportHandler ??= new ImportHandler(mDbConnectionString);

            mImportHandler.LoadStart += ImportStartHandler;
            mImportHandler.LoadProgress += ImportProgressHandler;
            mImportHandler.LoadEnd += ImportEndHandler;
            mImportHandler.CollectionLoadComplete += CollectionLoadHandler;

            lblBatchProgress.Text = "Fetching Organism and Collection Lists...";

            cboCollectionPicker.Enabled = false;
            cboOrganismFilter.Enabled = false;

            TriggerCollectionTableUpdate(false);

            mSourceListViewHandler = new DataListViewHandler(lvwSource);

            cmdLoadProteins.Enabled = false;

            cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
            lblBatchProgress.Text = string.Empty;

            // Setup collections for selected organism

            // Use 2-3 second delay after collection change before refreshing member list
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
                mDbConnectionString,
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

            mUploadHandler = new PSUploadHandler(mDbConnectionString);
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

        private void UpdateCachedInfoAfterLoadingProteins()
        {
            mSelectedCollectionId = Convert.ToInt32(cboCollectionPicker.SelectedValue);
            mSelectedAnnotationTypeId = Convert.ToInt32(cboAnnotationTypePicker.SelectedValue);

            mCollectionMembers = mImportHandler.LoadCollectionMembersById(mSelectedCollectionId, mSelectedAnnotationTypeId);

            mSourceListViewHandler.Load(mCollectionMembers);
            lvwSource.Focus();
            lvwSource.Enabled = true;
        }

        private void UpdateCachedProteinCollectionMembersAndProteinHeadersInDB()
        {
            try
            {
                var success1 = UpdateCachedProteinCollectionMembersInDB(out var collectionMembersStatus);
                var success2 = UpdateCachedProteinHeadersInDB(out var proteinHeadersStatus);

                var msg = string.Format("{0}; {1}", collectionMembersStatus, proteinHeadersStatus);

                if (success1 && success2)
                {
                    MessageBox.Show(msg, "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    OnWarningEvent(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = "ERROR calling methods UpdateCachedProteinCollectionMembersInDB and UpdateCachedProteinHeadersInDB: " + ex.Message;
                OnErrorEvent(msg, ex);

                lblTargetDatabase.Text = msg;
            }
        }

        private bool UpdateCachedProteinCollectionMembersInDB(out string statusMessage)
        {
            try
            {
                var proteinseqsConnectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mDbConnectionString, "PRISMSeq_Uploader");

                var dbTools = DbToolsFactory.GetDBTools(proteinseqsConnectionStringToUse);
                RegisterEvents(dbTools);

                var cmdSave = dbTools.CreateCommand("update_cached_protein_collection_members", CommandType.StoredProcedure);

                // Use a 5-minute timeout
                cmdSave.CommandTimeout = 300;

                // Define parameter for procedure's return value
                // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
                var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

                // Define parameters for the procedure's arguments
                dbTools.AddParameter(cmdSave, "@maxCollectionsToUpdate", SqlType.Int).Value = 0;
                var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

                // Call the procedure
                dbTools.ExecuteSP(cmdSave);

                // The return code is an integer on SQL Server, but is text on Postgres
                // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
                var returnCode = DBToolsBase.GetReturnCode(returnParam);

                string msg;

                if (returnCode == 0)
                {
                    msg = "Updated cached protein collection members";
                }
                else
                {
                    msg = string.Format("Procedure update_cached_protein_collection_members returned a non-zero return code: {0}", returnCode);
                }

                var procedureMessage = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(procedureMessage))
                {
                    msg += "; " + procedureMessage;
                }

                statusMessage = msg;
                return returnCode == 0;
            }
            catch (Exception ex)
            {
                statusMessage = "ERROR calling procedure update_cached_protein_collection_members: " + ex.Message;
                OnErrorEvent(statusMessage, ex);

                lblTargetDatabase.Text = statusMessage;
                return false;
            }
        }

        private bool UpdateCachedProteinHeadersInDB(out string statusMessage)
        {
            try
            {
                var proteinseqsConnectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mDbConnectionString, "PRISMSeq_Uploader");

                var dbTools = DbToolsFactory.GetDBTools(proteinseqsConnectionStringToUse);
                RegisterEvents(dbTools);

                var cmdSave = dbTools.CreateCommand("add_new_protein_headers", CommandType.StoredProcedure);

                // Use a 5-minute timeout
                cmdSave.CommandTimeout = 300;

                // Define parameter for procedure's return value
                // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
                var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

                // Define parameters for the procedure's arguments
                dbTools.AddParameter(cmdSave, "@maxProteinsToProcess", SqlType.Int).Value = 0;
                var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

                // Call the procedure
                dbTools.ExecuteSP(cmdSave);

                // The return code is an integer on SQL Server, but is text on Postgres
                // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
                var returnCode = DBToolsBase.GetReturnCode(returnParam);

                string msg;

                if (returnCode == 0)
                {
                    msg = "Updated cached protein sequence headers";
                }
                else
                {
                    msg = string.Format("Procedure add_new_protein_headers returned a non-zero return code: {0}", returnCode);
                }

                var procedureMessage = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(procedureMessage))
                {
                    msg += "; " + procedureMessage;
                }

                statusMessage = msg;
                return returnCode == 0;
            }
            catch (Exception ex)
            {
                statusMessage = "ERROR calling procedure update_cached_protein_collection_members: " + ex.Message;
                OnErrorEvent(statusMessage, ex);

                lblTargetDatabase.Text = statusMessage;
                return false;
            }
        }

        private void UpdateServerNameLabel()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mDbConnectionString))
                {
                    lblTargetDatabase.Text = "ERROR determining target database: mDbConnectionString is empty";
                    return;
                }

                var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mDbConnectionString, "PRISMSeq_Uploader");

                var dbTools = DbToolsFactory.GetDBTools(connectionStringToUse);
                RegisterEvents(dbTools);

                lblTargetDatabase.Text = string.Format("Target database: {0} (on {1})", dbTools.DatabaseName, dbTools.ServerName);
            }
            catch (Exception ex)
            {
                lblTargetDatabase.Text = "ERROR determining target database: " + ex.Message;
            }
        }

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

        private void cmdLoadProteins_Click(object sender, EventArgs e)
        {
            ImportStartHandler("Retrieving Protein Entries..");
            // var foundRows = mProteinCollections.Select("Protein_Collection_ID = " + cboCollectionPicker.SelectedValue);

            ImportProgressHandler(0.5d);
            UpdateCachedInfoAfterLoadingProteins();

            ImportProgressHandler(1.0d);
            ImportEndHandler();
        }

        private void cmdLoadFile_Click(object sender, EventArgs e)
        {
            BatchLoadController();
        }

        private void cmdUpdateCachedCollectionMembers_Click(object sender, EventArgs e)
        {
            UpdateCachedProteinCollectionMembersAndProteinHeadersInDB();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

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

        private void ImportEndHandler()
        {
            //mFileBatcher.LoadEnd()

            lblCurrentTask.Text = "Complete: " + lblCurrentTask.Text;
            Invalidate();
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
        protected void RegisterEvents(IEventNotifier sourceClass)
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

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            ShowAboutBox();
        }
    }
}