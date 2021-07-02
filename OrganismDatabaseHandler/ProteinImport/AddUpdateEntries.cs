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
            mDatabaseAccessor = new DBTask(pisConnectionString);
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
            var counter = default(int);

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
                var tmpPc = pc.GetProtein(s);

                counter++;
                if (counter % eventTriggerThresh == 0)
                {
                    OnProgressUpdate(counter / (double)counterMax);
                }

                tmpPc.ProteinId = AddProteinSequence(tmpPc);
            }

            OnLoadEnd();
        }

        public void UpdateProteinNames(
            ProteinStorage.ProteinStorage pc,
            List<string> selectedProteinList,
            int organismId,
            int authorityId)
        {
            OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection");
            var counter = default(int);
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

            foreach (var s in selectedProteinList)
            {
                var tmpPc = pc.GetProtein(s);
                counter++;
                if (counter % eventTriggerThresh == 0)
                {
                    OnProgressUpdate(counter / (double)counterMax);
                }

                tmpPc.ReferenceId = AddProteinReference(tmpPc.Reference, tmpPc.Description, organismId, authorityId, tmpPc.ProteinId, MaximumProteinNameLength);
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

            foreach (var s in selectedProteinList)
            {
                var tmpPc = pc.GetProtein(s);
                numProteinsActual++;
                if (numProteinsActual % eventTriggerThresh == 0)
                {
                    OnProgressUpdate(numProteinsActual / (double)counterMax);
                }

                numResiduesActual += tmpPc.Length;

                tmpPc.MemberId = AddProteinCollectionMember(tmpPc.ReferenceId, tmpPc.ProteinId, tmpPc.SortingIndex, proteinCollectionId);
            }

            RunSP_UpdateProteinCollectionCounts(numProteinsActual, numResiduesActual, proteinCollectionId);

            OnLoadEnd();
        }

        public int GetTotalResidueCount(
            ProteinStorage.ProteinStorage proteinCollection,
            List<string> selectedProteinList)
        {
            var totalLength = default(int);

            foreach (var s in selectedProteinList)
            {
                var tmpPc = proteinCollection.GetProtein(s);
                totalLength += tmpPc.Sequence.Length;
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
            return RunSP_GetProteinCollectionID(proteinCollectionName);
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
            int organismId,
            int authorityId,
            int proteinId,
            int maxProteinNameLength)
        {
            return RunSP_AddProteinReference(proteinName, description, organismId, authorityId, proteinId, maxProteinNameLength);
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

            var cmdSave = dbTools.CreateCommand("GetProteinCollectionState", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionId;

            var stateNameParam = dbTools.AddParameter(cmdSave, "@State_Name", SqlType.VarChar, 32, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);

            var stateName = dbTools.GetString(stateNameParam.Value);

            return stateName;
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

            var cmdSave = dbTools.CreateCommand("AddProteinSequence", CommandType.StoredProcedure);

            // Use a 5 minute timeout
            cmdSave.CommandTimeout = 300;

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence;
            dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length;
            dbTools.AddParameter(cmdSave, "@molecular_formula", SqlType.VarChar, 128).Value = molecularFormula;
            dbTools.AddParameter(cmdSave, "@monoisotopic_mass", SqlType.Float, 8).Value = monoisotopicMass;
            dbTools.AddParameter(cmdSave, "@average_mass", SqlType.Float, 8).Value = averageMass;
            dbTools.AddParameter(cmdSave, "@sha1_hash", SqlType.VarChar, 40).Value = sha1Hash;
            dbTools.AddParameter(cmdSave, "@is_encrypted", SqlType.TinyInt).Value = encryptionFlag;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString();
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

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

            var cmdSave = dbTools.CreateCommand("UpdateProteinSequenceInfo", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_ID", SqlType.Int).Value = proteinId;
            dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence;
            dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length;
            dbTools.AddParameter(cmdSave, "@molecular_formula", SqlType.VarChar, 128).Value = molecularFormula;
            dbTools.AddParameter(cmdSave, "@monoisotopic_mass", SqlType.Float, 8).Value = monoisotopicMass;
            dbTools.AddParameter(cmdSave, "@average_mass", SqlType.Float, 8).Value = averageMass;
            dbTools.AddParameter(cmdSave, "@sha1_hash", SqlType.VarChar, 40).Value = sha1Hash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
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

            var cmdSave = dbTools.CreateCommand("AddUpdateProteinCollection", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            // Note that the @fileName parameter is actually the protein collection name; not the original .fasta file name
            dbTools.AddParameter(cmdSave, "@fileName", SqlType.VarChar, 128).Value = proteinCollectionName;
            dbTools.AddParameter(cmdSave, "@Description", SqlType.VarChar, 900).Value = description;
            dbTools.AddParameter(cmdSave, "@collectionSource", SqlType.VarChar, 900).Value = collectionSource;
            dbTools.AddParameter(cmdSave, "@collection_type", SqlType.Int).Value = (int)collectionType;
            dbTools.AddParameter(cmdSave, "@collection_state", SqlType.Int).Value = (int)collectionState;
            dbTools.AddParameter(cmdSave, "@primary_annotation_type_id", SqlType.Int).Value = annotationTypeId;
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@numResidues", SqlType.Int).Value = numResidues;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString();
            var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);
            if (ret == 0)
            {
                // A zero was returned for the protein collection ID; this indicates and error
                // Raise an exception

                var msg = "AddUpdateProteinCollection returned 0 for the Protein Collection ID";

                var spMsg = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(spMsg))
                    msg += "; " + spMsg;
                throw new ConstraintException(msg);
            }

            return ret;
        }

        protected int RunSP_AddProteinCollectionMember(
            int referenceId, int proteinId,
            int sortingIndex, int proteinCollectionId)
        {
            return RunSP_AddUpdateProteinCollectionMember(referenceId, proteinId, sortingIndex, proteinCollectionId, "Add");
        }

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

            var cmdSave = dbTools.CreateCommand("AddUpdateProteinCollectionMember_New", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@reference_ID", SqlType.Int).Value = referenceId;
            dbTools.AddParameter(cmdSave, "@protein_ID", SqlType.Int).Value = proteinId;
            dbTools.AddParameter(cmdSave, "@sorting_index", SqlType.Int).Value = sortingIndex;
            dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 10).Value = mode;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_AddUpdateEncryptionMetadata(
            string passphrase, int proteinCollectionId)
        {
            var phraseHash = GenerateHash(passphrase);

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddUpdateEncryptionMetadata", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_Collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@Encryption_Passphrase", SqlType.VarChar, 64).Value = passphrase;
            dbTools.AddParameter(cmdSave, "@Passphrase_SHA1_Hash", SqlType.VarChar, 40).Value = phraseHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_AddNamingAuthority(
            string shortName,
            string fullName,
            string webAddress)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddNamingAuthority", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = shortName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = fullName;
            dbTools.AddParameter(cmdSave, "@web_address", SqlType.VarChar, 128).Value = webAddress;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_AddAnnotationType(
            string typeName,
            string description,
            string example,
            int authorityId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddAnnotationType", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = typeName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = description;
            dbTools.AddParameter(cmdSave, "@example", SqlType.VarChar, 128).Value = example;
            dbTools.AddParameter(cmdSave, "@authID", SqlType.Int).Value = authorityId;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_UpdateProteinCollectionStates(
            int proteinCollectionId,
            int collectionStateId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionState", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@state_ID", SqlType.Int).Value = collectionStateId;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        /// <summary>
        /// Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
        /// </summary>
        /// <param name="proteinCollectionId"></param>
        /// <param name="numProteinsForReLoad">The number of proteins that will be uploaded after this delete</param>
        /// <remarks>NumResidues in T_Protein_Collections is set to 0</remarks>
        protected int RunSP_DeleteProteinCollectionMembers(int proteinCollectionId, int numProteinsForReLoad)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("DeleteProteinCollectionMembers", CommandType.StoredProcedure);

            // Use a 10 minute timeout
            cmdSave.CommandTimeout = 600;

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@NumProteinsForReLoad", SqlType.Int).Value = numProteinsForReLoad;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_GetProteinCollectionMemberCount(int proteinCollectionId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("GetProteinCollectionMemberCount", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionId;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_AddProteinReference(
            string proteinName,
            string description,
            int organismId,
            int authorityId,
            int proteinId,
            int maxProteinNameLength)
        {
            if (maxProteinNameLength <= 0)
                maxProteinNameLength = 32;
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddProteinReference", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 128).Value = proteinName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 900).Value = description;

            // TODO (org fix) Remove this reference and fix associated stored procedure
            // myParam = dbTools.AddParameter(cmdSave, "@organism_ID", SqlType.Int)
            // myParam.Direction = ParameterDirection.Input
            // myParam.Value = OrganismID

            dbTools.AddParameter(cmdSave, "@authority_ID", SqlType.Int).Value = authorityId;
            dbTools.AddParameter(cmdSave, "@protein_ID", SqlType.Int).Value = proteinId;

            var textToHash = proteinName + "_" + description + "_" + proteinId;
            dbTools.AddParameter(cmdSave, "@nameDescHash", SqlType.VarChar, 40).Value = GenerateHash(textToHash.ToLower());

            var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);
            dbTools.AddParameter(cmdSave, "@MaxProteinNameLength", SqlType.Int).Value = maxProteinNameLength;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);
            if (ret == 0)
            {
                // A zero was returned for the protein reference ID; this indicates an error
                // Raise an exception

                var msg = "AddProteinReference returned 0";

                var spMsg = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(spMsg))
                    msg += "; " + spMsg;
                throw new ConstraintException(msg);
            }

            return ret;
        }

        protected int RunSP_GetProteinCollectionID(string proteinCollectionName)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("GetProteinCollectionID", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            // Note that the @fileName parameter is actually the protein collection name; not the original .fasta file name
            dbTools.AddParameter(cmdSave, "@fileName", SqlType.VarChar, 128).Value = proteinCollectionName;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);

        }

        protected int RunSP_AddCRC32FileAuthentication(
            int proteinCollectionId,
            string authenticationHash,
            int numProteins,
            int totalResidueCount)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddCRC32FileAuthentication", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@CRC32FileHash", SqlType.VarChar, 40).Value = authenticationHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@totalResidueCount", SqlType.Int).Value = totalResidueCount;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_AddCollectionOrganismXref(
            int proteinCollectionId,
            int organismId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddCollectionOrganismXref", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_Collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@Organism_ID", SqlType.Int).Value = organismId;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_UpdateProteinNameHash(
            int referenceId,
            string proteinName,
            string description,
            int proteinId)
        {
            var tmpHash = proteinName + "_" + description + "_" + proteinId;
            var tmpGenSha = GenerateHash(tmpHash.ToLower());

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinNameHash", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Reference_ID", SqlType.Int).Value = referenceId;
            dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 40).Value = tmpGenSha;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_UpdateProteinCollectionCounts(
            int numProteins,
            int numResidues,
            int proteinCollectionId)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionCounts", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionId;
            dbTools.AddParameter(cmdSave, "@NumProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@NumResidues", SqlType.Int).Value = numResidues;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        protected int RunSP_UpdateProteinSequenceHash(
            int proteinId,
            string proteinSequence)
        {
            var tmpGenSha = GenerateHash(proteinSequence);

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinSequenceHash", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_ID", SqlType.Int).Value = proteinId;
            dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 40).Value = tmpGenSha;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        [Obsolete("Unused")]
        protected int RunSP_GetProteinIDFromName(string proteinName)
        {
            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("GetProteinIDFromName", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 128).Value = proteinName;

            // Execute the stored procedure
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            return dbTools.GetInteger(returnParam.Value);
        }

        #endregion
    }
}