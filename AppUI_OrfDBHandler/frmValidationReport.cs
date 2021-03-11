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

            m_ErrorCollection = new List<CustomFastaValidator.ErrorInfoExtended>();
            m_WarningCollection = new List<CustomFastaValidator.ErrorInfoExtended>();
        }

        private readonly List<CustomFastaValidator.ErrorInfoExtended> m_ErrorCollection;
        private readonly List<CustomFastaValidator.ErrorInfoExtended> m_WarningCollection;

        private Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> m_FileErrorList;        // Tracks the errors found for each file
        private Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> m_FileWarningList;      // Tracks the warnings found for each file

        /// <summary>
        /// Keys are fasta file paths, values are upload info
        /// </summary>
        private Dictionary<string, PSUploadHandler.UploadInfo> m_FileValidList;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of error messages, tracking the count of each error
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> m_SummarizedFileErrors;

        /// <summary>
        /// Keys are fasta file names, values are dictionaries of warning messages, tracking the count of each warning
        /// </summary>
        private Dictionary<string, Dictionary<string, int>> m_SummarizedFileWarnings;

        private DataTable m_Organisms;

        private void frmValidationReport_Load(object sender, EventArgs e)
        {
            FillValidListView();
            BindFileListToErrorComboBox(m_FileErrorList);
            BindFileListToWarningComboBox(m_FileWarningList);

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
            if (m_ErrorCollection == null || m_ErrorCollection.Count == 0)
            {
                MessageBox.Show("Error list is empty; nothing to export", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DumpDetailedErrorOrWarningList(m_ErrorCollection, cboFileListErrors.Text, "Error");
            }
        }

        private void cmdExportWarningDetails_Click(object sender, EventArgs e)
        {
            if (m_WarningCollection == null || m_WarningCollection.Count == 0)
            {
                MessageBox.Show("Warning list is empty; nothing to export", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DumpDetailedErrorOrWarningList(m_WarningCollection, cboFileListWarnings.Text, "Warning");
            }
        }

        private void cboFileListErrors_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleErrorOrWarningListSelectedIndexChanged(
                cboFileListErrors.Text, lvwErrorList, m_FileErrorList, m_SummarizedFileErrors, m_ErrorCollection);
        }

        private void cboFileListWarnings_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleErrorOrWarningListSelectedIndexChanged(
                cboFileListWarnings.Text, lvwWarningList, m_FileWarningList, m_SummarizedFileWarnings, m_WarningCollection);
        }

        private void HandleErrorOrWarningListSelectedIndexChanged(
            string selectedItemText,
            ListView objListView,
            IReadOnlyDictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> itemListByFile,
            IReadOnlyDictionary<string, Dictionary<string, int>> summarizedItemList,
            List<CustomFastaValidator.ErrorInfoExtended> itemCollection)
        {
            objListView.Items.Clear();
            if (itemListByFile != null && itemListByFile.Count > 0)
            {
                List<CustomFastaValidator.ErrorInfoExtended> itemList = null;

                if (itemListByFile.TryGetValue(selectedItemText, out itemList))
                {
                    itemCollection.AddRange(itemList);
                }
                else
                {
                    itemCollection.Clear();
                }
            }

            if (summarizedItemList != null && summarizedItemList.Count > 0)
            {
                Dictionary<string, int> itemSummary = null;

                if (summarizedItemList.TryGetValue(selectedItemText, out itemSummary))
                {
                    FillErrorOrWarningListView(objListView, itemSummary);
                }
            }
        }

        internal Dictionary<string, Dictionary<string, int>> ErrorSummaryList
        {
            set => m_SummarizedFileErrors = value;
        }

        internal Dictionary<string, Dictionary<string, int>> WarningSummaryList
        {
            set => m_SummarizedFileWarnings = value;
        }

        internal Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> FileErrorList
        {
            set => m_FileErrorList = value;
        }

        internal Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> FileWarningList
        {
            set => m_FileWarningList = value;
        }

        internal Dictionary<string, PSUploadHandler.UploadInfo> FileValidList
        {
            set => m_FileValidList = value;
        }

        internal DataTable OrganismList
        {
            set => m_Organisms = value;
        }

        private string GetOrganismName(int organismID)
        {
            var foundRows = m_Organisms.Select("ID = " + organismID.ToString());

            return foundRows[0]["Display_Name"].ToString();
        }

        private void BindFileListToErrorComboBox(Dictionary<string, List<CustomFastaValidator.ErrorInfoExtended>> contents)
        {
            cboFileListErrors.SelectedIndexChanged -= cboFileListErrors_SelectedIndexChanged;

            if (contents != null)
            {
                cboFileListErrors.BeginUpdate();

                foreach (var item in contents)
                    cboFileListErrors.Items.Add(item.Key);

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
                    cboFileListWarnings.Items.Add(item.Key);

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
            if (m_FileValidList == null)
            {
                m_FileValidList = new Dictionary<string, PSUploadHandler.UploadInfo>();
            }

            if (m_FileValidList.Count == 0)
            {
                return;
            }

            lvwValidList.BeginUpdate();
            lvwValidList.Items.Clear();

            foreach (var item in m_FileValidList)
            {
                string FileName = Path.GetFileName(item.Key);
                var uploadInfo = item.Value;

                var li = new ListViewItem(FileName);
                li.SubItems.Add(GetOrganismName(uploadInfo.OrganismID));
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
            var SaveDialog = new SaveFileDialog();

            string SelectedSavePath;

            int intErrorCount = 0;

            if (string.IsNullOrWhiteSpace(messageType))
            {
                messageType = "Error";
            }

            SaveDialog.Title = "Save Protein Database File";
            SaveDialog.DereferenceLinks = true;
            SaveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            SaveDialog.FilterIndex = 1;
            SaveDialog.RestoreDirectory = true;
            SaveDialog.OverwritePrompt = true;
            SaveDialog.FileName = Path.GetFileNameWithoutExtension(fastaFileName) + "_" + messageType;

            if (SaveDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedSavePath = SaveDialog.FileName;
            }
            else
            {
                return;
            }

            using (var writer = new StreamWriter(new FileStream(SelectedSavePath, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                writer.WriteLine("Protein Name" + "\t" +
                                 "Line Number" + "\t" +
                                 "Message Type" + "\t" +
                                 "Message");

                if (errorList != null && errorList.Count > 0)
                {
                    foreach (var errorDetail in errorList)
                    {
                        writer.WriteLine(
                            errorDetail.ProteinName + "\t" +
                            errorDetail.LineNumber + "\t" +
                            errorDetail.Type + "\t" +
                            errorDetail.MessageText);
                        intErrorCount += 1;
                    }
                }

                writer.WriteLine();
            }

            MessageBox.Show("Wrote " + intErrorCount.ToString() + " " + messageType + "s to " + SaveDialog.FileName, "Detailed " + messageType + " List", MessageBoxButtons.OK);
        }
    }
}
