using Microsoft.VisualBasic;
using TableManipulationBase;

namespace Protein_Exporter
{
    public class GetFASTAFromDMSReversed : GetFASTAFromDMSForward
    {
        protected bool m_UseXXX;

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
            get
            {
                return m_UseXXX;
            }

            set
            {
                m_UseXXX = true;
            }
        }

        public override string SequenceExtender(string originalSequence, int collectionCount)
        {
            return Strings.StrReverse(originalSequence);
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