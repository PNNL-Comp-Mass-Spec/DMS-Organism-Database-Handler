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
        // Ignore Spelling: fastapro, passphrase

        private readonly DBTask mDatabaseAccessor;
        private readonly ExportProteins mFileDumper;

        /// <summary>
        /// Keys are protein collection IDs
        /// Values are protein collection name
        /// </summary>
        private readonly Dictionary<int, string> mAllCollections;

        private DataTable mCollectionsCache;
        private DataTable mOrganismCache;

        protected string NamingSuffix = "_forward";
        private readonly string mExtension;

        private RijndaelEncryptionHandler mRijndaelDecryption;

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
        public event FileGenerationProgressEventHandler FileGenerationProgress;
        public event FileGenerationStartedEventHandler FileGenerationStarted;

        public string FullOutputPath { get; set; }

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

            var proteinCollectionID = default(int);
            var proteinCollectionIDs = new List<string>();

            var nameCheckRegex = new Regex(@"(?<CollectionName>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))");

            if (!CheckProteinCollectionNameValidity(protCollectionList))
            {
                return "";
            }

            var user = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            var userId = user.Identity.Name;

            // Dictionary mapping protein collection name to the associated pass phrase
            var proteinCollectionPassphrases = new Dictionary<string, string>();

            var collectionNameList = string.Empty;

            // Check each collection name for encryption of contents
            foreach (var nameString in protCollectionList)
            {
                var encCheckRows = mCollectionsCache.Select("collection_name = '" + nameString + "' AND contents_encrypted > 0");

                if (encCheckRows.Length > 0)
                {
                    // Determine the encrypted collections to which this user has access
                    var authorizationSql = "SELECT protein_collection_id, protein_collection_name " +
                                           "FROM v_encrypted_collection_authorizations " +
                                           "WHERE login_name = '" + userId + "'";

                    var authorizationTable = mDatabaseAccessor.GetTable(authorizationSql);
                    var authCheckRows = authorizationTable.Select("protein_collection_name = '" + nameString + "' OR protein_collection_name = 'Administrator'");
                    if (authCheckRows.Length > 0)
                    {
                        proteinCollectionID = FindIdByName(nameString);
                        var passPhraseSql = "SELECT passphrase " +
                                            "FROM t_encrypted_collection_passphrases " +
                                            "WHERE protein_collection_id = " + proteinCollectionID;
                        var passPhraseTable = mDatabaseAccessor.GetTable(passPhraseSql);

                        proteinCollectionPassphrases.Add(nameString, passPhraseTable.Rows[0]["passphrase"].ToString());
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
            string tempOutputFilePath;

            do
            {
                var outputPathCandidate = Path.GetTempFileName();
                try
                {
                    // The GetTempFileName function created a temp file that we don't need; delete it now (but use try/catch just in case the deletion fails for some freak reason)
                    File.Delete(outputPathCandidate);
                }
                catch
                {
                    // Intentionally ignored
                }

                tempOutputFilePath = Path.Combine(destinationFolderPath, Path.GetFileName(outputPathCandidate));
                fiOutputPathCheck = new FileInfo(tempOutputFilePath);
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
                    trueName = m.Groups["CollectionName"].Value;
                }
                else
                {
                    trueName = proteinCollectionName;
                }

                // Lookup the number of proteins that should be in this protein collection
                var lengthCheckSql = "SELECT num_proteins FROM v_protein_collections " +
                                     "WHERE collection_name = '" + proteinCollectionName + "'";

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

                int currentFileProteinCount;

                do
                {
                    var sectionStart = currentCollectionPos;
                    var sectionEnd = sectionStart + 10000;

                    string collectionSql;
                    if (padWithPrimaryAnnotation)
                    {
                        proteinCollectionID = FindIdByName(trueName);
                        collectionSql =
                            "SELECT name, description, sequence, protein_id " +
                            "FROM v_protein_database_export " +
                            "WHERE " +
                                "protein_collection_id = " + proteinCollectionID + " " +
                                "AND sorting_index BETWEEN " + sectionStart + " AND " + sectionEnd + " " +
                            "ORDER BY sorting_index";
                    }
                    else
                    {
                        collectionSql =
                            "SELECT name, description, sequence, protein_id " +
                            "FROM v_protein_database_export " +
                            "WHERE protein_collection_id = " + proteinCollectionID + " " +
                                "AND annotation_type_id = " + alternateAnnotationTypeId + " " +
                                "AND sorting_index BETWEEN " + sectionStart + " AND " + sectionEnd + " " +
                            "ORDER BY sorting_index";
                    }

                    var collectionTable = mDatabaseAccessor.GetTable(collectionSql);

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

                    currentFileProteinCount = collectionTable.Rows.Count;

                    // collection.Tables.Add(collectionTable)
                    mFileDumper.Export(collectionTable, ref tempOutputFilePath);

                    currentCollectionPos = sectionEnd + 1;
                    currentCollectionCount += currentFileProteinCount;

                    var fractionDoneOverall = 0d;
                    if (collectionLength > 0)
                    {
                        fractionDoneOverall = proteinCollectionsExported / (double)protCollectionList.Count + currentCollectionCount / (double)collectionLength / protCollectionList.Count;
                    }

                    OnExportProgressUpdate(currentCollectionCount + " entries exported, collection " + (proteinCollectionsExported + 1) + " of " + protCollectionList.Count, fractionDoneOverall);
                }
                while (currentFileProteinCount > 0);

                proteinCollectionIDs.Add(proteinCollectionID.ToString("000000"));

                if (currentCollectionCount != collectionLength)
                {
                    throw new Exception(string.Format(
                        "The number of proteins exported for collection '{0}' does not match the expected value: " +
                        "{1} exported from t_protein_collection_members vs. {2} listed in t_protein_collections",
                        proteinCollectionName, currentCollectionCount, collectionLength));
                }

                proteinCollectionsExported++;
            }

            OnExportComplete(tempOutputFilePath);

            var tempOutputFile = new FileInfo(tempOutputFilePath);

            string name; // = hash;

            if (protCollectionList.Count > 1)
            {
                // Generate a filename composed of the protein collection IDs, separated by plus signs
                // For example, "002041+001810"

                name = string.Join("+", proteinCollectionIDs);
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

            tempOutputFile.MoveTo(FullOutputPath);

            // Assuming the final file now exists, delete the temporary file (if present)
            var finalOutputFile = new FileInfo(FullOutputPath);
            if (finalOutputFile.Exists)
            {
                var fileToDelete = new FileInfo(tempOutputFilePath);
                if (fileToDelete.Exists)
                {
                    fileToDelete.Delete();
                }
            }

            // Determine the CRC32 hash of the output file
            // This process will also rename the file, e.g. from "C:\Temp\SAR116_RBH_AA_012809_forward.fasta" to "C:\Temp\38FFACAC.fasta"
            var tempFullPath = FullOutputPath;
            var crc32Hash = mFileDumper.Export(new DataTable(), ref tempFullPath);
            FullOutputPath = tempFullPath;

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

            return mDatabaseAccessor.DataTableToDictionaryIntegerKeys(mCollectionsCache, "protein_collection_id", "collection_name");
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

            return mDatabaseAccessor.DataTableToDictionary(mCollectionsCache, "protein_collection_id", "collection_name", "organism_id = " + organismId);
        }

        public DataTable GetCollectionsByOrganismTable(int organismId)
        {
            var matchingProteinCollections = mCollectionsCache.Clone();

            foreach (var dataRow in mCollectionsCache.Select("organism_id = " + organismId))
            {
                matchingProteinCollections.ImportRow(dataRow);
            }

            return matchingProteinCollections;
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

            return mDatabaseAccessor.DataTableToDictionary(mOrganismCache, "organism_id", "name");
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
                mCollectionsCache = mDatabaseAccessor.GetTable(
                    "SELECT protein_collection_id, display, description, source, collection_state_id, state_name, collection_type_id, " +
                    "                type, num_proteins, num_residues, authentication_hash, collection_name, organism_id, authority_id, organism_name, " +
                    "                contents_encrypted, includes_contaminants, file_size_bytes " +
                    "FROM v_protein_collections_by_organism " +
                    "ORDER BY protein_collection_id");
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
                mOrganismCache = mDatabaseAccessor.GetTable("SELECT id as organism_id, short_name as name FROM v_organism_picker ORDER BY organism_id");
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
            var foundRows = mCollectionsCache.Select("collection_name = '" + collectionName + "'");
            if (foundRows.Length == 0)
            {
                RefreshCollectionCache();
                foundRows = mCollectionsCache.Select("collection_name = '" + collectionName + "'");
            }

            int id;
            try
            {
                id = Convert.ToInt32(foundRows[0]["protein_collection_id"]);
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
            var foundRows = mCollectionsCache.Select("protein_collection_id = " + collectionId).ToList();

            if (foundRows.Count == 0)
            {
                RefreshCollectionCache();
                foundRows = mCollectionsCache.Select("protein_collection_id = " + collectionId).ToList();
            }

            if (foundRows.Count > 0)
            {
                return foundRows[0]["Collection_Name"].ToString();
            }

            return string.Empty;
        }

        protected int FindPrimaryAnnotationId(int collectionId)
        {
            if (collectionId <= 0) throw new ArgumentOutOfRangeException(nameof(collectionId));
            var foundRows = mCollectionsCache.Select("protein_collection_id = " + collectionId).ToList();

            if (foundRows.Count > 0)
            {
                return Convert.ToInt32(foundRows[0]["primary_annotation_type_id"]);
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
            var foundRows = mCollectionsCache.Select("collection_name = '" + proteinCollectionName + "'");
            return foundRows[0]["authentication_hash"]?.ToString();
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