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
        // Ignore Spelling: fasta

        /// <summary>
        /// Protein import file types (effectively only FASTA, since Access is deprecated)
        /// </summary>
        public enum ProteinImportFileTypes
        {
            /// <summary>
            /// FASTA file with protein names, descriptions, and sequences
            /// </summary>
            FASTA,

            [Obsolete("Unused import file type")]
            Access
        }

        private readonly DBTask mSQLAccess;

        private readonly FASTAReader mImporter;

        private DataTable mCollectionsList;

        public event LoadStartEventHandler LoadStart;
        public event LoadProgressEventHandler LoadProgress;
        public event LoadEndEventHandler LoadEnd;
        public event CollectionLoadCompleteEventHandler CollectionLoadComplete;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnectionString">Protein sequences database connection string</param>
        public ImportHandler(string dbConnectionString)
        {
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(dbConnectionString, "OrganismDatabaseHandler");

            mSQLAccess = new DBTask(connectionStringToUse);
            RegisterEvents(mSQLAccess);

            mImporter = new FASTAReader(connectionStringToUse);
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
            var foundRows = mCollectionsList.Select("protein_collection_id = " + proteinCollectionId);
            var dataRow = foundRows[0];
            var collectionName = mSQLAccess.DbTools.GetString(dataRow["collection_name"]);

            return collectionName;
        }

        protected ProteinStorage.ProteinStorage LoadFASTA(string filePath)
        {
            // check for existence of current file
            var fastaContents = mImporter.GetProteinEntries(filePath);

            var errorMessage = mImporter.LastErrorMessage;

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                var proteinsLoaded = 0;

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
            const string orgSQL =
                "SELECT id, short_name, display_name, storage_location, organism_name, organism_name_abbrev_genus, og_short_name, search_terms, collection_count " +
                "FROM v_organism_picker " +
                "ORDER BY short_name";

            var organismTable = mSQLAccess.GetTable(orgSQL);

            var dataRow = organismTable.NewRow();

            dataRow["id"] = 0;
            dataRow["short_name"] = "None";
            dataRow["display_name"] = " -- None Selected -- ";

            organismTable.Rows.InsertAt(dataRow, 0);

            organismTable.AcceptChanges();

            var pk1 = new DataColumn[1];

            pk1[0] = organismTable.Columns["id"];
            organismTable.PrimaryKey = pk1;

            return organismTable;
        }

        [Obsolete("Unused")]
        public DataTable LoadAnnotationTypes(
            int proteinCollectionId)
        {
            var sqlQuery =
                "SELECT annotation_type_id " +
                "FROM v_protein_collection_authority " +
                "WHERE protein_collection_id = " + proteinCollectionId;

            var annotationTypeIdTable = mSQLAccess.GetTable(sqlQuery);

            DataRow dataRow;
            var authIdSb = new System.Text.StringBuilder();

            foreach (DataRow currentDr in annotationTypeIdTable.Rows)
            {
                dataRow = currentDr;
                authIdSb.Append(dataRow["annotation_type_id"]);
                authIdSb.Append(", ");
            }

            authIdSb.Remove(authIdSb.Length - 2, 2);

            var authSql =
                "SELECT id, display_name, details, authority_id " +
                "FROM v_annotation_type_picker " +
                "WHERE id IN (" + authIdSb + ") " +
                "ORDER BY display_name";

            var authorityTable = mSQLAccess.GetTable(authSql);

            dataRow = authorityTable.NewRow();

            dataRow["id"] = 0;
            dataRow["display_name"] = " -- None Selected -- ";
            dataRow["details"] = "None Selected";

            authorityTable.Rows.InsertAt(dataRow, 0);

            authorityTable.AcceptChanges();

            var pk1 = new DataColumn[1];

            pk1[0] = authorityTable.Columns["id"];
            authorityTable.PrimaryKey = pk1;

            return authorityTable;
        }

        public DataTable LoadAnnotationTypes()
        {
            const string AuthSQL = "SELECT id, display_name, details, authority_id FROM v_annotation_type_picker ORDER BY display_name";
            var annotationTypeTable = mSQLAccess.GetTable(AuthSQL);

            var dataRow = annotationTypeTable.NewRow();

            dataRow["id"] = 0;
            dataRow["display_name"] = " -- None Selected -- ";
            // dataRow["name"] = " -- None Selected -- ";
            dataRow["details"] = "None Selected";

            annotationTypeTable.Rows.InsertAt(dataRow, 0);

            annotationTypeTable.AcceptChanges();
            Authorities = mSQLAccess.DataTableToDictionary(annotationTypeTable, "id", "display_name");

            return annotationTypeTable;
        }

        public DataTable LoadAuthorities()
        {
            const string AuthSQL = "SELECT id, display_name, details FROM v_authority_picker ORDER BY display_name";
            var authorityTable = mSQLAccess.GetTable(AuthSQL);

            var dataRow = authorityTable.NewRow();

            dataRow["id"] = 0;
            dataRow["display_name"] = " -- None Selected -- ";
            dataRow["details"] = "None Selected";

            authorityTable.Rows.InsertAt(dataRow, 0);

            authorityTable.AcceptChanges();
            Authorities = mSQLAccess.DataTableToDictionary(authorityTable, "id", "display_name");

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
            const string pcSql =
                "SELECT min(collection_name) as collection_name, protein_collection_id, " +
                "       min(organism_id) as organism_id, min(authority_id) as authority_id, " +
                "       min(display) as display, min(authentication_hash) as authentication_hash " +
                "FROM v_protein_collections_by_organism " +
                "GROUP BY protein_collection_id " +
                "ORDER BY min(collection_name)";

            var proteinCollectionTable = mSQLAccess.GetTable(pcSql);

            var dataRow = proteinCollectionTable.NewRow();

            dataRow["protein_collection_id"] = 0;
            dataRow["display"] = " -- None Selected -- ";

            proteinCollectionTable.Rows.InsertAt(dataRow, 0);
            proteinCollectionTable.AcceptChanges();

            return proteinCollectionTable;
        }

        protected DataTable LoadProteinCollections(int organismId)
        {
            var sqlQuery =
                "SELECT collection_name, protein_collection_id, organism_id, authority_id, display, authentication_hash " +
                "FROM v_protein_collections_by_organism " +
                "WHERE organism_id = " + organismId + " " +
                "ORDER BY collection_name";
            var proteinCollectionTable = mSQLAccess.GetTable(sqlQuery);

            var dataRow = proteinCollectionTable.NewRow();

            dataRow["protein_collection_id"] = 0;
            dataRow["display"] = " -- None Selected -- ";

            proteinCollectionTable.Rows.InsertAt(dataRow, 0);
            proteinCollectionTable.AcceptChanges();

            return proteinCollectionTable;
        }

        public DataTable LoadProteinCollectionNames()
        {
            const string pcSql =
                "SELECT protein_collection_id, collection_name, authority_id " +
                "FROM v_protein_collections_by_organism " +
                "ORDER BY collection_name";

            var proteinCollectionTable = mSQLAccess.GetTable(pcSql);

            var dataRow = proteinCollectionTable.NewRow();

            dataRow["protein_collection_id"] = 0;
            dataRow["collection_name"] = " -- None Selected -- ";

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
                var foundRows = mCollectionsList.Select("protein_collection_id = " + collectionId);
                authorityId = mSQLAccess.DbTools.GetInteger(foundRows[0]["authority_id"]);
            }

            var sqlQuery =
                "SELECT name, description, sequence, monoisotopic_mass, average_mass, length, molecular_formula, " +
                "       annotation_type_id, protein_id, reference_id, protein_collection_id, " +
                "       primary_annotation_type_id, sha1_hash, member_id, sorting_index " +
                "FROM v_protein_storage_entry_import " +
                "WHERE protein_collection_id = " + collectionId + " " +
                "AND annotation_type_id = " + authorityId + " " +
                "ORDER BY name";
            return LoadCollectionMembers(sqlQuery);
        }

        [Obsolete("Unused")]
        public DataTable LoadCollectionMembersByName(
            string collectionName,
            int authorityId)
        {
            var sqlQuery =
                "SELECT protein_collection_id, primary_annotation_type_id " +
                "FROM t_protein_collections " +
                "WHERE collection_name = '" + collectionName + "' ORDER BY collection_name";

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
            var counter = 0;

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
                    dbTools.GetString(dataRow["name"]),
                    dbTools.GetString(dataRow["description"]),
                    dbTools.GetString(dataRow["sequence"]),
                    dbTools.GetInteger(dataRow["length"]),
                    dbTools.GetDouble(dataRow["monoisotopic_mass"]),
                    dbTools.GetDouble(dataRow["average_mass"]),
                    dbTools.GetString(dataRow["molecular_formula"]),
                    dbTools.GetString(dataRow["sha1_hash"]),
                    counter);

                if (counter % triggerCount > 0)
                {
                    Task_LoadProgress((float)(counter / (double)proteinCount));
                }

                ce.ProteinId = dbTools.GetInteger(dataRow["protein_id"]);
                proteinStorage.AddProtein(ce);
                counter++;
            }

            return proteinStorage;
        }

        /// <summary>
        /// Function to load FASTA file contents with no checking against the existing database entries
        /// used to load up the source collection ListView
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        [Obsolete("Unused")]
        public DataTable LoadProteinsRaw(
            string filePath,
            ProteinImportFileTypes fileType)
        {
            var proteinDatabaseTable = mSQLAccess.GetTableTemplate("v_protein_database_export");
            var counter = 0;
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
                dataRow["name"] = entry.Reference;
                dataRow["description"] = entry.Description;
                dataRow["sequence"] = entry.Sequence;
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
            return LoadFASTA(fullFilePath);
        }

        // Handles the LoadStart event for the FASTA importer module
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

        [Obsolete("Unused")]
        protected int RunSP_UpdateProteinCollectionsByOrganism()
        {
            var dbTools = mSQLAccess.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionsByOrganism", CommandType.StoredProcedure);

            // Define parameters

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return DBToolsBase.GetReturnCode(returnParam);
        }
    }
}