using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinExport;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.SequenceInfo;
using PRISM;
using PRISMDatabaseUtils;
using ProteinFileReader;

namespace AppUI_OrfDBHandler
{
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
            mDatabaseAccessor = new DBTask(psConnectionString);
            RegisterEvents(mDatabaseAccessor);

            mImporter = new AddUpdateEntries(psConnectionString);
            RegisterEvents(mImporter);
        }

        public int SyncCollectionsAndArchiveTables(string outputPath)
        {
            const string sql =
                "SELECT Protein_Collection_ID, FileName, Authentication_Hash, DateModified, Collection_Type_ID, NumProteins " +
                "FROM V_Missing_Archive_Entries";

            // TODO add collection list string
            const string proteinCollectionList = "";

            var dt = mDatabaseAccessor.GetTable(sql);
            const string CreationOptionsString = "seq_direction=forward,filetype=fasta";

            var currentCollectionProteinCount = 0;
            foreach (DataRow dr in dt.Rows)
            {
                mTotalProteinsCount += Convert.ToInt32(dr["NumProteins"]);
            }

            const GetFASTAFromDMS.SequenceTypes outputSequenceType = GetFASTAFromDMS.SequenceTypes.Forward;

            OnSyncStart("Synchronizing Archive Table with Collections Table");

            var fileArchiver = new ArchiveToFile(mDatabaseAccessor, mExporter);

            foreach (DataRow dr in dt.Rows)
            {
                OnSyncProgressUpdate("Processing - '" + dr["FileName"] + "'", currentCollectionProteinCount / (double)mTotalProteinsCount);
                currentCollectionProteinCount = Convert.ToInt32(dr["NumProteins"]);
                var proteinCollectionId = Convert.ToInt32(dr["Protein_Collection_ID"]);
                var sourceFilePath = Path.Combine(outputPath, dr["FileName"] + ".fasta");
                var sha1 = dr["Authentication_Hash"].ToString();

                fileArchiver.ArchiveCollection(
                    proteinCollectionId,
                    ArchiveOutputFilesBase.CollectionTypes.Static,
                    outputSequenceType, sourceFilePath, CreationOptionsString, sha1, proteinCollectionList);
            }

            OnSyncCompletion();

            return 0;
        }

        public void UpdateSHA1Hashes() // Implements IArchiveOutputFiles.UpdateSHA1Hashes
        {
            const string sql = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, NumProteins " +
                      "FROM V_Missing_Archive_Entries";

            var dt = mDatabaseAccessor.GetTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                mTotalProteinsCount += Convert.ToInt32(dr["NumProteins"]);
            }

            var elapsedTimeSb = new StringBuilder();

            var tmpPath = Path.GetTempPath();

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

            foreach (DataRow dr in dt.Rows)
            {
                var tmpId = Convert.ToInt32(dr["Protein_Collection_ID"]);
                var tmpStoredSHA = dr["Authentication_Hash"].ToString();
                var tmpFilename = dr["FileName"].ToString();
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
                //     + Format(tmpID, "0000")
                //     + " [Elapsed Time: "
                //     + elapsedTimeSB.ToString() + "]",
                //     currentProteinCount / (double)totalProteinCount)
                mCurrentStatusMsg = "Collection " + tmpId.ToString("0000") + " [Elapsed Time: " + elapsedTimeSb + "]";

                OnSyncProgressUpdate(
                    mCurrentStatusMsg,
                    mCurrentProteinCount / (double)mTotalProteinsCount);

                var tmpFullPath = Path.Combine(tmpPath, tmpFilename + ".fasta");
                // Debug.WriteLine("Start: " + tmpFilename + ": " + startTime.ToLongTimeString());

                var tmpGenSHA = mExporter.ExportFASTAFile(tmpId, tmpPath, GetFASTAFromDMS.SequenceTypes.Forward);

                if (!tmpStoredSHA.Equals(tmpGenSHA))
                {
                    var currentFastaProteinCount = 0;
                    var currentFastaResidueCount = 0;

                    if (!string.IsNullOrEmpty(mGeneratedFastaFilePath))
                    {
                        CountProteinsAndResidues(mGeneratedFastaFilePath, out currentFastaProteinCount, out currentFastaResidueCount);
                    }

                    mImporter.AddAuthenticationHash(tmpId, tmpGenSHA, currentFastaProteinCount, currentFastaResidueCount);
                }

                // Debug.WriteLine("End: " + tmpFilename + ": " + DateTime.Now.ToLongTimeString);
                // Debug.Flush();

                fileArchiver.ArchiveCollection(
                    tmpId,
                    ArchiveOutputFilesBase.CollectionTypes.Static,
                    GetFASTAFromDMS.SequenceTypes.Forward,
                    tmpFullPath, creationOptionsString, tmpGenSHA, "");

                // ArchiveCollection(
                //     tmpID,
                //     IArchiveOutputFiles.CollectionTypes.static,
                //     tmpFullPath, tmpGenSHA);

                // tmpNameList.Clear();
                var fi = new FileInfo(tmpFullPath);
                fi.Delete();
                mCurrentProteinCount += Convert.ToInt32(dr["NumProteins"]);
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

        private Dictionary<string, int> GetProteinSortingIndices(string filePath)
        {
            var fi = new FileInfo(filePath);
            var nameHash = new Dictionary<string, int>();
            var counter = default(int);

            var nameRegex = new Regex(
                @"^\>(?<name>\S+)\s*(?<description>.*)$",
                RegexOptions.Compiled);

            TextReader tr = fi.OpenText();
            var s = tr.ReadLine();

            while (s != null)
            {
                if (nameRegex.IsMatch(s))
                {
                    counter++;
                    var m = nameRegex.Match(s);
                    var tmpName = m.Groups["name"].Value;
                    if (!nameHash.ContainsKey(tmpName.ToLower()))
                    {
                        nameHash.Add(tmpName.ToLower(), counter);
                    }
                }

                s = tr.ReadLine();
            }

            tr.Close();

            return nameHash;
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
                    foreach (DataRow dr in proteinListResults.Rows)
                    {
                        var tmpRefId = dbTools.GetInteger(dr["Reference_ID"]);
                        var tmpProteinName = dbTools.GetString(dr["Name"]);
                        var tmpDescription = dbTools.GetString(dr["Description"]);
                        var tmpProteinId = dbTools.GetInteger(dr["Protein_ID"]);

                        mImporter.UpdateProteinNameHash(
                            tmpRefId,
                            tmpProteinName,
                            tmpDescription,
                            tmpProteinId);
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