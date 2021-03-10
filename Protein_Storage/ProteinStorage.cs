using System.Collections.Generic;

namespace Protein_Storage
{
    public class ProteinStorage
    {
        /// <summary>
        /// Keys are Protein_Name
        /// </summary>
        protected readonly Dictionary<string, ProteinStorageEntry> m_Proteins;
        protected int m_ResidueCount;
        protected readonly SortedSet<string> m_ProteinNames;
        protected string m_PassPhrase;

        public ProteinStorage(string fastaFileName)
        {
            FileName = fastaFileName;
            m_Proteins = new Dictionary<string, ProteinStorageEntry>();
            m_ProteinNames = new SortedSet<string>();
        }

        public virtual void AddProtein(ProteinStorageEntry proteinEntry)
        {
            if (!m_Proteins.ContainsKey(proteinEntry.Reference))
            {
                m_Proteins.Add(proteinEntry.Reference, proteinEntry);
                m_ProteinNames.Add(proteinEntry.Reference);
                m_ResidueCount += proteinEntry.Sequence.Length;
            }
            else
            {
                proteinEntry.SetReferenceName(proteinEntry.Reference + "_dup_" + proteinEntry.SHA1Hash.Substring(1, 10));
                AddProtein(proteinEntry);
                // flag with some kinda error so we can check out the duplicate entry and rename it
            }
        }

        protected string FileName { get; set; }

        public ProteinStorageEntry GetProtein(string reference)
        {
            ProteinStorageEntry proteinEntry = null;

            if (m_Proteins.TryGetValue(reference, out proteinEntry))
            {
                return proteinEntry;
            }
            else
            {
                return null;
            }
        }

        public SortedSet<string> GetSortedProteinNames()
        {
            return m_ProteinNames;
        }

        public virtual void ClearProteinEntries()
        {
            m_ResidueCount = 0;
            m_Proteins.Clear();
            m_ProteinNames.Clear();
        }

        public int TotalResidueCount
        {
            get
            {
                return m_ResidueCount;
            }
        }

        public int ProteinCount
        {
            get
            {
                return m_Proteins.Count;
            }
        }

        public bool EncryptSequences { get; set; }

        public string PassPhrase
        {
            get
            {
                if (EncryptSequences)
                {
                    return m_PassPhrase;
                }
                else
                {
                    return null;
                }
            }

            set
            {
                m_PassPhrase = value;
            }
        }

        public Dictionary<string, ProteinStorageEntry>.Enumerator GetEnumerator()
        {
            return m_Proteins.GetEnumerator();
        }

        public override string ToString()
        {
            return FileName + ": " + m_ProteinNames.Count + " proteins";
        }
    }
}