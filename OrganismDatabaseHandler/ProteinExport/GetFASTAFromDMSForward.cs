using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSForward
    {
        private readonly DBTask mDatabaseAccessor;
        private readonly ExportProteins mFileDumper;

        /// <summary>
        /// Keys are protein collection IDs
        /// Values are filename
        /// </summary>
        private readonly Dictionary<int, string> mAllCollections;

        private int mCurrentFileProteinCount;
        private DataTable mCollectionsCache;
        private DataTable mOrganismCache;

        protected string NamingSuffix = "_forward";
        private readonly string mExtension = "";

        private RijndaelEncryptionHandler mRijndaelDecryption;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        [Obsolete("Use the constructor that does not take databaseFormatType")]
        public GetFASTAFromDMSForward(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType) : this(databaseAccessor)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        public GetFASTAFromDMSForward(DBTask databaseAccessor)
        {
            mDatabaseAccessor = databaseAccessor;
            mAllCollections = GetCollectionNameList();

            mFileDumper = new ExportProteinsFASTA(this);
            mExtension = ".fasta";

            if (mFileDumper != null)
            {
                mFileDumper.ExportStart += OnExportStart;
                mFileDumper.ExportProgress += OnExportProgressUpdate;
            }
        }

        public event FileGenerationCompletedEventHandler FileGenerationCompleted;

        public delegate void FileGenerationCompletedEventHandler(string outputPath);

        public event FileGenerationProgressEventHandler FileGenerationProgress;

        public delegate void FileGenerationProgressEventHandler(string statusMsg, double fractionDone);

        public event FileGenerationStartedEventHandler FileGenerationStarted;

        public delegate void FileGenerationStartedEventHandler(string taskMsg);

        public string FullOutputPath { get; set; }

        // Unused
        // public readonly string ArchivalName => mCurrentArchiveFileName;

        protected virtual string ExtendedExportPath(
            string destinationFolderPath,
            string proteinCollectionName)
        {
            return Path.Combine(destinationFolderPath, proteinCollectionName + NamingSuffix + mExtension);
        }

        public virtual string SequenceExtender(string originalSequence, int collectionCount)
        {
            return originalSequence;
        }

        public virtual string ReferenceExtender(string originalReference)
        {
            return originalReference;
        }

        /// <summary>
        /// Create the FASTA file for the given protein collections
        /// </summary>
        /// <param name="protCollectionList"></param>
        /// <param name="destinationFolderPath"></param>
        /// <param name="alternateAnnotationTypeId"></param>
        /// <param name="padWithPrimaryAnnotation"></param>
        /// <returns>CRC32 hash for the file</returns>
        public virtual string ExportFASTAFile(
            List<string> protCollectionList,
            string destinationFolderPath,
            int alternateAnnotationTypeId,
            bool padWithPrimaryAnnotation)
        {
            var trueName = string.Empty;

            var tmpId = default(int);
            var tmpIdListSb = new StringBuilder();

            var nameCheckRegex = new Regex(@"(?<collectionname>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))");

            if (!CheckProteinCollectionNameValidity(protCollectionList))
            {
                return "";
            }

            var user = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var userId = user.Identity.Name;

            // Dictionary mapping protein collection name to the associated passphrase
            var proteinCollectionPassphrases = new Dictionary<string, string>();

            var collectionNameList = string.Empty;

            // Check each collection name for encryption of contents
            foreach (var nameString in protCollectionList)
            {
                var encCheckRows = mCollectionsCache.Select("Filename = '" + nameString + "' AND Contents_Encrypted > 0");

                if (encCheckRows.Length > 0)
                {
                    // Determine the encrypted collections to which this user has access
                    var authorizationSql = "SELECT Protein_Collection_ID, Protein_Collection_Name " +
                                           "FROM V_Encrypted_Collection_Authorizations " +
                                           "WHERE Login_Name = '" + userId + "'";

                    var authorizationTable = mDatabaseAccessor.GetTable(authorizationSql);
                    var authCheckRows = authorizationTable.Select("Protein_Collection_Name = '" + nameString + "' OR Protein_Collection_Name = 'Administrator'");
                    if (authCheckRows.Length > 0)
                    {
                        tmpId = FindIdByName(nameString);
                        var passPhraseSql = "SELECT Passphrase " +
                                            "FROM T_Encrypted_Collection_Passphrases " +
                                            "WHERE Protein_Collection_ID = " + tmpId;
                        var passPhraseTable = mDatabaseAccessor.GetTable(passPhraseSql);

                        proteinCollectionPassphrases.Add(nameString, passPhraseTable.Rows[0]["Passphrase"].ToString());
                    }
                    else
                    {
                        throw new Exception("User " + userId + " does not have access to the encrypted collection '" + nameString + "'");
                    }
                }

                if (collectionNameList.Length > 0)
                    collectionNameList += ", ";
                collectionNameList += nameString;
            }

            // Get a temp file name; however, create the file in the target folder path
            // (in case the system-defined temp directory is on a different drive than the target folder)

            FileInfo fiOutputPathCheck;
            string tmpOutputPath;

            do
            {
                var tmpOutputPathCandidate = Path.GetTempFileName();
                try
                {
                    // The GetTempFileName function created a temp file that we don't need; delete it now (but use try/catch just in case the deletion fails for some freak reason)
                    File.Delete(tmpOutputPathCandidate);
                }
                catch
                {
                    // Intentionally ignored
                }

                tmpOutputPath = Path.Combine(destinationFolderPath, Path.GetFileName(tmpOutputPathCandidate));
                fiOutputPathCheck = new FileInfo(tmpOutputPath);
            }
            while (fiOutputPathCheck.Exists);

            if (protCollectionList.Count == 1)
            {
                OnExportStart("Exporting protein collection " + collectionNameList);
            }
            else
            {
                OnExportStart("Exporting " + protCollectionList.Count + "protein collections: " + collectionNameList);
            }

            var proteinCollectionsExported = 0;
            foreach (var proteinCollName in protCollectionList)
            {
                var currentCollectionPos = 0;
                var currentCollectionCount = 0;

                // Make sure there are no leading or trailing spaces
                var proteinCollectionName = proteinCollName.Trim();

                if (nameCheckRegex.IsMatch(proteinCollectionName))
                {
                    var m = nameCheckRegex.Match(proteinCollectionName);
                    trueName = m.Groups["collectionname"].Value;
                }
                else
                {
                    trueName = proteinCollectionName;
                }

                // Lookup the number of proteins that should be in this protein collection
                var lengthCheckSql = "SELECT NumProteins FROM T_Protein_Collections " +
                                     "WHERE FileName = '" + proteinCollectionName + "'";

                var lengthCheckTable = mDatabaseAccessor.GetTable(lengthCheckSql);
                int collectionLength;

                if (lengthCheckTable.Rows.Count > 0)
                {
                    var foundRow = lengthCheckTable.Rows[0];
                    collectionLength = Convert.ToInt32(foundRow[0]);
                }
                else
                {
                    collectionLength = -1;
                }

                do
                {
                    var sectionStart = currentCollectionPos;
                    var sectionEnd = sectionStart + 10000;

                    string collectionSql;
                    if (padWithPrimaryAnnotation)
                    {
                        tmpId = FindIdByName(trueName);
                        collectionSql =
                            "SELECT Name, Description, Sequence, Protein_ID " +
                            "FROM V_Protein_Database_Export " +
                            "WHERE " +
                                "Protein_Collection_ID = " + tmpId + " " +
                                "AND Sorting_Index BETWEEN " + sectionStart + " AND " + sectionEnd + " " +
                            "ORDER BY Sorting_Index";
                    }
                    else
                    {
                        collectionSql =
                            "SELECT Name, Description, Sequence, Protein_ID " +
                            "FROM V_Protein_Database_Export " +
                            "WHERE Protein_Collection_ID = " + tmpId + ") " +
                                "AND Annotation_Type_ID = " + alternateAnnotationTypeId + " " +
                                "AND Sorting_Index BETWEEN " + sectionStart + " AND " + sectionEnd + " " +
                            "ORDER BY Sorting_Index";
                    }

                    collectionTable = mDatabaseAccessor.GetTable(collectionSql);

                    if (proteinCollectionPassphrases.TryGetValue(trueName, out var passPhraseForCollection))
                    {
                        mRijndaelDecryption = new RijndaelEncryptionHandler(passPhraseForCollection);
                        foreach (DataRow decryptionRow in collectionTable.Rows)
                        {
                            var cipherSeq = decryptionRow["Sequence"].ToString();
                            var clearSeq = mRijndaelDecryption.Decrypt(cipherSeq);
                            decryptionRow["Sequence"] = clearSeq;
                            decryptionRow.AcceptChanges();
                        }
                    }

                    string tableName;
                    if (collectionLength < 10000)
                    {
                        tableName = trueName;
                    }
                    else
                    {
                        tableName = trueName + "_" + sectionStart.ToString("0000000000") + "-" + sectionEnd.ToString("0000000000");
                    }

                    collectionTable.TableName = tableName;

                    mCurrentFileProteinCount = collectionTable.Rows.Count;

                    // collection.Tables.Add(collectionTable)
                    mFileDumper.Export(collectionTable, ref tmpOutputPath);

                    currentCollectionPos = sectionEnd + 1;
                    currentCollectionCount += collectionTable.Rows.Count;

                    var fractionDoneOverall = 0d;
                    if (collectionLength > 0)
                    {
                        fractionDoneOverall = proteinCollectionsExported / (double)protCollectionList.Count + currentCollectionCount / (double)collectionLength / protCollectionList.Count;
                    }

                    OnExportProgressUpdate(currentCollectionCount + " entries exported, collection " + (proteinCollectionsExported + 1) + " of " + protCollectionList.Count, fractionDoneOverall);
                }
                while (collectionTable.Rows.Count != 0);

                tmpIdListSb.Append(tmpId.ToString("000000"));
                tmpIdListSb.Append("+");
                if (currentCollectionCount != collectionLength)
                {
                    throw new Exception(string.Format(
                        "The number of proteins exported for collection '{0}' does not match the expected value: " +
                        "{1} exported from T_Protein_Collection_Members vs. {2} listed in T_Protein_Collections",
                        proteinCollectionName, currentCollectionCount, collectionLength));
                }

                proteinCollectionsExported++;
            }

            OnExportComplete(tmpOutputPath);

            var tmpFi = new FileInfo(tmpOutputPath);

            tmpIdListSb.Remove(tmpIdListSb.Length - 1, 1);
            string name; // = hash;

            if (protCollectionList.Count > 1)
            {
                name = tmpIdListSb.ToString();
                if (destinationFolderPath.Length + name.Length > 225)
                {
                    // If exporting a large number of protein collections, name can be very long
                    // This can lead to error: The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters
                    // Thus, truncate name
                    var intMaxNameLength = 225 - destinationFolderPath.Length;
                    if (intMaxNameLength < 30)
                        intMaxNameLength = 30;

                    name = name.Substring(0, intMaxNameLength);

                    // Find the last plus sign and truncate just before it
                    var intLastPlusLocation = name.LastIndexOf('+');
                    if (intLastPlusLocation > 30)
                    {
                        name = name.Substring(0, intLastPlusLocation);
                    }
                }
            }
            else
            {
                name = trueName;
            }

            FullOutputPath = ExtendedExportPath(destinationFolderPath, name);

            // Rename (move) the temporary file to the final, full name
            if (File.Exists(FullOutputPath))
            {
                File.Delete(FullOutputPath);
            }

            tmpFi.MoveTo(FullOutputPath);

            // Assuming the final file now exists, delete the temporary file (if present)
            var finalOutputFile = new FileInfo(FullOutputPath);
            if (finalOutputFile.Exists)
            {
                tmpFi = new FileInfo(tmpOutputPath);
                if (tmpFi.Exists)
                {
                    tmpFi.Delete();
                }
            }

            // Determine the CRC32 hash of the output file
            // This process will also rename the file, e.g. from "C:\Temp\SAR116_RBH_AA_012809_forward.fasta" to "C:\Temp\38FFACAC.fasta"
            var tempFullPath = FullOutputPath;
            var crc32Hash = mFileDumper.Export(new DataTable(), ref tempFullPath);
            FullOutputPath = FullOutputPath;

            OnExportComplete(FullOutputPath);

            return crc32Hash;
        }

        /// <summary>
        /// Create the FASTA file for the given protein collections
        /// </summary>
        /// <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
        /// <param name="destinationFolderPath"></param>
        /// <returns>CRC32 hash of the generated (or retrieved) file</returns>
        public virtual string ExportFASTAFile(
            List<string> protCollectionList,
            string destinationFolderPath)
        {
            const int primaryAuthorityID = 1;
            const bool padWithPrimaryAnnotation = true;

            return ExportFASTAFile(protCollectionList, destinationFolderPath, primaryAuthorityID, padWithPrimaryAnnotation);
        }

        protected bool CheckProteinCollectionNameValidity(List<string> protCollectionList)
        {
            foreach (var name in protCollectionList)
            {
                var id = FindIdByName(name);
                if (id < 1)
                {
                    throw new Exception("The collection named '" + name + "' does not exist in the system");
                }
            }

            return true;
        }

        // Unused
        // protected int GetPrimaryAuthorityID(int proteinCollectionID)
        // {
        //     var foundRows = mCollectionsCache.Select("Protein_Collection_ID = " + proteinCollectionID.ToString()).ToList();
        //
        //     var primaryAnnotationTypeID = foundRows[0]["Primary_Annotation_Type_ID"].ToString();
        //     int idValue;
        //     if (foundRows.Count == 0 || !int.TryParse(primaryAnnotationTypeID, ref idValue))
        //         return 0;
        //
        //     return idValue;
        // }

        // Unused
        // protected int GetPrimaryAuthorityID(string proteinCollectionName)
        // {
        //     int proteinCollectionID = FindIDByName(proteinCollectionName);
        //     return GetPrimaryAuthorityID(proteinCollectionID);
        // }

        public Dictionary<int, string> GetCollectionNameList()
        {
            if (mDatabaseAccessor == null)
            {
                return new Dictionary<int, string>();
            }

            if (mCollectionsCache == null)
            {
                RefreshCollectionCache();
            }

            return mDatabaseAccessor.DataTableToDictionaryIntegerKeys(mCollectionsCache, "Protein_Collection_ID", "FileName");
        }

        public Dictionary<string, string> GetCollectionsByOrganism(int organismId)
        {
            if (mDatabaseAccessor == null)
            {
                return new Dictionary<string, string>();
            }

            if (mCollectionsCache == null)
            {
                RefreshCollectionCache();
            }

            return mDatabaseAccessor.DataTableToDictionary(mCollectionsCache, "Protein_Collection_ID", "FileName", "[Organism_ID] = " + organismId);
        }

        public DataTable GetCollectionsByOrganismTable(int organismId)
        {
            var tmpTable = mCollectionsCache.Clone();

            var foundRows = mCollectionsCache.Select("[Organism_ID] = " + organismId);

            foreach (var dr in foundRows)
                tmpTable.ImportRow(dr);

            return tmpTable;
        }

        public Dictionary<string, string> GetOrganismList()
        {
            if (mDatabaseAccessor == null)
            {
                return new Dictionary<string, string>();
            }

            if (mOrganismCache == null)
            {
                RefreshOrganismCache();
            }

            return mDatabaseAccessor.DataTableToDictionary(mOrganismCache, "Organism_ID", "Name");
        }

        public DataTable GetOrganismListTable()
        {
            if (mDatabaseAccessor == null)
            {
                return new DataTable();
            }

            if (mOrganismCache == null)
            {
                RefreshOrganismCache();
            }

            return mOrganismCache;
        }

        protected void RefreshCollectionCache()
        {
            if (mDatabaseAccessor == null || string.IsNullOrWhiteSpace(mDatabaseAccessor.ConnectionString))
            {
                mCollectionsCache = new DataTable();
            }
            else
            {
                mCollectionsCache = mDatabaseAccessor.GetTable("SELECT * FROM V_Protein_Collections_By_Organism ORDER BY Protein_Collection_ID");
            }
        }

        protected void RefreshOrganismCache()
        {
            if (mDatabaseAccessor == null || string.IsNullOrWhiteSpace(mDatabaseAccessor.ConnectionString))
            {
                mOrganismCache = new DataTable();
            }
            else
            {
                mOrganismCache = mDatabaseAccessor.GetTable("SELECT ID as Organism_ID, Short_Name as Name FROM V_Organism_Picker ORDER BY Organism_ID");
            }
        }

        public int FindIdByName(string collectionName)
        {
            if (collectionName.Length == 0)
            {
                return 0;
            }

            // Make sure there are no leading or trailing spaces
            collectionName = collectionName.Trim();
            var foundRows = mCollectionsCache.Select("[FileName] = '" + collectionName + "'");
            if (foundRows.Length == 0)
            {
                RefreshCollectionCache();
                foundRows = mCollectionsCache.Select("[FileName] = '" + collectionName + "'");
            }

            int id;
            try
            {
                id = Convert.ToInt32(foundRows[0]["Protein_Collection_ID"]);
            }
            catch (Exception)
            {
                // Ignore errors here
                id = -1;
            }

            return id;
        }

        public string FindNameById(int collectionId)
        {
            var foundRows = mCollectionsCache.Select("Protein_Collection_ID = " + collectionId).ToList();

            if (foundRows.Count == 0)
            {
                RefreshCollectionCache();
                foundRows = mCollectionsCache.Select("Protein_Collection_ID = " + collectionId).ToList();
            }

            if (foundRows.Count > 0)
            {
                return foundRows[0]["FileName"].ToString();
            }

            return string.Empty;
        }

        protected int FindPrimaryAnnotationId(int collectionId)
        {
            if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
            var foundRows = mCollectionsCache.Select("Protein_Collection_ID = " + collectionId).ToList();

            if (foundRows.Count > 0)
            {
                return Convert.ToInt32(foundRows[0]["Primary_Annotation_Type_ID"]);
            }

            return 0;
        }

        /// <summary>
        /// Compute the CRC32 hash for the file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns>File hash</returns>
        public string GetFileHash(string fullFilePath)
        {
            return mFileDumper.GenerateFileAuthenticationHash(fullFilePath);
        }

        public string GetStoredHash(string proteinCollectionName)
        {
            var foundRows = mCollectionsCache.Select("[FileName] = '" + proteinCollectionName + "'");
            return foundRows[0]["Authentication_Hash"]?.ToString();
        }

        public string GetStoredHash(int proteinCollectionId)
        {
            var proteinCollectionName = mAllCollections[proteinCollectionId];
            return GetStoredHash(proteinCollectionName);
        }

        protected void OnExportStart(string taskMsg)
        {
            FileGenerationStarted?.Invoke(taskMsg);
        }

        protected void OnExportProgressUpdate(string statusMsg, double fractionDone)
        {
            FileGenerationProgress?.Invoke(statusMsg, fractionDone);
        }

        protected void OnExportComplete(string outputFilePath)
        {
            FileGenerationCompleted?.Invoke(outputFilePath);
        }
    }
}