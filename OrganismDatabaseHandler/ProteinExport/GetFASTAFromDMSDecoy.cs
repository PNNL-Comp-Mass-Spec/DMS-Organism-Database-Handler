using System.Collections.Generic;
using System.IO;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSDecoy : GetFASTAFromDMSForward
    {
        // Ignore Spelling: fastapro

        protected GetFASTAFromDMSReversed RevGenerator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="decoyUsesXXX"></param>
        public GetFASTAFromDMSDecoy(
            DBTask databaseAccessor,
            bool decoyUsesXXX)
            : base(databaseAccessor)
        {
            RevGenerator = new GetFASTAFromDMSReversed(databaseAccessor) { UseXXX = decoyUsesXXX };
        }

        /// <summary>
        /// Create the decoy FASTA file for the given protein collections
        /// </summary>
        /// <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
        /// <param name="destinationFolderPath"></param>
        /// <param name="alternateAnnotationTypeId"></param>
        /// <param name="padWithPrimaryAnnotation"></param>
        /// <returns>CRC32 hash of the generated (or retrieved) file</returns>
        public override string ExportFASTAFile(
            List<string> protCollectionList,
            string destinationFolderPath,
            int alternateAnnotationTypeId,
            bool padWithPrimaryAnnotation)
        {
            base.ExportFASTAFile(protCollectionList,
                                 destinationFolderPath, alternateAnnotationTypeId, padWithPrimaryAnnotation);

            var fwdFilePath = FullOutputPath;

            RevGenerator.ExportFASTAFile(protCollectionList,
                                           destinationFolderPath, alternateAnnotationTypeId, padWithPrimaryAnnotation);

            var revFilePath = RevGenerator.FullOutputPath;

            var forwardFile = new FileInfo(fwdFilePath);
            var reverseFile = new FileInfo(revFilePath);

            using (var reverseReader = new StreamReader(new FileStream(reverseFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            using (var appender = new StreamWriter(new FileStream(forwardFile.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
            {
                while (!reverseReader.EndOfStream)
                {
                    var dataLine = reverseReader.ReadLine();
                    appender.WriteLine(dataLine);
                }
            }

            reverseFile.Delete();

            return GetFileHash(forwardFile.FullName);
        }
    }
}