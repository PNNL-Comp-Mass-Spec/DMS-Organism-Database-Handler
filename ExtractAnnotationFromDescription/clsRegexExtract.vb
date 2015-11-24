Imports System.Text.RegularExpressions

Public Class clsRegexExtract
    Protected m_DatabaseHelper As TableManipulationBase.IGetSQLData
    Protected m_Importer As Protein_Importer.IImportProteins
    Protected m_Proteins As Protein_Storage.IProteinStorage

    Sub New(
        ConnectionString As String,
        CollectionID As Integer)

        m_DatabaseHelper = New TableManipulationBase.clsDBTask(ConnectionString)
        m_Importer = New Protein_Importer.clsImportHandler(ConnectionString)

        'm_Importer.

    End Sub

End Class
