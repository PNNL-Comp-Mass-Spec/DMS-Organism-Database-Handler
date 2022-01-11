using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinUpload;
using ValidateFastaFile;

namespace AppUI_OrfDBHandler
{
    public partial class frmValidationReport : Form
    {
        public frmValidationReport()
        {
            base.Load += frmValidationReport_Load;

            InitializeComponent();

            mErrorCollection = new List<CustomFastaValidator.ErrorInfoExtended>();
            mWarningCollection = new List<CustomFastaValidator.ErrorInfoExtended>();
        }

        private readonly List<CustomFastaValidator.ErrorInfoExtended> mErrorCollection;
        private readonly List<CustomFastaValidator.ErrorInfoExtended> mWarningCollection;

        private Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> mFileErrorList;        // Tracks the errors found for each file
        private Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> mFileWarningList;      // Tracks the warnings found for each file

        /// <summary>
        /// Keys are FASTA file paths, values are upload info
        /// </summary>
        private Dictionary<string, PSUploadHandler.UploadInfo> mFileValidList;

        /// <summary>
        /// Keys are FASTA file names, values are dictionaries of error messages, tracking the count of each error
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> mSummarizedFileErrors;

        /// <summary>
        /// Keys are FASTA file names, values are dictionaries of warning messages, tracking the count of each warning
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> mSummarizedFileWarnings;

        private DataTable mOrganisms;

        private void frmValidationReport_Load(object sender, EventArgs e)
        {
            FillValidListView();
            BindFileListToErrorComboBox(mFileErrorList);
            BindFileListToWarningComboBox(mFileWarningList);

            if (cboFileListErrors.Items.Count > 0)
            {
                cboFileListErrors.SelectedIndex = 0;
                cboFileListErrors.Select();
                cboFileListErrors_SelectedIndexChanged(this, null);
            }

            if (cboFileListWarnings.Items.Count > 0)
            {
                cboFileListWarnings.SelectedIndex = 0;
                cboFileListWarnings.Select();
                cboFileListWarnings_SelectedIndexChanged(this, null);
            }
        }

        private void cmdExportErrorDetails_Click(object sender, EventArgs e)
        {
            if (mErrorCollection == null || mErrorCollection.Count == 0)
            {
                MessageBox.Show("Error list is empty; nothing to export", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DumpDetailedErrorOrWarningList(mErrorCollection, cboFileListErrors.Text, "Error");
            }
        }

        private void cmdExportWarningDetails_Click(object sender, EventArgs e)
        {
            if (mWarningCollection == null || mWarningCollection.Count == 0)
            {
                MessageBox.Show("Warning list is empty; nothing to export", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DumpDetailedErrorOrWarningList(mWarningCollection, cboFileListWarnings.Text, "Warning");
            }
        }

        private void cboFileListErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleErrorOrWarningListSelectedIndexChanged(
                cboFileListErrors.Text, lvwErrorList, mFileErrorList, mSummarizedFileErrors, mErrorCollection);
        }

        private void cboFileListWarnings_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleErrorOrWarningListSelectedIndexChanged(
                cboFileListWarnings.Text, lvwWarningList, mFileWarningList, mSummarizedFileWarnings, mWarningCollection);
        }

        private void HandleErrorOrWarningListSelectedIndexChanged(
            string selectedItemText,
            ListView objListView,
            IReadOnlyDictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> itemListByFile,
            IReadOnlyDictionary<string, Dictionary<string, int>> summarizedItemList,
            List<CustomFastaValidator.ErrorInfoExtended> itemCollection)
        {
            objListView.Items.Clear();
            if (itemListByFile?.Count > 0)
            {
                if (itemListByFile.TryGetValue(selectedItemText, out var itemList))
                {
                    itemCollection.AddRange(itemList);
                }
                else
                {
                    itemCollection.Clear();
                }
            }

            if (summarizedItemList?.Count > 0)
            {
                if (summarizedItemList.TryGetValue(selectedItemText, out var itemSummary))
                {
                    FillErrorOrWarningListView(objListView, itemSummary);
                }
            }
        }

        internal Dictionary<string, Dictionary<string, int>> ErrorSummaryList
        {
            set => mSummarizedFileErrors = value;
        }

        internal Dictionary<string, Dictionary<string, int>> WarningSummaryList
        {
            set => mSummarizedFileWarnings = value;
        }

        internal Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> FileErrorList
        {
            set => mFileErrorList = value;
        }

        internal Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> FileWarningList
        {
            set => mFileWarningList = value;
        }

        internal Dictionary<string, PSUploadHandler.UploadInfo> FileValidList
        {
            set => mFileValidList = value;
        }

        internal DataTable OrganismList
        {
            set => mOrganisms = value;
        }

        private string GetOrganismName(int organismId)
        {
            var foundRows = mOrganisms.Select("ID = " + organismId);

            return foundRows[0]["Display_Name"].ToString();
        }

        private void BindFileListToErrorComboBox(Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> contents)
        {
            cboFileListErrors.SelectedIndexChanged -= cboFileListErrors_SelectedIndexChanged;

            if (contents != null)
            {
                cboFileListErrors.BeginUpdate();

                foreach (var item in contents)
                {
                    cboFileListErrors.Items.Add(item.Key);
                }

                cboFileListErrors.EndUpdate();
            }
            else
            {
                cboFileListErrors.BeginUpdate();
                cboFileListErrors.Items.Add("-- No Errors --");
                cboFileListErrors.EndUpdate();
            }

            cboFileListErrors.SelectedIndexChanged += cboFileListErrors_SelectedIndexChanged;
        }

        private void BindFileListToWarningComboBox(Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> contents)
        {
            cboFileListWarnings.SelectedIndexChanged -= cboFileListWarnings_SelectedIndexChanged;

            if (contents != null)
            {
                cboFileListWarnings.BeginUpdate();

                foreach (var item in contents)
                {
                    cboFileListWarnings.Items.Add(item.Key);
                }

                cboFileListWarnings.EndUpdate();
            }
            else
            {
                cboFileListWarnings.BeginUpdate();
                cboFileListWarnings.Items.Add("-- No Warnings --");
                cboFileListWarnings.EndUpdate();
            }

            cboFileListWarnings.SelectedIndexChanged += cboFileListWarnings_SelectedIndexChanged;
        }

        private void FillErrorOrWarningListView(ListView objListView, Dictionary<string, int> itemSummary)
        {
            if (itemSummary != null)
            {
                objListView.BeginUpdate();
                objListView.Items.Clear();

                foreach (var item in itemSummary)
                {
                    var li = new ListViewItem(item.Value.ToString());
                    li.SubItems.Add(item.Key);
                    objListView.Items.Add(li);
                }

                objListView.EndUpdate();
            }
        }

        private void FillValidListView()
        {
            if (mFileValidList == null)
            {
                mFileValidList = new Dictionary<string, PSUploadHandler.UploadInfo>();
            }

            if (mFileValidList.Count == 0)
            {
                return;
            }

            lvwValidList.BeginUpdate();
            lvwValidList.Items.Clear();

            foreach (var item in mFileValidList)
            {
                var fileName = Path.GetFileName(item.Key);
                var uploadInfo = item.Value;

                var li = new ListViewItem(fileName);
                li.SubItems.Add(GetOrganismName(uploadInfo.OrganismId));
                li.SubItems.Add(uploadInfo.ProteinCount.ToString());
                li.SubItems.Add(uploadInfo.ExportedProteinCount.ToString());

                lvwValidList.Items.Add(li);
            }

            lvwValidList.EndUpdate();
        }

        private void DumpDetailedErrorOrWarningList(
            IReadOnlyCollection<CustomFastaValidator.ErrorInfoExtended> errorList,
            string fastaFileName,
            string messageType)
        {
            var saveDialog = new SaveFileDialog();

            string selectedSavePath;

            var intErrorCount = 0;

            if (string.IsNullOrWhiteSpace(messageType))
            {
                messageType = "Error";
            }

            saveDialog.Title = "Save Protein Database File";
            saveDialog.DereferenceLinks = true;
            saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveDialog.FilterIndex = 1;
            saveDialog.RestoreDirectory = true;
            saveDialog.OverwritePrompt = true;
            saveDialog.FileName = Path.GetFileNameWithoutExtension(fastaFileName) + "_" + messageType;

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                selectedSavePath = saveDialog.FileName;
            }
            else
            {
                return;
            }

            using var writer = new StreamWriter(new FileStream(selectedSavePath, FileMode.Create, FileAccess.Write, FileShare.Read));

            writer.WriteLine("Protein Name" + "\t" +
                             "Line Number" + "\t" +
                             "Message Type" + "\t" +
                             "Message");

            if (errorList?.Count > 0)
            {
                foreach (var errorDetail in errorList)
                {
                    writer.WriteLine(
                        errorDetail.ProteinName + "\t" +
                        errorDetail.LineNumber + "\t" +
                        errorDetail.Type + "\t" +
                        errorDetail.MessageText);
                    intErrorCount++;
                }
            }

            writer.WriteLine();

            MessageBox.Show("Wrote " + intErrorCount + " " + messageType + "s to " + saveDialog.FileName, "Detailed " + messageType + " List", MessageBoxButtons.OK);
        }
    }
}
