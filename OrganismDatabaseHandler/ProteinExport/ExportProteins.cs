﻿using System.Collections.Generic;
using System.Data;
using System.IO;

namespace OrganismDatabaseHandler.ProteinExport
{
    public abstract class ExportProteins : PRISM.EventNotifier
    {
        protected GetFASTAFromDMSForward ExportComponent;

        protected ExportProteins(GetFASTAFromDMSForward exportComponent)
        {
            ExportComponent = exportComponent;
        }

        public event ExportStartEventHandler ExportStart;
        public event ExportProgressEventHandler ExportProgress;
        public event ExportEndEventHandler ExportEnd;

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        /// <param name="selectedProteinList"></param>
        protected string Export(
            ProteinStorage.ProteinStorage proteins,
            ref string destinationPath,
            List<string> selectedProteinList)
        {
            var newStorage = new ProteinStorage.ProteinStorage(Path.GetFileNameWithoutExtension(destinationPath));

            foreach (var reference in selectedProteinList)
            {
                newStorage.AddProtein(proteins.GetProtein(reference));
            }

            return Export(newStorage, ref destinationPath);
        }

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteins"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        public abstract string Export(
            ProteinStorage.ProteinStorage proteins,
            ref string destinationPath);

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTables"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        public abstract string Export(
            DataSet proteinTables,
            ref string destinationPath);

        /// <summary>
        /// Export the proteins to the given file
        /// </summary>
        /// <param name="proteinTable"></param>
        /// <param name="destinationPath">Destination file path; will get updated with the final path</param>
        public abstract string Export(
            DataTable proteinTable,
            ref string destinationPath);

        protected void OnExportStart(string taskTitle)
        {
            ExportStart?.Invoke(taskTitle);
        }

        protected void OnProgressUpdate(string statusMsg, double fractionDone)
        {
            ExportProgress?.Invoke(statusMsg, fractionDone);
        }

        protected void OnExportEnd()
        {
            ExportEnd?.Invoke();
        }

        // Unused
        // public string GetHashCRC32(string fullFilePath)
        // {
        //     return GetFileHash(fullFilePath);
        // }

        // Unused
        // public string GetHashMD5(string fullFilePath)
        // {
        //     return GetFileHashMD5(fullFilePath);
        // }

        /// <summary>
        /// Compute the CRC32 hash for the file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns>File hash</returns>
        protected string GetFileHash(string fullFilePath)
        {
            return GenerateFileAuthenticationHash(fullFilePath);
        }

        /// <summary>
        /// Compute the CRC32 hash for the file
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns>File hash</returns>
        public string GenerateFileAuthenticationHash(string fullFilePath)
        {
            var fi = new FileInfo(fullFilePath);

            if (!fi.Exists)
                return string.Empty;

            using var f = fi.OpenRead();

            var crc = PRISM.Crc32.Crc(f);

            return string.Format("{0:X8}", crc);
        }
    }
}