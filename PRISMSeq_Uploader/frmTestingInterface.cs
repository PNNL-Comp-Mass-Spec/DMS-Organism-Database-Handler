﻿using System;
using System.Data;
using System.IO;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinExport;
using OrganismDatabaseHandler.ProteinImport;
using PRISMDatabaseUtils;

namespace PRISMSeq_Uploader
{
    public partial class frmTestingInterface : Form
    {
        // Ignore Spelling: fasta, filetype, frm, na, outbox

        public frmTestingInterface()
        {
            base.Load += frmTestingInterface_Load;

            InitializeComponent();
        }

        private ImportHandler importer;
        private DataTable collectionList;
        private string mLastOutputDirectory = @"D:\outbox\output_test\";

#pragma warning disable CS0618 // Type or member is obsolete
        private SyncFASTAFileArchive mSyncer;
#pragma warning restore CS0618 // Type or member is obsolete

        private GetFASTAFromDMS mExporter;

        private string mTaskMessage;
        private string mProgressMessage;

        //private ExportCollectionsFromDMS.IExportCollectionsFromDMS exporter;

        private void frmTestingInterface_Load(object sender, EventArgs e)
        {
            importer = new ImportHandler(txtConnString.Text);
            collectionList = importer.LoadProteinCollections();
            cboCollectionsList.DataSource = collectionList;
            cboCollectionsList.DisplayMember = "display";
            cboCollectionsList.ValueMember = "protein_collection_id";
        }

        private void cmdLoadTT_Click(object sender, EventArgs e)
        {
            var transHandler = new TransTableHandler(txtConnString.Text);
            transHandler.GetAllTranslationTableEntries(txtTransFilePath.Text);
        }

        private void cmdLoadFF_Click(object sender, EventArgs e)
        {
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(txtConnString.Text, "PRISMSeq_Uploader");

            var importHandler = new ImportHandler(connectionStringToUse);

            //importHandler.LoadProteins(txtFASTAFilePath.Text, "", Protein_Importer.IImportProteins.ProteinImportFileTypes.FASTA, 4, 1);

            //Protein_Importer.IReadFASTA FASTAHandler;
            //// FASTAHandler = new Protein_Importer.FASTAReader();
            //TableManipulationBase.DBTask sqlData;
            //sqlData = new TableManipulationBase.DBTask(connectionStringToUse, true);

            //string SQL = "SELECT * FROM " + "T_Proteins";
            //SqlClient.SqlDataAdapter dmsDA = new SqlClient.SqlDataAdapter(SQL, sqlData.Connection);
            //SqlClient.SqlCommandBuilder dmsCB = new SqlClient.SqlCommandBuilder(dmsDA);

            //FASTAHandler.ProteinTable = sqlData.GetTable(SQL, dmsDA, dmsCB);
            //FASTAHandler.GetProteinEntries(txtFASTAFilePath.Text);

            //for each (DataRow dataRow in FASTAHandler.ProteinTable)
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

                if (proxy.Name == "cmdBrowseTT")
                {
                    txtTransFilePath.Text = newFilePath;
                }
                else if (proxy.Name == "cmdBrowseFF")
                {
                    txtFASTAFilePath.Text = newFilePath;
                }
            }
        }

        // Unused
        //private void Button2_Click(object sender, EventArgs e)
        //{
        //    var trans = new TranslateNucleotides(txtConnString.Text);

        //    trans.LoadTransMatrix(1);
        //}

        private void cmdExportFASTA_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                //DefaultExt = ".fasta",
                //FileName = cboCollectionsList.Text + ".fasta",
                SelectedPath = mLastOutputDirectory,
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var filePath = dialog.SelectedPath;
                mLastOutputDirectory = filePath;

                mExporter = new GetFASTAFromDMS(txtConnString.Text);
                mExporter.FileGenerationStarted += StartTask;
                mExporter.FileGenerationProgress += UpdateProgress;
                mExporter.FileGenerationCompleted += CompletedTask;

                //True Legacy FASTA file
                //fingerprint = mExporter.ExportFASTAFile("na", "na", "HCMV_2003+H_sapiens_IPI_2005-04-04.fasta", filePath)

                //Legacy FASTA file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("Shewanella_2003-12-19", "seq_direction=forward,filetype=fasta", "Shewanella_2003-12-19.fasta", filePath)

                //Legacy FASTA file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("M_Musculus_2007-10-24_IPI,Y_pestis_CO92_2006-05-22,Y_pestis_PestoidesF_2006-05-23,Y_pseudotuberculosis_All_2005-08-25", "seq_direction=forward,filetype=fasta", "na", filePath)

                //Legacy FASTA file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("H_sapiens_IPI_2008-02-07", "seq_direction=decoy", "na", filePath)

                //Legacy FASTA file with existing protein collection
                //fingerprint = mExporter.ExportFASTAFile("na", "na", "Shewanella_2003-12-19.fasta", filePath)
                mExporter.ExportFASTAFile("na", "na", "GOs_Surface_Sargasso_Meso_2009-02-11_24.fasta", filePath);

                //Collection of existing collections
                //fingerprint = mExporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=forward,filetype=fasta", "", filePath)
                mExporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=reversed,filetype=fasta", "", filePath);

                //fingerprint = mExporter.ExportFASTAFile("6_protein_Standard_2009-02-20,H_sapiens_IPI_2006-08-22", "seq_direction=decoy,filetype=fasta", "", filePath)
                mExporter.ExportFASTAFile("SAR116_RBH_AA_012809", "seq_direction=forward,filetype=fasta", "", filePath);

                mExporter.ExportFASTAFile("Phycomyces_blakesleeanus_v2_filtered_2009-12-16", "seq_direction=forward,filetype=fasta", "", filePath);

                //Protein collection from ComboBox exported forward
                //fingerprint = mExporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=forward,filetype=fasta", "na", filePath)

                //Protein Collection from ComboBox exported reversed
                //fingerprint = mExporter.ExportFASTAFile(GetCollectionName(CInt(cboCollectionsList.SelectedValue)), "seq_direction=reversed,filetype=fasta", "na", filePath)

                //Protein Collection from ComboBox exported scrambled
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

        private void cmdBatchLoadDMS_Click_1(object sender, EventArgs e)
        {
        }

        [Obsolete("Referenced by hidden button")]
        private void cmdUpdateArchiveTables_Click(object sender, EventArgs e)
        {
            if (mSyncer == null)
            {
                mSyncer = new SyncFASTAFileArchive(txtConnString.Text);
                mSyncer.SyncStart += StartTask;
                mSyncer.SyncProgress += UpdateProgress;
                mSyncer.SyncComplete += CompletedTask;
            }

            //string outputPath = string.Empty;
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
            lblProgress.Text = string.Empty;
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
        }
    }
}
