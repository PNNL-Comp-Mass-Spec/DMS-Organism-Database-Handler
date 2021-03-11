using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinExport;
using OrganismDatabaseHandler.ProteinImport;
using ValidateFastaFile;

namespace OrganismDatabaseHandler.ProteinUpload
{
    public class PSUploadHandler
    {
        public enum ValidationOptionConstants : int
        {
            AllowAsterisksInResidues = 0,
            AllowDashInResidues = 1,
            AllowAllSymbolsInProteinNames = 2,
        }

        public class UploadInfo
        {
            public UploadInfo()
            {
                EncryptionPassphrase = string.Empty;
                Description = string.Empty;
                Source = string.Empty;
            }

            public UploadInfo(FileInfo inputFile, int orgId, int annotationType)
            {
                FileInformation = inputFile;
                OrganismId = orgId;
                AnnotationTypeId = annotationType;
                EncryptionPassphrase = string.Empty;
                Description = string.Empty;
                Source = string.Empty;
            }

            public FileInfo FileInformation;
            public int OrganismId;
            public string Description;
            public string Source;
            public int AnnotationTypeId;
            public int ProteinCount;
            public List<string> ErrorList;
            public int ExportedProteinCount;

            // [Obsolete("No longer supported")]
            public bool EncryptSequences;

            // [Obsolete("No longer supported")]
            public string EncryptionPassphrase;
        }

        private string mNormalizedFastaFilePath;

        private readonly DBTask mDatabaseAccessor;
        private readonly ImportHandler mImporter;
        private readonly AddUpdateEntries mUpload;
        private readonly GetFASTAFromDMS mExport;
        private readonly CustomFastaValidator mValidator;

        public event LoadStartEventHandler LoadStart;

        public delegate void LoadStartEventHandler(string taskTitle);

        public event LoadProgressEventHandler LoadProgress;

        public delegate void LoadProgressEventHandler(double fractionDone);

        public event LoadEndEventHandler LoadEnd;

        public delegate void LoadEndEventHandler();

        public event BatchProgressEventHandler BatchProgress;

        public delegate void BatchProgressEventHandler(string status);

        public event ValidationProgressEventHandler ValidationProgress;

        public delegate void ValidationProgressEventHandler(string taskTitle, double fractionDone);

        public event ValidFASTAFileLoadedEventHandler ValidFASTAFileLoaded;

        public delegate void ValidFASTAFileLoadedEventHandler(string fastaFilePath, UploadInfo uploadData);

        public event InvalidFASTAFileEventHandler InvalidFASTAFile;

        public delegate void InvalidFASTAFileEventHandler(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> errorCollection);

        public event FASTAFileWarningsEventHandler FASTAFileWarnings;

        public delegate void FASTAFileWarningsEventHandler(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> warningCollection);

        public event FASTAValidationCompleteEventHandler FASTAValidationComplete;

        public delegate void FASTAValidationCompleteEventHandler(string fastaFilePath, UploadInfo uploadInfo);

        public event WroteLineEndNormalizedFASTAEventHandler WroteLineEndNormalizedFASTA;

        public delegate void WroteLineEndNormalizedFASTAEventHandler(string newFilePath);

        private int mMaximumProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;

        private CollectionEncryptor mEncryptor;

        // Note: this array gets initialized with space for 10 items
        // If ValidationOptionConstants gets more than 10 entries, then this array will need to be expanded
        private bool[] mValidationOptions;

        public int MaximumProteinNameLength
        {
            get => mMaximumProteinNameLength;
            set
            {
                mMaximumProteinNameLength = value;
                mUpload.MaximumProteinNameLength = value;
            }
        }

        public void ResetErrorList()
        {
            mValidator.ClearErrorList();
        }

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

        private void OnBatchProgressUpdate(string status)
        {
            BatchProgress?.Invoke(status);
        }

        private void OnFileValidationComplete(string fastaFilePath, UploadInfo uploadInfo)
        {
            FASTAValidationComplete?.Invoke(fastaFilePath, uploadInfo);
        }

        private void LoadStartHandler(string taskTitle)
        {
            OnLoadStart(taskTitle);
        }

        private void LoadProgressHandler(double fractionDone)
        {
            OnProgressUpdate(fractionDone);
        }

        private void LoadEndHandler()
        {
            OnLoadEnd();
        }

        private void Task_LoadProgress(string taskDescription, float percentComplete)
        {
            ValidationProgress?.Invoke(taskDescription, percentComplete / 100f);
        }

        //private void Encryption_Progress(string taskMsg, double fractionDone)
        //{
        //    LoadStart?.Invoke();
        //}

        private void OnNormalizedFASTAGeneration(string newFastaFilePath)
        {
            mNormalizedFastaFilePath = newFastaFilePath;
            WroteLineEndNormalizedFASTA?.Invoke(newFastaFilePath);
        }

        private void OnFASTAFileWarnings(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> warningCollection)
        {
            FASTAFileWarnings?.Invoke(fastaFilePath, warningCollection);
        }

        private void OnInvalidFASTAFile(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> errorCollection)
        {
            InvalidFASTAFile?.Invoke(fastaFilePath, errorCollection);
        }

        private void OnValidFASTAFileUpload(string fastaFilePath, UploadInfo uploadData)
        {
            ValidFASTAFileLoaded?.Invoke(fastaFilePath, uploadData);
        }

        public PSUploadHandler(string psConnectionString)
        {
            mDatabaseAccessor = new DBTask(psConnectionString);

            // Reserve space for tracking up to 10 validation updates (expand later if needed)
            mValidationOptions = new bool[11];

            mUpload = new AddUpdateEntries(psConnectionString);
            mUpload.LoadStart += LoadStartHandler;
            mUpload.LoadProgress += LoadProgressHandler;
            mUpload.LoadEnd += LoadEndHandler;

            mExport = new GetFASTAFromDMS(psConnectionString,
                                           GetFASTAFromDMS.DatabaseFormatTypes.Fasta,
                                           GetFASTAFromDMS.SequenceTypes.Forward);
            mExport.FileGenerationCompleted += mExport_FileGenerationCompleted;
            mExport.FileGenerationProgress += mExport_FileGenerationProgress;

            mValidator = new CustomFastaValidator();
            mValidator.ProgressUpdate += Task_LoadProgress;
            mValidator.WroteLineEndNormalizedFASTA += OnNormalizedFASTAGeneration;

            mImporter = new ImportHandler(mDatabaseAccessor.ConnectionString);
            mImporter.LoadStart += LoadStartHandler;
            mImporter.LoadProgress += LoadProgressHandler;
            mImporter.LoadEnd += LoadEndHandler;
        }

        // fileInfoList hash -> key =
        public void BatchUpload(
            IEnumerable<UploadInfo> fileInfoList)
        {
            var databaseAccessor = new DBTask(mDatabaseAccessor.ConnectionString);

            foreach (var upInfo in fileInfoList)
            {
                // upInfo.OriginalFileInformation = upInfo.FileInformation
                var currentFile = upInfo.FileInformation;

                // Configure the validator to possibly allow asterisks in the residues
                mValidator.SetOptionSwitch(FastaValidator.SwitchOptions.AllowAllSymbolsInProteinNames, mValidationOptions[(int)ValidationOptionConstants.AllowAllSymbolsInProteinNames]);

                // Configure the validator to possibly allow asterisks in the residues
                mValidator.SetOptionSwitch(FastaValidator.SwitchOptions.AllowAsteriskInResidues, mValidationOptions[(int)ValidationOptionConstants.AllowAsterisksInResidues]);

                // Configure the validator to possibly allow dashes in the residues
                mValidator.SetOptionSwitch(FastaValidator.SwitchOptions.AllowDashInResidues, mValidationOptions[(int)ValidationOptionConstants.AllowDashInResidues]);

                // Configure the additional validation options
                mValidator.SetValidationOptions(CustomFastaValidator.ValidationOptionConstants.AllowAllSymbolsInProteinNames,
                                                 mValidationOptions[(int)ValidationOptionConstants.AllowAllSymbolsInProteinNames]);

                mValidator.SetValidationOptions(CustomFastaValidator.ValidationOptionConstants.AllowAsterisksInResidues,
                                                 mValidationOptions[(int)ValidationOptionConstants.AllowAsterisksInResidues]);

                mValidator.SetValidationOptions(CustomFastaValidator.ValidationOptionConstants.AllowDashInResidues,
                                                 mValidationOptions[(int)ValidationOptionConstants.AllowDashInResidues]);

                // Update the default rules (important if AllowAsteriskInResidues = True or AllowDashInResidues = True)
                mValidator.SetDefaultRules();

                // Update the maximum protein name length
                if (mMaximumProteinNameLength <= 0)
                {
                    mMaximumProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
                }

                mValidator.MaximumProteinNameLength = mMaximumProteinNameLength;

                OnLoadStart("Validating fasta files");

                // Validate the fasta file (send full path)
                // This function returns True if the file is successfully processed (even if it has errors)
                var fileValidated = mValidator.StartValidateFASTAFile(currentFile.FullName);

                OnLoadEnd();

                if (!fileValidated)
                {
                    Console.WriteLine("--------------------------------------------------------------");
                    Console.WriteLine("Warning: Skipping protein collection due to validation error");
                    Console.WriteLine(currentFile.FullName);
                    Console.WriteLine("--------------------------------------------------------------");
                    continue;
                }

                if (mValidator.FASTAFileHasWarnings(currentFile.Name))
                {
                    // If any warnings were cached, return them with the OnFASTAFileWarnings event
                    OnFASTAFileWarnings(currentFile.FullName, mValidator.RecordedFASTAFileWarnings(currentFile.Name));
                }

                // Now check whether or not any errors were found for the file
                if (!mValidator.FASTAFileValid(currentFile.Name))
                {
                    // Errors were found; return the error collection with the InvalidFASTAFile event
                    OnInvalidFASTAFile(currentFile.FullName, mValidator.RecordedFASTAFileErrors(currentFile.Name));

                    Console.WriteLine("--------------------------------------------------------------");
                    Console.WriteLine("Warning: Skipping protein collection because validation failed");
                    Console.WriteLine(currentFile.FullName);
                    Console.WriteLine("--------------------------------------------------------------");
                    continue;
                }

                OnBatchProgressUpdate("Loading: " + currentFile.Name);
                if (mNormalizedFastaFilePath != null)
                {
                    if ((currentFile.FullName ?? "") != (mNormalizedFastaFilePath ?? ""))
                    {
                        upInfo.FileInformation = new FileInfo(mNormalizedFastaFilePath);
                    }
                }

                var proteinCollectionName = Path.GetFileNameWithoutExtension(currentFile.Name);

                var existingCollectionId = mUpload.GetProteinCollectionId(proteinCollectionName);
                DialogResult eResult;
                if (existingCollectionId > 0)
                {
                    var collectionState = mUpload.GetProteinCollectionState(existingCollectionId);

                    string logMessageIfCancelled;
                    string logLabelIfCancelled;

                    if (collectionState == "New" | collectionState == "Provisional")
                    {
                        var warningMessage = "The Collection '" + proteinCollectionName + "' has been declared '" +
                                             collectionState + "'. Are you sure you want to replace its contents?";

                        logMessageIfCancelled = "Collection was in State '" + collectionState + "' and was not changed";
                        logLabelIfCancelled = "Warning";

                        eResult = MessageBox.Show(
                            warningMessage,
                            "Confirm Replacement",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Stop,
                            MessageBoxDefaultButton.Button2);
                    }
                    else
                    {
                        logMessageIfCancelled = "Collections in State '" + collectionState + "' cannot be changed or deleted";
                        logLabelIfCancelled = "Error";
                        eResult = DialogResult.No;
                    }

                    if (eResult == DialogResult.No)
                    {
                        var errorCollection = new List<CustomFastaValidator.ErrorInfoExtended>()
                        {
                            new CustomFastaValidator.ErrorInfoExtended(
                                0, " N/A ", logMessageIfCancelled, "", logLabelIfCancelled)
                        };
                        OnInvalidFASTAFile(currentFile.FullName, errorCollection);
                    }
                }
                else
                {
                    eResult = DialogResult.Yes;
                }

                if (eResult == DialogResult.Yes)
                {
                    var proteinStorage = mImporter.LoadProteinsForBatch(upInfo.FileInformation.FullName);
                    if (proteinStorage != null)
                    {
                        if (proteinStorage.ProteinCount == 0)
                        {
                            // No proteins

                            var errorCollection = new List<CustomFastaValidator.ErrorInfoExtended>()
                            {
                                new CustomFastaValidator.ErrorInfoExtended(
                                    0, " N/A ", "No valid proteins were loaded from the .Fasta file", "", "Error")
                            };

                            OnInvalidFASTAFile(upInfo.FileInformation.FullName, errorCollection);
                        }
                        else
                        {
                            if (upInfo.EncryptSequences && !string.IsNullOrEmpty(upInfo.EncryptionPassphrase))
                            {
                                mEncryptor = new CollectionEncryptor(upInfo.EncryptionPassphrase, databaseAccessor);
                                mEncryptor.EncryptStorageCollectionSequences(proteinStorage);
                                proteinStorage.EncryptSequences = true;
                                proteinStorage.PassPhrase = upInfo.EncryptionPassphrase;
                            }

                            upInfo.ProteinCount = proteinStorage.ProteinCount;
                            CollectionBatchUploadCoordinator(proteinStorage, currentFile.FullName, upInfo.OrganismId, upInfo.AnnotationTypeId, upInfo.Description, upInfo.Source);
                            // upInfo.ExportedProteinCount = mExport.ExportedProteinCount;
                            OnValidFASTAFileUpload(upInfo.FileInformation.FullName, upInfo);
                            proteinStorage.ClearProteinEntries();
                        }
                    }
                    else
                    {
                        OnInvalidFASTAFile(upInfo.FileInformation.FullName, mValidator.RecordedFASTAFileErrors(currentFile.FullName));
                    }
                }
            }

            mImporter.TriggerProteinCollectionTableUpdate();
        }

        public int UploadCollection(
            ProteinStorage.ProteinStorage fileContents,
            List<string> selectedProteins,
            string filepath,
            string description,
            string collectionSource,
            AddUpdateEntries.CollectionTypes collectionType,
            int organismId,
            int annotationTypeId)
        {
            // task 2a - Get Protein_Collection_ID or make a new one

            var proteinCollectionName = Path.GetFileNameWithoutExtension(filepath);
            var existingCollectionId = mUpload.GetProteinCollectionId(proteinCollectionName);

            var collectionState = mUpload.GetProteinCollectionState(existingCollectionId);

            if (collectionState != "Unknown" &&
                collectionState != "New" &&
                collectionState != "Provisional")
            {
                throw new Exception("Protein collections in state " + collectionState + " cannot be updated");
            }

            var numProteins = selectedProteins.Count;
            var numResidues = mUpload.GetTotalResidueCount(fileContents, selectedProteins);

            int collectionId;

            if (existingCollectionId <= 0)
            {
                // Note that we're storing 0 for NumResidues at this time
                // That value will be updated later after all of the proteins have been added
                var newCollectionId = mUpload.MakeNewProteinCollection(
                    proteinCollectionName, description,
                    collectionSource, collectionType,
                    annotationTypeId, numProteins, 0);

                if (newCollectionId <= 0)
                {
                    // Error making the new protein collection
                    MessageBox.Show(string.Format(
                        "MakeNewProteinCollection was unable to create a new protein collection named {0}; the Collection ID returned was {1}",
                        proteinCollectionName, newCollectionId));
                    return -1;
                }

                collectionId = newCollectionId;
            }
            else
            {
                // Make sure there are no proteins defined for this protein collection
                // In addition, this will update NumResidues to be 0
                mUpload.DeleteProteinCollectionMembers(existingCollectionId, numProteins);
                collectionId = existingCollectionId;
            }

            // task 2b - Compare file to existing sequences and upload new sequences to T_Proteins
            mUpload.CompareProteinId(fileContents, selectedProteins);

            // task 3 - Add Protein References to T_Protein_Names
            mUpload.UpdateProteinNames(fileContents, selectedProteins, organismId, annotationTypeId);

            // task 4 - Add new collection members to T_Protein_Collection_Members
            mUpload.UpdateProteinCollectionMembers(collectionId, fileContents, selectedProteins, numProteins, numResidues);

            OnLoadStart("Associating protein collection with organism using T_Collection_OrganismXref");
            var xrefId = mUpload.AddCollectionOrganismXref(collectionId, organismId);
            OnLoadEnd();

            if (xrefId < 1)
            {
                // Throw New Exception("Could not add Collection/Organism Xref")
                MessageBox.Show("Could not add Collection/Organism Xref; mUpload.AddCollectionOrganismXref returned " + xrefId);
            }

            // task 5 - Update encryption metadata (if applicable)
            if (fileContents.EncryptSequences)
            {
                OnLoadStart("Storing encryption metadata");
                mUpload.UpdateEncryptionMetadata(collectionId, fileContents.PassPhrase);
                OnLoadEnd();
            }

            var tmpFileName = Path.GetTempPath();

            // Dim tmpFi As System.IO.FileInfo = New System.IO.FileInfo(tmpFileName)

            OnLoadStart("Generating Hash fingerprint");
            var fingerprint = mExport.ExportFASTAFile(collectionId, tmpFileName, GetFASTAFromDMS.DatabaseFormatTypes.Fasta, GetFASTAFromDMS.SequenceTypes.Forward);
            OnLoadEnd();

            OnLoadStart("Storing fingerprint in T_Protein_Collections");
            mUpload.AddAuthenticationHash(collectionId, fingerprint, numProteins, numResidues);
            OnLoadEnd();

            // TODO add in hash return
            return 0;
        }

        protected int CollectionBatchUploadCoordinator(
            ProteinStorage.ProteinStorage fileContents,
            string filepath,
            int organismId,
            int annotationTypeId,
            string description,
            string source)
        {
            var selectedList = new List<string>();

            var counter = fileContents.GetEnumerator();

            while (counter.MoveNext())
                selectedList.Add(counter.Current.Value.Reference);

            selectedList.Sort();

            return UploadCollection(
                fileContents, selectedList, filepath, description, source,
                AddUpdateEntries.CollectionTypes.ProtOriginalSource, organismId, annotationTypeId);
        }

        public void SetValidationOptions(ValidationOptionConstants eValidationOptionName, bool blnEnabled)
        {
            mValidationOptions[(int)eValidationOptionName] = blnEnabled;
        }

        private void mExport_FileGenerationCompleted(string fullOutputPath)
        {
        }

        private void mExport_FileGenerationProgress(string statusMsg, double fractionDone)
        {
            OnProgressUpdate(fractionDone);
        }
    }
}