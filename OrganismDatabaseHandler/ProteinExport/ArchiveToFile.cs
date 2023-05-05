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
        // Ignore Spelling: cbdms

        private const string DefaultBaseArchivePath = @"\\gigasax\DMS_FASTA_File_Archive\";

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
                mBaseArchivePath = DefaultBaseArchivePath;
            }
            else
            {
                var connectionStringCheck = databaseAccessor.ConnectionString.ToLower().Replace(" ", string.Empty);

                if (connectionStringCheck.Contains("source=cbdms"))
                {
                    mBaseArchivePath = @"\\cbdms\DMS_FASTA_File_Archive\";
                }
                else
                {
                    mBaseArchivePath = DefaultBaseArchivePath;
                }
            }

            mSHA1Provider = new SHA1Managed();
        }

        protected override int DispositionFile(
            int proteinCollectionId,
            string sourceFilePath,
            string creationOptionsString,
            string sourceAuthenticationHash,
            GetFASTAFromDMS.SequenceTypes outputSequenceType,
            CollectionTypes archivedFileType,
            string proteinCollectionsList)
        {
            int archivedFileEntryId;

            var fi = new FileInfo(sourceFilePath);

            // Check for existence of Archive Entry
            var checkSql =
                "SELECT archived_file_id, archived_file_path, coalesce(protein_collection_list, '') as protein_collection_list, coalesce(collection_list_hex_hash, '') as collection_list_hex_hash " +
                "FROM t_archived_output_files " +
                "WHERE authentication_hash = '" + sourceAuthenticationHash + "' AND " +
                "      archived_file_state_id <> 3 " +
                "ORDER BY file_modification_date DESC";

            var queryResults = DatabaseAccessor.GetTable(checkSql);
            var collectionListHexHash = GenerateHash(proteinCollectionsList + "/" + creationOptionsString);

            DataTable archivedOutputFileData;

            if (queryResults.Rows.Count == 0)
            {
                // Add a new entry to T_Archived_Output_Files

                var proteinCount = GetProteinCount(sourceFilePath);

                var archivePath = GenerateArchivePath(
                    sourceFilePath,
                    sourceAuthenticationHash,
                    archivedFileType, outputSequenceType);

                archivedFileEntryId = RunSP_AddOutputFileArchiveEntry(
                    proteinCollectionId, creationOptionsString, sourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount,
                    archivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType), proteinCollectionsList, collectionListHexHash);

                archivedOutputFileData = DatabaseAccessor.GetTable(checkSql);
            }
            else
            {
                // Archived file entry already exists
                archivedOutputFileData = queryResults;

                archivedFileEntryId = Convert.ToInt32(archivedOutputFileData.Rows[0]["Archived_File_ID"]);
                var collectionListHexHashInDb = archivedOutputFileData.Rows[0]["Collection_List_Hex_Hash"].ToString();
                var proteinCollectionsListFromDb = archivedOutputFileData.Rows[0]["Protein_Collection_List"].ToString();

                if (archivedOutputFileData.Rows[0]["Protein_Collection_List"].GetType().Name == "DBNull" ||
                    string.IsNullOrEmpty(collectionListHexHashInDb) ||
                    proteinCollectionsListFromDb != (proteinCollectionsList ?? string.Empty) ||
                    collectionListHexHashInDb != (collectionListHexHash ?? string.Empty))
                {
                    RunSP_UpdateFileArchiveEntryCollectionList(archivedFileEntryId, proteinCollectionsList, sourceAuthenticationHash, collectionListHexHash);
                }
            }

            mArchivedFilePath = archivedOutputFileData.Rows[0]["Archived_File_Path"].ToString();

            try
            {
                var destinationFile = new FileInfo(mArchivedFilePath);

                // ReSharper disable once MergeIntoPattern
                if (destinationFile.Directory != null && !destinationFile.Directory.Exists)
                {
                    destinationFile.Directory.Create();
                }

                if (!destinationFile.Exists)
                {
                    fi.CopyTo(mArchivedFilePath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("  Warning: access denied copying file to " + mArchivedFilePath);
            }
            catch (Exception ex)
            {
                LastError = "File copying error: " + ex.Message;
                return 0;
            }

            return archivedFileEntryId;
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
            return BitConverter.ToString(sha1Hash).Replace("-", string.Empty).ToLower();
        }

        protected string GenerateArchivePath(
            string sourceFilePath,
            string authenticationHash,
            CollectionTypes archivedFileType,
            GetFASTAFromDMS.SequenceTypes outputSequenceType)
        {
            var pathString = Path.Combine(mBaseArchivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType) ?? string.Empty);
            pathString = Path.Combine(pathString, Enum.GetName(typeof(GetFASTAFromDMS.SequenceTypes), outputSequenceType) ?? string.Empty);
            pathString = Path.Combine(pathString, "ID_00000_" + authenticationHash + Path.GetExtension(sourceFilePath));

            return pathString;
        }

        protected int RunSP_UpdateFileArchiveEntryCollectionList(
            int archivedFileEntryId,
            string proteinCollectionsList,
            string collectionListHash,
            string collectionListHexHash)
        {
            var dbTools = DatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("update_file_archive_entry_collection_list", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@archivedFileEntryID", SqlType.Int).Value = archivedFileEntryId;
            dbTools.AddParameter(cmdSave, "@proteinCollectionList", SqlType.VarChar, 8000).Value = proteinCollectionsList;
            dbTools.AddParameter(cmdSave, "@sha1Hash", SqlType.VarChar, 28).Value = collectionListHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.InputOutput;
            dbTools.AddParameter(cmdSave, "@collectionListHexHash", SqlType.VarChar, 128).Value = collectionListHexHash;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddOutputFileArchiveEntry(
            int proteinCollectionId,
            string creationOptionsString,
            string authenticationHash,
            DateTime fileModificationDate,
            long outputFileSize,
            int proteinCount,
            string archivedFileFullPath,
            string archivedFileType,
            string proteinCollectionsList,
            string collectionListHexHash)
        {
            var dbTools = DatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_output_file_archive_entry", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@proteinCollectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@crc32Authentication", SqlType.VarChar, 40).Value = authenticationHash;
            dbTools.AddParameter(cmdSave, "@fileModificationDate", SqlType.DateTime).Value = fileModificationDate;
            dbTools.AddParameter(cmdSave, "@fileSize", SqlType.BigInt).Value = outputFileSize;
            dbTools.AddParameter(cmdSave, "@proteinCount", SqlType.Int).Value = proteinCount;
            dbTools.AddParameter(cmdSave, "@archivedFileType", SqlType.VarChar, 64).Value = archivedFileType;
            dbTools.AddParameter(cmdSave, "@creationOptions", SqlType.VarChar, 250).Value = creationOptionsString;
            dbTools.AddParameter(cmdSave, "@proteinCollectionString", SqlType.VarChar, 8000).Value = proteinCollectionsList;
            dbTools.AddParameter(cmdSave, "@collectionStringHash", SqlType.VarChar, 40).Value = collectionListHexHash;
            dbTools.AddParameter(cmdSave, "@archivedFilePath", SqlType.VarChar, 250).Value = archivedFileFullPath;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.InputOutput;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            mArchivedFilePath = archivedFileFullPath;

            // The return code is an integer on SQL Server, but is text on Postgres
            // The return code will be the archived file id if no errors; it will be 0 if an error

            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code
            return DBToolsBase.GetReturnCode(returnParam);
        }
    }
}