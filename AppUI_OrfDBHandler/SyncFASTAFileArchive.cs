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
using PRISMDatabaseUtils;
using ProteinFileReader;

namespace AppUI_OrfDBHandler
{
    public class SyncFASTAFileArchive
    {

        // private IArchiveOutputFiles m_FileArchiver;
        private readonly DBTask m_DatabaseAccessor;
        private readonly AddUpdateEntries m_Importer;
        private GetFASTAFromDMS m_Exporter;

        public event SyncStartEventHandler SyncStart;

        public delegate void SyncStartEventHandler(string statusMsg);

        public event SyncProgressEventHandler SyncProgress;

        public delegate void SyncProgressEventHandler(string statusMsg, double fractionDone);

        public event SyncCompleteEventHandler SyncComplete;

        public delegate void SyncCompleteEventHandler();

        private string m_CurrentStatusMsg;
        private int m_CurrentProteinCount;
        private int m_TotalProteinsCount;

        private string m_GeneratedFastaFilePath;

        public SyncFASTAFileArchive(string psConnectionString)
        {
            m_DatabaseAccessor = new DBTask(psConnectionString);
            m_Importer = new AddUpdateEntries(psConnectionString);
        }

        public int SyncCollectionsAndArchiveTables(string outputPath)
        {
            string sql =
                "SELECT Protein_Collection_ID, FileName, Authentication_Hash, DateModified, Collection_Type_ID, NumProteins " +
                "FROM V_Missing_Archive_Entries";

            // TODO add collection list string
            string proteinCollectionList = "";

            DataTable dt;
            dt = m_DatabaseAccessor.GetTable(sql);
            DataRow dr;
            string sourceFilePath;
            int proteinCollectionID;
            string SHA1;
            string CreationOptionsString = "seq_direction=forward,filetype=fasta";
            var totalProteinsCount = default(int);
            int currentCollectionProteinCount = 0;
            foreach (DataRow currentDr in dt.Rows)
            {
                dr = currentDr;
                m_TotalProteinsCount += Convert.ToInt32(dr["NumProteins"]);
            }

            var outputSequenceType = GetFASTAFromDMS.SequenceTypes.forward;
            var databaseFormatType = GetFASTAFromDMS.DatabaseFormatTypes.fasta;

            OnSyncStart("Synchronizing Archive Table with Collections Table");

            var fileArchiver = new ArchiveToFile(m_DatabaseAccessor, m_Exporter);

            foreach (DataRow currentDr1 in dt.Rows)
            {
                dr = currentDr1;
                OnSyncProgressUpdate("Processing - '" + dr["FileName"].ToString() + "'", currentCollectionProteinCount / (double)totalProteinsCount);
                currentCollectionProteinCount = Convert.ToInt32(dr["NumProteins"]);
                proteinCollectionID = Convert.ToInt32(dr["Protein_Collection_ID"]);
                sourceFilePath = Path.Combine(outputPath, dr["FileName"].ToString() + ".fasta");
                SHA1 = dr["Authentication_Hash"].ToString();

                fileArchiver.ArchiveCollection(
                    proteinCollectionID,
                    ArchiveOutputFilesBase.CollectionTypes.@static,
                    outputSequenceType, databaseFormatType, sourceFilePath, CreationOptionsString, SHA1, proteinCollectionList);
            }

            OnSyncCompletion();

            return 0;
        }

        public void UpdateSHA1Hashes() // Implements IArchiveOutputFiles.UpdateSHA1Hashes
        {
            string sql;

            sql = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, NumProteins " +
                "FROM V_Missing_Archive_Entries";

            DataTable dt;

            dt = m_DatabaseAccessor.GetTable(sql);

            int tmpID;
            string tmpStoredSHA;
            string tmpGenSHA;
            string tmpFilename;
            FileInfo fi;
            string tmpFullPath;

            foreach (DataRow dr in dt.Rows)
            {
                m_TotalProteinsCount += Convert.ToInt32(dr["Numproteins"]);
            }

            DateTime startTime;
            TimeSpan elapsedTime;
            var elapsedTimeSB = new StringBuilder();

            string tmpPath = Path.GetTempPath();

            string connectionString;
            if (m_DatabaseAccessor == null || string.IsNullOrWhiteSpace(m_DatabaseAccessor.ConnectionString))
            {
                connectionString = string.Empty;
            }
            else
            {
                connectionString = m_DatabaseAccessor.ConnectionString;
            }

            m_Exporter = new GetFASTAFromDMS(
                connectionString, GetFASTAFromDMS.DatabaseFormatTypes.fasta,
                GetFASTAFromDMS.SequenceTypes.forward);
            m_Exporter.FileGenerationCompleted += m_Exporter_FileGenerationCompleted;

            string creationOptionsString;
            creationOptionsString = "seq_direction=forward,filetype=fasta";
            OnSyncStart("Updating Collections and Archive Entries");
            startTime = DateTime.UtcNow;

            var fileArchiver = new ArchiveToFile(m_DatabaseAccessor, m_Exporter);

            foreach (DataRow dr in dt.Rows)
            {
                tmpID = Convert.ToInt32(dr["Protein_Collection_ID"]);
                tmpStoredSHA = dr["Authentication_Hash"].ToString();
                tmpFilename = dr["FileName"].ToString();
                m_GeneratedFastaFilePath = string.Empty;

                elapsedTimeSB.Remove(0, elapsedTimeSB.Length);
                elapsedTime = DateTime.UtcNow.Subtract(startTime);
                if (elapsedTime.Minutes < 1 & elapsedTime.Hours == 0)
                {
                    elapsedTimeSB.Append("less than ");
                }
                else
                {
                    elapsedTimeSB.Append("about ");
                }

                if (elapsedTime.Hours > 0)
                {
                    elapsedTimeSB.Append(elapsedTime.Hours.ToString() + " hours, ");
                }

                if (elapsedTime.Minutes <= 1)
                {
                    elapsedTimeSB.Append("1 minute");
                }
                else
                {
                    elapsedTimeSB.Append(elapsedTime.Minutes.ToString() + " minutes");
                }

                // OnSyncProgressUpdate(
                //     "Collection "
                //     + Format(tmpID, "0000")
                //     + " [Elapsed Time: "
                //     + elapsedTimeSB.ToString() + "]",
                //     currentProteinCount / (double)totalProteinCount)
                m_CurrentStatusMsg = "Collection " + tmpID.ToString("0000") + " [Elapsed Time: " + elapsedTimeSB.ToString() + "]";

                OnSyncProgressUpdate(
                    m_CurrentStatusMsg,
                    m_CurrentProteinCount / (double)m_TotalProteinsCount);

                tmpFullPath = Path.Combine(tmpPath, tmpFilename + ".fasta");
                // Debug.WriteLine("Start: " + tmpFilename + ": " + startTime.ToLongTimeString());

                tmpGenSHA = m_Exporter.ExportFASTAFile(tmpID, tmpPath,
                    GetFASTAFromDMS.DatabaseFormatTypes.fasta,
                    GetFASTAFromDMS.SequenceTypes.forward);

                if (!tmpStoredSHA.Equals(tmpGenSHA))
                {
                    int currentFastaProteinCount = 0;
                    int currentFastaResidueCount = 0;

                    if (!string.IsNullOrEmpty(m_GeneratedFastaFilePath))
                    {
                        CountProteinsAndResidues(m_GeneratedFastaFilePath, out currentFastaProteinCount, out currentFastaResidueCount);
                    }

                    m_Importer.AddAuthenticationHash(tmpID, tmpGenSHA, currentFastaProteinCount, currentFastaResidueCount);
                }

                // Debug.WriteLine("End: " + tmpFilename + ": " + DateTime.Now.ToLongTimeString);
                // Debug.Flush();

                fileArchiver.ArchiveCollection(
                    tmpID,
                    ArchiveOutputFilesBase.CollectionTypes.@static,
                    GetFASTAFromDMS.SequenceTypes.forward,
                    GetFASTAFromDMS.DatabaseFormatTypes.fasta,
                    tmpFullPath, creationOptionsString, tmpGenSHA, "");
                // ArchiveCollection(
                //     tmpID,
                //     IArchiveOutputFiles.CollectionTypes.static,
                //     tmpFullPath, tmpGenSHA);

                // tmpNameList.Clear();
                fi = new FileInfo(tmpFullPath);
                fi.Delete();
                m_CurrentProteinCount += Convert.ToInt32(dr["NumProteins"]);
            }

            OnSyncCompletion();

            // OnSyncStart("Updating ProteinNames");

            // // Update T_Protein_Names
            // int tmpRefID;
            // string tmpName;
            // string tmpFingerprint;
            // int tmpProtID;
            // int errorCode;
            // int counter = 0;

            // sql = "SELECT Reference_ID, Name, Reference_Fingerprint, Protein_ID " +
            //       "FROM T_Protein_Names";

            // dt = m_DatabaseAccessor.GetTable(sql);

            // var dbTools = m_DatabaseAccessor.DBTools;

            // foreach (DataRow dr in dt.Rows)
            // {
            //     counter += 1;
            //     tmpRefID = dbTools.GetInteger(dr["Reference_ID"]);
            //     tmpName = dbTools.GetString(dr["Name"]);
            //     tmpFingerprint = dbTools.GetString(dr["Reference_Fingerprint"]);
            //     tmpProtID = dbTools.GetInteger(dr["Protein_ID"]);

            //     // tmpGenSHA = m_Importer.GenerateArbitraryHash(tmpName + tmpProtID.ToString)
            //     errorCode = m_Importer.UpdateProteinNameHash(tmpRefID, tmpName, tmpProtID);
            //     if (counter % 2000 == 0)
            //         Debug.WriteLine(counter.ToString());
            // }

            // // Update T_Proteins

            // string tmpSeq;
            // counter = 0;

            // sql = "SELECT Protein_ID, Sequence " +
            //       "FROM T_Proteins";

            // dt = m_DatabaseAccessor.GetTable(sql);

            // foreach (DataRow dr in dt.Rows)
            // {
            //     counter += 1;
            //     tmpProtID = dbTools.GetInteger(dr["Protein_ID");
            //     tmpSeq = dbTools.GetString(dr["Sequence");

            //     errorCode = m_Importer.UpdateProteinSequenceHash(tmpProtID, tmpSeq);

            //     if (counter % 2000 == 0)
            //         Debug.WriteLine(counter.ToString());
            // }
        }

        private void CountProteinsAndResidues(string fastaFilePath, out int proteinCount, out int residueCount)
        {
            proteinCount = 0;
            residueCount = 0;

            var oReader = new FastaFileReader();
            if (oReader.OpenFile(fastaFilePath))
            {
                while (oReader.ReadNextProteinEntry())
                {
                    proteinCount += 1;
                    residueCount += oReader.ProteinSequence.Length;
                }
            }

            oReader.CloseFile();
        }

        public void FixArchivedFilePaths()
        {
            string sql = "SELECT * FROM T_Temp_Archive_Path_Fix";

            var tmpTable = m_DatabaseAccessor.GetTable(sql);

            string tmpOldPath;
            string tmpNewPath;

            foreach (DataRow dr in tmpTable.Rows)
            {
                tmpOldPath = dr["Archived_File_Path"].ToString();
                tmpNewPath = dr["Newpath"].ToString();

                File.Move(tmpOldPath, tmpNewPath);
            }
        }

        public void AddSortingIndices()
        {
            string getCollectionsSQL = "SELECT Protein_Collection_ID, FileName, Organism_ID FROM V_Protein_Collections_By_Organism WHERE Collection_Type_ID = 1 or Collection_Type_ID = 5";

            var collectionTable = m_DatabaseAccessor.GetTable(getCollectionsSQL);

            string getLegacyFilesSQL = "SELECT DISTINCT FileName, Full_Path, Organism_ID FROM V_Legacy_Static_File_Locations";
            var legacyTable = m_DatabaseAccessor.GetTable(getLegacyFilesSQL);

            Dictionary<string, int> nameIndexHash;

            var dbTools = m_DatabaseAccessor.DBTools;

            foreach (DataRow collectionEntry in collectionTable.Rows)
            {
                string tmpCollectionName = collectionEntry["FileName"].ToString();
                int tmpCollectionID = Convert.ToInt32(collectionEntry["Protein_Collection_ID"]);
                if (tmpCollectionID == 1026)
                {
                    Debug.WriteLine("");
                }

                int tmpOrgID = Convert.ToInt32(collectionEntry["Organism_ID"]);

                var legacyFoundRows = legacyTable.Select("FileName = '" + tmpCollectionName + ".fasta' AND Organism_ID = " + tmpOrgID);
                if (legacyFoundRows.Length > 0)
                {
                    var getReferencesSQL = "SELECT * FROM V_Tmp_Member_Name_Lookup WHERE Protein_Collection_ID = " + tmpCollectionID.ToString() +
                                           " AND Sorting_Index == null";
                    var referencesTable = m_DatabaseAccessor.GetTable(getReferencesSQL);
                    if (referencesTable.Rows.Count > 0)
                    {
                        var legacyFileEntry = legacyFoundRows[0];
                        string legacyFullPath = legacyFileEntry["Full_Path"].ToString();
                        nameIndexHash = GetProteinSortingIndices(legacyFullPath);

                        foreach (DataRow referenceEntry in referencesTable.Rows)
                        {
                            int tmpRefID = dbTools.GetInteger(referenceEntry["Reference_ID"]);
                            int tmpProteinID = dbTools.GetInteger(referenceEntry["Protein_ID"]);
                            string tmpRefName = dbTools.GetString(referenceEntry["Name"]);

                            //try {
                            int tmpSortingIndex = nameIndexHash[tmpRefName.ToLower()];

                            if (tmpSortingIndex > 0)
                            {
                                m_Importer.UpdateProteinCollectionMember(
                                    tmpRefID, tmpProteinID,
                                    tmpSortingIndex, tmpCollectionID);
                            }

                            //} catch (Exception ex) {
                            //}
                        }
                    }
                }
            }
        }

        private Dictionary<string, int> GetProteinSortingIndices(string filePath)
        {
            var fi = new FileInfo(filePath);
            TextReader tr;
            string s;
            Regex nameRegex;
            Match m;
            var nameHash = new Dictionary<string, int>();
            var counter = default(int);
            string tmpName;

            nameRegex = new Regex(
                @"^\>(?<name>\S+)\s*(?<description>.*)$",
                RegexOptions.Compiled);

            tr = fi.OpenText();
            s = tr.ReadLine();

            while (s != null)
            {
                if (nameRegex.IsMatch(s))
                {
                    counter += 1;
                    m = nameRegex.Match(s);
                    tmpName = m.Groups["name"].Value;
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

        public void CorrectMasses()
        {
            var proteinList = new Dictionary<int, string>();
            DataTable proteinTable;
            var counter = default(int);
            int tmpRowCount = 1;

            var tmpTableInfo = m_DatabaseAccessor.GetTable(
                "SELECT TOP 1 TableRowCount " +
                "FROM V_Table_Row_Counts " +
                "WHERE TableName = 'T_Proteins'");

            DataRow pdr = tmpTableInfo.Rows[0];

            int tmpProteinCount = Convert.ToInt32(pdr["TableRowCount"]);

            int startCount;

            string proteinSelectSQL;

            OnSyncStart("Starting Mass Update");

            while (tmpRowCount > 0)
            {
                proteinList.Clear();
                startCount = counter;
                counter = counter + 10000;

                proteinSelectSQL = "SELECT Protein_ID, Sequence FROM T_Proteins " +
                    "WHERE Protein_ID <= " + counter.ToString() + " AND Protein_ID > " + startCount.ToString();

                // proteinSelectSQL = "SELECT Protein_ID, Sequence FROM T_Proteins " +
                //                    "WHERE Protein_ID = 285130";
                //     "WHERE Protein_ID <= " + counter.ToString();

                proteinTable = m_DatabaseAccessor.GetTable(proteinSelectSQL);

                tmpRowCount = proteinTable.Rows.Count;

                foreach (DataRow dr in proteinTable.Rows)
                {
                    proteinList.Add(Convert.ToInt32(dr["Protein_ID"]), dr["Sequence"].ToString());
                }

                OnSyncProgressUpdate("Processing Protein_ID " + startCount.ToString() + "-" + counter.ToString() + " of " + tmpProteinCount.ToString(), counter / (double)tmpProteinCount);
                if (proteinList.Count > 0)
                {
                    UpdateProteinSequenceInfo(proteinList);
                }
            }

            OnSyncCompletion();
        }

        public void RefreshNameHashes()
        {
            int totalNameCount;
            string nameCountSQL = "SELECT TOP 1 Reference_ID FROM T_Protein_Names ORDER BY Reference_ID DESC";

            var nameCountResults = m_DatabaseAccessor.GetTable(nameCountSQL);
            totalNameCount = Convert.ToInt32(nameCountResults.Rows[0]["Reference_ID"]);

            int tmpRefID;
            string tmpProteinName;
            string tmpDescription;
            int tmpProteinID;

            int startIndex = 0;
            int counter;
            int stepValue = 10000;

            if (totalNameCount <= stepValue)
            {
                stepValue = totalNameCount;
            }

            OnSyncStart("Updating Name Hashes");

            var dbTools = m_DatabaseAccessor.DBTools;

            for (counter = stepValue; counter <= totalNameCount + stepValue; counter += stepValue)
            {
                if (counter >= totalNameCount - stepValue)
                {
                    Debug.WriteLine("");
                }

                var rowRetrievalSQL = "SELECT Reference_ID, Name, Description, Protein_ID " +
                                      "FROM T_Protein_Names " +
                                      "WHERE Reference_ID > " + startIndex + " and Reference_ID <= " + counter +
                                      "ORDER BY Reference_ID";

                // Protein_Name(+"_" + Description + "_" + ProteinID.ToString)
                var proteinListResults = m_DatabaseAccessor.GetTable(rowRetrievalSQL);
                if (proteinListResults.Rows.Count > 0)
                {
                    foreach (DataRow dr in proteinListResults.Rows)
                    {
                        tmpRefID = dbTools.GetInteger(dr["Reference_ID"]);
                        tmpProteinName = dbTools.GetString(dr["Name"]);
                        tmpDescription = dbTools.GetString(dr["Description"]);
                        tmpProteinID = dbTools.GetInteger(dr["Protein_ID"]);

                        m_Importer.UpdateProteinNameHash(
                            tmpRefID,
                            tmpProteinName,
                            tmpDescription,
                            tmpProteinID);
                    }

                    OnSyncProgressUpdate("Processing " + startIndex + " to " + counter, counter / (double)totalNameCount);
                    startIndex = counter + 1;
                }
            }

            OnSyncCompletion();
        }

        private void UpdateProteinSequenceInfo(Dictionary<int, string> proteins)
        {
            var si = new SequenceInfoCalculator();

            foreach (var proteinID in proteins.Keys)
            {
                string sequence = proteins[proteinID];
                si.CalculateSequenceInfo(sequence);
                m_Importer.UpdateProteinSequenceInfo(
                    proteinID, sequence, sequence.Length,
                    si.MolecularFormula, si.MonoisotopicMass,
                    si.AverageMass, si.SHA1Hash);
            }
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

        private void m_Exporter_FileGenerationCompleted(string fullOutputPath)
        {
            m_GeneratedFastaFilePath = fullOutputPath;
        }
    }
}