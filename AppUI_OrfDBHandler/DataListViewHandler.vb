Public Class DataListViewHandler

    Public Event LoadStart(taskTitle As String)
    Public Event LoadProgress(fractionDone As Double)
    Public Event LoadEnd()
    Public Event NumberLoadedStatus(FilteredCount As Integer, TotalCount As Integer)


    Public Sub New(listViewToFill As ListView)
        m_LVW = listViewToFill
    End Sub

    Private m_LVW As ListView

    Public Sub Load(listTable As DataTable)
        FillListView(m_LVW, listTable)
    End Sub

    Public Sub Load(listTable As DataTable, quickFilterCriteria As String)
        FillFilteredListView(m_LVW, listTable, quickFilterCriteria)
    End Sub

    Private Sub SetupPickerListView(
        lvw As ListView,
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
        listViewToFill As ListView,
        listData As DataTable)

        listViewToFill.Items.Clear()
        SetupPickerListView(listViewToFill, listData)
    End Sub

    Protected Sub FillFilteredListView(
        listViewToFill As ListView,
        listData As DataTable,
        filterString As String)

        listViewToFill.Items.Clear()
        SetupPickerListView(listViewToFill, listData, filterString)
    End Sub

End Class
