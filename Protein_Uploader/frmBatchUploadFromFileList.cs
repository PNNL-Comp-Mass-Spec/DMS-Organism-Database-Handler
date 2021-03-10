using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Protein_Uploader
{
    public class frmBatchUploadFromFileList : Form
    {
        private readonly DataTable m_AnnotationTypeList;
        private readonly DataTable m_OrganismList;
        private const string m_SaveFileName = "FASTAFile_NamingAuth_XRef.txt";
        private readonly string m_SavePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        #region "Windows Form Designer generated code"

        public frmBatchUploadFromFileList(
            DataTable AuthorityList,
            DataTable AnnotationTypeList,
            DataTable OrganismList)
            : base()
        {
            base.Load += frmBatchUploadFromFileList_Load;
            base.Closing += frmBatchUploadFromFileList_Closing;
            m_AnnotationTypeList = AnnotationTypeList;
            m_OrganismList = OrganismList;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call

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
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        // Friend WithEvents lvwNewNames As ListView
        internal Button cmdUploadFiles;
        internal ColumnHeader colFileName;
        internal ColumnHeader colFilePath;
        internal ColumnHeader colAnnType;
        internal Button cmdCheckAll;
        internal ColumnHeader colOrganism;
        internal ComboBox cboAnnotationType;
        internal Button cmdUncheckAll;
        internal Label lblAnnotationType;
        internal ListView lvwFiles;
        internal Label lblOrganismPicker;
        internal ComboBox cboOrganismPicker;
        internal TextBox txtFilePath;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            cmdUploadFiles = new Button();
            cmdUploadFiles.Click += new EventHandler(cmdUploadFiles_Click);
            lblOrganismPicker = new Label();
            cboOrganismPicker = new ComboBox();
            cboOrganismPicker.SelectedIndexChanged += new EventHandler(cboOrganismPicker_SelectedIndexChanged);
            lvwFiles = new ListView();
            lvwFiles.SelectedIndexChanged += new EventHandler(lvwFiles_SelectedIndexChanged);
            colFileName = new ColumnHeader();
            colFilePath = new ColumnHeader();
            colOrganism = new ColumnHeader();
            colAnnType = new ColumnHeader();
            cboAnnotationType = new ComboBox();
            cboAnnotationType.SelectedIndexChanged += new EventHandler(cboAnnotationType_SelectedIndexChanged);
            lblAnnotationType = new Label();
            cmdCheckAll = new Button();
            cmdCheckAll.Click += new EventHandler(cmdCheckAll_Click);
            cmdUncheckAll = new Button();
            cmdUncheckAll.Click += new EventHandler(cmdUncheckAll_Click);
            txtFilePath = new TextBox();
            SuspendLayout();
            //
            // cmdUploadFiles
            //
            cmdUploadFiles.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdUploadFiles.DialogResult = DialogResult.OK;
            cmdUploadFiles.FlatStyle = FlatStyle.System;
            cmdUploadFiles.Location = new System.Drawing.Point(587, 630);
            cmdUploadFiles.Name = "cmdUploadFiles";
            cmdUploadFiles.Size = new System.Drawing.Size(221, 27);
            cmdUploadFiles.TabIndex = 9;
            cmdUploadFiles.Text = "Upload Checked Files";
            //
            // lblOrganismPicker
            //
            lblOrganismPicker.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblOrganismPicker.Location = new System.Drawing.Point(14, 569);
            lblOrganismPicker.Name = "lblOrganismPicker";
            lblOrganismPicker.Size = new System.Drawing.Size(319, 22);
            lblOrganismPicker.TabIndex = 16;
            lblOrganismPicker.Text = "Organism";
            //
            // cboOrganismPicker
            //
            cboOrganismPicker.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cboOrganismPicker.Location = new System.Drawing.Point(14, 589);
            cboOrganismPicker.Name = "cboOrganismPicker";
            cboOrganismPicker.Size = new System.Drawing.Size(385, 25);
            cboOrganismPicker.TabIndex = 17;
            //
            // lvwFiles
            //
            lvwFiles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwFiles.CheckBoxes = true;
            lvwFiles.Columns.AddRange(new ColumnHeader[] { colFileName, colFilePath, colOrganism, colAnnType });
            lvwFiles.FullRowSelect = true;
            lvwFiles.GridLines = true;
            lvwFiles.HideSelection = false;
            lvwFiles.Location = new System.Drawing.Point(1, 2);
            lvwFiles.Name = "lvwFiles";
            lvwFiles.Size = new System.Drawing.Size(820, 555);
            lvwFiles.Sorting = SortOrder.Ascending;
            lvwFiles.TabIndex = 19;
            lvwFiles.UseCompatibleStateImageBehavior = false;
            lvwFiles.View = View.Details;
            //
            // colFileName
            //
            colFileName.Text = "FileName";
            colFileName.Width = 215;
            //
            // colFilePath
            //
            colFilePath.Text = "Directory Path of File";
            colFilePath.Width = 247;
            //
            // colOrganism
            //
            colOrganism.Text = "Organism";
            colOrganism.Width = 125;
            //
            // colAnnType
            //
            colAnnType.Text = "Annotation Type";
            colAnnType.Width = 117;
            //
            // cboAnnotationType
            //
            cboAnnotationType.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cboAnnotationType.Location = new System.Drawing.Point(419, 589);
            cboAnnotationType.Name = "cboAnnotationType";
            cboAnnotationType.Size = new System.Drawing.Size(392, 25);
            cboAnnotationType.TabIndex = 21;
            //
            // lblAnnotationType
            //
            lblAnnotationType.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblAnnotationType.Location = new System.Drawing.Point(422, 569);
            lblAnnotationType.Name = "lblAnnotationType";
            lblAnnotationType.Size = new System.Drawing.Size(294, 22);
            lblAnnotationType.TabIndex = 20;
            lblAnnotationType.Text = "Annotation Type";
            //
            // cmdCheckAll
            //
            cmdCheckAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmdCheckAll.FlatStyle = FlatStyle.System;
            cmdCheckAll.Location = new System.Drawing.Point(14, 630);
            cmdCheckAll.Name = "cmdCheckAll";
            cmdCheckAll.Size = new System.Drawing.Size(140, 27);
            cmdCheckAll.TabIndex = 22;
            cmdCheckAll.Text = "Check All";
            //
            // cmdUncheckAll
            //
            cmdUncheckAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            cmdUncheckAll.FlatStyle = FlatStyle.System;
            cmdUncheckAll.Location = new System.Drawing.Point(165, 630);
            cmdUncheckAll.Name = "cmdUncheckAll";
            cmdUncheckAll.Size = new System.Drawing.Size(140, 27);
            cmdUncheckAll.TabIndex = 23;
            cmdUncheckAll.Text = "Uncheck All";
            //
            // txtFilePath
            //
            txtFilePath.Location = new System.Drawing.Point(319, 641);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.Size = new System.Drawing.Size(431, 24);
            txtFilePath.TabIndex = 24;
            //
            // frmBatchUploadFromFileList
            //
            AutoScaleBaseSize = new System.Drawing.Size(7, 17);
            ClientSize = new System.Drawing.Size(822, 667);
            Controls.Add(txtFilePath);
            Controls.Add(cmdUncheckAll);
            Controls.Add(cmdCheckAll);
            Controls.Add(cboAnnotationType);
            Controls.Add(lblAnnotationType);
            Controls.Add(lvwFiles);
            Controls.Add(cboOrganismPicker);
            Controls.Add(lblOrganismPicker);
            Controls.Add(cmdUploadFiles);
            Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Conversions.ToByte(0));
            MinimumSize = new System.Drawing.Size(840, 712);
            Name = "frmBatchUploadFromFileList";
            Text = "Batch Upload FASTA Files from FileList";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private void frmBatchUploadFromFileList_Load(object sender, EventArgs e)
        {
            PopulateDropDowns();
            if (FileCollection != null)
            {
                // Me.LoadFileNamingAuthorities();
                PopulateListView();
            }
        }

        private void frmBatchUploadFromFileList_Closing(object sender, CancelEventArgs e)
        {
            SaveFileNamingAuthorities();
        }

        public Dictionary<string, BatchUploadFromFileList.FileListInfo> FileCollection { get; set; }

        private void SaveFileNamingAuthorities()
        {
            string saveFilePath = Path.Combine(m_SavePath, m_SaveFileName);

            var fi = new FileInfo(saveFilePath);
            if (fi.Exists)
            {
                fi.Delete();
            }

            using (var writer = new StreamWriter(Path.Combine(m_SavePath, m_SaveFileName)))
            {
                foreach (var fli in FileCollection.Values)
                {
                    if (fli.AnnotationTypeID > 0)
                    {
                        writer.Write(fli.FileName);
                        writer.Write(ControlChars.Tab);
                        writer.Write(fli.AnnotationTypeID.ToString());
                        writer.Flush();
                    }
                }
            }
        }

        // private void LoadFileNamingAuthorities()
        // {
        //     string loadFilePath = System.IO.Path.Combine(this.m_SavePath, this.m_SaveFileName);
        //     System.IO.FileInfo fi = new System.IO.FileInfo(loadFilePath);
        //     System.IO.TextReader tr;
        //     string s;
        //
        //     string tmpFileName;
        //     int tmpAnnotationID;
        //     string[] fields;
        //
        //     BatchUploadFromFileList.FileListInfo fli;
        //
        //     if (fi.Exists & !this.m_FileCollection == null)
        //     {
        //         tr = fi.OpenText();
        //         s = tr.ReadLine();
        //         while (!s == null)
        //         {
        //             fields = s.Split(ControlChars.Tab);
        //             tmpFileName = fields[0];
        //             tmpAnnotationID = System.Convert.ToInt32(fields[1]);
        //             if (tmpAnnotationID > 0)
        //             {
        //                 fli = (BatchUploadFromFileList.FileListInfo)this.m_FileCollection[tmpFileName];
        //                 fli.AnnotationTypeID = tmpAnnotationID;
        //                 this.m_FileCollection[tmpFileName] = fli;
        //             }
        //
        //             s = tr.ReadLine();
        //         }
        //     }
        //     else
        //         return;
        // }

        private void LoadFileNamingAuthorities()
        {
            string loadFilePath = Path.Combine(this.m_SavePath, m_SaveFileName);
            FileInfo fi = new FileInfo(loadFilePath);
            // System.IO.TextReader tr;
            // string s;
            //
            // string tmpFileName;
            // int tmpAnnotationID;
            // string[] fields;
            //
            // BatchUploadFromFileList.FileListInfo fli;
            //
            // if (fi.Exists & !this.m_FileCollection == null)
            // {
            //     tr = fi.OpenText();
            //     s = tr.ReadLine();
            //     while (!s == null)
            //     {
            //         fields = s.Split(ControlChars.Tab);
            //         tmpFileName = fields[0];
            //         tmpAnnotationID = System.Convert.ToInt32(fields[1]);
            //         if (tmpAnnotationID > 0)
            //         {
            //             fli = (BatchUploadFromFileList.FileListInfo)this.m_FileCollection[tmpFileName];
            //             fli.AnnotationTypeID = tmpAnnotationID;
            //             this.m_FileCollection[tmpFileName] = fli;
            //         }
            //         s = tr.ReadLine();
            //     }
            // }
            // else
            //     return;
        }


        public Dictionary<string, BatchUploadFromFileList.FileListInfo> SelectedFilesCollection { get; private set; } = new Dictionary<string, BatchUploadFromFileList.FileListInfo>(StringComparer.OrdinalIgnoreCase);

        private void PopulateDropDowns()
        {
            DataRow dr;

            dr = m_AnnotationTypeList.NewRow();
            {
                dr["ID"] = 0;
                dr["Display_Name"] = "---------";
            }

            m_AnnotationTypeList.Rows.InsertAt(dr, 0);
            m_AnnotationTypeList.AcceptChanges();

            dr = m_OrganismList.NewRow();
            dr["ID"] = 0;
            dr["Display_Name"] = "---------";

            m_OrganismList.Rows.InsertAt(dr, 0);
            m_OrganismList.AcceptChanges();

            cboAnnotationType.SelectedIndexChanged -= cboAnnotationType_SelectedIndexChanged;

                cboAnnotationType.BeginUpdate();
                cboAnnotationType.DisplayMember = "Display_Name";
                cboAnnotationType.ValueMember = "ID";
                cboAnnotationType.DataSource = m_AnnotationTypeList;
                cboAnnotationType.EndUpdate();

            cboAnnotationType.Text = "---------";
            cboAnnotationType.SelectedIndexChanged += cboAnnotationType_SelectedIndexChanged;
            cboOrganismPicker.SelectedIndexChanged -= cboOrganismPicker_SelectedIndexChanged;
                cboOrganismPicker.BeginUpdate();
                cboOrganismPicker.DisplayMember = "Display_Name";
                cboOrganismPicker.ValueMember = "ID";
                cboOrganismPicker.DataSource = m_OrganismList;
                cboOrganismPicker.EndUpdate();

            cboOrganismPicker.Text = "---------";

            cboOrganismPicker.SelectedIndexChanged += cboOrganismPicker_SelectedIndexChanged;
        }

        public void PopulateListView()
        {
            DataRow[] foundRows;

            lvwFiles.SelectedIndexChanged -= lvwFiles_SelectedIndexChanged;

            if (FileCollection.Count > 0)
            {
                lvwFiles.BeginUpdate();
                foreach (var fli in FileCollection.Values)
                {
                    var li = new ListViewItem() { Text = fli.FileName };
                    li.SubItems.Add(fli.FullFilePath);
                    li.SubItems.Add(fli.OrganismName);
                    if (fli.AnnotationTypeID > 0)
                    {
                        foundRows = m_AnnotationTypeList.Select("ID = " + fli.AnnotationTypeID);
                        fli.AnnotationType = foundRows[0][1].ToString();
                        li.SubItems.Add(fli.AnnotationType);
                    }
                    else
                    {
                        li.SubItems.Add("---------");
                    }

                    lvwFiles.Items.Add(li);
                }

                lvwFiles.EndUpdate();

                lvwFiles.SelectedIndexChanged += lvwFiles_SelectedIndexChanged;
            }
        }

        private int BuildSelectedFilesList()
        {
            SelectedFilesCollection.Clear();

            foreach (ListViewItem li in lvwFiles.CheckedItems)
                SelectedFilesCollection.Add(li.Text, FileCollection[li.Text]);

            return lvwFiles.CheckedItems.Count;
        }

        private void cmdUploadFiles_Click(object sender, EventArgs e)
        {
            int selectedCount = BuildSelectedFilesList();
            if (selectedCount > 0)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void cmdCheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in lvwFiles.Items)
                li.Checked = true;
        }

        private void cmdUncheckAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem li in lvwFiles.Items)
                li.Checked = false;
        }

        private void cboOrganismPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            BatchUploadFromFileList.FileListInfo fli;

            if (lvwFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwFiles.SelectedItems)
                {
                    li.SubItems[2].Text = cbo.Text;
                    fli = FileCollection[li.Text];
                    fli.NamingAuthorityID = Conversions.ToInteger(cbo.SelectedValue);
                    FileCollection[li.Text] = fli;
                }
            }
        }

        private void cboAnnotationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            BatchUploadFromFileList.FileListInfo fli;

            if (lvwFiles.SelectedItems.Count > 0)
            {
                foreach (ListViewItem li in lvwFiles.SelectedItems)
                {
                    li.SubItems[3].Text = cbo.Text;
                    fli = FileCollection[li.Text];
                    fli.AnnotationTypeID = Conversions.ToInteger(cbo.SelectedValue);
                    FileCollection[li.Text] = fli;
                }
            }
        }

        private void lvwFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            BatchUploadFromFileList.FileListInfo fli;
            ListViewItem li;

            cboAnnotationType.SelectedIndexChanged -= cboAnnotationType_SelectedIndexChanged;
            cboOrganismPicker.SelectedIndexChanged -= cboOrganismPicker_SelectedIndexChanged;
            if (lvwFiles.SelectedItems.Count == 0)
            {
                return;
            }
            else if (lvwFiles.SelectedItems.Count == 1)
            {
                li = lvwFiles.SelectedItems[0];
                fli = FileCollection[li.Text];
                if (fli.AnnotationTypeID > 0)
                {
                    cboAnnotationType.SelectedValue = fli.AnnotationTypeID;
                }

                if (fli.OrganismID > 0)
                {
                    cboOrganismPicker.SelectedValue = fli.OrganismID;
                }

                if (fli.FullFilePath.Length > 0)
                {
                    txtFilePath.Text = Path.GetDirectoryName(fli.FullFilePath);
                }
            }
            else if (lvwFiles.SelectedItems.Count > 1)
            {
                cboAnnotationType.SelectedValue = 0;
                cboOrganismPicker.SelectedValue = 0;
            }

            cboAnnotationType.SelectedIndexChanged += cboAnnotationType_SelectedIndexChanged;
            cboOrganismPicker.SelectedIndexChanged += cboOrganismPicker_SelectedIndexChanged;
        }
    }
}