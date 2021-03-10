using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace AppUI_OrfDBHandler
{
    public class frmAddNamingAuthority : Form
    {
        #region "Windows Form Designer generated code"

        public frmAddNamingAuthority() : base()
        {
            base.Load += frmAddNamingAuthority_Load;

            // This call is required by the Windows Form Designer.
            InitializeComponent();

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
        internal Label lblAuthShortName;
        internal TextBox txtAuthName;
        internal TextBox txtAuthWeb;
        internal Label lblAuthWeb;
        internal Button cmdCancel;
        internal Button cmdOK;
        internal TextBox txtAuthFullName;
        internal Label lblAuthFullName;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            lblAuthShortName = new Label();
            txtAuthName = new TextBox();
            txtAuthFullName = new TextBox();
            lblAuthFullName = new Label();
            txtAuthWeb = new TextBox();
            txtAuthWeb.Validating += new CancelEventHandler(txtAuthWeb_Validating);
            lblAuthWeb = new Label();
            cmdCancel = new Button();
            cmdCancel.Click += new EventHandler(cmdCancel_Click);
            cmdOK = new Button();
            cmdOK.Click += new EventHandler(cmdOK_Click);
            SuspendLayout();
            //
            // lblAuthShortName
            //
            lblAuthShortName.Location = new Point(6, 8);
            lblAuthShortName.Name = "lblAuthShortName";
            lblAuthShortName.Size = new Size(266, 16);
            lblAuthShortName.TabIndex = 0;
            lblAuthShortName.Text = "Authority Short Name (64 char max)";
            //
            // txtAuthName
            //
            txtAuthName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAuthName.Location = new Point(6, 24);
            txtAuthName.MaxLength = 64;
            txtAuthName.Name = "txtAuthName";
            txtAuthName.Size = new Size(278, 21);
            txtAuthName.TabIndex = 1;
            txtAuthName.Text = "";
            //
            // txtAuthFullName
            //
            txtAuthFullName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAuthFullName.Location = new Point(7, 66);
            txtAuthFullName.MaxLength = 128;
            txtAuthFullName.Name = "txtAuthFullName";
            txtAuthFullName.Size = new Size(278, 21);
            txtAuthFullName.TabIndex = 3;
            txtAuthFullName.Text = "";
            //
            // lblAuthFullName
            //
            lblAuthFullName.Location = new Point(7, 50);
            lblAuthFullName.Name = "lblAuthFullName";
            lblAuthFullName.Size = new Size(265, 16);
            lblAuthFullName.TabIndex = 2;
            lblAuthFullName.Text = "Authority Full Name (128 char max)";
            //
            // txtAuthWeb
            //
            txtAuthWeb.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAuthWeb.Location = new Point(7, 108);
            txtAuthWeb.MaxLength = 128;
            txtAuthWeb.Name = "txtAuthWeb";
            txtAuthWeb.Size = new Size(278, 21);
            txtAuthWeb.TabIndex = 5;
            txtAuthWeb.Text = "";
            //
            // lblAuthWeb
            //
            lblAuthWeb.Location = new Point(7, 92);
            lblAuthWeb.Name = "lblAuthWeb";
            lblAuthWeb.Size = new Size(265, 16);
            lblAuthWeb.TabIndex = 4;
            lblAuthWeb.Text = "Web Address (optional, 128 char max)";
            //
            // cmdCancel
            //
            cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdCancel.DialogResult = DialogResult.Cancel;
            cmdCancel.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdCancel.Location = new Point(210, 140);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.TabIndex = 6;
            cmdCancel.Text = "Cancel";
            //
            // cmdOK
            //
            cmdOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdOK.DialogResult = DialogResult.OK;
            cmdOK.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdOK.Location = new Point(126, 140);
            cmdOK.Name = "cmdOK";
            cmdOK.TabIndex = 7;
            cmdOK.Text = "OK";
            //
            // frmAddNamingAuthority
            //
            AcceptButton = cmdOK;
            AutoScaleBaseSize = new Size(5, 14);
            CancelButton = cmdCancel;
            ClientSize = new Size(292, 172);
            Controls.Add(cmdOK);
            Controls.Add(cmdCancel);
            Controls.Add(txtAuthWeb);
            Controls.Add(txtAuthFullName);
            Controls.Add(txtAuthName);
            Controls.Add(lblAuthWeb);
            Controls.Add(lblAuthFullName);
            Controls.Add(lblAuthShortName);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximumSize = new Size(298, 196);
            MinimumSize = new Size(298, 196);
            Name = "frmAddNamingAuthority";
            StartPosition = FormStartPosition.Manual;
            Text = "Add Naming Authority";
            ResumeLayout(false);
        }

        #endregion

        private string m_ShortName;
        private string m_FullName;
        private string m_WebAddress;

        #region "Return Properties"

        public string ShortName
        {
            get
            {
                return m_ShortName;
            }

            set
            {
                m_ShortName = value;
            }
        }

        public string FullName
        {
            get
            {
                return m_FullName;
            }

            set
            {
                m_FullName = value;
            }
        }

        public string WebAddress
        {
            get
            {
                return m_WebAddress;
            }

            set
            {
                m_WebAddress = value;
            }
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
            Match m;
            var r1 = new Regex(@"(?:([^:/?#]+):)?(?://([^/?#]*))?([^?#]*)(?:\?([^#]*))?");  // Match with specific page noted
            string newAddress;
            var newAddressSB = new StringBuilder();

            if (r1.IsMatch(rawAddress))
            {
                m = r1.Match(rawAddress);
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

                newAddress = newAddressSB.ToString();

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