using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using NucleotideTranslator;
using Protein_Exporter;
using Protein_Importer;
using Protein_Uploader;
using TranslationTableImport;

namespace AppUI_OrfDBHandler
{
    public class frmTestingInterface : Form
    {
        #region "Windows Form Designer generated code"

        public frmTestingInterface() : base()
        {
            base.Load += frmTestingInterface_Load;

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
        internal TextBox txtTransFilePath;
        internal GroupBox gbxTransTableImportTest;
        internal GroupBox gbxFASTAImportTest;
        internal TextBox txtFASTAFilePath;
        internal TextBox txtConnString;
        internal Button cmdLoadTT;
        internal Button cmdBrowseTT;
        internal Button cmdBrowseFF;
        internal Button cmdLoadFF;
        internal GroupBox GroupBox2;
        internal Button Button2;
        internal TextBox TextBox1;
        internal GroupBox GroupBox3;
        internal Button cmdExportFASTA;
        internal ComboBox cboCollectionsList;
        internal GroupBox gbxConnectionString;
        internal Button cmdBatchLoadDMS;
        internal GroupBox gbxOtherStuff;
        internal Button cmdUpdateArchiveTables;
        internal Button Button3;
        internal Label lblProgress;
        internal ProgressBar pgbAdminConsole;

        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            txtTransFilePath = new TextBox();
            cmdLoadTT = new Button();
            cmdLoadTT.Click += new EventHandler(cmdLoadTT_Click);
            cmdBrowseTT = new Button();
            cmdBrowseTT.Click += new EventHandler(cmdBrowseTT_Click);
            gbxTransTableImportTest = new GroupBox();
            gbxFASTAImportTest = new GroupBox();
            cmdBrowseFF = new Button();
            cmdBrowseFF.Click += new EventHandler(cmdBrowseTT_Click);
            cmdLoadFF = new Button();
            cmdLoadFF.Click += new EventHandler(cmdLoadFF_Click);
            txtFASTAFilePath = new TextBox();
            txtConnString = new TextBox();
            gbxConnectionString = new GroupBox();
            GroupBox2 = new GroupBox();
            Button2 = new Button();
            Button2.Click += new EventHandler(Button2_Click);
            TextBox1 = new TextBox();
            GroupBox3 = new GroupBox();
            cmdBatchLoadDMS = new Button();
            cmdBatchLoadDMS.Click += new EventHandler(cmdBatchLoadDMS_Click_1);
            cboCollectionsList = new ComboBox();
            cmdExportFASTA = new Button();
            cmdExportFASTA.Click += new EventHandler(cmdExportFASTA_Click);
            gbxOtherStuff = new GroupBox();
            cmdUpdateArchiveTables = new Button();
            cmdUpdateArchiveTables.Click += new EventHandler(cmdUpdateArchiveTables_Click);
            Button3 = new Button();
            lblProgress = new Label();
            pgbAdminConsole = new ProgressBar();
            gbxTransTableImportTest.SuspendLayout();
            gbxFASTAImportTest.SuspendLayout();
            gbxConnectionString.SuspendLayout();
            GroupBox2.SuspendLayout();
            GroupBox3.SuspendLayout();
            gbxOtherStuff.SuspendLayout();
            SuspendLayout();
            //
            // txtTransFilePath
            //
            txtTransFilePath.Location = new Point(12, 22);
            txtTransFilePath.Name = "txtTransFilePath";
            txtTransFilePath.Size = new Size(590, 20);
            txtTransFilePath.TabIndex = 0;
            txtTransFilePath.Text = @"C:\Documents and Settings\d3k857\My Documents\Visual Studio Projects\Organism Dat" + @"abase Handler\Aux_Files\gc.ptr";
            //
            // cmdLoadTT
            //
            cmdLoadTT.FlatStyle = FlatStyle.System;
            cmdLoadTT.Location = new Point(694, 20);
            cmdLoadTT.Name = "cmdLoadTT";
            cmdLoadTT.TabIndex = 1;
            cmdLoadTT.Text = "Load";
            //
            // cmdBrowseTT
            //
            cmdBrowseTT.FlatStyle = FlatStyle.System;
            cmdBrowseTT.Location = new Point(612, 20);
            cmdBrowseTT.Name = "cmdBrowseTT";
            cmdBrowseTT.TabIndex = 3;
            cmdBrowseTT.Text = "Browse...";
            //
            // gbxTransTableImportTest
            //
            gbxTransTableImportTest.Controls.Add(cmdBrowseTT);
            gbxTransTableImportTest.Controls.Add(txtTransFilePath);
            gbxTransTableImportTest.Controls.Add(cmdLoadTT);
            gbxTransTableImportTest.FlatStyle = FlatStyle.System;
            gbxTransTableImportTest.Location = new Point(8, 68);
            gbxTransTableImportTest.Name = "gbxTransTableImportTest";
            gbxTransTableImportTest.Size = new Size(782, 54);
            gbxTransTableImportTest.TabIndex = 5;
            gbxTransTableImportTest.TabStop = false;
            gbxTransTableImportTest.Text = "Translation Table Import Test";
            //
            // gbxFASTAImportTest
            //
            gbxFASTAImportTest.Controls.Add(cmdBrowseFF);
            gbxFASTAImportTest.Controls.Add(cmdLoadFF);
            gbxFASTAImportTest.Controls.Add(txtFASTAFilePath);
            gbxFASTAImportTest.FlatStyle = FlatStyle.System;
            gbxFASTAImportTest.Location = new Point(8, 126);
            gbxFASTAImportTest.Name = "gbxFASTAImportTest";
            gbxFASTAImportTest.Size = new Size(780, 54);
            gbxFASTAImportTest.TabIndex = 6;
            gbxFASTAImportTest.TabStop = false;
            gbxFASTAImportTest.Text = "FASTA File Import Test";
            //
            // cmdBrowseFF
            //
            cmdBrowseFF.FlatStyle = FlatStyle.System;
            cmdBrowseFF.Location = new Point(614, 20);
            cmdBrowseFF.Name = "cmdBrowseFF";
            cmdBrowseFF.TabIndex = 5;
            cmdBrowseFF.Text = "Browse...";
            //
            // cmdLoadFF
            //
            cmdLoadFF.FlatStyle = FlatStyle.System;
            cmdLoadFF.Location = new Point(696, 20);
            cmdLoadFF.Name = "cmdLoadFF";
            cmdLoadFF.TabIndex = 4;
            cmdLoadFF.Text = "Load";
            //
            // txtFASTAFilePath
            //
            txtFASTAFilePath.Location = new Point(12, 22);
            txtFASTAFilePath.Name = "txtFASTAFilePath";
            txtFASTAFilePath.Size = new Size(590, 20);
            txtFASTAFilePath.TabIndex = 0;
            txtFASTAFilePath.Text = @"D:\Org_DB\Shewanella\FASTA\Shewanella_Heme_proteins_2003-11-19.fasta";
            //
            // txtConnString
            //
            txtConnString.Location = new Point(12, 22);
            txtConnString.Name = "txtConnString";
            txtConnString.Size = new Size(590, 20);
            txtConnString.TabIndex = 7;
            txtConnString.Text = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSP" + "I;";
            //
            // gbxConnectionString
            //
            gbxConnectionString.Controls.Add(txtConnString);
            gbxConnectionString.FlatStyle = FlatStyle.System;
            gbxConnectionString.Location = new Point(8, 10);
            gbxConnectionString.Name = "gbxConnectionString";
            gbxConnectionString.Size = new Size(780, 54);
            gbxConnectionString.TabIndex = 8;
            gbxConnectionString.TabStop = false;
            gbxConnectionString.Text = "Connection String";
            //
            // GroupBox2
            //
            GroupBox2.Controls.Add(Button2);
            GroupBox2.Controls.Add(TextBox1);
            GroupBox2.FlatStyle = FlatStyle.System;
            GroupBox2.Location = new Point(8, 188);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new Size(780, 54);
            GroupBox2.TabIndex = 6;
            GroupBox2.TabStop = false;
            GroupBox2.Text = "Translation Table Import Test";
            //
            // Button2
            //
            Button2.FlatStyle = FlatStyle.System;
            Button2.Location = new Point(696, 20);
            Button2.Name = "Button2";
            Button2.TabIndex = 4;
            Button2.Text = "Load";
            //
            // TextBox1
            //
            TextBox1.Location = new Point(12, 22);
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new Size(590, 20);
            TextBox1.TabIndex = 0;
            TextBox1.Text = "1";
            //
            // GroupBox3
            //
            GroupBox3.Controls.Add(cmdBatchLoadDMS);
            GroupBox3.Controls.Add(cboCollectionsList);
            GroupBox3.Controls.Add(cmdExportFASTA);
            GroupBox3.FlatStyle = FlatStyle.System;
            GroupBox3.Location = new Point(8, 248);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Size = new Size(780, 54);
            GroupBox3.TabIndex = 7;
            GroupBox3.TabStop = false;
            GroupBox3.Text = "FASTA File Import Test";
            //
            // cmdBatchLoadDMS
            //
            cmdBatchLoadDMS.FlatStyle = FlatStyle.System;
            cmdBatchLoadDMS.Location = new Point(612, 20);
            cmdBatchLoadDMS.Name = "cmdBatchLoadDMS";
            cmdBatchLoadDMS.TabIndex = 9;
            cmdBatchLoadDMS.Text = "Batch Load";
            //
            // cboCollectionsList
            //
            cboCollectionsList.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCollectionsList.Location = new Point(12, 22);
            cboCollectionsList.Name = "cboCollectionsList";
            cboCollectionsList.Size = new Size(590, 21);
            cboCollectionsList.TabIndex = 5;
            //
            // cmdExportFASTA
            //
            cmdExportFASTA.FlatStyle = FlatStyle.System;
            cmdExportFASTA.Location = new Point(696, 20);
            cmdExportFASTA.Name = "cmdExportFASTA";
            cmdExportFASTA.TabIndex = 4;
            cmdExportFASTA.Text = "Export";
            //
            // gbxOtherStuff
            //
            gbxOtherStuff.Controls.Add(cmdUpdateArchiveTables);
            gbxOtherStuff.Controls.Add(Button3);
            gbxOtherStuff.FlatStyle = FlatStyle.System;
            gbxOtherStuff.Location = new Point(8, 308);
            gbxOtherStuff.Name = "gbxOtherStuff";
            gbxOtherStuff.Size = new Size(780, 54);
            gbxOtherStuff.TabIndex = 9;
            gbxOtherStuff.TabStop = false;
            gbxOtherStuff.Text = "Miscellaneous";
            //
            // cmdUpdateArchiveTables
            //
            cmdUpdateArchiveTables.FlatStyle = FlatStyle.System;
            cmdUpdateArchiveTables.Location = new Point(566, 18);
            cmdUpdateArchiveTables.Name = "cmdUpdateArchiveTables";
            cmdUpdateArchiveTables.Size = new Size(120, 23);
            cmdUpdateArchiveTables.TabIndex = 9;
            cmdUpdateArchiveTables.Text = "Sync Archive Tables";
            //
            // Button3
            //
            Button3.FlatStyle = FlatStyle.System;
            Button3.Location = new Point(696, 18);
            Button3.Name = "Button3";
            Button3.TabIndex = 4;
            Button3.Text = "Export";
            //
            // lblProgress
            //
            lblProgress.Location = new Point(8, 368);
            lblProgress.Name = "lblProgress";
            lblProgress.Size = new Size(780, 16);
            lblProgress.TabIndex = 11;
            lblProgress.Text = "No Progress Status Yet";
            //
            // pgbAdminConsole
            //
            pgbAdminConsole.Location = new Point(8, 382);
            pgbAdminConsole.Name = "pgbAdminConsole";
            pgbAdminConsole.Size = new Size(780, 20);
            pgbAdminConsole.TabIndex = 10;
            pgbAdminConsole.Visible = false;
            //
            // frmTestingInterface
            //
            AutoScaleBaseSize = new Size(5, 13);
            ClientSize = new Size(796, 408);
            Controls.Add(lblProgress);
            Controls.Add(pgbAdminConsole);
            Controls.Add(gbxOtherStuff);
            Controls.Add(gbxConnectionString);
            Controls.Add(gbxFASTAImportTest);
            Controls.Add(gbxTransTableImportTest);
            Controls.Add(GroupBox2);
            Controls.Add(GroupBox3);
            Name = "frmTestingInterface";
            Text = "-";
            gbxTransTableImportTest.ResumeLayout(false);
            gbxFASTAImportTest.ResumeLayout(false);
            gbxConnectionString.ResumeLayout(false);
            GroupBox2.ResumeLayout(false);
            GroupBox3.ResumeLayout(false);
            gbxOtherStuff.ResumeLayout(false);
            ResumeLayout(false);
        }


        #endregion

        private ImportHandler importer;
        private DataTable collectionList;
        private string m_LastOutputDirectory = @"D:\outbox\output_test\";
        private string m_AppPath = Application.ExecutablePath;

        protected SyncFASTAFileArchive m_Syncer;
        protected GetFASTAFromDMS m_Exporter;

        protected string m_FullOutputPath;

        private string m_TaskMessage;
        private string m_ProgressMessage;

        //private ExportCollectionsFromDMS.IExportCollectionsFromDMS exporter;

        private void frmTestingInterface_Load(object sender, EventArgs e)
        {
            importer = new ImportHandler(txtConnString.Text);
            collectionList = importer.LoadProteinCollections();
            cboCollectionsList.DataSource = collectionList;
            cboCollectionsList.DisplayMember = "Display";
            cboCollectionsList.ValueMember = "Protein_Collection_ID";
        }

        private void cmdLoadTT_Click(object sender, EventArgs e)
        {
            var transHandler = new TransTableHandler(txtConnString.Text);
            transHandler.GetAllTranslationTableEntries(txtTransFilePath.Text);
        }

        private void cmdLoadFF_Click(object sender, EventArgs e)
        {
            var importHandler = new ImportHandler(txtConnString.Text);

            //importHandler.LoadProteins(txtFASTAFilePath.Text, "", Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA, 4, 1);

            //Protein_Importer.IReadFASTA FASTAHandler;
            //// FASTAHandler = new Protein_Importer.FASTAReader();
            //TableManipulationBase.DBTask sqlData;
            //sqlData = new TableManipulationBase.DBTask(txtConnString.Text, true);

            //string SQL = "SELECT * FROM " + "T_Proteins";
            //SqlClient.SqlDataAdapter dmsDA = new SqlClient.SqlDataAdapter(SQL, sqlData.Connection);
            //SqlClient.SqlCommandBuilder dmsCB = new SqlClient.SqlCommandBuilder(dmsDA);

            //FASTAHandler.ProteinTable = sqlData.GetTable(SQL, dmsDA, dmsCB);
            //FASTAHandler.GetProteinEntries(txtFASTAFilePath.Text);

            //foreach (DataRow dr in FASTAHandler.ProteinTable)
            //{
            //}

            //dmsDA.Update(FASTAHandler.ProteinTable);
        }

        private void cmdBrowseTT_Click(object sender, EventArgs e)
        {
            string newFilePath;
            var OpenDialog = new OpenFileDialog();

            Button proxy = (Button)sender;

            OpenDialog.Title = "Open Translation Definitions File";
            OpenDialog.DereferenceLinks = false;
            OpenDialog.InitialDirectory = @"D:\Org_DB\";
            OpenDialog.Filter = "All files (*.*)|*.*";
            OpenDialog.FilterIndex = 1;
            OpenDialog.RestoreDirectory = true;
            OpenDialog.Multiselect = false;

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                newFilePath = OpenDialog.FileName;
                if (proxy.Name.ToString() == "cmdBrowseTT")
                {
                    txtTransFilePath.Text = newFilePath;
                }
                else if (proxy.Name.ToString() == "cmdBrowseFF")
                {
                    txtFASTAFilePath.Text = newFilePath;
                }
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            var trans = new TranslateNucleotides(txtConnString.Text);

            trans.LoadTransMatrix(1);
        }

        private void cmdExportFASTA_Click(object sender, EventArgs e)
        {
            var sd = new FolderBrowserDialog();

            string filePath;
            DialogResult r;

            //Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS exporter;
            //var tmpNameList = new ArrayList();

            //sd.DefaultExt = ".fasta";
            //sd.FileName = cboCollectionsList.Text + ".fasta";

            sd.SelectedPath = m_LastOutputDirectory;

            r = sd.ShowDialog();

            if (r == DialogResult.OK)
            {
                filePath = sd.SelectedPath;
                m_LastOutputDirectory = filePath;

                //tmpNameList.Add(cboCollectionsList.Text.ToString());
                //exporter = new Protein_Exporter.GetFASTAFromDMS(txtConnString.Text, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward_sequence);
                //exporter = new Protein_Exporter.GetFASTAFromDMS(
                //    txtConnString.Text,
                //    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA,
                //    GetCollectionName(CInt(cboCollectionsList.SelectedValue)) + "_scrambled.fasta");

                //m_Exporter = New Protein_Exporter.GetFASTAFromDMS(
                //    txtConnString.Text,
                //    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                //    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward);
                m_Exporter = new GetFASTAFromDMS(txtConnString.Text);
                m_Exporter.FileGenerationStarted += StartTask;
                m_Exporter.FileGenerationProgress += UpdateProgress;
                m_Exporter.FileGenerationCompleted += CompletedTask;

                //True Legacy fasta file
                //fingerprint = m_Exporter.ExportFASTAFile("na", "na", "HCMV_2003+H_sapiens_IPI_2005-04-04.fasta", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = m_Exporter.ExportFASTAFile("Shewanella_2003-12-19", "seq_direction=forward,filetype=fasta", "Shewanella_2003-12-19.fasta", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = m_Exporter.ExportFASTAFile("M_Musculus_2007-10-24_IPI,Y_pestis_CO92_2006-05-22,Y_pestis_PestoidesF_2006-05-23,Y_pseudotuberculosis_All_2005-08-25", "seq_direction=forward,filetype=fasta", "na", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = m_Exporter.ExportFASTAFile("H_sapiens_IPI_2008-02-07", "seq_direction=decoy", "na", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = m_Exporter.ExportFASTAFile("na", "na", "Shewanella_2003-12-19.fasta", filePath)
                m_Exporter.ExportFASTAFile("na", "na", "GOs_Surface_Sargasso_Meso_2009-02-11_24.fasta", filePath);

                //Collection of existing collections
                //fingerprint = m_Exporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=forward,filetype=fasta", "", filePath)
                m_Exporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=reversed,filetype=fasta", "", filePath);

                //fingerprint = m_Exporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=decoy,filetype=fasta", "", filePath)
                m_Exporter.ExportFASTAFile("SAR116_RBH_AA_012809", "seq_direction=forward,filetype=fasta", "", filePath);

                m_Exporter.ExportFASTAFile("Phycomyces_blakesleeanus_v2_filtered_2009-12-16", "seq_direction=forward,filetype=fasta", "", filePath);

                //Protein collection from cbo exported forward
                //fingerprint = m_Exporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=forward,filetype=fasta", "na", filePath)

                //Protein Collection from cbo exported reversed
                //fingerprint = m_Exporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=reversed,filetype=fasta", "na", filePath)

                //Protein Collection from cbo exported scrambled
                //fingerprint = m_Exporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=scrambled,filetype=fasta", "na", filePath)

                //fingerprint = exporter.ExportFASTAFile(CInt(cboCollectionsList.SelectedValue), filePath)
                //exporter = New ExportCollectionsFromDMS.clsExportCollectionsFromDMS(txtConnString.Text, ExportCollectionsFromDMS.IExportCollectionsFromDMS.ExportClasses.ExportProteinsXTFASTA)
                //fingerprint = exporter.Export(CInt(cboCollectionsList.SelectedValue), filePath)

                //var outputFI = new System.IO.FileInfo(m_FullOutputPath);
                //var destPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(outputFI.FullName), GetCollectionName(CInt(cboCollectionsList.SelectedValue)) + ".fasta");
                //var destFI = new System.IO.FileInfo(destPath);
                //if (destFI.Exists)
                //    destFI.Delete();
                //outputFI.MoveTo(destPath);

                m_Exporter.FileGenerationStarted -= StartTask;
                m_Exporter.FileGenerationProgress -= UpdateProgress;
                m_Exporter.FileGenerationCompleted -= CompletedTask;
            }
        }

        private string GetCollectionName(int ProteinCollectionID)
        {
            DataRow[] foundRows;
            foundRows = collectionList.Select("Protein_Collection_ID = " + ProteinCollectionID.ToString());
            return foundRows[0]["FileName"].ToString();
        }

        private void cmdBatchLoadDMS_Click(object sender, EventArgs e)
        {
            var fileBatcher = new BatchUploadFromFileList(txtConnString.Text);
            fileBatcher.UploadBatch();
        }

        private void cmdBatchLoadDMS_Click_1(object sender, EventArgs e)
        {
        }

        private void cmdUpdateArchiveTables_Click(object sender, EventArgs e)
        {
            if (m_Syncer == null)
            {
                m_Syncer = new SyncFASTAFileArchive(txtConnString.Text);
                m_Syncer.SyncStart += StartTask;
                m_Syncer.SyncProgress += UpdateProgress;
                m_Syncer.SyncComplete += CompletedTask;
            }

            //string outputPath = "";
            //var f = new FolderBrowserDialog();
            //f.RootFolder = Environment.SpecialFolder.MyComputer;
            //f.ShowNewFolderButton = true;

            //var r = f.ShowDialog();

            //if (r == DialogResult.OK)
            //    outputPath = f.SelectedPath;

            //int errorCode = m_Syncer.SyncCollectionsAndArchiveTables(outputPath);
            m_Syncer.UpdateSHA1Hashes();
        }

        private void StartTask(string statusMsg)
        {
            pgbAdminConsole.Visible = true;
            lblProgress.Visible = true;

            pgbAdminConsole.Value = 0;

            m_TaskMessage = statusMsg;
            lblProgress.Text = m_TaskMessage;
            Application.DoEvents();
        }

        private void UpdateProgress(string statusMsg, double fractionDone)
        {
            m_ProgressMessage = statusMsg;
            int percentComplete = (int)Math.Round(fractionDone * 100d);
            if (fractionDone > 0d)
            {
                pgbAdminConsole.Value = (int)Math.Round(fractionDone * 100d);
                lblProgress.Text = m_TaskMessage + " (" + percentComplete + "% completed): " + m_ProgressMessage;
            }
            else
            {
                lblProgress.Text = m_TaskMessage + ": " + m_ProgressMessage;
            }

            Application.DoEvents();
        }

        private void CompletedTask()
        {
            pgbAdminConsole.Visible = false;
            lblProgress.Text = "";
            lblProgress.Visible = false;
        }

        private void CompletedTask(string fullOutputPath)
        {
            pgbAdminConsole.Visible = false;
            lblProgress.Text =
                "Wrote " +
                Path.GetFileName(fullOutputPath) +
                " to " + Path.GetDirectoryName(fullOutputPath);
            lblProgress.Visible = true;
            m_FullOutputPath = fullOutputPath;
        }
    }
}