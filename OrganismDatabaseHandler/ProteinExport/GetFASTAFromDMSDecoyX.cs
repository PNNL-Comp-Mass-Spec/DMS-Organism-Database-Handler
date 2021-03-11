using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSDecoyX : GetFASTAFromDMSDecoy
    {
        private const bool DecoyProteinsUseXXX = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        public GetFASTAFromDMSDecoyX(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
            : base(databaseAccessor, databaseFormatType, DecoyProteinsUseXXX)
        {
            RevGenerator = new GetFASTAFromDMSReversed(
                databaseAccessor, databaseFormatType)
            {
                UseXXX = DecoyProteinsUseXXX
            };
        }
    }
}