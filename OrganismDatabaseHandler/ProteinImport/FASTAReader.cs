using System;
using System.IO;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.ProteinStorage;
using OrganismDatabaseHandler.SequenceInfo;

namespace OrganismDatabaseHandler.ProteinImport
{
    public class FASTAReader
    {
        private string mFASTAFilePath;

        private string mLastError;
        private readonly Regex mDescLineRegEx;
        private readonly Regex mNoDescLineRegEx;
        private readonly Regex mDescLineMatcher;

        #region "Events"

        public event LoadStartEventHandler LoadStart;

        public delegate void LoadStartEventHandler(string taskTitle);

        public event LoadEndEventHandler LoadEnd;

        public delegate void LoadEndEventHandler();

        public event LoadProgressEventHandler LoadProgress;

        public delegate void LoadProgressEventHandler(double fractionDone);

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

        public string LastErrorMessage => mLastError;

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
            var sequence = string.Empty;

            var seqInfo = new SequenceInfoCalculator();

            var recordCount = default(int);

            mFASTAFilePath = filePath;

            var lineEndCharCount = LineEndCharacterCount(filePath);

            try
            {
                var fi = new FileInfo(mFASTAFilePath);
                var fileLength = (int)fi.Length;
                if (fi.Exists && fileLength > 0)
                {
                    LoadStart?.Invoke("Reading Source File..."); // Trigger the setup of the pgb

                    using (var fileReader = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        var s = fileReader.ReadLine()?.Trim();

                        while (s != null)
                        {
                            if (mDescLineMatcher.IsMatch(s))
                            {
                                // DescriptionLine, new record
                                if (currentPosition > 0) // dump current record
                                {
                                    seqInfo.CalculateSequenceInfo(sequence);
                                    recordCount++;
                                    if (recordCount % 100 == 0)
                                    {
                                        LoadProgress?.Invoke((float)(currentPosition / (double)fileLength));     // trigger pgb update every 10th record
                                    }

                                    fastaContents.AddProtein(new ProteinStorageEntry(
                                        reference, description, sequence, seqInfo.SequenceLength,
                                        seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                                        seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount));
                                }

                                reference = string.Empty;
                                description = string.Empty;
                                sequence = string.Empty;

                                Match descMatch;
                                if (mDescLineRegEx.IsMatch(s))
                                {
                                    descMatch = mDescLineRegEx.Match(s);
                                    reference = descMatch.Groups["name"].Value;
                                    description = descMatch.Groups["description"].Value;
                                }
                                else if (mNoDescLineRegEx.IsMatch(s))
                                {
                                    descMatch = mNoDescLineRegEx.Match(s);
                                    reference = descMatch.Groups[1].Value;
                                    description = string.Empty;
                                }
                            }
                            else
                            {
                                sequence += s;
                            }

                            if (numRecordsToLoad > 0 && recordCount >= numRecordsToLoad - 1)
                            {
                                break;
                            }

                            currentPosition += s.Length + lineEndCharCount;

                            if (fileReader.EndOfStream)
                            {
                                break;
                            }

                            s = fileReader.ReadLine()?.Trim();
                        }

                        // dump the last record
                        seqInfo.CalculateSequenceInfo(sequence);
                        recordCount++;

                        fastaContents.AddProtein(new ProteinStorageEntry(
                            reference, description, sequence, seqInfo.SequenceLength,
                            seqInfo.MonoisotopicMass, seqInfo.AverageMass,
                            seqInfo.MolecularFormula, seqInfo.SHA1Hash, recordCount));

                        LoadEnd?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                var stackTrace = PRISM.StackTraceFormatter.GetExceptionStackTrace(ex);
                mLastError = ex.Message + "; " + stackTrace;
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