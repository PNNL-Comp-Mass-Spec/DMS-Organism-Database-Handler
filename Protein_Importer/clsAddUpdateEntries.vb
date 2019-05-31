Imports System.Collections.Generic
Imports Protein_Exporter

Public Interface IAddUpdateEntries

    Sub CompareProteinID(
        proteinCollection As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String))

    Function GetProteinCollectionID(filePath As String) As Integer

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Sub DeleteProteinCollectionMembers(proteinCollectionID As Integer, NumProteins As Integer)

    Sub UpdateProteinCollectionMembers(
        proteinCollectionID As Integer,
        proteinCollection As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String),
        numProteinsExpected As Integer,
        numResiduesExpected As Integer)

    Function GetProteinCollectionMemberCount(proteinCollectionID As Integer) As Integer

    Function AddProteinReference(
        proteinName As String,
        description As String,
        organismID As Integer,
        authorityID As Integer,
        proteinID As Integer,
        maxProteinNameLength As Integer) As Integer


    Sub UpdateProteinNames(
            pc As Protein_Storage.IProteinStorage,
            selectedProteinList As List(Of String),
            organismID As Integer,
            authorityID As Integer)

    Function AddProteinCollectionMember(
        referenceID As Integer,
        proteinID As Integer,
        sorting_Index As Integer,
        proteinCollectionID As Integer) As Integer

    Function UpdateProteinCollectionMember(
        referenceID As Integer,
        proteinID As Integer,
        sorting_Index As Integer,
        proteinCollectionID As Integer) As Integer


    Function AddAuthenticationHash(
        proteinCollectionID As Integer,
        authenticationHash As String,
        numProteins As Integer,
        totalResidues As Integer) As Integer

    Function AddNamingAuthority(
        shortName As String,
        fullName As String,
        webAddress As String) As Integer

    Function AddAnnotationType(
        typeName As String,
        description As String,
        example As String,
        authorityID As Integer) As Integer

    Function AddCollectionOrganismXref(
        proteinCollectionID As Integer,
        organismID As Integer) As Integer

    Function GetProteinIDFromName(
        proteinName As String) As Integer

    Function MakeNewProteinCollection(
        fileName As String,
        description As String,
        collectionSource As String,
        collectionType As CollectionTypes,
        annotationTypeID As Integer,
        numProteins As Integer,
        numResidues As Integer) As Integer

    Function UpdateEncryptionMetadata(
        proteinCollectionID As Integer,
        passPhrase As String) As Integer

    Function GetTotalResidueCount(
        proteinCollection As Protein_Storage.IProteinStorage,
        selectedProteinList As List(Of String)) As Integer

    Function UpdateProteinCollectionState(
        proteinCollectionID As Integer,
        collectionStateID As Integer) As Integer

    Function UpdateProteinNameHash(
        referenceID As Integer,
        proteinName As String,
        description As String,
        proteinID As Integer) As Integer

    Function UpdateProteinSequenceHash(
        proteinID As Integer,
        proteinSequence As String) As Integer

    Function UpdateProteinSequenceInfo(
        proteinID As Integer,
        sequence As String,
        length As Integer,
        molecularFormula As String,
        monoisotopicMass As Double,
        averageMass As Double,
        sha1Hash As String) As Integer


    Function GetProteinCollectionState(
        proteinCollectionID As Integer) As String

    Function GenerateArbitraryHash(
        sourceText As String) As String

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
      proteinID As Integer,
      sequence As String,
      length As Integer,
      molecularFormula As String,
      monoisotopicMass As Double,
      averageMass As Double,
      sha1Hash As String) As Integer Implements IAddUpdateEntries.UpdateProteinSequenceInfo

        RunSP_UpdateProteinSequenceInfo(
            proteinID,
            sequence,
            length,
            molecularFormula,
            monoisotopicMass,
            averageMass,
            sha1Hash)


        Return 0
    End Function

    Protected Function AddNamingAuthority(
      shortName As String,
      fullName As String,
      webAddress As String) As Integer Implements IAddUpdateEntries.AddNamingAuthority

        Dim tmpAuthID As Integer

        tmpAuthID = RunSP_AddNamingAuthority(
            shortName,
            fullName,
            webAddress)

        Return tmpAuthID

    End Function

    Protected Function AddAnnotationType(
        typeName As String,
        description As String,
        example As String,
        authorityID As Integer) As Integer Implements IAddUpdateEntries.AddAnnotationType

        Dim tmpAnnTypeID As Integer

        tmpAnnTypeID = RunSP_AddAnnotationType(
            typeName, description, example, authorityID)

        Return tmpAnnTypeID

    End Function

    Protected Function MakeNewProteinCollection(
      fileName As String,
      description As String,
      collectionSource As String,
      collectionType As IAddUpdateEntries.CollectionTypes,
      annotationTypeID As Integer,
      numProteins As Integer,
      numResidues As Integer) As Integer Implements IAddUpdateEntries.MakeNewProteinCollection

        Dim tmpProteinCollectionID As Integer

        tmpProteinCollectionID = RunSP_AddUpdateProteinCollection(
            fileName, description, collectionSource, collectionType, IAddUpdateEntries.CollectionStates.NewEntry,
            annotationTypeID, numProteins, numResidues, IAddUpdateEntries.SPModes.add)

        Return tmpProteinCollectionID

    End Function

    Protected Function UpdateEncryptionMetadata(
      proteinCollectionID As Integer,
      passphrase As String) As Integer Implements IAddUpdateEntries.UpdateEncryptionMetadata


        Return RunSP_AddUpdateEncryptionMetadata(passphrase, proteinCollectionID)

    End Function

    Protected Function UpdateProteinCollectionState(
      proteinCollectionID As Integer,
      collectionStateID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinCollectionState

        Return RunSP_UpdateProteinCollectionStates(proteinCollectionID, collectionStateID)

    End Function

    Protected Function GetProteinCollectionState(proteinCollectionID As Integer) As String Implements IAddUpdateEntries.GetProteinCollectionState

        Return RunSP_GetProteinCollectionState(proteinCollectionID)

    End Function

    Protected Function GetProteinID(entry As Protein_Storage.IProteinStorageEntry, hitsTable As DataTable) As Integer
        Dim foundRows() As DataRow
        Dim tmpSeq As String
        Dim tmpProteinID As Integer

        foundRows = hitsTable.Select("[SHA1_Hash] = '" & entry.SHA1Hash & "'")
        If foundRows.Length > 0 Then
            For Each testRow As DataRow In foundRows
                tmpSeq = CStr(testRow.Item("Sequence"))
                If tmpSeq.Equals(entry.Sequence) Then
                    tmpProteinID = CInt(testRow.Item("Protein_ID"))
                End If
            Next
        Else
            tmpProteinID = 0
        End If

        Return tmpProteinID

    End Function

    Protected Function GetProteinIDFromName(proteinName As String) As Integer Implements IAddUpdateEntries.GetProteinIDFromName
        Return RunSP_GetProteinIDFromName(proteinName)
    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Sub DeleteProteinCollectionMembers(proteinCollectionID As Integer, numProteins As Integer) Implements IAddUpdateEntries.DeleteProteinCollectionMembers
        RunSP_DeleteProteinCollectionMembers(proteinCollectionID, numProteins)
    End Sub

    Protected Function GetProteinCollectionID(filePath As String) As Integer Implements IAddUpdateEntries.GetProteinCollectionID
        Return RunSP_GetProteinCollectionID(IO.Path.GetFileNameWithoutExtension(filePath))
    End Function

    Protected Function CountProteinCollectionMembers(proteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.GetProteinCollectionMemberCount
        Return RunSP_GetProteinCollectionMemberCount(proteinCollectionID)
    End Function

    Protected Function AddCollectionOrganismXref(proteinCollectionID As Integer, OrganismID As Integer) As Integer Implements IAddUpdateEntries.AddCollectionOrganismXref
        Return RunSP_AddCollectionOrganismXref(proteinCollectionID, OrganismID)
    End Function

    Protected Function AddProteinCollectionMember(
        referenceID As Integer,
        proteinID As Integer,
        sorting_Index As Integer,
        proteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.AddProteinCollectionMember

        Return RunSP_AddProteinCollectionMember(referenceID, proteinID, sorting_Index, proteinCollectionID)
    End Function

    Protected Function UpdateProteinCollectionMember(
        referenceID As Integer,
        proteinID As Integer,
        sorting_Index As Integer,
        proteinCollectionID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinCollectionMember

        Return RunSP_UpdateProteinCollectionMember(referenceID, proteinID, sorting_Index, proteinCollectionID)

    End Function

    Protected Function AddProteinReference(
      proteinName As String,
      description As String,
      organismID As Integer,
      authorityID As Integer,
      proteinID As Integer,
      maxProteinNameLength As Integer) As Integer Implements IAddUpdateEntries.AddProteinReference

        Dim ref_ID As Integer

        ref_ID = (RunSP_AddProteinReference(proteinName, description, organismID, authorityID, proteinID, maxProteinNameLength))
        Return ref_ID

    End Function

    Protected Function AddFileAuthenticationHash(
      proteinCollectionID As Integer,
      authenticationHash As String,
      numProteins As Integer,
      totalResidues As Integer) As Integer Implements IAddUpdateEntries.AddAuthenticationHash

        Return RunSP_AddCRC32FileAuthentication(proteinCollectionID, authenticationHash, numProteins, totalResidues)

    End Function

    Protected Function UpdateProteinNameHash(
      referenceID As Integer,
      proteinName As String,
      description As String,
      proteinID As Integer) As Integer Implements IAddUpdateEntries.UpdateProteinNameHash

        Return RunSP_UpdateProteinNameHash(referenceID, proteinName, description, proteinID)
    End Function

    Protected Function UpdateProteinSequenceHash(
      proteinID As Integer,
      proteinSequence As String) As Integer Implements IAddUpdateEntries.UpdateProteinSequenceHash

        Return RunSP_UpdateProteinSequenceHash(proteinID, proteinSequence)

    End Function

    Protected Function GenerateHash(sourceText As String) As String Implements IAddUpdateEntries.GenerateArbitraryHash
        'Create an encoding object to ensure the encoding standard for the source text
        Dim encoding As New Text.ASCIIEncoding

        'Retrieve a byte array based on the source text
        Dim byteSourceText() As Byte = encoding.GetBytes(sourceText)

        'Compute the hash value from the source
        Dim sha1_hash() As Byte = m_Hasher.ComputeHash(byteSourceText)

        'And convert it to String format for return
        Dim sha1string As String = clsRijndaelEncryptionHandler.ToHexString(sha1_hash)

        Return sha1string
    End Function


#Region " Stored Procedure Access "

    Protected Function RunSP_GetProteinCollectionState(
        proteinCollectionID As Integer) As String

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

        Dim stateName = CStr(sp_Save.Parameters("@State_Name").Value)

        Return stateName


    End Function

    Protected Function RunSP_AddProteinSequence(
      sequence As String,
      length As Integer,
      molecularFormula As String,
      monoisotopicMass As Double,
      averageMass As Double,
      sha1_Hash As String,
      isEncrypted As Boolean,
      mode As IAddUpdateEntries.SPModes) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim encryptionFlag = 0
        If isEncrypted Then
            encryptionFlag = 1
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
      proteinID As Integer,
      sequence As String,
      length As Integer,
      molecularFormula As String,
      monoisotopicMass As Double,
      averageMass As Double,
      sha1_Hash As String) As Integer

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
      fileName As String,
      description As String,
      collectionSource As String,
      collectionType As IAddUpdateEntries.CollectionTypes,
      collectionState As IAddUpdateEntries.CollectionStates,
      annotationTypeID As Integer,
      numProteins As Integer,
      numResidues As Integer,
      mode As IAddUpdateEntries.SPModes) As Integer


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
      reference_ID As Integer, Protein_ID As Integer,
      sortingIndex As Integer, Protein_Collection_ID As Integer) As Integer

        Return RunSP_AddUpdateProteinCollectionMember(reference_ID, Protein_ID, sortingIndex, Protein_Collection_ID, "Add")

    End Function

    Protected Function RunSP_UpdateProteinCollectionMember(
      reference_ID As Integer, Protein_ID As Integer,
      sortingIndex As Integer, Protein_Collection_ID As Integer) As Integer

        Return RunSP_AddUpdateProteinCollectionMember(reference_ID, Protein_ID, sortingIndex, Protein_Collection_ID, "Update")

    End Function

    Protected Function RunSP_AddUpdateProteinCollectionMember(
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
      reference_ID As Integer, Protein_ID As Integer,
      sortingIndex As Integer, Protein_Collection_ID As Integer,
      mode As String) As Integer

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
        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("AddUpdateEncryptionMetadata", m_DatabaseAccessor.Connection)
        passphrase As String, protein_Collection_ID As Integer) As Integer

        sp_Save.CommandType = CommandType.StoredProcedure
        Dim phraseHash As String = GenerateHash(passphrase)

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
      shortName As String,
      fullName As String,
      webAddress As String) As Integer

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
      typeName As String,
      description As String,
      example As String,
      authorityID As Integer) As Integer

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
      proteinCollectionID As Integer,
      collectionStateID As Integer) As Integer

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

        Dim sp_Save As SqlClient.SqlCommand
    Protected Function RunSP_GetProteinCollectionMemberCount(proteinCollectionID As Integer) As Integer

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
      protein_Name As String,
      description As String,
      organismID As Integer,
      authorityID As Integer,
      proteinID As Integer,
      maxProteinNameLength As Integer) As Integer

        Dim sp_Save As SqlClient.SqlCommand
        Dim hashableString As String

        If maxProteinNameLength <= 0 Then maxProteinNameLength = 32

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

    Protected Function RunSP_GetProteinCollectionID(fileName As String) As Integer

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
      protein_Collection_ID As Integer,
      authenticationHash As String,
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
      protein_Collection_ID As Integer,
      organismID As Integer) As Integer

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
      reference_ID As Integer,
      protein_Name As String,
      description As String,
      protein_ID As Integer) As Integer

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
        Dim sp_Save As SqlClient.SqlCommand

        sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionCounts", m_DatabaseAccessor.Connection)
      proteinCollectionID As Integer) As Integer

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
        Dim sp_Save As SqlClient.SqlCommand
        Dim tmpGenSHA As String = GenerateHash(Protein_Sequence)

        sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceHash", m_DatabaseAccessor.Connection)

        sp_Save.CommandType = CommandType.StoredProcedure

        'Define parameters
        Dim myParam As SqlClient.SqlParameter

        'Define parameter for sp's return value
        myParam = sp_Save.Parameters.Add("@Return", SqlDbType.Int)
        myParam.Direction = ParameterDirection.ReturnValue
      proteinID As Integer,
      proteinSequence As String) As Integer

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

    Protected Function RunSP_GetProteinIDFromName(proteinName As String) As Integer


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
