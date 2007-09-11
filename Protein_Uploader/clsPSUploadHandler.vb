Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Interface IUploadProteins
    Function UploadCollection( _
        ByRef fileContents As Protein_Storage.IProteinStorage, _
        ByVal selectedProteins As ArrayList, _
        ByVal CollectionName As String, _
        ByVal description As String, _
        ByVal collectionType As Protein_Importer.IAddUpdateEntries.CollectionTypes, _
        ByVal organismID As Integer, _
        ByVal authorityID As Integer) As Integer

    Function UploadCollection( _
        ByRef fileContents As Protein_Storage.IProteinStorage, _
        ByVal filepath As String, _
        ByVal organismID As Integer, _
        ByVal authorityID As Integer) As Integer

    Sub BatchUpload(ByVal fileInfoList As Hashtable)

    Sub InitialSetup()
    Sub ResetErrorList()

    ReadOnly Property ImportExportCountsMatched() As Boolean

    Structure UploadInfo
        Public Sub New(ByVal FileInformation As System.IO.FileInfo, ByVal OrgID As Integer, ByVal AuthID As Integer)
            Me.FileInformation = FileInformation
            Me.OrganismID = OrgID
            Me.AuthorityID = AuthID
            Me.EncryptionPassphrase = ""
        End Sub
        Public FileInformation As System.IO.FileInfo
        Public OriginalFileInformation As System.IO.FileInfo
        Public OrganismID As Integer
        Public AuthorityID As Integer
        Public ProteinCount As Integer
        Public ErrorList As ArrayList
        Public SummarizedErrors As Hashtable
        Public ExportedProteinCount As Integer
        Public EncryptSequences As Boolean
        Public EncryptionPassphrase As String
    End Structure


    Event LoadStart(ByVal taskTitle As String)
    Event LoadProgress(ByVal fractionDone As Double)
    Event LoadEnd()
    Event BatchProgress(ByVal status As String)
    Event ValidationProgress(ByVal taskTitle As String, ByVal fractionDone As Double)
    Event ValidFASTAFileLoaded(ByVal FASTAFilePath As String, ByVal UploadData As UploadInfo)
    Event InvalidFASTAFile(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList)
    Event FASTAValidationComplete(ByVal FASTAFilePath As String, ByVal UploadInfo As IUploadProteins.UploadInfo)
    Event WroteLineEndNormalizedFASTA(ByVal newFilePath As String)
End Interface


Public Class clsPSUploadHandler
    Implements IUploadProteins

    Protected m_PISConnectionString As String
    Protected m_PersistentTaskNum As Integer
    Protected m_NormalizedFASTAFilePath As String
    Protected m_ProteinCollectionsList As DataTable

    Protected m_SQLAccess As TableManipulationBase.IGetSQLData
    Protected WithEvents m_Importer As Protein_Importer.IImportProteins
    Protected WithEvents m_Upload As Protein_Importer.IAddUpdateEntries
    Protected WithEvents m_Export As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS
    Protected WithEvents m_Validator As ValidateFastaFile.ICustomValidation
    Protected WithEvents m_Archiver As Protein_Exporter.IArchiveOutputFiles


    Protected Event LoadStart(ByVal taskTitle As String) Implements IUploadProteins.LoadStart
    Protected Event LoadProgress(ByVal fractionDone As Double) Implements IUploadProteins.LoadProgress
    Protected Event LoadEnd() Implements IUploadProteins.LoadEnd
    Protected Event BatchProgress(ByVal status As String) Implements IUploadProteins.BatchProgress
    Protected Event ValidationProgress(ByVal taskTitle As String, ByVal fractionDone As Double) Implements IUploadProteins.ValidationProgress
    Protected Event ValidFASTAFileLoaded(ByVal FASTAFilePath As String, ByVal UploadData As IUploadProteins.UploadInfo) Implements IUploadProteins.ValidFASTAFileLoaded
    Protected Event InvalidFASTAFile(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList) Implements IUploadProteins.InvalidFASTAFile
    Protected Event FASTAValidationComplete(ByVal FASTAFilePath As String, ByVal UploadInfo As IUploadProteins.UploadInfo) Implements IUploadProteins.FASTAValidationComplete
    Protected Event WroteLineEndNormalizedFASTA(ByVal newFilePath As String) Implements IUploadProteins.WroteLineEndNormalizedFASTA

    Protected m_ExportedProteinCount As Integer
    Protected m_ImportExportCountsMatched As Boolean = False

    Private WithEvents m_Encryptor As clsCollectionEncryptor


    Protected ReadOnly Property ImportExportCountsMatched() As Boolean Implements IUploadProteins.ImportExportCountsMatched
        Get
            Return Me.m_ImportExportCountsMatched
        End Get
    End Property

    Private Sub ResetErrorList() Implements IUploadProteins.ResetErrorList
        Me.m_Validator.ClearErrorList()
    End Sub

    Private Sub OnLoadStart(ByVal taskTitle As String)
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Private Sub OnProgressUpdate(ByVal fractionDone As Double)
        RaiseEvent LoadProgress(fractionDone)
    End Sub

    Private Sub OnLoadEnd()
        RaiseEvent LoadEnd()
    End Sub

    Private Sub OnBatchProgressUpdate(ByVal status As String)
        RaiseEvent BatchProgress(status)
    End Sub

    Private Sub OnFileValidationComplete(ByVal FASTAFilePath As String, ByVal UploadInfo As IUploadProteins.UploadInfo)
        RaiseEvent FASTAValidationComplete(FASTAFilePath, UploadInfo)
    End Sub

    Private Sub LoadStartHandler(ByVal taskTitle As String) Handles m_Upload.LoadStart, m_Importer.LoadStart
        Me.OnLoadStart(taskTitle)
    End Sub

    Private Sub LoadProgressHandler(ByVal fractionDone As Double) Handles m_Upload.LoadProgress, m_Importer.LoadProgress
        Me.OnProgressUpdate(fractionDone)
    End Sub

    Private Sub LoadEndHandler() Handles m_Upload.LoadEnd, m_Importer.LoadEnd
        Me.OnLoadEnd()
    End Sub

    Private Sub Task_LoadProgress(ByVal taskDescription As String, ByVal percentComplete As Single) Handles m_Validator.ProgressChanged
        RaiseEvent ValidationProgress(taskDescription, CDbl(percentComplete / 100))
    End Sub

    'Private Sub Encryption_Progress(ByVal taskMsg As String, ByVal fractionDone As Double)
    '    RaiseEvent LoadStart()
    'End Sub



    Private Sub OnNormalizedFASTAGeneration(ByVal newFASTAFilePath As String) Handles m_Validator.WroteLineEndNormalizedFASTA
        Me.m_NormalizedFASTAFilePath = newFASTAFilePath
        RaiseEvent WroteLineEndNormalizedFASTA(newFASTAFilePath)
    End Sub

    Private Sub OnInvalidFASTAFile(ByVal FASTAFilePath As String, ByVal errorCollection As ArrayList)
        RaiseEvent InvalidFASTAFile(FASTAFilePath, errorCollection)
    End Sub

    Private Sub OnValidFASTAFileUpload(ByVal FASTAFilePath As String, ByVal UploadData As IUploadProteins.UploadInfo)
        RaiseEvent ValidFASTAFileLoaded(FASTAFilePath, UploadData)
    End Sub


    Public Sub New(ByVal PISConnectionString As String)
        Me.m_PISConnectionString = PISConnectionString
    End Sub

    Protected Overridable Sub SetupUploadModule() Implements IUploadProteins.InitialSetup
        Me.m_SQLAccess = New TableManipulationBase.clsDBTask(Me.m_PISConnectionString, True)
        Me.m_Upload = New Protein_Importer.clsAddUpdateEntries(Me.m_PISConnectionString)
        Me.m_Upload.Setup()
        Me.m_Export = New Protein_Exporter.clsGetFASTAFromDMS(Me.m_PISConnectionString, _
            IGetFASTAFromDMS.DatabaseFormatTypes.fasta, IGetFASTAFromDMS.SequenceTypes.forward)
        Me.m_Validator = New ValidateFastaFile.clsCustomValidateFastaFiles
    End Sub

    Protected Overridable Sub SetupImporterClass()
        Me.m_Importer = New Protein_Importer.clsImportHandler(Me.m_PISConnectionString)
    End Sub

    'fileInfoList hash -> key = 
    Protected Sub ProteinBatchLoadCoordinator( _
        ByVal fileInfoList As Hashtable) Implements IUploadProteins.BatchUpload

        SetupImporterClass()

        Dim fileContents As Protein_Storage.IProteinStorage
        Dim upInfo As IUploadProteins.UploadInfo
        Dim fi As System.io.FileInfo
        Dim tmpPS As Protein_Storage.IProteinStorage
        Dim tmpFileName As String
        Dim fileValid As Boolean
        Dim collectionState As String
        Dim collectionID As Integer
        Dim mboxText As String
        Dim mboxHeader As String
        Dim dboxResult As System.Windows.Forms.DialogResult
        Dim errorText As String
        Dim errorLabel As String
        Dim errorCollection As ArrayList
        Dim exportedProteinCount As Integer

        Dim en As IEnumerator

        For Each upInfo In fileInfoList.Values
            'upInfo.OriginalFileInformation = upInfo.FileInformation
            fi = upInfo.FileInformation

            fileValid = Me.m_Validator.ValidateFASTAFile(fi.FullName)

            If fileValid Then
                Me.OnBatchProgressUpdate("Loading: " & fi.Name)
                If Not Me.m_NormalizedFASTAFilePath Is Nothing Then
                    If fi.FullName <> Me.m_NormalizedFASTAFilePath Then
                        upInfo.FileInformation = New System.IO.FileInfo(Me.m_NormalizedFASTAFilePath)
                        'tmpFileName = System.IO.Path.GetFileNameWithoutExtension(Me.m_NormalizedFASTAFilePath)
                    Else
                    End If
                End If
                tmpFileName = System.IO.Path.GetFileNameWithoutExtension(fi.FullName)

                collectionID = Me.m_Upload.GetProteinCollectionID(tmpFileName)
                If collectionID > 0 Then
                    collectionState = Me.m_Upload.GetProteinCollectionState(collectionID)

                    If collectionState = "New" Or collectionState = "Provisional" Then
                        mboxText = "The Collection '" & tmpFileName & "' has been declared '" & _
                           collectionState & "'. Are you sure you want to replace its contents?"
                        mboxHeader = "Confirm Replacement"
                        errorText = "Collection was in State '" & collectionState & "' and was not changed"
                        errorLabel = "Warning"

                        dboxResult = System.Windows.Forms.MessageBox.Show( _
                          mboxText, _
                          mboxHeader, _
                          Windows.Forms.MessageBoxButtons.YesNo, _
                          Windows.Forms.MessageBoxIcon.Stop, _
                          Windows.Forms.MessageBoxDefaultButton.Button2)
                    Else
                        mboxText = "The Collection '" & tmpFileName & "' has been declared '" & _
                            collectionState & "' and cannot be removed or altered! " & _
                            "This entry will be skipped."
                        mboxHeader = "WARNING!"
                        errorText = "Collections in State '" & collectionState & "' cannot be changed or deleted"
                        errorLabel = "Error"
                        dboxResult = Windows.Forms.DialogResult.No
                    End If

                    If dboxResult = Windows.Forms.DialogResult.No Then
                        errorCollection = New ArrayList
                        errorCollection.Add(New ValidateFastaFile.ICustomValidation.udtErrorInfoExtended( _
                            0, " N/A ", errorText, "", errorLabel))
                        Me.OnInvalidFASTAFile(tmpFileName, errorCollection)

                    End If
                Else
                    dboxResult = Windows.Forms.DialogResult.Yes
                End If

                If dboxResult = Windows.Forms.DialogResult.Yes Then
                    tmpPS = Me.m_Importer.LoadProteinsForBatch(upInfo.FileInformation.FullName, upInfo.OrganismID, upInfo.AuthorityID)
                    If Not tmpPS Is Nothing Then
                        If upInfo.EncryptSequences And upInfo.EncryptionPassphrase.Length > 0 Then
                            Me.m_Encryptor = New clsCollectionEncryptor(upInfo.EncryptionPassphrase, Me.m_PISConnectionString)
                            Me.m_Encryptor.EncryptStorageCollectionSequences(tmpPS)
                            tmpPS.EncryptSequences = True
                            tmpPS.PassPhrase = upInfo.EncryptionPassphrase
                        End If

                        upInfo.ProteinCount = tmpPS.ProteinCount
                        Me.CollectionBatchUploadCoordinator(tmpPS, tmpFileName, upInfo.OrganismID, upInfo.AuthorityID)
                        'upInfo.ExportedProteinCount = Me.m_Export.ExportedProteinCount
                        Me.OnValidFASTAFileUpload(upInfo.FileInformation.FullName, upInfo)
                        tmpPS.ClearProteinEntries()
                    Else
                        Me.OnInvalidFASTAFile(upInfo.FileInformation.FullName, Me.m_Validator.RecordedFASTAFileErrors(fi.FullName))
                    End If
                End If

            End If
        Next

        Me.m_Importer.TriggerProteinCollectionTableUpdate()

    End Sub

    Protected Function CollectionUploadCoordinator( _
        ByRef fileContents As Protein_Storage.IProteinStorage, _
        ByVal selectedProteins As ArrayList, _
        ByVal filepath As String, _
        ByVal description As String, _
        ByVal collectionType As Protein_Importer.IAddUpdateEntries.CollectionTypes, _
        ByVal organismID As Integer, _
        ByVal authorityID As Integer) As Integer Implements IUploadProteins.UploadCollection


        Dim XrefID As Integer

        'task 2a - Get Protein_Collection_ID or make a new one
        Dim collectionID As Integer
        collectionID = Me.m_Upload.GetProteinCollectionID(filepath)

        Dim collectionState As String
        collectionState = Me.m_Upload.GetProteinCollectionState(collectionID)

        Dim memberCount As Integer
        If collectionID > 0 Then
            'collection already exists, check if any members already exist
            memberCount = Me.m_Upload.GetProteinCollectionMemberCount(collectionID)
        Else
            memberCount = 0
        End If


        If collectionID = 0 Then
            Dim totalLength As Integer
            totalLength = Me.m_Upload.GetTotalResidueCount(fileContents, selectedProteins)
            collectionID = Me.m_Upload.MakeNewProteinCollection( _
            System.IO.Path.GetFileNameWithoutExtension(filepath), description, _
            collectionType, authorityID, selectedProteins.Count, totalLength)
        End If

        If memberCount > 0 Then
            Me.m_Upload.DeleteProteinCollectionMembers(collectionID)
        End If

        'task 2b - Compare file to existing sequences and upload new sequences to T_Proteins
        Me.m_Upload.CompareProteinID(fileContents, selectedProteins)

        'task 3 - Add Protein References to T_Protein_Names
        Me.m_Upload.UpdateProteinNames(fileContents, selectedProteins, organismID, authorityID)

        'task 4 - Add new collection members to T_Protein_Collection_Members
        Me.m_Upload.UpdateProteinCollectionMembers(collectionID, fileContents, selectedProteins)

        XrefID = Me.m_Upload.AddCollectionOrganismXref(collectionID, organismID)
        If XrefID < 1 Then
            'Throw New Exception("Could not add Collection/Organism Xref")
            MsgBox("Could not add Collection/Organism Xref")
        End If


        'task 5 - Update encryption metadata (if applicable)
        If fileContents.EncryptSequences Then
            Me.m_Upload.UpdateEncryptionMetadata(collectionID, fileContents.PassPhrase)
        End If


        'fileContents = Me.m_Importer.LoadCollectionMembersByID(collectionID

        'Dim tmpFileName As String = _
        '    System.IO.Path.Combine( _
        '        System.IO.Path.GetTempPath, _
        '        System.IO.Path.GetFileName(filepath))

        Dim tmpFileName As String = System.IO.Path.GetTempPath

        'Dim tmpFi As System.IO.FileInfo = New System.IO.FileInfo(tmpFileName)

        Dim fingerprint As String = Me.m_Export.ExportFASTAFile(collectionID, tmpFileName, IGetFASTAFromDMS.DatabaseFormatTypes.fasta, IGetFASTAFromDMS.SequenceTypes.forward)

        'If Me.m_Export.ExportedProteinCount = fileContents.ProteinCount Then
        '    Me.m_ImportExportCountsMatched = True
        'Else
        '    Me.m_ImportExportCountsMatched = False
        'End If

        Me.m_Upload.AddAuthenticationHash(collectionID, fingerprint)

        'TODO add in hash return
        Return 0
    End Function

    Protected Function CollectionBatchUploadCoordinator( _
        ByRef fileContents As Protein_Storage.IProteinStorage, _
        ByVal filepath As String, _
        ByVal organismID As Integer, _
        ByVal authorityID As Integer) As Integer Implements IUploadProteins.UploadCollection

        Dim selectedList As New ArrayList
        Dim ps As Protein_Storage.IProteinStorageEntry

        Dim counter As IDictionaryEnumerator = fileContents.GetEnumerator

        'For Each ps In fileContents
        While counter.MoveNext = True
            ps = DirectCast(counter.Value, Protein_Storage.IProteinStorageEntry)
            selectedList.Add(ps.Reference)
        End While

        selectedList.Sort()

        Return Me.CollectionUploadCoordinator(fileContents, selectedList, filepath, "", _
        Protein_Importer.IAddUpdateEntries.CollectionTypes.prot_original_source, organismID, authorityID)

    End Function


End Class
