Friend Class AnnotationGroup
    Private m_AnnotationAuthorityID As Integer
    Private m_GroupName As String
    Private m_ImportThisGroup As Boolean
    Private m_AnnotationData As Hashtable       'key=PrimaryReferenceName, value=arraylist of xrefs
    Private m_Delimiter As String

    Sub New(ByVal GroupName As String)
        Me.m_GroupName = GroupName
    End Sub

    Property GroupName() As String
        Get
            Return Me.m_GroupName
        End Get
        Set(ByVal Value As String)
            Me.m_GroupName = Value
        End Set
    End Property

    Property AnnotationAuthorityID() As Integer
        Get
            Return Me.m_AnnotationAuthorityID
        End Get
        Set(ByVal Value As Integer)
            Me.m_AnnotationAuthorityID = Value
        End Set
    End Property

    Property XRefDelimiter() As String
        Get
            Return Me.m_Delimiter
        End Get
        Set(ByVal Value As String)
            Me.m_Delimiter = Value
        End Set
    End Property


    Property ImportThisGroup() As Boolean
        Get
            Return Me.m_ImportThisGroup
        End Get
        Set(ByVal Value As Boolean)
            Me.m_ImportThisGroup = Value
        End Set
    End Property

    Sub AddAnnotation( _
        ByVal PrimaryReferenceName As String, _
        ByVal XRefName As String)

        Dim xrefList As ArrayList

        If Me.m_AnnotationData Is Nothing Then
            Me.m_AnnotationData = New Hashtable
        End If

        If Not Me.m_AnnotationData.Contains(PrimaryReferenceName) Then
            xrefList = New ArrayList
            xrefList.Add(XRefName)
            xrefList = Nothing
        Else
            xrefList = DirectCast(Me.m_AnnotationData.Item(PrimaryReferenceName.ToString), ArrayList)
            If Not xrefList.Contains(XRefName) Then
                xrefList.Add(XRefName)
                Me.m_AnnotationData.Item(PrimaryReferenceName.ToString) = xrefList
            End If
            xrefList = Nothing
        End If

    End Sub

    Function GetAllXRefs() As Hashtable
        Return Me.m_AnnotationData
    End Function

    Function GetAllPrimaryReferences() As ArrayList
        Dim s As String
        Dim al As New ArrayList(Me.m_AnnotationData.Count)
        For Each s In Me.m_AnnotationData.Keys
            al.Add(s)
        Next

        al.Sort()

        Return al
    End Function

    Function GetXRefs(ByVal PrimaryReferenceName As String) As ArrayList
        Dim xrefList As ArrayList
        xrefList = DirectCast(Me.m_AnnotationData.Item(PrimaryReferenceName), ArrayList)

        If Me.m_Delimiter.Length > 0 Then
            Dim addnXRefs() As String
            Dim primeXRef As String
            Dim XRefCount As Integer
            Dim newXReflist As New ArrayList

            For Each primeXRef In xrefList
                addnXRefs = primeXRef.Split(Me.m_Delimiter.ToCharArray)
                For XRefCount = 0 To addnXRefs.Length - 1
                    newXReflist.Add(addnXRefs(XRefCount).ToString)
                Next
            Next
            xrefList.Clear()
            xrefList = newXReflist

        End If
        Return xrefList
    End Function

End Class
