Imports System.IO
Imports Protein_Importer
Imports Protein_Storage

Public Class clsFilePreviewHandler

    Private m_Proteins As IProteinStorage
    Private m_Loader As IReadProteinImportFile
    Private m_currentFilePath As String
    Private WithEvents m_frmPreview As frmFilePreview

    Event FormStatus(visible As Boolean)

    Sub New()

        Me.m_frmPreview = New frmFilePreview

    End Sub

    Private Sub GetProteins(
        filePath As String,
        lineCount As Integer)

        If Me.m_Loader Is Nothing Then
            Me.m_Loader = New FASTAReader
        End If

        Me.m_Proteins = Me.m_Loader.GetProteinEntries(filePath, lineCount)

        Dim li As ListViewItem

        Dim enumProteins = m_Proteins.GetEnumerator()
        Me.m_frmPreview.lvwPreview.BeginUpdate()
        Me.m_frmPreview.lvwPreview.Items.Clear()

        While enumProteins.MoveNext()
            Dim protein = enumProteins.Current.Value
            li = New ListViewItem(protein.Reference)
            li.SubItems.Add(protein.Description)
            Me.m_frmPreview.lvwPreview.Items.Add(li)
        End While

        Me.m_frmPreview.lvwPreview.EndUpdate()
    End Sub

    Private Sub FillPreview(lineCount As Integer) Handles m_frmPreview.RefreshRequest
        Me.GetProteins(Me.m_currentFilePath, lineCount)
    End Sub

    Sub ShowPreview(filePath As String, horizPos As Integer, vertPos As Integer, height As Integer)
        Me.m_currentFilePath = filePath
        If Me.m_frmPreview Is Nothing Then
            Me.m_frmPreview = New frmFilePreview
        End If
        With Me.m_frmPreview
            .DesktopLocation = New Point(horizPos, vertPos)
            .Height = height
            .WindowName = "Preview of: " & Path.GetFileName(filePath)
            If Me.m_frmPreview.Visible = False Then
                .Show()
            Else
                Me.FillPreview(CInt(Me.m_frmPreview.txtLineCount.Text))
            End If
        End With
        RaiseEvent FormStatus(True)
    End Sub

    Sub CloseForm()
        Me.m_frmPreview.Close()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        Me.m_Proteins = Nothing
        Me.m_Loader = Nothing
        Me.m_frmPreview = Nothing
    End Sub

    Sub OnFormClose() Handles m_frmPreview.FormClosing
        RaiseEvent FormStatus(False)
        Me.m_frmPreview = Nothing
    End Sub

End Class
