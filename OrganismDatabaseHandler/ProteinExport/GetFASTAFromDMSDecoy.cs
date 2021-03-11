using System.Collections.Generic;
using System.IO;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSDecoy : GetFASTAFromDMSForward
    {
        protected GetFASTAFromDMSReversed m_RevGenerator;

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
            m_RevGenerator = new GetFASTAFromDMSReversed(
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
            string fwdFilePath;
            string revFilePath;

            base.ExportFASTAFile(protCollectionList,
                                 destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation);

            fwdFilePath = FullOutputPath;

            m_RevGenerator.ExportFASTAFile(protCollectionList,
                                           destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation);

            revFilePath = m_RevGenerator.FullOutputPath;

            var fwdFI = new FileInfo(fwdFilePath);
            var revFI = new FileInfo(revFilePath);

            using (var reverseReader = new StreamReader(new FileStream(revFI.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            using (var appender = new StreamWriter(new FileStream(fwdFI.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                while (!reverseReader.EndOfStream)
                {
                    string dataLine = reverseReader.ReadLine();
                    appender.WriteLine(dataLine);
                }
            }

            revFI.Delete();

            string crc32HashFinal = GetFileHash(fwdFI.FullName);

            return crc32HashFinal;
        }
    }
}