using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Protein_Uploader;
using ValidateFastaFile;

namespace AppUI_OrfDBHandler
{
    public class frmValidationReport : Form
    {
        #region "Windows Form Designer generated code"

        public frmValidationReport() : base()
        {
            base.Load += frmValidationReport_Load;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call

            m_ErrorCollection = new List<clsCustomValidateFastaFiles.udtErrorInfoExtended>();
            m_WarningCollection = new List<clsCustomValidateFastaFiles.udtErrorInfoExtended>();
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
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        internal ColumnHeader colErrorDescription;
        internal ListView lvwErrorList;
        internal Label lblErrorList;
        internal Button cmdClose;
        internal GroupBox gbxValidFileList;
        internal GroupBox gbxInvalidFileList;
        internal ListView lvwValidList;
        internal ColumnHeader colFileName;
        internal ColumnHeader colOrganism;
        internal ColumnHeader colCount;
        internal ProgressBar pgbListViewLoad;
        internal ColumnHeader colNumOccurences;
        internal Button cmdExportErrorDetails;
        internal ColumnHeader colActualCount;
        internal GroupBox fraFastaFileWarnings;
        internal ListView lvwWarningList;
        internal Button cmdExportWarningDetails;
        internal ComboBox cboFileListWarnings;
        internal Label lblWarning;
        internal ColumnHeader ColumnHeader1;
        internal ColumnHeader ColumnHeader2;
        internal ComboBox cboFileListErrors;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            cboFileListErrors = new ComboBox();
            cboFileListErrors.SelectedIndexChanged += new EventHandler(cboFileListErrors_SelectedIndexChanged);
            cmdExportErrorDetails = new Button();
            cmdExportErrorDetails.Click += new EventHandler(cmdExportErrorDetails_Click);
            lblErrorList = new Label();
            cmdClose = new Button();
            lvwErrorList = new ListView();
            colNumOccurences = new ColumnHeader();
            colErrorDescription = new ColumnHeader();
            gbxValidFileList = new GroupBox();
            lvwValidList = new ListView();
            colFileName = new ColumnHeader();
            colOrganism = new ColumnHeader();
            colCount = new ColumnHeader();
            colActualCount = new ColumnHeader();
            gbxInvalidFileList = new GroupBox();
            pgbListViewLoad = new ProgressBar();
            fraFastaFileWarnings = new GroupBox();
            lvwWarningList = new ListView();
            ColumnHeader1 = new ColumnHeader();
            ColumnHeader2 = new ColumnHeader();
            cmdExportWarningDetails = new Button();
            cmdExportWarningDetails.Click += new EventHandler(cmdExportWarningDetails_Click);
            cboFileListWarnings = new ComboBox();
            cboFileListWarnings.SelectedIndexChanged += new EventHandler(cboFileListWarnings_SelectedIndexChanged);
            lblWarning = new Label();
            gbxValidFileList.SuspendLayout();
            gbxInvalidFileList.SuspendLayout();
            fraFastaFileWarnings.SuspendLayout();
            SuspendLayout();
            //
            // cboFileListErrors
            //
            cboFileListErrors.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboFileListErrors.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFileListErrors.Location = new Point(12, 28);
            cboFileListErrors.Name = "cboFileListErrors";
            cboFileListErrors.Size = new Size(448, 21);
            cboFileListErrors.TabIndex = 1;
            //
            // cmdExportErrorDetails
            //
            cmdExportErrorDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdExportErrorDetails.Location = new Point(468, 28);
            cmdExportErrorDetails.Name = "cmdExportErrorDetails";
            cmdExportErrorDetails.Size = new Size(114, 20);
            cmdExportErrorDetails.TabIndex = 3;
            cmdExportErrorDetails.Text = "Export Detailed List";
            //
            // lblErrorList
            //
            lblErrorList.Location = new Point(12, 56);
            lblErrorList.Name = "lblErrorList";
            lblErrorList.Size = new Size(406, 16);
            lblErrorList.TabIndex = 4;
            lblErrorList.Text = "Recorded Validation Errors";
            //
            // cmdClose
            //
            cmdClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdClose.DialogResult = DialogResult.OK;
            cmdClose.Location = new Point(520, 626);
            cmdClose.Name = "cmdClose";
            cmdClose.Size = new Size(84, 24);
            cmdClose.TabIndex = 5;
            cmdClose.Text = "Close";
            //
            // lvwErrorList
            //
            lvwErrorList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwErrorList.Columns.AddRange(new ColumnHeader[] { colNumOccurences, colErrorDescription });
            lvwErrorList.FullRowSelect = true;
            lvwErrorList.GridLines = true;
            lvwErrorList.Location = new Point(12, 76);
            lvwErrorList.MultiSelect = false;
            lvwErrorList.Name = "lvwErrorList";
            lvwErrorList.Size = new Size(572, 127);
            lvwErrorList.Sorting = SortOrder.Descending;
            lvwErrorList.TabIndex = 6;
            lvwErrorList.View = View.Details;
            //
            // colNumOccurences
            //
            colNumOccurences.Text = "Error Count";
            colNumOccurences.Width = 80;
            //
            // colErrorDescription
            //
            colErrorDescription.Text = "Error Description";
            colErrorDescription.Width = 488;
            //
            // gbxValidFileList
            //
            gbxValidFileList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbxValidFileList.Controls.Add(lvwValidList);
            gbxValidFileList.Location = new Point(10, 10);
            gbxValidFileList.Name = "gbxValidFileList";
            gbxValidFileList.Size = new Size(596, 146);
            gbxValidFileList.TabIndex = 7;
            gbxValidFileList.TabStop = false;
            gbxValidFileList.Text = "FASTA Files Successfully Uploaded";
            //
            // lvwValidList
            //
            lvwValidList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwValidList.Columns.AddRange(new ColumnHeader[] { colFileName, colOrganism, colCount, colActualCount });
            lvwValidList.FullRowSelect = true;
            lvwValidList.GridLines = true;
            lvwValidList.Location = new Point(12, 30);
            lvwValidList.MultiSelect = false;
            lvwValidList.Name = "lvwValidList";
            lvwValidList.Size = new Size(572, 101);
            lvwValidList.TabIndex = 7;
            lvwValidList.View = View.Details;
            //
            // colFileName
            //
            colFileName.Text = "File Name";
            colFileName.Width = 313;
            //
            // colOrganism
            //
            colOrganism.Text = "Organism";
            colOrganism.Width = 100;
            //
            // colCount
            //
            colCount.Text = "Protein Count";
            colCount.Width = 80;
            //
            // colActualCount
            //
            colActualCount.Text = "Actual Count";
            colActualCount.Width = 75;
            //
            // gbxInvalidFileList
            //
            gbxInvalidFileList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbxInvalidFileList.Controls.Add(lvwErrorList);
            gbxInvalidFileList.Controls.Add(cmdExportErrorDetails);
            gbxInvalidFileList.Controls.Add(cboFileListErrors);
            gbxInvalidFileList.Controls.Add(lblErrorList);
            gbxInvalidFileList.Location = new Point(10, 400);
            gbxInvalidFileList.Name = "gbxInvalidFileList";
            gbxInvalidFileList.Size = new Size(596, 216);
            gbxInvalidFileList.TabIndex = 8;
            gbxInvalidFileList.TabStop = false;
            gbxInvalidFileList.Text = "FASTA Files Not Uploaded Due to Errors";
            //
            // pgbListViewLoad
            //
            pgbListViewLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pgbListViewLoad.Location = new Point(10, 630);
            pgbListViewLoad.Name = "pgbListViewLoad";
            pgbListViewLoad.Size = new Size(496, 18);
            pgbListViewLoad.TabIndex = 9;
            pgbListViewLoad.Visible = false;
            //
            // fraFastaFileWarnings
            //
            fraFastaFileWarnings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            fraFastaFileWarnings.Controls.Add(lvwWarningList);
            fraFastaFileWarnings.Controls.Add(cmdExportWarningDetails);
            fraFastaFileWarnings.Controls.Add(cboFileListWarnings);
            fraFastaFileWarnings.Controls.Add(lblWarning);
            fraFastaFileWarnings.Location = new Point(8, 166);
            fraFastaFileWarnings.Name = "fraFastaFileWarnings";
            fraFastaFileWarnings.Size = new Size(596, 224);
            fraFastaFileWarnings.TabIndex = 10;
            fraFastaFileWarnings.TabStop = false;
            fraFastaFileWarnings.Text = "Fasta File Warnings";
            //
            // lvwWarningList
            //
            lvwWarningList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwWarningList.Columns.AddRange(new ColumnHeader[] { ColumnHeader1, ColumnHeader2 });
            lvwWarningList.FullRowSelect = true;
            lvwWarningList.GridLines = true;
            lvwWarningList.Location = new Point(12, 77);
            lvwWarningList.MultiSelect = false;
            lvwWarningList.Name = "lvwWarningList";
            lvwWarningList.Size = new Size(572, 136);
            lvwWarningList.Sorting = SortOrder.Descending;
            lvwWarningList.TabIndex = 6;
            lvwWarningList.View = View.Details;
            //
            // ColumnHeader1
            //
            ColumnHeader1.Text = "Warning Count";
            ColumnHeader1.Width = 80;
            //
            // ColumnHeader2
            //
            ColumnHeader2.Text = "Warning Description";
            ColumnHeader2.Width = 488;
            //
            // cmdExportWarningDetails
            //
            cmdExportWarningDetails.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdExportWarningDetails.Location = new Point(468, 29);
            cmdExportWarningDetails.Name = "cmdExportWarningDetails";
            cmdExportWarningDetails.Size = new Size(114, 20);
            cmdExportWarningDetails.TabIndex = 3;
            cmdExportWarningDetails.Text = "Export Detailed List";
            //
            // cboFileListWarnings
            //
            cboFileListWarnings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboFileListWarnings.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFileListWarnings.Location = new Point(12, 29);
            cboFileListWarnings.Name = "cboFileListWarnings";
            cboFileListWarnings.Size = new Size(448, 21);
            cboFileListWarnings.TabIndex = 1;
            //
            // lblWarning
            //
            lblWarning.Location = new Point(12, 56);
            lblWarning.Name = "lblWarning";
            lblWarning.Size = new Size(406, 16);
            lblWarning.TabIndex = 4;
            lblWarning.Text = "Recorded Validation Warnings";
            //
            // frmValidationReport
            //
            AutoScaleBaseSize = new Size(5, 14);
            ClientSize = new Size(614, 658);
            Controls.Add(fraFastaFileWarnings);
            Controls.Add(pgbListViewLoad);
            Controls.Add(gbxInvalidFileList);
            Controls.Add(gbxValidFileList);
            Controls.Add(cmdClose);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            MinimumSize = new Size(362, 416);
            Name = "frmValidationReport";
            Text = "FASTA File Validation Failure Report";
            gbxValidFileList.ResumeLayout(false);
            gbxInvalidFileList.ResumeLayout(false);
            fraFastaFileWarnings.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private readonly List<clsCustomValidateFastaFiles.udtErrorInfoExtended> m_ErrorCollection;
        private readonly List<clsCustomValidateFastaFiles.udtErrorInfoExtended> m_WarningCollection;

        private Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> m_FileErrorList;        // Tracks the errors found for each file
        private Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> m_FileWarningList;      // Tracks the warnings found for each file

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
            IReadOnlyDictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> itemListByFile,
            IReadOnlyDictionary<string, Dictionary<string, int>> summarizedItemList,
            List<clsCustomValidateFastaFiles.udtErrorInfoExtended> itemCollection)
        {
            objListView.Items.Clear();
            if (itemListByFile != null && itemListByFile.Count > 0)
            {
                List<clsCustomValidateFastaFiles.udtErrorInfoExtended> itemList = null;

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
            set
            {
                m_SummarizedFileErrors = value;
            }
        }

        internal Dictionary<string, Dictionary<string, int>> WarningSummaryList
        {
            set
            {
                m_SummarizedFileWarnings = value;
            }
        }

        internal Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> FileErrorList
        {
            set
            {
                m_FileErrorList = value;
            }
        }

        internal Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> FileWarningList
        {
            set
            {
                m_FileWarningList = value;
            }
        }

        internal Dictionary<string, PSUploadHandler.UploadInfo> FileValidList
        {
            set
            {
                m_FileValidList = value;
            }
        }

        internal DataTable OrganismList
        {
            set
            {
                m_Organisms = value;
            }
        }

        private string GetOrganismName(int organismID)
        {
            DataRow[] foundRows;

            foundRows = m_Organisms.Select("ID = " + organismID.ToString());

            return foundRows[0]["Display_Name"].ToString();
        }

        private void BindFileListToErrorComboBox(Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> contents)
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

        private void BindFileListToWarningComboBox(Dictionary<string, List<clsCustomValidateFastaFiles.udtErrorInfoExtended>> contents)
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
            ListViewItem li;

            if (itemSummary != null)
            {
                objListView.BeginUpdate();
                objListView.Items.Clear();

                foreach (var item in itemSummary)
                {
                    li = new ListViewItem(item.Value.ToString());
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
            IReadOnlyCollection<clsCustomValidateFastaFiles.udtErrorInfoExtended> errorList,
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