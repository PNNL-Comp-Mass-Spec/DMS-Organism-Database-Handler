using System;
using System.Collections.Generic;

namespace OrganismDatabaseHandler.ProteinStorage
{
    public class ProteinStorageEntry
    {
        public ProteinStorageEntry(
            string reference,
            string description,
            string sequence,
            int length,
            double monoisotopicMass,
            double averageMass,
            string molecularFormula,
            string authenticationHash,
            int sortingIndex)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                throw new Exception("Reference name cannot be empty");
            }

            Reference = reference;
            Description = description;
            Sequence = sequence;
            MonoisotopicMass = monoisotopicMass;
            AverageMass = averageMass;
            Length = length;
            MolecularFormula = molecularFormula;
            SHA1Hash = authenticationHash;
            SortingIndex = sortingIndex;

            Protein_ID = 0;
        }

        private string mAlternateReference;

        private List<string> mXRefList;

        public string Reference { get; private set; }

        protected string AlternateReference
        {
            get => mAlternateReference;
            set
            {
                if (value.Length > 0)
                {
                    mAlternateReference = value;
                }
                else
                {
                    mAlternateReference = null;
                }
            }
        }

        public bool HasAlternateReference => mAlternateReference != null;

        public string Description { get; }

        public string Sequence { get; set; }

        public bool IsEncrypted { get; set; } = false;

        public double MonoisotopicMass { get; }

        public double AverageMass { get; }

        public int Length { get; }

        public string MolecularFormula { get; }

        public string SHA1Hash { get; set; }

        public int Protein_ID { get; set; }

        public int Reference_ID { get; set; }

        public int Member_ID { get; set; }

        public int Authority_ID { get; set; }

        public int SortingIndex { get; set; }

        public List<string> NameXRefs => mXRefList;

        public void AddXRef(string newReference)
        {
            if (mXRefList == null)
            {
                mXRefList = new List<string>();
            }

            mXRefList.Add(newReference);
        }

        public void SetReferenceName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new Exception("New protein name cannot be empty");
            }

            Reference = newName;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Sequence))
            {
                return Reference + ", ResidueCount=0";
            }
            else
            {
                return Reference + ", ResidueCount=" + Length + ", " + Sequence.Substring(0, 20);
            }
        }
    }
}