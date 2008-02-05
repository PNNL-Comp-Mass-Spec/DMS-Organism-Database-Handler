Public Class clsGetFASTAFromDMS
    Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS

    Protected WithEvents m_Getter As clsGetFASTAFromDMSForward
    Protected m_Archiver As IArchiveOutputFiles
    Protected m_DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes
    Protected m_OutputSequenceType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes
    Protected m_CollectionType As IArchiveOutputFiles.CollectionTypes
    Protected m_FinalOutputPath As String
    Protected m_ArchivalName As String
    Protected m_CurrentFileProteinCount As Integer
    Protected m_PSConnectionString As String
    Protected m_ArchiveCollectionList As ArrayList
    Protected m_TableGetter As TableManipulationBase.IGetSQLData
    Protected m_UserID As String

    Public Sub New(ByVal ProteinStorageConnectionString As String)

        Me.m_PSConnectionString = ProteinStorageConnectionString
        Me.ClassSelector(ProteinStorageConnectionString, ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)

    End Sub

    Public ReadOnly Property ExporterComponent() As clsGetFASTAFromDMSForward ' Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.ExporterComponent
        Get
            Return Me.m_Getter
        End Get
    End Property

    Public Sub New( _
        ByVal ProteinStorageConnectionString As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal OutputSequenceType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes)

        Me.m_PSConnectionString = ProteinStorageConnectionString

        Dim user As New System.Security.Principal.WindowsPrincipal(System.Security.Principal.WindowsIdentity.GetCurrent())
        Me.m_UserID = user.Identity.Name  ' VB.NET

        Me.ClassSelector(ProteinStorageConnectionString, _
            DatabaseFormatType, OutputSequenceType)

    End Sub

    Private Sub ClassSelector( _
        ByVal ProteinStorageConnectionString As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal OutputSequenceType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes)

        Me.m_DatabaseFormatType = DatabaseFormatType
        Me.m_OutputSequenceType = OutputSequenceType

        Select Case OutputSequenceType

            Case ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward
                Me.m_Getter = New clsGetFASTAFromDMSForward( _
                    ProteinStorageConnectionString, DatabaseFormatType)
                Me.m_CollectionType = IArchiveOutputFiles.CollectionTypes.static

            Case ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.reversed
                Me.m_Getter = New clsGetFASTAFromDMSReversed( _
                    ProteinStorageConnectionString, DatabaseFormatType)
                Me.m_CollectionType = IArchiveOutputFiles.CollectionTypes.dynamic

            Case ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.scrambled
                Me.m_Getter = New clsGetFASTAFromDMSScrambled( _
                    ProteinStorageConnectionString, DatabaseFormatType)
                Me.m_CollectionType = IArchiveOutputFiles.CollectionTypes.dynamic
            Case ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.decoy
                Me.m_Getter = New clsGetFASTAFromDMSDecoy( _
                    ProteinStorageConnectionString, DatabaseFormatType)
                Me.m_CollectionType = IArchiveOutputFiles.CollectionTypes.dynamic

        End Select

        Me.m_Archiver = New clsArchiveToFile(ProteinStorageConnectionString, Me)

    End Sub

    Protected Overridable Function GetCollectionTable(ByVal selectionSQL As String) As DataTable
        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString, True)
        End If

        Return Me.m_TableGetter.GetTable(selectionSQL)

    End Function

    Overloads Function ExportFASTAFile( _
        ByVal ProteinCollectionID As Integer, _
        ByVal ExportPath As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal OutputSequenceType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes) As String _
            Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.ExportFASTAFile

        Dim proteincollectionname As String = Me.GetProteinCollectionName(ProteinCollectionID)
        Dim al As New ArrayList

        Dim creationOptionsHandler As New clsFileCreationOptions(Me.m_PSConnectionString)

        Dim creationOptions As String = creationOptionsHandler.MakeCreationOptionsString( _
            OutputSequenceType, DatabaseFormatType)

        al.Add(proteincollectionname)
        Return Me.ExportMultipleFASTAFiles(al, creationOptions, ExportPath, 0, True, DatabaseFormatType, OutputSequenceType)

    End Function

    Overloads Function ExportFASTAFile( _
        ByVal ProteinCollectionNameList As String, _
        ByVal CreationOptions As String, _
        ByVal LegacyFASTAFileName As String, _
        ByVal ExportPath As String) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.ExportFASTAFile

        Dim legacyLocationsSQL As String
        Dim foundRows() As DataRow

        Dim ProteinCollections() As String
        Dim collectionName As String

        Dim collectionList As ArrayList
        Dim legacyStaticFilelocations As DataTable
        Dim legacyStaticFilePath As String

        Dim optionsParser As clsFileCreationOptions
        optionsParser = New clsFileCreationOptions(Me.m_PSConnectionString)
        Dim cleanOptionsString As String

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If

        If ProteinCollectionNameList.Length > 0 And Not ProteinCollectionNameList.Equals("na") Then
            'Parse out protein collections from "," delimited list
            ProteinCollections = ProteinCollectionNameList.Split(","c)
            If collectionList Is Nothing Then
                collectionList = New ArrayList(ProteinCollections.Length)
            End If
            For Each collectionName In ProteinCollections
                collectionList.Add(collectionName)
            Next

            'Parse options string
            cleanOptionsString = optionsParser.ExtractOptions(CreationOptions)

            Return Me.ExportMultipleFASTAFiles( _
                collectionList, _
                cleanOptionsString, _
                ExportPath, 0, True, _
                optionsParser.FileFormatType, _
                optionsParser.SequenceDirection)

        ElseIf LegacyFASTAFileName.Length > 0 And Not LegacyFASTAFileName.Equals("na") Then

            legacyLocationsSQL = "SELECT FileName, Full_Path FROM V_Legacy_Static_File_Locations"
            legacyStaticFilelocations = Me.m_TableGetter.GetTable(legacyLocationsSQL)
            foundRows = legacyStaticFilelocations.Select("FileName = '" & LegacyFASTAFileName & "'")

            Me.m_Getter = New clsGetFASTAFromDMSForward( _
                Me.m_PSConnectionString, _
                ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta)
            RunSP_AddLegacyFileUploadRequest(LegacyFASTAFileName)
            legacyStaticFilePath = foundRows(0).Item("Full_Path").ToString
            Dim fi As New System.IO.FileInfo(legacyStaticFilePath)
            Dim copyFI As New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, LegacyFASTAFileName))
            If fi.Exists Then
                Dim sha1 As String = Me.GenerateFileAuthenticationHash(legacyStaticFilePath)
                If copyFI.Exists Then
                    copyFI.Delete()
                End If
                fi.CopyTo(System.IO.Path.Combine(ExportPath, LegacyFASTAFileName))
                Me.OnFileGenerationCompleted(System.IO.Path.Combine(ExportPath, LegacyFASTAFileName))
                Me.OnTaskCompletion(System.IO.Path.Combine(ExportPath, LegacyFASTAFileName))

                Return sha1
            End If

            'End If


        End If

        Return Nothing


    End Function

    Protected Overloads Function ExportMultipleFASTAFiles( _
        ByVal ProteinCollectionNameList As ArrayList, _
        ByVal CreationOptionsString As String, _
        ByVal ExportPath As String, _
        ByVal AlternateAnnotationTypeID As Integer, _
        ByVal PadWithPrimaryAnnotation As Boolean, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal OutputSequenceType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes) As String '_

        Dim CollectionName As String
        Dim FinalOutputPath As String

        Dim SHA1 As String
        Dim tmpID As Integer
        Dim destFI As System.IO.FileInfo
        Dim finalFI As System.IO.FileInfo

        Me.ClassSelector(Me.m_PSConnectionString, DatabaseFormatType, OutputSequenceType)

        If ProteinCollectionNameList.Count > 1 Then
            Me.m_CollectionType = IArchiveOutputFiles.CollectionTypes.dynamic
        End If


        SHA1 = Me.m_Getter.ExportFASTAFile( _
                    ProteinCollectionNameList, _
                    ExportPath, _
                    AlternateAnnotationTypeID, _
                    PadWithPrimaryAnnotation)

        Dim counter As Integer = 0
        Dim Archived_File_ID As Integer

        For Each CollectionName In ProteinCollectionNameList
            If counter = 0 Then
                Archived_File_ID = Me.m_Archiver.ArchiveCollection( _
                    CollectionName, _
                    Me.m_CollectionType, _
                    Me.m_OutputSequenceType, _
                    Me.m_DatabaseFormatType, _
                    Me.m_FinalOutputPath, _
                    CreationOptionsString, SHA1)
            Else
                tmpID = Me.GetProteinCollectionID(CollectionName)
                Me.m_Archiver.AddArchiveCollectionXRef(tmpID, Archived_File_ID)
            End If
            counter += 1
        Next

        destFI = New System.IO.FileInfo(Me.m_FinalOutputPath)

        FinalOutputPath = System.IO.Path.Combine(ExportPath, System.IO.Path.GetFileName(Me.m_Archiver.Archived_File_Name))
        finalFI = New System.IO.FileInfo(FinalOutputPath)
        If Not finalFI.Exists Then
            destFI.CopyTo(FinalOutputPath)
            destFI.Delete()
        End If

        Me.OnTaskCompletion(FinalOutputPath)
        Return SHA1



    End Function


    Event FileGenerationCompleted(ByVal FullOutputPath As String) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationCompleted
    Event FileGenerationProgress(ByVal statusMsg As String, ByVal fractionDone As Double) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationProgress
    Event FileGenerationStarted(ByVal taskMsg As String) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationStarted

    Private Sub OnFileGenerationCompleted(ByVal FullOutputPath As String) Handles m_Getter.FileGenerationCompleted
        If Me.m_ArchiveCollectionList Is Nothing Then
            Me.m_ArchiveCollectionList = New ArrayList
        End If
        Me.m_ArchiveCollectionList.Add(System.IO.Path.GetFileName(FullOutputPath))
        Me.m_FinalOutputPath = FullOutputPath
    End Sub

    Private Sub OnTaskCompletion(ByVal FinalOutputPath As String)
        RaiseEvent FileGenerationCompleted(FinalOutputPath)
    End Sub

#Region " Pass-Through Functionality "
    Private Sub OnFileGenerationStarted(ByVal taskMsg As String) Handles m_Getter.FileGenerationStarted
        RaiseEvent FileGenerationStarted(taskMsg)
    End Sub

    Private Sub OnFileGenerationProgressUpdate(ByVal statusMsg As String, ByVal fractionDone As Double) Handles m_Getter.FileGenerationProgress
        RaiseEvent FileGenerationProgress(statusMsg, fractionDone)
    End Sub

    Function GenerateFileAuthenticationHash(ByVal FullFilePath As String) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GenerateFileAuthenticationHash
        Return Me.m_Getter.GetFileHash(FullFilePath)
    End Function

    Function GetAllCollections() As System.Collections.Hashtable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetAllCollections
        Return Me.m_Getter.GetCollectionNameList
    End Function

    Function GetCollectionsByOrganism(ByVal OrganismID As Integer) As System.Collections.Hashtable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetCollectionsByOrganism
        Return Me.m_Getter.GetCollectionsByOrganism(OrganismID)
    End Function

    Function GetCollectionsByOrganismTable(ByVal OrganismID As Integer) As System.Data.DataTable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetCollectionsByOrganismTable
        Return Me.m_Getter.GetCollectionsByOrganismTable(OrganismID)
    End Function

    Function GetOrganismList() As System.Collections.Hashtable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetOrganismList
        Return Me.m_Getter.GetOrganismList
    End Function

    Function GetOrganismListTable() As System.Data.DataTable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetOrganismListTable
        Return Me.m_Getter.GetOrganismListTable
    End Function

    Overloads Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionID As Integer) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetStoredFileAuthenticationHash
        Return Me.m_Getter.GetStoredHash(ProteinCollectionID)
    End Function

    Overloads Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionName As String) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetStoredFileAuthenticationHash
        Return Me.m_Getter.GetStoredHash(ProteinCollectionName)
    End Function

    Function GetProteinCollectionID(ByVal ProteinCollectionName As String) As Integer Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetProteinCollectionID
        Return Me.m_Getter.FindIDByName(System.IO.Path.GetFileNameWithoutExtension(ProteinCollectionName))
    End Function

    Protected Function GetProteinCollectionName(ByVal ProteinCollectionID As Integer) As String
        Return Me.m_Getter.FindNameByID(ProteinCollectionID)
    End Function
#End Region


    Protected Function RunSP_AddLegacyFileUploadRequest(ByVal LegacyFilename As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddLegacyFileUploadRequest", Me.m_TableGetter.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@legacy_File_name", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = LegacyFilename

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

End Class
