Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()>
Partial Class frmNewCollectionMetadataEditor
    Inherits Form

    'Form overrides dispose to clean up the component list.
    <DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.txtDescription = New TextBox()
        Me.lblDescription = New Label()
        Me.lblSource = New Label()
        Me.txtSource = New TextBox()
        Me.cmdCancel = New Button()
        Me.cmdOk = New Button()
        Me.SuspendLayout()
        '
        'txtDescription
        '
        Me.txtDescription.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.txtDescription.Location = New Point(15, 34)
        Me.txtDescription.Multiline = True
        Me.txtDescription.Name = "txtDescription"
        Me.txtDescription.Size = New Size(484, 68)
        Me.txtDescription.TabIndex = 1
        '
        'lblDescription
        '
        Me.lblDescription.Location = New Point(12, 9)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New Size(208, 22)
        Me.lblDescription.TabIndex = 0
        Me.lblDescription.Text = "Description"
        '
        'lblSource
        '
        Me.lblSource.Location = New Point(12, 115)
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Size = New Size(208, 22)
        Me.lblSource.TabIndex = 2
        Me.lblSource.Text = "Source (Person, URL, FTP site)"
        '
        'txtSource
        '
        Me.txtSource.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
            Or AnchorStyles.Right), AnchorStyles)
        Me.txtSource.Location = New Point(15, 140)
        Me.txtSource.Multiline = True
        Me.txtSource.Name = "txtSource"
        Me.txtSource.Size = New Size(484, 68)
        Me.txtSource.TabIndex = 3
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.cmdCancel.DialogResult = DialogResult.Cancel
        Me.cmdCancel.Location = New Point(415, 220)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New Size(84, 27)
        Me.cmdCancel.TabIndex = 5
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdOk
        '
        Me.cmdOk.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
        Me.cmdOk.DialogResult = DialogResult.OK
        Me.cmdOk.Location = New Point(310, 220)
        Me.cmdOk.Name = "cmdOk"
        Me.cmdOk.Size = New Size(84, 27)
        Me.cmdOk.TabIndex = 4
        Me.cmdOk.Text = "&Ok"
        '
        'frmNewCollectionMetadataEditor
        '
        Me.AutoScaleDimensions = New SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New Size(517, 259)
        Me.ControlBox = False
        Me.Controls.Add(Me.cmdOk)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.txtSource)
        Me.Controls.Add(Me.lblSource)
        Me.Controls.Add(Me.txtDescription)
        Me.Controls.Add(Me.lblDescription)
        Me.Name = "frmNewCollectionMetadataEditor"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.Text = "Protein Collection Metadata"
        Me.TopMost = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtDescription As TextBox
    Friend WithEvents lblDescription As Label
    Friend WithEvents lblSource As Label
    Friend WithEvents txtSource As TextBox
    Friend WithEvents cmdCancel As Button
    Friend WithEvents cmdOk As Button
End Class
