
Public Class clsProteinStorageDMS
    Implements IProteinStorage


    Private m_Proteins As Hashtable             'Protein_ID, clsProteinStorageEntry
    Private m_UniqueProteinIDList As Hashtable  'Protein_ID, Protein_Name
    'Private m_ProteinIDLookup As Hashtable      'Protein_Name, Protein_ID
    Private m_ProteinNameList As ArrayList

    Private m_ResidueCount As Integer
    Private m_Duplicates As New ArrayList
    Private m_FileName As String
    Private m_EncryptionFlag As Boolean
    Private m_PassPhrase As String


    Public Sub New(ByVal FileName As String)
        Me.m_FileName = FileName
    End Sub

    Protected Overloads Sub Add(ByVal proteinEntry As IProteinStorageEntry) Implements IProteinStorage.AddProtein
        Dim nameList As ArrayList
        Dim proteinName As String
        Dim ProteinEntryID As Integer = proteinEntry.Protein_ID
        Dim ProteinEntryName As String = proteinEntry.Reference

        If Me.m_Proteins Is Nothing Then
            Me.m_Proteins = New Hashtable
        End If

        If Me.m_UniqueProteinIDList Is Nothing Then
            Me.m_UniqueProteinIDList = New Hashtable
        End If

        If Me.m_ProteinNameList Is Nothing Then
            Me.m_ProteinNameList = New ArrayList
        End If

        If Not Me.m_UniqueProteinIDList.Contains(ProteinEntryID) Then
            nameList = New ArrayList
            nameList.Add(ProteinEntryName)
            Me.m_UniqueProteinIDList.Add(ProteinEntryID, nameList)
            Me.m_ResidueCount += proteinEntry.Sequence.Length
        Else
            Dim existingEntry As IProteinStorageEntry
            nameList = DirectCast(Me.m_UniqueProteinIDList(ProteinEntryID), ArrayList)
            For Each proteinName In nameList
                existingEntry = DirectCast(Me.m_Proteins.Item(proteinName), clsProteinStorageEntry)
                If Not proteinEntry.Reference.Equals(existingEntry.Reference) Then
                    existingEntry.AddXRef(ProteinEntryName)
                    proteinEntry.AddXRef(existingEntry.Reference)
                    Me.m_Proteins(proteinName) = existingEntry
                End If

                'Me.m_ProteinIDLookup.Add(proteinEntry.Reference, proteinEntry.Protein_ID)
            Next
            nameList.Add(ProteinEntryName)
            Me.m_UniqueProteinIDList(ProteinEntryID) = nameList
        End If

        'flag with some kinda error so we can check out the duplicate entry and rename it
        If Not Me.m_ProteinNameList.Contains(ProteinEntryName) Then
            Me.m_Proteins.Add(ProteinEntryName, proteinEntry)
            Me.m_ProteinNameList.Add(ProteinEntryName)
        End If
    End Sub

    Protected Function SortedProteinNameList() As ArrayList Implements IProteinStorage.GetSortedProteinNames
        Dim al As ArrayList

        al = Me.m_ProteinNameList

        al.Sort()
        Return al
    End Function

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
        If Me.m_Proteins.Contains(Reference) Then
            Return DirectCast(Me.m_Proteins.Item(Reference), Protein_Storage.clsProteinStorageEntry)
        Else
            Return Nothing
        End If
    End Function

    Protected Sub ClearProteins() Implements IProteinStorage.ClearProteinEntries

        Me.m_ResidueCount = 0

        Try
            If Not m_Proteins Is Nothing Then
                Me.m_Proteins.Clear()
            End If
        Catch ex As Exception
            ' Ignore errors here
        End Try

        Try
            If Not m_ProteinNameList Is Nothing Then
                Me.m_ProteinNameList.Clear()
            End If
        Catch ex As Exception
            ' Ignore errors here
        End Try

        Try
            If Not m_UniqueProteinIDList Is Nothing Then
                Me.m_UniqueProteinIDList.Clear()
            End If
        Catch ex As Exception
            ' Ignore errors here
        End Try

    End Sub

    Protected ReadOnly Property TotalResidueCount() As Integer Implements IProteinStorage.TotalResidueCount
        Get
            Return Me.m_ResidueCount
        End Get
    End Property

    Protected ReadOnly Property ProteinCount() As Integer Implements IProteinStorage.ProteinCount
        Get
            Return Me.m_Proteins.Count
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
        Return Me.m_Proteins.GetEnumerator
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

