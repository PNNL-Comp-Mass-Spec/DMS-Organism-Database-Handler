Imports System.Collections.Generic
Imports Protein_Exporter

Public Interface IAddUpdateEntries

    Sub CompareProteinID(
        proteinCollection As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String))

    Function GetProteinCollectionID(FilePath As String) As Integer

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Sub DeleteProteinCollectionMembers(ProteinCollectionID As Integer, NumProteins As Integer)

    Sub UpdateProteinCollectionMembers(
        ProteinCollectionID As Integer,
        proteinCollection As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String),
        numProteinsExpected As Integer,
        numResiduesExpected As Integer)

    Function GetProteinCollectionMemberCount(ProteinCollectionID As Integer) As Integer

    Function AddProteinReference(
        ProteinName As String,
        Description As String,
        OrganismID As Integer,
        AuthorityID As Integer,
        ProteinID As Integer,
        MaxProteinNameLength As Integer) As Integer


    Sub UpdateProteinNames(
            pc As Protein_Storage.IProteinStorage,
            selectedProteinList As List(Of String),
            organismID As Integer,
            authorityID As Integer)

    Function AddProteinCollectionMember(
        ReferenceID As Integer,
        ProteinID As Integer,
        Sorting_Index As Integer,
        ProteinCollectionID As Integer) As Integer

    Function UpdateProteinCollectionMember(
        ReferenceID As Integer,
        ProteinID As Integer,
        Sorting_Index As Integer,
        ProteinCollectionID As Integer) As Integer


    Function AddAuthenticationHash(
        ProteinCollectionID As Integer,
        AuthenticationHash As String,
        numProteins As Integer,
        totalResidues As Integer) As Integer

    Function AddNamingAuthority(
        ShortName As String,
        FullName As String,
        WebAddress As String) As Integer

    Function AddAnnotationType(
        TypeName As String,
        Description As String,
        Example As String,
        AuthorityID As Integer) As Integer

    Function AddCollectionOrganismXref(
        ProteinCollectionID As Integer,
        OrganismID As Integer) As Integer

    Function GetProteinIDFromName(
        ProteinName As String) As Integer

    Function MakeNewProteinCollection(
        FileName As String,
        Description As String,
        collectionSource As String,
        CollectionType As CollectionTypes,
        AnnotationTypeID As Integer,
        NumProteins As Integer,
        NumResidues As Integer) As Integer

    Function UpdateEncryptionMetadata(
        ProteinCollectionID As Integer,
        PassPhrase As String) As Integer

    Function GetTotalResidueCount(
        proteinCollection As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String)) As Integer

    Function UpdateProteinCollectionState(
        ProteinCollectionID As Integer,
        CollectionStateID As Integer) As Integer

    Function UpdateProteinNameHash(
        ReferenceID As Integer,
        ProteinName As String,
        Description As String,
        ProteinID As Integer) As Integer

    Function UpdateProteinSequenceHash(
        ProteinID As Integer,
        ProteinSequence As String) As Integer


    Function UpdateProteinSequenceInfo(
        ProteinID As Integer,
        Sequence As String,
        Length As Integer,
        MolecularFormula As String,
        MonoisotopicMass As Double,
        AverageMass As Double,
        SHA1Hash As String) As Integer


    Function GetProteinCollectionState(
        ProteinCollectionID As Integer) As String

    Function GenerateArbitraryHash(
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

    Protected ReadOnly m_DatabaseAccessor As TableManipulationBase.IGetSQLData
    Protected m_OrganismID As Integer
    Protected m_ProteinLengths As Hashtable
    Protected m_MaxProteinNameLength As Integer

    Protected m_Hasher As Security.Cryptography.SHA1Managed
    Protected ProteinHashThread As Threading.Thread
    Protected ReferenceHashThread As Threading.Thread

#Region "Properties"
    Public Property MaximumProteinNameLength As Integer Implements IAddUpdateEntries.MaximumProteinNameLength
        Get
            Return m_MaxProteinNameLength
        End Get
        Set
            m_MaxProteinNameLength = Value
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
        m_DatabaseAccessor = New TableManipulationBase.clsDBTask(PISConnectionString, True)
        m_Hasher = New Security.Cryptography.SHA1Managed
    End Sub

    Public Sub CloseConnection()
        m_DatabaseAccessor.CloseConnection()
    End Sub

    ''' <summary>
    ''' Checks for the existence of protein sequences in the T_Proteins table
    ''' Gets Protein_ID if located, makes a new entry if not
    ''' Updates Protein_ID field in clsProteinStorageEntry instance
    ''' </summary>
    ''' <param name="pc"></param>
    ''' <param name="selectedProteinList"></param>
    ''' <remarks></remarks>
    Protected Sub CompareProteinID(
        pc As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String)) Implements IAddUpdateEntries.CompareProteinID

        Dim tmpPC As Protein_Storage.IProteinStorageEntry

        Dim s As String

        OnLoadStart("Comparing to existing sequences and adding new proteins")
        Dim counterMax As Integer = selectedProteinList.Count
        Dim counter As Integer

        Dim EventTriggerThresh As Integer
        If counterMax <= 100 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 100)
            If EventTriggerThresh > 100 Then EventTriggerThresh = 100
        End If

        For Each s In selectedProteinList

            tmpPC = pc.GetProtein(s)

            counter += 1
            If (counter Mod EventTriggerThresh) = 0 Then
                OnProgressUpdate(CDbl(counter / counterMax))
            End If

            tmpPC.Protein_ID = AddProteinSequence(tmpPC)

        Next

        OnLoadEnd()

    End Sub

    Protected Sub UpdateProteinNames(
        pc As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String),
        organismID As Integer,
        authorityID As Integer) Implements IAddUpdateEntries.UpdateProteinNames

        OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection")
        Dim tmpPC As Protein_Storage.IProteinStorageEntry
        Dim counter As Integer
        Dim counterMax As Integer = selectedProteinList.Count
        Dim s As String

        Dim EventTriggerThresh As Integer
        If counterMax <= 100 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 100)
            If EventTriggerThresh > 100 Then EventTriggerThresh = 100
        End If

        For Each s In selectedProteinList
            tmpPC = pc.GetProtein(s)
            counter += 1
            If (counter Mod EventTriggerThresh) = 0 Then
                OnProgressUpdate(CDbl(counter / counterMax))
            End If

            tmpPC.Reference_ID = AddProteinReference(tmpPC.Reference, tmpPC.Description, organismID, authorityID, tmpPC.Protein_ID, m_MaxProteinNameLength)
        Next

        OnLoadEnd()

    End Sub

    Protected Sub UpdateProteinCollection(
        ProteinCollectionID As Integer,
        pc As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String),
        numProteinsExpected As Integer,
        numResiduesExpected As Integer) Implements IAddUpdateEntries.UpdateProteinCollectionMembers

        Dim counterMax As Integer = selectedProteinList.Count
        Dim s As String

        Dim EventTriggerThresh As Integer
        If counterMax <= 100 Then
            EventTriggerThresh = 1
        Else
            EventTriggerThresh = CInt(counterMax / 100)
            If EventTriggerThresh > 100 Then EventTriggerThresh = 100
        End If

        OnLoadStart("Storing Protein Collection Members")

        Dim numProteinsActual As Integer
        Dim numResiduesActual As Integer

        For Each s In selectedProteinList
            Dim tmpPC As Protein_Storage.IProteinStorageEntry = pc.GetProtein(s)
            numProteinsActual += 1
            If (numProteinsActual Mod EventTriggerThresh) = 0 Then
                OnProgressUpdate(CDbl(numProteinsActual / counterMax))
            End If

            numResiduesActual += tmpPC.Length

            tmpPC.Member_ID = AddProteinCollectionMember(tmpPC.Reference_ID, tmpPC.Protein_ID, tmpPC.SortingIndex, ProteinCollectionID)
        Next

        RunSP_UpdateProteinCollectionCounts(numProteinsActual, numResiduesActual, ProteinCollectionID)

        OnLoadEnd()

    End Sub

    Protected Function GetTotalResidueCount(
      proteinCollection As Protein_Storage.IProteinStorage,
      selectedProteinList As List(Of String)) As Integer Implements IAddUpdateEntries.GetTotalResidueCount
        Dim s As String
        Dim totalLength As Integer
        Dim tmpPC As Protein_Storage.IProteinStorageEntry

        For Each s In selectedProteinList
            tmpPC = proteinCollection.GetProtein(s)
            totalLength += tmpPC.Sequence.Length
        Next

        Return totalLength
    End Function


    Protected Function AddProteinSequence(protein As Protein_Storage.IProteinStorageEntry) As Integer
        Dim protein_id As Integer

        With protein
            protein_id = RunSP_AddProteinSequence(
                .Sequence,
                .Length,
                .MolecularFormula,
                .MonoisotopicMass,
                .AverageMass,
                .SHA1Hash,
                .IsEncrypted,
                IAddUpdateEntries.SPModes.add)
        End With

        Return protein_id

    End Function

    Protected Function UpdateProteinSequenceInfo(
        ProteinID As Integer,
        Sequence As String,
        Length As Integer,
        MolecularFormula As String,
        MonoisotopicMass As Double,
        AverageMass As Double,
        SHA1Hash As String) As Integer Implements IAddUpdateEntries.UpdateProteinSequenceInfo

        RunSP_UpdateProteinSequenceInfo(
            ProteinID,
            Sequence,
            Length,
            MolecularFormula,
            MonoisotopicMass,
            AverageMass,
            SHA1Hash)


        Return 0
    End Function

    Protected Function AddNamingAuthority(
        ShortName As String,
        FullName As String,
        WebAddress As String) As Integer Implements IAddUpdateEntries.AddNamingAuthority

        Dim tmpAuthID As Integer

        tmpAuthID = RunSP_AddNamingAuthority(
            ShortName,
            FullName,
            WebAddress)

        Return tmpAuthID

    End Function

    Protected Function AddAnnotationType(
        TypeName As String,
        Description As String,
        Example As String,
        AuthorityID As Integer) As Integer Implements IAddUpdateEntries.AddAnnotationType

        Dim tmpAnnTypeID As Integer

        tmpAnnTypeID = RunSP_AddAnnotationType(
            TypeName, Description, Example, AuthorityID)

        Return tmpAnnTypeID

    End Function

    Protected Function MakeNewProteinCollection(
        FileName As String,
        Description As String,
        collectionSource As String,
        CollectionType As IAddUpdateEntries.CollectionTypes,
        AnnotationTypeID As Integer,
        NumProteins As Integer,
        NumResidues As Integer) As Integer Implements IAddUpdateEntries.MakeNewProteinCollection

        Dim tmpProteinCollectionID As Integer

        tmpProteinCollectionID = RunSP_AddUpdateProteinCollection(
            FileName, Description, collectionSource, CollectionType, IAddUpdateEntries.CollectionStates.NewEntry,
            AnnotationTypeID, NumProteins, NumResidues, IAddUpdateEntries.SPModes.add)

        Return tmpProteinCollectionID

    End Function

    Protected Function UpdateEncryptionMetadata(
        ProteinCollectionID As Integer,
        Passphrase As String) As Integer Implements IAddUpdateEntries.UpdateEncryptionMetadata


        Return RunSP_AddUpdateEncryptionMetadata(Passphrase, ProteinCollectionID)

    End Function

    Protected Function UpdateProteinCollectionState(
        ProteinCollectionID As Integer,
        CollectionStateID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinCollectionState

        Return RunSP_UpdateProteinCollectionStates(ProteinCollectionID, CollectionStateID)

    End Function

    Protected Function GetProteinCollectionState(
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
        Return RunSP_GetProteinIDFromName(ProteinName)
    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Sub DeleteProteinCollectionMembers(proteinCollectionID As Integer, numProteins As Integer) Implements IAddUpdateEntries.DeleteProteinCollectionMembers
        RunSP_DeleteProteinCollectionMembers(proteinCollectionID, numProteins)
    End Sub

    Protected Function GetProteinCollectionID(FilePath As String) As Integer Implements IAddUpdateEntries.GetProteinCollectionID
        Return RunSP_GetProteinCollectionID(IO.Path.GetFileNameWithoutExtension(FilePath))
    End Function

    Protected Function CountProteinCollectionMembers(ProteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.GetProteinCollectionMemberCount
        Return RunSP_GetProteinCollectionMemberCount(ProteinCollectionID)
    End Function

    Protected Function AddCollectionOrganismXref(ProteinCollectionID As Integer, OrganismID As Integer) As Integer Implements IAddUpdateEntries.AddCollectionOrganismXref
        Return RunSP_AddCollectionOrganismXref(ProteinCollectionID, OrganismID)
    End Function

    Protected Function AddProteinCollectionMember(
        ReferenceID As Integer,
        ProteinID As Integer,
        Sorting_Index As Integer,
        ProteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.AddProteinCollectionMember

        Return RunSP_AddProteinCollectionMember(ReferenceID, ProteinID, Sorting_Index, ProteinCollectionID)
    End Function

    Protected Function UpdateProteinCollectionMember(
        ReferenceID As Integer,
        ProteinID As Integer,
        Sorting_Index As Integer,
        ProteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinCollectionMember

        Return RunSP_UpdateProteinCollectionMember(ReferenceID, ProteinID, Sorting_Index, ProteinCollectionID)

    End Function

    Protected Function AddProteinReference(
     ProteinName As String,
     Description As String,
     OrganismID As Integer,
     AuthorityID As Integer,
     ProteinID As Integer,
     MaxProteinNameLength As Integer) As Integer Implements IAddUpdateEntries.AddProteinReference

        Dim ref_ID As Integer

        ref_ID = (RunSP_AddProteinReference(ProteinName, Description, OrganismID, AuthorityID, ProteinID, MaxProteinNameLength))
        Return ref_ID

    End Function

    Protected Function AddFileAuthenticationHash(
        ProteinCollectionID As Integer,
        AuthenticationHash As String,
        numProteins As Integer,
        totalResidues As Integer) As Integer Implements IAddUpdateEntries.AddAuthenticationHash

        Return RunSP_AddCRC32FileAuthentication(ProteinCollectionID, AuthenticationHash, numProteins, totalResidues)

    End Function

    Protected Function UpdateProteinNameHash(
        ReferenceID As Integer,
        ProteinName As String,
        Description As String,
        ProteinID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinNameHash

        Return RunSP_UpdateProteinNameHash(ReferenceID, ProteinName, Description, ProteinID)
    End Function

    Protected Function UpdateProteinSequenceHash(
        ProteinID As Integer,
        ProteinSequence As String) As Integer Implements IAddUpdateEntries.UpdateProteinSequenceHash

        Return RunSP_UpdateProteinSequenceHash(ProteinID, ProteinSequence)

    End Function

    Protected Function GenerateHash(SourceText As String) As String Implements IAddUpdateEntries.GenerateArbitraryHash
        'Create an encoding object to ensure the encoding standard for the source text
        Dim Ue As New Text.ASCIIEncoding

        'Retrieve a byte array based on the source text
        Dim ByteSourceText() As Byte = Ue.GetBytes(SourceText)

        'Compute the hash value from the source
        Dim SHA1_hash() As Byte = m_Hasher.ComputeHash(ByteSourceText)

        'And convert it to String format for return
        Dim SHA1string As String = clsRijndaelEncryptionHandler.ToHexString(SHA1_hash)

        Return SHA1string
    End Function


#Region " Stored Procedure Access "

    Protected Function RunSP_GetProteinCollectionState(
        ProteinCollectionID As Integer) As String

        Dim StateName As String

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinCollectionState", m_DatabaseAccessor.Connection)

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
        ' Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        StateName = CStr(sp_Save.Parameters("@State_Name").Value)

        Return StateName


    End Function

    Protected Function RunSP_AddProteinSequence(
        Sequence As String,
        Length As Integer,
        MolecularFormula As String,
        MonoisotopicMass As Double,
        AverageMass As Double,
        SHA1_Hash As String,
        IsEncrypted As Boolean,
        mode As IAddUpdateEntries.SPModes) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim EncryptionFlag = 0
        If IsEncrypted Then
            EncryptionFlag = 1
        End If

        sp_Save = New SqlClient.SqlCommand("AddProteinSequence", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function


    Protected Function RunSP_UpdateProteinSequenceInfo(
        ProteinID As Integer,
        Sequence As String,
        Length As Integer,
        MolecularFormula As String,
        MonoisotopicMass As Double,
        AverageMass As Double,
        SHA1_Hash As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceInfo", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function


    Protected Function RunSP_AddUpdateProteinCollection(
        FileName As String,
        Description As String,
        collectionSource As String,
        collectionType As IAddUpdateEntries.CollectionTypes,
        collectionState As IAddUpdateEntries.CollectionStates,
        annotationTypeID As Integer,
        numProteins As Integer,
        numResidues As Integer,
        mode As IAddUpdateEntries.SPModes) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateProteinCollection", m_DatabaseAccessor.Connection)

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

        myParam = sp_Save.Parameters.Add("@collectionSource", SqlDbType.VarChar, 900)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = collectionSource

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        If ret = 0 Then
            ' A zero was returned for the protein collection ID; this indicates and error
            ' Raise an exception

            Dim Message As String
            Dim SPMsg As String

            Message = "AddUpdateProteinCollection returned 0 for the Protein Collection ID"

            SPMsg = CStr(sp_Save.Parameters("@Message").Value)

            If Not String.IsNullOrEmpty(SPMsg) Then Message &= "; " & SPMsg

            Throw New ConstraintException(Message)

        End If


        Return ret

    End Function

    Protected Function RunSP_AddProteinCollectionMember(
      Reference_ID As Integer, Protein_ID As Integer,
      SortingIndex As Integer, Protein_Collection_ID As Integer) As Integer

        Return RunSP_AddUpdateProteinCollectionMember(Reference_ID, Protein_ID, SortingIndex, Protein_Collection_ID, "Add")

    End Function

    Protected Function RunSP_UpdateProteinCollectionMember(
      Reference_ID As Integer, Protein_ID As Integer,
      SortingIndex As Integer, Protein_Collection_ID As Integer) As Integer

        Return RunSP_AddUpdateProteinCollectionMember(Reference_ID, Protein_ID, SortingIndex, Protein_Collection_ID, "Update")

    End Function

    Protected Function RunSP_AddUpdateProteinCollectionMember(
      Reference_ID As Integer, Protein_ID As Integer,
      SortingIndex As Integer, Protein_Collection_ID As Integer,
      Mode As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateProteinCollectionMember_New", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddUpdateEncryptionMetadata(
        Passphrase As String, Protein_Collection_ID As Integer) As Integer

        Dim phraseHash As String = GenerateHash(Passphrase)
        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateEncryptionMetadata", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddNamingAuthority(
        ShortName As String, FullName As String,
        WebAddress As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddNamingAuthority", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddAnnotationType(
        TypeName As String, Description As String,
        Example As String, AuthID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddAnnotationType", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionStates(
        Protein_Collection_ID As Integer,
        Collection_State_ID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionState", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="proteinCollectionID"></param>
    ''' <param name="numProteinsForReLoad">The number of proteins that will be uploaded after this delete</param>
    ''' <remarks>NumResidues in T_Protein_Collections is set to 0</remarks>
    Protected Function RunSP_DeleteProteinCollectionMembers(proteinCollectionID As Integer, numProteinsForReLoad As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("DeleteProteinCollectionMembers", m_DatabaseAccessor.Connection)

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
        myParam.Value = proteinCollectionID

        myParam = sp_Save.Parameters.Add("@NumProteinsForReLoad", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = numProteinsForReLoad

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_GetProteinCollectionMemberCount(
        Protein_Collection_ID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinCollectionMemberCount", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddProteinReference(
     Protein_Name As String,
     Description As String,
     OrganismID As Integer,
     AuthorityID As Integer,
     ProteinID As Integer,
     MaxProteinNameLength As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim hashableString As String

        If MaxProteinNameLength <= 0 Then MaxProteinNameLength = 32

        sp_Save = New SqlClient.SqlCommand("AddProteinReference", m_DatabaseAccessor.Connection)

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
        myParam.Value = GenerateHash(hashableString.ToLower)

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        myParam = sp_Save.Parameters.Add("@MaxProteinNameLength", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = MaxProteinNameLength

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        If ret = 0 Then
            ' A zero was returned for the protein reference ID; this indicates and error
            ' Raise an exception

            Dim Message As String
            Dim SPMsg As String

            Message = "AddProteinReference returned 0"

            SPMsg = CStr(sp_Save.Parameters("@Message").Value)

            If Not String.IsNullOrEmpty(SPMsg) Then Message &= "; " & SPMsg

            Throw New ConstraintException(Message)

        End If

        Return ret


    End Function

    Protected Function RunSP_GetProteinCollectionID(
        FileName As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinCollectionID", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret


    End Function

    Protected Function RunSP_AddCRC32FileAuthentication(
      Protein_Collection_ID As Integer,
      AuthenticationHash As String,
      numProteins As Integer,
      totalResidueCount As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddCRC32FileAuthentication", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddCollectionOrganismXref(
        Protein_Collection_ID As Integer,
        OrganismID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddCollectionOrganismXref", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinNameHash(
        Reference_ID As Integer,
        Protein_Name As String,
        Description As String,
        Protein_ID As Integer) As Integer

        Dim tmpHash As String

        Dim sp_Save As SqlClient.SqlCommand
        tmpHash = Protein_Name + "_" + Description + "_" + Protein_ID.ToString
        Dim tmpGenSHA As String = GenerateHash(tmpHash.ToLower)

        sp_Save = New SqlClient.SqlCommand("UpdateProteinNameHash", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionCounts(
      numProteins As Integer,
      numResidues As Integer,
      ProteinCollectionID As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionCounts", m_DatabaseAccessor.Connection)

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

        myParam = sp_Save.Parameters.Add("@NumProteins", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = numProteins

        myParam = sp_Save.Parameters.Add("@NumResidues", SqlDbType.Int)
        myParam.Direction = ParameterDirection.Input
        myParam.Value = numResidues

        myParam = sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256)
        myParam.Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret
    End Function


    Protected Function RunSP_UpdateProteinSequenceHash(
        Protein_ID As Integer,
        Protein_Sequence As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim tmpGenSHA As String = GenerateHash(Protein_Sequence)

        sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceHash", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function



    Protected Function RunSP_GetProteinIDFromName(
        ProteinName As String) As Integer

        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("GetProteinIDFromName", m_DatabaseAccessor.Connection)

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
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret


    End Function

#End Region


End Class
