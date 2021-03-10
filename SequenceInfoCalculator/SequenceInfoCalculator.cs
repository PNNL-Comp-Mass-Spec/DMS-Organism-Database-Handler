using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using TableManipulationBase;

namespace SequenceInfoCalculator
{
    public class SequenceInfoCalculator
    {
        internal static Dictionary<string, AminoAcidInfo> m_AminoAcids;

        private double m_MonoisotopicMass;
        private double m_AverageMass;
        private int m_Length;
        private string m_MolFormula;
        private string m_SHA1Hash;

        private static SHA1Managed m_SHA1Provider;

        private readonly string m_DMSConnectionString = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;";

        public SequenceInfoCalculator()
        {
            if (m_AminoAcids == null)
            {
                InitializeFromDMS();
            }

            if (m_SHA1Provider == null)
            {
                m_SHA1Provider = new SHA1Managed();
            }
        }

        #region "Ken's Added Properties "

        public double MonoisotopicMass => m_MonoisotopicMass;

        public double AverageMass => m_AverageMass;

        public int SequenceLength => m_Length;

        public string MolecularFormula => m_MolFormula;

        public string SHA1Hash => m_SHA1Hash;

        #endregion

        public void CalculateSequenceInfo(string sequence)
        {
            var tmpSeqInfo = SequenceInfo(sequence);
            m_MonoisotopicMass = tmpSeqInfo.MonoisotopicMass;
            m_AverageMass = tmpSeqInfo.AverageMass;
            m_MolFormula = tmpSeqInfo.MolecularFormula;
            m_Length = sequence.Length;
            m_SHA1Hash = GenerateHash(sequence);
        }

        protected SequenceInfo SequenceInfo(string sequence, string description = "")
        {
            var result = new SequenceInfo(string.Empty, description);

            var aaString = sequence.ToCharArray();

            try
            {
                foreach (char aa in aaString)
                {
                    AminoAcidInfo aaInfo = null;

                    if (!m_AminoAcids.TryGetValue(aa.ToString(), out aaInfo))
                    {
                        result.AddSequenceInfo(new SequenceInfo(aa.ToString(), "Not Found, adding input"));
                    }
                    else
                    {
                        result.AddSequenceInfo(aaInfo);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                string sequenceExcerpt;
                if (sequence.Length <= 20)
                {
                    sequenceExcerpt = sequence;
                }
                else
                {
                    sequenceExcerpt = sequence.Substring(0, 20) + "...";
                }

                throw new Exception("Error parsing " + sequenceExcerpt + ": " + ex.Message, ex);
            }
        }

        public string GenerateHash(string SourceText)
        {
            // Create an encoding object to ensure the encoding standard for the source text
            var Ue = new ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var ByteSourceText = Ue.GetBytes(SourceText);

            // Compute the hash value from the source
            var SHA1_hash = m_SHA1Provider.ComputeHash(ByteSourceText);

            // And convert it to String format for return
            string SHA1string = ToHexString(SHA1_hash);

            return SHA1string;
        }

        private void InitializeFromDMS()
        {
            m_AminoAcids = new Dictionary<string, AminoAcidInfo>(30);

            var getSQL = new DBTask(m_DMSConnectionString);

            string sqlString = "SELECT * FROM T_Residues WHERE [Num_C] > 0";
            var tmpAATable = getSQL.GetTable(sqlString);

            foreach (DataRow dr in tmpAATable.Rows)
            {
                string singleLetterSymbol = dr["Residue_Symbol"].ToString();
                string description = dr["Description"].ToString();
                int countC = Convert.ToInt32(dr["Num_C"]);
                int countH = Convert.ToInt32(dr["Num_H"]);
                int countN = Convert.ToInt32(dr["Num_N"]);
                int countO = Convert.ToInt32(dr["Num_O"]);
                int countS = Convert.ToInt32(dr["Num_S"]);
                double monoMass = Convert.ToDouble(dr["Monoisotopic_Mass"]);
                double avgMass = Convert.ToDouble(dr["Average_Mass"]);

                AddAminoAcid(new AminoAcidInfo(singleLetterSymbol, description, countC, countH, countN, countO, countS, avgMass, monoMass));
            }
        }

        private void AddAminoAcid(AminoAcidInfo aa)
        {
            m_AminoAcids.Add(aa.Symbol, aa);
        }

        internal class AminoAcidInfo : SequenceInfo
        {
            public AminoAcidInfo(string seq, string name,
                int C_Count, int H_Count, int N_Count, int O_Count, int S_Count,
                double average, double monoisotopic)
                : base(seq, name, C_Count, H_Count, N_Count, O_Count, S_Count, average, monoisotopic)
            {
                if (seq.Length != 1)
                {
                    throw new ApplicationException("'" + seq + "' is not a valid amino acid.  Must be only one character long.");
                }
            }

            public string Symbol => Sequence;
        }

        public static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();

            foreach (var b in bytes)
                sb.Append(string.Format("{0:X2}", b));

            return sb.ToString();
        }
    }

    public class SequenceInfo
    {
        private bool m_invalidated = false;
        private string m_sequence;
        private int m_C_Count;
        private int m_H_Count;
        private int m_N_Count;
        private int m_O_Count;
        private int m_S_Count;
        private double m_Average_Mass;
        private double m_Monoisotopic_Mass;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="seqName"></param>
        public SequenceInfo(string seq, string seqName)
            : this(seq, seqName, 0, 0, 0, 0, 0, 0.0d, 0.0d)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="seq"></param>
        /// <param name="seqName"></param>
        /// <param name="C_Count"></param>
        /// <param name="H_Count"></param>
        /// <param name="N_Count"></param>
        /// <param name="O_Count"></param>
        /// <param name="S_Count"></param>
        /// <param name="average"></param>
        /// <param name="monoisotopic"></param>
        public SequenceInfo(string seq, string seqName,
            int C_Count, int H_Count, int N_Count, int O_Count, int S_Count,
            double average, double monoisotopic)
        {
            m_sequence = seq;
            Name = seqName;
            m_C_Count = C_Count;
            m_H_Count = H_Count;
            m_N_Count = N_Count;
            m_O_Count = O_Count;
            m_S_Count = S_Count;
            m_Average_Mass = average;
            m_Monoisotopic_Mass = monoisotopic;
        }

        public string Sequence => m_sequence;

        public void Invalidate()
        {
            m_invalidated = true;
            m_Average_Mass = 0d;
            m_C_Count = 0;
            m_H_Count = 0;
            m_Monoisotopic_Mass = 0d;
            m_N_Count = 0;
            m_O_Count = 0;
            m_S_Count = 0;
        }

        public bool Invalidated => m_invalidated;

        public string Name { get; private set; }

        public int C_Count => m_C_Count;

        public int H_Count => m_H_Count;

        public int N_Count => m_N_Count;

        public int O_Count => m_O_Count;

        public int S_Count => m_S_Count;

        public double AverageMass => m_Average_Mass + 18.01528d;

        public double MonoisotopicMass => m_Monoisotopic_Mass + 18.0105633d;

        public string MolecularFormula => GetMolecularFormula();

        private string GetMolecularFormula()
        {
            string mf = "C" + m_C_Count + " H" + m_H_Count + " N" + m_N_Count + " O" + m_O_Count + " S" + m_S_Count;
            return mf;
        }

        public void AddSequenceInfo(SequenceInfo info)
        {
            if (m_sequence.Length == 0)
            {
                m_H_Count = 2;
                m_O_Count = 1;
            }

            m_sequence = m_sequence + info.Sequence;
            if (!m_invalidated)
            {
                m_C_Count = m_C_Count + info.C_Count;
                m_H_Count = m_H_Count + info.H_Count;
                m_N_Count = m_N_Count + info.N_Count;
                m_O_Count = m_O_Count + info.O_Count;
                m_S_Count = m_S_Count + info.S_Count;
                m_Monoisotopic_Mass = m_Monoisotopic_Mass + info.m_Monoisotopic_Mass;
                m_Average_Mass = m_Average_Mass + info.m_Average_Mass;
            }
        }
    }
}