Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Security.Principal
Imports System.Text
Imports System.Text.RegularExpressions
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports TableManipulationBase

Public Class clsGetFASTAFromDMSForward

    Protected ReadOnly m_DatabaseAccessor As IGetSQLData
    Protected WithEvents m_fileDumper As IExportProteins
    Protected m_AllCollections As Hashtable
    Protected m_OrganismList As Hashtable

    Protected m_CurrentFullOutputPath As String
    Friend m_CurrentFileProteinCount As Integer
    Protected m_CurrentArchiveFileName As String

    Protected m_CollectionsCache As DataTable
    Protected m_OrganismCache As DataTable

    Protected m_Naming_Suffix As String = "_forward"
    Protected m_Extension As String = ""

    Protected m_RijndaelDecryption As clsRijndaelEncryptionHandler

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
    ''' <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
    Public Sub New(
      databaseAccessor As IGetSQLData,
      databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes)

        m_DatabaseAccessor = databaseAccessor
        m_AllCollections = GetCollectionNameList()
        m_OrganismList = GetOrganismList()

        Select Case databaseFormatType
            Case IGetFASTAFromDMS.DatabaseFormatTypes.fasta
                m_fileDumper = New clsExportProteinsFASTA(Me)
                m_Extension = ".fasta"
            Case IGetFASTAFromDMS.DatabaseFormatTypes.fastapro
                m_fileDumper = New clsExportProteinsXTFASTA(Me)
                m_Extension = ".fasta.pro"
        End Select

    End Sub

    Event FileGenerationCompleted(outputPath As String)
    Event FileGenerationProgress(statusMsg As String, fractionDone As Double)
    Event FileGenerationStarted(taskMsg As String)

    Property FullOutputPath As String
        Get
            Return m_CurrentFullOutputPath
        End Get
        Set
            m_CurrentFullOutputPath = Value
        End Set
    End Property

    ' Unused
    'ReadOnly Property ArchivalName As String
    '    Get
    '        Return m_CurrentArchiveFileName
    '    End Get
    'End Property

    Protected Overridable Function ExtendedExportPath(
       destinationFolderPath As String,
       proteinCollectionName As String) As String

        Return Path.Combine(destinationFolderPath, proteinCollectionName + m_Naming_Suffix + m_Extension)

    End Function

    Overridable Function SequenceExtender(originalSequence As String, collectionCount As Integer) As String
        Return originalSequence
    End Function

    Overridable Function ReferenceExtender(originalReference As String) As String
        Return originalReference
    End Function

    ''' <summary>
    ''' Create the FASTA file for the given protein collections
    ''' </summary>
    ''' <param name="protCollectionList"></param>
    ''' <param name="destinationFolderPath"></param>
    ''' <param name="alternateAnnotationTypeID"></param>
    ''' <param name="padWithPrimaryAnnotation"></param>
    ''' <returns>CRC32 hash for the file</returns>
    Overridable Overloads Function ExportFASTAFile(
       protCollectionList As List(Of String),
       destinationFolderPath As String,
       alternateAnnotationTypeID As Integer,
       padWithPrimaryAnnotation As Boolean) As String

        Dim ProteinCollectionName As String
        Dim collectionSQL As String
        Dim collectionTable As DataTable

        Dim authorizationSQL As String
        Dim authorizationTable As DataTable

        Dim passPhraseSQL As String
        Dim passPhraseTable As DataTable

        Dim encCheckRows() As DataRow
        Dim authCheckRows() As DataRow

        Dim trueName As String = String.Empty

        Dim tmpID As Integer
        Dim tmpIDListSB As New StringBuilder
        Dim cipherSeq As String
        Dim clearSeq As String

        Dim decryptionRow As DataRow

        Dim nameCheckRegex As New Regex("(?<collectionname>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))")

        If Not CheckProteinCollectionNameValidity(protCollectionList) Then
            Return ""
        End If

        Dim m As Match

        Dim user As New WindowsPrincipal(WindowsIdentity.GetCurrent)
        Dim user_ID As String = user.Identity.Name

        ' Dictionary mapping protein collection name to the associated passphrase
        Dim proteinCollectionPassphrases = New Dictionary(Of String, String)

        Dim collectionNameList As String = String.Empty

        ' Check each collection name for encryption of contents
        For Each nameString As String In protCollectionList
            encCheckRows = m_CollectionsCache.Select("Filename = '" & nameString & "' AND Contents_Encrypted > 0")

            If encCheckRows.Length > 0 Then
                'Determine the encrypted collections to which this user has access
                authorizationSQL = "SELECT Protein_Collection_ID, Protein_Collection_Name " &
                  "FROM V_Encrypted_Collection_Authorizations " &
                  "WHERE Login_Name = '" & user_ID & "'"

                authorizationTable = m_DatabaseAccessor.GetTable(authorizationSQL)
                authCheckRows = authorizationTable.Select("Protein_Collection_Name = '" & nameString & "' OR Protein_Collection_Name = 'Administrator'")
                If authCheckRows.Length > 0 Then
                    tmpID = FindIDByName(nameString)
                    passPhraseSQL = "SELECT Passphrase " &
                     "FROM T_Encrypted_Collection_Passphrases " &
                     "WHERE Protein_Collection_ID = " & tmpID.ToString
                    passPhraseTable = m_DatabaseAccessor.GetTable(passPhraseSQL)

                    proteinCollectionPassphrases.Add(nameString, passPhraseTable.Rows(0).Item("Passphrase").ToString)
                Else
                    Throw New Exception("User " & user_ID & " does not have access to the encrypted collection '" & nameString & "'")
                End If
            End If

            If collectionNameList.Length > 0 Then collectionNameList &= ", "
            collectionNameList &= nameString
        Next

        ' Get a temp file name; however, create the file in the target folder path
        ' (in case the system-defined temp directory is on a different drive than the target folder)

        Dim fiOutputPathCheck As FileInfo
        Dim tmpOutputPath As String

        Do
            Dim tmpOutputPathCandidate = Path.GetTempFileName
            Try
                ' The GetTempFileName function created a temp file that we don't need; delete it now (but use try/catch just in case the deletion fails for some freak reason)
                File.Delete(tmpOutputPathCandidate)
            Catch ex As Exception
            End Try

            tmpOutputPath = Path.Combine(destinationFolderPath, Path.GetFileName(tmpOutputPathCandidate))
            fiOutputPathCheck = New FileInfo(tmpOutputPath)

        Loop While fiOutputPathCheck.Exists

        If protCollectionList.Count = 1 Then
            OnExportStart("Exporting protein collection " & collectionNameList)
        Else
            OnExportStart("Exporting " & protCollectionList.Count & "protein collections: " & collectionNameList)
        End If

        Dim proteinCollectionsExported = 0
        For Each ProteinCollectionName In protCollectionList
            Dim currentCollectionPos = 0
            Dim currentCollectionCount = 0

            ' Make sure there are no leading or trailing spaces
            ProteinCollectionName = ProteinCollectionName.Trim()

            If nameCheckRegex.IsMatch(ProteinCollectionName) Then
                m = nameCheckRegex.Match(ProteinCollectionName)
                trueName = m.Groups("collectionname").Value
            Else
                trueName = ProteinCollectionName
            End If

            ' Lookup the number of proteins that should be in this protein collection
            Dim lengthCheckSQL = "SELECT NumProteins FROM T_Protein_Collections " &
                                 "WHERE FileName = '" & ProteinCollectionName & "'"

            Dim lengthCheckTable = m_DatabaseAccessor.GetTable(lengthCheckSQL)
            Dim collectionLength As Integer

            If lengthCheckTable.Rows.Count > 0 Then
                Dim foundRow = lengthCheckTable.Rows(0)
                collectionLength = CType(foundRow.Item(0), Int32)
            Else
                collectionLength = -1
            End If

            Do
                Dim sectionStart = currentCollectionPos
                Dim sectionEnd = sectionStart + 10000

                If padWithPrimaryAnnotation Then
                    tmpID = FindIDByName(trueName)
                    collectionSQL =
                     "SELECT Name, Description, Sequence, Protein_ID " &
                     "FROM V_Protein_Database_Export " &
                     "WHERE " &
                       "Protein_Collection_ID = " & tmpID.ToString & " " &
                       "AND Sorting_Index BETWEEN " & sectionStart.ToString & " AND " & sectionEnd.ToString & " " &
                     "ORDER BY Sorting_Index"

                Else
                    collectionSQL =
                     "SELECT Name, Description, Sequence, Protein_ID " &
                     "FROM V_Protein_Database_Export " &
                     "WHERE Protein_Collection_ID = " & tmpID.ToString & ") " &
                       "AND Annotation_Type_ID = " & alternateAnnotationTypeID & " " &
                       "AND Sorting_Index BETWEEN " & sectionStart.ToString & " AND " & sectionEnd.ToString & " " &
                     "ORDER BY Sorting_Index"
                End If

                collectionTable = m_DatabaseAccessor.GetTable(collectionSQL)

                Dim passPhraseForCollection = ""
                If proteinCollectionPassphrases.TryGetValue(trueName, passPhraseForCollection) Then

                    m_RijndaelDecryption = New clsRijndaelEncryptionHandler(passPhraseForCollection)
                    For Each decryptionRow In collectionTable.Rows
                        cipherSeq = decryptionRow.Item("Sequence").ToString()
                        clearSeq = m_RijndaelDecryption.Decrypt(cipherSeq)
                        decryptionRow.Item("Sequence") = clearSeq
                        decryptionRow.AcceptChanges()
                    Next
                End If

                Dim tableName As String
                If collectionLength < 10000 Then
                    tableName = trueName
                Else
                    tableName = trueName + "_" + Format(sectionStart, "0000000000") + "-" + Format(sectionEnd, "0000000000")
                End If

                collectionTable.TableName = tableName

                m_CurrentFileProteinCount = collectionTable.Rows.Count

                'collection.Tables.Add(collectionTable)
                m_fileDumper.Export(collectionTable, tmpOutputPath)


                currentCollectionPos = sectionEnd + 1
                currentCollectionCount += collectionTable.Rows.Count

                Dim fractionDoneOverall As Double = 0
                If collectionLength > 0 Then
                    fractionDoneOverall = (proteinCollectionsExported / protCollectionList.Count) + (currentCollectionCount / collectionLength) / protCollectionList.Count
                End If

                OnExportProgressUpdate(currentCollectionCount & " entries exported, collection " & (proteinCollectionsExported + 1) & " of " & (protCollectionList.Count), fractionDoneOverall)

            Loop Until collectionTable.Rows.Count = 0

            tmpIDListSB.Append(Format(tmpID, "000000"))
            tmpIDListSB.Append("+")
            If currentCollectionCount <> collectionLength Then
                Throw New Exception(String.Format(
                    "The number of proteins exported for collection '{0}' does not match the expected value: " &
                    "{1} exported from T_Protein_Collection_Members vs. {2} listed in T_Protein_Collections",
                    ProteinCollectionName, currentCollectionCount, collectionLength))
            End If

            proteinCollectionsExported += 1
        Next
        OnExportComplete(tmpOutputPath)

        Dim tmpFI = New FileInfo(tmpOutputPath)

        tmpIDListSB.Remove(tmpIDListSB.Length - 1, 1)
        Dim name As String '= hash

        If protCollectionList.Count > 1 Then
            name = tmpIDListSB.ToString()
            If destinationFolderPath.Length + name.Length > 225 Then
                ' If exporting a large number of protein collections, name can be very long
                ' This can lead to error: The fully qualified file name must be less than 260 characters, and the directory name must be less than 248 characters
                ' Thus, truncate name
                Dim intMaxNameLength As Integer
                intMaxNameLength = 225 - destinationFolderPath.Length
                If intMaxNameLength < 30 Then intMaxNameLength = 30

                name = name.Substring(0, intMaxNameLength)

                ' Find the last plus sign and truncate just before it
                Dim intLastPlusLocation As Integer
                intLastPlusLocation = name.LastIndexOf("+"c)
                If intLastPlusLocation > 30 Then
                    name = name.Substring(0, intLastPlusLocation)
                End If

            End If
        Else
            name = trueName
        End If

        m_CurrentFullOutputPath = ExtendedExportPath(destinationFolderPath, name)
        m_CurrentArchiveFileName = name

        ' Rename (move) the temporary file to the final, full name
        If File.Exists(m_CurrentFullOutputPath) Then
            File.Delete(m_CurrentFullOutputPath)
        End If
        tmpFI.MoveTo(m_CurrentFullOutputPath)

        ' Assuming the final file now exists, delete the temporary file (if present)
        Dim finalOutputFile = New FileInfo(m_CurrentFullOutputPath)
        If finalOutputFile.Exists Then
            tmpFI = New FileInfo(tmpOutputPath)
            If tmpFI.Exists Then
                tmpFI.Delete()
            End If
        End If

        ' Determine the CRC32 hash of the output file
        ' This process will also rename the file, e.g. from "C:\Temp\SAR116_RBH_AA_012809_forward.fasta" to "C:\Temp\38FFACAC.fasta"
        Dim crc32Hash = m_fileDumper.Export(New DataTable, m_CurrentFullOutputPath)

        OnExportComplete(m_CurrentFullOutputPath)

        Return crc32Hash

    End Function

    ''' <summary>
    ''' Create the FASTA file for the given protein collections
    ''' </summary>
    ''' <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
    ''' <param name="destinationFolderPath"></param>
    ''' <returns>CRC32 hash of the generated (or retrieved) file</returns>
    Overridable Overloads Function ExportFASTAFile(
       protCollectionList As List(Of String),
       destinationFolderPath As String) As String

        Dim primaryAuthorityID = 1
        Const padWithPrimaryAnnotation = True

        Return ExportFASTAFile(protCollectionList, destinationFolderPath, primaryAuthorityID, padWithPrimaryAnnotation)

    End Function

    Protected Function CheckProteinCollectionNameValidity(protCollectionList As List(Of String)) As Boolean
        Dim name As String
        Dim id As Integer
        For Each name In protCollectionList
            id = FindIDByName(name)
            If id < 1 Then
                Throw New Exception("The collection named '" + name + "' does not exist in the system")
            End If
        Next
        Return True
    End Function

    ' Unused
    'Protected Function GetPrimaryAuthorityID(proteinCollectionID As Integer) As Integer
    '    Dim foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " & proteinCollectionID.ToString()).ToList()

    '    Dim primaryAnnotationTypeID = foundRows(0).Item("Primary_Annotation_Type_ID").ToString()
    '    Dim idValue As Integer
    '    If foundRows.Count = 0 OrElse Not Integer.TryParse(primaryAnnotationTypeID, idValue) Then
    '        Return 0
    '    End If

    '    Return idValue
    'End Function

    ' Unused
    'Protected Function GetPrimaryAuthorityID(proteinCollectionName As String) As Integer
    '    Dim proteinCollectionID As Integer = FindIDByName(proteinCollectionName)
    '    Return GetPrimaryAuthorityID(proteinCollectionID)
    'End Function

    Function GetCollectionNameList() As Hashtable 'Implements IGetFASTAFromDMS.GetAllCollections
        If m_DatabaseAccessor Is Nothing Then
            Return New Hashtable
        End If

        If m_CollectionsCache Is Nothing Then
            RefreshCollectionCache()
        End If

        Return m_DatabaseAccessor.DataTableToHashtable(m_CollectionsCache, "Protein_Collection_ID", "FileName")
    End Function

    Function GetCollectionsByOrganism(OrganismID As Integer) As Hashtable
        If m_DatabaseAccessor Is Nothing Then
            Return New Hashtable
        End If

        If m_CollectionsCache Is Nothing Then
            RefreshCollectionCache()
        End If

        Return m_DatabaseAccessor.DataTableToHashtable(m_CollectionsCache, "Protein_Collection_ID", "FileName", "[Organism_ID] = " & OrganismID.ToString)
    End Function

    Function GetCollectionsByOrganismTable(OrganismID As Integer) As DataTable
        Dim tmpTable As DataTable = m_CollectionsCache.Clone

        Dim dr As DataRow
        Dim foundRows() As DataRow = m_CollectionsCache.Select("[Organism_ID] = " & OrganismID.ToString)

        For Each dr In foundRows
            tmpTable.ImportRow(dr)
        Next

        Return tmpTable
    End Function

    Function GetOrganismList() As Hashtable
        If m_DatabaseAccessor Is Nothing Then
            Return New Hashtable
        End If

        If m_OrganismCache Is Nothing Then
            RefreshOrganismCache()
        End If

        Return m_DatabaseAccessor.DataTableToHashtable(m_OrganismCache, "Organism_ID", "Name")
    End Function

    Function GetOrganismListTable() As DataTable
        If m_DatabaseAccessor Is Nothing Then
            Return New DataTable
        End If

        If m_OrganismCache Is Nothing Then
            RefreshOrganismCache()
        End If

        Return m_OrganismCache
    End Function

    Protected Sub RefreshCollectionCache()
        If m_DatabaseAccessor Is Nothing OrElse String.IsNullOrWhiteSpace(m_DatabaseAccessor.ConnectionString) Then
            m_CollectionsCache = New DataTable()
        Else
            m_CollectionsCache = m_DatabaseAccessor.GetTable(
                "SELECT * FROM V_Protein_Collections_By_Organism ORDER BY Protein_Collection_ID")
        End If
    End Sub

    Protected Sub RefreshOrganismCache()
        If m_DatabaseAccessor Is Nothing OrElse String.IsNullOrWhiteSpace(m_DatabaseAccessor.ConnectionString) Then
            m_OrganismCache = New DataTable()
        Else
            m_OrganismCache = m_DatabaseAccessor.GetTable(
                "SELECT ID as Organism_ID, Short_Name as Name FROM V_Organism_Picker ORDER BY Organism_ID")
        End If

    End Sub

    Function FindIDByName(CollectionName As String) As Integer
        If CollectionName.Length = 0 Then
            Return 0
        End If
        'Dim dr As DataRow
        Dim foundRows() As DataRow

        ' Make sure there are no leading or trailing spaces
        CollectionName = CollectionName.Trim()
        foundRows = m_CollectionsCache.Select("[FileName] = '" & CollectionName & "'")
        If foundRows.Length = 0 Then
            RefreshCollectionCache()
            foundRows = m_CollectionsCache.Select("[FileName] = '" & CollectionName & "'")
        End If
        Dim id As Integer
        Try
            id = CInt(foundRows(0).Item("Protein_Collection_ID"))
        Catch ex As Exception
            id = -1
        End Try
        Return id
    End Function

    Function FindNameByID(CollectionID As Integer) As String
        Dim foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " & CollectionID.ToString).ToList()

        If foundRows.Count = 0 Then
            RefreshCollectionCache()
            foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " & CollectionID.ToString).ToList()
        End If

        If foundRows.Count > 0 Then
            Return foundRows(0).Item("FileName").ToString()
        End If

        Return String.Empty
    End Function

    Protected Function FindPrimaryAnnotationID(collectionID As Integer) As Integer
        Dim foundRows = m_CollectionsCache.Select("Protein_Collection_ID = " & collectionID.ToString()).ToList()

        If foundRows.Count > 0 Then
            Return CInt(foundRows(0).Item("Primary_Annotation_Type_ID"))
        End If

        Return 0
    End Function

    ''' <summary>
    ''' Compute the CRC32 hash for the file
    ''' </summary>
    ''' <param name="fullFilePath"></param>
    ''' <returns>File hash</returns>
    Function GetFileHash(FullFilePath As String) As String

        Return m_fileDumper.GenerateFileAuthenticationHash(FullFilePath)

    End Function

    Function GetStoredHash(ProteinCollectionName As String) As String
        Dim foundRows() As DataRow = m_CollectionsCache.Select("[FileName] = '" & ProteinCollectionName & "'")
        Return CStr(foundRows(0).Item("Authentication_Hash"))
    End Function

    Function GetStoredHash(ProteinCollectionID As Integer) As String
        Dim ProteinCollectionName = CStr(m_AllCollections.Item(ProteinCollectionID))
        Return GetStoredHash(ProteinCollectionName)
    End Function

    Protected Sub OnExportStart(taskMsg As String) Handles m_fileDumper.ExportStart
        RaiseEvent FileGenerationStarted(taskMsg)
    End Sub

    Protected Sub OnExportProgressUpdate(statusMsg As String, fractionDone As Double) Handles m_fileDumper.ExportProgress
        RaiseEvent FileGenerationProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportComplete(outputFilePath As String)
        RaiseEvent FileGenerationCompleted(outputFilePath)
    End Sub

End Class
