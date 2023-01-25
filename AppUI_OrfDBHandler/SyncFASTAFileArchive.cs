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
                "SELECT protein_collection_id, collection_name, authentication_hash, date_modified, collection_type_id, num_proteins " +
                "FROM v_missing_archive_entries";

            // TODO add collection list string
            const string proteinCollectionList = "";

            var dt = mDatabaseAccessor.GetTable(sql);
            const string CreationOptionsString = "seq_direction=forward,filetype=fasta";

            var currentCollectionProteinCount = 0;
            foreach (DataRow dataRow in dt.Rows)
            {
                mTotalProteinsCount += Convert.ToInt32(dataRow["num_proteins"]);
            }

            const GetFASTAFromDMS.SequenceTypes outputSequenceType = GetFASTAFromDMS.SequenceTypes.Forward;

            OnSyncStart("Synchronizing Archive Table with Collections Table");

            var fileArchiver = new ArchiveToFile(mDatabaseAccessor, mExporter);

            foreach (DataRow dataRow in dt.Rows)
            {
                OnSyncProgressUpdate("Processing - '" + dataRow["collection_name"] + "'", currentCollectionProteinCount / (double)mTotalProteinsCount);
                currentCollectionProteinCount = Convert.ToInt32(dataRow["num_proteins"]);
                var proteinCollectionId = Convert.ToInt32(dataRow["protein_collection_id"]);
                var sourceFilePath = Path.Combine(outputPath, dataRow["collection_name"] + ".fasta");
                var sha1 = dataRow["authentication_hash"].ToString();

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
            const string sql = "SELECT protein_collection_id, collection_name, authentication_hash, num_proteins " +
                               "FROM v_missing_archive_entries";

            var dt = mDatabaseAccessor.GetTable(sql);

            foreach (DataRow dataRow in dt.Rows)
            {
                mTotalProteinsCount += Convert.ToInt32(dataRow["num_proteins"]);
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
                var proteinCollectionID = Convert.ToInt32(dataRow["protein_collection_id"]);
                var storedSHA = dataRow["authentication_hash"].ToString();
                var collectionName = dataRow["collection_name"].ToString();
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

                mCurrentProteinCount += Convert.ToInt32(dataRow["num_proteins"]);
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

        [Obsolete("Valid, but unused and could take a very long time to run")]
        public void RefreshNameHashes()
        {
            const string nameCountSQL = "Select reference_id From t_protein_names order by reference_id Desc OFFSET 0 Rows FETCH FIRST 1 ROW ONLY";

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

                var rowRetrievalSql = "SELECT reference_id, name, description, protein_id " +
                                      "FROM t_protein_names " +
                                      "WHERE reference_id > " + startIndex + " AND reference_id <= " + counter +
                                      "ORDER BY reference_id";

                // Protein_Name(+"_" + Description + "_" + ProteinID.ToString)
                var proteinListResults = mDatabaseAccessor.GetTable(rowRetrievalSql);
                if (proteinListResults.Rows.Count > 0)
                {
                    foreach (DataRow dataRow in proteinListResults.Rows)
                    {
                        var refId = dbTools.GetInteger(dataRow["reference_id"]);
                        var proteinName = dbTools.GetString(dataRow["name"]);
                        var description = dbTools.GetString(dataRow["description"]);
                        var proteinId = dbTools.GetInteger(dataRow["protein_id"]);

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