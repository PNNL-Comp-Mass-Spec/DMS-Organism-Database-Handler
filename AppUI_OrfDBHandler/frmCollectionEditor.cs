using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using AppUI_OrfDBHandler.ExtractAdditionalAnnotations;
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
            SearchTimer = new System.Timers.Timer(2000d);
            SearchTimer.Elapsed += SearchTimerHandler;
            MemberLoadTimer = new System.Timers.Timer(2000d);
            MemberLoadTimer.Elapsed += MemberLoadTimerHandler;
            base.Load += frmCollectionEditor_Load;

            InitializeComponent();

            CheckTransferButtonsEnabledStatus();

            m_CachedFileDescriptions = new Dictionary<string, KeyValuePair<string, string>>();

            ReadSettings();
        }

        [STAThread()]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.DoEvents();
            Application.Run(new frmCollectionEditor());
        }

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
        private int m_LastValueForMaxProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
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
        private Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> m_FileErrorList;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of error messages, tracking the count of each error
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> m_SummarizedFileErrorList;

        /// <summary>
        /// Keys are fasta file names, values are lists of warnings
        /// </summary>
        private Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> m_FileWarningList;

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
        private readonly System.Timers.Timer SearchTimer;
        private readonly System.Timers.Timer MemberLoadTimer;

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
            var resultReturn = frmBatchUpload.ShowDialog();

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
                    //Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", m_LastSelectedOrganism);
                    Settings.Default.LastSelectedOrganism = m_LastSelectedOrganism;
                }

                if (!string.IsNullOrEmpty(m_LastSelectedAnnotationType))
                {
                    //Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", m_LastSelectedAnnotationType);
                    Settings.Default.LastSelectedAnnotationType = m_LastSelectedAnnotationType;
                }

                if (!string.IsNullOrEmpty(m_LastBatchULDirectoryPath))
                {
                    //Interaction.SaveSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", m_LastBatchULDirectoryPath);
                    Settings.Default.LastBatchULDirectoryPath = m_LastBatchULDirectoryPath;
                }

                Settings.Default.Save();
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

            var tmpSelectedFileList = frmBatchUpload.FileList;

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
                m_UploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAllSymbolsInProteinNames, frmBatchUpload.ValidationAllowAllSymbolsInProteinNames);
                m_UploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAsterisksInResidues, frmBatchUpload.ValidationAllowAsterisks);
                m_UploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowDashInResidues, frmBatchUpload.ValidationAllowDash);

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
                //m_LastSelectedOrganism = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedOrganism", "");
                //m_LastSelectedAnnotationType = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastSelectedAnnotationType", "");
                //m_LastBatchULDirectoryPath = Interaction.GetSetting("ProteinCollectionEditor", "UserOptions", "LastBatchULDirectoryPath", "");
                m_LastSelectedOrganism = Settings.Default.LastSelectedOrganism ?? "";
                m_LastSelectedAnnotationType = Settings.Default.LastSelectedAnnotationType ?? "";
                m_LastBatchULDirectoryPath = Settings.Default.LastBatchULDirectoryPath ?? "";
            }
            catch (Exception ex)
            {
                // Ignore errors here
            }
        }

        private void ShowAboutBox()
        {
            //var AboutBox = new frmAboutBox;

            //AboutBox.Location = m_MainProcess.myAppSettings.AboutBoxLocation;
            //AboutBox.ShowDialog();

            var strMessage = "This is version " + Application.ProductVersion + ", " + PROGRAM_DATE;

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
            if (Convert.ToInt32(cboOrganismFilter.SelectedValue) != 0)
            {
                m_ProteinCollections.DefaultView.RowFilter = "[Organism_ID] = " + cboOrganismFilter.SelectedValue.ToString();
            }
            else
            {
                m_ProteinCollections.DefaultView.RowFilter = "";
            }

            m_SelectedOrganismID = Convert.ToInt32(cboOrganismFilter.SelectedValue);

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
            m_SelectedCollectionID = Convert.ToInt32(cboCollectionPicker.SelectedValue);

            if (m_SelectedCollectionID > 0)
            {
                var foundRows = m_ProteinCollections.Select("[Protein_Collection_ID] = " + m_SelectedCollectionID.ToString());
                m_SelectedAnnotationTypeID = Convert.ToInt32(foundRows[0]["Authority_ID"]);
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
                m_SelectedAnnotationTypeID = Convert.ToInt32(cboAnnotationTypePicker.SelectedValue);
            }
            else
            {
                //m_SelectedAuthorityID = 0;
            }

            if (m_SelectedCollectionID > 0)
            {
                var foundRows = m_ProteinCollections.Select("[Protein_Collection_ID] = " + m_SelectedCollectionID.ToString());
                m_SelectedAnnotationTypeID = Convert.ToInt32(foundRows[0]["Authority_ID"]);
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
            m_SelectedCollectionID = Convert.ToInt32(cboCollectionPicker.SelectedValue);
            m_SelectedAnnotationTypeID = Convert.ToInt32(cboAnnotationTypePicker.SelectedValue);

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

        private void InvalidFASTAFileHandler(string FASTAFilePath, List<CustomFastaValidator.ErrorInfoExtended> errorCollection)
        {
            if (m_FileErrorList == null)
            {
                m_FileErrorList = new Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>>();
            }

            m_FileErrorList.Add(Path.GetFileName(FASTAFilePath), errorCollection);

            if (m_SummarizedFileErrorList == null)
            {
                m_SummarizedFileErrorList = new Dictionary<string, Dictionary<string, int>>();
            }

            m_SummarizedFileErrorList.Add(Path.GetFileName(FASTAFilePath), SummarizeErrors(errorCollection));
        }

        private void FASTAFileWarningsHandler(string FASTAFilePath, List<CustomFastaValidator.ErrorInfoExtended> warningCollection)
        {
            if (m_FileWarningList == null)
            {
                m_FileWarningList = new Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>>();
            }

            m_FileWarningList.Add(Path.GetFileName(FASTAFilePath), warningCollection);

            if (m_SummarizedFileWarningList == null)
            {
                m_SummarizedFileWarningList = new Dictionary<string, Dictionary<string, int>>();
            }

            m_SummarizedFileWarningList.Add(Path.GetFileName(FASTAFilePath), SummarizeErrors(warningCollection));
        }

        private Dictionary<string, int> SummarizeErrors(IReadOnlyCollection<CustomFastaValidator.ErrorInfoExtended> errorCollection)
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