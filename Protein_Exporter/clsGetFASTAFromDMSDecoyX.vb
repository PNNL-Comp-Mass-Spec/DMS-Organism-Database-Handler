
Option Strict On

Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports TableManipulationBase

Public Class clsGetFASTAFromDMSDecoyX
    Inherits clsGetFASTAFromDMSDecoy

    Private Const DECOY_PROTEINS_USE_XXX As Boolean = True

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
    ''' <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
    Public Sub New(
        databaseAccessor As IGetSQLData,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(databaseAccessor, databaseFormatType, DECOY_PROTEINS_USE_XXX)

        m_RevGenerator = New clsGetFASTAFromDMSReversed(
            databaseAccessor, databaseFormatType) With {
            .UseXXX = DECOY_PROTEINS_USE_XXX
            }

    End Sub


End Class
