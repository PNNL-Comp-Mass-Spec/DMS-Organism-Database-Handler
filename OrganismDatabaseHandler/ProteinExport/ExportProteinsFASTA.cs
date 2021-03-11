using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.ProteinStorage;
using PRISM;
using PRISMWin;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class ExportProteinsFASTA : ExportProteins
    {
        private readonly int m_seqLineLength = 60;

        public ExportProteinsFASTA(GetFASTAFromDMSForward exportComponent)
            : base(exportComponent)
        {
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="Proteins"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Export(
            ProteinStorage.ProteinStorage proteins,
            ref string destinationPath)
        {
            const int REQUIRED_SIZE_MB = 150;

            long currentFreeSpaceBytes;
            string errorMessage = string.Empty;

            bool success = DiskInfo.GetDiskFreeSpace(destinationPath, out currentFreeSpaceBytes, out errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to save FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to save FASTA file at " + destinationPath + ". " + errorMessage);
            }

            using (var writer = new StreamWriter(destinationPath))
            {
                int proteinPosition;
                int proteinLength;

                string tmpSeq;

                string tmpDesc;
                string seqLine;
                ProteinStorageEntry tmpPC;
                string tmpAltNames = string.Empty;

                OnExportStart("Writing to FASTA File");

                int counterMax = proteins.ProteinCount;
                var counter = default(int);
                var hexCodeFinder = new Regex(@"[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled);

                int EventTriggerThresh;
                if (counterMax <= 25)
                {
                    EventTriggerThresh = 1;
                }
                else
                {
                    EventTriggerThresh = (int)Math.Round(counterMax / 25d);
                }

                var nameList = proteins.GetSortedProteinNames();

                foreach (var tmpName in nameList)
                {
                    OnExportStart("Writing: " + tmpName);

                    tmpPC = proteins.GetProtein(tmpName);
                    tmpSeq = tmpPC.Sequence;

                    counter += 1;

                    if (counter % EventTriggerThresh == 0)
                    {
                        OnProgressUpdate("Processing: " + tmpName, Math.Round(counter / (double)counterMax, 3));
                    }

                    proteinLength = tmpSeq.Length;
                    tmpDesc = hexCodeFinder.Replace(tmpPC.Description, " ");

                    writer.WriteLine((">" + tmpPC.Reference + " " + tmpDesc + tmpAltNames).Trim());

                    for (proteinPosition = 1; proteinPosition <= proteinLength; proteinPosition += m_seqLineLength)
                    {
                        seqLine = tmpSeq.Substring(proteinPosition, m_seqLineLength);
                        writer.WriteLine(seqLine);
                    }
                }
            }

            string fingerprint = GenerateFileAuthenticationHash(destinationPath);

            var fi = new FileInfo(destinationPath);

            string newDestinationPath;
            newDestinationPath = Path.Combine(
                Path.GetDirectoryName(destinationPath),
                fingerprint + Path.GetExtension(destinationPath));

            var targetFI = new FileInfo(newDestinationPath);

            if (fi.Exists)
            {
                if (targetFI.Exists)
                {
                    targetFI.Delete();
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
        /// <returns></returns>
        public override string Export(
            DataSet proteinTables,
            ref string destinationPath)
        {
            foreach (DataTable proteinTable in proteinTables.Tables)
                WriteFromDataTable(proteinTable, destinationPath);

            return FinalizeFile(ref destinationPath);
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTable"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        /// <returns></returns>
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
            const int REQUIRED_SIZE_MB = 150;

            int counterMax; // = ProteinTable.Rows.Count;
            var counter = default(int);
            int proteinsWritten = 0;

            var hexCodeFinder = new Regex(@"[\x00-\x1F\x7F-\xFF]", RegexOptions.Compiled);

            string tmpAltNames = string.Empty;
            int EventTriggerThresh;

            long currentFreeSpaceBytes;
            string errorMessage = string.Empty;

            bool success = DiskInfo.GetDiskFreeSpace(destinationPath, out currentFreeSpaceBytes, out errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to append to FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to append to FASTA file at " + destinationPath + ". " + errorMessage);
            }

            // Open the output file for append
            using (var writer = new StreamWriter(new FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.Read)))
            {

                // OnDetailedExportStart("Writing: " + proteinTable.TableName);

                counterMax = proteinTable.Rows.Count;
                if (counterMax <= 25)
                {
                    EventTriggerThresh = 1;
                }
                else
                {
                    EventTriggerThresh = (int)Math.Round(counterMax / 25d);
                }

                var foundRows = proteinTable.Select("");

                foreach (var currentRow in foundRows)
                {
                    string tmpSeq = m_ExportComponent.SequenceExtender(currentRow["Sequence"].ToString(), proteinTable.Rows.Count);

                    counter += 1;

                    if (counter % EventTriggerThresh == 0)
                    {
                        // OnDetailedProgressUpdate("Processing: " + tmpName, Math.Round(counter / (double)counterMax, 3));
                    }

                    int proteinLength = tmpSeq.Length;
                    string tmpDesc = hexCodeFinder.Replace(currentRow["Description"].ToString(), " ");
                    string tmpName = m_ExportComponent.ReferenceExtender(currentRow["Name"].ToString());

                    writer.WriteLine((">" + tmpName + " " + tmpDesc + tmpAltNames).Trim());

                    for (int proteinPosition = 1; proteinPosition <= proteinLength; proteinPosition += m_seqLineLength)
                    {
                        string seqLinePortion = tmpSeq.Substring(proteinPosition, m_seqLineLength);
                        writer.WriteLine(seqLinePortion);
                    }

                    proteinsWritten += 1;
                }
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
            string fingerprint = GenerateFileAuthenticationHash(destinationPath);

            var fi = new FileInfo(destinationPath);

            string newDestinationPath;
            newDestinationPath = Path.Combine(
                Path.GetDirectoryName(destinationPath),
                fingerprint + Path.GetExtension(destinationPath));

            var targetFI = new FileInfo(newDestinationPath);

            if (fi.Exists)
            {
                if (targetFI.Exists)
                {
                    targetFI.Delete();
                }

                fi.MoveTo(newDestinationPath);
                destinationPath = newDestinationPath;
            }

            OnExportEnd();

            return fingerprint;
        }
    }
}