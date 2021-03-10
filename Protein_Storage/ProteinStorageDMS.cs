using System.Collections.Generic;

namespace Protein_Storage
{
    public class ProteinStorageDMS : ProteinStorage
    {
        private readonly Dictionary<int, SortedSet<string>> m_UniqueProteinIDList;        // Protein_ID, Protein_Name

        public ProteinStorageDMS(string fastaFileName)
            : base(fastaFileName)
        {
            m_UniqueProteinIDList = new Dictionary<int, SortedSet<string>>();
        }

        public override void AddProtein(ProteinStorageEntry proteinEntry)
        {
            int proteinEntryID = proteinEntry.Protein_ID;
            string proteinEntryName = proteinEntry.Reference;

            SortedSet<string> nameList = null;

            if (!m_UniqueProteinIDList.TryGetValue(proteinEntryID, out nameList))
            {
                nameList = new SortedSet<string>();
                nameList.Add(proteinEntryName);
                m_UniqueProteinIDList.Add(proteinEntryID, nameList);
                m_ResidueCount += proteinEntry.Sequence.Length;
            }
            else
            {
                foreach (var proteinName in nameList)
                {
                    var existingEntry = m_Proteins[proteinName];

                    if (!proteinEntry.Reference.Equals(existingEntry.Reference))
                    {
                        existingEntry.AddXRef(proteinEntryName);
                        proteinEntry.AddXRef(existingEntry.Reference);
                    }
                }

                nameList.Add(proteinEntryName);
                m_UniqueProteinIDList[proteinEntryID] = nameList;
            }

            if (!m_ProteinNames.Contains(proteinEntryName))
            {
                m_Proteins.Add(proteinEntryName, proteinEntry);
                m_ProteinNames.Add(proteinEntryName);
                m_ResidueCount += proteinEntry.Sequence.Length;
            }
        }

        public override void ClearProteinEntries()
        {
            base.ClearProteinEntries();
            m_UniqueProteinIDList.Clear();
        }
    }
}