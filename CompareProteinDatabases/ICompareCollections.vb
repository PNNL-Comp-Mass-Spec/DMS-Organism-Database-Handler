Public Interface ICompareCollections

    Function CompareCollections( _
        ByRef Collection1 As Protein_Storage.clsProteinStorage, _
        ByRef Collection2 As Protein_Storage.clsProteinStorage, _
        ByVal ComparisonType As CompTypes) As DataTable

    Enum CompTypes
        ExactStringMatch
        SubstringMatch
        'BLASTMatch -- I don't even know if I can get code for this!
    End Enum

    ReadOnly Property Collection1to2Matches() As Hashtable   'BaseReference in key, MatchingReference in Value
    ReadOnly Property Collection2to1Matches() As Hashtable   'MatchingReference in key, BaseReference in Value

    ReadOnly Property Collection1Orphans() As ArrayList
    ReadOnly Property Collection2Orphans() As ArrayList

End Interface
