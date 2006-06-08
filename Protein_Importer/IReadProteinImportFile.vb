Public Interface IReadProteinImportFile
    ReadOnly Property LastErrorMessage() As String

    Event LoadStart(ByVal taskTitle As String)
    Event LoadEnd()
    Event LoadProgress(ByVal fractionDone As Double)

    Function GetProteinEntries(ByVal FASTAFilePath As String) As Protein_Storage.IProteinStorage
    Function GetProteinEntries(ByVal FASTAFilePath As String, ByVal NumRecordsToLoad As Integer) As Protein_Storage.IProteinStorage
End Interface
