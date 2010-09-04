Option Strict On

Public Class clsArchiveToFile
    Inherits clsArchiveOutputFilesBase

    Const BASE_ARCHIVE_PATH As String = "\\gigasax\DMS_FASTA_File_Archive\"
    'Const BASE_ARCHIVE_PATH As String = "D:\outbox\output_test\archive\"
    Protected m_SHA1Provider As System.Security.Cryptography.SHA1Managed

    Sub New(ByVal PSConnectionString As String, ByRef ExporterModule As Protein_Exporter.clsGetFASTAFromDMS)


        MyBase.New(PSConnectionString, ExporterModule)
        Me.m_SHA1Provider = New System.Security.Cryptography.SHA1Managed
    End Sub

    Protected Overrides Function DispositionFile( _
        ByVal ProteinCollectionID As Integer, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal SourceAuthenticationHash As String, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
        ByVal ProteinCollectionsList As String) As Integer

        Dim CollectionListHexHash As String
        Dim CollectionListHexHashInDB As String

        Dim ProteinCollectionsListFromDB As String

        Me.CheckTableGetterStatus()

        Dim ArchivedFileEntryID As Integer = 0


        Dim archivePath As String
        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(SourceFilePath)
        Dim destFI As System.IO.FileInfo
        Dim di As System.IO.DirectoryInfo

        Dim proteinCount As Integer

        'Check for existence of Archive Entry
        Dim checkSQL As String = "SELECT Archived_File_ID, Archived_File_Path, IsNull(Protein_Collection_List, '') as Protein_Collection_List, IsNull(Collection_List_Hex_Hash, '') AS Collection_List_Hex_Hash " & _
                                 "FROM T_Archived_Output_Files " & _
                                 "WHERE Authentication_Hash = '" & SourceAuthenticationHash & "' ORDER BY File_Modification_Date DESC"

        Dim tmptable As DataTable = Me.m_TableGetter.GetTable(checkSQL)
        CollectionListHexHash = Me.GenerateHash(ProteinCollectionsList + "/" + CreationOptionsString)
        If tmptable.Rows.Count = 0 Then
            proteinCount = Me.GetProteinCount(SourceFilePath)

            archivePath = Me.GenerateArchivePath( _
                SourceFilePath, ProteinCollectionID, _
                fi.LastWriteTime, _
                SourceAuthenticationHash, _
                ArchivedFileType, OutputSequenceType)

            ArchivedFileEntryID = Me.RunSP_AddOutputFileArchiveEntry( _
                ProteinCollectionID, CreationOptionsString, SourceAuthenticationHash, fi.LastWriteTime, fi.Length, proteinCount, _
                archivePath, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType), ProteinCollectionsList, CollectionListHexHash)
            tmptable = Me.m_TableGetter.GetTable(checkSQL)

        Else
            ArchivedFileEntryID = CInt(tmptable.Rows(0).Item("Archived_File_ID"))
            CollectionListHexHashInDB = CStr(tmptable.Rows(0).Item("Collection_List_Hex_Hash"))
            ProteinCollectionsListFromDB = CStr(tmptable.Rows(0).Item("Protein_Collection_List"))

            If tmptable.Rows(0).Item("Protein_Collection_List").GetType.Name = "DBNull" OrElse _
                CollectionListHexHashInDB = "" OrElse _
                ProteinCollectionsListFromDB <> ProteinCollectionsList OrElse _
                CollectionListHexHashInDB <> CollectionListHexHash Then
                Me.RunSP_UpdateFileArchiveEntryCollectionList(ArchivedFileEntryID, ProteinCollectionsList, SourceAuthenticationHash, CollectionListHexHash)
            End If
        End If
        Me.m_Archived_File_Name = tmptable.Rows(0).Item("Archived_File_Path").ToString




        Try
            di = New System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(Me.m_Archived_File_Name))
            destFI = New System.IO.FileInfo(Me.m_Archived_File_Name)
            If Not di.Exists Then
                di.Create()
            End If

            If Not destFI.Exists Then
                fi.CopyTo(Me.m_Archived_File_Name)
            End If


        Catch ex As Exception
            Me.m_LastError = "File copying error: " + ex.Message
            Return 0
        End Try

        fi = Nothing
        destFI = Nothing

        Return ArchivedFileEntryID


    End Function

    Protected Function GenerateHash(ByVal SourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New System.Text.ASCIIEncoding
        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)
        'Compute the hash value from the source
        Dim SHA1_hash() As Byte = Me.m_SHA1Provider.ComputeHash(ByteSourceText)
        'And convert it to String format for return
        'Dim SHA1string As String = Convert.ToBase64String(SHA1_hash)
        Dim SHA1string As String = BitConverter.ToString(SHA1_hash).Replace("-", "").ToLower

        Return SHA1string
    End Function

    Protected Function GenerateArchivePath( _
        ByVal SourceFilePath As String, _
        ByVal ProteinCollectionID As Integer, _
        ByVal FileDate As DateTime, _
        ByVal Authentication_Hash As String, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes) As String

        Dim pathString As String
        pathString = System.IO.Path.Combine(clsArchiveToFile.BASE_ARCHIVE_PATH, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType))
        pathString = System.IO.Path.Combine(pathString, [Enum].GetName(GetType(Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes), OutputSequenceType))
        pathString = System.IO.Path.Combine(pathString, "ID_00000_" + Authentication_Hash + System.IO.Path.GetExtension(SourceFilePath))

        Return pathString

    End Function


    Protected Function RunSP_UpdateFileArchiveEntryCollectionList( _
        ByVal ArchivedFileEntryID As Integer, _
        ByVal ProteinCollectionsList As String, _
        ByVal CollectionListHash As String, _
        ByVal CollectionListHexHash As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateFileArchiveEntryCollectionList", Me.m_TableGetter.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

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

        'Me.m_Archived_File_Name = ArchivedFileFullPath


        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddOutputFileArchiveEntry( _
        ByVal ProteinCollectionID As Integer, _
        ByVal CreationOptionsString As String, _
        ByVal Authentication_Hash As String, _
        ByVal FileModificationDate As DateTime, _
        ByVal OutputFileSize As Int64, _
        ByVal ProteinCount As Integer, _
        ByVal ArchivedFileFullPath As String, _
        ByVal ArchivedFileType As String, _
        ByVal ProteinCollectionsList As String, _
        ByVal CollectionListHexHash As String) As Integer



        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddOutputFileArchiveEntry", Me.m_TableGetter.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

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

        Me.m_Archived_File_Name = ArchivedFileFullPath

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



End Class
