using System;
using System.IO;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.ProteinStorage;
using OrganismDatabaseHandler.SequenceInfo;

namespace OrganismDatabaseHandler.ProteinImport
{
    public class FASTAReader
    {
        private string m_FASTAFilePath;

        private string m_LastError;
        private readonly Regex m_DescLineRegEx;
        private readonly Regex m_NoDescLineRegEx;
        private readonly Regex m_DescLineMatcher;

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
            m_DescLineMatcher = new Regex(@"^\>.+$");
            m_DescLineRegEx = new Regex(@"^\>(?<name>\S+)\s+(?<description>.*)$");
            m_NoDescLineRegEx = new Regex(@"^\>(?<name>\S+)$");
        }

        public string LastErrorMessage => m_LastError;

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
            int currentPosition = 0;

            var fastaContents = new ProteinStorage.ProteinStorage(filePath);

            string reference = string.Empty;
            string description = string.Empty;
            string sequence = string.Empty;

            var seqInfo = new SequenceInfoCalculator();

            var recordCount = default(int);

            m_FASTAFilePath = filePath;

            int lineEndCharCount = LineEndCharacterCount(filePath);

            try
            {
                var fi = new FileInfo(m_FASTAFilePath);
                var fileLength = (int)fi.Length;
                if (fi.Exists & fileLength > 0)
                {
                    LoadStart?.Invoke("Reading Source File..."); // Trigger the setup of the pgb

                    using (var fileReader = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        string s = fileReader.ReadLine()?.Trim();

                        while (s != null)
                        {
                            if (m_DescLineMatcher.IsMatch(s))
                            {
                                // DescriptionLine, new record
                                if (currentPosition > 0) // dump current record
                                {
                                    seqInfo.CalculateSequenceInfo(sequence);
                                    recordCount += 1;
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
                                if (m_DescLineRegEx.IsMatch(s))
                                {
                                    descMatch = m_DescLineRegEx.Match(s);
                                    reference = descMatch.Groups["name"].Value;
                                    description = descMatch.Groups["description"].Value;
                                }
                                else if (m_NoDescLineRegEx.IsMatch(s))
                                {
                                    descMatch = m_NoDescLineRegEx.Match(s);
                                    reference = descMatch.Groups[1].Value;
                                    description = string.Empty;
                                }
                            }
                            else
                            {
                                sequence += s;
                            }

                            if (numRecordsToLoad > 0 & recordCount >= numRecordsToLoad - 1)
                            {
                                break;
                            }

                            currentPosition += s.Length + lineEndCharCount;

                            if (fileReader.EndOfStream)
                            {
                                break;
                            }

                            s = fileReader.ReadLine().Trim();
                        }

                        // dump the last record
                        seqInfo.CalculateSequenceInfo(sequence);
                        recordCount += 1;

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
                string stackTrace = PRISM.StackTraceFormatter.GetExceptionStackTrace(ex);
                m_LastError = ex.Message + "; " + stackTrace;
            }

            return fastaContents;
        }

        protected int LineEndCharacterCount(string filePath)
        {
            var fi = new FileInfo(m_FASTAFilePath);
            if (fi.Exists)
            {
                using (var fileReader = new StreamReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    while (!fileReader.EndOfStream)
                    {
                        int testCode = fileReader.Read();
                        if (testCode == 10 || testCode == 13)
                        {
                            if (fileReader.EndOfStream)
                            {
                                return 1;
                            }

                            int testCode2 = fileReader.Read();
                            if (testCode2 == 10 | testCode2 == 13)
                            {
                                return 2;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    }
                }
            }

            return 2;
        }
    }
}