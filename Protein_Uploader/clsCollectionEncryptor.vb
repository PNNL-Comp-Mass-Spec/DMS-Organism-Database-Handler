Friend Class clsCollectionEncryptor

    Protected m_RijndaelEncryptor As Protein_Exporter.clsRijndaelEncryptionHandler
    Shared m_TableGrabber As TableManipulationBase.IGetSQLData
    Event EncryptionStart(ByVal taskMsg As String)
    Event EncryptionProgress(ByVal statusMsg As String, ByVal fractionDone As Double)
    Event EncryptionComplete()

    Sub New(ByVal PassPhrase As String, ByVal PSConnectionString As String)

        Me.m_RijndaelEncryptor = New Protein_Exporter.clsRijndaelEncryptionHandler(PassPhrase)

        If Me.m_TableGrabber Is Nothing Then
            Me.m_TableGrabber = New TableManipulationBase.clsDBTask(PSConnectionString, True)
        End If

    End Sub

    Sub EncryptStorageCollectionSequences(ByRef StorageCollection As Protein_Storage.IProteinStorage)


        Dim e As IDictionaryEnumerator = StorageCollection.GetEnumerator
        Dim ce As Protein_Storage.IProteinStorageEntry

        Me.OnEncryptionStart("Encrypting Sequences")
        Dim counter As Integer = 0
        Dim counterMax As Integer = StorageCollection.ProteinCount
        Dim eventTriggerEdge As Integer

        If counterMax > 20 Then
            eventTriggerEdge = CInt(counterMax / 20)
        Else
            eventTriggerEdge = 1
        End If


        While e.MoveNext = True
            If counter Mod eventTriggerEdge = 0 Then
                Me.OnEncryptionProgressUpdate(CDbl(counter / counterMax))
            End If
            ce = DirectCast(e.Value, Protein_Storage.IProteinStorageEntry)
            ce.Sequence = Me.m_RijndaelEncryptor.Encrypt(ce.Sequence)
            ce.SHA1Hash = Me.m_RijndaelEncryptor.MakeArbitraryHash(ce.Sequence)
            ce.IsEncrypted = True
            counter += 1
        End While

    End Sub

    Sub AddUpdateEncryptionMetadata()

    End Sub

    Private Sub OnEncryptionStart(ByVal taskMsg As String)
        RaiseEvent EncryptionStart(taskMsg)
    End Sub

    Private Sub OnEncryptionProgressUpdate(ByVal fractionDone As Double)
        RaiseEvent EncryptionProgress("", fractionDone)
    End Sub

    Private Sub OnEncryptionComplete()
        RaiseEvent EncryptionComplete()
    End Sub
End Class