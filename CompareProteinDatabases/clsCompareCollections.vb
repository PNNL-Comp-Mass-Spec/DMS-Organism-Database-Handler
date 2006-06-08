Public Class clsCompareCollections
    Implements ICompareCollections

    Sub New()

    End Sub

    Public ReadOnly Property Collection1Orphans() As System.Collections.ArrayList Implements ICompareCollections.Collection1Orphans
        Get

        End Get
    End Property

    Public ReadOnly Property Collection1to2Matches() As System.Collections.Hashtable Implements ICompareCollections.Collection1to2Matches
        Get

        End Get
    End Property

    Public ReadOnly Property Collection2Orphans() As System.Collections.ArrayList Implements ICompareCollections.Collection2Orphans
        Get

        End Get
    End Property

    Public ReadOnly Property Collection2to1Matches() As System.Collections.Hashtable Implements ICompareCollections.Collection2to1Matches
        Get

        End Get
    End Property

    Public Function CompareCollections(ByRef Collection1 As Protein_Storage.clsProteinStorage, ByRef Collection2 As Protein_Storage.clsProteinStorage, ByVal ComparisonType As ICompareCollections.CompTypes) As System.Data.DataTable Implements ICompareCollections.CompareCollections

    End Function

End Class
