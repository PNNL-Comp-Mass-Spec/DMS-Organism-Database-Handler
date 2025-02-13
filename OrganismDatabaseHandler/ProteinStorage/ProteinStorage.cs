using System.Collections.Generic;

namespace OrganismDatabaseHandler.ProteinStorage
{
    public class ProteinStorage
    {
        // Ignore Spelling: FASTA

        /// <summary>
        /// Keys are Protein_Name
        /// </summary>
        protected readonly Dictionary<string, ProteinStorageEntry> Proteins;
        protected int ResidueCount;
        protected readonly SortedSet<string> ProteinNames;
        private string mPassPhrase;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fastaFilePath"></param>
        public ProteinStorage(string fastaFilePath)
        {
            FilePath = fastaFilePath;
            Proteins = new Dictionary<string, ProteinStorageEntry>();
            ProteinNames = new SortedSet<string>();
        }

        public virtual void AddProtein(ProteinStorageEntry proteinEntry)
        {
            if (!Proteins.ContainsKey(proteinEntry.Reference))
            {
                Proteins.Add(proteinEntry.Reference, proteinEntry);
                ProteinNames.Add(proteinEntry.Reference);
                ResidueCount += proteinEntry.Sequence.Length;
            }
            else
            {
                proteinEntry.SetReferenceName(proteinEntry.Reference + "_dup_" + proteinEntry.SHA1Hash.Substring(1, 10));
                AddProtein(proteinEntry);
                // flag with some kinda error so we can check out the duplicate entry and rename it
            }
        }

        /// <summary>
        /// Full path to the FASTA file
        /// </summary>
        protected string FilePath { get; set; }

        public ProteinStorageEntry GetProtein(string reference)
        {
            if (Proteins.TryGetValue(reference, out var proteinEntry))
            {
                return proteinEntry;
            }

            return null;
        }

        public SortedSet<string> GetSortedProteinNames()
        {
            return ProteinNames;
        }

        public virtual void ClearProteinEntries()
        {
            ResidueCount = 0;
            Proteins.Clear();
            ProteinNames.Clear();
        }

        public int TotalResidueCount => ResidueCount;

        public int ProteinCount => Proteins.Count;

        public bool EncryptSequences { get; set; }

        public string PassPhrase
        {
            get
            {
                if (EncryptSequences)
                {
                    return mPassPhrase;
                }

                return null;
            }
            set => mPassPhrase = value;
        }

        public Dictionary<string, ProteinStorageEntry>.Enumerator GetEnumerator()
        {
            return Proteins.GetEnumerator();
        }

        public IEnumerable<ProteinStorageEntry> GetEntriesIEnumerable()
        {
            return Proteins.Values;
        }

        public override string ToString()
        {
            return FilePath + ": " + ProteinNames.Count + " proteins";
        }
    }
}