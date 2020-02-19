Imports Protein_Importer

Public Class clsAddNamingAuthority

    Protected ReadOnly m_ConnectionString As String
    Protected m_SPRunner As clsAddUpdateEntries

    Protected m_shortName As String
    Protected m_fullName As String
    Protected m_webAddress As String
    Protected m_EntryExists As Boolean = False
    Protected m_Importer As clsImportHandler
    Protected ReadOnly m_AuthorityTable As DataTable
    Protected m_FormLocation As Point

    ReadOnly Property ShortName As String
        Get
            Return m_shortName
        End Get
    End Property

    ReadOnly Property FullName As String
        Get
            Return m_fullName
        End Get
    End Property

    ReadOnly Property WebAddress As String
        Get
            Return m_webAddress
        End Get
    End Property

    ReadOnly Property EntryExists As Boolean
        Get
            Return m_EntryExists
        End Get
    End Property

    ReadOnly Property AuthoritiesTable As DataTable
        Get
            Return m_AuthorityTable
        End Get
    End Property

    WriteOnly Property FormLocation As Point
        Set
            m_FormLocation = Value
        End Set
    End Property


    Sub New(psConnectionString As String)
        m_ConnectionString = psConnectionString
        m_AuthorityTable = GetAuthoritiesList()
    End Sub


    Function AddNamingAuthority() As Integer

        Dim frmAuth As New frmAddNamingAuthority
        frmAuth.DesktopLocation = m_FormLocation
        Dim authID As Integer
        If m_SPRunner Is Nothing Then
            m_SPRunner = New clsAddUpdateEntries(m_ConnectionString)
        End If

        Dim r As DialogResult = frmAuth.ShowDialog


        If r = DialogResult.OK Then
            m_shortName = frmAuth.ShortName
            m_fullName = frmAuth.FullName
            m_webAddress = frmAuth.WebAddress

            authID = m_SPRunner.AddNamingAuthority(m_shortName, m_fullName, m_webAddress)
            If authID < 0 Then

                MessageBox.Show(
                    "An entry for '" + m_shortName + "' already exists in the Authorities table",
                    "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                m_EntryExists = True
                authID = -authID
            Else
                m_EntryExists = False
            End If
        Else
            authID = -1
        End If

        m_SPRunner = Nothing

        Return authID

    End Function

    Private Function GetAuthoritiesList() As DataTable
        If m_Importer Is Nothing Then
            m_Importer = New clsImportHandler(m_ConnectionString)
        End If

        Dim tmpAuthTable As DataTable
        tmpAuthTable = m_Importer.LoadAuthorities()

        Return tmpAuthTable

    End Function

End Class
