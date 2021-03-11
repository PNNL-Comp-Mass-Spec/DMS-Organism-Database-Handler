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

        private const string DMS_Org_DB_Table_Name = "V_Legacy_Static_File_Locations";
        private const string Protein_Collections_Table_Name = "T_Protein_Collections";

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

            mCurrentFileList = GetDMSFileEntities();

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
            const string authSQL = "SELECT ID, Display_Name, Details FROM V_Authority_Picker";
            return mDatabaseAccessor.GetTable(authSQL);
        }

        protected DataTable GetAnnotationTypeTable()
        {
            const string annoSQL = "SELECT ID, Display_Name, Details FROM V_Annotation_Type_Picker";
            return mDatabaseAccessor.GetTable(annoSQL);
        }

        protected DataTable GetOrganismsTable()
        {
            const string orgSQL = "SELECT ID, Short_Name, Display_Name, OrganismName FROM V_OrganismPicker";
            return mDatabaseAccessor.GetTable(orgSQL);
        }

        private PSUploadHandler.UploadInfo TransformToUploadInfo(FileListInfo fli)
        {
            var fi = new FileInfo(fli.FullFilePath);
            var ui = new PSUploadHandler.UploadInfo(fi, fli.OrganismID, fli.AnnotationTypeID);

            return ui;
        }

        protected Dictionary<string, FileListInfo> GetDMSFileEntities()
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

            string loadedCollectionsSQL = "SELECT FileName, Full_Path, OrganismName, OrganismID, Annotation_Type_ID, Authority_ID FROM V_Collections_Reload_Filtered";

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
                    string fileName = dr["FileName"].ToString();
                    string organismName = dr["OrganismName"].ToString();
                    int organismID = Convert.ToInt32(dr["OrganismID"]);
                    string fullPath = dr["Full_Path"].ToString();
                    int annotationTypeID = Convert.ToInt32(dr["Annotation_Type_ID"]);
                    int authorityTypeID = Convert.ToInt32(dr["Authority_ID"]);

                    string baseName = Path.GetFileNameWithoutExtension(fileName);

                    if (!fileList.ContainsKey(fileName) & !collectionList.Contains(baseName))
                    {
                        fileList.Add(fileName,
                                     new FileListInfo(fileName, fullPath, organismName, organismID, annotationTypeID, authorityTypeID));
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
                    new FileInfo(fli.FullFilePath), fli.OrganismID, fli.NamingAuthorityID);
                selectedFileList.Add(upInfoContainer);
            }

            mUploader.BatchUpload(selectedFileList);
            return default;
        }

        public class FileListInfo
        {
            public FileListInfo(
                string FileName,
                string FullFilePath,
                string OrganismName,
                int OrganismID)
            {
                this.FileName = FileName;
                this.FullFilePath = FullFilePath;
                this.OrganismName = OrganismName;
                this.OrganismID = OrganismID;
                AnnotationType = "";
            }

            public FileListInfo(
                string FileName,
                string FullFilePath,
                string OrganismName,
                int OrganismID,
                int AnnotationTypeID,
                int NamingAuthorityID)
            {
                this.FileName = FileName;
                this.FullFilePath = FullFilePath;
                this.OrganismName = OrganismName;
                this.OrganismID = OrganismID;
                this.AnnotationTypeID = AnnotationTypeID;
                this.NamingAuthorityID = NamingAuthorityID;
                AnnotationType = "";
            }

            public string FileName { get; set; }

            public string FullFilePath { get; set; }

            public string OrganismName { get; set; }

            public int OrganismID { get; set; }

            public int NamingAuthorityID { get; set; }

            public int AnnotationTypeID { get; set; }

            public string AnnotationType { get; set; }
        }
    }
}