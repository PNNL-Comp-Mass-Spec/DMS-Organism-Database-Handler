using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.DatabaseTools;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.ProteinExport
{
    public abstract class ArchiveOutputFilesBase
    {
        public enum CollectionTypes
        {
            Static = 1,
            Dynamic = 2
        }

        private readonly GetFASTAFromDMSForward mExporter;
        protected readonly DBTask DatabaseAccessor;
        protected string LastError;
        // Unused: protected GetFASTAFromDMS.SequenceTypes mOutputSequenceType;
        protected string mArchived_File_Name;

        protected event ArchiveStartEventHandler ArchiveStart;

        protected delegate void ArchiveStartEventHandler();

        protected event OverallProgressUpdateEventHandler OverallProgressUpdate;

        protected delegate void OverallProgressUpdateEventHandler(double fractionDone);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor"></param>
        /// <param name="exporterModule"></param>
        protected ArchiveOutputFilesBase(DBTask databaseAccessor, GetFASTAFromDMS exporterModule)
        {
            DatabaseAccessor = databaseAccessor;

            mExporter = exporterModule.ExporterComponent;
        }

        private string LastErrorMessage => LastError;

        public string Archived_File_Name => mArchived_File_Name;

        public int ArchiveCollection(int proteinCollectionId, CollectionTypes archivedFileType, GetFASTAFromDMS.SequenceTypes outputSequenceType, GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType, string sourceFilePath, string creationOptionsString, string authenticationHash, string proteinCollectionList)
        {
            OnArchiveStart();

            return DispositionFile(proteinCollectionId, sourceFilePath, creationOptionsString, authenticationHash, outputSequenceType, archivedFileType, proteinCollectionList);
        }

        public int ArchiveCollection(string proteinCollectionName, CollectionTypes archivedFileType, GetFASTAFromDMS.SequenceTypes outputSequenceType, GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType, string sourceFilePath, string creationOptionsString, string authenticationHash, string proteinCollectionList)
        {
            var proteinCollectionId = GetProteinCollectionId(proteinCollectionName);

            return ArchiveCollection(proteinCollectionId, archivedFileType, outputSequenceType, databaseFormatType, sourceFilePath, creationOptionsString, authenticationHash, proteinCollectionList);
        }

        protected abstract int DispositionFile(int proteinCollectionId, string sourceFilePath, string creationOptionsString, string sourceAuthenticationHash, GetFASTAFromDMS.SequenceTypes outputSequenceType, CollectionTypes archivedFileType, string proteinCollectionsList);

        protected int GetProteinCount(string sourceFilePath)
        {
            var idLineRegex = new Regex("^>.+", RegexOptions.Compiled);

            var fi = new FileInfo(sourceFilePath);
            var counter = 0;

            if (fi.Exists)
            {
                using (var fileReader = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    string dataLine;
                    while ((dataLine = fileReader.ReadLine()) != null)
                    {
                        if (idLineRegex.IsMatch(dataLine))
                        {
                            counter += 1;
                        }
                    }
                }
            }

            return counter;
        }

        //Unused
        //protected int CheckForExistingArchiveEntry(
        //    string authentication_Hash,
        //    IArchiveOutputFiles.CollectionTypes archivedFileType,
        //    string creationOptionsString)
        //{
        //    var SQL = "SELECT Archived_File_ID,  Archived_File_Path " +
        //        "FROM V_Archived_Output_Files " +
        //        "WHERE Authentication_Hash = '" + authentication_Hash + "' AND " +
        //        "Archived_File_Type = '" +
        //        Enum.GetName(typeof(IArchiveOutputFiles.CollectionTypes), archivedFileType) +
        //        "' AND " + "Creation_Options = '" + creationOptionsString + "'";

        //    DataTable dt;
        //    dt = mDatabaseAccessor.GetTable(SQL);

        //    if (dt.Rows.Count > 0)
        //    {
        //        mArchived_File_Name = dt.Rows[0]["Archived_File_Path"].ToString();
        //        return System.Convert.ToInt32(dt.Rows[0]["Archived_File_ID"]);
        //    }
        //    else
        //        return 0;
        //}

        public void AddArchiveCollectionXRef(int proteinCollectionId, int archivedFileId)
        {
            var intReturn = RunSP_AddArchivedFileEntryXRef(proteinCollectionId, archivedFileId);

            if (intReturn != 0)
            {
                throw new Exception("Error calling RunSP_AddArchivedFileEntryXRef with ProteinCollectionID " + proteinCollectionId + " and ArchivedFileID " + archivedFileId + ", ReturnCode=" + intReturn);
            }
        }

        // Unused
        //protected string GetFileAuthenticationHash(string sourcePath)
        //{
        //    return mExporter.GetFileHash(sourcePath);
        //}

        //// Unused
        //protected string GetStoredFileAuthenticationHash(int ProteinCollectionId)
        //{
        //    return mExporter.GetStoredHash(ProteinCollectionId);
        //}

        protected int GetProteinCollectionId(string proteinCollectionName)
        {
            return mExporter.FindIdByName(proteinCollectionName);
        }

        protected int RunSP_AddArchivedFileEntryXRef(int proteinCollectionId, int archivedFileId)
        {
            var dbTools = DatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddArchivedFileEntryXRef", CommandType.StoredProcedure);

            // Define parameters

            dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionId;

            dbTools.AddParameter(cmdSave, "@Archived_File_ID", SqlType.Int).Value = archivedFileId;

            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 250, ParameterDirection.Output);

            var errorMessage = string.Empty;

            // Execute the sp
            var returnValue = dbTools.ExecuteSP(cmdSave, out errorMessage);

            return returnValue;
        }

        protected void OnArchiveStart()
        {
            ArchiveStart?.Invoke();
            OnOverallProgressUpdate(0.0d);
        }

        // Unused
        //protected void OnSubTaskStart(string subTaskDescription)
        //{
        //    SubTaskStart?.Invoke(subTaskDescription);
        //    OnSubTaskProgressUpdate(0.0);
        //}

        protected void OnOverallProgressUpdate(double fractionDone)
        {
            OverallProgressUpdate?.Invoke(fractionDone);
        }

        // Unused
        //protected void OnSubTaskProgressUpdate(double fractionDone)
        //{
        //    SubTaskProgressUpdate?.Invoke(fractionDone);
        //}

        //protected void OnArchiveComplete(string archivedPath)
        //{
        //    ArchiveComplete?.Invoke(archivedPath);
        //}
    }
}