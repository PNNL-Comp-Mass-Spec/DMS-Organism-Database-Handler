<Obsolete("Unused")>
Friend Class clsAnnotationInfo

    Private ReadOnly m_AnnotationDetails As Dictionary(Of Integer, AnnotationDetails)

    Private m_AuthorityLookup As NameLookups
    Private m_AnnotationGroupLookup As NameLookups

    Sub New()

        Me.m_AnnotationDetails = New Dictionary(Of Integer, AnnotationDetails)
        Me.m_AuthorityLookup = New NameLookups()
        Me.m_AnnotationGroupLookup = New NameLookups()
    End Sub

    Sub AddPrimaryAnnotation(proteinID As Integer,
        protName As String, description As String,
        refID As Integer, namingAuthorityID As Integer)

        Me.m_AnnotationDetails.Add(proteinID,
            New AnnotationDetails(
                protName, description, refID,
                proteinID))
    End Sub



    Sub AddAdditionalAnnotation(
        ProteinID As Integer,
        NewName As String,
        AnnotationGroupID As Integer)

        Dim tmpDetails As AnnotationDetails

        tmpDetails = m_AnnotationDetails(ProteinID)
        tmpDetails.AddNewName(AnnotationGroupID, NewName)

    End Sub

    Sub AddAuthorityNameToLookup(
        AuthorityID As Integer, authName As String)

        Me.m_AuthorityLookup.AddName(AuthorityID, authName)

    End Sub

    Sub AddAnnotationGroupLookup(
        AnnotationGroupCode As Integer,
        AuthorityID As Integer)

        Me.m_AnnotationGroupLookup.AddName(
            AnnotationGroupCode,
            Me.m_AuthorityLookup.GetName(AuthorityID))
    End Sub


    ReadOnly Property ProteinName(
        ProteinID As Integer,
        AnnotationGroupCode As Integer) As String
        Get
            Dim details = m_AnnotationDetails.Item(ProteinID)
            Return details.Name(AnnotationGroupCode)
        End Get
    End Property

    ReadOnly Property ReferenceID(
        ProteinID As Integer,
        AnnotationGroupCode As Integer) As Integer
        Get
            Dim details = m_AnnotationDetails.Item(ProteinID)
            Return details.ReferenceID
        End Get
    End Property

    ReadOnly Property AuthorityName(AnnotationGroupCode As Integer) As String
        Get
            Return Me.m_AuthorityLookup.GetName(AnnotationGroupCode)
        End Get
    End Property

    Structure NameLookups
        Private Names As Dictionary(Of Integer, String)

        Sub AddName(
            ID As Integer,
            Name As String)

            If Names Is Nothing Then
                Names = New Dictionary(Of Integer, String)
            End If

            Names.Add(ID, Name)
        End Sub

        Function GetName(ID As Integer) As String
            If Me.Names.ContainsKey(ID) Then
                Return Me.Names(ID)
            Else
                Return String.Empty
            End If
        End Function

    End Structure

    Structure AnnotationDetails
        Friend Description As String
        Friend ReferenceID As Integer
        Friend ProteinID As Integer
        Friend NamingAuthorityID As Integer

        Friend Names As Dictionary(Of Integer, String)

        'Key is AnnotationGroupID, Value is Name

        Sub New(
            PrimaryName As String,
            Description As String,
            ReferenceID As Integer,
            ProteinID As Integer)

            Me.Description = Description
            Me.ReferenceID = ReferenceID
            Me.ProteinID = ProteinID

            Me.Names = New Dictionary(Of Integer, String)
            Me.Names.Add(0, PrimaryName)
        End Sub

        Sub AddNewName(annotationGroupId As Integer, annotationName As String)
            If Not Me.Names.ContainsValue(annotationName) Then
                Me.Names.Add(annotationGroupId, annotationName)
            End If
        End Sub

        ReadOnly Property PrimaryName As String
            Get
                Return Me.Names(0).ToString
            End Get
        End Property

        ReadOnly Property Name(annotationGroupCode As Integer) As String
            Get
                Return Me.Names(annotationGroupCode)
            End Get
        End Property

    End Structure

End Class
