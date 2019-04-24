Friend Class clsAnnotationInfo

    Private ReadOnly m_AnnotationDetails As Hashtable

    Private m_AuthorityLookup As NameLookups
    Private m_AnnotationGroupLookup As NameLookups

    Sub New()

        Me.m_AnnotationDetails = New Hashtable
        Me.m_AuthorityLookup = New NameLookups
        Me.m_AnnotationGroupLookup = New NameLookups
    End Sub
    
    Sub AddPrimaryAnnotation(ProteinID As Integer,
        protName As String, Description As String,
        refID As Integer, NamingAuthorityID As Integer)

        Me.m_AnnotationDetails.Add(ProteinID,
            New AnnotationDetails(
                protName, Description, refID,
                ProteinID))
    End Sub



    Sub AddAdditionalAnnotation(
        ProteinID As Integer,
        NewName As String,
        AnnotationGroupID As Integer)

        Dim tmpDetails As AnnotationDetails

        tmpDetails = DirectCast(Me.m_AnnotationDetails(ProteinID.ToString), AnnotationDetails)
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
            Dim details = DirectCast(Me.m_AnnotationDetails.Item(ProteinID.ToString), AnnotationDetails)
            Return details.Name(AnnotationGroupCode).ToString
        End Get
    End Property

    ReadOnly Property ReferenceID(
        ProteinID As Integer,
        AnnotationGroupCode As Integer) As Integer
        Get
            Dim details = DirectCast(Me.m_AnnotationDetails.Item(ProteinID.ToString), AnnotationDetails)
            Return details.ReferenceID
        End Get
    End Property

    ReadOnly Property AuthorityName(AnnotationGroupCode As Integer) As String
        Get
            Return Me.m_AuthorityLookup.GetName(AnnotationGroupCode)
        End Get
    End Property

    Structure NameLookups
        Private Names As Hashtable

        Sub AddName(
            ID As Integer,
            Name As String)

            If Names Is Nothing Then
                Names = New Hashtable
            End If

            Names.Add(ID, Name)
        End Sub

        Function GetName(ID As Integer) As String
            If Me.Names.Contains(ID) Then
                Return Me.Names.Item(ID.ToString).ToString
            Else
                Return ""
            End If
        End Function

    End Structure

    Structure AnnotationDetails
        Friend Description As String
        Friend ReferenceID As Integer
        Friend ProteinID As Integer
        Friend NamingAuthorityID As Integer

        Friend Names As Hashtable
        'Key is AnnotationGroupID, Value is Name

        Sub New(
            PrimaryName As String,
            Description As String,
            ReferenceID As Integer,
            ProteinID As Integer)

            Me.Description = Description
            Me.ReferenceID = ReferenceID
            Me.ProteinID = ProteinID

            Me.Names = New Hashtable
            Me.Names.Add(0, PrimaryName)
        End Sub

        Sub AddNewName(AnnotationGroup As Integer, annotationName As String)
            If Not Me.Names.Contains(annotationName) Then
                Me.Names.Add(AnnotationGroup, annotationName)
            End If
        End Sub

        ReadOnly Property PrimaryName As String
            Get
                Return Me.Names(0).ToString
            End Get
        End Property

        ReadOnly Property Name(AnnotationGroupCode As Integer) As String
            Get
                Return Me.Names(AnnotationGroupCode).ToString
            End Get
        End Property

    End Structure

End Class
