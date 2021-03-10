namespace ExtractAnnotationFromDescription
{
    partial class frmRegexSpecifyFromDescription
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
            this.lblStoredExpressions = new System.Windows.Forms.Label();
            this.cboStoredExpressions = new System.Windows.Forms.ComboBox();
            this.lblRegexEditor = new System.Windows.Forms.Label();
            this.txtRegexEditor = new System.Windows.Forms.TextBox();
            this.lblNewNames = new System.Windows.Forms.Label();
            this.lvwNewNames = new System.Windows.Forms.ListView();
            this.colAnnGroup = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAnnGroupName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colExtAnnotation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAnnotationTYpe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblMatchCount = new System.Windows.Forms.Label();
            this.cmdUploadAnnotations = new System.Windows.Forms.Button();
            this.cmdAddExpression = new System.Windows.Forms.Button();
            this.cmdRemoveExpression = new System.Windows.Forms.Button();
            this.lblCurrentCollectionInfo = new System.Windows.Forms.Label();
            this.cmdMatch = new System.Windows.Forms.Button();
            this.lblNamingAuthority = new System.Windows.Forms.Label();
            this.cboNamingAuthority = new System.Windows.Forms.ComboBox();
            this.lvwProteins = new System.Windows.Forms.ListView();
            this.colProteinName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rdbNameSelect = new System.Windows.Forms.RadioButton();
            this.rdbDescriptionSelect = new System.Windows.Forms.RadioButton();
            this.gbxExtractionSource = new System.Windows.Forms.GroupBox();
            this.gbxExtractionSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStoredExpressions
            // 
            this.lblStoredExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStoredExpressions.Location = new System.Drawing.Point(8, 359);
            this.lblStoredExpressions.Name = "lblStoredExpressions";
            this.lblStoredExpressions.Size = new System.Drawing.Size(164, 11);
            this.lblStoredExpressions.TabIndex = 3;
            this.lblStoredExpressions.Text = "Stored and Recent Expressions";
            // 
            // cboStoredExpressions
            // 
            this.cboStoredExpressions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboStoredExpressions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStoredExpressions.Location = new System.Drawing.Point(8, 374);
            this.cboStoredExpressions.Name = "cboStoredExpressions";
            this.cboStoredExpressions.Size = new System.Drawing.Size(628, 21);
            this.cboStoredExpressions.TabIndex = 2;
            // 
            // lblRegexEditor
            // 
            this.lblRegexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRegexEditor.Location = new System.Drawing.Point(8, 276);
            this.lblRegexEditor.Name = "lblRegexEditor";
            this.lblRegexEditor.Size = new System.Drawing.Size(246, 18);
            this.lblRegexEditor.TabIndex = 4;
            this.lblRegexEditor.Text = "Current Regular Expression";
            // 
            // txtRegexEditor
            // 
            this.txtRegexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRegexEditor.Location = new System.Drawing.Point(8, 293);
            this.txtRegexEditor.Multiline = true;
            this.txtRegexEditor.Name = "txtRegexEditor";
            this.txtRegexEditor.Size = new System.Drawing.Size(670, 57);
            this.txtRegexEditor.TabIndex = 5;
            // 
            // lblNewNames
            // 
            this.lblNewNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewNames.Location = new System.Drawing.Point(8, 402);
            this.lblNewNames.Name = "lblNewNames";
            this.lblNewNames.Size = new System.Drawing.Size(360, 17);
            this.lblNewNames.TabIndex = 6;
            this.lblNewNames.Text = "Extracted Annotations";
            this.lblNewNames.Click += new System.EventHandler(this.lblNewNames_Click);
            // 
            // lvwNewNames
            // 
            this.lvwNewNames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwNewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAnnGroup,
            this.colAnnGroupName,
            this.colExtAnnotation,
            this.colAnnotationTYpe});
            this.lvwNewNames.FullRowSelect = true;
            this.lvwNewNames.GridLines = true;
            this.lvwNewNames.HideSelection = false;
            this.lvwNewNames.Location = new System.Drawing.Point(8, 419);
            this.lvwNewNames.Name = "lvwNewNames";
            this.lvwNewNames.Size = new System.Drawing.Size(804, 197);
            this.lvwNewNames.TabIndex = 18;
            this.lvwNewNames.UseCompatibleStateImageBehavior = false;
            this.lvwNewNames.View = System.Windows.Forms.View.Details;
            this.lvwNewNames.SelectedIndexChanged += new System.EventHandler(this.lbxNewNames_SelectedIndexChanged);
            // 
            // colAnnGroup
            // 
            this.colAnnGroup.Text = "Group ID";
            // 
            // colAnnGroupName
            // 
            this.colAnnGroupName.Text = "Group Name";
            this.colAnnGroupName.Width = 120;
            // 
            // colExtAnnotation
            // 
            this.colExtAnnotation.Text = "Extracted Annotation";
            this.colExtAnnotation.Width = 200;
            // 
            // colAnnotationTYpe
            // 
            this.colAnnotationTYpe.Text = "Annotation Type";
            this.colAnnotationTYpe.Width = 190;
            // 
            // lblMatchCount
            // 
            this.lblMatchCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMatchCount.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblMatchCount.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatchCount.Location = new System.Drawing.Point(358, 405);
            this.lblMatchCount.Name = "lblMatchCount";
            this.lblMatchCount.Size = new System.Drawing.Size(452, 14);
            this.lblMatchCount.TabIndex = 8;
            this.lblMatchCount.Text = "(Matches 0/0 Descriptions)";
            this.lblMatchCount.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // cmdUploadAnnotations
            // 
            this.cmdUploadAnnotations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUploadAnnotations.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdUploadAnnotations.Location = new System.Drawing.Point(654, 639);
            this.cmdUploadAnnotations.Name = "cmdUploadAnnotations";
            this.cmdUploadAnnotations.Size = new System.Drawing.Size(158, 22);
            this.cmdUploadAnnotations.TabIndex = 9;
            this.cmdUploadAnnotations.Text = "Upload Extracted Annotations";
            // 
            // cmdAddExpression
            // 
            this.cmdAddExpression.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdAddExpression.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdAddExpression.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdAddExpression.Location = new System.Drawing.Point(640, 374);
            this.cmdAddExpression.Name = "cmdAddExpression";
            this.cmdAddExpression.Size = new System.Drawing.Size(20, 21);
            this.cmdAddExpression.TabIndex = 10;
            this.cmdAddExpression.Text = "+";
            // 
            // cmdRemoveExpression
            // 
            this.cmdRemoveExpression.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdRemoveExpression.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdRemoveExpression.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdRemoveExpression.Location = new System.Drawing.Point(660, 374);
            this.cmdRemoveExpression.Name = "cmdRemoveExpression";
            this.cmdRemoveExpression.Size = new System.Drawing.Size(20, 21);
            this.cmdRemoveExpression.TabIndex = 11;
            this.cmdRemoveExpression.Text = "-";
            // 
            // lblCurrentCollectionInfo
            // 
            this.lblCurrentCollectionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCurrentCollectionInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCurrentCollectionInfo.Location = new System.Drawing.Point(0, 257);
            this.lblCurrentCollectionInfo.Name = "lblCurrentCollectionInfo";
            this.lblCurrentCollectionInfo.Size = new System.Drawing.Size(824, 15);
            this.lblCurrentCollectionInfo.TabIndex = 12;
            this.lblCurrentCollectionInfo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // cmdMatch
            // 
            this.cmdMatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdMatch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmdMatch.Location = new System.Drawing.Point(686, 373);
            this.cmdMatch.Name = "cmdMatch";
            this.cmdMatch.Size = new System.Drawing.Size(126, 22);
            this.cmdMatch.TabIndex = 13;
            this.cmdMatch.Text = "Test Current Expression";
            this.cmdMatch.Click += new System.EventHandler(this.cmdMatch_Click);
            // 
            // lblNamingAuthority
            // 
            this.lblNamingAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNamingAuthority.Location = new System.Drawing.Point(8, 622);
            this.lblNamingAuthority.Name = "lblNamingAuthority";
            this.lblNamingAuthority.Size = new System.Drawing.Size(494, 18);
            this.lblNamingAuthority.TabIndex = 16;
            this.lblNamingAuthority.Text = "Annotation Type for Selected Group";
            // 
            // cboNamingAuthority
            // 
            this.cboNamingAuthority.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboNamingAuthority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNamingAuthority.Location = new System.Drawing.Point(8, 639);
            this.cboNamingAuthority.Name = "cboNamingAuthority";
            this.cboNamingAuthority.Size = new System.Drawing.Size(634, 21);
            this.cboNamingAuthority.TabIndex = 17;
            // 
            // lvwProteins
            // 
            this.lvwProteins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwProteins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colProteinName,
            this.colDescription});
            this.lvwProteins.FullRowSelect = true;
            this.lvwProteins.GridLines = true;
            this.lvwProteins.HideSelection = false;
            this.lvwProteins.Location = new System.Drawing.Point(1, 2);
            this.lvwProteins.MultiSelect = false;
            this.lvwProteins.Name = "lvwProteins";
            this.lvwProteins.Size = new System.Drawing.Size(820, 253);
            this.lvwProteins.TabIndex = 19;
            this.lvwProteins.UseCompatibleStateImageBehavior = false;
            this.lvwProteins.View = System.Windows.Forms.View.Details;
            // 
            // colProteinName
            // 
            this.colProteinName.Text = "Protein Name";
            this.colProteinName.Width = 170;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 416;
            // 
            // rdbNameSelect
            // 
            this.rdbNameSelect.Location = new System.Drawing.Point(8, 20);
            this.rdbNameSelect.Name = "rdbNameSelect";
            this.rdbNameSelect.Size = new System.Drawing.Size(104, 16);
            this.rdbNameSelect.TabIndex = 20;
            this.rdbNameSelect.Text = "Protein Name";
            this.rdbNameSelect.CheckedChanged += new System.EventHandler(this.rdbSourceSelect_CheckedChanged);
            // 
            // rdbDescriptionSelect
            // 
            this.rdbDescriptionSelect.Location = new System.Drawing.Point(8, 40);
            this.rdbDescriptionSelect.Name = "rdbDescriptionSelect";
            this.rdbDescriptionSelect.Size = new System.Drawing.Size(104, 14);
            this.rdbDescriptionSelect.TabIndex = 21;
            this.rdbDescriptionSelect.Text = "Description Text";
            this.rdbDescriptionSelect.CheckedChanged += new System.EventHandler(this.rdbSourceSelect_CheckedChanged);
            // 
            // gbxExtractionSource
            // 
            this.gbxExtractionSource.Controls.Add(this.rdbNameSelect);
            this.gbxExtractionSource.Controls.Add(this.rdbDescriptionSelect);
            this.gbxExtractionSource.Location = new System.Drawing.Point(460, 170);
            this.gbxExtractionSource.Name = "gbxExtractionSource";
            this.gbxExtractionSource.Size = new System.Drawing.Size(118, 66);
            this.gbxExtractionSource.TabIndex = 22;
            this.gbxExtractionSource.TabStop = false;
            this.gbxExtractionSource.Text = "Extract From";
            // 
            // frmRegexSpecifyFromDescription
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 673);
            this.Controls.Add(this.gbxExtractionSource);
            this.Controls.Add(this.lvwProteins);
            this.Controls.Add(this.cboNamingAuthority);
            this.Controls.Add(this.lblNamingAuthority);
            this.Controls.Add(this.cmdMatch);
            this.Controls.Add(this.cmdRemoveExpression);
            this.Controls.Add(this.cmdAddExpression);
            this.Controls.Add(this.cmdUploadAnnotations);
            this.Controls.Add(this.lvwNewNames);
            this.Controls.Add(this.lblNewNames);
            this.Controls.Add(this.txtRegexEditor);
            this.Controls.Add(this.lblRegexEditor);
            this.Controls.Add(this.lblStoredExpressions);
            this.Controls.Add(this.cboStoredExpressions);
            this.Controls.Add(this.lblCurrentCollectionInfo);
            this.Controls.Add(this.lblMatchCount);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(600, 586);
            this.Name = "frmRegexSpecifyFromDescription";
            this.Text = "Extract Annotations From Description";
            this.gbxExtractionSource.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStoredExpressions;
        private System.Windows.Forms.ComboBox cboStoredExpressions;
        private System.Windows.Forms.Label lblRegexEditor;
        private System.Windows.Forms.TextBox txtRegexEditor;
        private System.Windows.Forms.Label lblNewNames;
        private System.Windows.Forms.Label lblMatchCount;
        private System.Windows.Forms.Button cmdUploadAnnotations;
        private System.Windows.Forms.Button cmdAddExpression;
        private System.Windows.Forms.Button cmdRemoveExpression;
        private System.Windows.Forms.Label lblCurrentCollectionInfo;
        private System.Windows.Forms.Button cmdMatch;
        private System.Windows.Forms.Label lblNamingAuthority;
        private System.Windows.Forms.ComboBox cboNamingAuthority;
        private System.Windows.Forms.ListView lvwProteins;
        private System.Windows.Forms.ColumnHeader colProteinName;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.ListView lvwNewNames;
        private System.Windows.Forms.ColumnHeader colAnnGroup;
        private System.Windows.Forms.ColumnHeader colAnnGroupName;
        private System.Windows.Forms.ColumnHeader colExtAnnotation;
        private System.Windows.Forms.ColumnHeader colAnnotationTYpe;
        private System.Windows.Forms.RadioButton rdbNameSelect;
        private System.Windows.Forms.RadioButton rdbDescriptionSelect;
        private System.Windows.Forms.GroupBox gbxExtractionSource;
    }
}