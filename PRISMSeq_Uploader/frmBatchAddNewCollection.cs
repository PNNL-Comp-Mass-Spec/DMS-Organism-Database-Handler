using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.ProteinUpload;
using PRISM;
using PRISMDatabaseUtils;
using Raccoom.Windows.Forms;
using ValidateFastaFile;

namespace PRISMSeq_Uploader
{
    public partial class frmBatchAddNewCollection : Form
    {
        // Ignore spelling: frm, lvw, uploader, Passphrase, terabytes, petabytes, exabytes, zettabytes, yottabytes

        public frmBatchAddNewCollection(
            ImportHandler importHandler,
            DataTable organismList,
            DataTable annotationTypeList,
            DataTable existingCollectionsList,
            string dbConnectionString,
            string selectedFolderPath,
            Dictionary<string, KeyValuePair<string, string>> cachedFileDescriptions)
        {
            Load += frmBatchAddNewCollection_Load;
            Closing += frmBatchAddNewCollection_Closing;

            InitializeComponent();

            mStatusResetTimer = new Timer { Interval = 1000 };
            mStatusResetTimer.Tick += StatusResetTimer_Tick;

            mStatusResetTimer.Start();

            ClearStatus();

            mImportHandler = importHandler;

            mOrganismListSorted = new DataView(organismList) { Sort = "display_name" };

            mAnnotationTypeList = annotationTypeList;
            mCollectionsTable = existingCollectionsList;
            mDbConnectionString = dbConnectionString;

            mCachedFileDescriptions = cachedFileDescriptions;

            ctlTreeViewFolderBrowser.DataSource = new TreeStrategyFolderBrowserProvider();
            ctlTreeViewFolderBrowser.CheckBoxBehaviorMode = CheckBoxBehaviorMode.None;

            InitializeTreeView(selectedFolderPath);
        }

        private enum FolderContentsColumn
        {
            FileName = 0,
            LastModified = 1,
            FileSize = 2,
            ExistingCollection = 3,
            FilePath = 4,
        }

        private enum SelectedFileColumn
        {
            ProteinCollectionName = 0,
            Organism = 1,
            Description = 2,
            Source = 3,
            AnnotationType = 4,
            FilePath = 5,
        }

        private struct ProteinCollectionMetadata
        {
            public string Description;
            public string Source;
        }

        private string mLastSelectedOrganism = string.Empty;
        private string mLastSelectedAnnotationType = string.Empty;

        /// <summary>
        /// Keys are full paths to the FASTA file
        /// Values are FileInfo instances
        /// </summary>
        private Dictionary<string, FileInfo> mFileList;

        // Keys are file paths, values are UploadInfo objects
        private Dictionary<string, PSUploadHandler.UploadInfo> mSelectedFileList;

        private readonly ImportHandler mImportHandler;

        private DataView mOrganismListSorted;

        private readonly DataTable mAnnotationTypeList;
        private readonly DataTable mCollectionsTable;

        /// <summary>
        /// Keys are protein collection ID
        /// Values are Protein Collection name
        /// </summary>
        private Dictionary<int, string> mCollectionsList;

        private readonly string mDbConnectionString;
        private bool mReallyClose;
        private FilePreviewHandler mFilePreviewer;

        private bool mPreviewFormStatus;

        // private HashTable mPassphraseList;
        // private string mCachedPassphrase;

        private const string AddFilesMessage = "You must first select an organism and annotation type, and then select one or more protein collections.";
        private bool mAllowAddFiles;
        private string mAllowAddFilesMessage = AddFilesMessage;

        // Tracks the index of the last column clicked in lvwSelectedFiles
        private int mSortColumnSelectedItems = -1;

        // Tracks the index of the last column clicked in lvwFolderContents
        private int mSortColumnFolderContents = -1;

        /// <summary>
        /// Tracks Description and Source that the uploader has defined for each file (not persisted when the application closes)
        /// </summary>
        /// <remarks>Useful in case validation fails and the uploader needs to try again to upload a FASTA file</remarks>
        private readonly Dictionary<string, KeyValuePair<string, string>> mCachedFileDescriptions;

        private bool mStatusResetRequired;
        private DateTime mStatusClearTime;
        private readonly Timer mStatusResetTimer;

        public List<PSUploadHandler.UploadInfo> FileList { get; private set; }

        public int SelectedOrganismID { get; private set; }

        public int SelectedAnnotationTypeID { get; private set; }

        public string CurrentDirectory { get; set; }

        private string SelectedNodeFolderPath
        {
            get
            {
                try
                {
                    if (ctlTreeViewFolderBrowser.SelectedNode is TreeNodePath currentNode && !string.IsNullOrWhiteSpace(currentNode.Path))
                    {
                        return currentNode.Path;
                    }
                }
                catch (Exception)
                {
                    // Ignore errors
                }

                return string.Empty;
            }
        }

        public string SelectedOrganismName
        {
            get => cboOrganismSelect.Items.Count > 0 ? cboOrganismSelect.Text : string.Empty;
            set => mLastSelectedOrganism = value;
        }

        public string SelectedAnnotationType
        {
            get
            {
                if (cboAnnotationTypePicker.Items.Count > 0)
                {
                    return cboAnnotationTypePicker.Text;
                }

                return string.Empty;
            }
            set => mLastSelectedAnnotationType = value;
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
                if (int.TryParse(txtMaximumProteinNameLength.Text, out var intValue))
                {
                    return intValue;
                }

                return FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
            }
            set
            {
                if (value < 5)
                    value = 5;
                txtMaximumProteinNameLength.Text = value.ToString();
            }
        }

        private void frmBatchAddNewCollection_Load(object sender, EventArgs e)
        {
            mCollectionsList = CollectionsTableToList(mCollectionsTable);

            // Initialize mFileList if null
            mFileList ??= new Dictionary<string, FileInfo>();

            FileList = new List<PSUploadHandler.UploadInfo>();
            LoadOrganismPicker(cboOrganismSelect, mOrganismListSorted);
            LoadAnnotationTypePicker(cboAnnotationTypePicker, mAnnotationTypeList);
            cmdUploadChecked.Enabled = false;
            cmdAddFile.Enabled = true;
            cmdRemoveFile.Enabled = false;

            SelectComboBoxItemByName(cboOrganismSelect, mLastSelectedOrganism, 2);
            SelectComboBoxItemByName(cboAnnotationTypePicker, mLastSelectedAnnotationType, 1);
        }

        private Dictionary<int, string> CollectionsTableToList(DataTable dataTable)
        {
            var collectionInfo = new Dictionary<int, string>(dataTable.Rows.Count);

            foreach (var dataRow in dataTable.Select("", "protein_collection_id"))
            {
                var proteinCollectionID = DatabaseUtilsExtensions.GetInteger(null, dataRow["protein_collection_id"]);
                var collectionName = dataRow["collection_name"].ToString();

                if (!collectionInfo.ContainsKey(proteinCollectionID))
                {
                    collectionInfo.Add(proteinCollectionID, collectionName);
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

            CurrentDirectory = directoryPath;

            var foundFastaFiles = di.GetFiles();

            if (mFileList != null)
            {
                mFileList.Clear();
            }
            else
            {
                mFileList = new Dictionary<string, FileInfo>();
            }

            foreach (var fi in foundFastaFiles)
            {
                var fileExtension = Path.GetExtension(fi.Name);

                switch (fileExtension.ToLower())
                {
                    case ".fasta":
                    case ".fst":
                    case ".fa":
                    case ".pep":
                    case ".faa":
                        mFileList.Add(fi.FullName, fi);
                        break;
                }
            }

            LoadListView();
        }

        /// <summary>
        /// Populate the top ListView with FASTA files in the selected folder
        /// </summary>
        private void LoadListView()
        {
            lvwFolderContents.BeginUpdate();

            lvwFolderContents.Items.Clear();

            // Initialize mCollectionsList if null
            mCollectionsList ??= new Dictionary<int, string>();

            foreach (var fi in mFileList.Values)
            {
                var proteinCollectionName = Path.GetFileNameWithoutExtension(fi.Name);

                var item = new ListViewItem
                {
                    // FASTA file name (with the extension)
                    Text = fi.Name
                };

                // Last Write Time
                item.SubItems.Add(fi.LastWriteTime.ToString("g"));

                // File Size
                item.SubItems.Add(Numeric2Bytes(fi.Length));

                // Check whether the FASTA file is already a protein collection
                if (mCollectionsList.ContainsValue(proteinCollectionName))
                {
                    item.SubItems.Add("Yes");
                }
                else
                {
                    item.SubItems.Add("No");
                }

                // Full file path
                item.SubItems.Add(fi.FullName);

                lvwFolderContents.Items.Add(item);
            }

            lvwFolderContents.EndUpdate();
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private void LoadOrganismPicker(ComboBox cbo, DataView orgList)
        {
            cboOrganismSelect.SelectedIndexChanged -= cboOrganismSelect_SelectedIndexChanged;
            cbo.DataSource = orgList;
            cbo.DisplayMember = "display_name";
            cbo.ValueMember = "id";

            cboOrganismSelect.SelectedIndexChanged += cboOrganismSelect_SelectedIndexChanged;
        }

        private void LoadAnnotationTypePicker(ComboBox cbo, DataTable authList)
        {
            cboAnnotationTypePicker.SelectedIndexChanged -= cboAnnotationTypePicker_SelectedIndexChanged;

            cbo.BeginUpdate();
            var dataRow = authList.NewRow();

            dataRow["id"] = -2;
            dataRow["display_name"] = "Add New Annotation Type...";
            dataRow["details"] = "Brings up a dialog box to allow adding a naming authority to the list";

            var pk1 = new DataColumn[1];
            pk1[0] = authList.Columns["id"];
            authList.PrimaryKey = pk1;

            if (authList.Rows.Contains(dataRow["id"]))
            {
                var rdr = authList.Rows.Find(dataRow["id"]);
                authList.Rows.Remove(rdr);
            }

            authList.Rows.Add(dataRow);
            authList.AcceptChanges();

            cbo.DataSource = authList;
            cbo.DisplayMember = "display_name";
            cbo.ValueMember = "id";
            cbo.EndUpdate();

            cboAnnotationTypePicker.SelectedIndexChanged += cboAnnotationTypePicker_SelectedIndexChanged;
        }

        public string Numeric2Bytes(double b)
        {
            var bSize = new string[9];

            bSize[0] = "Bytes";
            bSize[1] = "KB"; // kilobytes
            bSize[2] = "MB"; // megabytes
            bSize[3] = "GB"; // gigabytes
            bSize[4] = "TB"; // terabytes
            bSize[5] = "PB"; // petabytes
            bSize[6] = "EB"; // exabytes
            bSize[7] = "ZB"; // zettabytes
            bSize[8] = "YB"; // yottabytes

            for (var i = bSize.Length; i >= 0; --i)
            {
                if (b >= Math.Pow(1024d, i))
                {
                    return FormatDecimal(b / Math.Pow(1024d, i)) + " " + bSize[i];
                }
            }

            return b + " Bytes";
        }

        /// <summary>
        /// Return the value formatted to include one or two digits after the decimal point
        /// </summary>
        /// <remarks>
        /// Examples:
        /// 1
        /// 123
        /// 12.3
        /// 2.4
        /// 1.2
        /// 0.12
        /// </remarks>
        /// <param name="value"></param>
        private string FormatDecimal(double value)
        {
            if (value >= 100d)
            {
                // No digits after the decimal
                return ((int)Math.Round(value)).ToString();
            }

            if (value >= 1d)
            {
                // One digit after the decimal
                return value.ToString("0.0");
            }

            // Two digits after the decimal
            return value.ToString("0.00");
        }

        private void AddFileToSelectedList()
        {
            try
            {
                // Initialize mSelectedFileList if null
                mSelectedFileList ??= new Dictionary<string, PSUploadHandler.UploadInfo>(StringComparer.CurrentCultureIgnoreCase);

                foreach (ListViewItem li in lvwFolderContents.SelectedItems)
                {
                    var fastaFilePath = GetFolderContentsColumn(li, FolderContentsColumn.FilePath);

                    if (cboOrganismSelect.SelectedValue == null)
                    {
                        if (string.IsNullOrWhiteSpace(cboOrganismSelect.Text))
                        {
                            MessageBox.Show("Please select an organism", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }

                        if (!SelectClosestMatchingOrganism(cboOrganismSelect))
                        {
                            MessageBox.Show("Invalid organism: " + cboOrganismSelect.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    if (cboOrganismSelect.SelectedValue == null || !int.TryParse(cboOrganismSelect.SelectedValue.ToString(), out var organismId))
                    {
                        MessageBox.Show("Please select a valid organism", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    var upInfo = new PSUploadHandler.UploadInfo
                    {
                        FileInformation = mFileList[fastaFilePath],
                        OrganismId = organismId,
                        AnnotationTypeId = (int)cboAnnotationTypePicker.SelectedValue,
                        Description = string.Empty,
                        Source = string.Empty
                    };

                    var proteinCollection = Path.GetFileNameWithoutExtension(upInfo.FileInformation.Name);

                    if (mSelectedFileList.ContainsKey(upInfo.FileInformation.FullName))
                    {
                        mSelectedFileList.Remove(upInfo.FileInformation.FullName);

                        foreach (ListViewItem si in lvwSelectedFiles.Items)
                        {
                            if ((si.Text ?? "") == proteinCollection)
                            {
                                lvwSelectedFiles.Items.Remove(si);
                            }
                        }
                    }

                    string fileDescription;
                    string fileSource;

                    if (mCachedFileDescriptions.TryGetValue(proteinCollection, out var kvDescriptionSource))
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
                    mSelectedFileList.Add(upInfo.FileInformation.FullName, upInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in AddFileToSelectedList: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void AddUpdateDictionaryItem(IDictionary<string, int> itemList, string newItem)
        {
            if (itemList.TryGetValue(newItem, out var itemCount))
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
            var folderPath = SelectedNodeFolderPath;

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

            mStatusClearTime = DateTime.UtcNow.AddSeconds(5d);
            mStatusResetRequired = true;
        }

        private string GetFolderContentsColumn(ListViewItem li, FolderContentsColumn eColumn)
        {
            var value = li.SubItems[(int)eColumn].Text;
            return value;
        }

        /// <summary>
        /// Get value of the specified column
        /// </summary>
        /// <param name="li"></param>
        /// <param name="eColumn"></param>
        private string GetSelectedFileColumn(ListViewItem li, SelectedFileColumn eColumn)
        {
            var value = li.SubItems[(int)eColumn].Text;
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
                    {
                        currentFolder = currentFolder.Parent;
                    }

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
        /// Populates mCheckedFileList
        /// </summary>
        /// <returns>True if success, false if no protein collections are selected or if one or more is missing a description and/or source</returns>
        private bool MakeCheckedFileList()
        {
            var nameList = new Dictionary<string, ProteinCollectionMetadata>();

            foreach (ListViewItem li in lvwSelectedFiles.Items)
            {
                var fastaFilePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);

                if (!nameList.ContainsKey(fastaFilePath))
                {
                    var udtMetadata = new ProteinCollectionMetadata
                    {
                        Description = GetSelectedFileColumn(li, SelectedFileColumn.Description),
                        Source = GetSelectedFileColumn(li, SelectedFileColumn.Source)
                    };
                    nameList.Add(fastaFilePath, udtMetadata);
                }
            }

            foreach (var item in mSelectedFileList)
            {
                var upInfo = item.Value;
                var fi = upInfo.FileInformation;

                if (nameList.TryGetValue(fi.FullName, out var udtMetadata))
                {
                    upInfo.Description = udtMetadata.Description;
                    upInfo.Source = udtMetadata.Source;

                    if (string.IsNullOrWhiteSpace(upInfo.Description) && string.IsNullOrWhiteSpace(upInfo.Source))
                    {
                        MessageBox.Show("Each new protein collection must have a description and/or source defined", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    FileList.Add(upInfo);
                }
            }

            if (FileList.Count == 0)
            {
                MessageBox.Show("No FASTA files are selected", "Nothing to Do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

                var filePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);

                if (mSelectedFileList.ContainsKey(filePath))
                {
                    mSelectedFileList.Remove(filePath);
                }
            }
        }

        private void SelectAllRows(ListView lv)
        {
            foreach (ListViewItem li in lv.Items)
            {
                li.Selected = true;
            }
        }

        private void SelectComboBoxItemByName(ComboBox objComboBox, string strValue, int intDataColumnIndexToCheck)
        {
            // Look for strValue in a ComboBox that has a data table attached via the .DataSource property
            // If the value is found, the given item in the ComboBox is selected

            try
            {
                if (!string.IsNullOrEmpty(strValue))
                {
                    for (var intIndex = 0; intIndex < objComboBox.Items.Count; intIndex++)
                    {
                        var objRow = (DataRowView)objComboBox.Items[intIndex];

                        if (!DBNull.Value.Equals(objRow[intDataColumnIndexToCheck]))
                        {
                            if ((objRow[intDataColumnIndexToCheck]?.ToString() ?? "") == strValue)
                            {
                                objComboBox.SelectedIndex = intIndex;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Ignore errors here
            }
        }

        private bool SelectClosestMatchingOrganism(ListControl cbo)
        {
            var textToFind = cbo.Text;

            if (string.IsNullOrWhiteSpace(textToFind))
                return false;

            foreach (DataRowView item in mOrganismListSorted)
            {
                if (item[2].ToString().StartsWith(textToFind))
                {
                    cbo.SelectedValue = item[0];
                    return true;
                }
            }

            return false;
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
        private string StripWhiteSpace(string value)
        {
            return value.Trim().Replace("\t", "; ").Replace(Environment.NewLine, "; ").Replace("\r", "; ").Replace("\n", "; ");
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
                    var descriptionCurrent = GetSelectedFileColumn(li, SelectedFileColumn.Description);
                    var sourceCurrent = GetSelectedFileColumn(li, SelectedFileColumn.Source);

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
                {
                    itemsToUpdate.Add(li);
                }
            }

            // Show a window with the most commonly used description and source

            var oMetadataWindow = new frmNewCollectionMetadataEditor
            {
                Description = MostCommonItem(descriptionList),
                Source = MostCommonItem(sourceList)
            };

            var eDialogResult = oMetadataWindow.ShowDialog();

            if (eDialogResult == DialogResult.OK)
            {
                var updatedDescription = StripWhiteSpace(oMetadataWindow.Description);
                var updatedSource = StripWhiteSpace(oMetadataWindow.Source);

                foreach (var li in itemsToUpdate)
                {
                    li.SubItems[(int)SelectedFileColumn.Description].Text = updatedDescription;
                    li.SubItems[(int)SelectedFileColumn.Source].Text = updatedSource;

                    var proteinCollection = li.SubItems[(int)SelectedFileColumn.ProteinCollectionName].Text;

                    // Add/update the dictionary item
                    mCachedFileDescriptions[proteinCollection] = new KeyValuePair<string, string>(updatedDescription, updatedSource);
                }
            }
        }

        private void cboOrganismSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = (ComboBox)sender;

            if (cbo.SelectedValue == null || !int.TryParse(cbo.SelectedValue.ToString(), out var selectedOrganismId))
            {
                // A valid item is not currently selected
                // Look for the first item that starts with the text

                SelectClosestMatchingOrganism(cbo);
                return;
            }

            SelectedOrganismID = selectedOrganismId;
            CheckTransferEnable();

            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    var fastaFilePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);

                    // Update organism in mSelectedFileList
                    var uploadInfo = mSelectedFileList[fastaFilePath];
                    uploadInfo.OrganismId = SelectedOrganismID;

                    mSelectedFileList[fastaFilePath] = uploadInfo;

                    // Update organism in lvwSelectedFiles
                    li.SubItems[(int)SelectedFileColumn.Organism].Text = cbo.Text;
                }
            }
        }

        private void cboAnnotationTypePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = (ComboBox)sender;

            if (ReferenceEquals(cboAnnotationTypePicker.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                SelectedAnnotationTypeID = (int)Math.Round(Convert.ToDouble(cboAnnotationTypePicker.SelectedValue));
            }

            CheckTransferEnable();

            if (SelectedAnnotationTypeID == -2)
            {
                // Bring up an additional dialog
                var annotationTypeHandler = new AddAnnotationTypeType(mDbConnectionString);
                RegisterEvents(annotationTypeHandler);

                annotationTypeHandler.FormLocation = new Point(Left + Width + 10, Top);

                var annotationTypeId = annotationTypeHandler.AddAnnotationType();

                if (!annotationTypeHandler.EntryExists && annotationTypeId > 0)
                {
                    var dataRow = mAnnotationTypeList.NewRow();

                    dataRow["id"] = annotationTypeId;
                    dataRow["display_name"] = annotationTypeHandler.DisplayName;
                    dataRow["details"] = annotationTypeHandler.Description;

                    mAnnotationTypeList.Rows.Add(dataRow);
                    mAnnotationTypeList.AcceptChanges();
                    LoadAnnotationTypePicker(cboAnnotationTypePicker, mAnnotationTypeList);
                    SelectedAnnotationTypeID = annotationTypeId;

                    cboAnnotationTypePicker.SelectedValue = annotationTypeId;
                }
            }

            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    // Update annotation type in mSelectedFileList
                    var fastaFilePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);
                    var uploadInfo = mSelectedFileList[fastaFilePath];

                    mSelectedFileList[fastaFilePath] =
                        new PSUploadHandler.UploadInfo(
                            uploadInfo.FileInformation,
                            SelectedOrganismID,
                            SelectedAnnotationTypeID);

                    // Update annotation type in lvwSelectedFiles
                    li.SubItems[(int)SelectedFileColumn.AnnotationType].Text = cbo.Text;
                }
            }
        }

        private void CheckTransferEnable()
        {
            if (SelectedOrganismID > 0 && SelectedAnnotationTypeID > 0 && lvwFolderContents.SelectedItems.Count > 0)
            {
                mAllowAddFiles = true;
                mAllowAddFilesMessage = string.Empty;
            }
            else
            {
                mAllowAddFiles = false;
                mAllowAddFilesMessage = AddFilesMessage;
            }

            cmdRemoveFile.Enabled = lvwSelectedFiles.Items.Count > 0;
            cmdUploadChecked.Enabled = lvwSelectedFiles.Items.Count > 0;
            cmdPreviewFile.Enabled = lvwFolderContents.SelectedItems.Count > 0;
        }

        private void cmdUploadChecked_Click(object sender, EventArgs e)
        {
            var validInfo = MakeCheckedFileList();

            if (!validInfo)
                return;
            mReallyClose = true;
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
            if (!mAllowAddFiles)
            {
                MessageBox.Show(mAllowAddFilesMessage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (mFilePreviewer == null)
            {
                mFilePreviewer = new FilePreviewHandler(mDbConnectionString);
                RegisterEvents(mFilePreviewer);

                mFilePreviewer.FormStatus += OnPreviewFormStatusChange;
            }

            if (lvwFolderContents.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a file to preview", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var li = lvwFolderContents.SelectedItems[0];
            var fullName = GetFolderContentsColumn(li, FolderContentsColumn.FilePath);

            mFilePreviewer.ShowPreview(fullName, Left + Width + 10, Top, Height);
        }

        private void cmdRefreshFiles_Click(object sender, EventArgs e)
        {
            InitializeTreeView(CurrentDirectory);
        }

        private void cmdUpdateDescription_Click(object sender, EventArgs e)
        {
            UpdateProteinCollectionMetadata();
        }

        private void cmdUpdateOrganisms_Click(object sender, EventArgs e)
        {
            var organismList = mImportHandler.LoadOrganisms();

            mOrganismListSorted = new DataView(organismList) { Sort = "display_name" };

            LoadOrganismPicker(cboOrganismSelect, mOrganismListSorted);
        }

        private void lvwSelectedFiles_Click(object sender, EventArgs e)
        {
            if (lvwSelectedFiles.SelectedItems.Count == 0)
                return;
            var li = lvwSelectedFiles.SelectedItems[0];

            var selectedOrganism = GetSelectedFileColumn(li, SelectedFileColumn.Organism);

            cboAnnotationTypePicker.Text = GetSelectedFileColumn(li, SelectedFileColumn.AnnotationType);
            cboOrganismSelect.Text = selectedOrganism;

            CheckTransferEnable();
        }

        private void lvwFolderContents_Click(object sender, EventArgs e)
        {
            lvwSelectedFiles.SelectedItems.Clear();
        }

        private void frmBatchAddNewCollection_Closing(object sender, CancelEventArgs e)
        {
            if (lvwSelectedFiles.Items.Count > 0 && !mReallyClose)
            {
                var r = MessageBox.Show("You have files selected for upload. Really close the form?",
                    "Files selected for upload", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                e.Cancel = r == DialogResult.No;
            }

            if (mFilePreviewer != null && mPreviewFormStatus)
            {
                mFilePreviewer.CloseForm();
                mFilePreviewer.FormStatus -= OnPreviewFormStatusChange;
                mFilePreviewer = null;
            }
        }

        private void lvwFolderContents_MouseUp(object sender, MouseEventArgs e)
        {
            CheckTransferEnable();
        }

        public void OnPreviewFormStatusChange(bool visibility)
        {
            if (visibility)
            {
                cmdPreviewFile.Enabled = false;
                mPreviewFormStatus = true;
            }
            else
            {
                cmdPreviewFile.Enabled = true;
                mPreviewFormStatus = false;
            }
        }

        private void txtMaximumProteinNameLength_Validating(object sender, CancelEventArgs e)
        {
            if (txtMaximumProteinNameLength.TextLength == 0)
            {
                txtMaximumProteinNameLength.Text = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH.ToString();
            }
            else
            {
                if (int.TryParse(txtMaximumProteinNameLength.Text, out var intValue))
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
            var isDateColumn = e.Column == (int)FolderContentsColumn.LastModified;
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

        private void lvwFolderContents_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshFiles();
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
                var folderPath = SelectedNodeFolderPath;

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
                RefreshFiles();
            }
        }

        private void RefreshFiles()
        {
            try
            {
                var folderPath = SelectedNodeFolderPath;

                if (string.IsNullOrWhiteSpace(folderPath))
                    return;
                var currentFolder = new DirectoryInfo(folderPath);

                while (!currentFolder.Exists && currentFolder.Parent != null)
                {
                    currentFolder = currentFolder.Parent;
                }

                ctlTreeViewFolderBrowser.Populate(currentFolder.FullName);

                InitializeTreeView(currentFolder.FullName);

                if (!ctlTreeViewFolderBrowser.SelectedNode.IsExpanded)
                {
                    ctlTreeViewFolderBrowser.SelectedNode.Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in RefreshFiles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void StatusResetTimer_Tick(object sender, EventArgs e)
        {
            if (mStatusResetRequired && DateTime.UtcNow > mStatusClearTime)
            {
                mStatusResetRequired = false;
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
                var returnVal = 0;
                var compared = false;

                if (mSortingDates)
                {
                    try
                    {
                        // Parse the two objects passed as a parameter as a DateTime
                        var dateA = DateTime.Parse(((ListViewItem)x)?.SubItems[mColIndex].Text);
                        var dateB = DateTime.Parse(((ListViewItem)y)?.SubItems[mColIndex].Text);

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
                    returnVal = string.CompareOrdinal(
                        ((ListViewItem)x)?.SubItems[mColIndex].Text,
                        ((ListViewItem)y)?.SubItems[mColIndex].Text);
                }

                if (mSortOrder == SortOrder.Descending)
                {
                    return returnVal * -1;
                }

                return returnVal;
            }
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
    }
}
