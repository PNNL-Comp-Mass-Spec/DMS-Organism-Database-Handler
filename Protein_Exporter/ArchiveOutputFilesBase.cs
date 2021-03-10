using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using PRISMDatabaseUtils;
using TableManipulationBase;

namespace Protein_Exporter
{
    public abstract class ArchiveOutputFilesBase
    {
        public enum CollectionTypes
        {
            @static = 1,
            dynamic = 2
        }

        private GetFASTAFromDMSForward m_Exporter;
        protected readonly DBTask m_DatabaseAccessor;
        protected string m_LastError;
        // Unused: protected GetFASTAFromDMS.SequenceTypes m_OutputSequenceType;
        protected string m_Archived_File_Name;

        protected event ArchiveStartEventHandler ArchiveStart;

        protected delegate void ArchiveStartEventHandler();

        protected event SubTaskStartEventHandler SubTaskStart;

        protected delegate void SubTaskStartEventHandler(string taskDescription);

        protected event SubTaskProgressUpdateEventHandler SubTaskProgressUpdate;

        protected delegate void SubTaskProgressUpdateEventHandler(double fractionDone);

        protected event OverallProgressUpdateEventHandler OverallProgressUpdate;

        protected delegate void OverallProgressUpdateEventHandler(double fractionDone);

        protected event ArchiveCompleteEventHandler ArchiveComplete;

        protected delegate void ArchiveCompleteEventHandler(string archivePath);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor"></param>
        /// <param name="exporterModule"></param>
        public ArchiveOutputFilesBase(DBTask databaseAccessor, GetFASTAFromDMS exporterModule)
        {
            m_DatabaseAccessor = databaseAccessor;

            m_Exporter = exporterModule.ExporterComponent;
        }

        private string LastErrorMessage => m_LastError;

        public string Archived_File_Name => m_Archived_File_Name;

        public int ArchiveCollection(int proteinCollectionID, CollectionTypes archivedFileType, GetFASTAFromDMS.SequenceTypes outputSequenceType, GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType, string sourceFilePath, string creationOptionsString, string authentication_Hash, string proteinCollectionList)
        {
            OnArchiveStart();

            return DispositionFile(proteinCollectionID, sourceFilePath, creationOptionsString, authentication_Hash, outputSequenceType, archivedFileType, proteinCollectionList);
        }

        public int ArchiveCollection(string proteinCollectionName, CollectionTypes archivedFileType, GetFASTAFromDMS.SequenceTypes outputSequenceType, GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType, string sourceFilePath, string creationOptionsString, string authentication_Hash, string proteinCollectionList)
        {
            int proteinCollectionID = GetProteinCollectionID(proteinCollectionName);

            return ArchiveCollection(proteinCollectionID, archivedFileType, outputSequenceType, databaseFormatType, sourceFilePath, creationOptionsString, authentication_Hash, proteinCollectionList);
        }

        protected abstract int DispositionFile(int proteinCollectionID, string sourceFilePath, string creationOptionsString, string sourceAuthenticationHash, GetFASTAFromDMS.SequenceTypes outputSequenceType, CollectionTypes archivedFileType, string ProteinCollectionsList);

        protected int GetProteinCount(string sourceFilePath)
        {
            var idLineRegex = new Regex("^>.+", RegexOptions.Compiled);

            var fi = new FileInfo(sourceFilePath);
            int counter = 0;

            if (fi.Exists)
            {
                using (var fileReader = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    while (!fileReader.EndOfStream)
                    {
                        string dataLine = fileReader.ReadLine();
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
        //    dt = m_DatabaseAccessor.GetTable(SQL);

        //    if (dt.Rows.Count > 0)
        //    {
        //        m_Archived_File_Name = dt.Rows[0]["Archived_File_Path"].ToString();
        //        return System.Convert.ToInt32(dt.Rows[0]["Archived_File_ID"]);
        //    }
        //    else
        //        return 0;
        //}

        public void AddArchiveCollectionXRef(int proteinCollectionID, int archivedFileID)
        {
            int intReturn = RunSP_AddArchivedFileEntryXRef(proteinCollectionID, archivedFileID);

            if (intReturn != 0)
            {
                throw new Exception("Error calling RunSP_AddArchivedFileEntryXRef with ProteinCollectionID " + proteinCollectionID + " and ArchivedFileID " + archivedFileID + ", ReturnCode=" + intReturn);
            }
        }

        // Unused
        //protected string GetFileAuthenticationHash(string sourcePath)
        //{
        //    return m_Exporter.GetFileHash(sourcePath);
        //}

        //// Unused
        //protected string GetStoredFileAuthenticationHash(int ProteinCollectionID)
        //{
        //    return m_Exporter.GetStoredHash(ProteinCollectionID);
        //}

        protected int GetProteinCollectionID(string proteinCollectionName)
        {
            return m_Exporter.FindIDByName(proteinCollectionName);
        }

        protected int RunSP_AddArchivedFileEntryXRef(int proteinCollectionID, int archivedFileID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddArchivedFileEntryXRef", CommandType.StoredProcedure);

            // Define parameters

            dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID;

            dbTools.AddParameter(cmdSave, "@Archived_File_ID", SqlType.Int).Value = archivedFileID;

            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 250, ParameterDirection.Output);

            string errorMessage = string.Empty;

            // Execute the sp
            int returnValue = dbTools.ExecuteSP(cmdSave, out errorMessage);

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