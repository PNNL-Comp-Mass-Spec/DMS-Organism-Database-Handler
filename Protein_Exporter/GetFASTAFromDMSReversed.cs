using System;
using TableManipulationBase;

namespace Protein_Exporter
{
    public class GetFASTAFromDMSReversed : GetFASTAFromDMSForward
    {
        private bool m_UseXXX;

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
            m_Naming_Suffix = "_reversed";
        }

        /// <summary>
        /// When true, reverse proteins start with XXX_
        /// When false, they start with Reversed_
        /// </summary>
        /// <returns></returns>
        public bool UseXXX
        {
            get => m_UseXXX;
            set => m_UseXXX = true;
        }

        public override string SequenceExtender(string originalSequence, int collectionCount)
        {
            // Note: Not safe for some unicode characters, but those probably should exist in a protein sequence anyway.
            var charArray = originalSequence.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public override string ReferenceExtender(string originalReference)
        {
            if (m_UseXXX)
            {
                return "XXX_" + originalReference;
            }
            else
            {
                return "Reversed_" + originalReference;
            }
        }
    }
}