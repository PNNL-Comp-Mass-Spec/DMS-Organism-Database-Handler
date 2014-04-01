Option Strict On

Imports System.IO

Public Class clsGetFASTAFromDMS
	Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS

	Public Const LOCK_FILE_PROGRESS_TEXT As String = "Lockfile"

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

	Protected WithEvents m_FileTools As PRISM.Files.clsFileTools
	Protected m_LastLockQueueWaitTimeLog As System.DateTime

	Public Sub New(ByVal ProteinStorageConnectionString As String)

		m_PSConnectionString = ProteinStorageConnectionString
		ClassSelector(ProteinStorageConnectionString, ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes.fasta, ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.forward)
		m_SHA1Provider = New System.Security.Cryptography.SHA1Managed
		m_FileTools = New PRISM.Files.clsFileTools
	End Sub

	Public ReadOnly Property ExporterComponent() As clsGetFASTAFromDMSForward ' Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.ExporterComponent
		Get
			Return Me.m_Getter
		End Get
	End Property

	Public ReadOnly Property WaitingForLockFile() As Boolean
		Get
			Return m_WaitingForLockFile
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

			Case ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes.decoyX
				Me.m_Getter = New clsGetFASTAFromDMSDecoyX( _
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

		Dim collectionList As ArrayList = Nothing

		Dim optionsParser As clsFileCreationOptions
		optionsParser = New clsFileCreationOptions(Me.m_PSConnectionString)
		Dim cleanOptionsString As String

		If Me.m_TableGetter Is Nothing Then
			Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
		End If

		' Trim any leading or trailing commas
		ProteinCollectionNameList = ProteinCollectionNameList.Trim(","c)

		' Look for cases of multiple commas in a row or spaces around a comma
		' Replace any matches with a single comma
		Dim extraCommaCheckRegex As New System.Text.RegularExpressions.Regex("[, ]{2,}")

		ProteinCollectionNameList = extraCommaCheckRegex.Replace(ProteinCollectionNameList, ",")

		If ProteinCollectionNameList.Length > 0 And Not ProteinCollectionNameList.ToLower.Equals("na") Then
			'Parse out protein collections from "," delimited list
			ProteinCollections = ProteinCollectionNameList.Split(","c)

			collectionList = New ArrayList(ProteinCollections.Length)
			For Each collectionName In ProteinCollections
				collectionList.Add(collectionName.Trim())
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

			Return ExportLegacyFastaFile(LegacyFASTAFileName, ExportPath)

		End If

		Return Nothing


	End Function

	Protected Function ExportLegacyFastaFile(ByVal LegacyFASTAFileName As String, _
	  ByVal ExportPath As String) As String

		Dim legacyStaticFilePath As String = ""
		Dim finalFileHash As String = ""

		Dim strCollectionListHexHash As String = Me.GenerateHash(LegacyFASTAFileName)
		Dim LockFileHash As String = strCollectionListHexHash

		Dim lockFi As FileInfo

		If Not LookupLegacyFastaFileDetails(LegacyFASTAFileName, legacyStaticFilePath, finalFileHash) Then
			' Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
			' An exception has probably already been thrown
			Return Nothing
		End If

		Dim fiSourceFile = New FileInfo(legacyStaticFilePath)

		If Not fiSourceFile.Exists Then
			Throw New System.Exception("Legacy fasta file not found: " & legacyStaticFilePath & " (path comes from V_Legacy_Static_File_Locations)")
		End If

		' Look for file LegacyFASTAFileName in folder ExportPath
		' If it exists, and if a .lock file does not exist, then compare file sizes and file modification dates

		Dim fiFinalFile As FileInfo
		fiFinalFile = New FileInfo(Path.Combine(ExportPath, LegacyFASTAFileName))
		If fiFinalFile.Exists AndAlso fiFinalFile.Length > 0 Then
			' Make sure a .lock file doesn't exist
			' If it does exist, then another process on this computer is likely creating the .Fasta file

			lockFi = New FileInfo(Path.Combine(ExportPath, LockFileHash + ".lock"))

			If lockFi.Exists Then
				' Another program is creating a .Fasta file; cannot assume it is ready-for-use
			Else
				' Make sure the file sizes match and that the local file is not older than the source file
				If fiSourceFile.Length = fiFinalFile.Length AndAlso fiFinalFile.LastWriteTimeUtc >= fiSourceFile.LastWriteTimeUtc.AddSeconds(-0.1) Then
					If ExportLegacyFastaValidateHash(fiFinalFile, finalFileHash, False) Then
						Me.OnTaskCompletion(fiFinalFile.FullName)
						Return finalFileHash
					End If
				End If
			End If
		End If


		' The file is not present on the local computer (or the file size is different or it is older than the parent fasta file)
		' We need to create a lock file, then copy the .fasta file locally

		If String.IsNullOrEmpty(legacyStaticFilePath) Then
			Throw New System.Exception("Storage path for " & LegacyFASTAFileName & " is empty according to V_Legacy_Static_File_Locations; unable to continue")
		End If

		' Make sure we have enough disk free space

		Dim errorMessage As String = String.Empty
		Dim sourceFileSizeMB As Double = fiSourceFile.Length / 1024.0 / 1024.0

		If Not PRISM.Files.clsFileTools.ValidateFreeDiskSpace(Path.Combine(ExportPath, "TargetFile.tmp"), sourceFileSizeMB, 150, errorMessage) Then
			If String.IsNullOrEmpty(errorMessage) Then errorMessage = "clsFileTools.ValidateFreeDiskSpace returned a blank error message"
			Throw New IOException("Unable to copy legacy FASTA file to " & ExportPath & ". " & errorMessage)
		End If

		' If we get here, then finalFileName = "" or the file is not present or the LockFile is present
		' Try to create a lock file, then either wait for an existing lock file to go away or export the database
		Dim lockStream As FileStream
		lockStream = CreateLockStream(ExportPath, LockFileHash, LegacyFASTAFileName)

		If lockStream Is Nothing Then
			' Unable to create a lock stream; an exception has likely already been thrown
			Throw New System.Exception("Unable to create lock file required to export " & LegacyFASTAFileName)
		End If

		If Not fiFinalFile Is Nothing Then

			' Check again for the existence of the desired .Fasta file
			' It's possible another process created .Fasta file while this process was waiting for the other process's lock file to disappear
			fiFinalFile.Refresh()
			If fiFinalFile.Exists AndAlso fiSourceFile.Length = fiFinalFile.Length AndAlso fiFinalFile.LastWriteTimeUtc >= fiSourceFile.LastWriteTimeUtc.AddSeconds(-0.1) Then
				' The final file now does exist (and has the correct size / date)
				' The other process that made the file should have updated the database with the file hash; determine the hash now
				If Not LookupLegacyFastaFileDetails(LegacyFASTAFileName, legacyStaticFilePath, finalFileHash) Then
					' Could not find LegacyFASTAFileName in V_Legacy_Static_File_Locations
					' An exception has probably already been thrown
					Return Nothing
				End If

				If ExportLegacyFastaValidateHash(fiFinalFile, finalFileHash, False) Then
					DeleteLockStream(ExportPath, LockFileHash, lockStream)
					Me.OnTaskCompletion(fiFinalFile.FullName)
					Return finalFileHash
				End If

			End If

		End If

		' Copy the .Fasta file from the remote computer to this computer
		' We're temporarily naming it with the hash name
		Dim InterimFastaFI As New FileInfo(Path.Combine(ExportPath, strCollectionListHexHash & "_" & Path.GetFileNameWithoutExtension(legacyStaticFilePath) & ".fasta"))
		If InterimFastaFI.Exists Then
			InterimFastaFI.Delete()
		End If

		m_LastLockQueueWaitTimeLog = System.DateTime.UtcNow
		m_FileTools.CopyFileUsingLocks(fiSourceFile, InterimFastaFI.FullName, "OrgDBHandler", Overwrite:=False)

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
		If ExportLegacyFastaValidateHash(fiFinalFile, finalFileHash, True) Then
			DeleteLockStream(ExportPath, LockFileHash, lockStream)
			Me.OnTaskCompletion(fiFinalFile.FullName)
			Return finalFileHash
		End If

		' This code will only get reached if an error occurred in ExportLegacyFastaValidateHash()
		' We'll go ahead and return the hash anyway
		DeleteLockStream(ExportPath, LockFileHash, lockStream)
		Me.OnFileGenerationCompleted(fiFinalFile.FullName)
		Me.OnTaskCompletion(fiFinalFile.FullName)

		Return finalFileHash

	End Function

	Protected Function ExportLegacyFastaValidateHash(ByRef finalFileFI As FileInfo, _
	 ByRef finalFileHash As String, _
	 ByVal blnForceRegenerateHash As Boolean) As Boolean

		If String.IsNullOrEmpty(finalFileHash) Then
			finalFileHash = GenerateAndStoreLegacyFileHash(finalFileFI.FullName)

			' Update the hash validation file
			UpdateHashValidationFile(finalFileFI.FullName, finalFileHash)

			Return True
		Else
			' ValidateMatchingHash will use Me.GenerateFileAuthenticationHash() to generate a hash for the given file
			' Since this can be time consuming, we only do this every 48 hours
			' If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
			If ValidateMatchingHash(finalFileFI.FullName, finalFileHash, 48, blnForceRegenerateHash) Then
				Return True
			End If
		End If

		Return False

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
		Dim InterimFastaFI As FileInfo
		Dim lockFi As FileInfo

		Dim strProteinCollectionList As String
		strProteinCollectionList = Join(ProteinCollectionNameList.ToArray, ",")

		Dim hashableString As String
		hashableString = strProteinCollectionList + "/" + CreationOptionsString
		Dim strCollectionListHexHash As String = Me.GenerateHash(hashableString)
		Dim LockFileHash As String = strCollectionListHexHash

		Dim finalFileName As String
		Dim fileNameSql As String
		Dim finalFileHash As String
		Dim fileNameTable As DataTable
		Dim foundRow As DataRow

		If Me.m_TableGetter Is Nothing Then
			Me.m_TableGetter = New TableManipulationBase.clsDBTask(Me.m_PSConnectionString)
		End If

		fileNameSql = "SELECT Archived_File_Path, Archived_File_ID, Authentication_Hash " &
		  "FROM T_Archived_Output_Files " &
		  "WHERE Collection_List_Hex_Hash = '" & strCollectionListHexHash & "' AND " &
		  "Protein_Collection_List = '" & strProteinCollectionList & "' AND " &
		  "Archived_File_State_ID <> 3 " &
		  "ORDER BY File_Modification_Date desc"

		fileNameTable = Me.m_TableGetter.GetTable(fileNameSql)
		If fileNameTable.Rows.Count >= 1 Then
			foundRow = fileNameTable.Rows(0)
			finalFileName = Path.GetFileName(CStr(foundRow.Item("Archived_File_Path")))
			finalFileHash = CStr(foundRow.Item("Authentication_Hash"))
		Else
			finalFileName = ""
			finalFileHash = ""
		End If

		Dim finalFileFI As FileInfo = Nothing

		If finalFileName.Length > 0 Then
			' Look for file finalFileName in folder ExportPath
			' If it exists, and if a .lock file does not exist, then we can safely assume the .Fasta file is ready for use

			finalFileFI = New FileInfo(Path.Combine(ExportPath, finalFileName))
			If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
				' Make sure a .lock file doesn't exist
				' If it does exist, then another process on this computer is likely creating the .Fasta file

				lockFi = New FileInfo(Path.Combine(ExportPath, LockFileHash + ".lock"))

				If lockFi.Exists Then
					' Another program is creating a .Fasta file; cannot assume it is ready-for-use
				Else
					' ValidateMatchingHash will use Me.GenerateFileAuthenticationHash() to generate a hash for the given file
					' Since this can be time consuming, we only do this every 48 hours
					' If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
					If ValidateMatchingHash(finalFileFI.FullName, finalFileHash, 48, False) Then
						Me.OnTaskCompletion(finalFileFI.FullName)
						Return finalFileHash
					End If
				End If
			End If
		End If

		' If we get here, then finalFileName = "" or the file is not present or the LockFile is present or the hash file is out-of-date
		' Try to create a lock file, then either wait for an existing lock file to go away or export the database
		Dim lockStream As FileStream
		lockStream = CreateLockStream(ExportPath, LockFileHash, "Protein collection list " & strProteinCollectionList)

		If lockStream Is Nothing Then
			' Unable to create a lock stream; an exception has likely already been thrown
			Throw New System.Exception("Unable to create lock file required to export " & finalFileName)
		End If

		If Not finalFileFI Is Nothing Then

			' Check again for the existence of the desired .Fasta file
			' It's possible another process created .Fasta file while this process was waiting for the other process's lock file to disappear
			finalFileFI.Refresh()
			If finalFileFI.Exists AndAlso finalFileFI.Length > 0 Then
				' The final file now does exist (and is non-zero in size); we're good to go

				If String.IsNullOrEmpty(finalFileHash) Then
					' FinalFileHash is empty, which means the other process that just exported this file was the first process to actually use this file
					' We need to return a non-empty hash value, so compute the SHA1 hash now
					finalFileHash = GenerateFileAuthenticationHash(finalFileFI.FullName)
				End If

				' ValidateMatchingHash will use Me.GenerateFileAuthenticationHash() to generate a hash for the given file
				' Since this can be time consuming, we only do this every 48 hours
				' If the generated hash does not match the expected hash (finalFileHash) then we will re-generate the .fasta file
				If ValidateMatchingHash(finalFileFI.FullName, finalFileHash, 48, False) Then
					DeleteLockStream(ExportPath, LockFileHash, lockStream)
					Me.OnTaskCompletion(finalFileFI.FullName)
					Return finalFileHash
				End If

			End If
		End If

		' We're finally ready to generate the .Fasta file

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

		Dim counter As Integer
		Dim Archived_File_ID As Integer

		If String.IsNullOrEmpty(SHA1) Then
			Throw New Exception("m_Getter.ExportFASTAFile returned a blank string for the Sha1 authentication hash; this likely represents a problem")
			Return SHA1
		End If

		counter = 0
		For Each CollectionName In ProteinCollectionNameList
			If counter = 0 Then
				Archived_File_ID = Me.m_Archiver.ArchiveCollection( _
				 CollectionName, _
				 Me.m_CollectionType, _
				 Me.m_OutputSequenceType, _
				 Me.m_DatabaseFormatType, _
				 Me.m_FinalOutputPath, _
				 CreationOptionsString, SHA1, strProteinCollectionList)

				If Archived_File_ID = 0 Then
					' Error making an entry in T_Archived_Output_Files; abort
					Throw New Exception("Error archiving collection; Archived_File_ID = 0")
				End If

			Else
				tmpID = Me.GetProteinCollectionID(CollectionName)
				Me.m_Archiver.AddArchiveCollectionXRef(tmpID, Archived_File_ID)
			End If
			counter += 1
		Next

		' Rename the new protein collection to the correct, final name on the local computer
		' E.g. rename from 38FFACAC.fasta to ID_001874_38FFACAC.fasta
		InterimFastaFI = New FileInfo(Me.m_FinalOutputPath)

		finalFileName = Path.GetFileName(Me.m_Archiver.Archived_File_Name)
		finalFileFI = New FileInfo(Path.Combine(ExportPath, finalFileName))

		If finalFileFI.Exists Then
			' Somehow the final file has appeared in the folder (this shouldn't have happened with the lock file present)
			' Delete it
			finalFileFI.Delete()
		End If

		' Delete any other files that exist with the same extension as finalFileFI.FullName
		' These are likely index files used by Inspect or MSGFDB and they will need to be re-generated
		DeleteFASTAIndexFiles(finalFileFI)

		InterimFastaFI.MoveTo(finalFileFI.FullName)

		' Update the hash validation file
		UpdateHashValidationFile(finalFileFI.FullName, SHA1)

		DeleteLockStream(ExportPath, LockFileHash, lockStream)

		Me.OnTaskCompletion(finalFileFI.FullName)
		Return SHA1

	End Function

	Protected Function CreateLockStream(ByVal ExportPath As String, _
	   ByVal LockFileHash As String, _
	   ByVal ProteinCollectionListOrLegacyFastaFileName As String) As FileStream

		' Creates a new lock file
		' If an existing file is not found, but a lock file was successfully created, then lockStream will be a valid file stream

		Dim lockFi As FileInfo
		Dim startTime As DateTime = DateTime.UtcNow
		Dim intAttemptCount As Integer = 0

		Dim lockStream As FileStream

		lockFi = New FileInfo(Path.Combine(ExportPath, LockFileHash + ".lock"))

		Do
			intAttemptCount += 1

			Try
				lockFi.Refresh()
				If lockFi.Exists Then
					Me.m_WaitingForLockFile = True

					Dim LockTimeoutTime As DateTime = lockFi.LastWriteTimeUtc.AddMinutes(60)
					OnFileGenerationProgressUpdate(LOCK_FILE_PROGRESS_TEXT & " found; waiting until it is deleted or until " & LockTimeoutTime.ToLocalTime().ToString() & ": " & lockFi.Name, 0)

					While lockFi.Exists AndAlso System.DateTime.UtcNow < LockTimeoutTime
						System.Threading.Thread.Sleep(5000)
						lockFi.Refresh()
						If DateTime.UtcNow.Subtract(startTime).TotalMinutes >= 60 Then
							Exit While
						End If
					End While

					lockFi.Refresh()
					If lockFi.Exists Then
						OnFileGenerationProgressUpdate(LOCK_FILE_PROGRESS_TEXT & " still exists; assuming another process timed out; thus, now deleting file " & lockFi.Name, 0)
						lockFi.Delete()
					End If

					Me.m_WaitingForLockFile = False

				End If

				' Try to create a lock file so that the calling procedure can create the required .Fasta file (or validate that it now exists)

				' Try to create the lock file
				' If another process is still using it, an exception will be thrown
				lockStream = lockFi.Create()

				' We have successfully created a lock file, 
				' so we should exit the Do Loop
				Exit Do

			Catch ex As Exception
				OnFileGenerationProgressUpdate("Exception while monitoring " & LOCK_FILE_PROGRESS_TEXT & " " & lockFi.FullName & ": " & ex.Message, 0)
			End Try

			' Something went wrong; wait for 15 seconds then try again
			System.Threading.Thread.Sleep(15000)

			If intAttemptCount >= 4 Then
				' Something went wrong 4 times in a row (typically either creating or deleting the .Lock file)
				' Give up trying to export
				If ProteinCollectionListOrLegacyFastaFileName Is Nothing Then
					ProteinCollectionListOrLegacyFastaFileName = "??"
				End If

				' Exception: Unable to create Lockfile required to export Protein collection ...
				Throw New System.Exception("Unable to create " & LOCK_FILE_PROGRESS_TEXT & " required to export " & ProteinCollectionListOrLegacyFastaFileName & "; tried 4 times without success")
			End If
		Loop


		Return lockStream

	End Function

	Protected Sub DeleteFASTAIndexFiles(ByRef fiFinalFastaFile As FileInfo)

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

			' This code will also delete the .hashcheck file; that's OK
			' e.g., ID_002750_1363538A.fasta.1363538A.hashcheck

			For Each fiFileToDelete As FileInfo In fiFinalFastaFile.Directory.GetFileSystemInfos(strBaseName & ".*")
				DeleteFastaIndexFile(fiFileToDelete.FullName)
			Next

			For Each fiFileToDelete As FileInfo In fiFinalFastaFile.Directory.GetFileSystemInfos(strBaseName & "_shuffle*.*")
				DeleteFastaIndexFile(fiFileToDelete.FullName)
			Next
		Catch ex As Exception

		End Try

	End Sub

	Protected Sub DeleteFastaIndexFile(ByVal strFilePath As String)
		Try
			File.Delete(strFilePath)
		Catch ex As Exception
			Console.WriteLine("Error deleting file: " & ex.Message)
		End Try
	End Sub

	Protected Sub DeleteLockStream(ByVal ExportPath As String, ByVal LockFileHash As String, ByVal lockStream As FileStream)

		If Not lockStream Is Nothing Then
			lockStream.Close()
		End If

		Dim lockFi As FileInfo
		lockFi = New FileInfo(Path.Combine(ExportPath, LockFileHash + ".lock"))
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
		RunSP_AddLegacyFileUploadRequest(Path.GetFileName(strFastaFilePath), FileHash)

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

	Protected Function GetHashFileValidationInfo(ByVal strFastaFilePath As String, ByVal strSHA1 As String) As FileInfo

		Dim fiFastaFile As FileInfo
		Dim strHashValidationFileName As String

		fiFastaFile = New FileInfo(strFastaFilePath)
		strHashValidationFileName = Path.Combine(fiFastaFile.DirectoryName, fiFastaFile.Name & "." & strSHA1 & ".hashcheck")

		Return New FileInfo(strHashValidationFileName)

	End Function

	Protected Sub UpdateHashValidationFile(ByVal strFastaFilePath As String, ByVal strSHA1 As String)
		Dim fiHashValidationFile As FileInfo
		fiHashValidationFile = GetHashFileValidationInfo(strFastaFilePath, strSHA1)
		UpdateHashValidationFile(fiHashValidationFile)
	End Sub

	Protected Sub UpdateHashValidationFile(ByRef fiHashValidationFile As FileInfo)

		Dim swOutFile As StreamWriter
		swOutFile = New StreamWriter(fiHashValidationFile.Open(IO.FileMode.Create))

		swOutFile.WriteLine("Hash validated " & System.DateTime.Now.ToString)
		swOutFile.Close()

	End Sub

	''' <summary>
	''' Validates that the hash of a .fasta file matches the expected hash value
	''' If the actual hash differs and if blnForceRegenerateHash=True, then this strExpectedHash get updated
	''' blnForceRegenerateHash should be set to True only when processing legacy fasta files that have been newly copied to this computer
	''' </summary>
	''' <param name="strFastaFilePath">Fasta file to check</param>
	''' <param name="strExpectedHash">Expected SHA-1 hash.</param>
	''' <param name="intRetryHoldoffHours">Time between re-generating the hash value for an existing file</param>
	''' <param name="blnForceRegenerateHash">Re-generate the hash</param>
	''' <returns>True if the hash values match, or if blnForceRegenerateHash=True</returns>
	''' <remarks></remarks>
	Protected Function ValidateMatchingHash(ByVal strFastaFilePath As String, _
	 ByRef strExpectedHash As String, _
	 ByVal intRetryHoldoffHours As Integer, _
	 ByVal blnForceRegenerateHash As Boolean) As Boolean

		Dim fiFastaFile As FileInfo
		Dim fiHashValidationFile As FileInfo

		Dim strSHA1 As String

		Try
			fiFastaFile = New FileInfo(strFastaFilePath)

			If fiFastaFile.Exists Then
				fiHashValidationFile = GetHashFileValidationInfo(strFastaFilePath, strExpectedHash)

				If fiHashValidationFile.Exists And Not blnForceRegenerateHash Then
					If System.DateTime.UtcNow.Subtract(fiHashValidationFile.LastWriteTimeUtc).TotalHours <= intRetryHoldoffHours Then
						' Hash check file exists, and the file is less than 48 hours old
						Return True
					End If
				End If

				' Either the hash validation file doesn't exist, or it's too old, or blnForceRegenerateHash = True
				' Regenerate the hash
				strSHA1 = Me.GenerateFileAuthenticationHash(fiFastaFile.FullName)

				If strExpectedHash = strSHA1 OrElse blnForceRegenerateHash Then
					' Update the hash validation file
					UpdateHashValidationFile(strFastaFilePath, strSHA1)

					If strExpectedHash <> strSHA1 And blnForceRegenerateHash Then
						' Hash values don't match, but blnForceRegenerateHash=True
						' Update the hash value stored in T_Legacy_File_Upload_Requests for this fasta file
						RunSP_AddLegacyFileUploadRequest(fiFastaFile.Name, strSHA1)

						' Update strExpectedHash
						strExpectedHash = strSHA1
					End If

					Return True
				End If
			End If
		Catch ex As Exception
			OnFileGenerationProgressUpdate("Exception while re-computing the hash of the fasta file: " & ex.Message, 0)
		End Try

		Return False

	End Function

#Region "Events and Event Handlers"
	Public Event FileGenerationCompleted(ByVal FullOutputPath As String) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationCompleted
	Public Event FileGenerationProgress(ByVal statusMsg As String, ByVal fractionDone As Double) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationProgress
	Public Event FileGenerationStarted(ByVal taskMsg As String) Implements ExportProteinCollectionsIFC.IGetFASTAFromDMS.FileGenerationStarted

	Private Sub OnFileGenerationCompleted(ByVal FullOutputPath As String) Handles m_Getter.FileGenerationCompleted
		If Me.m_ArchiveCollectionList Is Nothing Then
			Me.m_ArchiveCollectionList = New ArrayList
		End If
		Me.m_ArchiveCollectionList.Add(Path.GetFileName(FullOutputPath))
		Me.m_FinalOutputPath = FullOutputPath
	End Sub

	Private Sub OnTaskCompletion(ByVal FinalOutputPath As String)
		RaiseEvent FileGenerationCompleted(FinalOutputPath)
	End Sub


	Private Sub m_FileTools_WaitingForLockQueue(SourceFilePath As String, TargetFilePath As String, MBBacklogSource As Integer, MBBacklogTarget As Integer) Handles m_FileTools.WaitingForLockQueue
		Dim strServers As String

		If System.DateTime.UtcNow.Subtract(m_LastLockQueueWaitTimeLog).TotalSeconds >= 30 Then
			m_LastLockQueueWaitTimeLog = System.DateTime.UtcNow
			Console.WriteLine("Waiting for lockfile queue to fall below threshold to fall below threshold (Protein_Exporter); SourceBacklog=" & MBBacklogSource & " MB, TargetBacklog=" & MBBacklogTarget & " MB, Source=" & SourceFilePath & ", Target=" & TargetFilePath)

			If MBBacklogSource > 0 AndAlso MBBacklogTarget > 0 Then
				strServers = m_FileTools.GetServerShareBase(SourceFilePath) & " and " & m_FileTools.GetServerShareBase(TargetFilePath)
			ElseIf MBBacklogTarget > 0 Then
				strServers = m_FileTools.GetServerShareBase(TargetFilePath)
			Else
				strServers = m_FileTools.GetServerShareBase(SourceFilePath)
			End If

			OnFileGenerationProgressUpdate("Waiting for lockfile queue on " & strServers & " to fall below threshold", 0)
		End If
	End Sub

#End Region

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
		Return Me.m_Getter.FindIDByName(Path.GetFileNameWithoutExtension(ProteinCollectionName))
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
