Option Strict On

Imports TableManipulationBase

Public Class GetFASTAFromDMSReversed
    Inherits GetFASTAFromDMSForward

    Protected m_UseXXX As Boolean

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
    ''' <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
    Public Sub New(
        databaseAccessor As DBTask,
        databaseFormatType As GetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(databaseAccessor, databaseFormatType)
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
