using System;
using FastaFileMaker.Properties;
using OrganismDatabaseHandler.ProteinExport;
using PRISM;
using PRISM.Logging;

namespace FastaFileMaker
{
    internal static class Program
    {
        // Ignore Spelling: fasta, filetype, na, proteinseqs, Shewanella, yyyy-MM-dd, hh:mm:ss tt

        public static readonly string ProgramDate;

        static Program()
        {
            // ReSharper disable once StringLiteralTypo
            ProgramDate = ThisAssembly.GitCommitDate.ToLocalTime().ToString("MMMM dd, yyyy");
        }

        private const int DebugLevel = 4;
        private const int FastaGenTimeoutIntervalMinutes = 70;
        private const string DefaultCollectionOptions = "seq_direction=forward,filetype=fasta";
        private static GetFASTAFromDMS mFastaTools;
        private static string mFastaToolsCnStr = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;";
        private static string mMessage;
        private static string mFastaFileName;
        private static System.Timers.Timer mFastaTimer;
        private static bool mFastaGenTimeOut;
        private static bool mGenerationComplete;

        private static DateTime mFastaGenStartTime = DateTime.UtcNow;

        private static string mProteinCollectionList;
        private static string mCreationOpts;
        private static string mLegacyFasta;
        private static string mOutputDirectory;
        private static bool mLogProteinFileDetails;

        #region "Event handlers"

        private static void FastaTools_DebugEvent(string message)
        {
            ConsoleMsgUtils.ShowDebug(message);
        }

        private static void FastaTools_ErrorEvent(string message, Exception ex)
        {
            ConsoleMsgUtils.ShowError(message, ex);
        }

        private static void FastaTools_StatusEvent(string message)
        {
            Console.WriteLine(message);
        }

        private static void FastaTools_WarningEvent(string message)
        {
            ConsoleMsgUtils.ShowWarning(message);
        }

        private static void FastaTools_FileGenerationStarted(string taskMsg)
        {
        }

        private static void FastaTools_FileGenerationCompleted(string fullOutputPath)
        {
            mFastaFileName = System.IO.Path.GetFileName(fullOutputPath);
            mGenerationComplete = true;
        }

        private static DateTime mLastLogTime = DateTime.UtcNow;
        private static double mFractionDoneSaved;

        private static void FastaTools_FileGenerationProgress(string statusMsg, double fractionDone)
        {
            const int minimumLogIntervalSec = 15;

            if (DebugLevel >= 3)
            {
                // Limit the logging to once every minimumLogIntervalSec seconds
                if (DateTime.UtcNow.Subtract(dtLastLogTime).TotalSeconds >= minimumLogIntervalSec ||
                    fractionDone - dblFractionDoneSaved >= 0.25d)
                {
                    dtLastLogTime = DateTime.UtcNow;
                    dblFractionDoneSaved = fractionDone;
                    Console.WriteLine("Generating Fasta file, " + (fractionDone * 100d).ToString("0.0") + "% complete, " + statusMsg);
                }
            }
        }

        private static void FastaTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.UtcNow.Subtract(mFastaGenStartTime).TotalMinutes < FastaGenTimeoutIntervalMinutes)
                return;

            // Set the timeout flag so an error will be reported
            mFastaGenTimeOut = true;

            // Set the completion flag so the FASTA generation wait loop will exit
            mGenerationComplete = true;
        }

        #endregion

        public static void Main()
        {
            var commandLineParser = new clsParseCommandLine();

            try
            {
                var blnProceed = false;

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
                        mCreationOpts = DefaultCollectionOptions;
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
                    var proteinSeqsConnectionString = Settings.Default.ProteinSeqsDBConnectStr;

                    if (!string.IsNullOrWhiteSpace(proteinSeqsConnectionString))
                    {
                        mFastaToolsCnStr = proteinSeqsConnectionString;
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
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " (" + programDate + ")";
        }

        private static string GetHumanReadableTimeInterval(TimeSpan dtInterval)
        {
            if (dtInterval.TotalDays >= 1d)
            {
                // Report Days
                return dtInterval.TotalDays.ToString("0.00") + " days";
            }

            if (dtInterval.TotalHours >= 1d)
            {
                // Report hours
                return dtInterval.TotalHours.ToString("0.00") + " hours";
            }

            if (dtInterval.TotalMinutes >= 1d)
            {
                // Report minutes
                return dtInterval.TotalMinutes.ToString("0.00") + " minutes";
            }

            // Report seconds
            return dtInterval.TotalSeconds.ToString("0.0") + " seconds";
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

            var writeHeader = false;

            try
            {
                // Create a new log file each day
                var logFileName = "FastaFileMakerLog_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

                string logFilePath;

                if (!string.IsNullOrEmpty(logDirectoryPath))
                {
                    logFilePath = System.IO.Path.Combine(logDirectoryPath, logFileName);
                }
                else
                {
                    logFilePath = string.Copy(logFileName);
                }

                if (!System.IO.File.Exists(logFilePath))
                {
                    writeHeader = true;
                }

                using var writer = new System.IO.StreamWriter(new System.IO.FileStream(logFilePath, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read));

                if (writeHeader)
                {
                    writer.WriteLine("Date" + "\t" +
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

                if (!string.IsNullOrEmpty(destinationDirectoryPath))
                {
                    fiFastaFile = new System.IO.FileInfo(System.IO.Path.Combine(destinationDirectoryPath, fastaFileName));
                }
                else
                {
                    fiFastaFile = new System.IO.FileInfo(fastaFileName);
                }

                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd") + "\t" +
                                 DateTime.Now.ToString("hh:mm:ss tt") + "\t" +
                                 proteinCollectionList + "\t" +
                                 creationOpts + "\t" +
                                 legacyFasta + "\t" +
                                 crc32Hash + "\t" +
                                 fastaFileName + "\t" +
                                 fiFastaFile.LastWriteTime + "\t" +
                                 fiFastaFile.CreationTime + "\t" +
                                 fiFastaFile.Length + "\t" +
                                 GetHumanReadableTimeInterval(DateTime.UtcNow.Subtract(fiFastaFile.LastWriteTimeUtc)));
            }
            catch
            {
                // Ignore errors here
            }
        }

        private static void RegisterEvents(IEventNotifier sourceClass)
        {
            sourceClass.DebugEvent += FastaTools_DebugEvent;
            sourceClass.ErrorEvent += FastaTools_ErrorEvent;
            sourceClass.StatusEvent += FastaTools_StatusEvent;
            sourceClass.WarningEvent += FastaTools_WarningEvent;
        }

        private static bool SetOptionsUsingCommandLineParameters(clsParseCommandLine commandLineParser)
        {
            // Returns True if no problems; otherwise, returns false

            var strValidParameters = new[] { "P", "C", "L", "O", "D" };

            try
            {
                // Make sure no invalid parameters are present
                if (commandLineParser.InvalidParametersPresent(strValidParameters))
                {
                    return false;
                }

                // Query commandLineParser to see if various parameters are present
                if (commandLineParser.RetrieveValueForParameter("P", out var proteinCollectionList))
                    mProteinCollectionList = proteinCollectionList;

                if (commandLineParser.RetrieveValueForParameter("C", out var creationOptions))
                    mCreationOpts = creationOptions;

                if (commandLineParser.RetrieveValueForParameter("L", out var legacyFasta))
                    mLegacyFasta = legacyFasta;

                if (commandLineParser.RetrieveValueForParameter("O", out var outputDirectory))
                    mOutputDirectory = outputDirectory;

                if (commandLineParser.IsParameterPresent("D"))
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

                        if (mProteinCollectionList.EndsWith(".fasta", StringComparison.OrdinalIgnoreCase) ||
                            mProteinCollectionList.EndsWith(".fasta\"", StringComparison.OrdinalIgnoreCase))
                        {
                            // User specified a .fasta file
                            mLegacyFasta = string.Copy(mProteinCollectionList);
                            mProteinCollectionList = string.Empty;
                        }
                    }
                }

                return true;
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
                var exeName = System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);

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
                    "The default is: " + DefaultCollectionOptions));
                Console.WriteLine();
                Console.WriteLine("To export a legacy FASTA file, use /L, for example /L:FileName.fasta");
                Console.WriteLine();
                Console.WriteLine("Optionally use /O to specify the output directory.");
                Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                    "Optionally use /D to log the details of the protein collections, options, and resultant file to a log file."));
                Console.WriteLine();

                Console.WriteLine("Program written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)");
                Console.WriteLine("Version: " + GetAppVersion(ProgramDate));
                Console.WriteLine();

                Console.WriteLine("E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov");
                Console.WriteLine("Website: https://github.com/PNNL-Comp-Mass-Spec/ or https://panomics.pnnl.gov/ or https://www.pnnl.gov/integrative-omics");
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
                                      bool logProteinFileDetails)
        {
            string crc32Hash;

            // Instantiate FASTA tool if not already done
            if (mFastaTools == null)
            {
                if (string.IsNullOrEmpty(mFastaToolsCnStr))
                {
                    mMessage = "Protein database connection string not specified";
                    Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " + mMessage);
                    return false;
                }

                mFastaTools = new GetFASTAFromDMS(mFastaToolsCnStr);
                RegisterEvents(mFastaTools);

                mFastaTools.FileGenerationStarted += FastaTools_FileGenerationStarted;
                mFastaTools.FileGenerationCompleted += FastaTools_FileGenerationCompleted;
                mFastaTools.FileGenerationProgress += FastaTools_FileGenerationProgress;
            }

            mFastaTimer = new System.Timers.Timer();
            mFastaTimer.Elapsed += FastaTimer_Elapsed;
            mFastaTimer.Interval = 5000d;
            mFastaTimer.AutoReset = true;

            // Note that mFastaTools does not spawn a new thread
            // Since it does not spawn a new thread, the while loop after this Try block won't actually get reached while mFastaTools.ExportFASTAFile is running
            // Furthermore, even if mFastaTimer_Elapsed sets mFastaGenTimeOut to True, this won't do any good since mFastaTools.ExportFASTAFile will still be running
            mFastaGenTimeOut = false;
            mFastaGenStartTime = DateTime.UtcNow;
            try
            {
                mFastaTimer.Start();
                crc32Hash = mFastaTools.ExportFASTAFile(proteinCollectionList, creationOpts, legacyFasta, destinationDirectoryPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("clsAnalysisResources.CreateFastaFile(), Exception generating OrgDb file: " + ex.Message);
                Console.WriteLine(StackTraceFormatter.GetExceptionStackTraceMultiLine(ex));
                return false;
            }

            // Wait for FASTA creation to finish
            while (!mGenerationComplete)
            {
                System.Threading.Thread.Sleep(2000);

                if (DateTime.UtcNow.Subtract(mFastaGenStartTime).TotalMinutes >= FastaGenTimeoutIntervalMinutes)
                {
                    mFastaGenTimeOut = true;
                    break;
                }
            }

            mFastaTimer.Stop();

            if (mFastaGenTimeOut)
            {
                // Fasta generator hung - report error and exit
                mMessage = "Timeout error while generating FASTA file (" + FastaGenTimeoutIntervalMinutes + " minutes have elapsed)";
                Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " + mMessage);

                return false;
            }

            if (logProteinFileDetails)
            {
                LogProteinFileDetails(proteinCollectionList, creationOpts, legacyFasta, crc32Hash, destinationDirectoryPath, mFastaFileName, string.Empty);
            }

            return true;
        }
    }
}
