using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.ProteinStorage;
using OrganismDatabaseHandler.SequenceInfo;
using PRISM;

namespace OrganismDatabaseHandler.ProteinImport
{
    public class FASTAReader : EventNotifier
    {
        // Ignore Spelling: fasta

        private string mFASTAFilePath;

        private readonly string mDbConnectionString;
        private readonly Regex mDescLineRegEx;
        private readonly Regex mNoDescLineRegEx;
        private readonly Regex mDescLineMatcher;

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadProgressEventHandler LoadProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbConnectionString">Protein sequences database connection string</param>
        public FASTAReader(string dbConnectionString)
        {
            mDbConnectionString = dbConnectionString;
            mDescLineMatcher = new Regex(@"^\>.+$");
            mDescLineRegEx = new Regex(@"^\>(?<name>\S+)\s+(?<description>.*)$");
            mNoDescLineRegEx = new Regex(@"^\>(?<name>\S+)$");
        }

        public string LastErrorMessage { get; private set; }

        public bool GetProteinEntries(string filePath, out ProteinStorage.ProteinStorage fastaContents)
        {
            return LoadFASTAFile(filePath, -1, out fastaContents);
        }

        public bool GetProteinEntries(string filePath, int numRecordsToLoad, out ProteinStorage.ProteinStorage fastaContents)
        {
            return LoadFASTAFile(filePath, numRecordsToLoad, out fastaContents);
        }

        [Obsolete("Unused method; duplicate of GetProteinEntries()")]
        public bool LoadFASTAFile(string filePath, out ProteinStorage.ProteinStorage fastaContents)
        {
            return LoadFASTAFile(filePath, -1, out fastaContents);
        }

        public bool LoadFASTAFile(string filePath, int numRecordsToLoad, out ProteinStorage.ProteinStorage fastaContents)
        {
            var currentPosition = 0;

            fastaContents = new ProteinStorage.ProteinStorage(filePath);

            var proteinName = string.Empty;
            var description = string.Empty;
            var sequence = new StringBuilder();

            LastErrorMessage = string.Empty;

            SequenceInfoCalculator seqInfo;

            try
            {
                seqInfo = new SequenceInfoCalculator(mDbConnectionString);
                RegisterEvents(seqInfo);
            }
            catch (Exception ex)
            {
                var stackTrace = StackTraceFormatter.GetExceptionStackTrace(ex);
                LastErrorMessage = string.Format("Error initializing the sequence info calculator: {0}; {1}", ex.Message, stackTrace);

                return false;
            }

            var recordCount = 0;

            mFASTAFilePath = filePath;

            var lineEndCharCount = LineEndCharacterCount(filePath);

            try
            {
                var fastaFile = new FileInfo(mFASTAFilePath);
                var fileLength = fastaFile.Length;

                if (!fastaFile.Exists)
                {
                    LastErrorMessage = string.Format("FASTA file not found: {0}", mFASTAFilePath);
                    return false;
                }

                if (fileLength == 0)
                {
                    LastErrorMessage = string.Format("FASTA file is zero bytes: {0}", mFASTAFilePath);
                    return false;
                }

                // Trigger the setup of the progress bar
                LoadStart?.Invoke("Reading Source File...");

                using var fileReader = new StreamReader(new FileStream(fastaFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));

                while (!fileReader.EndOfStream)
                {
                    var dataLine = fileReader.ReadLine()?.Trim();

                    if (string.IsNullOrWhiteSpace(dataLine))
                        continue;

                    if (mDescLineMatcher.IsMatch(dataLine))
                    {
                        // DescriptionLine, new record
                        if (currentPosition > 0) // dump current record
                        {
                            seqInfo.CalculateSequenceInfo(sequence.ToString());

                            if (seqInfo.MonoisotopicMass < 20)
                            {
                                // The protein mass is likely 18.0105633, which indicates that the residue masses are all zero
                                // Abort adding this protein collection

                                LastErrorMessage = string.Format("Computed mass for protein {0} is {1:F1}, which is too low; aborting upload of {2}",
                                    proteinName,
                                    seqInfo.MonoisotopicMass,
                                    mFASTAFilePath);

                                return false;
                            }

                            recordCount++;

                            if (recordCount % 100 == 0)
                            {
                                // trigger progress bar update every 100th record
                                LoadProgress?.Invoke((float)(currentPosition / (double)fileLength));
                            }

                            fastaContents.AddProtein(new ProteinStorageEntry(
                                proteinName, description, sequence.ToString(), seqInfo.SequenceLength,
                                seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                                seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount));
                        }

                        proteinName = string.Empty;
                        description = string.Empty;
                        sequence.Clear();

                        Match descMatch;

                        if (mDescLineRegEx.IsMatch(dataLine))
                        {
                            descMatch = mDescLineRegEx.Match(dataLine);
                            proteinName = descMatch.Groups["name"].Value;
                            description = descMatch.Groups["description"].Value;
                        }
                        else if (mNoDescLineRegEx.IsMatch(dataLine))
                        {
                            descMatch = mNoDescLineRegEx.Match(dataLine);
                            proteinName = descMatch.Groups[1].Value;
                            description = string.Empty;
                        }
                    }
                    else
                    {
                        sequence.Append(dataLine);
                    }

                    if (numRecordsToLoad > 0 && recordCount >= numRecordsToLoad - 1)
                    {
                        break;
                    }

                    currentPosition += dataLine.Length + lineEndCharCount;
                }

                if (sequence.Length > 0)
                {
                    // dump the last record
                    seqInfo.CalculateSequenceInfo(sequence.ToString());
                    recordCount++;

                    fastaContents.AddProtein(new ProteinStorageEntry(
                        proteinName, description, sequence.ToString(), seqInfo.SequenceLength,
                        seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                        seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount));
                }

                LoadEnd?.Invoke();

                if (recordCount > 0)
                {
                    return true;
                }

                LastErrorMessage = string.Format("FASTA file does not have any proteins: {0}", mFASTAFilePath);
                return false;
            }
            catch (Exception ex)
            {
                var stackTrace = StackTraceFormatter.GetExceptionStackTrace(ex);
                LastErrorMessage = string.Format("Error loading the FASTA file: {0}; {1}", ex.Message, stackTrace);
                return false;
            }
        }

        protected int LineEndCharacterCount(string filePath)
        {
            var fastaFile = new FileInfo(filePath);

            if (!fastaFile.Exists)
                return 2;

            using var fileReader = new StreamReader(new FileStream(fastaFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            while (!fileReader.EndOfStream)
            {
                var testCode = fileReader.Read();

                if (testCode == 10 || testCode == 13)
                {
                    if (fileReader.EndOfStream)
                    {
                        return 1;
                    }

                    var testCode2 = fileReader.Read();

                    if (testCode2 == 10 || testCode2 == 13)
                    {
                        return 2;
                    }

                    return 1;
                }
            }

            return 2;
        }
    }
}