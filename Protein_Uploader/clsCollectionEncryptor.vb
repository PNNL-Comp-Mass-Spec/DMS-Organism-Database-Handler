Friend Class clsCollectionEncryptor

    Protected m_RijndaelEncryptor As Protein_Exporter.clsRijndaelEncryptionHandler
    Private ReadOnly m_DatabaseAccessor As TableManipulationBase.IGetSQLData
    Event EncryptionStart(taskMsg As String)
    Event EncryptionProgress(statusMsg As String, fractionDone As Double)
    Event EncryptionComplete()

    Sub New(PassPhrase As String, databaseAccessor As TableManipulationBase.IGetSQLData)

        m_RijndaelEncryptor = New Protein_Exporter.clsRijndaelEncryptionHandler(PassPhrase)
        m_DatabaseAccessor = databaseAccessor

    End Sub

    Sub EncryptStorageCollectionSequences(storageCollection As Protein_Storage.IProteinStorage)

        Dim e = storageCollection.GetEnumerator

        OnEncryptionStart("Encrypting Sequences")
        Dim counter = 0
        Dim counterMax As Integer = storageCollection.ProteinCount
        Dim EventTriggerThresh As Integer

        If counterMax <= 50 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 50)
        End If

        While e.MoveNext()
            If counter Mod EventTriggerThresh = 0 Then
                OnEncryptionProgressUpdate(CDbl(counter / counterMax))
            End If

            Dim ce = e.Current.Value
            ce.Sequence = m_RijndaelEncryptor.Encrypt(ce.Sequence)
            ce.SHA1Hash = m_RijndaelEncryptor.MakeArbitraryHash(ce.Sequence)
            ce.IsEncrypted = True
            counter += 1
        End While

    End Sub

    Sub AddUpdateEncryptionMetadata()

    End Sub

    Private Sub OnEncryptionStart(taskMsg As String)
        RaiseEvent EncryptionStart(taskMsg)
    End Sub

    Private Sub OnEncryptionProgressUpdate(fractionDone As Double)
        RaiseEvent EncryptionProgress("", fractionDone)
    End Sub

    Private Sub OnEncryptionComplete()
        RaiseEvent EncryptionComplete()
    End Sub
End Class