using System.Collections.Generic;

namespace OrganismDatabaseHandler.ProteinStorage
{
    public class ProteinStorageDMS : ProteinStorage
    {
        private readonly Dictionary<int, SortedSet<string>> mUniqueProteinIDList;        // Protein_ID, Protein_Name

        public ProteinStorageDMS(string fastaFileName)
            : base(fastaFileName)
        {
            mUniqueProteinIDList = new Dictionary<int, SortedSet<string>>();
        }

        public override void AddProtein(ProteinStorageEntry proteinEntry)
        {
            int proteinEntryID = proteinEntry.Protein_ID;
            string proteinEntryName = proteinEntry.Reference;

            SortedSet<string> nameList = null;

            if (!mUniqueProteinIDList.TryGetValue(proteinEntryID, out nameList))
            {
                nameList = new SortedSet<string>();
                nameList.Add(proteinEntryName);
                mUniqueProteinIDList.Add(proteinEntryID, nameList);
                mResidueCount += proteinEntry.Sequence.Length;
            }
            else
            {
                foreach (var proteinName in nameList)
                {
                    var existingEntry = mProteins[proteinName];

                    if (!proteinEntry.Reference.Equals(existingEntry.Reference))
                    {
                        existingEntry.AddXRef(proteinEntryName);
                        proteinEntry.AddXRef(existingEntry.Reference);
                    }
                }

                nameList.Add(proteinEntryName);
                mUniqueProteinIDList[proteinEntryID] = nameList;
            }

            if (!mProteinNames.Contains(proteinEntryName))
            {
                mProteins.Add(proteinEntryName, proteinEntry);
                mProteinNames.Add(proteinEntryName);
                mResidueCount += proteinEntry.Sequence.Length;
            }
        }

        public override void ClearProteinEntries()
        {
            base.ClearProteinEntries();
            mUniqueProteinIDList.Clear();
        }
    }
}