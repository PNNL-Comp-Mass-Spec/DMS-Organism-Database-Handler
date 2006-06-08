Friend Class AnnotationStorage
    Private m_AnnotationGroups As Hashtable
    Private m_GroupNameLookup As Hashtable
    Private m_GlobalProteinNameList As ArrayList

    Sub AddAnnotationGroup(ByVal GroupID As Integer, ByVal GroupName As String)

        If Me.m_GroupNameLookup Is Nothing Then
            Me.m_GroupNameLookup = New Hashtable
        End If

        If Me.m_AnnotationGroups Is Nothing Then
            Me.m_AnnotationGroups = New Hashtable
        End If

        Dim newGroup As New AnnotationGroup(GroupName)
        newGroup.ImportThisGroup = False
        Me.m_AnnotationGroups.Add(GroupID, newGroup)
        Me.m_GroupNameLookup.Add(GroupID, GroupName)

    End Sub

    Sub ClearAnnotationGroups()
        If Not Me.m_AnnotationGroups Is Nothing Then
            Me.m_AnnotationGroups.Clear()
        End If
        If Not Me.m_GroupNameLookup Is Nothing Then
            Me.m_GroupNameLookup.Clear()
        End If
    End Sub

    Sub AddAnnotation( _
        ByVal groupID As Integer, _
        ByVal PrimaryReferenceName As String, _
        ByVal XRefName As String)

        Dim ag As AnnotationGroup = Me.GetGroup(groupID)
        ag.AddAnnotation(PrimaryReferenceName, XRefName)
        Me.m_AnnotationGroups.Item(groupID) = ag
        If Not Me.m_GlobalProteinNameList.Contains(PrimaryReferenceName) Then
            Me.m_GlobalProteinNameList.Add(PrimaryReferenceName)
        End If
    End Sub

    Sub AddDelimiter( _
        ByVal groupID As Integer, _
        ByVal newDelimiter As String)

        Me.GetGroup(groupID).XRefDelimiter = newDelimiter

    End Sub

    Sub SetAnnotationGroupStatus(ByVal GroupID As Integer, ByVal NewState As Boolean)
        Dim group As AnnotationGroup
        group = DirectCast(Me.m_AnnotationGroups.Item(GroupID), AnnotationGroup)
        group.ImportThisGroup = NewState
        Me.m_AnnotationGroups.Item(GroupID) = group
        group = Nothing
    End Sub

    'Controls the import state of the named annotation group
    Sub SetAnnotationGroupStatus(ByVal GroupName As String, ByVal NewState As Boolean)
        Dim groupID As Integer
        groupID = DirectCast(Me.m_GroupNameLookup(GroupName), Int32)
        Me.SetAnnotationGroupStatus(groupID, NewState)
    End Sub

    Function GetAllPrimaryReferences() As ArrayList
        Return Me.m_GlobalProteinNameList
    End Function

    ReadOnly Property GroupCount() As Integer
        Get
            Return Me.m_AnnotationGroups.Count
        End Get
    End Property

    ReadOnly Property Delimiter(ByVal GroupID As Integer) As String
        Get
            Return Me.GetGroup(GroupID).XRefDelimiter
        End Get
    End Property

    Property AnnotationAuthorityID(ByVal GroupID As Integer) As Integer
        Get
            Return DirectCast(Me.m_AnnotationGroups.Item(GroupID), _
                AnnotationGroup).AnnotationAuthorityID
        End Get
        Set(ByVal Value As Integer)
            DirectCast(Me.m_AnnotationGroups.Item(GroupID), _
                AnnotationGroup).AnnotationAuthorityID = Value
        End Set
    End Property

    Property GroupName(ByVal GroupID As Integer) As String
        Get
            Return Me.GetGroup(GroupID).GroupName
        End Get
        Set(ByVal Value As String)
            Dim oldName As String
            Dim group As AnnotationGroup = Me.GetGroup(GroupID)
            oldName = group.GroupName
            group.GroupName = Value
            Me.m_AnnotationGroups.Item(GroupID) = group
            group = Nothing
            Me.m_GroupNameLookup.Remove(oldName)
            Me.m_GroupNameLookup.Item(Value) = GroupID
        End Set
    End Property

    'Function GetAnnotationGroup(ByVal GroupName As String) As Hashtable
    '    Dim groupID As Integer
    '    groupID = DirectCast(Me.m_GroupNameLookup(GroupName), Int32)
    '    Return Me.GetAnnotationGroupData(groupID)
    'End Function

    'Function GetAnnotationGroupData(ByVal GroupID As Integer) As AnnotationGroup
    '    Return Me.GetGroup(GroupID)
    'End Function

    'Returns hashtable containing all the added primary reference names as keys
    'and arraylists of their corresponding xref names for the specified 
    'Annotation group id
    Function GetAllRawXRefs(ByVal GroupID As Integer) As Hashtable
        Return Me.GetGroup(GroupID).GetAllXRefs
    End Function

    'Returns an arraylist containing all the xref names for the given 
    'primary reference name
    Function GetXRefs( _
        ByVal PrimaryReferenceName As String, _
        ByVal GroupID As Integer) As ArrayList

        Dim group As AnnotationGroup = Me.GetGroup(GroupID)
        Return group.GetXRefs(PrimaryReferenceName)

    End Function

    Function GetGroup(ByVal groupid As Integer) As AnnotationGroup
        Dim group As AnnotationGroup
        group = DirectCast(Me.m_AnnotationGroups(groupid), AnnotationGroup)
        Return group

    End Function

End Class
