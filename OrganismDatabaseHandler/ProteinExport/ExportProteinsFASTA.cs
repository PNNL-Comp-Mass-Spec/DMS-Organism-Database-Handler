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
        private readonly int mSeqLineLength = 60;

        public ExportProteinsFASTA(GetFASTAFromDMSForward exportComponent)
            : base(exportComponent)
        {
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        public override string Export(
            ProteinStorage.ProteinStorage proteins,
            ref string destinationPath)
        {
            const int requiredSizeMb = 150;

            var success = DiskInfo.GetDiskFreeSpace(destinationPath, out var currentFreeSpaceBytes, out var errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to save FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, requiredSizeMb, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to save FASTA file at " + destinationPath + ". " + errorMessage);
            }

            using (var writer = new StreamWriter(destinationPath))
            {
                var alternateNames = string.Empty;

                OnExportStart("Writing to FASTA File");

                var counterMax = proteins.ProteinCount;
                var counter = default(int);
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

            var fingerprint = GenerateFileAuthenticationHash(destinationPath);

            var fi = new FileInfo(destinationPath);

            var newDestinationPath = Path.Combine(
                Path.GetDirectoryName(destinationPath),
                fingerprint + Path.GetExtension(destinationPath));

            var targetFi = new FileInfo(newDestinationPath);

            if (fi.Exists)
            {
                if (targetFi.Exists)
                {
                    targetFi.Delete();
                }

                fi.MoveTo(newDestinationPath);
                destinationPath = newDestinationPath;
            }

            OnExportEnd();

            return fingerprint;
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTables"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        public override string Export(
            DataSet proteinTables,
            ref string destinationPath)
        {
            foreach (DataTable proteinTable in proteinTables.Tables)
            {
                WriteFromDataTable(proteinTable, destinationPath);
            }

            return FinalizeFile(ref destinationPath);
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTable"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        public override string Export(
            DataTable proteinTable,
            ref string destinationPath)
        {
            if (proteinTable.Rows.Count > 0)
            {
                WriteFromDataTable(proteinTable, destinationPath);
            }
            else
            {
                return FinalizeFile(ref destinationPath);
            }

            return destinationPath;
        }

        public int WriteFromDataTable(DataTable proteinTable, string destinationPath)
        {
            const int requiredSizeMb = 150;

            var counter = default(int);
            var proteinsWritten = 0;

            var hexCodeFinder = new Regex(@"[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled);

            var alternateNames = string.Empty;

            var success = DiskInfo.GetDiskFreeSpace(destinationPath, out var currentFreeSpaceBytes, out var errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to append to FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, requiredSizeMb, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to append to FASTA file at " + destinationPath + ". " + errorMessage);
            }

            // Open the output file for append
            using var writer = new StreamWriter(new FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.Read));

            // OnDetailedExportStart("Writing: " + proteinTable.TableName);

            var counterMax = proteinTable.Rows.Count;
            int eventTriggerThresh;
            if (counterMax <= 25)
            {
                eventTriggerThresh = 1;
            }
            else
            {
                eventTriggerThresh = (int)Math.Round(counterMax / 25d);
            }

            var foundRows = proteinTable.Select("");

            foreach (var currentRow in foundRows)
            {
                var proteinSequence = ExportComponent.SequenceExtender(currentRow["Sequence"].ToString(), proteinTable.Rows.Count);

                counter++;

                if (counter % eventTriggerThresh == 0)
                {
                    // OnDetailedProgressUpdate("Processing: " + tmpName, Math.Round(counter / (double)counterMax, 3));
                }
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

            return proteinsWritten;
        }

        /// <summary>
        /// Rename the file to include the fingerprint
        /// </summary>
        /// <param name="destinationPath">File path to finalize; will get updated with the new name that includes the fingerprint</param>
        /// <returns>Fingerprint, e.g. 9B916A8B</returns>
        public string FinalizeFile(ref string destinationPath)
        {
            var fingerprint = GenerateFileAuthenticationHash(destinationPath);

            var fi = new FileInfo(destinationPath);

            var newDestinationPath = Path.Combine(
                Path.GetDirectoryName(destinationPath),
                fingerprint + Path.GetExtension(destinationPath));

            var targetFi = new FileInfo(newDestinationPath);

            if (fi.Exists)
            {
                if (targetFi.Exists)
                {
                    targetFi.Delete();
                }

                fi.MoveTo(newDestinationPath);
                destinationPath = newDestinationPath;
            }

            OnExportEnd();

            return fingerprint;
        }
    }
}