using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler.ProteinUpload
{
    public partial class frmBatchUploadFromFileList : Form
    {
        private readonly DataTable m_AnnotationTypeList;
        private readonly DataTable m_OrganismList;
        private const string m_SaveFileName = "FASTAFile_NamingAuth_XRef.txt";
        private readonly string m_SavePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public frmBatchUploadFromFileList(
            DataTable AuthorityList,
            DataTable AnnotationTypeList,
            DataTable OrganismList)
        {
            base.Load += frmBatchUploadFromFileList_Load;
            base.Closing += frmBatchUploadFromFileList_Closing;
            m_AnnotationTypeList = AnnotationTypeList;
            m_OrganismList = OrganismList;

            InitializeComponent();
        }

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
                        writer.Write("\t");
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
        //             fields = s.Split('\t');
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
            //         fields = s.Split('\t');
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
                    fli.NamingAuthorityID = Convert.ToInt32(cbo.SelectedValue);
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
                    fli.AnnotationTypeID = Convert.ToInt32(cbo.SelectedValue);
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