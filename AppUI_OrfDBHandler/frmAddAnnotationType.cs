using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public class frmAddAnnotationType : Form
    {
        #region "Windows Form Designer generated code"

        public frmAddAnnotationType() : base()
        {
            base.Load += frmAddAnnotationType_Load;

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
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        internal Button cmdCancel;
        internal Button cmdOK;
        internal Label lblAnnTypeName;
        internal TextBox txtAnnTypeName;
        internal TextBox txtDescription;
        internal Label lblDescription;
        internal TextBox txtTypeExample;
        internal Label lblTypeExample;
        internal Label lblAuthority;
        internal ComboBox cboAuthorityName;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            lblAnnTypeName = new Label();
            txtAnnTypeName = new TextBox();
            txtDescription = new TextBox();
            lblDescription = new Label();
            txtTypeExample = new TextBox();
            lblTypeExample = new Label();
            cmdCancel = new Button();
            cmdCancel.Click += new EventHandler(cmdCancel_Click);
            cmdOK = new Button();
            cmdOK.Click += new EventHandler(cmdOK_Click);
            lblAuthority = new Label();
            cboAuthorityName = new ComboBox();
            cboAuthorityName.SelectedIndexChanged += new EventHandler(cboAuthorityName_SelectedIndexChanged);
            //cboAuthorityName.Validating += txtAuthWeb_Validating;
            SuspendLayout();
            //
            // lblAnnTypeName
            //
            lblAnnTypeName.Location = new Point(6, 8);
            lblAnnTypeName.Name = "lblAnnTypeName";
            lblAnnTypeName.Size = new Size(266, 16);
            lblAnnTypeName.TabIndex = 0;
            lblAnnTypeName.Text = "Annotation Type Name (64 char max)";
            //
            // txtAnnTypeName
            //
            txtAnnTypeName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtAnnTypeName.Location = new Point(6, 24);
            txtAnnTypeName.MaxLength = 64;
            txtAnnTypeName.Name = "txtAnnTypeName";
            txtAnnTypeName.Size = new Size(276, 21);
            txtAnnTypeName.TabIndex = 1;
            txtAnnTypeName.Text = "";
            //
            // txtDescription
            //
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.Location = new Point(7, 66);
            txtDescription.MaxLength = 128;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(276, 21);
            txtDescription.TabIndex = 3;
            txtDescription.Text = "";
            //
            // lblDescription
            //
            lblDescription.Location = new Point(7, 50);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(265, 16);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Annotation Type Description (128 char max)";
            //
            // txtTypeExample
            //
            txtTypeExample.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTypeExample.Location = new Point(7, 108);
            txtTypeExample.MaxLength = 128;
            txtTypeExample.Name = "txtTypeExample";
            txtTypeExample.Size = new Size(276, 21);
            txtTypeExample.TabIndex = 5;
            txtTypeExample.Text = "";
            //
            // lblTypeExample
            //
            lblTypeExample.Location = new Point(7, 92);
            lblTypeExample.Name = "lblTypeExample";
            lblTypeExample.Size = new Size(265, 16);
            lblTypeExample.TabIndex = 4;
            lblTypeExample.Text = "Example of Annotation (optional, 128 char max)";
            //
            // cmdCancel
            //
            cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdCancel.DialogResult = DialogResult.Cancel;
            cmdCancel.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            cmdCancel.Location = new Point(208, 182);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.TabIndex = 6;
            cmdCancel.Text = "Cancel";
            //
            // cmdOK
            //
            cmdOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdOK.DialogResult = DialogResult.OK;
            cmdOK.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            cmdOK.Location = new Point(124, 182);
            cmdOK.Name = "cmdOK";
            cmdOK.TabIndex = 7;
            cmdOK.Text = "OK";
            //
            // lblAuthority
            //
            lblAuthority.Location = new Point(7, 134);
            lblAuthority.Name = "lblAuthority";
            lblAuthority.Size = new Size(265, 16);
            lblAuthority.TabIndex = 8;
            lblAuthority.Text = "Naming Authority";
            //
            // cboAuthorityName
            //
            cboAuthorityName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboAuthorityName.Location = new Point(7, 150);
            cboAuthorityName.Name = "cboAuthorityName";
            cboAuthorityName.Size = new Size(276, 21);
            cboAuthorityName.TabIndex = 9;
            //
            // frmAddAnnotationType
            //
            AcceptButton = cmdOK;
            AutoScaleBaseSize = new Size(5, 14);
            CancelButton = cmdCancel;
            ClientSize = new Size(292, 216);
            Controls.Add(cboAuthorityName);
            Controls.Add(lblAuthority);
            Controls.Add(cmdOK);
            Controls.Add(cmdCancel);
            Controls.Add(txtTypeExample);
            Controls.Add(txtDescription);
            Controls.Add(txtAnnTypeName);
            Controls.Add(lblTypeExample);
            Controls.Add(lblDescription);
            Controls.Add(lblAnnTypeName);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximumSize = new Size(298, 240);
            MinimumSize = new Size(298, 240);
            Name = "frmAddAnnotationType";
            StartPosition = FormStartPosition.Manual;
            Text = "Add Annotation Type";
            ResumeLayout(false);
        }

        #endregion

        private string m_TypeName;
        private string m_Description;
        private string m_Example;
        private int m_AuthID;
        private DataTable m_AuthoritiesTable;
        private string m_PSConnectionString;

        #region "Return Properties"

        public string TypeName
        {
            get
            {
                return m_TypeName;
            }

            set
            {
                m_TypeName = value;
            }
        }

        public string Description
        {
            get
            {
                return m_Description;
            }

            set
            {
                m_Description = value;
            }
        }

        public string Example
        {
            get
            {
                return m_Example;
            }

            set
            {
                m_Example = value;
            }
        }

        public int AuthorityID
        {
            get
            {
                return m_AuthID;
            }

            set
            {
                m_AuthID = value;
            }
        }

        public string ConnectionString
        {
            set
            {
                m_PSConnectionString = value;
            }
        }

        public DataTable AuthorityTable
        {
            set
            {
                m_AuthoritiesTable = value;
            }
        }

        #endregion

        private void frmAddAnnotationType_Load(object sender, EventArgs e)
        {
            if (m_TypeName != null)
            {
                txtAnnTypeName.Text = m_TypeName;
            }

            if (m_Description != null)
            {
                txtDescription.Text = m_Description;
            }

            if (m_Example != null)
            {
                txtTypeExample.Text = m_Example;
            }

            LoadAuthoritiesList();

            if (m_AuthID > 0)
            {
                cboAuthorityName.SelectedValue = m_AuthID;
                cboAuthorityName.Select();
            }
        }

        private void LoadAuthoritiesList()
        {
            cboAuthorityName.SelectedIndexChanged -= cboAuthorityName_SelectedIndexChanged;

            var dr = m_AuthoritiesTable.NewRow();

            dr["ID"] = -2;
            dr["Display_Name"] = "Add New Naming Authority...";
            dr["Details"] = "Brings up a dialog box to allow adding a naming authority to the list";

            var pk1 = new DataColumn[1];
            pk1[0] = m_AuthoritiesTable.Columns["ID"];
            m_AuthoritiesTable.PrimaryKey = pk1;

            if (m_AuthoritiesTable.Rows.Contains(dr["ID"]))
            {
                var rdr = m_AuthoritiesTable.Rows.Find(dr["ID"]);
                m_AuthoritiesTable.Rows.Remove(rdr);
            }

            m_AuthoritiesTable.Rows.Add(dr);

            cboAuthorityName.DataSource = m_AuthoritiesTable;
            cboAuthorityName.DisplayMember = "Display_Name";
            cboAuthorityName.ValueMember = "ID";

            cboAuthorityName.SelectedIndexChanged += cboAuthorityName_SelectedIndexChanged;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            m_TypeName = txtAnnTypeName.Text;
            m_Description = txtDescription.Text;
            m_Example = txtTypeExample.Text;
            m_AuthID = Convert.ToInt32(cboAuthorityName.SelectedValue);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        //private void txtAuthWeb_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    TextBox txt = (TextBox)sender;
        //    var tmpAddress = ValidateWebAddressFormat(txt.Text;
        //    txt.Text = tmpAddress;
        //}

        private void cboAuthorityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (ReferenceEquals(cbo.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                m_AuthID = Convert.ToInt32(cbo.SelectedValue);
            }
            else
            {
                // m_SelectedAuthorityID = 0
            }

            int tmpAuthID;

            if (m_AuthID == -2)
            {
                // Bring up addition dialog
                var AuthAdd = new AddNamingAuthorityType(m_PSConnectionString);
                AuthAdd.FormLocation = new Point(Left + 20, Top + 30);
                tmpAuthID = AuthAdd.AddNamingAuthority();

                if (!AuthAdd.EntryExists & tmpAuthID > 0)
                {
                    DataRow dr;
                    dr = m_AuthoritiesTable.NewRow();

                    dr["ID"] = tmpAuthID;
                    dr["Display_Name"] = AuthAdd.ShortName;
                    dr["Details"] = AuthAdd.FullName;

                    m_AuthoritiesTable.Rows.Add(dr);
                    m_AuthoritiesTable.AcceptChanges();
                    LoadAuthoritiesList();
                    m_AuthID = tmpAuthID;
                }

                cboAuthorityName.SelectedValue = tmpAuthID;
            }

            //if (lvwSelectedFiles.SelectedItems.Count > 0)
            //{
            //    foreach (ListViewItem li In lvwSelectedFiles.SelectedItems)
            //    {
            //        tmpUpInfo = (Protein_Uploader.PSUploadHandler.UploadInfo) m_SelectedFileList[li.SubItems[3].Text];
            //        m_SelectedFileList[li.SubItems[3].Text] =
            //            new Protein_Uploader.PSUploadHandler.UploadInfo(tmpUpInfo.FileInformation, m_SelectedOrganismID, tmpUpInfo.AuthorityID);
            //        li.SubItems[2].Text = cbo.Text;
            //    }
            //}

        }
    }
}