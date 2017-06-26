Option Strict On

Imports System.Collections.Generic
Imports Protein_Storage

Namespace ExportProteinCollectionsIFC

    Public Interface IExportProteins

        Event ExportStart(taskTitle As String)
        Event ExportProgress(statusMsg As String, fractionDone As Double)
        Event ExportEnd()

        Function Export(
            Proteins As IProteinStorage,
            ByRef destinationPath As String,
            SelectedProteinList As List(Of String)) As String

        Function Export(
            Proteins As IProteinStorage,
            ByRef destinationPath As String) As String

        Function Export(
            ProteinTables As DataSet,
            ByRef destinationPath As String) As String

        Function Export(
            ProteinTable As DataTable,
            ByRef destintationPath As String) As String

        Function GenerateFileAuthenticationHash(FullFilePath As String) As String

    End Interface

    Public Interface IGetFASTAFromDMS

        Function GetOrganismList() As Hashtable
        Function GetCollectionsByOrganism(OrganismID As Integer) As Hashtable
        Function GetAllCollections() As Hashtable

        Function GetOrganismListTable() As DataTable
        Function GetCollectionsByOrganismTable(OrganismID As Integer) As DataTable

        Function ExportFASTAFile(
            ProteinCollectionID As Integer,
            destinationFolderPath As String,
            DatabaseFormatType As DatabaseFormatTypes,
            OutputSequenceType As SequenceTypes) As String

        Function ExportFASTAFile(
            protCollectionList As String,
            CreationOptions As String,
            LegacyFASTAFileName As String,
            destinationFolderPath As String) As String

        Function GenerateFileAuthenticationHash(FullFilePath As String) As String
        Function GetStoredFileAuthenticationHash(ProteinCollectionName As String) As String
        Function GetStoredFileAuthenticationHash(ProteinCollectionID As Integer) As String
        Function GetProteinCollectionID(ProteinCollectionName As String) As Integer

        Event FileGenerationStarted(taskMsg As String)
        Event FileGenerationProgress(statusMsg As String, fractionDone As Double)
        Event FileGenerationCompleted(outputPath As String)

        Enum SequenceTypes
            forward = 1
            reversed = 2
            scrambled = 3
            decoy = 4
            decoyX = 5
        End Enum

        Enum DatabaseFormatTypes
            fasta
            fastapro
        End Enum

    End Interface


End Namespace
