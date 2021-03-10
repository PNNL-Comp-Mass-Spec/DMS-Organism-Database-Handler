using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualBasic.CompilerServices;
using PRISMDatabaseUtils;
using Protein_Exporter;

namespace Protein_Importer
{
    public class AddUpdateEntries
    {
        public enum SPModes
        {
            add,
            // Unused: update,
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
            prot_original_source = 1,
            // Unused: modified_source = 2,
            // Unused: runtime_combined_collection = 3,
            // Unused: loadtime_combined_collection = 4,
            // Unused: nuc_original_source = 5,
        }

        protected readonly TableManipulationBase.DBTask m_DatabaseAccessor;
        // Unused: protected int m_OrganismID;
        // Unused: protected HashTable m_ProteinLengths;
        protected int m_MaxProteinNameLength;

        protected System.Security.Cryptography.SHA1Managed m_Hasher;
        // Unused: protected Threading.Thread ProteinHashThread;
        // Unused: protected Threading.Thread ReferenceHashThread;

        #region "Properties"
        public int MaximumProteinNameLength
        {
            get
            {
                return m_MaxProteinNameLength;
            }

            set
            {
                m_MaxProteinNameLength = value;
            }
        }
        #endregion

        #region "Events"
        public event LoadStartEventHandler LoadStart;

        public delegate void LoadStartEventHandler(string taskTitle);

        public event LoadEndEventHandler LoadEnd;

        public delegate void LoadEndEventHandler();

        public event LoadProgressEventHandler LoadProgress;

        public delegate void LoadProgressEventHandler(double fractionDone);

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

        public AddUpdateEntries(string PISConnectionString)
        {
            m_DatabaseAccessor = new TableManipulationBase.DBTask(PISConnectionString);
            m_Hasher = new System.Security.Cryptography.SHA1Managed();
        }

        [Obsolete("No longer used")]
        public void CloseConnection()
        {
        }

        /// <summary>
        /// Checks for the existence of protein sequences in the T_Proteins table
        /// Gets Protein_ID if located, makes a new entry if not
        /// Updates Protein_ID field in ProteinStorageEntry instance
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="selectedProteinList"></param>
        /// <remarks></remarks>
        public void CompareProteinID(
            Protein_Storage.ProteinStorage pc,
            List<string> selectedProteinList)
        {
            Protein_Storage.ProteinStorageEntry tmpPC;

            OnLoadStart("Comparing to existing sequences and adding new proteins");
            int counterMax = selectedProteinList.Count;
            var counter = default(int);

            int EventTriggerThresh;
            if (counterMax <= 100)
            {
                EventTriggerThresh = 1;
            }
            else
            {
                EventTriggerThresh = (int)Math.Round(counterMax / 100d);
                if (EventTriggerThresh > 100)
                    EventTriggerThresh = 100;
            }

            foreach (var s in selectedProteinList)
            {
                tmpPC = pc.GetProtein(s);

                counter += 1;
                if (counter % EventTriggerThresh == 0)
                {
                    OnProgressUpdate(counter / (double)counterMax);
                }

                tmpPC.Protein_ID = AddProteinSequence(tmpPC);
            }

            OnLoadEnd();
        }

        public void UpdateProteinNames(
            Protein_Storage.ProteinStorage pc,
            List<string> selectedProteinList,
            int organismID,
            int authorityID)
        {
            OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection");
            Protein_Storage.ProteinStorageEntry tmpPC;
            var counter = default(int);
            int counterMax = selectedProteinList.Count;

            int EventTriggerThresh;
            if (counterMax <= 100)
            {
                EventTriggerThresh = 1;
            }
            else
            {
                EventTriggerThresh = (int)Math.Round(counterMax / 100d);
                if (EventTriggerThresh > 100)
                    EventTriggerThresh = 100;
            }

            foreach (var s in selectedProteinList)
            {
                tmpPC = pc.GetProtein(s);
                counter += 1;
                if (counter % EventTriggerThresh == 0)
                {
                    OnProgressUpdate(counter / (double)counterMax);
                }

                tmpPC.Reference_ID = AddProteinReference(tmpPC.Reference, tmpPC.Description, organismID, authorityID, tmpPC.Protein_ID, m_MaxProteinNameLength);
            }

            OnLoadEnd();
        }

        public void UpdateProteinCollectionMembers(
            int ProteinCollectionID,
            Protein_Storage.ProteinStorage pc,
            List<string> selectedProteinList,
            int numProteinsExpected,
            int numResiduesExpected)
        {
            int counterMax = selectedProteinList.Count;

            int EventTriggerThresh;
            if (counterMax <= 100)
            {
                EventTriggerThresh = 1;
            }
            else
            {
                EventTriggerThresh = (int)Math.Round(counterMax / 100d);
                if (EventTriggerThresh > 100)
                    EventTriggerThresh = 100;
            }

            OnLoadStart("Storing Protein Collection Members");

            var numProteinsActual = default(int);
            var numResiduesActual = default(int);

            foreach (var s in selectedProteinList)
            {
                var tmpPC = pc.GetProtein(s);
                numProteinsActual += 1;
                if (numProteinsActual % EventTriggerThresh == 0)
                {
                    OnProgressUpdate(numProteinsActual / (double)counterMax);
                }

                numResiduesActual += tmpPC.Length;

                tmpPC.Member_ID = AddProteinCollectionMember(tmpPC.Reference_ID, tmpPC.Protein_ID, tmpPC.SortingIndex, ProteinCollectionID);
            }

            RunSP_UpdateProteinCollectionCounts(numProteinsActual, numResiduesActual, ProteinCollectionID);

            OnLoadEnd();
        }

        public int GetTotalResidueCount(
            Protein_Storage.ProteinStorage proteinCollection,
            List<string> selectedProteinList)
        {
            var totalLength = default(int);
            Protein_Storage.ProteinStorageEntry tmpPC;

            foreach (var s in selectedProteinList)
            {
                tmpPC = proteinCollection.GetProtein(s);
                totalLength += tmpPC.Sequence.Length;
            }

            return totalLength;
        }

        protected int AddProteinSequence(Protein_Storage.ProteinStorageEntry protein)
        {
            int protein_id;
            protein_id = RunSP_AddProteinSequence(
                protein.Sequence,
                protein.Length,
                protein.MolecularFormula,
                protein.MonoisotopicMass,
                protein.AverageMass,
                protein.SHA1Hash,
                protein.IsEncrypted,
                SPModes.add);

            return protein_id;
        }

        public int UpdateProteinSequenceInfo(
            int proteinID,
            string sequence,
            int length,
            string molecularFormula,
            double monoisotopicMass,
            double averageMass,
            string sha1Hash)
        {
            RunSP_UpdateProteinSequenceInfo(
                proteinID,
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
            int tmpAuthID;

            tmpAuthID = RunSP_AddNamingAuthority(
                shortName,
                fullName,
                webAddress);

            return tmpAuthID;
        }

        public int AddAnnotationType(
            string typeName,
            string description,
            string example,
            int authorityID)
        {
            int tmpAnnTypeID;

            tmpAnnTypeID = RunSP_AddAnnotationType(
                typeName, description, example, authorityID);

            return tmpAnnTypeID;
        }

        public int MakeNewProteinCollection(
            string proteinCollectionName,
            string description,
            string collectionSource,
            CollectionTypes collectionType,
            int annotationTypeID,
            int numProteins,
            int numResidues)
        {
            int tmpProteinCollectionID;

            tmpProteinCollectionID = RunSP_AddUpdateProteinCollection(
                proteinCollectionName, description, collectionSource, collectionType, CollectionStates.NewEntry,
                annotationTypeID, numProteins, numResidues, SPModes.add);

            return tmpProteinCollectionID;
        }

        public int UpdateEncryptionMetadata(
            int proteinCollectionID,
            string passphrase)
        {
            return RunSP_AddUpdateEncryptionMetadata(passphrase, proteinCollectionID);
        }

        public int UpdateProteinCollectionState(
            int proteinCollectionID,
            int collectionStateID)
        {
            return RunSP_UpdateProteinCollectionStates(proteinCollectionID, collectionStateID);
        }

        public string GetProteinCollectionState(int proteinCollectionID)
        {
            return RunSP_GetProteinCollectionState(proteinCollectionID);
        }

        protected int GetProteinID(Protein_Storage.ProteinStorageEntry entry, DataTable hitsTable)
        {
            DataRow[] foundRows;
            string tmpSeq;
            var tmpProteinID = default(int);

            foundRows = hitsTable.Select("[SHA1_Hash] = '" + entry.SHA1Hash + "'");
            if (foundRows.Length > 0)
            {
                foreach (DataRow testRow in foundRows)
                {
                    tmpSeq = Conversions.ToString(testRow["Sequence"]);
                    if (tmpSeq.Equals(entry.Sequence))
                    {
                        tmpProteinID = Conversions.ToInteger(testRow["Protein_ID"]);
                    }
                }
            }
            else
            {
                tmpProteinID = 0;
            }

            return tmpProteinID;
        }

        public int GetProteinIDFromName(string proteinName)
        {
            return RunSP_GetProteinIDFromName(proteinName);
        }

        /// <summary>
        /// Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
        /// </summary>
        /// <param name="ProteinCollectionID"></param>
        /// <remarks></remarks>
        public void DeleteProteinCollectionMembers(int proteinCollectionID, int numProteins)
        {
            RunSP_DeleteProteinCollectionMembers(proteinCollectionID, numProteins);
        }

        public int GetProteinCollectionID(string proteinCollectionName)
        {
            return RunSP_GetProteinCollectionID(proteinCollectionName);
        }

        public int CountProteinCollectionMembers(int proteinCollectionID)
        {
            return RunSP_GetProteinCollectionMemberCount(proteinCollectionID);
        }

        public int AddCollectionOrganismXref(int proteinCollectionID, int OrganismID)
        {
            return RunSP_AddCollectionOrganismXref(proteinCollectionID, OrganismID);
        }

        public int AddProteinCollectionMember(
            int referenceID,
            int proteinID,
            int sorting_Index,
            int proteinCollectionID)
        {
            return RunSP_AddProteinCollectionMember(referenceID, proteinID, sorting_Index, proteinCollectionID);
        }

        public int UpdateProteinCollectionMember(
            int referenceID,
            int proteinID,
            int sorting_Index,
            int proteinCollectionID)
        {
            return RunSP_UpdateProteinCollectionMember(referenceID, proteinID, sorting_Index, proteinCollectionID);
        }

        public int AddProteinReference(
            string proteinName,
            string description,
            int organismID,
            int authorityID,
            int proteinID,
            int maxProteinNameLength)
        {
            int ref_ID;

            ref_ID = RunSP_AddProteinReference(proteinName, description, organismID, authorityID, proteinID, maxProteinNameLength);
            return ref_ID;
        }

        public int AddAuthenticationHash(
            int proteinCollectionID,
            string authenticationHash,
            int numProteins,
            int totalResidues)
        {
            return RunSP_AddCRC32FileAuthentication(proteinCollectionID, authenticationHash, numProteins, totalResidues);
        }

        public int UpdateProteinNameHash(
            int referenceID,
            string proteinName,
            string description,
            int proteinID)
        {
            return RunSP_UpdateProteinNameHash(referenceID, proteinName, description, proteinID);
        }

        public int UpdateProteinSequenceHash(
            int proteinID,
            string proteinSequence)
        {
            return RunSP_UpdateProteinSequenceHash(proteinID, proteinSequence);
        }

        public string GenerateHash(string sourceText)
        {
            // Create an encoding object to ensure the encoding standard for the source text
            var encoding = new System.Text.ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var byteSourceText = encoding.GetBytes(sourceText);

            // Compute the hash value from the source
            var sha1_hash = m_Hasher.ComputeHash(byteSourceText);

            // And convert it to String format for return
            string sha1string = RijndaelEncryptionHandler.ToHexString(sha1_hash);

            return sha1string;
        }


        #region "Stored Procedure Access"

        protected string RunSP_GetProteinCollectionState(
            int proteinCollectionID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("GetProteinCollectionState", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID;

            var stateNameParam = dbTools.AddParameter(cmdSave, "@State_Name", SqlType.VarChar, 32, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            string stateName = dbTools.GetString(stateNameParam.Value);

            return stateName;
        }

        protected int RunSP_AddProteinSequence(
            string sequence,
            int length,
            string molecularFormula,
            double monoisotopicMass,
            double averageMass,
            string sha1_Hash,
            bool isEncrypted,
            SPModes mode)
        {
            int encryptionFlag = 0;
            if (isEncrypted)
            {
                encryptionFlag = 1;
            }

            var dbTools = m_DatabaseAccessor.DBTools;

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
            dbTools.AddParameter(cmdSave, "@sha1_hash", SqlType.VarChar, 40).Value = sha1_Hash;
            dbTools.AddParameter(cmdSave, "@is_encrypted", SqlType.TinyInt).Value = encryptionFlag;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString();
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_UpdateProteinSequenceInfo(
            int proteinID,
            string sequence,
            int length,
            string molecularFormula,
            double monoisotopicMass,
            double averageMass,
            string sha1_Hash)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinSequenceInfo", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_ID", SqlType.Int).Value = proteinID;
            dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence;
            dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length;
            dbTools.AddParameter(cmdSave, "@molecular_formula", SqlType.VarChar, 128).Value = molecularFormula;
            dbTools.AddParameter(cmdSave, "@monoisotopic_mass", SqlType.Float, 8).Value = monoisotopicMass;
            dbTools.AddParameter(cmdSave, "@average_mass", SqlType.Float, 8).Value = averageMass;
            dbTools.AddParameter(cmdSave, "@sha1_hash", SqlType.VarChar, 40).Value = sha1_Hash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddUpdateProteinCollection(
            string proteinCollectionName,
            string description,
            string collectionSource,
            CollectionTypes collectionType,
            CollectionStates collectionState,
            int annotationTypeID,
            int numProteins,
            int numResidues,
            SPModes mode)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

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
            dbTools.AddParameter(cmdSave, "@primary_annotation_type_id", SqlType.Int).Value = annotationTypeID;
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@numResidues", SqlType.Int).Value = numResidues;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString();
            var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);
            if (ret == 0)
            {
                // A zero was returned for the protein collection ID; this indicates and error
                // Raise an exception

                string msg = "AddUpdateProteinCollection returned 0 for the Protein Collection ID";

                string spMsg = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(spMsg))
                    msg += "; " + spMsg;
                throw new ConstraintException(msg);
            }

            return ret;
        }

        protected int RunSP_AddProteinCollectionMember(
            int reference_ID, int Protein_ID,
            int sortingIndex, int Protein_Collection_ID)
        {
            return RunSP_AddUpdateProteinCollectionMember(reference_ID, Protein_ID, sortingIndex, Protein_Collection_ID, "Add");
        }

        protected int RunSP_UpdateProteinCollectionMember(
            int reference_ID, int Protein_ID,
            int sortingIndex, int Protein_Collection_ID)
        {
            return RunSP_AddUpdateProteinCollectionMember(reference_ID, Protein_ID, sortingIndex, Protein_Collection_ID, "Update");
        }

        protected int RunSP_AddUpdateProteinCollectionMember(
            int reference_ID, int Protein_ID,
            int sortingIndex, int Protein_Collection_ID,
            string mode)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddUpdateProteinCollectionMember_New", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@reference_ID", SqlType.Int).Value = reference_ID;
            dbTools.AddParameter(cmdSave, "@protein_ID", SqlType.Int).Value = Protein_ID;
            dbTools.AddParameter(cmdSave, "@sorting_index", SqlType.Int).Value = sortingIndex;
            dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = Protein_Collection_ID;
            dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 10).Value = mode;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddUpdateEncryptionMetadata(
            string passphrase, int protein_Collection_ID)
        {
            string phraseHash = GenerateHash(passphrase);

            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddUpdateEncryptionMetadata", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_Collection_ID", SqlType.Int).Value = protein_Collection_ID;
            dbTools.AddParameter(cmdSave, "@Encryption_Passphrase", SqlType.VarChar, 64).Value = passphrase;
            dbTools.AddParameter(cmdSave, "@Passphrase_SHA1_Hash", SqlType.VarChar, 40).Value = phraseHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddNamingAuthority(
            string shortName,
            string fullName,
            string webAddress)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddNamingAuthority", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = shortName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = fullName;
            dbTools.AddParameter(cmdSave, "@web_address", SqlType.VarChar, 128).Value = webAddress;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddAnnotationType(
            string typeName,
            string description,
            string example,
            int authorityID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddAnnotationType", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = typeName;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = description;
            dbTools.AddParameter(cmdSave, "@example", SqlType.VarChar, 128).Value = example;
            dbTools.AddParameter(cmdSave, "@authID", SqlType.Int).Value = authorityID;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_UpdateProteinCollectionStates(
            int proteinCollectionID,
            int collectionStateID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionState", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionID;
            dbTools.AddParameter(cmdSave, "@state_ID", SqlType.Int).Value = collectionStateID;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        /// <summary>
        /// Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
        /// </summary>
        /// <param name="proteinCollectionID"></param>
        /// <param name="numProteinsForReLoad">The number of proteins that will be uploaded after this delete</param>
        /// <remarks>NumResidues in T_Protein_Collections is set to 0</remarks>
        protected int RunSP_DeleteProteinCollectionMembers(int proteinCollectionID, int numProteinsForReLoad)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("DeleteProteinCollectionMembers", CommandType.StoredProcedure);

            // Use a 10 minute timeout
            cmdSave.CommandTimeout = 600;

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID;
            dbTools.AddParameter(cmdSave, "@NumProteinsForReLoad", SqlType.Int).Value = numProteinsForReLoad;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_GetProteinCollectionMemberCount(int proteinCollectionID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("GetProteinCollectionMemberCount", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddProteinReference(
            string protein_Name,
            string description,
            int organismID,
            int authorityID,
            int proteinID,
            int maxProteinNameLength)
        {
            if (maxProteinNameLength <= 0)
                maxProteinNameLength = 32;
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddProteinReference", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 128).Value = protein_Name;
            dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 900).Value = description;

            // TODO (org fix) Remove this reference and fix associated stored procedure
            // myParam = dbTools.AddParameter(cmdSave, "@organism_ID", SqlType.Int)
            // myParam.Direction = ParameterDirection.Input
            // myParam.Value = OrganismID

            dbTools.AddParameter(cmdSave, "@authority_ID", SqlType.Int).Value = authorityID;
            dbTools.AddParameter(cmdSave, "@protein_ID", SqlType.Int).Value = proteinID;

            string textToHash = protein_Name + "_" + description + "_" + proteinID.ToString();
            dbTools.AddParameter(cmdSave, "@nameDescHash", SqlType.VarChar, 40).Value = GenerateHash(textToHash.ToLower());

            var messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);
            dbTools.AddParameter(cmdSave, "@MaxProteinNameLength", SqlType.Int).Value = maxProteinNameLength;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);
            if (ret == 0)
            {
                // A zero was returned for the protein reference ID; this indicates an error
                // Raise an exception

                string msg = "AddProteinReference returned 0";

                string spMsg = dbTools.GetString(messageParam.Value);

                if (!string.IsNullOrEmpty(spMsg))
                    msg += "; " + spMsg;
                throw new ConstraintException(msg);
            }

            return ret;
        }

        protected int RunSP_GetProteinCollectionID(string proteinCollectionName)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("GetProteinCollectionID", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            // Note that the @fileName parameter is actually the protein collection name; not the original .fasta file name
            dbTools.AddParameter(cmdSave, "@fileName", SqlType.VarChar, 128).Value = proteinCollectionName;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddCRC32FileAuthentication(
            int protein_Collection_ID,
            string authenticationHash,
            int numProteins,
            int totalResidueCount)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddCRC32FileAuthentication", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = protein_Collection_ID;
            dbTools.AddParameter(cmdSave, "@CRC32FileHash", SqlType.VarChar, 40).Value = authenticationHash;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);
            dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@totalResidueCount", SqlType.Int).Value = totalResidueCount;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_AddCollectionOrganismXref(
            int protein_Collection_ID,
            int organismID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("AddCollectionOrganismXref", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_Collection_ID", SqlType.Int).Value = protein_Collection_ID;
            dbTools.AddParameter(cmdSave, "@Organism_ID", SqlType.Int).Value = organismID;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_UpdateProteinNameHash(
            int reference_ID,
            string protein_Name,
            string description,
            int protein_ID)
        {
            string tmpHash;

            tmpHash = protein_Name + "_" + description + "_" + protein_ID.ToString();
            string tmpGenSHA = GenerateHash(tmpHash.ToLower());

            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinNameHash", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Reference_ID", SqlType.Int).Value = reference_ID;
            dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 40).Value = tmpGenSHA;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_UpdateProteinCollectionCounts(
            int numProteins,
            int numResidues,
            int proteinCollectionID)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinCollectionCounts", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID;
            dbTools.AddParameter(cmdSave, "@NumProteins", SqlType.Int).Value = numProteins;
            dbTools.AddParameter(cmdSave, "@NumResidues", SqlType.Int).Value = numResidues;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_UpdateProteinSequenceHash(
            int proteinID,
            string proteinSequence)
        {
            string tmpGenSHA = GenerateHash(proteinSequence);

            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("UpdateProteinSequenceHash", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@Protein_ID", SqlType.Int).Value = proteinID;
            dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 40).Value = tmpGenSHA;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output);

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        protected int RunSP_GetProteinIDFromName(string proteinName)
        {
            var dbTools = m_DatabaseAccessor.DBTools;

            var cmdSave = dbTools.CreateCommand("GetProteinIDFromName", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 128).Value = proteinName;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            int ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        #endregion
    }
}