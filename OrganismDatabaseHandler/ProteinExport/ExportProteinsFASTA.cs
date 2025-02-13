using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using PRISM;
using PRISMWin;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class ExportProteinsFASTA : ExportProteins
    {
        // Ignore Spelling: fasta

        private readonly int mSeqLineLength = 60;

        // ReSharper disable once ConvertToPrimaryConstructor
        public ExportProteinsFASTA(GetFASTAFromDMSForward exportComponent)
            : base(exportComponent)
        {
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="fastaFilePath">Destination file path; will get updated with the final path</param>
        public override string Export(
            ProteinStorage.ProteinStorage proteins,
            ref string fastaFilePath)
        {
            const int requiredSizeMb = 150;

            var success = DiskInfo.GetDiskFreeSpace(fastaFilePath, out var currentFreeSpaceBytes, out var errorMessage);

            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to save FASTA file at " + fastaFilePath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(fastaFilePath, requiredSizeMb, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to save FASTA file at " + fastaFilePath + ". " + errorMessage);
            }

            using (var writer = new StreamWriter(fastaFilePath))
            {
                var alternateNames = string.Empty;

                OnExportStart("Writing to FASTA File");

                var counterMax = proteins.ProteinCount;
                var counter = 0;
                var hexCodeFinder = new Regex(@"[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled);

                int eventTriggerThresh;

                if (counterMax <= 25)
                {
                    eventTriggerThresh = 1;
                }
                else
                {
                    eventTriggerThresh = (int)Math.Round(counterMax / 25d);
                }

                foreach (var proteinName in proteins.GetSortedProteinNames())
                {
                    OnExportStart("Writing: " + proteinName);

                    var proteinInfo = proteins.GetProtein(proteinName);
                    var proteinSequence = proteinInfo.Sequence;

                    counter++;

                    if (counter % eventTriggerThresh == 0)
                    {
                        OnProgressUpdate("Processing: " + proteinName, Math.Round(counter / (double)counterMax, 3));
                    }

                    var proteinLength = proteinSequence.Length;
                    var proteinDescription = hexCodeFinder.Replace(proteinInfo.Description, " ");

                    writer.WriteLine((">" + proteinInfo.Reference + " " + proteinDescription + alternateNames).Trim());

                    for (var startIndex = 0; startIndex < proteinLength; startIndex += mSeqLineLength)
                    {
                        var seqLine = proteinSequence.Substring(startIndex, mSeqLineLength);
                        writer.WriteLine(seqLine);
                    }
                }
            }

            var fingerprint = GenerateFileAuthenticationHash(fastaFilePath);

            var sourceFile = new FileInfo(fastaFilePath);

            var newDestinationPath = Path.Combine(
                Path.GetDirectoryName(fastaFilePath) ?? string.Empty,
                fingerprint + Path.GetExtension(fastaFilePath));

            var targetFi = new FileInfo(newDestinationPath);

            if (sourceFile.Exists)
            {
                if (targetFi.Exists)
                {
                    targetFi.Delete();
                }

                sourceFile.MoveTo(newDestinationPath);
                fastaFilePath = newDestinationPath;
            }

            OnExportEnd();

            return fingerprint;
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTables"></param>
        /// <param name="fastaFilePath">Destination file path; will get updated with the final path</param>
        public override string Export(
            DataSet proteinTables,
            ref string fastaFilePath)
        {
            foreach (DataTable proteinTable in proteinTables.Tables)
            {
                WriteFromDataTable(proteinTable, fastaFilePath);
            }

            return FinalizeFile(ref fastaFilePath);
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTable"></param>
        /// <param name="fastaFilePath">Destination file path; will get updated with the final path</param>
        public override string Export(
            DataTable proteinTable,
            ref string fastaFilePath)
        {
            if (proteinTable.Rows.Count > 0)
            {
                WriteFromDataTable(proteinTable, fastaFilePath);
            }
            else
            {
                return FinalizeFile(ref fastaFilePath);
            }

            return fastaFilePath;
        }

        public int WriteFromDataTable(DataTable proteinTable, string fastaFilePath)
        {
            const int requiredSizeMb = 150;

            var proteinsWritten = 0;

            var hexCodeFinder = new Regex(@"[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled);

            var alternateNames = string.Empty;

            var success = DiskInfo.GetDiskFreeSpace(fastaFilePath, out var currentFreeSpaceBytes, out var errorMessage);

            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to append to FASTA file at " + fastaFilePath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(fastaFilePath, requiredSizeMb, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to append to FASTA file at " + fastaFilePath + ". " + errorMessage);
            }

            // Open the output file for append
            // Sometimes the file is locked by an antivirus tool, so try up to four times to instantiate the writer

            for (var iteration = 1; iteration <= 4; iteration++)
            {
                proteinsWritten = 0;

                try
                {
                    using var writer = new StreamWriter(new FileStream(fastaFilePath, FileMode.Append, FileAccess.Write, FileShare.Read));

                    foreach (var currentRow in proteinTable.Select(string.Empty))
                    {
                        var proteinSequence = ExportComponent.SequenceExtender(currentRow["Sequence"].ToString(), proteinTable.Rows.Count);

                        var proteinLength = proteinSequence.Length;
                        var proteinDescription = hexCodeFinder.Replace(currentRow["Description"].ToString(), " ");
                        var proteinName = ExportComponent.ReferenceExtender(currentRow["Name"].ToString());

                        writer.WriteLine((">" + proteinName + " " + proteinDescription + alternateNames).Trim());

                        for (var startIndex = 0; startIndex < proteinLength; startIndex += mSeqLineLength)
                        {
                            var charLength = Math.Min(mSeqLineLength, proteinLength - startIndex);
                            var seqLinePortion = proteinSequence.Substring(startIndex, charLength);
                            writer.WriteLine(seqLinePortion);
                        }

                        proteinsWritten++;
                    }

                    break;
                }
                catch (Exception ex)
                {
                    OnErrorEvent(string.Format("Error opening {0} for append (iteration {1})", fastaFilePath, iteration), ex);

                    var randomGenerator = new Random();
                    var sleepTimeMsec = randomGenerator.Next(3000, 8000);

                    OnDebugEvent("Sleeping for {0:F1} seconds", sleepTimeMsec / 1000.0);

                    AppUtils.SleepMilliseconds(sleepTimeMsec);
                }
            }

            return proteinsWritten;
        }

        /// <summary>
        /// Rename the file to include the fingerprint
        /// </summary>
        /// <param name="fastaFilePath">File path to finalize; will get updated with the new name that includes the fingerprint</param>
        /// <returns>Fingerprint, e.g. 9B916A8B</returns>
        public string FinalizeFile(ref string fastaFilePath)
        {
            var fingerprint = GenerateFileAuthenticationHash(fastaFilePath);

            var sourceFile = new FileInfo(fastaFilePath);

            var newDestinationPath = Path.Combine(
                Path.GetDirectoryName(fastaFilePath) ?? string.Empty,
                fingerprint + Path.GetExtension(fastaFilePath));

            var destinationFile = new FileInfo(newDestinationPath);

            if (sourceFile.Exists)
            {
                if (destinationFile.Exists)
                {
                    destinationFile.Delete();
                }

                sourceFile.MoveTo(newDestinationPath);
                fastaFilePath = newDestinationPath;
            }

            OnExportEnd();

            return fingerprint;
        }
    }
}