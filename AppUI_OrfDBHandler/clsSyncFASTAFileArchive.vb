Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports ProteinFileReader
Imports Protein_Exporter
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports Protein_Importer
Imports TableManipulationBase

Public Class clsSyncFASTAFileArchive

    Private ReadOnly m_PSConnectionString As String
    Private m_FileArchiver As IArchiveOutputFiles
    Private m_TableGetter As IGetSQLData
    Private m_Importer As IAddUpdateEntries

    Private WithEvents m_Exporter As clsGetFASTAFromDMS

    Event SyncStart(StatusMsg As String)
    Event SyncProgress(StatusMsg As String, fractionDone As Double)
    Event SyncComplete()
    Event FileSyncProgress(CurrentProteinCount As Integer)

    Private m_CurrentStatusMsg As String
    Private m_CurrentProteinCount As Integer
    Private m_TotalProteinsCount As Integer

    Private m_GeneratedFastaFilePath As String

    Sub New(PSConnectionString As String)

        Me.m_PSConnectionString = PSConnectionString
        'Me.m_FileArchiver = New Protein_Exporter.clsArchiveToFile(PSConnectionString, Me.m_Exporter)
        Me.m_TableGetter = New clsDBTask(Me.m_PSConnectionString, True)
        Me.m_Importer = New clsAddUpdateEntries(Me.m_PSConnectionString)


    End Sub

    Function SyncCollectionsAndArchiveTables(OutputPath As String) As Integer
        Dim SQL As String
        SQL = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, DateModified, Collection_Type_ID, NumProteins " &
            "FROM V_Missing_Archive_Entries"

        'TODO add collection list string
        Dim proteinCollectionList = ""


        Dim dt As DataTable
        dt = Me.m_TableGetter.GetTable(SQL)
        Dim dr As DataRow
        Dim sourceFilePath As String
        Dim proteinCollectionID As Integer
        Dim ArchiveEntryID As Integer
        Dim SHA1 As String
        Dim CreationOptionsString = "seq_direction=forward,filetype=fasta"
        Dim totalProteinsCount As Integer
        Dim currentCollectionProteinCount = 0
        For Each dr In dt.Rows
            Me.m_TotalProteinsCount += CInt(dr.Item("NumProteins"))
        Next


        Dim outputSequenceType As IGetFASTAFromDMS.SequenceTypes
        outputSequenceType = IGetFASTAFromDMS.SequenceTypes.forward
        Dim databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes
        databaseFormatType = IGetFASTAFromDMS.DatabaseFormatTypes.fasta




        Me.OnSyncStart("Synchronizing Archive Table with Collections Table")

        For Each dr In dt.Rows
            Me.OnSyncProgressUpdate("Processing - '" & dr.Item("FileName").ToString & "'", CDbl(currentCollectionProteinCount / totalProteinsCount))
            currentCollectionProteinCount = CInt(dr.Item("NumProteins"))
            proteinCollectionID = CInt(dr.Item("Protein_Collection_ID"))
            sourceFilePath = Path.Combine(OutputPath, dr.Item("FileName").ToString & ".fasta")
            SHA1 = dr.Item("Authentication_Hash").ToString


            Me.m_FileArchiver.ArchiveCollection(
                proteinCollectionID,
                IArchiveOutputFiles.CollectionTypes.static,
                outputSequenceType, databaseFormatType, sourceFilePath, CreationOptionsString, SHA1, proteinCollectionList)

        Next

        Me.OnSyncCompletion()

        Return 0

    End Function

    Public Sub UpdateSHA1Hashes() 'Implements IArchiveOutputFiles.UpdateSHA1Hashes
        If Me.m_FileArchiver Is Nothing Then
            Me.m_FileArchiver = New clsArchiveToFile(Me.m_PSConnectionString, Me.m_Exporter)
        End If

        Dim sql As String

        Me.m_Importer = New clsAddUpdateEntries(Me.m_PSConnectionString)

        sql = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, NumProteins " &
        "FROM V_Missing_Archive_Entries"

        Dim dt As DataTable

        dt = Me.m_TableGetter.GetTable(sql)

        Dim dr As DataRow
        Dim tmpID As Integer
        Dim tmpStoredSHA As String
        Dim tmpGenSHA As String
        Dim tmpFilename As String
        Dim fi As FileInfo
        Dim tmpFullPath As String

        For Each dr In dt.Rows
            Me.m_TotalProteinsCount += CInt(dr.Item("Numproteins"))
        Next

        Dim starttime As DateTime
        Dim elapsedTime As TimeSpan
        Dim elapsedTimeSB As New StringBuilder

        Dim tmpPath As String = Path.GetTempPath

        Me.m_Exporter = New clsGetFASTAFromDMS(
            Me.m_PSConnectionString, IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
            IGetFASTAFromDMS.SequenceTypes.forward)

        Dim creationOptionsString As String
        creationOptionsString = "seq_direction=forward,filetype=fasta"
        Me.OnSyncStart("Updating Collections and Archive Entries")
        starttime = DateTime.UtcNow

        For Each dr In dt.Rows
            tmpID = CInt(dr.Item("Protein_Collection_ID"))
            tmpStoredSHA = dr.Item("Authentication_Hash").ToString
            tmpFilename = dr.Item("FileName").ToString
            m_GeneratedFastaFilePath = String.Empty

            elapsedTimeSB.Remove(0, elapsedTimeSB.Length)
            elapsedTime = DateTime.UtcNow.Subtract(starttime)
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


            'Me.OnSyncProgressUpdate(
            '    "Collection "
            '    & Format(tmpID, "0000")
            '    & " [Elapsed Time: "
            '    & elapsedTimeSB.ToString & "]",
            '    CDbl(currentProteinCount / totalProteinCount))
            Me.m_CurrentStatusMsg = "Collection " & Format(tmpID, "0000") & " [Elapsed Time: " & elapsedTimeSB.ToString() & "]"

            Me.OnSyncProgressUpdate(
                Me.m_CurrentStatusMsg,
                CDbl(Me.m_CurrentProteinCount / Me.m_TotalProteinsCount))

            tmpFullPath = Path.Combine(tmpPath, tmpFilename & ".fasta")
            'Debug.WriteLine("Start: " & tmpFilename & ": " & starttime.ToLongTimeString)

            tmpGenSHA = Me.m_Exporter.ExportFASTAFile(tmpID, tmpPath,
                IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                IGetFASTAFromDMS.SequenceTypes.forward)

            If Not tmpStoredSHA.Equals(tmpGenSHA) Then
                Dim currentFastaProteinCount = 0
                Dim currentFastaResidueCount = 0

                If Not String.IsNullOrEmpty(m_GeneratedFastaFilePath) Then
                    CountProteinsAndResidues(m_GeneratedFastaFilePath, currentFastaProteinCount, currentFastaResidueCount)
                End If

                Me.m_Importer.AddAuthenticationHash(tmpID, tmpGenSHA, currentFastaProteinCount, currentFastaResidueCount)
            End If

            'Debug.WriteLine("End: " & tmpFilename & ": " & DateTime.Now.ToLongTimeString)
            'Debug.Flush()

            Me.m_FileArchiver.ArchiveCollection(
                tmpID,
                IArchiveOutputFiles.CollectionTypes.static,
                IGetFASTAFromDMS.SequenceTypes.forward,
                IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                tmpFullPath, creationOptionsString, tmpGenSHA, "")
            'Me.ArchiveCollection(
            '    tmpID,
            '    IArchiveOutputFiles.CollectionTypes.static,
            '    tmpFullPath, tmpGenSHA)

            'tmpNameList.Clear()
            fi = New FileInfo(tmpFullPath)
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

        '    sql = "SELECT Reference_ID, Name, Reference_Fingerprint, Protein_ID " &
        '            "FROM T_Protein_Names"

        '    dt = Me.m_TableGetter.GetTable(sql)

        '    For Each dr In dt.Rows
        '        counter += 1
        '        tmpRefID = DirectCast(dr.Item("Reference_ID"), Integer)
        '        tmpName = dr.Item("Name").ToString
        '        tmpFingerprint = dr.Item("Reference_Fingerprint").ToString
        '        tmpProtID = DirectCast(dr.Item("Protein_ID"), Integer)

        '        'tmpGenSHA = Me.m_Importer.GenerateArbitraryHash(tmpName + tmpProtID.ToString)
        '        errorCode = Me.m_Importer.UpdateProteinNameHash(tmpRefID, tmpName, tmpProtID)
        '        If counter Mod 2000 = 0 Then
        '            Debug.WriteLine(counter.ToString)
        '        End If
        '    Next

        '    'Update T_Proteins

        '    Dim tmpSeq As String
        '    counter = 0

        '    sql = "SELECT Protein_ID, Sequence " &
        '            "FROM T_Proteins"

        '    dt = Me.m_TableGetter.GetTable(sql)

        '    For Each dr In dt.Rows
        '        counter += 1
        '        tmpProtID = DirectCast(dr.Item("Protein_ID"), Integer)
        '        tmpSeq = dr.Item("Sequence").ToString

        '        errorCode = Me.m_Importer.UpdateProteinSequenceHash(tmpProtID, tmpSeq)

        '        If counter Mod 2000 = 0 Then
        '            Debug.WriteLine(counter.ToString)
        '        End If
        '    Next

    End Sub

    Private Sub CountProteinsAndResidues(fastaFilePath As String, <Out> ByRef proteinCount As Integer, <Out> ByRef residueCount As Integer)

        proteinCount = 0
        residueCount = 0

        Dim oReader = New FastaFileReader()
        If oReader.OpenFile(fastaFilePath) Then
            While oReader.ReadNextProteinEntry()
                proteinCount += 1
                residueCount += oReader.ProteinSequence.Length
            End While
        End If

        oReader.CloseFile()

    End Sub

    Public Sub FixArchivedFilePaths()
        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New clsDBTask(Me.m_PSConnectionString)
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

    Public Sub AddSortingIndices()

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New clsDBTask(Me.m_PSConnectionString)
        End If

        Dim getCollectionsSQL = "SELECT Protein_Collection_ID, FileName, Organism_ID FROM V_Protein_Collections_By_Organism WHERE Collection_Type_ID = 1 or Collection_Type_ID = 5"

        Dim collectionTable As DataTable = Me.m_TableGetter.GetTable(getCollectionsSQL)
        Dim collectionEntry As DataRow
        Dim tmpCollectionName As String
        Dim tmpCollectionID As Integer

        Dim getLegacyFilesSQL = "SELECT DISTINCT FileName, Full_Path, Organism_ID FROM V_Legacy_Static_File_Locations"
        Dim legacyTable As DataTable = Me.m_TableGetter.GetTable(getLegacyFilesSQL)
        Dim legacyfoundrows() As DataRow
        Dim legacyFileEntry As DataRow
        Dim legacyFullPath As String

        Dim nameIndexHash As Hashtable

        Dim getReferencesSQL As String
        Dim referencesTable As DataTable
        Dim referenceEntry As DataRow

        Dim tmpRefName As String
        Dim tmpRefID As Integer
        Dim tmpProteinID As Integer
        Dim tmpSortingIndex As Integer
        Dim tmpOrgID As Integer

        For Each collectionEntry In collectionTable.Rows
            tmpCollectionName = collectionEntry.Item("FileName").ToString
            tmpCollectionID = CInt(collectionEntry.Item("Protein_Collection_ID"))
            If tmpCollectionID = 1026 Then
                Debug.WriteLine("")
            End If
            tmpOrgID = CInt(collectionEntry.Item("Organism_ID"))
            legacyfoundrows = legacyTable.Select("FileName = '" & tmpCollectionName & ".fasta' AND Organism_ID = " & tmpOrgID)
            If legacyfoundrows.Length > 0 Then
                getReferencesSQL = "SELECT * FROM V_Tmp_Member_Name_Lookup WHERE Protein_Collection_ID = " & tmpCollectionID.ToString &
                                    " AND Sorting_Index is NULL"
                referencesTable = Me.m_TableGetter.GetTable(getReferencesSQL)
                If referencesTable.Rows.Count > 0 Then
                    legacyFileEntry = legacyfoundrows(0)
                    legacyFullPath = legacyFileEntry.Item("Full_Path").ToString
                    nameIndexHash = Me.GetProteinSortingIndices(legacyFullPath)
                    For Each referenceEntry In referencesTable.Rows
                        tmpRefID = DirectCast(referenceEntry.Item("Reference_ID"), Integer)
                        tmpProteinID = DirectCast(referenceEntry.Item("Protein_ID"), Integer)
                        tmpRefName = referenceEntry.Item("Name").ToString
                        'Try
                        tmpSortingIndex = DirectCast(nameIndexHash.Item(tmpRefName.ToLower), Integer)

                        If tmpSortingIndex > 0 Then
                            Me.m_Importer.UpdateProteinCollectionMember(
                                tmpRefID, tmpProteinID,
                                tmpSortingIndex, tmpCollectionID)
                        End If

                        'Catch ex As Exception

                        'End Try
                    Next
                End If
            End If

        Next

    End Sub

    Private Function GetProteinSortingIndices(FilePath As String) As Hashtable
        Dim fi = New FileInfo(FilePath)
        Dim tr As TextReader
        Dim s As String
        Dim nameRegex As Regex
        Dim m As Match
        Dim nameHash As New Hashtable
        Dim counter As Integer
        Dim tmpName As String

        nameRegex = New Regex(
            "^\>(?<name>\S+)\s*(?<description>.*)$",
            RegexOptions.Compiled)

        tr = fi.OpenText
        s = tr.ReadLine

        While Not s Is Nothing
            If nameRegex.IsMatch(s) Then
                counter += 1
                m = nameRegex.Match(s)
                tmpName = m.Groups("name").Value
                If Not nameHash.ContainsKey(tmpName.ToLower()) Then
                    nameHash.Add(tmpName.ToLower(), counter)
                End If
            End If
            s = tr.ReadLine
        End While

        tr.Close()

        Return nameHash

    End Function

        Dim proteinList As New Hashtable
    Public Sub CorrectMasses()
        Dim proteinTable As DataTable
        Dim dr As DataRow
        Dim counter As Integer
        Dim tmpRowCount = 1

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New clsDBTask(Me.m_PSConnectionString)
        End If

        Dim tmpTableInfo As DataTable = Me.m_TableGetter.GetTable(
            "SELECT TOP 1 TableRowCount " +
            "FROM V_Table_Row_Counts " +
            "WHERE TableName = 'T_Proteins'")

        dr = tmpTableInfo.Rows(0)

        Dim tmpProteinCount = CInt(dr.Item("TableRowCount"))

        Dim startcount As Integer

        Dim proteinSelectSQL As String

        Me.OnSyncStart("Starting Mass Update")

        While tmpRowCount > 0
            proteinList.Clear()
            startcount = counter
            counter = counter + 10000

            proteinSelectSQL = "SELECT Protein_ID, Sequence FROM T_Proteins " &
                "WHERE Protein_ID <= " + counter.ToString + " AND Protein_ID > " + startcount.ToString

            'proteinSelectSQL = "SELECT Protein_ID, Sequence FROM T_Proteins " &
            '                    "WHERE Protein_ID = 285130"
            '    "WHERE Protein_ID <= " + counter.ToString

            proteinTable = Me.m_TableGetter.GetTable(proteinSelectSQL)

            tmpRowCount = proteinTable.Rows.Count

            For Each dr In proteinTable.Rows
                proteinList.Add(CInt(dr.Item("Protein_ID")), dr.Item("Sequence").ToString())
            Next

            Me.OnSyncProgressUpdate("Processing Protein_ID " + startcount.ToString + "-" + counter.ToString + " of " + tmpProteinCount.ToString, CDbl(counter / tmpProteinCount))
            If proteinList.Count > 0 Then
                Me.UpdateProteinSequenceInfo(proteinList)
            End If
        End While

        Me.OnSyncCompletion()
    End Sub

    Public Sub RefreshNameHashes()
        Dim TotalNameCount As Integer
        Dim NameCountSQL = "SELECT TOP 1 Reference_ID FROM T_Protein_Names ORDER BY Reference_ID DESC"
        Dim tmptable As DataTable
        Dim dr As DataRow

        tmptable = Me.m_TableGetter.GetTable(NameCountSQL)
        TotalNameCount = CInt(tmptable.Rows(0).Item("Reference_ID"))

        Dim tmpRefID As Integer
        Dim tmpProteinName As String
        Dim tmpDescription As String
        Dim tmpProteinID As Integer

        Dim rowRetrievalSQL As String

        Dim startIndex = 0
        Dim counter As Integer
        Dim stepvalue = 10000

        If TotalNameCount <= stepvalue Then
            stepvalue = TotalNameCount
        End If
        Me.OnSyncStart("Updating Name Hashes")

        For counter = stepvalue To TotalNameCount + stepvalue Step stepvalue
            If counter >= TotalNameCount - stepvalue Then
                Debug.WriteLine("")
            End If
            rowRetrievalSQL = "SELECT Reference_ID, Name, Description, Protein_ID " &
                              "FROM T_Protein_Names " &
                              "WHERE Reference_ID > " & startIndex & " and Reference_ID <= " & counter &
                              "ORDER BY Reference_ID"

            'Protein_Name(+"_" + Description + "_" + ProteinID.ToString)
            tmptable = Me.m_TableGetter.GetTable(rowRetrievalSQL)
            If tmptable.Rows.Count > 0 Then
                For Each dr In tmptable.Rows
                    tmpRefID = DirectCast(dr.Item("Reference_ID"), Integer)
                    tmpProteinName = dr.Item("Name").ToString
                    tmpDescription = dr.Item("Description").ToString
                    tmpProteinID = DirectCast(dr.Item("Protein_ID"), Integer)

                    Me.m_Importer.UpdateProteinNameHash(
                        tmpRefID,
                        tmpProteinName,
                        tmpDescription,
                        tmpProteinID)
                Next
                tmptable.Clear()
                Me.OnSyncProgressUpdate("Processing " & startIndex & " to " & counter, CDbl(counter / TotalNameCount))
                startIndex = counter + 1
            End If

        Next
        Me.OnSyncCompletion()



    End Sub

    Protected Sub UpdateProteinSequenceInfo(Proteins As Hashtable)

        If Me.m_Importer Is Nothing Then
            Me.m_Importer = New clsAddUpdateEntries(Me.m_PSConnectionString)
        End If

        Dim si = New SequenceInfoCalculator.SequenceInfoCalculator

        For Each proteinID In Proteins.Keys
            Dim sequence = Proteins.Item(proteinID)
            si.CalculateSequenceInfo(sequence)
            Me.m_Importer.UpdateProteinSequenceInfo(
                proteinID, sequence, sequence.Length,
                si.MolecularFormula, si.MonoIsotopicMass,
                si.AverageMass, si.SHA1Hash)
        Next

    End Sub

    Private Sub OnSyncStart(StatusMsg As String)
        RaiseEvent SyncStart(StatusMsg)
    End Sub

    Private Sub OnSyncProgressUpdate(StatusMsg As String, fractionDone As Double)
        RaiseEvent SyncProgress(StatusMsg, fractionDone)
    End Sub

    Private Sub OnSyncCompletion()
        RaiseEvent SyncComplete()
    End Sub

    Private Sub m_Exporter_FileGenerationCompleted(FullOutputPath As String) Handles m_Exporter.FileGenerationCompleted
        m_GeneratedFastaFilePath = FullOutputPath
    End Sub
End Class
