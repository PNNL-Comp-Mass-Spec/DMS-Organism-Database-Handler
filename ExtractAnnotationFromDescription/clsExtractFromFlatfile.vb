Imports System.IO

Friend Class clsExtractFromFlatfile

    Private m_FilePath As String
    Private m_FileContents As ArrayList
    Private m_ColumnNameLookup As Hashtable
    Private m_Authorities As Hashtable
    Private m_AnnotationStorage As AnnotationStorage
    Private m_firstLine As String
    Private m_PSConnectionString As String
    Private m_Uploader As Protein_Importer.IAddUpdateEntries
    Private m_ProteinIDLookup As Hashtable
    Private m_ProteinIDGlobalLookup As Hashtable

	Private m_MaxProteinNameLength As Integer

    'AuthorityLookupHash key = AuthorityID, value = AuthorityName
    Sub New(ByVal AuthorityList As Hashtable, ByVal PSConnectionString As String)
        Me.m_Authorities = AuthorityList
        Me.m_PSConnectionString = PSConnectionString
    End Sub


    ReadOnly Property FileContents() As ArrayList
        Get
            Return Me.m_FileContents
        End Get
    End Property

    ReadOnly Property Annotations() As AnnotationStorage
        Get
            Return Me.m_AnnotationStorage
        End Get
    End Property

    ReadOnly Property ColumnNames() As Hashtable
        Get
            Return Me.m_ColumnNameLookup
        End Get
    End Property

    Private Sub ExtractGroupsFromLine( _
         ByVal entryLine As String, _
        ByVal delimiter As String, _
        ByVal useContentsAsColumnNames As Boolean)

        Dim lineHash As Hashtable
        Dim columnNumber As Integer
        Dim columnName As String
        Me.m_AnnotationStorage = New AnnotationStorage

        lineHash = Me.StringToHash(entryLine, delimiter)

        If Me.m_ColumnNameLookup Is Nothing Then
            Me.m_ColumnNameLookup = New Hashtable(lineHash.Count)
        End If

        Me.m_ColumnNameLookup.Clear()
        Me.m_AnnotationStorage.ClearAnnotationGroups()

        For columnNumber = 1 To lineHash.Count

            If useContentsAsColumnNames Then
                columnName = lineHash(columnNumber).ToString
                'Me.m_ColumnNameLookup.Add(columnNumber, )
            Else
                columnName = "Column_" & Format(columnNumber, "00")
                'Me.m_ColumnNameLookup.Add(columnNumber, )
            End If
            Me.m_ColumnNameLookup.Add(columnNumber, columnName)
            Me.m_AnnotationStorage.AddAnnotationGroup(columnNumber, columnName)
        Next



    End Sub

    Private Function StringToHash( _
        ByVal entryLine As String, _
        ByVal delimiter As String) As Hashtable

        Dim lineEntries() As String
        Dim lineEntry As String
        Dim lineHash As Hashtable
        Dim columnID As Integer

        lineEntries = entryLine.Split(delimiter.ToCharArray)
        lineHash = New Hashtable(lineEntries.Length)

        For Each lineEntry In lineEntries
            columnID += 1
            If lineEntry.Trim(" "c).Length > 0 Then
                lineHash.Add(columnID, lineEntry)
            Else
                lineHash.Add(columnID, "---")
            End If
        Next

        Return lineHash

    End Function

    Function HashToListViewItem( _
        ByVal lineHash As Hashtable, _
        ByVal lineCount As Integer) As System.Windows.Forms.ListViewItem

        Dim li As System.Windows.Forms.ListViewItem
        Dim columnCount As Integer = lineHash.Count
        Dim columnNumber As Integer
        Dim item As String
        Dim maxColumnCount As Integer = Me.ColumnNames.Count

        Dim blankColumnCount As Integer


        li = New System.Windows.Forms.ListViewItem(lineHash.Item(1).ToString)
        For columnNumber = 2 To columnCount

            item = lineHash.Item(columnNumber).ToString
            If item.Length > 0 Then
                li.SubItems.Add(lineHash.Item(columnNumber).ToString)
            Else
                li.SubItems.Add("---")
            End If
        Next
        blankColumnCount = maxColumnCount - columnCount
        If blankColumnCount > 0 Then
            For columnNumber = 1 To blankColumnCount
                li.SubItems.Add("---")
            Next
        End If

        Return li

    End Function

    Function LoadGroups( _
        ByVal delimiter As String, _
        ByVal UseHeaderLineInfo As Boolean) As Integer

        Me.ExtractGroupsFromLine(Me.m_firstLine, delimiter, UseHeaderLineInfo)

    End Function

    'Returns number of lines loaded
    Function LoadFile( _
        ByVal filePath As String, _
        ByVal delimiter As String, _
        ByVal UseHeaderLineInfo As Boolean) As Integer

        Dim fi As New System.IO.FileInfo(filePath)
        Dim tr As TextReader = fi.OpenText
        Dim entryLine As String
        Dim lineHash As Hashtable

        Me.m_FileContents = New ArrayList

        entryLine = tr.ReadLine
        Me.m_firstLine = entryLine

        While Not entryLine Is Nothing
            lineHash = Me.StringToHash(entryLine, delimiter)
            Me.m_FileContents.Add(lineHash)
            entryLine = tr.ReadLine
        End While

        'Get Column names if possible
        Me.ExtractGroupsFromLine(Me.m_firstLine, delimiter, UseHeaderLineInfo)

        tr.Close()
        tr = Nothing
        fi = Nothing

        Return Me.m_FileContents.Count
    End Function

    'PrimaryReferenceNameColumnID is the number of the column with the name to use as primary
    'AuthorityHash is a hashtable with columnID (number), and authorityID for that column
    Sub ParseLoadedFile( _
        ByVal PrimaryReferenceNameColumnID As Integer, _
        ByVal AuthorityHash As Hashtable)


        'Me.m_AnnotationStorage = New AnnotationStorage

        Dim columnNumber As Integer

        Dim primaryRef As String

        'For columnNumber = 1 To Me.m_ColumnNameLookup.Count
        '    Me.m_AnnotationStorage.AddAnnotationGroup( _
        '        columnNumber, _
        '        Me.m_ColumnNameLookup(columnNumber.ToString).ToString)
        'Next

        Dim lineHash As Hashtable

        For Each lineHash In Me.m_FileContents
            primaryRef = lineHash(PrimaryReferenceNameColumnID).ToString

            For columnNumber = 1 To lineHash.Count
                If Not columnNumber.Equals(PrimaryReferenceNameColumnID) And _
                    Not lineHash.Item(columnNumber).Equals("---") Then
                    Me.m_AnnotationStorage.AddAnnotation( _
                        columnNumber, primaryRef, _
                        lineHash.Item(columnNumber).ToString)
                End If
            Next
        Next
    End Sub

    Function LookupAuthorityName(ByVal AuthorityID As Integer) As String
        Return Me.m_Authorities.Item(AuthorityID.ToString).ToString
    End Function

    Function GetListViewItemForGroup( _
        ByVal GroupID As Integer) As System.Windows.Forms.ListViewItem

        Dim li As New System.Windows.Forms.ListViewItem(GroupID.ToString)
        With li.SubItems
            .Add(Me.m_AnnotationStorage.GroupName(GroupID))
            If Me.m_AnnotationStorage.AnnotationAuthorityID(GroupID) > 0 Then
                .Add(Me.m_Authorities.Item(Me.m_AnnotationStorage.AnnotationAuthorityID(GroupID).ToString).ToString)
            Else
                .Add("-- None Selected --")
            End If
            If Not Me.m_AnnotationStorage.Delimiter(GroupID) Is Nothing Then
                .Add(Me.m_AnnotationStorage.Delimiter(GroupID).ToString)
            Else
                .Add(" ")
            End If
        End With

        Return li
    End Function

    Sub ChangeAuthorityIDforGroup(ByVal GroupID As Integer, ByVal AuthorityID As Integer)

        Me.m_AnnotationStorage.AnnotationAuthorityID(GroupID) = AuthorityID

    End Sub

    Sub UploadNewNames(ByVal PrimaryReferenceNameColumnID As Integer)
        Me.ParseLoadedFile(PrimaryReferenceNameColumnID, Me.m_Authorities)
        If Me.m_Uploader Is Nothing Then
            Me.m_Uploader = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)
        End If

        Dim ag As AnnotationGroup
        Dim groupCount As Integer = Me.m_AnnotationStorage.GroupCount
        Dim columnCount As Integer
        Dim referenceLookup As Hashtable
        Dim proteinName As String

        'Dim al As ArrayList = Me.m_AnnotationStorage.GetAllPrimaryReferences
        Me.m_ProteinIDLookup = Me.GetProteinIDsForPrimaryReferences(Me.m_AnnotationStorage.GetAllPrimaryReferences)

        For columnCount = 1 To groupCount
            If Not columnCount.Equals(PrimaryReferenceNameColumnID) Then
                ag = Me.m_AnnotationStorage.GetGroup(columnCount)
                referenceLookup = ag.GetAllXRefs()
                For Each proteinName In referenceLookup.Keys
					Me.m_Uploader.AddProteinReference(proteinName, _
					 String.Empty, 0, ag.AnnotationAuthorityID, CInt(Me.m_ProteinIDLookup.Item(proteinName)), m_MaxProteinNameLength)
                Next

            End If
        Next


    End Sub

    Private Function GetProteinIDsForPrimaryReferences(ByVal PrimaryReferences As ArrayList) As Hashtable
        Dim name As String
        Dim ht As New Hashtable(PrimaryReferences.Count)
        Dim id As Integer

        If Me.m_Uploader Is Nothing Then
            Me.m_Uploader = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)
        End If


        For Each name In PrimaryReferences
            If Not ht.Contains(name) Then
                If Me.m_ProteinIDLookup.Contains(name) Then
                    id = DirectCast(Me.m_ProteinIDLookup(name), Int32)
                Else
                    id = Me.m_Uploader.GetProteinIDFromName(name)
                End If
                ht.Add(name, id)
            End If
        Next

        Return ht

    End Function

End Class
