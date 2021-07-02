using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OrganismDatabaseHandler;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinUpload;

namespace AppUI_OrfDBHandler.ProteinUpload
{
    public class BatchUploadFromFileList
    {
        private readonly PSUploadHandler mUploader;
        private readonly DBTask mDatabaseAccessor;

        public BatchUploadFromFileList(string psConnectionString)
        {
            mUploader = new PSUploadHandler(psConnectionString);
            mUploader.BatchProgress += OnTaskChange;
            mUploader.LoadProgress += OnProgressUpdate;
            mUploader.LoadStart += OnLoadStart;
            mUploader.LoadEnd += OnLoadEnd;
            mUploader.LoadStart += OnLoadStart;

            mDatabaseAccessor = new DBTask(psConnectionString);
        }

        public event ProgressUpdateEventHandler ProgressUpdate;
        public event TaskChangeEventHandler TaskChange;
        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;

        private void OnTaskChange(string currentTaskTitle)
        {
            TaskChange?.Invoke(currentTaskTitle);
        }

        private void OnProgressUpdate(double fractionDone)
        {
            ProgressUpdate?.Invoke(fractionDone);
        }

        private void OnLoadStart(string taskTitle)
        {
            LoadStart?.Invoke(taskTitle);
        }

        private void OnLoadEnd()
        {
            LoadEnd?.Invoke();
        }

        // ReSharper disable once UnusedMember.Global
        protected DataTable GetAuthorityTable()
        {
            const string authSql = "SELECT ID, Display_Name, Details FROM V_Authority_Picker";
            return mDatabaseAccessor.GetTable(authSql);
        }

        protected DataTable GetAnnotationTypeTable()
        {
            const string annoSql = "SELECT ID, Display_Name, Details FROM V_Annotation_Type_Picker";
            return mDatabaseAccessor.GetTable(annoSql);
        }

        protected DataTable GetOrganismsTable()
        {
            const string orgSql = "SELECT ID, Short_Name, Display_Name, Organism_Name FROM V_Organism_Picker";
            return mDatabaseAccessor.GetTable(orgSql);
        }

        private PSUploadHandler.UploadInfo TransformToUploadInfo(FileListInfo fli)
        {
            var fi = new FileInfo(fli.FullFilePath);
            var ui = new PSUploadHandler.UploadInfo(fi, fli.OrganismId, fli.AnnotationTypeId);

            return ui;
        }

        protected int UploadSelectedFiles(Dictionary<string, FileListInfo> fileNameList)
        {
            var selectedFileList = new List<PSUploadHandler.UploadInfo>();

            foreach (var fli in fileNameList.Values)
            {
                var upInfoContainer = new PSUploadHandler.UploadInfo(
                    new FileInfo(fli.FullFilePath), fli.OrganismId, fli.NamingAuthorityId);
                selectedFileList.Add(upInfoContainer);
            }

            mUploader.BatchUpload(selectedFileList);
            return default;
        }

        public class FileListInfo
        {
            public FileListInfo(
                string fileName,
                string fullFilePath,
                string organismName,
                int organismId)
            {
                this.FileName = fileName;
                this.FullFilePath = fullFilePath;
                this.OrganismName = organismName;
                this.OrganismId = organismId;
                AnnotationType = "";
            }

            public FileListInfo(
                string fileName,
                string fullFilePath,
                string organismName,
                int organismId,
                int annotationTypeId,
                int namingAuthorityId)
            {
                this.FileName = fileName;
                this.FullFilePath = fullFilePath;
                this.OrganismName = organismName;
                this.OrganismId = organismId;
                this.AnnotationTypeId = annotationTypeId;
                this.NamingAuthorityId = namingAuthorityId;
                AnnotationType = "";
            }

            public string FileName { get; set; }

            public string FullFilePath { get; set; }

            public string OrganismName { get; set; }

            public int OrganismId { get; set; }

            public int NamingAuthorityId { get; set; }

            public int AnnotationTypeId { get; set; }

            public string AnnotationType { get; set; }
        }
    }
}