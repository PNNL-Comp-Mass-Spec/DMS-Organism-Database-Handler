using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinExport;
using OrganismDatabaseHandler.ProteinImport;
using PRISM;
using PRISMDatabaseUtils;
using ProteinFileReader;

namespace AppUI_OrfDBHandler
{
    [Obsolete("Referenced by hidden button and hidden menu")]
    public class SyncFASTAFileArchive : EventNotifier
    {
        // Ignore Spelling: filetype

        private readonly DBTask mDatabaseAccessor;
        private readonly AddUpdateEntries mImporter;
        private GetFASTAFromDMS mExporter;

        public event SyncStartEventHandler SyncStart;
        public event SyncProgressEventHandler SyncProgress;
        public event SyncCompleteEventHandler SyncComplete;

        private string mCurrentStatusMsg;
        private int mCurrentProteinCount;
        private int mTotalProteinsCount;

        private string mGeneratedFastaFilePath;

        public SyncFASTAFileArchive(string psConnectionString)
        {
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(psConnectionString, "PRISMSeq_Uploader");

            mDatabaseAccessor = new DBTask(connectionStringToUse);
            RegisterEvents(mDatabaseAccessor);

            mImporter = new AddUpdateEntries(connectionStringToUse);
            RegisterEvents(mImporter);
        }

        [Obsolete("Unused")]
        public int SyncCollectionsAndArchiveTables(string outputPath)
        {
            const string sql =
                "SELECT Protein_Collection_ID, Collection_Name, Authentication_Hash, DateModified, Collection_Type_ID, NumProteins " +
                "FROM V_Missing_Archive_Entries";

            // TODO add collection list string
            const string proteinCollectionList = "";

            var dt = mDatabaseAccessor.GetTable(sql);
            const string CreationOptionsString = "seq_direction=forward,filetype=fasta";

            var currentCollectionProteinCount = 0;
            foreach (DataRow dataRow in dt.Rows)
            {
                mTotalProteinsCount += Convert.ToInt32(dataRow["NumProteins"]);
            }

            const GetFASTAFromDMS.SequenceTypes outputSequenceType = GetFASTAFromDMS.SequenceTypes.Forward;

            OnSyncStart("Synchronizing Archive Table with Collections Table");

            var fileArchiver = new ArchiveToFile(mDatabaseAccessor, mExporter);

            foreach (DataRow dataRow in dt.Rows)
            {
                OnSyncProgressUpdate("Processing - '" + dataRow["Collection_Name"] + "'", currentCollectionProteinCount / (double)mTotalProteinsCount);
                currentCollectionProteinCount = Convert.ToInt32(dataRow["NumProteins"]);
                var proteinCollectionId = Convert.ToInt32(dataRow["Protein_Collection_ID"]);
                var sourceFilePath = Path.Combine(outputPath, dataRow["Collection_Name"] + ".fasta");
                var sha1 = dataRow["Authentication_Hash"].ToString();

                fileArchiver.ArchiveCollection(
                    proteinCollectionId,
                    ArchiveOutputFilesBase.CollectionTypes.Static,
                    outputSequenceType, sourceFilePath, CreationOptionsString, sha1, proteinCollectionList);
            }

            OnSyncCompletion();

            return 0;
        }

        [Obsolete("Referenced by hidden button")]
        public void UpdateSHA1Hashes()
        {
            const string sql = "SELECT Protein_Collection_ID, Collection_Name, Authentication_Hash, NumProteins " +
                      "FROM V_Missing_Archive_Entries";

            var dt = mDatabaseAccessor.GetTable(sql);

            foreach (DataRow dataRow in dt.Rows)
            {
                mTotalProteinsCount += Convert.ToInt32(dataRow["NumProteins"]);
            }

            var elapsedTimeSb = new StringBuilder();

            var tempFilePath = Path.GetTempPath();

            string connectionString;
            if (mDatabaseAccessor == null || string.IsNullOrWhiteSpace(mDatabaseAccessor.ConnectionString))
            {
                connectionString = string.Empty;
            }
            else
            {
                connectionString = mDatabaseAccessor.ConnectionString;
            }

            mExporter = new GetFASTAFromDMS(connectionString, GetFASTAFromDMS.SequenceTypes.Forward);
            mExporter.FileGenerationCompleted += Exporter_FileGenerationCompleted;

            const string creationOptionsString = "seq_direction=forward,filetype=fasta";
            OnSyncStart("Updating Collections and Archive Entries");
            var startTime = DateTime.UtcNow;

            var fileArchiver = new ArchiveToFile(mDatabaseAccessor, mExporter);

            foreach (DataRow dataRow in dt.Rows)
            {
                var proteinCollectionID = Convert.ToInt32(dataRow["Protein_Collection_ID"]);
                var storedSHA = dataRow["Authentication_Hash"].ToString();
                var collectionName = dataRow["Collection_Name"].ToString();
                mGeneratedFastaFilePath = string.Empty;

                elapsedTimeSb.Remove(0, elapsedTimeSb.Length);
                var elapsedTime = DateTime.UtcNow.Subtract(startTime);
                if (elapsedTime.Minutes < 1 && elapsedTime.Hours == 0)
                {
                    elapsedTimeSb.Append("less than ");
                }
                else
                {
                    elapsedTimeSb.Append("about ");
                }

                if (elapsedTime.Hours > 0)
                {
                    elapsedTimeSb.AppendFormat("{0} hours, ", elapsedTime.Hours);
                }

                if (elapsedTime.Minutes <= 1)
                {
                    elapsedTimeSb.Append("1 minute");
                }
                else
                {
                    elapsedTimeSb.AppendFormat("{0} minutes", elapsedTime.Minutes);
                }

                // OnSyncProgressUpdate(
                //     "Collection "
                //     + Format(proteinCollectionID, "0000")
                //     + " [Elapsed Time: "
                //     + elapsedTimeSB.ToString() + "]",
                //     currentProteinCount / (double)totalProteinCount)
                mCurrentStatusMsg = "Collection " + proteinCollectionID.ToString("0000") + " [Elapsed Time: " + elapsedTimeSb + "]";

                OnSyncProgressUpdate(
                    mCurrentStatusMsg,
                    mCurrentProteinCount / (double)mTotalProteinsCount);

                var fullPath = Path.Combine(tempFilePath, collectionName + ".fasta");

                var genSHA = mExporter.ExportFASTAFile(proteinCollectionID, fullPath, GetFASTAFromDMS.SequenceTypes.Forward);

                if (!storedSHA.Equals(genSHA))
                {
                    var currentFastaProteinCount = 0;
                    var currentFastaResidueCount = 0;

                    if (!string.IsNullOrEmpty(mGeneratedFastaFilePath))
                    {
                        CountProteinsAndResidues(mGeneratedFastaFilePath, out currentFastaProteinCount, out currentFastaResidueCount);
                    }

                    mImporter.AddAuthenticationHash(proteinCollectionID, genSHA, currentFastaProteinCount, currentFastaResidueCount);
                }

                fileArchiver.ArchiveCollection(
                    proteinCollectionID,
                    ArchiveOutputFilesBase.CollectionTypes.Static,
                    GetFASTAFromDMS.SequenceTypes.Forward,
                    fullPath, creationOptionsString, genSHA, "");

                // ArchiveCollection(
                //     proteinCollectionID,
                //     IArchiveOutputFiles.CollectionTypes.static,
                //     fullPath, genSHA);

                // nameList.Clear();

                var fileToDelete = new FileInfo(fullPath);
                fileToDelete.Delete();

                mCurrentProteinCount += Convert.ToInt32(dataRow["NumProteins"]);
            }

            OnSyncCompletion();
        }

        private void CountProteinsAndResidues(string fastaFilePath, out int proteinCount, out int residueCount)
        {
            proteinCount = 0;
            residueCount = 0;

            var reader = new FastaFileReader();
            if (reader.OpenFile(fastaFilePath))
            {
                while (reader.ReadNextProteinEntry())
                {
                    proteinCount++;
                    residueCount += reader.ProteinSequence.Length;
                }
            }

            reader.CloseFile();
        }

        [Obsolete("Valid, but unused and could take a very long time")]
        public void RefreshNameHashes()
        {
            const string nameCountSQL = "SELECT TOP 1 Reference_ID FROM T_Protein_Names ORDER BY Reference_ID DESC";

            var nameCountResults = mDatabaseAccessor.GetTable(nameCountSQL);
            var totalNameCount = Convert.ToInt32(nameCountResults.Rows[0]["Reference_ID"]);

            var startIndex = 0;
            var stepValue = 10000;

            if (totalNameCount <= stepValue)
            {
                stepValue = totalNameCount;
            }

            OnSyncStart("Updating Name Hashes");

            var dbTools = mDatabaseAccessor.DbTools;

            for (var counter = stepValue; counter <= totalNameCount + stepValue; counter += stepValue)
            {
                if (counter >= totalNameCount - stepValue)
                {
                    Debug.WriteLine("");
                }

                var rowRetrievalSql = "SELECT Reference_ID, Name, Description, Protein_ID " +
                                      "FROM T_Protein_Names " +
                                      "WHERE Reference_ID > " + startIndex + " and Reference_ID <= " + counter +
                                      "ORDER BY Reference_ID";

                // Protein_Name(+"_" + Description + "_" + ProteinID.ToString)
                var proteinListResults = mDatabaseAccessor.GetTable(rowRetrievalSql);
                if (proteinListResults.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in proteinListResults.Rows)
                    {
                        var refId = dbTools.GetInteger(dataRow["Reference_ID"]);
                        var proteinName = dbTools.GetString(dataRow["Name"]);
                        var description = dbTools.GetString(dataRow["Description"]);
                        var proteinId = dbTools.GetInteger(dataRow["Protein_ID"]);

                        mImporter.UpdateProteinNameHash(
                            refId,
                            proteinName,
                            description,
                            proteinId);
                    }

                    OnSyncProgressUpdate("Processing " + startIndex + " to " + counter, counter / (double)totalNameCount);
                    startIndex = counter + 1;
                }
            }

            OnSyncCompletion();
        }

        private void OnSyncStart(string statusMsg)
        {
            SyncStart?.Invoke(statusMsg);
        }

        private void OnSyncProgressUpdate(string statusMsg, double fractionDone)
        {
            SyncProgress?.Invoke(statusMsg, fractionDone);
        }

        private void OnSyncCompletion()
        {
            SyncComplete?.Invoke();
        }

        private void Exporter_FileGenerationCompleted(string fullOutputPath)
        {
            mGeneratedFastaFilePath = fullOutputPath;
        }
    }
}