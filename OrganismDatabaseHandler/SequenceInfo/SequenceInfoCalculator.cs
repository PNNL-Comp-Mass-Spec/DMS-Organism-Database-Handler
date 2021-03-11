using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.SequenceInfo
{
    public class SequenceInfoCalculator
    {
        private static Dictionary<string, AminoAcidInfo> mAminoAcids;

        private double mMonoisotopicMass;
        private double mAverageMass;
        private int mLength;
        private string mMolFormula;
        private string mSHA1Hash;

        private static SHA1Managed mSHA1Provider;

        private readonly string mDMSConnectionString = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;";

        public SequenceInfoCalculator()
        {
            if (mAminoAcids == null)
            {
                InitializeFromDMS();
            }

            if (mSHA1Provider == null)
            {
                mSHA1Provider = new SHA1Managed();
            }
        }

        #region "Ken's Added Properties "

        public double MonoisotopicMass => mMonoisotopicMass;

        public double AverageMass => mAverageMass;

        public int SequenceLength => mLength;

        public string MolecularFormula => mMolFormula;

        public string SHA1Hash => mSHA1Hash;

        #endregion

        public void CalculateSequenceInfo(string sequence)
        {
            var tmpSeqInfo = SequenceInfo(sequence);
            mMonoisotopicMass = tmpSeqInfo.MonoisotopicMass;
            mAverageMass = tmpSeqInfo.AverageMass;
            mMolFormula = tmpSeqInfo.MolecularFormula;
            mLength = sequence.Length;
            mSHA1Hash = GenerateHash(sequence);
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

                    if (!mAminoAcids.TryGetValue(aa.ToString(), out aaInfo))
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
            var SHA1_hash = mSHA1Provider.ComputeHash(ByteSourceText);

            // And convert it to String format for return
            string SHA1string = ToHexString(SHA1_hash);

            return SHA1string;
        }

        private void InitializeFromDMS()
        {
            mAminoAcids = new Dictionary<string, AminoAcidInfo>(30);

            var getSQL = new DBTask(mDMSConnectionString);

            string sqlString = "SELECT * FROM T_Residues WHERE [NumC] > 0";
            var tmpAATable = getSQL.GetTable(sqlString);

            foreach (DataRow dr in tmpAATable.Rows)
            {
                string singleLetterSymbol = dr["Residue_Symbol"].ToString();
                string description = dr["Description"].ToString();
                int countC = Convert.ToInt32(dr["NumC"]);
                int countH = Convert.ToInt32(dr["NumH"]);
                int countN = Convert.ToInt32(dr["NumN"]);
                int countO = Convert.ToInt32(dr["NumO"]);
                int countS = Convert.ToInt32(dr["NumS"]);
                double monoMass = Convert.ToDouble(dr["Monoisotopic_Mass"]);
                double avgMass = Convert.ToDouble(dr["Average_Mass"]);

                AddAminoAcid(new AminoAcidInfo(singleLetterSymbol, description, countC, countH, countN, countO, countS, avgMass, monoMass));
            }
        }

        private void AddAminoAcid(AminoAcidInfo aa)
        {
            mAminoAcids.Add(aa.Symbol, aa);
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
        private bool minvalidated = false;
        private string msequence;
        private int mC_Count;
        private int mH_Count;
        private int mN_Count;
        private int mO_Count;
        private int mS_Count;
        private double mAverage_Mass;
        private double mMonoisotopic_Mass;

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
            msequence = seq;
            Name = seqName;
            mC_Count = C_Count;
            mH_Count = H_Count;
            mN_Count = N_Count;
            mO_Count = O_Count;
            mS_Count = S_Count;
            mAverage_Mass = average;
            mMonoisotopic_Mass = monoisotopic;
        }

        public string Sequence => msequence;

        public void Invalidate()
        {
            minvalidated = true;
            mAverage_Mass = 0d;
            mC_Count = 0;
            mH_Count = 0;
            mMonoisotopic_Mass = 0d;
            mN_Count = 0;
            mO_Count = 0;
            mS_Count = 0;
        }

        public bool Invalidated => minvalidated;

        public string Name { get; private set; }

        public int C_Count => mC_Count;

        public int H_Count => mH_Count;

        public int N_Count => mN_Count;

        public int O_Count => mO_Count;

        public int S_Count => mS_Count;

        public double AverageMass => mAverage_Mass + 18.01528d;

        public double MonoisotopicMass => mMonoisotopic_Mass + 18.0105633d;

        public string MolecularFormula => GetMolecularFormula();

        private string GetMolecularFormula()
        {
            string mf = "C" + mC_Count + " H" + mH_Count + " N" + mN_Count + " O" + mO_Count + " S" + mS_Count;
            return mf;
        }

        public void AddSequenceInfo(SequenceInfo info)
        {
            if (msequence.Length == 0)
            {
                mH_Count = 2;
                mO_Count = 1;
            }

            msequence = msequence + info.Sequence;
            if (!minvalidated)
            {
                mC_Count = mC_Count + info.C_Count;
                mH_Count = mH_Count + info.H_Count;
                mN_Count = mN_Count + info.N_Count;
                mO_Count = mO_Count + info.O_Count;
                mS_Count = mS_Count + info.S_Count;
                mMonoisotopic_Mass = mMonoisotopic_Mass + info.mMonoisotopic_Mass;
                mAverage_Mass = mAverage_Mass + info.mAverage_Mass;
            }
        }
    }
}