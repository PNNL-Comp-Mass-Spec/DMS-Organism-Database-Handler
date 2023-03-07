namespace AppUI_OrfDBHandler
{
    partial class frmFilePreview
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
            this.lvwPreview = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtLineCount = new System.Windows.Forms.TextBox();
            this.cmdRefresh = new System.Windows.Forms.Button();
            this.lblLineCount = new System.Windows.Forms.Label();
            this.lblPreviewTitle = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwPreview
            // 
            this.lvwPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwPreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colDescription});
            this.lvwPreview.FullRowSelect = true;
            this.lvwPreview.GridLines = true;
            this.lvwPreview.HideSelection = false;
            this.lvwPreview.Location = new System.Drawing.Point(-2, 48);
            this.lvwPreview.MultiSelect = false;
            this.lvwPreview.Name = "lvwPreview";
            this.lvwPreview.Size = new System.Drawing.Size(665, 462);
            this.lvwPreview.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwPreview.TabIndex = 0;
            this.lvwPreview.UseCompatibleStateImageBehavior = false;
            this.lvwPreview.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Protein Name";
            this.colName.Width = 200;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description Line";
            this.colDescription.Width = 352;
            // 
            // txtLineCount
            // 
            this.txtLineCount.Location = new System.Drawing.Point(198, 21);
            this.txtLineCount.Name = "txtLineCount";
            this.txtLineCount.Size = new System.Drawing.Size(100, 21);
            this.txtLineCount.TabIndex = 1;
            this.txtLineCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineCount.TextChanged += new System.EventHandler(this.txtLineCount_TextChanged);
            this.txtLineCount.Validating += new System.ComponentModel.CancelEventHandler(this.txtLineCount_Validating);
            // 
            // cmdRefresh
            // 
            this.cmdRefresh.Location = new System.Drawing.Point(307, 13);
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.Size = new System.Drawing.Size(76, 30);
            this.cmdRefresh.TabIndex = 2;
            this.cmdRefresh.Text = "&Refresh List";
            this.cmdRefresh.Click += new System.EventHandler(this.cmdRefresh_Click);
            // 
            // lblLineCount
            // 
            this.lblLineCount.Location = new System.Drawing.Point(196, 5);
            this.lblLineCount.Name = "lblLineCount";
            this.lblLineCount.Size = new System.Drawing.Size(98, 16);
            this.lblLineCount.TabIndex = 3;
            this.lblLineCount.Text = "# Lines to Preview";
            // 
            // lblPreviewTitle
            // 
            this.lblPreviewTitle.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreviewTitle.Location = new System.Drawing.Point(2, 28);
            this.lblPreviewTitle.Name = "lblPreviewTitle";
            this.lblPreviewTitle.Size = new System.Drawing.Size(184, 16);
            this.lblPreviewTitle.TabIndex = 4;
            this.lblPreviewTitle.Text = "Preview of File Contents";
            // 
            // cmdClose
            // 
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Location = new System.Drawing.Point(387, 13);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(76, 30);
            this.cmdClose.TabIndex = 5;
            this.cmdClose.Text = "&Close";
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // frmFilePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdClose;
            this.ClientSize = new System.Drawing.Size(660, 510);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lblLineCount);
            this.Controls.Add(this.cmdRefresh);
            this.Controls.Add(this.txtLineCount);
            this.Controls.Add(this.lvwPreview);
            this.Controls.Add(this.lblPreviewTitle);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(244, 360);
            this.Name = "frmFilePreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Preview of: ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLineCount;
        private System.Windows.Forms.TextBox txtLineCount;
        private System.Windows.Forms.Button cmdRefresh;
        private System.Windows.Forms.Label lblPreviewTitle;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ListView lvwPreview;
    }
}