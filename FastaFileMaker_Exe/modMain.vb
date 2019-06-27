Option Strict On

Imports PRISM
Imports Protein_Exporter

Module modMain
    Public Const PROGRAM_DATE As String = "June 26, 2019"

    Const m_DebugLevel As Integer = 4
    Const FASTA_GEN_TIMEOUT_INTERVAL_MINUTES As Integer = 70
    Const DEFAULT_COLLECTION_OPTIONS As String = "seq_direction=forward,filetype=fasta"

    Private WithEvents m_FastaTools As clsGetFASTAFromDMS

    Private m_FastaToolsCnStr As String = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;"
    Private m_message As String
    Private m_FastaFileName As String
    Private WithEvents m_FastaTimer As Timers.Timer
    Private m_FastaGenTimeOut As Boolean
    Private m_GenerationComplete As Boolean = False

    Private m_FastaGenStartTime As DateTime = DateTime.UtcNow

    Private mProteinCollectionList As String
    Private mCreationOpts As String
    Private mLegacyFasta As String
    Private mOutputDirectory As String
    Private mLogProteinFileDetails As Boolean

#Region "Event handlers"

    Private Sub m_FastaTools_DebugEvent(message As String) Handles m_FastaTools.DebugEvent
        ConsoleMsgUtils.ShowDebug(message)
    End Sub

    Private Sub m_FastaTools_ErrorEvent(message As String, ex As Exception) Handles m_FastaTools.ErrorEvent
        ConsoleMsgUtils.ShowError(message, ex)
    End Sub

    Private Sub m_FastaTools_StatusEvent(message As String) Handles m_FastaTools.StatusEvent
        Console.WriteLine(message)
    End Sub

    Private Sub m_FastaTools_WarningEvent(message As String) Handles m_FastaTools.WarningEvent
        ConsoleMsgUtils.ShowWarning(message)
    End Sub

    Private Sub m_FastaTools_FileGenerationStarted(taskMsg As String) Handles m_FastaTools.FileGenerationStarted
    End Sub

    Private Sub m_FastaTools_FileGenerationCompleted(fullOutputPath As String) Handles m_FastaTools.FileGenerationCompleted

        m_FastaFileName = IO.Path.GetFileName(fullOutputPath)  'Get the name of the fasta file that was generated
        m_GenerationComplete = True     'Set the completion flag

    End Sub

    Private Sub m_FastaTools_FileGenerationProgress(statusMsg As String, fractionDone As Double) Handles m_FastaTools.FileGenerationProgress
        Const MINIMUM_LOG_INTERVAL_SEC = 15
        Static dtLastLogTime As DateTime
        Static dblFractionDoneSaved As Double = -1

        If m_DebugLevel >= 3 Then
            ' Limit the logging to once every MINIMUM_LOG_INTERVAL_SEC seconds
            If DateTime.UtcNow.Subtract(dtLastLogTime).TotalSeconds >= MINIMUM_LOG_INTERVAL_SEC OrElse
               fractionDone - dblFractionDoneSaved >= 0.25 Then
                dtLastLogTime = DateTime.UtcNow
                dblFractionDoneSaved = fractionDone
                Console.WriteLine("Generating Fasta file, " & (fractionDone * 100).ToString("0.0") & "% complete, " & statusMsg)
            End If
        End If
    End Sub

    Private Sub m_FastaTimer_Elapsed(sender As Object, e As Timers.ElapsedEventArgs) Handles m_FastaTimer.Elapsed

        If DateTime.UtcNow.Subtract(m_FastaGenStartTime).TotalMinutes >= FASTA_GEN_TIMEOUT_INTERVAL_MINUTES Then
            m_FastaGenTimeOut = True        'Set the timeout flag so an error will be reported
            m_GenerationComplete = True     'Set the completion flag so the fasta generation wait loop will exit
        End If

    End Sub

#End Region

    Public Sub Main()
        Dim commandLineParser As New clsParseCommandLine
        Dim blnProceed As Boolean

        Try
            blnProceed = False

            mProteinCollectionList = String.Empty
            mCreationOpts = String.Empty
            mLegacyFasta = String.Empty
            mOutputDirectory = String.Empty
            mLogProteinFileDetails = False

            If commandLineParser.ParseCommandLine Then
                If SetOptionsUsingCommandLineParameters(commandLineParser) Then blnProceed = True
            End If

            If Not blnProceed OrElse
               commandLineParser.NeedToShowHelp OrElse
               commandLineParser.ParameterCount + commandLineParser.NonSwitchParameterCount = 0 OrElse
               (mProteinCollectionList.Length = 0 AndAlso mLegacyFasta.Length = 0) Then
                ShowProgramHelp()
            Else

                ' To hard-code defaults, enter them here
                'mProteinCollectionList = "Shewanella_2006-07-11"
                'mCreationOpts = "seq_direction=forward,filetype=fasta"
                'mLegacyFasta = "na"
                'mOutputDirectory = "C:\DMS_Temp_Org"
                'mLogProteinFileDetails = True

                If mLegacyFasta.Length = 0 Then
                    mLegacyFasta = "na"
                End If

                If mCreationOpts.Length = 0 Then
                    mCreationOpts = DEFAULT_COLLECTION_OPTIONS
                End If

                If mOutputDirectory.Length = 0 Then
                    mOutputDirectory = IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)
                End If

                If mProteinCollectionList.IndexOf(","c) > 0 AndAlso
                   mProteinCollectionList.IndexOf(".fasta", StringComparison.OrdinalIgnoreCase) > 0 Then
                    ConsoleMsgUtils.ShowError("Protein collection list should not have file extension .fasta; edit the list: " & mProteinCollectionList)
                    Return
                End If

                If mProteinCollectionList.IndexOf(","c) < 0 AndAlso
                   mProteinCollectionList.EndsWith(".fasta", StringComparison.OrdinalIgnoreCase) Then
                    ConsoleMsgUtils.ShowWarning("Auto removing '.fasta' from the protein collection name")
                    mProteinCollectionList = mProteinCollectionList.Substring(0, mProteinCollectionList.Length - ".fasta".Length)
                End If

                ' Data Source=proteinseqs;Initial Catalog=Protein_Sequences
                Dim proteinSeqsConnectionString = My.Settings.ProteinSeqsDBConnectStr

                If Not String.IsNullOrWhiteSpace(proteinSeqsConnectionString) Then
                    m_FastaToolsCnStr = proteinSeqsConnectionString
                End If

                TestExport(mProteinCollectionList, mCreationOpts, mLegacyFasta, mOutputDirectory, mLogProteinFileDetails)

                Console.WriteLine("Destination directory: " & mOutputDirectory)
            End If

        Catch ex As Exception
            ConsoleMsgUtils.ShowError("Error occurred in modMain", ex)
        End Try

    End Sub

    Private Function GetAppVersion(programDate As String) As String
        Return Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() & " (" & programDate & ")"
    End Function

    Private Function GetHumanReadableTimeInterval(dtInterval As TimeSpan) As String

        If dtInterval.TotalDays >= 1 Then
            ' Report Days
            Return dtInterval.TotalDays.ToString("0.00") & " days"
        ElseIf dtInterval.TotalHours >= 1 Then
            ' Report hours
            Return dtInterval.TotalHours.ToString("0.00") & " hours"
        ElseIf dtInterval.TotalMinutes >= 1 Then
            ' Report minutes
            Return dtInterval.TotalMinutes.ToString("0.00") & " minutes"
        Else
            ' Report seconds
            Return dtInterval.TotalSeconds.ToString("0.0") & " seconds"
        End If

    End Function

    Private Sub LogProteinFileDetails(proteinCollectionList As String,
                                      creationOpts As String,
                                      legacyFasta As String,
                                      crc32Hash As String,
                                      destinationDirectoryPath As String,
                                      fastaFileName As String,
                                      logDirectoryPath As String)


        ' Appends a new entry to the log file
        Dim swOutFile As IO.StreamWriter

        Dim strLogFileName As String
        Dim strLogFilePath As String
        Dim blnWriteHeader = False

        Try
            ' Create a new log file each day
            strLogFileName = "FastaFileMakerLog_" & DateTime.Now.ToString("yyyy-MM-dd") & ".txt"

            If Not logDirectoryPath Is Nothing AndAlso logDirectoryPath.Length > 0 Then
                strLogFilePath = IO.Path.Combine(logDirectoryPath, strLogFileName)
            Else
                strLogFilePath = String.Copy(strLogFileName)
            End If

            If Not IO.File.Exists(strLogFilePath) Then
                blnWriteHeader = True
            End If

            swOutFile = New IO.StreamWriter(New IO.FileStream(strLogFilePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.Read))

            If blnWriteHeader Then
                swOutFile.WriteLine("Date" & ControlChars.Tab &
                                    "Time" & ControlChars.Tab &
                                    "Protein_Collection_List" & ControlChars.Tab &
                                    "Creation_Options" & ControlChars.Tab &
                                    "Legacy_Fasta_Name" & ControlChars.Tab &
                                    "Hash_String" & ControlChars.Tab &
                                    "Fasta_File_Name" & ControlChars.Tab &
                                    "Fasta_File_Last_Modified" & ControlChars.Tab &
                                    "Fasta_File_Created" & ControlChars.Tab &
                                    "Fasta_File_Size_Bytes" & ControlChars.Tab &
                                    "Fasta_File_Age_vs_PresentTime")
            End If

            Dim fiFastaFile As IO.FileInfo
            If Not destinationDirectoryPath Is Nothing AndAlso destinationDirectoryPath.Length > 0 Then
                fiFastaFile = New IO.FileInfo(IO.Path.Combine(destinationDirectoryPath, fastaFileName))
            Else
                fiFastaFile = New IO.FileInfo(fastaFileName)
            End If


            swOutFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd") & ControlChars.Tab &
                                DateTime.Now.ToString("hh:mm:ss tt") & ControlChars.Tab &
                                proteinCollectionList & ControlChars.Tab &
                                creationOpts & ControlChars.Tab &
                                legacyFasta & ControlChars.Tab &
                                crc32Hash & ControlChars.Tab &
                                fastaFileName & ControlChars.Tab &
                                fiFastaFile.LastWriteTime.ToString() & ControlChars.Tab &
                                fiFastaFile.CreationTime.ToString() & ControlChars.Tab &
                                fiFastaFile.Length & ControlChars.Tab &
                                GetHumanReadableTimeInterval(DateTime.UtcNow.Subtract(fiFastaFile.LastWriteTimeUtc)))

            If Not swOutFile Is Nothing Then
                swOutFile.Close()
            End If

        Catch
            ' Ignore errors here
        End Try

    End Sub

    Private Function SetOptionsUsingCommandLineParameters(commandLineParser As clsParseCommandLine) As Boolean
        ' Returns True if no problems; otherwise, returns false

        Dim strValue As String = String.Empty
        Dim strValidParameters = New String() {"P", "C", "L", "O", "D"}

        Try
            ' Make sure no invalid parameters are present
            If commandLineParser.InvalidParametersPresent(strValidParameters) Then
                Return False
            Else
                With commandLineParser
                    ' Query commandLineParser to see if various parameters are present
                    If .RetrieveValueForParameter("P", strValue) Then mProteinCollectionList = strValue
                    If .RetrieveValueForParameter("C", strValue) Then mCreationOpts = strValue
                    If .RetrieveValueForParameter("L", strValue) Then mLegacyFasta = strValue
                    If .RetrieveValueForParameter("O", strValue) Then mOutputDirectory = strValue

                    If .RetrieveValueForParameter("D", strValue) Then mLogProteinFileDetails = True

                    If mProteinCollectionList.Length > 0 Then
                        mLegacyFasta = String.Empty
                    ElseIf mLegacyFasta.Length = 0 Then
                        ' Neither /P nor /L were used

                        If .NonSwitchParameterCount > 0 Then
                            ' User specified a non-switch parameter
                            ' Assume it is a protein collection list
                            mProteinCollectionList = .RetrieveNonSwitchParameter(0)
                            If mProteinCollectionList.ToLower.EndsWith(".fasta") OrElse
                               mProteinCollectionList.ToLower.EndsWith(".fasta""") Then
                                ' User specified a .fasta file
                                mLegacyFasta = String.Copy(mProteinCollectionList)
                                mProteinCollectionList = String.Empty
                            End If
                        End If
                    End If

                End With

                Return True
            End If

        Catch ex As Exception
            Console.WriteLine("Error parsing the command line parameters: " & ControlChars.NewLine & ex.Message)
            Return False
        End Try

    End Function

    Private Sub ShowProgramHelp()

        Try
            Dim exeName = IO.Path.GetFileName(Reflection.Assembly.GetExecutingAssembly().Location)

            Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                "This program can export protein collection(s) from the DMS Protein_Sequences database to create a .Fasta file. " &
                "Alternatively, you can specify a legacy .Fasta file name to retrieve"))
            Console.WriteLine()
            Console.WriteLine("Program syntax:")
            Console.WriteLine("  " & exeName & " /P:ProteinCollectionList [/C:ProteinCollectionCreationOptions] [/O:OutputDirectory] [/D]")
            Console.WriteLine("   or   ")
            Console.WriteLine("  " & exeName & " /L:LegacyFastaFileName [/O:OutputDirectory] [/D]")
            Console.WriteLine()
            Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                "To export one or more protein collections, specify the protein collection names as a comma separated list after the /P switch. " &
                "When exporting protein collections, use optional switch /C to change the protein collection export options. " &
                "The default is: " & DEFAULT_COLLECTION_OPTIONS))
            Console.WriteLine()
            Console.WriteLine("To export a legacy fasta file, use /L, for example /L:FileName.fasta")
            Console.WriteLine()
            Console.WriteLine("Optionally use /O to specify the output directory.")
            Console.WriteLine(ConsoleMsgUtils.WrapParagraph(
                "Optionally use /D to log the details of the protein collections, options, and resultant file to a log file."))
            Console.WriteLine()

            Console.WriteLine("Program written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA) in 2009")
            Console.WriteLine("Version: " & GetAppVersion(PROGRAM_DATE))
            Console.WriteLine()

            Console.WriteLine("E-mail: matthew.monroe@pnnl.gov or proteomics@pnnl.gov")
            Console.WriteLine("Website: https://omics.pnl.gov/ or https://panomics.pnnl.gov/")
            Console.WriteLine()

            Console.WriteLine("Licensed under the Apache License, Version 2.0; you may not use this file except in compliance with the License.  " &
                              "You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0")
            Console.WriteLine()

            ' Delay for 750 msec in case the user double clicked this file from within Windows Explorer (or started the program via a shortcut)
            Threading.Thread.Sleep(750)

        Catch ex As Exception
            ConsoleMsgUtils.ShowError("Error displaying the program syntax", ex)
        End Try

    End Sub

    Public Function TestExport(proteinCollectionList As String,
                               creationOpts As String,
                               legacyFasta As String,
                               destinationDirectoryPath As String,
                               blnLogProteinFileDetails As Boolean) As Boolean

        Dim crc32Hash As String

        'Instantiate fasta tool if not already done
        If m_FastaTools Is Nothing Then
            If m_FastaToolsCnStr = "" Then
                m_message = "Protein database connection string not specified"
                Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " & m_message)
                Return False
            End If
            m_FastaTools = New clsGetFASTAFromDMS(m_FastaToolsCnStr)
        End If

        m_FastaTimer = New Timers.Timer
        m_FastaTimer.Interval = 5000
        m_FastaTimer.AutoReset = True

        ' Note that m_FastaTools does not spawn a new thread
        '   Since it does not spawn a new thread, the while loop after this Try block won't actually get reached while m_FastaTools.ExportFASTAFile is running
        '   Furthermore, even if m_FastaTimer_Elapsed sets m_FastaGenTimeOut to True, this won't do any good since m_FastaTools.ExportFASTAFile will still be running
        m_FastaGenTimeOut = False
        m_FastaGenStartTime = DateTime.UtcNow
        Try
            m_FastaTimer.Start()
            crc32Hash = m_FastaTools.ExportFASTAFile(proteinCollectionList, creationOpts, legacyFasta, destinationDirectoryPath)
        Catch Ex As Exception
            Console.WriteLine("clsAnalysisResources.CreateFastaFile(), Exception generating OrgDb file: " & Ex.Message)
            Console.WriteLine(StackTraceFormatter.GetExceptionStackTraceMultiLine(Ex))
            Return False
        End Try

        'Wait for fasta creation to finish
        While Not m_GenerationComplete
            Threading.Thread.Sleep(2000)
            If DateTime.UtcNow.Subtract(m_FastaGenStartTime).TotalMinutes >= FASTA_GEN_TIMEOUT_INTERVAL_MINUTES Then
                m_FastaGenTimeOut = True
                Exit While
            End If
        End While

        m_FastaTimer.Stop()
        If m_FastaGenTimeOut Then
            'Fasta generator hung - report error and exit
            m_message = "Timeout error while generating OrdDb file (" & FASTA_GEN_TIMEOUT_INTERVAL_MINUTES.ToString & " minutes have elapsed)"
            Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " & m_message)

            Return False
        End If

        If blnLogProteinFileDetails Then
            LogProteinFileDetails(proteinCollectionList, creationOpts, legacyFasta, crc32Hash, destinationDirectoryPath, m_FastaFileName, "")
        End If

        Return True

    End Function

End Module
