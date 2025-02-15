﻿using System.Collections.Generic;

namespace OrganismDatabaseHandler.ProteinStorage
{
    public class ProteinStorageDMS : ProteinStorage
    {
        // Ignore Spelling: fasta

        private readonly Dictionary<int, SortedSet<string>> mUniqueProteinIdList;        // Protein_ID, Protein_Name

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fastaFileName"></param>
        public ProteinStorageDMS(string fastaFileName)
            : base(fastaFileName)
        {
            mUniqueProteinIdList = new Dictionary<int, SortedSet<string>>();
        }

        public override void AddProtein(ProteinStorageEntry proteinEntry)
        {
            var proteinEntryId = proteinEntry.ProteinId;
            var proteinEntryName = proteinEntry.Reference;

            if (!mUniqueProteinIdList.TryGetValue(proteinEntryId, out var nameList))
            {
                nameList = new SortedSet<string> {proteinEntryName};
                mUniqueProteinIdList.Add(proteinEntryId, nameList);
                ResidueCount += proteinEntry.Sequence.Length;
            }
            else
            {
                foreach (var proteinName in nameList)
                {
                    var existingEntry = Proteins[proteinName];

                    if (!proteinEntry.Reference.Equals(existingEntry.Reference))
                    {
                        existingEntry.AddXRef(proteinEntryName);
                        proteinEntry.AddXRef(existingEntry.Reference);
                    }
                }

                nameList.Add(proteinEntryName);
                mUniqueProteinIdList[proteinEntryId] = nameList;
            }

            if (!ProteinNames.Contains(proteinEntryName))
            {
                Proteins.Add(proteinEntryName, proteinEntry);
                ProteinNames.Add(proteinEntryName);
                ResidueCount += proteinEntry.Sequence.Length;
            }
        }

        public override void ClearProteinEntries()
        {
            base.ClearProteinEntries();
            mUniqueProteinIdList.Clear();
        }
    }
}