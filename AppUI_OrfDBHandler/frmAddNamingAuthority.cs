﻿using System;
using System.ComponentModel;
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

        private string mShortName;
        private string mFullName;
        private string mWebAddress;

        #region "Return Properties"

        public string ShortName
        {
            get => mShortName;
            set => mShortName = value;
        }

        public string FullName
        {
            get => mFullName;
            set => mFullName = value;
        }

        public string WebAddress
        {
            get => mWebAddress;
            set => mWebAddress = value;
        }

        #endregion

        private void frmAddNamingAuthority_Load(object sender, EventArgs e)
        {
            if (mShortName != null)
            {
                txtAuthName.Text = mShortName;
            }

            if (mFullName != null)
            {
                txtAuthFullName.Text = mFullName;
            }

            if (mWebAddress != null)
            {
                txtAuthWeb.Text = mWebAddress;
            }
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            mShortName = txtAuthName.Text;
            mFullName = txtAuthFullName.Text;
            mWebAddress = txtAuthWeb.Text;

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

                var newAddress = newAddressSb.ToString();

                return newAddress;
            }

            return rawAddress;
        }

        private void txtAuthWeb_Validating(object sender, CancelEventArgs e)
        {
            var txt = (TextBox)sender;
            var tmpAddress = ValidateWebAddressFormat(txt.Text);
            txt.Text = tmpAddress;
        }
    }
}
