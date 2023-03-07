namespace AppUI_OrfDBHandler
{
    partial class frmCollectionStateEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCollectionStateEditor));
            this.lvwCollections = new System.Windows.Forms.ListView();
            this.colCollectionName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOrganism = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDateAdded = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCurrState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblCollectionsListView = new System.Windows.Forms.Label();
            this.txtLiveSearch = new System.Windows.Forms.TextBox();
            this.pbxLiveSearchBkg = new System.Windows.Forms.PictureBox();
            this.lblStateChanger = new System.Windows.Forms.Label();
            this.cboStateChanger = new System.Windows.Forms.ComboBox();
            this.cmdStateChanger = new System.Windows.Forms.Button();
            this.MainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.mnuTools = new System.Windows.Forms.MenuItem();
            this.mnuToolsDeleteSelected = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLiveSearchBkg)).BeginInit();
            this.SuspendLayout();
            // 
            // lvwCollections
            // 
            this.lvwCollections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCollectionName,
            this.colOrganism,
            this.colDateAdded,
            this.colCurrState});
            this.lvwCollections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwCollections.FullRowSelect = true;
            this.lvwCollections.GridLines = true;
            this.lvwCollections.HideSelection = false;
            this.lvwCollections.Location = new System.Drawing.Point(10, 30);
            this.lvwCollections.Name = "lvwCollections";
            this.lvwCollections.Size = new System.Drawing.Size(594, 660);
            this.lvwCollections.TabIndex = 0;
            this.lvwCollections.UseCompatibleStateImageBehavior = false;
            this.lvwCollections.View = System.Windows.Forms.View.Details;
            this.lvwCollections.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwSearchResults_ColumnClick);
            // 
            // colCollectionName
            // 
            this.colCollectionName.Text = "Collection Name";
            this.colCollectionName.Width = 278;
            // 
            // colOrganism
            // 
            this.colOrganism.Text = "Organism";
            this.colOrganism.Width = 110;
            // 
            // colDateAdded
            // 
            this.colDateAdded.Text = "Date Added";
            this.colDateAdded.Width = 100;
            // 
            // colCurrState
            // 
            this.colCurrState.Text = "State";
            this.colCurrState.Width = 80;
            // 
            // lblCollectionsListView
            // 
            this.lblCollectionsListView.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCollectionsListView.Location = new System.Drawing.Point(14, 17);
            this.lblCollectionsListView.Name = "lblCollectionsListView";
            this.lblCollectionsListView.Size = new System.Drawing.Size(826, 24);
            this.lblCollectionsListView.TabIndex = 1;
            this.lblCollectionsListView.Text = "Available Collections";
            // 
            // txtLiveSearch
            // 
            this.txtLiveSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLiveSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLiveSearch.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.txtLiveSearch.Location = new System.Drawing.Point(48, 696);
            this.txtLiveSearch.Name = "txtLiveSearch";
            this.txtLiveSearch.Size = new System.Drawing.Size(215, 14);
            this.txtLiveSearch.TabIndex = 17;
            this.txtLiveSearch.Text = "Search";
            this.txtLiveSearch.Click += new System.EventHandler(this.txtLiveSearch_Click);
            this.txtLiveSearch.TextChanged += new System.EventHandler(this.txtLiveSearch_TextChanged);
            this.txtLiveSearch.Leave += new System.EventHandler(this.txtLiveSearch_Leave);
            // 
            // pbxLiveSearchBkg
            // 
            this.pbxLiveSearchBkg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbxLiveSearchBkg.Image = ((System.Drawing.Image)(resources.GetObject("pbxLiveSearchBkg.Image")));
            this.pbxLiveSearchBkg.Location = new System.Drawing.Point(17, 689);
            this.pbxLiveSearchBkg.Name = "pbxLiveSearchBkg";
            this.pbxLiveSearchBkg.Size = new System.Drawing.Size(280, 29);
            this.pbxLiveSearchBkg.TabIndex = 18;
            this.pbxLiveSearchBkg.TabStop = false;
            // 
            // lblStateChanger
            // 
            this.lblStateChanger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStateChanger.Location = new System.Drawing.Point(305, 696);
            this.lblStateChanger.Name = "lblStateChanger";
            this.lblStateChanger.Size = new System.Drawing.Size(255, 17);
            this.lblStateChanger.TabIndex = 19;
            this.lblStateChanger.Text = "Change Selected Collections To...";
            // 
            // cboStateChanger
            // 
            this.cboStateChanger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStateChanger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStateChanger.Location = new System.Drawing.Point(560, 691);
            this.cboStateChanger.Name = "cboStateChanger";
            this.cboStateChanger.Size = new System.Drawing.Size(0, 21);
            this.cboStateChanger.TabIndex = 20;
            this.cboStateChanger.SelectedIndexChanged += new System.EventHandler(this.cboStateChanger_SelectedIndexChanged);
            // 
            // cmdStateChanger
            // 
            this.cmdStateChanger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStateChanger.Location = new System.Drawing.Point(504, 691);
            this.cmdStateChanger.Name = "cmdStateChanger";
            this.cmdStateChanger.Size = new System.Drawing.Size(96, 24);
            this.cmdStateChanger.TabIndex = 21;
            this.cmdStateChanger.Text = "Change";
            this.cmdStateChanger.Click += new System.EventHandler(this.cmdStateChanger_Click);
            // 
            // MainMenu1
            // 
            this.MainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuTools});
            // 
            // mnuTools
            // 
            this.mnuTools.Index = 0;
            this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuToolsDeleteSelected});
            this.mnuTools.Text = "Tools";
            this.mnuTools.Visible = false;
            // 
            // mnuToolsDeleteSelected
            // 
            this.mnuToolsDeleteSelected.Index = 0;
            this.mnuToolsDeleteSelected.Text = "Delete Selected Collections...";
            // 
            // frmCollectionStateEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 740);
            this.Controls.Add(this.cmdStateChanger);
            this.Controls.Add(this.cboStateChanger);
            this.Controls.Add(this.lblStateChanger);
            this.Controls.Add(this.txtLiveSearch);
            this.Controls.Add(this.pbxLiveSearchBkg);
            this.Controls.Add(this.lvwCollections);
            this.Controls.Add(this.lblCollectionsListView);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Menu = this.MainMenu1;
            this.Name = "frmCollectionStateEditor";
            this.Padding = new System.Windows.Forms.Padding(10, 30, 10, 50);
            this.Text = "Collection State Editor";
            ((System.ComponentModel.ISupportInitialize)(this.pbxLiveSearchBkg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwCollections;
        private System.Windows.Forms.Label lblCollectionsListView;
        private System.Windows.Forms.ColumnHeader colCollectionName;
        private System.Windows.Forms.ColumnHeader colOrganism;
        private System.Windows.Forms.ColumnHeader colDateAdded;
        private System.Windows.Forms.ColumnHeader colCurrState;
        private System.Windows.Forms.TextBox txtLiveSearch;
        private System.Windows.Forms.PictureBox pbxLiveSearchBkg;
        private System.Windows.Forms.Label lblStateChanger;
        private System.Windows.Forms.ComboBox cboStateChanger;
        private System.Windows.Forms.Button cmdStateChanger;
        private System.Windows.Forms.MainMenu MainMenu1;
        private System.Windows.Forms.MenuItem mnuTools;
        private System.Windows.Forms.MenuItem mnuToolsDeleteSelected;
    }
}