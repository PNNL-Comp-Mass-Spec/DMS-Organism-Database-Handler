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

        private string mTypeName;
        private string mDescription;
        private string mExample;
        private int mAuthId;
        private DataTable mAuthoritiesTable;
        private string mPsConnectionString;

        #region "Return Properties"

        public string TypeName
        {
            get => mTypeName;
            set => mTypeName = value;
        }

        public string Description
        {
            get => mDescription;
            set => mDescription = value;
        }

        public string Example
        {
            get => mExample;
            set => mExample = value;
        }

        public int AuthorityID
        {
            get => mAuthId;
            set => mAuthId = value;
        }

        public string ConnectionString
        {
            set => mPsConnectionString = value;
        }

        public DataTable AuthorityTable
        {
            set => mAuthoritiesTable = value;
        }

        #endregion

        private void frmAddAnnotationType_Load(object sender, EventArgs e)
        {
            if (mTypeName != null)
            {
                txtAnnTypeName.Text = mTypeName;
            }

            if (mDescription != null)
            {
                txtDescription.Text = mDescription;
            }

            if (mExample != null)
            {
                txtTypeExample.Text = mExample;
            }

            LoadAuthoritiesList();

            if (mAuthId > 0)
            {
                cboAuthorityName.SelectedValue = mAuthId;
                cboAuthorityName.Select();
            }
        }

        private void LoadAuthoritiesList()
        {
            cboAuthorityName.SelectedIndexChanged -= cboAuthorityName_SelectedIndexChanged;

            var dr = mAuthoritiesTable.NewRow();

            dr["ID"] = -2;
            dr["Display_Name"] = "Add New Naming Authority...";
            dr["Details"] = "Brings up a dialog box to allow adding a naming authority to the list";

            var pk1 = new DataColumn[1];
            pk1[0] = mAuthoritiesTable.Columns["ID"];
            mAuthoritiesTable.PrimaryKey = pk1;

            if (mAuthoritiesTable.Rows.Contains(dr["ID"]))
            {
                var rdr = mAuthoritiesTable.Rows.Find(dr["ID"]);
                mAuthoritiesTable.Rows.Remove(rdr);
            }

            mAuthoritiesTable.Rows.Add(dr);

            cboAuthorityName.DataSource = mAuthoritiesTable;
            cboAuthorityName.DisplayMember = "Display_Name";
            cboAuthorityName.ValueMember = "ID";

            cboAuthorityName.SelectedIndexChanged += cboAuthorityName_SelectedIndexChanged;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            mTypeName = txtAnnTypeName.Text;
            mDescription = txtDescription.Text;
            mExample = txtTypeExample.Text;
            mAuthId = Convert.ToInt32(cboAuthorityName.SelectedValue);

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
                mAuthId = Convert.ToInt32(cbo.SelectedValue);
            }
            else
            {
                // mSelectedAuthorityID = 0
            }

            if (mAuthId == -2)
            {
                // Bring up addition dialog
                var authAdd = new AddNamingAuthorityType(mPsConnectionString);
                authAdd.FormLocation = new Point(Left + 20, Top + 30);
                var tmpAuthId = authAdd.AddNamingAuthority();

                if (!authAdd.EntryExists & tmpAuthId > 0)
                {
                    var dr = mAuthoritiesTable.NewRow();

                    dr["ID"] = tmpAuthId;
                    dr["Display_Name"] = authAdd.ShortName;
                    dr["Details"] = authAdd.FullName;

                    mAuthoritiesTable.Rows.Add(dr);
                    mAuthoritiesTable.AcceptChanges();
                    LoadAuthoritiesList();
                    mAuthId = tmpAuthId;
                }

                cboAuthorityName.SelectedValue = tmpAuthId;
            }

            //if (lvwSelectedFiles.SelectedItems.Count > 0)
            //{
            //    foreach (ListViewItem li In lvwSelectedFiles.SelectedItems)
            //    {
            //        tmpUpInfo = (Protein_Uploader.PSUploadHandler.UploadInfo) mSelectedFileList[li.SubItems[3].Text];
            //        mSelectedFileList[li.SubItems[3].Text] =
            //            new Protein_Uploader.PSUploadHandler.UploadInfo(tmpUpInfo.FileInformation, mSelectedOrganismID, tmpUpInfo.AuthorityID);
            //        li.SubItems[2].Text = cbo.Text;
            //    }
            //}

        }
    }
}
