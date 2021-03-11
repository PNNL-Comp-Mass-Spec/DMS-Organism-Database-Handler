using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinUpload;
using PRISMDatabaseUtils;
using Raccoom.Windows.Forms;
using ValidateFastaFile;

namespace AppUI_OrfDBHandler
{
    public partial class frmBatchAddNewCollection : Form
    {
        public frmBatchAddNewCollection(
            DataTable organismList,
            DataTable annotationTypeList,
            DataTable existingCollectionsList,
            string psConnectionString,
            string selectedFolderPath,
            Dictionary<string, KeyValuePair<string, string>> cachedFileDescriptions)
        {
            base.Load += frmBatchAddNewCollection_Load;
            base.Closing += frmBatchAddNewCollection_Closing;

            InitializeComponent();

            m_StatusResetTimer = new Timer() { Interval = 1000 };
            m_StatusResetTimer.Tick += m_StatusResetTimer_Tick;

            m_StatusResetTimer.Start();

            ClearStatus();

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

        public List<PSUploadHandler.UploadInfo> FileList => m_CheckedFileList;

        public int SelectedOrganismID => m_SelectedOrganismID;

        public int SelectedAnnotationTypeID => m_SelectedAnnotationTypeID;

        public string CurrentDirectory
        {
            get => m_LastUsedDirectory;
            set => m_LastUsedDirectory = value;
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
            set => m_LastSelectedOrganism = value;
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
            set => m_LastSelectedAnnotationType = value;
        }

        public bool ValidationAllowAsterisks
        {
            get => chkValidationAllowAsterisks.Checked;
            set => chkValidationAllowAsterisks.Checked = value;
        }

        public bool ValidationAllowDash
        {
            get => chkValidationAllowDash.Checked;
            set => chkValidationAllowDash.Checked = value;
        }

        public bool ValidationAllowAllSymbolsInProteinNames
        {
            get => chkValidationAllowAllSymbolsInProteinNames.Checked;
            set => chkValidationAllowAllSymbolsInProteinNames.Checked = value;
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
                    return FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
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

            foreach (DataRow dr in foundRows)
            {
                var tmpID = DatabaseUtilsExtensions.GetInteger(null, dr["Protein_Collection_ID"]);
                var tmpName = dr["FileName"].ToString();
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

            bSize[0] = "Bytes";
            bSize[1] = "KB"; // Kilobytes
            bSize[2] = "MB"; // Megabytes
            bSize[3] = "GB"; // Gigabytes
            bSize[4] = "TB"; // Terabytes
            bSize[5] = "PB"; // Petabytes
            bSize[6] = "EB"; // Exabytes
            bSize[7] = "ZB"; // Zettabytes
            bSize[8] = "YB"; // Yottabytes

            for (var i = bSize.Length; i >= 0; i -= 1)
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

            try
            {
                if (strValue != null && strValue.Length > 0)
                {
                    for (var intIndex = 0; intIndex < objComboBox.Items.Count; intIndex++)
                    {
                        var objRow = (DataRowView)objComboBox.Items[intIndex];

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

            if (ReferenceEquals(cboAnnotationTypePicker.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                m_SelectedAnnotationTypeID = (int)Math.Round(Convert.ToDouble(cboAnnotationTypePicker.SelectedValue));
            }
            else
            {
                // m_SelectedAuthorityID = 0
            }

            CheckTransferEnable();

            if (m_SelectedAnnotationTypeID == -2)
            {
                // Bring up an additional dialog
                var AnnTypeAdd = new AddAnnotationTypeType(m_PSConnectionString);
                AnnTypeAdd.FormLocation = new Point(Left + Width + 10, Top);
                var tmpAnnTypeID = AnnTypeAdd.AddAnnotationType();
                // Dim AuthAdd As New AddNamingAuthority(m_PSConnectionString)
                // tmpAuthID = AuthAdd.AddNamingAuthority

                if (!AnnTypeAdd.EntryExists & tmpAnnTypeID > 0)
                {
                    var dr = m_AnnotationTypeList.NewRow();

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
                    var tmpUpInfo = m_SelectedFileList[fastaFilePath];

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
                var r = MessageBox.Show("You have files selected for upload. Really close the form?",
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
                txtMaximumProteinNameLength.Text = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString();
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
                    txtMaximumProteinNameLength.Text = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString();
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
