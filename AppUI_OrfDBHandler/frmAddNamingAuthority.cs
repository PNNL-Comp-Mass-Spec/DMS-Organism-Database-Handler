using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public partial class frmAddNamingAuthority : Form
    {
        public frmAddNamingAuthority()
        {
            base.Load += frmAddNamingAuthority_Load;

            InitializeComponent();
        }

        private string m_ShortName;
        private string m_FullName;
        private string m_WebAddress;

        #region "Return Properties"

        public string ShortName
        {
            get => m_ShortName;
            set => m_ShortName = value;
        }

        public string FullName
        {
            get => m_FullName;
            set => m_FullName = value;
        }

        public string WebAddress
        {
            get => m_WebAddress;
            set => m_WebAddress = value;
        }

        #endregion

        private void frmAddNamingAuthority_Load(object sender, EventArgs e)
        {
            if (m_ShortName != null)
            {
                txtAuthName.Text = m_ShortName;
            }

            if (m_FullName != null)
            {
                txtAuthFullName.Text = m_FullName;
            }

            if (m_WebAddress != null)
            {
                txtAuthWeb.Text = m_WebAddress;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            m_ShortName = txtAuthName.Text;
            m_FullName = txtAuthFullName.Text;
            m_WebAddress = txtAuthWeb.Text;

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
            var newAddressSB = new StringBuilder();

            if (r1.IsMatch(rawAddress))
            {
                var m = r1.Match(rawAddress);
                if (m.Groups[1].Value.Length == 0)
                {
                    newAddressSB.Append("http://");
                    newAddressSB.Append(m.Groups[3].Value);
                }
                else
                {
                    newAddressSB.Append(m.Groups[1].Value);
                    newAddressSB.Append("://");
                    newAddressSB.Append(m.Groups[2].Value);
                    newAddressSB.Append(m.Groups[3].Value);
                }

                newAddressSB.Append(m.Groups[4].Value);

                var newAddress = newAddressSB.ToString();

                return newAddress;
            }

            return rawAddress;
        }

        private void txtAuthWeb_Validating(object sender, CancelEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            string tmpAddress = ValidateWebAddressFormat(txt.Text);
            txt.Text = tmpAddress;
        }
    }
}
