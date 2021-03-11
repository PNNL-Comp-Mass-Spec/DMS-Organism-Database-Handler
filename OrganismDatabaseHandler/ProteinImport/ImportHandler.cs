using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinStorage;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.ProteinImport
{
    public class ImportHandler
    {
        public enum ProteinImportFileTypes
        {
            FASTA,
            Access
        }

        private DBTask mSQLAccess;

        private readonly FASTAReader mImporter;

        private int mPersistentTaskNum;

        private string mSPError;

        // Unused constants
        // protected const string ProteinCollectionsTable = "T_Protein_Collections";
        // protected const string ProteinsTable = "T_Proteins";
        // protected const string MembersTable = "T_Protein_Collection_Members";
        // protected const string NamesTable = "T_Protein_Names";
        // protected const string PositionTable = "T_Position_Info";
        // protected const string CollectionProteinMap = "V_Protein_Collections_By_Organism";

        private ProteinStorage.ProteinStorage mFileContents;
        private DataTable mCollectionsList;

        private Dictionary<string, string> mAuthoritiesList;
        private DataTable mAuthoritiesTable;

        public event LoadStartEventHandler LoadStart;

        public delegate void LoadStartEventHandler(string taskTitle);

        public event LoadProgressEventHandler LoadProgress;

        public delegate void LoadProgressEventHandler(double fractionDone);

        public event LoadEndEventHandler LoadEnd;

        public delegate void LoadEndEventHandler();

        public event CollectionLoadCompleteEventHandler CollectionLoadComplete;

        public delegate void CollectionLoadCompleteEventHandler(DataTable CollectionsTable);

        public ImportHandler(string psConnectionString)
        {
            mSQLAccess = new DBTask(psConnectionString);
            mImporter = new FASTAReader();
            mImporter.LoadStart += Task_LoadStart;
            mImporter.LoadProgress += Task_LoadProgress;
            mImporter.LoadEnd += Task_LoadEnd;
            mCollectionsList = LoadProteinCollectionNames();
        }

        public ProteinStorage.ProteinStorage CollectionMembers => mFileContents;

        public Dictionary<string, string> Authorities => mAuthoritiesList;

        protected string GetCollectionNameFromID(int ProteinCollectionID)
        {
            var foundRows = mCollectionsList.Select("Protein_Collection_ID = " + ProteinCollectionID.ToString());
            var dr = foundRows[0];
            string collectionName = mSQLAccess.DBTools.GetString(dr["FileName"]);

            return collectionName;
        }

        protected ProteinStorage.ProteinStorage LoadFASTA(string filePath)
        {

            // check for existence of current file
            var fastaContents = mImporter.GetProteinEntries(filePath);

            string errorMessage = mImporter.LastErrorMessage;

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                var proteinsLoaded = default(int);
                try
                {
                    if (fastaContents != null)
                    {
                        proteinsLoaded = fastaContents.ProteinCount;
                    }
                }
                catch (Exception ex)
                {
                    // Ignore errors here
                }

                MessageBox.Show("GetProteinEntries returned an error after loading " + proteinsLoaded + " proteins: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                fastaContents.ClearProteinEntries();
            }

            return fastaContents;
        }

        public DataTable LoadOrganisms()
        {
            string orgSQL = "SELECT * FROM V_OrganismPicker ORDER BY Short_Name";
            var tmpOrgTable = mSQLAccess.GetTable(orgSQL);

            var dr = tmpOrgTable.NewRow();

            dr["ID"] = 0;
            dr["Short_Name"] = "None";
            dr["Display_Name"] = " -- None Selected -- ";

            tmpOrgTable.Rows.InsertAt(dr, 0);

            tmpOrgTable.AcceptChanges();

            var pk1 = new DataColumn[1];

            pk1[0] = tmpOrgTable.Columns["ID"];
            tmpOrgTable.PrimaryKey = pk1;

            return tmpOrgTable;
        }

        public DataTable LoadAnnotationTypes(
            int proteinCollectionID)
        {
            string sqlQuery =
                "SELECT Annotation_Type_ID " +
                "FROM V_Protein_Collection_Authority " +
                "WHERE Protein_Collection_ID = " + proteinCollectionID.ToString();
            var tmpAnnTypeIDTable = mSQLAccess.GetTable(sqlQuery);

            DataRow dr;
            var authIDSB = new System.Text.StringBuilder();
            foreach (DataRow currentDr in tmpAnnTypeIDTable.Rows)
            {
                dr = currentDr;
                authIDSB.Append(dr["Annotation_Type_ID"].ToString());
                authIDSB.Append(", ");
            }

            tmpAnnTypeIDTable = null;

            authIDSB.Remove(authIDSB.Length - 2, 2);

            string AuthSQL =
                "SELECT * FROM V_Annotation_Type_Picker " +
                "WHERE ID IN (" + authIDSB.ToString() + ") " +
                "ORDER BY Display_Name";

            var tmpAuthTable = mSQLAccess.GetTable(AuthSQL);

            dr = tmpAuthTable.NewRow();

            dr["ID"] = 0;
            dr["Display_Name"] = " -- None Selected -- ";
            dr["Details"] = "None Selected";

            tmpAuthTable.Rows.InsertAt(dr, 0);

            tmpAuthTable.AcceptChanges();

            var pk1 = new DataColumn[1];

            pk1[0] = tmpAuthTable.Columns["ID"];
            tmpAuthTable.PrimaryKey = pk1;

            return tmpAuthTable;
        }

        public DataTable LoadAnnotationTypes()
        {
            string AuthSQL = "SELECT * FROM V_Annotation_Type_Picker ORDER BY Display_Name";
            var tmpAnnTypeTable = mSQLAccess.GetTable(AuthSQL);

            var dr = tmpAnnTypeTable.NewRow();

            dr["ID"] = 0;
            dr["Display_Name"] = " -- None Selected -- ";
            // dr["name"] = " -- None Selected -- ";
            dr["Details"] = "None Selected";

            tmpAnnTypeTable.Rows.InsertAt(dr, 0);

            tmpAnnTypeTable.AcceptChanges();
            mAuthoritiesList = mSQLAccess.DataTableToDictionary(tmpAnnTypeTable, "ID", "Display_Name");
            mAuthoritiesTable = tmpAnnTypeTable.Copy();

            return tmpAnnTypeTable;
        }

        public DataTable LoadAuthorities()
        {
            string AuthSQL = "SELECT * FROM V_Authority_Picker ORDER BY Display_Name";
            var tmpAuthTable = mSQLAccess.GetTable(AuthSQL);

            var dr = tmpAuthTable.NewRow();

            dr["ID"] = 0;
            dr["Display_Name"] = " -- None Selected -- ";
            dr["Details"] = "None Selected";

            tmpAuthTable.Rows.InsertAt(dr, 0);

            tmpAuthTable.AcceptChanges();
            mAuthoritiesList = mSQLAccess.DataTableToDictionary(tmpAuthTable, "ID", "Display_Name");

            return tmpAuthTable;
        }

        public void ClearProteinCollection()
        {
            if (mFileContents != null)
            {
                mFileContents.ClearProteinEntries();
            }
        }

        public void TriggerProteinCollectionsLoad()
        {
            OnCollectionLoadComplete(LoadProteinCollections());
        }

        public void TriggerProteinCollectionsLoad(int OrganismID)
        {
            OnCollectionLoadComplete(LoadProteinCollections(OrganismID));
        }

        public void TriggerProteinCollectionTableUpdate()
        {
            // Dim errCode As Integer = RunSP_UpdateProteinCollectionsByOrganism
            OnCollectionLoadComplete(LoadProteinCollections());
        }

        public DataTable LoadProteinCollections()
        {
            var PCSQL = "SELECT MIN(FileName) AS FileName, Protein_Collection_ID, " +
                        "MIN(OrganismID) AS OrganismID, MIN(Authority_ID) AS Authority_ID, " +
                        "MIN(Display) AS Display, MIN(Authentication_Hash) AS Authentication_Hash " +
                        "FROM V_Protein_Collections_By_Organism " +
                        "GROUP BY Protein_Collection_ID " +
                        "ORDER BY MIN(FileName)";

            var tmpPCTable = mSQLAccess.GetTable(PCSQL);

            var dr = tmpPCTable.NewRow();

            dr["Protein_Collection_ID"] = 0;
            dr["Display"] = " -- None Selected -- ";

            tmpPCTable.Rows.InsertAt(dr, 0);
            tmpPCTable.AcceptChanges();

            return tmpPCTable;
        }

        protected DataTable LoadProteinCollections(int OrganismID)
        {
            var sqlQuery = "SELECT FileName, Protein_Collection_ID, OrganismID, Authority_ID, Display, Authentication_Hash" +
                           " FROM V_Protein_Collections_By_Organism" +
                           " WHERE OrganismID = " + OrganismID +
                           " ORDER BY FileName";
            var tmpPCTable = mSQLAccess.GetTable(sqlQuery);

            var dr = tmpPCTable.NewRow();

            dr["Protein_Collection_ID"] = 0;
            dr["Display"] = " -- None Selected -- ";

            tmpPCTable.Rows.InsertAt(dr, 0);
            tmpPCTable.AcceptChanges();

            return tmpPCTable;
        }

        public DataTable LoadProteinCollectionNames()
        {
            string PCSQL =
                "SELECT Protein_Collection_ID, FileName, Authority_ID " +
                "FROM V_Protein_Collections_By_Organism " +
                "ORDER BY FileName";
            var tmpPCTable = mSQLAccess.GetTable(PCSQL);

            var dr = tmpPCTable.NewRow();

            dr["Protein_Collection_ID"] = 0;
            dr["FileName"] = " -- None Selected -- ";

            tmpPCTable.Rows.InsertAt(dr, 0);
            tmpPCTable.AcceptChanges();

            return tmpPCTable;
        }

        public DataTable LoadCollectionMembersByID(
            int collectionID,
            int authorityID)
        {
            mCollectionsList = LoadProteinCollections();

            if (authorityID <= 0)
            {
                var foundRows = mCollectionsList.Select("Protein_Collection_ID = " + collectionID);
                authorityID = mSQLAccess.DBTools.GetInteger(foundRows[0]["Authority_ID"]);
            }

            string sqlQuery =
                "SELECT * From V_Protein_Storage_Entry_Import " +
                "WHERE Protein_Collection_ID = " + collectionID + " " +
                "AND Annotation_Type_ID = " + authorityID + " " +
                "ORDER BY Name";
            return LoadCollectionMembers(sqlQuery);
        }

        public DataTable LoadCollectionMembersByName(
            string collectionName,
            int authorityID)
        {
            string sqlQuery =
                "SELECT Protein_Collection_ID, Primary_Annotation_Type_ID " +
                "FROM T_Protein_Collections " +
                "WHERE FileName = " + collectionName + " ORDER BY Name";

            var tmpTable = mSQLAccess.GetTable(sqlQuery);
            var foundRow = tmpTable.Rows[0];
            int collectionID = mSQLAccess.DBTools.GetInteger(foundRow["Protein_Collection_ID"]);
            // Dim authorityID = mSQLAccess.dbTools.GetInteger(foundRow.Item("Primary_Authority_ID"))

            return LoadCollectionMembersByID(collectionID, authorityID);
        }

        private DataTable LoadCollectionMembers(string SelectStatement)
        {
            var tmpMemberTable = mSQLAccess.GetTable(SelectStatement);

            mFileContents = LoadProteinInfo(tmpMemberTable.Select(""));

            return tmpMemberTable;
        }

        protected ProteinStorage.ProteinStorage LoadProteinInfo(DataRow[] proteinCollectionMembers)
        {
            var tmpPS = new ProteinStorageDMS("");
            int triggerCount;
            var counter = default(int);

            LoadStart?.Invoke("Retrieving Protein Entries...");

            var proteinCount = proteinCollectionMembers.Length;

            if (proteinCount > 20)
            {
                triggerCount = (int)Math.Round(proteinCount / 20d);
            }
            else
            {
                triggerCount = 1;
            }

            var dbTools = mSQLAccess.DBTools;

            foreach (DataRow dr in proteinCollectionMembers)
            {
                dbTools.GetInteger(dr["Authority_ID"]);

                var ce = new ProteinStorageEntry(
                    dbTools.GetString(dr["Name"]),
                    dbTools.GetString(dr["Description"]),
                    dbTools.GetString(dr["Sequence"]),
                    dbTools.GetInteger(dr["Length"]),
                    dbTools.GetDouble(dr["Monoisotopic_Mass"]),
                    dbTools.GetDouble(dr["Average_Mass"]),
                    dbTools.GetString(dr["Molecular_Formula"]),
                    dbTools.GetString(dr["SHA1_Hash"]),
                    counter);

                if (counter % triggerCount > 0)
                {
                    Task_LoadProgress((float)(counter / (double)proteinCount));
                }

                ce.Protein_ID = dbTools.GetInteger(dr["Protein_ID"]);
                tmpPS.AddProtein(ce);
                counter += 1;
            }

            return tmpPS;
        }

        // Function to load fasta file contents with no checking against the existing database entries
        // used to load up the source collection ListView
        public DataTable LoadProteinsRaw(
            string filePath,
            ProteinImportFileTypes fileType)
        {
            var tmpProteinTable = mSQLAccess.GetTableTemplate("V_Protein_Database_Export");
            var counter = default(int);
            int triggerCount;

            switch (fileType)
            {
                case ProteinImportFileTypes.FASTA:
                    mFileContents = LoadFASTA(filePath);
                    break;

                default:
                    return null;
            }

            if (mFileContents == null)
            {
                return null;
            }

            var proteinCount = mFileContents.ProteinCount;
            if (proteinCount > 20)
            {
                triggerCount = (int)Math.Round(proteinCount / 20d);
            }
            else
            {
                triggerCount = 1;
            }

            var contentsEnum = mFileContents.GetEnumerator();

            // Move certain elements of the protein record to a DataTable for display in the source window
            Task_LoadStart("Updating Display List...");
            while (contentsEnum.MoveNext())
            {
                var entry = contentsEnum.Current.Value;
                var dr = tmpProteinTable.NewRow();
                dr["Name"] = entry.Reference;
                dr["Description"] = entry.Description;
                dr["Sequence"] = entry.Sequence;
                tmpProteinTable.Rows.Add(dr);
                if (counter % triggerCount > 0)
                {
                    Task_LoadProgress((float)(counter / (double)proteinCount));
                }

                counter += 1;
            }

            Task_LoadEnd();
            return tmpProteinTable;
        }

        public ProteinStorage.ProteinStorage LoadProteinsForBatch(string fullFilePath)
        {
            var ps = LoadFASTA(fullFilePath);

            return ps;
        }

        #region "Event Handlers"

        // Handles the LoadStart event for the fasta importer module
        protected void Task_LoadStart(string taskTitle)
        {
            // mPersistentTaskNum += 1
            LoadStart?.Invoke(taskTitle);
        }

        protected void Task_LoadProgress(double fractionDone)
        {
            LoadProgress?.Invoke(fractionDone);
        }

        //protected void Task_LoadProgress(string taskTitle, double fractionDone)
        //{
        //    ValidationProgress?.Invoke(taskTitle, fractionDone);
        //}

        protected void Task_LoadEnd()
        {
            LoadEnd?.Invoke();
        }

        //private void OnInvalidFASTAFile(string FASTAFilePath, List errorCollection)
        //{
        //    InvalidFASTAFile?.Invoke(FASTAFilePath, errorCollection);
        //}

        protected void OnCollectionLoadComplete(DataTable CollectionsList)
        {
            CollectionLoadComplete?.Invoke(CollectionsList);
        }
        #endregion

        #region "Stored Procedure Access"
        protected int RunSP_UpdateProteinCollectionsByOrganism()
        {
            var dbTools = mSQLAccess.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionsByOrganism", CommandType.StoredProcedure);

            // Define parameters

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        #endregion
    }
}