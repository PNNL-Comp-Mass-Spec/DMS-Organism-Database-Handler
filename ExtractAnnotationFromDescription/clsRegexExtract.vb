Imports System.Text.RegularExpressions

Public Class clsRegexExtract
    Protected m_DatabaseHelper As TableManipulationBase.clsDBTask
    Protected m_Importer As Protein_Importer.clsImportHandler
    Protected m_Proteins As Protein_Storage.clsProteinStorage

    Sub New(
        connectionString As String,
        collectionID As Integer)

        m_DatabaseHelper = New TableManipulationBase.clsDBTask(connectionString)
        m_Importer = New Protein_Importer.clsImportHandler(connectionString)

        'm_Importer.

    End Sub

End Class
