using System.Collections.Generic;

namespace OrganismDatabaseHandler.ProteinStorage
{
    public class ProteinStorageDMS : ProteinStorage
    {
        private readonly Dictionary<int, SortedSet<string>> mUniqueProteinIdList;        // Protein_ID, Protein_Name

        public ProteinStorageDMS(string fastaFileName)
            : base(fastaFileName)
        {
            mUniqueProteinIdList = new Dictionary<int, SortedSet<string>>();
        }

        public override void AddProtein(ProteinStorageEntry proteinEntry)
        {
            var proteinEntryId = proteinEntry.ProteinId;
            var proteinEntryName = proteinEntry.Reference;

            SortedSet<string> nameList = null;

            if (!mUniqueProteinIdList.TryGetValue(proteinEntryId, out nameList))
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