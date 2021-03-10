using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using PRISM;
using PRISMWin;
using Protein_Storage;

namespace Protein_Exporter
{
    public class ExportProteinsXTFASTA : ExportProteins
    {
        public ExportProteinsXTFASTA(GetFASTAFromDMSForward exportComponent)
            : base(exportComponent)
        {
        }

        private const string HEADER_STRING = "xbang-pro-fasta-format";

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        /// <returns></returns>
        public override string Export(
            ProteinStorage proteins,
            ref string destinationPath)
        {
            const int REQUIRED_SIZE_MB = 150;

            var buffer = Encoding.Default.GetBytes(HEADER_STRING);

            Array.Resize(ref buffer, 256);

            long currentFreeSpaceBytes;
            string errorMessage = string.Empty;

            bool success = DiskInfo.GetDiskFreeSpace(destinationPath, out currentFreeSpaceBytes, out errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            using (var bw = new BinaryWriter(File.OpenWrite(destinationPath)))
            {
                bw.BaseStream.Seek(0L, SeekOrigin.Begin);

                int proteinLength;

                string tmpSeq;

                ProteinStorageEntry tmpPC;
                int tmpNum;

                OnExportStart("Writing to X!Tandem formatted FASTA File");

                int counterMax = proteins.ProteinCount;
                var counter = default(int);

                var proteinArray = new SortedSet<string>();

                var proteinEnum = proteins.GetEnumerator();

                while (proteinEnum.MoveNext())
                    proteinArray.Add(proteinEnum.Current.Key);

                var encoding = new ASCIIEncoding();

                int EventTriggerThresh;
                if (counterMax <= 25)
                {
                    EventTriggerThresh = 1;
                }
                else
                {
                    EventTriggerThresh = (int)Math.Round(counterMax / 25d);
                }

                bw.Write(buffer);

                foreach (var tmpName in proteinArray)
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

                    Array.Clear(buffer, 0, 4);
                    tmpNum = tmpName.Length + 1;
                    buffer = ConvIntegerToByteArray(tmpNum, 4);
                    Array.Reverse(buffer);

                    bw.Write(buffer);
                    buffer = encoding.GetBytes(tmpName);
                    bw.Write(buffer);
                    bw.Write(ConvIntegerToByteArray(0L, 1));

                    Array.Clear(buffer, 0, 4);
                    tmpNum = proteinLength + 1;
                    buffer = ConvIntegerToByteArray(tmpNum, 4);
                    Array.Reverse(buffer);

                    bw.Write(buffer);
                    buffer = encoding.GetBytes(tmpSeq);
                    bw.Write(buffer);
                    bw.Write(ConvIntegerToByteArray(0L, 1));
                }
            }

            string fingerprint = GenerateFileAuthenticationHash(destinationPath);

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
            const int REQUIRED_SIZE_MB = 150;

            var buffer = Encoding.Default.GetBytes(HEADER_STRING);

            Array.Resize(ref buffer, 256);

            long currentFreeSpaceBytes;
            string errorMessage = string.Empty;

            bool success = DiskInfo.GetDiskFreeSpace(destinationPath, out currentFreeSpaceBytes, out errorMessage);
            if (!success)
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            if (!FileTools.ValidateFreeDiskSpace(destinationPath, REQUIRED_SIZE_MB, currentFreeSpaceBytes, out errorMessage))
            {
                if (string.IsNullOrEmpty(errorMessage))
                    errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message";
                throw new IOException("Unable to create FASTA file at " + destinationPath + ". " + errorMessage);
            }

            using (var bw = new BinaryWriter(File.OpenWrite(destinationPath)))
            {
                bw.BaseStream.Seek(0L, SeekOrigin.Begin);

                // IEnumerator e = Proteins.GetEnumerator;

                int proteinLength;

                DataRow[] foundRows;

                string tmpSeq;
                string tmpName;

                int tmpNum;

                OnExportStart("Writing to X!Tandem formatted FASTA File");

                // int counterMax = Proteins.ProteinCount;
                int counterMax; // = ProteinTable.Rows.Count;
                var counter = default(int);

                foreach (DataTable proteinTable in proteinTables.Tables)
                {
                    OnExportStart("Writing: " + proteinTable.TableName);
                    counterMax = proteinTable.Rows.Count;
                    foundRows = proteinTable.Select("", "Name");

                    var encoding = new ASCIIEncoding();

                    int EventTriggerThresh;
                    if (counterMax <= 25)
                    {
                        EventTriggerThresh = 1;
                    }
                    else
                    {
                        EventTriggerThresh = (int)Math.Round(counterMax / 25d);
                    }

                    bw.Write(buffer);

                    foreach (var dr in foundRows)
                    {
                        // tmpPC = Proteins.GetProtein(tmpName);
                        // tmpSeq = tmpPC.Sequence;
                        tmpSeq = dr["Sequence"].ToString();
                        tmpName = dr["Name"].ToString();
                        // tmpDesc = dr.Item("Description").ToString();

                        counter += 1;

                        if (counter % EventTriggerThresh == 0)
                        {
                            OnProgressUpdate("Processing: " + tmpName, Math.Round(counter / (double)counterMax, 3));
                        }

                        proteinLength = tmpSeq.Length;

                        Array.Clear(buffer, 0, 4);
                        tmpNum = tmpName.Length + 1;
                        buffer = ConvIntegerToByteArray(tmpNum, 4);
                        Array.Reverse(buffer);

                        bw.Write(buffer);
                        buffer = encoding.GetBytes(tmpName);
                        bw.Write(buffer);
                        bw.Write(ConvIntegerToByteArray(0L, 1));

                        Array.Clear(buffer, 0, 4);
                        tmpNum = proteinLength + 1;
                        buffer = ConvIntegerToByteArray(tmpNum, 4);
                        Array.Reverse(buffer);

                        bw.Write(buffer);
                        buffer = encoding.GetBytes(tmpSeq);
                        bw.Write(buffer);
                        bw.Write(ConvIntegerToByteArray(0L, 1));
                    }
                }
            }

            string fingerprint = GenerateFileAuthenticationHash(destinationPath);

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
            int i, k;
            string h;
            h = Conversion.Hex(n).PadLeft(16, '0');
            k = 16;
            for (i = lg - 1; i >= 0; i -= 1)
            {
                k = k - 2;
                m[i] = Conversions.ToByte("&H" + h.Substring(k, 2));
            }

            return m;
        }

        public long ConvByteArrayToInteger(byte[] b, int ln = 0, int sidx = 0)
        {
            int i;
            long j, k;
            if (ln == 0)
                ln = Information.UBound(b) + 1;
            ln = sidx + ln - 1;
            k = 1L;
            j = b[ln];
            for (i = ln - 1; i >= sidx; i -= 1)
            {
                k = 256L * k;
                j = j + k * b[i];
            }

            return j;
        }
    }
}