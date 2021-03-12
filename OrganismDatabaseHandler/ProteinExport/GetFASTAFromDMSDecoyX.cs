using System;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSDecoyX : GetFASTAFromDMSDecoy
    {
        // Ignore Spelling: fastapro

        private const bool DecoyProteinsUseXXX = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        [Obsolete("Use the constructor that does not take databaseFormatType")]
        public GetFASTAFromDMSDecoyX(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
            : this(databaseAccessor)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        public GetFASTAFromDMSDecoyX(DBTask databaseAccessor)
            : base(databaseAccessor, DecoyProteinsUseXXX)
        {
            RevGenerator = new GetFASTAFromDMSReversed(databaseAccessor)
            {
                UseXXX = DecoyProteinsUseXXX
            };
        }
    }
}