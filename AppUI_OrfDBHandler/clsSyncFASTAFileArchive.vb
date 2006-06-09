Public Class clsSyncFASTAFileArchive

    Private m_PSConnectionString As String
    Private m_FileArchiver As Protein_Exporter.IArchiveOutputFiles
    Private m_TableGetter As TableManipulationBase.IGetSQLData
    Private m_Importer As Protein_Importer.IAddUpdateEntries
    Private WithEvents m_Exporter As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS

    Event SyncStart(ByVal StatusMsg As String)
    Event SyncProgress(ByVal StatusMsg As String, ByVal fractionDone As Double)
    Event SyncComplete()
    Event FileSyncProgress(ByVal CurrentProteinCount As Integer)

    Private m_CurrentStatusMsg As String
    Private m_CurrentProteinCount As Integer
    Private m_TotalProteinsCount As Integer

    Sub New(ByVal PSConnectionString As String)

        Me.m_PSConnectionString = PSConnectionString
        Me.m_FileArchiver = New Protein_Exporter.clsArchiveToFile(PSConnectionString)
        Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString, True)
        Me.m_Importer = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)


    End Sub

    Function SyncCollectionsAndArchiveTables(ByVal OutputPath As String) As Integer
        Dim SQL As String
        SQL = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, DateModified, Collection_Type_ID, NumProteins " & _
            "FROM V_Missing_Archive_Entries"


        Dim dt As DataTable
        dt = Me.m_TableGetter.GetTable(SQL)
        Dim dr As DataRow
        Dim sourceFilePath As String
        Dim proteinCollectionID As Integer
        Dim ArchiveEntryID As Integer
        Dim SHA1 As String
        Dim CreationOptionsString As String = "seq_direction=forward,filetype=fasta"
        Dim totalProteinsCount As Integer
        Dim currentCollectionProteinCount As Integer = 0
        For Each dr In dt.Rows
            Me.m_TotalProteinsCount += CInt(dr.Item("NumProteins"))
        Next


        Dim outputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes
        outputSequenceType = Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward
        Dim databaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes
        databaseFormatType = Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta




        Me.OnSyncStart("Synchronizing Archive Table with Collections Table")

        For Each dr In dt.Rows
            Me.OnSyncProgressUpdate("Processing - '" & dr.Item("FileName").ToString & "'", CDbl(currentCollectionProteinCount / totalProteinsCount))
            currentCollectionProteinCount = CInt(dr.Item("NumProteins"))
            proteinCollectionID = CInt(dr.Item("Protein_Collection_ID"))
            sourceFilePath = System.IO.Path.Combine(OutputPath, dr.Item("FileName").ToString & ".fasta")
            SHA1 = dr.Item("Authentication_Hash").ToString


            ArchiveEntryID = Me.m_FileArchiver.ArchiveCollection( _
                proteinCollectionID, _
                Protein_Exporter.IArchiveOutputFiles.CollectionTypes.static, _
                outputSequenceType, databaseFormatType, sourceFilePath, CreationOptionsString, SHA1)

        Next

        Me.OnSyncCompletion()

        Return 0

    End Function

    Sub UpdateSHA1Hashes() 'Implements IArchiveOutputFiles.UpdateSHA1Hashes
        If Me.m_FileArchiver Is Nothing Then
            Me.m_FileArchiver = New Protein_Exporter.clsArchiveToFile(Me.m_PSConnectionString)
        End If

        Dim sql As String

        Me.m_Importer = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)

        sql = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, NumProteins " & _
        "FROM V_Missing_Archive_Entries"

        Dim dt As DataTable

        dt = Me.m_TableGetter.GetTable(sql)

        Dim dr As DataRow
        Dim tmpID As Integer
        Dim tmpStoredSHA As String
        Dim tmpGenSHA As String
        Dim tmpFilename As String
        Dim fi As System.IO.FileInfo
        Dim tmpFullPath As String
        Dim tmpNameList As ArrayList
        Dim totalProteinCount As Integer
        Dim currentProteinCount As Integer = 0

        For Each dr In dt.Rows
            Me.m_TotalProteinsCount += CInt(dr.Item("Numproteins"))
        Next

        Dim starttime As DateTime
        Dim elapsedTime As TimeSpan
        Dim elapsedTimeSB As New System.Text.StringBuilder

        Dim fileCounter As Integer
        Dim totalFileCount As Integer = dt.Rows.Count

        Dim tmpPath As String = System.IO.Path.GetTempPath

        Me.m_Exporter = New Protein_Exporter.clsGetFASTAFromDMS( _
            Me.m_PSConnectionString, Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, _
            Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)

        Dim creationOptionsString As String
        creationOptionsString = "seq_direction=forward,filetype=fasta"
        Me.OnSyncStart("Updating Collections and Archive Entries")
        starttime = System.DateTime.Now

        For Each dr In dt.Rows
            tmpID = CInt(dr.Item("Protein_Collection_ID"))
            tmpStoredSHA = dr.Item("Authentication_Hash").ToString
            tmpFilename = dr.Item("FileName").ToString
            elapsedTimeSB.Remove(0, elapsedTimeSB.Length)
            'elapsedTime = Format(System.DateTime.Now.op_Subtraction(System.DateTime.Now, starttime), "H:mm:ss")
            elapsedTime = System.DateTime.Now.Subtract(starttime)
            If elapsedTime.Minutes < 1 And elapsedTime.Hours = 0 Then
                elapsedTimeSB.Append("less than ")
            Else
                elapsedTimeSB.Append("about ")
            End If
            If elapsedTime.Hours > 0 Then
                elapsedTimeSB.Append(elapsedTime.Hours.ToString + " hours, ")
            End If
            If elapsedTime.Minutes <= 1 Then
                elapsedTimeSB.Append("1 minute")
            Else
                elapsedTimeSB.Append(elapsedTime.Minutes.ToString + " minutes")
            End If


            'Me.OnSyncProgressUpdate( _
            '    "Collection " _
            '    & Format(tmpID, "0000") _
            '    & " [Elapsed Time: " _
            '    & elapsedTimeSB.ToString & "]", _
            '    CDbl(currentProteinCount / totalProteinCount))
            Me.m_CurrentStatusMsg = "Collection " _
                & Format(tmpID, "0000") _
                & " [Elapsed Time: " _
                & elapsedTimeSB.ToString & "]"

            Me.OnSyncProgressUpdate( _
                Me.m_CurrentStatusMsg, _
                CDbl(Me.m_CurrentProteinCount / Me.m_TotalProteinsCount))

            fileCounter += 1

            tmpFullPath = System.IO.Path.Combine(tmpPath, tmpFilename & ".fasta")
            'Debug.WriteLine("Start: " & tmpFilename & ": " & starttime.ToLongTimeString)

            tmpGenSHA = Me.m_Exporter.ExportFASTAFile(tmpID, tmpPath, _
                Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, _
                Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)

            If Not tmpStoredSHA.Equals(tmpGenSHA) Then
                Me.m_Importer.AddAuthenticationHash(tmpID, tmpGenSHA)
            End If

            'Debug.WriteLine("End: " & tmpFilename & ": " & DateTime.Now.ToLongTimeString)
            'Debug.Flush()

            Me.m_FileArchiver.ArchiveCollection( _
                tmpID, _
                Protein_Exporter.IArchiveOutputFiles.CollectionTypes.static, _
                Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward, _
                Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, _
                tmpFullPath, creationOptionsString, tmpGenSHA)
            'Me.ArchiveCollection( _
            '    tmpID, _
            '    IArchiveOutputFiles.CollectionTypes.static, _
            '    tmpFullPath, tmpGenSHA)

            'tmpNameList.Clear()
            fi = New System.IO.FileInfo(tmpFullPath)
            fi.Delete()
            Me.m_CurrentProteinCount += CInt(dr.Item("NumProteins"))

        Next

        Me.OnSyncCompletion()

        'me.OnSyncStart("Updating ProteinNames

        'Update T_Protein_Names

        '    Dim tmpRefID As Integer
        '    Dim tmpName As String
        '    Dim tmpFingerprint As String
        '    Dim tmpProtID As Integer
        '    Dim errorCode As Integer
        '    Dim counter As Integer = 0

        '    sql = "SELECT Reference_ID, Name, Reference_Fingerprint, Protein_ID " & _
        '            "FROM T_Protein_Names"

        '    dt = Me.m_TableGetter.GetTable(sql)

        '    For Each dr In dt.Rows
        '        counter += 1
        '        tmpRefID = DirectCast(dr.Item("Reference_ID"), Int32)
        '        tmpName = dr.Item("Name").ToString
        '        tmpFingerprint = dr.Item("Reference_Fingerprint").ToString
        '        tmpProtID = DirectCast(dr.Item("Protein_ID"), Int32)

        '        'tmpGenSHA = Me.m_Importer.GenerateArbitraryHash(tmpName + tmpProtID.ToString)
        '        errorCode = Me.m_Importer.UpdateProteinNameHash(tmpRefID, tmpName, tmpProtID)
        '        If counter Mod 2000 = 0 Then
        '            Debug.WriteLine(counter.ToString)
        '        End If
        '    Next

        '    'Update T_Proteins

        '    Dim tmpSeq As String
        '    counter = 0

        '    sql = "SELECT Protein_ID, Sequence " & _
        '            "FROM T_Proteins"

        '    dt = Me.m_TableGetter.GetTable(sql)

        '    For Each dr In dt.Rows
        '        counter += 1
        '        tmpProtID = DirectCast(dr.Item("Protein_ID"), Int32)
        '        tmpSeq = dr.Item("Sequence").ToString

        '        errorCode = Me.m_Importer.UpdateProteinSequenceHash(tmpProtID, tmpSeq)

        '        If counter Mod 2000 = 0 Then
        '            Debug.WriteLine(counter.ToString)
        '        End If
        '    Next

    End Sub

    Sub FixArchivedFilePaths()
        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If

        Dim SelectSQL As String
        SelectSQL = "SELECT * FROM T_Temp_Archive_Path_Fix"

        Dim tmpTable As DataTable = Me.m_TableGetter.GetTable(SelectSQL)

        Dim dr As DataRow
        Dim tmpOldPath As String
        Dim tmpNewPath As String

        For Each dr In tmpTable.Rows
            tmpOldPath = dr.Item("Archived_File_Path").ToString
            tmpNewPath = dr.Item("Newpath").ToString

            Rename(tmpOldPath, tmpNewPath)
        Next

    End Sub

    Sub CorrectMasses()

        Dim proteinList As New Hashtable
        Dim proteinTable As DataTable
        Dim dr As DataRow

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If

        Dim proteinSelectSQL As String = "SELECT Protein_ID, Sequence FROM T_Proteins " & _
            "WHERE Monoisotopic_Mass = 0 or Average_Mass = 0"

        proteinTable = Me.m_TableGetter.GetTable(proteinSelectSQL)

        For Each dr In proteinTable.Rows
            proteinList.Add(CInt(dr.Item("Protein_ID")), dr.Item("Sequence").ToString)
        Next

        Me.UpdateProteinSequenceInfo(proteinList)


    End Sub

    Sub RefreshNameHashes()
        Dim TotalNameCount As Integer
        Dim NameCountSQL As String = "SELECT TOP 1 Reference_ID FROM T_Protein_Names ORDER BY Reference_ID DESC"
        Dim tmptable As DataTable
        Dim dr As DataRow
        Dim foundRows() As DataRow
        tmptable = Me.m_TableGetter.GetTable(NameCountSQL)
        TotalNameCount = CInt(tmptable.Rows(0).Item("Reference_ID"))

        Dim tmpRefID As Integer
        Dim tmpProteinName As String
        Dim tmpDescription As String
        Dim tmpProteinID As Integer

        Dim rowRetrievalSQL As String

        Dim startIndex As Integer = 0
        Dim counter As Integer
        Dim stepvalue As Integer = 10000

        If TotalNameCount <= stepvalue Then
            stepvalue = TotalNameCount
        End If
        Me.OnSyncStart("Updating Name Hashes")

        For counter = stepvalue To TotalNameCount + stepvalue Step stepvalue
            If counter >= TotalNameCount - stepvalue Then
                Debug.WriteLine("")
            End If
            rowRetrievalSQL = "SELECT Reference_ID, Name, Description, Protein_ID " & _
                              "FROM T_Protein_Names " & _
                              "WHERE Reference_ID > " & startIndex & " and Reference_ID <= " & counter & _
                              "ORDER BY Reference_ID"

            'Protein_Name(+"_" + Description + "_" + ProteinID.ToString)
            tmptable = Me.m_TableGetter.GetTable(rowRetrievalSQL)
            If tmptable.Rows.Count > 0 Then
                For Each dr In tmptable.Rows
                    tmpRefID = DirectCast(dr.Item("Reference_ID"), Int32)
                    tmpProteinName = dr.Item("Name").ToString
                    tmpDescription = dr.Item("Description").ToString
                    tmpProteinID = DirectCast(dr.Item("Protein_ID"), Int32)

                    Me.m_Importer.UpdateProteinNameHash( _
                        tmpRefID, _
                        tmpProteinName, _
                        tmpDescription, _
                        tmpProteinID)
                Next
                tmptable.Clear()
                Me.OnSyncProgressUpdate("Processing " & startIndex & " to " & counter, CDbl(counter / TotalNameCount))
                startIndex = counter + 1
            End If

        Next
        Me.OnSyncCompletion()



    End Sub

    Protected Sub UpdateProteinSequenceInfo(ByVal Proteins As Hashtable)

        If Me.m_Importer Is Nothing Then
            Me.m_Importer = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)
        End If

        Dim proteinID As Integer
        Dim sequence As String
        Dim si As SequenceInfoCalculator.ICalculateSeqInfo

        For Each proteinID In Proteins.Keys
            sequence = Proteins.Item(proteinID).ToString
            si = New SequenceInfoCalculator.SequenceInfoCalculator
            si.CalculateSequenceInfo(sequence)
            Me.m_Importer.UpdateProteinSequenceInfo( _
                proteinID, sequence, sequence.Length, _
                si.MolecularFormula, si.MonoIsotopicMass, _
                si.AverageMass, si.SHA1Hash)
        Next

    End Sub

    Private Sub OnSyncStart(ByVal StatusMsg As String)
        RaiseEvent SyncStart(StatusMsg)
    End Sub

    Private Sub OnSyncProgressUpdate(ByVal StatusMsg As String, ByVal fractionDone As Double)
        RaiseEvent SyncProgress(StatusMsg, fractionDone)
    End Sub

    Private Sub OnSyncCompletion()
        RaiseEvent SyncComplete()
    End Sub

    'Private Sub OnExportProgressUpdate(ByVal currentProteinCount As Integer) Handles m_Exporter.FileGenerationProgress
    '    RaiseEvent SyncProgress(Me.m_CurrentStatusMsg, CDbl((Me.m_CurrentProteinCount + currentProteinCount) / Me.m_TotalProteinsCount))
    'End Sub



End Class
