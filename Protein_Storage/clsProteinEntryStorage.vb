Public Class clsProteinStorageEntry

    Public Sub New( _
        ByVal Reference As String, _
        ByVal Description As String, _
        ByVal Sequence As String, _
        ByVal Length As Integer, _
        ByVal MonoisotopicMass As Double, _
        ByVal AverageMass As Double, _
        ByVal MolecularFormula As String, _
        ByVal SHA1Hash As String)

        m_Reference = Reference
        m_Description = Description
        m_Sequence = Sequence
        m_MonoMass = MonoisotopicMass
        m_AvgMass = AverageMass
        m_Length = Length
        m_MolecularFormula = MolecularFormula
        m_SHA1Hash = SHA1Hash

        m_Protein_ID = 0

    End Sub

    Private m_Reference As String
    Private m_Description As String
    Private m_Sequence As String

    Private m_MonoMass As Double
    Private m_AvgMass As Double
    Private m_Length As Integer
    Private m_MolecularFormula As String
    Private m_SHA1Hash As String
    Private m_Protein_ID As Integer
    Private m_Reference_ID As Integer

    Public ReadOnly Property Reference() As String
        Get
            Return Me.m_Reference
        End Get
    End Property

    Public ReadOnly Property Description() As String
        Get
            Return Me.m_Description
        End Get
    End Property

    Public ReadOnly Property Sequence() As String
        Get
            Return Me.m_Sequence
        End Get
    End Property

    Public ReadOnly Property MonoisotopicMass() As Double
        Get
            Return Me.m_MonoMass
        End Get
    End Property

    Public ReadOnly Property AverageMass() As Double
        Get
            Return Me.m_AvgMass
        End Get
    End Property

    Public ReadOnly Property Length() As Integer
        Get
            Return Me.m_Length
        End Get
    End Property

    Public ReadOnly Property MolecularFormula() As String
        Get
            Return Me.m_MolecularFormula
        End Get
    End Property

    Public ReadOnly Property SHA1Hash() As String
        Get
            Return Me.m_SHA1Hash
        End Get
    End Property

    Public Property Protein_ID() As Integer
        Get
            Return Me.m_Protein_ID
        End Get
        Set(ByVal Value As Integer)
            Me.m_Protein_ID = Value
        End Set
    End Property

    Public Property Reference_ID() As Integer
        Get
            Return Me.m_Reference_ID
        End Get
        Set(ByVal Value As Integer)
            Me.m_Reference_ID = Value
        End Set
    End Property

End Class
