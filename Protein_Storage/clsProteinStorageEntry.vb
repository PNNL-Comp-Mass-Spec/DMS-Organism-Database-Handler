Imports System.Security.Cryptography

Public Class clsProteinStorageEntry
    Implements IProteinStorageEntry

    Public Sub New( _
        ByVal Reference As String, _
        ByVal Description As String, _
        ByVal Sequence As String, _
        ByVal Length As Integer, _
        ByVal MonoisotopicMass As Double, _
        ByVal AverageMass As Double, _
        ByVal MolecularFormula As String, _
        ByVal AuthenticationHash As String, _
        ByVal SortingIndex As Integer)

        Me.m_Reference = Reference
        Me.m_Description = Description
        Me.m_Sequence = Sequence
        Me.m_MonoMass = MonoisotopicMass
        Me.m_AvgMass = AverageMass
        Me.m_Length = Length
        Me.m_MolecularFormula = MolecularFormula
        Me.m_AuthHash = AuthenticationHash
        Me.m_SortCount = SortingIndex

        m_Protein_ID = 0

    End Sub

    'Public Sub New( _
    '    ByVal Reference As String, _
    '    ByVal Description As String, _
    '    ByVal Sequence As String)

    '    m_Reference = Reference
    '    m_Description = Description
    '    m_Sequence = Sequence
    '    m_MonoMass = 0
    '    m_AvgMass = 0
    '    m_Length = 0
    '    m_MolecularFormula = ""
    '    m_AuthHash = ""

    '    m_Protein_ID = 0

    'End Sub

    'Public Sub New( _
    '    ByVal Reference As String, _
    '    ByVal Description As String, _
    '    ByVal Sequence As String, _
    '    ByVal ProteinID As Integer)

    '    m_Reference = Reference
    '    m_Description = Description
    '    m_Sequence = Sequence
    '    m_MonoMass = 0
    '    m_AvgMass = 0
    '    m_Length = 0
    '    m_MolecularFormula = ""
    '    m_AuthHash = ""

    '    m_Protein_ID = ProteinID

    'End Sub

    Protected m_Reference As String
    Protected m_AlternateReference As String
    Protected m_Description As String
    Protected m_Sequence As String

    Protected m_MonoMass As Double
    Protected m_AvgMass As Double
    Protected m_Length As Integer
    Protected m_MolecularFormula As String
    Protected m_AuthHash As String
    Protected m_Protein_ID As Integer
    Protected m_Reference_ID As Integer
    Protected m_Member_ID As Integer
    Protected m_Authority_ID As Integer
    Protected m_XRefList As ArrayList
    Protected m_SortCount As Integer

    Protected m_IsEncrypted As Boolean = False

    Protected ReadOnly Property Reference() As String Implements IProteinStorageEntry.Reference
        Get
            Return Me.m_Reference
        End Get
    End Property

    Protected Property AlternateReference() As String Implements IProteinStorageEntry.AlternateReference
        Get
            Return Me.m_AlternateReference
        End Get
        Set(ByVal Value As String)
            If Value.Length > 0 Then
                Me.m_AlternateReference = Value
            Else
                Me.m_AlternateReference = Nothing
            End If
        End Set
    End Property

    Protected ReadOnly Property HasAlternateReference() As Boolean Implements IProteinStorageEntry.HasAlternateReferences
        Get
            Return Not Me.m_AlternateReference Is Nothing
        End Get
    End Property

    Protected ReadOnly Property Description() As String Implements IProteinStorageEntry.Description
        Get
            Return Me.m_Description
        End Get
    End Property

    Protected Property Sequence() As String Implements IProteinStorageEntry.Sequence
        Get
            Return Me.m_Sequence
        End Get
        Set(ByVal Value As String)
            Me.m_Sequence = Value
        End Set
    End Property

    Protected Property IsEncrypted() As Boolean Implements IProteinStorageEntry.IsEncrypted
        Get
            Return Me.m_IsEncrypted
        End Get
        Set(ByVal Value As Boolean)
            Me.m_IsEncrypted = Value
        End Set
    End Property

    Protected ReadOnly Property MonoisotopicMass() As Double Implements IProteinStorageEntry.MonoisotopicMass
        Get
            Return Me.m_MonoMass
        End Get
    End Property

    Protected ReadOnly Property AverageMass() As Double Implements IProteinStorageEntry.AverageMass
        Get
            Return Me.m_AvgMass
        End Get
    End Property

    Protected ReadOnly Property Length() As Integer Implements IProteinStorageEntry.Length
        Get
            Return Me.m_Length
        End Get
    End Property

    Protected ReadOnly Property MolecularFormula() As String Implements IProteinStorageEntry.MolecularFormula
        Get
            Return Me.m_MolecularFormula
        End Get
    End Property

    Protected Property SHA1Hash() As String Implements IProteinStorageEntry.SHA1Hash
        Get
            Return Me.m_AuthHash
        End Get
        Set(ByVal value As String)
            Me.m_AuthHash = value
        End Set
    End Property

    Protected Property Protein_ID() As Integer Implements IProteinStorageEntry.Protein_ID
        Get
            Return Me.m_Protein_ID
        End Get
        Set(ByVal Value As Integer)
            Me.m_Protein_ID = Value
        End Set
    End Property

    Protected Property Reference_ID() As Integer Implements IProteinStorageEntry.Reference_ID
        Get
            Return Me.m_Reference_ID
        End Get
        Set(ByVal Value As Integer)
            Me.m_Reference_ID = Value
        End Set
    End Property

    Protected Property Member_ID() As Integer Implements IProteinStorageEntry.Member_ID
        Get
            Return Me.m_Member_ID
        End Get
        Set(ByVal Value As Integer)
            Me.m_Member_ID = Value
        End Set
    End Property

    Protected Property Authority_ID() As Integer Implements IProteinStorageEntry.Authority_ID
        Get
            Return Me.m_Authority_ID
        End Get
        Set(ByVal Value As Integer)
            Me.m_Authority_ID = Value
        End Set
    End Property

    Protected Property SortingIndex() As Integer Implements IProteinStorageEntry.SortingIndex
        Get
            Return Me.m_SortCount
        End Get
        Set(ByVal Value As Integer)
            Me.m_SortCount = Value
        End Set
    End Property

    Protected ReadOnly Property NameXRefs() As ArrayList Implements IProteinStorageEntry.NameXRefs
        Get
            Return Me.m_XRefList
        End Get
    End Property

    Protected Sub AddXRef(ByVal Reference As String) Implements IProteinStorageEntry.AddXRef
        If Me.m_XRefList Is Nothing Then
            Me.m_XRefList = New ArrayList
        End If
        Me.m_XRefList.Add(Reference)
    End Sub

End Class
