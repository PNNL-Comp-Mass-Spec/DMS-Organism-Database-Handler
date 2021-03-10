using System;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public partial class frmNewCollectionMetadataEditor
    {
        public frmNewCollectionMetadataEditor()
        {
            InitializeComponent();
            txtDescription.Name = "txtDescription";
            txtSource.Name = "txtSource";
            cmdCancel.Name = "cmdCancel";
            cmdOk.Name = "cmdOk";
        }

        public string Description
        {
            get
            {
                return txtDescription.Text;
            }

            set
            {
                txtDescription.Text = value;
            }
        }

        public string Source
        {
            get
            {
                return txtSource.Text;
            }

            set
            {
                txtSource.Text = value;
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                txtDescription.SelectAll();
            }
        }

        private void txtSource_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                txtSource.SelectAll();
            }
        }
    }
}