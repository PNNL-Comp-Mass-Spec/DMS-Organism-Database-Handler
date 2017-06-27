Friend Interface IGetAnnotationsFromDB

End Interface



Friend Class clsGetAnnotationsFromDB
    Private m_ConnectionString As String
    Private m_DatabaseHelper As TableManipulationBase.IGetSQLData
    Private m_AnnotationCollection As clsAnnotationInfo

    Sub New(PSConnectionString As String)
        Me.m_ConnectionString = PSConnectionString
    End Sub

    Function GetAnnotationDetails(ProteinCollectionID As Integer) As clsAnnotationInfo
        Me.m_DatabaseHelper = New TableManipulationBase.clsDBTask(Me.m_ConnectionString, True)

        Dim CollectionName As String = String.Empty


        Dim SQLStatement As String
        Dim dr As DataRow
        Dim foundrows() As DataRow

        Dim tmpRefID As Integer
        Dim tmpName As String
        Dim tmpDesc As String
        Dim tmpProtID As Integer
        Dim tmpNameAuthID As Integer

        ' FYI: The constructor for clsAnnotationInfo() doesn't use CollectionName or ProteinCollectionID at present
        Dim info As New clsAnnotationInfo()

        Dim tmpNameTable As DataTable
        Dim tmpAuthorityTable As DataTable
        Dim tmpAnnotationGroupTable As DataTable
        Dim tmpAnnotationTable As DataTable

        'Get Protein Collection Name
        SQLStatement = "SELECT TOP 1 Name FROM V_Collection_Picker " &
                            "WHERE ID = " & ProteinCollectionID
        tmpNameTable = Me.m_DatabaseHelper.GetTable(SQLStatement)
        CollectionName = tmpNameTable.Rows(0).Item("Name").ToString

        tmpNameTable.Clear()
        tmpNameTable = Nothing

        'Get Naming Authority Lookup

        SQLStatement = "SELECT Authority_ID, Name FROM T_Naming_Authorities"
        tmpAuthorityTable = Me.m_DatabaseHelper.GetTable(SQLStatement)

        foundrows = tmpAuthorityTable.Select("")
        For Each dr In foundrows
            info.AddAuthorityNameToLookup(DirectCast(dr.Item("Authority_ID"), Int32),
                DirectCast(dr.Item("Name"), String))
        Next

        tmpAuthorityTable.Clear()
        tmpAuthorityTable = Nothing


        'Get Annotation Group Information

        SQLStatement = "SELECT Annotation_Group, Authority_ID " &
            "FROM T_Annotation_Groups " &
            "WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString

        tmpAnnotationGroupTable = Me.m_DatabaseHelper.GetTable(SQLStatement)

        For Each dr In tmpAnnotationGroupTable.Rows
            info.AddAnnotationGroupLookup(
                DirectCast(dr.Item("Annotation_Group"), Int32),
                DirectCast(dr.Item("Authority_ID"), Int32))
        Next



        'Get Collection Member Primary Information

        'Get Reference_ID, Name, Description, Protein_ID, Protein_Collection_ID,
        '   Authority_ID, Annotation_Group_ID
        SQLStatement = "SELECT * FROM V_Protein_Collection_Members " &
            "WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString &
            " AND Annotation_Group_ID = 0"
        tmpAnnotationTable = Me.m_DatabaseHelper.GetTable(SQLStatement)


        foundrows = tmpAnnotationTable.Select("")

        For Each dr In foundrows
            tmpRefID = DirectCast(dr.Item("Reference_ID"), Int32)
            tmpName = DirectCast(dr.Item("Name"), String)
            tmpDesc = DirectCast(dr.Item("Description"), String)
            tmpProtID = DirectCast(dr.Item("Protein_ID"), Int32)
            tmpNameAuthID = DirectCast(dr.Item("Annotation_Type_ID"), Int32)

            info.AddPrimaryAnnotation(
                tmpProtID, tmpName, tmpDesc, tmpRefID, tmpNameAuthID)
        Next

        tmpAnnotationTable.Clear()
        tmpAnnotationTable = Nothing


        'Get Additional Annotations
        SQLStatement = "SELECT "

        Return info

    End Function

End Class
