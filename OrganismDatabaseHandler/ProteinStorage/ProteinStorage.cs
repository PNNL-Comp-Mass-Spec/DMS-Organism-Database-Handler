using System.Collections.Generic;

namespace OrganismDatabaseHandler.ProteinStorage
{
    public class ProteinStorage
    {
        /// <summary>
        /// Keys are Protein_Name
        /// </summary>
        protected readonly Dictionary<string, ProteinStorageEntry> mProteins;
        protected int mResidueCount;
        protected readonly SortedSet<string> mProteinNames;
        private string mPassPhrase;

        public ProteinStorage(string fastaFileName)
        {
            FileName = fastaFileName;
            mProteins = new Dictionary<string, ProteinStorageEntry>();
            mProteinNames = new SortedSet<string>();
        }

        public virtual void AddProtein(ProteinStorageEntry proteinEntry)
        {
            if (!mProteins.ContainsKey(proteinEntry.Reference))
            {
                mProteins.Add(proteinEntry.Reference, proteinEntry);
                mProteinNames.Add(proteinEntry.Reference);
                mResidueCount += proteinEntry.Sequence.Length;
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

            if (mProteins.TryGetValue(reference, out proteinEntry))
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
            return mProteinNames;
        }

        public virtual void ClearProteinEntries()
        {
            mResidueCount = 0;
            mProteins.Clear();
            mProteinNames.Clear();
        }

        public int TotalResidueCount => mResidueCount;

        public int ProteinCount => mProteins.Count;

        public bool EncryptSequences { get; set; }

        public string PassPhrase
        {
            get
            {
                if (EncryptSequences)
                {
                    return mPassPhrase;
                }
                else
                {
                    return null;
                }
            }
            set => mPassPhrase = value;
        }

        public Dictionary<string, ProteinStorageEntry>.Enumerator GetEnumerator()
        {
            return mProteins.GetEnumerator();
        }

        public IEnumerable<ProteinStorageEntry> GetEntriesIEnumerable()
        {
            return mProteins.Values;
        }

        public override string ToString()
        {
            return FileName + ": " + mProteinNames.Count + " proteins";
        }
    }
}