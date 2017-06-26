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

        Dim fwdHash As String
        fwdHash = MyBase.ExportFASTAFile(ProteinCollectionNameList,
            ExportPath, AlternateAuthorityID, PadWithPrimaryAnnotation)

        fwdFilePath = Me.FullOutputPath

        Dim revHash As String

        revHash = Me.m_RevGenerator.ExportFASTAFile(ProteinCollectionNameList,
            ExportPath, AlternateAuthorityID, PadWithPrimaryAnnotation)

        revFilePath = Me.m_RevGenerator.FullOutputPath

        Dim fwdFI = New System.IO.FileInfo(fwdFilePath)

        Dim appendWriter As System.IO.TextWriter = fwdFI.AppendText

        Dim revFI = New System.IO.FileInfo(revFilePath)

        Dim revReader As System.IO.TextReader = revFI.OpenText

        Dim s As String

        s = revReader.ReadLine
        While Not s Is Nothing
            appendWriter.WriteLine(s)
            s = revReader.ReadLine
        End While

        appendWriter.Flush()
        appendWriter.Close()

        revReader.Close()
        revFI.Delete()

        Dim crc32HashFinal = Me.GetFileHash(fwdFI.FullName)

        Return crc32HashFinal

    End Function




End Class
