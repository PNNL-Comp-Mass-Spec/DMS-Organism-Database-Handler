﻿using System;
using System.Diagnostics;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class GetFASTAFromDMSScrambled : GetFASTAFromDMSForward
    {
        private Random mRndNumGen;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
        /// <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
        public GetFASTAFromDMSScrambled(
            DBTask databaseAccessor,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
            : base(databaseAccessor, databaseFormatType)
        {
        }

        public override string SequenceExtender(string originalSequence, int collectionCount)
        {
            var sb = new StringBuilder(originalSequence.Length);
            string sequence = originalSequence;

            if (mRndNumGen == null)
            {
                mRndNumGen = new Random(collectionCount);
                mNaming_Suffix = "_scrambled_seed_" + collectionCount.ToString();
            }

            var counter = sequence.Length;

            while (counter > 0)
            {
                Debug.Assert(counter == sequence.Length);
                var index = mRndNumGen.Next(counter);
                sb.Append(sequence.Substring(index, 1));

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

                counter -= 1;
            }

            return sb.ToString();
        }

        public override string ReferenceExtender(string originalReference)
        {
            return "Scrambled_" + originalReference;
        }
    }
}