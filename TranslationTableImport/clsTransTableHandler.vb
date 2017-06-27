Public Interface ITransTableImport
    Function GetAllTranslationTableEntries(ASN1_FilePath As String) As DataTable

End Interface

'* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
'
'  Handles importation/searching/etc. of ASN.1 Formatted Genetic Code Tables from NCBI
'
'   Example entry from ASN.1 file (between the - - - )
'
'- - - - - - - -
'
'       Genetic-code-table ::= {
'        {
'        name "Standard" ,
'        name "SGC0" ,
'        id 1 ,
'        ncbieaa  "FFLLSSSSYY**CC*WLLLLPPPPHHQQRRRRIIIMTTTTNNKKSSRRVVVVAAAADDEEGGGG",
'        sncbieaa "---M---------------M---------------M----------------------------"
'        -- Base1  TTTTTTTTTTTTTTTTCCCCCCCCCCCCCCCCAAAAAAAAAAAAAAAAGGGGGGGGGGGGGGGG
'        -- Base2  TTTTCCCCAAAAGGGGTTTTCCCCAAAAGGGGTTTTCCCCAAAAGGGGTTTTCCCCAAAAGGGG
'        -- Base3  TCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAG
'        }
'       }
'
'- - - - - - - --


Public Class clsTransTableHandler
    Implements ITransTableImport

    Private m_Translation_Entries As DataTable
    Private m_Translation_Tables As DataTable
    Private m_ConnectionString As String
    Const EntriesTableName As String = "T_DNA_Translation_Table_Members"
    Const IDTableName As String = "T_DNA_Translation_Tables"

    Public Sub New(PIS_ConnectionString As String)
        m_ConnectionString = PIS_ConnectionString
    End Sub

    Friend Function GetAllTranslationTableEntries(FilePath As String) As DataTable Implements ITransTableImport.GetAllTranslationTableEntries
        ScanFileForEntries(FilePath)

        ' Not implemented
        Return New DataTable()
    End Function

    Private Sub ScanFileForEntries(filePath As String)
        'Look through a given ASN.1 file and scan for translation table entries
        Dim fi As System.IO.FileInfo
        Dim tr As System.IO.TextReader
        Dim rawEntry As System.Collections.Specialized.StringCollection
        Dim entryLine As String
        Dim tmpLineCache As String
        Dim checkString As String



        Dim dba As TableManipulationBase.IGetSQLData

        dba = New TableManipulationBase.clsDBTask(m_ConnectionString, True)

        Dim EntrySQL As String = "SELECT * FROM " & clsTransTableHandler.EntriesTableName
        Dim entryDA = New SqlClient.SqlDataAdapter(EntrySQL, dba.Connection)

        m_Translation_Entries = dba.GetTable(EntrySQL, entryDA)



        Dim idSQL As String = "SELECT * FROM " & clsTransTableHandler.IDTableName
        Dim idDA = New SqlClient.SqlDataAdapter(idSQL, dba.Connection)

        m_Translation_Tables = dba.GetTable(idSQL, idDA)



        'Try
        fi = New System.IO.FileInfo(filePath)
        If (fi.Exists) Then
            tr = fi.OpenText
            tmpLineCache = tr.ReadLine

            'Get table format


            Do While Not tmpLineCache Is Nothing
                checkString = Left(tmpLineCache, 2)
                If checkString <> "--" Then      'not a comment line. Process further
                    If checkString = " {" Then   'Beginning of an entry block
                        rawEntry = New System.Collections.Specialized.StringCollection
                        entryLine = tr.ReadLine
                        Do While Left(entryLine, 2) <> " }"
                            rawEntry.Add(entryLine)
                            entryLine = tr.ReadLine
                        Loop
                        ProcessTranslationEntry(rawEntry)
                        entryDA.Update(m_Translation_Entries)
                        idDA.Update(m_Translation_Tables)
                    End If
                    tmpLineCache = tr.ReadLine
                Else
                    tmpLineCache = tr.ReadLine           'comment line. Ignore
                End If
            Loop
        End If


        'Catch ex As Exception

        'End Try

    End Sub

    Private Sub SyncLocalToDMS()
        Dim dba As TableManipulationBase.IGetSQLData = New TableManipulationBase.clsDBTask(m_ConnectionString)

        Dim dmsDA As SqlClient.SqlDataAdapter = New SqlClient.SqlDataAdapter("SELECT * FROM " & clsTransTableHandler.EntriesTableName, dba.Connection)
        Dim dmsCB As SqlClient.SqlCommandBuilder = New SqlClient.SqlCommandBuilder(dmsDA)

        dmsCB.QuotePrefix = "["
        dmsCB.QuoteSuffix = "]"

        Dim dmsDS As DataSet = New DataSet

        dmsDA.Fill(dmsDS, clsTransTableHandler.EntriesTableName)



    End Sub


    Private Sub ProcessTranslationEntry(rawEntryCollection As System.Collections.Specialized.StringCollection)
        Dim id As Integer
        Dim AAList As String = String.Empty
        Dim StartList As String = String.Empty
        Dim Base1List As String = String.Empty
        Dim Base2List As String = String.Empty
        Dim Base3List As String = String.Empty
        Dim nameList As New System.Collections.Specialized.StringCollection
        Dim tmpNameList() As String
        Dim tmpName As String

        Dim tmp As String
        Dim tmpStartPos As Integer

        Dim trimString As String = " ,"""
        Dim trimChar As Char() = trimString.ToCharArray

        Dim s As String
        For Each s In rawEntryCollection
            s = s.Trim
            Select Case Left(s, 3)
                Case "nam"
                    tmp = s.TrimStart
                    tmpStartPos = InStr(tmp, " ") + 1
                    tmp = Mid(tmp, tmpStartPos)
                    tmp = tmp.Trim(trimChar)
                    tmpNameList = tmp.Split(";".ToCharArray)
                    For Each tmpName In tmpNameList
                        nameList.Add(tmpName)
                    Next

                Case "id "
                    tmp = s.TrimStart
                    tmp = Mid(tmp, InStr(tmp, " ") + 1)
                    tmp = tmp.TrimEnd(trimChar)
                    id = CInt(tmp)
                Case "ncb"
                    tmp = s.TrimStart
                    tmpStartPos = InStr(tmp, """") + 1
                    tmp = Mid(tmp, tmpStartPos)
                    tmp = tmp.TrimEnd(trimChar)
                    AAList = tmp
                Case "snc"
                    tmp = s.TrimStart
                    tmp = Mid(tmp, tmpStartPos)
                    tmp = tmp.TrimEnd(trimChar)
                    StartList = tmp
                Case "-- "

                    Select Case Left(s, 8)
                        Case "-- Base1"
                            Base1List = ProcessBaseString(s)
                        Case "-- Base2"
                            Base2List = ProcessBaseString(s)
                        Case "-- Base3"
                            Base3List = ProcessBaseString(s)
                        Case Else

                    End Select

                Case Else
            End Select
        Next

        Dim success As Boolean = SplitCodonEntries(AAList, StartList, Base1List, Base2List, Base3List, nameList, id)

    End Sub

    Private Function ProcessBaseString(rawBaseString As String) As String
        Dim tmpString As String
        tmpString = rawBaseString.TrimStart

        tmpString = Mid(tmpString, 11)

        Return tmpString

    End Function
    
    Private Function SplitCodonEntries(
        AAString As String,
        StartString As String,
        Base1List As String,
        Base2List As String,
        Base3List As String,
        NameList As System.Collections.Specialized.StringCollection,
        ID As Integer) As Boolean

        'Check for length consistency
        Dim baseLength As Integer = AAString.Length

        If baseLength <> StartString.Length Or
            baseLength <> Base1List.Length Or
            baseLength <> Base2List.Length Or
            baseLength <> Base3List.Length Then
            Return False
        End If

        Dim tmpAA As String
        Dim tmpStartString As String
        Dim tmpStart As Boolean
        Dim Base1 As String
        Dim Base2 As String
        Dim Base3 As String

        Dim counter As Integer
        Dim dr As DataRow

        Dim tmpName As String

        For Each tmpName In NameList
            dr = m_Translation_Tables.NewRow
            dr.Item("Translation_Table_Name") = tmpName.Trim & " (ID = " & CStr(ID) & ")"
            dr.Item("DNA_Translation_Table_ID") = ID
            m_Translation_Tables.Rows.Add(dr)
        Next


        Dim arrAA As Char() = AAString.ToCharArray

        For Each tmpAA In arrAA
            dr = m_Translation_Entries.NewRow
            counter += 1
            tmpStartString = Mid(StartString, counter, 1)
            If tmpStartString = "M" Then
                tmpStart = True
            Else
                tmpStart = False
            End If

            Base1 = Mid(Base1List, counter, 1)
            Base2 = Mid(Base2List, counter, 1)
            Base3 = Mid(Base3List, counter, 1)

            dr.Item("Coded_AA") = tmpAA
            dr.Item("Start_Sequence") = tmpStartString
            dr.Item("Base_1") = Base1
            dr.Item("Base_2") = Base2
            dr.Item("Base_3") = Base3
            dr.Item("DNA_Translation_Table_ID") = ID

            m_Translation_Entries.Rows.Add(dr)

        Next



    End Function
End Class


