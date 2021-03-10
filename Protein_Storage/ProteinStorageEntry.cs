using System;
using System.Collections.Generic;

namespace Protein_Storage
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

            m_Reference = reference;
            m_Description = description;
            m_Sequence = sequence;
            m_MonoMass = monoisotopicMass;
            m_AvgMass = averageMass;
            m_Length = length;
            m_MolecularFormula = molecularFormula;
            m_AuthHash = authenticationHash;
            m_SortCount = sortingIndex;

            m_Protein_ID = 0;
        }

        private string m_Reference;
        private string m_AlternateReference;
        private string m_Description;
        private string m_Sequence;

        private double m_MonoMass;
        private double m_AvgMass;
        private int m_Length;
        private string m_MolecularFormula;
        private string m_AuthHash;
        private int m_Protein_ID;
        private int m_Reference_ID;
        private int m_Member_ID;
        private int m_Authority_ID;
        private List<string> m_XRefList;
        private int m_SortCount;

        private bool m_IsEncrypted = false;

        public string Reference => m_Reference;

        protected string AlternateReference
        {
            get => m_AlternateReference;
            set
            {
                if (value.Length > 0)
                {
                    m_AlternateReference = value;
                }
                else
                {
                    m_AlternateReference = null;
                }
            }
        }

        public bool HasAlternateReference => m_AlternateReference != null;

        public string Description => m_Description;

        public string Sequence
        {
            get => m_Sequence;
            set => m_Sequence = value;
        }

        public bool IsEncrypted
        {
            get => m_IsEncrypted;
            set => m_IsEncrypted = value;
        }

        public double MonoisotopicMass => m_MonoMass;

        public double AverageMass => m_AvgMass;

        public int Length => m_Length;

        public string MolecularFormula => m_MolecularFormula;

        public string SHA1Hash
        {
            get => m_AuthHash;
            set => m_AuthHash = value;
        }

        public int Protein_ID
        {
            get => m_Protein_ID;
            set => m_Protein_ID = value;
        }

        public int Reference_ID
        {
            get => m_Reference_ID;
            set => m_Reference_ID = value;
        }

        public int Member_ID
        {
            get => m_Member_ID;
            set => m_Member_ID = value;
        }

        public int Authority_ID
        {
            get => m_Authority_ID;
            set => m_Authority_ID = value;
        }

        public int SortingIndex
        {
            get => m_SortCount;
            set => m_SortCount = value;
        }

        public List<string> NameXRefs => m_XRefList;

        public void AddXRef(string newReference)
        {
            if (m_XRefList == null)
            {
                m_XRefList = new List<string>();
            }

            m_XRefList.Add(newReference);
        }

        public void SetReferenceName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new Exception("New protein name cannot be empty");
            }

            m_Reference = newName;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(m_Sequence))
            {
                return m_Reference + ", ResidueCount=0";
            }
            else
            {
                return m_Reference + ", ResidueCount=" + m_Length + ", " + m_Sequence.Substring(0, 20);
            }
        }
    }
}