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
        // Ignore Spelling: Accessor, cbdms

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

            var collectionListHexHash = GenerateHash(proteinCollectionsList + "/" + creationOptionsString);

            // Check for existence of Archive Entry
            var checkSql =
                "SELECT archived_file_id, archived_file_path, coalesce(protein_collection_list, '') as protein_collection_list, coalesce(collection_list_hex_hash, '') as collection_list_hex_hash " +
                "FROM t_archived_output_files " +
                "WHERE authentication_hash = '" + sourceAuthenticationHash + "' AND " +
                "      collection_list_hex_hash = '" + collectionListHexHash + "' AND " +
                "      archived_file_state_id <> 3 " +
                "ORDER BY archived_file_id DESC";

            var queryResultsA = DatabaseAccessor.GetTable(checkSql);

            DataTable queryResults;

            if (queryResultsA.Rows.Count > 0)
            {
                queryResults = queryResultsA;
            }
            else
            {
                // Wait between 2 and 6 seconds and check again in case another manager is also trying to create the same protein collection

                var randomGenerator = new Random();
                var sleepTimeSeconds = 2 + randomGenerator.NextDouble() * 4;

                PRISM.ConsoleMsgUtils.ShowDebug(
                    "Sleeping for {0:0.0} seconds, then re-checking table t_archived_output_files for hash {1}",
                    sleepTimeSeconds, sourceAuthenticationHash);

                PRISM.AppUtils.SleepMilliseconds((int)Math.Round(sleepTimeSeconds * 1000, 0));

                queryResults = DatabaseAccessor.GetTable(checkSql);
            }

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

        /// <summary>
        /// Updates the protein collection list and hash values in T_Archived_Output_Files for the given archived output file
        /// </summary>
        /// <param name="archivedFileEntryId">Archive output file ID</param>
        /// <param name="proteinCollectionsList">Protein collection list (comma-separated list of protein collection names)</param>
        /// <param name="authenticationHash">CRC32 authentication hash (hash of the bytes in the file)</param>
        /// <param name="collectionListHexHash">SHA-1 hash of the protein collection list and creation options (separated by a forward slash)</param>
        /// <returns>0 if no error; return code if an error</returns>
        protected int RunSP_UpdateFileArchiveEntryCollectionList(
            int archivedFileEntryId,
            string proteinCollectionsList,
            string authenticationHash,
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
            dbTools.AddParameter(cmdSave, "@crc32Authentication", SqlType.VarChar, 28).Value = authenticationHash;
            dbTools.AddParameter(cmdSave, "@collectionListHexHash", SqlType.VarChar, 128).Value = collectionListHexHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.InputOutput;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code
            return DBToolsBase.GetReturnCode(returnParam);
        }

        // ReSharper disable CommentTypo

        /// <summary>
        /// Adds an archive output file to table T_Archived_Output_Files
        /// </summary>
        /// <param name="proteinCollectionId">Protein collection ID</param>
        /// <param name="creationOptionsString">Creation options (e.g. 'seq_direction=forward,filetype=fasta')</param>
        /// <param name="authenticationHash">CRC32 authentication hash (hash of the bytes in the file)</param>
        /// <param name="fileModificationDate">File modification timestamp</param>
        /// <param name="outputFileSize">File size, in bytes</param>
        /// <param name="proteinCount">Protein count</param>
        /// <param name="archivedFileFullPath">Full path to th file</param>
        /// <param name="archivedFileType">Archived file type ('static' if a single protein collection; 'dynamic' if a combination of multiple protein collections)</param>
        /// <param name="proteinCollectionsList">Protein collection list (comma-separated list of protein collection names)</param>
        /// <param name="collectionListHexHash">SHA-1 hash of the protein collection list and creation options (separated by a forward slash)</param>
        /// <remarks>
        /// Example for collectionListHexHash:
        /// The SHA-1 hash of 'H_sapiens_UniProt_SPROT_2023-03-01,Tryp_Pig_Bov/seq_direction=forward,filetype=fasta'
        /// is '11822db6bbfc1cb23c0a728a0b53c3b9d97db1f5'
        /// </remarks>
        /// <returns>Archived output file ID of the newly added row</returns>
        // ReSharper restore CommentTypo
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

            var archivedFilePathParam = dbTools.AddParameter(cmdSave, "@archivedFilePath", SqlType.VarChar, 250);
            archivedFilePathParam.Direction = ParameterDirection.InputOutput;
            archivedFilePathParam.Value = archivedFileFullPath;

            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.InputOutput;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            mArchivedFilePath = archivedFilePathParam.Value.ToString();

            // The return code is an integer on SQL Server, but is text on Postgres
            // The return code will be the archived file id if no errors; it will be 0 if an error

            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code
            return DBToolsBase.GetReturnCode(returnParam);
        }
    }
}