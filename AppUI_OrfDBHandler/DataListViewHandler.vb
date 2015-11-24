Public Class DataListViewHandler

    Public Event LoadStart(taskTitle As String)
    Public Event LoadProgress(fractionDone As Double)
    Public Event LoadEnd()
    Public Event NumberLoadedStatus(FilteredCount As Integer, TotalCount As Integer)


    Public Sub New(ByRef ListViewToFill As ListView)
        Me.m_LVW = ListViewToFill
    End Sub

    Private m_LVW As ListView

    Public Sub Load(ListTable As DataTable)
        Me.FillListView(Me.m_LVW, ListTable)
    End Sub

    Public Sub Load(ListTable As DataTable, QuickFilterCriteria As String)
        Me.FillFilteredListView(Me.m_LVW, ListTable, QuickFilterCriteria)
    End Sub

    Private Sub SetupPickerListView(
        ByRef lvw As ListView,
        dt As DataTable,
        Optional filterCriteria As String = "")

        Dim itemRow As DataRow
        Dim itemRows() As DataRow
        Dim filterString As String = String.Empty
        Dim proteinCount As Integer
        Dim triggerCount As Integer
        Dim counter As Integer

        If Len(filterCriteria) <> 0 Then
            filterString = "[Name] LIKE '%" & filterCriteria & "%' " &
                "OR [Description] LIKE '%" & filterCriteria & "%'"
        End If

        lvw.BeginUpdate()

        itemRows = dt.Select(filterString, "Name")
        triggerCount = itemRows.Length

        'RaiseEvent NumberLoadedStatus(itemRows.Length, dt.Rows.Count)

        proteinCount = CInt(itemRows.Length)
        If proteinCount > 20 Then
            triggerCount = CInt(proteinCount / 20)
        Else
            triggerCount = 1
        End If

        'RaiseEvent LoadStart("Filling List...")

        For Each itemRow In itemRows
            counter += 1
            Dim item As New ListViewItem
            item.Text = CStr(itemRow.Item(0))
            item.SubItems.Add(CStr(itemRow.Item(1)))
            lvw.Items.Add(item)
            'If counter Mod triggerCount > 0 Then
            '    RaiseEvent LoadProgress(CSng(counter / proteinCount))
            'End If
        Next

        lvw.EndUpdate()
    End Sub

    Protected Sub FillListView(
        ByRef ListViewToFill As ListView,
        ListData As DataTable)

        ListViewToFill.Items.Clear()
        SetupPickerListView(ListViewToFill, ListData)
    End Sub

    Protected Sub FillFilteredListView(
        ByRef ListViewToFill As ListView,
        ListData As DataTable,
        FilterString As String)

        ListViewToFill.Items.Clear()
        SetupPickerListView(ListViewToFill, ListData, FilterString)
    End Sub

End Class
