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
                var connectionStringCheck = databaseAccessor.ConnectionString.ToLower().Replace(" ", "");

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
            var checkSql = "SELECT Archived_File_ID, Archived_File_Path, IsNull(Protein_Collection_List, '') as Protein_Collection_List, IsNull(Collection_List_Hex_Hash, '') AS Collection_List_Hex_Hash " +
                "FROM T_Archived_Output_Files " +
                "WHERE Authentication_Hash = '" + sourceAuthenticationHash + "' AND " +
                "Archived_File_State_ID <> 3 " +
                "ORDER BY File_Modification_Date DESC";

            var tmpTable = DatabaseAccessor.GetTable(checkSql);
            var collectionListHexHash = GenerateHash(proteinCollectionsList + "/" + creationOptionsString);
            if (tmpTable.Rows.Count == 0)
            {
                var proteinCount = GetProteinCount(sourceFilePath);

                var archivePath = GenerateArchivePath(
                    sourceFilePath,
                    sourceAuthenticationHash,
                    archivedFileType, outputSequenceType);

                archivedFileEntryId = RunSP_AddOutputFileArchiveEntry(
                    proteinCollectionId, creationOptionsString, sourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount,
                    archivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType), proteinCollectionsList, collectionListHexHash);

                tmpTable = DatabaseAccessor.GetTable(checkSql);
            }
            else
            {
                // Archived file entry already exists

                archivedFileEntryId = Convert.ToInt32(tmpTable.Rows[0]["Archived_File_ID"]);
                var collectionListHexHashInDb = tmpTable.Rows[0]["Collection_List_Hex_Hash"].ToString();
                var proteinCollectionsListFromDb = tmpTable.Rows[0]["Protein_Collection_List"].ToString();

                if (tmpTable.Rows[0]["Protein_Collection_List"].GetType().Name == "DBNull" ||
                    string.IsNullOrEmpty(collectionListHexHashInDb) ||
                    proteinCollectionsListFromDb != (proteinCollectionsList ?? "") ||
                    collectionListHexHashInDb != (collectionListHexHash ?? ""))
                {
                    RunSP_UpdateFileArchiveEntryCollectionList(archivedFileEntryId, proteinCollectionsList, sourceAuthenticationHash, collectionListHexHash);
                }
            }

            mArchivedFilePath = tmpTable.Rows[0]["Archived_File_Path"].ToString();

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
            return BitConverter.ToString(sha1Hash).Replace("-", "").ToLower();
        }

        protected string GenerateArchivePath(
            string sourceFilePath,
            string authenticationHash,
            CollectionTypes archivedFileType,
            GetFASTAFromDMS.SequenceTypes outputSequenceType)
        {
            var pathString = Path.Combine(mBaseArchivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType));
            pathString = Path.Combine(pathString, Enum.GetName(typeof(GetFASTAFromDMS.SequenceTypes), outputSequenceType));
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

            var cmdSave = dbTools.CreateCommand("UpdateFileArchiveEntryCollectionList", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Archived_File_Entry_ID", SqlType.Int).Value = archivedFileEntryId;
            dbTools.AddParameter(cmdSave, "@ProteinCollectionList", SqlType.VarChar, 8000).Value = proteinCollectionsList;
            dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 28).Value = collectionListHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.Output;
            dbTools.AddParameter(cmdSave, "@CollectionListHexHash", SqlType.VarChar, 128).Value = collectionListHexHash;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);

            return ret;
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

            var cmdSave = dbTools.CreateCommand("AddOutputFileArchiveEntry", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@crc32_authentication", SqlType.VarChar, 40).Value = authenticationHash;
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

            mArchivedFilePath = archivedFileFullPath;

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }
    }
}