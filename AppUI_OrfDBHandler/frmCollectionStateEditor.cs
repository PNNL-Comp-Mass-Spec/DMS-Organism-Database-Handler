using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public class frmCollectionStateEditor : Form
    {
        #region "Windows Form Designer generated code"

        public frmCollectionStateEditor(string ProteinStorageConnectionString) : base()
        {
            SearchTimer = new System.Timers.Timer(2000d);
            SearchTimer.Elapsed += TimerHandler;

            base.Load += frmCollectionStateEditor_Load;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call
            m_PSConnectionString = ProteinStorageConnectionString;
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
        internal ListView lvwCollections;
        internal Label lblCollectionsListView;
        internal ColumnHeader colCollectionName;
        internal ColumnHeader colOrganism;
        internal ColumnHeader colDateAdded;
        internal ColumnHeader colCurrState;
        internal TextBox txtLiveSearch;
        internal PictureBox pbxLiveSearchBkg;
        internal Label lblStateChanger;
        internal ComboBox cboStateChanger;
        internal Button cmdStateChanger;
        internal MainMenu MainMenu1;
        internal MenuItem mnuTools;
        internal MenuItem mnuToolsDeleteSelected;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCollectionStateEditor));
            lvwCollections = new ListView();
            lvwCollections.ColumnClick += new ColumnClickEventHandler(lvwSearchResults_ColumnClick);
            colCollectionName = new ColumnHeader();
            colOrganism = new ColumnHeader();
            colDateAdded = new ColumnHeader();
            colCurrState = new ColumnHeader();
            lblCollectionsListView = new Label();
            txtLiveSearch = new TextBox();
            txtLiveSearch.TextChanged += new EventHandler(txtLiveSearch_TextChanged);
            txtLiveSearch.Click += new EventHandler(txtLiveSearch_Click);
            txtLiveSearch.Leave += new EventHandler(txtLiveSearch_Leave);
            pbxLiveSearchBkg = new PictureBox();
            lblStateChanger = new Label();
            cboStateChanger = new ComboBox();
            cboStateChanger.SelectedIndexChanged += new EventHandler(cboStateChanger_SelectedIndexChanged);
            cmdStateChanger = new Button();
            cmdStateChanger.Click += new EventHandler(cmdStateChanger_Click);
            MainMenu1 = new MainMenu(components);
            mnuTools = new MenuItem();
            mnuToolsDeleteSelected = new MenuItem();
            ((System.ComponentModel.ISupportInitialize)pbxLiveSearchBkg).BeginInit();
            SuspendLayout();
            //
            // lvwCollections
            //
            lvwCollections.Columns.AddRange(new ColumnHeader[] { colCollectionName, colOrganism, colDateAdded, colCurrState });
            lvwCollections.Dock = DockStyle.Fill;
            lvwCollections.FullRowSelect = true;
            lvwCollections.GridLines = true;
            lvwCollections.Location = new Point(10, 30);
            lvwCollections.Name = "lvwCollections";
            lvwCollections.Size = new Size(594, 660);
            lvwCollections.TabIndex = 0;
            lvwCollections.UseCompatibleStateImageBehavior = false;
            lvwCollections.View = View.Details;
            //
            // colCollectionName
            //
            colCollectionName.Text = "Collection Name";
            colCollectionName.Width = 278;
            //
            // colOrganism
            //
            colOrganism.Text = "Organism";
            colOrganism.Width = 110;
            //
            // colDateAdded
            //
            colDateAdded.Text = "Date Added";
            colDateAdded.Width = 100;
            //
            // colCurrState
            //
            colCurrState.Text = "State";
            colCurrState.Width = 80;
            //
            // lblCollectionsListView
            //
            lblCollectionsListView.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            lblCollectionsListView.Location = new Point(14, 17);
            lblCollectionsListView.Name = "lblCollectionsListView";
            lblCollectionsListView.Size = new Size(826, 24);
            lblCollectionsListView.TabIndex = 1;
            lblCollectionsListView.Text = "Available Collections";
            //
            // txtLiveSearch
            //
            txtLiveSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            txtLiveSearch.BorderStyle = BorderStyle.None;
            txtLiveSearch.ForeColor = SystemColors.InactiveCaption;
            txtLiveSearch.Location = new Point(48, 696);
            txtLiveSearch.Name = "txtLiveSearch";
            txtLiveSearch.Size = new Size(215, 17);
            txtLiveSearch.TabIndex = 17;
            txtLiveSearch.Text = "Search";
            //
            // pbxLiveSearchBkg
            //
            pbxLiveSearchBkg.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pbxLiveSearchBkg.Image = (Image)resources.GetObject("pbxLiveSearchBkg.Image");
            pbxLiveSearchBkg.Location = new Point(17, 689);
            pbxLiveSearchBkg.Name = "pbxLiveSearchBkg";
            pbxLiveSearchBkg.Size = new Size(280, 29);
            pbxLiveSearchBkg.TabIndex = 18;
            pbxLiveSearchBkg.TabStop = false;
            //
            // lblStateChanger
            //
            lblStateChanger.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblStateChanger.Location = new Point(305, 696);
            lblStateChanger.Name = "lblStateChanger";
            lblStateChanger.Size = new Size(255, 17);
            lblStateChanger.TabIndex = 19;
            lblStateChanger.Text = "Change Selected Collections To...";
            //
            // cboStateChanger
            //
            cboStateChanger.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cboStateChanger.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStateChanger.Location = new Point(560, 691);
            cboStateChanger.Name = "cboStateChanger";
            cboStateChanger.Size = new Size(0, 25);
            cboStateChanger.TabIndex = 20;
            //
            // cmdStateChanger
            //
            cmdStateChanger.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdStateChanger.Location = new Point(504, 691);
            cmdStateChanger.Name = "cmdStateChanger";
            cmdStateChanger.Size = new Size(96, 24);
            cmdStateChanger.TabIndex = 21;
            cmdStateChanger.Text = "Change";
            //
            // MainMenu1
            //
            MainMenu1.MenuItems.AddRange(new MenuItem[] { mnuTools });
            //
            // mnuTools
            //
            mnuTools.Index = 0;
            mnuTools.MenuItems.AddRange(new MenuItem[] { mnuToolsDeleteSelected });
            mnuTools.Text = "Tools";
            mnuTools.Visible = false;
            //
            // mnuToolsDeleteSelected
            //
            mnuToolsDeleteSelected.Index = 0;
            mnuToolsDeleteSelected.Text = "Delete Selected Collections...";
            //
            // frmCollectionStateEditor
            //
            AutoScaleBaseSize = new Size(7, 17);
            ClientSize = new Size(614, 740);
            Controls.Add(cmdStateChanger);
            Controls.Add(cboStateChanger);
            Controls.Add(lblStateChanger);
            Controls.Add(txtLiveSearch);
            Controls.Add(pbxLiveSearchBkg);
            Controls.Add(lvwCollections);
            Controls.Add(lblCollectionsListView);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            Menu = MainMenu1;
            Name = "frmCollectionStateEditor";
            Padding = new Padding(10, 30, 10, 50);
            Text = "Collection State Editor";
            ((System.ComponentModel.ISupportInitialize)pbxLiveSearchBkg).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

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