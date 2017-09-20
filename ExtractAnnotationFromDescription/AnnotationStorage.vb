Imports System.Collections.Generic

Friend Class AnnotationStorage
    Private m_AnnotationGroups As Dictionary(Of Integer, AnnotationGroup)
    Private m_GroupNameLookup As Dictionary(Of String, Integer)
    Private ReadOnly m_GlobalProteinNameList As New SortedSet(Of String)

    Sub AddAnnotationGroup(GroupID As Integer, groupNameToAdd As String)

        If Me.m_GroupNameLookup Is Nothing Then
            Me.m_GroupNameLookup = New Dictionary(Of String, Integer)
        End If

        If Me.m_AnnotationGroups Is Nothing Then
            Me.m_AnnotationGroups = New Dictionary(Of Integer, AnnotationGroup)
        End If

        Dim newGroup As New AnnotationGroup(groupNameToAdd)
        newGroup.ImportThisGroup = False
        Me.m_AnnotationGroups.Add(GroupID, newGroup)
        Me.m_GroupNameLookup.Add(groupNameToAdd, GroupID)

    End Sub

    Sub ClearAnnotationGroups()
        If Not Me.m_AnnotationGroups Is Nothing Then
            Me.m_AnnotationGroups.Clear()
        End If
        If Not Me.m_GroupNameLookup Is Nothing Then
            Me.m_GroupNameLookup.Clear()
        End If
    End Sub

    Sub AddAnnotation(
        groupID As Integer,
        PrimaryReferenceName As String,
        XRefName As String)

        Dim ag As AnnotationGroup = Me.GetGroup(groupID)
        ag.AddAnnotation(PrimaryReferenceName, XRefName)
        Me.m_AnnotationGroups.Item(groupID) = ag
        If Not Me.m_GlobalProteinNameList.Contains(PrimaryReferenceName) Then
            Me.m_GlobalProteinNameList.Add(PrimaryReferenceName)
        End If
    End Sub

    Sub AddDelimiter(
        groupID As Integer,
        newDelimiter As String)

        Me.GetGroup(groupID).XRefDelimiter = newDelimiter

    End Sub

    Sub SetAnnotationGroupStatus(GroupID As Integer, NewState As Boolean)
        Dim group = Me.m_AnnotationGroups.Item(GroupID)
        group.ImportThisGroup = NewState
        Me.m_AnnotationGroups.Item(GroupID) = group
        group = Nothing
    End Sub

    'Controls the import state of the named annotation group
    Sub SetAnnotationGroupStatus(groupNameToUpdate As String, newStateForGroup As Boolean)
        Dim groupID = Me.m_GroupNameLookup(groupNameToUpdate)
        Me.SetAnnotationGroupStatus(groupID, newStateForGroup)
    End Sub

    Function GetAllPrimaryReferences() As SortedSet(Of String)
        Return Me.m_GlobalProteinNameList
    End Function

    ReadOnly Property GroupCount() As Integer
        Get
            Return Me.m_AnnotationGroups.Count
        End Get
    End Property

    ReadOnly Property Delimiter(GroupID As Integer) As String
        Get
            Return Me.GetGroup(GroupID).XRefDelimiter
        End Get
    End Property

    Property AnnotationAuthorityID(GroupID As Integer) As Integer
        Get
            Return Me.m_AnnotationGroups.Item(GroupID).AnnotationAuthorityID
        End Get
        Set(Value As Integer)
            Me.m_AnnotationGroups.Item(GroupID).AnnotationAuthorityID = Value
        End Set
    End Property

    Property GroupName(GroupID As Integer) As String
        Get
            Return Me.GetGroup(GroupID).GroupName
        End Get
        Set(Value As String)
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

    'Function GetAnnotationGroup(GroupName As String) As Hashtable
    '    Dim groupID As Integer
    '    groupID = Me.m_GroupNameLookup(GroupName)
    '    Return Me.GetAnnotationGroupData(groupID)
    'End Function

    'Function GetAnnotationGroupData(GroupID As Integer) As AnnotationGroup
    '    Return Me.GetGroup(GroupID)
    'End Function

    'Returns hashtable containing all the added primary reference names as keys
    'and SortedSets of their corresponding xref names for the specified
    'Annotation group id
    Function GetAllRawXRefs(GroupID As Integer) As Dictionary(Of String, SortedSet(Of String))
        Return Me.GetGroup(GroupID).GetAllXRefs
    End Function

    'Returns a SortedSet containing all the xref names for the given
    'primary reference name
    Function GetXRefs(
        PrimaryReferenceName As String,
        GroupID As Integer) As SortedSet(Of String)

        Dim group As AnnotationGroup = Me.GetGroup(GroupID)
        Return group.GetXRefs(PrimaryReferenceName)

    End Function

    Function GetGroup(groupid As Integer) As AnnotationGroup
        Dim group As AnnotationGroup
        group = Me.m_AnnotationGroups(groupid)
        Return group

    End Function

End Class
