Option Strict On

Public Interface ICalculateSeqInfo
    Sub CalculateSequenceInfo(Sequence As String)
    Function GenerateSequenceHash(Sequence As String) As String

    ReadOnly Property MonoIsotopicMass As Double
    ReadOnly Property AverageMass As Double
    ReadOnly Property SequenceLength As Integer
    ReadOnly Property MolecularFormula As String
    ReadOnly Property SHA1Hash As String

End Interface


Public Class SequenceInfoCalculator
    Implements ICalculateSeqInfo
    Friend Shared m_AminoAcids As Hashtable

    Private m_MonoIsotopicMass As Double
    Private m_AverageMass As Double
    Private m_Length As Integer
    Private m_MolFormula As String
    Private m_SHA1Hash As String

    Shared m_SHA1Provider As System.Security.Cryptography.SHA1Managed

    Private m_DMSConnectionString As String = "Data Source=gigasax;Initial Catalog=DMS5;Integrated Security=SSPI;"

    Public Sub New()
        If m_AminoAcids Is Nothing Then
            Me.InitializeFromDMS()
        End If

        If m_SHA1Provider Is Nothing Then
            m_SHA1Provider = New System.Security.Cryptography.SHA1Managed
        End If
    End Sub


#Region " Ken's Added Properties "

    Public ReadOnly Property MonoIsotopicMass As Double Implements ICalculateSeqInfo.MonoIsotopicMass
        Get
            Return Me.m_MonoIsotopicMass
        End Get
    End Property

    Public ReadOnly Property AverageMass As Double Implements ICalculateSeqInfo.AverageMass
        Get
            Return Me.m_AverageMass
        End Get
    End Property

    Public ReadOnly Property SequenceLength As Integer Implements ICalculateSeqInfo.SequenceLength
        Get
            Return Me.m_Length
        End Get
    End Property

    Public ReadOnly Property MolecularFormula As String Implements ICalculateSeqInfo.MolecularFormula
        Get
            Return Me.m_MolFormula
        End Get
    End Property

    Public ReadOnly Property SHA1Hash As String Implements ICalculateSeqInfo.SHA1Hash
        Get
            Return Me.m_SHA1Hash
        End Get
    End Property


#End Region

    Public Sub CalculateSequenceInfo(Sequence As String) Implements ICalculateSeqInfo.CalculateSequenceInfo
        Dim tmpSeqInfo As SequenceInfo
        tmpSeqInfo = SequenceInfo(Sequence)
        Me.m_MonoIsotopicMass = tmpSeqInfo.MonoisotopicMass
        Me.m_AverageMass = tmpSeqInfo.AverageMass
        Me.m_MolFormula = tmpSeqInfo.MolecularFormula
        Me.m_Length = Sequence.Length
        Me.m_SHA1Hash = Me.GenerateHash(Sequence)

    End Sub

    Protected Function SequenceInfo(sequence As String, Optional description As String = "") As SequenceInfo
        Dim aminoAcids As Char() = sequence.ToCharArray()
        Dim i As Integer = 0
        Dim result As SequenceInfo = New SequenceInfo("", description, 0, 0, 0, 0, 0, 0.0, 0.0)

        Dim aaInfo As AminoAcidInfo
        Dim aaString() As Char
        Dim aa As Char
        aaString = sequence.ToCharArray

        For Each aa In aaString
            'For i = 0 To aminoAcids.Length - 1
            'Dim aaString As String = New String(aminoAcids, i, 1)
            'Dim aaInfo As AminoAcidInfo = DirectCast(m_AminoAcids.Item(aaString), AminoAcidInfo)
            aaInfo = DirectCast(SequenceInfoCalculator.m_AminoAcids.Item(aa.ToString), AminoAcidInfo)
            If (aaInfo) Is Nothing Then
                'result.Invalidate()
                result.AddSequenceInfo(New SequenceInfo(aaString, "Not Found, adding input", 0, 0, 0, 0, 0, 0, 0))
            Else
                result.AddSequenceInfo(aaInfo)
            End If
        Next

        Return result
    End Function

    Protected Function GenerateHash(SourceText As String) As String Implements ICalculateSeqInfo.GenerateSequenceHash
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New System.Text.ASCIIEncoding
        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)
        'Compute the hash value from the source
        Dim SHA1_hash() As Byte = SequenceInfoCalculator.m_SHA1Provider.ComputeHash(ByteSourceText)
        'And convert it to String format for return
        Dim SHA1string As String = HexConverter.ToHexString(SHA1_hash)

        Return SHA1string
    End Function

    Private Sub InitializeFromDMS()
        SequenceInfoCalculator.m_AminoAcids = New Hashtable(30)
        Dim getSQL As TableManipulationBase.IGetSQLData

        Dim tmpSLC As String
        Dim tmpDesc As String
        Dim tmpNumC As Integer
        Dim tmpNumH As Integer
        Dim tmpNumO As Integer
        Dim tmpNumN As Integer
        Dim tmpNumS As Integer
        Dim tmpMM As Double
        Dim tmpAM As Double

        getSQL = New TableManipulationBase.clsDBTask(Me.m_DMSConnectionString)

        Dim sqlString As String = "SELECT * FROM T_Residues WHERE [Num_C] > 0"
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
                Return Me.Sequence
            End Get
        End Property

        Public ReadOnly Property Abbreviation As String
            Get
                Return Me.Name
            End Get
        End Property

    End Class
    Class HexConverter

        Private Shared hexDigits As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c}

        Public Shared Function ToHexString(bytes() As Byte) As String

            Dim hexStr As String = ""
            Dim i As Integer

            Dim sb As New System.Text.StringBuilder

            For i = 0 To bytes.Length - 1

                sb.Append(bytes(i).ToString("X").PadLeft(2, "0"c))

            Next

            hexStr = sb.ToString
            Return hexStr.ToUpper

        End Function 'ToHexString
    End Class 'HexConverter



End Class




Public Class SequenceInfo
    Private m_invalidated As Boolean = False
    Private m_sequence As String
    Private ReadOnly m_name As String
    Private m_C_Count As Integer
    Private m_H_Count As Integer
    Private m_N_Count As Integer
    Private m_O_Count As Integer
    Private m_S_Count As Integer
    Private m_Average_Mass As Double
    Private m_Monoisotopic_Mass As Double

    Shared ResidueCheck As System.Text.RegularExpressions.Regex

    Public Sub New(seq As String, name As String,
                    C_Count As Integer, H_Count As Integer, N_Count As Integer, O_Count As Integer, S_Count As Integer,
                    average As Double, monoisotopic As Double)
        m_sequence = seq
        m_name = name
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
        Me.m_invalidated = True
        Me.m_Average_Mass = 0
        Me.m_C_Count = 0
        Me.m_H_Count = 0
        Me.m_Monoisotopic_Mass = 0
        Me.m_N_Count = 0
        Me.m_O_Count = 0
        Me.m_S_Count = 0
    End Sub

    Public ReadOnly Property Invalidated As Boolean
        Get
            Return Me.m_invalidated
        End Get
    End Property

    Public ReadOnly Property Name As String
        Get
            Return m_name
        End Get
    End Property

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
            Return Me.m_Average_Mass + 18.01528
        End Get
    End Property

    Public ReadOnly Property MonoisotopicMass As Double
        Get
            Return Me.m_Monoisotopic_Mass + 18.0105633
        End Get
    End Property

    Public ReadOnly Property MolecularFormula As String
        Get
            Return GetMolecularFormula()
        End Get
    End Property

    Private Function GetMolecularFormula() As String
        Dim mf As String = "C" & Me.m_C_Count & " H" & Me.m_H_Count & " N" & Me.m_N_Count & " O" & Me.m_O_Count & " S" & Me.m_S_Count
        Return mf
    End Function

    Public Sub AddSequenceInfo(info As SequenceInfo)

        If Me.m_sequence.Length = 0 Then
            Me.m_H_Count = 2
            Me.m_O_Count = 1
        End If
        Me.m_sequence = Me.m_sequence & info.Sequence
        If Not (Me.m_invalidated) Then
            Me.m_C_Count = Me.m_C_Count + info.C_Count
            Me.m_H_Count = Me.m_H_Count + info.H_Count
            Me.m_N_Count = Me.m_N_Count + info.N_Count
            Me.m_O_Count = Me.m_O_Count + info.O_Count
            Me.m_S_Count = Me.m_S_Count + info.S_Count
            Me.m_Monoisotopic_Mass = Me.m_Monoisotopic_Mass + info.m_Monoisotopic_Mass
            Me.m_Average_Mass = Me.m_Average_Mass + info.m_Average_Mass
        End If
    End Sub
End Class
