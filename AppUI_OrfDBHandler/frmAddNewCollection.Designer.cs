namespace AppUI_OrfDBHandler
{
    partial class frmAddNewCollection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtCollectionName = new System.Windows.Forms.TextBox();
            this.lblCollectionName = new System.Windows.Forms.Label();
            this.cboOrganismPicker = new System.Windows.Forms.ComboBox();
            this.lblOrganismPicker = new System.Windows.Forms.Label();
            this.lblAuthorityPicker = new System.Windows.Forms.Label();
            this.cboAuthorityPicker = new System.Windows.Forms.ComboBox();
            this.cmdAddOrganism = new System.Windows.Forms.Button();
            this.cmdAddAuthority = new System.Windows.Forms.Button();
            this.gbxMetaData = new System.Windows.Forms.GroupBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.lblProteinCount = new System.Windows.Forms.Label();
            this.lblResidueCount = new System.Windows.Forms.Label();
            this.gbxMetaData.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCollectionName
            // 
            this.txtCollectionName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCollectionName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCollectionName.Location = new System.Drawing.Point(12, 30);
            this.txtCollectionName.Name = "txtCollectionName";
            this.txtCollectionName.Size = new System.Drawing.Size(470, 24);
            this.txtCollectionName.TabIndex = 0;
            this.txtCollectionName.Leave += new System.EventHandler(this.txtCollectionName_Leave);
            // 
            // lblCollectionName
            // 
            this.lblCollectionName.Location = new System.Drawing.Point(10, 16);
            this.lblCollectionName.Name = "lblCollectionName";
            this.lblCollectionName.Size = new System.Drawing.Size(100, 12);
            this.lblCollectionName.TabIndex = 1;
            this.lblCollectionName.Text = "Name";
            // 
            // cboOrganismPicker
            // 
            this.cboOrganismPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboOrganismPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboOrganismPicker.Location = new System.Drawing.Point(4, 216);
            this.cboOrganismPicker.Name = "cboOrganismPicker";
            this.cboOrganismPicker.Size = new System.Drawing.Size(445, 25);
            this.cboOrganismPicker.TabIndex = 2;
            // 
            // lblOrganismPicker
            // 
            this.lblOrganismPicker.Location = new System.Drawing.Point(2, 198);
            this.lblOrganismPicker.Name = "lblOrganismPicker";
            this.lblOrganismPicker.Size = new System.Drawing.Size(100, 17);
            this.lblOrganismPicker.TabIndex = 3;
            this.lblOrganismPicker.Text = "Organism";
            // 
            // lblAuthorityPicker
            // 
            this.lblAuthorityPicker.Location = new System.Drawing.Point(2, 240);
            this.lblAuthorityPicker.Name = "lblAuthorityPicker";
            this.lblAuthorityPicker.Size = new System.Drawing.Size(100, 17);
            this.lblAuthorityPicker.TabIndex = 4;
            this.lblAuthorityPicker.Text = "Authority";
            // 
            // cboAuthorityPicker
            // 
            this.cboAuthorityPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAuthorityPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAuthorityPicker.Location = new System.Drawing.Point(4, 258);
            this.cboAuthorityPicker.Name = "cboAuthorityPicker";
            this.cboAuthorityPicker.Size = new System.Drawing.Size(445, 25);
            this.cboAuthorityPicker.TabIndex = 5;
            // 
            // cmdAddOrganism
            // 
            this.cmdAddOrganism.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddOrganism.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdAddOrganism.Enabled = false;
            this.cmdAddOrganism.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddOrganism.Location = new System.Drawing.Point(455, 216);
            this.cmdAddOrganism.Name = "cmdAddOrganism";
            this.cmdAddOrganism.Size = new System.Drawing.Size(20, 20);
            this.cmdAddOrganism.TabIndex = 10;
            this.cmdAddOrganism.Text = "+";
            this.cmdAddOrganism.Click += new System.EventHandler(this.cmdAddOrganismClick);
            // 
            // cmdAddAuthority
            // 
            this.cmdAddAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddAuthority.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdAddAuthority.Enabled = false;
            this.cmdAddAuthority.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddAuthority.Location = new System.Drawing.Point(455, 258);
            this.cmdAddAuthority.Name = "cmdAddAuthority";
            this.cmdAddAuthority.Size = new System.Drawing.Size(20, 20);
            this.cmdAddAuthority.TabIndex = 11;
            this.cmdAddAuthority.Text = "+";
            this.cmdAddAuthority.Click += new System.EventHandler(this.cmdAddAuthority_Click);
            // 
            // gbxMetaData
            // 
            this.gbxMetaData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxMetaData.Controls.Add(this.lblSource);
            this.gbxMetaData.Controls.Add(this.txtSource);
            this.gbxMetaData.Controls.Add(this.cmdAddOrganism);
            this.gbxMetaData.Controls.Add(this.cboAuthorityPicker);
            this.gbxMetaData.Controls.Add(this.cmdAddAuthority);
            this.gbxMetaData.Controls.Add(this.lblAuthorityPicker);
            this.gbxMetaData.Controls.Add(this.txtCollectionName);
            this.gbxMetaData.Controls.Add(this.cboOrganismPicker);
            this.gbxMetaData.Controls.Add(this.lblCollectionName);
            this.gbxMetaData.Controls.Add(this.lblOrganismPicker);
            this.gbxMetaData.Controls.Add(this.lblDescription);
            this.gbxMetaData.Controls.Add(this.txtDescription);
            this.gbxMetaData.Location = new System.Drawing.Point(8, 6);
            this.gbxMetaData.Name = "gbxMetaData";
            this.gbxMetaData.Size = new System.Drawing.Size(494, 291);
            this.gbxMetaData.TabIndex = 13;
            this.gbxMetaData.TabStop = false;
            this.gbxMetaData.Text = "Collection Information";
            // 
            // lblSource
            // 
            this.lblSource.Location = new System.Drawing.Point(10, 125);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(240, 17);
            this.lblSource.TabIndex = 13;
            this.lblSource.Text = "Source (Person, URL, FTP site)";
            // 
            // txtSource
            // 
            this.txtSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSource.BackColor = System.Drawing.SystemColors.Window;
            this.txtSource.Location = new System.Drawing.Point(12, 142);
            this.txtSource.MaxLength = 256;
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(470, 47);
            this.txtSource.TabIndex = 12;
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(10, 56);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(100, 16);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.BackColor = System.Drawing.SystemColors.Window;
            this.txtDescription.Location = new System.Drawing.Point(12, 73);
            this.txtDescription.MaxLength = 256;
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(470, 46);
            this.txtDescription.TabIndex = 0;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(427, 320);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 14;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Location = new System.Drawing.Point(342, 320);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 15;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // lblProteinCount
            // 
            this.lblProteinCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProteinCount.Location = new System.Drawing.Point(8, 316);
            this.lblProteinCount.Name = "lblProteinCount";
            this.lblProteinCount.Size = new System.Drawing.Size(100, 11);
            this.lblProteinCount.TabIndex = 17;
            this.lblProteinCount.Text = "Protein Count: -";
            // 
            // lblResidueCount
            // 
            this.lblResidueCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResidueCount.Location = new System.Drawing.Point(8, 332);
            this.lblResidueCount.Name = "lblResidueCount";
            this.lblResidueCount.Size = new System.Drawing.Size(100, 12);
            this.lblResidueCount.TabIndex = 16;
            this.lblResidueCount.Text = "Residue Count: -";
            // 
            // frmAddNewCollection
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(510, 344);
            this.Controls.Add(this.lblProteinCount);
            this.Controls.Add(this.lblResidueCount);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.gbxMetaData);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(640, 391);
            this.MinimumSize = new System.Drawing.Size(320, 124);
            this.Name = "frmAddNewCollection";
            this.Text = "Upload a Protein Collection";
            this.gbxMetaData.ResumeLayout(false);
            this.gbxMetaData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxMetaData;
        private System.Windows.Forms.TextBox txtCollectionName;
        private System.Windows.Forms.Label lblCollectionName;
        private System.Windows.Forms.ComboBox cboOrganismPicker;
        private System.Windows.Forms.Label lblOrganismPicker;
        private System.Windows.Forms.Label lblAuthorityPicker;
        private System.Windows.Forms.ComboBox cboAuthorityPicker;
        private System.Windows.Forms.Button cmdAddOrganism;
        private System.Windows.Forms.Button cmdAddAuthority;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label lblProteinCount;
        private System.Windows.Forms.Label lblResidueCount;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.TextBox txtDescription;
    }
}