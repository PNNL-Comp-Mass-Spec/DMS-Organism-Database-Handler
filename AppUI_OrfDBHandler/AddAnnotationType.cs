Imports System.Linq
Imports Protein_Importer

Public Class AddAnnotationType
    Protected m_ConnectionString As String
    Protected m_SPRunner As AddUpdateEntries

    Protected m_TypeName As String
    Protected m_Description As String
    Protected m_Example As String
    Protected m_AuthID As Integer
    Protected m_EntryExists As Boolean = False
    Protected m_AuthAdd As AddNamingAuthority
    Protected m_Authorities As DataTable
    Protected m_FormLocation As Point

    ReadOnly Property TypeName As String
        Get
            Return m_TypeName
        End Get
    End Property

    ReadOnly Property Description As String
        Get
            Return m_Description
        End Get
    End Property

    ReadOnly Property AnnotationExample As String
        Get
            Return m_Example
        End Get
    End Property

    ReadOnly Property AuthorityID As Integer
        Get
            Return m_AuthID
        End Get
    End Property

    ReadOnly Property DisplayName As String
        Get
            Return GetDisplayName(m_AuthID, m_TypeName)
        End Get
    End Property

    ReadOnly Property EntryExists As Boolean
        Get
            Return m_EntryExists
        End Get
    End Property

    WriteOnly Property FormLocation As Point
        Set
            m_FormLocation = Value
        End Set
    End Property


    Sub New(psConnectionString As String)
        m_ConnectionString = psConnectionString

        m_AuthAdd = New AddNamingAuthority(m_ConnectionString)
        m_Authorities = m_AuthAdd.AuthoritiesTable

    End Sub

    Private Function GetDisplayName(authID As Integer, authTypeName As String) As String
        Dim authName As String
        Dim foundRows = m_Authorities.Select("ID = " & authID).ToList()

        If (foundRows.Count > 0) Then
            authName = foundRows(0).Item("Display_Name").ToString()
        Else
            authName = "UnknownAuth"
        End If

        Return authName & " - " & authTypeName
    End Function

    Function AddAnnotationType() As Integer

        Dim frmAnn As New frmAddAnnotationType
        Dim annTypeID As Integer
        If m_SPRunner Is Nothing Then
            m_SPRunner = New AddUpdateEntries(m_ConnectionString)
        End If

        If m_AuthAdd Is Nothing Then
            m_AuthAdd = New AddNamingAuthority(m_ConnectionString)
        End If

        frmAnn.AuthorityTable = m_Authorities
        frmAnn.ConnectionString = m_ConnectionString
        frmAnn.DesktopLocation = m_FormLocation
        Dim r As DialogResult = frmAnn.ShowDialog
        Dim authNames() As DataRow
        Dim authName As String


        If r = DialogResult.OK Then
            m_TypeName = frmAnn.TypeName
            m_Description = frmAnn.Description
            m_Example = frmAnn.Example
            m_AuthID = frmAnn.AuthorityID

            annTypeID = m_SPRunner.AddAnnotationType(
                 m_TypeName, m_Description, m_Example, m_AuthID)

            If annTypeID < 0 Then
                authNames = m_Authorities.Select("Authority_ID = " & m_AuthID.ToString)
                authName = authNames(0).Item("Name").ToString
                MessageBox.Show(
                    "An entry called '" + m_TypeName + "' for '" & authName & "' already exists in the Annotation Types table",
                    "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                m_EntryExists = True
                annTypeID = -annTypeID
            Else
                m_EntryExists = False
            End If
        Else
            annTypeID = 0
            m_EntryExists = True
        End If

        m_SPRunner = Nothing

        Return annTypeID

    End Function

End Class
