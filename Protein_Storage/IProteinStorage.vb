Public Interface IProteinStorage
    Sub AddProtein(ByVal proteinEntry As IProteinStorageEntry)
    ReadOnly Property TotalResidueCount() As Integer
    ReadOnly Property ProteinCount() As Integer
    Function GetEnumerator() As IDictionaryEnumerator
    Function GetProtein(ByVal Reference As String) As Protein_Storage.IProteinStorageEntry
    Function GetSortedProteinNames() As ArrayList
    Sub ClearProteinEntries()

    ReadOnly Property DuplicateEntryCount() As Integer
    ReadOnly Property DuplicateReferenceList() As ArrayList
    Property FileName() As String
    Property EncryptSequences() As Boolean
    Property PassPhrase() As String
End Interface

