Option Strict On

Public Class clsGetFASTAFromDMSReversed
    Inherits clsGetFASTAFromDMSForward

    Private m_RndNumGen As Random
    Protected m_UseXXX As Boolean

    Public Sub New(
        ProteinStorageConnectionString As String,
        DatabaseFormatType As ExportProteinCollectionsIFC.IGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(ProteinStorageConnectionString, DatabaseFormatType)
        Me.m_Naming_Suffix = "_reversed"
    End Sub

    Public Property UseXXX() As Boolean
        Get
            Return m_UseXXX
        End Get
        Set(value As Boolean)
            m_UseXXX = True
        End Set
    End Property

    Overrides Function SequenceExtender(originalSequence As String, collectionCount As Integer) As String

        Return StrReverse(originalSequence)

    End Function

    Overrides Function ReferenceExtender(originalReference As String) As String
        If m_UseXXX Then
            Return "XXX." + originalReference
        Else
            Return "Reversed_" + originalReference
        End If

    End Function



End Class
