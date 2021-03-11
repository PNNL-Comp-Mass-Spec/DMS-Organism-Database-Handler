using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSDecoyX : GetFASTAFromDMSDecoy
    {
        private const bool DECOY_PROTEINS_USE_XXX = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        public GetFASTAFromDMSDecoyX(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
            : base(databaseAccessor, databaseFormatType, DECOY_PROTEINS_USE_XXX)
        {
            m_RevGenerator = new GetFASTAFromDMSReversed(
                databaseAccessor, databaseFormatType)
            {
                UseXXX = DECOY_PROTEINS_USE_XXX
            };
        }
    }
}