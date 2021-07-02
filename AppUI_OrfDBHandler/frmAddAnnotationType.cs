using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PRISM;

namespace AppUI_OrfDBHandler
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

                var tmpAuthId = authAdd.AddNamingAuthority();

                if (!authAdd.EntryExists && tmpAuthId > 0)
                {
                    var dr = mAuthoritiesTable.NewRow();

                    dr["ID"] = tmpAuthId;
                    dr["Display_Name"] = authAdd.ShortName;
                    dr["Details"] = authAdd.FullName;

                    mAuthoritiesTable.Rows.Add(dr);
                    mAuthoritiesTable.AcceptChanges();
                    LoadAuthoritiesList();
                    AuthorityID = tmpAuthId;
                }

                cboAuthorityName.SelectedValue = tmpAuthId;
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
