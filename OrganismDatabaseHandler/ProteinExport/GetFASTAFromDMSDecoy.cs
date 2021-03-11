using System.Collections.Generic;
using System.IO;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSDecoy : GetFASTAFromDMSForward
    {
        protected GetFASTAFromDMSReversed RevGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        public GetFASTAFromDMSDecoy(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType,
            bool decoyUsesXXX)
            : base(databaseAccessor, databaseFormatType)
        {
            RevGenerator = new GetFASTAFromDMSReversed(
                databaseAccessor, databaseFormatType) { UseXXX = decoyUsesXXX };
        }

        /// <summary>
        /// Create the decoy FASTA file for the given protein collections
        /// </summary>
        /// <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
        /// <param name="destinationFolderPath"></param>
        /// <returns>CRC32 hash of the generated (or retrieved) file</returns>
        public override string ExportFASTAFile(
            List<string> protCollectionList,
            string destinationFolderPath,
            int alternateAnnotationTypeID,
            bool padWithPrimaryAnnotation)
        {
            base.ExportFASTAFile(protCollectionList,
                                 destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation);

            var fwdFilePath = FullOutputPath;

            RevGenerator.ExportFASTAFile(protCollectionList,
                                           destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation);

            var revFilePath = RevGenerator.FullOutputPath;

            var fwdFi = new FileInfo(fwdFilePath);
            var revFi = new FileInfo(revFilePath);

            using (var reverseReader = new StreamReader(new FileStream(revFi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            using (var appender = new StreamWriter(new FileStream(fwdFi.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                while (!reverseReader.EndOfStream)
                {
                    string dataLine = reverseReader.ReadLine();
                    appender.WriteLine(dataLine);
                }
            }

            revFi.Delete();

            string crc32HashFinal = GetFileHash(fwdFi.FullName);

            return crc32HashFinal;
        }
    }
}