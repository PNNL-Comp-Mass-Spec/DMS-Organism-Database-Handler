Imports System.Collections.Generic

Public Class clsProteinStorageEntry

    Public Sub New(
        reference As String,
        description As String,
        sequence As String,
        length As Integer,
        monoisotopicMass As Double,
        averageMass As Double,
        molecularFormula As String,
        authenticationHash As String,
        sortingIndex As Integer)

        If String.IsNullOrWhiteSpace(reference) Then
            Throw New Exception("Reference name cannot be empty")
        End If

        m_Reference = reference
        m_Description = description
        m_Sequence = sequence
        m_MonoMass = monoisotopicMass
        m_AvgMass = averageMass
        m_Length = length
        m_MolecularFormula = molecularFormula
        m_AuthHash = authenticationHash
        m_SortCount = sortingIndex

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

    Public ReadOnly Property Reference As String
        Get
            Return m_Reference
        End Get
    End Property

    Protected Property AlternateReference As String
        Get
            Return m_AlternateReference
        End Get
        Set
            If Value.Length > 0 Then
                m_AlternateReference = Value
            Else
                m_AlternateReference = Nothing
            End If
        End Set
    End Property

    Public ReadOnly Property HasAlternateReference As Boolean
        Get
            Return Not m_AlternateReference Is Nothing
        End Get
    End Property

    Public ReadOnly Property Description As String
        Get
            Return m_Description
        End Get
    End Property

    Public Property Sequence As String
        Get
            Return m_Sequence
        End Get
        Set
            m_Sequence = Value
        End Set
    End Property

    Public Property IsEncrypted As Boolean
        Get
            Return m_IsEncrypted
        End Get
        Set
            m_IsEncrypted = Value
        End Set
    End Property

    Public ReadOnly Property MonoisotopicMass As Double
        Get
            Return m_MonoMass
        End Get
    End Property

    Public ReadOnly Property AverageMass As Double
        Get
            Return m_AvgMass
        End Get
    End Property

    Public ReadOnly Property Length As Integer
        Get
            Return m_Length
        End Get
    End Property

    Public ReadOnly Property MolecularFormula As String
        Get
            Return m_MolecularFormula
        End Get
    End Property

    Public Property SHA1Hash As String
        Get
            Return m_AuthHash
        End Get
        Set
            m_AuthHash = Value
        End Set
    End Property

    Public Property Protein_ID As Integer
        Get
            Return m_Protein_ID
        End Get
        Set
            m_Protein_ID = Value
        End Set
    End Property

    Public Property Reference_ID As Integer
        Get
            Return m_Reference_ID
        End Get
        Set
            m_Reference_ID = Value
        End Set
    End Property

    Public Property Member_ID As Integer
        Get
            Return m_Member_ID
        End Get
        Set
            m_Member_ID = Value
        End Set
    End Property

    Public Property Authority_ID As Integer
        Get
            Return m_Authority_ID
        End Get
        Set
            m_Authority_ID = Value
        End Set
    End Property

    Public Property SortingIndex As Integer
        Get
            Return m_SortCount
        End Get
        Set
            m_SortCount = Value
        End Set
    End Property

    Public ReadOnly Property NameXRefs As List(Of String)
        Get
            Return m_XRefList
        End Get
    End Property

    Public Sub AddXRef(newReference As String)
        If m_XRefList Is Nothing Then
            m_XRefList = New List(Of String)
        End If
        m_XRefList.Add(newReference)
    End Sub

    Public Sub SetReferenceName(newName As String)
        If String.IsNullOrWhiteSpace(newName) Then
            Throw New Exception("New protein name cannot be empty")
        End If
        m_Reference = newName
    End Sub

    Public Overrides Function ToString() As String

        If String.IsNullOrWhiteSpace(m_Sequence) Then
            Return m_Reference & ", ResidueCount=0"
        Else
            Return m_Reference & ", ResidueCount=" & m_Length & ", " & m_Sequence.Substring(0, 20)
        End If

    End Function
End Class
