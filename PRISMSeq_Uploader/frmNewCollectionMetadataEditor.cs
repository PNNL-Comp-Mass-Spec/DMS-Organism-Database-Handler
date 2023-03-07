using System;
using System.Windows.Forms;

namespace PRISMSeq_Uploader
{
    public partial class frmNewCollectionMetadataEditor : Form
    {
        public frmNewCollectionMetadataEditor()
        {
            InitializeComponent();
        }

        public string Description
        {
            get => txtDescription.Text;
            set => txtDescription.Text = value;
        }

        public string Source
        {
            get => txtSource.Text;
            set => txtSource.Text = value;
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
