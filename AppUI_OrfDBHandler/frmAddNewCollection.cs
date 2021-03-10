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

        protected string m_CollectionName;
        protected string m_Description;
        protected string m_CollectionSource;

        protected int m_OrganismID;
        protected int m_AnnotationTypeID;

        protected DataTable m_AnnotationTypes;
        protected DataTable m_Organisms;

        protected bool m_Local_File;

        private void frmAddNewCollection_Load(object sender, EventArgs e)
        {
            if (IsLocalFile)
            {
                cboAuthorityPicker.Enabled = true;
                cboOrganismPicker.Enabled = true;
                // txtCollectionName.Visible = false;
                txtCollectionName.Text = m_CollectionName;

                m_Organisms.Rows[0]["Display_Name"] = " -- Select an Organism --";
                m_Organisms.Rows[0]["ID"] = 0;
                m_Organisms.AcceptChanges();

                BindToCombo(cboAuthorityPicker, m_AnnotationTypes, "Display_Name", "ID");
                BindToCombo(cboOrganismPicker, m_Organisms, "Display_Name", "ID");
                //cboOrganismPicker.Items.RemoveAt(0);
                cboOrganismPicker.SelectedValue = 1;
                cboOrganismPicker.SelectedIndexChanged += cboOrganismPicker_SelectedIndexChanged;
                cboAuthorityPicker.SelectedIndexChanged += cboAuthorityPicker_SelectedIndexChanged;
            }
            else
            {
                txtCollectionName.Visible = true;
                txtCollectionName.Text = m_CollectionName;
                BindToCombo(cboOrganismPicker, m_Organisms, "Display_Name", "ID");
                cboOrganismPicker.SelectedValue = m_OrganismID;
                BindToCombo(cboAuthorityPicker, m_AnnotationTypes, "Display_Name", "ID");
                cboAuthorityPicker.SelectedValue = m_AnnotationTypeID;
                cboAuthorityPicker.Enabled = false;
                cboOrganismPicker.Enabled = false;
            }
        }

        internal bool IsLocalFile
        {
            get => m_Local_File;
            set => m_Local_File = value;
        }

        internal string CollectionName
        {
            get => m_CollectionName;
            set => m_CollectionName = value;
        }

        internal string CollectionDescription
        {
            get => m_Description;
            set => m_Description = value;
        }

        internal string CollectionSource
        {
            get => m_CollectionSource;
            set => m_CollectionSource = value;
        }

        internal DataTable OrganismList
        {
            set => m_Organisms = value;
        }

        internal DataTable AnnotationTypes
        {
            set => m_AnnotationTypes = value;
        }

        internal int OrganismID
        {
            get => m_OrganismID;
            set => m_OrganismID = value;
        }

        internal int AnnotationTypeID
        {
            get => m_AnnotationTypeID;
            set => m_AnnotationTypeID = value;
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
            m_CollectionName = txtCollectionName.Text;
        }

        private void cboOrganismPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_OrganismID = Convert.ToInt32(cboOrganismPicker.SelectedValue);
            if (m_OrganismID == 0)
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
            m_AnnotationTypeID = Convert.ToInt32(cboAuthorityPicker.SelectedValue);
        }

        private void cmdAddOrganism_Click(object sender, EventArgs e)
        {
        }

        private void cmdAddAuthority_Click(object sender, EventArgs e)
        {
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            m_CollectionName = txtCollectionName.Text;
            m_Description = txtDescription.Text;
            m_CollectionSource = txtSource.Text;
            m_OrganismID = Convert.ToInt32(cboOrganismPicker.SelectedValue);
            m_AnnotationTypeID = Convert.ToInt32(cboAuthorityPicker.SelectedValue);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            m_CollectionName = null;
            m_OrganismID = default;
            m_AnnotationTypeID = default;
        }

        #endregion
    }
}
