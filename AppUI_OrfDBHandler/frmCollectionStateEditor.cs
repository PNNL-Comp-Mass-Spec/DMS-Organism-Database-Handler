using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using PRISM;

namespace AppUI_OrfDBHandler
{
    public partial class frmCollectionStateEditor : Form
    {
        public frmCollectionStateEditor(string proteinStorageConnectionString)
        {
            searchTimer = new System.Timers.Timer(2000d);
            searchTimer.Elapsed += TimerHandler;

            Load += frmCollectionStateEditor_Load;

            InitializeComponent();

            mPsConnectionString = proteinStorageConnectionString;
        }

        private readonly System.Timers.Timer searchTimer;

        private bool mSearchActive;
        private CollectionStatePickerHandler mHandler;
        private readonly string mPsConnectionString;
        private DataTable mStatesTable;
        private int mSelectedNewStateId = 1;
        private bool mSortOrderAsc = true;
        private int mSelectedCol;

        #region "Live Search Handler"

        private void txtLiveSearch_TextChanged(object sender, EventArgs e)
        {
            if (mSearchActive)
            {
                searchTimer.Start();
            }
        }

        private void txtLiveSearch_Click(object sender, EventArgs e)
        {
            if (mSearchActive)
            {
                return;
            }

            txtLiveSearch.Text = null;
            txtLiveSearch.ForeColor = SystemColors.ControlText;
            mSearchActive = true;
        }

        private void txtLiveSearch_Leave(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length == 0)
            {
                txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
                txtLiveSearch.Text = "Search";
                mSearchActive = false;
                searchTimer.Stop();
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
            mHandler = new CollectionStatePickerHandler(mPsConnectionString);
            RegisterEvents(mHandler);

            mStatesTable = mHandler.GetStates();

            cboStateChanger.SelectedIndexChanged -= cboStateChanger_SelectedIndexChanged;

            cboStateChanger.BeginUpdate();
            cboStateChanger.DataSource = mStatesTable;
            cboStateChanger.DisplayMember = "State";
            cboStateChanger.ValueMember = "ID";
            cboStateChanger.EndUpdate();

            cboStateChanger.SelectedIndexChanged += cboStateChanger_SelectedIndexChanged;

            cboStateChanger.SelectedValue = mSelectedNewStateId;

            mHandler.FillListView(lvwCollections);
        }

        private void cboStateChanger_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = (ComboBox)sender;
            mSelectedNewStateId = Convert.ToInt32(cbo.SelectedValue);
        }

        private void cmdStateChanger_Click(object sender, EventArgs e)
        {
            var al = new ArrayList();

            foreach (ListViewItem item in lvwCollections.SelectedItems)
            {
                al.Add(item.Tag);
            }

            mHandler.ChangeSelectedCollectionStates(mSelectedNewStateId, al);

            mHandler.ForceIdTableReload = true;

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
            private readonly bool mSortAscending;

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
                var item1 = x as ListViewItem;
                var item2 = y as ListViewItem;

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
                    compareResult = string.CompareOrdinal(item1.SubItems[colIndex].Text, item2.SubItems[colIndex].Text);
                }

                if (mSortAscending)
                {
                    return compareResult;
                }

                return -compareResult;
            }
        }

        /// <summary>
        /// Use this method to chain events between classes
        /// </summary>
        /// <param name="sourceClass"></param>
        protected void RegisterEvents(EventNotifier sourceClass)
        {
            // sourceClass.DebugEvent += OnDebugEvent;
            // sourceClass.StatusEvent += OnStatusEvent;
            sourceClass.ErrorEvent += OnErrorEvent;
            sourceClass.WarningEvent += OnWarningEvent;
            // sourceClass.ProgressUpdate += OnProgressUpdate;
        }

        private void OnWarningEvent(string message)
        {
            MessageBox.Show("Warning: " + message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OnErrorEvent(string message, Exception ex)
        {
            MessageBox.Show("Error: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
