using System;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using PRISMDatabaseUtils;
using TableManipulationBase;

namespace Protein_Exporter
{
    public class ArchiveToFile : ArchiveOutputFilesBase
    {
        private const string DEFAULT_BASE_ARCHIVE_PATH = @"\\gigasax\DMS_FASTA_File_Archive\";

        protected readonly string m_BaseArchivePath;
        protected readonly SHA1Managed m_SHA1Provider;

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
                m_BaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH;
            }
            else
            {
                string connectionStringCheck = databaseAccessor.ConnectionString.ToLower().Replace(" ", "");

                if (connectionStringCheck.Contains("source=cbdms"))
                {
                    m_BaseArchivePath = @"\\cbdms\DMS_FASTA_File_Archive\";
                }
                else
                {
                    m_BaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH;
                }
            }

            m_SHA1Provider = new SHA1Managed();
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
            string CollectionListHexHash;
            string CollectionListHexHashInDB;

            string ProteinCollectionsListFromDB;

            int ArchivedFileEntryID;

            string archivePath;
            var fi = new FileInfo(sourceFilePath);
            FileInfo destFI;
            DirectoryInfo di;

            int proteinCount;

            // Check for existence of Archive Entry
            var checkSQL = "SELECT Archived_File_ID, Archived_File_Path, IsNull(Protein_Collection_List, '') as Protein_Collection_List, IsNull(Collection_List_Hex_Hash, '') AS Collection_List_Hex_Hash " +
                "FROM T_Archived_Output_Files " +
                "WHERE Authentication_Hash = '" + sourceAuthenticationHash + "' AND " +
                "Archived_File_State_ID <> 3 " +
                "ORDER BY File_Modification_Date DESC";

            var tmpTable = m_DatabaseAccessor.GetTable(checkSQL);
            CollectionListHexHash = GenerateHash(proteinCollectionsList + "/" + creationOptionsString);
            if (tmpTable.Rows.Count == 0)
            {
                proteinCount = GetProteinCount(sourceFilePath);

                archivePath = GenerateArchivePath(
                    sourceFilePath,
                    sourceAuthenticationHash,
                    archivedFileType, outputSequenceType);

                ArchivedFileEntryID = RunSP_AddOutputFileArchiveEntry(
                    proteinCollectionID, creationOptionsString, sourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount,
                    archivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType), proteinCollectionsList, CollectionListHexHash);

                tmpTable = m_DatabaseAccessor.GetTable(checkSQL);
            }
            else
            {
                // Archived file entry already exists

                ArchivedFileEntryID = Conversions.ToInteger(tmpTable.Rows[0]["Archived_File_ID"]);
                CollectionListHexHashInDB = Conversions.ToString(tmpTable.Rows[0]["Collection_List_Hex_Hash"]);
                ProteinCollectionsListFromDB = Conversions.ToString(tmpTable.Rows[0]["Protein_Collection_List"]);

                if (tmpTable.Rows[0]["Protein_Collection_List"].GetType().Name == "DBNull" ||
                    string.IsNullOrEmpty(CollectionListHexHashInDB) ||
                    (ProteinCollectionsListFromDB ?? "") != (proteinCollectionsList ?? "") ||
                    (CollectionListHexHashInDB ?? "") != (CollectionListHexHash ?? ""))
                {
                    RunSP_UpdateFileArchiveEntryCollectionList(ArchivedFileEntryID, proteinCollectionsList, sourceAuthenticationHash, CollectionListHexHash);
                }
            }

            m_Archived_File_Name = tmpTable.Rows[0]["Archived_File_Path"].ToString();

            try
            {
                di = new DirectoryInfo(Path.GetDirectoryName(m_Archived_File_Name));
                destFI = new FileInfo(m_Archived_File_Name);
                if (!di.Exists)
                {
                    di.Create();
                }

                if (!destFI.Exists)
                {
                    fi.CopyTo(m_Archived_File_Name);
                }
            }
            catch (UnauthorizedAccessException exUnauthorized)
            {
                Console.WriteLine("  Warning: access denied copying file to " + m_Archived_File_Name);
            }
            catch (Exception ex)
            {
                m_LastError = "File copying error: " + ex.Message;
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
            var sha1Hash = m_SHA1Provider.ComputeHash(byteSourceText);

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
            string pathString;
            pathString = Path.Combine(m_BaseArchivePath, Enum.GetName(typeof(CollectionTypes), archivedFileType));
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
            var dbTools = m_DatabaseAccessor.DBTools;

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
            var dbTools = m_DatabaseAccessor.DBTools;

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

            m_Archived_File_Name = archivedFileFullPath;

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }
    }
}