using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OrganismDatabaseHandler.DatabaseTools;
using PRISM;
using PRISMDatabaseUtils;
using PRISMWin;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMS : EventNotifier
    {
        // Ignore Spelling: Lockfile, fastapro, filetype, Desc, Ensembl

        public enum SequenceTypes
        {
            Forward = 1,
            Reversed = 2,
            Scrambled = 3,
            Decoy = 4,
            DecoyX = 5,
        }

        [Obsolete("Deprecated: only .fasta files are supported")]
        public enum DatabaseFormatTypes
        {
            Fasta,
            FastaPro,
        }

        public const string LockFileProgressText = "Lockfile";
        public const string HashcheckSuffix = ".hashcheck";

        private GetFASTAFromDMSForward mGetter;
        private ArchiveOutputFilesBase mArchiver;
        private SequenceTypes mOutputSequenceType;
        private ArchiveOutputFilesBase.CollectionTypes mCollectionType;
        private string mFinalOutputPath;

        private readonly DBTask mDatabaseAccessor;
        private readonly SHA1Managed sha1Provider;
        private readonly FileTools mFileTools;

        private DateTime mLastLockQueueWaitTimeLog;

        public bool DecoyProteinsUseXXX { get; set; } = true;

        public GetFASTAFromDMSForward ExporterComponent => mGetter;

        // Unused
        // public bool WaitingForLockFile => mWaitingForLockFile;

        // ReSharper disable once UnusedMember.Global
        /// <summary>
        /// Constructor when running in offline mode
        /// </summary>
        /// <remarks>Useful if only calling ValidateMatchingHash</remarks>
        public GetFASTAFromDMS()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Constructor that takes connection string, assumes file format FASTA and forward-only sequences
        /// </summary>
        /// <param name="dbConnectionString">Protein sequences database connection string</param>
        public GetFASTAFromDMS(string dbConnectionString)
            : this(dbConnectionString, SequenceTypes.Forward)
        {
        }

        /// <summary>
        /// Constructor that takes connection string, database format type, and output sequence type
        /// </summary>
        /// <param name="dbConnectionString">Database connection string; empty string if offline and only planning to use ValidateMatchingHash</param>
        /// <param name="databaseFormatType"></param>
        /// <param name="outputSequenceType"></param>
        /// <param name="decoyUsesXXX">When true, decoy proteins start with XXX_ instead of Reversed_</param>
        [Obsolete("Use the constructor that does not have databaseFormatType")]
        public GetFASTAFromDMS(string dbConnectionString, DatabaseFormatTypes databaseFormatType, SequenceTypes outputSequenceType, bool decoyUsesXXX = true)
             : this(dbConnectionString, outputSequenceType, decoyUsesXXX)
        {
        }

        /// <summary>
        /// Constructor that takes connection string, database format type, and output sequence type
        /// </summary>
        /// <param name="dbConnectionString">Database connection string; empty string if offline and only planning to use ValidateMatchingHash</param>
        /// <param name="outputSequenceType"></param>
        /// <param name="decoyUsesXXX">When true, decoy proteins start with XXX_ instead of Reversed_</param>
        public GetFASTAFromDMS(string dbConnectionString, SequenceTypes outputSequenceType, bool decoyUsesXXX = true)
        {
            sha1Provider = new SHA1Managed();

            if (string.IsNullOrWhiteSpace(dbConnectionString))
            {
                mDatabaseAccessor = null;
            }
            else
            {
                mDatabaseAccessor = new DBTask(dbConnectionString);
                RegisterEvents(mDatabaseAccessor);
            }

            ClassSelector(outputSequenceType, decoyUsesXXX);

            mFileTools = new FileTools();
            mFileTools.WaitingForLockQueue += FileTools_WaitingForLockQueue;
            RegisterEvents(mFileTools);
        }

        private void ClassSelector(SequenceTypes outputSequenceType, bool decoyUsesXXX)
        {
            if (mGetter != null)
            {
                mGetter.FileGenerationCompleted -= OnFileGenerationCompleted;
                mGetter.FileGenerationStarted -= OnFileGenerationStarted;
                mGetter.FileGenerationProgress -= OnFileGenerationProgressUpdate;
            }

            mOutputSequenceType = outputSequenceType;

            switch (outputSequenceType)
            {
                case SequenceTypes.Forward:
                    mGetter = new GetFASTAFromDMSForward(mDatabaseAccessor);
                    mCollectionType = ArchiveOutputFilesBase.CollectionTypes.Static;
                    break;

                case SequenceTypes.Reversed:
                    mGetter = new GetFASTAFromDMSReversed(mDatabaseAccessor);
                    mCollectionType = ArchiveOutputFilesBase.CollectionTypes.Dynamic;
                    break;

                case SequenceTypes.Scrambled:
                    mGetter = new GetFASTAFromDMSScrambled(mDatabaseAccessor);
                    mCollectionType = ArchiveOutputFilesBase.CollectionTypes.Dynamic;
                    break;

                case SequenceTypes.Decoy:
                    mGetter = new GetFASTAFromDMSDecoy(mDatabaseAccessor, decoyUsesXXX);
                    mCollectionType = ArchiveOutputFilesBase.CollectionTypes.Dynamic;
                    break;

                case SequenceTypes.DecoyX:
                    mGetter = new GetFASTAFromDMSDecoyX(mDatabaseAccessor);
                    mCollectionType = ArchiveOutputFilesBase.CollectionTypes.Dynamic;
                    break;
            }

            if (mGetter != null)
            {
                mGetter.FileGenerationCompleted += OnFileGenerationCompleted;
                mGetter.FileGenerationStarted += OnFileGenerationStarted;
                mGetter.FileGenerationProgress += OnFileGenerationProgressUpdate;
            }

            mArchiver = new ArchiveToFile(mDatabaseAccessor, this);
        }

        // Unused
        // protected virtual DataTable GetCollectionTable(string selectionSQL)
        // {
        //     if (mDatabaseAccessor == null)
        //         mDatabaseAccessor = new DBTask(mPSConnectionString, true);

        //     return mDatabaseAccessor.GetTable(selectionSQL);
        // }

        /// <summary>
        /// Create the FASTA file for the given protein collection ID
        /// </summary>
        /// <param name="proteinCollectionId">Protein collection ID</param>
        /// <param name="destinationFolderPath"></param>
        /// <param name="databaseFormatType">Typically fasta for .fasta files; fastapro will create a .fasta.pro file</param>
        /// <param name="outputSequenceType">Sequence type (forward, reverse, scrambled, decoy, or decoyX)</param>
        /// <returns>CRC32 hash of the generated (or retrieved) file</returns>
        [Obsolete("Use the method that does not take databaseFormatType")]
        public string ExportFASTAFile(int proteinCollectionId, string destinationFolderPath, DatabaseFormatTypes databaseFormatType, SequenceTypes outputSequenceType)
        {
            return ExportFASTAFile(proteinCollectionId, destinationFolderPath, outputSequenceType);
        }

        /// <summary>
        /// Create the FASTA file for the given protein collection ID
        /// </summary>
        /// <param name="proteinCollectionId">Protein collection ID</param>
        /// <param name="destinationFolderPath"></param>
        /// <param name="outputSequenceType">Sequence type (forward, reverse, scrambled, decoy, or decoyX)</param>
        /// <returns>CRC32 hash of the generated (or retrieved) file</returns>
        public string ExportFASTAFile(int proteinCollectionId, string destinationFolderPath, SequenceTypes outputSequenceType)
        {
            var proteinCollectionName = GetProteinCollectionName(proteinCollectionId);

            var creationOptionsHandler = new FileCreationOptions(mDatabaseAccessor);

            var creationOptions = creationOptionsHandler.MakeCreationOptionsString(outputSequenceType);

            var protCollectionList = new List<string>()
            {
                proteinCollectionName
            };

            return ExportProteinCollections(protCollectionList, creationOptions, destinationFolderPath, 0, true, outputSequenceType);
        }

        /// <summary>
        /// Create the FASTA file, either for the given protein collections, or for the legacy FASTA file
        /// </summary>
        /// <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
        /// <param name="creationOptions">Creation options, for example: seq_direction=forward,filetype=fasta</param>
        /// <param name="legacyFASTAFileName">Legacy FASTA file name, or empty string if exporting protein collections</param>
        /// <param name="destinationFolderPath"></param>
        /// <returns>CRC32 hash of the generated (or retrieved) file</returns>
        public string ExportFASTAFile(string protCollectionList, string creationOptions, string legacyFASTAFileName, string destinationFolderPath)
        {
            // Returns the CRC32 hash of the exported file
            // Returns nothing or "" if an error

            var optionsParser = new FileCreationOptions(mDatabaseAccessor);

            // Trim any leading or trailing commas
            protCollectionList = protCollectionList.Trim(',');

            // Look for cases of multiple commas in a row or spaces around a comma
            // Replace any matches with a single comma
            var extraCommaCheckRegex = new Regex("[, ]{2,}");

            protCollectionList = extraCommaCheckRegex.Replace(protCollectionList, ",");

            if (protCollectionList.Length > 0 && !protCollectionList.Equals("na", StringComparison.OrdinalIgnoreCase))
            {
                // Parse out protein collections from "," delimited list

                var collectionList = protCollectionList.Split(',').ToList();

                // Parse options string
                var cleanOptionsString = optionsParser.ExtractOptions(creationOptions);

                return ExportProteinCollections(collectionList, cleanOptionsString, destinationFolderPath, 0, true, optionsParser.SequenceDirection);
            }
            else if (legacyFASTAFileName.Length > 0 && !legacyFASTAFileName.Equals("na", StringComparison.OrdinalIgnoreCase))
            {
                return ExportLegacyFastaFile(legacyFASTAFileName, destinationFolderPath);
            }

            return null;
        }

        private string ExportLegacyFastaFile(string legacyFASTAFileName, string destinationFolderPath)
        {
            var filenameSha1Hash = GenerateHash(legacyFASTAFileName);
            var lockFileHash = filenameSha1Hash;

            if (!LookupLegacyFastaFileDetails(legacyFASTAFileName, out var legacyStaticFilePath, out var crc32Hash))
            {
                // Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
                // An exception has probably already been thrown
                return null;
            }

            var fiSourceFile = new FileInfo(legacyStaticFilePath);

            if (!fiSourceFile.Exists)
            {
                // Be careful changing this message; the AnalysisResources class in the Analysis Manager
                // looks for error messages that start with "Legacy fasta file not found:"
                var msg = "Legacy fasta file not found: " + legacyStaticFilePath + " (path comes from V_Legacy_Static_File_Locations)";
                OnErrorEvent(msg);
                throw new Exception(msg);
            }

            // Look for file LegacyFASTAFileName in folder destinationFolderPath
            // If it exists, and if a .lock file does not exist, then compare file sizes and file modification dates

            var fiFinalFile = new FileInfo(Path.Combine(destinationFolderPath, legacyFASTAFileName));
            if (fiFinalFile.Exists && fiFinalFile.Length > 0L)
            {
                // Make sure a .lock file doesn't exist
                // If it does exist, then another process on this computer is likely creating the .Fasta file

                var lockFi = new FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"));

                if (lockFi.Exists)
                {
                    // Another program is creating a .Fasta file; cannot assume it is ready-for-use
                }
                // Make sure the file sizes match and that the local file is not older than the source file
                else if (fiSourceFile.Length == fiFinalFile.Length && fiFinalFile.LastWriteTimeUtc >= fiSourceFile.LastWriteTimeUtc.AddSeconds(-0.1d))
                {
                    if (ExportLegacyFastaValidateHash(fiFinalFile, ref crc32Hash, false))
                    {
                        OnTaskCompletion(fiFinalFile.FullName);
                        return crc32Hash;
                    }
                }
            }

            // The file is not present on the local computer (or the file size is different or it is older than the parent fasta file)
            // We need to create a lock file, then copy the .fasta file locally

            if (string.IsNullOrEmpty(legacyStaticFilePath))
            {
                var msg = "Storage path for " + legacyFASTAFileName + " is empty according to V_Legacy_Static_File_Locations; unable to continue";
                OnErrorEvent(msg);
                throw new Exception(msg);
            }

            // Make sure we have enough disk free space

            var destinationPath = Path.Combine(destinationFolderPath, "TargetFile.tmp");

            var sourceFileSizeMB = fiSourceFile.Length / 1024.0d / 1024.0d;

            var success = DiskInfo.GetDiskFreeSpace(destinationPath, out var currentFreeSpaceBytes, out var errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                var spaceValidationError = "Unable to copy legacy FASTA file to " + destinationFolderPath + ". " + errorMessage;
                OnErrorEvent(spaceValidationError);
                throw new IOException(spaceValidationError);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, sourceFileSizeMB, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                var spaceValidationError = "Unable to copy legacy FASTA file to " + destinationFolderPath + ". " + errorMessage;
                OnErrorEvent(spaceValidationError);
                throw new IOException(spaceValidationError);
            }

            // If we get here, then finalFileName = "" or the file is not present or the LockFile is present
            // Try to create a lock file, then either wait for an existing lock file to go away or export the database
            var lockStream = CreateLockStream(destinationFolderPath, lockFileHash, legacyFASTAFileName);

            if (lockStream == null)
            {
                // Unable to create a lock stream; an exception has likely already been thrown
                var msg = "Unable to create lock file required to export " + legacyFASTAFileName;
                OnErrorEvent(msg);
                throw new Exception(msg);
            }

            if (fiFinalFile != null)
            {
                // Check again for the existence of the desired .Fasta file
                // It's possible another process created .Fasta file while this process was waiting for the other process's lock file to disappear
                fiFinalFile.Refresh();
                if (fiFinalFile.Exists && fiSourceFile.Length == fiFinalFile.Length && fiFinalFile.LastWriteTimeUtc >= fiSourceFile.LastWriteTimeUtc.AddSeconds(-0.1d))
                {
                    // The final file now does exist (and has the correct size / date)
                    // The other process that made the file should have updated the database with the file hash; determine the hash now
                    if (!LookupLegacyFastaFileDetails(legacyFASTAFileName, out legacyStaticFilePath, out crc32Hash))
                    {
                        // Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
                        // An exception has probably already been thrown
                        return null;
                    }

                    if (ExportLegacyFastaValidateHash(fiFinalFile, ref crc32Hash, false))
                    {
                        DeleteLockStream(destinationFolderPath, lockFileHash, lockStream);
                        OnTaskCompletion(fiFinalFile.FullName);
                        return crc32Hash;
                    }
                }
            }

            // Copy the .Fasta file from the remote computer to this computer
            // We're temporarily naming it with a SHA1 hash based on the filename
            var interimFastaFi = new FileInfo(Path.Combine(destinationFolderPath, filenameSha1Hash + "_" + Path.GetFileNameWithoutExtension(legacyStaticFilePath) + ".fasta"));
            if (interimFastaFi.Exists)
            {
                interimFastaFi.Delete();
            }

            mLastLockQueueWaitTimeLog = DateTime.UtcNow;
            mFileTools.CopyFileUsingLocks(fiSourceFile, interimFastaFi.FullName, "OrgDBHandler", overWrite: false);

            // Now that the copy is done, rename the file to the final name
            fiFinalFile.Refresh();
            if (fiFinalFile.Exists)
            {
                // Somehow the final file has appeared in the folder; it could be a corrupt version of the .fasta file
                // Delete it
                fiFinalFile.Delete();
            }

            interimFastaFi.MoveTo(fiFinalFile.FullName);

            // File successfully copied to this computer
            // Update the hash validation file, and update the DB if the newly copied file's hash value differs from the DB
            if (ExportLegacyFastaValidateHash(fiFinalFile, ref crc32Hash, true))
            {
                DeleteLockStream(destinationFolderPath, lockFileHash, lockStream);
                OnTaskCompletion(fiFinalFile.FullName);
                return crc32Hash;
            }

            // This code will only get reached if an error occurred in ExportLegacyFastaValidateHash()
            // We'll go ahead and return the hash anyway
            DeleteLockStream(destinationFolderPath, lockFileHash, lockStream);
            OnFileGenerationCompleted(fiFinalFile.FullName);
            OnTaskCompletion(fiFinalFile.FullName);

            return crc32Hash;
        }

        private bool ExportLegacyFastaValidateHash(FileSystemInfo finalFileFi, ref string finalFileHash, bool forceRegenerateHash)
        {
            if (string.IsNullOrEmpty(finalFileHash))
            {
                finalFileHash = GenerateAndStoreLegacyFileHash(finalFileFi.FullName);

                // Update the hash validation file
                UpdateHashValidationFile(finalFileFi.FullName, finalFileHash);

                return true;
            }
            // ValidateMatchingHash will use GenerateFileAuthenticationHash() to generate a hash for the given file
            // Since this can be time consuming, we only do this every 48 hours
            // If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
            else if (ValidateMatchingHash(finalFileFi.FullName, ref finalFileHash, 48, forceRegenerateHash))
            {
                return true;
            }

            return false;
        }

        private string ExportProteinCollections(
            List<string> protCollectionList,
            string creationOptionsString,
            string destinationFolderPath,
            int alternateAnnotationTypeId,
            bool padWithPrimaryAnnotation,
            SequenceTypes outputSequenceType)
        {
            var proteinCollectionList = string.Join(",", protCollectionList);

            var stringToHash = proteinCollectionList + "/" + creationOptionsString;
            var filenameSha1Hash = GenerateHash(stringToHash);
            var lockFileHash = filenameSha1Hash;

            string finalFileName;
            string finalFileHash;

            var fileNameSql = "SELECT Archived_File_Path, Archived_File_ID, Authentication_Hash " +
                              "FROM T_Archived_Output_Files " +
                              "WHERE Collection_List_Hex_Hash = '" + filenameSha1Hash + "' AND " +
                                     "Protein_Collection_List = '" + proteinCollectionList + "' AND " +
                                     "Archived_File_State_ID <> 3 " +
                              "ORDER BY File_Modification_Date Desc";

            var fileNameTable = mDatabaseAccessor.GetTable(fileNameSql);
            if (fileNameTable.Rows.Count >= 1)
            {
                var foundRow = fileNameTable.Rows[0];
                finalFileName = Path.GetFileName(foundRow["Archived_File_Path"].ToString());
                finalFileHash = foundRow["Authentication_Hash"].ToString();
            }
            else
            {
                finalFileName = string.Empty;
                finalFileHash = string.Empty;
            }

            FileInfo finalFileFi = null;

            if (finalFileName.Length > 0)
            {
                // Look for file finalFileName in folder destinationFolderPath
                // If it exists, and if a .lock file does not exist, then we can safely assume the .Fasta file is ready for use

                finalFileFi = new FileInfo(Path.Combine(destinationFolderPath, finalFileName));
                if (finalFileFi.Exists && finalFileFi.Length > 0L)
                {
                    // Make sure a .lock file doesn't exist
                    // If it does exist, then another process on this computer is likely creating the .Fasta file

                    var lockFile = new FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"));

                    if (lockFile.Exists)
                    {
                        // Another program is creating a .Fasta file; cannot assume it is ready-for-use
                    }
                    // ValidateMatchingHash will use GenerateFileAuthenticationHash() to generate a hash for the given file
                    // Since this can be time consuming, we only do this every 48 hours
                    // If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
                    else if (ValidateMatchingHash(finalFileFi.FullName, ref finalFileHash, 48, false))
                    {
                        OnTaskCompletion(finalFileFi.FullName);
                        return finalFileHash;
                    }
                }
            }

            // If we get here, then finalFileName = "" or the file is not present or the LockFile is present or the hash file is out-of-date
            // Try to create a lock file, then either wait for an existing lock file to go away or export the database
            var lockStream = CreateLockStream(destinationFolderPath, lockFileHash, "Protein collection list " + proteinCollectionList);

            if (lockStream == null)
            {
                // Unable to create a lock stream; an exception has likely already been thrown
                var msg = "Unable to create lock file required to export " + finalFileName;
                OnErrorEvent(msg);
                throw new Exception(msg);
            }

            if (finalFileFi != null)
            {
                // Check again for the existence of the desired .Fasta file
                // It's possible another process created the .Fasta file while this process was waiting for the other process's lock file to disappear
                finalFileFi.Refresh();
                if (finalFileFi.Exists && finalFileFi.Length > 0L)
                {
                    // The final file now does exist (and is non-zero in size); we're good to go

                    if (string.IsNullOrEmpty(finalFileHash))
                    {
                        // FinalFileHash is empty, which means the other process that just exported this file was the first process to actually use this file
                        // We need to return a non-empty hash value, so compute the SHA1 hash now
                        finalFileHash = GenerateFileAuthenticationHash(finalFileFi.FullName);
                    }

                    // ValidateMatchingHash will use GenerateFileAuthenticationHash() to generate a hash for the given file
                    // Since this can be time consuming, we only do this every 48 hours
                    // If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
                    if (ValidateMatchingHash(finalFileFi.FullName, ref finalFileHash, 48, false))
                    {
                        DeleteLockStream(destinationFolderPath, lockFileHash, lockStream);
                        OnTaskCompletion(finalFileFi.FullName);
                        return finalFileHash;
                    }
                }
            }

            // We're finally ready to generate the .Fasta file

            // Initialize the ClassSelector
            ClassSelector(outputSequenceType, DecoyProteinsUseXXX);

            // If more than one protein collection, then we're generating a dynamic protein collection
            if (protCollectionList.Count > 1)
            {
                mCollectionType = ArchiveOutputFilesBase.CollectionTypes.Dynamic;
            }

            string crc32Hash;

            try
            {
                OnDebugEvent("Retrieving fasta file for protein collections " + string.Join(",", protCollectionList.ToArray()));

                // Export the fasta file
                crc32Hash = mGetter.ExportFASTAFile(protCollectionList, destinationFolderPath, alternateAnnotationTypeId, padWithPrimaryAnnotation);

                if (string.IsNullOrEmpty(crc32Hash))
                {
                    const string msg = "mGetter.ExportFASTAFile returned a blank string for the CRC32 authentication hash; this likely represents a problem";
                    OnErrorEvent(msg);
                    throw new Exception(msg);
                }

                var firstCollectionProcessed = false;
                var archivedFileId = default(int);

                foreach (var collectionName in protCollectionList)
                {
                    if (!firstCollectionProcessed)
                    {
                        archivedFileId = mArchiver.ArchiveCollection(
                            collectionName, mCollectionType, mOutputSequenceType,
                            mFinalOutputPath,
                            creationOptionsString, crc32Hash, proteinCollectionList);

                        if (archivedFileId == 0)
                        {
                            // Error making an entry in T_Archived_Output_Files; abort
                            const string msg = "Error archiving collection; Archived_File_ID = 0";
                            OnErrorEvent(msg);
                            throw new Exception(msg);
                        }

                        firstCollectionProcessed = true;
                    }
                    else
                    {
                        var existingCollectionId = GetProteinCollectionId(collectionName);
                        mArchiver.AddArchiveCollectionXRef(existingCollectionId, archivedFileId);
                    }
                }

                // Rename the new protein collection to the correct, final name on the local computer
                // E.g. rename from 38FFACAC.fasta to ID_001874_38FFACAC.fasta
                var interimFastaFile = new FileInfo(mFinalOutputPath);

                finalFileName = Path.GetFileName(mArchiver.Archived_File_Name);
                finalFileFi = new FileInfo(Path.Combine(destinationFolderPath, finalFileName));

                if (finalFileFi.Exists)
                {
                    // Somehow the final file has appeared in the directory (this shouldn't have happened with the lock file present)
                    // Delete it
                    finalFileFi.Delete();
                }

                // Delete any other files that exist with the same extension as finalFileFI.FullName
                // These are likely index files used by Inspect or MSGF+ (aka MSGFDB) and they will need to be re-generated
                DeleteFASTAIndexFiles(finalFileFi);

                interimFastaFile.MoveTo(finalFileFi.FullName);

                OnStatusEvent("Created fasta file " + finalFileFi.FullName);

                // Update the hash validation file
                UpdateHashValidationFile(finalFileFi.FullName, crc32Hash);
            }
            catch
            {
                DeleteLockStream(destinationFolderPath, lockFileHash, lockStream);
                throw;
            }

            DeleteLockStream(destinationFolderPath, lockFileHash, lockStream);

            OnTaskCompletion(finalFileFi.FullName);
            return crc32Hash;
        }

        private FileStream CreateLockStream(string destinationFolderPath, string lockFileHash, string proteinCollectionListOrLegacyFastaFileName)
        {
            // Creates a new lock file
            // If an existing file is not found, but a lock file was successfully created, then lockStream will be a valid file stream

            var startTime = DateTime.UtcNow;
            var intAttemptCount = 0;

            var lockFile = new FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"));
            while (true)
            {
                intAttemptCount++;
                try
                {
                    lockFile.Refresh();
                    if (lockFile.Exists)
                    {
                        var lockTimeoutTime = lockFile.LastWriteTimeUtc.AddMinutes(60d);
                        var msg = LockFileProgressText + " found; waiting until it is deleted or until " + lockTimeoutTime.ToLocalTime() + ": " + lockFile.Name;
                        OnDebugEvent(msg);
                        OnFileGenerationProgressUpdate(msg, 0d);

                        while (lockFile.Exists && DateTime.UtcNow < lockTimeoutTime)
                        {
                            Thread.Sleep(5000);
                            lockFile.Refresh();
                            if (DateTime.UtcNow.Subtract(startTime).TotalMinutes >= 60d)
                            {
                                break;
                            }
                        }

                        lockFile.Refresh();
                        if (lockFile.Exists)
                        {
                            var warningMsg = LockFileProgressText + " still exists; assuming another process timed out; thus, now deleting file " + lockFile.Name;
                            OnWarningEvent(warningMsg);
                            OnFileGenerationProgressUpdate(warningMsg, 0d);
                            lockFile.Delete();
                        }
                    }

                    // Try to create a lock file so that the calling procedure can create the required .Fasta file (or validate that it now exists)

                    // Try to create the lock file
                    // If another process is still using it, an exception will be thrown
                    var lockStream = new FileStream(lockFile.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.Read);

                    return lockStream;
                }
                catch (Exception ex)
                {
                    var msg = "Exception while monitoring " + LockFileProgressText + " " + lockFile.FullName + ": " + ex.Message;
                    OnErrorEvent(msg);
                    OnFileGenerationProgressUpdate(msg, 0d);
                }

                // Something went wrong; wait for 15 seconds then try again
                Thread.Sleep(15000);

                if (intAttemptCount >= 4)
                {
                    // Something went wrong 4 times in a row (typically either creating or deleting the .Lock file)
                    // Give up trying to export
                    if (proteinCollectionListOrLegacyFastaFileName == null)
                    {
                        proteinCollectionListOrLegacyFastaFileName = "??";
                    }

                    // Exception: Unable to create Lockfile required to export Protein collection ...
                    var msg = "Unable to create " + LockFileProgressText + " required to export " + proteinCollectionListOrLegacyFastaFileName + "; tried 4 times without success";
                    OnErrorEvent(msg);
                    throw new Exception(msg);
                }
            }
        }

        private void DeleteFASTAIndexFiles(FileInfo fiFinalFastaFile)
        {
            try
            {
                var strBaseName = Path.GetFileNameWithoutExtension(fiFinalFastaFile.Name);

                // Delete files with the same name but different extensions
                // For example, Inspect's PrepDB.py script creates these files:
                // ID_002750_1363538A.index
                // ID_002750_1363538A_shuffle.index
                // ID_002750_1363538A.trie
                // ID_002750_1363538A_shuffle.trie
                // ID_002750_1363538A_shuffle_Log.txt

                // ReSharper disable CommentTypo

                // MSGFDB's BuildSA function creates these files:
                // ID_002614_23305E80.revConcat.fasta
                // ID_002614_23305E80.fasta.23305E80.hashcheck
                // ID_002614_23305E80_sarray.lock
                // ID_002614_23305E80.revConcat.sarray
                // ID_002614_23305E80.sarray
                // ID_002614_23305E80.revConcat.seq
                // ID_002614_23305E80.seq
                // ID_002614_23305E80.revConcat.seqanno
                // ID_002614_23305E80.seqanno

                // ReSharper restore CommentTypo

                // This code will also delete the .hashcheck file; that's OK
                // e.g., ID_002750_1363538A.fasta.1363538A.hashcheck

                foreach (FileInfo fiFileToDelete in fiFinalFastaFile.Directory.GetFiles(strBaseName + ".*"))
                    DeleteFastaIndexFile(fiFileToDelete.FullName);

                foreach (FileInfo fiFileToDelete in fiFinalFastaFile.Directory.GetFiles(strBaseName + "_shuffle*.*"))
                    DeleteFastaIndexFile(fiFileToDelete.FullName);
            }
            catch (Exception)
            {
                // Ignore errors here
            }
        }

        private void DeleteFastaIndexFile(string strFilePath)
        {
            try
            {
                File.Delete(strFilePath);
            }
            catch (Exception ex)
            {
                OnErrorEvent("Error deleting file", ex);
            }
        }

        private void DeleteLockStream(string destinationFolderPath, string lockFileHash, Stream lockStream)
        {
            lockStream?.Close();

            var lockFi = new FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"));
            if (lockFi != null)
            {
                if (lockFi.Exists)
                {
                    lockFi.Delete();
                }
            }
        }

        private string GenerateAndStoreLegacyFileHash(string fastaFilePath)
        {
            // The database does not have a valid Authentication_Hash values for this .Fasta file; generate one now
            var crc32Hash = GenerateFileAuthenticationHash(fastaFilePath);

            // Add an entry to T_Legacy_File_Upload_Requests
            // Also store the CRC32 hash for future use
            RunSP_AddLegacyFileUploadRequest(Path.GetFileName(fastaFilePath), crc32Hash);

            return crc32Hash;
        }

        private bool LookupLegacyFastaFileDetails(string legacyFASTAFileName, out string legacyStaticFilePathOutput, out string crc32HashOutput)
        {
            // Lookup the details for LegacyFASTAFileName in the database
            var legacyLocationsSql = "SELECT FileName, Full_Path, Authentication_Hash FROM V_Legacy_Static_File_Locations WHERE FileName = '" + legacyFASTAFileName + "'";

            var legacyStaticFileLocations = mDatabaseAccessor.GetTable(legacyLocationsSql);
            if (legacyStaticFileLocations.Rows.Count == 0)
            {
                var msg = "Legacy fasta file " + legacyFASTAFileName + " not found in V_Legacy_Static_File_Locations; unable to continue";
                OnErrorEvent(msg);
                throw new Exception(msg);
            }

            legacyStaticFilePathOutput = legacyStaticFileLocations.Rows[0]["Full_Path"].ToString();
            crc32HashOutput = legacyStaticFileLocations.Rows[0]["Authentication_Hash"]?.ToString() ?? string.Empty;

            return true;
        }

        /// <summary>
        /// Construct the hashcheck file path, given the FASTA file path and its CRC32 hash
        /// </summary>
        /// <param name="strFastaFilePath"></param>
        /// <param name="crc32Hash"></param>
        /// <param name="hashCheckExtension">Hashcheck file extension; if an empty string, the default of .hashcheck is used</param>
        /// <returns>FileInfo object for the .hashcheck file</returns>
        /// <remarks>
        /// Example .hashcheck filenames:
        /// ID_004137_23AA5A07.fasta.23AA5A07.hashcheck
        /// H_sapiens_Ensembl_v68_2013-01-08.fasta.DF687525.hashcheck
        /// </remarks>
        private FileInfo GetHashFileValidationInfo(string strFastaFilePath, string crc32Hash, string hashCheckExtension = "")
        {
            string extensionToUse;
            if (string.IsNullOrWhiteSpace(hashCheckExtension))
            {
                extensionToUse = HashcheckSuffix;
            }
            else
            {
                extensionToUse = hashCheckExtension;
            }

            var fastaFile = new FileInfo(strFastaFilePath);
            var hashValidationFileName = Path.Combine(fastaFile.DirectoryName, fastaFile.Name + "." + crc32Hash + extensionToUse);

            return new FileInfo(hashValidationFileName);
        }

        /// <summary>
        /// Update the hashcheck file
        /// </summary>
        /// <param name="strFastaFilePath"></param>
        /// <param name="crc32Hash"></param>
        /// <param name="hashcheckExtension">Hashcheck file extension; if an empty string, the default of .hashcheck is used</param>
        private void UpdateHashValidationFile(string strFastaFilePath, string crc32Hash, string hashcheckExtension = "")
        {
            var fiHashValidationFile = GetHashFileValidationInfo(strFastaFilePath, crc32Hash, hashcheckExtension);

            using var writer = new StreamWriter(new FileStream(fiHashValidationFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read));

            writer.WriteLine("Hash validated " + DateTime.Now);
            writer.WriteLine("Validated on " + Environment.MachineName);
        }

        /// <summary>
        /// Validates that the hash of a .fasta file matches the expected hash value
        /// If the actual hash differs and if forceRegenerateHash=True, then this strExpectedHash get updated
        /// forceRegenerateHash should be set to True only when processing legacy fasta files that have been newly copied to this computer
        /// </summary>
        /// <param name="fastaFilePath">Fasta file to check</param>
        /// <param name="expectedHash">Expected CRC32 hash; updated if incorrect and forceRegenerateHash is true</param>
        /// <param name="retryHoldoffHours">Time between re-generating the hash value for an existing file</param>
        /// <param name="forceRegenerateHash">Re-generate the hash</param>
        /// <param name="hashcheckExtension">Hashcheck file extension; if an empty string, the default of .hashcheck is used</param>
        /// <returns>True if the hash values match, or if forceRegenerateHash=True</returns>
        /// <remarks>Public method because the Analysis Manager uses this class when running offline jobs</remarks>
        public bool ValidateMatchingHash(string fastaFilePath, ref string expectedHash, int retryHoldoffHours = 48, bool forceRegenerateHash = false, string hashcheckExtension = "")
        {
            try
            {
                var fastaFile = new FileInfo(fastaFilePath);

                if (fastaFile.Exists)
                {
                    var hashValidationFile = GetHashFileValidationInfo(fastaFilePath, expectedHash, hashcheckExtension);

                    if (hashValidationFile.Exists && !forceRegenerateHash)
                    {
                        if (DateTime.UtcNow.Subtract(hashValidationFile.LastWriteTimeUtc).TotalHours <= retryHoldoffHours)
                        {
                            OnDebugEvent("Validated hash validation file (recently verified): " + hashValidationFile.FullName);
                            // Hash check file exists, and the file is less than 48 hours old
                            return true;
                        }
                    }

                    // Either the hash validation file doesn't exist, or it's too old, or forceRegenerateHash = True
                    // Regenerate the hash
                    var crc32Hash = GenerateFileAuthenticationHash(fastaFile.FullName);

                    if (string.Equals(expectedHash, crc32Hash) || forceRegenerateHash)
                    {
                        // Update the hash validation file
                        UpdateHashValidationFile(fastaFilePath, crc32Hash, hashcheckExtension);

                        if ((expectedHash ?? "") != (crc32Hash ?? "") && forceRegenerateHash)
                        {
                            // Hash values don't match, but forceRegenerateHash=True
                            // Update the hash value stored in T_Legacy_File_Upload_Requests for this fasta file
                            RunSP_AddLegacyFileUploadRequest(fastaFile.Name, crc32Hash);

                            // Update expectedHash
                            expectedHash = crc32Hash;

                            OnStatusEvent("Re-exported protein collection and created new hash file due to CRC32 hash mismatch: " + hashValidationFile.FullName);
                        }
                        else
                        {
                            OnDebugEvent("Validated hash validation file (re-verified): " + hashValidationFile.FullName);
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = "Exception while re-computing the hash of the fasta file: " + ex.Message;
                OnErrorEvent(msg, ex);
                OnFileGenerationProgressUpdate(msg, 0d);
            }

            return false;
        }

        #region "Events and Event Handlers"

        public event FileGenerationCompletedEventHandler FileGenerationCompleted;
        public event FileGenerationProgressEventHandler FileGenerationProgress;
        public event FileGenerationStartedEventHandler FileGenerationStarted;

        private void OnFileGenerationCompleted(string outputPath)
        {
            mFinalOutputPath = outputPath;
            OnDebugEvent("Saved fasta file to " + outputPath);
        }

        /// <summary>
        /// Raises event FileGenerationCompleted is raised once the fasta file is done being created
        /// </summary>
        /// <param name="finalOutputPath"></param>
        private void OnTaskCompletion(string finalOutputPath)
        {
            FileGenerationCompleted?.Invoke(finalOutputPath);
        }

        private void FileTools_WaitingForLockQueue(string sourceFilePath, string targetFilePath, int sourceBacklogMB, int targetBacklogMB)
        {
            if (DateTime.UtcNow.Subtract(mLastLockQueueWaitTimeLog).TotalSeconds >= 30d)
            {
                mLastLockQueueWaitTimeLog = DateTime.UtcNow;
                Console.WriteLine("Waiting for lockfile queue to fall below threshold to fall below threshold (Protein_Exporter); " +
                                  "SourceBacklog=" + sourceBacklogMB + " MB, " +
                                  "TargetBacklog=" + targetBacklogMB + " MB, " +
                                  "Source=" + sourceFilePath + ", " +
                                  "Target=" + targetFilePath);

                string strServers;
                if (sourceBacklogMB > 0 && targetBacklogMB > 0)
                {
                    strServers = mFileTools.GetServerShareBase(sourceFilePath) + " and " + mFileTools.GetServerShareBase(targetFilePath);
                }
                else if (targetBacklogMB > 0)
                {
                    strServers = mFileTools.GetServerShareBase(targetFilePath);
                }
                else
                {
                    strServers = mFileTools.GetServerShareBase(sourceFilePath);
                }

                var msg = "Waiting for lockfile queue on " + strServers + " to fall below threshold";
                OnDebugEvent(msg);
                OnFileGenerationProgressUpdate(msg, 0d);
            }
        }

        private void OnFileGenerationStarted(string taskMsg)
        {
            FileGenerationStarted?.Invoke(taskMsg);
        }

        private void OnFileGenerationProgressUpdate(string statusMsg, double fractionDone)
        {
            FileGenerationProgress?.Invoke(statusMsg, fractionDone);
        }

        /// <summary>
        /// Compute the CRC32 hash for the file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns>File hash</returns>
        public string GenerateFileAuthenticationHash(string fullFilePath)
        {
            return mGetter.GetFileHash(fullFilePath);
        }

        public Dictionary<int, string> GetAllCollections()
        {
            return mGetter.GetCollectionNameList();
        }

        public Dictionary<string, string> GetCollectionsByOrganism(int organismId)
        {
            return mGetter.GetCollectionsByOrganism(organismId);
        }

        public DataTable GetCollectionsByOrganismTable(int organismId)
        {
            return mGetter.GetCollectionsByOrganismTable(organismId);
        }

        public Dictionary<string, string> GetOrganismList()
        {
            return mGetter.GetOrganismList();
        }

        public DataTable GetOrganismListTable()
        {
            return mGetter.GetOrganismListTable();
        }

        public string GetStoredFileAuthenticationHash(int proteinCollectionId)
        {
            return mGetter.GetStoredHash(proteinCollectionId);
        }

        public string GetStoredFileAuthenticationHash(string proteinCollectionName)
        {
            return mGetter.GetStoredHash(proteinCollectionName);
        }

        public int GetProteinCollectionId(string proteinCollectionName)
        {
            return mGetter.FindIdByName(proteinCollectionName);
        }

        private string GetProteinCollectionName(int proteinCollectionId)
        {
            return mGetter.FindNameById(proteinCollectionId);
        }

        #endregion

        private int RunSP_AddLegacyFileUploadRequest(string legacyFilename, string authenticationHash)
        {
            if (mDatabaseAccessor == null)
            {
                return 0;
            }

            var dbTools = mDatabaseAccessor.DbTools;

            var cmdSave = dbTools.CreateCommand("AddLegacyFileUploadRequest", CommandType.StoredProcedure);

            // Define parameter for procedure's return value
            var returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue);

            // Define parameters for the procedure's arguments
            dbTools.AddParameter(cmdSave, "@legacy_File_name", SqlType.VarChar, 128).Value = legacyFilename;
            dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256).Direction = ParameterDirection.Output;
            dbTools.AddParameter(cmdSave, "@AuthenticationHash", SqlType.VarChar, 8).Value = authenticationHash;

            // Execute the sp
            dbTools.ExecuteSP(cmdSave);

            // Get return value
            var ret = dbTools.GetInteger(returnParam.Value);

            return ret;
        }

        private string GenerateHash(string sourceText)
        {
            // Create an encoding object to ensure the encoding standard for the source text
            var encoding = new ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var byteSourceText = encoding.GetBytes(sourceText);

            // Compute the hash value from the source
            var sha1Hash = sha1Provider.ComputeHash(byteSourceText);

            // And convert it to String format for return
            var sha1String = BitConverter.ToString(sha1Hash).Replace("-", "").ToLower();

            return sha1String;
        }
    }
}