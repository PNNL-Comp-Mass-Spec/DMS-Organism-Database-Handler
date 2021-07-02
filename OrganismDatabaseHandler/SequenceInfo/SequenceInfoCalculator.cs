using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;
using PRISM;

namespace OrganismDatabaseHandler.SequenceInfo
{
    public class SequenceInfoCalculator : EventNotifier
    {
        private static Dictionary<string, AminoAcidInfo> mAminoAcids;

        private static SHA1Managed sha1Provider;

        private readonly string mDMSConnectionString = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;";

        public SequenceInfoCalculator()
        {
            if (mAminoAcids == null)
            {
                InitializeFromDMS();
            }

            sha1Provider ??= new SHA1Managed();
        }

        #region "Ken's Added Properties "

        public double MonoisotopicMass { get; private set; }

        public double AverageMass { get; private set; }

        public int SequenceLength { get; private set; }

        public string MolecularFormula { get; private set; }

        public string SHA1Hash { get; private set; }

        #endregion

        public void CalculateSequenceInfo(string sequence)
        {
            var tmpSeqInfo = SequenceInfo(sequence);
            MonoisotopicMass = tmpSeqInfo.MonoisotopicMass;
            AverageMass = tmpSeqInfo.AverageMass;
            MolecularFormula = tmpSeqInfo.MolecularFormula;
            SequenceLength = sequence.Length;
            SHA1Hash = GenerateHash(sequence);
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
            RegisterEvents(getSql);

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
            Sequence = seq;
            Name = seqName;
            C = cCount;
            H = hCount;
            N = nCount;
            O = oCount;
            S = sCount;
            mAverageMass = average;
            mMonoisotopicMass = monoisotopic;
        }

        public string Sequence { get; private set; }

        public void Invalidate()
        {
            Invalidated = true;
            mAverageMass = 0d;
            C = 0;
            H = 0;
            mMonoisotopicMass = 0d;
            N = 0;
            O = 0;
            S = 0;
        }

        public bool Invalidated { get; private set; }

        public string Name { get; }

        /// <summary>
        /// Number of C atoms
        /// </summary>
        public int C { get; private set; }

        /// <summary>
        /// Number of H atoms
        /// </summary>
        public int H { get; private set; }

        /// <summary>
        /// Number of N atoms
        /// </summary>
        public int N { get; private set; }

        /// <summary>
        /// Number of O atoms
        /// </summary>
        public int O { get; private set; }

        /// <summary>
        /// Number of S atoms
        /// </summary>
        public int S { get; private set; }

        public double AverageMass => mAverageMass + 18.01528d;

        public double MonoisotopicMass => mMonoisotopicMass + 18.0105633d;

        public string MolecularFormula => GetMolecularFormula();

        private string GetMolecularFormula()
        {
            return string.Format("C{0} H{1} N{2} O{3} S{4}", C, H, N, O, S);
        }

        public void AddSequenceInfo(SequenceInfo info)
        {
            if (Sequence.Length == 0)
            {
                H = 2;
                O = 1;
            }

            Sequence += info.Sequence;
            if (!Invalidated)
            {
                C += info.C;
                H += info.H;
                N += info.N;
                O += info.O;
                S += info.S;
                mMonoisotopicMass += info.mMonoisotopicMass;
                mAverageMass += info.mAverageMass;
            }
        }
    }
}