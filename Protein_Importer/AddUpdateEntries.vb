Imports System.Collections.Generic
Imports PRISMDatabaseUtils
Imports Protein_Exporter

Public Class AddUpdateEntries

    Public Enum SPModes
        add
        'Unused: update
    End Enum

    Public Enum CollectionStates
        NewEntry = 1
        'Unused: Provisional = 2
        'Unused: Production = 3
        'Unused: Historical = 4
    End Enum

    Public Enum CollectionTypes
        prot_original_source = 1
        'Unused: modified_source = 2
        'Unused: runtime_combined_collection = 3
        'Unused: loadtime_combined_collection = 4
        'Unused: nuc_original_source = 5
    End Enum

    Protected ReadOnly m_DatabaseAccessor As TableManipulationBase.DBTask
    'Unused: Protected m_OrganismID As Integer
    'Unused: Protected m_ProteinLengths As Hashtable
    Protected m_MaxProteinNameLength As Integer

    Protected m_Hasher As Security.Cryptography.SHA1Managed
    'Unused: Protected ProteinHashThread As Threading.Thread
    'Unused: Protected ReferenceHashThread As Threading.Thread

#Region "Properties"
    Public Property MaximumProteinNameLength As Integer
        Get
            Return m_MaxProteinNameLength
        End Get
        Set
            m_MaxProteinNameLength = Value
        End Set
    End Property
#End Region

#Region " Events "

    Public Event LoadStart(taskTitle As String)
    Public Event LoadEnd()
    Public Event LoadProgress(fractionDone As Double)

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
        m_DatabaseAccessor = New TableManipulationBase.DBTask(PISConnectionString)
        m_Hasher = New Security.Cryptography.SHA1Managed
    End Sub

    <Obsolete("No longer used")>
    Public Sub CloseConnection()

    End Sub

    ''' <summary>
    ''' Checks for the existence of protein sequences in the T_Proteins table
    ''' Gets Protein_ID if located, makes a new entry if not
    ''' Updates Protein_ID field in ProteinStorageEntry instance
    ''' </summary>
    ''' <param name="pc"></param>
    ''' <param name="selectedProteinList"></param>
    ''' <remarks></remarks>
    Public Sub CompareProteinID(
        pc As Protein_Storage.ProteinStorage,
        selectedProteinList As List(Of String))

        Dim tmpPC As Protein_Storage.ProteinStorageEntry

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

    Public Sub UpdateProteinNames(
        pc As Protein_Storage.ProteinStorage,
        selectedProteinList As List(Of String),
        organismID As Integer,
        authorityID As Integer)

        OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection")
        Dim tmpPC As Protein_Storage.ProteinStorageEntry
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

    Public Sub UpdateProteinCollectionMembers(
        ProteinCollectionID As Integer,
        pc As Protein_Storage.ProteinStorage,
        selectedProteinList As List(Of String),
        numProteinsExpected As Integer,
        numResiduesExpected As Integer)

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
            Dim tmpPC As Protein_Storage.ProteinStorageEntry = pc.GetProtein(s)
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

    Public Function GetTotalResidueCount(
      proteinCollection As Protein_Storage.ProteinStorage,
      selectedProteinList As List(Of String)) As Integer
        Dim s As String
        Dim totalLength As Integer
        Dim tmpPC As Protein_Storage.ProteinStorageEntry

        For Each s In selectedProteinList
            tmpPC = proteinCollection.GetProtein(s)
            totalLength += tmpPC.Sequence.Length
        Next

        Return totalLength
    End Function


    Protected Function AddProteinSequence(protein As Protein_Storage.ProteinStorageEntry) As Integer
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
                SPModes.add)
        End With

        Return protein_id

    End Function

    Public Function UpdateProteinSequenceInfo(
      proteinID As Integer,
      sequence As String,
      length As Integer,
      molecularFormula As String,
      monoisotopicMass As Double,
      averageMass As Double,
      sha1Hash As String) As Integer

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

    Public Function AddNamingAuthority(
      shortName As String,
      fullName As String,
      webAddress As String) As Integer

        Dim tmpAuthID As Integer

        tmpAuthID = RunSP_AddNamingAuthority(
            shortName,
            fullName,
            webAddress)

        Return tmpAuthID

    End Function

    Public Function AddAnnotationType(
        typeName As String,
        description As String,
        example As String,
        authorityID As Integer) As Integer

        Dim tmpAnnTypeID As Integer

        tmpAnnTypeID = RunSP_AddAnnotationType(
            typeName, description, example, authorityID)

        Return tmpAnnTypeID

    End Function

    Public Function MakeNewProteinCollection(
      proteinCollectionName As String,
      description As String,
      collectionSource As String,
      collectionType As CollectionTypes,
      annotationTypeID As Integer,
      numProteins As Integer,
      numResidues As Integer) As Integer

        Dim tmpProteinCollectionID As Integer

        tmpProteinCollectionID = RunSP_AddUpdateProteinCollection(
            proteinCollectionName, description, collectionSource, collectionType, CollectionStates.NewEntry,
            annotationTypeID, numProteins, numResidues, SPModes.add)

        Return tmpProteinCollectionID

    End Function

    Public Function UpdateEncryptionMetadata(
      proteinCollectionID As Integer,
      passphrase As String) As Integer


        Return RunSP_AddUpdateEncryptionMetadata(passphrase, proteinCollectionID)

    End Function

    Public Function UpdateProteinCollectionState(
      proteinCollectionID As Integer,
      collectionStateID As Integer) As Integer

        Return RunSP_UpdateProteinCollectionStates(proteinCollectionID, collectionStateID)

    End Function

    Public Function GetProteinCollectionState(proteinCollectionID As Integer) As String

        Return RunSP_GetProteinCollectionState(proteinCollectionID)

    End Function

    Protected Function GetProteinID(entry As Protein_Storage.ProteinStorageEntry, hitsTable As DataTable) As Integer
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

    Public Function GetProteinIDFromName(proteinName As String) As Integer
        Return RunSP_GetProteinIDFromName(proteinName)
    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="ProteinCollectionID"></param>
    ''' <remarks></remarks>
    Public Sub DeleteProteinCollectionMembers(proteinCollectionID As Integer, numProteins As Integer)
        RunSP_DeleteProteinCollectionMembers(proteinCollectionID, numProteins)
    End Sub

    Public Function GetProteinCollectionID(proteinCollectionName As String) As Integer
        Return RunSP_GetProteinCollectionID(proteinCollectionName)
    End Function

    Public Function CountProteinCollectionMembers(proteinCollectionID As Integer) As Integer
        Return RunSP_GetProteinCollectionMemberCount(proteinCollectionID)
    End Function

    Public Function AddCollectionOrganismXref(proteinCollectionID As Integer, OrganismID As Integer) As Integer
        Return RunSP_AddCollectionOrganismXref(proteinCollectionID, OrganismID)
    End Function

    Public Function AddProteinCollectionMember(
        referenceID As Integer,
        proteinID As Integer,
        sorting_Index As Integer,
        proteinCollectionID As Integer) As Integer

        Return RunSP_AddProteinCollectionMember(referenceID, proteinID, sorting_Index, proteinCollectionID)
    End Function

    Public Function UpdateProteinCollectionMember(
        referenceID As Integer,
        proteinID As Integer,
        sorting_Index As Integer,
        proteinCollectionID As Integer) As Integer

        Return RunSP_UpdateProteinCollectionMember(referenceID, proteinID, sorting_Index, proteinCollectionID)

    End Function

    Public Function AddProteinReference(
      proteinName As String,
      description As String,
      organismID As Integer,
      authorityID As Integer,
      proteinID As Integer,
      maxProteinNameLength As Integer) As Integer

        Dim ref_ID As Integer

        ref_ID = (RunSP_AddProteinReference(proteinName, description, organismID, authorityID, proteinID, maxProteinNameLength))
        Return ref_ID

    End Function

    Public Function AddAuthenticationHash(
      proteinCollectionID As Integer,
      authenticationHash As String,
      numProteins As Integer,
      totalResidues As Integer) As Integer

        Return RunSP_AddCRC32FileAuthentication(proteinCollectionID, authenticationHash, numProteins, totalResidues)

    End Function

    Public Function UpdateProteinNameHash(
      referenceID As Integer,
      proteinName As String,
      description As String,
      proteinID As Integer) As Integer

        Return RunSP_UpdateProteinNameHash(referenceID, proteinName, description, proteinID)
    End Function

    Public Function UpdateProteinSequenceHash(
      proteinID As Integer,
      proteinSequence As String) As Integer

        Return RunSP_UpdateProteinSequenceHash(proteinID, proteinSequence)

    End Function

    Public Function GenerateHash(sourceText As String) As String
        'Create an encoding object to ensure the encoding standard for the source text
        Dim encoding As New Text.ASCIIEncoding

        'Retrieve a byte array based on the source text
        Dim byteSourceText() As Byte = encoding.GetBytes(sourceText)

        'Compute the hash value from the source
        Dim sha1_hash() As Byte = m_Hasher.ComputeHash(byteSourceText)

        'And convert it to String format for return
        Dim sha1string As String = RijndaelEncryptionHandler.ToHexString(sha1_hash)

        Return sha1string
    End Function


#Region " Stored Procedure Access "

    Protected Function RunSP_GetProteinCollectionState(
        proteinCollectionID As Integer) As String

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("GetProteinCollectionState", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID

        Dim stateNameParam = dbTools.AddParameter(cmdSave, "@State_Name", SqlType.VarChar, 32, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Dim stateName = dbTools.GetString(stateNameParam.Value)

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
      mode As AddUpdateEntries.SPModes) As Integer

        Dim encryptionFlag = 0
        If isEncrypted Then
            encryptionFlag = 1
        End If

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddProteinSequence", CommandType.StoredProcedure)

        ' Use a 5 minute timeout
        cmdSave.CommandTimeout = 300

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence
        dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length
        dbTools.AddParameter(cmdSave, "@molecular_formula", SqlType.VarChar, 128).Value = molecularFormula
        dbTools.AddParameter(cmdSave, "@monoisotopic_mass", SqlType.Float, 8).Value = monoisotopicMass
        dbTools.AddParameter(cmdSave, "@average_mass", SqlType.Float, 8).Value = averageMass
        dbTools.AddParameter(cmdSave, "@sha1_hash", SqlType.VarChar, 40).Value = sha1_Hash
        dbTools.AddParameter(cmdSave, "@is_encrypted", SqlType.TinyInt).Value = encryptionFlag
        dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinSequenceInfo(
      proteinID As Integer,
      sequence As String,
      length As Integer,
      molecularFormula As String,
      monoisotopicMass As Double,
      averageMass As Double,
      sha1_Hash As String) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("UpdateProteinSequenceInfo", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Protein_ID", SqlType.Int).Value = proteinID
        dbTools.AddParameter(cmdSave, "@sequence", SqlType.Text).Value = sequence
        dbTools.AddParameter(cmdSave, "@length", SqlType.Int).Value = length
        dbTools.AddParameter(cmdSave, "@molecular_formula", SqlType.VarChar, 128).Value = molecularFormula
        dbTools.AddParameter(cmdSave, "@monoisotopic_mass", SqlType.Float, 8).Value = monoisotopicMass
        dbTools.AddParameter(cmdSave, "@average_mass", SqlType.Float, 8).Value = averageMass
        dbTools.AddParameter(cmdSave, "@sha1_hash", SqlType.VarChar, 40).Value = sha1_Hash
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddUpdateProteinCollection(
      proteinCollectionName As String,
      description As String,
      collectionSource As String,
      collectionType As CollectionTypes,
      collectionState As CollectionStates,
      annotationTypeID As Integer,
      numProteins As Integer,
      numResidues As Integer,
      mode As SPModes) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddUpdateProteinCollection", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        ' Note that the @fileName parameter is actually the protein collection name; not the original .fasta file name
        dbTools.AddParameter(cmdSave, "@fileName", SqlType.VarChar, 128).Value = proteinCollectionName
        dbTools.AddParameter(cmdSave, "@Description", SqlType.VarChar, 900).Value = description
        dbTools.AddParameter(cmdSave, "@collectionSource", SqlType.VarChar, 900).Value = collectionSource
        dbTools.AddParameter(cmdSave, "@collection_type", SqlType.Int).Value = CInt(collectionType)
        dbTools.AddParameter(cmdSave, "@collection_state", SqlType.Int).Value = CInt(collectionState)
        dbTools.AddParameter(cmdSave, "@primary_annotation_type_id", SqlType.Int).Value = CInt(annotationTypeID)
        dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins
        dbTools.AddParameter(cmdSave, "@numResidues", SqlType.Int).Value = numResidues
        dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 12).Value = mode.ToString
        Dim messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 512, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        If ret = 0 Then
            ' A zero was returned for the protein collection ID; this indicates and error
            ' Raise an exception

            Dim msg = "AddUpdateProteinCollection returned 0 for the Protein Collection ID"

            Dim spMsg = dbTools.GetString(messageParam.Value)

            If Not String.IsNullOrEmpty(SPMsg) Then msg &= "; " & SPMsg

            Throw New ConstraintException(msg)

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
      reference_ID As Integer, Protein_ID As Integer,
      sortingIndex As Integer, Protein_Collection_ID As Integer,
      mode As String) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddUpdateProteinCollectionMember_New", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@reference_ID", SqlType.Int).Value = reference_ID
        dbTools.AddParameter(cmdSave, "@protein_ID", SqlType.Int).Value = Protein_ID
        dbTools.AddParameter(cmdSave, "@sorting_index", SqlType.Int).Value = sortingIndex
        dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = Protein_Collection_ID
        dbTools.AddParameter(cmdSave, "@mode", SqlType.VarChar, 10).Value = mode
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddUpdateEncryptionMetadata(
        passphrase As String, protein_Collection_ID As Integer) As Integer

        Dim phraseHash As String = GenerateHash(passphrase)

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddUpdateEncryptionMetadata", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Protein_Collection_ID", SqlType.Int).Value = protein_Collection_ID
        dbTools.AddParameter(cmdSave, "@Encryption_Passphrase", SqlType.VarChar, 64).Value = passphrase
        dbTools.AddParameter(cmdSave, "@Passphrase_SHA1_Hash", SqlType.VarChar, 40).Value = phraseHash
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddNamingAuthority(
      shortName As String,
      fullName As String,
      webAddress As String) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddNamingAuthority", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = shortName
        dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = fullName
        dbTools.AddParameter(cmdSave, "@web_address", SqlType.VarChar, 128).Value = webAddress
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddAnnotationType(
      typeName As String,
      description As String,
      example As String,
      authorityID As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddAnnotationType", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 64).Value = typeName
        dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 128).Value = description
        dbTools.AddParameter(cmdSave, "@example", SqlType.VarChar, 128).Value = example
        dbTools.AddParameter(cmdSave, "@authID", SqlType.Int).Value = authorityID
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionStates(
      proteinCollectionID As Integer,
      collectionStateID As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("UpdateProteinCollectionState", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@protein_collection_ID", SqlType.Int).Value = proteinCollectionID
        dbTools.AddParameter(cmdSave, "@state_ID", SqlType.Int).Value = collectionStateID
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    ''' <summary>
    ''' Deletes the proteins for the given protein collection in preparation for re-uploading the proteins
    ''' </summary>
    ''' <param name="proteinCollectionID"></param>
    ''' <param name="numProteinsForReLoad">The number of proteins that will be uploaded after this delete</param>
    ''' <remarks>NumResidues in T_Protein_Collections is set to 0</remarks>
    Protected Function RunSP_DeleteProteinCollectionMembers(proteinCollectionID As Integer, numProteinsForReLoad As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("DeleteProteinCollectionMembers", CommandType.StoredProcedure)

        ' Use a 10 minute timeout
        cmdSave.CommandTimeout = 600

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID
        dbTools.AddParameter(cmdSave, "@NumProteinsForReLoad", SqlType.Int).Value = numProteinsForReLoad
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_GetProteinCollectionMemberCount(proteinCollectionID As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("GetProteinCollectionMemberCount", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddProteinReference(
      protein_Name As String,
      description As String,
      organismID As Integer,
      authorityID As Integer,
      proteinID As Integer,
      maxProteinNameLength As Integer) As Integer

        If maxProteinNameLength <= 0 Then maxProteinNameLength = 32

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddProteinReference", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 128).Value = protein_Name
        dbTools.AddParameter(cmdSave, "@description", SqlType.VarChar, 900).Value = description

        'TODO (org fix) Remove this reference and fix associated stored procedure
        'myParam = dbTools.AddParameter(cmdSave, "@organism_ID", SqlType.Int)
        'myParam.Direction = ParameterDirection.Input
        'myParam.Value = OrganismID

        dbTools.AddParameter(cmdSave, "@authority_ID", SqlType.Int).Value = authorityID
        dbTools.AddParameter(cmdSave, "@protein_ID", SqlType.Int).Value = proteinID

        Dim textToHash = protein_Name + "_" + description + "_" + proteinID.ToString
        dbTools.AddParameter(cmdSave, "@nameDescHash", SqlType.VarChar, 40).Value = GenerateHash(textToHash.ToLower())

        Dim messageParam = dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)
        dbTools.AddParameter(cmdSave, "@MaxProteinNameLength", SqlType.Int).Value = maxProteinNameLength

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        If ret = 0 Then
            ' A zero was returned for the protein reference ID; this indicates an error
            ' Raise an exception

            Dim msg = "AddProteinReference returned 0"

            Dim spMsg = dbTools.GetString(messageParam.Value)

            If Not String.IsNullOrEmpty(spMsg) Then msg &= "; " & spMsg

            Throw New ConstraintException(msg)

        End If

        Return ret


    End Function

    Protected Function RunSP_GetProteinCollectionID(proteinCollectionName As String) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("GetProteinCollectionID", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        ' Note that the @fileName parameter is actually the protein collection name; not the original .fasta file name
        dbTools.AddParameter(cmdSave, "@fileName", SqlType.VarChar, 128).Value = proteinCollectionName

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret


    End Function

    Protected Function RunSP_AddCRC32FileAuthentication(
      protein_Collection_ID As Integer,
      authenticationHash As String,
      numProteins As Integer,
      totalResidueCount As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddCRC32FileAuthentication", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = protein_Collection_ID
        dbTools.AddParameter(cmdSave, "@CRC32FileHash", SqlType.VarChar, 40).Value = authenticationHash
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)
        dbTools.AddParameter(cmdSave, "@numProteins", SqlType.Int).Value = numProteins
        dbTools.AddParameter(cmdSave, "@totalResidueCount", SqlType.Int).Value = totalResidueCount

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_AddCollectionOrganismXref(
      protein_Collection_ID As Integer,
      organismID As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("AddCollectionOrganismXref", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Protein_Collection_ID", SqlType.Int).Value = protein_Collection_ID
        dbTools.AddParameter(cmdSave, "@Organism_ID", SqlType.Int).Value = organismID
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinNameHash(
      reference_ID As Integer,
      protein_Name As String,
      description As String,
      protein_ID As Integer) As Integer

        Dim tmpHash As String

        tmpHash = protein_Name + "_" + description + "_" + protein_ID.ToString
        Dim tmpGenSHA As String = GenerateHash(tmpHash.ToLower)

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("UpdateProteinNameHash", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Reference_ID", SqlType.Int).Value = reference_ID
        dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 40).Value = tmpGenSHA
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionCounts(
      numProteins As Integer,
      numResidues As Integer,
      proteinCollectionID As Integer) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("UpdateProteinCollectionCounts", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Collection_ID", SqlType.Int).Value = proteinCollectionID
        dbTools.AddParameter(cmdSave, "@NumProteins", SqlType.Int).Value = numProteins
        dbTools.AddParameter(cmdSave, "@NumResidues", SqlType.Int).Value = numResidues
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret
    End Function


    Protected Function RunSP_UpdateProteinSequenceHash(
      proteinID As Integer,
      proteinSequence As String) As Integer

        Dim tmpGenSHA As String = GenerateHash(proteinSequence)

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("UpdateProteinSequenceHash", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@Protein_ID", SqlType.Int).Value = proteinID
        dbTools.AddParameter(cmdSave, "@SHA1Hash", SqlType.VarChar, 40).Value = tmpGenSHA
        dbTools.AddParameter(cmdSave, "@message", SqlType.VarChar, 256, ParameterDirection.Output)

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret

    End Function

    Protected Function RunSP_GetProteinIDFromName(proteinName As String) As Integer

        Dim dbTools = m_DatabaseAccessor.DBTools

        Dim cmdSave = dbTools.CreateCommand("GetProteinIDFromName", CommandType.StoredProcedure)

        ' Define parameter for procedure's return value
        Dim returnParam = dbTools.AddParameter(cmdSave, "@Return", SqlType.Int, ParameterDirection.ReturnValue)

        ' Define parameters for the procedure's arguments
        dbTools.AddParameter(cmdSave, "@name", SqlType.VarChar, 128).Value = proteinName

        ' Execute the sp
        dbTools.ExecuteSP(cmdSave)

        ' Get return value
        Dim ret = dbTools.GetInteger(returnParam.Value)

        Return ret


    End Function

#End Region


End Class
