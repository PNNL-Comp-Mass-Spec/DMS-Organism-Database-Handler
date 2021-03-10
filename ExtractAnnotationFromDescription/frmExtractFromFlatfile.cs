using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ExtractAnnotationFromDescription
{
    public partial class frmExtractFromFlatfile : Form
    {
        public frmExtractFromFlatfile(Dictionary<string, string> AuthorityList, string psConnectionString)
        {
            base.Load += frmExtractFromFlatfile_Load;
            m_AuthorityList = AuthorityList;
            m_PSConnectionString = psConnectionString;

            InitializeComponent();
        }

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
