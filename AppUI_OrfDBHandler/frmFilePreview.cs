using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace AppUI_OrfDBHandler
{
    public class frmFilePreview : Form
    {
        #region "Windows Form Designer generated code"

        public frmFilePreview() : base()
        {
            base.Load += frmFilePreview_Load;
            base.Closed += frmFilePreview_Closed;

            // This call is required by the Windows Form Designer.
            InitializeComponent();
            validationRegex = new Regex(@"^(\d+)$");
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
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        internal Label lblLineCount;
        internal TextBox txtLineCount;
        internal Button cmdRefresh;
        internal Label lblPreviewTitle;
        internal ColumnHeader colName;
        internal ColumnHeader colDescription;
        internal Button cmdClose;
        internal ListView lvwPreview;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            lvwPreview = new ListView();
            colName = new ColumnHeader();
            colDescription = new ColumnHeader();
            txtLineCount = new TextBox();
            txtLineCount.Validating += new CancelEventHandler(txtLineCount_Validating);
            txtLineCount.TextChanged += new EventHandler(txtLineCount_TextChanged);
            cmdRefresh = new Button();
            cmdRefresh.Click += new EventHandler(cmdRefresh_Click);
            lblLineCount = new Label();
            lblPreviewTitle = new Label();
            cmdClose = new Button();
            cmdClose.Click += new EventHandler(cmdClose_Click);
            SuspendLayout();
            //
            // lvwPreview
            //
            lvwPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lvwPreview.Columns.AddRange(new ColumnHeader[] { colName, colDescription });
            lvwPreview.FullRowSelect = true;
            lvwPreview.GridLines = true;
            lvwPreview.Location = new Point(-3, 58);
            lvwPreview.MultiSelect = false;
            lvwPreview.Name = "lvwPreview";
            lvwPreview.Size = new Size(666, 452);
            lvwPreview.Sorting = SortOrder.Ascending;
            lvwPreview.TabIndex = 0;
            lvwPreview.UseCompatibleStateImageBehavior = false;
            lvwPreview.View = View.Details;
            //
            // colName
            //
            colName.Text = "Protein Name";
            colName.Width = 200;
            //
            // colDescription
            //
            colDescription.Text = "Description Line";
            colDescription.Width = 352;
            //
            // txtLineCount
            //
            txtLineCount.Location = new Point(277, 26);
            txtLineCount.Name = "txtLineCount";
            txtLineCount.Size = new Size(140, 24);
            txtLineCount.TabIndex = 1;
            txtLineCount.TextAlign = HorizontalAlignment.Right;
            //
            // cmdRefresh
            //
            cmdRefresh.Location = new Point(430, 16);
            cmdRefresh.Name = "cmdRefresh";
            cmdRefresh.Size = new Size(106, 36);
            cmdRefresh.TabIndex = 2;
            cmdRefresh.Text = "&Refresh List";
            //
            // lblLineCount
            //
            lblLineCount.Location = new Point(275, 6);
            lblLineCount.Name = "lblLineCount";
            lblLineCount.Size = new Size(137, 20);
            lblLineCount.TabIndex = 3;
            lblLineCount.Text = "# Lines to Preview";
            //
            // lblPreviewTitle
            //
            lblPreviewTitle.Font = new Font("Tahoma", 11.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            lblPreviewTitle.Location = new Point(3, 34);
            lblPreviewTitle.Name = "lblPreviewTitle";
            lblPreviewTitle.Size = new Size(257, 19);
            lblPreviewTitle.TabIndex = 4;
            lblPreviewTitle.Text = "Preview of File Contents";
            //
            // cmdClose
            //
            cmdClose.DialogResult = DialogResult.Cancel;
            cmdClose.Location = new Point(542, 16);
            cmdClose.Name = "cmdClose";
            cmdClose.Size = new Size(106, 36);
            cmdClose.TabIndex = 5;
            cmdClose.Text = "&Close";
            //
            // frmFilePreview
            //
            AutoScaleBaseSize = new Size(7, 17);
            CancelButton = cmdClose;
            ClientSize = new Size(660, 510);
            Controls.Add(cmdClose);
            Controls.Add(lblLineCount);
            Controls.Add(cmdRefresh);
            Controls.Add(txtLineCount);
            Controls.Add(lvwPreview);
            Controls.Add(lblPreviewTitle);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            MinimumSize = new Size(342, 437);
            Name = "frmFilePreview";
            StartPosition = FormStartPosition.Manual;
            Text = "Preview of: ";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        public event RefreshRequestEventHandler RefreshRequest;

        public delegate void RefreshRequestEventHandler(int lineCount);

        public new event FormClosingEventHandler FormClosing;

        public new delegate void FormClosingEventHandler();

        private readonly Regex validationRegex;
        private int m_currentLineCount = 100;

        public string WindowName
        {
            set
            {
                Text = value;
            }
        }

        public bool FormVisibility
        {
            get
            {
                return Visible;
            }
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            RefreshRequest?.Invoke(m_currentLineCount);
            if (cmdRefresh.Enabled == true)
            {
                cmdRefresh.Enabled = false;
            }
        }

        private void txtLineCount_Validating(object sender, CancelEventArgs e)
        {
            int value;
            Match m;
            string countText = txtLineCount.Text;

            if (validationRegex.IsMatch(Conversions.ToInteger(countText).ToString()))
            {
                m = validationRegex.Match(Conversions.ToInteger(countText).ToString());
                value = Conversions.ToInteger(m.Groups[0].Value);
                txtLineCount.Text = value.ToString();
                m_currentLineCount = value;
                if (cmdRefresh.Enabled == false)
                {
                    cmdRefresh.Enabled = true;
                }
            }
            else
            {
                txtLineCount.Text = m_currentLineCount.ToString();
                e.Cancel = true;
            }
        }

        private void frmFilePreview_Load(object sender, EventArgs e)
        {
            txtLineCount.Text = m_currentLineCount.ToString();
            RefreshRequest?.Invoke(m_currentLineCount);
        }

        private void txtLineCount_TextChanged(object sender, EventArgs e)
        {
            string countText = txtLineCount.Text;
            if (validationRegex.IsMatch(countText))
            {
                cmdRefresh.Enabled = true;
            }
            else
            {
                cmdRefresh.Enabled = false;
            }
        }

        private void frmFilePreview_Closed(object sender, EventArgs e)
        {
            FormClosing?.Invoke();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}