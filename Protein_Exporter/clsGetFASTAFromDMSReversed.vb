Public Class clsGetFASTAFromDMSReversed
    Inherits clsGetFASTAFromDMSForward

    Private m_RndNumGen As Random

    Public Sub New( _
        ByVal ProteinStorageConnectionString As String, _
        ByVal DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(ProteinStorageConnectionString, DatabaseFormatType)
        Me.m_Naming_Suffix = "_reversed"
    End Sub


    Overrides Function SequenceExtender(ByVal originalSequence As String, ByVal collectionCount As Integer) As String

        Return StrReverse(originalSequence)

    End Function

    Overrides Function ReferenceExtender(ByVal originalReference As String) As String
        Return "Reversed_" + originalReference
    End Function



End Class
