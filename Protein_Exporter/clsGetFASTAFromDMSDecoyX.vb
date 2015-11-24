
Option Strict On

Public Class clsGetFASTAFromDMSDecoyX
    Inherits clsGetFASTAFromDMSDecoy

    Public Sub New(
        ProteinStorageConnectionString As String,
        DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(ProteinStorageConnectionString, DatabaseFormatType)

        Me.m_RevGenerator = New clsGetFASTAFromDMSReversed(
            ProteinStorageConnectionString, DatabaseFormatType)

        m_RevGenerator.UseXXX = True

    End Sub


End Class
