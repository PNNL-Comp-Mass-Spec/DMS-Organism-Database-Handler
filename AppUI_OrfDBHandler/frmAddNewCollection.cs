using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public partial class frmAddNewCollection : Form
    {
        public frmAddNewCollection()
        {
            base.Load += frmAddNewCollection_Load;

            InitializeComponent();
        }

        private string mCollectionName;
        private string mDescription;
        private string mCollectionSource;

        private int mOrganismID;
        private int mAnnotationTypeID;

        private DataTable mAnnotationTypes;
        private DataTable mOrganisms;

        private bool mLocal_File;

        private void frmAddNewCollection_Load(object sender, EventArgs e)
        {
            if (IsLocalFile)
            {
                cboAuthorityPicker.Enabled = true;
                cboOrganismPicker.Enabled = true;
                // txtCollectionName.Visible = false;
                txtCollectionName.Text = mCollectionName;

                mOrganisms.Rows[0]["Display_Name"] = " -- Select an Organism --";
                mOrganisms.Rows[0]["ID"] = 0;
                mOrganisms.AcceptChanges();

                BindToCombo(cboAuthorityPicker, mAnnotationTypes, "Display_Name", "ID");
                BindToCombo(cboOrganismPicker, mOrganisms, "Display_Name", "ID");
                //cboOrganismPicker.Items.RemoveAt(0);
                cboOrganismPicker.SelectedValue = 1;
                cboOrganismPicker.SelectedIndexChanged += cboOrganismPicker_SelectedIndexChanged;
                cboAuthorityPicker.SelectedIndexChanged += cboAuthorityPicker_SelectedIndexChanged;
            }
            else
            {
                txtCollectionName.Visible = true;
                txtCollectionName.Text = mCollectionName;
                BindToCombo(cboOrganismPicker, mOrganisms, "Display_Name", "ID");
                cboOrganismPicker.SelectedValue = mOrganismID;
                BindToCombo(cboAuthorityPicker, mAnnotationTypes, "Display_Name", "ID");
                cboAuthorityPicker.SelectedValue = mAnnotationTypeID;
                cboAuthorityPicker.Enabled = false;
                cboOrganismPicker.Enabled = false;
            }
        }

        internal bool IsLocalFile
        {
            get => mLocal_File;
            set => mLocal_File = value;
        }

        internal string CollectionName
        {
            get => mCollectionName;
            set => mCollectionName = value;
        }

        internal string CollectionDescription
        {
            get => mDescription;
            set => mDescription = value;
        }

        internal string CollectionSource
        {
            get => mCollectionSource;
            set => mCollectionSource = value;
        }

        internal DataTable OrganismList
        {
            set => mOrganisms = value;
        }

        internal DataTable AnnotationTypes
        {
            set => mAnnotationTypes = value;
        }

        internal int OrganismID
        {
            get => mOrganismID;
            set => mOrganismID = value;
        }

        internal int AnnotationTypeID
        {
            get => mAnnotationTypeID;
            set => mAnnotationTypeID = value;
        }

        protected void BindToCombo(
            ComboBox cbo,
            DataTable list,
            string DisplayMember,
            string ValueMember)
        {
            //foreach (DataRow dr in list.Rows)
            //    Debug.WriteLine(dr[0].ToString() + ", " + dr[1].ToString() + ", " + dr[2].ToString() + ", ");

            cbo.DataSource = list;
            cbo.DisplayMember = DisplayMember;
            //cbo.DisplayMember = list.Columns["Display_Name"].ColumnName.ToString();
            cbo.ValueMember = ValueMember;
            //cbo.ValueMember = list.Columns["ID"].ColumnName.ToString();
        }

        #region "Event Handlers"

        private void txtCollectionName_Leave(object sender, EventArgs e)
        {
            mCollectionName = txtCollectionName.Text;
        }

        private void cboOrganismPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            mOrganismID = Convert.ToInt32(cboOrganismPicker.SelectedValue);
            if (mOrganismID == 0)
            {
                cmdOK.Enabled = false;
            }
            else
            {
                cmdOK.Enabled = true;
            }
        }

        private void cboAuthorityPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            mAnnotationTypeID = Convert.ToInt32(cboAuthorityPicker.SelectedValue);
        }

        private void cmdAddOrganismClick(object sender, EventArgs e)
        {
        }

        private void cmdAddAuthority_Click(object sender, EventArgs e)
        {
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            mCollectionName = txtCollectionName.Text;
            mDescription = txtDescription.Text;
            mCollectionSource = txtSource.Text;
            mOrganismID = Convert.ToInt32(cboOrganismPicker.SelectedValue);
            mAnnotationTypeID = Convert.ToInt32(cboAuthorityPicker.SelectedValue);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            mCollectionName = null;
            mOrganismID = default;
            mAnnotationTypeID = default;
        }

        #endregion
    }
}
