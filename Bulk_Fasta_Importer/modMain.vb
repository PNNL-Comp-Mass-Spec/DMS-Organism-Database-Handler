Option Strict On

Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports PRISM
Imports ValidateFastaFile

''' <summary>
''' This program can be used to load one or more FASTA files into the Protein Sequences database
''' </summary>
Module modMain

    Public Const PROGRAM_DATE As String = "March 5, 2019"

    Private mInputFilePath As String
    Private mPreviewMode As Boolean
    Private mMaxProteinNameLength As Integer = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH

    Private mLogMessagesToFile As Boolean
    Private mLogFilePath As String

    Private mQuietMode As Boolean

    Private mLastProgressReportTime As DateTime
    Private mLastProgressReportValue As Integer

    Public Function Main() As Integer
        ' Returns 0 if no error, error code if an error

        Dim returnCode As Integer
        Dim commandLineParser As New clsParseCommandLine()
        Dim proceed As Boolean

        ' Initialize the options
        mInputFilePath = String.Empty
        mPreviewMode = False

        mQuietMode = False
        mLogMessagesToFile = False
        mLogFilePath = String.Empty

        Try
            proceed = False
            If commandLineParser.ParseCommandLine Then
                If SetOptionsUsingCommandLineParameters(commandLineParser) Then proceed = True
            End If

            If Not proceed OrElse
               commandLineParser.NeedToShowHelp OrElse
               commandLineParser.ParameterCount + commandLineParser.NonSwitchParameterCount = 0 OrElse
               mInputFilePath.Length = 0 Then
                ShowProgramHelp()
                returnCode = -1
            Else

                Dim fastaImporter = New clsBulkFastaImporter With {
                    .PreviewMode = mPreviewMode,
                    .ValidationMaxProteinNameLength = mMaxProteinNameLength
                }

                AddHandler fastaImporter.ProgressUpdate, AddressOf BulkImporter_ProgressChanged
                AddHandler fastaImporter.ProgressReset, AddressOf BulkImporter_ProgressReset

                ' Data Source=proteinseqs;Initial Catalog=Protein_Sequences
                Dim proteinSeqsConnectionString = My.Settings.ProteinSeqsDBConnectStr
                Dim dmsConnectionString = My.Settings.DMSConnectStr

                If Not String.IsNullOrWhiteSpace(proteinSeqsConnectionString) Then
                    fastaImporter.ProteinSeqsConnectionString = proteinSeqsConnectionString
                End If

                If Not String.IsNullOrWhiteSpace(dmsConnectionString) Then
                    fastaImporter.DMSConnectionString = dmsConnectionString
                End If

                fastaImporter.LogMessagesToFile = mLogMessagesToFile
                If Not String.IsNullOrEmpty(mLogFilePath) Then fastaImporter.LogFilePath = mLogFilePath

                Dim outputFolderNamePlaceholder As String = String.Empty
                Dim paramFilePathPlaceholder As String = String.Empty

                If fastaImporter.ProcessFilesWildcard(mInputFilePath, outputFolderNamePlaceholder, paramFilePathPlaceholder) Then
                    returnCode = 0
                Else
                    returnCode = fastaImporter.ErrorCode
                    If returnCode <> 0 AndAlso Not mQuietMode Then
                        Console.WriteLine("Error while processing: " & fastaImporter.GetErrorMessage())
                    End If
                End If

                DisplayProgressPercent(mLastProgressReportValue, True)
            End If

        Catch ex As Exception
            ShowErrorMessage("Error occurred in modMain->Main: " & ControlChars.NewLine & ex.Message)
            returnCode = -1
        End Try

        Return returnCode

    End Function

    Private Sub DisplayProgressPercent(percentComplete As Integer, addCarriageReturn As Boolean)
        If addCarriageReturn Then
            Console.WriteLine()
        End If
        If percentComplete > 100 Then percentComplete = 100
        Console.Write("Processing: " & percentComplete.ToString & "% ")
        If addCarriageReturn Then
            Console.WriteLine()
        End If
    End Sub

    Private Function GetAppVersion() As String
        Return Assembly.GetExecutingAssembly.GetName.Version.ToString & " (" & PROGRAM_DATE & ")"
    End Function

    Private Function SetOptionsUsingCommandLineParameters(commandLineParser As clsParseCommandLine) As Boolean
        ' Returns True if no problems; otherwise, returns false

        Dim strValue As String = String.Empty
        Dim strValidParameters = New String() {"I", "L", "Preview", "MaxLength"}

        Try
            ' Make sure no invalid parameters are present
            If commandLineParser.InvalidParametersPresent(strValidParameters) Then
                Return False
            Else
                With commandLineParser
                    ' Query commandLineParser to see if various parameters are present
                    If .RetrieveValueForParameter("I", strValue) Then
                        mInputFilePath = strValue
                    ElseIf .NonSwitchParameterCount > 0 Then
                        mInputFilePath = .RetrieveNonSwitchParameter(0)
                    End If

                    If .RetrieveValueForParameter("L", strValue) Then
                        mLogMessagesToFile = True
                        If Not String.IsNullOrEmpty(strValue) Then
                            mLogFilePath = strValue
                        End If
                    End If

                    If .IsParameterPresent("Preview") Then mPreviewMode = True

                    If .RetrieveValueForParameter("MaxLength", strValue) Then
                        If Not Integer.TryParse(strValue, mMaxProteinNameLength) Then
                            ShowErrorMessage("Integer not found for the /MaxLength switch")
                            Return False
                        End If
                    End If

                End With

                Return True
            End If

        Catch ex As Exception
            ShowErrorMessage("Error parsing the command line parameters: " & ControlChars.NewLine & ex.Message)
            Return False
        End Try

    End Function

    Private Sub ShowErrorMessage(strMessage As String)
        Dim strSeparator = "------------------------------------------------------------------------------"

        Console.WriteLine()
        Console.WriteLine(strSeparator)
        Console.WriteLine(strMessage)
        Console.WriteLine(strSeparator)
        Console.WriteLine()

    End Sub

    Private Sub ShowProgramHelp()

        Try

            Console.WriteLine("This program reads a tab delimited file containing a list of FASTA files to import into the Protein_Sequences database in DMS.")
            Console.WriteLine()

            Console.WriteLine("Program syntax:")
            Console.WriteLine(Path.GetFileName(Assembly.GetExecutingAssembly().Location) &
                              " FastaInfoFile.txt [/MaxLength:##] [/Preview] [/L]")
            Console.WriteLine()
            Console.WriteLine("FastaInfoFile.txt is a tab delimited text file listing the FASTA files to import")
            Console.WriteLine("Required columns are: FastaFilePath, OrganismName_or_ID, and AnnotationTypeName_or_ID")
            Console.WriteLine()
            Console.WriteLine("Use /MaxLength to define the maximum allowable length for protein names")
            Console.WriteLine("The default is /MaxLength:" & clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH)
            Console.WriteLine()

            Console.WriteLine("Use /Preview to see the fasta files that would be imported")
            Console.WriteLine("Use /L to log messages to a file; optionally specify the filename using /L:FilePath")
            Console.WriteLine()

            Console.WriteLine("Program written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2014")
            Console.WriteLine("Version: " & GetAppVersion())
            Console.WriteLine()

            Console.WriteLine("E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com")
            Console.WriteLine("Website: http://panomics.pnnl.gov/ or http://omics.pnl.gov")
            Console.WriteLine()

            ' Delay for 750 msec in case the user double clicked this file from within Windows Explorer (or started the program via a shortcut)
            Thread.Sleep(750)

        Catch ex As Exception
            ShowErrorMessage("Error displaying the program syntax: " & ex.Message)
        End Try

    End Sub

    Private Sub BulkImporter_ProgressChanged(taskDescription As String, percentComplete As Single)
        Const PERCENT_REPORT_INTERVAL = 25
        Const PROGRESS_DOT_INTERVAL_MSEC = 250

        If percentComplete >= mLastProgressReportValue Then
            If mLastProgressReportValue > 0 Then
                Console.WriteLine()
            End If
            DisplayProgressPercent(mLastProgressReportValue, False)
            mLastProgressReportValue += PERCENT_REPORT_INTERVAL
            mLastProgressReportTime = DateTime.UtcNow
        Else
            If DateTime.UtcNow.Subtract(mLastProgressReportTime).TotalMilliseconds > PROGRESS_DOT_INTERVAL_MSEC Then
                mLastProgressReportTime = DateTime.UtcNow
                Console.Write(".")
            End If
        End If
    End Sub

    Private Sub BulkImporter_ProgressReset()
        mLastProgressReportTime = DateTime.UtcNow
        mLastProgressReportValue = 0
    End Sub
End Module
