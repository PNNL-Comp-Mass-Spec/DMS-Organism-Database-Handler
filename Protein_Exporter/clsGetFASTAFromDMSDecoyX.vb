
Option Strict On

Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsGetFASTAFromDMSDecoyX
    Inherits clsGetFASTAFromDMSDecoy

    Public Sub New(
        dbConnectionString As String,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(dbConnectionString, databaseFormatType)

        m_RevGenerator = New clsGetFASTAFromDMSReversed(
            dbConnectionString, databaseFormatType)

        m_RevGenerator.UseXXX = True

    End Sub


End Class
