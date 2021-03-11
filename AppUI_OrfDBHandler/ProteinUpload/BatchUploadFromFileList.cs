using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinUpload;

namespace AppUI_OrfDBHandler.ProteinUpload
{
    public class BatchUploadFromFileList
    {
        private readonly PSUploadHandler mUploader;
        private readonly DBTask mDatabaseAccessor;
        private Dictionary<string, FileListInfo> mCurrentFileList;

        private DataTable mAuthorityTable;
        private DataTable mAnnotationTypeTable;
        private DataTable mOrganismTable;

        private frmBatchUploadFromFileList mBatchForm;

        private const string DmsOrgDbTableName = "V_Legacy_Static_File_Locations";
        private const string ProteinCollectionsTableName = "T_Protein_Collections";

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

        public delegate void ProgressUpdateEventHandler(double fractionDone);

        public event TaskChangeEventHandler TaskChange;

        public delegate void TaskChangeEventHandler(string currentTaskTitle);

        public event LoadStartEventHandler LoadStart;

        public delegate void LoadStartEventHandler(string taskTitle);

        public event LoadEndEventHandler LoadEnd;

        public delegate void LoadEndEventHandler();

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

        public void UploadBatch()
        {
            var uiList = new List<PSUploadHandler.UploadInfo>();

            mAnnotationTypeTable = GetAnnotationTypeTable();
            mAuthorityTable = GetAuthorityTable();
            mOrganismTable = GetOrganismsTable();

            mBatchForm = new frmBatchUploadFromFileList(mAuthorityTable, mAnnotationTypeTable, mOrganismTable);

            mCurrentFileList = GetDmsFileEntities();

            mBatchForm.FileCollection = mCurrentFileList;

            var r = mBatchForm.ShowDialog();

            if (r == DialogResult.OK)
            {
                var fileCollection = mBatchForm.SelectedFilesCollection;

                foreach (var fce in fileCollection.Values)
                {
                    var ui = TransformToUploadInfo(fce);
                    uiList.Add(ui);
                }

                mUploader.BatchUpload(uiList);
            }
        }

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
            const string orgSql = "SELECT ID, Short_Name, Display_Name, OrganismName FROM V_OrganismPicker";
            return mDatabaseAccessor.GetTable(orgSql);
        }

        private PSUploadHandler.UploadInfo TransformToUploadInfo(FileListInfo fli)
        {
            var fi = new FileInfo(fli.FullFilePath);
            var ui = new PSUploadHandler.UploadInfo(fi, fli.OrganismId, fli.AnnotationTypeId);

            return ui;
        }

        protected Dictionary<string, FileListInfo> GetDmsFileEntities()
        {
            var fileList = new Dictionary<string, FileListInfo>(StringComparer.OrdinalIgnoreCase);
            var collectionList = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

            // DataRow dr;

            // string LoadedCollectionsSQL;

            // string tmpFileName;
            // string tmpOrganismName;
            // int tmpOrganismID;
            // string tmpFullPath;
            // int tmpAnnTypeID;
            // int tmpAuthTypeID;

            var loadedCollectionsSQL = "SELECT FileName, Full_Path, OrganismName, OrganismID, Annotation_Type_ID, Authority_ID FROM V_Collections_Reload_Filtered";

            using (var fileTable = mDatabaseAccessor.GetTable(loadedCollectionsSQL))
            {
                if (mCurrentFileList == null)
                {
                    mCurrentFileList = new Dictionary<string, FileListInfo>();
                }
                else
                {
                    mCurrentFileList.Clear();
                }

                foreach (DataRow dr in fileTable.Rows)
                {
                    var fileName = dr["FileName"].ToString();
                    var organismName = dr["OrganismName"].ToString();
                    var organismId = Convert.ToInt32(dr["OrganismID"]);
                    var fullPath = dr["Full_Path"].ToString();
                    var annotationTypeId = Convert.ToInt32(dr["Annotation_Type_ID"]);
                    var authorityTypeId = Convert.ToInt32(dr["Authority_ID"]);

                    var baseName = Path.GetFileNameWithoutExtension(fileName);

                    if (!fileList.ContainsKey(fileName) && !collectionList.Contains(baseName))
                    {
                        fileList.Add(fileName,
                                     new FileListInfo(fileName, fullPath, organismName, organismId, annotationTypeId, authorityTypeId));
                    }
                }

                fileTable.Clear();
            }

            return fileList;
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