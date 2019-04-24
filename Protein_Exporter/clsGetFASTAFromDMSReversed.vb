Option Strict On

Imports Protein_Exporter.ExportProteinCollectionsIFC

Public Class clsGetFASTAFromDMSReversed
    Inherits clsGetFASTAFromDMSForward

    Protected m_UseXXX As Boolean

    Public Sub New(
        dbConnectionString As String,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(dbConnectionString, databaseFormatType)
        m_Naming_Suffix = "_reversed"
    End Sub

    ''' <summary>
    ''' When true, reverse proteins start with XXX_
    ''' When false, they start with Reversed_
    ''' </summary>
    ''' <returns></returns>
    Public Property UseXXX As Boolean
        Get
            Return m_UseXXX
        End Get
        Set
            m_UseXXX = True
        End Set
    End Property

    Overrides Function SequenceExtender(originalSequence As String, collectionCount As Integer) As String

        Return StrReverse(originalSequence)

    End Function

    Overrides Function ReferenceExtender(originalReference As String) As String
        If m_UseXXX Then
            Return "XXX_" + originalReference
        Else
            Return "Reversed_" + originalReference
        End If

    End Function



End Class
