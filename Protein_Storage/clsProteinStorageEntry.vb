Imports System.Collections.Generic

Public Class clsProteinStorageEntry
    Implements IProteinStorageEntry

    Public Sub New(
        Reference As String,
        Description As String,
        Sequence As String,
        Length As Integer,
        MonoisotopicMass As Double,
        AverageMass As Double,
        MolecularFormula As String,
        AuthenticationHash As String,
        SortingIndex As Integer)

        If String.IsNullOrWhiteSpace(Reference) Then
            Throw New Exception("Reference name cannot be empty")
        End If

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
    Protected m_XRefList As List(Of String)
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
        Set(Value As String)
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
        Set(Value As String)
            Me.m_Sequence = Value
        End Set
    End Property

    Protected Property IsEncrypted() As Boolean Implements IProteinStorageEntry.IsEncrypted
        Get
            Return Me.m_IsEncrypted
        End Get
        Set(Value As Boolean)
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
        Set(value As String)
            Me.m_AuthHash = value
        End Set
    End Property

    Public Property Protein_ID() As Integer Implements IProteinStorageEntry.Protein_ID
        Get
            Return Me.m_Protein_ID
        End Get
        Set(Value As Integer)
            Me.m_Protein_ID = Value
        End Set
    End Property

    Protected Property Reference_ID() As Integer Implements IProteinStorageEntry.Reference_ID
        Get
            Return Me.m_Reference_ID
        End Get
        Set(Value As Integer)
            Me.m_Reference_ID = Value
        End Set
    End Property

    Protected Property Member_ID() As Integer Implements IProteinStorageEntry.Member_ID
        Get
            Return Me.m_Member_ID
        End Get
        Set(Value As Integer)
            Me.m_Member_ID = Value
        End Set
    End Property

    Protected Property Authority_ID() As Integer Implements IProteinStorageEntry.Authority_ID
        Get
            Return Me.m_Authority_ID
        End Get
        Set(Value As Integer)
            Me.m_Authority_ID = Value
        End Set
    End Property

    Protected Property SortingIndex() As Integer Implements IProteinStorageEntry.SortingIndex
        Get
            Return Me.m_SortCount
        End Get
        Set(Value As Integer)
            Me.m_SortCount = Value
        End Set
    End Property

    Protected ReadOnly Property NameXRefs() As List(Of String) Implements IProteinStorageEntry.NameXRefs
        Get
            Return Me.m_XRefList
        End Get
    End Property

    Protected Sub AddXRef(newReference As String) Implements IProteinStorageEntry.AddXRef
        If Me.m_XRefList Is Nothing Then
            Me.m_XRefList = New List(Of String)
        End If
        Me.m_XRefList.Add(newReference)
    End Sub

    Protected Sub ChangeReferenceName(newName As String) Implements IProteinStorageEntry.SetReferenceName
        If String.IsNullOrWhiteSpace(newName) Then
            Throw New Exception("New protein name cannot be empty")
        End If
        Me.m_Reference = newName
    End Sub

    Public Overrides Function ToString() As String

        If String.IsNullOrWhiteSpace(m_Sequence) Then
            Return m_Reference & ", ResidueCount=0"
        Else
            Return m_Reference & ", ResidueCount=" & m_Length & ", " & m_Sequence.Substring(0, 20)
        End If
        
    End Function
End Class
