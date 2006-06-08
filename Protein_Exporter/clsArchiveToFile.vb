Public Class clsArchiveToFile
    Inherits clsArchiveOutputFilesBase

    Const BASE_ARCHIVE_PATH As String = "\\gigasax\DMS_FASTA_File_Archive\"
    'Const BASE_ARCHIVE_PATH As String = "D:\outbox\output_test\archive\"


    Sub New(ByVal PSConnectionString As String)

        MyBase.New(PSConnectionString)

    End Sub


    Protected Overrides Function DispositionFile( _
        ByVal ProteinCollectionID As Integer, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal SourceAuthenticationHash As String, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes) As Integer



        Me.CheckTableGetterStatus()

        Dim ArchivedFileEntryID As Integer = 0

        Dim archivePath As String
        Dim fi As System.IO.FileInfo = New System.IO.FileInfo(SourceFilePath)
        Dim destFI As System.IO.FileInfo
        Dim di As System.IO.DirectoryInfo


        archivePath = Me.GenerateArchivePath( _
            SourceFilePath, ProteinCollectionID, _
            fi.LastWriteTime, SourceAuthenticationHash, _
            ArchivedFileType, OutputSequenceType)

        Try
            di = New System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(archivePath))
            destFI = New System.IO.FileInfo(archivePath)
            If Not di.Exists Then
                di.Create()
            End If

            If Not destFI.Exists Then
                fi.CopyTo(archivePath)
            End If


        Catch ex As Exception
            Me.m_LastError = "File copying error: " + ex.Message
            Return 0
        End Try

        Try
            'ArchivedFileEntryID = Me.RunSP_AddOutputFileArchiveEntry( _
            '    ProteinCollectionID, CreationOptionsString, SourceSHA1Authentication, fi.LastWriteTime, fi.Length, _
            '    [Enum].GetName(GetType(Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes), _
            '    OutputSequenceType), archivePath, _
            '    [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType))
            ArchivedFileEntryID = Me.RunSP_AddOutputFileArchiveEntry( _
                ProteinCollectionID, CreationOptionsString, SourceAuthenticationHash, fi.LastWriteTime, fi.Length, _
                 archivePath, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType))




        Catch ex As Exception
            Me.m_LastError = "Stored Procedure Runner error: " + ex.Message
            Return 0
        End Try

        Return ArchivedFileEntryID


    End Function


    'Protected Function GenerateArchivePath( _
    '    ByVal SourceFilePath As String, _
    '    ByVal FileDate As DateTime, _
    '    ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
    '    ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes) As String

    '    Dim pathString As String
    '    pathString = System.IO.Path.Combine(Me.BASE_ARCHIVE_PATH, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType))
    '    pathString = System.IO.Path.Combine(pathString, [Enum].GetName(GetType(Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes), OutputSequenceType))
    '    pathString = System.IO.Path.Combine(pathString, System.IO.Path.GetFileName(SourceFilePath))

    '    Return pathString

    'End Function

    Protected Function GenerateArchivePath( _
        ByVal SourceFilePath As String, _
        ByVal ProteinCollectionID As Integer, _
        ByVal FileDate As DateTime, _
        ByVal Authentication_Hash As String, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes) As String

        Dim pathString As String
        pathString = System.IO.Path.Combine(Me.BASE_ARCHIVE_PATH, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType))
        pathString = System.IO.Path.Combine(pathString, [Enum].GetName(GetType(Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes), OutputSequenceType))
        pathString = System.IO.Path.Combine(pathString, "ID_00000_" + Authentication_Hash + System.IO.Path.GetExtension(SourceFilePath))
        'pathString = System.IO.Path.Combine(pathString, "ID_" & Format(ProteinCollectionID, "00000") + "_" + Authentication_Hash + System.IO.Path.GetExtension(SourceFilePath))

        Return pathString

    End Function



    'Protected Function RunSP_AddOutputFileArchiveEntry( _
    '    ByVal ProteinCollectionID As Integer, _
    '    ByVal CreationOptionsString As String, _
    '    ByVal SHA1AuthenticationHash As String, _
    '    ByVal FileModificationDate As DateTime, _
    '    ByVal OutputFileSize As Int64, _
    '    ByVal OutputSequenceType As String, _
    '    ByVal ArchivedFileFullPath As String, _
    '    ByVal ArchivedFileType As String) As Integer
    Protected Function RunSP_AddOutputFileArchiveEntry( _
    ByVal ProteinCollectionID As Integer, _
    ByVal CreationOptionsString As String, _
    ByVal Authentication_Hash As String, _
    ByVal FileModificationDate As DateTime, _
    ByVal OutputFileSize As Int64, _
    ByVal ArchivedFileFullPath As String, _
    ByVal ArchivedFileType As String) As Integer


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

        myParam = sp_Save.Parameters.Add("@archived_file_path", SqlDbType.VarChar, 250)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ArchivedFileFullPath

        myParam = sp_Save.Parameters.Add("@creation_options", SqlDbType.VarChar, 250)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CreationOptionsString

        myParam = sp_Save.Parameters.Add("@archived_file_type", SqlDbType.VarChar, 64)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ArchivedFileType

        'myParam = sp_Save.Parameters.Add("@output_sequence_type", SqlDbType.VarChar, 64)
        'myParam.Direction = ParameterDirection.Input
        'myParam.Value = OutputSequenceType

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 250)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



End Class
