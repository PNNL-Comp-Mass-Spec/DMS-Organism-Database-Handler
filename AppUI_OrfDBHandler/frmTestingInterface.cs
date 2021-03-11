using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using AppUI_OrfDBHandler.NucleotideTranslator;
using AppUI_OrfDBHandler.ProteinUpload;
using OrganismDatabaseHandler.ProteinExport;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public partial class frmTestingInterface : Form
    {
        public frmTestingInterface()
        {
            base.Load += frmTestingInterface_Load;

            InitializeComponent();
        }

        private ImportHandler importer;
        private DataTable collectionList;
        private string m_LastOutputDirectory = @"D:\outbox\output_test\";
        private string m_AppPath = Application.ExecutablePath;

        private SyncFASTAFileArchive m_Syncer;
        private GetFASTAFromDMS m_Exporter;

        private string m_FullOutputPath;

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
