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
        proteinCollectionID As Integer,
        proteinCollectionType As CollectionTypes,
        outputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes,
        sourceFilePath As String,
        creationOptionsString As String,
        authentication_Hash As String,
        proteinCollectionList As String) As Integer

    Function ArchiveCollection(
        proteinCollectionName As String,
        proteinCollectionType As CollectionTypes,
        outputSequenceType As IGetFASTAFromDMS.SequenceTypes,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes,
        sourceFilePath As String,
        creationOptionsString As String,
        authentication_Hash As String,
        proteinCollectionList As String) As Integer

    Sub AddArchiveCollectionXRef(
        proteinCollectionID As Integer,
        archived_File_ID As Integer)

End Interface
