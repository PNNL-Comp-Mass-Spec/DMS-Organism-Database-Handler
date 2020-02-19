Option Strict On

Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports PRISMDatabaseUtils
Imports TableManipulationBase

Public Class clsArchiveToFile
    Inherits clsArchiveOutputFilesBase

    Const DEFAULT_BASE_ARCHIVE_PATH As String = "\\gigasax\DMS_FASTA_File_Archive\"

    Protected ReadOnly m_BaseArchivePath As String
    Protected ReadOnly m_SHA1Provider As SHA1Managed

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor"></param>
    ''' <param name="exporterModule"></param>
    Sub New(databaseAccessor As clsDBTask, exporterModule As clsGetFASTAFromDMS)

        MyBase.New(databaseAccessor, exporterModule)

        If databaseAccessor Is Nothing Then
            m_BaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH
        Else
            Dim connectionStringCheck = databaseAccessor.ConnectionString.ToLower().Replace(" ", "")

            If connectionStringCheck.Contains("source=cbdms") Then
                m_BaseArchivePath = "\\cbdms\DMS_FASTA_File_Archive\"
            Else
                m_BaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH
            End If
        End If

        m_SHA1Provider = New SHA1Managed()
    End Sub

    Protected Overrides Function DispositionFile(
     proteinCollectionID As Integer,
     sourceFilePath As String,
     creationOptionsString As String,
     sourceAuthenticationHash As String,
     outputSequenceType As clsGetFASTAFromDMS.SequenceTypes,
     archivedFileType As clsArchiveOutputFilesBase.CollectionTypes,
     proteinCollectionsList As String) As Integer

        Dim CollectionListHexHash As String
        Dim CollectionListHexHashInDB As String

        Dim ProteinCollectionsListFromDB As String

        Dim ArchivedFileEntryID As Integer

        Dim archivePath As String
        Dim fi = New FileInfo(sourceFilePath)
        Dim destFI As FileInfo
        Dim di As DirectoryInfo

        Dim proteinCount As Integer

        'Check for existence of Archive Entry
        Dim checkSQL As String = "SELECT Archived_File_ID, Archived_File_Path, IsNull(Protein_Collection_List, '') as Protein_Collection_List, IsNull(Collection_List_Hex_Hash, '') AS Collection_List_Hex_Hash " &
          "FROM T_Archived_Output_Files " &
          "WHERE Authentication_Hash = '" & sourceAuthenticationHash & "' AND " &
          "Archived_File_State_ID <> 3 " &
          "ORDER BY File_Modification_Date DESC"

        Dim tmpTable As DataTable = m_DatabaseAccessor.GetTable(checkSQL)
        CollectionListHexHash = GenerateHash(proteinCollectionsList + "/" + creationOptionsString)
        If tmpTable.Rows.Count = 0 Then
            proteinCount = GetProteinCount(sourceFilePath)

            archivePath = GenerateArchivePath(
              sourceFilePath, proteinCollectionID,
              fi.LastWriteTime,
              sourceAuthenticationHash,
              archivedFileType, outputSequenceType)

            ArchivedFileEntryID = RunSP_AddOutputFileArchiveEntry(
              proteinCollectionID, creationOptionsString, sourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount,
              archivePath, [Enum].GetName(GetType(clsArchiveOutputFilesBase.CollectionTypes), archivedFileType), proteinCollectionsList, CollectionListHexHash)

            tmpTable = m_DatabaseAccessor.GetTable(checkSQL)

        Else
            ' Archived file entry already exists

            ArchivedFileEntryID = CInt(tmpTable.Rows(0).Item("Archived_File_ID"))
            CollectionListHexHashInDB = CStr(tmpTable.Rows(0).Item("Collection_List_Hex_Hash"))
            ProteinCollectionsListFromDB = CStr(tmpTable.Rows(0).Item("Protein_Collection_List"))

            If tmpTable.Rows(0).Item("Protein_Collection_List").GetType.Name = "DBNull" OrElse
             CollectionListHexHashInDB = "" OrElse
             ProteinCollectionsListFromDB <> proteinCollectionsList OrElse
             CollectionListHexHashInDB <> CollectionListHexHash Then
                RunSP_UpdateFileArchiveEntryCollectionList(ArchivedFileEntryID, proteinCollectionsList, sourceAuthenticationHash, CollectionListHexHash)
            End If
        End If
        m_Archived_File_Name = tmpTable.Rows(0).Item("Archived_File_Path").ToString


        Try
            di = New DirectoryInfo(Path.GetDirectoryName(m_Archived_File_Name))
            destFI = New FileInfo(m_Archived_File_Name)
            If Not di.Exists Then
                di.Create()
            End If

            If Not destFI.Exists Then
                fi.CopyTo(m_Archived_File_Name)
            End If

        Catch exUnauthorized As UnauthorizedAccessException
            Console.WriteLine("  Warning: access denied copying file to " & m_Archived_File_Name)
        Catch ex As Exception
            m_LastError = "File copying error: " + ex.Message
            Return 0
        End Try

        Return ArchivedFileEntryID

    End Function

    Protected Function GenerateHash(sourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim encoding As New ASCIIEncoding()

        'Retrieve a byte array based on the source text
        Dim byteSourceText() As Byte = encoding.GetBytes(sourceText)

        'Compute the hash value from the source
        Dim sha1Hash() As Byte = m_SHA1Provider.ComputeHash(byteSourceText)

        'And convert it to String format for return
        Dim sha1String As String = BitConverter.ToString(sha1Hash).Replace("-", "").ToLower()

        Return sha1String
    End Function

    Protected Function GenerateArchivePath(
     sourceFilePath As String,
     proteinCollectionID As Integer,
     fileDate As DateTime,
     authentication_Hash As String,
     archivedFileType As clsArchiveOutputFilesBase.CollectionTypes,
     outputSequenceType As clsGetFASTAFromDMS.SequenceTypes) As String

        Dim pathString As String
        pathString = Path.Combine(m_BaseArchivePath, [Enum].GetName(GetType(clsArchiveOutputFilesBase.CollectionTypes), archivedFileType))
        pathString = Path.Combine(pathString, [Enum].GetName(GetType(clsGetFASTAFromDMS.SequenceTypes), outputSequenceType))
        pathString = Path.Combine(pathString, "ID_00000_" + authentication_Hash + Path.GetExtension(sourceFilePath))

        Return pathString

    End Function

    Protected Function RunSP_UpdateFileArchiveEntryCollectionList(
     archivedFileEntryID As Integer,
     proteinCollectionsList As String,
     collectionListHash As String,
     collectionListHexHash As String) As Integer


        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("UpdateFileArchiveEntryCollectionList", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Archived_File_Entry_ID", SqlType.Int).Value = archivedFileEntryID
        dbTools.AddParameter(cmdSave, "@ProteinCollectionList", SqlType.VarChar, 8000).Value = proteinCollectionsList
        dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 28).Value = collectionListHash
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.Output
        dbTools.AddParameter(cmdSave, "@CollectionListHexHash", SqlType.VarChar, 128).Value = collectionListHexHash

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddOutputFileArchiveEntry(
     proteinCollectionID As Integer,
     creationOptionsString As String,
     authentication_Hash As String,
     fileModificationDate As DateTime,
     outputFileSize As Int64,
     proteinCount As Integer,
     archivedFileFullPath As String,
     archivedFileType As String,
     proteinCollectionsList As String,
     collectionListHexHash As String) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddOutputFileArchiveEntry", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionID
        dbTools.AddParameter(cmdSave, "@crc32_authentication", SqlType.VarChar, 40).Value = authentication_Hash
        dbTools.AddParameter(cmdSave, "@file_modification_date", SqlType.DateTime).Value = fileModificationDate
        dbTools.AddParameter(cmdSave, "@file_size", SqlType.BigInt).Value = outputFileSize
        dbTools.AddParameter(cmdSave, "@protein_count", SqlType.Int).Value = proteinCount
        dbTools.AddParameter(cmdSave, "@archived_file_type", SqlType.VarChar, 64).Value = archivedFileType
        dbTools.AddParameter(cmdSave, "@creation_options", SqlType.VarChar, 250).Value = creationOptionsString
        dbTools.AddParameter(cmdSave, "@protein_collection_string", SqlType.VarChar, 8000).Value = proteinCollectionsList
        dbTools.AddParameter(cmdSave, "@collection_string_hash", SqlType.VarChar, 40).Value = collectionListHexHash
        dbTools.AddParameter(cmdSave, "@archived_file_path", SqlType.VarChar, 250).Value = archivedFileFullPath
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512).Direction = ParameterDirection.Output

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        m_Archived_File_Name = archivedFileFullPath

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function



End Class
