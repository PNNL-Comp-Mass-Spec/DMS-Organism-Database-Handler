using System;
using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices;

namespace ExtractAnnotationFromDescription
{
    public class frmRegexSpecifyFromDescription : System.Windows.Forms.Form
    {
        #region "Windows Form Designer generated code"

        public frmRegexSpecifyFromDescription() : base()
        {
            base.Load += frmRegexSpecify_Load;

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
        internal System.Windows.Forms.Label lblStoredExpressions;
        internal System.Windows.Forms.ComboBox cboStoredExpressions;
        internal System.Windows.Forms.Label lblRegexEditor;
        internal System.Windows.Forms.TextBox txtRegexEditor;
        internal System.Windows.Forms.Label lblNewNames;
        internal System.Windows.Forms.Label lblMatchCount;
        internal System.Windows.Forms.Button cmdUploadAnnotations;
        internal System.Windows.Forms.Button cmdAddExpression;
        internal System.Windows.Forms.Button cmdRemoveExpression;
        internal System.Windows.Forms.Label lblCurrentCollectionInfo;
        internal System.Windows.Forms.Button cmdMatch;
        internal System.Windows.Forms.Label lblNamingAuthority;
        internal System.Windows.Forms.ComboBox cboNamingAuthority;
        internal System.Windows.Forms.ListView lvwProteins;
        internal System.Windows.Forms.ColumnHeader colProteinName;
        internal System.Windows.Forms.ColumnHeader colDescription;
        internal System.Windows.Forms.ListView lvwNewNames;
        internal System.Windows.Forms.ColumnHeader colAnnGroup;
        internal System.Windows.Forms.ColumnHeader colAnnGroupName;
        internal System.Windows.Forms.ColumnHeader colExtAnnotation;
        internal System.Windows.Forms.ColumnHeader colAnnotationTYpe;
        internal System.Windows.Forms.RadioButton rdbNameSelect;
        internal System.Windows.Forms.RadioButton rdbDescriptionSelect;
        internal System.Windows.Forms.GroupBox gbxExtractionSource;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            lblStoredExpressions = new System.Windows.Forms.Label();
            cboStoredExpressions = new System.Windows.Forms.ComboBox();
            lblRegexEditor = new System.Windows.Forms.Label();
            txtRegexEditor = new System.Windows.Forms.TextBox();
            lblNewNames = new System.Windows.Forms.Label();
            lblNewNames.Click += new EventHandler(lblNewNames_Click);
            lvwNewNames = new System.Windows.Forms.ListView();
            lvwNewNames.SelectedIndexChanged += new EventHandler(lbxNewNames_SelectedIndexChanged);
            colAnnGroup = new System.Windows.Forms.ColumnHeader();
            colAnnGroupName = new System.Windows.Forms.ColumnHeader();
            colExtAnnotation = new System.Windows.Forms.ColumnHeader();
            colAnnotationTYpe = new System.Windows.Forms.ColumnHeader();
            lblMatchCount = new System.Windows.Forms.Label();
            cmdUploadAnnotations = new System.Windows.Forms.Button();
            cmdAddExpression = new System.Windows.Forms.Button();
            cmdRemoveExpression = new System.Windows.Forms.Button();
            lblCurrentCollectionInfo = new System.Windows.Forms.Label();
            cmdMatch = new System.Windows.Forms.Button();
            cmdMatch.Click += new EventHandler(cmdMatch_Click);
            lblNamingAuthority = new System.Windows.Forms.Label();
            cboNamingAuthority = new System.Windows.Forms.ComboBox();
            lvwProteins = new System.Windows.Forms.ListView();
            colProteinName = new System.Windows.Forms.ColumnHeader();
            colDescription = new System.Windows.Forms.ColumnHeader();
            rdbNameSelect = new System.Windows.Forms.RadioButton();
            rdbNameSelect.CheckedChanged += new EventHandler(rdbSourceSelect_CheckedChanged);
            rdbDescriptionSelect = new System.Windows.Forms.RadioButton();
            rdbDescriptionSelect.CheckedChanged += new EventHandler(rdbSourceSelect_CheckedChanged);
            gbxExtractionSource = new System.Windows.Forms.GroupBox();
            gbxExtractionSource.SuspendLayout();
            SuspendLayout();
            //
            // lblStoredExpressions
            //
            lblStoredExpressions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblStoredExpressions.Location = new System.Drawing.Point(11, 291);
            lblStoredExpressions.Name = "lblStoredExpressions";
            lblStoredExpressions.Size = new System.Drawing.Size(230, 14);
            lblStoredExpressions.TabIndex = 3;
            lblStoredExpressions.Text = "Stored and Recent Expressions";
            //
            // cboStoredExpressions
            //
            cboStoredExpressions.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboStoredExpressions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboStoredExpressions.Location = new System.Drawing.Point(11, 310);
            cboStoredExpressions.Name = "cboStoredExpressions";
            cboStoredExpressions.Size = new System.Drawing.Size(550, 25);
            cboStoredExpressions.TabIndex = 2;
            //
            // lblRegexEditor
            //
            lblRegexEditor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblRegexEditor.Location = new System.Drawing.Point(11, 191);
            lblRegexEditor.Name = "lblRegexEditor";
            lblRegexEditor.Size = new System.Drawing.Size(345, 22);
            lblRegexEditor.TabIndex = 4;
            lblRegexEditor.Text = "Current Regular Expression";
            //
            // txtRegexEditor
            //
            txtRegexEditor.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtRegexEditor.Location = new System.Drawing.Point(11, 211);
            txtRegexEditor.Multiline = true;
            txtRegexEditor.Name = "txtRegexEditor";
            txtRegexEditor.Size = new System.Drawing.Size(609, 70);
            txtRegexEditor.TabIndex = 5;
            //
            // lblNewNames
            //
            lblNewNames.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblNewNames.Location = new System.Drawing.Point(11, 344);
            lblNewNames.Name = "lblNewNames";
            lblNewNames.Size = new System.Drawing.Size(175, 20);
            lblNewNames.TabIndex = 6;
            lblNewNames.Text = "Extracted Annotations";
            //
            // lvwNewNames
            //
            lvwNewNames.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lvwNewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colAnnGroup, colAnnGroupName, colExtAnnotation, colAnnotationTYpe });
            lvwNewNames.FullRowSelect = true;
            lvwNewNames.GridLines = true;
            lvwNewNames.Location = new System.Drawing.Point(11, 364);
            lvwNewNames.Name = "lvwNewNames";
            lvwNewNames.Size = new System.Drawing.Size(797, 240);
            lvwNewNames.TabIndex = 18;
            lvwNewNames.UseCompatibleStateImageBehavior = false;
            lvwNewNames.View = System.Windows.Forms.View.Details;
            //
            // colAnnGroup
            //
            colAnnGroup.Text = "Group ID";
            //
            // colAnnGroupName
            //
            colAnnGroupName.Text = "Group Name";
            colAnnGroupName.Width = 120;
            //
            // colExtAnnotation
            //
            colExtAnnotation.Text = "Extracted Annotation";
            colExtAnnotation.Width = 200;
            //
            // colAnnotationTYpe
            //
            colAnnotationTYpe.Text = "Annotation Type";
            colAnnotationTYpe.Width = 190;
            //
            // lblMatchCount
            //
            lblMatchCount.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblMatchCount.FlatStyle = System.Windows.Forms.FlatStyle.System;
            lblMatchCount.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Conversions.ToByte(0));
            lblMatchCount.Location = new System.Drawing.Point(501, 347);
            lblMatchCount.Name = "lblMatchCount";
            lblMatchCount.Size = new System.Drawing.Size(304, 17);
            lblMatchCount.TabIndex = 8;
            lblMatchCount.Text = "(Matches 0/0 Descriptions)";
            lblMatchCount.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            //
            // cmdUploadAnnotations
            //
            cmdUploadAnnotations.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cmdUploadAnnotations.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdUploadAnnotations.Location = new System.Drawing.Point(587, 631);
            cmdUploadAnnotations.Name = "cmdUploadAnnotations";
            cmdUploadAnnotations.Size = new System.Drawing.Size(221, 27);
            cmdUploadAnnotations.TabIndex = 9;
            cmdUploadAnnotations.Text = "Upload Extracted Annotations";
            //
            // cmdAddExpression
            //
            cmdAddExpression.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cmdAddExpression.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdAddExpression.Font = new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Conversions.ToByte(0));
            cmdAddExpression.Location = new System.Drawing.Point(567, 310);
            cmdAddExpression.Name = "cmdAddExpression";
            cmdAddExpression.Size = new System.Drawing.Size(28, 25);
            cmdAddExpression.TabIndex = 10;
            cmdAddExpression.Text = "+";
            //
            // cmdRemoveExpression
            //
            cmdRemoveExpression.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cmdRemoveExpression.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdRemoveExpression.Font = new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Conversions.ToByte(0));
            cmdRemoveExpression.Location = new System.Drawing.Point(595, 310);
            cmdRemoveExpression.Name = "cmdRemoveExpression";
            cmdRemoveExpression.Size = new System.Drawing.Size(28, 25);
            cmdRemoveExpression.TabIndex = 11;
            cmdRemoveExpression.Text = "-";
            //
            // lblCurrentCollectionInfo
            //
            lblCurrentCollectionInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblCurrentCollectionInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            lblCurrentCollectionInfo.Location = new System.Drawing.Point(0, 167);
            lblCurrentCollectionInfo.Name = "lblCurrentCollectionInfo";
            lblCurrentCollectionInfo.Size = new System.Drawing.Size(825, 19);
            lblCurrentCollectionInfo.TabIndex = 12;
            lblCurrentCollectionInfo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            //
            // cmdMatch
            //
            cmdMatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            cmdMatch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            cmdMatch.Location = new System.Drawing.Point(631, 308);
            cmdMatch.Name = "cmdMatch";
            cmdMatch.Size = new System.Drawing.Size(177, 27);
            cmdMatch.TabIndex = 13;
            cmdMatch.Text = "Test Current Expression";
            //
            // lblNamingAuthority
            //
            lblNamingAuthority.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblNamingAuthority.Location = new System.Drawing.Point(11, 611);
            lblNamingAuthority.Name = "lblNamingAuthority";
            lblNamingAuthority.Size = new System.Drawing.Size(363, 22);
            lblNamingAuthority.TabIndex = 16;
            lblNamingAuthority.Text = "Annotation Type for Selected Group";
            //
            // cboNamingAuthority
            //
            cboNamingAuthority.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            cboNamingAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboNamingAuthority.Location = new System.Drawing.Point(11, 631);
            cboNamingAuthority.Name = "cboNamingAuthority";
            cboNamingAuthority.Size = new System.Drawing.Size(559, 25);
            cboNamingAuthority.TabIndex = 17;
            //
            // lvwProteins
            //
            lvwProteins.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            lvwProteins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colProteinName, colDescription });
            lvwProteins.FullRowSelect = true;
            lvwProteins.GridLines = true;
            lvwProteins.Location = new System.Drawing.Point(1, 2);
            lvwProteins.MultiSelect = false;
            lvwProteins.Name = "lvwProteins";
            lvwProteins.Size = new System.Drawing.Size(819, 163);
            lvwProteins.TabIndex = 19;
            lvwProteins.UseCompatibleStateImageBehavior = false;
            lvwProteins.View = System.Windows.Forms.View.Details;
            //
            // colProteinName
            //
            colProteinName.Text = "Protein Name";
            colProteinName.Width = 170;
            //
            // colDescription
            //
            colDescription.Text = "Description";
            colDescription.Width = 416;
            //
            // rdbNameSelect
            //
            rdbNameSelect.Location = new System.Drawing.Point(11, 24);
            rdbNameSelect.Name = "rdbNameSelect";
            rdbNameSelect.Size = new System.Drawing.Size(146, 20);
            rdbNameSelect.TabIndex = 20;
            rdbNameSelect.Text = "Protein Name";
            //
            // rdbDescriptionSelect
            //
            rdbDescriptionSelect.Location = new System.Drawing.Point(11, 49);
            rdbDescriptionSelect.Name = "rdbDescriptionSelect";
            rdbDescriptionSelect.Size = new System.Drawing.Size(146, 17);
            rdbDescriptionSelect.TabIndex = 21;
            rdbDescriptionSelect.Text = "Description Text";
            //
            // gbxExtractionSource
            //
            gbxExtractionSource.Controls.Add(rdbNameSelect);
            gbxExtractionSource.Controls.Add(rdbDescriptionSelect);
            gbxExtractionSource.Location = new System.Drawing.Point(644, 206);
            gbxExtractionSource.Name = "gbxExtractionSource";
            gbxExtractionSource.Size = new System.Drawing.Size(165, 81);
            gbxExtractionSource.TabIndex = 22;
            gbxExtractionSource.TabStop = false;
            gbxExtractionSource.Text = "Extract From";
            //
            // frmRegexSpecifyFromDescription
            //
            AutoScaleBaseSize = new System.Drawing.Size(7, 17);
            ClientSize = new System.Drawing.Size(822, 667);
            Controls.Add(gbxExtractionSource);
            Controls.Add(lvwProteins);
            Controls.Add(cboNamingAuthority);
            Controls.Add(lblNamingAuthority);
            Controls.Add(cmdMatch);
            Controls.Add(cmdRemoveExpression);
            Controls.Add(cmdAddExpression);
            Controls.Add(cmdUploadAnnotations);
            Controls.Add(lvwNewNames);
            Controls.Add(lblNewNames);
            Controls.Add(txtRegexEditor);
            Controls.Add(lblRegexEditor);
            Controls.Add(lblStoredExpressions);
            Controls.Add(cboStoredExpressions);
            Controls.Add(lblCurrentCollectionInfo);
            Controls.Add(lblMatchCount);
            Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Conversions.ToByte(0));
            MinimumSize = new System.Drawing.Size(840, 712);
            Name = "frmRegexSpecifyFromDescription";
            Text = "Extract Annotations From Description";
            gbxExtractionSource.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ExtractionSources m_ExtractionSource;

        public enum ExtractionSources
        {
            Name,
            Description,
        }

        private void frmRegexSpecify_Load(object sender, EventArgs e)
        {
        }

        private void lblNewNames_Click(object sender, EventArgs e)
        {
        }

        private void lbxNewNames_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmdMatch_Click(object sender, EventArgs e)
        {
        }

        private void rdbSourceSelect_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.RadioButton rdb = (System.Windows.Forms.RadioButton)sender;

            if (rdb.Checked == true)
            {
                if ((rdb.Name ?? "") == (rdbNameSelect.Name ?? ""))
                {
                    m_ExtractionSource = ExtractionSources.Name;
                }
                else if ((rdb.Name ?? "") == (rdbDescriptionSelect.Name ?? ""))
                {
                    m_ExtractionSource = ExtractionSources.Description;
                }
            }
        }
    }
}