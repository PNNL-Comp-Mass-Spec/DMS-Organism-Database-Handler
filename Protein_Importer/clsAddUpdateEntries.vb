Imports System.Collections.Generic
Imports Protein_Exporter

Public Class clsAddUpdateEntries

    Public Enum SPModes
        add
        update
    End Enum

    Public Enum CollectionStates
        NewEntry = 1
        Provisional = 2
        Production = 3
        Historical = 4
    End Enum

    Public Enum CollectionTypes
        prot_original_source = 1
        modified_source = 2
        runtime_combined_collection = 3
        loadtime_combined_collection = 4
        nuc_original_source = 5
    End Enum

    Protected ReadOnly m_DatabaseAccessor As TableManipulationBase.clsDBTask
    Protected m_OrganismID As Integer
    Protected m_ProteinLengths As Hashtable
    Protected m_MaxProteinNameLength As Integer

    Protected m_Hasher As Security.Cryptography.SHA1Managed
    Protected ProteinHashThread As Threading.Thread
    Protected ReferenceHashThread As Threading.Thread

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
    Public Sub CompareProteinID(
        pc As Protein_Storage.clsProteinStorage,
        selectedProteinList As List(Of String))

        Dim tmpPC As Protein_Storage.clsProteinStorageEntry

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
        pc As Protein_Storage.clsProteinStorage,
        selectedProteinList As List(Of String),
        organismID As Integer,
        authorityID As Integer)

        OnLoadStart("Storing Protein Names and Descriptions specific to this protein collection")
        Dim tmpPC As Protein_Storage.clsProteinStorageEntry
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
        pc As Protein_Storage.clsProteinStorage,
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
            Dim tmpPC As Protein_Storage.clsProteinStorageEntry = pc.GetProtein(s)
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
      proteinCollection As Protein_Storage.clsProteinStorage,
      selectedProteinList As List(Of String)) As Integer
        Dim s As String
        Dim totalLength As Integer
        Dim tmpPC As Protein_Storage.clsProteinStorageEntry

        For Each s In selectedProteinList
            tmpPC = proteinCollection.GetProtein(s)
            totalLength += tmpPC.Sequence.Length
        Next

        Return totalLength
    End Function


    Protected Function AddProteinSequence(protein As Protein_Storage.clsProteinStorageEntry) As Integer
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

    Protected Function GetProteinID(entry As Protein_Storage.clsProteinStorageEntry, hitsTable As DataTable) As Integer
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

    Public Function GetProteinCollectionID(filePath As String) As Integer
        Return RunSP_GetProteinCollectionID(IO.Path.GetFileNameWithoutExtension(filePath))
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
        Dim sha1string As String = clsRijndaelEncryptionHandler.ToHexString(sha1_hash)

        Return sha1string
    End Function


#Region " Stored Procedure Access "

    Protected Function RunSP_GetProteinCollectionState(
        proteinCollectionID As Integer) As String

        Dim sp_Save = New SqlClient.SqlCommand("GetProteinCollectionState", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int).Value = proteinCollectionID

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@State_Name", SqlDbType.VarChar, 32).Direction = ParameterDirection.Output

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
      mode As clsAddUpdateEntries.SPModes) As Integer

        Dim encryptionFlag = 0
        If isEncrypted Then
            encryptionFlag = 1
        End If

        ' Use a 5 minute timeout
        Dim sp_Save = New SqlClient.SqlCommand("AddProteinSequence", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure,
            .CommandTimeout = 300
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@sequence", SqlDbType.Text).Value = sequence
        sp_Save.Parameters.Add("@length", SqlDbType.Int).Value = length
        sp_Save.Parameters.Add("@molecular_formula", SqlDbType.VarChar, 128).Value = molecularFormula
        sp_Save.Parameters.Add("@monoisotopic_mass", SqlDbType.Float, 8).Value = monoisotopicMass
        sp_Save.Parameters.Add("@average_mass", SqlDbType.Float, 8).Value = averageMass
        sp_Save.Parameters.Add("@sha1_hash", SqlDbType.VarChar, 40).Value = sha1_Hash
        sp_Save.Parameters.Add("@is_encrypted", SqlDbType.TinyInt).Value = encryptionFlag
        sp_Save.Parameters.Add("@mode", SqlDbType.VarChar, 12).Value = mode.ToString
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

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

        Dim sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceInfo", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Protein_ID", SqlDbType.Int).Value = proteinID
        sp_Save.Parameters.Add("@sequence", SqlDbType.Text).Value = sequence
        sp_Save.Parameters.Add("@length", SqlDbType.Int).Value = length
        sp_Save.Parameters.Add("@molecular_formula", SqlDbType.VarChar, 128).Value = molecularFormula
        sp_Save.Parameters.Add("@monoisotopic_mass", SqlDbType.Float, 8).Value = monoisotopicMass
        sp_Save.Parameters.Add("@average_mass", SqlDbType.Float, 8).Value = averageMass
        sp_Save.Parameters.Add("@sha1_hash", SqlDbType.VarChar, 40).Value = sha1_Hash
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

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

        Dim sp_Save = New SqlClient.SqlCommand("AddUpdateProteinCollection", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        ' Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        ' Define parameters for the procedure's arguments
        ' Note that the @fileName parameter is actually the protein collection name; not the original .fasta file name
        sp_Save.Parameters.Add("@fileName", SqlDbType.VarChar, 128).Value = proteinCollectionName
        sp_Save.Parameters.Add("@Description", SqlDbType.VarChar, 900).Value = description
        sp_Save.Parameters.Add("@collectionSource", SqlDbType.VarChar, 900).Value = collectionSource
        sp_Save.Parameters.Add("@collection_type", SqlDbType.Int).Value = CInt(collectionType)
        sp_Save.Parameters.Add("@collection_state", SqlDbType.Int).Value = CInt(collectionState)
        sp_Save.Parameters.Add("@primary_annotation_type_id", SqlDbType.Int).Value = CInt(annotationTypeID)
        sp_Save.Parameters.Add("@numProteins", SqlDbType.Int).Value = numProteins
        sp_Save.Parameters.Add("@numResidues", SqlDbType.Int).Value = numResidues
        sp_Save.Parameters.Add("@mode", SqlDbType.VarChar, 12).Value = mode.ToString
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 512).Direction = ParameterDirection.Output

        ' Execute the sp
        sp_Save.ExecuteNonQuery()

        ' Get return value
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
      reference_ID As Integer, Protein_ID As Integer,
      sortingIndex As Integer, Protein_Collection_ID As Integer,
      mode As String) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("AddUpdateProteinCollectionMember_New", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@reference_ID", SqlDbType.Int).Value = reference_ID
        sp_Save.Parameters.Add("@protein_ID", SqlDbType.Int).Value = Protein_ID
        sp_Save.Parameters.Add("@sorting_index", SqlDbType.Int).Value = sortingIndex
        sp_Save.Parameters.Add("@protein_collection_ID", SqlDbType.Int).Value = Protein_Collection_ID
        sp_Save.Parameters.Add("@mode", SqlDbType.VarChar, 10).Value = mode
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddUpdateEncryptionMetadata(
        passphrase As String, protein_Collection_ID As Integer) As Integer

        Dim phraseHash As String = GenerateHash(passphrase)

        Dim sp_Save = New SqlClient.SqlCommand("AddUpdateEncryptionMetadata", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Protein_Collection_ID", SqlDbType.Int).Value = protein_Collection_ID
        sp_Save.Parameters.Add("@Encryption_Passphrase", SqlDbType.VarChar, 64).Value = passphrase
        sp_Save.Parameters.Add("@Passphrase_SHA1_Hash", SqlDbType.VarChar, 40).Value = phraseHash
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddNamingAuthority(
      shortName As String,
      fullName As String,
      webAddress As String) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("AddNamingAuthority", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 64).Value = shortName
        sp_Save.Parameters.Add("@description", SqlDbType.VarChar, 128).Value = fullName
        sp_Save.Parameters.Add("@web_address", SqlDbType.VarChar, 128).Value = webAddress
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddAnnotationType(
      typeName As String,
      description As String,
      example As String,
      authorityID As Integer) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("AddAnnotationType", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 64).Value = typeName
        sp_Save.Parameters.Add("@description", SqlDbType.VarChar, 128).Value = description
        sp_Save.Parameters.Add("@example", SqlDbType.VarChar, 128).Value = example
        sp_Save.Parameters.Add("@authID", SqlDbType.Int).Value = authorityID
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionStates(
      proteinCollectionID As Integer,
      collectionStateID As Integer) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionState", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@protein_collection_ID", SqlDbType.Int).Value = proteinCollectionID
        sp_Save.Parameters.Add("@state_ID", SqlDbType.Int).Value = collectionStateID
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

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

        ' Use a 10 minute timeout
        Dim sp_Save = New SqlClient.SqlCommand("DeleteProteinCollectionMembers", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure,
            .CommandTimeout = 600
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int).Value = proteinCollectionID
        sp_Save.Parameters.Add("@NumProteinsForReLoad", SqlDbType.Int).Value = numProteinsForReLoad
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_GetProteinCollectionMemberCount(proteinCollectionID As Integer) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("GetProteinCollectionMemberCount", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int).Value = proteinCollectionID

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

        Dim hashableString As String

        If maxProteinNameLength <= 0 Then maxProteinNameLength = 32

        Dim sp_Save = New SqlClient.SqlCommand("AddProteinReference", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 128).Value = protein_Name
        sp_Save.Parameters.Add("@description", SqlDbType.VarChar, 900).Value = description

        'TODO (org fix) Remove this reference and fix associated Sproc
        'myParam = sp_Save.Parameters.Add("@organism_ID", SqlDbType.Int)
        'myParam.Direction = ParameterDirection.Input
        'myParam.Value = OrganismID

        sp_Save.Parameters.Add("@authority_ID", SqlDbType.Int).Value = authorityID
        sp_Save.Parameters.Add("@protein_ID", SqlDbType.Int).Value = proteinID

        Dim textToHash = protein_Name + "_" + description + "_" + proteinID.ToString
        sp_Save.Parameters.Add("@nameDescHash", SqlDbType.VarChar, 40).Value = GenerateHash(textToHash.ToLower())

        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output
        sp_Save.Parameters.Add("@MaxProteinNameLength", SqlDbType.Int).Value = maxProteinNameLength

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

        Dim sp_Save = New SqlClient.SqlCommand("GetProteinCollectionID", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@fileName", SqlDbType.VarChar, 128).Value = fileName

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

        Dim sp_Save = New SqlClient.SqlCommand("AddCRC32FileAuthentication", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int).Value = protein_Collection_ID
        sp_Save.Parameters.Add("@CRC32FileHash", SqlDbType.VarChar, 40).Value = authenticationHash
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output
        sp_Save.Parameters.Add("@numProteins", SqlDbType.Int).Value = numProteins
        sp_Save.Parameters.Add("@totalResidueCount", SqlDbType.Int).Value = totalResidueCount

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_AddCollectionOrganismXref(
      protein_Collection_ID As Integer,
      organismID As Integer) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("AddCollectionOrganismXref", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Protein_Collection_ID", SqlDbType.Int).Value = protein_Collection_ID
        sp_Save.Parameters.Add("@Organism_ID", SqlDbType.Int).Value = organismID
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

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

        tmpHash = protein_Name + "_" + description + "_" + protein_ID.ToString
        Dim tmpGenSHA As String = GenerateHash(tmpHash.ToLower)

        Dim sp_Save = New SqlClient.SqlCommand("UpdateProteinNameHash", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Reference_ID", SqlDbType.Int).Value = reference_ID
        sp_Save.Parameters.Add("@SHA1Hash", SqlDbType.VarChar, 40).Value = tmpGenSHA
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_UpdateProteinCollectionCounts(
      numProteins As Integer,
      numResidues As Integer,
      proteinCollectionID As Integer) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("UpdateProteinCollectionCounts", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Collection_ID", SqlDbType.Int).Value = proteinCollectionID
        sp_Save.Parameters.Add("@NumProteins", SqlDbType.Int).Value = numProteins
        sp_Save.Parameters.Add("@NumResidues", SqlDbType.Int).Value = numResidues
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret
    End Function


    Protected Function RunSP_UpdateProteinSequenceHash(
      proteinID As Integer,
      proteinSequence As String) As Integer

        Dim tmpGenSHA As String = GenerateHash(proteinSequence)

        Dim sp_Save = New SqlClient.SqlCommand("UpdateProteinSequenceHash", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@Protein_ID", SqlDbType.Int).Value = proteinID
        sp_Save.Parameters.Add("@SHA1Hash", SqlDbType.VarChar, 40).Value = tmpGenSHA
        sp_Save.Parameters.Add("@message", SqlDbType.VarChar, 256).Direction = ParameterDirection.Output

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret

    End Function

    Protected Function RunSP_GetProteinIDFromName(proteinName As String) As Integer

        Dim sp_Save = New SqlClient.SqlCommand("GetProteinIDFromName", m_DatabaseAccessor.Connection) With {
            .CommandType = CommandType.StoredProcedure
        }

        'Define parameter for procedure's return value
        sp_Save.Parameters.Add("@Return", SqlDbType.Int).Direction = ParameterDirection.ReturnValue

        'Define parameters for the procedure's arguments
        sp_Save.Parameters.Add("@name", SqlDbType.VarChar, 128).Value = proteinName

        'Execute the sp
        sp_Save.ExecuteNonQuery()

        'Get return value
        Dim ret = CInt(sp_Save.Parameters("@Return").Value)

        Return ret


    End Function

#End Region


End Class
