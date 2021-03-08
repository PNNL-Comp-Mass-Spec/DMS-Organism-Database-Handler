Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports PRISM
Imports PRISMDatabaseUtils
Imports PRISMWin
Imports TableManipulationBase

Public Class clsGetFASTAFromDMS
    Inherits EventNotifier

    Public Enum SequenceTypes
        forward = 1
        reversed = 2
        scrambled = 3
        decoy = 4
        decoyX = 5
    End Enum

    Public Enum DatabaseFormatTypes
        fasta
        fastapro
    End Enum

    Public Const LOCK_FILE_PROGRESS_TEXT As String = "Lockfile"
    Public Const HASHCHECK_SUFFIX As String = ".hashcheck"

    Private WithEvents m_Getter As clsGetFASTAFromDMSForward
    Private m_Archiver As clsArchiveOutputFilesBase
    Private m_DatabaseFormatType As DatabaseFormatTypes
    Private m_OutputSequenceType As SequenceTypes
    Private m_CollectionType As clsArchiveOutputFilesBase.CollectionTypes
    Private m_FinalOutputPath As String

    Private m_ArchiveCollectionList As List(Of String)
    Private ReadOnly m_DatabaseAccessor As clsDBTask
    Private ReadOnly m_SHA1Provider As SHA1Managed

    Private WithEvents m_FileTools As FileTools
    Private m_LastLockQueueWaitTimeLog As DateTime

    Public Property DecoyProteinsUseXXX As Boolean = True

    Public ReadOnly Property ExporterComponent As clsGetFASTAFromDMSForward
        Get
            Return m_Getter
        End Get
    End Property

    ' Unused
    'Public ReadOnly Property WaitingForLockFile As Boolean
    '    Get
    '        Return m_WaitingForLockFile
    '    End Get
    'End Property

    ' ReSharper disable once UnusedMember.Global
    ''' <summary>
    ''' Constructor when running in offline mode
    ''' </summary>
    ''' <remarks>Useful if only calling ValidateMatchingHash</remarks>
    Public Sub New()
        Me.New(String.Empty)
    End Sub

    ''' <summary>
    '''  Constructor that takes connection string, assumes file format FASTA and forward-only sequences
    ''' </summary>
    ''' <param name="dbConnectionString">Protein sequences database connection string</param>
    Public Sub New(dbConnectionString As String)
        Me.New(dbConnectionString, DatabaseFormatTypes.fasta, SequenceTypes.forward)
    End Sub

    ''' <summary>
    ''' Constructor that takes connection string, database format type, and output sequence type
    ''' </summary>
    ''' <param name="dbConnectionString">Database connection string; empty string if offline and only planning to use ValidateMatchingHash</param>
    ''' <param name="databaseFormatType"></param>
    ''' <param name="outputSequenceType"></param>
    ''' <param name="decoyUsesXXX">When true, decoy proteins start with XXX_ instead of Reversed_</param>
    Public Sub New(dbConnectionString As String, databaseFormatType As DatabaseFormatTypes, outputSequenceType As SequenceTypes, Optional decoyUsesXXX As Boolean = True)

        m_SHA1Provider = New SHA1Managed()

        If String.IsNullOrWhiteSpace(dbConnectionString) Then
            m_DatabaseAccessor = Nothing
        Else
            m_DatabaseAccessor = New clsDBTask(dbConnectionString)
            RegisterEvents(m_DatabaseAccessor)
        End If

        ClassSelector(databaseFormatType, outputSequenceType, decoyUsesXXX)

        m_FileTools = New FileTools()
        RegisterEvents(m_FileTools)
    End Sub

    Private Sub ClassSelector(databaseFormatType As DatabaseFormatTypes, outputSequenceType As SequenceTypes, decoyUsesXXX As Boolean)

        m_DatabaseFormatType = databaseFormatType
        m_OutputSequenceType = outputSequenceType

        Select Case outputSequenceType

            Case SequenceTypes.forward
                m_Getter = New clsGetFASTAFromDMSForward(m_DatabaseAccessor, databaseFormatType)
                m_CollectionType = clsArchiveOutputFilesBase.CollectionTypes.static

            Case SequenceTypes.reversed
                m_Getter = New clsGetFASTAFromDMSReversed(m_DatabaseAccessor, databaseFormatType)
                m_CollectionType = clsArchiveOutputFilesBase.CollectionTypes.dynamic

            Case SequenceTypes.scrambled
                m_Getter = New clsGetFASTAFromDMSScrambled(m_DatabaseAccessor, databaseFormatType)
                m_CollectionType = clsArchiveOutputFilesBase.CollectionTypes.dynamic

            Case SequenceTypes.decoy
                m_Getter = New clsGetFASTAFromDMSDecoy(m_DatabaseAccessor, databaseFormatType, decoyUsesXXX)

                m_CollectionType = clsArchiveOutputFilesBase.CollectionTypes.dynamic

            Case SequenceTypes.decoyX
                m_Getter = New clsGetFASTAFromDMSDecoyX(m_DatabaseAccessor, databaseFormatType)
                m_CollectionType = clsArchiveOutputFilesBase.CollectionTypes.dynamic

        End Select

        m_Archiver = New clsArchiveToFile(m_DatabaseAccessor, Me)
    End Sub

    ' Unused
    'Private Overridable Function GetCollectionTable(selectionSQL As String) As DataTable
    '    If m_DatabaseAccessor Is Nothing Then
    '        m_DatabaseAccessor = New clsDBTask(m_PSConnectionString, True)
    '    End If

    '    Return m_DatabaseAccessor.GetTable(selectionSQL)

    'End Function

    ''' <summary>
    ''' Create the FASTA file for the given protein collection ID
    ''' </summary>
    ''' <param name="destinationFolderPath"></param>
    ''' <param name="proteinCollectionID">Protein collection ID</param>
    ''' <param name="databaseFormatType">Typically fasta for .fasta files; fastapro will create a .fasta.pro file</param>
    ''' <param name="outputSequenceType">Sequence type (forward, reverse, scrambled, decoy, or decoyX)</param>
    ''' <returns>CRC32 hash of the generated (or retrieved) file</returns>
    Overloads Function ExportFASTAFile(proteinCollectionID As Integer, destinationFolderPath As String, databaseFormatType As DatabaseFormatTypes, outputSequenceType As SequenceTypes) As String

        Dim proteinCollectionName As String = GetProteinCollectionName(proteinCollectionID)

        Dim creationOptionsHandler As New clsFileCreationOptions(m_DatabaseAccessor)

        Dim creationOptions As String = creationOptionsHandler.MakeCreationOptionsString(outputSequenceType, databaseFormatType)

        Dim protCollectionList As New List(Of String) From {
                proteinCollectionName
                }

        Return ExportProteinCollections(protCollectionList, creationOptions, destinationFolderPath, 0, True, databaseFormatType, outputSequenceType)
    End Function

    ''' <summary>
    ''' Create the FASTA file, either for the given protein collections, or for the legacy FASTA file
    ''' </summary>
    ''' <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
    ''' <param name="creationOptions">Creation options, for example: seq_direction=forward,filetype=fasta</param>
    ''' <param name="legacyFASTAFileName">Legacy FASTA file name, or empty string if exporting protein collections</param>
    ''' <param name="destinationFolderPath"></param>
    ''' <returns>CRC32 hash of the generated (or retrieved) file</returns>
    Overloads Function ExportFASTAFile(protCollectionList As String, creationOptions As String, legacyFASTAFileName As String, destinationFolderPath As String) As String

        ' Returns the CRC32 hash of the exported file
        ' Returns nothing or "" if an error

        Dim optionsParser = New clsFileCreationOptions(m_DatabaseAccessor)
        Dim cleanOptionsString As String

        ' Trim any leading or trailing commas
        protCollectionList = protCollectionList.Trim(","c)

        ' Look for cases of multiple commas in a row or spaces around a comma
        ' Replace any matches with a single comma
        Dim extraCommaCheckRegex As New Regex("[, ]{2,}")

        protCollectionList = extraCommaCheckRegex.Replace(protCollectionList, ",")

        If protCollectionList.Length > 0 And Not protCollectionList.ToLower.Equals("na") Then
            'Parse out protein collections from "," delimited list

            Dim collectionList = protCollectionList.Split(","c).ToList()

            'Parse options string
            cleanOptionsString = optionsParser.ExtractOptions(creationOptions)

            Return ExportProteinCollections(collectionList, cleanOptionsString, destinationFolderPath, 0, True, optionsParser.FileFormatType, optionsParser.SequenceDirection)

        ElseIf legacyFASTAFileName.Length > 0 And Not legacyFASTAFileName.ToLower.Equals("na") Then

            Return ExportLegacyFastaFile(legacyFASTAFileName, destinationFolderPath)

        End If

        Return Nothing
    End Function

    Private Function ExportLegacyFastaFile(legacyFASTAFileName As String, destinationFolderPath As String) As String

        Dim legacyStaticFilePath = ""
        Dim crc32Hash = ""

        Dim filenameSha1Hash As String = GenerateHash(legacyFASTAFileName)
        Dim lockFileHash As String = filenameSha1Hash

        Dim lockFi As FileInfo

        If Not LookupLegacyFastaFileDetails(legacyFASTAFileName, legacyStaticFilePath, crc32Hash) Then
            ' Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
            ' An exception has probably already been thrown
            Return Nothing
        End If

        Dim fiSourceFile = New FileInfo(legacyStaticFilePath)

        If Not fiSourceFile.Exists Then
            ' Be careful changing this message; the AnalysisResources class in the Analysis Manager
            ' looks for error messages that start with "Legacy fasta file not found:"
            Dim msg = "Legacy fasta file not found: " & legacyStaticFilePath & " (path comes from V_Legacy_Static_File_Locations)"
            OnErrorEvent(msg)
            Throw New Exception(msg)
        End If

        ' Look for file LegacyFASTAFileName in folder destinationFolderPath
        ' If it exists, and if a .lock file does not exist, then compare file sizes and file modification dates

        Dim fiFinalFile As FileInfo
        fiFinalFile = New FileInfo(Path.Combine(destinationFolderPath, legacyFASTAFileName))
        If fiFinalFile.Exists AndAlso fiFinalFile.Length > 0 Then
            ' Make sure a .lock file doesn't exist
            ' If it does exist, then another process on this computer is likely creating the .Fasta file

            lockFi = New FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"))

            If lockFi.Exists Then
                ' Another program is creating a .Fasta file; cannot assume it is ready-for-use
            Else
                ' Make sure the file sizes match and that the local file is not older than the source file
                If fiSourceFile.Length = fiFinalFile.Length AndAlso fiFinalFile.LastWriteTimeUtc >= fiSourceFile.LastWriteTimeUtc.AddSeconds(-0.1) Then
                    If ExportLegacyFastaValidateHash(fiFinalFile, crc32Hash, False) Then
                        OnTaskCompletion(fiFinalFile.FullName)
                        Return crc32Hash
                    End If
                End If
            End If
        End If


        ' The file is not present on the local computer (or the file size is different or it is older than the parent fasta file)
        ' We need to create a lock file, then copy the .fasta file locally

        If String.IsNullOrEmpty(legacyStaticFilePath) Then
            Dim msg = "Storage path for " & legacyFASTAFileName & " is empty according to V_Legacy_Static_File_Locations; unable to continue"
            OnErrorEvent(msg)
            Throw New Exception(msg)
        End If

        ' Make sure we have enough disk free space

        Dim destinationPath = Path.Combine(destinationFolderPath, "TargetFile.tmp")
        Dim errorMessage As String = String.Empty
        Dim sourceFileSizeMB As Double = fiSourceFile.Length / 1024.0 / 1024.0

        Dim currentFreeSpaceBytes As Int64

        Dim success = DiskInfo.GetDiskFreeSpace(destinationPath, currentFreeSpaceBytes, errorMessage)
        If Not success Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "DiskInfo.GetDiskFreeSpace returned a blank error message"
            Dim spaceValidationError = "Unable to copy legacy FASTA file to " & destinationFolderPath & ". " & errorMessage
            OnErrorEvent(spaceValidationError)
            Throw New IOException(spaceValidationError)
        End If

        If Not FileTools.ValidateFreeDiskSpace(destinationPath, sourceFileSizeMB, currentFreeSpaceBytes, errorMessage) Then
            If String.IsNullOrEmpty(errorMessage) Then errorMessage = "FileTools.ValidateFreeDiskSpace returned a blank error message"
            Dim spaceValidationError = "Unable to copy legacy FASTA file to " & destinationFolderPath & ". " & errorMessage
            OnErrorEvent(spaceValidationError)
            Throw New IOException(spaceValidationError)
        End If

        ' If we get here, then finalFileName = "" or the file is not present or the LockFile is present
        ' Try to create a lock file, then either wait for an existing lock file to go away or export the database
        Dim lockStream As FileStream
        lockStream = CreateLockStream(destinationFolderPath, lockFileHash, legacyFASTAFileName)

        If lockStream Is Nothing Then
            ' Unable to create a lock stream; an exception has likely already been thrown
            Dim msg = "Unable to create lock file required to export " & legacyFASTAFileName
            OnErrorEvent(msg)
            Throw New Exception(msg)
        End If

        If Not fiFinalFile Is Nothing Then

            ' Check again for the existence of the desired .Fasta file
            ' It's possible another process created .Fasta file while this process was waiting for the other process's lock file to disappear
            fiFinalFile.Refresh()
            If fiFinalFile.Exists AndAlso fiSourceFile.Length = fiFinalFile.Length AndAlso fiFinalFile.LastWriteTimeUtc >= fiSourceFile.LastWriteTimeUtc.AddSeconds(-0.1) Then
                ' The final file now does exist (and has the correct size / date)
                ' The other process that made the file should have updated the database with the file hash; determine the hash now
                If Not LookupLegacyFastaFileDetails(legacyFASTAFileName, legacyStaticFilePath, crc32Hash) Then
                    ' Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
                    ' An exception has probably already been thrown
                    Return Nothing
                End If

                If ExportLegacyFastaValidateHash(fiFinalFile, crc32Hash, False) Then
                    DeleteLockStream(destinationFolderPath, lockFileHash, lockStream)
                    OnTaskCompletion(fiFinalFile.FullName)
                    Return crc32Hash
                End If

            End If

        End If

        ' Copy the .Fasta file from the remote computer to this computer
        ' We're temporarily naming it with a SHA1 hash based on the filename
        Dim InterimFastaFI As New FileInfo(Path.Combine(destinationFolderPath, filenameSha1Hash & "_" & Path.GetFileNameWithoutExtension(legacyStaticFilePath) & ".fasta"))
        If InterimFastaFI.Exists Then
            InterimFastaFI.Delete()
        End If

        m_LastLockQueueWaitTimeLog = DateTime.UtcNow
        m_FileTools.CopyFileUsingLocks(fiSourceFile, InterimFastaFI.FullName, "OrgDBHandler", overWrite:=False)

        ' Now that the copy is done, rename the file to the final name
        fiFinalFile.Refresh()
        If fiFinalFile.Exists Then
            ' Somehow the final file has appeared in the folder; it could be a corrupt version of the .fasta file
            ' Delete it
            fiFinalFile.Delete()
        End If

        InterimFastaFI.MoveTo(fiFinalFile.FullName)

        ' File successfully copied to this computer
        ' Update the hash validation file, and update the DB if the newly copied file's hash value differs from the DB
        If ExportLegacyFastaValidateHash(fiFinalFile, crc32Hash, True) Then
            DeleteLockStream(destinationFolderPath, lockFileHash, lockStream)
            OnTaskCompletion(fiFinalFile.FullName)
            Return crc32Hash
        End If

        ' This code will only get reached if an error occurred in ExportLegacyFastaValidateHash()
        ' We'll go ahead and return the hash anyway
        DeleteLockStream(destinationFolderPath, lockFileHash, lockStream)
        OnFileGenerationCompleted(fiFinalFile.FullName)
        OnTaskCompletion(fiFinalFile.FullName)

        Return crc32Hash
    End Function

    Private Function ExportLegacyFastaValidateHash(finalFileFI As FileSystemInfo, ByRef finalFileHash As String, forceRegenerateHash As Boolean) As Boolean

        If String.IsNullOrEmpty(finalFileHash) Then
            finalFileHash = GenerateAndStoreLegacyFileHash(finalFileFI.FullName)

            ' Update the hash validation file
            UpdateHashValidationFile(finalFileFI.FullName, finalFileHash)

            Return True
        Else
            ' ValidateMatchingHash will use GenerateFileAuthenticationHash() to generate a hash for the given file
            ' Since this can be time consuming, we only do this every 48 hours
            ' If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
            If ValidateMatchingHash(finalFileFI.FullName, finalFileHash, 48, forceRegenerateHash) Then
                Return True
            End If
        End If

        Return False
    End Function

    Private Function ExportProteinCollections(protCollectionList As List(Of String), creationOptionsString As String, destinationFolderPath As String, alternateAnnotationTypeID As Integer, padWithPrimaryAnnotation As Boolean, databaseFormatType As DatabaseFormatTypes, outputSequenceType As SequenceTypes) As String

        Dim proteinCollectionList = Join(protCollectionList.ToArray, ",")

        Dim stringToHash As String = proteinCollectionList + "/" + creationOptionsString
        Dim filenameSha1Hash As String = GenerateHash(stringToHash)
        Dim lockFileHash As String = filenameSha1Hash

        Dim finalFileName As String
        Dim finalFileHash As String

        Dim fileNameSql = "SELECT Archived_File_Path, Archived_File_ID, Authentication_Hash " &
                          "FROM T_Archived_Output_Files " &
                          "WHERE Collection_List_Hex_Hash = '" & filenameSha1Hash & "' AND " &
                                 "Protein_Collection_List = '" & proteinCollectionList & "' AND " &
                                 "Archived_File_State_ID <> 3 " &
                          "ORDER BY File_Modification_Date desc"

        Dim fileNameTable = m_DatabaseAccessor.GetTable(fileNameSql)
        If fileNameTable.Rows.Count >= 1 Then
            Dim foundRow = fileNameTable.Rows(0)
            finalFileName = Path.GetFileName(CStr(foundRow.Item("Archived_File_Path")))
            finalFileHash = CStr(foundRow.Item("Authentication_Hash"))
        Else
            finalFileName = String.Empty
            finalFileHash = String.Empty
        End If

        Dim finalFileFI As FileInfo = Nothing

        If finalFileName.Length > 0 Then
            ' Look for file finalFileName in folder destinationFolderPath
            ' If it exists, and if a .lock file does not exist, then we can safely assume the .Fasta file is ready for use

            finalFileFI = New FileInfo(Path.Combine(destinationFolderPath, finalFileName))
            If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
                ' Make sure a .lock file doesn't exist
                ' If it does exist, then another process on this computer is likely creating the .Fasta file

                Dim lockFile = New FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"))

                If lockFile.Exists Then
                    ' Another program is creating a .Fasta file; cannot assume it is ready-for-use
                Else
                    ' ValidateMatchingHash will use GenerateFileAuthenticationHash() to generate a hash for the given file
                    ' Since this can be time consuming, we only do this every 48 hours
                    ' If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
                    If ValidateMatchingHash(finalFileFI.FullName, finalFileHash, 48, False) Then
                        OnTaskCompletion(finalFileFI.FullName)
                        Return finalFileHash
                    End If
                End If
            End If
        End If

        ' If we get here, then finalFileName = "" or the file is not present or the LockFile is present or the hash file is out-of-date
        ' Try to create a lock file, then either wait for an existing lock file to go away or export the database
        Dim lockStream As FileStream
        lockStream = CreateLockStream(destinationFolderPath, lockFileHash, "Protein collection list " & proteinCollectionList)

        If lockStream Is Nothing Then
            ' Unable to create a lock stream; an exception has likely already been thrown
            Dim msg = "Unable to create lock file required to export " & finalFileName
            OnErrorEvent(msg)
            Throw New Exception(msg)
        End If

        If Not finalFileFI Is Nothing Then

            ' Check again for the existence of the desired .Fasta file
            ' It's possible another process created the .Fasta file while this process was waiting for the other process's lock file to disappear
            finalFileFI.Refresh()
            If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
                ' The final file now does exist (and is non-zero in size); we're good to go

                If String.IsNullOrEmpty(finalFileHash) Then
                    ' FinalFileHash is empty, which means the other process that just exported this file was the first process to actually use this file
                    ' We need to return a non-empty hash value, so compute the SHA1 hash now
                    finalFileHash = GenerateFileAuthenticationHash(finalFileFI.FullName)
                End If

                ' ValidateMatchingHash will use GenerateFileAuthenticationHash() to generate a hash for the given file
                ' Since this can be time consuming, we only do this every 48 hours
                ' If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
                If ValidateMatchingHash(finalFileFI.FullName, finalFileHash, 48, False) Then
                    DeleteLockStream(destinationFolderPath, lockFileHash, lockStream)
                    OnTaskCompletion(finalFileFI.FullName)
                    Return finalFileHash
                End If

            End If
        End If

        ' We're finally ready to generate the .Fasta file

        ' Initialize the ClassSelector
        ClassSelector(databaseFormatType, outputSequenceType, DecoyProteinsUseXXX)

        ' If more than one protein collection, then we're generating a dynamic protein collection
        If protCollectionList.Count > 1 Then
            m_CollectionType = clsArchiveOutputFilesBase.CollectionTypes.dynamic
        End If

        Dim crc32Hash As String

        Try
            OnDebugEvent("Retrieving fasta file for protein collections " & String.Join(","c, protCollectionList.ToArray()))

            ' Export the fasta file
            crc32Hash = m_Getter.ExportFASTAFile(protCollectionList, destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation)

            If String.IsNullOrEmpty(crc32Hash) Then
                Dim msg = "m_Getter.ExportFASTAFile returned a blank string for the CRC32 authentication hash; this likely represents a problem"
                OnErrorEvent(msg)
                Throw New Exception(msg)
            End If

            Dim firstCollectionProcessed = False
            Dim archivedFileId As Integer

            For Each collectionName In protCollectionList
                If Not firstCollectionProcessed Then
                    archivedFileId = m_Archiver.ArchiveCollection(
                        collectionName, m_CollectionType, m_OutputSequenceType,
                        m_DatabaseFormatType, m_FinalOutputPath,
                        creationOptionsString, crc32Hash, proteinCollectionList)

                    If archivedFileId = 0 Then
                        ' Error making an entry in T_Archived_Output_Files; abort
                        Dim msg = "Error archiving collection; Archived_File_ID = 0"
                        OnErrorEvent(msg)
                        Throw New Exception(msg)
                    End If

                    firstCollectionProcessed = True
                Else
                    Dim existingCollectionID = GetProteinCollectionID(collectionName)
                    m_Archiver.AddArchiveCollectionXRef(existingCollectionID, archivedFileId)
                End If
            Next

            ' Rename the new protein collection to the correct, final name on the local computer
            ' E.g. rename from 38FFACAC.fasta to ID_001874_38FFACAC.fasta
            Dim interimFastaFile = New FileInfo(m_FinalOutputPath)

            finalFileName = Path.GetFileName(m_Archiver.Archived_File_Name)
            finalFileFI = New FileInfo(Path.Combine(destinationFolderPath, finalFileName))

            If finalFileFI.Exists Then
                ' Somehow the final file has appeared in the folder (this shouldn't have happened with the lock file present)
                ' Delete it
                finalFileFI.Delete()
            End If

            ' Delete any other files that exist with the same extension as finalFileFI.FullName
            ' These are likely index files used by Inspect or MSGF+ (aka MSGFDB) and they will need to be re-generated
            DeleteFASTAIndexFiles(finalFileFI)

            interimFastaFile.MoveTo(finalFileFI.FullName)

            OnStatusEvent("Created fasta file " + finalFileFI.FullName)

            ' Update the hash validation file
            UpdateHashValidationFile(finalFileFI.FullName, crc32Hash)

        Catch
            DeleteLockStream(destinationFolderPath, lockFileHash, lockStream)
            Throw
        End Try

        DeleteLockStream(destinationFolderPath, lockFileHash, lockStream)

        OnTaskCompletion(finalFileFI.FullName)
        Return crc32Hash
    End Function

    Private Function CreateLockStream(destinationFolderPath As String, lockFileHash As String, proteinCollectionListOrLegacyFastaFileName As String) As FileStream

        ' Creates a new lock file
        ' If an existing file is not found, but a lock file was successfully created, then lockStream will be a valid file stream

        Dim startTime As DateTime = DateTime.UtcNow
        Dim intAttemptCount = 0

        Dim lockFile = New FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"))

        Do
            intAttemptCount += 1

            Try
                lockFile.Refresh()
                If lockFile.Exists Then
                    Dim LockTimeoutTime As DateTime = lockFile.LastWriteTimeUtc.AddMinutes(60)
                    Dim msg = LOCK_FILE_PROGRESS_TEXT & " found; waiting until it is deleted or until " & LockTimeoutTime.ToLocalTime().ToString() & ": " & lockFile.Name
                    OnDebugEvent(msg)
                    OnFileGenerationProgressUpdate(msg, 0)

                    While lockFile.Exists AndAlso DateTime.UtcNow < LockTimeoutTime
                        Thread.Sleep(5000)
                        lockFile.Refresh()
                        If DateTime.UtcNow.Subtract(startTime).TotalMinutes >= 60 Then
                            Exit While
                        End If
                    End While

                    lockFile.Refresh()
                    If lockFile.Exists Then
                        Dim warningMsg = LOCK_FILE_PROGRESS_TEXT & " still exists; assuming another process timed out; thus, now deleting file " & lockFile.Name
                        OnWarningEvent(warningMsg)
                        OnFileGenerationProgressUpdate(warningMsg, 0)
                        lockFile.Delete()
                    End If

                End If

                ' Try to create a lock file so that the calling procedure can create the required .Fasta file (or validate that it now exists)

                ' Try to create the lock file
                ' If another process is still using it, an exception will be thrown
                Dim lockStream = New FileStream(lockFile.FullName, FileMode.CreateNew, FileAccess.Write, FileShare.Read)

                Return lockStream

            Catch ex As Exception
                Dim msg = "Exception while monitoring " & LOCK_FILE_PROGRESS_TEXT & " " & lockFile.FullName & ": " & ex.Message
                OnErrorEvent(msg)
                OnFileGenerationProgressUpdate(msg, 0)
            End Try

            ' Something went wrong; wait for 15 seconds then try again
            Thread.Sleep(15000)

            If intAttemptCount >= 4 Then
                ' Something went wrong 4 times in a row (typically either creating or deleting the .Lock file)
                ' Give up trying to export
                If proteinCollectionListOrLegacyFastaFileName Is Nothing Then
                    proteinCollectionListOrLegacyFastaFileName = "??"
                End If

                ' Exception: Unable to create Lockfile required to export Protein collection ...
                Dim msg = "Unable to create " & LOCK_FILE_PROGRESS_TEXT & " required to export " & proteinCollectionListOrLegacyFastaFileName & "; tried 4 times without success"
                OnErrorEvent(msg)
                Throw New Exception(msg)
            End If
        Loop
    End Function

    Private Sub DeleteFASTAIndexFiles(fiFinalFastaFile As FileInfo)

        Try
            Dim strBaseName As String
            strBaseName = Path.GetFileNameWithoutExtension(fiFinalFastaFile.Name)

            ' Delete files with the same name but different extensions
            ' For example, Inspect's PrepDB.py script creates these files:
            ' ID_002750_1363538A.index
            ' ID_002750_1363538A_shuffle.index
            ' ID_002750_1363538A.trie
            ' ID_002750_1363538A_shuffle.trie
            ' ID_002750_1363538A_shuffle_Log.txt

            ' ReSharper disable CommentTypo

            ' MSGFDB's BuildSA function creates these files:
            ' ID_002614_23305E80.revConcat.fasta
            ' ID_002614_23305E80.fasta.23305E80.hashcheck
            ' ID_002614_23305E80_sarray.lock
            ' ID_002614_23305E80.revConcat.sarray
            ' ID_002614_23305E80.sarray
            ' ID_002614_23305E80.revConcat.seq
            ' ID_002614_23305E80.seq
            ' ID_002614_23305E80.revConcat.seqanno
            ' ID_002614_23305E80.seqanno

            ' ReSharper restore CommentTypo

            ' This code will also delete the .hashcheck file; that's OK
            ' e.g., ID_002750_1363538A.fasta.1363538A.hashcheck

            For Each fiFileToDelete As FileInfo In fiFinalFastaFile.Directory.GetFileSystemInfos(strBaseName & ".*")
                DeleteFastaIndexFile(fiFileToDelete.FullName)
            Next

            For Each fiFileToDelete As FileInfo In fiFinalFastaFile.Directory.GetFileSystemInfos(strBaseName & "_shuffle*.*")
                DeleteFastaIndexFile(fiFileToDelete.FullName)
            Next
        Catch ex As Exception
            ' Ignore errors here
        End Try
    End Sub

    Private Sub DeleteFastaIndexFile(strFilePath As String)
        Try
            File.Delete(strFilePath)
        Catch ex As Exception
            OnErrorEvent("Error deleting file: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub DeleteLockStream(destinationFolderPath As String, lockFileHash As String, lockStream As Stream)

        If Not lockStream Is Nothing Then
            lockStream.Close()
        End If

        Dim lockFi = New FileInfo(Path.Combine(destinationFolderPath, lockFileHash + ".lock"))
        If Not lockFi Is Nothing Then
            If lockFi.Exists Then
                lockFi.Delete()
            End If
        End If
    End Sub

    Private Function GenerateAndStoreLegacyFileHash(fastaFilePath As String) As String

        ' The database does not have a valid Authentication_Hash values for this .Fasta file; generate one now
        Dim crc32Hash = GenerateFileAuthenticationHash(fastaFilePath)

        ' Add an entry to T_Legacy_File_Upload_Requests
        ' Also store the CRC32 hash for future use
        RunSP_AddLegacyFileUploadRequest(Path.GetFileName(fastaFilePath), crc32Hash)

        Return crc32Hash
    End Function

    Private Function LookupLegacyFastaFileDetails(legacyFASTAFileName As String, <Out> ByRef legacyStaticFilePathOutput As String, <Out> ByRef crc32HashOutput As String) As Boolean

        ' Lookup the details for LegacyFASTAFileName in the database
        Dim legacyLocationsSQL = "SELECT FileName, Full_Path, Authentication_Hash FROM V_Legacy_Static_File_Locations WHERE FileName = '" & legacyFASTAFileName & "'"

        Dim legacyStaticFileLocations = m_DatabaseAccessor.GetTable(legacyLocationsSQL)
        If legacyStaticFileLocations.Rows.Count = 0 Then
            Dim msg = "Legacy fasta file " & legacyFASTAFileName & " not found in V_Legacy_Static_File_Locations; unable to continue"
            OnErrorEvent(msg)
            Throw New Exception(msg)
        End If

        legacyStaticFilePathOutput = legacyStaticFileLocations.Rows(0).Item("Full_Path").ToString
        crc32HashOutput = legacyStaticFileLocations.Rows(0).Item("Authentication_Hash").ToString
        If crc32HashOutput Is Nothing Then crc32HashOutput = String.Empty

        Return True
    End Function

    ''' <summary>
    ''' Construct the hashcheck file path, given the FASTA file path and its CRC32 hash
    ''' </summary>
    ''' <param name="strFastaFilePath"></param>
    ''' <param name="crc32Hash"></param>
    ''' <param name="hashcheckExtension">Hashcheck file extension; if an empty string, the default of .hashcheck is used</param>
    ''' <returns>FileInfo object for the .hashcheck file</returns>
    ''' <remarks>
    ''' Example .hashcheck filenames:
    '''   ID_004137_23AA5A07.fasta.23AA5A07.hashcheck
    '''   H_sapiens_Ensembl_v68_2013-01-08.fasta.DF687525.hashcheck
    ''' </remarks>
    Private Function GetHashFileValidationInfo(strFastaFilePath As String, crc32Hash As String, Optional hashcheckExtension As String = "") As FileInfo

        Dim extensionToUse As String
        If String.IsNullOrWhiteSpace(hashcheckExtension) Then
            extensionToUse = HASHCHECK_SUFFIX
        Else
            extensionToUse = hashcheckExtension
        End If

        Dim fastaFile = New FileInfo(strFastaFilePath)
        Dim hashValidationFileName = Path.Combine(fastaFile.DirectoryName, fastaFile.Name & "." & crc32Hash & extensionToUse)

        Return New FileInfo(hashValidationFileName)
    End Function

    ''' <summary>
    ''' Update the hashcheck file
    ''' </summary>
    ''' <param name="strFastaFilePath"></param>
    ''' <param name="crc32Hash"></param>
    ''' <param name="hashcheckExtension">Hashcheck file extension; if an empty string, the default of .hashcheck is used</param>
    Private Sub UpdateHashValidationFile(strFastaFilePath As String, crc32Hash As String, Optional hashcheckExtension As String = "")

        Dim fiHashValidationFile = GetHashFileValidationInfo(strFastaFilePath, crc32Hash, hashcheckExtension)

        Using swOutFile = New StreamWriter(New FileStream(fiHashValidationFile.FullName, FileMode.Create, FileAccess.Write, FileShare.Read))
            swOutFile.WriteLine("Hash validated " & DateTime.Now.ToString())
            swOutFile.WriteLine("Validated on " + Environment.MachineName)
        End Using
    End Sub


    ''' <summary>
    ''' Validates that the hash of a .fasta file matches the expected hash value
    ''' If the actual hash differs and if forceRegenerateHash=True, then this strExpectedHash get updated
    ''' forceRegenerateHash should be set to True only when processing legacy fasta files that have been newly copied to this computer
    ''' </summary>
    ''' <param name="fastaFilePath">Fasta file to check</param>
    ''' <param name="expectedHash">Expected CRC32 hash; updated if incorrect and forceRegenerateHash is true</param>
    ''' <param name="retryHoldoffHours">Time between re-generating the hash value for an existing file</param>
    ''' <param name="forceRegenerateHash">Re-generate the hash</param>
    ''' <param name="hashcheckExtension">Hashcheck file extension; if an empty string, the default of .hashcheck is used</param>
    ''' <returns>True if the hash values match, or if forceRegenerateHash=True</returns>
    ''' <remarks>Public method because the Analysis Manager uses this class when running offline jobs</remarks>
    Public Function ValidateMatchingHash(fastaFilePath As String, ByRef expectedHash As String, Optional retryHoldoffHours As Integer = 48, Optional forceRegenerateHash As Boolean = False, Optional hashcheckExtension As String = "") As Boolean


        Try
            Dim fastaFile = New FileInfo(fastaFilePath)

            If fastaFile.Exists Then
                Dim hashValidationFile = GetHashFileValidationInfo(fastaFilePath, expectedHash, hashcheckExtension)

                If hashValidationFile.Exists And Not forceRegenerateHash Then
                    If DateTime.UtcNow.Subtract(hashValidationFile.LastWriteTimeUtc).TotalHours <= retryHoldoffHours Then
                        OnDebugEvent("Validated hash validation file (recently verified): " + hashValidationFile.FullName)
                        ' Hash check file exists, and the file is less than 48 hours old
                        Return True
                    End If
                End If

                ' Either the hash validation file doesn't exist, or it's too old, or forceRegenerateHash = True
                ' Regenerate the hash
                Dim crc32Hash = GenerateFileAuthenticationHash(fastaFile.FullName)

                If String.Equals(expectedHash, crc32Hash) OrElse forceRegenerateHash Then
                    ' Update the hash validation file
                    UpdateHashValidationFile(fastaFilePath, crc32Hash, hashcheckExtension)

                    If expectedHash <> crc32Hash And forceRegenerateHash Then
                        ' Hash values don't match, but forceRegenerateHash=True
                        ' Update the hash value stored in T_Legacy_File_Upload_Requests for this fasta file
                        RunSP_AddLegacyFileUploadRequest(fastaFile.Name, crc32Hash)

                        ' Update expectedHash
                        expectedHash = crc32Hash

                        OnStatusEvent("Re-exported protein collection and created new hash file due to CRC32 hash mismatch: " + hashValidationFile.FullName)
                    Else
                        OnDebugEvent("Validated hash validation file (re-verified): " + hashValidationFile.FullName)
                    End If

                    Return True
                End If
            End If
        Catch ex As Exception
            Dim msg = "Exception while re-computing the hash of the fasta file: " & ex.Message
            OnErrorEvent(msg, ex)
            OnFileGenerationProgressUpdate(msg, 0)
        End Try

        Return False
    End Function

#Region "Events and Event Handlers"

    Public Event FileGenerationCompleted(outputPath As String)
    Public Event FileGenerationProgress(statusMsg As String, fractionDone As Double)
    Public Event FileGenerationStarted(taskMsg As String)

    Private Sub OnFileGenerationCompleted(outputPath As String) Handles m_Getter.FileGenerationCompleted
        If m_ArchiveCollectionList Is Nothing Then
            m_ArchiveCollectionList = New List(Of String)
        End If
        m_ArchiveCollectionList.Add(Path.GetFileName(outputPath))
        m_FinalOutputPath = outputPath
        OnDebugEvent("Saved fasta file to " + outputPath)
    End Sub

    ''' <summary>
    ''' Raises event FileGenerationCompleted is raised once the fasta file is done being created
    ''' </summary>
    ''' <param name="finalOutputPath"></param>
    ''' <remarks></remarks>
    Private Sub OnTaskCompletion(finalOutputPath As String)
        RaiseEvent FileGenerationCompleted(finalOutputPath)
    End Sub

    Private Sub m_FileTools_WaitingForLockQueue(sourceFilePath As String, targetFilePath As String, sourceBacklogMB As Integer, targetBacklogMB As Integer) Handles m_FileTools.WaitingForLockQueue
        Dim strServers As String

        If DateTime.UtcNow.Subtract(m_LastLockQueueWaitTimeLog).TotalSeconds >= 30 Then
            m_LastLockQueueWaitTimeLog = DateTime.UtcNow
            Console.WriteLine("Waiting for lockfile queue to fall below threshold to fall below threshold (Protein_Exporter); " + "SourceBacklog=" & sourceBacklogMB & " MB, " + "TargetBacklog=" & targetBacklogMB & " MB, " + "Source=" & sourceFilePath & ", " + "Target=" & targetFilePath)

            If sourceBacklogMB > 0 AndAlso targetBacklogMB > 0 Then
                strServers = m_FileTools.GetServerShareBase(sourceFilePath) & " and " & m_FileTools.GetServerShareBase(targetFilePath)
            ElseIf targetBacklogMB > 0 Then
                strServers = m_FileTools.GetServerShareBase(targetFilePath)
            Else
                strServers = m_FileTools.GetServerShareBase(sourceFilePath)
            End If

            Dim msg = "Waiting for lockfile queue on " & strServers & " to fall below threshold"
            OnDebugEvent(msg)
            OnFileGenerationProgressUpdate(msg, 0)
        End If
    End Sub

#End Region

#Region " Pass-Through Functionality "

    Private Sub OnFileGenerationStarted(taskMsg As String) Handles m_Getter.FileGenerationStarted
        RaiseEvent FileGenerationStarted(taskMsg)
    End Sub

    Private Sub OnFileGenerationProgressUpdate(statusMsg As String, fractionDone As Double) Handles m_Getter.FileGenerationProgress
        RaiseEvent FileGenerationProgress(statusMsg, fractionDone)
    End Sub

    ''' <summary>
    ''' Compute the CRC32 hash for the file
    ''' </summary>
    ''' <param name="fullFilePath"></param>
    ''' <returns>File hash</returns>
    Function GenerateFileAuthenticationHash(fullFilePath As String) As String
        Return m_Getter.GetFileHash(fullFilePath)
    End Function

    Function GetAllCollections() As Dictionary(Of Integer, String)
        Return m_Getter.GetCollectionNameList
    End Function

    Function GetCollectionsByOrganism(organismID As Integer) As Dictionary(Of String, String)
        Return m_Getter.GetCollectionsByOrganism(organismID)
    End Function

    Function GetCollectionsByOrganismTable(organismID As Integer) As DataTable
        Return m_Getter.GetCollectionsByOrganismTable(organismID)
    End Function

    Function GetOrganismList() As Dictionary(Of String, String)
        Return m_Getter.GetOrganismList
    End Function

    Function GetOrganismListTable() As DataTable
        Return m_Getter.GetOrganismListTable
    End Function

    Overloads Function GetStoredFileAuthenticationHash(proteinCollectionID As Integer) As String
        Return m_Getter.GetStoredHash(proteinCollectionID)
    End Function

    Overloads Function GetStoredFileAuthenticationHash(proteinCollectionName As String) As String
        Return m_Getter.GetStoredHash(proteinCollectionName)
    End Function

    Function GetProteinCollectionID(proteinCollectionName As String) As Integer
        Return m_Getter.FindIDByName(proteinCollectionName)
    End Function

    Private Function GetProteinCollectionName(proteinCollectionID As Integer) As String
        Return m_Getter.FindNameByID(proteinCollectionID)
    End Function

#End Region

    Private Function RunSP_AddLegacyFileUploadRequest(legacyFilename As String, authenticationHash As String) As Integer

        If m_DatabaseAccessor Is Nothing Then
            Return 0
        End If

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddLegacyFileUploadRequest", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@legacy_File_name", SqlType.VarChar, 128).Value = legacyFilename
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256).Direction = ParameterDirection.Output
        dbTools.AddParameter(cmdSave, "@AuthenticationHash", SqlType.VarChar, 8).Value = authenticationHash

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Private Function GenerateHash(sourceText As String) As String
        ' Create an encoding object to ensure the encoding standard for the source text
        Dim encoding As New ASCIIEncoding()

        ' Retrieve a byte array based on the source text
        Dim byteSourceText() As Byte = encoding.GetBytes(sourceText)

        ' Compute the hash value from the source
        Dim sha1Hash() As Byte = m_SHA1Provider.ComputeHash(byteSourceText)

        ' And convert it to String format for return
        Dim sha1String As String = BitConverter.ToString(sha1Hash).Replace("-", "").ToLower()

        Return sha1String
    End Function
End Class
