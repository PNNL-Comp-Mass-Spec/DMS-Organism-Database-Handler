Imports Protein_Importer

Public Class clsAddNamingAuthority

    Protected ReadOnly m_ConnectionString As String
    Protected m_SPRunner As IAddUpdateEntries

    Protected m_shortName As String
    Protected m_fullName As String
    Protected m_webAddress As String
    Protected m_EntryExists As Boolean = False
    Protected m_Importer As IImportProteins
    Protected ReadOnly m_AuthorityTable As DataTable
    Protected m_FormLocation As Point

    ReadOnly Property ShortName() As String
        Get
            Return Me.m_shortName
        End Get
    End Property

    ReadOnly Property FullName() As String
        Get
            Return Me.m_fullName
        End Get
    End Property

    ReadOnly Property WebAddress() As String
        Get
            Return Me.m_webAddress
        End Get
    End Property

    ReadOnly Property EntryExists() As Boolean
        Get
            Return Me.m_EntryExists
        End Get
    End Property

    ReadOnly Property AuthoritiesTable() As DataTable
        Get
            Return Me.m_AuthorityTable
        End Get
    End Property

    WriteOnly Property FormLocation() As Point
        Set(Value As Point)
            Me.m_FormLocation = Value
        End Set
    End Property


    Sub New(PSConnectionString As String)
        Me.m_ConnectionString = PSConnectionString
        Me.m_AuthorityTable = Me.GetAuthoritiesList()
    End Sub


    Function AddNamingAuthority() As Integer

        Dim frmAuth As New frmAddNamingAuthority
        frmAuth.DesktopLocation = Me.m_FormLocation
        Dim authID As Integer
        If Me.m_SPRunner Is Nothing Then
            Me.m_SPRunner = New clsAddUpdateEntries(Me.m_ConnectionString)
        End If

        Dim r As DialogResult = frmAuth.ShowDialog


        If r = DialogResult.OK Then
            Me.m_shortName = frmAuth.ShortName
            Me.m_fullName = frmAuth.FullName
            Me.m_webAddress = frmAuth.WebAddress

            authID = Me.m_SPRunner.AddNamingAuthority(Me.m_shortName, Me.m_fullName, Me.m_webAddress)
            If authID < 0 Then

                MessageBox.Show(
                    "An entry for '" + Me.m_shortName + "' already exists in the Authorities table",
                    "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                Me.m_EntryExists = True
                authID = -authID
            Else
                Me.m_EntryExists = False
            End If
        Else
            authID = -1
        End If

        Me.m_SPRunner = Nothing

        Return authID

    End Function

    Private Function GetAuthoritiesList() As DataTable
        If Me.m_Importer Is Nothing Then
            Me.m_Importer = New clsImportHandler(Me.m_ConnectionString)
        End If

        Dim tmpAuthTable As DataTable
        tmpAuthTable = Me.m_Importer.LoadAuthorities

        Return tmpAuthTable

    End Function

End Class
