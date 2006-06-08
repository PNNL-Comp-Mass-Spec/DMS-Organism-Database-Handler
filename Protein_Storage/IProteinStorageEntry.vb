Public Interface IProteinStorageEntry

    ReadOnly Property Reference() As String
    ReadOnly Property HasAlternateReferences() As Boolean
    ReadOnly Property Description() As String
    Property Sequence() As String

    ReadOnly Property MonoisotopicMass() As Double
    ReadOnly Property AverageMass() As Double
    ReadOnly Property Length() As Integer

    ReadOnly Property MolecularFormula() As String
    Property SHA1Hash() As String

    ReadOnly Property NameXRefs() As ArrayList

    Property AlternateReference() As String
    Property Protein_ID() As Integer
    Property Reference_ID() As Integer
    Property Member_ID() As Integer
    Property Authority_ID() As Integer
    Property IsEncrypted() As Boolean

    Sub AddXRef(ByVal Reference As String)


End Interface
