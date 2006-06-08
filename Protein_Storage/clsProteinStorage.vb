
Public Class clsProteinStorage
    Inherits Hashtable
    Implements IProteinStorage

    Private m_ResidueCount As Integer
    Private m_Duplicates As New ArrayList
    Private m_FileName As String
    Private m_ProteinNames As ArrayList
    Private m_EncryptionFlag As Boolean
    Private m_PassPhrase As String

    Public Sub New(ByVal FileName As String)
        Me.m_FileName = FileName
    End Sub

    Protected Overloads Sub Add(ByVal proteinEntry As IProteinStorageEntry) Implements IProteinStorage.AddProtein
        If Me.m_ProteinNames Is Nothing Then
            Me.m_ProteinNames = New ArrayList
        End If
        If Not Me.Contains(proteinEntry.Reference) Then
            Me.Add(proteinEntry.Reference, proteinEntry)
            Me.m_ProteinNames.Add(proteinEntry.Reference)
            Me.m_ResidueCount += proteinEntry.Sequence.Length
        Else
            Me.m_Duplicates.Add(proteinEntry.Reference)
            'flag with some kinda error so we can check out the duplicate entry and rename it
        End If
    End Sub

    Protected ReadOnly Property DuplicateEntryCount() As Integer Implements IProteinStorage.DuplicateEntryCount
        Get
            Return Me.m_Duplicates.Count
        End Get
    End Property

    Protected ReadOnly Property DuplicateReferenceList() As ArrayList Implements IProteinStorage.DuplicateReferenceList
        Get
            Return Me.m_Duplicates
        End Get
    End Property

    Protected Property FileName() As String Implements IProteinStorage.FileName
        Get
            Return Me.m_FileName
        End Get
        Set(ByVal Value As String)
            Me.m_FileName = Value
        End Set
    End Property

    Protected Function GetProtein(ByVal Reference As String) As Protein_Storage.IProteinStorageEntry Implements IProteinStorage.GetProtein
        If Me.Contains(Reference) Then
            Return DirectCast(Me.Item(Reference), Protein_Storage.clsProteinStorageEntry)
        Else
            Return Nothing
        End If
    End Function

    Protected Function SortedProteinNameList() As ArrayList Implements IProteinStorage.GetSortedProteinNames
        Me.m_ProteinNames.Sort()
        Return Me.m_ProteinNames
    End Function



    Protected Sub ClearProteins() Implements IProteinStorage.ClearProteinEntries
        Me.m_ResidueCount = 0
        Me.Clear()
    End Sub

    Protected ReadOnly Property TotalResidueCount() As Integer Implements IProteinStorage.TotalResidueCount
        Get
            Return Me.m_ResidueCount
        End Get
    End Property

    Protected ReadOnly Property ProteinCount() As Integer Implements IProteinStorage.ProteinCount
        Get
            Return Me.Count
        End Get
    End Property

    Protected Property EncryptSequences() As Boolean Implements IProteinStorage.EncryptSequences
        Get
            Return Me.m_EncryptionFlag
        End Get
        Set(ByVal Value As Boolean)
            Me.m_EncryptionFlag = Value
        End Set
    End Property

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

    Protected Function m_GetEnumerator() As IDictionaryEnumerator Implements IProteinStorage.GetEnumerator
        Return Me.GetEnumerator
    End Function

    Private Class ProteinStorageEntryComparer
        Implements IComparer


        Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
            Dim Protein_1 As Protein_Storage.IProteinStorageEntry = DirectCast(x, Protein_Storage.IProteinStorageEntry)
            Dim Protein_2 As Protein_Storage.IProteinStorageEntry = DirectCast(y, Protein_Storage.IProteinStorageEntry)

            Dim Reference_1 As String = Protein_1.Reference
            Dim Reference_2 As String = Protein_2.Reference

            If Reference_1 > Reference_2 Then
                Return 1
            ElseIf Reference_1 < Reference_2 Then
                Return -1
            Else
                Return 0
            End If
        End Function

    End Class


End Class

