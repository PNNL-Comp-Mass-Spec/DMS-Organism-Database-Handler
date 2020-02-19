Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports PRISMDatabaseUtils
Imports ProteinFileReader
Imports Protein_Exporter
Imports Protein_Importer
Imports TableManipulationBase

Public Class clsSyncFASTAFileArchive

    ' Private m_FileArchiver As IArchiveOutputFiles
    Private ReadOnly m_DatabaseAccessor As clsDBTask
    Private ReadOnly m_Importer As clsAddUpdateEntries

    Private WithEvents m_Exporter As clsGetFASTAFromDMS

    Event SyncStart(statusMsg As String)
    Event SyncProgress(statusMsg As String, fractionDone As Double)
    Event SyncComplete()

    Private m_CurrentStatusMsg As String
    Private m_CurrentProteinCount As Integer
    Private m_TotalProteinsCount As Integer

    Private m_GeneratedFastaFilePath As String

    Public Sub New(psConnectionString As String)

        m_DatabaseAccessor = New clsDBTask(psConnectionString)
        m_Importer = New clsAddUpdateEntries(psConnectionString)

    End Sub

    Public Function SyncCollectionsAndArchiveTables(outputPath As String) As Integer
        Dim sql =
                "SELECT Protein_Collection_ID, FileName, Authentication_Hash, DateModified, Collection_Type_ID, NumProteins " &
                "FROM V_Missing_Archive_Entries"

        'TODO add collection list string
        Dim proteinCollectionList = ""


        Dim dt As DataTable
        dt = m_DatabaseAccessor.GetTable(sql)
        Dim dr As DataRow
        Dim sourceFilePath As String
        Dim proteinCollectionID As Integer
        Dim SHA1 As String
        Dim CreationOptionsString = "seq_direction=forward,filetype=fasta"
        Dim totalProteinsCount As Integer
        Dim currentCollectionProteinCount = 0
        For Each dr In dt.Rows
            m_TotalProteinsCount += CInt(dr.Item("NumProteins"))
        Next

        Dim outputSequenceType = clsGetFASTAFromDMS.SequenceTypes.forward
        Dim databaseFormatType = clsGetFASTAFromDMS.DatabaseFormatTypes.fasta

        OnSyncStart("Synchronizing Archive Table with Collections Table")

        Dim fileArchiver = New clsArchiveToFile(m_DatabaseAccessor, m_Exporter)

        For Each dr In dt.Rows
            OnSyncProgressUpdate("Processing - '" & dr.Item("FileName").ToString & "'", CDbl(currentCollectionProteinCount / totalProteinsCount))
            currentCollectionProteinCount = CInt(dr.Item("NumProteins"))
            proteinCollectionID = CInt(dr.Item("Protein_Collection_ID"))
            sourceFilePath = Path.Combine(outputPath, dr.Item("FileName").ToString & ".fasta")
            SHA1 = dr.Item("Authentication_Hash").ToString


            fileArchiver.ArchiveCollection(
                proteinCollectionID,
                clsArchiveOutputFilesBase.CollectionTypes.static,
                outputSequenceType, databaseFormatType, sourceFilePath, CreationOptionsString, SHA1, proteinCollectionList)

        Next

        OnSyncCompletion()

        Return 0

    End Function

    Public Sub UpdateSHA1Hashes() 'Implements IArchiveOutputFiles.UpdateSHA1Hashes

        Dim sql As String

        sql = "SELECT Protein_Collection_ID, FileName, Authentication_Hash, NumProteins " &
        "FROM V_Missing_Archive_Entries"

        Dim dt As DataTable

        dt = m_DatabaseAccessor.GetTable(sql)

        Dim dr As DataRow
        Dim tmpID As Integer
        Dim tmpStoredSHA As String
        Dim tmpGenSHA As String
        Dim tmpFilename As String
        Dim fi As FileInfo
        Dim tmpFullPath As String

        For Each dr In dt.Rows
            m_TotalProteinsCount += CInt(dr.Item("Numproteins"))
        Next

        Dim startTime As DateTime
        Dim elapsedTime As TimeSpan
        Dim elapsedTimeSB As New StringBuilder

        Dim tmpPath As String = Path.GetTempPath

        Dim connectionString As String
        If m_DatabaseAccessor Is Nothing OrElse String.IsNullOrWhiteSpace(m_DatabaseAccessor.ConnectionString) Then
            connectionString = String.Empty
        Else
            connectionString = m_DatabaseAccessor.ConnectionString
        End If

        m_Exporter = New clsGetFASTAFromDMS(
            connectionString, clsGetFASTAFromDMS.DatabaseFormatTypes.fasta,
            clsGetFASTAFromDMS.SequenceTypes.forward)

        Dim creationOptionsString As String
        creationOptionsString = "seq_direction=forward,filetype=fasta"
        OnSyncStart("Updating Collections and Archive Entries")
        startTime = DateTime.UtcNow

        Dim fileArchiver = New clsArchiveToFile(m_DatabaseAccessor, m_Exporter)

        For Each dr In dt.Rows
            tmpID = CInt(dr.Item("Protein_Collection_ID"))
            tmpStoredSHA = dr.Item("Authentication_Hash").ToString
            tmpFilename = dr.Item("FileName").ToString
            m_GeneratedFastaFilePath = String.Empty

            elapsedTimeSB.Remove(0, elapsedTimeSB.Length)
            elapsedTime = DateTime.UtcNow.Subtract(startTime)
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


            'OnSyncProgressUpdate(
            '    "Collection "
            '    & Format(tmpID, "0000")
            '    & " [Elapsed Time: "
            '    & elapsedTimeSB.ToString & "]",
            '    CDbl(currentProteinCount / totalProteinCount))
            m_CurrentStatusMsg = "Collection " & Format(tmpID, "0000") & " [Elapsed Time: " & elapsedTimeSB.ToString() & "]"

            OnSyncProgressUpdate(
                m_CurrentStatusMsg,
                CDbl(m_CurrentProteinCount / m_TotalProteinsCount))

            tmpFullPath = Path.Combine(tmpPath, tmpFilename & ".fasta")
            'Debug.WriteLine("Start: " & tmpFilename & ": " & startTime.ToLongTimeString)

            tmpGenSHA = m_Exporter.ExportFASTAFile(tmpID, tmpPath,
                clsGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                clsGetFASTAFromDMS.SequenceTypes.forward)

            If Not tmpStoredSHA.Equals(tmpGenSHA) Then
                Dim currentFastaProteinCount = 0
                Dim currentFastaResidueCount = 0

                If Not String.IsNullOrEmpty(m_GeneratedFastaFilePath) Then
                    CountProteinsAndResidues(m_GeneratedFastaFilePath, currentFastaProteinCount, currentFastaResidueCount)
                End If

                m_Importer.AddAuthenticationHash(tmpID, tmpGenSHA, currentFastaProteinCount, currentFastaResidueCount)
            End If

            'Debug.WriteLine("End: " & tmpFilename & ": " & DateTime.Now.ToLongTimeString)
            'Debug.Flush()

            fileArchiver.ArchiveCollection(
                tmpID,
                clsArchiveOutputFilesBase.CollectionTypes.static,
                clsGetFASTAFromDMS.SequenceTypes.forward,
                clsGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                tmpFullPath, creationOptionsString, tmpGenSHA, "")
            'ArchiveCollection(
            '    tmpID,
            '    IArchiveOutputFiles.CollectionTypes.static,
            '    tmpFullPath, tmpGenSHA)

            'tmpNameList.Clear()
            fi = New FileInfo(tmpFullPath)
            fi.Delete()
            m_CurrentProteinCount += CInt(dr.Item("NumProteins"))

        Next

        OnSyncCompletion()

        'OnSyncStart("Updating ProteinNames

        'Update T_Protein_Names

        '    Dim tmpRefID As Integer
        '    Dim tmpName As String
        '    Dim tmpFingerprint As String
        '    Dim tmpProtID As Integer
        '    Dim errorCode As Integer
        '    Dim counter As Integer = 0

        '    sql = "SELECT Reference_ID, Name, Reference_Fingerprint, Protein_ID " &
        '            "FROM T_Protein_Names"

        '    dt = m_DatabaseAccessor.GetTable(sql)

        '    Dim dbTools = m_DatabaseAccessor.DBTools

        '    For Each dr In dt.Rows
        '        counter += 1
        '        tmpRefID = dbTools.GetInteger(dr.Item("Reference_ID"))
        '        tmpName = dbTools.GetString(dr.Item("Name"))
        '        tmpFingerprint = dbTools.GetString(dr.Item("Reference_Fingerprint"))
        '        tmpProtID = dbTools.GetInteger(dr.Item("Protein_ID"))

        '        'tmpGenSHA = m_Importer.GenerateArbitraryHash(tmpName + tmpProtID.ToString)
        '        errorCode = m_Importer.UpdateProteinNameHash(tmpRefID, tmpName, tmpProtID)
        '        If counter Mod 2000 = 0 Then
        '            Debug.WriteLine(counter.ToString)
        '        End If
        '    Next

        '    'Update T_Proteins

        '    Dim tmpSeq As String
        '    counter = 0

        '    sql = "SELECT Protein_ID, Sequence " &
        '            "FROM T_Proteins"

        '    dt = m_DatabaseAccessor.GetTable(sql)

        '    For Each dr In dt.Rows
        '        counter += 1
        '        tmpProtID = dbTools.GetInteger(dr.Item("Protein_ID"))
        '        tmpSeq = dbTools.GetString(dr.Item("Sequence"))

        '        errorCode = m_Importer.UpdateProteinSequenceHash(tmpProtID, tmpSeq)

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

        Dim sql = "SELECT * FROM T_Temp_Archive_Path_Fix"

        Dim tmpTable As DataTable = m_DatabaseAccessor.GetTable(sql)

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

        Dim getCollectionsSQL = "SELECT Protein_Collection_ID, FileName, Organism_ID FROM V_Protein_Collections_By_Organism WHERE Collection_Type_ID = 1 or Collection_Type_ID = 5"

        Dim collectionTable As DataTable = m_DatabaseAccessor.GetTable(getCollectionsSQL)

        Dim getLegacyFilesSQL = "SELECT DISTINCT FileName, Full_Path, Organism_ID FROM V_Legacy_Static_File_Locations"
        Dim legacyTable As DataTable = m_DatabaseAccessor.GetTable(getLegacyFilesSQL)

        Dim nameIndexHash As Dictionary(Of String, Integer)

        Dim dbTools = m_DatabaseAccessor.DBTools

        For Each collectionEntry As DataRow In collectionTable.Rows
            Dim tmpCollectionName = collectionEntry.Item("FileName").ToString
            Dim tmpCollectionID = CInt(collectionEntry.Item("Protein_Collection_ID"))
            If tmpCollectionID = 1026 Then
                Debug.WriteLine("")
            End If

            Dim tmpOrgID = CInt(collectionEntry.Item("Organism_ID"))

            Dim legacyFoundRows = legacyTable.Select("FileName = '" & tmpCollectionName & ".fasta' AND Organism_ID = " & tmpOrgID)
            If legacyFoundRows.Length > 0 Then
                Dim getReferencesSQL = "SELECT * FROM V_Tmp_Member_Name_Lookup WHERE Protein_Collection_ID = " & tmpCollectionID.ToString &
                                    " AND Sorting_Index is NULL"
                Dim referencesTable = m_DatabaseAccessor.GetTable(getReferencesSQL)
                If referencesTable.Rows.Count > 0 Then
                    Dim legacyFileEntry = legacyFoundRows(0)
                    Dim legacyFullPath = legacyFileEntry.Item("Full_Path").ToString
                    nameIndexHash = GetProteinSortingIndices(legacyFullPath)

                    For Each referenceEntry As DataRow In referencesTable.Rows
                        Dim tmpRefID = dbTools.GetInteger(referenceEntry.Item("Reference_ID"))
                        Dim tmpProteinID = dbTools.GetInteger(referenceEntry.Item("Protein_ID"))
                        Dim tmpRefName = dbTools.GetString(referenceEntry.Item("Name"))

                        'Try
                        Dim tmpSortingIndex = nameIndexHash.Item(tmpRefName.ToLower())

                        If tmpSortingIndex > 0 Then
                            m_Importer.UpdateProteinCollectionMember(
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

    Private Function GetProteinSortingIndices(filePath As String) As Dictionary(Of String, Integer)
        Dim fi = New FileInfo(filePath)
        Dim tr As TextReader
        Dim s As String
        Dim nameRegex As Regex
        Dim m As Match
        Dim nameHash As New Dictionary(Of String, Integer)
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

    Public Sub CorrectMasses()

        Dim proteinList = New Dictionary(Of Integer, String)
        Dim proteinTable As DataTable
        Dim dr As DataRow
        Dim counter As Integer
        Dim tmpRowCount = 1

        Dim tmpTableInfo As DataTable = m_DatabaseAccessor.GetTable(
            "SELECT TOP 1 TableRowCount " +
            "FROM V_Table_Row_Counts " +
            "WHERE TableName = 'T_Proteins'")

        dr = tmpTableInfo.Rows(0)

        Dim tmpProteinCount = CInt(dr.Item("TableRowCount"))

        Dim startCount As Integer

        Dim proteinSelectSQL As String

        OnSyncStart("Starting Mass Update")

        While tmpRowCount > 0
            proteinList.Clear()
            startCount = counter
            counter = counter + 10000

            proteinSelectSQL = "SELECT Protein_ID, Sequence FROM T_Proteins " &
                "WHERE Protein_ID <= " + counter.ToString + " AND Protein_ID > " + startCount.ToString()

            'proteinSelectSQL = "SELECT Protein_ID, Sequence FROM T_Proteins " &
            '                    "WHERE Protein_ID = 285130"
            '    "WHERE Protein_ID <= " + counter.ToString

            proteinTable = m_DatabaseAccessor.GetTable(proteinSelectSQL)

            tmpRowCount = proteinTable.Rows.Count

            For Each dr In proteinTable.Rows
                proteinList.Add(CInt(dr.Item("Protein_ID")), dr.Item("Sequence").ToString())
            Next

            OnSyncProgressUpdate("Processing Protein_ID " + startCount.ToString() + "-" + counter.ToString() + " of " + tmpProteinCount.ToString(), CDbl(counter / tmpProteinCount))
            If proteinList.Count > 0 Then
                UpdateProteinSequenceInfo(proteinList)
            End If
        End While

        OnSyncCompletion()
    End Sub

    Public Sub RefreshNameHashes()
        Dim totalNameCount As Integer
        Dim nameCountSQL = "SELECT TOP 1 Reference_ID FROM T_Protein_Names ORDER BY Reference_ID DESC"
        Dim dr As DataRow

        Dim nameCountResults = m_DatabaseAccessor.GetTable(nameCountSQL)
        totalNameCount = CInt(nameCountResults.Rows(0).Item("Reference_ID"))

        Dim tmpRefID As Integer
        Dim tmpProteinName As String
        Dim tmpDescription As String
        Dim tmpProteinID As Integer

        Dim startIndex = 0
        Dim counter As Integer
        Dim stepValue = 10000

        If totalNameCount <= stepValue Then
            stepValue = totalNameCount
        End If
        OnSyncStart("Updating Name Hashes")

        Dim dbTools = m_DatabaseAccessor.DBTools

        For counter = stepValue To totalNameCount + stepValue Step stepValue
            If counter >= totalNameCount - stepValue Then
                Debug.WriteLine("")
            End If
            Dim rowRetrievalSQL = "SELECT Reference_ID, Name, Description, Protein_ID " &
                                  "FROM T_Protein_Names " &
                                  "WHERE Reference_ID > " & startIndex & " and Reference_ID <= " & counter &
                                  "ORDER BY Reference_ID"

            'Protein_Name(+"_" + Description + "_" + ProteinID.ToString)
            Dim proteinListResults = m_DatabaseAccessor.GetTable(rowRetrievalSQL)
            If proteinListResults.Rows.Count > 0 Then
                For Each dr In proteinListResults.Rows
                    tmpRefID = dbTools.GetInteger(dr.Item("Reference_ID"))
                    tmpProteinName = dbTools.GetString(dr.Item("Name"))
                    tmpDescription = dbTools.GetString(dr.Item("Description"))
                    tmpProteinID = dbTools.GetInteger(dr.Item("Protein_ID"))

                    m_Importer.UpdateProteinNameHash(
                        tmpRefID,
                        tmpProteinName,
                        tmpDescription,
                        tmpProteinID)
                Next
                OnSyncProgressUpdate("Processing " & startIndex & " to " & counter, CDbl(counter / totalNameCount))
                startIndex = counter + 1
            End If

        Next
        OnSyncCompletion()



    End Sub

    Private Sub UpdateProteinSequenceInfo(proteins As Dictionary(Of Integer, String))

        Dim si = New SequenceInfoCalculator.SequenceInfoCalculator

        For Each proteinID In proteins.Keys
            Dim sequence = proteins.Item(proteinID)
            si.CalculateSequenceInfo(sequence)
            m_Importer.UpdateProteinSequenceInfo(
                proteinID, sequence, sequence.Length,
                si.MolecularFormula, si.MonoisotopicMass,
                si.AverageMass, si.SHA1Hash)
        Next

    End Sub

    Private Sub OnSyncStart(statusMsg As String)
        RaiseEvent SyncStart(statusMsg)
    End Sub

    Private Sub OnSyncProgressUpdate(statusMsg As String, fractionDone As Double)
        RaiseEvent SyncProgress(statusMsg, fractionDone)
    End Sub

    Private Sub OnSyncCompletion()
        RaiseEvent SyncComplete()
    End Sub

    Private Sub m_Exporter_FileGenerationCompleted(fullOutputPath As String) Handles m_Exporter.FileGenerationCompleted
        m_GeneratedFastaFilePath = fullOutputPath
    End Sub
End Class
