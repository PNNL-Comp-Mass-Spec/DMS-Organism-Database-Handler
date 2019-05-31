
Imports System.Collections.Generic

Public Class clsProteinStorage

    ''' <summary>
    ''' Keys are Protein_Name
    ''' </summary>
    Protected ReadOnly m_Proteins As Dictionary(Of String, clsProteinStorageEntry)
    Protected m_ResidueCount As Integer
    Protected ReadOnly m_ProteinNames As SortedSet(Of String)
    Protected m_PassPhrase As String

    Public Sub New(fastaFileName As String)
        FileName = fastaFileName
        m_Proteins = New Dictionary(Of String, clsProteinStorageEntry)
        m_ProteinNames = New SortedSet(Of String)
    End Sub

    Public Overridable Sub AddProtein(proteinEntry As clsProteinStorageEntry)

        If Not m_Proteins.ContainsKey(proteinEntry.Reference) Then
            m_Proteins.Add(proteinEntry.Reference, proteinEntry)
            m_ProteinNames.Add(proteinEntry.Reference)
            m_ResidueCount += proteinEntry.Sequence.Length
        Else
            proteinEntry.SetReferenceName(proteinEntry.Reference + "_dup_" + proteinEntry.SHA1Hash.Substring(1, 10))
            AddProtein(proteinEntry)
            'flag with some kinda error so we can check out the duplicate entry and rename it
        End If
    End Sub

    Protected Property FileName As String

    Public Function GetProtein(reference As String) As clsProteinStorageEntry

        Dim proteinEntry As clsProteinStorageEntry = Nothing

        If m_Proteins.TryGetValue(reference, proteinEntry) Then
            Return proteinEntry
        Else
            Return Nothing
        End If
    End Function

    Public Function GetSortedProteinNames() As SortedSet(Of String)
        Return m_ProteinNames
    End Function

    Public Overridable Sub ClearProteinEntries()
        m_ResidueCount = 0
        m_Proteins.Clear()
        m_ProteinNames.Clear()
    End Sub

    Public ReadOnly Property TotalResidueCount As Integer
        Get
            Return m_ResidueCount
        End Get
    End Property

    Public ReadOnly Property ProteinCount As Integer
        Get
            Return m_Proteins.Count()
        End Get
    End Property

    Public Property EncryptSequences As Boolean

    Public Property PassPhrase As String
        Get
            If EncryptSequences Then
                Return m_PassPhrase
            Else
                Return Nothing
            End If
        End Get
        Set
            m_PassPhrase = Value
        End Set
    End Property

    Public Function GetEnumerator() As Dictionary(Of String, clsProteinStorageEntry).Enumerator
        Return m_Proteins.GetEnumerator
    End Function

    Public Overrides Function ToString() As String
        Return FileName & ": " & m_ProteinNames.Count() & " proteins"
    End Function

End Class

