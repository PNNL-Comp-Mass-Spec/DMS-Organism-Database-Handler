namespace PRISMSeq_Uploader
{
    partial class frmAddAnnotationType
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
            this.lblAnnTypeName = new System.Windows.Forms.Label();
            this.txtAnnTypeName = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtTypeExample = new System.Windows.Forms.TextBox();
            this.lblTypeExample = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.lblAuthority = new System.Windows.Forms.Label();
            this.cboAuthorityName = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblAnnTypeName
            // 
            this.lblAnnTypeName.Location = new System.Drawing.Point(6, 8);
            this.lblAnnTypeName.Name = "lblAnnTypeName";
            this.lblAnnTypeName.Size = new System.Drawing.Size(266, 16);
            this.lblAnnTypeName.TabIndex = 0;
            this.lblAnnTypeName.Text = "Annotation Type Name (64 char max)";
            // 
            // txtAnnTypeName
            // 
            this.txtAnnTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnnTypeName.Location = new System.Drawing.Point(6, 24);
            this.txtAnnTypeName.MaxLength = 64;
            this.txtAnnTypeName.Name = "txtAnnTypeName";
            this.txtAnnTypeName.Size = new System.Drawing.Size(276, 21);
            this.txtAnnTypeName.TabIndex = 1;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(7, 66);
            this.txtDescription.MaxLength = 128;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(276, 21);
            this.txtDescription.TabIndex = 3;
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(7, 50);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(265, 16);
            this.lblDescription.TabIndex = 2;
            this.lblDescription.Text = "Annotation Type Description (128 char max)";
            // 
            // txtTypeExample
            // 
            this.txtTypeExample.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTypeExample.Location = new System.Drawing.Point(7, 108);
            this.txtTypeExample.MaxLength = 128;
            this.txtTypeExample.Name = "txtTypeExample";
            this.txtTypeExample.Size = new System.Drawing.Size(276, 21);
            this.txtTypeExample.TabIndex = 5;
            // 
            // lblTypeExample
            // 
            this.lblTypeExample.Location = new System.Drawing.Point(7, 92);
            this.lblTypeExample.Name = "lblTypeExample";
            this.lblTypeExample.Size = new System.Drawing.Size(265, 16);
            this.lblTypeExample.TabIndex = 4;
            this.lblTypeExample.Text = "Example of Annotation (optional, 128 char max)";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(208, 182);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 6;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOK.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOK.Location = new System.Drawing.Point(124, 182);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 7;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // lblAuthority
            // 
            this.lblAuthority.Location = new System.Drawing.Point(7, 134);
            this.lblAuthority.Name = "lblAuthority";
            this.lblAuthority.Size = new System.Drawing.Size(265, 16);
            this.lblAuthority.TabIndex = 8;
            this.lblAuthority.Text = "Naming Authority";
            // 
            // cboAuthorityName
            // 
            this.cboAuthorityName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboAuthorityName.Location = new System.Drawing.Point(7, 150);
            this.cboAuthorityName.Name = "cboAuthorityName";
            this.cboAuthorityName.Size = new System.Drawing.Size(276, 21);
            this.cboAuthorityName.TabIndex = 9;
            this.cboAuthorityName.SelectedIndexChanged += new System.EventHandler(this.cboAuthorityName_SelectedIndexChanged);
            // 
            // frmAddAnnotationType
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(282, 201);
            this.Controls.Add(this.cboAuthorityName);
            this.Controls.Add(this.lblAuthority);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.txtTypeExample);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtAnnTypeName);
            this.Controls.Add(this.lblTypeExample);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblAnnTypeName);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(298, 240);
            this.MinimumSize = new System.Drawing.Size(298, 240);
            this.Name = "frmAddAnnotationType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Add Annotation Type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label lblAnnTypeName;
        private System.Windows.Forms.TextBox txtAnnTypeName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtTypeExample;
        private System.Windows.Forms.Label lblTypeExample;
        private System.Windows.Forms.Label lblAuthority;
        private System.Windows.Forms.ComboBox cboAuthorityName;
    }
}