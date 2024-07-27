using System;
using System.Diagnostics;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSScrambled : GetFASTAFromDMSForward
    {
        // Ignore Spelling: accessor, fastapro

        private Random mRndNumGen;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        public GetFASTAFromDMSScrambled(DBTask databaseAccessor)
            : base(databaseAccessor)
        {
        }

        public override string SequenceExtender(string originalSequence, int collectionCount)
        {
            var sb = new StringBuilder(originalSequence.Length);
            var sequence = originalSequence;

            if (mRndNumGen == null)
            {
                mRndNumGen = new Random(collectionCount);
                NamingSuffix = "_scrambled_seed_" + collectionCount;
            }

            var counter = sequence.Length;

            while (counter > 0)
            {
                Debug.Assert(counter == sequence.Length);
                var index = mRndNumGen.Next(counter);
                sb.Append(sequence, index, 1);

                if (index > 0)
                {
                    if (index < sequence.Length - 1)
                    {
                        sequence = sequence.Substring(0, index) + sequence.Substring(index + 1);
                    }
                    else
                    {
                        sequence = sequence.Substring(0, index);
                    }
                }
                else
                {
                    sequence = sequence.Substring(index + 1);
                }

                counter--;
            }

            return sb.ToString();
        }

        public override string ReferenceExtender(string originalReference)
        {
            return "Scrambled_" + originalReference;
        }
    }
}