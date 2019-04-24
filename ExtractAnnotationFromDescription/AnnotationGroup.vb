Imports System.Collections.Generic

Friend Class AnnotationGroup
    Private m_AnnotationData As Dictionary(Of String, SortedSet(Of String))       'key=PrimaryReferenceName, value=SortedSet of xrefs
    Private m_Delimiter As String

    Sub New(GroupName As String)
        Me.GroupName = GroupName
    End Sub

    Public Property GroupName As String

    Public Property AnnotationAuthorityID As Integer

    Property XRefDelimiter As String
        Get
            Return Me.m_Delimiter
        End Get
        Set
            Me.m_Delimiter = value
        End Set
    End Property

    Public Property ImportThisGroup As Boolean

    Sub AddAnnotation(PrimaryReferenceName As String, XRefName As String)

        Dim xrefList As SortedSet(Of String)

        If Me.m_AnnotationData Is Nothing Then
            Me.m_AnnotationData = New Dictionary(Of String, SortedSet(Of String))
        End If

        If Not Me.m_AnnotationData.ContainsKey(PrimaryReferenceName) Then
            xrefList = New SortedSet(Of String)
            xrefList.Add(XRefName)
        Else
            xrefList = m_AnnotationData.Item(PrimaryReferenceName.ToString)
            If Not xrefList.Contains(XRefName) Then
                xrefList.Add(XRefName)
                Me.m_AnnotationData.Item(PrimaryReferenceName.ToString) = xrefList
            End If
        End If
    End Sub

    Function GetAllXRefs() As Dictionary(Of String, SortedSet(Of String))
        Return Me.m_AnnotationData
    End Function

    Function GetAllPrimaryReferences() As SortedSet(Of String)
        Dim s As String
        Dim annotationKeys As New SortedSet(Of String)
        For Each s In Me.m_AnnotationData.Keys
            annotationKeys.Add(s)
        Next

        Return annotationKeys
    End Function

    Function GetXRefs(PrimaryReferenceName As String) As SortedSet(Of String)
        Dim xrefList = Me.m_AnnotationData.Item(PrimaryReferenceName)

        If Me.m_Delimiter.Length > 0 Then
            Dim addnXRefs() As String
            Dim primeXRef As String
            Dim XRefCount As Integer
            Dim newXReflist As New SortedSet(Of String)

            For Each primeXRef In xrefList
                addnXRefs = primeXRef.Split(Me.m_Delimiter.ToCharArray)
                For XRefCount = 0 To addnXRefs.Length - 1
                    Dim newItem = addnXRefs(XRefCount).ToString
                    If Not newXReflist.Contains(newItem) Then
                        newXReflist.Add(newItem)
                    End If
                Next
            Next

            xrefList = newXReflist

        End If

        Return xrefList
    End Function
End Class
