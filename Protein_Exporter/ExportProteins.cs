Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports Protein_Storage

Public MustInherit Class ExportProteins

    Protected m_ExportComponent As GetFASTAFromDMSForward

    Public Sub New(exportComponent As GetFASTAFromDMSForward)
        m_ExportComponent = exportComponent
    End Sub

    Public Event ExportStart(taskTitle As String)
    Public Event ExportProgress(statusMsg As String, fractionDone As Double)
    Public Event ExportEnd()

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <param name="selectedProteinList"></param>
    ''' <returns></returns>
    Protected Function Export(
      proteins As ProteinStorage,
      ByRef destinationPath As String,
      selectedProteinList As List(Of String)) As String

        Dim tmpProteinsList As ProteinStorage

        tmpProteinsList = New ProteinStorage(Path.GetFileNameWithoutExtension(destinationPath))

        For Each reference In selectedProteinList
            tmpProteinsList.AddProtein(proteins.GetProtein(reference))
        Next

        Return Export(tmpProteinsList, destinationPath)

    End Function

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Public MustOverride Function Export(
      proteins As ProteinStorage,
      ByRef destinationPath As String) As String

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteinTables"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Public MustOverride Function Export(
      proteinTables As DataSet,
      ByRef destinationPath As String) As String

    ''' <summary>
    ''' Export the proteins to the given file
    ''' </summary>
    ''' <param name="proteinTable"></param>
    ''' <param name="destinationPath">Destination file path; will get updated with the final path</param>
    ''' <returns></returns>
    Public MustOverride Function Export(
      proteinTable As DataTable,
      ByRef destinationPath As String) As String

    Protected Sub OnExportStart(taskTitle As String)
        RaiseEvent ExportStart(taskTitle)
    End Sub

    Protected Sub OnProgressUpdate(statusMsg As String, fractionDone As Double)
        RaiseEvent ExportProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportEnd()
        RaiseEvent ExportEnd()
    End Sub

    ' Unused
    'Public Function GetHashCRC32(fullFilePath As String) As String
    '    Return GetFileHash(fullFilePath)
    'End Function

    ' Unused
    'Public Function GetHashMD5(fullFilePath As String) As String
    '    Return GetFileHashMD5(fullFilePath)
    'End Function

    ''' <summary>
    ''' Compute the CRC32 hash for the file
    ''' </summary>
    ''' <param name="fullFilePath"></param>
    ''' <returns>File hash</returns>
    Protected Function GetFileHash(fullFilePath As String) As String
        Return GenerateFileAuthenticationHash(fullFilePath)
    End Function

    ''' <summary>
    ''' Compute the CRC32 hash for the file
    ''' </summary>
    ''' <param name="fullFilePath"></param>
    ''' <returns>File hash</returns>
    Public Function GenerateFileAuthenticationHash(fullFilePath As String) As String

        Dim fi As New FileInfo(fullFilePath)

        If Not fi.Exists Then Return String.Empty

        Using f = fi.OpenRead()

            Dim crc = PRISM.Crc32.Crc(f)

            Dim crcString As String = String.Format("{0:X8}", crc)

            Return crcString

        End Using

    End Function

    '' <summary>
    '' Compute the MD5 hash for the file
    '' </summary>
    '' <param name="fullFilePath"></param>
    '' <returns>File hash</returns>
    'Protected Function GetFileHashMD5(fullFilePath As String) As String

    '    Dim md5Gen = New MD5CryptoServiceProvider

    '    Dim fi As New FileInfo(fullFilePath)

    '    If Not fi.Exists Then Return String.Empty

    '    Using f = fi.OpenRead()

    '        Dim tmpHash = md5Gen.ComputeHash(f)
    '        Dim md5String As String = RijndaelEncryptionHandler.ToHexString(tmpHash)
    '        Return md5String

    '    End Using

    'End Function

End Class
