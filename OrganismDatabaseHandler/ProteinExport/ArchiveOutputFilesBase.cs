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
        // Ignore Spelling: Accessor

        public enum CollectionTypes
        {
            Static = 1,
            Dynamic = 2
        }

        private readonly GetFASTAFromDMSForward mExporter;

        protected readonly DBTask DatabaseAccessor;

        protected string LastError;

        // Unused: protected GetFASTAFromDMS.SequenceTypes mOutputSequenceType;

        protected string mArchivedFilePath;

        protected event ArchiveStartEventHandler ArchiveStart;

        protected delegate void ArchiveStartEventHandler();

        protected event OverallProgressUpdateEventHandler OverallProgressUpdate;

        protected delegate void OverallProgressUpdateEventHandler(double fractionDone);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Database accessor</param>
        /// <param name="exporterModule">Protein exporter module</param>
        protected ArchiveOutputFilesBase(DBTask databaseAccessor, GetFASTAFromDMS exporterModule)
        {
            DatabaseAccessor = databaseAccessor;

            mExporter = exporterModule.ExporterComponent;
        }

        public string Archived_File_Name => mArchivedFilePath;

        public int ArchiveCollection(
            int proteinCollectionId,
            CollectionTypes archivedFileType,
            GetFASTAFromDMS.SequenceTypes outputSequenceType,
            string sourceFilePath,
            string creationOptionsString,
            string authenticationHash,
            string proteinCollectionList)
        {
            OnArchiveStart();

            return DispositionFile(proteinCollectionId, sourceFilePath, creationOptionsString, authenticationHash, outputSequenceType, archivedFileType, proteinCollectionList);
        }

        public int ArchiveCollection(
            string proteinCollectionName,
            CollectionTypes archivedFileType,
            GetFASTAFromDMS.SequenceTypes outputSequenceType,
            string sourceFilePath,
            string creationOptionsString,
            string authenticationHash,
            string proteinCollectionList)
        {
            var proteinCollectionId = GetProteinCollectionId(proteinCollectionName);

            if (proteinCollectionId <= 0)
            {
                PRISM.ConsoleMsgUtils.ShowWarning("Protein collection not found: " + proteinCollectionName);
                return 0;
            }

            return ArchiveCollection(proteinCollectionId, archivedFileType, outputSequenceType, sourceFilePath, creationOptionsString, authenticationHash, proteinCollectionList);
        }

        protected abstract int DispositionFile(int proteinCollectionId, string sourceFilePath, string creationOptionsString, string sourceAuthenticationHash, GetFASTAFromDMS.SequenceTypes outputSequenceType, CollectionTypes archivedFileType, string proteinCollectionsList);

        protected int GetProteinCount(string sourceFilePath)
        {
            var idLineRegex = new Regex("^>.+", RegexOptions.Compiled);

            var sourceFile = new FileInfo(sourceFilePath);

            if (!sourceFile.Exists)
                return 0;

            using var fileReader = new StreamReader(new FileStream(sourceFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            // ReSharper disable once MoveVariableDeclarationInsideLoopCondition
            string dataLine;
            var counter = 0;

            while ((dataLine = fileReader.ReadLine()) != null)
            {
                if (idLineRegex.IsMatch(dataLine))
                {
                    counter++;
                }
            }

            return counter;
        }

        public void AddArchiveCollectionXRef(int proteinCollectionId, int archivedFileId)
        {
            var returnCode = RunSP_AddArchivedFileEntryXRef(proteinCollectionId, archivedFileId);

            if (returnCode != 0)
            {
                throw new Exception("Error calling RunSP_AddArchivedFileEntryXRef with ProteinCollectionID " + proteinCollectionId + " and ArchivedFileID " + archivedFileId + ", ReturnCode=" + returnCode);
            }
        }

        protected int GetProteinCollectionId(string proteinCollectionName)
        {
            return mExporter.FindIdByName(proteinCollectionName);
        }

        protected int RunSP_AddArchivedFileEntryXRef(int proteinCollectionId, int archivedFileId)
        {
            var dbTools = DatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_archived_file_entry_xref", CommandType.StoredProcedure);

            // Define parameters

            dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            dbTools.AddParameter(cmdSave, "@collectionID", SqlType.Int).Value = proteinCollectionId;

            dbTools.AddParameter(cmdSave, "@archivedFileID", SqlType.Int).Value = archivedFileId;

            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 250, ParameterDirection.InputOutput);

            return dbTools.ExecuteSP(cmdSave, out _);
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