using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace AppUI_OrfDBHandler
{
    [DesignerGenerated()]
    public partial class frmNewCollectionMetadataEditor : Form
    {

        // Form overrides dispose to clean up the component list.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Required by the Windows Form Designer
        private IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            txtDescription = new TextBox();
            txtDescription.KeyDown += new KeyEventHandler(txtDescription_KeyDown);
            lblDescription = new Label();
            lblSource = new Label();
            txtSource = new TextBox();
            txtSource.KeyDown += new KeyEventHandler(txtSource_KeyDown);
            cmdCancel = new Button();
            cmdCancel.Click += new EventHandler(cmdCancel_Click);
            cmdOk = new Button();
            cmdOk.Click += new EventHandler(cmdOk_Click);
            SuspendLayout();
            // 
            // txtDescription
            // 
            txtDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDescription.Location = new Point(15, 34);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(484, 68);
            txtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.Location = new Point(12, 9);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(208, 22);
            lblDescription.TabIndex = 0;
            lblDescription.Text = "Description";
            // 
            // lblSource
            // 
            lblSource.Location = new Point(12, 115);
            lblSource.Name = "lblSource";
            lblSource.Size = new Size(208, 22);
            lblSource.TabIndex = 2;
            lblSource.Text = "Source (Person, URL, FTP site)";
            // 
            // txtSource
            // 
            txtSource.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSource.Location = new Point(15, 140);
            txtSource.Multiline = true;
            txtSource.Name = "txtSource";
            txtSource.Size = new Size(484, 68);
            txtSource.TabIndex = 3;
            // 
            // cmdCancel
            // 
            cmdCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdCancel.DialogResult = DialogResult.Cancel;
            cmdCancel.Location = new Point(415, 220);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new Size(84, 27);
            cmdCancel.TabIndex = 5;
            cmdCancel.Text = "Cancel";
            // 
            // cmdOk
            // 
            cmdOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdOk.DialogResult = DialogResult.OK;
            cmdOk.Location = new Point(310, 220);
            cmdOk.Name = "cmdOk";
            cmdOk.Size = new Size(84, 27);
            cmdOk.TabIndex = 4;
            cmdOk.Text = "&Ok";
            // 
            // frmNewCollectionMetadataEditor
            // 
            AutoScaleDimensions = new SizeF(8.0f, 16.0f);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cmdCancel;
            ClientSize = new Size(517, 259);
            ControlBox = false;
            Controls.Add(cmdOk);
            Controls.Add(cmdCancel);
            Controls.Add(txtSource);
            Controls.Add(lblSource);
            Controls.Add(txtDescription);
            Controls.Add(lblDescription);
            Name = "frmNewCollectionMetadataEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Protein Collection Metadata";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        internal TextBox txtDescription;
        internal Label lblDescription;
        internal Label lblSource;
        internal TextBox txtSource;
        internal Button cmdCancel;
        internal Button cmdOk;
    }
}