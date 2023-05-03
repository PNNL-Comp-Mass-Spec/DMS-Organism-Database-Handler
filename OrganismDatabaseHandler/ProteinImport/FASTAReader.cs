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
        private string mFASTAFilePath;

        private readonly Regex mDescLineRegEx;
        private readonly Regex mNoDescLineRegEx;
        private readonly Regex mDescLineMatcher;

        #region "Events"

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadProgressEventHandler LoadProgress;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public FASTAReader()
        {
            mDescLineMatcher = new Regex(@"^\>.+$");
            mDescLineRegEx = new Regex(@"^\>(?<name>\S+)\s+(?<description>.*)$");
            mNoDescLineRegEx = new Regex(@"^\>(?<name>\S+)$");
        }

        public string LastErrorMessage { get; private set; }

        public ProteinStorage.ProteinStorage GetProteinEntries(string filePath)
        {
            return LoadFASTAFile(filePath, -1);
        }

        public ProteinStorage.ProteinStorage GetProteinEntries(string filePath, int numRecordsToLoad)
        {
            return LoadFASTAFile(filePath, numRecordsToLoad);
        }

        public ProteinStorage.ProteinStorage LoadFASTAFile(string filePath)
        {
            return LoadFASTAFile(filePath, -1);
        }

        public ProteinStorage.ProteinStorage LoadFASTAFile(string filePath, int numRecordsToLoad)
        {
            var currentPosition = 0;

            var fastaContents = new ProteinStorage.ProteinStorage(filePath);

            var reference = string.Empty;
            var description = string.Empty;
            var sequence = new StringBuilder();

            var seqInfo = new SequenceInfoCalculator();
            RegisterEvents(seqInfo);

            var recordCount = 0;

            mFASTAFilePath = filePath;

            var lineEndCharCount = LineEndCharacterCount(filePath);

            try
            {
                var fastaFile = new FileInfo(mFASTAFilePath);
                var fileLength = fastaFile.Length;

                if (!fastaFile.Exists || fileLength == 0)
                {
                    return fastaContents;
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
                            recordCount++;

                            if (recordCount % 100 == 0)
                            {
                                // trigger progress bar update every 100th record
                                LoadProgress?.Invoke((float)(currentPosition / (double)fileLength));
                            }

                            fastaContents.AddProtein(new ProteinStorageEntry(
                                reference, description, sequence.ToString(), seqInfo.SequenceLength,
                                seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                                seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount));
                        }

                        reference = string.Empty;
                        description = string.Empty;
                        sequence.Clear();

                        Match descMatch;

                        if (mDescLineRegEx.IsMatch(dataLine))
                        {
                            descMatch = mDescLineRegEx.Match(dataLine);
                            reference = descMatch.Groups["name"].Value;
                            description = descMatch.Groups["description"].Value;
                        }
                        else if (mNoDescLineRegEx.IsMatch(dataLine))
                        {
                            descMatch = mNoDescLineRegEx.Match(dataLine);
                            reference = descMatch.Groups[1].Value;
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
                        reference, description, sequence.ToString(), seqInfo.SequenceLength,
                        seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                        seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount));
                }

                LoadEnd?.Invoke();
            }
            catch (Exception ex)
            {
                var stackTrace = PRISM.StackTraceFormatter.GetExceptionStackTrace(ex);
                LastErrorMessage = ex.Message + "; " + stackTrace;
            }

            return fastaContents;
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