Public Class clsTranslateNucleotides

    Protected m_TranslationMatrix As ArrayList
    Protected m_GetSQLData As TableManipulationBase.clsDBTask

    Protected m_TranTableListName As String = "T_DNA_Translation_Tables"
    Protected m_TransTableMembersName As String = "T_DNA_Translation_Table_Members"

    Public Sub New(DMSConnectionString As String)
        Me.m_GetSQLData = New TableManipulationBase.clsDBTask(DMSConnectionString)
    End Sub

    Public Function LoadTransMatrix(TranslationTableID As Integer) As ArrayList

        Dim BaseArray() As Char = "ATGC".ToCharArray
        Dim base_1 As Char
        Dim base_2 As Char
        Dim base_3 As Char

        Dim selectSQL As String =
            "SELECT * FROM " & Me.m_TransTableMembersName &
            " WHERE DNA_Translation_Table_ID = " & TranslationTableID

        Dim members As DataTable = Me.m_GetSQLData.GetTable(selectSQL)

        Dim dr As DataRow
        Dim tertSelect As String
        Dim TertiaryRows() As DataRow

        Dim PrimaryList As New ArrayList
        Dim SecondaryList As New ArrayList
        Dim TertiaryList As New ArrayList

        For Each base_1 In BaseArray
            For Each base_2 In BaseArray

                For Each base_3 In BaseArray
                    tertSelect = "Base_1 = '" & base_1.ToString &
                        "' AND Base_2 = '" & base_2.ToString &
                        "' AND Base_3 = '" & base_3.ToString & "'"
                    TertiaryRows = members.Select(tertSelect)

                    dr = TertiaryRows(0)

                    TertiaryList.Add(New clsTranslationEntry(base_3.ToString, CStr(dr.Item("Coded_AA"))))
                Next
                SecondaryList.Add(New clsTranslationEntry(base_2.ToString, TertiaryList))
                TertiaryList = New ArrayList
            Next

            PrimaryList.Add(New clsTranslationEntry(base_1.ToString, SecondaryList))
            SecondaryList = New ArrayList
        Next

        Return PrimaryList
    End Function

    Protected Function LoadNucPositions(filePath As String) As Integer

    End Function

End Class
