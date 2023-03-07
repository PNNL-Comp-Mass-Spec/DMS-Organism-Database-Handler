using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PRISM;

namespace PRISMSeq_Uploader
{
    public partial class frmAddAnnotationType : Form
    {
        public frmAddAnnotationType()
        {
            Load += frmAddAnnotationType_Load;

            InitializeComponent();
        }

        private DataTable mAuthoritiesTable;
        private string mPsConnectionString;

        #region "Return Properties"

        public string TypeName { get; set; }

        public string Description { get; set; }

        public string Example { get; set; }

        public int AuthorityID { get; set; }

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
            if (TypeName != null)
            {
                txtAnnTypeName.Text = TypeName;
            }

            if (Description != null)
            {
                txtDescription.Text = Description;
            }

            if (Example != null)
            {
                txtTypeExample.Text = Example;
            }

            LoadAuthoritiesList();

            if (AuthorityID > 0)
            {
                cboAuthorityName.SelectedValue = AuthorityID;
                cboAuthorityName.Select();
            }
        }

        private void LoadAuthoritiesList()
        {
            cboAuthorityName.SelectedIndexChanged -= cboAuthorityName_SelectedIndexChanged;

            var dataRow = mAuthoritiesTable.NewRow();

            dataRow["id"] = -2;
            dataRow["display_name"] = "Add New Naming Authority...";
            dataRow["details"] = "Brings up a dialog box to allow adding a naming authority to the list";

            var pk1 = new DataColumn[1];
            pk1[0] = mAuthoritiesTable.Columns["id"];
            mAuthoritiesTable.PrimaryKey = pk1;

            if (mAuthoritiesTable.Rows.Contains(dataRow["id"]))
            {
                var rdr = mAuthoritiesTable.Rows.Find(dataRow["id"]);
                mAuthoritiesTable.Rows.Remove(rdr);
            }

            mAuthoritiesTable.Rows.Add(dataRow);

            cboAuthorityName.DataSource = mAuthoritiesTable;
            cboAuthorityName.DisplayMember = "display_name";
            cboAuthorityName.ValueMember = "id";

            cboAuthorityName.SelectedIndexChanged += cboAuthorityName_SelectedIndexChanged;
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            TypeName = txtAnnTypeName.Text;
            Description = txtDescription.Text;
            Example = txtTypeExample.Text;
            AuthorityID = Convert.ToInt32(cboAuthorityName.SelectedValue);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cboAuthorityName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = (ComboBox)sender;

            if (ReferenceEquals(cbo.SelectedValue.GetType(), Type.GetType("System.Int32")))
            {
                AuthorityID = Convert.ToInt32(cbo.SelectedValue);
            }

            if (AuthorityID == -2)
            {
                // Bring up addition dialog
                var authAdd = new AddNamingAuthorityType(mPsConnectionString);
                RegisterEvents(authAdd);

                authAdd.FormLocation = new Point(Left + 20, Top + 30);

                var authorityId = authAdd.AddNamingAuthority();

                if (!authAdd.EntryExists && authorityId > 0)
                {
                    var dataRow = mAuthoritiesTable.NewRow();

                    dataRow["id"] = authorityId;
                    dataRow["display_name"] = authAdd.ShortName;
                    dataRow["details"] = authAdd.FullName;

                    mAuthoritiesTable.Rows.Add(dataRow);
                    mAuthoritiesTable.AcceptChanges();
                    LoadAuthoritiesList();
                    AuthorityID = authorityId;
                }

                cboAuthorityName.SelectedValue = authorityId;
            }
        }

        /// <summary>
        /// Use this method to chain events between classes
        /// </summary>
        /// <param name="sourceClass"></param>
        protected void RegisterEvents(EventNotifier sourceClass)
        {
            // sourceClass.DebugEvent += OnDebugEvent;
            // sourceClass.StatusEvent += OnStatusEvent;
            sourceClass.ErrorEvent += OnErrorEvent;
            sourceClass.WarningEvent += OnWarningEvent;
            // sourceClass.ProgressUpdate += OnProgressUpdate;
        }

        private void OnWarningEvent(string message)
        {
            MessageBox.Show("Warning: " + message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void OnErrorEvent(string message, Exception ex)
        {
            MessageBox.Show("Error: " + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}
