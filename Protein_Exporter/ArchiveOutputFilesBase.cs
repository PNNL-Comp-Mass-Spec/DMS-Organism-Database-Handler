Option Strict On

Imports System.IO
Imports System.Text.RegularExpressions
Imports PRISMDatabaseUtils
Imports TableManipulationBase

Public MustInherit Class ArchiveOutputFilesBase
    Public Enum CollectionTypes
        [static] = 1
        dynamic = 2
    End Enum

    Protected m_Exporter As GetFASTAFromDMSForward
    Protected ReadOnly m_DatabaseAccessor As DBTask
    Protected m_LastError As String
    ' Unused: Protected m_OutputSequenceType As GetFASTAFromDMS.SequenceTypes
    Protected m_Archived_File_Name As String

    Protected Event ArchiveStart()
    Protected Event SubTaskStart(taskDescription As String)
    Protected Event SubTaskProgressUpdate(fractionDone As Double)
    Protected Event OverallProgressUpdate(fractionDone As Double)
    Protected Event ArchiveComplete(archivePath As String)

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor"></param>
    ''' <param name="exporterModule"></param>
    Sub New(databaseAccessor As DBTask, exporterModule As GetFASTAFromDMS)
        m_DatabaseAccessor = databaseAccessor

        m_Exporter = exporterModule.ExporterComponent
    End Sub

    Protected ReadOnly Property LastErrorMessage As String
        Get
            Return m_LastError
        End Get
    End Property

    Public ReadOnly Property Archived_File_Name As String
        Get
            Return m_Archived_File_Name
        End Get
    End Property

    Public Function ArchiveCollection(proteinCollectionID As Integer, archivedFileType As CollectionTypes, outputSequenceType As GetFASTAFromDMS.SequenceTypes, databaseFormatType As GetFASTAFromDMS.DatabaseFormatTypes, sourceFilePath As String, creationOptionsString As String, authentication_Hash As String, proteinCollectionList As String) As Integer

        OnArchiveStart()

        Return DispositionFile(proteinCollectionID, sourceFilePath, creationOptionsString, authentication_Hash, outputSequenceType, archivedFileType, proteinCollectionList)
    End Function

    Public Function ArchiveCollection(proteinCollectionName As String, archivedFileType As CollectionTypes, outputSequenceType As GetFASTAFromDMS.SequenceTypes, databaseFormatType As GetFASTAFromDMS.DatabaseFormatTypes, sourceFilePath As String, creationOptionsString As String, authentication_Hash As String, proteinCollectionList As String) As Integer

        Dim proteinCollectionID As Integer = GetProteinCollectionID(proteinCollectionName)

        Return ArchiveCollection(proteinCollectionID, archivedFileType, outputSequenceType, databaseFormatType, sourceFilePath, creationOptionsString, authentication_Hash, proteinCollectionList)
    End Function

    Protected MustOverride Function DispositionFile(proteinCollectionID As Integer, sourceFilePath As String, creationOptionsString As String, sourceAuthenticationHash As String, outputSequenceType As GetFASTAFromDMS.SequenceTypes, archivedFileType As ArchiveOutputFilesBase.CollectionTypes, ProteinCollectionsList As String) As Integer

    Protected Function GetProteinCount(sourceFilePath As String) As Integer
        Dim idLineRegex = New Regex("^>.+", RegexOptions.Compiled)

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
    '    dt = m_DatabaseAccessor.GetTable(SQL)

    '    If dt.Rows.Count > 0 Then
    '        m_Archived_File_Name = dt.Rows(0).Item("Archived_File_Path").ToString
    '        Return CInt(dt.Rows(0).Item("Archived_File_ID"))
    '    Else
    '        Return 0
    '    End If


    'End Function

    Public Sub AddArchiveCollectionXRef(proteinCollectionID As Integer, archivedFileID As Integer)

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

    Protected Function GetProteinCollectionID(proteinCollectionName As String) As Integer
        Return m_Exporter.FindIDByName(proteinCollectionName)
    End Function

    Protected Function RunSP_AddArchivedFileEntryXRef(proteinCollectionID As Integer, archivedFileID As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddArchivedFileEntryXRef", CommandType.StoredProcedure)

        ' Define parameters

        dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID

        dbTools.AddParameter(cmdSave, "@Archived_File_ID", SqlType.Int).Value = archivedFileID

        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 250, ParameterDirection.Output)

        Dim errorMessage As String = String.Empty

        ' Execute the sp
        Dim returnValue = dbTools.ExecuteSP(cmdSave, errorMessage)

        Return returnValue
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
