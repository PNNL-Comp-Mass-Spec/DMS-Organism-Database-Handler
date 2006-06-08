Friend Interface IProteinCompare

    Function CompareSequences( _
        ByVal Sequence1 As String, _
        ByVal Sequence2 As String) As ComparisonResults

    Enum ComparisonResults
        Protein1MatchesProtein2         'Exact Match between compared sequences
        Protein1ContainedInProtein2     'Sequence 1 substring matches with Sequence 2
        Protein2ContainedInProtein1     'Sequence 2 substring matches with Sequence 1
        NoMatch                         'No Match (duh)
    End Enum

End Interface
