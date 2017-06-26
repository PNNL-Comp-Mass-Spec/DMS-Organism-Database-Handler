Option Strict On

Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Interface IArchiveOutputFiles

    Enum CollectionTypes
        [static] = 1
        dynamic = 2
    End Enum

    ReadOnly Property LastErrorMessage() As String
    ReadOnly Property Archived_File_Name() As String

    Event ArchiveStart()
    Event SubTaskStart(TaskDescription As String)
    Event SubTaskProgressUpdate(fractionDone As Double)
    Event OverallProgressUpdate(fractionDone As Double)
    Event ArchiveComplete(ArchivePath As String)

    Function ArchiveCollection(
        ProteinCollectionID As Integer,
        ProteinCollectionType As CollectionTypes,
        OutputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        DatabaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes,
        SourceFilePath As String,
        CreationOptionsString As String,
        Authentication_Hash As String,
        ProteinCollectionList As String) As Integer

    Function ArchiveCollection(
        ProteinCollectionName As String,
        ProteinCollectionType As CollectionTypes,
        OutputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        DatabaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes,
        SourceFilePath As String,
        CreationOptionsString As String,
        Authentication_Hash As String,
        ProteinCollectionList As String) As Integer

    Sub AddArchiveCollectionXRef(
        ProteinCollectionID As Integer,
        Archived_File_ID As Integer)

End Interface
