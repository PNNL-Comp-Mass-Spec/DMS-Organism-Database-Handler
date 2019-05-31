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

        m_frmPreview = New frmFilePreview

    End Sub

    Private Sub GetProteins(
        filePath As String,
        lineCount As Integer)

        If m_Loader Is Nothing Then
            m_Loader = New FASTAReader
        End If

        m_Proteins = m_Loader.GetProteinEntries(filePath, lineCount)

        Dim li As ListViewItem

        Dim enumProteins = m_Proteins.GetEnumerator()
        m_frmPreview.lvwPreview.BeginUpdate()
        m_frmPreview.lvwPreview.Items.Clear()

        While enumProteins.MoveNext()
            Dim protein = enumProteins.Current.Value
            li = New ListViewItem(protein.Reference)
            li.SubItems.Add(protein.Description)
            m_frmPreview.lvwPreview.Items.Add(li)
        End While

        m_frmPreview.lvwPreview.EndUpdate()
    End Sub

    Private Sub FillPreview(lineCount As Integer) Handles m_frmPreview.RefreshRequest
        GetProteins(m_currentFilePath, lineCount)
    End Sub

    Sub ShowPreview(filePath As String, horizontalPos As Integer, verticalPos As Integer, height As Integer)
        m_currentFilePath = filePath
        If m_frmPreview Is Nothing Then
            m_frmPreview = New frmFilePreview
        End If
        With m_frmPreview
            .DesktopLocation = New Point(horizontalPos, verticalPos)
            .Height = height
            .WindowName = "Preview of: " & Path.GetFileName(filePath)
            If m_frmPreview.Visible = False Then
                .Show()
            Else
                FillPreview(CInt(m_frmPreview.txtLineCount.Text))
            End If
        End With
        RaiseEvent FormStatus(True)
    End Sub

    Sub CloseForm()
        m_frmPreview.Close()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        m_Proteins = Nothing
        m_Loader = Nothing
        m_frmPreview = Nothing
    End Sub

    Sub OnFormClose() Handles m_frmPreview.FormClosing
        RaiseEvent FormStatus(False)
        m_frmPreview = Nothing
    End Sub

End Class
