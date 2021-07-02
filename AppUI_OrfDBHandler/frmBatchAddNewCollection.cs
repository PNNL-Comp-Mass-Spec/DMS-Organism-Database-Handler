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
        // Ignore spelling: lvw, uploader, Passphrase, terabytes, petabytes, exabytes, zettabytes, yottabytes

        public frmBatchAddNewCollection(
            DataTable organismList,
            DataTable annotationTypeList,
            DataTable existingCollectionsList,
            string psConnectionString,
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

            mOrganismListSorted = new DataView(organismList) { Sort = "Display_Name" };

            mAnnotationTypeList = annotationTypeList;
            mCollectionsTable = existingCollectionsList;
            mPsConnectionString = psConnectionString;

            mCachedFileDescriptions = cachedFileDescriptions;

            ctlTreeViewFolderBrowser.DataSource = new TreeStrategyFolderBrowserProvider();
            ctlTreeViewFolderBrowser.CheckBoxBehaviorMode = CheckBoxBehaviorMode.None;

            InitializeTreeView(selectedFolderPath);
        }

        #region "Constants, enums, and member variables"

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
        /// Keys are full paths to the fasta file
        /// Values are FileInfo instances
        /// </summary>
        private Dictionary<string, FileInfo> mFileList;

        // Keys are file paths, values are UploadInfo objects
        private Dictionary<string, PSUploadHandler.UploadInfo> mSelectedFileList;

        private readonly DataView mOrganismListSorted;

        private readonly DataTable mAnnotationTypeList;
        private readonly DataTable mCollectionsTable;

        /// <summary>
        /// Keys are protein collection ID
        /// Values are Protein Collection name
        /// </summary>
        private Dictionary<int, string> mCollectionsList;

        private readonly string mPsConnectionString;
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

        #endregion

        #region "Properties"

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
            get
            {
                if (cboOrganismSelect.Items.Count > 0)
                {
                    return cboOrganismSelect.Text;
                }

                return string.Empty;
            }
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

        #endregion

        private void frmBatchAddNewCollection_Load(object sender, EventArgs e)
        {
            mCollectionsList = CollectionsTableToList(mCollectionsTable);

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

        #region "Directory Loading"

        private Dictionary<int, string> CollectionsTableToList(DataTable dt)
        {
            var collectionInfo = new Dictionary<int, string>(dt.Rows.Count);
            var foundRows = dt.Select("", "Protein_Collection_ID");

            foreach (var dr in foundRows)
            {
                var tmpId = DatabaseUtilsExtensions.GetInteger(null, dr["Protein_Collection_ID"]);
                var tmpName = dr["FileName"].ToString();
                if (!collectionInfo.ContainsKey(tmpId))
                {
                    collectionInfo.Add(tmpId, tmpName);
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

        #endregion

        #region "UI Loading Functions"

        /// <summary>
        /// Populate the top ListView with fasta files in the selected folder
        /// </summary>
        private void LoadListView()
        {
            lvwFolderContents.BeginUpdate();

            lvwFolderContents.Items.Clear();

            mCollectionsList ??= new Dictionary<int, string>();

            foreach (var fi in mFileList.Values)
            {
                var proteinCollectionName = Path.GetFileNameWithoutExtension(fi.Name);

                var item = new ListViewItem
                {
                    // Fasta file name (with the extension)
                    Text = fi.Name
                };

                // Last Write Time
                item.SubItems.Add(fi.LastWriteTime.ToString("g"));

                // File Size
                item.SubItems.Add(Numeric2Bytes(fi.Length));

                // Whether or not the fasta file is already a protein collection
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
        /// <param name="value"></param>
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

            if (value >= 1d)
            {
                // One digit after the decimal
                return value.ToString("0.0");
            }

            // Two digits after the decimal
            return value.ToString("0.00");
        }

        #endregion

        private void AddFileToSelectedList()
        {
            try
            {
                mSelectedFileList ??= new Dictionary<string, PSUploadHandler.UploadInfo>(StringComparer.CurrentCultureIgnoreCase);

                foreach (ListViewItem li in lvwFolderContents.SelectedItems)
                {
                    var fastaFilePath = GetFolderContentsColumn(li, FolderContentsColumn.FilePath);

                    var upInfo = new PSUploadHandler.UploadInfo
                    {
                        FileInformation = mFileList[fastaFilePath],
                        OrganismId = (int)cboOrganismSelect.SelectedValue,
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
            var tmpNameList = new Dictionary<string, ProteinCollectionMetadata>();

            foreach (ListViewItem li in lvwSelectedFiles.Items)
            {
                var fastaFilePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);

                if (!tmpNameList.ContainsKey(fastaFilePath))
                {
                    var udtMetadata = new ProteinCollectionMetadata
                    {
                        Description = GetSelectedFileColumn(li, SelectedFileColumn.Description),
                        Source = GetSelectedFileColumn(li, SelectedFileColumn.Source)
                    };
                    tmpNameList.Add(fastaFilePath, udtMetadata);
                }
            }

            foreach (var item in mSelectedFileList)
            {
                var upInfo = item.Value;
                var fi = upInfo.FileInformation;

                if (tmpNameList.TryGetValue(fi.FullName, out var udtMetadata))
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
            // If the value is found, then the given item in the ComboBox is selected

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
            var updatedValue = value.Trim().Replace("\t", "; ").Replace(Environment.NewLine, "; ").Replace("\r", "; ").Replace("\n", "; ");

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

            var oMetadataWindow = new frmNewCollectionMetadataEditor()
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

                    var kvDescriptionSource = new KeyValuePair<string, string>(updatedDescription, updatedSource);

                    // Add/update the dictionary item
                    mCachedFileDescriptions[proteinCollection] = kvDescriptionSource;
                }
            }
        }

        #region "Button and Combo Handlers"

        private void cboOrganismSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = (ComboBox)sender;

            SelectedOrganismID = (int)cbo.SelectedValue;
            CheckTransferEnable();
            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    var fastaFilePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);

                    // Update organism in mSelectedFileList
                    var tmpUpInfo = mSelectedFileList[fastaFilePath];
                    tmpUpInfo.OrganismId = SelectedOrganismID;

                    mSelectedFileList[fastaFilePath] = tmpUpInfo;

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
                var annTypeAdd = new AddAnnotationTypeType(mPsConnectionString)
                {
                    FormLocation = new Point(Left + Width + 10, Top)
                };
                var tmpAnnTypeId = annTypeAdd.AddAnnotationType();
                // Dim AuthorityAdd As New AddNamingAuthority(mPSConnectionString)
                // tempAuthorityID = AuthAdd.AddNamingAuthority

                if (!annTypeAdd.EntryExists && tmpAnnTypeId > 0)
                {
                    var dr = mAnnotationTypeList.NewRow();

                    dr["ID"] = tmpAnnTypeId;
                    dr["Display_Name"] = annTypeAdd.DisplayName;
                    dr["Details"] = annTypeAdd.Description;

                    mAnnotationTypeList.Rows.Add(dr);
                    mAnnotationTypeList.AcceptChanges();
                    LoadAnnotationTypePicker(cboAnnotationTypePicker, mAnnotationTypeList);
                    SelectedAnnotationTypeID = tmpAnnTypeId;

                    cboAnnotationTypePicker.SelectedValue = tmpAnnTypeId;
                }
            }

            if (lvwSelectedFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwSelectedFiles.SelectedItems)
                {
                    // Update annotation type in mSelectedFileList
                    var fastaFilePath = GetSelectedFileColumn(li, SelectedFileColumn.FilePath);
                    var tmpUpInfo = mSelectedFileList[fastaFilePath];

                    mSelectedFileList[fastaFilePath] =
                        new PSUploadHandler.UploadInfo(
                            tmpUpInfo.FileInformation,
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
                mFilePreviewer = new FilePreviewHandler();
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

        #endregion

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
                    MessageBox.Show("Error in NodeMouseDoubleClick: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
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
                var returnVal = default(int);
                var compared = false;

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
                    returnVal = string.CompareOrdinal(((ListViewItem)x).SubItems[mColIndex].Text,
                                               ((ListViewItem)y).SubItems[mColIndex].Text);
                }

                if (mSortOrder == SortOrder.Descending)
                {
                    return returnVal * -1;
                }

                return returnVal;
            }
        }
    }
}
