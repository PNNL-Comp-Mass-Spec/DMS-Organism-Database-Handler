using System;
using System.Collections.Generic;
using System.Data;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinExport;
using OrganismDatabaseHandler.ProteinStorage;
using PRISM;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.ProteinImport
{
    public class AddUpdateEntries : EventNotifier
    {
        // Ignore Spelling: Passphrase, Sha, Xref

        public enum SpModes
        {
            Add,
            // Unused: Update,
        }

        public enum CollectionStates
        {
            NewEntry = 1,
            // Unused: Provisional = 2,
            // Unused: Production = 3,
            // Unused: Historical = 4,
        }

        public enum CollectionTypes
        {
            ProtOriginalSource = 1,
            // Unused: ModifiedSource = 2,
            // Unused: RunTimeCombinedCollection = 3,
            // Unused: LoadTimeCombinedCollection = 4,
            // Unused: NucleotideOriginalSource = 5,
        }

        private readonly DBTask mDatabaseAccessor;

        private readonly System.Security.Cryptography.SHA1Managed mHasher;

        #region "Properties"

        public int MaximumProteinNameLength { get; set; }

        #endregion

        #region "Events"
        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadProgressEventHandler LoadProgress;

        private void OnLoadStart(string taskTitle)
        {
            LoadStart?.Invoke(taskTitle);
        }

        private void OnProgressUpdate(double fractionDone)
        {
            LoadProgress?.Invoke(fractionDone);
        }

        private void OnLoadEnd()
        {
            LoadEnd?.Invoke();
        }

        #endregion

        public AddUpdateEntries(string pisConnectionString)
        {
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(pisConnectionString, "OrganismDatabaseHandler");

            mDatabaseAccessor = new DBTask(connectionStringToUse);
            RegisterEvents(mDatabaseAccessor);

            mHasher = new System.Security.Cryptography.SHA1Managed();
        }

        /// <summary>
        /// Checks for the existence of protein sequences in the T_Proteins table
        /// Gets Protein_ID if located, makes a new entry if not
        /// Updates Protein_ID field in ProteinStorageEntry instance
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="selectedProteinList"></param>
        public void CompareProteinId(
            ProteinStorage.ProteinStorage pc,
            List<string> selectedProteinList)
        {
            OnLoadStart("Comparing to existing sequences and adding new proteins");
            var counterMax = selectedProteinList.Count;
            var counter = 0;

            int eventTriggerThresh;

            if (counterMax <= 100)
            {
                eventTriggerThresh = 1;
            }
            else
            {
                eventTriggerThresh = (int)Math.Round(counterMax / 100d);

                if (eventTriggerThresh > 100)
                    eventTriggerThresh = 100;
            }

            foreach (var s in selectedProteinList)
            {
                var storageEntry = pc.GetProtein(s);

                counter++;

                if (counter % eventTriggerThresh == 0)
                {
                    OnProgressUpdate(counter / (double)counterMax);
                }

                storageEntry.ProteinId = AddProteinSequence(storageEntry);
            }

            OnLoadEnd();
        }

        public void UpdateProteinNames(
            ProteinStorage.ProteinStorage pc,
            List<string> selectedProteinList,
            int authorityId)
        {
            OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection");
            var counter = 0;
            var counterMax = selectedProteinList.Count;

            int eventTriggerThresh;

            if (counterMax <= 100)
            {
                eventTriggerThresh = 1;
            }
            else
            {
                eventTriggerThresh = (int)Math.Round(counterMax / 100d);

                if (eventTriggerThresh > 100)
                    eventTriggerThresh = 100;
            }

            foreach (var protein in selectedProteinList)
            {
                var storageEntry = pc.GetProtein(protein);

                counter++;

                if (counter % eventTriggerThresh == 0)
                {
                    OnProgressUpdate(counter / (double)counterMax);
                }

                storageEntry.ReferenceId = AddProteinReference(storageEntry.Reference, storageEntry.Description, authorityId, storageEntry.ProteinId, MaximumProteinNameLength);
            }

            OnLoadEnd();
        }

        public void UpdateProteinCollectionMembers(
            int proteinCollectionId,
            ProteinStorage.ProteinStorage pc,
            List<string> selectedProteinList,
            int numProteinsExpected,
            int numResiduesExpected)
        {
            var counterMax = selectedProteinList.Count;

            int eventTriggerThresh;

            if (counterMax <= 100)
            {
                eventTriggerThresh = 1;
            }
            else
            {
                eventTriggerThresh = (int)Math.Round(counterMax / 100d);

                if (eventTriggerThresh > 100)
                    eventTriggerThresh = 100;
            }

            OnLoadStart("Storing Protein Collection Members");

            var numProteinsActual = 0;
            var numResiduesActual = 0;

            foreach (var protein in selectedProteinList)
            {
                var storageEntry = pc.GetProtein(protein);

                numProteinsActual++;

                if (numProteinsActual % eventTriggerThresh == 0)
                {
                    OnProgressUpdate(numProteinsActual / (double)counterMax);
                }

                numResiduesActual += storageEntry.Length;

                storageEntry.MemberId = AddProteinCollectionMember(storageEntry.ReferenceId, storageEntry.ProteinId, storageEntry.SortingIndex, proteinCollectionId);
            }

            if (numProteinsActual != numProteinsExpected)
            {
                ConsoleMsgUtils.ShowWarning(
                    "Number of proteins in selectedProteinList does not match the expected value: {0} actual vs. {1} expected",
                    numProteinsActual,
                    numProteinsExpected);
            }

            if (numResiduesActual != numResiduesExpected)
            {
                ConsoleMsgUtils.ShowWarning(
                    "Number of residues in the proteins in selectedProteinList does not match the expected value: {0} actual vs. {1} expected",
                    numResiduesActual,
                    numResiduesExpected);
            }

            if (numResiduesActual == 0 && numResiduesExpected > 0)
            {
                numResiduesActual = numResiduesExpected;
            }

            RunSP_UpdateProteinCollectionCounts(numProteinsActual, numResiduesActual, proteinCollectionId);

            OnLoadEnd();
        }

        public int GetTotalResidueCount(
            ProteinStorage.ProteinStorage proteinCollection,
            List<string> selectedProteinList)
        {
            var totalLength = 0;

            foreach (var protein in selectedProteinList)
            {
                var storageEntry = proteinCollection.GetProtein(protein);
                totalLength += storageEntry.Sequence.Length;
            }

            return totalLength;
        }

        /// <summary>
        /// Add the protein
        /// </summary>
        /// <param name="protein"></param>
        /// <returns>Protein ID</returns>
        protected int AddProteinSequence(ProteinStorageEntry protein)
        {
            return RunSP_AddProteinSequence(
                protein.Sequence,
                protein.Length,
                protein.MolecularFormula,
                protein.MonoisotopicMass,
                protein.AverageMass,
                protein.SHA1Hash,
                protein.IsEncrypted,
                SpModes.Add);
        }

        [Obsolete("Unused")]
        public int UpdateProteinSequenceInfo(
            int proteinId,
            string sequence,
            int length,
            string molecularFormula,
            double monoisotopicMass,
            double averageMass,
            string sha1Hash)
        {
            RunSP_UpdateProteinSequenceInfo(
                proteinId,
                sequence,
                length,
                molecularFormula,
                monoisotopicMass,
                averageMass,
                sha1Hash);

            return 0;
        }

        public int AddNamingAuthority(
            string shortName,
            string fullName,
            string webAddress)
        {
            return RunSP_AddNamingAuthority(shortName, fullName, webAddress);
        }

        public int AddAnnotationType(
            string typeName,
            string description,
            string example,
            int authorityId)
        {
            return RunSP_AddAnnotationType(typeName, description, example, authorityId);
        }

        public int MakeNewProteinCollection(
            string proteinCollectionName,
            string description,
            string collectionSource,
            CollectionTypes collectionType,
            int annotationTypeId,
            int numProteins,
            int numResidues)
        {
            return RunSP_AddUpdateProteinCollection(
                proteinCollectionName, description, collectionSource, collectionType, CollectionStates.NewEntry,
                annotationTypeId, numProteins, numResidues, SpModes.Add);
        }

        public int UpdateEncryptionMetadata(
            int proteinCollectionId,
            string passphrase)
        {
            return RunSP_AddUpdateEncryptionMetadata(passphrase, proteinCollectionId);
        }

        public int UpdateProteinCollectionState(
            int proteinCollectionId,
            int collectionStateId)
        {
            return RunSP_UpdateProteinCollectionStates(proteinCollectionId, collectionStateId);
        }

        public string GetProteinCollectionState(int proteinCollectionId)
        {
            return RunSP_GetProteinCollectionState(proteinCollectionId);
        }

        /// <summary>
        /// Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
        /// </summary>
        /// <param name="proteinCollectionId"></param>
        /// <param name="numProteins"></param>
        public void DeleteProteinCollectionMembers(int proteinCollectionId, int numProteins)
        {
            RunSP_DeleteProteinCollectionMembers(proteinCollectionId, numProteins);
        }

        public int GetProteinCollectionId(string proteinCollectionName)
        {
            return GetProteinCollectionID(proteinCollectionName);
        }

        public int AddCollectionOrganismXref(int proteinCollectionId, int organismId)
        {
            return RunSP_AddCollectionOrganismXref(proteinCollectionId, organismId);
        }

        public int AddProteinCollectionMember(
            int referenceId,
            int proteinId,
            int sortingIndex,
            int proteinCollectionId)
        {
            return RunSP_AddProteinCollectionMember(referenceId, proteinId, sortingIndex, proteinCollectionId);
        }

        public int AddProteinReference(
            string proteinName,
            string description,
            int authorityId,
            int proteinId,
            int maxProteinNameLength)
        {
            return RunSP_AddProteinReference(proteinName, description, authorityId, proteinId, maxProteinNameLength);
        }

        public int AddAuthenticationHash(
            int proteinCollectionId,
            string authenticationHash,
            int numProteins,
            int totalResidues)
        {
            return RunSP_AddCRC32FileAuthentication(proteinCollectionId, authenticationHash, numProteins, totalResidues);
        }

        public int UpdateProteinNameHash(
            int referenceId,
            string proteinName,
            string description,
            int proteinId)
        {
            return RunSP_UpdateProteinNameHash(referenceId, proteinName, description, proteinId);
        }

        public string GenerateHash(string sourceText)
        {
            // Create an encoding object to ensure the encoding standard for the source text
            var encoding = new System.Text.ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var byteSourceText = encoding.GetBytes(sourceText);

            // Compute the hash value from the source
            var sha1Hash = mHasher.ComputeHash(byteSourceText);

            // And convert it to String format for return
            return RijndaelEncryptionHandler.ToHexString(sha1Hash);
        }

        #region "Stored Procedure Access"

        protected string RunSP_GetProteinCollectionState(
            int proteinCollectionId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("get_protein_collection_state", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@collectionID", SqlType.Int).Value = proteinCollectionId;

            var stateNameParam = dbTools.AddParameter(cmdSave, "@stateName", SqlType.VarChar, 32, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            var returnValue = DBToolsBase.GetReturnCode(returnParam);

            if (returnValue != 0)
            {
                OnWarningEvent("Procedure get_protein_collection_state returned a non-zero return code: " + returnValue);
            }

            return dbTools.GetString(stateNameParam.Value);
        }

        protected int RunSP_AddProteinSequence(
            string sequence,
            int length,
            string molecularFormula,
            double monoisotopicMass,
            double averageMass,
            string sha1Hash,
            bool isEncrypted,
            SpModes mode)
        {
            var encryptionFlag = 0;

            if (isEncrypted)
            {
                encryptionFlag = 1;
            }

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_protein_sequence", CommandType.StoredProcedure);

            // Use a 5-minute timeout
            cmdSave.CommandTimeout = 300;

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence;
            dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length;
            dbTools.AddParameter(cmdSave, "@molecularFormula", SqlType.VarChar, 128).Value = molecularFormula;
            dbTools.AddParameter(cmdSave, "@monoisotopicMass", SqlType.Float, 8).Value = monoisotopicMass;
            dbTools.AddParameter(cmdSave, "@averageMass", SqlType.Float, 8).Value = averageMass;
            dbTools.AddParameter(cmdSave, "@sha1Hash", SqlType.VarChar, 40).Value = sha1Hash;
            dbTools.AddParameter(cmdSave, "@isEncrypted", SqlType.TinyInt).Value = encryptionFlag;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString();
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        [Obsolete("Unused")]
        protected int RunSP_UpdateProteinSequenceInfo(
            int proteinId,
            string sequence,
            int length,
            string molecularFormula,
            double monoisotopicMass,
            double averageMass,
            string sha1Hash)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("update_protein_sequence_info", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@proteinID", SqlType.Int).Value = proteinId;
            dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence;
            dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length;
            dbTools.AddParameter(cmdSave, "@molecularFormula", SqlType.VarChar, 128).Value = molecularFormula;
            dbTools.AddParameter(cmdSave, "@monoisotopicMass", SqlType.Float, 8).Value = monoisotopicMass;
            dbTools.AddParameter(cmdSave, "@averageMass", SqlType.Float, 8).Value = averageMass;
            dbTools.AddParameter(cmdSave, "@sha1Hash", SqlType.VarChar, 40).Value = sha1Hash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddUpdateProteinCollection(
            string proteinCollectionName,
            string description,
            string collectionSource,
            CollectionTypes collectionType,
            CollectionStates collectionState,
            int annotationTypeId,
            int numProteins,
            int numResidues,
            SpModes mode)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_update_protein_collection", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@collectionName", SqlType.VarChar, 128).Value = proteinCollectionName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 900).Value = description;
            dbTools.AddParameter(cmdSave, "@collectionSource", SqlType.VarChar, 900).Value = collectionSource;
            dbTools.AddParameter(cmdSave, "@collectionType", SqlType.Int).Value = (int)collectionType;
            dbTools.AddParameter(cmdSave, "@collectionState", SqlType.Int).Value = (int)collectionState;
            dbTools.AddParameter(cmdSave, "@primaryAnnotationTypeId", SqlType.Int).Value = annotationTypeId;
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@numResidues", SqlType.Int).Value = numResidues;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString();
            var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            var returnCode = DBToolsBase.GetReturnCode(returnParam);

            if (returnCode == 0)
            {
                // A zero was returned for the protein collection ID; this indicates an error
                // Raise an exception

                var msg = "Procedure add_update_protein_collection returned 0 for the Protein Collection ID";

                var spMsg = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(spMsg))
                    msg += "; " + spMsg;
                throw new ConstraintException(msg);
            }

            return returnCode;
        }

        protected int RunSP_AddProteinCollectionMember(
            int referenceId, int proteinId,
            int sortingIndex, int proteinCollectionId)
        {
            return RunSP_AddUpdateProteinCollectionMember(referenceId, proteinId, sortingIndex, proteinCollectionId, "Add");
        }

        [Obsolete("Unused")]
        protected int RunSP_UpdateProteinCollectionMember(
            int referenceId, int proteinId,
            int sortingIndex, int proteinCollectionId)
        {
            return RunSP_AddUpdateProteinCollectionMember(referenceId, proteinId, sortingIndex, proteinCollectionId, "Update");
        }

        protected int RunSP_AddUpdateProteinCollectionMember(
            int referenceId, int proteinId,
            int sortingIndex, int proteinCollectionId,
            string mode)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_update_protein_collection_member", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // If querying a Postgres DB, dbTools will auto-change "@return" to "_returnCode"
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@referenceID", SqlType.Int).Value = referenceId;
            dbTools.AddParameter(cmdSave, "@proteinID", SqlType.Int).Value = proteinId;
            dbTools.AddParameter(cmdSave, "@sortingIndex", SqlType.Int).Value = sortingIndex;
            dbTools.AddParameter(cmdSave, "@proteinCollectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 10).Value = mode;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddUpdateEncryptionMetadata(
            string passphrase, int proteinCollectionId)
        {
            var phraseHash = GenerateHash(passphrase);

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_update_encryption_metadata", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@proteinCollectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@encryptionPassphrase", SqlType.VarChar, 64).Value = passphrase;
            dbTools.AddParameter(cmdSave, "@passphraseSHA1Hash", SqlType.VarChar, 40).Value = phraseHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddNamingAuthority(
            string shortName,
            string fullName,
            string webAddress)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_naming_authority", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = shortName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = fullName;
            dbTools.AddParameter(cmdSave, "@webAddress", SqlType.VarChar, 128).Value = webAddress;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddAnnotationType(
            string typeName,
            string description,
            string example,
            int authorityId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_annotation_type", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = typeName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = description;
            dbTools.AddParameter(cmdSave, "@example", SqlType.VarChar, 128).Value = example;
            dbTools.AddParameter(cmdSave, "@authID", SqlType.Int).Value = authorityId;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)

            // The return code will be the annotation type ID of the newly inserted row in table t_annotation_types
            // However, if the annotation type already existed, the return code will be the negative value of the existing annotation type ID
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_UpdateProteinCollectionStates(
            int proteinCollectionId,
            int collectionStateId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("update_protein_collection_state", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@proteinCollectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@stateID", SqlType.Int).Value = collectionStateId;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        /// <summary>
        /// Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
        /// </summary>
        /// <remarks>NumResidues in T_Protein_Collections is set to 0</remarks>
        /// <param name="proteinCollectionId"></param>
        /// <param name="numProteinsForReload">The number of proteins that will be uploaded after this delete</param>
        protected int RunSP_DeleteProteinCollectionMembers(int proteinCollectionId, int numProteinsForReload)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("delete_protein_collection_members", CommandType.StoredProcedure);

            // Use a 10-minute timeout
            cmdSave.CommandTimeout = 600;

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@collectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@numProteinsForReload", SqlType.Int).Value = numProteinsForReload;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddProteinReference(
              string proteinName,
              string description,
              int authorityId,
              int proteinId,
              int maxProteinNameLength)
        {
            if (maxProteinNameLength <= 0)
                maxProteinNameLength = 32;

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_protein_reference", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            // Although the datatype is specified as an integer here, dbTools will change it to text
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@proteinName", SqlType.VarChar, 128).Value = proteinName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 900).Value = description;
            dbTools.AddParameter(cmdSave, "@authorityID", SqlType.Int).Value = authorityId;
            dbTools.AddParameter(cmdSave, "@proteinID", SqlType.Int).Value = proteinId;

            var textToHash = proteinName + "_" + description + "_" + proteinId;
            dbTools.AddParameter(cmdSave, "@nameDescHash", SqlType.VarChar, 40).Value = GenerateHash(textToHash.ToLower());

            var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);
            dbTools.AddParameter(cmdSave, "@maxProteinNameLength", SqlType.Int).Value = maxProteinNameLength;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            var returnCode = DBToolsBase.GetReturnCode(returnParam);

            if (returnCode == 0)
            {
                // A zero was returned for the protein reference ID; this indicates an error
                // Raise an exception

                var msg = "Procedure add_protein_reference returned 0";

                var spMsg = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(spMsg))
                    msg += "; " + spMsg;

                throw new ConstraintException(msg);
            }

            return returnCode;
        }

        protected int GetProteinCollectionID(string proteinCollectionName)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var query = string.Format("SELECT protein_collection_id FROM t_protein_collections WHERE collection_name = '{0}'", proteinCollectionName);

            var dbCmd = dbTools.CreateCommand(query);

            var success = dbTools.GetQueryScalar(dbCmd, out var queryResult, 1);

            if (!success)
            {
                throw new Exception(string.Format("Error looking up the protein collection ID using {0}", query));
            }

            return queryResult.CastDBVal<int>();
        }

        [Obsolete("Use GetProteinCollectionID instead")]
        protected int RunSP_GetProteinCollectionID(string proteinCollectionName)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("get_protein_collection_id", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@collectionName", SqlType.VarChar, 128).Value = proteinCollectionName;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddCRC32FileAuthentication(
            int proteinCollectionId,
            string authenticationHash,
            int numProteins,
            int totalResidueCount)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_crc32_file_authentication", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@collectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@crc32FileHash", SqlType.VarChar, 40).Value = authenticationHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@totalResidueCount", SqlType.Int).Value = totalResidueCount;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_AddCollectionOrganismXref(
            int proteinCollectionId,
            int organismId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("add_collection_organism_xref", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@proteinCollectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@organismID", SqlType.Int).Value = organismId;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_UpdateProteinNameHash(
            int referenceId,
            string proteinName,
            string description,
            int proteinId)
        {
            var nameToHash = proteinName + "_" + description + "_" + proteinId;
            var sha1Hash = GenerateHash(nameToHash.ToLower());

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("update_protein_name_hash", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@referenceID", SqlType.Int).Value = referenceId;
            dbTools.AddParameter(cmdSave, "@sha1Hash", SqlType.VarChar, 40).Value = sha1Hash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        protected int RunSP_UpdateProteinCollectionCounts(
            int numProteins,
            int numResidues,
            int proteinCollectionId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("update_protein_collection_counts", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@collectionID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@numResidues", SqlType.Int).Value = numResidues;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.InputOutput);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // The return code is an integer on SQL Server, but is text on Postgres
            // Use GetReturnCode to obtain the integer, or find the first integer in the text-based return code (-1 if no integer is found)
            return DBToolsBase.GetReturnCode(returnParam);
        }

        #endregion
    }
}