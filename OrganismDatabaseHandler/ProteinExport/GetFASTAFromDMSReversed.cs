using System;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSReversed : GetFASTAFromDMSForward
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        public GetFASTAFromDMSReversed(DBTask databaseAccessor)
            : base(databaseAccessor)
        {
            NamingSuffix = "_reversed";
        }

        /// <summary>
        /// When true, reverse proteins start with XXX_
        /// When false, they start with REV_
        /// </summary>
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