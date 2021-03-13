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
        private string sha1Hash;

        private static SHA1Managed sha1Provider;

        private readonly string mDMSConnectionString = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;";

        public SequenceInfoCalculator()
        {
            if (mAminoAcids == null)
            {
                InitializeFromDMS();
            }

            if (sha1Provider == null)
            {
                sha1Provider = new SHA1Managed();
            }
        }

        #region "Ken's Added Properties "

        public double MonoisotopicMass => mMonoisotopicMass;

        public double AverageMass => mAverageMass;

        public int SequenceLength => mLength;

        public string MolecularFormula => mMolFormula;

        public string SHA1Hash => sha1Hash;

        #endregion

        public void CalculateSequenceInfo(string sequence)
        {
            var tmpSeqInfo = SequenceInfo(sequence);
            mMonoisotopicMass = tmpSeqInfo.MonoisotopicMass;
            mAverageMass = tmpSeqInfo.AverageMass;
            mMolFormula = tmpSeqInfo.MolecularFormula;
            mLength = sequence.Length;
            sha1Hash = GenerateHash(sequence);
        }

        protected SequenceInfo SequenceInfo(string sequence, string description = "")
        {
            var result = new SequenceInfo(string.Empty, description);

            var aaString = sequence.ToCharArray();

            try
            {
                foreach (var aa in aaString)
                {
                    if (!mAminoAcids.TryGetValue(aa.ToString(), out var aaInfo))
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

        public string GenerateHash(string sourceText)
        {
            // Create an encoding object to ensure the encoding standard for the source text
            var ue = new ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var byteSourceText = ue.GetBytes(sourceText);

            // Compute the hash value from the source
            var sha1HashBytes = sha1Provider.ComputeHash(byteSourceText);

            // And convert it to String format for return
            var sha1String = ToHexString(sha1HashBytes);

            return sha1String;
        }

        private void InitializeFromDMS()
        {
            mAminoAcids = new Dictionary<string, AminoAcidInfo>(30);

            var getSql = new DBTask(mDMSConnectionString);

            const string sqlString = "SELECT * FROM T_Residues WHERE [Num_C] > 0";
            var tmpAATable = getSql.GetTable(sqlString);

            foreach (DataRow dr in tmpAATable.Rows)
            {
                var singleLetterSymbol = dr["Residue_Symbol"].ToString();
                var description = dr["Description"].ToString();
                var countC = Convert.ToInt32(dr["Num_C"]);
                var countH = Convert.ToInt32(dr["Num_H"]);
                var countN = Convert.ToInt32(dr["Num_N"]);
                var countO = Convert.ToInt32(dr["Num_O"]);
                var countS = Convert.ToInt32(dr["Num_S"]);
                var monoMass = Convert.ToDouble(dr["Monoisotopic_Mass"]);
                var avgMass = Convert.ToDouble(dr["Average_Mass"]);

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
                int countC, int countH, int countN, int countO, int countS,
                double average, double monoisotopic)
                : base(seq, name, countC, countH, countN, countO, countS, average, monoisotopic)
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
            {
                sb.AppendFormat("{0:X2}", b);
            }

            return sb.ToString();
        }
    }

    public class SequenceInfo
    {
        private bool mInvalidated = false;
        private string mSequence;
        private int countC;
        private int countH;
        private int countN;
        private int countO;
        private int countS;
        private double mAverageMass;
        private double mMonoisotopicMass;

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
        /// <param name="cCount"></param>
        /// <param name="hCount"></param>
        /// <param name="nCount"></param>
        /// <param name="oCount"></param>
        /// <param name="sCount"></param>
        /// <param name="average"></param>
        /// <param name="monoisotopic"></param>
        public SequenceInfo(string seq, string seqName,
            int cCount, int hCount, int nCount, int oCount, int sCount,
            double average, double monoisotopic)
        {
            mSequence = seq;
            Name = seqName;
            countC = cCount;
            countH = hCount;
            countN = nCount;
            countO = oCount;
            countS = sCount;
            mAverageMass = average;
            mMonoisotopicMass = monoisotopic;
        }

        public string Sequence => mSequence;

        public void Invalidate()
        {
            mInvalidated = true;
            mAverageMass = 0d;
            countC = 0;
            countH = 0;
            mMonoisotopicMass = 0d;
            countN = 0;
            countO = 0;
            countS = 0;
        }

        public bool Invalidated => mInvalidated;

        public string Name { get; }

        public int C => countC;

        public int H => countH;

        public int N => countN;

        public int O => countO;

        public int S => countS;

        public double AverageMass => mAverageMass + 18.01528d;

        public double MonoisotopicMass => mMonoisotopicMass + 18.0105633d;

        public string MolecularFormula => GetMolecularFormula();

        private string GetMolecularFormula()
        {
            var mf = "C" + countC + " H" + countH + " N" + countN + " O" + countO + " S" + countS;
            return mf;
        }

        public void AddSequenceInfo(SequenceInfo info)
        {
            if (mSequence.Length == 0)
            {
                countH = 2;
                countO = 1;
            }

            mSequence += info.Sequence;
            if (!mInvalidated)
            {
                countC += info.countC;
                countH += info.countH;
                countN += info.countN;
                countO += info.countO;
                countS += info.countS;
                mMonoisotopicMass += info.mMonoisotopicMass;
                mAverageMass += info.mAverageMass;
            }
        }
    }
}