using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public partial class frmAddAnnotationType : Form
    {
        public frmAddAnnotationType()
        {
            base.Load += frmAddAnnotationType_Load;

            InitializeComponent();
        }

        private string m_TypeName;
        private string m_Description;
        private string m_Example;
        private int m_AuthID;
        private DataTable m_AuthoritiesTable;
        private string m_PSConnectionString;

        #region "Return Properties"

        public string TypeName
        {
            get => m_TypeName;
            set => m_TypeName = value;
        }

        public string Description
        {
            get => m_Description;
            set => m_Description = value;
        }

        public string Example
        {
            get => m_Example;
            set => m_Example = value;
        }

        public int AuthorityID
        {
            get => m_AuthID;
            set => m_AuthID = value;
        }

        public string ConnectionString
        {
            set => m_PSConnectionString = value;
        }

        public DataTable AuthorityTable
        {
            set => m_AuthoritiesTable = value;
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

            if (m_AuthID == -2)
            {
                // Bring up addition dialog
                var AuthAdd = new AddNamingAuthorityType(m_PSConnectionString);
                AuthAdd.FormLocation = new Point(Left + 20, Top + 30);
                var tmpAuthID = AuthAdd.AddNamingAuthority();

                if (!AuthAdd.EntryExists & tmpAuthID > 0)
                {
                    var dr = m_AuthoritiesTable.NewRow();

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
