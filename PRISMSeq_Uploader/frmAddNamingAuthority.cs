using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PRISMSeq_Uploader
{
    public partial class frmAddNamingAuthority : Form
    {
        // Ignore Spelling: frm

        public frmAddNamingAuthority()
        {
            base.Load += frmAddNamingAuthority_Load;

            InitializeComponent();
        }

        public string ShortName { get; set; }

        public string FullName { get; set; }

        public string WebAddress { get; set; }

        private void frmAddNamingAuthority_Load(object sender, EventArgs e)
        {
            if (ShortName != null)
            {
                txtAuthName.Text = ShortName;
            }

            if (FullName != null)
            {
                txtAuthFullName.Text = FullName;
            }

            if (WebAddress != null)
            {
                txtAuthWeb.Text = WebAddress;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            ShortName = txtAuthName.Text;
            FullName = txtAuthFullName.Text;
            WebAddress = txtAuthWeb.Text;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public string ValidateWebAddressFormat(string rawAddress)
        {
            var r1 = new Regex(@"(?:([^:/?#]+):)?(?://([^/?#]*))?([^?#]*)(?:\?([^#]*))?");  // Match with specific page noted
            var newAddressSb = new StringBuilder();

            if (r1.IsMatch(rawAddress))
            {
                var m = r1.Match(rawAddress);

                if (m.Groups[1].Value.Length == 0)
                {
                    newAddressSb.Append("http://");
                    newAddressSb.Append(m.Groups[3].Value);
                }
                else
                {
                    newAddressSb.Append(m.Groups[1].Value);
                    newAddressSb.Append("://");
                    newAddressSb.Append(m.Groups[2].Value);
                    newAddressSb.Append(m.Groups[3].Value);
                }

                newAddressSb.Append(m.Groups[4].Value);

                return newAddressSb.ToString();
            }

            return rawAddress;
        }

        private void txtAuthWeb_Validating(object sender, CancelEventArgs e)
        {
            var targetTextBox = (TextBox)sender;
            targetTextBox.Text = ValidateWebAddressFormat(targetTextBox.Text);
        }
    }
}
