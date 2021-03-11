﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using OrganismDatabaseHandler.ProteinUpload;
using PRISMDatabaseUtils;
using ValidateFastaFile;

// This class will read a text file specifying one or more fasta files to load into the Protein Sequences database
//
// -------------------------------------------------------------------------------
// Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
// Program started October 10, 2014

// E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov
// Website: http://panomics.pnnl.gov/ or http://omics.pnl.gov
// -------------------------------------------------------------------------------
//
// Licensed under the Apache License, Version 2.0; you may not use this file except
// in compliance with the License.  You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0

namespace Bulk_Fasta_Importer
{
    public class BulkFastaImporter : PRISM.FileProcessor.ProcessFilesBase
    {
        #region "Constants and Enums"

        public const string DmsConnectionString = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;";
        public const string ProteinseqsConnectionString = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;";

        public enum BulkImporterErrorCodes
        {
            NoError = 0,
            DatabaseError = 1,
            UnspecifiedError = -1,
        }

        #endregion

        #region "Structures"
        public struct FastaFileInfoType
        {
            public string FilePath;
            public int OrganismId;
            public int AuthId;
        }
        #endregion

        #region "Classwide Variables"

        private PSUploadHandler mUploadHandler;

        /// <summary>
        /// Organism info, where keys are organism name and values are organism ID
        /// </summary>
        /// <remarks>String searches will be case insensitive</remarks>
        private Dictionary<string, int> mOrganismInfo;

        /// <summary>
        /// Annotation type info, where keys are annotation type names and values are annotation type IDs
        /// </summary>
        /// <remarks>String searches will be case insensitive</remarks>
        private Dictionary<string, int> mAnnotationTypeInfo;

        /// <summary>
        /// Protein collection info, where keys are protein collection names and values are protein collection IDs
        /// </summary>
        /// <remarks>String searches will be case insensitive</remarks>
        private Dictionary<string, int> mProteinCollectionInfo;

        private bool mDatabaseDataLoaded;
        private IDBTools mDbToolsDms;
        private IDBTools mDbToolsProteinSeqs;

        private bool mAnnotationViewInfoShown;
        private bool mOrganismViewInfoShown;

        private DateTime mLastProgressTime;

        private BulkImporterErrorCodes mLocalErrorCode;

        #endregion

        #region "Properties"

        public string DMSConnectionString { get; private set; }

        public bool PreviewMode { get; set; }

        public string ProteinSeqsConnectionString { get; private set; }

        public bool ValidationAllowAllSymbolsInProteinNames { get; set; }
        public bool ValidationAllowAsterisks { get; set; }
        public bool ValidationAllowDash { get; set; }
        public int ValidationMaxProteinNameLength { get; set; }

        #endregion

        public BulkFastaImporter(string dmsConnString, string proteinSeqsConnString)
        {
            mFileDate = "February 18, 2020";

            if (string.IsNullOrWhiteSpace(dmsConnString))
            {
                dmsConnString = DmsConnectionString;
            }

            if (string.IsNullOrWhiteSpace(proteinSeqsConnString))
            {
                proteinSeqsConnString = ProteinseqsConnectionString;
            }

            DMSConnectionString = dmsConnString;
            ProteinSeqsConnectionString = proteinSeqsConnString;

            InitializeLocalVariables();
        }

        /// <summary>
        /// Get the error message, or an empty string if no error
        /// </summary>
        /// <returns></returns>
        public override string GetErrorMessage()
        {
            string errorMessage;

            if (ErrorCode == ProcessFilesErrorCodes.LocalizedError ||
                ErrorCode == ProcessFilesErrorCodes.NoError)
            {
                switch (mLocalErrorCode)
                {
                    case BulkImporterErrorCodes.NoError:
                        errorMessage = "";
                        break;

                    case BulkImporterErrorCodes.DatabaseError:
                        errorMessage = "Database query error";
                        break;

                    case BulkImporterErrorCodes.UnspecifiedError:
                        errorMessage = "Unspecified localized error";
                        break;

                    default:
                        // This shouldn't happen
                        errorMessage = "Unknown error state";
                        break;
                }
            }
            else
            {
                errorMessage = GetBaseClassErrorMessage();
            }

            return errorMessage;
        }

        protected void InitializeLocalVariables()
        {
            mDatabaseDataLoaded = false;
            mOrganismInfo = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            mAnnotationTypeInfo = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            mProteinCollectionInfo = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

            mLocalErrorCode = BulkImporterErrorCodes.NoError;
            mLastProgressTime = DateTime.UtcNow;

            mDbToolsDms = DbToolsFactory.GetDBTools(DmsConnectionString);
            RegisterEvents(mDbToolsDms);

            mDbToolsProteinSeqs = DbToolsFactory.GetDBTools(ProteinseqsConnectionString);
            RegisterEvents(mDbToolsProteinSeqs);

            ValidationAllowAllSymbolsInProteinNames = false;
            ValidationAllowAsterisks = true;
            ValidationAllowDash = true;
            ValidationMaxProteinNameLength = FastaValidator.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;
        }

        protected List<FastaFileInfoType> ParseFastaInfoFile(string fastaInfoFilePath)
        {
            try
            {
                var sourceFileList = new List<FastaFileInfoType>();
                var sourceFileNames = new SortedSet<string>(StringComparer.CurrentCultureIgnoreCase);

                var fiInfoFile = new FileInfo(fastaInfoFilePath);
                if (!fiInfoFile.Exists)
                {
                    ShowErrorMessage("File not found: " + fastaInfoFilePath);
                    SetBaseClassErrorCode(ProcessFilesErrorCodes.InvalidInputFilePath);
                    return sourceFileList;
                }

                var requiredColsShown = default(bool);
                var currentLine = 0;

                using (var reader = new StreamReader(new FileStream(fiInfoFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                {
                    while (!reader.EndOfStream)
                    {
                        var dataLine = reader.ReadLine();
                        currentLine += 1;

                        if (string.IsNullOrWhiteSpace(dataLine))
                        {
                            continue;
                        }

                        // Required columns are:
                        // FastaFilePath, OrganismName_or_ID, AnnotationTypeName_or_ID

                        var dataCols = dataLine.Split('\t');
                        if (dataCols.Count() < 3)
                        {
                            ShowWarning("Invalid row; does not have 3 columns: " + dataLine);
                            if (!requiredColsShown)
                            {
                                ShowMessage("Required columns are: FastaFilePath, OrganismName_or_ID, and AnnotationTypeName_or_ID");
                                requiredColsShown = true;
                            }

                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(dataCols[0]))
                        {
                            ShowWarning("Fasta file path is empty for line: " + currentLine);
                            continue;
                        }

                        var fastaFilePath = dataCols[0];
                        if (!fastaFilePath.Contains(@"\"))
                        {
                            fastaFilePath = Path.Combine(fiInfoFile.DirectoryName, dataCols[0]);
                        }

                        var fiFastaFile = new FileInfo(fastaFilePath);
                        if (!fiFastaFile.Exists)
                        {
                            ShowWarning("Fasta file not found: " + fastaFilePath);
                            continue;
                        }

                        var udtFastaFileInfo = new FastaFileInfoType {FilePath = fiFastaFile.FullName};

                        if (!LookupOrganismId(dataCols[1], out var organismID))
                        {
                            continue;
                        }

                        if (!LookupAnnotationTypeId(dataCols[2], out var annotationTypeId))
                        {
                            continue;
                        }

                        udtFastaFileInfo.OrganismId = organismID;
                        udtFastaFileInfo.AuthId = annotationTypeId;

                        // Make sure the protein collection is not already in the Protein Sequences database
                        if (!LookupProteinCollectionId(Path.GetFileNameWithoutExtension(fiFastaFile.Name), out var proteinCollectionID))
                        {
                            continue;
                        }

                        if (proteinCollectionID > 0)
                        {
                            ShowWarning("Fasta file already exists as a protein collection; skipping " + fiFastaFile.Name);
                            continue;
                        }

                        // Make sure we don't add duplicate files to sourceFileList
                        if (sourceFileNames.Contains(fiFastaFile.Name))
                        {
                            ShowWarning("Skipping duplicate file: " + fiFastaFile.Name);
                            continue;
                        }

                        sourceFileList.Add(udtFastaFileInfo);
                    }
                }

                if (sourceFileList.Count == 0)
                {
                    ShowWarning("FastaInfoFile did not have any valid rows");
                }

                return sourceFileList;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error reading the Fasta Info File: " + ex.Message);
                return new List<FastaFileInfoType>();
            }
        }

        private bool LoadDatabaseInfo()
        {
            try
            {
                if (!LoadOrganisms())
                {
                    return false;
                }

                if (!LoadAnnotationInfo())
                {
                    return false;
                }

                if (!LoadProteinCollectionInfo())
                {
                    return false;
                }

                mDatabaseDataLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading database info: " + ex.Message);
                return false;
            }
        }

        private bool LoadAnnotationInfo()
        {
            try
            {
                const string sqlQuery = "SELECT ID, Display_Name FROM V_Annotation_Type_Picker";

                mAnnotationTypeInfo.Clear();

                var cmd = mDbToolsProteinSeqs.CreateCommand(sqlQuery);

                DataTable queryResults = null;
                var success = mDbToolsProteinSeqs.GetQueryResultsDataTable(cmd, out queryResults);

                if (!success)
                {
                    ReportDatabaseError("Error obtaining data from V_Annotation_Type_Picker using GetQueryResultsDataTable");
                    return false;
                }

                foreach (DataRow resultRow in queryResults.Rows)
                {
                    var annotationTypeId = mDbToolsDms.GetInteger(resultRow[0]);
                    var annotationTypeName = mDbToolsDms.GetString(resultRow[1]);

                    mAnnotationTypeInfo.Add(annotationTypeName, annotationTypeId);
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading annotation type info: " + ex.Message);
                return false;
            }
        }

        private bool LoadOrganisms()
        {
            try
            {
                const string sqlQuery = "SELECT OrganismID, Name FROM V_OrganismExport";

                mOrganismInfo.Clear();

                var cmd = mDbToolsDms.CreateCommand(sqlQuery);

                DataTable queryResults = null;
                var success = mDbToolsDms.GetQueryResultsDataTable(cmd, out queryResults);

                if (!success)
                {
                    ReportDatabaseError("Error obtaining data from V_OrganismExport using GetQueryResultsDataTable");
                    return false;
                }

                foreach (DataRow resultRow in queryResults.Rows)
                {
                    var organismId = mDbToolsDms.GetInteger(resultRow[0]);
                    var organismName = mDbToolsDms.GetString(resultRow[1]);

                    mOrganismInfo.Add(organismName, organismId);
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading organism info: " + ex.Message);
                return false;
            }
        }

        private bool LoadProteinCollectionInfo()
        {
            try
            {
                const string sqlQuery = "SELECT DISTINCT Protein_Collection_ID, Name, Collection_State FROM V_Protein_Collection_List_Export";

                mProteinCollectionInfo.Clear();

                var cmd = mDbToolsProteinSeqs.CreateCommand(sqlQuery);

                DataTable queryResults = null;
                var success = mDbToolsProteinSeqs.GetQueryResultsDataTable(cmd, out queryResults);

                if (!success)
                {
                    ReportDatabaseError("Error obtaining data from V_Protein_Collection_List_Export using GetQueryResultsDataTable");
                    return false;
                }

                foreach (DataRow resultRow in queryResults.Rows)
                {
                    var proteinCollectionId = mDbToolsDms.GetInteger(resultRow[0]);
                    var proteinCollectionName = mDbToolsDms.GetString(resultRow[1]);

                    if (!mProteinCollectionInfo.ContainsKey(proteinCollectionName))
                    {
                        mProteinCollectionInfo.Add(proteinCollectionName, proteinCollectionId);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading protein collection info: " + ex.Message);
                return false;
            }
        }

        private bool LookupAnnotationTypeId(string annotationTypeNameOrId, out int annotationTypeId)
        {
            if (!mDatabaseDataLoaded)
            {
                annotationTypeId = 0;
                if (!LoadDatabaseInfo())
                    return false;
            }

            if (int.TryParse(annotationTypeNameOrId, out annotationTypeId))
            {
                // Make sure the ID is valid
                if (!mAnnotationTypeInfo.ContainsValue(annotationTypeId))
                {
                    ShowWarning("Invalid Annotation Type ID: " + annotationTypeId);
                    if (!mAnnotationViewInfoShown)
                    {
                        Console.WriteLine("  ... see view V_Annotation_Type_Picker in the ProteinSeqs database");
                        mAnnotationViewInfoShown = true;
                    }

                    return false;
                }

                return true;
            }

            if (mAnnotationTypeInfo.TryGetValue(annotationTypeNameOrId, out annotationTypeId))
            {
                return true;
            }

            ShowWarning("Invalid Annotation Name: " + annotationTypeNameOrId);
            if (!mAnnotationViewInfoShown)
            {
                Console.WriteLine("  ... see view V_Annotation_Type_Picker in the ProteinSeqs database");
                mAnnotationViewInfoShown = true;
            }

            return false;
        }

        private bool LookupOrganismId(string organismNameOrId, out int organismId)
        {
            if (!mDatabaseDataLoaded)
            {
                organismId = 0;
                if (!LoadDatabaseInfo())
                    return false;
            }

            if (int.TryParse(organismNameOrId, out organismId))
            {
                // Make sure the ID is valid
                if (!mOrganismInfo.ContainsValue(organismId))
                {
                    ShowWarning("Invalid Organism ID: " + organismId);
                    if (!mOrganismViewInfoShown)
                    {
                        Console.WriteLine("  ... see view V_OrganismExport in the DMS5 database");
                        mOrganismViewInfoShown = true;
                    }

                    return false;
                }

                return true;
            }

            if (mOrganismInfo.TryGetValue(organismNameOrId, out organismId))
            {
                return true;
            }

            ShowWarning("Invalid Organism Name: " + organismNameOrId);
            if (!mOrganismViewInfoShown)
            {
                Console.WriteLine("  ... see view V_OrganismExport in the DMS5 database");
                mOrganismViewInfoShown = true;
            }

            return false;
        }

        /// <summary>
        /// Lookup the protein collection ID using the protein collection name
        /// </summary>
        /// <param name="proteinCollectionName"></param>
        /// <param name="proteinCollectionId">ID if a match; 0 if no match</param>
        /// <returns></returns>
        /// <remarks>True if success (even if the protein collection does not exist); false if a database error</remarks>
        private bool LookupProteinCollectionId(string proteinCollectionName, out int proteinCollectionId)
        {
            if (!mDatabaseDataLoaded)
            {
                proteinCollectionId = 0;
                if (!LoadDatabaseInfo())
                    return false;
            }

            if (mProteinCollectionInfo.TryGetValue(proteinCollectionName, out proteinCollectionId))
            {
                // Collection exists
                return true;
            }

            // Collection does not exist
            proteinCollectionId = 0;
            return true;
        }

        public override bool ProcessFile(string inputFilePath, string outputDirectoryPath, string parameterFilePath, bool resetErrorCode)
        {
            try
            {
                var sourceFileList = ParseFastaInfoFile(inputFilePath);

                if (sourceFileList.Count == 0)
                    return false;
                var success = UploadFastaFileList(sourceFileList);
                return success;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error in ProcessFile: " + ex.Message);
                return false;
            }
        }

        private void ReportDatabaseError(string errorMessage)
        {
            mLocalErrorCode = BulkImporterErrorCodes.DatabaseError;
            SetBaseClassErrorCode(ProcessFilesErrorCodes.LocalizedError);
            ShowErrorMessage(errorMessage);
        }

        [Obsolete("Unused")]
        public bool UploadFastaFile(string fastaFilePath, int organismId, int authId)
        {
            var sourceFileList = new List<FastaFileInfoType>();

            var udtFastaFileInfo = new FastaFileInfoType()
            {
                FilePath = fastaFilePath,
                OrganismId = organismId,
                AuthId = authId
            };

            sourceFileList.Add(udtFastaFileInfo);

            var success = UploadFastaFileList(sourceFileList);
            return success;
        }

        public bool UploadFastaFileList(List<FastaFileInfoType> sourceFileList)
        {
            var fileInfoList = new List<PSUploadHandler.UploadInfo>();

            foreach (var sourceFile in sourceFileList)
            {
                var fiSourceFile = new FileInfo(sourceFile.FilePath);
                if (!fiSourceFile.Exists)
                {
                    ShowWarning("File not found: " + sourceFile.FilePath);
                    continue;
                }

                var upInfo = new PSUploadHandler.UploadInfo(fiSourceFile, sourceFile.OrganismId, sourceFile.AuthId);
                fileInfoList.Add(upInfo);
            }

            return UploadFastaFileList(fileInfoList);
        }

        public bool UploadFastaFileList(List<PSUploadHandler.UploadInfo> fileInfoList)
        {
            try
            {
                // Initialize the uploader
                mUploadHandler = new PSUploadHandler(ProteinSeqsConnectionString);
                mUploadHandler.BatchProgress += mUploadHandler_BatchProgress;
                mUploadHandler.FASTAFileWarnings += mUploadHandler_FASTAFileWarnings;
                mUploadHandler.FASTAValidationComplete += mUploadHandler_FASTAValidationComplete;
                mUploadHandler.InvalidFASTAFile += mUploadHandler_InvalidFASTAFile;
                mUploadHandler.LoadEnd += mUploadHandler_LoadEnd;
                mUploadHandler.LoadProgress += mUploadHandler_LoadProgress;
                mUploadHandler.LoadStart += mUploadHandler_LoadStart;
                mUploadHandler.ValidationProgress += mUploadHandler_ValidationProgress;
                mUploadHandler.ValidFASTAFileLoaded += mUploadHandler_ValidFASTAFileLoaded;
                mUploadHandler.WroteLineEndNormalizedFASTA += mUploadHandler_WroteLineEndNormalizedFASTA;

                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAllSymbolsInProteinNames, ValidationAllowAllSymbolsInProteinNames);
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowAsterisksInResidues, ValidationAllowAsterisks);
                mUploadHandler.SetValidationOptions(PSUploadHandler.ValidationOptionConstants.AllowDashInResidues, ValidationAllowDash);
                mUploadHandler.MaximumProteinNameLength = ValidationMaxProteinNameLength;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error initializing the uploader: " + ex.Message);
                return false;
            }

            try
            {
                Console.WriteLine();
                if (PreviewMode)
                {
                    ShowMessage("Previewing upload of " + fileInfoList.Count + " file(s)");
                    foreach (var fileInfo in fileInfoList)
                        Console.WriteLine(fileInfo.FileInformation.FullName);

                    return true;
                }

                ShowMessage("Uploading " + fileInfoList.Count + " file(s)");

                // Start the upload
                mUploadHandler.BatchUpload(fileInfoList);

                return true;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error uploading fasta file list: " + ex.Message);
                return false;
            }
        }

        private void mUploadHandler_BatchProgress(string status)
        {
            if (DateTime.UtcNow.Subtract(mLastProgressTime).TotalSeconds >= 1d)
            {
                mLastProgressTime = DateTime.UtcNow;
                Console.WriteLine(status);
            }
        }

        private void mUploadHandler_FASTAFileWarnings(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> warningCollection)
        {
            try
            {
                foreach (var item in warningCollection)
                    ShowMessage("  ... Warning: " + item.MessageText + ": " + item.ProteinName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("warningCollection is not type ValidateFastaFile.CustomFastaValidator.ErrorInfoExtended");
            }
        }

        private void mUploadHandler_FASTAValidationComplete(string fastaFilePath, PSUploadHandler.UploadInfo uploadInfo)
        {
            ShowMessage("Validated " + fastaFilePath);
            ShowMessage("  ... ProteinCount: " + uploadInfo.ProteinCount);
            try
            {
                if (uploadInfo.ErrorList != null && uploadInfo.ErrorList.Count > 0)
                {
                    ShowMessage("  ... Error count: " + uploadInfo.ErrorList.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception examining UploadInfo.ErrorList: " + ex.Message);
            }
        }

        private void mUploadHandler_InvalidFASTAFile(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> errorCollection)
        {
            ShowWarning("Invalid fasta file: " + fastaFilePath);
            try
            {
                foreach (var item in errorCollection)
                    ShowMessage("  ... Error: " + item.MessageText + ": " + item.ProteinName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("errorCollection is not type ValidateFastaFile.CustomFastaValidator.ErrorInfoExtended");
            }
        }

        private void mUploadHandler_LoadEnd()
        {
        }

        private void mUploadHandler_LoadProgress(double fractionDone)
        {
            if (DateTime.UtcNow.Subtract(mLastProgressTime).TotalSeconds >= 1d)
            {
                mLastProgressTime = DateTime.UtcNow;
                Console.WriteLine("  " + (fractionDone * 100d).ToString("0.0") + "%");
            }
        }

        private void mUploadHandler_LoadStart(string taskTitle)
        {
            ShowMessage(taskTitle);
        }

        private void mUploadHandler_ValidationProgress(string taskTitle, double fractionDone)
        {
        }

        private void mUploadHandler_ValidFASTAFileLoaded(string fastaFilePath, PSUploadHandler.UploadInfo uploadData)
        {
            ShowMessage("Uploaded " + fastaFilePath);
            ShowMessage("  ... ProteinCount: " + uploadData.ProteinCount);
            try
            {
                if (uploadData.ErrorList != null && uploadData.ErrorList.Count > 0)
                {
                    ShowMessage("  ... Error count: " + uploadData.ErrorList.Count);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception examining UploadData.ErrorList: " + ex.Message);
            }
        }

        private void mUploadHandler_WroteLineEndNormalizedFASTA(string newFilePath)
        {
            Console.WriteLine("WroteLineEndNormalizedFASTA: " + newFilePath);
        }
    }
}