
Imports System.Collections.Generic

Public Class ProteinStorageDMS
    Inherits ProteinStorage

    Private ReadOnly m_UniqueProteinIDList As Dictionary(Of Integer, SortedSet(Of String))        'Protein_ID, Protein_Name

    Public Sub New(fastaFileName As String)
        MyBase.New(fastaFileName)

        m_UniqueProteinIDList = New Dictionary(Of Integer, SortedSet(Of String))
    End Sub

    Public Overrides Sub AddProtein(proteinEntry As ProteinStorageEntry)

        Dim proteinName As String
        Dim proteinEntryID As Integer = proteinEntry.Protein_ID
        Dim proteinEntryName As String = proteinEntry.Reference

        Dim nameList As SortedSet(Of String) = Nothing

        If Not m_UniqueProteinIDList.TryGetValue(proteinEntryID, nameList) Then
            nameList = New SortedSet(Of String)
            nameList.Add(proteinEntryName)
            m_UniqueProteinIDList.Add(proteinEntryID, nameList)
            m_ResidueCount += proteinEntry.Sequence.Length
        Else

            For Each proteinName In nameList
                Dim existingEntry = m_Proteins.Item(proteinName)

                If Not proteinEntry.Reference.Equals(existingEntry.Reference) Then
                    existingEntry.AddXRef(proteinEntryName)
                    proteinEntry.AddXRef(existingEntry.Reference)
                End If
            Next

            nameList.Add(proteinEntryName)
            m_UniqueProteinIDList(proteinEntryID) = nameList
        End If

        If Not m_ProteinNames.Contains(proteinEntryName) Then
            m_Proteins.Add(proteinEntryName, proteinEntry)
            m_ProteinNames.Add(proteinEntryName)
            m_ResidueCount += proteinEntry.Sequence.Length
        End If

    End Sub

    Public Overrides Sub ClearProteinEntries()

        MyBase.ClearProteinEntries()
        m_UniqueProteinIDList.Clear()

    End Sub

End Class

