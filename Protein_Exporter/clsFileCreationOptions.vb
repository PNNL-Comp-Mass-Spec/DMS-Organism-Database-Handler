Option Strict On

Imports System.Text
Imports System.Text.RegularExpressions
Imports Protein_Exporter.ExportProteinCollectionsIFC
Imports TableManipulationBase

Friend Class clsFileCreationOptions
    Private ReadOnly m_TableGetter As IGetSQLData
    Private ReadOnly m_PSConnectionString As String
    Private m_SeqDirection As IGetFASTAFromDMS.SequenceTypes
    Private m_FileType As IGetFASTAFromDMS.DatabaseFormatTypes
    Private m_CreationValuesTable As DataTable
    Private m_KeywordTable As DataTable

    Sub New(PSConnectionString As String)
        m_PSConnectionString = PSConnectionString
        m_TableGetter = New clsDBTask(m_PSConnectionString, True)
    End Sub

    ReadOnly Property SequenceDirection As IGetFASTAFromDMS.SequenceTypes
        Get
            Return m_SeqDirection
        End Get
    End Property

    ReadOnly Property FileFormatType As IGetFASTAFromDMS.DatabaseFormatTypes
        Get
            Return m_FileType
        End Get
    End Property
    'Options string looks like... "seq_direction=forward;filetype=fasta"
    Function ExtractOptions(optionsString As String) As String
        Dim keywordTableSQL As String
        'Dim valuesTableSQL As String
        Dim creationValuesSQL As String
        Dim optionsHash As New Hashtable
        Dim mCollection As MatchCollection
        Dim m As Match
        Dim optionsStringParser As Regex
        Dim dr As DataRow
        Dim foundRows() As DataRow
        Dim checkRows() As DataRow
        Dim errorString As StringBuilder

        Dim tmpKeyword As String
        Dim tmpValue As String

        Dim validKeyword As Boolean
        Dim validValue As Boolean

        Dim cleanOptionsString As New StringBuilder

        keywordTableSQL = "SELECT Keyword_ID, Keyword, Default_Value FROM T_Creation_Option_Keywords"
        'valuesTableSQL = "SELECT Value_ID, Value_String, Keyword_ID FROM T_Creation_Option_Values"
        creationValuesSQL = "SELECT Keyword, Value_String, String_Element FROM V_Creation_String_Lookup"

        If m_KeywordTable Is Nothing Then
            m_KeywordTable = m_TableGetter.GetTable(keywordTableSQL)
        End If

        'If m_ValuesTable Is Nothing Then
        '    m_ValuesTable = m_TableGetter.GetTable(valuesTableSQL)
        'End If

        If m_CreationValuesTable Is Nothing Then
            m_CreationValuesTable = m_TableGetter.GetTable(creationValuesSQL)
        End If

        'optionsStringParser = New System.Text.RegularExpressions.Regex(
        '    "(?<keyword>\S+)\s*=\s*(?<value>\S+),*?")
        optionsStringParser = New Regex(
            "(?<keyword>[^,\s]*)\s*=\s*(?<value>[^,\s]+)")

        mCollection = optionsStringParser.Matches(optionsString)

        For Each m In mCollection
            'optionsHash.Add(m.Groups("keyword").Value, m.Groups("value").Value)
            tmpKeyword = m.Groups("keyword").Value
            tmpValue = m.Groups("value").Value

            'Check for valid keyword/value pair
            foundRows = m_CreationValuesTable.Select("Keyword = '" & tmpKeyword & "' AND Value_String = '" & tmpValue & "'")
            If foundRows.Length < 1 Then
                'check if keyword or value is bad
                errorString = New StringBuilder
                checkRows = m_CreationValuesTable.Select("Keyword = '" & tmpKeyword)
                If checkRows.Length > 0 Then validKeyword = True

                checkRows = m_CreationValuesTable.Select("Value_String = '" & tmpValue & "'")
                If checkRows.Length > 0 Then validValue = True

                If Not validKeyword Then
                    errorString.Append("Keyword: " & tmpKeyword & " is not valid")
                End If

                If Not validValue Then
                    If errorString.ToString.Length > 0 Then errorString.Append(", ")
                    errorString.Append("Value: " & tmpValue & "is not a valid option")
                End If
                Throw New Exception(errorString.ToString)
            End If

            If optionsHash.ContainsKey(tmpKeyword) Then
                Throw New Exception(tmpKeyword & " is a duplicate keyword")
            Else
                optionsHash.Add(tmpKeyword, tmpValue)
            End If

        Next

        'parse hashtable into canonical options string for return
        foundRows = m_KeywordTable.Select("", "Keyword_ID ASC")
        For Each dr In foundRows
            If cleanOptionsString.ToString.Length > 0 Then
                cleanOptionsString.Append(",")
            End If

            tmpKeyword = dr.Item("Keyword").ToString
            If optionsHash.ContainsKey(tmpKeyword) Then
                tmpValue = optionsHash.Item(tmpKeyword).ToString
            Else
                tmpValue = dr.Item("Default_Value").ToString
            End If


            Select Case tmpKeyword
                Case "seq_direction"
                    m_SeqDirection = DirectCast([Enum].Parse(GetType(IGetFASTAFromDMS.SequenceTypes), tmpValue),
                        IGetFASTAFromDMS.SequenceTypes)

                Case "filetype"
                    m_FileType = DirectCast([Enum].Parse(GetType(IGetFASTAFromDMS.DatabaseFormatTypes), tmpValue),
                        IGetFASTAFromDMS.DatabaseFormatTypes)
            End Select

            With cleanOptionsString
                .Append(tmpKeyword)
                .Append("=")
                .Append(tmpValue)
            End With
        Next


        Return cleanOptionsString.ToString

    End Function

    Function MakeCreationOptionsString(
        seqDirection As IGetFASTAFromDMS.SequenceTypes,
        databaseFormatType As IGetFASTAFromDMS.DatabaseFormatTypes) As String

        Dim creationOptionsSB As New StringBuilder

        With creationOptionsSB
            .Append("seq_direction=")
            .Append(seqDirection.ToString)
            .Append(",")
            .Append("filetype=")
            .Append(databaseFormatType.ToString)
        End With

        Return creationOptionsSB.ToString

    End Function

End Class
