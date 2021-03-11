using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler.ExtractAdditionalAnnotations
{
    public partial class frmExtractFromFlatFile : Form
    {
        public frmExtractFromFlatFile(Dictionary<string, string> authorityList, string psConnectionString)
        {
            base.Load += frmExtractFromFlatfile_Load;
            mAuthorityList = authorityList;
            mPSConnectionString = psConnectionString;

            InitializeComponent();
        }

        private bool mUseHeaderInfo = false;
        private ExtractFromFlatFile mExtract;
        private Dictionary<string, string> mAuthorityList;
        private int mCurrentAuthorityId;
        private int mCurrentGroupId;
        private string mPSConnectionString;

        private void frmExtractFromFlatfile_Load(object sender, EventArgs e)
        {
            mExtract = new ExtractFromFlatFile(mAuthorityList, mPSConnectionString);
            var openFrm = new System.Windows.Forms.OpenFileDialog();

            chkUseHeader.CheckedChanged -= chkUseHeader_CheckedChanged;

            if (mUseHeaderInfo == true)
            {
                chkUseHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            }
            else
            {
                chkUseHeader.CheckState = System.Windows.Forms.CheckState.Unchecked;
            }

            chkUseHeader.CheckedChanged += chkUseHeader_CheckedChanged;

            LoadAuthorityCombobox(cboNamingAuthority, mAuthorityList);

            openFrm.Filter = "tab-delimited text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFrm.AddExtension = true;
            openFrm.DefaultExt = "txt";
            openFrm.DereferenceLinks = true;
            openFrm.FilterIndex = 0;
            openFrm.Multiselect = false;
            openFrm.Title = "Select a tab delimited text file to load";
            var r = openFrm.ShowDialog();

            if (r == System.Windows.Forms.DialogResult.OK)
            {
                var filePath = openFrm.FileName;
                mExtract.LoadFile(filePath, "\t", mUseHeaderInfo);
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
            var lvw = (System.Windows.Forms.ListView)sender;
            if (lvw.SelectedItems.Count > 0)
            {
                mCurrentGroupId = Convert.ToInt32(lvw.SelectedItems[0].Text);
                cboNamingAuthority.SelectedValue = Convert.ToInt32(lvw.SelectedItems[0].Tag);
                // cboNamingAuthority.Select();
            }
        }

        private void chkUseHeader_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (System.Windows.Forms.CheckBox)sender;

            if (chk.CheckState == System.Windows.Forms.CheckState.Checked)
            {
                mUseHeaderInfo = true;
            }
            else
            {
                mUseHeaderInfo = false;
            }

            mExtract.LoadGroups("\t", mUseHeaderInfo);
            RefreshRawFileListViewHeaders();
            LoadAnnotationGroupListView();
        }

        private void RefreshRawFileListViewHeaders()
        {
            var columnCollection = mExtract.ColumnNames;

            var columnCount = columnCollection.Count;
            for (var columnNumber = 1; columnNumber <= columnCount; columnNumber++)
                lvwProteins.Columns[columnNumber - 1].Text = columnCollection[columnNumber];

            if (mUseHeaderInfo)
            {
                lvwProteins.Items.RemoveAt(0);
            }
            else
            {
                lvwProteins.Items.Insert(0, mExtract.DataLineToListViewItem(mExtract.FileContents[0], 1));
            }
        }

        private void LoadRawFileListView()
        {
            var fc = mExtract.FileContents;
            var maxIndex = fc.Count - 1;
            var maxColumnCount = 0;

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
            var columnCollection = mExtract.ColumnNames;

            var columnCount = columnCollection.Count;
            for (var columnNumber = 1; columnNumber <= columnCount; columnNumber++)
            {
                var ch = new System.Windows.Forms.ColumnHeader {Text = columnCollection[columnNumber], Width = 70};
                lvwProteins.Columns.Add(ch);
            }

            for (var lineCount = 0; lineCount <= maxIndex; lineCount++)
            {
                var lineHash = fc[lineCount];
                var lvItem = mExtract.DataLineToListViewItem(lineHash, lineCount);

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
            var maxIndex = mExtract.Annotations.GroupCount;

            lvwNewNames.BeginUpdate();
            lvwNewNames.Items.Clear();;
            for (var groupId = 1; groupId <= maxIndex; groupId++)
            {
                var lvItem = mExtract.GetListViewItemForGroup(groupId);
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
                var authorityName = item.Value.ToString();
                var authorityId = Convert.ToInt32(item.Key);
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
            var cbo = (System.Windows.Forms.ComboBox)sender;

            if (lvwNewNames.SelectedItems.Count > 0)
            {
                mExtract.ChangeAuthorityIdForGroup(mCurrentGroupId, Convert.ToInt32(cbo.SelectedValue));
                lvwNewNames.SelectedItems[0].SubItems[2].Text = cbo.Text;
                lvwNewNames.SelectedItems[0].Tag = Convert.ToInt32(cbo.SelectedValue);
            }
        }

        private class AuthorityContainer
        {
            public AuthorityContainer(string authName, int authId)
            {
                AuthorityId = authId;
                AuthorityName = authName;
            }

            public string AuthorityName { get; private set; }

            public int AuthorityId { get; private set; }
        }

        private class AuthorityContainerComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var auth1 = (AuthorityContainer)x;
                var auth2 = (AuthorityContainer)y;

                var reference1 = auth1.AuthorityName;
                var reference2 = auth2.AuthorityName;

                if (string.Compare(reference1, reference2, StringComparison.Ordinal) > 0)
                {
                    return 1;
                }
                else if (string.Compare(reference1, reference2, StringComparison.Ordinal) < 0)
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
