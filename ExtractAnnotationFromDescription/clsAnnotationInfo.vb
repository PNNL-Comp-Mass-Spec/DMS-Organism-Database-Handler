Friend Class clsAnnotationInfo

    Private m_AnnotationDetails As Hashtable

    Private m_ProteinCollectionID As Integer

    Private m_ProteinCollectionName As String
    Private m_AuthorityLookup As NameLookups
    Private m_AnnotationGroupLookup As NameLookups

    Sub New( _
        ByVal ProteinCollectionName As String, _
        ByVal ProteinCollectionID As Integer)

        Me.m_AnnotationDetails = New Hashtable
        Me.m_AuthorityLookup = New NameLookups
        Me.m_AnnotationGroupLookup = New NameLookups
    End Sub



    Sub AddPrimaryAnnotation(ByVal ProteinID As Integer, _
        ByVal ProteinName As String, ByVal Description As String, _
        ByVal ReferenceID As Integer, ByVal NamingAuthorityID As Integer)

        Me.m_AnnotationDetails.Add(ProteinID, _
            New AnnotationDetails( _
                ProteinName, Description, ReferenceID, _
                ProteinID, NamingAuthorityID))
    End Sub



    Sub AddAdditionalAnnotation( _
        ByVal ProteinID As Integer, _
        ByVal NewName As String, _
        ByVal AnnotationGroupID As Integer)

        Dim tmpDetails As AnnotationDetails

        tmpDetails = DirectCast(Me.m_AnnotationDetails(ProteinID.ToString), AnnotationDetails)
        tmpDetails.AddNewName(AnnotationGroupID, NewName)

    End Sub

    Sub AddAuthorityNameToLookup( _
        ByVal AuthorityID As Integer, ByVal AuthorityName As String)

        Me.m_AuthorityLookup.AddName(AuthorityID, AuthorityName)

    End Sub

    Sub AddAnnotationGroupLookup( _
        ByVal AnnotationGroupCode As Integer, _
        ByVal AuthorityID As Integer)

        Me.m_AnnotationGroupLookup.AddName( _
            AnnotationGroupCode, _
            Me.m_AuthorityLookup.GetName(AuthorityID))
    End Sub


    ReadOnly Property ProteinName( _
        ByVal ProteinID As Integer, _
        ByVal AnnotationGroupCode As Integer) As String
        Get
            Dim details As clsAnnotationInfo.AnnotationDetails
            details = DirectCast(Me.m_AnnotationDetails.Item(ProteinID.ToString), _
                clsAnnotationInfo.AnnotationDetails)
            Return details.Name(AnnotationGroupCode).ToString
            details = Nothing
        End Get
    End Property

    ReadOnly Property ReferenceID( _
        ByVal ProteinID As Integer, _
        ByVal AnnotationGroupCode As Integer) As Integer
        Get
            Dim details As clsAnnotationInfo.AnnotationDetails
            details = DirectCast(Me.m_AnnotationDetails.Item(ProteinID.ToString), _
                clsAnnotationInfo.AnnotationDetails)
            Return details.ReferenceID
            details = Nothing
        End Get
    End Property

    ReadOnly Property AuthorityName(ByVal AnnotationGroupCode As Integer) As String
        Get
            Return Me.m_AuthorityLookup.GetName(AnnotationGroupCode)
        End Get
    End Property

    Structure NameLookups
        Private Names As Hashtable

        Sub AddName( _
            ByVal ID As Integer, _
            ByVal Name As String)

            If Names Is Nothing Then
                Names = New Hashtable
            End If

            Names.Add(ID, Name)
        End Sub

        Function GetName(ByVal ID As Integer) As String
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

        Sub New( _
            ByVal PrimaryName As String, _
            ByVal Description As String, _
            ByVal ReferenceID As Integer, _
            ByVal ProteinID As Integer, _
            ByVal NamingAuthorityID As Integer)

            Me.Description = Description
            Me.ReferenceID = ReferenceID
            Me.ProteinID = ProteinID

            Me.Names = New Hashtable
            Me.Names.Add(0, PrimaryName)
        End Sub

        Sub AddNewName(ByVal AnnotationGroup As Integer, ByVal Name As String)
            If Not Me.Names.Contains(Name) Then
                Me.Names.Add(AnnotationGroup, Name)
            End If
        End Sub

        ReadOnly Property PrimaryName() As String
            Get
                Return Me.Names(0).ToString
            End Get
        End Property

        ReadOnly Property Name(ByVal AnnotationGroupCode As Integer) As String
            Get
                Return Me.Names(AnnotationGroupCode).ToString
            End Get
        End Property

    End Structure

End Class
