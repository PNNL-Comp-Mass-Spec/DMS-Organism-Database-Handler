Public Interface ICalculatePeptides

End Interface

Public Class clsCalculatePeptides
    Implements ICalculatePeptides

    Private m_ORF_ID As Integer
    'Private m_SeqInfoCalc As SequenceInfoCalculator.ICalculateSeqInfo


    Public Sub New(ByVal ORFDataset As DataSet, ByVal PeptideTableName As String)
        'Me.m_SeqInfoCalc = New SequenceInfoCalculator.SequenceInfoCalculator

    End Sub



#Region " Peptide Info Calc Procedures "

    Private Function GeneratePeptideInfo(ByVal ProteinSequence As String) As DataTable
        'Wrapper function to handle generating of all peptide info for a protein
        Dim m_StartLocation As Integer
        Dim m_StopLocation As Integer
        Dim m_Length As Integer
        Dim m_ORF_ID As Integer
        Dim m_MonoMass As Single
        Dim m_AvgMass As Single
        Dim m_Sequence As String
        Dim m_StartRes As String
        Dim m_EndRes As String



    End Function

#End Region

End Class
