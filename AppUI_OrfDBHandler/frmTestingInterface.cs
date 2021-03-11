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
        private string mLastOutputDirectory = @"D:\outbox\output_test\";
        private string mAppPath = Application.ExecutablePath;

        private SyncFASTAFileArchive mSyncer;
        private GetFASTAFromDMS mExporter;

        private string mFullOutputPath;

        private string mTaskMessage;
        private string mProgressMessage;

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
            var openDialog = new OpenFileDialog();

            var proxy = (Button)sender;

            openDialog.Title = "Open Translation Definitions File";
            openDialog.DereferenceLinks = false;
            openDialog.InitialDirectory = @"D:\Org_DB\";
            openDialog.Filter = "All files (*.*)|*.*";
            openDialog.FilterIndex = 1;
            openDialog.RestoreDirectory = true;
            openDialog.Multiselect = false;

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                var newFilePath = openDialog.FileName;
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
            //Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS exporter;
            //var tmpNameList = new ArrayList();

            var sd = new FolderBrowserDialog
            {
                //DefaultExt = ".fasta",
                //FileName = cboCollectionsList.Text + ".fasta",
                SelectedPath = mLastOutputDirectory,
            };

            var r = sd.ShowDialog();

            if (r == DialogResult.OK)
            {
                var filePath = sd.SelectedPath;
                mLastOutputDirectory = filePath;

                //tmpNameList.Add(cboCollectionsList.Text.ToString());
                //exporter = new Protein_Exporter.GetFASTAFromDMS(txtConnString.Text, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward_sequence);
                //exporter = new Protein_Exporter.GetFASTAFromDMS(
                //    txtConnString.Text,
                //    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.FASTA,
                //    GetCollectionName(CInt(cboCollectionsList.SelectedValue)) + "_scrambled.fasta");

                //mExporter = New Protein_Exporter.GetFASTAFromDMS(
                //    txtConnString.Text,
                //    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                //    Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward);
                mExporter = new GetFASTAFromDMS(txtConnString.Text);
                mExporter.FileGenerationStarted += StartTask;
                mExporter.FileGenerationProgress += UpdateProgress;
                mExporter.FileGenerationCompleted += CompletedTask;

                //True Legacy fasta file
                //fingerprint = mExporter.ExportFASTAFile("na", "na", "HCMV_2003+H_sapiens_IPI_2005-04-04.fasta", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("Shewanella_2003-12-19", "seq_direction=forward,filetype=fasta", "Shewanella_2003-12-19.fasta", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("M_Musculus_2007-10-24_IPI,Y_pestis_CO92_2006-05-22,Y_pestis_PestoidesF_2006-05-23,Y_pseudotuberculosis_All_2005-08-25", "seq_direction=forward,filetype=fasta", "na", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("H_sapiens_IPI_2008-02-07", "seq_direction=decoy", "na", filePath)

                //Legacy fasta file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("na", "na", "Shewanella_2003-12-19.fasta", filePath)
                mExporter.ExportFASTAFile("na", "na", "GOs_Surface_Sargasso_Meso_2009-02-11_24.fasta", filePath);

                //Collection of existing collections
                //fingerprint = mExporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=forward,filetype=fasta", "", filePath)
                mExporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=reversed,filetype=fasta", "", filePath);

                //fingerprint = mExporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=decoy,filetype=fasta", "", filePath)
                mExporter.ExportFASTAFile("SAR116_RBH_AA_012809", "seq_direction=forward,filetype=fasta", "", filePath);

                mExporter.ExportFASTAFile("Phycomyces_blakesleeanus_v2_filtered_2009-12-16", "seq_direction=forward,filetype=fasta", "", filePath);

                //Protein collection from cbo exported forward
                //fingerprint = mExporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=forward,filetype=fasta", "na", filePath)

                //Protein Collection from cbo exported reversed
                //fingerprint = mExporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=reversed,filetype=fasta", "na", filePath)

                //Protein Collection from cbo exported scrambled
                //fingerprint = mExporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=scrambled,filetype=fasta", "na", filePath)

                //fingerprint = exporter.ExportFASTAFile(CInt(cboCollectionsList.SelectedValue), filePath)
                //exporter = New ExportCollectionsFromDMS.clsExportCollectionsFromDMS(txtConnString.Text, ExportCollectionsFromDMS.IExportCollectionsFromDMS.ExportClasses.ExportProteinsXTFASTA)
                //fingerprint = exporter.Export(CInt(cboCollectionsList.SelectedValue), filePath)

                //var outputFI = new System.IO.FileInfo(mFullOutputPath);
                //var destPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(outputFI.FullName), GetCollectionName(CInt(cboCollectionsList.SelectedValue)) + ".fasta");
                //var destFI = new System.IO.FileInfo(destPath);
                //if (destFI.Exists)
                //    destFI.Delete();
                //outputFI.MoveTo(destPath);

                mExporter.FileGenerationStarted -= StartTask;
                mExporter.FileGenerationProgress -= UpdateProgress;
                mExporter.FileGenerationCompleted -= CompletedTask;
            }
        }

        private string GetCollectionName(int proteinCollectionId)
        {
            var foundRows = collectionList.Select("Protein_Collection_ID = " + proteinCollectionId.ToString());
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
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(txtConnString.Text);
                mSyncer.SyncStart += StartTask;
                mSyncer.SyncProgress += UpdateProgress;
                mSyncer.SyncComplete += CompletedTask;
            }

            //string outputPath = "";
            //var f = new FolderBrowserDialog();
            //f.RootFolder = Environment.SpecialFolder.MyComputer;
            //f.ShowNewFolderButton = true;

            //var r = f.ShowDialog();

            //if (r == DialogResult.OK)
            //    outputPath = f.SelectedPath;

            //int errorCode = mSyncer.SyncCollectionsAndArchiveTables(outputPath);
            mSyncer.UpdateSHA1Hashes();
        }

        private void StartTask(string statusMsg)
        {
            pgbAdminConsole.Visible = true;
            lblProgress.Visible = true;

            pgbAdminConsole.Value = 0;

            mTaskMessage = statusMsg;
            lblProgress.Text = mTaskMessage;
            Application.DoEvents();
        }

        private void UpdateProgress(string statusMsg, double fractionDone)
        {
            mProgressMessage = statusMsg;
            var percentComplete = (int)Math.Round(fractionDone * 100d);
            if (fractionDone > 0d)
            {
                pgbAdminConsole.Value = (int)Math.Round(fractionDone * 100d);
                lblProgress.Text = mTaskMessage + " (" + percentComplete + "% completed): " + mProgressMessage;
            }
            else
            {
                lblProgress.Text = mTaskMessage + ": " + mProgressMessage;
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
            mFullOutputPath = fullOutputPath;
        }
    }
}
