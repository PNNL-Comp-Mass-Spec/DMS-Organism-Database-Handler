using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using PRISM;
using PRISMWin;

namespace OrganismDatabaseHandler.ProteinExport
{
    [Obsolete("Unused")]
    public class ExportProteinsXTFASTA : ExportProteins
    {
        public ExportProteinsXTFASTA(GetFASTAFromDMSForward exportComponent)
            : base(exportComponent)
        {
        }

        private const string HeaderString = "xbang-pro-fasta-format";

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        /// <returns></returns>
        public override string Export(
            ProteinStorage.ProteinStorage proteins,
            ref string destinationPath)
        {
            const int requiredSizeMb = 150;

            var buffer = Encoding.Default.GetBytes(HeaderString);

            Array.Resize(ref buffer, 256);

            var success = DiskInfo.GetDiskFreeSpace(destinationPath, out var currentFreeSpaceBytes, out var errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, requiredSizeMb, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            using (var writer = new BinaryWriter(File.OpenWrite(destinationPath)))
            {
                writer.BaseStream.Seek(0L, SeekOrigin.Begin);

                OnExportStart("Writing to X!Tandem formatted FASTA File");

                var counterMax = proteins.ProteinCount;
                var counter = default(int);

                var proteinArray = new SortedSet<string>();

                foreach (var protein in proteins)
                {
                    proteinArray.Add(protein.Key);
                }

                var encoding = new ASCIIEncoding();

                int eventTriggerThresh;
                if (counterMax <= 25)
                {
                    eventTriggerThresh = 1;
                }
                else
                {
                    eventTriggerThresh = (int)Math.Round(counterMax / 25d);
                }

                writer.Write(buffer);

                foreach (var tmpName in proteinArray)
                {
                    OnExportStart("Writing: " + tmpName);
                    var tmpPc = proteins.GetProtein(tmpName);
                    var tmpSeq = tmpPc.Sequence;

                    counter++;

                    if (counter % eventTriggerThresh == 0)
                    {
                        OnProgressUpdate("Processing: " + tmpName, Math.Round(counter / (double)counterMax, 3));
                    }

                    var proteinLength = tmpSeq.Length;

                    Array.Clear(buffer, 0, 4);
                    var tmpNum = tmpName.Length + 1;
                    buffer = ConvIntegerToByteArray(tmpNum, 4);
                    Array.Reverse(buffer);

                    writer.Write(buffer);
                    buffer = encoding.GetBytes(tmpName);
                    writer.Write(buffer);
                    writer.Write(ConvIntegerToByteArray(0L, 1));

                    Array.Clear(buffer, 0, 4);
                    tmpNum = proteinLength + 1;
                    buffer = ConvIntegerToByteArray(tmpNum, 4);
                    Array.Reverse(buffer);

                    writer.Write(buffer);
                    buffer = encoding.GetBytes(tmpSeq);
                    writer.Write(buffer);
                    writer.Write(ConvIntegerToByteArray(0L, 1));
                }
            }

            var fingerprint = GenerateFileAuthenticationHash(destinationPath);

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
            const int requiredSizeMb = 150;

            var buffer = Encoding.Default.GetBytes(HeaderString);

            Array.Resize(ref buffer, 256);

            var success = DiskInfo.GetDiskFreeSpace(destinationPath, out var currentFreeSpaceBytes, out var errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, requiredSizeMb, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            using (var writer = new BinaryWriter(File.OpenWrite(destinationPath)))
            {
                writer.BaseStream.Seek(0L, SeekOrigin.Begin);

                // IEnumerator e = Proteins.GetEnumerator;

                OnExportStart("Writing to X!Tandem formatted FASTA File");

                // int counterMax = Proteins.ProteinCount;
                var counter = default(int);

                foreach (DataTable proteinTable in proteinTables.Tables)
                {
                    OnExportStart("Writing: " + proteinTable.TableName);
                    var counterMax = proteinTable.Rows.Count; // = ProteinTable.Rows.Count;
                    var foundRows = proteinTable.Select("", "Name");

                    var encoding = new ASCIIEncoding();

                    int eventTriggerThresh;
                    if (counterMax <= 25)
                    {
                        eventTriggerThresh = 1;
                    }
                    else
                    {
                        eventTriggerThresh = (int)Math.Round(counterMax / 25d);
                    }

                    writer.Write(buffer);

                    foreach (var dr in foundRows)
                    {
                        // tmpPC = Proteins.GetProtein(tmpName);
                        // tmpSeq = tmpPC.Sequence;
                        var tmpSeq = dr["Sequence"].ToString();
                        var tmpName = dr["Name"].ToString();
                        // tmpDesc = dr.Item("Description").ToString();

                        counter++;

                        if (counter % eventTriggerThresh == 0)
                        {
                            OnProgressUpdate("Processing: " + tmpName, Math.Round(counter / (double)counterMax, 3));
                        }

                        var proteinLength = tmpSeq.Length;

                        Array.Clear(buffer, 0, 4);
                        var tmpNum = tmpName.Length + 1;
                        buffer = ConvIntegerToByteArray(tmpNum, 4);
                        Array.Reverse(buffer);

                        writer.Write(buffer);
                        buffer = encoding.GetBytes(tmpName);
                        writer.Write(buffer);
                        writer.Write(ConvIntegerToByteArray(0L, 1));

                        Array.Clear(buffer, 0, 4);
                        tmpNum = proteinLength + 1;
                        buffer = ConvIntegerToByteArray(tmpNum, 4);
                        Array.Reverse(buffer);

                        writer.Write(buffer);
                        buffer = encoding.GetBytes(tmpSeq);
                        writer.Write(buffer);
                        writer.Write(ConvIntegerToByteArray(0L, 1));
                    }
                }
            }

            var fingerprint = GenerateFileAuthenticationHash(destinationPath);

            OnExportEnd();

            return fingerprint;
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
            // Not implemented for this class
            return string.Empty;
        }

        internal byte[] ConvIntegerToByteArray(long n, int lg)
        {
            // converts an integer to a byte array of length lg
            var m = new byte[lg];
            var h = n.ToString("X").PadLeft(16, '0');
            var k = 16;
            for (var i = lg - 1; i >= 0; --i)
            {
                k -= 2;
                m[i] = Convert.ToByte("&H" + h.Substring(k, 2));
            }

            return m;
        }

        [Obsolete("Unused")]
        public long ConvByteArrayToInteger(byte[] b, int ln = 0, int sidx = 0)
        {
            if (ln == 0)
                ln = b.Length + 1;
            ln = sidx + ln - 1;
            var k = 1L;
            long j = b[ln];
            for (var i = ln - 1; i >= sidx; --i)
            {
                k = 256L * k;
                j += k * b[i];
            }

            return j;
        }
    }
}