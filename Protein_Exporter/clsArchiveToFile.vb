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

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="psConnectionString"></param>
    ''' <param name="exporterModule"></param>
    Sub New(psConnectionString As String, ByRef exporterModule As clsGetFASTAFromDMS)

        MyBase.New(psConnectionString, exporterModule)

        Dim connectionStringCheck = psConnectionString.ToLower().Replace(" ", "")

        If connectionStringCheck.Contains("source=cbdms") Then
            m_BaseArchivePath = "\\cbdms\DMS_FASTA_File_Archive\"
        Else
            m_BaseArchivePath = DEFAULT_BASE_ARCHIVE_PATH
        End If

        m_SHA1Provider = New SHA1Managed()
    End Sub

    Protected Overrides Function DispositionFile(
     proteinCollectionID As Integer,
     sourceFilePath As String,
     creationOptionsString As String,
     sourceAuthenticationHash As String,
     outputSequenceType As IGetFASTAFromDMS.SequenceTypes,
     archivedFileType As IArchiveOutputFiles.CollectionTypes,
     proteinCollectionsList As String) As Integer

        Dim CollectionListHexHash As String
        Dim CollectionListHexHashInDB As String

        Dim ProteinCollectionsListFromDB As String

        CheckTableGetterStatus()

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

        Dim tmpTable As DataTable = m_TableGetter.GetTable(checkSQL)
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
              archivePath, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), archivedFileType), proteinCollectionsList, CollectionListHexHash)

            tmpTable = m_TableGetter.GetTable(checkSQL)

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
     archivedFileType As IArchiveOutputFiles.CollectionTypes,
     outputSequenceType As IGetFASTAFromDMS.SequenceTypes) As String

        Dim pathString As String
        pathString = Path.Combine(m_BaseArchivePath, [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), archivedFileType))
        pathString = Path.Combine(pathString, [Enum].GetName(GetType(IGetFASTAFromDMS.SequenceTypes), outputSequenceType))
        pathString = Path.Combine(pathString, "ID_00000_" + authentication_Hash + Path.GetExtension(sourceFilePath))

        Return pathString

    End Function

    Protected Function RunSP_UpdateFileArchiveEntryCollectionList(
     archivedFileEntryID As Integer,
     proteinCollectionsList As String,
     collectionListHash As String,
     collectionListHexHash As String) As Integer

        Dim sp_Save = New SqlCommand("UpdateFileArchiveEntryCollectionList", m_TableGetter.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        ' Define parameters
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        sp_Save.Parameters.Add("@Archived_File_Entry_ID", SqlDbType.Int).Value = archivedFileEntryID

        sp_Save.Parameters.Add("@ProteinCollectionList", SqlDbType.VarChar, 8000).Value = proteinCollectionsList

        sp_Save.Parameters.Add("@SHA1Hash", SqlDbType.VarChar, 28).Value = collectionListHash

        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 512).Direction = ParameterDirection.Output

        sp_Save.Parameters.Add("@CollectionListHexHash", SqlDbType.VarChar, 128).Value = collectionListHexHash

        ' Execute the sp
        sp_Save.ExecuteNonQuery()


        ' Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

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

        Dim sp_Save = New SqlCommand("AddOutputFileArchiveEntry", m_TableGetter.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        ' Define parameters
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        sp_Save.Parameters.Add("@protein_collection_ID", SqlDbType.Int).Value = proteinCollectionID

        sp_Save.Parameters.Add("@crc32_authentication", SqlDbType.VarChar, 40).Value = authentication_Hash

        sp_Save.Parameters.Add("@file_modification_date", SqlDbType.DateTime).Value = fileModificationDate

        sp_Save.Parameters.Add("@file_size", SqlDbType.BigInt).Value = outputFileSize

        sp_Save.Parameters.Add("@protein_count", SqlDbType.Int).Value = proteinCount

        sp_Save.Parameters.Add("@archived_file_type", SqlDbType.VarChar, 64).Value = archivedFileType

        sp_Save.Parameters.Add("@creation_options", SqlDbType.VarChar, 250).Value = creationOptionsString

        sp_Save.Parameters.Add("@protein_collection_string", SqlDbType.VarChar, 8000).Value = proteinCollectionsList

        sp_Save.Parameters.Add("@collection_string_hash", SqlDbType.VarChar, 40).Value = collectionListHexHash

        sp_Save.Parameters.Add("@archived_file_path", SqlDbType.VarChar, 250).Value = archivedFileFullPath

        ' sp_Save.Parameters.Add("@output_sequence_type", SqlDbType.VarChar, 64).Value = OutputSequenceType

        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 512).Direction = ParameterDirection.Output

        ' Execute the sp
        sp_Save.ExecuteNonQuery()

        m_Archived_File_Name = archivedFileFullPath

        ' Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



End Class
