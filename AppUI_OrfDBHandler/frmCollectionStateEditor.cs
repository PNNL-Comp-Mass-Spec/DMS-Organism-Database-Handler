using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public partial class frmCollectionStateEditor : Form
    {
        public frmCollectionStateEditor(string ProteinStorageConnectionString)
        {
            SearchTimer = new System.Timers.Timer(2000d);
            SearchTimer.Elapsed += TimerHandler;

            base.Load += frmCollectionStateEditor_Load;

            InitializeComponent();

            mPSConnectionString = ProteinStorageConnectionString;
        }

        private readonly System.Timers.Timer SearchTimer;

        private bool mSearchActive = false;
        private CollectionStatePickerHandler mHandler;
        private readonly string mPSConnectionString;
        private DataTable mStatesTable;
        private int mSelectedNewStateID = 1;
        private bool mSortOrderAsc = true;
        private int mSelectedCol = 0;

        #region "Live Search Handler"

        private void txtLiveSearch_TextChanged(object sender, EventArgs e)
        {
            if (mSearchActive)
            {
                SearchTimer.Start();
            }
        }

        private void txtLiveSearch_Click(object sender, EventArgs e)
        {
            if (mSearchActive)
            {
            }
            else
            {
                txtLiveSearch.Text = null;
                txtLiveSearch.ForeColor = SystemColors.ControlText;
                mSearchActive = true;
            }
        }

        private void txtLiveSearch_Leave(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length == 0)
            {
                txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
                txtLiveSearch.Text = "Search";
                mSearchActive = false;
                SearchTimer.Stop();
                mHandler.FillListView(lvwCollections);
            }
        }

        #endregion

        internal void TimerHandler(object sender, ElapsedEventArgs e)
        {
            mHandler.FillFilteredListView(lvwCollections, txtLiveSearch.Text);
        }

        #region "Form Event Handlers"

        private void frmCollectionStateEditor_Load(object sender, EventArgs e)
        {
            mHandler = new CollectionStatePickerHandler(mPSConnectionString);
            mStatesTable = mHandler.GetStates();

            cboStateChanger.SelectedIndexChanged -= cboStateChanger_SelectedIndexChanged;

            cboStateChanger.BeginUpdate();
            cboStateChanger.DataSource = mStatesTable;
            cboStateChanger.DisplayMember = "State";
            cboStateChanger.ValueMember = "ID";
            cboStateChanger.EndUpdate();

            cboStateChanger.SelectedIndexChanged += cboStateChanger_SelectedIndexChanged;

            cboStateChanger.SelectedValue = mSelectedNewStateID;

            mHandler.FillListView(lvwCollections);
        }

        private void cboStateChanger_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            mSelectedNewStateID = Convert.ToInt32(cbo.SelectedValue);
        }

        private void cmdStateChanger_Click(object sender, EventArgs e)
        {
            var al = new ArrayList();

            foreach (ListViewItem item in lvwCollections.SelectedItems)
                al.Add(item.Tag);

            mHandler.ChangeSelectedCollectionStates(mSelectedNewStateID, al);

            mHandler.ForceIDTableReload = true;

            if (mSearchActive)
            {
                mHandler.FillFilteredListView(lvwCollections, txtLiveSearch.Text);
            }
            else
            {
                mHandler.FillListView(lvwCollections);
            }
        }

        #endregion

        private void lvwSearchResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // If selected column is same as previously selected column, then reverse sort order. Otherwise,
            // sort newly selected column in ascending order

            // Set up ascending/descending criteria
            if (e.Column == mSelectedCol)
            {
                mSortOrderAsc = !mSortOrderAsc;
            }
            else
            {
                mSortOrderAsc = true;
                mSelectedCol = e.Column;
            }

            // Perform sort
            lvwCollections.ListViewItemSorter = new ListViewItemComparer(e.Column, mSortOrderAsc);
        }

        private class ListViewItemComparer : IComparer
        {
            // Implements the manual sorting of items by columns.
            private readonly bool mSortAscending = true;

            private readonly int colIndex;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="columnIndex"></param>
            /// <param name="sortAscending"></param>
            public ListViewItemComparer(int columnIndex, bool sortAscending)
            {
                colIndex = columnIndex;
                mSortAscending = sortAscending;
            }

            public int Compare(object x, object y)
            {
                ListViewItem item1 = x as ListViewItem;
                ListViewItem item2 = y as ListViewItem;

                if (item1 == null && item2 == null)
                {
                    return 0;
                }

                int compareResult;
                if (item1 == null)
                {
                    compareResult = 1;
                }
                else if (item2 == null)
                {
                    compareResult = -1;
                }
                else
                {
                    compareResult = string.Compare(item1.SubItems[colIndex].Text, item2.SubItems[colIndex].Text);
                }

                if (mSortAscending)
                {
                    return compareResult;
                }
                else
                {
                    return -compareResult;
                }
            }
        }
    }
}
