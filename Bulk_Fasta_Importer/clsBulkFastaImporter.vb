Option Strict On

' This class will read a text file specifying one or more fasta files to load into the Protein Sequences database
'
' -------------------------------------------------------------------------------
' Written by Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
' Program started October 10, 2014

' E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
' Website: http://panomics.pnnl.gov/ or http://omics.pnl.gov
' -------------------------------------------------------------------------------
'
' Licensed under the Apache License, Version 2.0; you may not use this file except
' in compliance with the License.  You may obtain a copy of the License at
' http://www.apache.org/licenses/LICENSE-2.0

Imports System.Data.SqlClient
Imports System.IO
Imports System.Runtime.InteropServices
Imports Protein_Uploader
Imports ValidateFastaFile

Public Class clsBulkFastaImporter
    Inherits clsProcessFilesBaseClass

#Region "Constants and Enums"

    Public Const DMS_CONNECTION_STRING As String = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;"
    Public Const PROTEINSEQS_CONNECTION_STRING As String = "Data Source=proteinseqs;Initial Catalog=Protein_Sequences;Integrated Security=SSPI;"
    Protected Const QUERY_TIMEOUT_SECONDS As Integer = 20

    Enum eBulkImporterErrorCodes
        NoError = 0
        UnspecifiedError = -1
    End Enum

#End Region

#Region "Structures"
    Public Structure udtFastaFileInfoType
        Public FilePath As String
        Public OrganismID As Integer
        Public AuthID As Integer
    End Structure
#End Region


#Region "Classwide Variables"

    Protected WithEvents m_UploadHandler As IUploadProteins

    ''' <summary>
    ''' Organism info, where keys are organism name and values are organism ID
    ''' </summary>
    ''' <remarks>String searches will be case insensitive</remarks>
    Private mOrganismInfo As Dictionary(Of String, Integer)

    ''' <summary>
    ''' Annotation type info, where keys are annotation type names and values are annotation type IDs
    ''' </summary>
    ''' <remarks>String searches will be case insensitive</remarks>
    Private mAnnotationTypeInfo As Dictionary(Of String, Integer)

    ''' <summary>
    ''' Protein collection info, where keys are protecin collection names and values are protein collection IDs
    ''' </summary>
    ''' <remarks>String searches will be case insensitive</remarks>
    Private mProteinCollectionInfo As Dictionary(Of String, Integer)

    Private mDatabaseDataLoaded As Boolean
    Private mAnnotationViewInfoShown As Boolean
    Private mOrganismViewInfoShown As Boolean

    Private mLastProgressTime As DateTime

    Private mLocalErrorCode As eBulkImporterErrorCodes
#End Region

#Region "Properties"

    Public Property DMSConnectionString As String

    Public Property PreviewMode() As Boolean

    Public Property ProteinSeqsConnectionString As String

    Public Property ValidationAllowAllSymbolsInProteinNames As Boolean
    Public Property ValidationAllowAsterisks As Boolean
    Public Property ValidationAllowDash As Boolean
    Public Property ValidationMaxProteinNameLength As Integer

#End Region

    Public Sub New()
        MyBase.mFileDate = "October 22, 2015"
        InitializeLocalVariables()
    End Sub

    Public Overrides Function GetErrorMessage() As String
        ' Returns "" if no error

        Dim strErrorMessage As String

        If MyBase.ErrorCode = eProcessFilesErrorCodes.LocalizedError Or
           MyBase.ErrorCode = eProcessFilesErrorCodes.NoError Then
            Select Case mLocalErrorCode
                Case eBulkImporterErrorCodes.NoError
                    strErrorMessage = ""
                Case eBulkImporterErrorCodes.UnspecifiedError
                    strErrorMessage = "Unspecified localized error"
                Case Else
                    ' This shouldn't happen
                    strErrorMessage = "Unknown error state"
            End Select
        Else
            strErrorMessage = MyBase.GetBaseClassErrorMessage()
        End If

        Return strErrorMessage
    End Function

    Protected Sub InitializeLocalVariables()

        mDatabaseDataLoaded = False
        mOrganismInfo = New Dictionary(Of String, Integer)(StringComparer.CurrentCultureIgnoreCase)
        mAnnotationTypeInfo = New Dictionary(Of String, Integer)(StringComparer.CurrentCultureIgnoreCase)
        mProteinCollectionInfo = New Dictionary(Of String, Integer)(StringComparer.CurrentCultureIgnoreCase)

        mLocalErrorCode = eBulkImporterErrorCodes.NoError
        mLastProgressTime = DateTime.UtcNow()

        DMSConnectionString = DMS_CONNECTION_STRING
        ProteinSeqsConnectionString = PROTEINSEQS_CONNECTION_STRING
        ValidationAllowAllSymbolsInProteinNames = False
        ValidationAllowAsterisks = True
        ValidationAllowDash = True
        ValidationMaxProteinNameLength = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH

    End Sub

    Protected Function ParseFastaInfoFile(fastaInfoFilePath As String) As List(Of udtFastaFileInfoType)
        Try
            Dim sourceFileList = New List(Of udtFastaFileInfoType)
            Dim sourceFileNames = New SortedSet(Of String)(StringComparer.CurrentCultureIgnoreCase)

            Dim fiInfoFile = New FileInfo(fastaInfoFilePath)
            If Not fiInfoFile.Exists Then
                ShowErrorMessage("File not found: " & fastaInfoFilePath)
                MyBase.SetBaseClassErrorCode(eProcessFilesErrorCodes.InvalidInputFilePath)
                Return sourceFileList
            End If

            Dim requiredColsShown As Boolean
            Dim currentLine = 0

            Using srFastaInfoFile = New StreamReader(New FileStream(fiInfoFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                While srFastaInfoFile.Peek > -1
                    Dim dataLine = srFastaInfoFile.ReadLine()
                    currentLine += 1

                    If String.IsNullOrWhiteSpace(dataLine) Then
                        Continue While
                    End If

                    ' Required columns are:
                    ' FastaFilePath, OrganismName_or_ID, AnnotationTypeName_or_ID

                    Dim dataCols = dataLine.Split(ControlChars.Tab)
                    If dataCols.Count < 3 Then
                        ShowWarning("Invalid row; does not have 3 columns: " & dataLine)
                        If Not requiredColsShown Then
                            ShowMessage("Required columns are: FastaFilePath, OrganismName_or_ID, and AnnotationTypeName_or_ID")
                            requiredColsShown = True
                        End If
                        Continue While
                    End If

                    If String.IsNullOrWhiteSpace(dataCols(0)) Then
                        ShowWarning("Fasta file path is empty for line: " & currentLine)
                        Continue While
                    End If

                    Dim fastaFilePath = dataCols(0)
                    If Not fastaFilePath.Contains("\") Then
                        fastaFilePath = Path.Combine(fiInfoFile.DirectoryName, dataCols(0))
                    End If

                    Dim fiFastaFile = New FileInfo(fastaFilePath)
                    If Not fiFastaFile.Exists Then
                        ShowWarning("Fasta file not found: " & fastaFilePath)
                        Continue While
                    End If

                    Dim udtFastaFileInfo = New udtFastaFileInfoType
                    udtFastaFileInfo.FilePath = fiFastaFile.FullName

                    Dim organismID As Integer
                    If Not LookupOrganismID(dataCols(1), organismID) Then
                        Continue While
                    End If

                    Dim annotationTypeId As Integer
                    If Not LookupAnnotationTypeID(dataCols(2), annotationTypeId) Then
                        Continue While
                    End If

                    udtFastaFileInfo.OrganismID = organismID
                    udtFastaFileInfo.AuthID = annotationTypeId

                    ' Make sure the protein collection is not already in the Protein Sequences database
                    Dim proteinCollectionID As Integer
                    If Not LookupProteinCollectionID(Path.GetFileNameWithoutExtension(fiFastaFile.Name), proteinCollectionID) Then
                        Continue While
                    End If

                    If proteinCollectionID > 0 Then
                        ShowWarning("Fasta file already exists as a protein collection; skipping " & fiFastaFile.Name)
                        Continue While
                    End If

                    ' Make sure we don't add duplicate files to sourceFileList
                    If sourceFileNames.Contains(fiFastaFile.Name) Then
                        ShowWarning("Skipping duplicate file: " & fiFastaFile.Name)
                        Continue While
                    End If

                    sourceFileList.Add(udtFastaFileInfo)

                End While
            End Using

            If sourceFileList.Count = 0 Then
                ShowWarning("FastaInfoFile did not have any valid rows")
            End If

            Return sourceFileList

        Catch ex As Exception
            ShowErrorMessage("Error reading the Fasta Info File: " & ex.Message)
            Return New List(Of udtFastaFileInfoType)
        End Try

    End Function

    Private Function LoadDatabaseInfo() As Boolean
        Try
            If Not LoadOrganisms() Then
                Return False
            End If

            If Not LoadAnnotationInfo() Then
                Return False
            End If

            If Not LoadProteinCollectionInfo() Then
                Return False
            End If

            mDatabaseDataLoaded = True
            Return True
        Catch ex As Exception
            ShowErrorMessage("Error loading database info: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadAnnotationInfo() As Boolean

        Try
            Const sqlQuery = "SELECT ID, Display_Name FROM V_Annotation_Type_Picker"

            mAnnotationTypeInfo.Clear()

            Using cn = New SqlConnection(ProteinSeqsConnectionString)
                cn.Open()

                Using cmd = New SqlCommand(sqlQuery, cn)

                    cmd.CommandTimeout = QUERY_TIMEOUT_SECONDS

                    Using dbReader = cmd.ExecuteReader()
                        While dbReader.Read
                            Dim annotationTypeID = dbReader.GetInt32(0)
                            Dim annotationTypeName = dbReader.GetString(1)

                            mAnnotationTypeInfo.Add(annotationTypeName, annotationTypeID)
                        End While
                    End Using

                End Using

            End Using

            Return True

        Catch ex As Exception
            ShowErrorMessage("Error loading annotation type info: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadOrganisms() As Boolean
        Try
            Const sqlQuery = "SELECT Organism_ID, Name FROM V_Organism_Export"

            mOrganismInfo.Clear()

            Using cn = New SqlConnection(DMSConnectionString)
                cn.Open()

                Using cmd = New SqlCommand(sqlQuery, cn)

                    cmd.CommandTimeout = QUERY_TIMEOUT_SECONDS

                    Using dbReader = cmd.ExecuteReader()
                        While dbReader.Read
                            Dim organismID = dbReader.GetInt32(0)
                            Dim organismName = dbReader.GetString(1)

                            mOrganismInfo.Add(organismName, organismID)
                        End While
                    End Using

                End Using

            End Using

            Return True

        Catch ex As Exception
            ShowErrorMessage("Error loading organism info: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LoadProteinCollectionInfo() As Boolean

        Try
            Const sqlQuery = "SELECT DISTINCT Protein_Collection_ID, Name, Collection_State FROM V_Protein_Collection_List_Export"

            mProteinCollectionInfo.Clear()

            Using cn = New SqlConnection(ProteinSeqsConnectionString)
                cn.Open()

                Using cmd = New SqlCommand(sqlQuery, cn)

                    cmd.CommandTimeout = QUERY_TIMEOUT_SECONDS

                    Using dbReader = cmd.ExecuteReader()
                        While dbReader.Read
                            Dim proteinCollectionID = dbReader.GetInt32(0)
                            Dim proteinCollectionName = dbReader.GetString(1)

                            If Not mProteinCollectionInfo.ContainsKey(proteinCollectionName) Then
                                mProteinCollectionInfo.Add(proteinCollectionName, proteinCollectionID)
                            End If

                        End While
                    End Using

                End Using

            End Using

            Return True

        Catch ex As Exception
            ShowErrorMessage("Error loading protein collection info: " & ex.Message)
            Return False
        End Try

    End Function

    Private Function LookupAnnotationTypeID(annotationTypeNameOrID As String, <Out> ByRef annotationTypeId As Integer) As Boolean

        If Not mDatabaseDataLoaded Then
            annotationTypeId = 0
            If Not LoadDatabaseInfo() Then Return False
        End If

        If Integer.TryParse(annotationTypeNameOrID, annotationTypeId) Then
            ' Make sure the ID is valid
            If Not mAnnotationTypeInfo.ContainsValue(annotationTypeId) Then
                ShowWarning("Invalid Annotation Type ID: " & annotationTypeId)
                If Not mAnnotationViewInfoShown Then
                    Console.WriteLine("  ... see view V_Annotation_Type_Picker in the ProteinSeqs database")
                    mAnnotationViewInfoShown = True
                End If
                Return False
            End If
            Return True
        End If

        If mAnnotationTypeInfo.TryGetValue(annotationTypeNameOrID, annotationTypeId) Then
            Return True
        End If

        ShowWarning("Invalid Annotation Name: " & annotationTypeNameOrID)
        If Not mAnnotationViewInfoShown Then
            Console.WriteLine("  ... see view V_Annotation_Type_Picker in the ProteinSeqs database")
            mAnnotationViewInfoShown = True
        End If
        Return False

    End Function

    Private Function LookupOrganismID(organismNameOrID As String, <Out> ByRef organismId As Integer) As Boolean

        If Not mDatabaseDataLoaded Then
            organismId = 0
            If Not LoadDatabaseInfo() Then Return False
        End If

        If Integer.TryParse(organismNameOrID, organismId) Then
            ' Make sure the ID is valid
            If Not mOrganismInfo.ContainsValue(organismId) Then
                ShowWarning("Invalid Organism ID: " & organismId)
                If Not mOrganismViewInfoShown Then
                    Console.WriteLine("  ... see view V_Organism_Export in the DMS5 database")
                    mOrganismViewInfoShown = True
                End If
                Return False
            End If
            Return True
        End If

        If mOrganismInfo.TryGetValue(organismNameOrID, organismId) Then
            Return True
        End If

        ShowWarning("Invalid Organism Name: " & organismNameOrID)
        If Not mOrganismViewInfoShown Then
            Console.WriteLine("  ... see view V_Organism_Export in the DMS5 database")
            mOrganismViewInfoShown = True
        End If
        Return False

    End Function

    ''' <summary>
    ''' Lookup the protein collection ID using the protein collection name
    ''' </summary>
    ''' <param name="proteinCollectionName"></param>
    ''' <param name="proteinCollectionID">ID if a match; 0 if no match</param>
    ''' <returns></returns>
    ''' <remarks>True if success (even if the protein collection does not exist); false if a database error</remarks>
    Private Function LookupProteinCollectionID(proteinCollectionName As String, <Out> ByRef proteinCollectionID As Integer) As Boolean

        If Not mDatabaseDataLoaded Then
            proteinCollectionID = 0
            If Not LoadDatabaseInfo() Then Return False
        End If

        If mProteinCollectionInfo.TryGetValue(proteinCollectionName, proteinCollectionID) Then
            ' Collection exists
            Return True
        End If

        ' Collection does not exist
        proteinCollectionID = 0
        Return True

    End Function

    Public Overrides Function ProcessFile(strInputFilePath As String, strOutputFolderPath As String, strParameterFilePath As String, blnResetErrorCode As Boolean) As Boolean
        Try
            Dim sourceFileList As List(Of udtFastaFileInfoType)

            sourceFileList = ParseFastaInfoFile(strInputFilePath)

            If sourceFileList.Count = 0 Then Return False

            Dim success = UploadFastaFileList(sourceFileList)
            Return success

        Catch ex As Exception
            ShowErrorMessage("Error in ProcessFile: " & ex.Message)
            Return False
        End Try

    End Function

    Public Function UploadFastaFile(fastaFilePath As String, orgID As Integer, authID As Integer) As Boolean
        Dim sourceFileList = New List(Of udtFastaFileInfoType)

        Dim udtFastaFileInfo = New udtFastaFileInfoType
        udtFastaFileInfo.FilePath = fastaFilePath
        udtFastaFileInfo.OrganismID = orgID
        udtFastaFileInfo.AuthID = authID

        sourceFileList.Add(udtFastaFileInfo)

        Dim success = UploadFastaFileList(sourceFileList)
        Return success

    End Function

    Public Function UploadFastaFileList(sourceFileList As List(Of udtFastaFileInfoType)) As Boolean

        Dim fileInfoList = New List(Of IUploadProteins.UploadInfo)

        For Each sourceFile In sourceFileList
            Dim fiSourceFile = New FileInfo(sourceFile.FilePath)
            If Not fiSourceFile.Exists Then
                ShowWarning("File not found: " & sourceFile.FilePath)
                Continue For
            End If

            Dim upInfo = New IUploadProteins.UploadInfo(fiSourceFile, sourceFile.OrganismID, sourceFile.AuthID)
            fileInfoList.Add(upInfo)
        Next

        Return UploadFastaFileList(fileInfoList)

    End Function

    Public Function UploadFastaFileList(fileInfoList As List(Of IUploadProteins.UploadInfo)) As Boolean

        Try
            ' Initialize the uploader
            m_UploadHandler = New clsPSUploadHandler(ProteinSeqsConnectionString)

            m_UploadHandler.InitialSetup()

            m_UploadHandler.SetValidationOptions(IUploadProteins.eValidationOptionConstants.AllowAllSymbolsInProteinNames, ValidationAllowAllSymbolsInProteinNames)
            m_UploadHandler.SetValidationOptions(IUploadProteins.eValidationOptionConstants.AllowAsterisksInResidues, ValidationAllowAsterisks)
            m_UploadHandler.SetValidationOptions(IUploadProteins.eValidationOptionConstants.AllowDashInResidues, ValidationAllowDash)
            m_UploadHandler.MaximumProteinNameLength = ValidationMaxProteinNameLength

        Catch ex As Exception
            ShowErrorMessage("Error initializing the uploader: " & ex.Message)
            Return False
        End Try

        Try
            Console.WriteLine()
            If PreviewMode Then
                ShowMessage("Previewing upload of " & fileInfoList.Count & " file(s)")
                For Each fileInfo In fileInfoList
                    Console.WriteLine(fileInfo.FileInformation.FullName)
                Next
                Return True
            End If

            ShowMessage("Uploading " & fileInfoList.Count & " file(s)")

            ' Start the upload
            m_UploadHandler.BatchUpload(fileInfoList)

            Return True

        Catch ex As Exception
            ShowErrorMessage("Error uploading fasta file list: " & ex.Message)
            Return False
        End Try

    End Function

    Private Sub m_UploadHandler_BatchProgress(status As String) Handles m_UploadHandler.BatchProgress
        If DateTime.UtcNow.Subtract(mLastProgressTime).TotalSeconds >= 1 Then
            mLastProgressTime = DateTime.UtcNow
            Console.WriteLine(status)
        End If
    End Sub

    Private Sub m_UploadHandler_FASTAFileWarnings(FASTAFilePath As String, warningCollection As List(Of ICustomValidation.udtErrorInfoExtended)) Handles m_UploadHandler.FASTAFileWarnings
        Try
            For Each item In warningCollection
                ShowMessage("  ... Warning: " & item.MessageText & ": " & item.ProteinName)
            Next
        Catch ex As Exception
            Console.WriteLine("warningCollection is not type ValidateFastaFile.ICustomValidation.udtErrorInfoExtended")
        End Try

    End Sub

    Private Sub m_UploadHandler_FASTAValidationComplete(FASTAFilePath As String, UploadInfo As IUploadProteins.UploadInfo) Handles m_UploadHandler.FASTAValidationComplete
        ShowMessage("Validated " & FASTAFilePath)
        ShowMessage("  ... ProteinCount: " & UploadInfo.ProteinCount)

        Try
            If Not UploadInfo.ErrorList Is Nothing AndAlso UploadInfo.ErrorList.Count > 0 Then
                ShowMessage("  ... Error count: " & UploadInfo.ErrorList.Count)
            End If
        Catch ex As Exception
            Console.WriteLine("Exception examining UploadInfo.ErrorList: " & ex.Message)
        End Try

    End Sub

    Private Sub m_UploadHandler_InvalidFASTAFile(FASTAFilePath As String, errorCollection As List(Of ICustomValidation.udtErrorInfoExtended)) Handles m_UploadHandler.InvalidFASTAFile
        ShowWarning("Invalid fasta file: " & FASTAFilePath)

        Try
            For Each item In errorCollection
                ShowMessage("  ... Error: " & item.MessageText & ": " & item.ProteinName)
            Next
        Catch ex As Exception
            Console.WriteLine("errorCollection is not type ValidateFastaFile.ICustomValidation.udtErrorInfoExtended")
        End Try

    End Sub

    Private Sub m_UploadHandler_LoadEnd() Handles m_UploadHandler.LoadEnd

    End Sub

    Private Sub m_UploadHandler_LoadProgress(fractionDone As Double) Handles m_UploadHandler.LoadProgress
        If DateTime.UtcNow.Subtract(mLastProgressTime).TotalSeconds >= 1 Then
            mLastProgressTime = DateTime.UtcNow
            Console.WriteLine("  " & (fractionDone * 100).ToString("0.0") & "%")
        End If
    End Sub

    Private Sub m_UploadHandler_LoadStart(taskTitle As String) Handles m_UploadHandler.LoadStart
        ShowMessage(taskTitle)
    End Sub

    Private Sub m_UploadHandler_ValidationProgress(taskTitle As String, fractionDone As Double) Handles m_UploadHandler.ValidationProgress

    End Sub

    Private Sub m_UploadHandler_ValidFASTAFileLoaded(FASTAFilePath As String, UploadData As IUploadProteins.UploadInfo) Handles m_UploadHandler.ValidFASTAFileLoaded
        ShowMessage("Uploaded " & FASTAFilePath)
        ShowMessage("  ... ProteinCount: " & UploadData.ProteinCount)

        Try
            If Not UploadData.ErrorList Is Nothing AndAlso UploadData.ErrorList.Count > 0 Then
                ShowMessage("  ... Error count: " & UploadData.ErrorList.Count)
            End If
        Catch ex As Exception
            Console.WriteLine("Exception examining UploadData.ErrorList: " & ex.Message)
        End Try

    End Sub

    Private Sub m_UploadHandler_WroteLineEndNormalizedFASTA(newFilePath As String) Handles m_UploadHandler.WroteLineEndNormalizedFASTA
        Console.WriteLine("WroteLineEndNormalizedFASTA: " & newFilePath)
    End Sub

End Class
