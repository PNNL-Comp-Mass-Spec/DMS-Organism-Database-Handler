using System;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSReversed : GetFASTAFromDMSForward
    {
        // Ignore Spelling: fastapro

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        public GetFASTAFromDMSReversed(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
            : base(databaseAccessor, databaseFormatType)
        {
            NamingSuffix = "_reversed";
        }

        /// <summary>
        /// When true, reverse proteins start with XXX_
        /// When false, they start with REV_
        /// </summary>
        /// <returns></returns>
        public bool UseXXX { get; set; } = true;

        public override string SequenceExtender(string originalSequence, int collectionCount)
        {
            // Note: Not safe for some Unicode characters, but those probably should exist in a protein sequence anyway.
            var charArray = originalSequence.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public override string ReferenceExtender(string originalReference)
        {
            if (UseXXX)
            {
                return "XXX_" + originalReference;
            }
            else
            {
                return "REV_" + originalReference;
            }
        }
    }
}