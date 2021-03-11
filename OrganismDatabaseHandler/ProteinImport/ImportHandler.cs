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

        private readonly DBTask mSQLAccess;

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

        public delegate void CollectionLoadCompleteEventHandler(DataTable collectionsTable);

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

        protected string GetCollectionNameFromId(int proteinCollectionId)
        {
            var foundRows = mCollectionsList.Select("Protein_Collection_ID = " + proteinCollectionId);
            var dr = foundRows[0];
            var collectionName = mSQLAccess.DbTools.GetString(dr["FileName"]);

            return collectionName;
        }

        protected ProteinStorage.ProteinStorage LoadFASTA(string filePath)
        {

            // check for existence of current file
            var fastaContents = mImporter.GetProteinEntries(filePath);

            var errorMessage = mImporter.LastErrorMessage;

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

                fastaContents?.ClearProteinEntries();
            }

            return fastaContents;
        }

        public DataTable LoadOrganisms()
        {
            var orgSQL = "SELECT * FROM V_Organism_Picker ORDER BY Short_Name";
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
            int proteinCollectionId)
        {
            var sqlQuery =
                "SELECT Annotation_Type_ID " +
                "FROM V_Protein_Collection_Authority " +
                "WHERE Protein_Collection_ID = " + proteinCollectionId;
            var tmpAnnTypeIdTable = mSQLAccess.GetTable(sqlQuery);

            DataRow dr;
            var authIdSb = new System.Text.StringBuilder();
            foreach (DataRow currentDr in tmpAnnTypeIdTable.Rows)
            {
                dr = currentDr;
                authIdSb.Append(dr["Annotation_Type_ID"]);
                authIdSb.Append(", ");
            }

            tmpAnnTypeIdTable = null;

            authIdSb.Remove(authIdSb.Length - 2, 2);

            var authSql =
                "SELECT * FROM V_Annotation_Type_Picker " +
                "WHERE ID IN (" + authIdSb + ") " +
                "ORDER BY Display_Name";

            var tmpAuthTable = mSQLAccess.GetTable(authSql);

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
            var AuthSQL = "SELECT * FROM V_Annotation_Type_Picker ORDER BY Display_Name";
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
            var AuthSQL = "SELECT * FROM V_Authority_Picker ORDER BY Display_Name";
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
            mFileContents?.ClearProteinEntries();
        }

        public void TriggerProteinCollectionsLoad()
        {
            OnCollectionLoadComplete(LoadProteinCollections());
        }

        public void TriggerProteinCollectionsLoad(int organismId)
        {
            OnCollectionLoadComplete(LoadProteinCollections(organismId));
        }

        public void TriggerProteinCollectionTableUpdate()
        {
            // Dim errCode As Integer = RunSP_UpdateProteinCollectionsByOrganism
            OnCollectionLoadComplete(LoadProteinCollections());
        }

        public DataTable LoadProteinCollections()
        {
            var pcSql = "SELECT MIN(FileName) AS FileName, Protein_Collection_ID, " +
                        "MIN(Organism_ID) AS Organism_ID, MIN(Authority_ID) AS Authority_ID, " +
                        "MIN(Display) AS Display, MIN(Authentication_Hash) AS Authentication_Hash " +
                        "FROM V_Protein_Collections_By_Organism " +
                        "GROUP BY Protein_Collection_ID " +
                        "ORDER BY MIN(FileName)";

            var tmpPcTable = mSQLAccess.GetTable(pcSql);

            var dr = tmpPcTable.NewRow();

            dr["Protein_Collection_ID"] = 0;
            dr["Display"] = " -- None Selected -- ";

            tmpPcTable.Rows.InsertAt(dr, 0);
            tmpPcTable.AcceptChanges();

            return tmpPcTable;
        }

        protected DataTable LoadProteinCollections(int organismId)
        {
            var sqlQuery = "SELECT FileName, Protein_Collection_ID, Organism_ID, Authority_ID, Display, Authentication_Hash" +
                           " FROM V_Protein_Collections_By_Organism" +
                           " WHERE Organism_ID = " + organismId +
                           " ORDER BY FileName";
            var tmpPcTable = mSQLAccess.GetTable(sqlQuery);

            var dr = tmpPcTable.NewRow();

            dr["Protein_Collection_ID"] = 0;
            dr["Display"] = " -- None Selected -- ";

            tmpPcTable.Rows.InsertAt(dr, 0);
            tmpPcTable.AcceptChanges();

            return tmpPcTable;
        }

        public DataTable LoadProteinCollectionNames()
        {
            var pcSql =
                "SELECT Protein_Collection_ID, FileName, Authority_ID " +
                "FROM V_Protein_Collections_By_Organism " +
                "ORDER BY FileName";
            var tmpPcTable = mSQLAccess.GetTable(pcSql);

            var dr = tmpPcTable.NewRow();

            dr["Protein_Collection_ID"] = 0;
            dr["FileName"] = " -- None Selected -- ";

            tmpPcTable.Rows.InsertAt(dr, 0);
            tmpPcTable.AcceptChanges();

            return tmpPcTable;
        }

        public DataTable LoadCollectionMembersById(
            int collectionId,
            int authorityId)
        {
            mCollectionsList = LoadProteinCollections();

            if (authorityId <= 0)
            {
                var foundRows = mCollectionsList.Select("Protein_Collection_ID = " + collectionId);
                authorityId = mSQLAccess.DbTools.GetInteger(foundRows[0]["Authority_ID"]);
            }

            var sqlQuery =
                "SELECT * From V_Protein_Storage_Entry_Import " +
                "WHERE Protein_Collection_ID = " + collectionId + " " +
                "AND Annotation_Type_ID = " + authorityId + " " +
                "ORDER BY Name";
            return LoadCollectionMembers(sqlQuery);
        }

        [Obsolete("Unused")]
        public DataTable LoadCollectionMembersByName(
            string collectionName,
            int authorityId)
        {
            var sqlQuery =
                "SELECT Protein_Collection_ID, Primary_Annotation_Type_ID " +
                "FROM T_Protein_Collections " +
                "WHERE FileName = '" + collectionName + "' ORDER BY FileName";

            var tmpTable = mSQLAccess.GetTable(sqlQuery);
            var foundRow = tmpTable.Rows[0];
            var collectionId = mSQLAccess.DbTools.GetInteger(foundRow["Protein_Collection_ID"]);
            //var authorityId = mSqlAccess.dbTools.GetInteger(foundRow.Item("Primary_Authority_ID"))

            return LoadCollectionMembersById(collectionId, authorityId);
        }

        private DataTable LoadCollectionMembers(string selectStatement)
        {
            var tmpMemberTable = mSQLAccess.GetTable(selectStatement);

            mFileContents = LoadProteinInfo(tmpMemberTable.Select(""));

            return tmpMemberTable;
        }

        protected ProteinStorage.ProteinStorage LoadProteinInfo(DataRow[] proteinCollectionMembers)
        {
            var tmpPs = new ProteinStorageDMS("");
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

            var dbTools = mSQLAccess.DbTools;

            foreach (var dr in proteinCollectionMembers)
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

                ce.ProteinId = dbTools.GetInteger(dr["Protein_ID"]);
                tmpPs.AddProtein(ce);
                counter += 1;
            }

            return tmpPs;
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

            // Move certain elements of the protein record to a DataTable for display in the source window
            Task_LoadStart("Updating Display List...");
            foreach (var entry in mFileContents.GetEntriesIEnumerable())
            {
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

        protected void OnCollectionLoadComplete(DataTable collectionsList)
        {
            CollectionLoadComplete?.Invoke(collectionsList);
        }
        #endregion

        #region "Stored Procedure Access"
        protected int RunSP_UpdateProteinCollectionsByOrganism()
        {
            var dbTools = mSQLAccess.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionsByOrganism", CommandType.StoredProcedure);

            // Define parameters

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        #endregion
    }
}