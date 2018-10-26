Option Strict On

Imports System.Data.SqlClient
Imports System.IO
Imports System.Text.RegularExpressions
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports TableManipulationBase

Public MustInherit Class clsArchiveOutputFilesBase
    Implements IArchiveOutputFiles

    Protected m_Exporter As clsGetFASTAFromDMSForward
    Protected m_TableGetter As IGetSQLData
    Protected m_PSConnectionString As String
    Protected m_LastError As String
    ' Unused: Protected m_OutputSequenceType As IGetFASTAFromDMS.SequenceTypes
    Protected m_Archived_File_Name As String

    Protected Event ArchiveStart() Implements IArchiveOutputFiles.ArchiveStart
    Protected Event SubTaskStart(TaskDescription As String) Implements IArchiveOutputFiles.SubTaskStart
    Protected Event SubTaskProgressUpdate(fractionDone As Double) Implements IArchiveOutputFiles.SubTaskProgressUpdate
    Protected Event OverallProgressUpdate(fractionDone As Double) Implements IArchiveOutputFiles.OverallProgressUpdate
    Protected Event ArchiveComplete(ArchivePath As String) Implements IArchiveOutputFiles.ArchiveComplete


    Sub New(dbConnectionString As String, ByRef ExporterModule As clsGetFASTAFromDMS)

        m_PSConnectionString = dbConnectionString

        Dim persistConnection = Not String.IsNullOrWhiteSpace(dbConnectionString)

        m_TableGetter = New clsDBTask(dbConnectionString, persistConnection)
        m_Exporter = ExporterModule.ExporterComponent
    End Sub

    Protected ReadOnly Property LastErrorMessage() As String Implements IArchiveOutputFiles.LastErrorMessage
        Get
            Return m_LastError
        End Get
    End Property

    Protected ReadOnly Property Archived_File_Name() As String Implements IArchiveOutputFiles.Archived_File_Name
        Get
            Return m_Archived_File_Name
        End Get
    End Property


    Protected Function ArchiveCollection(
        proteinCollectionID As Integer,
        archivedFileType As IArchiveOutputFiles.CollectionTypes,
        outputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes,
        sourceFilePath As String,
        creationOptionsString As String,
        authentication_Hash As String,
        proteinCollectionList As String) As Integer Implements IArchiveOutputFiles.ArchiveCollection

        OnArchiveStart()

        Return DispositionFile(
            proteinCollectionID,
            sourceFilePath,
            creationOptionsString,
            authentication_Hash,
            outputSequenceType,
            archivedFileType,
            proteinCollectionList)

    End Function

    Protected Function ArchiveCollection(
        proteinCollectionName As String,
        archivedFileType As IArchiveOutputFiles.CollectionTypes,
        outputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes,
        sourceFilePath As String,
        creationOptionsString As String,
        authentication_Hash As String,
        proteinCollectionList As String) As Integer Implements IArchiveOutputFiles.ArchiveCollection

        Dim proteinCollectionID As Integer = GetProteinCollectionID(proteinCollectionName)

        Return ArchiveCollection(
            proteinCollectionID,
            archivedFileType,
            outputSequenceType,
            databaseFormatType,
            sourceFilePath,
            creationOptionsString,
            authentication_Hash,
            proteinCollectionList)

    End Function

    Protected MustOverride Function DispositionFile(
        proteinCollectionID As Integer,
        sourceFilePath As String,
        creationOptionsString As String,
        sourceAuthenticationHash As String,
        outputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        archivedFileType As IArchiveOutputFiles.CollectionTypes, ProteinCollectionsList As String) As Integer

    Protected Function GetProteinCount(sourceFilePath As String) As Integer
        Dim idLineRegex As Regex
        idLineRegex = New Regex("^>.+", RegexOptions.Compiled)

        Dim fi = New FileInfo(sourceFilePath)
        Dim counter = 0

        If (fi.Exists) Then
            Using fileReader = New StreamReader(New FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                While Not fileReader.EndOfStream
                    Dim dataLine = fileReader.ReadLine
                    If idLineRegex.IsMatch(dataLine) Then
                        counter += 1
                    End If
                End While
            End Using
        End If

        Return counter

    End Function

    Protected Sub CheckTableGetterStatus()
        If m_TableGetter Is Nothing Then
            m_TableGetter = New clsDBTask(m_PSConnectionString, True)
        End If
    End Sub

    ' Unused
    'Protected Function CheckForExistingArchiveEntry(
    '    authentication_Hash As String,
    '    archivedFileType As IArchiveOutputFiles.CollectionTypes,
    '    creationOptionsString As String) As Integer

    '    Dim SQL As String
    '    SQL = "SELECT Archived_File_ID,  Archived_File_Path " &
    '            "FROM V_Archived_Output_Files " &
    '            "WHERE Authentication_Hash = '" & authentication_Hash & "' AND " &
    '                    "Archived_File_Type = '" &
    '                    [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), archivedFileType) &
    '                    "' AND " & "Creation_Options = '" & creationOptionsString & "'"

    '    Dim dt As DataTable
    '    dt = m_TableGetter.GetTable(SQL)

    '    If dt.Rows.Count > 0 Then
    '        m_Archived_File_Name = dt.Rows(0).Item("Archived_File_Path").ToString
    '        Return CInt(dt.Rows(0).Item("Archived_File_ID"))
    '    Else
    '        Return 0
    '    End If


    'End Function

    Protected Sub AddArchiveCollectionXRef(
        proteinCollectionID As Integer,
        archivedFileID As Integer) Implements IArchiveOutputFiles.AddArchiveCollectionXRef

        Dim intReturn As Integer = RunSP_AddArchivedFileEntryXRef(proteinCollectionID, archivedFileID)

        If intReturn <> 0 Then
            Throw New Exception("Error calling RunSP_AddArchivedFileEntryXRef with ProteinCollectionID " & proteinCollectionID & " and ArchivedFileID " & archivedFileID & ", ReturnCode=" & intReturn)
        End If

    End Sub

    ' Unused
    'Protected Function GetFileAuthenticationHash(sourcePath As String) As String
    '    Return m_Exporter.GetFileHash(sourcePath)
    'End Function

    ' Unused
    'Protected Function GetStoredFileAuthenticationHash(ProteinCollectionID As Integer) As String
    '    Return m_Exporter.GetStoredHash(ProteinCollectionID)
    'End Function

    Protected Function GetProteinCollectionID(ProteinCollectionName As String) As Integer
        Return m_Exporter.FindIDByName(ProteinCollectionName)
    End Function

    Protected Function RunSP_AddArchivedFileEntryXRef(
        proteinCollectionID As Integer,
        archivedFileID As Integer) As Integer

        Dim sp_Save = New SqlCommand("AddArchivedFileEntryXRef", m_TableGetter.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        ' Define parameters
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int).Value = proteinCollectionID

        sp_Save.Parameters.Add("@Archived_File_ID", SqlDbType.Int).Value = archivedFileID

        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 250).Direction = ParameterDirection.Output

        ' Execute the sp
        sp_Save.ExecuteNonQuery()

        ' Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Sub OnArchiveStart()
        RaiseEvent ArchiveStart()
        OnOverallProgressUpdate(0.0)
    End Sub

    ' Unused
    'Protected Sub OnSubTaskStart(subTaskDescription As String)
    '    RaiseEvent SubTaskStart(subTaskDescription)
    '    OnSubTaskProgressUpdate(0.0)
    'End Sub

    Protected Sub OnOverallProgressUpdate(fractionDone As Double)
        RaiseEvent OverallProgressUpdate(fractionDone)
    End Sub

    ' Unused
    'Protected Sub OnSubTaskProgressUpdate(fractionDone As Double)
    '    RaiseEvent SubTaskProgressUpdate(fractionDone)
    'End Sub

    'Protected Sub OnArchiveComplete(archivedPath As String)
    '    RaiseEvent ArchiveComplete(archivedPath)
    'End Sub

End Class
