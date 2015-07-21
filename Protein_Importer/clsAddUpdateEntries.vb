Public Interface IAddUpdateEntries

    Sub Setup()

    Sub CompareProteinID( _
        ByRef proteinCollection As Protein_Storage.IProteinStorage, _
        SelectedProteinList As ArrayList)


    Function GetProteinCollectionID(FilePath As String) As Integer

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Sub DeleteProteinCollectionMembers(ProteinCollectionID As Integer)

    Sub UpdateProteinCollectionMembers( _
        ProteinCollectionID As Integer, _
        proteinCollection As Protein_Storage.IProteinStorage, _
        SelectedProteinList As ArrayList)

    Function GetProteinCollectionMemberCount(ProteinCollectionID As Integer) As Integer

    Function AddProteinReference( _
        ProteinName As String, _
        Description As String, _
        OrganismID As Integer, _
        AuthorityID As Integer, _
        ProteinID As Integer, _
        MaxProteinNameLength As Integer) As Integer


    Sub UpdateProteinNames( _
            pc As Protein_Storage.IProteinStorage, _
            SelectedProteinList As ArrayList, _
            organismID As Integer, _
            authorityID As Integer)

    Function AddProteinCollectionMember( _
        ReferenceID As Integer, _
        ProteinID As Integer, _
        Sorting_Index As Integer, _
        ProteinCollectionID As Integer) As Integer

    Function UpdateProteinCollectionMember( _
        ReferenceID As Integer, _
        ProteinID As Integer, _
        Sorting_Index As Integer, _
        ProteinCollectionID As Integer) As Integer


    Function AddAuthenticationHash( _
        ProteinCollectionID As Integer, _
        AuthenticationHash As String,
        numProteins As Integer,
        totalResidues As Integer) As Integer

    Function AddNamingAuthority( _
        ShortName As String, _
        FullName As String, _
        WebAddress As String) As Integer

    Function AddAnnotationType( _
        TypeName As String, _
        Description As String, _
        Example As String, _
        AuthorityID As Integer) As Integer

    Function AddCollectionOrganismXref( _
        ProteinCollectionID As Integer, _
        OrganismID As Integer) As Integer

    Function GetProteinIDFromName( _
        ProteinName As String) As Integer

    Function MakeNewProteinCollection( _
        FileName As String, _
        Description As String, _
        CollectionType As CollectionTypes, _
        AnnotationTypeID As Integer, _
        NumProteins As Integer, _
        NumResidues As Integer) As Integer

    Function UpdateEncryptionMetadata( _
        ProteinCollectionID As Integer, _
        PassPhrase As String) As Integer

    Function GetTotalResidueCount(ByRef proteinCollection As Protein_Storage.IProteinStorage, _
        SelectedProteinList As ArrayList) As Integer

    Function UpdateProteinCollectionState( _
        ProteinCollectionID As Integer, _
        CollectionStateID As Integer) As Integer

    Function UpdateProteinNameHash( _
        ReferenceID As Integer, _
        ProteinName As String, _
        Description As String, _
        ProteinID As Integer) As Integer

    Function UpdateProteinSequenceHash( _
        ProteinID As Integer, _
        ProteinSequence As String) As Integer


    Function UpdateProteinSequenceInfo( _
        ProteinID As Integer, _
        Sequence As String, _
        Length As Integer, _
        MolecularFormula As String, _
        MonoisotopicMass As Double, _
        AverageMass As Double, _
        SHA1Hash As String) As Integer


    Function GetProteinCollectionState( _
        ProteinCollectionID As Integer) As String

    Function GenerateArbitraryHash( _
        SourceText As String) As String

    Property MaximumProteinNameLength As Integer

    Event LoadStart(taskTitle As String)
    Event LoadEnd()
    Event LoadProgress(fractionDone As Double)

    Enum SPModes
        add
        update
    End Enum

    Enum CollectionStates
        NewEntry = 1
        Provisional = 2
        Production = 3
        Historical = 4
    End Enum

    Enum CollectionTypes
        prot_original_source = 1
        modified_source = 2
        runtime_combined_collection = 3
        loadtime_combined_collection = 4
        nuc_original_source = 5
    End Enum

End Interface

Public Class clsAddUpdateEntries
    Implements IAddUpdateEntries

    Protected m_SQLAccess As TableManipulationBase.IGetSQLData
    Protected m_OrganismID As Integer
    'Protected m_ProteinFingerprints As Hashtable
    'Protected m_ReferenceFingerprints As Hashtable
    Protected m_ProteinLengths As Hashtable
    '    Protected m_Hasher As System.Security.Cryptography.SHA1CryptoServiceProvider
    Protected m_MaxProteinNameLength As Integer

    Protected m_Hasher As System.Security.Cryptography.SHA1Managed
    Protected ProteinHashThread As System.Threading.Thread
    Protected ReferenceHashThread As System.Threading.Thread

#Region "Properties"
    Public Property MaximumProteinNameLength As Integer Implements IAddUpdateEntries.MaximumProteinNameLength
        Get
            Return m_MaxProteinNameLength
        End Get
        Set(value As Integer)
            m_MaxProteinNameLength = value
        End Set
    End Property
#End Region

#Region " Events "

    Protected Event LoadStart(taskTitle As String) Implements IAddUpdateEntries.LoadStart
    Protected Event LoadEnd() Implements IAddUpdateEntries.LoadEnd
    Protected Event LoadProgress(fractionDone As Double) Implements IAddUpdateEntries.LoadProgress

    Private Sub OnLoadStart(taskTitle As String)
        RaiseEvent LoadStart(taskTitle)
    End Sub

    Private Sub OnProgressUpdate(fractionDone As Double)
        RaiseEvent LoadProgress(fractionDone)
    End Sub

    Private Sub OnLoadEnd()
        RaiseEvent LoadEnd()
    End Sub

#End Region

    Public Sub New(PISConnectionString As String)
        Me.m_SQLAccess = New TableManipulationBase.clsDBTask(PISConnectionString, True)
        'Me.SpawnGetHashesThread()
        '        Me.m_Hasher = New System.Security.Cryptography.SHA1CryptoServiceProvider
        Me.m_Hasher = New System.Security.Cryptography.SHA1Managed
    End Sub

    Public Sub CloseConnection()
        Me.m_SQLAccess.CloseConnection()
    End Sub

    Protected Sub Setup() Implements IAddUpdateEntries.Setup
        'Me.GetProteinFingerprintHash()
        'Me.GetReferenceFingerprintHash()
    End Sub


    'Checks for the existence of protein sequences in the T_Proteins table
    'gets Protein_ID if located, makes a new entry if not
    'updates Protein_ID field in clsProteinStorageEntry instance
    Protected Sub CompareProteinID( _
        ByRef pc As Protein_Storage.IProteinStorage, _
        SelectedProteinList As ArrayList) Implements IAddUpdateEntries.CompareProteinID

        Dim tmpPC As Protein_Storage.IProteinStorageEntry

        Dim s As String


        Me.OnLoadStart("Comparing to existing sequences and adding new proteins")
        Dim counterMax As Integer = SelectedProteinList.Count
        Dim counter As Integer

        Dim EventTriggerThresh As Integer
        If counterMax <= 100 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 100)
            If EventTriggerThresh > 100 Then EventTriggerThresh = 100
        End If

        For Each s In SelectedProteinList

            tmpPC = pc.GetProtein(s)

            counter += 1
            If (counter Mod EventTriggerThresh) = 0 Then
                Me.OnProgressUpdate(CDbl(counter / counterMax))
            End If

            tmpPC.Protein_ID = Me.AddProteinSequence(tmpPC)

        Next

        Me.OnLoadEnd()

    End Sub

    Protected Sub SpawnGetHashesThread()
        'ProteinHashThread = New System.Threading.Thread(AddressOf Me.GetProteinFingerprintHash)
        'ProteinHashThread.Start()
        'ReferenceHashThread = New System.Threading.Thread(AddressOf Me.GetReferenceFingerprintHash)
        'ReferenceHashThread.Start()
    End Sub

    Protected Sub UpdateProteinNames( _
        pc As Protein_Storage.IProteinStorage, _
        SelectedProteinList As ArrayList, _
        organismID As Integer, _
        authorityID As Integer) Implements IAddUpdateEntries.UpdateProteinNames

        Me.OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection")
        Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim counter As Integer
        Dim counterMax As Integer = SelectedProteinList.Count
        Dim s As String

        Dim EventTriggerThresh As Integer
        If counterMax <= 100 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 100)
            If EventTriggerThresh > 100 Then EventTriggerThresh = 100
        End If

        'For Each tmpPC In pc
        For Each s In SelectedProteinList
            tmpPC = pc.GetProtein(s)
            counter += 1
            If (counter Mod EventTriggerThresh) = 0 Then
                Me.OnProgressUpdate(CDbl(counter / counterMax))
            End If

            tmpPC.Reference_ID = Me.AddProteinReference(tmpPC.Reference, tmpPC.Description, organismID, authorityID, tmpPC.Protein_ID, m_MaxProteinNameLength)
        Next

        Me.OnLoadEnd()

    End Sub

    Protected Sub UpdateProteinCollection( _
        ProteinCollectionID As Integer, _
        pc As Protein_Storage.IProteinStorage, _
        SelectedProteinList As ArrayList) Implements IAddUpdateEntries.UpdateProteinCollectionMembers

        Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim counter As Integer
        Dim counterMax As Integer = SelectedProteinList.Count
        Dim s As String

        Dim EventTriggerThresh As Integer
        If counterMax <= 100 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 100)
            If EventTriggerThresh > 100 Then EventTriggerThresh = 100
        End If

        Me.OnLoadStart("Storing Protein Collection Members")

        For Each s In SelectedProteinList
            tmpPC = pc.GetProtein(s)
            counter += 1
            If (counter Mod EventTriggerThresh) = 0 Then
                Me.OnProgressUpdate(CDbl(counter / counterMax))
            End If

            tmpPC.Member_ID = Me.AddProteinCollectionMember(tmpPC.Reference_ID, tmpPC.Protein_ID, tmpPC.SortingIndex, ProteinCollectionID)
        Next

        Me.OnLoadEnd()

    End Sub

    Protected Function GetTotalResidueCount(ByRef proteinCollection As Protein_Storage.IProteinStorage, _
    SelectedProteinList As ArrayList) As Integer Implements IAddUpdateEntries.GetTotalResidueCount
        Dim s As String
        Dim totalLength As Integer
        Dim tmpPC As Protein_Storage.IProteinStorageEntry

        For Each s In SelectedProteinList
            tmpPC = proteinCollection.GetProtein(s)
            totalLength += tmpPC.Sequence.Length
        Next

        Return totalLength
    End Function


    Protected Function AddProteinSequence(protein As Protein_Storage.IProteinStorageEntry) As Integer
        Dim protein_id As Integer

        'If Not Me.m_ProteinFingerprints.ContainsKey(protein.SHA1Hash) Then

        With protein
            protein_id = Me.RunSP_AddProteinSequence( _
                .Sequence, _
                .Length, _
                .MolecularFormula, _
                .MonoisotopicMass, _
                .AverageMass, _
                .SHA1Hash, _
                .IsEncrypted, _
                IAddUpdateEntries.SPModes.add)
            'Me.m_ProteinFingerprints.Add(.SHA1Hash, protein_id)
        End With
        'Else
        'protein_id = DirectCast(Me.m_ProteinFingerprints.Item(protein.SHA1Hash), Int32)
        'End If

        Return protein_id

    End Function

    Protected Function UpdateProteinSequenceInfo( _
        ProteinID As Integer, _
        Sequence As String, _
        Length As Integer, _
        MolecularFormula As String, _
        MonoisotopicMass As Double, _
        AverageMass As Double, _
        SHA1Hash As String) As Integer Implements IAddUpdateEntries.UpdateProteinSequenceInfo

        Me.RunSP_UpdateProteinSequenceInfo( _
            ProteinID, _
            Sequence, _
            Length, _
            MolecularFormula, _
            MonoisotopicMass, _
            AverageMass, _
            SHA1Hash)


        Return 0
    End Function

    Protected Function AddNamingAuthority( _
        ShortName As String, _
        FullName As String, _
        WebAddress As String) As Integer Implements IAddUpdateEntries.AddNamingAuthority

        Dim tmpAuthID As Integer

        tmpAuthID = Me.RunSP_AddNamingAuthority( _
            ShortName, _
            FullName, _
            WebAddress)

        Return tmpAuthID

    End Function

    Protected Function AddAnnotationType( _
        TypeName As String, _
        Description As String, _
        Example As String, _
        AuthorityID As Integer) As Integer Implements IAddUpdateEntries.AddAnnotationType

        Dim tmpAnnTypeID As Integer

        tmpAnnTypeID = Me.RunSP_AddAnnotationType( _
            TypeName, Description, Example, AuthorityID)

        Return tmpAnnTypeID

    End Function


    Protected Function MakeNewProteinCollection( _
        FileName As String, _
        Description As String, _
        CollectionType As IAddUpdateEntries.CollectionTypes, _
        AnnotationTypeID As Integer, _
        NumProteins As Integer, _
        NumResidues As Integer) As Integer Implements IAddUpdateEntries.MakeNewProteinCollection

        Dim tmpProteinCollectionID As Integer

        tmpProteinCollectionID = Me.RunSP_AddUpdateProteinCollection( _
            FileName, Description, CollectionType, IAddUpdateEntries.CollectionStates.NewEntry, _
            AnnotationTypeID, NumProteins, NumResidues, IAddUpdateEntries.SPModes.add)

        Return tmpProteinCollectionID

    End Function

    Protected Function UpdateEncryptionMetadata( _
        ProteinCollectionID As Integer, _
        Passphrase As String) As Integer Implements IAddUpdateEntries.UpdateEncryptionMetadata


        Return Me.RunSP_AddUpdateEncryptionMetadata(Passphrase, ProteinCollectionID)

    End Function

    Protected Function UpdateProteinCollectionState( _
        ProteinCollectionID As Integer, _
        CollectionStateID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinCollectionState

        Return Me.RunSP_UpdateProteinCollectionStates(ProteinCollectionID, CollectionStateID)

    End Function

    Protected Function GetProteinCollectionState( _
        ProteinCollectionID As Integer) As String Implements IAddUpdateEntries.GetProteinCollectionState

        Return RunSP_GetProteinCollectionState(ProteinCollectionID)

    End Function

    Protected Function GetProteinID(entry As Protein_Storage.IProteinStorageEntry, ByRef hitsTable As DataTable) As Integer
        Dim testrow As DataRow
        Dim foundRows() As DataRow
        Dim tmpSeq As String
        Dim tmpProteinID As Integer

        foundRows = hitsTable.Select("[SHA1_Hash] = '" & entry.SHA1Hash & "'")
        If foundRows.Length > 0 Then
            For Each testrow In foundRows
                tmpSeq = CStr(testrow.Item("Sequence"))
                If tmpSeq.Equals(entry.Sequence) Then
                    tmpProteinID = CInt(testrow.Item("Protein_ID"))
                End If
            Next
        Else
            tmpProteinID = 0
        End If

        Return tmpProteinID

    End Function

    Protected Function GetProteinIDFromName(ProteinName As String) As Integer Implements IAddUpdateEntries.GetProteinIDFromName
        Return Me.RunSP_GetProteinIDFromName(ProteinName)
    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Sub DeleteProteinCollectionMembers(ProteinCollectionID As Integer) Implements IAddUpdateEntries.DeleteProteinCollectionMembers
        Me.RunSP_DeleteProteinCollectionMembers(ProteinCollectionID)
    End Sub

    Protected Function GetProteinCollectionID(FilePath As String) As Integer Implements IAddUpdateEntries.GetProteinCollectionID
        Return Me.RunSP_GetProteinCollectionID(System.IO.Path.GetFileNameWithoutExtension(FilePath))
    End Function

    Protected Function CountProteinCollectionMembers(ProteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.GetProteinCollectionMemberCount
        Return Me.RunSP_GetProteinCollectionMemberCount(ProteinCollectionID)
    End Function

    Protected Function AddCollectionOrganismXref(ProteinCollectionID As Integer, OrganismID As Integer) As Integer Implements IAddUpdateEntries.AddCollectionOrganismXref
        Return Me.RunSP_AddCollectionOrganismXref(ProteinCollectionID, OrganismID)
    End Function

    Protected Function AddProteinCollectionMember( _
        ReferenceID As Integer, _
        ProteinID As Integer, _
        Sorting_Index As Integer, _
        ProteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.AddProteinCollectionMember

        'Return Me.RunSP_AddUpdateProteinCollectionMember(ReferenceID, ProteinID, ProteinCollectionID)
        Return Me.RunSP_AddProteinCollectionMember(ReferenceID, ProteinID, Sorting_Index, ProteinCollectionID)
    End Function

    Protected Function UpdateProteinCollectionMember( _
        ReferenceID As Integer, _
        ProteinID As Integer, _
        Sorting_Index As Integer, _
        ProteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinCollectionMember

        Return Me.RunSP_UpdateProteinCollectionMember(ReferenceID, ProteinID, Sorting_Index, ProteinCollectionID)

    End Function

    Protected Function AddProteinReference( _
     ProteinName As String, _
     Description As String, _
     OrganismID As Integer, _
     AuthorityID As Integer, _
     ProteinID As Integer, _
     MaxProteinNameLength As Integer) As Integer Implements IAddUpdateEntries.AddProteinReference

        Dim ref_ID As Integer

        'Dim nameDescHash As String = Me.GenerateHash(ProteinName & ProteinID.ToString)

        'If Not Me.m_ReferenceFingerprints.ContainsKey(nameDescHash) Then

        ref_ID = (Me.RunSP_AddProteinReference(ProteinName, Description, OrganismID, AuthorityID, ProteinID, MaxProteinNameLength))
        'Me.m_ReferenceFingerprints.Add(nameDescHash, ref_ID)
        'Else
        '    ref_ID = DirectCast(Me.m_ReferenceFingerprints.Item(nameDescHash), Int32)
        'End If

        Return ref_ID

    End Function

    Protected Function AddFileAuthenticationHash( _
        ProteinCollectionID As Integer, _
        AuthenticationHash As String,
        numProteins As Integer,
        totalResidues As Integer) As Integer Implements IAddUpdateEntries.AddAuthenticationHash

        Return Me.RunSP_AddCRC32FileAuthentication(ProteinCollectionID, AuthenticationHash, numProteins, totalResidues)

    End Function

    Protected Function UpdateProteinNameHash( _
        ReferenceID As Integer, _
        ProteinName As String, _
        Description As String, _
        ProteinID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinNameHash

        Return Me.RunSP_UpdateProteinNameHash(ReferenceID, ProteinName, Description, ProteinID)
    End Function

    Protected Function UpdateProteinSequenceHash( _
        ProteinID As Integer, _
        ProteinSequence As String) As Integer Implements IAddUpdateEntries.UpdateProteinSequenceHash

        Return Me.RunSP_UpdateProteinSequenceHash(ProteinID, ProteinSequence)

    End Function

    Protected Function GenerateHash(SourceText As String) As String Implements IAddUpdateEntries.GenerateArbitraryHash
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New System.Text.ASCIIEncoding
        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)
        'Compute the hash value from the source
        Dim SHA1_hash() As Byte = Me.m_Hasher.ComputeHash(ByteSourceText)
        'And convert it to String format for return
        Dim SHA1string As String = HexConverter.ToHexString(SHA1_hash)

        Return SHA1string
    End Function


#Region " Stored Procedure Access "

    Protected Function RunSP_GetProteinCollectionState( _
        ProteinCollectionID As Integer) As String

        Dim StateName As String

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinCollectionState", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinCollectionID

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@State_Name", SqlDbType.VarChar, 32)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)
        StateName = CStr(sp_Save.Parameters("@State_Name").Value)

        Return StateName


    End Function

    Protected Function RunSP_AddProteinSequence( _
        Sequence As String, _
        Length As Integer, _
        MolecularFormula As String, _
        MonoisotopicMass As Double, _
        AverageMass As Double, _
        SHA1_Hash As String, _
        IsEncrypted As Boolean, _
        mode As IAddUpdateEntries.SPModes) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim EncryptionFlag As Integer = 0
        If IsEncrypted Then
            EncryptionFlag = 1
        End If

        sp_Save = New SqlClient.SqlCommand("AddProteinSequence", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        ' Increase the timeout to 5 minutes
        sp_Save.CommandTimeout = 300

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@sequence", SqlDbType.Text)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Sequence

        myParam = sp_Save.Parameters.Add("@length", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Length

        myParam = sp_Save.Parameters.Add("@molecular_formula", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = MolecularFormula

        myParam = sp_Save.Parameters.Add("@monoisotopic_mass", SqlDbType.Float, 8)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = MonoisotopicMass

        myParam = sp_Save.Parameters.Add("@average_mass", SqlDbType.Float, 8)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = AverageMass

        myParam = sp_Save.Parameters.Add("@sha1_hash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = SHA1_Hash

        myParam = sp_Save.Parameters.Add("@is_encrypted", SqlDbType.TinyInt)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = EncryptionFlag

        myParam = sp_Save.Parameters.Add("@mode", SqlDbType.VarChar, 12)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = mode.ToString

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function


    Protected Function RunSP_UpdateProteinSequenceInfo( _
        ProteinID As Integer, _
        Sequence As String, _
        Length As Integer, _
        MolecularFormula As String, _
        MonoisotopicMass As Double, _
        AverageMass As Double, _
        SHA1_Hash As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceInfo", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Protein_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinID

        myParam = sp_Save.Parameters.Add("@sequence", SqlDbType.Text)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Sequence

        myParam = sp_Save.Parameters.Add("@length", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Length

        myParam = sp_Save.Parameters.Add("@molecular_formula", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = MolecularFormula

        myParam = sp_Save.Parameters.Add("@monoisotopic_mass", SqlDbType.Float, 8)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = MonoisotopicMass

        myParam = sp_Save.Parameters.Add("@average_mass", SqlDbType.Float, 8)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = AverageMass

        myParam = sp_Save.Parameters.Add("@sha1_hash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = SHA1_Hash

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function


    Protected Function RunSP_AddUpdateProteinCollection( _
        FileName As String, _
        Description As String, _
        collectionType As IAddUpdateEntries.CollectionTypes, _
        collectionState As IAddUpdateEntries.CollectionStates, _
        annotationTypeID As Integer, _
        numProteins As Integer, _
        numResidues As Integer, _
        mode As IAddUpdateEntries.SPModes) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateProteinCollection", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@fileName", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = FileName

        myParam = sp_Save.Parameters.Add("@Description", SqlDbType.VarChar, 900)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Description

        myParam = sp_Save.Parameters.Add("@collection_type", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CInt(collectionType)

        myParam = sp_Save.Parameters.Add("@collection_state", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CInt(collectionState)

        myParam = sp_Save.Parameters.Add("@primary_annotation_type_id", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = CInt(annotationTypeID)

        myParam = sp_Save.Parameters.Add("@numProteins", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = numProteins

        myParam = sp_Save.Parameters.Add("@numResidues", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = numResidues

        myParam = sp_Save.Parameters.Add("@mode", SqlDbType.VarChar, 12)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = mode.ToString

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 512)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        If ret = 0 Then
            ' A zero was returned for the protein collection ID; this indicates and error
            ' Raise an exception

            Dim Message As String
            Dim SPMsg As String

            Message = "AddUpdateProteinCollection returned 0 for the Protein Collection ID"

            SPMsg = CStr(sp_Save.Parameters("@Message").Value)

            If Not String.IsNullOrEmpty(SPMsg) Then Message &= "; " & SPMsg

            Throw New System.Data.ConstraintException(Message)

        End If


        Return ret

    End Function


    Protected Function RunSP_AddProteinCollectionMember( _
        Reference_ID As Integer, Protein_ID As Integer, _
        SortingIndex As Integer, Protein_Collection_ID As Integer) As Integer

        Return Me.RunSP_AddUpdateProteinCollectionMember(Reference_ID, Protein_ID, SortingIndex, Protein_Collection_ID, "Add")

    End Function

    Protected Function RunSP_UpdateProteinCollectionMember( _
    Reference_ID As Integer, Protein_ID As Integer, _
    SortingIndex As Integer, Protein_Collection_ID As Integer) As Integer

        Return Me.RunSP_AddUpdateProteinCollectionMember(Reference_ID, Protein_ID, SortingIndex, Protein_Collection_ID, "Update")

    End Function


    Protected Function RunSP_AddUpdateProteinCollectionMember( _
    Reference_ID As Integer, Protein_ID As Integer, _
    SortingIndex As Integer, Protein_Collection_ID As Integer, _
    Mode As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateProteinCollectionMember_New", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@reference_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Reference_ID

        myParam = sp_Save.Parameters.Add("@protein_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_ID

        myParam = sp_Save.Parameters.Add("@sorting_index", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = SortingIndex

        myParam = sp_Save.Parameters.Add("@protein_collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        myParam = sp_Save.Parameters.Add("@mode", SqlDbType.VarChar, 10)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Mode

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddUpdateEncryptionMetadata( _
        Passphrase As String, Protein_Collection_ID As Integer) As Integer

        Dim phraseHash As String = Me.GenerateHash(Passphrase)
        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateEncryptionMetadata", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Protein_Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        myParam = sp_Save.Parameters.Add("@Encryption_Passphrase", SqlDbType.VarChar, 64)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Passphrase

        myParam = sp_Save.Parameters.Add("@Passphrase_SHA1_Hash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = phraseHash

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddNamingAuthority( _
        ShortName As String, FullName As String, _
        WebAddress As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddNamingAuthority", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 64)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ShortName

        myParam = sp_Save.Parameters.Add("@description", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = FullName

        myParam = sp_Save.Parameters.Add("@web_address", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = WebAddress

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddAnnotationType( _
        TypeName As String, Description As String, _
        Example As String, AuthID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddAnnotationType", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 64)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = TypeName

        myParam = sp_Save.Parameters.Add("@description", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Description

        myParam = sp_Save.Parameters.Add("@example", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Example

        myParam = sp_Save.Parameters.Add("@authID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = AuthID

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionStates( _
        Protein_Collection_ID As Integer, _
        Collection_State_ID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionState", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@protein_collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        myParam = sp_Save.Parameters.Add("@state_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Collection_State_ID

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="Protein_Collection_ID"></param>
    ''' <remarks></remarks>
    Protected Function RunSP_DeleteProteinCollectionMembers(Protein_Collection_ID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("DeleteProteinCollectionMembers", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        ' Increase the timeout to 10 minutes
        sp_Save.CommandTimeout = 600

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_GetProteinCollectionMemberCount( _
        Protein_Collection_ID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinCollectionMemberCount", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddProteinReference( _
     Protein_Name As String, _
     Description As String, _
     OrganismID As Integer, _
     AuthorityID As Integer, _
     ProteinID As Integer, _
     MaxProteinNameLength As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim hashableString As String

        If MaxProteinNameLength <= 0 Then MaxProteinNameLength = 32

        sp_Save = New SqlClient.SqlCommand("AddProteinReference", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Name

        myParam = sp_Save.Parameters.Add("@description", SqlDbType.VarChar, 900)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Description

        'TODO (org fix) Remove this reference and fix associated Sproc
        'myParam = sp_Save.Parameters.Add("@organism_ID", SqlDbType.Int)
        'myParam.Direction = ParameterDirection.Input
        'myParam.Value = OrganismID

        myParam = sp_Save.Parameters.Add("@authority_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = AuthorityID

        myParam = sp_Save.Parameters.Add("@protein_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinID

        myParam = sp_Save.Parameters.Add("@nameDescHash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        hashableString = Protein_Name + "_" + Description + "_" + ProteinID.ToString
        myParam.Value = Me.GenerateHash(hashableString.ToLower)

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        myParam = sp_Save.Parameters.Add("@MaxProteinNameLength", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = MaxProteinNameLength

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        If ret = 0 Then
            ' A zero was returned for the protein reference ID; this indicates and error
            ' Raise an exception

            Dim Message As String
            Dim SPMsg As String

            Message = "AddProteinReference returned 0"

            SPMsg = CStr(sp_Save.Parameters("@Message").Value)

            If Not String.IsNullOrEmpty(SPMsg) Then Message &= "; " & SPMsg

            Throw New System.Data.ConstraintException(Message)

        End If

        Return ret


    End Function

    Protected Function RunSP_GetProteinCollectionID( _
        FileName As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinCollectionID", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@fileName", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = FileName

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret


    End Function

    Protected Function RunSP_AddCRC32FileAuthentication(
      Protein_Collection_ID As Integer,
      AuthenticationHash As String,
      numProteins As Integer,
      totalResidueCount As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddCRC32FileAuthentication", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        myParam = sp_Save.Parameters.Add("@CRC32FileHash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = AuthenticationHash

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        myParam = sp_Save.Parameters.Add("@numProteins", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = numProteins

        myParam = sp_Save.Parameters.Add("@totalResidueCount", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = totalResidueCount

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddCollectionOrganismXref( _
        Protein_Collection_ID As Integer, _
        OrganismID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddCollectionOrganismXref", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Protein_Collection_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_Collection_ID

        myParam = sp_Save.Parameters.Add("@Organism_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = OrganismID

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinNameHash( _
        Reference_ID As Integer, _
        Protein_Name As String, _
        Description As String, _
        Protein_ID As Integer) As Integer

        Dim tmpHash As String

        Dim sp_Save As SqlClient.SqlCommand
        tmpHash = Protein_Name + "_" + Description + "_" + Protein_ID.ToString
        Dim tmpGenSHA As String = Me.GenerateHash(tmpHash.ToLower)

        sp_Save = New SqlClient.SqlCommand("UpdateProteinNameHash", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Reference_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Reference_ID

        myParam = sp_Save.Parameters.Add("@SHA1Hash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = tmpGenSHA

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function


    Protected Function RunSP_UpdateProteinSequenceHash( _
        Protein_ID As Integer, _
        Protein_Sequence As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim tmpGenSHA As String = Me.GenerateHash(Protein_Sequence)

        sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceHash", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@Protein_ID", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = Protein_ID

        myParam = sp_Save.Parameters.Add("@SHA1Hash", SqlDbType.VarChar, 40)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = tmpGenSHA

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output


        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



    Protected Function RunSP_GetProteinIDFromName( _
        ProteinName As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinIDFromName", Me.m_SQLAccess.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue

        'Define parameters for the sp's arguments
        myParam = sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 128)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = ProteinName

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret As Integer = CInt(sp_Save.Parameters("@Return").Value)

        Return ret


    End Function


#End Region

    Class HexConverter

        Private Shared hexDigits As Char() = {"0"c, "1"c, "2"c, "3"c, "4"c, "5"c, "6"c, "7"c, "8"c, "9"c, "A"c, "B"c, "C"c, "D"c, "E"c, "F"c}

        Public Shared Function ToHexString(bytes() As Byte) As String

            Dim hexStr As String = ""
            Dim i As Integer

            Dim sb As New System.Text.StringBuilder

            For i = 0 To bytes.Length - 1

                sb.Append(bytes(i).ToString("X").PadLeft(2, "0"c))

            Next

            hexStr = sb.ToString
            Return hexStr.ToUpper

        End Function 'ToHexString

    End Class 'HexConverter

End Class
