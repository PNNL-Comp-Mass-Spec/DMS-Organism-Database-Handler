using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class ArchiveToFile : ArchiveOutputFilesBase
    {
        private const string DEFAULT_BASE_ARCHIVE_PATH = @"\\gigasax\DMS_FASTA_File_Archive\";

        private readonly string mBaseArchivePath;
        private readonly SHA1Managed mSHA1Provider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor"></param>
        /// <param name="exporterModule"></param>
        public ArchiveToFile(DBTask databaseAccessor, GetFASTAFromDMS exporterModule)
            : base(databaseAccessor, exporterModule)
        {
            if (databaseAccessor == null)
            {
                mBaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH;
            }
            else
            {
                string connectionStringCheck = databaseAccessor.ConnectionString.ToLower().Replace(" ", "");

                if (connectionStringCheck.Contains("source=cbdms"))
                {
                    mBaseArchivePath = @"\\cbdms\DMS_FASTA_File_Archive\";
                }
                else
                {
                    mBaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH;
                }
            }

            mSHA1Provider = new SHA1Managed();
        }

        protected override int DispositionFile(
            int proteinCollectionID,
            string sourceFilePath,
            string creationOptionsString,
            string sourceAuthenticationHash,
            GetFASTAFromDMS.SequenceTypes outputSequenceType,
            CollectionTypes archivedFileType,
            string proteinCollectionsList)
        {
            int ArchivedFileEntryID;

            var fi = new FileInfo(sourceFilePath);

            // Check for existence of Archive Entry
            var checkSQL = "SELECT Archived_File_ID, Archived_File_Path, IsNull(Protein_Collection_List, '') as Protein_Collection_List, IsNull(Collection_List_Hex_Hash, '') AS Collection_List_Hex_Hash " +
                "FROM T_Archived_Output_Files " +
                "WHERE Authentication_Hash = '" + sourceAuthenticationHash + "' AND " +
                "Archived_File_State_ID <> 3 " +
                "ORDER BY File_Modification_Date DESC";

            var tmpTable = mDatabaseAccessor.GetTable(checkSQL);
            var CollectionListHexHash = GenerateHash(proteinCollectionsList + "/" + creationOptionsString);
            if (tmpTable.Rows.Count == 0)
            {
                var proteinCount = GetProteinCount(sourceFilePath);

                var archivePath = GenerateArchivePath(
                    sourceFilePath,
                    sourceAuthenticationHash,
                    archivedFileType, outputSequenceType);

                ArchivedFileEntryID = RunSP_AddOutputFileArchiveEntry(
                    proteinCollectionID, creationOptionsString, sourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount,
                    archivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType), proteinCollectionsList, CollectionListHexHash);

                tmpTable = mDatabaseAccessor.GetTable(checkSQL);
            }
            else
            {
                // Archived file entry already exists

                ArchivedFileEntryID = Convert.ToInt32(tmpTable.Rows[0]["Archived_File_ID"]);
                var CollectionListHexHashInDB = tmpTable.Rows[0]["Collection_List_Hex_Hash"].ToString();
                var ProteinCollectionsListFromDB = tmpTable.Rows[0]["Protein_Collection_List"].ToString();

                if (tmpTable.Rows[0]["Protein_Collection_List"].GetType().Name == "DBNull" ||
                    string.IsNullOrEmpty(CollectionListHexHashInDB) ||
                    (ProteinCollectionsListFromDB ?? "") != (proteinCollectionsList ?? "") ||
                    (CollectionListHexHashInDB ?? "") != (CollectionListHexHash ?? ""))
                {
                    RunSP_UpdateFileArchiveEntryCollectionList(ArchivedFileEntryID, proteinCollectionsList, sourceAuthenticationHash, CollectionListHexHash);
                }
            }

            mArchived_File_Name = tmpTable.Rows[0]["Archived_File_Path"].ToString();

            try
            {
                var di = new DirectoryInfo(Path.GetDirectoryName(mArchived_File_Name));
                var destFI = new FileInfo(mArchived_File_Name);
                if (!di.Exists)
                {
                    di.Create();
                }

                if (!destFI.Exists)
                {
                    fi.CopyTo(mArchived_File_Name);
                }
            }
            catch (UnauthorizedAccessException exUnauthorized)
            {
                Console.WriteLine("  Warning: access denied copying file to " + mArchived_File_Name);
            }
            catch (Exception ex)
            {
                mLastError = "File copying error: " + ex.Message;
                return 0;
            }

            return ArchivedFileEntryID;
        }

        protected string GenerateHash(string sourceText)
        {
            // Create an encoding object to ensure the encoding standard for the source text
            var encoding = new ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var byteSourceText = encoding.GetBytes(sourceText);

            // Compute the hash value from the source
            var sha1Hash = mSHA1Provider.ComputeHash(byteSourceText);

            // And convert it to String format for return
            string sha1String = BitConverter.ToString(sha1Hash).Replace("-", "").ToLower();

            return sha1String;
        }

        protected string GenerateArchivePath(
            string sourceFilePath,
            string authentication_Hash,
            CollectionTypes archivedFileType,
            GetFASTAFromDMS.SequenceTypes outputSequenceType)
        {
            var pathString = Path.Combine(mBaseArchivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType));
            pathString = Path.Combine(pathString, Enum.GetName(typeof(GetFASTAFromDMS.SequenceTypes), outputSequenceType));
            pathString = Path.Combine(pathString, "ID_00000_" + authentication_Hash + Path.GetExtension(sourceFilePath));

            return pathString;
        }

        protected int RunSP_UpdateFileArchiveEntryCollectionList(
            int archivedFileEntryID,
            string proteinCollectionsList,
            string collectionListHash,
            string collectionListHexHash)
        {
            var dbTools = mDatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateFileArchiveEntryCollectionList", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Archived_File_Entry_ID", SqlType.Int).Value = archivedFileEntryID;
            dbTools.AddParameter(cmdSave, "@ProteinCollectionList", SqlType.VarChar, 8000).Value = proteinCollectionsList;
            dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 28).Value = collectionListHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.Output;
            dbTools.AddParameter(cmdSave, "@CollectionListHexHash", SqlType.VarChar, 128).Value = collectionListHexHash;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddOutputFileArchiveEntry(
            int proteinCollectionID,
            string creationOptionsString,
            string authentication_Hash,
            DateTime fileModificationDate,
            long outputFileSize,
            int proteinCount,
            string archivedFileFullPath,
            string archivedFileType,
            string proteinCollectionsList,
            string collectionListHexHash)
        {
            var dbTools = mDatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddOutputFileArchiveEntry", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionID;
            dbTools.AddParameter(cmdSave, "@crc32_authentication", SqlType.VarChar, 40).Value = authentication_Hash;
            dbTools.AddParameter(cmdSave, "@file_modification_date", SqlType.DateTime).Value = fileModificationDate;
            dbTools.AddParameter(cmdSave, "@file_size", SqlType.BigInt).Value = outputFileSize;
            dbTools.AddParameter(cmdSave, "@protein_count", SqlType.Int).Value = proteinCount;
            dbTools.AddParameter(cmdSave, "@archived_file_type", SqlType.VarChar, 64).Value = archivedFileType;
            dbTools.AddParameter(cmdSave, "@creation_options", SqlType.VarChar, 250).Value = creationOptionsString;
            dbTools.AddParameter(cmdSave, "@protein_collection_string", SqlType.VarChar, 8000).Value = proteinCollectionsList;
            dbTools.AddParameter(cmdSave, "@collection_string_hash", SqlType.VarChar, 40).Value = collectionListHexHash;
            dbTools.AddParameter(cmdSave, "@archived_file_path", SqlType.VarChar, 250).Value = archivedFileFullPath;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.Output;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            mArchived_File_Name = archivedFileFullPath;

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }
    }
}