Public Class clsAddAnnotationType
    Protected m_ConnectionString As String
    Protected m_SPRunner As Protein_Importer.IAddUpdateEntries

    Protected m_TypeName As String
    Protected m_Description As String
    Protected m_Example As String
    Protected m_AuthID As Integer
	Protected m_EntryExists As Boolean = False
    Protected m_AuthAdd As clsAddNamingAuthority
    Protected m_Authorities As DataTable
    Protected m_FormLocation As System.Drawing.Point

    ReadOnly Property TypeName() As String
        Get
            Return Me.m_TypeName
        End Get
    End Property

    ReadOnly Property Description() As String
        Get
            Return Me.m_Description
        End Get
    End Property

    ReadOnly Property AnnotationExample() As String
        Get
            Return Me.m_Example
        End Get
    End Property

    ReadOnly Property AuthorityID() As Integer
        Get
            Return Me.m_AuthID
        End Get
    End Property

    ReadOnly Property DisplayName() As String
        Get
            Return GetDisplayName(Me.m_AuthID, Me.m_TypeName)
        End Get
    End Property

    ReadOnly Property EntryExists() As Boolean
        Get
            Return Me.m_EntryExists
        End Get
    End Property

    WriteOnly Property FormLocation() As System.Drawing.Point
        Set(Value As System.Drawing.Point)
            Me.m_FormLocation = Value
        End Set
    End Property


    Sub New(PSConnectionString As String)
        Me.m_ConnectionString = PSConnectionString

        Me.m_AuthAdd = New clsAddNamingAuthority(Me.m_ConnectionString)
        Me.m_Authorities = Me.m_AuthAdd.AuthoritiesTable

    End Sub

    Private Function GetDisplayName(AuthorityID As Integer, typeName As String) As String
        Dim authName As String
        Dim foundrows As DataRow() = Me.m_Authorities.Select("ID = " & AuthorityID)

        authName = foundrows(0).Item("Display_Name").ToString

        Return authName & " - " & typeName
    End Function

    Function AddAnnotationType() As Integer

        Dim frmAnn As New frmAddAnnotationType
        Dim annTypeID As Integer
        If Me.m_SPRunner Is Nothing Then
            Me.m_SPRunner = New Protein_Importer.clsAddUpdateEntries(Me.m_ConnectionString)
        End If

        If Me.m_AuthAdd Is Nothing Then
            Me.m_AuthAdd = New clsAddNamingAuthority(Me.m_ConnectionString)
        End If

        Dim errorResult As DialogResult

        frmAnn.AuthorityTable = Me.m_Authorities
        frmAnn.ConnectionString = Me.m_ConnectionString
        frmAnn.DesktopLocation = Me.m_FormLocation
        Dim r As DialogResult = frmAnn.ShowDialog
        Dim authNames() As DataRow
        Dim authName As String


        If r = DialogResult.OK Then
            Me.m_TypeName = frmAnn.TypeName
            Me.m_Description = frmAnn.Description
            Me.m_Example = frmAnn.Example
            Me.m_AuthID = frmAnn.AuthorityID

            annTypeID = Me.m_SPRunner.AddAnnotationType(
                 Me.m_TypeName, Me.m_Description, Me.m_Example, Me.m_AuthID)

            If annTypeID < 0 Then
                authNames = Me.m_Authorities.Select("Authority_ID = " & Me.m_AuthID.ToString)
                authname = authNames(0).Item("Name").ToString
                errorResult = System.Windows.Forms.MessageBox.Show(
                    "An entry called '" + Me.m_TypeName + "' for '" & authName & "' already exists in the Annotation Types table",
                    "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                Me.m_EntryExists = True
                annTypeID = -annTypeID
            Else
                Me.m_EntryExists = False
            End If
        Else
            annTypeID = 0
            Me.m_EntryExists = True
        End If

        frmAnn = Nothing
        Me.m_SPRunner = Nothing

        Return annTypeID

    End Function

End Class
