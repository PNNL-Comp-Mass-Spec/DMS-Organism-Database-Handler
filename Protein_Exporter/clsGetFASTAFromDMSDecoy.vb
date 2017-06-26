Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsGetFASTAFromDMSDecoy
    Inherits clsGetFASTAFromDMSForward

    Private m_RndNumGen As Random
    'Private m_FwdGenerator As clsGetFASTAFromDMSForward
    Protected m_RevGenerator As clsGetFASTAFromDMSReversed

    Public Sub New(
        ProteinStorageConnectionString As String,
        DatabaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(ProteinStorageConnectionString, DatabaseFormatType)
        Me.m_RevGenerator = New clsGetFASTAFromDMSReversed(
            ProteinStorageConnectionString, DatabaseFormatType)
        'Me.m_Naming_Suffix = "_reversed"
    End Sub

    ''' <summary>
    ''' Create the decoy FASTA file for the given protein collections
    ''' </summary>
    ''' <param name="protCollectionList">Protein collection list, or empty string if retrieving a legacy FASTA file</param>
    ''' <param name="destinationFolderPath"></param>
    ''' <returns>CRC32 hash of the generated (or retrieved) file</returns>
    Overloads Overrides Function ExportFASTAFile(
        protCollectionList As List(Of String),
        destinationFolderPath As String,
        AlternateAuthorityID As Integer,
        PadWithPrimaryAnnotation As Boolean) As String

        Dim fwdFilePath As String
        Dim revFilePath As String

        MyBase.ExportFASTAFile(protCollectionList,
                               destinationFolderPath, AlternateAuthorityID, PadWithPrimaryAnnotation)

        fwdFilePath = Me.FullOutputPath

        Me.m_RevGenerator.ExportFASTAFile(protCollectionList,
                                          destinationFolderPath, AlternateAuthorityID, PadWithPrimaryAnnotation)

        revFilePath = Me.m_RevGenerator.FullOutputPath

        Dim fwdFI = New FileInfo(fwdFilePath)
        Dim revFI = New FileInfo(revFilePath)

        Using reverseReader = New StreamReader(New FileStream(revFI.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            Using appender = New StreamWriter(New FileStream(fwdFI.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))

                While (Not reverseReader.EndOfStream)
                    Dim dataLine = reverseReader.ReadLine()
                    appender.WriteLine(dataLine)
                End While
            End Using
        End Using

        revFI.Delete()

        Dim crc32HashFinal = Me.GetFileHash(fwdFI.FullName)

        Return crc32HashFinal

    End Function




End Class
