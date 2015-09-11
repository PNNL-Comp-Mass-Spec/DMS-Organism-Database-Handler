
Imports System.Collections.Generic

Public Class clsProteinStorage
    Implements IProteinStorage

    Private ReadOnly m_Proteins As Dictionary(Of String, IProteinStorageEntry)
    Private m_ResidueCount As Integer
    Private m_ProteinNames As SortedSet(Of String)
    Private m_PassPhrase As String

    Public Sub New(ByVal fastaFileName As String)
        Me.FileName = fastaFileName
        m_Proteins = New Dictionary(Of String, IProteinStorageEntry)
    End Sub

    Protected Sub AddProtein(ByVal proteinEntry As IProteinStorageEntry) Implements IProteinStorage.AddProtein
        If Me.m_ProteinNames Is Nothing Then
            Me.m_ProteinNames = New SortedSet(Of String)
        End If

        If Not m_Proteins.ContainsKey(proteinEntry.Reference) Then
            m_Proteins.Add(proteinEntry.Reference, proteinEntry)
            Me.m_ProteinNames.Add(proteinEntry.Reference)
            Me.m_ResidueCount += proteinEntry.Sequence.Length
        Else
            proteinEntry.SetReferenceName(proteinEntry.Reference + "_dup_" + proteinEntry.SHA1Hash.Substring(1, 10))
            Me.AddProtein(proteinEntry)
            'flag with some kinda error so we can check out the duplicate entry and rename it
        End If
    End Sub

    Protected Property FileName As String Implements IProteinStorage.FileName

    Protected Function GetProtein(ByVal Reference As String) As Protein_Storage.IProteinStorageEntry Implements IProteinStorage.GetProtein

        Dim proteinEntry As IProteinStorageEntry = Nothing

        If m_Proteins.TryGetValue(Reference, proteinEntry) Then
            Return proteinEntry
        Else
            Return Nothing
        End If
    End Function

    Protected Function SortedProteinNameList() As SortedSet(Of String) Implements IProteinStorage.GetSortedProteinNames
        Return Me.m_ProteinNames
    End Function

    Protected Sub ClearProteins() Implements IProteinStorage.ClearProteinEntries
        m_ResidueCount = 0
        m_Proteins.Clear()
        m_ProteinNames.Clear()
    End Sub

    Protected ReadOnly Property TotalResidueCount() As Integer Implements IProteinStorage.TotalResidueCount
        Get
            Return Me.m_ResidueCount
        End Get
    End Property

    Protected ReadOnly Property ProteinCount() As Integer Implements IProteinStorage.ProteinCount
        Get
            Return m_Proteins.Count()
        End Get
    End Property

    Protected Property EncryptSequences As Boolean Implements IProteinStorage.EncryptSequences
    
    Protected Property PassPhrase() As String Implements IProteinStorage.PassPhrase
        Get
            If EncryptSequences Then
                Return Me.m_PassPhrase
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal Value As String)
            Me.m_PassPhrase = Value
        End Set
    End Property

    Protected Function m_GetEnumerator() As Dictionary(Of String, IProteinStorageEntry).Enumerator Implements IProteinStorage.GetEnumerator
        Return m_Proteins.GetEnumerator
    End Function

    Public Overrides Function ToString() As String
        Return FileName & ": " & m_ProteinNames.Count() & " proteins"
    End Function

End Class

