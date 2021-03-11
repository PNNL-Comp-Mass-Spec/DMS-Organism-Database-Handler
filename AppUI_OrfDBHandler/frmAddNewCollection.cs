using System;
using System.Data;
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

        private DataTable mAnnotationTypes;
        private DataTable mOrganisms;

        private void frmAddNewCollection_Load(object sender, EventArgs e)
        {
            if (IsLocalFile)
            {
                cboAuthorityPicker.Enabled = true;
                cboOrganismPicker.Enabled = true;
                // txtCollectionName.Visible = false;
                txtCollectionName.Text = CollectionName;

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
                txtCollectionName.Text = CollectionName;
                BindToCombo(cboOrganismPicker, mOrganisms, "Display_Name", "ID");
                cboOrganismPicker.SelectedValue = OrganismId;
                BindToCombo(cboAuthorityPicker, mAnnotationTypes, "Display_Name", "ID");
                cboAuthorityPicker.SelectedValue = AnnotationTypeId;
                cboAuthorityPicker.Enabled = false;
                cboOrganismPicker.Enabled = false;
            }
        }

        internal bool IsLocalFile { get; set; }

        internal string CollectionName { get; set; }

        internal string CollectionDescription { get; set; }

        internal string CollectionSource { get; set; }

        internal DataTable OrganismList
        {
            set => mOrganisms = value;
        }

        internal DataTable AnnotationTypes
        {
            set => mAnnotationTypes = value;
        }

        internal int OrganismId { get; set; }

        internal int AnnotationTypeId { get; set; }

        protected void BindToCombo(
            ComboBox cbo,
            DataTable list,
            string displayMember,
            string valueMember)
        {
            //foreach (DataRow dr in list.Rows)
            //    Debug.WriteLine(dr[0].ToString() + ", " + dr[1].ToString() + ", " + dr[2].ToString() + ", ");

            cbo.DataSource = list;
            cbo.DisplayMember = displayMember;
            //cbo.DisplayMember = list.Columns["Display_Name"].ColumnName.ToString();
            cbo.ValueMember = valueMember;
            //cbo.ValueMember = list.Columns["ID"].ColumnName.ToString();
        }

        #region "Event Handlers"

        private void txtCollectionName_Leave(object sender, EventArgs e)
        {
            CollectionName = txtCollectionName.Text;
        }

        private void cboOrganismPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            OrganismId = Convert.ToInt32(cboOrganismPicker.SelectedValue);
            cmdOK.Enabled = OrganismId != 0;
        }

        private void cboAuthorityPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            AnnotationTypeId = Convert.ToInt32(cboAuthorityPicker.SelectedValue);
        }

        private void cmdAddOrganismClick(object sender, EventArgs e)
        {
        }

        private void cmdAddAuthority_Click(object sender, EventArgs e)
        {
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            CollectionName = txtCollectionName.Text;
            CollectionDescription = txtDescription.Text;
            CollectionSource = txtSource.Text;
            OrganismId = Convert.ToInt32(cboOrganismPicker.SelectedValue);
            AnnotationTypeId = Convert.ToInt32(cboAuthorityPicker.SelectedValue);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            CollectionName = null;
            OrganismId = default;
            AnnotationTypeId = default;
        }

        #endregion
    }
}
