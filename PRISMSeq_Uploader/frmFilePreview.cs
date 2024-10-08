﻿using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PRISMSeq_Uploader
{
    public partial class frmFilePreview : Form
    {
        // Ignore Spelling: frm

        public frmFilePreview()
        {
            base.Load += frmFilePreview_Load;

            InitializeComponent();

            validationRegex = new Regex(@"^(\d+)$");
        }

        public event RefreshRequestEventHandler RefreshRequest;

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

            if (cmdRefresh.Enabled)
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

                if (!cmdRefresh.Enabled)
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
            cmdRefresh.Enabled = validationRegex.IsMatch(countText);
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
