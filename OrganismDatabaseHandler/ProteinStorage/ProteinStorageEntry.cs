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

            ProteinId = 0;

            NameXRefs = new List<string>();
        }

        public string Reference { get; private set; }

        public string Description { get; }

        public string Sequence { get; set; }

        public bool IsEncrypted { get; set; } = false;

        public double MonoisotopicMass { get; }

        public double AverageMass { get; }

        public int Length { get; }

        public string MolecularFormula { get; }

        public string SHA1Hash { get; set; }

        public int ProteinId { get; set; }

        public int ReferenceId { get; set; }

        public int MemberId { get; set; }

        public int SortingIndex { get; set; }

        public List<string> NameXRefs { get; }

        public void AddXRef(string newReference)
        {
            NameXRefs.Add(newReference);
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