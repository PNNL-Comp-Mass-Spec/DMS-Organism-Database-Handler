
Namespace ExportProteinCollectionsIFC

    Public Interface IExportProteins

        Event ExportStart(ByVal taskTitle As String)
        Event ExportProgress(ByVal statusMsg As String, ByVal fractionDone As Double)
        Event ExportEnd()

        Function Export( _
            ByRef Proteins As Protein_Storage.IProteinStorage, _
            ByRef destinationPath As String, ByVal SelectedProteinList As ArrayList) As String

        Function Export( _
            ByRef Proteins As Protein_Storage.IProteinStorage, _
            ByRef destinationPath As String) As String

        Function Export( _
            ByRef ProteinTables As DataSet, _
            ByRef destinationPath As String) As String

        Function GenerateFileAuthenticationHash(ByVal FullFilePath As String) As String

    End Interface

    Public Interface IGetFASTAFromDMS

        Function GetOrganismList() As Hashtable
        Function GetCollectionsByOrganism(ByVal OrganismID As Integer) As Hashtable
        Function GetAllCollections() As Hashtable

        Function GetOrganismListTable() As DataTable
        Function GetCollectionsByOrganismTable(ByVal OrganismID As Integer) As DataTable

        Function ExportFASTAFile( _
            ByVal ProteinCollectionID As Integer, _
            ByVal ExportPath As String, _
            ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
            ByVal OutputSequenceType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes) As String

        Function ExportFASTAFile( _
            ByVal ProteinCollectionNameList As String, _
            ByVal CreationOptions As String, _
            ByVal LegacyFASTAFileName As String, _
            ByVal ExportPath As String) As String

        Function GenerateFileAuthenticationHash(ByVal FullFilePath As String) As String
        Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionName As String) As String
        Function GetStoredFileAuthenticationHash(ByVal ProteinCollectionID As Integer) As String
        Function GetProteinCollectionID(ByVal ProteinCollectionName As String) As Integer

        Event FileGenerationStarted(ByVal taskMsg As String)
        Event FileGenerationProgress(ByVal statusMsg As String, ByVal fractionDone As Double)
        Event FileGenerationCompleted(ByVal FullOutputPath As String)

        Enum SequenceTypes
            forward = 1
            reversed = 2
            scrambled = 3
        End Enum

        Enum DatabaseFormatTypes
            fasta
            fastapro
        End Enum

    End Interface


End Namespace
