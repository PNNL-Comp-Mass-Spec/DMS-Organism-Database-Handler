using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using TableManipulationBase;

namespace Protein_Exporter
{
    public class GetFASTAFromDMSForward
    {
        private readonly DBTask m_DatabaseAccessor;
        private readonly ExportProteins m_fileDumper;

        /// <summary>
        /// Keys are protein collection IDs
        /// Values are filename
        /// </summary>
        private Dictionary<int, string> m_AllCollections;

        private Dictionary<string, string> m_OrganismList;

        private string m_CurrentFullOutputPath;
        private int m_CurrentFileProteinCount;
        private string m_CurrentArchiveFileName;

        private DataTable m_CollectionsCache;
        private DataTable m_OrganismCache;

        protected string m_Naming_Suffix = "_forward";
        private string m_Extension = "";

        private RijndaelEncryptionHandler m_RijndaelDecryption;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        public GetFASTAFromDMSForward(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
        {
            m_DatabaseAccessor = databaseAccessor;
            m_AllCollections = GetCollectionNameList();
            m_OrganismList = GetOrganismList();

            switch (databaseFormatType)
            {
                case GetFASTAFromDMS.DatabaseFormatTypes.fasta:
                    m_fileDumper = new ExportProteinsFASTA(this);
                    m_Extension = ".fasta";
                    break;

                case GetFASTAFromDMS.DatabaseFormatTypes.fastapro:
                    m_fileDumper = new ExportProteinsXTFASTA(this);
                    m_Extension = ".fasta.pro";
                    break;
            }

            if (m_fileDumper != null)
            {
                m_fileDumper.ExportStart += OnExportStart;
                m_fileDumper.ExportProgress += OnExportProgressUpdate;
            }
        }

        public event FileGenerationCompletedEventHandler FileGenerationCompleted;

        public delegate void FileGenerationCompletedEventHandler(string outputPath);

        public event FileGenerationProgressEventHandler FileGenerationProgress;

        public delegate void FileGenerationProgressEventHandler(string statusMsg, double fractionDone);

        public event FileGenerationStartedEventHandler FileGenerationStarted;

        public delegate void FileGenerationStartedEventHandler(string taskMsg);

        public string FullOutputPath
        {
            get => m_CurrentFullOutputPath;
            set => m_CurrentFullOutputPath = value;
        }

        // Unused
        // public readonly string ArchivalName => m_CurrentArchiveFileName;

        protected virtual string ExtendedExportPath(
            string destinationFolderPath,
            string proteinCollectionName)
        {
            return Path.Combine(destinationFolderPath, proteinCollectionName + m_Naming_Suffix + m_Extension);
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
        /// <param name="alternateAnnotationTypeID"></param>
        /// <param name="padWithPrimaryAnnotation"></param>
        /// <returns>CRC32 hash for the file</returns>
        public virtual string ExportFASTAFile(
            List<string> protCollectionList,
            string destinationFolderPath,
            int alternateAnnotationTypeID,
            bool padWithPrimaryAnnotation)
        {
            string collectionSQL;
            DataTable collectionTable;

            string authorizationSQL;
            DataTable authorizationTable;

            string passPhraseSQL;
            DataTable passPhraseTable;

            DataRow[] encCheckRows;
            DataRow[] authCheckRows;

            string trueName = string.Empty;

            var tmpID = default(int);
            var tmpIDListSB = new StringBuilder();
            string cipherSeq;
            string clearSeq;

            var nameCheckRegex = new Regex(@"(?<collectionname>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))");

            if (!CheckProteinCollectionNameValidity(protCollectionList))
            {
                return "";
            }

            Match m;

            var user = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            string user_ID = user.Identity.Name;

            // Dictionary mapping protein collection name to the associated passphrase
            var proteinCollectionPassphrases = new Dictionary<string, string>();

            string collectionNameList = string.Empty;

            // Check each collection name for encryption of contents
            foreach (string nameString in protCollectionList)
            {
                encCheckRows = m_CollectionsCache.Select("Filename = '" + nameString + "' AND Contents_Encrypted > 0");

                if (encCheckRows.Length > 0)
                {
                    // Determine the encrypted collections to which this user has access
                    authorizationSQL = "SELECT Protein_Collection_ID, Protein_Collection_Name " +
                        "FROM V_Encrypted_Collection_Authorizations " +
                        "WHERE Login_Name = '" + user_ID + "'";

                    authorizationTable = m_DatabaseAccessor.GetTable(authorizationSQL);
                    authCheckRows = authorizationTable.Select("Protein_Collection_Name = '" + nameString + "' OR Protein_Collection_Name = 'Administrator'");
                    if (authCheckRows.Length > 0)
                    {
                        tmpID = FindIDByName(nameString);
                        passPhraseSQL = "SELECT Passphrase " +
                            "FROM T_Encrypted_Collection_Passphrases " +
                            "WHERE Protein_Collection_ID = " + tmpID.ToString();
                        passPhraseTable = m_DatabaseAccessor.GetTable(passPhraseSQL);

                        proteinCollectionPassphrases.Add(nameString, passPhraseTable.Rows[0]["Passphrase"].ToString());
                    }
                    else
                    {
                        throw new Exception("User " + user_ID + " does not have access to the encrypted collection '" + nameString + "'");
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
                string tmpOutputPathCandidate = Path.GetTempFileName();
                try
                {
                    // The GetTempFileName function created a temp file that we don't need; delete it now (but use try/catch just in case the deletion fails for some freak reason)
                    File.Delete(tmpOutputPathCandidate);
                }
                catch (Exception ex)
                {
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

            int proteinCollectionsExported = 0;
            foreach (var proteinCollName in protCollectionList)
            {
                int currentCollectionPos = 0;
                int currentCollectionCount = 0;

                // Make sure there are no leading or trailing spaces
                var ProteinCollectionName = proteinCollName.Trim();

                if (nameCheckRegex.IsMatch(ProteinCollectionName))
                {
                    m = nameCheckRegex.Match(ProteinCollectionName);
                    trueName = m.Groups["collectionname"].Value;
                }
                else
                {
                    trueName = ProteinCollectionName;
                }

                // Lookup the number of proteins that should be in this protein collection
                var lengthCheckSQL = "SELECT NumProteins FROM T_Protein_Collections " +
                                     "WHERE FileName = '" + ProteinCollectionName + "'";

                var lengthCheckTable = m_DatabaseAccessor.GetTable(lengthCheckSQL);
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
                    int sectionStart = currentCollectionPos;
                    int sectionEnd = sectionStart + 10000;

                    if (padWithPrimaryAnnotation)
                    {
                        tmpID = FindIDByName(trueName);
                        collectionSQL =
                            "SELECT Name, Description, Sequence, Protein_ID " +
                            "FROM V_Protein_Database_Export " +
                            "WHERE " +
                                "Protein_Collection_ID = " + tmpID.ToString() + " " +
                                "AND Sorting_Index BETWEEN " + sectionStart.ToString() + " AND " + sectionEnd.ToString() + " " +
                            "ORDER BY Sorting_Index";
                    }
                    else
                    {
                        collectionSQL =
                            "SELECT Name, Description, Sequence, Protein_ID " +
                            "FROM V_Protein_Database_Export " +
                            "WHERE Protein_Collection_ID = " + tmpID.ToString() + ") " +
                                "AND Annotation_Type_ID = " + alternateAnnotationTypeID + " " +
                                "AND Sorting_Index BETWEEN " + sectionStart.ToString() + " AND " + sectionEnd.ToString() + " " +
                            "ORDER BY Sorting_Index";
                    }

                    collectionTable = m_DatabaseAccessor.GetTable(collectionSQL);

                    string passPhraseForCollection = "";
                    if (proteinCollectionPassphrases.TryGetValue(trueName, out passPhraseForCollection))
                    {
                        m_RijndaelDecryption = new RijndaelEncryptionHandler(passPhraseForCollection);
                        foreach (DataRow decryptionRow in collectionTable.Rows)
                        {
                            cipherSeq = decryptionRow["Sequence"].ToString();
                            clearSeq = m_RijndaelDecryption.Decrypt(cipherSeq);
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

                    m_CurrentFileProteinCount = collectionTable.Rows.Count;

                    // collection.Tables.Add(collectionTable)
                    m_fileDumper.Export(collectionTable, ref tmpOutputPath);

                    currentCollectionPos = sectionEnd + 1;
                    currentCollectionCount += collectionTable.Rows.Count;

                    double fractionDoneOverall = 0d;
                    if (collectionLength > 0)
                    {
                        fractionDoneOverall = proteinCollectionsExported / (double)protCollectionList.Count + currentCollectionCount / (double)collectionLength / protCollectionList.Count;
                    }

                    OnExportProgressUpdate(currentCollectionCount + " entries exported, collection " + (proteinCollectionsExported + 1) + " of " + protCollectionList.Count, fractionDoneOverall);
                }
                while (collectionTable.Rows.Count != 0);

                tmpIDListSB.Append(tmpID.ToString("000000"));
                tmpIDListSB.Append("+");
                if (currentCollectionCount != collectionLength)
                {
                    throw new Exception(string.Format(
                        "The number of proteins exported for collection '{0}' does not match the expected value: " +
                        "{1} exported from T_Protein_Collection_Members vs. {2} listed in T_Protein_Collections",
                        ProteinCollectionName, currentCollectionCount, collectionLength));
                }

                proteinCollectionsExported += 1;
            }

            OnExportComplete(tmpOutputPath);

            var tmpFI = new FileInfo(tmpOutputPath);

            tmpIDListSB.Remove(tmpIDListSB.Length - 1, 1);
            string name; // = hash;

            if (protCollectionList.Count > 1)
            {
                name = tmpIDListSB.ToString();
                if (destinationFolderPath.Length + name.Length > 225)
                {
                    // If exporting a large number of protein collections, name can be very long
                    // This can lead to error: The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters
                    // Thus, truncate name
                    int intMaxNameLength;
                    intMaxNameLength = 225 - destinationFolderPath.Length;
                    if (intMaxNameLength < 30)
                        intMaxNameLength = 30;

                    name = name.Substring(0, intMaxNameLength);

                    // Find the last plus sign and truncate just before it
                    int intLastPlusLocation;
                    intLastPlusLocation = name.LastIndexOf('+');
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

            m_CurrentFullOutputPath = ExtendedExportPath(destinationFolderPath, name);
            m_CurrentArchiveFileName = name;

            // Rename (move) the temporary file to the final, full name
            if (File.Exists(m_CurrentFullOutputPath))
            {
                File.Delete(m_CurrentFullOutputPath);
            }

            tmpFI.MoveTo(m_CurrentFullOutputPath);

            // Assuming the final file now exists, delete the temporary file (if present)
            var finalOutputFile = new FileInfo(m_CurrentFullOutputPath);
            if (finalOutputFile.Exists)
            {
                tmpFI = new FileInfo(tmpOutputPath);
                if (tmpFI.Exists)
                {
                    tmpFI.Delete();
                }
            }

            // Determine the CRC32 hash of the output file
            // This process will also rename the file, e.g. from "C:\Temp\SAR116_RBH_AA_012809_forward.fasta" to "C:\Temp\38FFACAC.fasta"
            string crc32Hash = m_fileDumper.Export(new DataTable(), ref m_CurrentFullOutputPath);

            OnExportComplete(m_CurrentFullOutputPath);

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
            int primaryAuthorityID = 1;
            const bool padWithPrimaryAnnotation = true;

            return ExportFASTAFile(protCollectionList, destinationFolderPath, primaryAuthorityID, padWithPrimaryAnnotation);
        }

        protected bool CheckProteinCollectionNameValidity(List<string> protCollectionList)
        {
            int id;
            foreach (var name in protCollectionList)
            {
                id = FindIDByName(name);
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
        //     var foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " + proteinCollectionID.ToString()).ToList();
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
            if (m_DatabaseAccessor == null)
            {
                return new Dictionary<int, string>();
            }

            if (m_CollectionsCache == null)
            {
                RefreshCollectionCache();
            }

            return m_DatabaseAccessor.DataTableToDictionaryIntegerKeys(m_CollectionsCache, "Protein_Collection_ID", "FileName");
        }

        public Dictionary<string, string> GetCollectionsByOrganism(int organismID)
        {
            if (m_DatabaseAccessor == null)
            {
                return new Dictionary<string, string>();
            }

            if (m_CollectionsCache == null)
            {
                RefreshCollectionCache();
            }

            return m_DatabaseAccessor.DataTableToDictionary(m_CollectionsCache, "Protein_Collection_ID", "FileName", "[Organism_ID] = " + organismID.ToString());
        }

        public DataTable GetCollectionsByOrganismTable(int organismID)
        {
            var tmpTable = m_CollectionsCache.Clone();

            var foundRows = m_CollectionsCache.Select("[Organism_ID] = " + organismID.ToString());

            foreach (var dr in foundRows)
                tmpTable.ImportRow(dr);

            return tmpTable;
        }

        public Dictionary<string, string> GetOrganismList()
        {
            if (m_DatabaseAccessor == null)
            {
                return new Dictionary<string, string>();
            }

            if (m_OrganismCache == null)
            {
                RefreshOrganismCache();
            }

            return m_DatabaseAccessor.DataTableToDictionary(m_OrganismCache, "Organism_ID", "Name");
        }

        public DataTable GetOrganismListTable()
        {
            if (m_DatabaseAccessor == null)
            {
                return new DataTable();
            }

            if (m_OrganismCache == null)
            {
                RefreshOrganismCache();
            }

            return m_OrganismCache;
        }

        protected void RefreshCollectionCache()
        {
            if (m_DatabaseAccessor == null || string.IsNullOrWhiteSpace(m_DatabaseAccessor.ConnectionString))
            {
                m_CollectionsCache = new DataTable();
            }
            else
            {
                m_CollectionsCache = m_DatabaseAccessor.GetTable("SELECT * FROM V_Protein_Collections_By_Organism ORDER BY Protein_Collection_ID");
            }
        }

        protected void RefreshOrganismCache()
        {
            if (m_DatabaseAccessor == null || string.IsNullOrWhiteSpace(m_DatabaseAccessor.ConnectionString))
            {
                m_OrganismCache = new DataTable();
            }
            else
            {
                m_OrganismCache = m_DatabaseAccessor.GetTable("SELECT ID as Organism_ID, Short_Name as Name FROM V_Organism_Picker ORDER BY Organism_ID");
            }
        }

        public int FindIDByName(string collectionName)
        {
            if (collectionName.Length == 0)
            {
                return 0;
            }

            DataRow[] foundRows;

            // Make sure there are no leading or trailing spaces
            collectionName = collectionName.Trim();
            foundRows = m_CollectionsCache.Select("[FileName] = '" + collectionName + "'");
            if (foundRows.Length == 0)
            {
                RefreshCollectionCache();
                foundRows = m_CollectionsCache.Select("[FileName] = '" + collectionName + "'");
            }

            int id;
            try
            {
                id = Convert.ToInt32(foundRows[0]["Protein_Collection_ID"]);
            }
            catch (Exception ex)
            {
                id = -1;
            }

            return id;
        }

        public string FindNameByID(int collectionID)
        {
            var foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " + collectionID.ToString()).ToList();

            if (foundRows.Count == 0)
            {
                RefreshCollectionCache();
                foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " + collectionID.ToString()).ToList();
            }

            if (foundRows.Count > 0)
            {
                return foundRows[0]["FileName"].ToString();
            }

            return string.Empty;
        }

        protected int FindPrimaryAnnotationID(int collectionID)
        {
            var foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " + collectionID.ToString()).ToList();

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
            return m_fileDumper.GenerateFileAuthenticationHash(fullFilePath);
        }

        public string GetStoredHash(string proteinCollectionName)
        {
            var foundRows = m_CollectionsCache.Select("[FileName] = '" + proteinCollectionName + "'");
            return foundRows[0]["Authentication_Hash"]?.ToString();
        }

        public string GetStoredHash(int proteinCollectionID)
        {
            string ProteinCollectionName = m_AllCollections[proteinCollectionID];
            return GetStoredHash(ProteinCollectionName);
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