Imports System.Collections.Generic
Imports System.IO
Imports System.Windows.Forms
Imports Protein_Exporter
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports Protein_Importer
Imports Protein_Storage
Imports TableManipulationBase
Imports ValidateFastaFile

Public Interface IUploadProteins

    Enum eValidationOptionConstants As Integer
        AllowAsterisksInResidues = 0
        AllowDashInResidues = 1
        AllowAllSymbolsInProteinNames = 2
    End Enum

    Function UploadCollection(
        fileContents As IProteinStorage,
        selectedProteins As List(Of String),
        collectionName As String,
        description As String,
        collectionSource As String,
        collectionType As IAddUpdateEntries.CollectionTypes,
        organismID As Integer,
        annotationTypeID As Integer) As Integer

    Function UploadCollection(
        fileContents As IProteinStorage,
        filepath As String,
        organismID As Integer,
        annotationTypeID As Integer,
        description As String,
        source As String) As Integer

    Sub BatchUpload(fileInfoList As IEnumerable(Of UploadInfo))

    Sub ResetErrorList()

    Sub SetValidationOptions(eValidationOptionName As eValidationOptionConstants, blnEnabled As Boolean)

    Property MaximumProteinNameLength As Integer

    Structure UploadInfo
        Public Sub New(inputFile As FileInfo, orgId As Integer, annotTypeID As Integer)
            FileInformation = inputFile
            OrganismID = orgId
            AnnotationTypeID = annotTypeID
            EncryptionPassphrase = String.Empty
            Description = String.Empty
            Source = String.Empty
        End Sub
        Public FileInformation As FileInfo
        Public OriginalFileInformation As FileInfo
        Public OrganismID As Integer
        Public Description As String
        Public Source As String
        Public AnnotationTypeID As Integer
        Public ProteinCount As Integer
        Public ErrorList As List(Of String)
        Public SummarizedErrors As Hashtable
        Public ExportedProteinCount As Integer

        ' <Obsolete("No longer supported")>
        Public EncryptSequences As Boolean

        ' <Obsolete("No longer supported")>
        Public EncryptionPassphrase As String
    End Structure

    Event LoadStart(taskTitle As String)
    Event LoadProgress(fractionDone As Double)
    Event LoadEnd()
    Event BatchProgress(status As String)
    Event ValidationProgress(taskTitle As String, fractionDone As Double)
    Event ValidFASTAFileLoaded(fastaFilePath As String, uploadData As UploadInfo)
    Event InvalidFASTAFile(fastaFilePath As String, errorCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))
    Event FASTAFileWarnings(fastaFilePath As String, warningCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))
    Event FASTAValidationComplete(fastaFilePath As String, uploadInfo As UploadInfo)
    Event WroteLineEndNormalizedFASTA(newFilePath As String)
End Interface


Public Class clsPSUploadHandler
    Implements IUploadProteins

    Protected m_PersistentTaskNum As Integer
    Protected m_NormalizedFASTAFilePath As String
    Protected m_ProteinCollectionsList As DataTable

    Protected ReadOnly m_DatabaseAccessor As IGetSQLData
    Protected WithEvents m_Importer As IImportProteins
    Protected WithEvents m_Upload As IAddUpdateEntries
    Protected WithEvents m_Export As IGetFASTAFromDMS
    Protected WithEvents m_Validator As clsCustomValidateFastaFiles

    Protected Event LoadStart(taskTitle As String) Implements IUploadProteins.LoadStart
    Protected Event LoadProgress(fractionDone As Double) Implements IUploadProteins.LoadProgress
    Protected Event LoadEnd() Implements IUploadProteins.LoadEnd
    Protected Event BatchProgress(status As String) Implements IUploadProteins.BatchProgress
    Protected Event ValidationProgress(taskTitle As String, fractionDone As Double) Implements IUploadProteins.ValidationProgress
    Protected Event ValidFASTAFileLoaded(fastaFilePath As String, uploadData As IUploadProteins.UploadInfo) Implements IUploadProteins.ValidFASTAFileLoaded
    Protected Event InvalidFASTAFile(fastaFilePath As String, errorCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)) Implements IUploadProteins.InvalidFASTAFile
    Protected Event FASTAFileWarnings(fastaFilePath As String, warningCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)) Implements IUploadProteins.FASTAFileWarnings
    Protected Event FASTAValidationComplete(fastaFilePath As String, uploadInfo As IUploadProteins.UploadInfo) Implements IUploadProteins.FASTAValidationComplete
    Protected Event WroteLineEndNormalizedFASTA(newFilePath As String) Implements IUploadProteins.WroteLineEndNormalizedFASTA


    Protected m_ExportedProteinCount As Integer
    Protected mMaximumProteinNameLength As Integer = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH

    Private WithEvents m_Encryptor As clsCollectionEncryptor

    ' Note: this array gets initialized with space for 10 items
    ' If eValidationOptionConstants gets more than 10 entries, then this array will need to be expanded
    Protected mValidationOptions() As Boolean

    Public Property MaximumProteinNameLength As Integer Implements IUploadProteins.MaximumProteinNameLength
        Get
            Return mMaximumProteinNameLength
        End Get
        Set
            mMaximumProteinNameLength = Value
            m_Upload.MaximumProteinNameLength = Value
        End Set
    End Property

    Private Sub ResetErrorList() Implements IUploadProteins.ResetErrorList
        m_Validator.ClearErrorList()
    End Sub

    Private Sub OnLoadStart(taskTitle As String)
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Private Sub OnProgressUpdate(fractionDone As Double)
        RaiseEvent LoadProgress(fractionDone)
    End Sub

    Private Sub OnLoadEnd()
        RaiseEvent LoadEnd()
    End Sub

    Private Sub OnBatchProgressUpdate(status As String)
        RaiseEvent BatchProgress(status)
    End Sub

    Private Sub OnFileValidationComplete(fastaFilePath As String, uploadInfo As IUploadProteins.UploadInfo)
        RaiseEvent FASTAValidationComplete(fastaFilePath, uploadInfo)
    End Sub

    Private Sub LoadStartHandler(taskTitle As String) Handles m_Upload.LoadStart, m_Importer.LoadStart
        OnLoadStart(taskTitle)
    End Sub

    Private Sub LoadProgressHandler(fractionDone As Double) Handles m_Upload.LoadProgress, m_Importer.LoadProgress
        OnProgressUpdate(fractionDone)
    End Sub

    Private Sub LoadEndHandler() Handles m_Upload.LoadEnd, m_Importer.LoadEnd
        OnLoadEnd()
    End Sub

    Private Sub Task_LoadProgress(taskDescription As String, percentComplete As Single) Handles m_Validator.ProgressUpdate
        RaiseEvent ValidationProgress(taskDescription, CDbl(percentComplete / 100))
    End Sub

    'Private Sub Encryption_Progress(taskMsg As String, fractionDone As Double)
    '    RaiseEvent LoadStart()
    'End Sub

    Private Sub OnNormalizedFASTAGeneration(newFASTAFilePath As String) Handles m_Validator.WroteLineEndNormalizedFASTA
        m_NormalizedFASTAFilePath = newFASTAFilePath
        RaiseEvent WroteLineEndNormalizedFASTA(newFASTAFilePath)
    End Sub

    Private Sub OnFASTAFileWarnings(fastaFilePath As String, warningCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))
        RaiseEvent FASTAFileWarnings(fastaFilePath, warningCollection)
    End Sub

    Private Sub OnInvalidFASTAFile(fastaFilePath As String, errorCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended))
        RaiseEvent InvalidFASTAFile(fastaFilePath, errorCollection)
    End Sub

    Private Sub OnValidFASTAFileUpload(fastaFilePath As String, uploadData As IUploadProteins.UploadInfo)
        RaiseEvent ValidFASTAFileLoaded(fastaFilePath, uploadData)
    End Sub


    Public Sub New(psConnectionString As String)
        m_DatabaseAccessor = New clsDBTask(psConnectionString, True)

        ' Reserve space for tracking up to 10 validation updates (expand later if needed)
        ReDim mValidationOptions(10)

        m_Upload = New clsAddUpdateEntries(psConnectionString)

        m_Export = New clsGetFASTAFromDMS(psConnectionString,
                                          IGetFASTAFromDMS.DatabaseFormatTypes.fasta,
                                          IGetFASTAFromDMS.SequenceTypes.forward)

        m_Validator = New clsCustomValidateFastaFiles

        m_Importer = New clsImportHandler(m_DatabaseAccessor.ConnectionString)
    End Sub

    'fileInfoList hash -> key =
    Protected Sub ProteinBatchLoadCoordinator(
        fileInfoList As IEnumerable(Of IUploadProteins.UploadInfo)) Implements IUploadProteins.BatchUpload

        Dim upInfo As IUploadProteins.UploadInfo
        Dim fi As FileInfo
        Dim tmpPS As IProteinStorage
        Dim tmpFileName As String
        Dim blnFileValidated As Boolean
        Dim collectionState As String
        Dim collectionID As Integer
        Dim mboxText As String
        Dim mboxHeader As String
        Dim dboxResult As DialogResult
        Dim errorText As String
        Dim errorLabel As String
        Dim errorCollection As List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)

        Dim databaseAccessor = New clsDBTask(m_DatabaseAccessor.ConnectionString, True)

        For Each upInfo In fileInfoList
            'upInfo.OriginalFileInformation = upInfo.FileInformation
            fi = upInfo.FileInformation

            ' Configure the validator to possibly allow asterisks in the residues
            m_Validator.OptionSwitch(clsValidateFastaFile.SwitchOptions.AllowAllSymbolsInProteinNames) = mValidationOptions(IUploadProteins.eValidationOptionConstants.AllowAllSymbolsInProteinNames)

            ' Configure the validator to possibly allow asterisks in the residues
            m_Validator.OptionSwitch(clsValidateFastaFile.SwitchOptions.AllowAsteriskInResidues) = mValidationOptions(IUploadProteins.eValidationOptionConstants.AllowAsterisksInResidues)

            ' Configure the validator to possibly allow dashes in the residues
            m_Validator.OptionSwitch(clsValidateFastaFile.SwitchOptions.AllowDashInResidues) = mValidationOptions(IUploadProteins.eValidationOptionConstants.AllowDashInResidues)

            ' Configure the additional validation options
            m_Validator.SetValidationOptions(clsCustomValidateFastaFiles.eValidationOptionConstants.AllowAllSymbolsInProteinNames,
                                                mValidationOptions(IUploadProteins.eValidationOptionConstants.AllowAllSymbolsInProteinNames))

            m_Validator.SetValidationOptions(clsCustomValidateFastaFiles.eValidationOptionConstants.AllowAsterisksInResidues,
                                                mValidationOptions(IUploadProteins.eValidationOptionConstants.AllowAsterisksInResidues))

            m_Validator.SetValidationOptions(clsCustomValidateFastaFiles.eValidationOptionConstants.AllowDashInResidues,
                                                mValidationOptions(IUploadProteins.eValidationOptionConstants.AllowDashInResidues))

            ' Update the default rules (important if AllowAsteriskInResidues = True or AllowDashInResidues = True)
            m_Validator.SetDefaultRules()

            ' Update the maximum protein name length
            If mMaximumProteinNameLength <= 0 Then
                mMaximumProteinNameLength = clsValidateFastaFile.DEFAULT_MAXIMUM_PROTEIN_NAME_LENGTH
            End If

            m_Validator.MaximumProteinNameLength = mMaximumProteinNameLength

            OnLoadStart("Validating fasta files")

            ' Validate the fasta file (send full path)
            ' This function returns True if the file is successfully processed (even if it has errors)
            blnFileValidated = m_Validator.StartValidateFASTAFile(fi.FullName)

            OnLoadEnd()

            If Not blnFileValidated Then
                Console.WriteLine("--------------------------------------------------------------")
                Console.WriteLine("Warning: Skipping protein collection due to validation error")
                Console.WriteLine(fi.FullName)
                Console.WriteLine("--------------------------------------------------------------")
                Continue For
            End If

            If m_Validator.FASTAFileHasWarnings(fi.Name) Then
                ' If any warnings were cached, return them with the OnFASTAFileWarnings event
                OnFASTAFileWarnings(fi.FullName, m_Validator.RecordedFASTAFileWarnings(fi.Name))
            End If

            ' Now check whether or not any errors were found for the file
            If Not m_Validator.FASTAFileValid(fi.Name) Then
                ' Errors were found; return the error collection with the InvalidFASTAFile event
                OnInvalidFASTAFile(fi.FullName, m_Validator.RecordedFASTAFileErrors(fi.Name))

                Console.WriteLine("--------------------------------------------------------------")
                Console.WriteLine("Warning: Skipping protein collection because validation failed")
                Console.WriteLine(fi.FullName)
                Console.WriteLine("--------------------------------------------------------------")
                Continue For
            End If

            OnBatchProgressUpdate("Loading: " & fi.Name)
            If Not m_NormalizedFASTAFilePath Is Nothing Then
                If fi.FullName <> m_NormalizedFASTAFilePath Then
                    upInfo.FileInformation = New FileInfo(m_NormalizedFASTAFilePath)
                    'tmpFileName = System.IO.Path.GetFileNameWithoutExtension(m_NormalizedFASTAFilePath)
                Else
                End If
            End If
            tmpFileName = Path.GetFileNameWithoutExtension(fi.FullName)

            collectionID = m_Upload.GetProteinCollectionID(tmpFileName)
            If collectionID > 0 Then
                collectionState = m_Upload.GetProteinCollectionState(collectionID)

                If collectionState = "New" Or collectionState = "Provisional" Then
                    mboxText = "The Collection '" & tmpFileName & "' has been declared '" &
                               collectionState & "'. Are you sure you want to replace its contents?"
                    mboxHeader = "Confirm Replacement"
                    errorText = "Collection was in State '" & collectionState & "' and was not changed"
                    errorLabel = "Warning"

                    dboxResult = MessageBox.Show(
                        mboxText,
                        mboxHeader,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Stop,
                        MessageBoxDefaultButton.Button2)
                Else
                    errorText = "Collections in State '" & collectionState & "' cannot be changed or deleted"
                    errorLabel = "Error"
                    dboxResult = DialogResult.No
                End If

                If dboxResult = DialogResult.No Then
                    errorCollection = New List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)
                    errorCollection.Add(New clsCustomValidateFastaFiles.udtErrorInfoExtended(
                        0, " N/A ", errorText, "", errorLabel))
                    OnInvalidFASTAFile(tmpFileName, errorCollection)

                End If
            Else
                dboxResult = DialogResult.Yes
            End If

            If dboxResult = DialogResult.Yes Then
                tmpPS = m_Importer.LoadProteinsForBatch(upInfo.FileInformation.FullName, upInfo.OrganismID, upInfo.AnnotationTypeID)
                If Not tmpPS Is Nothing Then
                    If tmpPS.ProteinCount = 0 Then
                        ' No proteins

                        errorCollection = New List(Of clsCustomValidateFastaFiles.udtErrorInfoExtended)
                        errorCollection.Add(New clsCustomValidateFastaFiles.udtErrorInfoExtended(
                            0, " N/A ", "No valid proteins were loaded from the .Fasta file", "", "Error"))

                        OnInvalidFASTAFile(upInfo.FileInformation.FullName, errorCollection)
                    Else

                        If upInfo.EncryptSequences AndAlso Not String.IsNullOrEmpty(upInfo.EncryptionPassphrase) Then
                            m_Encryptor = New clsCollectionEncryptor(upInfo.EncryptionPassphrase, databaseAccessor)
                            m_Encryptor.EncryptStorageCollectionSequences(tmpPS)
                            tmpPS.EncryptSequences = True
                            tmpPS.PassPhrase = upInfo.EncryptionPassphrase
                        End If

                        upInfo.ProteinCount = tmpPS.ProteinCount
                        CollectionBatchUploadCoordinator(tmpPS, tmpFileName, upInfo.OrganismID, upInfo.AnnotationTypeID, upInfo.Description, upInfo.Source)
                        'upInfo.ExportedProteinCount = m_Export.ExportedProteinCount
                        OnValidFASTAFileUpload(upInfo.FileInformation.FullName, upInfo)
                        tmpPS.ClearProteinEntries()

                    End If
                Else
                    OnInvalidFASTAFile(upInfo.FileInformation.FullName, m_Validator.RecordedFASTAFileErrors(fi.FullName))
                End If
            End If

        Next

        m_Importer.TriggerProteinCollectionTableUpdate()

    End Sub

    Protected Function CollectionUploadCoordinator(
        fileContents As IProteinStorage,
        selectedProteins As List(Of String),
        filepath As String,
        description As String,
        collectionSource As String,
        collectionType As IAddUpdateEntries.CollectionTypes,
        organismID As Integer,
        annotationTypeID As Integer) As Integer Implements IUploadProteins.UploadCollection

        Dim XrefID As Integer

        'task 2a - Get Protein_Collection_ID or make a new one
        Dim collectionID = m_Upload.GetProteinCollectionID(filepath)

        Dim collectionState = m_Upload.GetProteinCollectionState(collectionID)

        If collectionState <> "Unknown" And
           collectionState <> "New" And
           collectionState <> "Provisional" Then
            Throw New Exception("Protein collections in state " & collectionState & " cannot be updated")
        End If

        Dim numProteins = selectedProteins.Count
        Dim numResidues = m_Upload.GetTotalResidueCount(fileContents, selectedProteins)

        If collectionID = 0 Then

            ' Note that we're storing 0 for NumResidues at this time
            ' That value will be updated later after all of the proteins have been added
            collectionID = m_Upload.MakeNewProteinCollection(
                              Path.GetFileNameWithoutExtension(filepath), description,
                              collectionSource, collectionType, annotationTypeID, numProteins, 0)

            If collectionID = 0 Then
                ' Error making the new protein collection
            End If

        Else
            ' Make sure there are no proteins defined for this protein collection
            ' In addition, this will update NumResidues to be 0
            m_Upload.DeleteProteinCollectionMembers(collectionID, numProteins)
        End If

        'task 2b - Compare file to existing sequences and upload new sequences to T_Proteins
        m_Upload.CompareProteinID(fileContents, selectedProteins)

        'task 3 - Add Protein References to T_Protein_Names
        m_Upload.UpdateProteinNames(fileContents, selectedProteins, organismID, annotationTypeID)

        'task 4 - Add new collection members to T_Protein_Collection_Members
        m_Upload.UpdateProteinCollectionMembers(collectionID, fileContents, selectedProteins, numProteins, numResidues)

        OnLoadStart("Associating protein collection with organism using T_Collection_Organism_Xref")
        XrefID = m_Upload.AddCollectionOrganismXref(collectionID, organismID)
        OnLoadEnd()

        If XrefID < 1 Then
            'Throw New Exception("Could not add Collection/Organism Xref")
            MsgBox("Could not add Collection/Organism Xref; m_Upload.AddCollectionOrganismXref returned " & XrefID)
        End If


        'task 5 - Update encryption metadata (if applicable)
        If fileContents.EncryptSequences Then
            OnLoadStart("Storing encryption metadata")
            m_Upload.UpdateEncryptionMetadata(collectionID, fileContents.PassPhrase)
            OnLoadEnd()
        End If

        Dim tmpFileName As String = Path.GetTempPath

        'Dim tmpFi As System.IO.FileInfo = New System.IO.FileInfo(tmpFileName)

        Dim fingerprint As String
        OnLoadStart("Generating Hash fingerprint")
        fingerprint = m_Export.ExportFASTAFile(collectionID, tmpFileName, IGetFASTAFromDMS.DatabaseFormatTypes.fasta, IGetFASTAFromDMS.SequenceTypes.forward)
        OnLoadEnd()

        OnLoadStart("Storing fingerprint in T_Protein_Collections")
        m_Upload.AddAuthenticationHash(collectionID, fingerprint, numProteins, numResidues)
        OnLoadEnd()

        'TODO add in hash return
        Return 0
    End Function

    Protected Function CollectionBatchUploadCoordinator(
        fileContents As IProteinStorage,
        filepath As String,
        organismID As Integer,
        annotationTypeID As Integer,
        description As String,
        source As String) As Integer Implements IUploadProteins.UploadCollection

        Dim selectedList As New List(Of String)

        Dim counter As Dictionary(Of String, IProteinStorageEntry).Enumerator = fileContents.GetEnumerator

        While counter.MoveNext()
            selectedList.Add(counter.Current.Value.Reference)
        End While

        selectedList.Sort()

        Return CollectionUploadCoordinator(
            fileContents, selectedList, filepath, description, source,
            IAddUpdateEntries.CollectionTypes.prot_original_source, organismID, annotationTypeID)

    End Function

    Public Sub SetValidationOptions(eValidationOptionName As IUploadProteins.eValidationOptionConstants, blnEnabled As Boolean) Implements IUploadProteins.SetValidationOptions
        mValidationOptions(eValidationOptionName) = blnEnabled
    End Sub

    Private Sub m_Export_FileGenerationCompleted(fullOutputPath As String) Handles m_Export.FileGenerationCompleted

    End Sub

    Private Sub m_Export_FileGenerationProgress(statusMsg As String, fractionDone As Double) Handles m_Export.FileGenerationProgress
        OnProgressUpdate(fractionDone)
    End Sub
End Class
