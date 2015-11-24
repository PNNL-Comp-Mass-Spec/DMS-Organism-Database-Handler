Imports System.Collections.Generic

Public Interface IProteinStorage
    Sub AddProtein(proteinEntry As IProteinStorageEntry)
    ReadOnly Property TotalResidueCount() As Integer
    ReadOnly Property ProteinCount() As Integer
    Function GetEnumerator() As Dictionary(Of String, IProteinStorageEntry).Enumerator
    Function GetProtein(Reference As String) As Protein_Storage.IProteinStorageEntry
    Function GetSortedProteinNames() As SortedSet(Of String)
    Sub ClearProteinEntries()

    Property FileName() As String
    Property EncryptSequences() As Boolean
    Property PassPhrase() As String
End Interface

