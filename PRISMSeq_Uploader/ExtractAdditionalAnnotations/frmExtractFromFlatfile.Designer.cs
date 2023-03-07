namespace PRISMSeq_Uploader.ExtractAdditionalAnnotations
{
    partial class frmExtractFromFlatFile
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
            this.lblNewNames = new System.Windows.Forms.Label();
            this.lvwNewNames = new System.Windows.Forms.ListView();
            this.colAnnGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAnnGroupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colNamingAuth = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSplitChar = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmdUploadAnnotations = new System.Windows.Forms.Button();
            this.lblCurrentCollectionInfo = new System.Windows.Forms.Label();
            this.lblNamingAuthority = new System.Windows.Forms.Label();
            this.cboNamingAuthority = new System.Windows.Forms.ComboBox();
            this.lvwProteins = new System.Windows.Forms.ListView();
            this.chkUseHeader = new System.Windows.Forms.CheckBox();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblNewNames
            // 
            this.lblNewNames.Location = new System.Drawing.Point(8, 236);
            this.lblNewNames.Name = "lblNewNames";
            this.lblNewNames.Size = new System.Drawing.Size(344, 16);
            this.lblNewNames.TabIndex = 6;
            this.lblNewNames.Text = "Annotations Extracted from Loaded Flat text file";
            // 
            // lvwNewNames
            // 
            this.lvwNewNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwNewNames.CheckBoxes = true;
            this.lvwNewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAnnGroup,
            this.colAnnGroupName,
            this.colNamingAuth,
            this.colSplitChar});
            this.lvwNewNames.FullRowSelect = true;
            this.lvwNewNames.GridLines = true;
            this.lvwNewNames.HideSelection = false;
            this.lvwNewNames.Location = new System.Drawing.Point(8, 252);
            this.lvwNewNames.MultiSelect = false;
            this.lvwNewNames.Name = "lvwNewNames";
            this.lvwNewNames.Size = new System.Drawing.Size(804, 332);
            this.lvwNewNames.TabIndex = 18;
            this.lvwNewNames.UseCompatibleStateImageBehavior = false;
            this.lvwNewNames.View = System.Windows.Forms.View.Details;
            this.lvwNewNames.SelectedIndexChanged += new System.EventHandler(this.lvwNewNames_SelectedIndexChanged);
            // 
            // colAnnGroup
            // 
            this.colAnnGroup.Text = "Group ID";
            this.colAnnGroup.Width = 59;
            // 
            // colAnnGroupName
            // 
            this.colAnnGroupName.Text = "Group Name";
            this.colAnnGroupName.Width = 145;
            // 
            // colNamingAuth
            // 
            this.colNamingAuth.Text = "Naming Authority";
            this.colNamingAuth.Width = 290;
            // 
            // colSplitChar
            // 
            this.colSplitChar.Text = "Delimiter";
            this.colSplitChar.Width = 59;
            // 
            // cmdUploadAnnotations
            // 
            this.cmdUploadAnnotations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUploadAnnotations.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdUploadAnnotations.Location = new System.Drawing.Point(654, 639);
            this.cmdUploadAnnotations.Name = "cmdUploadAnnotations";
            this.cmdUploadAnnotations.Size = new System.Drawing.Size(158, 22);
            this.cmdUploadAnnotations.TabIndex = 9;
            this.cmdUploadAnnotations.Text = "Upload Checked Groups";
            // 
            // lblCurrentCollectionInfo
            // 
            this.lblCurrentCollectionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentCollectionInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCurrentCollectionInfo.Location = new System.Drawing.Point(0, 212);
            this.lblCurrentCollectionInfo.Name = "lblCurrentCollectionInfo";
            this.lblCurrentCollectionInfo.Size = new System.Drawing.Size(824, 16);
            this.lblCurrentCollectionInfo.TabIndex = 12;
            this.lblCurrentCollectionInfo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lblNamingAuthority
            // 
            this.lblNamingAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNamingAuthority.Location = new System.Drawing.Point(238, 591);
            this.lblNamingAuthority.Name = "lblNamingAuthority";
            this.lblNamingAuthority.Size = new System.Drawing.Size(176, 17);
            this.lblNamingAuthority.TabIndex = 16;
            this.lblNamingAuthority.Text = "Naming Authority";
            // 
            // cboNamingAuthority
            // 
            this.cboNamingAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNamingAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNamingAuthority.Location = new System.Drawing.Point(10, 607);
            this.cboNamingAuthority.Name = "cboNamingAuthority";
            this.cboNamingAuthority.Size = new System.Drawing.Size(718, 21);
            this.cboNamingAuthority.TabIndex = 17;
            this.cboNamingAuthority.SelectedIndexChanged += new System.EventHandler(this.cboNamingAuthority_SelectedIndexChanged);
            // 
            // lvwProteins
            // 
            this.lvwProteins.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwProteins.FullRowSelect = true;
            this.lvwProteins.GridLines = true;
            this.lvwProteins.HideSelection = false;
            this.lvwProteins.Location = new System.Drawing.Point(1, 2);
            this.lvwProteins.MultiSelect = false;
            this.lvwProteins.Name = "lvwProteins";
            this.lvwProteins.Size = new System.Drawing.Size(820, 206);
            this.lvwProteins.TabIndex = 19;
            this.lvwProteins.UseCompatibleStateImageBehavior = false;
            this.lvwProteins.View = System.Windows.Forms.View.Details;
            // 
            // chkUseHeader
            // 
            this.chkUseHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkUseHeader.Location = new System.Drawing.Point(630, 234);
            this.chkUseHeader.Name = "chkUseHeader";
            this.chkUseHeader.Size = new System.Drawing.Size(184, 16);
            this.chkUseHeader.TabIndex = 20;
            this.chkUseHeader.Text = "Use First Line as Group Names?";
            this.chkUseHeader.CheckedChanged += new System.EventHandler(this.chkUseHeader_CheckedChanged);
            // 
            // TextBox1
            // 
            this.TextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox1.Location = new System.Drawing.Point(736, 607);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(70, 21);
            this.TextBox1.TabIndex = 23;
            // 
            // Label1
            // 
            this.Label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label1.Location = new System.Drawing.Point(734, 591);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(68, 17);
            this.Label1.TabIndex = 22;
            this.Label1.Text = "Delimiter";
            // 
            // frmExtractFromFlatfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 673);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.chkUseHeader);
            this.Controls.Add(this.lvwProteins);
            this.Controls.Add(this.cboNamingAuthority);
            this.Controls.Add(this.lblNamingAuthority);
            this.Controls.Add(this.cmdUploadAnnotations);
            this.Controls.Add(this.lvwNewNames);
            this.Controls.Add(this.lblNewNames);
            this.Controls.Add(this.lblCurrentCollectionInfo);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(600, 586);
            this.Name = "frmExtractFromFlatFile";
            this.Text = "Extract Annotations From Flatfile";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblNewNames;
        private System.Windows.Forms.Button cmdUploadAnnotations;
        private System.Windows.Forms.Label lblCurrentCollectionInfo;
        private System.Windows.Forms.Label lblNamingAuthority;
        private System.Windows.Forms.ComboBox cboNamingAuthority;
        private System.Windows.Forms.ListView lvwProteins;
        private System.Windows.Forms.ListView lvwNewNames;
        private System.Windows.Forms.ColumnHeader colAnnGroup;
        private System.Windows.Forms.ColumnHeader colAnnGroupName;
        private System.Windows.Forms.ColumnHeader colNamingAuth;
        private System.Windows.Forms.CheckBox chkUseHeader;
        private System.Windows.Forms.ColumnHeader colSplitChar;
        private System.Windows.Forms.TextBox TextBox1;
        private System.Windows.Forms.Label Label1;
    }
}