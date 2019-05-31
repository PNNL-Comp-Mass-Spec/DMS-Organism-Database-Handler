Option Strict On

Imports System.Text
Imports TableManipulationBase

Public Class clsGetFASTAFromDMSScrambled
    Inherits clsGetFASTAFromDMSForward

    Private m_RndNumGen As Random

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="databaseAccessor">Object for retrieving data from the protein sequences database</param>
    ''' <param name="databaseFormatType">Typically fasta; but also supports fastapro to create .fasta.pro files</param>
    Public Sub New(
        databaseAccessor As clsDBTask,
        databaseFormatType As clsGetFASTAFromDMS.DatabaseFormatTypes)

        MyBase.New(databaseAccessor, databaseFormatType)

    End Sub

    Overrides Function SequenceExtender(originalSequence As String, collectionCount As Integer) As String

        Dim sb As New StringBuilder(originalSequence.Length)
        Dim sequence As String = originalSequence

        Dim index As Integer
        Dim counter As Integer

        If m_RndNumGen Is Nothing Then
            m_RndNumGen = New Random(collectionCount)
            m_Naming_Suffix = "_scrambled_seed_" + collectionCount.ToString
        End If

        counter = sequence.Length

        While counter > 0
            Debug.Assert(counter = sequence.Length)
            index = m_RndNumGen.Next(counter)
            sb.Append(sequence.Substring(index, 1))

            If index > 0 Then
                If index < sequence.Length - 1 Then
                    sequence = sequence.Substring(0, index) & sequence.Substring(index + 1)
                Else
                    sequence = sequence.Substring(0, index)
                End If
            Else
                sequence = sequence.Substring(index + 1)
            End If
            counter -= 1

        End While

        Return sb.ToString

    End Function


    Overrides Function ReferenceExtender(originalReference As String) As String
        Return "Scrambled_" + originalReference
    End Function
End Class
