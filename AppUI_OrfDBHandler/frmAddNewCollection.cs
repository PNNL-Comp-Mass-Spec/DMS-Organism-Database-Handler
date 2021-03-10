using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace AppUI_OrfDBHandler
{
    public class frmAddNewCollection : Form
    {
        #region "Windows Form Designer generated code"

        public frmAddNewCollection() : base()
        {
            base.Load += frmAddNewCollection_Load;

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
        internal GroupBox gbxMetaData;
        internal TextBox txtCollectionName;
        internal Label lblCollectionName;
        internal ComboBox cboOrganismPicker;
        internal Label lblOrganismPicker;
        internal Label lblAuthorityPicker;
        internal ComboBox cboAuthorityPicker;
        internal Button cmdAddOrganism;
        internal Button cmdAddAuthority;
        internal Button cmdCancel;
        internal Button cmdOK;
        internal Label lblProteinCount;
        internal Label lblResidueCount;
        internal Label lblDescription;
        internal Label lblSource;
        internal TextBox txtSource;
        internal TextBox txtDescription;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            txtCollectionName = new TextBox();
            txtCollectionName.Leave += new EventHandler(txtCollectionName_Leave);
            lblCollectionName = new Label();
            cboOrganismPicker = new ComboBox();
            lblOrganismPicker = new Label();
            lblAuthorityPicker = new Label();
            cboAuthorityPicker = new ComboBox();
            cmdAddOrganism = new Button();
            cmdAddOrganism.Click += new EventHandler(cmdAddOrganism_Click);
            cmdAddAuthority = new Button();
            cmdAddAuthority.Click += new EventHandler(cmdAddAuthority_Click);
            gbxMetaData = new GroupBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            cmdCancel = new Button();
            cmdCancel.Click += new EventHandler(cmdCancel_Click);
            cmdOK = new Button();
            cmdOK.Click += new EventHandler(cmdOK_Click);
            lblProteinCount = new Label();
            lblResidueCount = new Label();
            lblSource = new Label();
            txtSource = new TextBox();
            gbxMetaData.SuspendLayout();
            SuspendLayout();
            //
            // txtCollectionName
            //
            txtCollectionName.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtCollectionName.BackColor = SystemColors.Window;
            txtCollectionName.Location = new Point(17, 36);
            txtCollectionName.Name = "txtCollectionName";
            txtCollectionName.Size = new Size(454, 24);
            txtCollectionName.TabIndex = 0;
            //
            // lblCollectionName
            //
            lblCollectionName.Location = new Point(14, 19);
            lblCollectionName.Name = "lblCollectionName";
            lblCollectionName.Size = new Size(140, 15);
            lblCollectionName.TabIndex = 1;
            lblCollectionName.Text = "Name";
            //
            // cboOrganismPicker
            //
            cboOrganismPicker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboOrganismPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOrganismPicker.Location = new Point(6, 262);
            cboOrganismPicker.Name = "cboOrganismPicker";
            cboOrganismPicker.Size = new Size(418, 25);
            cboOrganismPicker.TabIndex = 2;
            //
            // lblOrganismPicker
            //
            lblOrganismPicker.Location = new Point(3, 241);
            lblOrganismPicker.Name = "lblOrganismPicker";
            lblOrganismPicker.Size = new Size(140, 20);
            lblOrganismPicker.TabIndex = 3;
            lblOrganismPicker.Text = "Organism";
            //
            // lblAuthorityPicker
            //
            lblAuthorityPicker.Location = new Point(3, 292);
            lblAuthorityPicker.Name = "lblAuthorityPicker";
            lblAuthorityPicker.Size = new Size(140, 20);
            lblAuthorityPicker.TabIndex = 4;
            lblAuthorityPicker.Text = "Authority";
            //
            // cboAuthorityPicker
            //
            cboAuthorityPicker.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboAuthorityPicker.DropDownStyle = ComboBoxStyle.DropDownList;
            cboAuthorityPicker.Location = new Point(6, 313);
            cboAuthorityPicker.Name = "cboAuthorityPicker";
            cboAuthorityPicker.Size = new Size(418, 25);
            cboAuthorityPicker.TabIndex = 5;
            //
            // cmdAddOrganism
            //
            cmdAddOrganism.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdAddOrganism.DialogResult = DialogResult.Cancel;
            cmdAddOrganism.Enabled = false;
            cmdAddOrganism.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdAddOrganism.Location = new Point(432, 262);
            cmdAddOrganism.Name = "cmdAddOrganism";
            cmdAddOrganism.Size = new Size(28, 25);
            cmdAddOrganism.TabIndex = 10;
            cmdAddOrganism.Text = "+";
            //
            // cmdAddAuthority
            //
            cmdAddAuthority.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdAddAuthority.DialogResult = DialogResult.Cancel;
            cmdAddAuthority.Enabled = false;
            cmdAddAuthority.Font = new Font("Microsoft Sans Serif", 15.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            cmdAddAuthority.Location = new Point(432, 313);
            cmdAddAuthority.Name = "cmdAddAuthority";
            cmdAddAuthority.Size = new Size(28, 25);
            cmdAddAuthority.TabIndex = 11;
            cmdAddAuthority.Text = "+";
            //
            // gbxMetaData
            //
            gbxMetaData.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbxMetaData.Controls.Add(lblSource);
            gbxMetaData.Controls.Add(txtSource);
            gbxMetaData.Controls.Add(cmdAddOrganism);
            gbxMetaData.Controls.Add(cboAuthorityPicker);
            gbxMetaData.Controls.Add(cmdAddAuthority);
            gbxMetaData.Controls.Add(lblAuthorityPicker);
            gbxMetaData.Controls.Add(txtCollectionName);
            gbxMetaData.Controls.Add(cboOrganismPicker);
            gbxMetaData.Controls.Add(lblCollectionName);
            gbxMetaData.Controls.Add(lblOrganismPicker);
            gbxMetaData.Controls.Add(lblDescription);
            gbxMetaData.Controls.Add(txtDescription);
            gbxMetaData.Location = new Point(11, 7);
            gbxMetaData.Name = "gbxMetaData";
            gbxMetaData.Size = new Size(488, 354);
            gbxMetaData.TabIndex = 13;
            gbxMetaData.TabStop = false;
            gbxMetaData.Text = "Collection Information";
            //
            // lblDescription
            //
            lblDescription.Location = new Point(14, 68);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(140, 20);
            lblDescription.TabIndex = 1;
            lblDescription.Text = "Description";
            //
            // txtDescription
            //
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.BackColor = SystemColors.Window;
            txtDescription.Location = new Point(17, 89);
            txtDescription.MaxLength = 256;
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(454, 56);
            txtDescription.TabIndex = 0;
            //
            // cmdCancel
            //
            cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdCancel.DialogResult = DialogResult.Cancel;
            cmdCancel.Location = new Point(393, 391);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(105, 28);
            cmdCancel.TabIndex = 14;
            cmdCancel.Text = "Cancel";
            //
            // cmdOK
            //
            cmdOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdOK.DialogResult = DialogResult.OK;
            cmdOK.Location = new Point(275, 391);
            cmdOK.Name = "cmdOK";
            cmdOK.Size = new Size(105, 28);
            cmdOK.TabIndex = 15;
            cmdOK.Text = "OK";
            //
            // lblProteinCount
            //
            lblProteinCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblProteinCount.Location = new Point(11, 386);
            lblProteinCount.Name = "lblProteinCount";
            lblProteinCount.Size = new Size(140, 14);
            lblProteinCount.TabIndex = 17;
            lblProteinCount.Text = "Protein Count: -";
            //
            // lblResidueCount
            //
            lblResidueCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblResidueCount.Location = new Point(11, 405);
            lblResidueCount.Name = "lblResidueCount";
            lblResidueCount.Size = new Size(140, 15);
            lblResidueCount.TabIndex = 16;
            lblResidueCount.Text = "Residue Count: -";
            //
            // lblSource
            //
            lblSource.Location = new Point(14, 152);
            lblSource.Name = "lblSource";
            lblSource.Size = new Size(300, 20);
            lblSource.TabIndex = 13;
            lblSource.Text = "Source (person, url, ftp site, etc.)";
            //
            // txtSource
            //
            txtSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSource.BackColor = SystemColors.Window;
            txtSource.Location = new Point(17, 173);
            txtSource.MaxLength = 256;
            txtSource.Multiline = true;
            txtSource.Name = "txtSource";
            txtSource.Size = new Size(454, 56);
            txtSource.TabIndex = 12;
            //
            // frmAddNewCollection
            //
            AcceptButton = cmdOK;
            AutoScaleBaseSize = new Size(7, 17);
            CancelButton = cmdCancel;
            ClientSize = new Size(510, 430);
            Controls.Add(lblProteinCount);
            Controls.Add(lblResidueCount);
            Controls.Add(cmdOK);
            Controls.Add(cmdCancel);
            Controls.Add(gbxMetaData);
            Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            MaximumSize = new Size(896, 475);
            MinimumSize = new Size(448, 150);
            Name = "frmAddNewCollection";
            Text = "Upload a Protein Collection";
            gbxMetaData.ResumeLayout(false);
            gbxMetaData.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

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
            get
            {
                return m_Local_File;
            }

            set
            {
                m_Local_File = value;
            }
        }

        internal string CollectionName
        {
            get
            {
                return m_CollectionName;
            }

            set
            {
                m_CollectionName = value;
            }
        }

        internal string CollectionDescription
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

        internal string CollectionSource
        {
            get
            {
                return m_CollectionSource;
            }

            set
            {
                m_CollectionSource = value;
            }
        }

        internal DataTable OrganismList
        {
            set
            {
                m_Organisms = value;
            }
        }

        internal DataTable AnnotationTypes
        {
            set
            {
                m_AnnotationTypes = value;
            }
        }

        internal int OrganismID
        {
            get
            {
                return m_OrganismID;
            }

            set
            {
                m_OrganismID = value;
            }
        }

        internal int AnnotationTypeID
        {
            get
            {
                return m_AnnotationTypeID;
            }

            set
            {
                m_AnnotationTypeID = value;
            }
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
            m_OrganismID = Conversions.ToInteger(cboOrganismPicker.SelectedValue);
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
            m_AnnotationTypeID = Conversions.ToInteger(cboAuthorityPicker.SelectedValue);
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
            m_OrganismID = Conversions.ToInteger(cboOrganismPicker.SelectedValue);
            m_AnnotationTypeID = Conversions.ToInteger(cboAuthorityPicker.SelectedValue);
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