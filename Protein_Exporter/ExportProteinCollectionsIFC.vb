Option Strict On

Imports System.Collections.Generic
Imports Protein_Storage

Namespace ExportProteinCollectionsIFC

    Public Interface IExportProteins

        Event ExportStart(taskTitle As String)
        Event ExportProgress(statusMsg As String, fractionDone As Double)
        Event ExportEnd()

        ''' <summary>
        ''' Export the proteins to the given file
        ''' </summary>
        ''' <param name="proteins"></param>
        ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
        ''' <param name="selectedProteinList"></param>
        ''' <returns></returns>
        Function Export(
            proteins As IProteinStorage,
            ByRef destinationPath As String,
            selectedProteinList As List(Of String)) As String

        ''' <summary>
        ''' Export the proteins to the given file
        ''' </summary>
        ''' <param name="proteins"></param>
        ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
        ''' <returns></returns>
        Function Export(
            proteins As IProteinStorage,
            ByRef destinationPath As String) As String

        ''' <summary>
        ''' Export the proteins to the given file
        ''' </summary>
        ''' <param name="proteinTables"></param>
        ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
        ''' <returns></returns>
        Function Export(
            proteinTables As DataSet,
            ByRef destinationPath As String) As String

        ''' <summary>
        ''' Export the proteins to the given file
        ''' </summary>
        ''' <param name="proteinTable"></param>
        ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
        ''' <returns></returns>
        Function Export(
            proteinTable As DataTable,
            ByRef destinationPath As String) As String

        Function GenerateFileAuthenticationHash(fullFilePath As String) As String

    End Interface

    Public Interface IGetFASTAFromDMS

        Function GetOrganismList() As Hashtable
        Function GetCollectionsByOrganism(organismID As Integer) As Hashtable
        Function GetAllCollections() As Hashtable

        Function GetOrganismListTable() As DataTable
        Function GetCollectionsByOrganismTable(organismID As Integer) As DataTable

        Function ExportFASTAFile(
            proteinCollectionID As Integer,
            destinationFolderPath As String,
            databaseFormatType As DatabaseFormatTypes,
            outputSequenceType As SequenceTypes) As String

        Function ExportFASTAFile(
            protCollectionList As String,
            creationOptions As String,
            legacyFASTAFileName As String,
            destinationFolderPath As String) As String

        Function GenerateFileAuthenticationHash(fullFilePath As String) As String
        Function GetStoredFileAuthenticationHash(proteinCollectionName As String) As String
        Function GetStoredFileAuthenticationHash(proteinCollectionID As Integer) As String
        Function GetProteinCollectionID(proteinCollectionName As String) As Integer

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
