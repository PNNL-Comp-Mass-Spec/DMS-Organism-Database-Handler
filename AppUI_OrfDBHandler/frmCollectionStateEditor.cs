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

            m_PSConnectionString = ProteinStorageConnectionString;
        }

        internal readonly System.Timers.Timer SearchTimer;

        private bool m_SearchActive = false;
        private CollectionStatePickerHandler m_Handler;
        private readonly string m_PSConnectionString;
        private DataTable m_StatesTable;
        private int m_SelectedNewStateID = 1;
        private bool m_SortOrderAsc = true;
        private int m_SelectedCol = 0;

        #region "Live Search Handler"

        private void txtLiveSearch_TextChanged(object sender, EventArgs e)
        {
            if (m_SearchActive)
            {
                SearchTimer.Start();
            }
        }

        private void txtLiveSearch_Click(object sender, EventArgs e)
        {
            if (m_SearchActive)
            {
            }
            else
            {
                txtLiveSearch.Text = null;
                txtLiveSearch.ForeColor = SystemColors.ControlText;
                m_SearchActive = true;
            }
        }

        private void txtLiveSearch_Leave(object sender, EventArgs e)
        {
            if (txtLiveSearch.Text.Length == 0)
            {
                txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
                txtLiveSearch.Text = "Search";
                m_SearchActive = false;
                SearchTimer.Stop();
                m_Handler.FillListView(lvwCollections);
            }
        }

        #endregion

        internal void TimerHandler(object sender, ElapsedEventArgs e)
        {
            m_Handler.FillFilteredListView(lvwCollections, txtLiveSearch.Text);
        }

        #region "Form Event Handlers"

        private void frmCollectionStateEditor_Load(object sender, EventArgs e)
        {
            m_Handler = new CollectionStatePickerHandler(m_PSConnectionString);
            m_StatesTable = m_Handler.GetStates();

            cboStateChanger.SelectedIndexChanged -= cboStateChanger_SelectedIndexChanged;

            cboStateChanger.BeginUpdate();
            cboStateChanger.DataSource = m_StatesTable;
            cboStateChanger.DisplayMember = "State";
            cboStateChanger.ValueMember = "ID";
            cboStateChanger.EndUpdate();

            cboStateChanger.SelectedIndexChanged += cboStateChanger_SelectedIndexChanged;

            cboStateChanger.SelectedValue = m_SelectedNewStateID;

            m_Handler.FillListView(lvwCollections);
        }

        private void cboStateChanger_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            m_SelectedNewStateID = Convert.ToInt32(cbo.SelectedValue);
        }

        private void cmdStateChanger_Click(object sender, EventArgs e)
        {
            var al = new ArrayList();

            foreach (ListViewItem item in lvwCollections.SelectedItems)
                al.Add(item.Tag);

            m_Handler.ChangeSelectedCollectionStates(m_SelectedNewStateID, al);

            m_Handler.ForceIDTableReload = true;

            if (m_SearchActive)
            {
                m_Handler.FillFilteredListView(lvwCollections, txtLiveSearch.Text);
            }
            else
            {
                m_Handler.FillListView(lvwCollections);
            }
        }

        #endregion

        private void lvwSearchResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // If selected column is same as previously selected column, then reverse sort order. Otherwise,
            // sort newly selected column in ascending order

            // Set up ascending/descending criteria
            if (e.Column == m_SelectedCol)
            {
                m_SortOrderAsc = !m_SortOrderAsc;
            }
            else
            {
                m_SortOrderAsc = true;
                m_SelectedCol = e.Column;
            }

            // Perform sort
            lvwCollections.ListViewItemSorter = new ListViewItemComparer(e.Column, m_SortOrderAsc);
        }

        private class ListViewItemComparer : IComparer
        {
            // Implements the manual sorting of items by columns.
            private readonly bool m_SortAscending = true;

            private readonly int colIndex;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="columnIndex"></param>
            /// <param name="sortAscending"></param>
            public ListViewItemComparer(int columnIndex, bool sortAscending)
            {
                colIndex = columnIndex;
                m_SortAscending = sortAscending;
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

                if (m_SortAscending)
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
