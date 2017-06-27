Option Strict On

Imports System.Data.SqlClient
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsArchiveToFile
    Inherits clsArchiveOutputFilesBase

    Const DEFAULT_BASE_ARCHIVE_PATH As String = "\\gigasax\DMS_FASTA_File_Archive\"

    Protected ReadOnly m_BaseArchivePath As String
    Protected ReadOnly m_SHA1Provider As SHA1Managed

    Sub New(PSConnectionString As String, ByRef ExporterModule As clsGetFASTAFromDMS)

        MyBase.New(PSConnectionString, ExporterModule)

        Dim connectionStringCheck = PSConnectionString.ToLower().Replace(" ", "")

        If connectionStringCheck.Contains("source=cbdms") Then
            m_BaseArchivePath = "\\cbdms\DMS_FASTA_File_Archive\"
        Else
            m_BaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH
        End If

        m_SHA1Provider = New SHA1Managed()
    End Sub

    Protected Overrides Function DispositionFile(
     ProteinCollectionID As Integer,
     SourceFilePath As String,
     CreationOptionsString As String,
     SourceAuthenticationHash As String,
     OutputSequenceType As IGetFASTAFromDMS.SequenceTypes,
     ArchivedFileType As IArchiveOutputFiles.CollectionTypes,
     ProteinCollectionsList As String) As Integer

        Dim CollectionListHexHash As String
        Dim CollectionListHexHashInDB As String

        Dim ProteinCollectionsListFromDB As String

        CheckTableGetterStatus()

        Dim ArchivedFileEntryID As Integer

        Dim archivePath As String
        Dim fi = New FileInfo(SourceFilePath)
        Dim destFI As FileInfo
        Dim di As DirectoryInfo

        Dim proteinCount As Integer

        'Check for existence of Archive Entry
        Dim checkSQL As String = "SELECT Archived_File_ID, Archived_File_Path, IsNull(Protein_Collection_List, '') as Protein_Collection_List, IsNull(Collection_List_Hex_Hash, '') AS Collection_List_Hex_Hash " &
          "FROM T_Archived_Output_Files " &
          "WHERE Authentication_Hash = '" & SourceAuthenticationHash & "' AND " &
          "Archived_File_State_ID <> 3 " &
          "ORDER BY File_Modification_Date DESC"

        Dim tmptable As DataTable = m_TableGetter.GetTable(checkSQL)
        CollectionListHexHash = GenerateHash(ProteinCollectionsList + "/" + CreationOptionsString)
        If tmptable.Rows.Count = 0 Then
            proteinCount = GetProteinCount(SourceFilePath)

            archivePath = GenerateArchivePath(
              SourceFilePath, ProteinCollectionID,
              fi.LastWriteTime,
              SourceAuthenticationHash,
              ArchivedFileType, OutputSequenceType)

            ArchivedFileEntryID = RunSP_AddOutputFileArchiveEntry(
              ProteinCollectionID, CreationOptionsString, SourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount,
              archivePath, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType), ProteinCollectionsList, CollectionListHexHash)

            tmptable = m_TableGetter.GetTable(checkSQL)

        Else
            ' Archived file entry already exists

            ArchivedFileEntryID = CInt(tmptable.Rows(0).Item("Archived_File_ID"))
            CollectionListHexHashInDB = CStr(tmptable.Rows(0).Item("Collection_List_Hex_Hash"))
            ProteinCollectionsListFromDB = CStr(tmptable.Rows(0).Item("Protein_Collection_List"))

            If tmptable.Rows(0).Item("Protein_Collection_List").GetType.Name = "DBNull" OrElse
             CollectionListHexHashInDB = "" OrElse
             ProteinCollectionsListFromDB <> ProteinCollectionsList OrElse
             CollectionListHexHashInDB <> CollectionListHexHash Then
                RunSP_UpdateFileArchiveEntryCollectionList(ArchivedFileEntryID, ProteinCollectionsList, SourceAuthenticationHash, CollectionListHexHash)
            End If
        End If
        m_Archived_File_Name = tmptable.Rows(0).Item("Archived_File_Path").ToString


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

    Protected Function GenerateHash(SourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New ASCIIEncoding()

        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)

        'Compute the hash value from the source
        Dim SHA1_hash() As Byte = m_SHA1Provider.ComputeHash(ByteSourceText)

        'And convert it to String format for return
        'Dim SHA1string As String = Convert.ToBase64String(SHA1_hash)
        Dim SHA1string As String = BitConverter.ToString(SHA1_hash).Replace("-", "").ToLower()

        Return SHA1string
    End Function

    Protected Function GenerateArchivePath(
     SourceFilePath As String,
     ProteinCollectionID As Integer,
     FileDate As DateTime,
     Authentication_Hash As String,
     ArchivedFileType As IArchiveOutputFiles.CollectionTypes,
     OutputSequenceType As IGetFASTAFromDMS.SequenceTypes) As String

        Dim pathString As String
        pathString = Path.Combine(m_BaseArchivePath, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType))
        pathString = Path.Combine(pathString, [Enum].GetName(GetType(IGetFASTAFromDMS.SequenceTypes), OutputSequenceType))
        pathString = Path.Combine(pathString, "ID_00000_" + Authentication_Hash + Path.GetExtension(SourceFilePath))

        Return pathString

    End Function


    Protected Function RunSP_UpdateFileArchiveEntryCollectionList(
     ArchivedFileEntryID As Integer,
     ProteinCollectionsList As String,
     CollectionListHash As String,
     CollectionListHexHash As String) As Integer

        Dim sp_Save As SqlCommand

        sp_Save = New SqlCommand("UpdateFileArchiveEntryCollectionList", m_TableGetter.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Archived_File_Entry_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ArchivedFileEntryID

        myParam = sp_Save.Parameters.Add("@ProteinCollectionList", SqlDbType.VarChar, 8000)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinCollectionsList

        myParam = sp_Save.Parameters.Add("@SHA1Hash", SqlDbType.VarChar, 28)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CollectionListHash

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 512)
        myParam.Direction = ParameterDirection.Output

        myParam = sp_Save.Parameters.Add("@CollectionListHexHash", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CollectionListHexHash

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'm_Archived_File_Name = ArchivedFileFullPath


        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddOutputFileArchiveEntry(
     ProteinCollectionID As Integer,
     CreationOptionsString As String,
     Authentication_Hash As String,
     FileModificationDate As DateTime,
     OutputFileSize As Int64,
     ProteinCount As Integer,
     ArchivedFileFullPath As String,
     ArchivedFileType As String,
     ProteinCollectionsList As String,
     CollectionListHexHash As String) As Integer



        Dim sp_Save As SqlCommand

        sp_Save = New SqlCommand("AddOutputFileArchiveEntry", m_TableGetter.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@protein_collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinCollectionID

        myParam = sp_Save.Parameters.Add("@crc32_authentication", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Authentication_Hash

        myParam = sp_Save.Parameters.Add("@file_modification_date", SqlDbType.DateTime)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = FileModificationDate

        myParam = sp_Save.Parameters.Add("@file_size", SqlDbType.BigInt)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = OutputFileSize

        myParam = sp_Save.Parameters.Add("@protein_count", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinCount

        myParam = sp_Save.Parameters.Add("@archived_file_type", SqlDbType.VarChar, 64)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ArchivedFileType

        myParam = sp_Save.Parameters.Add("@creation_options", SqlDbType.VarChar, 250)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CreationOptionsString

        myParam = sp_Save.Parameters.Add("@protein_collection_string", SqlDbType.VarChar, 8000)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinCollectionsList

        myParam = sp_Save.Parameters.Add("@collection_string_hash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CollectionListHexHash

        myParam = sp_Save.Parameters.Add("@archived_file_path", SqlDbType.VarChar, 250)
        myParam.Direction = ParameterDirection.InputOutput
        myParam.Value = ArchivedFileFullPath

        'myParam = sp_Save.Parameters.Add("@output_sequence_type", SqlDbType.VarChar, 64)
        'myParam.Direction = ParameterDirection.Input
        'myParam.Value = OutputSequenceType

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 512)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        m_Archived_File_Name = ArchivedFileFullPath

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



End Class
