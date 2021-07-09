using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinStorage;
using PRISM;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.ProteinImport
{
    public class ImportHandler : EventNotifier
    {
        public enum ProteinImportFileTypes
        {
            FASTA,
            Access
        }

        private readonly DBTask mSQLAccess;

        private readonly FASTAReader mImporter;

        private DataTable mCollectionsList;

        public event LoadStartEventHandler LoadStart;
        public event LoadProgressEventHandler LoadProgress;
        public event LoadEndEventHandler LoadEnd;
        public event CollectionLoadCompleteEventHandler CollectionLoadComplete;

        public ImportHandler(string psConnectionString)
        {
            mSQLAccess = new DBTask(psConnectionString);
            RegisterEvents(mSQLAccess);

            mImporter = new FASTAReader();
            RegisterEvents(mImporter);

            mImporter.LoadStart += Task_LoadStart;
            mImporter.LoadProgress += Task_LoadProgress;
            mImporter.LoadEnd += Task_LoadEnd;
            mCollectionsList = LoadProteinCollectionNames();
        }

        public ProteinStorage.ProteinStorage CollectionMembers { get; private set; }

        public Dictionary<string, string> Authorities { get; private set; }

        [Obsolete("Unused")]
        protected string GetCollectionNameFromId(int proteinCollectionId)
        {
            var foundRows = mCollectionsList.Select("Protein_Collection_ID = " + proteinCollectionId);
            var dataRow = foundRows[0];
            var collectionName = mSQLAccess.DbTools.GetString(dataRow["FileName"]);

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
                catch (Exception)
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
            const string orgSQL = "SELECT * FROM V_Organism_Picker ORDER BY Short_Name";
            var organismTable = mSQLAccess.GetTable(orgSQL);

            var dataRow = organismTable.NewRow();

            dataRow["ID"] = 0;
            dataRow["Short_Name"] = "None";
            dataRow["Display_Name"] = " -- None Selected -- ";

            organismTable.Rows.InsertAt(dataRow, 0);

            organismTable.AcceptChanges();

            var pk1 = new DataColumn[1];

            pk1[0] = organismTable.Columns["ID"];
            organismTable.PrimaryKey = pk1;

            return organismTable;
        }

        [Obsolete("Unused")]
        public DataTable LoadAnnotationTypes(
            int proteinCollectionId)
        {
            var sqlQuery =
                "SELECT Annotation_Type_ID " +
                "FROM V_Protein_Collection_Authority " +
                "WHERE Protein_Collection_ID = " + proteinCollectionId;
            
            var annotationTypeIdTable = mSQLAccess.GetTable(sqlQuery);

            DataRow dataRow;
            var authIdSb = new System.Text.StringBuilder();
            foreach (DataRow currentDr in annotationTypeIdTable.Rows)
            {
                dataRow = currentDr;
                authIdSb.Append(dataRow["Annotation_Type_ID"]);
                authIdSb.Append(", ");
            }

            authIdSb.Remove(authIdSb.Length - 2, 2);

            var authSql =
                "SELECT * FROM V_Annotation_Type_Picker " +
                "WHERE ID IN (" + authIdSb + ") " +
                "ORDER BY Display_Name";

            var authorityTable = mSQLAccess.GetTable(authSql);

            dataRow = authorityTable.NewRow();

            dataRow["ID"] = 0;
            dataRow["Display_Name"] = " -- None Selected -- ";
            dataRow["Details"] = "None Selected";

            authorityTable.Rows.InsertAt(dataRow, 0);

            authorityTable.AcceptChanges();

            var pk1 = new DataColumn[1];

            pk1[0] = authorityTable.Columns["ID"];
            authorityTable.PrimaryKey = pk1;

            return authorityTable;
        }

        public DataTable LoadAnnotationTypes()
        {
            const string AuthSQL = "SELECT * FROM V_Annotation_Type_Picker ORDER BY Display_Name";
            var annotationTypeTable = mSQLAccess.GetTable(AuthSQL);

            var dataRow = annotationTypeTable.NewRow();

            dataRow["ID"] = 0;
            dataRow["Display_Name"] = " -- None Selected -- ";
            // dataRow["name"] = " -- None Selected -- ";
            dataRow["Details"] = "None Selected";

            annotationTypeTable.Rows.InsertAt(dataRow, 0);

            annotationTypeTable.AcceptChanges();
            Authorities = mSQLAccess.DataTableToDictionary(annotationTypeTable, "ID", "Display_Name");

            return annotationTypeTable;
        }

        public DataTable LoadAuthorities()
        {
            const string AuthSQL = "SELECT * FROM V_Authority_Picker ORDER BY Display_Name";
            var authorityTable = mSQLAccess.GetTable(AuthSQL);

            var dataRow = authorityTable.NewRow();

            dataRow["ID"] = 0;
            dataRow["Display_Name"] = " -- None Selected -- ";
            dataRow["Details"] = "None Selected";

            authorityTable.Rows.InsertAt(dataRow, 0);

            authorityTable.AcceptChanges();
            Authorities = mSQLAccess.DataTableToDictionary(authorityTable, "ID", "Display_Name");

            return authorityTable;
        }

        public void ClearProteinCollection()
        {
            CollectionMembers?.ClearProteinEntries();
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
            const string pcSql = "SELECT MIN(FileName) AS FileName, Protein_Collection_ID, " +
                        "MIN(Organism_ID) AS Organism_ID, MIN(Authority_ID) AS Authority_ID, " +
                        "MIN(Display) AS Display, MIN(Authentication_Hash) AS Authentication_Hash " +
                        "FROM V_Protein_Collections_By_Organism " +
                        "GROUP BY Protein_Collection_ID " +
                        "ORDER BY MIN(FileName)";

            var proteinCollectionTable = mSQLAccess.GetTable(pcSql);

            var dataRow = proteinCollectionTable.NewRow();

            dataRow["Protein_Collection_ID"] = 0;
            dataRow["Display"] = " -- None Selected -- ";

            proteinCollectionTable.Rows.InsertAt(dataRow, 0);
            proteinCollectionTable.AcceptChanges();

            return proteinCollectionTable;
        }

        protected DataTable LoadProteinCollections(int organismId)
        {
            var sqlQuery = "SELECT FileName, Protein_Collection_ID, Organism_ID, Authority_ID, Display, Authentication_Hash" +
                           " FROM V_Protein_Collections_By_Organism" +
                           " WHERE Organism_ID = " + organismId +
                           " ORDER BY FileName";
            var proteinCollectionTable = mSQLAccess.GetTable(sqlQuery);

            var dataRow = proteinCollectionTable.NewRow();

            dataRow["Protein_Collection_ID"] = 0;
            dataRow["Display"] = " -- None Selected -- ";

            proteinCollectionTable.Rows.InsertAt(dataRow, 0);
            proteinCollectionTable.AcceptChanges();

            return proteinCollectionTable;
        }

        public DataTable LoadProteinCollectionNames()
        {
            const string pcSql =
                "SELECT Protein_Collection_ID, FileName, Authority_ID " +
                "FROM V_Protein_Collections_By_Organism " +
                "ORDER BY FileName";

            var proteinCollectionTable = mSQLAccess.GetTable(pcSql);

            var dataRow = proteinCollectionTable.NewRow();

            dataRow["Protein_Collection_ID"] = 0;
            dataRow["FileName"] = " -- None Selected -- ";

            proteinCollectionTable.Rows.InsertAt(dataRow, 0);
            proteinCollectionTable.AcceptChanges();

            return proteinCollectionTable;
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

            var proteinCollectionTable = mSQLAccess.GetTable(sqlQuery);
            var foundRow = proteinCollectionTable.Rows[0];
            var collectionId = mSQLAccess.DbTools.GetInteger(foundRow["Protein_Collection_ID"]);
            //var authorityId = mSqlAccess.dbTools.GetInteger(foundRow.Item("Primary_Authority_ID"))

            return LoadCollectionMembersById(collectionId, authorityId);
        }

        private DataTable LoadCollectionMembers(string selectStatement)
        {
            var collectionMembersTable = mSQLAccess.GetTable(selectStatement);

            CollectionMembers = LoadProteinInfo(collectionMembersTable.Select(string.Empty));

            return collectionMembersTable;
        }

        protected ProteinStorage.ProteinStorage LoadProteinInfo(DataRow[] proteinCollectionMembers)
        {
            var proteinStorage = new ProteinStorageDMS(string.Empty);
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

            foreach (var dataRow in proteinCollectionMembers)
            {
                var ce = new ProteinStorageEntry(
                    dbTools.GetString(dataRow["Name"]),
                    dbTools.GetString(dataRow["Description"]),
                    dbTools.GetString(dataRow["Sequence"]),
                    dbTools.GetInteger(dataRow["Length"]),
                    dbTools.GetDouble(dataRow["Monoisotopic_Mass"]),
                    dbTools.GetDouble(dataRow["Average_Mass"]),
                    dbTools.GetString(dataRow["Molecular_Formula"]),
                    dbTools.GetString(dataRow["SHA1_Hash"]),
                    counter);

                if (counter % triggerCount > 0)
                {
                    Task_LoadProgress((float)(counter / (double)proteinCount));
                }

                ce.ProteinId = dbTools.GetInteger(dataRow["Protein_ID"]);
                proteinStorage.AddProtein(ce);
                counter++;
            }

            return proteinStorage;
        }

        /// <summary>
        /// Function to load fasta file contents with no checking against the existing database entries
        /// used to load up the source collection ListView
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        [Obsolete("Unused")]
        public DataTable LoadProteinsRaw(
            string filePath,
            ProteinImportFileTypes fileType)
        {
            var proteinDatabaseTable = mSQLAccess.GetTableTemplate("V_Protein_Database_Export");
            var counter = default(int);
            int triggerCount;

            switch (fileType)
            {
                case ProteinImportFileTypes.FASTA:
                    CollectionMembers = LoadFASTA(filePath);
                    break;

                default:
                    return null;
            }

            if (CollectionMembers == null)
            {
                return null;
            }

            var proteinCount = CollectionMembers.ProteinCount;
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
            foreach (var entry in CollectionMembers.GetEntriesIEnumerable())
            {
                var dataRow = proteinDatabaseTable.NewRow();
                dataRow["Name"] = entry.Reference;
                dataRow["Description"] = entry.Description;
                dataRow["Sequence"] = entry.Sequence;
                proteinDatabaseTable.Rows.Add(dataRow);

                if (counter % triggerCount > 0)
                {
                    Task_LoadProgress((float)(counter / (double)proteinCount));
                }

                counter++;
            }

            Task_LoadEnd();
            return proteinDatabaseTable;
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

        [Obsolete("Unused")]
        protected int RunSP_UpdateProteinCollectionsByOrganism()
        {
            var dbTools = mSQLAccess.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionsByOrganism", CommandType.StoredProcedure);

            // Define parameters

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        #endregion
    }
}