using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public partial class frmFilePreview : Form
    {
        public frmFilePreview()
        {
            base.Load += frmFilePreview_Load;
            base.Closed += frmFilePreview_Closed;

            InitializeComponent();

            validationRegex = new Regex(@"^(\d+)$");
        }

        public event RefreshRequestEventHandler RefreshRequest;

        public delegate void RefreshRequestEventHandler(int lineCount);

        public new event FormClosingEventHandler FormClosing;

        public delegate void FormClosingEventHandler();

        private readonly Regex validationRegex;
        private int mCurrentLineCount = 100;

        public string WindowName
        {
            set => Text = value;
        }

        public bool FormVisibility => Visible;

        public int GetLineCount()
        {
            return Convert.ToInt32(txtLineCount.Text);
        }

        public void SetPreviewItems(ListViewItem[] newContents)
        {
            lvwPreview.BeginUpdate();
            lvwPreview.Items.Clear();

            lvwPreview.Items.AddRange(newContents);

            lvwPreview.EndUpdate();
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            RefreshRequest?.Invoke(mCurrentLineCount);
            if (cmdRefresh.Enabled == true)
            {
                cmdRefresh.Enabled = false;
            }
        }

        private void txtLineCount_Validating(object sender, CancelEventArgs e)
        {
            var countText = txtLineCount.Text;

            if (validationRegex.IsMatch(Convert.ToInt32(countText).ToString()))
            {
                var m = validationRegex.Match(Convert.ToInt32(countText).ToString());
                var value = Convert.ToInt32(m.Groups[0].Value);
                txtLineCount.Text = value.ToString();
                mCurrentLineCount = value;
                if (cmdRefresh.Enabled == false)
                {
                    cmdRefresh.Enabled = true;
                }
            }
            else
            {
                txtLineCount.Text = mCurrentLineCount.ToString();
                e.Cancel = true;
            }
        }

        private void frmFilePreview_Load(object sender, EventArgs e)
        {
            txtLineCount.Text = mCurrentLineCount.ToString();
            RefreshRequest?.Invoke(mCurrentLineCount);
        }

        private void txtLineCount_TextChanged(object sender, EventArgs e)
        {
            var countText = txtLineCount.Text;
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
