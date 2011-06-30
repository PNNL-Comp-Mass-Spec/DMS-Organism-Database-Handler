
Option Strict On

Public Class clsGetFASTAFromDMSDecoyX
    Inherits clsGetFASTAFromDMSDecoy

    Public Sub New( _
        ByVal ProteinStorageConnectionString As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(ProteinStorageConnectionString, DatabaseFormatType)

        Me.m_RevGenerator = New clsGetFASTAFromDMSReversed( _
            ProteinStorageConnectionString, DatabaseFormatType)

        m_RevGenerator.UseXXX = True

    End Sub


End Class
