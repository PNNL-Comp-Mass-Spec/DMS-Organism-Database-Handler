using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Bulk_Fasta_Importer.Properties;
using Microsoft.VisualBasic;
using PRISM;
using ValidateFastaFile;

namespace Bulk_Fasta_Importer
{
    /// <summary>
    /// This program can be used to load one or more FASTA files into the Protein Sequences database
    /// </summary>
    static class Program
    {
        public const string PROGRAM_DATE = "February 18, 2020";

        private static string mInputFilePath;
        private static bool mPreviewMode;
        private static int mMaxProteinNameLength = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH;

        private static bool mLogMessagesToFile;
        private static string mLogFilePath;

        private static bool mQuietMode;

        private static DateTime mLastProgressReportTime;
        private static int mLastProgressReportValue;

        public static int Main()
        {
            // Returns 0 if no error, error code if an error

            int returnCode;
            var commandLineParser = new clsParseCommandLine();
            bool proceed;

            // Initialize the options
            mInputFilePath = string.Empty;
            mPreviewMode = false;

            mQuietMode = false;
            mLogMessagesToFile = false;
            mLogFilePath = string.Empty;
            try
            {
                proceed = false;
                if (commandLineParser.ParseCommandLine())
                {
                    if (SetOptionsUsingCommandLineParameters(commandLineParser))
                        proceed = true;
                }

                if (!proceed ||
                    commandLineParser.NeedToShowHelp ||
                    commandLineParser.ParameterCount + commandLineParser.NonSwitchParameterCount == 0 ||
                    mInputFilePath.Length == 0)
                {
                    ShowProgramHelp();
                    return -1;
                }

                // Data Source=proteinseqs;Initial Catalog=Protein_Sequences
                string proteinSeqsConnectionString = Settings.Default.ProteinSeqsDBConnectStr;

                // Data Source=dms5;Initial Catalog=Protein_Sequences
                string dmsConnectionString = Settings.Default.DMSConnectStr;

                var fastaImporter = new BulkFastaImporter(dmsConnectionString, proteinSeqsConnectionString)
                {
                    PreviewMode = mPreviewMode,
                    ValidationMaxProteinNameLength = mMaxProteinNameLength
                };

                fastaImporter.ProgressUpdate += BulkImporter_ProgressChanged;
                fastaImporter.ProgressReset += BulkImporter_ProgressReset;

                fastaImporter.LogMessagesToFile = mLogMessagesToFile;
                if (!string.IsNullOrEmpty(mLogFilePath))
                    fastaImporter.LogFilePath = mLogFilePath;

                string outputFolderNamePlaceholder = string.Empty;
                string paramFilePathPlaceholder = string.Empty;

                if (fastaImporter.ProcessFilesWildcard(mInputFilePath, outputFolderNamePlaceholder, paramFilePathPlaceholder))
                {
                    returnCode = 0;
                }
                else
                {
                    returnCode = (int)fastaImporter.ErrorCode;
                    if (returnCode != 0 && !mQuietMode)
                    {
                        Console.WriteLine("Error while processing: " + fastaImporter.GetErrorMessage());
                    }
                }

                DisplayProgressPercent(mLastProgressReportValue, true);

                return returnCode;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error occurred in modMain->Main: " + ControlChars.NewLine + ex.Message);
                return -1;
            }
        }

        private static void DisplayProgressPercent(int percentComplete, bool addCarriageReturn)
        {
            if (addCarriageReturn)
            {
                Console.WriteLine();
            }

            if (percentComplete > 100)
                percentComplete = 100;
            Console.Write("Processing: " + percentComplete.ToString() + "% ");
            if (addCarriageReturn)
            {
                Console.WriteLine();
            }
        }

        private static string GetAppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString() + " (" + PROGRAM_DATE + ")";
        }

        private static bool SetOptionsUsingCommandLineParameters(clsParseCommandLine commandLineParser)
        {
            // Returns True if no problems; otherwise, returns false

            string strValue = string.Empty;
            var strValidParameters = new string[] { "I", "L", "Preview", "MaxLength" };
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
                    if (commandLineParser.RetrieveValueForParameter("I", out strValue))
                    {
                        mInputFilePath = strValue;
                    }
                    else if (commandLineParser.NonSwitchParameterCount > 0)
                    {
                        mInputFilePath = commandLineParser.RetrieveNonSwitchParameter(0);
                    }

                    if (commandLineParser.RetrieveValueForParameter("L", out strValue))
                    {
                        mLogMessagesToFile = true;
                        if (!string.IsNullOrEmpty(strValue))
                        {
                            mLogFilePath = strValue;
                        }
                    }

                    if (commandLineParser.IsParameterPresent("Preview"))
                        mPreviewMode = true;

                    if (commandLineParser.RetrieveValueForParameter("MaxLength", out strValue))
                    {
                        if (!int.TryParse(strValue, out mMaxProteinNameLength))
                        {
                            ShowErrorMessage("Integer not found for the /MaxLength switch");
                            return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error parsing the command line parameters: " + ControlChars.NewLine + ex.Message);
                return false;
            }
        }

        private static void ShowErrorMessage(string strMessage)
        {
            string strSeparator = "------------------------------------------------------------------------------";

            Console.WriteLine();
            Console.WriteLine(strSeparator);
            Console.WriteLine(strMessage);
            Console.WriteLine(strSeparator);
            Console.WriteLine();
        }

        private static void ShowProgramHelp()
        {
            try
            {
                Console.WriteLine("This program reads a tab delimited file containing a list of FASTA files to import into the Protein_Sequences database in DMS.");
                Console.WriteLine();

                Console.WriteLine("Program syntax:");
                Console.WriteLine(Path.GetFileName(Assembly.GetExecutingAssembly().Location) +
                                  " FastaInfoFile.txt [/MaxLength:##] [/Preview] [/L]");
                Console.WriteLine();
                Console.WriteLine("FastaInfoFile.txt is a tab delimited text file listing the FASTA files to import");
                Console.WriteLine("Required columns are: FastaFilePath, OrganismName_or_ID, and AnnotationTypeName_or_ID");
                Console.WriteLine();
                Console.WriteLine("Use /MaxLength to define the maximum allowable length for protein names");
                Console.WriteLine("The default is /MaxLength:" + clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH);
                Console.WriteLine();

                Console.WriteLine("Use /Preview to see the fasta files that would be imported");
                Console.WriteLine("Use /L to log messages to a file; optionally specify the filename using /L:FilePath");
                Console.WriteLine();

                Console.WriteLine("Program written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2014");
                Console.WriteLine("Version: " + GetAppVersion());
                Console.WriteLine();

                Console.WriteLine("E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov");
                Console.WriteLine("Website: http://panomics.pnnl.gov/ or http://omics.pnl.gov");
                Console.WriteLine();

                // Delay for 750 msec in case the user double clicked this file from within Windows Explorer (or started the program via a shortcut)
                Thread.Sleep(750);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error displaying the program syntax: " + ex.Message);
            }
        }

        private static void BulkImporter_ProgressChanged(string taskDescription, float percentComplete)
        {
            const int PERCENT_REPORT_INTERVAL = 25;
            const int PROGRESS_DOT_INTERVAL_MSEC = 250;

            if (percentComplete >= mLastProgressReportValue)
            {
                if (mLastProgressReportValue > 0)
                {
                    Console.WriteLine();
                }

                DisplayProgressPercent(mLastProgressReportValue, false);
                mLastProgressReportValue += PERCENT_REPORT_INTERVAL;
                mLastProgressReportTime = DateTime.UtcNow;
            }
            else if (DateTime.UtcNow.Subtract(mLastProgressReportTime).TotalMilliseconds > PROGRESS_DOT_INTERVAL_MSEC)
            {
                mLastProgressReportTime = DateTime.UtcNow;
                Console.Write(".");
            }
        }

        private static void BulkImporter_ProgressReset()
        {
            mLastProgressReportTime = DateTime.UtcNow;
            mLastProgressReportValue = 0;
        }
    }
}
