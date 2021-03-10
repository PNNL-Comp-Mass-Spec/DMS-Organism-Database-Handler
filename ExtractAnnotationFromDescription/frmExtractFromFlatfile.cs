using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExtractAnnotationFromDescription
{
    public class frmExtractFromFlatfile : System.Windows.Forms.Form
    {
        #region "Windows Form Designer generated code"

        public frmExtractFromFlatfile(Dictionary<string, string> AuthorityList, string psConnectionString) : base()
        {
            base.Load += frmExtractFromFlatfile_Load;
            m_AuthorityList = AuthorityList;
            m_PSConnectionString = psConnectionString;
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
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        internal System.Windows.Forms.Label lblNewNames;
        // Friend WithEvents lvwNewNames As System.Windows.Forms.ListView
        internal System.Windows.Forms.Button cmdUploadAnnotations;
        internal System.Windows.Forms.Label lblCurrentCollectionInfo;
        internal System.Windows.Forms.Label lblNamingAuthority;
        internal System.Windows.Forms.ComboBox cboNamingAuthority;
        internal System.Windows.Forms.ListView lvwProteins;
        internal System.Windows.Forms.ListView lvwNewNames;
        internal System.Windows.Forms.ColumnHeader colAnnGroup;
        internal System.Windows.Forms.ColumnHeader colAnnGroupName;
        internal System.Windows.Forms.ColumnHeader colNamingAuth;
        internal System.Windows.Forms.CheckBox chkUseHeader;
        internal System.Windows.Forms.ColumnHeader colSplitChar;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.Label Label1;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            lblNewNames = new System.Windows.Forms.Label();
            lvwNewNames = new System.Windows.Forms.ListView();
            lvwNewNames.SelectedIndexChanged += new EventHandler(lvwNewNames_SelectedIndexChanged);
            colAnnGroup = new System.Windows.Forms.ColumnHeader();
            colAnnGroupName = new System.Windows.Forms.ColumnHeader();
            colNamingAuth = new System.Windows.Forms.ColumnHeader();
            colSplitChar = new System.Windows.Forms.ColumnHeader();
            cmdUploadAnnotations = new System.Windows.Forms.Button();
            lblCurrentCollectionInfo = new System.Windows.Forms.Label();
            lblNamingAuthority = new System.Windows.Forms.Label();
            cboNamingAuthority = new System.Windows.Forms.ComboBox();
            cboNamingAuthority.SelectedIndexChanged += new EventHandler(cboNamingAuthority_SelectedIndexChanged);
            lvwProteins = new System.Windows.Forms.ListView();
            chkUseHeader = new System.Windows.Forms.CheckBox();
            chkUseHeader.CheckedChanged += new EventHandler(chkUseHeader_CheckedChanged);
            TextBox1 = new System.Windows.Forms.TextBox();
            Label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            //
            // lblNewNames
            //
            lblNewNames.Location = new System.Drawing.Point(11, 287);
            lblNewNames.Name = "lblNewNames";
            lblNewNames.Size = new System.Drawing.Size(482, 19);
            lblNewNames.TabIndex = 6;
            lblNewNames.Text = "Annotations Extracted from Loaded Flat text file";
            //
            // lvwNewNames
            //
            lvwNewNames.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvwNewNames.CheckBoxes = true;
            lvwNewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colAnnGroup, colAnnGroupName, colNamingAuth, colSplitChar });
            lvwNewNames.FullRowSelect = true;
            lvwNewNames.GridLines = true;
            lvwNewNames.HideSelection = false;
            lvwNewNames.Location = new System.Drawing.Point(11, 306);
            lvwNewNames.MultiSelect = false;
            lvwNewNames.Name = "lvwNewNames";
            lvwNewNames.Size = new System.Drawing.Size(797, 259);
            lvwNewNames.TabIndex = 18;
            lvwNewNames.UseCompatibleStateImageBehavior = false;
            lvwNewNames.View = System.Windows.Forms.View.Details;
            //
            // colAnnGroup
            //
            colAnnGroup.Text = "Group ID";
            colAnnGroup.Width = 59;
            //
            // colAnnGroupName
            //
            colAnnGroupName.Text = "Group Name";
            colAnnGroupName.Width = 145;
            //
            // colNamingAuth
            //
            colNamingAuth.Text = "Naming Authority";
            colNamingAuth.Width = 290;
            //
            // colSplitChar
            //
            colSplitChar.Text = "Delimiter";
            colSplitChar.Width = 59;
            //
            // cmdUploadAnnotations
            //
            cmdUploadAnnotations.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cmdUploadAnnotations.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdUploadAnnotations.Location = new System.Drawing.Point(587, 631);
            cmdUploadAnnotations.Name = "cmdUploadAnnotations";
            cmdUploadAnnotations.Size = new System.Drawing.Size(221, 27);
            cmdUploadAnnotations.TabIndex = 9;
            cmdUploadAnnotations.Text = "Upload Checked Groups";
            //
            // lblCurrentCollectionInfo
            //
            lblCurrentCollectionInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblCurrentCollectionInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            lblCurrentCollectionInfo.Location = new System.Drawing.Point(0, 257);
            lblCurrentCollectionInfo.Name = "lblCurrentCollectionInfo";
            lblCurrentCollectionInfo.Size = new System.Drawing.Size(825, 20);
            lblCurrentCollectionInfo.TabIndex = 12;
            lblCurrentCollectionInfo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            //
            // lblNamingAuthority
            //
            lblNamingAuthority.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            lblNamingAuthority.Location = new System.Drawing.Point(4, 573);
            lblNamingAuthority.Name = "lblNamingAuthority";
            lblNamingAuthority.Size = new System.Drawing.Size(247, 21);
            lblNamingAuthority.TabIndex = 16;
            lblNamingAuthority.Text = "Naming Authority";
            //
            // cboNamingAuthority
            //
            cboNamingAuthority.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboNamingAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboNamingAuthority.Location = new System.Drawing.Point(14, 592);
            cboNamingAuthority.Name = "cboNamingAuthority";
            cboNamingAuthority.Size = new System.Drawing.Size(676, 25);
            cboNamingAuthority.TabIndex = 17;
            //
            // lvwProteins
            //
            lvwProteins.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvwProteins.FullRowSelect = true;
            lvwProteins.GridLines = true;
            lvwProteins.Location = new System.Drawing.Point(1, 2);
            lvwProteins.MultiSelect = false;
            lvwProteins.Name = "lvwProteins";
            lvwProteins.Size = new System.Drawing.Size(819, 251);
            lvwProteins.TabIndex = 19;
            lvwProteins.UseCompatibleStateImageBehavior = false;
            lvwProteins.View = System.Windows.Forms.View.Details;
            //
            // chkUseHeader
            //
            chkUseHeader.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            chkUseHeader.Location = new System.Drawing.Point(553, 284);
            chkUseHeader.Name = "chkUseHeader";
            chkUseHeader.Size = new System.Drawing.Size(258, 20);
            chkUseHeader.TabIndex = 20;
            chkUseHeader.Text = "Use First Line as Group Names?";
            //
            // TextBox1
            //
            TextBox1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            TextBox1.Location = new System.Drawing.Point(701, 592);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new System.Drawing.Size(98, 24);
            TextBox1.TabIndex = 23;
            //
            // Label1
            //
            Label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            Label1.Location = new System.Drawing.Point(699, 573);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(95, 21);
            Label1.TabIndex = 22;
            Label1.Text = "Delimiter";
            //
            // frmExtractFromFlatfile
            //
            AutoScaleBaseSize = new System.Drawing.Size(7, 17);
            ClientSize = new System.Drawing.Size(822, 667);
            Controls.Add(TextBox1);
            Controls.Add(Label1);
            Controls.Add(chkUseHeader);
            Controls.Add(lvwProteins);
            Controls.Add(cboNamingAuthority);
            Controls.Add(lblNamingAuthority);
            Controls.Add(cmdUploadAnnotations);
            Controls.Add(lvwNewNames);
            Controls.Add(lblNewNames);
            Controls.Add(lblCurrentCollectionInfo);
            Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            MinimumSize = new System.Drawing.Size(840, 712);
            Name = "frmExtractFromFlatfile";
            Text = "Extract Annotations From Flatfile";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private bool m_UseHeaderInfo = false;
        private ExtractFromFlatFile m_Extract;
        private Dictionary<string, string> m_AuthorityList;
        private int m_CurrentAuthorityID;
        private int m_CurrentGroupID;
        private string m_PSConnectionString;

        private void frmExtractFromFlatfile_Load(object sender, EventArgs e)
        {
            m_Extract = new ExtractFromFlatFile(m_AuthorityList, m_PSConnectionString);
            var openFrm = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult r;
            string filePath;

            chkUseHeader.CheckedChanged -= chkUseHeader_CheckedChanged;

            if (m_UseHeaderInfo == true)
            {
                chkUseHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            }
            else
            {
                chkUseHeader.CheckState = System.Windows.Forms.CheckState.Unchecked;
            }

            chkUseHeader.CheckedChanged += chkUseHeader_CheckedChanged;

            LoadAuthorityCombobox(cboNamingAuthority, m_AuthorityList);

            openFrm.Filter = "tab-delimited text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFrm.AddExtension = true;
            openFrm.DefaultExt = "txt";
            openFrm.DereferenceLinks = true;
            openFrm.FilterIndex = 0;
            openFrm.Multiselect = false;
            openFrm.Title = "Select a tab delimited text file to load";
            r = openFrm.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.OK)
            {
                filePath = openFrm.FileName;
                m_Extract.LoadFile(filePath, "\t", m_UseHeaderInfo);
                LoadRawFileListView();
                LoadAnnotationGroupListView();
            }
            else
            {
                Close();
            }
        }

        private void lvwNewNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ListView lvw = (System.Windows.Forms.ListView)sender;
            if (lvw.SelectedItems.Count > 0)
            {
                m_CurrentGroupID = Convert.ToInt32(lvw.SelectedItems[0].Text);
                cboNamingAuthority.SelectedValue = Convert.ToInt32(lvw.SelectedItems[0].Tag);
                // cboNamingAuthority.Select();
            }
        }

        private void chkUseHeader_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.CheckBox chk = (System.Windows.Forms.CheckBox)sender;

            if (chk.CheckState == System.Windows.Forms.CheckState.Checked)
            {
                m_UseHeaderInfo = true;
            }
            else
            {
                m_UseHeaderInfo = false;
            }

            m_Extract.LoadGroups("\t", m_UseHeaderInfo);
            RefreshRawFileListViewHeaders();
            LoadAnnotationGroupListView();
        }

        private void RefreshRawFileListViewHeaders()
        {
            var columnCollection = m_Extract.ColumnNames;

            int columnCount = columnCollection.Count;
            int columnNumber;
            for (columnNumber = 1; columnNumber <= columnCount; columnNumber++)
                lvwProteins.Columns[columnNumber - 1].Text = columnCollection[columnNumber];

            if (m_UseHeaderInfo)
            {
                lvwProteins.Items.RemoveAt(0);
            }
            else
            {
                lvwProteins.Items.Insert(0, m_Extract.DataLineToListViewItem(m_Extract.FileContents[0], 1));
            }
        }

        private void LoadRawFileListView()
        {
            var fc = m_Extract.FileContents;
            int maxIndex = fc.Count - 1;
            int maxColumnCount = 0;

            foreach (var item in fc)
            {
                if (item.Count > maxColumnCount)
                {
                    maxColumnCount = item.Count;
                }
            }

            lvwProteins.BeginUpdate();

            lvwProteins.Clear();

            // Create Columns
            var columnCollection = m_Extract.ColumnNames;

            int columnCount = columnCollection.Count;
            int columnNumber;
            for (columnNumber = 1; columnNumber <= columnCount; columnNumber++)
            {
                var ch = new System.Windows.Forms.ColumnHeader();
                ch.Text = columnCollection[columnNumber];
                ch.Width = 70;
                lvwProteins.Columns.Add(ch);
            }

            for (int lineCount = 0; lineCount <= maxIndex; lineCount++)
            {
                var lineHash = fc[lineCount];
                var lvItem = m_Extract.DataLineToListViewItem(lineHash, lineCount);

                // lvItem = new System.Windows.Forms.ListViewItem((lineCount + 1).ToString())
                // columnCount = lineHash.Count;
                // for (var columnNumber = 1; columnNumber <= columnCount; columnNumber++)
                // {
                //     item = lineHash[columnNumber].ToString();
                //     if (item.Length > 0)
                //         lvItem.SubItems.Add(lineHash.Item(columnNumber).ToString());
                //     else
                //         lvItem.SubItems.Add("---");
                // }
                // blankColumnCount = maxColumnCount - columnCount;
                // if (blankColumnCount > 0)
                // {
                //     for (var columnNumber = 1; columnNumber <= blankColumnCount; columnNumber++)
                //         lvItem.SubItems.Add("---");
                // }

                lvwProteins.Items.Add(lvItem);
            }

            lvwProteins.EndUpdate();
        }

        private void LoadAnnotationGroupListView()
        {
            int maxIndex = m_Extract.Annotations.GroupCount;
            int groupID;
            System.Windows.Forms.ListViewItem lvItem;

            lvwNewNames.BeginUpdate();
            lvwNewNames.Items.Clear();;
            for (groupID = 1; groupID <= maxIndex; groupID++)
            {
                lvItem = m_Extract.GetListViewItemForGroup(groupID);
                lvwNewNames.Items.Add(lvItem);
            }

            lvwNewNames.EndUpdate();
        }

        private void LoadAuthorityCombobox(
            System.Windows.Forms.ComboBox cbo,
            Dictionary<string, string> authorityList)
        {
            var a = new ArrayList();

            cbo.BeginUpdate();
            foreach (var item in authorityList)
            {
                string authorityName = item.Value.ToString();
                int authorityId = Convert.ToInt32(item.Key);
                a.Add(new AuthorityContainer(authorityName, authorityId));
            }

            a.Sort(new AuthorityContainerComparer());

            cbo.DataSource = a;
            cbo.DisplayMember = "AuthorityName";
            cbo.ValueMember = "AuthorityID";

            cbo.EndUpdate();
        }

        private void cboNamingAuthority_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox cbo = (System.Windows.Forms.ComboBox)sender;

            if (lvwNewNames.SelectedItems.Count > 0)
            {
                m_Extract.ChangeAuthorityIDforGroup(m_CurrentGroupID, Convert.ToInt32(cbo.SelectedValue));
                lvwNewNames.SelectedItems[0].SubItems[2].Text = cbo.Text;
                lvwNewNames.SelectedItems[0].Tag = Convert.ToInt32(cbo.SelectedValue);
            }
        }

        private class AuthorityContainer
        {
            public AuthorityContainer(string authName, int authId)
            {
                AuthorityID = authId;
                AuthorityName = authName;
            }

            public string AuthorityName { get; private set; }

            public int AuthorityID { get; private set; }
        }

        private class AuthorityContainerComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                AuthorityContainer auth_1 = (AuthorityContainer)x;
                AuthorityContainer auth_2 = (AuthorityContainer)y;

                string reference_1 = auth_1.AuthorityName;
                string reference_2 = auth_2.AuthorityName;

                if (string.Compare(reference_1, reference_2, StringComparison.Ordinal) > 0)
                {
                    return 1;
                }
                else if (string.Compare(reference_1, reference_2, StringComparison.Ordinal) < 0)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}