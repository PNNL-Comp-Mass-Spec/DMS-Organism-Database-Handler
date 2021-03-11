using System;
using FastaFileMaker_Exe.Properties;
using OrganismDatabaseHandler.ProteinExport;
using PRISM;

namespace FastaFileMaker_Exe
{
    static class Program
    {
        public const string PROGRAM_DATE = "February 18, 2020";

        private const int m_DebugLevel = 4;
        private const int FASTA_GEN_TIMEOUT_INTERVAL_MINUTES = 70;
        private const string DEFAULT_COLLECTION_OPTIONS = "seq_direction=forward,filetype=fasta";
        private static GetFASTAFromDMS m_FastaTools;
        private static string m_FastaToolsCnStr = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;";
        private static string m_message;
        private static string m_FastaFileName;
        private static System.Timers.Timer m_FastaTimer;
        private static bool m_FastaGenTimeOut;
        private static bool m_GenerationComplete = false;

        private static DateTime m_FastaGenStartTime = DateTime.UtcNow;

        private static string mProteinCollectionList;
        private static string mCreationOpts;
        private static string mLegacyFasta;
        private static string mOutputDirectory;
        private static bool mLogProteinFileDetails;

        #region "Event handlers"

        private static void m_FastaTools_DebugEvent(string message)
        {
            ConsoleMsgUtils.ShowDebug(message);
        }

        private static void m_FastaTools_ErrorEvent(string message, Exception ex)
        {
            ConsoleMsgUtils.ShowError(message, ex);
        }

        private static void m_FastaTools_StatusEvent(string message)
        {
            Console.WriteLine(message);
        }

        private static void m_FastaTools_WarningEvent(string message)
        {
            ConsoleMsgUtils.ShowWarning(message);
        }

        private static void m_FastaTools_FileGenerationStarted(string taskMsg)
        {
        }

        private static void m_FastaTools_FileGenerationCompleted(string fullOutputPath)
        {
            m_FastaFileName = System.IO.Path.GetFileName(fullOutputPath);  // Get the name of the fasta file that was generated
            m_GenerationComplete = true;     // Set the completion flag
        }

        private static DateTime dtLastLogTime = DateTime.MinValue;
        private static double dblFractionDoneSaved = -1;

        private static void m_FastaTools_FileGenerationProgress(string statusMsg, double fractionDone)
        {
            const int MINIMUM_LOG_INTERVAL_SEC = 15;

            if (m_DebugLevel >= 3)
            {
                // Limit the logging to once every MINIMUM_LOG_INTERVAL_SEC seconds
                if (DateTime.UtcNow.Subtract(dtLastLogTime).TotalSeconds >= (double)MINIMUM_LOG_INTERVAL_SEC ||
                    fractionDone - dblFractionDoneSaved >= 0.25d)
                {
                    dtLastLogTime = DateTime.UtcNow;
                    dblFractionDoneSaved = fractionDone;
                    Console.WriteLine("Generating Fasta file, " + (fractionDone * 100d).ToString("0.0") + "% complete, " + statusMsg);
                }
            }
        }

        private static void m_FastaTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.UtcNow.Subtract(m_FastaGenStartTime).TotalMinutes >= FASTA_GEN_TIMEOUT_INTERVAL_MINUTES)
            {
                m_FastaGenTimeOut = true;        // Set the timeout flag so an error will be reported
                m_GenerationComplete = true;     // Set the completion flag so the fasta generation wait loop will exit
            }
        }

        #endregion

        public static void Main()
        {
            var commandLineParser = new clsParseCommandLine();
            bool blnProceed;

            try
            {
                blnProceed = false;

                mProteinCollectionList = string.Empty;
                mCreationOpts = string.Empty;
                mLegacyFasta = string.Empty;
                mOutputDirectory = string.Empty;
                mLogProteinFileDetails = false;

                if (commandLineParser.ParseCommandLine())
                {
                    if (SetOptionsUsingCommandLineParameters(commandLineParser))
                        blnProceed = true;
                }

                if (!blnProceed ||
                    commandLineParser.NeedToShowHelp ||
                    commandLineParser.ParameterCount + commandLineParser.NonSwitchParameterCount == 0 ||
                    (mProteinCollectionList.Length == 0 && mLegacyFasta.Length == 0))
                {
                    ShowProgramHelp();
                }
                else
                {
                    // To hard-code defaults, enter them here
                    // mProteinCollectionList = "Shewanella_2006-07-11";
                    // mCreationOpts = "seq_direction=forward,filetype=fasta";
                    // mLegacyFasta = "na";
                    // mOutputDirectory = "C:\DMS_Temp_Org";
                    // mLogProteinFileDetails = True;

                    if (mLegacyFasta.Length == 0)
                    {
                        mLegacyFasta = "na";
                    }

                    if (mCreationOpts.Length == 0)
                    {
                        mCreationOpts = DEFAULT_COLLECTION_OPTIONS;
                    }

                    if (mOutputDirectory.Length == 0)
                    {
                        mOutputDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    }

                    if (mProteinCollectionList.IndexOf(',') > 0 &&
                        mProteinCollectionList.IndexOf(".fasta", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        ConsoleMsgUtils.ShowError("Protein collection list should not have file extension .fasta; edit the list: " + mProteinCollectionList);
                        return;
                    }

                    if (mProteinCollectionList.IndexOf(',') < 0 &&
                        mProteinCollectionList.EndsWith(".fasta", StringComparison.OrdinalIgnoreCase))
                    {
                        ConsoleMsgUtils.ShowWarning("Auto removing '.fasta' from the protein collection name");
                        mProteinCollectionList = mProteinCollectionList.Substring(0, mProteinCollectionList.Length - ".fasta".Length);
                    }

                    // Data Source=proteinseqs;Initial Catalog=Protein_Sequences
                    string proteinSeqsConnectionString = Settings.Default.ProteinSeqsDBConnectStr;

                    if (!string.IsNullOrWhiteSpace(proteinSeqsConnectionString))
                    {
                        m_FastaToolsCnStr = proteinSeqsConnectionString;
                    }

                    TestExport(mProteinCollectionList, mCreationOpts, mLegacyFasta, mOutputDirectory, mLogProteinFileDetails);

                    Console.WriteLine("Destination directory: " + mOutputDirectory);
                }
            }
            catch (Exception ex)
            {
                ConsoleMsgUtils.ShowError("Error occurred in modMain", ex);
            }
        }

        private static string GetAppVersion(string programDate)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (" + programDate + ")";
        }

        private static string GetHumanReadableTimeInterval(TimeSpan dtInterval)
        {
            if (dtInterval.TotalDays >= 1d)
            {
                // Report Days
                return dtInterval.TotalDays.ToString("0.00") + " days";
            }
            else if (dtInterval.TotalHours >= 1d)
            {
                // Report hours
                return dtInterval.TotalHours.ToString("0.00") + " hours";
            }
            else if (dtInterval.TotalMinutes >= 1d)
            {
                // Report minutes
                return dtInterval.TotalMinutes.ToString("0.00") + " minutes";
            }
            else
            {
                // Report seconds
                return dtInterval.TotalSeconds.ToString("0.0") + " seconds";
            }
        }

        private static void LogProteinFileDetails(string proteinCollectionList,
                                                  string creationOpts,
                                                  string legacyFasta,
                                                  string crc32Hash,
                                                  string destinationDirectoryPath,
                                                  string fastaFileName,
                                                  string logDirectoryPath)
        {
            // Appends a new entry to the log file
            System.IO.StreamWriter swOutFile;

            string strLogFileName;
            string strLogFilePath;
            bool blnWriteHeader = false;

            try
            {
                // Create a new log file each day
                strLogFileName = "FastaFileMakerLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

                if (logDirectoryPath != null && logDirectoryPath.Length > 0)
                {
                    strLogFilePath = System.IO.Path.Combine(logDirectoryPath, strLogFileName);
                }
                else
                {
                    strLogFilePath = string.Copy(strLogFileName);
                }

                if (!System.IO.File.Exists(strLogFilePath))
                {
                    blnWriteHeader = true;
                }

                swOutFile = new System.IO.StreamWriter(new System.IO.FileStream(strLogFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read));

                if (blnWriteHeader)
                {
                    swOutFile.WriteLine("Date" + "\t" +
                                        "Time" + "\t" +
                                        "Protein_Collection_List" + "\t" +
                                        "Creation_Options" + "\t" +
                                        "Legacy_Fasta_Name" + "\t" +
                                        "Hash_String" + "\t" +
                                        "Fasta_File_Name" + "\t" +
                                        "Fasta_File_Last_Modified" + "\t" +
                                        "Fasta_File_Created" + "\t" +
                                        "Fasta_File_Size_Bytes" + "\t" +
                                        "Fasta_File_Age_vs_PresentTime");
                }

                System.IO.FileInfo fiFastaFile;
                if (destinationDirectoryPath != null && destinationDirectoryPath.Length > 0)
                {
                    fiFastaFile = new System.IO.FileInfo(System.IO.Path.Combine(destinationDirectoryPath, fastaFileName));
                }
                else
                {
                    fiFastaFile = new System.IO.FileInfo(fastaFileName);
                }

                swOutFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd") + "\t" +
                                    DateTime.Now.ToString("hh:mm:ss tt") + "\t" +
                                    proteinCollectionList + "\t" +
                                    creationOpts + "\t" +
                                    legacyFasta + "\t" +
                                    crc32Hash + "\t" +
                                    fastaFileName + "\t" +
                                    fiFastaFile.LastWriteTime.ToString() + "\t" +
                                    fiFastaFile.CreationTime.ToString() + "\t" +
                                    fiFastaFile.Length + "\t" +
                                    GetHumanReadableTimeInterval(DateTime.UtcNow.Subtract(fiFastaFile.LastWriteTimeUtc)));

                if (swOutFile != null)
                {
                    swOutFile.Close();
                }
            }
            catch
            {
                // Ignore errors here
            }
        }

        private static bool SetOptionsUsingCommandLineParameters(clsParseCommandLine commandLineParser)
        {
            // Returns True if no problems; otherwise, returns false

            string strValue = string.Empty;
            var strValidParameters = new string[] { "P", "C", "L", "O", "D" };

            try
            {
                // Make sure no invalid parameters are present
                if (commandLineParser.InvalidParametersPresent(strValidParameters))
                {
                    return false;
                }
                else
                {
                    // Query commandLineParser to see if various parameters are present
                    if (commandLineParser.RetrieveValueForParameter("P", out strValue))
                        mProteinCollectionList = strValue;
                    if (commandLineParser.RetrieveValueForParameter("C", out strValue))
                        mCreationOpts = strValue;
                    if (commandLineParser.RetrieveValueForParameter("L", out strValue))
                        mLegacyFasta = strValue;
                    if (commandLineParser.RetrieveValueForParameter("O", out strValue))
                        mOutputDirectory = strValue;
                    if (commandLineParser.RetrieveValueForParameter("D", out strValue))
                        mLogProteinFileDetails = true;
                    if (mProteinCollectionList.Length > 0)
                    {
                        mLegacyFasta = string.Empty;
                    }
                    else if (mLegacyFasta.Length == 0)
                    {
                        // Neither /P nor /L were used

                        if (commandLineParser.NonSwitchParameterCount > 0)
                        {
                            // User specified a non-switch parameter
                            // Assume it is a protein collection list
                            mProteinCollectionList = commandLineParser.RetrieveNonSwitchParameter(0);
                            if (mProteinCollectionList.ToLower().EndsWith(".fasta") ||
                                mProteinCollectionList.ToLower().EndsWith(".fasta\""))
                            {
                                // User specified a .fasta file
                                mLegacyFasta = string.Copy(mProteinCollectionList);
                                mProteinCollectionList = string.Empty;
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing the command line parameters: " + Environment.NewLine + ex.Message);
                return false;
            }
        }

        private static void ShowProgramHelp()
        {
            try
            {
                string exeName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                    "This program can export protein collection(s) from the DMS Protein_Sequences database to create a .Fasta file. " +
                    "Alternatively, you can specify a legacy .Fasta file name to retrieve"));
                Console.WriteLine();
                Console.WriteLine("Program syntax:");
                Console.WriteLine("  " + exeName + " /P:ProteinCollectionList [/C:ProteinCollectionCreationOptions] [/O:OutputDirectory] [/D]");
                Console.WriteLine("   or   ");
                Console.WriteLine("  " + exeName + " /L:LegacyFastaFileName [/O:OutputDirectory] [/D]");
                Console.WriteLine();
                Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                    "To export one or more protein collections, specify the protein collection names as a comma separated list after the /P switch. " +
                    "When exporting protein collections, use optional switch /C to change the protein collection export options. " +
                    "The default is: " + DEFAULT_COLLECTION_OPTIONS));
                Console.WriteLine();
                Console.WriteLine("To export a legacy fasta file, use /L, for example /L:FileName.fasta");
                Console.WriteLine();
                Console.WriteLine("Optionally use /O to specify the output directory.");
                Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                    "Optionally use /D to log the details of the protein collections, options, and resultant file to a log file."));
                Console.WriteLine();

                Console.WriteLine("Program written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2009");
                Console.WriteLine("Version: " + GetAppVersion(PROGRAM_DATE));
                Console.WriteLine();

                Console.WriteLine("E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov");
                Console.WriteLine("Website: https://omics.pnl.gov/ or https://panomics.pnnl.gov/");
                Console.WriteLine();

                Console.WriteLine("Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License.  " +
                    "You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0");
                Console.WriteLine();

                // Delay for 750 msec in case the user double clicked this file from within Windows Explorer (or started the program via a shortcut)
                System.Threading.Thread.Sleep(750);
            }
            catch (Exception ex)
            {
                ConsoleMsgUtils.ShowError("Error displaying the program syntax", ex);
            }
        }

        public static bool TestExport(string proteinCollectionList,
                                      string creationOpts,
                                      string legacyFasta,
                                      string destinationDirectoryPath,
                                      bool blnLogProteinFileDetails)
        {
            string crc32Hash;

            // Instantiate fasta tool if not already done
            if (m_FastaTools == null)
            {
                if (string.IsNullOrEmpty(m_FastaToolsCnStr))
                {
                    m_message = "Protein database connection string not specified";
                    Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " + m_message);
                    return false;
                }

                m_FastaTools = new GetFASTAFromDMS(m_FastaToolsCnStr);
                m_FastaTools.DebugEvent += m_FastaTools_DebugEvent;
                m_FastaTools.ErrorEvent += m_FastaTools_ErrorEvent;
                m_FastaTools.StatusEvent += m_FastaTools_StatusEvent;
                m_FastaTools.WarningEvent += m_FastaTools_WarningEvent;
                m_FastaTools.FileGenerationStarted += m_FastaTools_FileGenerationStarted;
                m_FastaTools.FileGenerationCompleted += m_FastaTools_FileGenerationCompleted;
                m_FastaTools.FileGenerationProgress += m_FastaTools_FileGenerationProgress;
            }

            m_FastaTimer = new System.Timers.Timer();
            m_FastaTimer.Elapsed += m_FastaTimer_Elapsed;
            m_FastaTimer.Interval = 5000d;
            m_FastaTimer.AutoReset = true;

            // Note that m_FastaTools does not spawn a new thread
            // Since it does not spawn a new thread, the while loop after this Try block won't actually get reached while m_FastaTools.ExportFASTAFile is running
            // Furthermore, even if m_FastaTimer_Elapsed sets m_FastaGenTimeOut to True, this won't do any good since m_FastaTools.ExportFASTAFile will still be running
            m_FastaGenTimeOut = false;
            m_FastaGenStartTime = DateTime.UtcNow;
            try
            {
                m_FastaTimer.Start();
                crc32Hash = m_FastaTools.ExportFASTAFile(proteinCollectionList, creationOpts, legacyFasta, destinationDirectoryPath);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("clsAnalysisResources.CreateFastaFile(), Exception generating OrgDb file: " + Ex.Message);
                Console.WriteLine(StackTraceFormatter.GetExceptionStackTraceMultiLine(Ex));
                return false;
            }

            // Wait for fasta creation to finish
            while (!m_GenerationComplete)
            {
                System.Threading.Thread.Sleep(2000);
                if (DateTime.UtcNow.Subtract(m_FastaGenStartTime).TotalMinutes >= FASTA_GEN_TIMEOUT_INTERVAL_MINUTES)
                {
                    m_FastaGenTimeOut = true;
                    break;
                }
            }

            m_FastaTimer.Stop();
            if (m_FastaGenTimeOut)
            {
                // Fasta generator hung - report error and exit
                m_message = "Timeout error while generating OrdDb file (" + FASTA_GEN_TIMEOUT_INTERVAL_MINUTES.ToString() + " minutes have elapsed)";
                Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " + m_message);

                return false;
            }

            if (blnLogProteinFileDetails)
            {
                LogProteinFileDetails(proteinCollectionList, creationOpts, legacyFasta, crc32Hash, destinationDirectoryPath, m_FastaFileName, "");
            }

            return true;
        }
    }
}
