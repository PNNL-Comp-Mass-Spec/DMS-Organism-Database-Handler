using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using OrganismDatabaseHandler.DatabaseTools;
using PRISM;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.SequenceInfo
{
    public class SequenceInfoCalculator : EventNotifier
    {
        // Ignore Spelling: SHA

        // Prior to February 2025:    Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI
        // Starting in February 2025: Host=prismdb2.emsl.pnl.gov;Port=5432;Database=dms;UserId=pceditor

        private const string DEFAULT_DB_CONNECTION_STRING = "Host=prismdb2.emsl.pnl.gov;Port=5432;Database=dms;UserId=pceditor";

        private static Dictionary<string, AminoAcidInfo> mAminoAcids;

        private readonly string mDMSConnectionString;

        private static SHA1Managed mSHA1Provider;

        public double MonoisotopicMass { get; private set; }

        public double AverageMass { get; private set; }

        public int SequenceLength { get; private set; }

        public string MolecularFormula { get; private set; }

        public string SHA1Hash { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnectionString">DMS database connection string</param>
        public SequenceInfoCalculator(string dbConnectionString)
        {
            if (string.IsNullOrWhiteSpace(dbConnectionString))
            {
                mDMSConnectionString = DEFAULT_DB_CONNECTION_STRING;
            }
            else
            {
                mDMSConnectionString = dbConnectionString;
            }

            // Note that mAminoAcids is a static dictionary
            if (mAminoAcids == null)
            {
                InitializeFromDMS();
            }

            mSHA1Provider = new SHA1Managed();
        }

        public void CalculateSequenceInfo(string sequence)
        {
            var seqInfo = SequenceInfo(sequence);
            MonoisotopicMass = seqInfo.MonoisotopicMass;
            AverageMass = seqInfo.AverageMass;
            MolecularFormula = seqInfo.MolecularFormula;
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
            var sha1HashBytes = mSHA1Provider.ComputeHash(byteSourceText);

            // Convert it to a string
            return ToHexString(sha1HashBytes);
        }

        private void InitializeFromDMS()
        {
            mAminoAcids = new Dictionary<string, AminoAcidInfo>(30);

            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mDMSConnectionString, "OrganismDatabaseHandler");

            var getSql = new DBTask(connectionStringToUse);
            RegisterEvents(getSql);

            const string sqlString =
                "SELECT Residue_ID, Residue_Symbol, Description, Abbreviation, Average_Mass, Monoisotopic_Mass, " +
                "       Num_C, Num_H, Num_N, Num_O, Num_S, " +
                "       Empirical_Formula, Amino_Acid_Name " +
                "FROM T_Residues " +
                "WHERE Num_C > 0";

            var aminoAcidTable = getSql.GetTable(sqlString);

            foreach (DataRow dataRow in aminoAcidTable.Rows)
            {
                var singleLetterSymbol = dataRow["residue_symbol"].ToString();
                var description = dataRow["description"].ToString();
                var countC = Convert.ToInt32(dataRow["num_c"]);
                var countH = Convert.ToInt32(dataRow["num_h"]);
                var countN = Convert.ToInt32(dataRow["num_n"]);
                var countO = Convert.ToInt32(dataRow["num_o"]);
                var countS = Convert.ToInt32(dataRow["num_s"]);
                var monoMass = Convert.ToDouble(dataRow["monoisotopic_mass"]);
                var avgMass = Convert.ToDouble(dataRow["average_mass"]);

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