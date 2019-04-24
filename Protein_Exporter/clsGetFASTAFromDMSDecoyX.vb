
Option Strict On

Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsGetFASTAFromDMSDecoyX
    Inherits clsGetFASTAFromDMSDecoy

    private const DECOY_PROTEINS_USE_XXX as Boolean = True

    Public Sub New(
        dbConnectionString As String,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(dbConnectionString, databaseFormatType, DECOY_PROTEINS_USE_XXX)

        m_RevGenerator = New clsGetFASTAFromDMSReversed(
            dbConnectionString, databaseFormatType) With {
            .UseXXX = DECOY_PROTEINS_USE_XXX
            }

    End Sub


End Class
