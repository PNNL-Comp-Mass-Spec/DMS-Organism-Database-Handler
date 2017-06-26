Option Strict On

Imports System.Collections.Generic
Imports System.IO
Imports System.Security.Cryptography
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports Protein_Storage

Public MustInherit Class clsExportProteins
    Implements IExportProteins

    Protected m_ExportComponent As clsGetFASTAFromDMSForward

    Public Sub New(ByRef ExportComponent As clsGetFASTAFromDMSForward)
        Me.m_ExportComponent = ExportComponent
    End Sub

    Protected Event ExportStart(taskTitle As String) Implements IExportProteins.ExportStart
    Protected Event ExportProgress(statusMsg As String, fractionDone As Double) Implements IExportProteins.ExportProgress
    Protected Event ExportEnd() Implements IExportProteins.ExportEnd

    Protected Function Export(
      Proteins As IProteinStorage,
      ByRef destinationPath As String,
      SelectedProteinList As List(Of String)) As String Implements IExportProteins.Export

        Dim tmpProteinsList As IProteinStorage

        tmpProteinsList = New clsProteinStorage(Path.GetFileNameWithoutExtension(destinationPath))

        Dim Reference As String

        For Each Reference In SelectedProteinList
            tmpProteinsList.AddProtein(Proteins.GetProtein(Reference))
        Next

        Return Export(tmpProteinsList, destinationPath)

    End Function

    Protected MustOverride Function Export(
      Proteins As IProteinStorage,
      ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected MustOverride Function Export(
      ProteinTables As DataSet,
      ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected MustOverride Function Export(
      ProteinTable As DataTable,
      ByRef destinationPath As String) As String Implements IExportProteins.Export

    Protected Sub OnExportStart(taskTitle As String)
        RaiseEvent ExportStart(taskTitle)
    End Sub

    Protected Sub OnProgressUpdate(statusMsg As String, fractionDone As Double)
        RaiseEvent ExportProgress(statusMsg, fractionDone)
    End Sub

    Protected Sub OnExportEnd()
        RaiseEvent ExportEnd()
    End Sub

    Public Function GetHashCRC32(fullFilePath As String) As String
        Return GetFileHash(fullFilePath)
    End Function

    Public Function GetHashMD5(fullFilePath As String) As String
        Return GetFileHashMD5(fullFilePath)
    End Function

    ''' <summary>
    ''' Compute the CRC32 hash for the file
    ''' </summary>
    ''' <param name="fullFilePath"></param>
    ''' <returns>File hash</returns>
    Protected Function GetFileHash(fullFilePath As String) As String Implements IExportProteins.GenerateFileAuthenticationHash

        Dim crcGen = New CRC32

        Dim fi As New FileInfo(fullFilePath)

        If Not fi.Exists Then Return String.Empty

        Using f = fi.OpenRead()

            Dim crc = crcGen.GetCrc32(f)
            Dim crcString As String = String.Format("{0:X8}", crc)
            Return crcString

        End Using

    End Function

    ''' <summary>
    ''' Compute the MD5 hash for the file
    ''' </summary>
    ''' <param name="fullFilePath"></param>
    ''' <returns>File hash</returns>
    Protected Function GetFileHashMD5(fullFilePath As String) As String

        Dim md5Gen = New MD5CryptoServiceProvider

        Dim fi As New FileInfo(fullFilePath)

        If Not fi.Exists Then Return String.Empty

        Using f = fi.OpenRead()

            Dim tmpHash = md5Gen.ComputeHash(f)
            Dim md5String As String = clsRijndaelEncryptionHandler.ToHexString(tmpHash)
            Return md5String

        End Using

    End Function

End Class
