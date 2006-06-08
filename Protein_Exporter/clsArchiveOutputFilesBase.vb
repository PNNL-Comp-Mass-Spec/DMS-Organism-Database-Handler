Public MustInherit Class clsArchiveOutputFilesBase
    Implements IArchiveOutputFiles

    Protected m_Exporter As Protein_Exporter.clsGetFASTAFromDMSForward
    Protected m_TableGetter As TableManipulationBase.IGetSQLData
    Protected m_PSConnectionString As String
    Protected m_LastError As String
    Protected m_OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes

    Protected Event ArchiveStart() Implements IArchiveOutputFiles.ArchiveStart
    Protected Event SubTaskStart(ByVal TaskDescription As String) Implements IArchiveOutputFiles.SubTaskStart
    Protected Event SubTaskProgressUpdate(ByVal fractionDone As Double) Implements IArchiveOutputFiles.SubTaskProgressUpdate
    Protected Event OverallProgressUpdate(ByVal fractionDone As Double) Implements IArchiveOutputFiles.OverallProgressUpdate
    Protected Event ArchiveComplete(ByVal ArchivePath As String) Implements IArchiveOutputFiles.ArchiveComplete


    Sub New(ByVal PSConnectionString As String)


        Me.m_PSConnectionString = PSConnectionString
        Me.m_TableGetter = New TableManipulationBase.clsDBTask(PSConnectionString, True)
        Me.m_Exporter = New clsGetFASTAFromDMSForward(PSConnectionString, ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta)
    End Sub

    Protected ReadOnly Property LastErrorMessage() As String Implements IArchiveOutputFiles.LastErrorMessage
        Get
            Return Me.m_LastError
        End Get
    End Property

    Protected Function ArchiveCollection( _
        ByVal ProteinCollectionID As Integer, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal DatabaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal Authentication_Hash As String) As Integer Implements IArchiveOutputFiles.ArchiveCollection

        Me.OnArchiveStart()

        Me.OnSubTaskStart("Checking for Existing Archive Entry...")

        Dim ArchivedFileEntryID As Integer
        ArchivedFileEntryID = _
            Me.CheckForExistingArchiveEntry( _
                Authentication_Hash, _
                ArchivedFileType, CreationOptionsString)

        Me.OnSubTaskProgressUpdate(1.0)

        If ArchivedFileEntryID > 0 Then
            Return ArchivedFileEntryID
        Else



            Return Me.DispositionFile( _
                ProteinCollectionID, _
                SourceFilePath, _
                CreationOptionsString, _
                Authentication_Hash, _
                OutputSequenceType, _
                ArchivedFileType)

        End If

    End Function

    Protected Function ArchiveCollection( _
        ByVal ProteinCollectionName As String, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal DatabaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal Authentication_Hash As String) As Integer Implements IArchiveOutputFiles.ArchiveCollection


        'Dim directionMatchingRegex As New System.Text.RegularExpressions.Regex("(?<collectionname>.+)(?<direction>_(forward|reversed|scrambled)).*\.(?<type>(fasta|fasta\.pro))")
        'Dim m As System.Text.RegularExpressions.Match = directionMatchingRegex.Match(ProteinCollectionName)

        'Dim ProteinCollectionID As Integer = Me.GetProteinCollectionID(m.Groups("collectionname").Value)
        Dim ProteinCollectionID As Integer = Me.GetProteinCollectionID(ProteinCollectionName)

        Return Me.ArchiveCollection( _
            ProteinCollectionID, _
            ArchivedFileType, _
            OutputSequenceType, _
            DatabaseFormatType, _
            SourceFilePath, _
            CreationOptionsString, _
            Authentication_Hash)

    End Function

    Protected MustOverride Function DispositionFile( _
        ByVal ProteinCollectionID As Integer, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal SourceAuthenticationHash As String, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes) As Integer


    Protected Sub CheckTableGetterStatus()
        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString, True)
        End If
    End Sub

    Protected Function CheckForExistingArchiveEntry( _
        ByVal Authentication_Hash As String, _
        ByVal ArchivedFileType As IArchiveOutputFiles.CollectionTypes, _
        ByVal CreationOptionsString As String) As Integer

        Dim SQL As String
        SQL = "SELECT Archived_File_ID " & _
                "FROM V_Archived_Output_Files " & _
                "WHERE Authentication_Hash = '" & Authentication_Hash & "' AND " & _
                        "Archived_File_Type = '" & _
                        [Enum].GetName(GetType(IArchiveOutputFiles.CollectionTypes), ArchivedFileType) _
                        & "' AND " & "Creation_Options = '" & CreationOptionsString & "'"

        Dim dt As DataTable
        dt = Me.m_TableGetter.GetTable(SQL)

        If dt.Rows.Count > 0 Then
            Return CInt(dt.Rows(0).Item(0))
        Else
            Return 0
        End If


    End Function

    Protected Sub AddArchiveCollectionXRef( _
        ByVal ProteinCollectionID As Integer, _
        ByVal ArchivedFileID As Integer) Implements IArchiveOutputFiles.AddArchiveCollectionXRef

        Me.RunSP_AddArchivedFileEntryXRef(ProteinCollectionID, ArchivedFileID)

    End Sub

    Protected Function GetFileAuthenticationHash(ByVal sourcePath As String) As String
        Return Me.m_Exporter.GetFileHash(sourcePath)
    End Function

    Protected Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionID As Integer) As String
        Return Me.m_Exporter.GetStoredHash(ProteinCollectionID)
    End Function

    Protected Function GetProteinCollectionID(ByVal ProteinCollectionName As String) As Integer
        Return Me.m_Exporter.FindIDByName(ProteinCollectionName)
    End Function

    Protected Function RunSP_AddArchivedFileEntryXRef( _
        ByVal ProteinCollectionID As Integer, _
        ByVal ArchivedFileID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddArchivedFileEntryXRef", Me.m_TableGetter.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinCollectionID

        myParam = sp_Save.Parameters.Add("@Archived_File_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ArchivedFileID

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 250)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



    Protected Sub OnArchiveStart()
        RaiseEvent ArchiveStart()
        Me.OnOverallProgressUpdate(0.0)
    End Sub

    Protected Sub OnSubTaskStart(ByVal SubTaskDescription As String)
        RaiseEvent SubTaskStart(SubTaskDescription)
        Me.OnSubTaskProgressUpdate(0.0)
    End Sub

    Protected Sub OnOverallProgressUpdate(ByVal fractionDone As Double)
        RaiseEvent OverallProgressUpdate(fractionDone)
    End Sub

    Protected Sub OnSubTaskProgressUpdate(ByVal fractionDone As Double)
        RaiseEvent SubTaskProgressUpdate(fractionDone)
    End Sub

    Protected Sub OnArchiveComplete(ByVal ArchivedPath As String)
        RaiseEvent ArchiveComplete(ArchivedPath)
    End Sub

End Class
