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



    End Sub

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
        End Select

        Me.m_Archiver = New clsArchiveToFile(ProteinStorageConnectionString)

    End Sub

    Protected Overridable Function GetCollectionTable(ByVal selectionSQL As String) As DataTable
        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString, True)
        End If

        Return Me.m_TableGetter.GetTable(selectionSQL)

    End Function

    Protected Overloads Function ExportFASTAFile( _
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

    Protected Overloads Function ExportFASTAFile( _
        ByVal ProteinCollectionNameList As String, _
        ByVal CreationOptions As String, _
        ByVal LegacyFASTAFileName As String, _
        ByVal ExportPath As String) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.ExportFASTAFile

        Dim legacyCollectionID As Integer
        Dim legacyCollectionName As String
        'Dim tableGetter As TableManipulationBase.IGetSQLData
        Dim collectionSQL As String
        Dim legacyLocationsSQL As String
        Dim tmpCollectionTable As DataTable
        Dim foundRows() As DataRow

        Dim ProteinCollections() As String
        Dim collectionName As String

        Dim collectionList As ArrayList
        Dim legacyStaticFilelocations As DataTable
        Dim legacyStaticFilePath As String

        Dim legacyFileType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes
        Dim legacyFileTypeRegex As System.Text.RegularExpressions.Regex
        Dim m As System.Text.RegularExpressions.Match

        Dim optionsParser As clsFileCreationOptions
        optionsParser = New clsFileCreationOptions(Me.m_PSConnectionString)
        Dim cleanOptionsString As String

        legacyFileTypeRegex = New System.Text.RegularExpressions.Regex( _
            ".+\.(?<databasetype>(fasta.pro|fasta))$")


        collectionSQL = "SELECT Protein_Collection_ID, FileName, NumProteins, NumResidues " & _
                "FROM T_Protein_Collections"

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString, True)
        End If

        tmpCollectionTable = Me.m_TableGetter.GetTable(collectionSQL)


        'check legacy filename for existing collection with that name
        If LegacyFASTAFileName.Length > 0 And Not LegacyFASTAFileName.Equals("na") Then
            'does a collection exist with this name?
            'tableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString, True)
            legacyCollectionName = System.IO.Path.GetFileNameWithoutExtension(LegacyFASTAFileName)
            If legacyFileTypeRegex.IsMatch(LegacyFASTAFileName) Then
                m = legacyFileTypeRegex.Match(LegacyFASTAFileName)
                Select Case m.Groups("databasetype").Value
                    Case "fasta"
                        legacyFileType = ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta
                    Case "fasta.pro"
                        legacyFileType = ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fastapro
                    Case Else
                        legacyFileType = ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta
                End Select
            End If

            foundRows = tmpCollectionTable.Select("FileName = '" & legacyCollectionName & "'")

            If foundRows.Length > 0 Then
                legacyCollectionID = CInt(foundRows(0).Item("Protein_Collection_ID"))
                If collectionList Is Nothing Then
                    collectionList = New ArrayList(1)
                End If
                CreationOptions = "seq_direction=forward,filetype=fasta"
                collectionList.Add(legacyCollectionName)
                Return Me.ExportMultipleFASTAFiles( _
                    collectionList, CreationOptions, _
                    ExportPath, 0, True, _
                    legacyFileType, _
                    ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)
            Else
                'TODO:routine to grab file from DMS static location
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

            End If


        Else
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


    Protected Event FileGenerationCompleted(ByVal FullOutputPath As String) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationCompleted
    Protected Event FileGenerationProgress(ByVal statusMsg As String, ByVal fractionDone As Double) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationProgress
    Protected Event FileGenerationStarted(ByVal taskMsg As String) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationStarted

    Private Sub OnFileGenerationCompleted(ByVal FullOutputPath As String) Handles m_Getter.FileGenerationCompleted
        'RaiseEvent FileGenerationCompleted(FullOutputPath)
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

    Protected Function GenerateFileAuthenticationHash(ByVal FullFilePath As String) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GenerateFileAuthenticationHash
        Return Me.m_Getter.GetFileHash(FullFilePath)
    End Function

    Protected Function GetAllCollections() As System.Collections.Hashtable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetAllCollections
        Return Me.m_Getter.GetCollectionNameList
    End Function

    Protected Function GetCollectionsByOrganism(ByVal OrganismID As Integer) As System.Collections.Hashtable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetCollectionsByOrganism
        Return Me.m_Getter.GetCollectionsByOrganism(OrganismID)
    End Function

    Protected Function GetCollectionsByOrganismTable(ByVal OrganismID As Integer) As System.Data.DataTable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetCollectionsByOrganismTable
        Return Me.m_Getter.GetCollectionsByOrganismTable(OrganismID)
    End Function

    Protected Function GetOrganismList() As System.Collections.Hashtable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetOrganismList
        Return Me.m_Getter.GetOrganismList
    End Function

    Protected Function GetOrganismListTable() As System.Data.DataTable Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetOrganismListTable
        Return Me.m_Getter.GetOrganismListTable
    End Function

    Protected Overloads Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionID As Integer) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetStoredFileAuthenticationHash
        Return Me.m_Getter.GetStoredHash(ProteinCollectionID)
    End Function

    Protected Overloads Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionName As String) As String Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetStoredFileAuthenticationHash
        Return Me.m_Getter.GetStoredHash(ProteinCollectionName)
    End Function

    Protected Function GetProteinCollectionID(ByVal ProteinCollectionName As String) As Integer Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.GetProteinCollectionID
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
