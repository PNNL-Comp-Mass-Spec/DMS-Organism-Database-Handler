Friend Interface IGetAnnotationsFromDB

End Interface

<Obsolete("Unused")>
Friend Class clsGetAnnotationsFromDB
    Private m_ConnectionString As String
    Private m_DatabaseHelper As TableManipulationBase.clsDBTask
    Private m_AnnotationCollection As clsAnnotationInfo

    Sub New(PSConnectionString As String)
        Me.m_ConnectionString = PSConnectionString
    End Sub

    Function GetAnnotationDetails(ProteinCollectionID As Integer) As clsAnnotationInfo
        Me.m_DatabaseHelper = New TableManipulationBase.clsDBTask(m_ConnectionString)

        ' FYI: The constructor for clsAnnotationInfo() doesn't use CollectionName or ProteinCollectionID at present
        Dim info As New clsAnnotationInfo()

        'Get Protein Collection Name
        Dim sqlQuery1 = "SELECT TOP 1 Name FROM V_Collection_Picker " &
                       "WHERE ID = " & ProteinCollectionID
        Dim nameLookupTable = Me.m_DatabaseHelper.GetTable(sqlQuery1)

        ' ReSharper disable once UnusedVariable
        Dim collectionName = nameLookupTable.Rows(0).Item("Name").ToString()

        'Get Naming Authority Lookup

        Dim sqlQuery2 = "SELECT Authority_ID, Name FROM T_Naming_Authorities"
        Dim authorityLookupTable = Me.m_DatabaseHelper.GetTable(sqlQuery2)

        Dim authorityLookupRows = authorityLookupTable.Select("")
        For Each dr As DataRow In authorityLookupRows
            info.AddAuthorityNameToLookup(
                DBTools.GetInteger(dr.Item("Authority_ID")),
                DBTools.GetString(dr.Item("Name")))
        Next


        'Get Annotation Group Information

        Dim sqlQuery3 = "SELECT Annotation_Group, Authority_ID " &
                        "FROM T_Annotation_Groups " &
                        "WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString

        Dim annotationGroupLookup = Me.m_DatabaseHelper.GetTable(sqlQuery3)

        For Each dr As DataRow In annotationGroupLookup.Rows
            info.AddAnnotationGroupLookup(
                DBTools.GetInteger(dr.Item("Annotation_Group")),
                DBTools.GetInteger(dr.Item("Authority_ID")))
        Next


        'Get Collection Member Primary Information

        'Get Reference_ID, Name, Description, Protein_ID, Protein_Collection_ID,
        '   Authority_ID, Annotation_Group_ID
        Dim sqlQuery4 = "SELECT * FROM V_Protein_Collection_Members " &
            "WHERE Protein_Collection_ID = " & ProteinCollectionID.ToString &
            " AND Annotation_Group_ID = 0"
        Dim annotationTableLookup = Me.m_DatabaseHelper.GetTable(sqlQuery4)

        Dim annotationTableRows = annotationTableLookup.Select("")

        For Each dr As DataRow In annotationTableRows
            Dim tmpRefID = DBTools.GetInteger(dr.Item("Reference_ID"))
            Dim tmpName = DBTools.GetString(dr.Item("Name"))
            Dim tmpDesc = DBTools.GetString(dr.Item("Description"))
            Dim tmpProtID = DBTools.GetInteger(dr.Item("Protein_ID"))
            Dim tmpNameAuthID = DBTools.GetInteger(dr.Item("Annotation_Type_ID"))

            info.AddPrimaryAnnotation(
                tmpProtID, tmpName, tmpDesc, tmpRefID, tmpNameAuthID)
        Next


        Return info

    End Function

End Class
