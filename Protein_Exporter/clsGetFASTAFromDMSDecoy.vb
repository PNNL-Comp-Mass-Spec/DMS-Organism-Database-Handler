Option Strict On

Public Class clsGetFASTAFromDMSDecoy
    Inherits clsGetFASTAFromDMSForward

    Private m_RndNumGen As Random
    'Private m_FwdGenerator As clsGetFASTAFromDMSForward
    Protected m_RevGenerator As clsGetFASTAFromDMSReversed

    Public Sub New( _
        ByVal ProteinStorageConnectionString As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(ProteinStorageConnectionString, DatabaseFormatType)
        Me.m_RevGenerator = New clsGetFASTAFromDMSReversed( _
            ProteinStorageConnectionString, DatabaseFormatType)
        'Me.m_Naming_Suffix = "_reversed"
    End Sub

    Overloads Overrides Function ExportFASTAFile( _
        ByVal ProteinCollectionNameList As ArrayList, _
        ByVal ExportPath As String, _
        ByVal AlternateAuthorityID As Integer, _
        ByVal PadWithPrimaryAnnotation As Boolean) As String

        Dim fwdFilePath As String
        Dim revFilePath As String

        Dim fwdHash As String
        fwdHash = MyBase.ExportFASTAFile(ProteinCollectionNameList, _
            ExportPath, AlternateAuthorityID, PadWithPrimaryAnnotation)

        fwdFilePath = Me.FullOutputPath

        Dim revHash As String

        revHash = Me.m_RevGenerator.ExportFASTAFile(ProteinCollectionNameList, _
            ExportPath, AlternateAuthorityID, PadWithPrimaryAnnotation)

        revFilePath = Me.m_RevGenerator.FullOutputPath

        Dim fwdFI As System.IO.FileInfo = New System.IO.FileInfo(fwdFilePath)

        Dim appendWriter As System.IO.TextWriter = fwdFI.AppendText

        Dim revFI As System.IO.FileInfo = New System.IO.FileInfo(revFilePath)

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

        Dim returnHash As String
        returnHash = Me.GetFileHash(fwdFI.FullName)

        Return returnHash

    End Function




End Class
