Imports System.ComponentModel
Imports System.Text
Imports System.Text.RegularExpressions

Public Class frmAddNamingAuthority
    Inherits Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    Friend WithEvents lblAuthShortName As Label
    Friend WithEvents txtAuthName As TextBox
    Friend WithEvents txtAuthWeb As TextBox
    Friend WithEvents lblAuthWeb As Label
    Friend WithEvents cmdCancel As Button
    Friend WithEvents cmdOK As Button
    Friend WithEvents txtAuthFullName As TextBox
    Friend WithEvents lblAuthFullName As Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.lblAuthShortName = New Label
        Me.txtAuthName = New TextBox
        Me.txtAuthFullName = New TextBox
        Me.lblAuthFullName = New Label
        Me.txtAuthWeb = New TextBox
        Me.lblAuthWeb = New Label
        Me.cmdCancel = New Button
        Me.cmdOK = New Button
        Me.SuspendLayout()
        '
        'lblAuthShortName
        '
        Me.lblAuthShortName.Location = New System.Drawing.Point(6, 8)
        Me.lblAuthShortName.Name = "lblAuthShortName"
        Me.lblAuthShortName.Size = New System.Drawing.Size(266, 16)
        Me.lblAuthShortName.TabIndex = 0
        Me.lblAuthShortName.Text = "Authority Short Name (64 char max)"
        '
        'txtAuthName
        '
        Me.txtAuthName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAuthName.Location = New System.Drawing.Point(6, 24)
        Me.txtAuthName.MaxLength = 64
        Me.txtAuthName.Name = "txtAuthName"
        Me.txtAuthName.Size = New System.Drawing.Size(278, 21)
        Me.txtAuthName.TabIndex = 1
        Me.txtAuthName.Text = ""
        '
        'txtAuthFullName
        '
        Me.txtAuthFullName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAuthFullName.Location = New System.Drawing.Point(7, 66)
        Me.txtAuthFullName.MaxLength = 128
        Me.txtAuthFullName.Name = "txtAuthFullName"
        Me.txtAuthFullName.Size = New System.Drawing.Size(278, 21)
        Me.txtAuthFullName.TabIndex = 3
        Me.txtAuthFullName.Text = ""
        '
        'lblAuthFullName
        '
        Me.lblAuthFullName.Location = New System.Drawing.Point(7, 50)
        Me.lblAuthFullName.Name = "lblAuthFullName"
        Me.lblAuthFullName.Size = New System.Drawing.Size(265, 16)
        Me.lblAuthFullName.TabIndex = 2
        Me.lblAuthFullName.Text = "Authority Full Name (128 char max)"
        '
        'txtAuthWeb
        '
        Me.txtAuthWeb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtAuthWeb.Location = New System.Drawing.Point(7, 108)
        Me.txtAuthWeb.MaxLength = 128
        Me.txtAuthWeb.Name = "txtAuthWeb"
        Me.txtAuthWeb.Size = New System.Drawing.Size(278, 21)
        Me.txtAuthWeb.TabIndex = 5
        Me.txtAuthWeb.Text = ""
        '
        'lblAuthWeb
        '
        Me.lblAuthWeb.Location = New System.Drawing.Point(7, 92)
        Me.lblAuthWeb.Name = "lblAuthWeb"
        Me.lblAuthWeb.Size = New System.Drawing.Size(265, 16)
        Me.lblAuthWeb.TabIndex = 4
        Me.lblAuthWeb.Text = "Web Address (optional, 128 char max)"
        '
        'cmdCancel
        '
        Me.cmdCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.cmdCancel.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel.Location = New System.Drawing.Point(210, 140)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.TabIndex = 6
        Me.cmdCancel.Text = "Cancel"
        '
        'cmdOK
        '
        Me.cmdOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.cmdOK.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdOK.Location = New System.Drawing.Point(126, 140)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.TabIndex = 7
        Me.cmdOK.Text = "OK"
        '
        'frmAddNamingAuthority
        '
        Me.AcceptButton = Me.cmdOK
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.CancelButton = Me.cmdCancel
        Me.ClientSize = New System.Drawing.Size(292, 172)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.txtAuthWeb)
        Me.Controls.Add(Me.txtAuthFullName)
        Me.Controls.Add(Me.txtAuthName)
        Me.Controls.Add(Me.lblAuthWeb)
        Me.Controls.Add(Me.lblAuthFullName)
        Me.Controls.Add(Me.lblAuthShortName)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.MaximumSize = New System.Drawing.Size(298, 196)
        Me.MinimumSize = New System.Drawing.Size(298, 196)
        Me.Name = "frmAddNamingAuthority"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "Add Naming Authority"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private m_ShortName As String
    Private m_FullName As String
    Private m_WebAddress As String

#Region " Return Properties "

    Property ShortName As String
        Get
            Return m_ShortName
        End Get
        Set
            m_ShortName = Value
        End Set
    End Property

    Property FullName As String
        Get
            Return m_FullName
        End Get
        Set
            m_FullName = Value
        End Set
    End Property

    Property WebAddress As String
        Get
            Return m_WebAddress
        End Get
        Set
            m_WebAddress = Value
        End Set
    End Property

#End Region

    Private Sub frmAddNamingAuthority_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not m_ShortName Is Nothing Then
            txtAuthName.Text = m_ShortName
        End If

        If Not m_FullName Is Nothing Then
            txtAuthFullName.Text = m_FullName
        End If

        If Not m_WebAddress Is Nothing Then
            txtAuthWeb.Text = m_WebAddress
        End If

    End Sub

    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        m_ShortName = txtAuthName.Text
        m_FullName = txtAuthFullName.Text
        m_WebAddress = txtAuthWeb.Text

        DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub cmdCancel_Click(sender As Object, e As EventArgs) Handles cmdCancel.Click
        DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub



    Function ValidateWebAddressFormat(rawAddress As String) As String
        Dim m As Match
        Dim r1 As New Regex("(?:([^:/?#]+):)?(?://([^/?#]*))?([^?#]*)(?:\?([^#]*))?")  'Match with specific page noted
        Dim newaddress As String
        Dim newAddressSB As New StringBuilder

        If r1.IsMatch(rawAddress) Then
            m = r1.Match(rawAddress)
            If m.Groups(1).Value.Length = 0 Then
                newAddressSB.Append("http://")
                newAddressSB.Append(m.Groups(3).Value)
            Else
                newAddressSB.Append(m.Groups(1).Value)
                newAddressSB.Append("://")
                newAddressSB.Append(m.Groups(2).Value)
                newAddressSB.Append(m.Groups(3).Value)
            End If
            newAddressSB.Append(m.Groups(4).Value)

            newaddress = newAddressSB.ToString

            Return newaddress


        End If

        Return rawAddress
    End Function

    Private Sub txtAuthWeb_Validating(sender As Object, e As CancelEventArgs) Handles txtAuthWeb.Validating
        Dim txt = DirectCast(sender, TextBox)
        Dim tmpAddress As String = ValidateWebAddressFormat(txt.Text)
        txt.Text = tmpAddress
    End Sub
End Class
