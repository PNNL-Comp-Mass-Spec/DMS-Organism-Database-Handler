Public Class frmNewCollectionMetadataEditor

    Public Property Description As String
        Get
            Return txtDescription.Text
        End Get
        Set
            txtDescription.Text = Value
        End Set
    End Property

    Public Property Source As String
        Get
            Return txtSource.Text
        End Get
        Set
            txtSource.Text = Value
        End Set
    End Property

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
    
    Private Sub cmdOk_Click(sender As Object, e As EventArgs) Handles cmdOk.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub txtDescription_KeyDown(sender As Object, e As KeyEventArgs) Handles txtDescription.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.A Then
            txtDescription.SelectAll()
        End If
    End Sub

    Private Sub txtSource_KeyDown(sender As Object, e As KeyEventArgs) Handles txtSource.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.A Then
            txtSource.SelectAll()
        End If
    End Sub

End Class