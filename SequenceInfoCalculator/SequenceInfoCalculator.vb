Option Strict On

Imports System.Security.Cryptography
Imports System.Text
Imports TableManipulationBase

Public Class SequenceInfoCalculator

    Friend Shared m_AminoAcids As Hashtable

    Private m_MonoIsotopicMass As Double
    Private m_AverageMass As Double
    Private m_Length As Integer
    Private m_MolFormula As String
    Private m_SHA1Hash As String

    Shared m_SHA1Provider As SHA1Managed

    Private ReadOnly m_DMSConnectionString As String = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;"

    Public Sub New()
        If m_AminoAcids Is Nothing Then
            InitializeFromDMS()
        End If

        If m_SHA1Provider Is Nothing Then
            m_SHA1Provider = New SHA1Managed()
        End If
    End Sub


#Region " Ken's Added Properties "

    Public ReadOnly Property MonoIsotopicMass As Double
        Get
            Return m_MonoIsotopicMass
        End Get
    End Property

    Public ReadOnly Property AverageMass As Double
        Get
            Return m_AverageMass
        End Get
    End Property

    Public ReadOnly Property SequenceLength As Integer
        Get
            Return m_Length
        End Get
    End Property

    Public ReadOnly Property MolecularFormula As String
        Get
            Return m_MolFormula
        End Get
    End Property

    Public ReadOnly Property SHA1Hash As String
        Get
            Return m_SHA1Hash
        End Get
    End Property


#End Region

    Public Sub CalculateSequenceInfo(Sequence As String)
        Dim tmpSeqInfo As SequenceInfo
        tmpSeqInfo = SequenceInfo(Sequence)
        m_MonoIsotopicMass = tmpSeqInfo.MonoisotopicMass
        m_AverageMass = tmpSeqInfo.AverageMass
        m_MolFormula = tmpSeqInfo.MolecularFormula
        m_Length = Sequence.Length
        m_SHA1Hash = GenerateHash(Sequence)

    End Sub

    Protected Function SequenceInfo(sequence As String, Optional description As String = "") As SequenceInfo
        Dim result = New SequenceInfo("", description, 0, 0, 0, 0, 0, 0.0, 0.0)

        Dim aaInfo As AminoAcidInfo
        Dim aaString() As Char
        Dim aa As Char
        aaString = sequence.ToCharArray

        For Each aa In aaString
            aaInfo = DirectCast(m_AminoAcids.Item(aa.ToString), AminoAcidInfo)
            If (aaInfo) Is Nothing Then
                result.AddSequenceInfo(New SequenceInfo(aaString, "Not Found, adding input", 0, 0, 0, 0, 0, 0, 0))
            Else
                result.AddSequenceInfo(aaInfo)
            End If
        Next

        Return result
    End Function

    Public Function GenerateHash(SourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New ASCIIEncoding()

        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)

        'Compute the hash value from the source
        Dim SHA1_hash() As Byte = m_SHA1Provider.ComputeHash(ByteSourceText)

        'And convert it to String format for return
        Dim SHA1string As String = ToHexString(SHA1_hash)

        Return SHA1string
    End Function

    Private Sub InitializeFromDMS()
        m_AminoAcids = New Hashtable(30)
        Dim getSQL As clsDBTask

        Dim tmpSLC As String
        Dim tmpDesc As String
        Dim tmpNumC As Integer
        Dim tmpNumH As Integer
        Dim tmpNumO As Integer
        Dim tmpNumN As Integer
        Dim tmpNumS As Integer
        Dim tmpMM As Double
        Dim tmpAM As Double

        getSQL = New clsDBTask(m_DMSConnectionString)

        Dim sqlString = "SELECT * FROM T_Residues WHERE [Num_C] > 0"
        Dim tmpAATable As DataTable = getSQL.GetTable(sqlString)

        Dim dr As DataRow

        For Each dr In tmpAATable.Rows
            tmpSLC = CType(dr.Item("Residue_Symbol"), String)
            tmpDesc = CType(dr.Item("Description"), String)
            tmpNumC = CInt(dr.Item("Num_C"))
            tmpNumH = CInt(dr.Item("Num_H"))
            tmpNumO = CInt(dr.Item("Num_O"))
            tmpNumN = CInt(dr.Item("Num_N"))
            tmpNumS = CInt(dr.Item("Num_S"))
            tmpMM = CDbl(dr.Item("Monoisotopic_Mass"))
            tmpAM = CDbl(dr.Item("Average_Mass"))

            AddAminoAcid(New AminoAcidInfo(tmpSLC, tmpDesc, tmpNumC, tmpNumH, tmpNumN, tmpNumO, tmpNumS, tmpAM, tmpMM))
        Next


    End Sub

    Private Sub AddAminoAcid(aa As AminoAcidInfo)
        m_AminoAcids.Add(aa.Symbol, aa)
    End Sub

    Private Class AminoAcidInfo
        Inherits SequenceInfo

        Public Sub New(seq As String, name As String,
                    C_Count As Integer, H_Count As Integer, N_Count As Integer, O_Count As Integer, S_Count As Integer,
                    average As Double, monoisotopic As Double)
            MyBase.New(seq, name, C_Count, H_Count, N_Count, O_Count, S_Count, average, monoisotopic)
            If (seq.Length) <> 1 Then
                Throw New ApplicationException("'" & seq & "' is not a valid amino acid.  Must be only one character long.")
            End If
        End Sub

        Public ReadOnly Property Symbol As String
            Get
                Return Sequence
            End Get
        End Property

    End Class

    Public Shared Function ToHexString(bytes() As Byte) As String

        Dim sb As New StringBuilder

        For Each b In bytes
            sb.Append(String.Format("{0:X2}", b))
        Next

        Return sb.ToString()

    End Function

End Class


Public Class SequenceInfo
    Private m_invalidated As Boolean = False
    Private m_sequence As String
    Private m_C_Count As Integer
    Private m_H_Count As Integer
    Private m_N_Count As Integer
    Private m_O_Count As Integer
    Private m_S_Count As Integer
    Private m_Average_Mass As Double
    Private m_Monoisotopic_Mass As Double

    Public Sub New(seq As String, seqName As String,
                    C_Count As Integer, H_Count As Integer, N_Count As Integer, O_Count As Integer, S_Count As Integer,
                    average As Double, monoisotopic As Double)

        m_sequence = seq
        Name = seqName
        m_C_Count = C_Count
        m_H_Count = H_Count
        m_N_Count = N_Count
        m_O_Count = O_Count
        m_S_Count = S_Count
        m_Average_Mass = average
        m_Monoisotopic_Mass = monoisotopic

    End Sub

    Public ReadOnly Property Sequence As String
        Get
            Return m_sequence
        End Get
    End Property

    Public Sub Invalidate()
        m_invalidated = True
        m_Average_Mass = 0
        m_C_Count = 0
        m_H_Count = 0
        m_Monoisotopic_Mass = 0
        m_N_Count = 0
        m_O_Count = 0
        m_S_Count = 0
    End Sub

    Public ReadOnly Property Invalidated As Boolean
        Get
            Return m_invalidated
        End Get
    End Property

    Public ReadOnly Property Name As String

    Public ReadOnly Property C_Count As Integer
        Get
            Return m_C_Count
        End Get
    End Property

    Public ReadOnly Property H_Count As Integer
        Get
            Return m_H_Count
        End Get
    End Property

    Public ReadOnly Property N_Count As Integer
        Get
            Return m_N_Count
        End Get
    End Property

    Public ReadOnly Property O_Count As Integer
        Get
            Return m_O_Count
        End Get
    End Property

    Public ReadOnly Property S_Count As Integer
        Get
            Return m_S_Count
        End Get
    End Property

    Public ReadOnly Property AverageMass As Double
        Get
            Return m_Average_Mass + 18.01528
        End Get
    End Property

    Public ReadOnly Property MonoisotopicMass As Double
        Get
            Return m_Monoisotopic_Mass + 18.0105633
        End Get
    End Property

    Public ReadOnly Property MolecularFormula As String
        Get
            Return GetMolecularFormula()
        End Get
    End Property

    Private Function GetMolecularFormula() As String
        Dim mf As String = "C" & m_C_Count & " H" & m_H_Count & " N" & m_N_Count & " O" & m_O_Count & " S" & m_S_Count
        Return mf
    End Function

    Public Sub AddSequenceInfo(info As SequenceInfo)

        If m_sequence.Length = 0 Then
            m_H_Count = 2
            m_O_Count = 1
        End If
        m_sequence = m_sequence & info.Sequence
        If Not (m_invalidated) Then
            m_C_Count = m_C_Count + info.C_Count
            m_H_Count = m_H_Count + info.H_Count
            m_N_Count = m_N_Count + info.N_Count
            m_O_Count = m_O_Count + info.O_Count
            m_S_Count = m_S_Count + info.S_Count
            m_Monoisotopic_Mass = m_Monoisotopic_Mass + info.m_Monoisotopic_Mass
            m_Average_Mass = m_Average_Mass + info.m_Average_Mass
        End If
    End Sub
End Class
