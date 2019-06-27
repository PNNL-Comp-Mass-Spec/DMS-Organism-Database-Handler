Imports System.IO

Friend Class clsExtractFromFlatFile

    Private m_FilePath As String

    ''' <summary>
    ''' Each entry in this list is a dictionary where keys are column name (1-based) and values are the value for that column
    ''' </summary>
    Private m_FileContents As List(Of Dictionary(Of Integer, String))

    ''' <summary>
    ''' Keys are column number (starting at 1)
    ''' Values are column names
    ''' </summary>
    Private m_ColumnNameLookup As Dictionary(Of Integer, String)

    Private ReadOnly m_Authorities As Dictionary(Of String, String)
    Private m_AnnotationStorage As AnnotationStorage
    Private m_firstLine As String
    Private ReadOnly m_PSConnectionString As String
    Private m_Uploader As Protein_Importer.clsAddUpdateEntries
    Private m_ProteinIDLookup As Dictionary(Of String, Integer)

    Private m_MaxProteinNameLength As Integer = 32

    'AuthorityLookupHash key = AuthorityID, value = AuthorityName
    Sub New(AuthorityList As Dictionary(Of String, String), PSConnectionString As String)
        Me.m_Authorities = AuthorityList
        Me.m_PSConnectionString = PSConnectionString
    End Sub

    ReadOnly Property FileContents As List(Of Dictionary(Of Integer, String))
        Get
            Return Me.m_FileContents
        End Get
    End Property

    ReadOnly Property Annotations As AnnotationStorage
        Get
            Return Me.m_AnnotationStorage
        End Get
    End Property

    ''' <summary>
    ''' Keys are column number (starting at 1)
    ''' Values are column names
    ''' </summary>
    ReadOnly Property ColumnNames As Dictionary(Of Integer, String)
        Get
            Return Me.m_ColumnNameLookup
        End Get
    End Property

    Private Sub ExtractGroupsFromLine(
      entryLine As String,
      delimiter As String,
      useContentsAsColumnNames As Boolean)

        Me.m_AnnotationStorage = New AnnotationStorage()

        ' In dictionary valuesByColumnId:
        '  Keys are column number (starting at 1)
        '  Values are column names
        Dim valuesByColumnId = Me.GetLineValuesByColumnId(entryLine, delimiter)

        If Me.m_ColumnNameLookup Is Nothing Then
            Me.m_ColumnNameLookup = New Dictionary(Of Integer, String)(valuesByColumnId.Count)
        Else
            Me.m_ColumnNameLookup.Clear()
        End If

        Me.m_AnnotationStorage.ClearAnnotationGroups()

        For columnNumber = 1 To valuesByColumnId.Count

            Dim columnName As String
            If useContentsAsColumnNames Then
                columnName = valuesByColumnId(columnNumber)
            Else
                columnName = "Column_" & Format(columnNumber, "00")
            End If

            Me.m_ColumnNameLookup.Add(columnNumber, columnName)
            Me.m_AnnotationStorage.AddAnnotationGroup(columnNumber, columnName)
        Next

    End Sub

    Private Function GetLineValuesByColumnId(
        entryLine As String,
        delimiter As String) As Dictionary(Of Integer, String)

        Dim lineEntries = entryLine.Split(delimiter.ToCharArray)
        Dim valuesByColumnId = New Dictionary(Of Integer, String)(lineEntries.Length)

        For columnID = 1 To lineEntries.Length
            Dim lineEntry = lineEntries(columnID - 1)

            If lineEntry.Trim(" "c).Length > 0 Then
                valuesByColumnId.Add(columnID, lineEntry)
            Else
                valuesByColumnId.Add(columnID, "---")
            End If

        Next

        Return valuesByColumnId

    End Function

    Function DataLineToListViewItem(
        dataLine As Dictionary(Of Integer, String),
        lineCount As Integer) As System.Windows.Forms.ListViewItem

        Dim lvItem As System.Windows.Forms.ListViewItem
        Dim columnCount As Integer = dataLine.Count

        Dim maxColumnCount As Integer = Me.ColumnNames.Count

        Dim blankColumnCount As Integer

        lvItem = New System.Windows.Forms.ListViewItem(dataLine.Item(1))
        For columnNumber = 2 To columnCount

            Dim dataValue = dataLine.Item(columnNumber)
            If dataValue.Length > 0 Then
                lvItem.SubItems.Add(dataValue)
            Else
                lvItem.SubItems.Add("---")
            End If
        Next

        blankColumnCount = maxColumnCount - columnCount
        If blankColumnCount > 0 Then
            For columnNumber = 1 To blankColumnCount
                lvItem.SubItems.Add("---")
            Next
        End If

        Return lvItem

    End Function

    Function LoadGroups(
        delimiter As String,
        UseHeaderLineInfo As Boolean) As Integer

        Me.ExtractGroupsFromLine(Me.m_firstLine, delimiter, UseHeaderLineInfo)

    End Function

    'Returns number of lines loaded
    Function LoadFile(
        filePath As String,
        delimiter As String,
        useHeaderLineInfo As Boolean) As Integer

        Dim inputFile As New System.IO.FileInfo(filePath)

        Me.m_FileContents = New List(Of Dictionary(Of Integer, String))

        Using reader = New StreamReader(New FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))

            Dim firstLineStored = False
            While Not reader.EndOfStream
                Dim entryLine = reader.ReadLine()
                If Not firstLineStored Then
                    Me.m_firstLine = entryLine
                    firstLineStored = True
                End If

                Dim valuesByColumnId = Me.GetLineValuesByColumnId(entryLine, delimiter)
                Me.m_FileContents.Add(valuesByColumnId)

            End While
        End Using

        'Get Column names if possible
        Me.ExtractGroupsFromLine(Me.m_firstLine, delimiter, useHeaderLineInfo)

        Return Me.m_FileContents.Count
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="primaryReferenceNameColumnID">The number of the column with the name to use as primary</param>
    ''' <param name="authorityHash">Dictionary with columnID (number), and authority name for that column</param>
    Sub ParseLoadedFile(
        primaryReferenceNameColumnID As Integer,
        authorityHash As Dictionary(Of String, String))

        For Each dataLine In Me.m_FileContents
            Dim primaryRef = dataLine(primaryReferenceNameColumnID)

            For columnNumber = 1 To dataLine.Count
                If Not columnNumber.Equals(primaryReferenceNameColumnID) And
                    Not dataLine.Item(columnNumber).Equals("---") Then
                    Me.m_AnnotationStorage.AddAnnotation(
                        columnNumber, primaryRef,
                        dataLine.Item(columnNumber))
                End If
            Next
        Next
    End Sub

    Function LookupAuthorityName(AuthorityID As Integer) As String
        Return Me.m_Authorities.Item(AuthorityID.ToString())
    End Function

    Function GetListViewItemForGroup(
        GroupID As Integer) As System.Windows.Forms.ListViewItem

        Dim li As New System.Windows.Forms.ListViewItem(GroupID.ToString)
        With li.SubItems
            .Add(Me.m_AnnotationStorage.GroupName(GroupID))
            If Me.m_AnnotationStorage.AnnotationAuthorityID(GroupID) > 0 Then
                .Add(Me.m_Authorities.Item(Me.m_AnnotationStorage.AnnotationAuthorityID(GroupID).ToString).ToString())
            Else
                .Add("-- None Selected --")
            End If
            If Not Me.m_AnnotationStorage.Delimiter(GroupID) Is Nothing Then
                .Add(Me.m_AnnotationStorage.Delimiter(GroupID).ToString())
            Else
                .Add(" ")
            End If
        End With

        Return li
    End Function

    Sub ChangeAuthorityIDforGroup(GroupID As Integer, AuthorityID As Integer)

        Me.m_AnnotationStorage.AnnotationAuthorityID(GroupID) = AuthorityID

    End Sub

    Sub UploadNewNames(PrimaryReferenceNameColumnID As Integer)
        Me.ParseLoadedFile(PrimaryReferenceNameColumnID, Me.m_Authorities)
        If Me.m_Uploader Is Nothing Then
            Me.m_Uploader = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)
        End If

        Dim groupCount As Integer = Me.m_AnnotationStorage.GroupCount

        Me.m_ProteinIDLookup = Me.GetProteinIDsForPrimaryReferences(Me.m_AnnotationStorage.GetAllPrimaryReferences)

        For columnCount = 1 To groupCount
            If Not columnCount.Equals(PrimaryReferenceNameColumnID) Then
                Dim ag = Me.m_AnnotationStorage.GetGroup(columnCount)
                Dim referenceLookup = ag.GetAllXRefs()
                For Each proteinName In referenceLookup.Keys
                    Me.m_Uploader.AddProteinReference(
                       proteinName,
                       String.Empty,
                       0,
                       ag.AnnotationAuthorityID,
                       m_ProteinIDLookup.Item(proteinName),
                       m_MaxProteinNameLength)
                Next

            End If
        Next

    End Sub

    Private Function GetProteinIDsForPrimaryReferences(PrimaryReferences As IReadOnlyCollection(Of String)) As Dictionary(Of String, Integer)
        Dim name As String
        Dim ht As New Dictionary(Of String, Integer)(PrimaryReferences.Count)
        Dim id As Integer

        If Me.m_Uploader Is Nothing Then
            Me.m_Uploader = New Protein_Importer.clsAddUpdateEntries(Me.m_PSConnectionString)
        End If

        For Each name In PrimaryReferences
            If Not ht.ContainsKey(name) Then
                If Me.m_ProteinIDLookup.ContainsKey(name) Then
                    id = m_ProteinIDLookup(name)
                Else
                    id = Me.m_Uploader.GetProteinIDFromName(name)
                End If
                ht.Add(name, id)
            End If
        Next

        Return ht

    End Function

End Class
