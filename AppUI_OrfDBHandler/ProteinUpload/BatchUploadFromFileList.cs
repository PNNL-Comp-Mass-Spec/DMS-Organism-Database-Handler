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
        private readonly PSUploadHandler m_Uploader;
        private readonly DBTask m_DatabaseAccessor;
        private Dictionary<string, FileListInfo> m_CurrentFileList;

        private DataTable m_AuthorityTable;
        private DataTable m_AnnotationTypeTable;
        private DataTable m_OrganismTable;

        private frmBatchUploadFromFileList m_BatchForm;

        private const string DMS_Org_DB_Table_Name = "V_Legacy_Static_File_Locations";
        private const string Protein_Collections_Table_Name = "T_Protein_Collections";

        public BatchUploadFromFileList(string psConnectionString)
        {
            m_Uploader = new PSUploadHandler(psConnectionString);
            m_Uploader.BatchProgress += OnTaskChange;
            m_Uploader.LoadProgress += OnProgressUpdate;
            m_Uploader.LoadStart += OnLoadStart;
            m_Uploader.LoadEnd += OnLoadEnd;
            m_Uploader.LoadStart += OnLoadStart;

            m_DatabaseAccessor = new DBTask(psConnectionString);
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

            m_AnnotationTypeTable = GetAnnotationTypeTable();
            m_AuthorityTable = GetAuthorityTable();
            m_OrganismTable = GetOrganismsTable();

            m_BatchForm = new frmBatchUploadFromFileList(m_AuthorityTable, m_AnnotationTypeTable, m_OrganismTable);

            m_CurrentFileList = GetDMSFileEntities();

            m_BatchForm.FileCollection = m_CurrentFileList;

            var r = m_BatchForm.ShowDialog();

            if (r == DialogResult.OK)
            {
                var fileCollection = m_BatchForm.SelectedFilesCollection;

                foreach (var fce in fileCollection.Values)
                {
                    var ui = TransformToUploadInfo(fce);
                    uiList.Add(ui);
                }

                m_Uploader.BatchUpload(uiList);
            }
        }

        protected DataTable GetAuthorityTable()
        {
            const string authSQL = "SELECT ID, Display_Name, Details FROM V_Authority_Picker";
            return m_DatabaseAccessor.GetTable(authSQL);
        }

        protected DataTable GetAnnotationTypeTable()
        {
            const string annoSQL = "SELECT ID, Display_Name, Details FROM V_Annotation_Type_Picker";
            return m_DatabaseAccessor.GetTable(annoSQL);
        }

        protected DataTable GetOrganismsTable()
        {
            const string orgSQL = "SELECT ID, Short_Name, Display_Name, Organism_Name FROM V_Organism_Picker";
            return m_DatabaseAccessor.GetTable(orgSQL);
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

            string loadedCollectionsSQL = "SELECT FileName, Full_Path, Organism_Name, Organism_ID, Annotation_Type_ID, Authority_ID FROM V_Collections_Reload_Filtered";

            using (var fileTable = m_DatabaseAccessor.GetTable(loadedCollectionsSQL))
            {
                if (m_CurrentFileList == null)
                {
                    m_CurrentFileList = new Dictionary<string, FileListInfo>();
                }
                else
                {
                    m_CurrentFileList.Clear();
                }

                foreach (DataRow dr in fileTable.Rows)
                {
                    string fileName = dr["FileName"].ToString();
                    string organismName = dr["Organism_Name"].ToString();
                    int organismID = Convert.ToInt32(dr["Organism_ID"]);
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

            m_Uploader.BatchUpload(selectedFileList);
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