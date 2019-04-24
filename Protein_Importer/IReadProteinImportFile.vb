Public Interface IReadProteinImportFile
    ReadOnly Property LastErrorMessage As String

    Event LoadStart(taskTitle As String)
    Event LoadEnd()
    Event LoadProgress(fractionDone As Double)

    Function GetProteinEntries(FASTAFilePath As String) As Protein_Storage.IProteinStorage
    Function GetProteinEntries(FASTAFilePath As String, NumRecordsToLoad As Integer) As Protein_Storage.IProteinStorage
End Interface
