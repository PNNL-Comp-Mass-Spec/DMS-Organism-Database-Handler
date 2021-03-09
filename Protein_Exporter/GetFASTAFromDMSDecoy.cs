Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports TableManipulationBase

Public Class GetFASTAFromDMSDecoy
    Inherits GetFASTAFromDMSForward

    Protected m_RevGenerator As GetFASTAFromDMSReversed

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
    ''' <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
    Public Sub New(
        databaseAccessor As DBTask,
        databaseFormatType As GetFASTAFromDMS.DatabaseFormatTypes,
        decoyUsesXXX As Boolean)

        MyBase.New(databaseAccessor, databaseFormatType)
        m_RevGenerator = New GetFASTAFromDMSReversed(
            databaseAccessor, databaseFormatType) With {
            .UseXXX = decoyUsesXXX
            }
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
        alternateAnnotationTypeID As Integer,
        padWithPrimaryAnnotation As Boolean) As String

        Dim fwdFilePath As String
        Dim revFilePath As String

        MyBase.ExportFASTAFile(protCollectionList,
                               destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation)

        fwdFilePath = FullOutputPath

        m_RevGenerator.ExportFASTAFile(protCollectionList,
                                       destinationFolderPath, alternateAnnotationTypeID, padWithPrimaryAnnotation)

        revFilePath = m_RevGenerator.FullOutputPath

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

        Dim crc32HashFinal = GetFileHash(fwdFI.FullName)

        Return crc32HashFinal

    End Function




End Class
