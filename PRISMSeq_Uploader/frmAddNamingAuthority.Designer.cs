namespace PRISMSeq_Uploader
{
    partial class frmAddNamingAuthority
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
            this.lblAuthShortName = new System.Windows.Forms.Label();
            this.txtAuthName = new System.Windows.Forms.TextBox();
            this.txtAuthFullName = new System.Windows.Forms.TextBox();
            this.lblAuthFullName = new System.Windows.Forms.Label();
            this.txtAuthWeb = new System.Windows.Forms.TextBox();
            this.lblAuthWeb = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAuthShortName
            // 
            this.lblAuthShortName.Location = new System.Drawing.Point(6, 8);
            this.lblAuthShortName.Name = "lblAuthShortName";
            this.lblAuthShortName.Size = new System.Drawing.Size(266, 16);
            this.lblAuthShortName.TabIndex = 0;
            this.lblAuthShortName.Text = "Authority Short Name (64 char max)";
            // 
            // txtAuthName
            // 
            this.txtAuthName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAuthName.Location = new System.Drawing.Point(6, 24);
            this.txtAuthName.MaxLength = 64;
            this.txtAuthName.Name = "txtAuthName";
            this.txtAuthName.Size = new System.Drawing.Size(278, 21);
            this.txtAuthName.TabIndex = 1;
            // 
            // txtAuthFullName
            // 
            this.txtAuthFullName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAuthFullName.Location = new System.Drawing.Point(7, 66);
            this.txtAuthFullName.MaxLength = 128;
            this.txtAuthFullName.Name = "txtAuthFullName";
            this.txtAuthFullName.Size = new System.Drawing.Size(278, 21);
            this.txtAuthFullName.TabIndex = 3;
            // 
            // lblAuthFullName
            // 
            this.lblAuthFullName.Location = new System.Drawing.Point(7, 50);
            this.lblAuthFullName.Name = "lblAuthFullName";
            this.lblAuthFullName.Size = new System.Drawing.Size(265, 16);
            this.lblAuthFullName.TabIndex = 2;
            this.lblAuthFullName.Text = "Authority Full Name (128 char max)";
            // 
            // txtAuthWeb
            // 
            this.txtAuthWeb.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAuthWeb.Location = new System.Drawing.Point(7, 108);
            this.txtAuthWeb.MaxLength = 128;
            this.txtAuthWeb.Name = "txtAuthWeb";
            this.txtAuthWeb.Size = new System.Drawing.Size(278, 21);
            this.txtAuthWeb.TabIndex = 5;
            this.txtAuthWeb.Validating += new System.ComponentModel.CancelEventHandler(this.txtAuthWeb_Validating);
            // 
            // lblAuthWeb
            // 
            this.lblAuthWeb.Location = new System.Drawing.Point(7, 92);
            this.lblAuthWeb.Name = "lblAuthWeb";
            this.lblAuthWeb.Size = new System.Drawing.Size(265, 16);
            this.lblAuthWeb.TabIndex = 4;
            this.lblAuthWeb.Text = "Web Address (optional, 128 char max)";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdCancel.Location = new System.Drawing.Point(210, 140);
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
            this.cmdOK.Location = new System.Drawing.Point(126, 140);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 7;
            this.cmdOK.Text = "OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmAddNamingAuthority
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(282, 157);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.txtAuthWeb);
            this.Controls.Add(this.txtAuthFullName);
            this.Controls.Add(this.txtAuthName);
            this.Controls.Add(this.lblAuthWeb);
            this.Controls.Add(this.lblAuthFullName);
            this.Controls.Add(this.lblAuthShortName);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(298, 196);
            this.MinimumSize = new System.Drawing.Size(298, 196);
            this.Name = "frmAddNamingAuthority";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Add Naming Authority";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label lblAuthShortName;
        private System.Windows.Forms.TextBox txtAuthName;
        private System.Windows.Forms.TextBox txtAuthWeb;
        private System.Windows.Forms.Label lblAuthWeb;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.TextBox txtAuthFullName;
        private System.Windows.Forms.Label lblAuthFullName;
    }
}