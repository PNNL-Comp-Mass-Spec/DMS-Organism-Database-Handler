Option Strict On
Imports Protein_Exporter

Module modMain

    Const m_DebugLevel As Integer = 4
    Const FASTA_GEN_TIMEOUT_INTERVAL_MINUTES As Integer = 70

    Private WithEvents m_FastaTools As ExportProteinCollectionsIFC.IGetFASTAFromDMS

    Private m_FastaToolsCnStr As String = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;"
    Private m_message As String
    Private m_FastaFileName As String
    Private WithEvents m_FastaTimer As System.Timers.Timer
    Private m_FastaGenTimeOut As Boolean
    Private m_GenerationComplete As Boolean = False
    Private m_GenerationStarted As Boolean = False

    Private m_FastaGenStartTime As DateTime = System.DateTime.Now

#Region "Event handlers"
    Private Sub m_FastaTools_FileGenerationStarted(ByVal taskMsg As String) Handles m_FastaTools.FileGenerationStarted

        m_GenerationStarted = True

    End Sub

    Private Sub m_FastaTools_FileGenerationCompleted(ByVal FullOutputPath As String) Handles m_FastaTools.FileGenerationCompleted

        m_FastaFileName = System.IO.Path.GetFileName(FullOutputPath)  'Get the name of the fasta file that was generated
        m_GenerationComplete = True     'Set the completion flag

    End Sub

    Private Sub m_FastaTools_FileGenerationProgress(ByVal statusMsg As String, ByVal fractionDone As Double) Handles m_FastaTools.FileGenerationProgress
        Const MINIMUM_LOG_INTERVAL_SEC As Integer = 15
        Static dtLastLogTime As DateTime
        Static dblFractionDoneSaved As Double = -1

        If m_DebugLevel >= 3 Then
            ' Limit the logging to once every MINIMUM_LOG_INTERVAL_SEC seconds
            If System.DateTime.Now.Subtract(dtLastLogTime).TotalSeconds >= MINIMUM_LOG_INTERVAL_SEC OrElse _
               fractionDone - dblFractionDoneSaved >= 0.25 Then
                dtLastLogTime = System.DateTime.Now
                dblFractionDoneSaved = fractionDone
                Console.WriteLine("Generating Fasta file, " & (fractionDone * 100).ToString("0.0") & "% complete, " & statusMsg)
            End If
        End If
    End Sub

    Private Sub m_FastaTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles m_FastaTimer.Elapsed

        If System.DateTime.Now.Subtract(m_FastaGenStartTime).TotalMinutes >= FASTA_GEN_TIMEOUT_INTERVAL_MINUTES Then
            m_FastaGenTimeOut = True      'Set the timeout flag so an error will be reported
            m_GenerationComplete = True     'Set the completion flag so the fasta generation wait loop will exit
        End If

    End Sub

#End Region


    Public Sub Main()
        Dim CollectionList As String
        Dim CreationOpts As String
        Dim LegacyFasta As String
        Dim DestFolder As String

        CollectionList = "Shewanella_2006-07-11"
        CollectionList = "Aspergillus_fumigatus_2008-03-07"
        CreationOpts = "seq_direction=forward,filetype=fasta"
        LegacyFasta = "na"

        'CollectionList = "na"
        'CreationOpts = "na"
        'LegacyFasta = "A_Thaliana_AGI_TIGR_2002-01-09.fasta"

        DestFolder = "C:\DMS_Temp_Org"

        TestExport(CollectionList, CreationOpts, LegacyFasta, DestFolder)

    End Sub

    Public Function TestExport(ByVal CollectionList As String, _
                               ByVal CreationOpts As String, _
                               ByVal LegacyFasta As String, _
                               ByVal DestFolder As String) As Boolean

        Dim HashString As String

        'Instantiate fasta tool if not already done
        If m_FastaTools Is Nothing Then
            If m_FastaToolsCnStr = "" Then
                m_message = "Protein database connection string not specified"
                Console.WriteLine("clsAnalysisResources.CreateFastaFile(), " & m_message)
                Return False
            End If
            m_FastaTools = New clsGetFASTAFromDMS(m_FastaToolsCnStr)

        End If

        m_FastaTimer = New System.Timers.Timer
        m_FastaTimer.Interval = 5000
        m_FastaTimer.AutoReset = True

        ' Note that m_FastaTools does not spawn a new thread
        '   Since it does not spawn a new thread, the while loop after this Try block won't actually get reached while m_FastaTools.ExportFASTAFile is running
        '   Furthermore, even if m_FastaTimer_Elapsed sets m_FastaGenTimeOut to True, this won't do any good since m_FastaTools.ExportFASTAFile will still be running
        m_FastaGenTimeOut = False
        m_FastaGenStartTime = System.DateTime.Now
        Try
            m_FastaTimer.Start()
            HashString = m_FastaTools.ExportFASTAFile(CollectionList, CreationOpts, LegacyFasta, DestFolder)
        Catch Ex As Exception
            Console.WriteLine("clsAnalysisResources.CreateFastaFile(), Exception generating OrgDb file: " & Ex.Message & _
            "; " & GetExceptionStackTrace(Ex))
            Return False
        End Try

        'Wait for fasta creation to finish
        While Not m_GenerationComplete
            System.Threading.Thread.Sleep(2000)
            If System.DateTime.Now.Subtract(m_FastaGenStartTime).TotalMinutes >= FASTA_GEN_TIMEOUT_INTERVAL_MINUTES Then
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


    End Function

    ''' <summary>
    ''' Parses the .StackTrace text of the given expression to return a compact description of the current stack
    ''' </summary>
    ''' <param name="objException"></param>
    ''' <returns>String similar to "Stack trace: clsCodeTest.Test->clsCodeTest.TestException->clsCodeTest.InnerTestException in clsCodeTest.vb:line 86"</returns>
    ''' <remarks></remarks>
    Public Function GetExceptionStackTrace(ByVal objException As System.Exception) As String
        Const REGEX_FUNCTION_NAME As String = "at ([^(]+)\("
        Const REGEX_FILE_NAME As String = "in .+\\(.+)"

        Dim trTextReader As System.IO.StringReader
        Dim intIndex As Integer

        Dim intFunctionCount As Integer = 0
        Dim strFunctions() As String

        Dim strCurrentFunction As String
        Dim strFinalFile As String = String.Empty

        Dim strLine As String = String.Empty
        Dim strStackTrace As String = String.Empty

        Dim reFunctionName As New System.Text.RegularExpressions.Regex(REGEX_FUNCTION_NAME, System.Text.RegularExpressions.RegexOptions.Compiled Or System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim reFileName As New System.Text.RegularExpressions.Regex(REGEX_FILE_NAME, System.Text.RegularExpressions.RegexOptions.Compiled Or System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim objMatch As System.Text.RegularExpressions.Match

        ' Process each line in objException.StackTrace
        ' Populate strFunctions() with the function name of each line
        trTextReader = New System.IO.StringReader(objException.StackTrace)

        intFunctionCount = 0
        ReDim strFunctions(9)

        Do While trTextReader.Peek >= 0
            strLine = trTextReader.ReadLine

            If Not strLine Is Nothing AndAlso strLine.Length > 0 Then
                strCurrentFunction = String.Empty

                objMatch = reFunctionName.Match(strLine)
                If objMatch.Success AndAlso objMatch.Groups.Count > 1 Then
                    strCurrentFunction = objMatch.Groups(1).Value
                Else
                    ' Look for the word " in "
                    intIndex = strLine.ToLower.IndexOf(" in ")
                    If intIndex = 0 Then
                        ' " in" not found; look for the first space after startIndex 4
                        intIndex = strLine.IndexOf(" ", 4)
                    End If
                    If intIndex = 0 Then
                        ' Space not found; use the entire string
                        intIndex = strLine.Length - 1
                    End If

                    If intIndex > 0 Then
                        strCurrentFunction = strLine.Substring(0, intIndex)
                    End If

                End If

                If Not strCurrentFunction Is Nothing AndAlso strCurrentFunction.Length > 0 Then
                    If intFunctionCount >= strFunctions.Length Then
                        ' Reserve more space in strFunctions()
                        ReDim Preserve strFunctions(strFunctions.Length * 2 - 1)
                    End If

                    strFunctions(intFunctionCount) = strCurrentFunction
                    intFunctionCount += 1
                End If

                If strFinalFile.Length = 0 Then
                    ' Also extract the file name where the Exception occurred
                    objMatch = reFileName.Match(strLine)
                    If objMatch.Success AndAlso objMatch.Groups.Count > 1 Then
                        strFinalFile = objMatch.Groups(1).Value
                    End If
                End If

            End If
        Loop

        strStackTrace = String.Empty
        For intIndex = intFunctionCount - 1 To 0 Step -1
            If Not strFunctions(intIndex) Is Nothing Then
                If strStackTrace.Length = 0 Then
                    strStackTrace = "Stack trace: " & strFunctions(intIndex)
                Else
                    strStackTrace &= "->" & strFunctions(intIndex)
                End If
            End If
        Next intIndex

        If Not strStackTrace Is Nothing AndAlso strFinalFile.Length > 0 Then
            strStackTrace &= " in " & strFinalFile
        End If

        Return strStackTrace

    End Function
End Module
