Public Interface IArchiveOutputFiles

    Enum CollectionTypes
        [static] = 1
        dynamic = 2
    End Enum

    ReadOnly Property LastErrorMessage() As String
    ReadOnly Property Archived_File_Name() As String

    Event ArchiveStart()
    Event SubTaskStart(ByVal TaskDescription As String)
    Event SubTaskProgressUpdate(ByVal fractionDone As Double)
    Event OverallProgressUpdate(ByVal fractionDone As Double)
    Event ArchiveComplete(ByVal ArchivePath As String)

    'Function ArchiveCollection( _
    '    ByVal ProteinCollectionID As Integer, _
    '    ByVal ProteinCollectionType As CollectionTypes, _
    '    ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
    '    ByVal DatabaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
    '    ByVal SourceFilePath As String, _
    '    ByVal CreationOptionsString As String) As Integer

    Function ArchiveCollection( _
        ByVal ProteinCollectionID As Integer, _
        ByVal ProteinCollectionType As CollectionTypes, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal DatabaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal Authentication_Hash As String, _
        ByVal ProteinCollectionList As String) As Integer

    'Function ArchiveCollection( _
    '    ByVal ProteinCollectionName As String, _
    '    ByVal ProteinCollectionType As CollectionTypes, _
    '    ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
    '    ByVal DatabaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
    '    ByVal SourceFilePath As String, _
    '    ByVal CreationOptionsString As String) As Integer

    Function ArchiveCollection( _
        ByVal ProteinCollectionName As String, _
        ByVal ProteinCollectionType As CollectionTypes, _
        ByVal OutputSequenceType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.SequenceTypes, _
        ByVal DatabaseFormatType As Protein_Exporter.ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes, _
        ByVal SourceFilePath As String, _
        ByVal CreationOptionsString As String, _
        ByVal Authentication_Hash As String, _
        ByVal ProteinCollectionList As String) As Integer

    Sub AddArchiveCollectionXRef( _
        ByVal ProteinCollectionID As Integer, _
        ByVal Archived_File_ID As Integer)

    'Function ArchiveCollection( _
    '    ByVal ProteinCollectionID As Integer, _
    '    ByVal ProteinCollectionType As CollectionTypes, _
    '    ByVal SourceFilePath As String) As Integer


    'Function ArchiveNewDynamicCollection( _
    '    ByVal ProteinCollectionIDList As ArrayList, _
    '    ByVal SequenceOutputType As String, _
    '    ByVal SourceFilePath As String) As Integer

    'Function SyncCollectionsAndArchiveTables( _
    '    ByVal OutputPath As String) As Integer

    'Sub UpdateSHA1Hashes()


End Interface
