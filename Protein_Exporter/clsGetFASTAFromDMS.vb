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
    Protected m_SHA1Provider As System.Security.Cryptography.SHA1Managed
    Protected m_WaitingForLockFile As Boolean = False

    Public Sub New(ByVal ProteinStorageConnectionString As String)

        Me.m_PSConnectionString = ProteinStorageConnectionString
        Me.ClassSelector(ProteinStorageConnectionString, ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)
        Me.m_SHA1Provider = New System.Security.Cryptography.SHA1Managed
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
        Me.m_SHA1Provider = New System.Security.Cryptography.SHA1Managed
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

        ' Returns the Sha1 hash of the exported file
        ' Returns nothing or "" if an error

        Dim ProteinCollections() As String
        Dim collectionName As String

        Dim collectionList As ArrayList

        Dim optionsParser As clsFileCreationOptions
        optionsParser = New clsFileCreationOptions(Me.m_PSConnectionString)
        Dim cleanOptionsString As String

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If

        ProteinCollectionNameList = ProteinCollectionNameList.Trim(","c)
        Dim extraCommaCheckRegex As New System.Text.RegularExpressions.Regex("[,]{2,}")

        ProteinCollectionNameList = extraCommaCheckRegex.Replace(ProteinCollectionNameList, ",")

        If ProteinCollectionNameList.Length > 0 And Not ProteinCollectionNameList.ToLower.Equals("na") Then
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

        ElseIf LegacyFASTAFileName.Length > 0 And Not LegacyFASTAFileName.ToLower.Equals("na") Then

            Dim legacyStaticFilePath As String
            Dim finalFileHash As String = "XYZ"

            If Not LookupLegacyFastaFileDetails(LegacyFASTAFileName, legacyStaticFilePath, finalFileHash) Then
                ' Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
                ' An exception has probably already been thrown
                Return Nothing
            End If

            ' Instantiate m_Getter (though it doesn't actually appear to be used)
            Me.m_Getter = New clsGetFASTAFromDMSForward( _
                Me.m_PSConnectionString, _
                ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta)

            ' Look for file LegacyFASTAFileName in folder ExportPath
            ' If it exists, we can safely assume the .Fasta file is ready for use
            '
            ' Note: we could use Me.GenerateFileAuthenticationHash(finalFileFI.FullName) to generate a hash for the given file
            ' However, for large .Fasta files this will be time consuming so we will not perform this extra step every time,
            '  and we'll instead return the value stored in finalFileHash


            Dim finalFileFI As System.IO.FileInfo
            finalFileFI = New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, LegacyFASTAFileName))
            If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
                If finalFileHash.Length = 0 Then
                    finalFileHash = GenerateAndStoreLegacyFileHash(finalFileFI.FullName)
                End If

                Me.OnTaskCompletion(finalFileFI.FullName)
                Return finalFileHash
            End If


            ' The file is not present on the local computer
            ' We need to create a lock file, then copy the .fasta file locally

            If legacyStaticFilePath Is Nothing OrElse legacyStaticFilePath.Length = 0 Then
                Throw New System.Exception("Storage path for " & LegacyFASTAFileName & " is empty according to V_Legacy_Static_File_Locations; unable to continue")
            End If

            Dim FastaSourceFI As New System.IO.FileInfo(legacyStaticFilePath)
            If Not FastaSourceFI.Exists Then
                Throw New System.Exception("Legacy fasta file not found: " & legacyStaticFilePath & " (path comes from V_Legacy_Static_File_Locations)")
            Else

                Dim strCollectionListHexHash As String = Me.GenerateHash(LegacyFASTAFileName)
                Dim lockStream As System.io.FileStream
                lockStream = CreateLockStream(ExportPath, strCollectionListHexHash, LegacyFASTAFileName)

                If lockStream Is Nothing Then
                    ' Unable to create a lock stream; an exception has likely already been thrown
                    Throw New System.Exception("Unable to create lock file required to export " & LegacyFASTAFileName)
                End If

                If Not finalFileFI Is Nothing Then
                    ' Check again for the existence of the desired .Fasta file
                    finalFileFI.Refresh()
                    If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
                        ' The final file now does exist (and is non-zero in size)
                        ' The other process that made the file should have updated the database with the file hash; determine the hash now
                        If Not LookupLegacyFastaFileDetails(LegacyFASTAFileName, legacyStaticFilePath, finalFileHash) Then
                            ' Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
                            ' An exception has probably already been thrown
                            Return Nothing
                        End If

                        If finalFileHash.Length = 0 Then
                            finalFileHash = GenerateAndStoreLegacyFileHash(finalFileFI.FullName)
                        End If

                        Me.OnTaskCompletion(finalFileFI.FullName)
                        DeleteLockStream(ExportPath, strCollectionListHexHash, lockStream)
                        Return finalFileHash
                    End If

                End If

                ' Copy the .Fasta file from the remote computer to this computer
                ' We're temporarily naming it with the hash name
                Dim TargetFI As New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, strCollectionListHexHash & "_" & System.IO.Path.GetFileNameWithoutExtension(legacyStaticFilePath) & ".fasta"))
                If TargetFI.Exists Then
                    TargetFI.Delete()
                End If
                FastaSourceFI.CopyTo(TargetFI.FullName)

                ' Now that the copy is done, rename the file to the final name
                finalFileFI.Refresh()
                If finalFileFI.Exists Then
                    finalFileFI.Delete()
                End If

                TargetFI.MoveTo(finalFileFI.FullName)

                If finalFileHash.Length = 0 Then
                    ' Generate and store theh has for this file
                    finalFileHash = GenerateAndStoreLegacyFileHash(finalFileFI.FullName)
                End If

                Me.OnFileGenerationCompleted(finalFileFI.FullName)
                Me.OnTaskCompletion(finalFileFI.FullName)

                DeleteLockStream(ExportPath, strCollectionListHexHash, lockStream)
                Return finalFileHash
                End If

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

        Dim SHA1 As String
        Dim tmpID As Integer
        Dim InterimFastaFI As System.IO.FileInfo
        Dim finalFI As System.IO.FileInfo

        Dim hashableString As String
        hashableString = Join(ProteinCollectionNameList.ToArray, ",") + "/" + CreationOptionsString
        Dim strCollectionListHexHash As String = Me.GenerateHash(hashableString)

        Dim finalFileName As String
        Dim fileNameSql As String
        Dim finalFileHash As String
        Dim fileNameTable As DataTable
        Dim foundRow As DataRow
        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If
        fileNameSql = "SELECT Archived_File_Path,Archived_File_ID,Authentication_Hash FROM T_Archived_Output_Files " & _
                        "WHERE Collection_List_Hex_Hash = '" & strCollectionListHexHash & "' AND " & _
                            "Protein_Collection_List = '" & Join(ProteinCollectionNameList.ToArray, ",") & "' ORDER BY File_Modification_Date desc"
        fileNameTable = Me.m_TableGetter.GetTable(fileNameSql)
        If fileNameTable.Rows.Count = 1 Then
            foundRow = fileNameTable.Rows(0)
            finalFileName = System.IO.Path.GetFileName(CStr(foundRow.Item("Archived_File_Path")))
            finalFileHash = CStr(foundRow.Item("Authentication_Hash"))
        Else
            finalFileName = ""
            finalFileHash = ""
        End If

        Dim finalFileFI As System.IO.FileInfo

        If finalFileName.Length > 0 Then
            ' Look for file finalFileName in folder ExportPath
            ' If it exists, we can safely assume the .Fasta file is ready for use

            '
            ' Note: we could use Me.GenerateFileAuthenticationHash(finalFileFI.FullName) to generate a hash for the given file
            ' However, for large .Fasta files this will be time consuming so we will not perform this extra step

            finalFileFI = New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, finalFileName))
            If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
                Me.OnTaskCompletion(finalFileFI.FullName)
                Return finalFileHash
            End If
        End If

        ' Either finalFileName = "" or the file is not present
        ' We need to create a lock file, then export the database

        Dim lockStream As System.io.FileStream
        lockStream = CreateLockStream(ExportPath, strCollectionListHexHash, finalFileName)

        If lockStream Is Nothing Then
            ' Unable to create a lock stream; an exception has likely already been thrown
            Throw New System.Exception("Unable to create lock file required to export " & finalFileName)
        End If

        If Not finalFileFI Is Nothing Then
            ' Check again for the existence of the desired .Fasta file
            finalFileFI.Refresh()
            If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
                ' The final file now does exist (and is non-zero in size); we're good to go
                Me.OnTaskCompletion(finalFileFI.FullName)
                DeleteLockStream(ExportPath, strCollectionListHexHash, lockStream)
                Return finalFileHash
            End If
        End If

        ' Initialize the ClassSelector
        Me.ClassSelector(Me.m_PSConnectionString, DatabaseFormatType, OutputSequenceType)

        ' If more than one protein collection, then we're generating a dynamic protein collection
        If ProteinCollectionNameList.Count > 1 Then
            Me.m_CollectionType = IArchiveOutputFiles.CollectionTypes.dynamic
        End If

        ' Export the fasta file
        SHA1 = Me.m_Getter.ExportFASTAFile( _
                    ProteinCollectionNameList, _
                    ExportPath, _
                    AlternateAnnotationTypeID, _
                    PadWithPrimaryAnnotation)

        Dim counter As Integer = 0
        Dim Archived_File_ID As Integer

        If SHA1.Length = 0 Then
            Throw New Exception("m_Getter.ExportFASTAFile returned a blank string for the Sha1 authentication hash; this likely represents a problem")
            Return SHA1
        End If

        For Each CollectionName In ProteinCollectionNameList
            If counter = 0 Then
                Archived_File_ID = Me.m_Archiver.ArchiveCollection( _
                    CollectionName, _
                    Me.m_CollectionType, _
                    Me.m_OutputSequenceType, _
                    Me.m_DatabaseFormatType, _
                    Me.m_FinalOutputPath, _
                    CreationOptionsString, SHA1, Join(ProteinCollectionNameList.ToArray, ","))
            Else
                tmpID = Me.GetProteinCollectionID(CollectionName)
                Me.m_Archiver.AddArchiveCollectionXRef(tmpID, Archived_File_ID)
            End If
            counter += 1
        Next

        ' Rename the new protein collection to the correct, final name on the local computer
        InterimFastaFI = New System.IO.FileInfo(Me.m_FinalOutputPath)

        finalFileName = System.IO.Path.GetFileName(Me.m_Archiver.Archived_File_Name)
        finalFileFI = New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, finalFileName))

        If finalFileFI.Exists Then
            ' Somehow the final file has appeared in the folder (this shouldn't have happened with the lock file present)
            ' Delete it
            finalFileFI.Delete()
        End If
        InterimFastaFI.MoveTo(finalFileFI.FullName)

        DeleteLockStream(ExportPath, strCollectionListHexHash, lockStream)

        Me.OnTaskCompletion(finalFileFI.FullName)
        Return SHA1

    End Function

    Protected Function CreateLockStream(ByVal ExportPath As String, ByVal LockFileHash As String, ByVal FinalFileName As String) As System.io.FileStream

        ' Returns True if an existing file is found, False if not present
        ' If an existing file is not found, but a lock file was successfully created, then lockStream will be a valid file stream

        Dim lockFi As System.IO.FileInfo
        Dim startTime As DateTime = DateTime.Now
        Dim intAttemptCount As Integer = 0

        Dim lockStream As System.IO.FileStream

        lockFi = New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, LockFileHash + ".lock"))

        Do
            intAttemptCount += 1

            Try
                lockFi.Refresh()
                If lockFi.Exists Then
                    Me.m_WaitingForLockFile = True
                    Dim LockTimeoutTime As DateTime = lockFi.LastWriteTime.AddMinutes(60)
                    RaiseEvent FileGenerationProgress("Lockfile found; waiting until it is deleted or until " & LockTimeoutTime.ToString() & ": " & lockFi.Name, 0)

                    While lockFi.Exists AndAlso System.DateTime.Now < LockTimeoutTime
                        System.Threading.Thread.Sleep(5000)
                        lockFi.Refresh()
                        If DateTime.Now.Subtract(startTime).TotalMinutes >= 60 Then
                            Exit While
                        End If
                    End While

                    lockFi.Refresh()
                    If lockFi.Exists Then
                        RaiseEvent FileGenerationProgress("Lockfile still exists; assuming another process timed out; thus, now deleting file " & lockFi.Name, 0)
                        lockFi.Delete()
                    End If
                    'Debug.WriteLine("Lockfile gone")

                    ' Now that the lock file is gone, check whether the final files is now ready

                End If

                ' Try to create a lock file so that the calling procedure can create the required .Fasta file

                ' Try to create the lock file
                ' If another process is still using it, an exception will be thrown
                lockStream = lockFi.Create()

                ' We have successfully created a lock file, 
                ' so we should exit the Do Loop
                Exit Do

            Catch ex As Exception
                RaiseEvent FileGenerationProgress("Exception while monitoring lockfile: " & ex.Message, 0)
            End Try

            ' Something went wrong; wait for 15 seconds then try again
            System.Threading.Thread.Sleep(15000)

            If intAttemptCount >= 4 Then
                ' Something went wrong 4 times in a row (typically either creating or deleting the .Lock file)
                ' Give up trying to export
                If FinalFileName Is Nothing Then FinalFileName = "??"

                Throw New System.Exception("Unable to create lock file required to export " & FinalFileName & "; tried 4 times without success")
            End If
        Loop


        Return lockStream

    End Function

    Protected Sub DeleteLockStream(ByVal ExportPath As String, ByVal LockFileHash As String, ByVal lockStream As System.IO.FileStream)

        If Not lockStream Is Nothing Then
            lockStream.Close()
        End If

        Dim lockFi As System.IO.FileInfo
        lockFi = New System.IO.FileInfo(System.IO.Path.Combine(ExportPath, LockFileHash + ".lock"))
        If Not lockFi Is Nothing Then
            If lockFi.Exists Then
                lockFi.Delete()
            End If
        End If

    End Sub

    Protected Function GenerateAndStoreLegacyFileHash(ByVal strFastaFilePath As String) As String
        Dim FileHash As String = String.Empty

        ' The database does not have a valid Authentication_Hash values for this .Fasta file; generate one now
        FileHash = Me.GenerateFileAuthenticationHash(strFastaFilePath)

        ' Add an entry to T_Legacy_File_Upload_Requests
        ' Also store the Sha1 hash for future use
        RunSP_AddLegacyFileUploadRequest(System.IO.Path.GetFileName(strFastaFilePath), FileHash)

        Return FileHash

    End Function

    Protected Function LookupLegacyFastaFileDetails(ByVal LegacyFASTAFileName As String, _
                                                    ByRef LegacyStaticFilePathOutput As String, _
                                                    ByRef FileHashOutput As String) As Boolean

        Dim legacyLocationsSQL As String

        Dim legacyStaticFilelocations As DataTable

        ' Lookup the details for LegacyFASTAFileName in the database
        legacyLocationsSQL = "SELECT FileName, Full_Path, Authentication_Hash FROM V_Legacy_Static_File_Locations WHERE FileName = '" & LegacyFASTAFileName & "'"

        If Me.m_TableGetter Is Nothing Then
            Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
        End If
        legacyStaticFilelocations = Me.m_TableGetter.GetTable(legacyLocationsSQL)
        If legacyStaticFilelocations.Rows.Count = 0 Then
            Throw New System.Exception("Legacy fasta file " & LegacyFASTAFileName & " not found in V_Legacy_Static_File_Locations; unable to continue")
            Return False
        End If

        LegacyStaticFilePathOutput = legacyStaticFilelocations.Rows(0).Item("Full_Path").ToString
        FileHashOutput = legacyStaticFilelocations.Rows(0).Item("Authentication_Hash").ToString
        If FileHashOutput Is Nothing Then FileHashOutput = String.Empty

        Return True

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


    Protected Function RunSP_AddLegacyFileUploadRequest(ByVal LegacyFilename As String, ByVal AuthenticationHash As String) As Integer

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

        myParam = sp_Save.Parameters.Add("@AuthenticationHash", SqlDbType.VarChar, 8)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = AuthenticationHash

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

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

End Class
