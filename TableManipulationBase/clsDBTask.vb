Imports System.Collections.Specialized
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb

Public Interface IGetSQLData
    Function GetTable(SelectSQL As String) As DataTable
    Function GetTable(
        SelectSQL As String,
        ByRef SQLDataAdapter As SqlDataAdapter,
        ByRef SQLCommandBuilder As SqlCommandBuilder) As DataTable

    Function GetTableTemplate(TableName As String) As DataTable
    Function GetTableTemplate(
        TableName As String,
        ByRef SQLDataAdapter As SqlDataAdapter,
        ByRef SQLCommandBuilder As SqlCommandBuilder) As DataTable
    Function DataTableToHashtable(
        ByRef dt As DataTable,
        keyFieldName As String,
        valueFieldName As String,
        Optional filterString As String = "") As Hashtable
    Function DataTableToComplexHashtable(
        ByRef dt As DataTable,
        keyFieldName As String,
        valueFieldName As String,
        Optional filterString As String = "") As Hashtable

    Sub OpenConnection()
    Sub OpenConnection(connString As String)
    Sub CloseConnection()

    Property ConnectionString() As String
    ReadOnly Property Connected() As Boolean
    ReadOnly Property Connection() As SqlConnection

End Interface

Public Class clsDBTask
    Implements IGetSQLData

#Region "Member Variables"

    ' DB access
    Protected m_connection_str As String
    Protected m_DBCn As SqlConnection
    Protected m_error_list As New StringCollection
    Protected m_PersistConnection As Boolean

#End Region

    ' constructor
    Public Sub New(connString As String, Optional persistConnection As Boolean = False)
        Me.m_connection_str = connString
        Me.SetupNew(persistConnection)
    End Sub

    Public Sub New(Optional persistConnection As Boolean = False)
        Me.SetupNew(persistConnection)
    End Sub

    Private Sub SetupNew(persistConnection As Boolean)
        Me.m_PersistConnection = persistConnection
        If Me.m_PersistConnection Then
            Me.OpenConnection(Me.m_connection_str)
        Else
            ' Nothing to do
        End If
    End Sub


    '------[for DB access]-----------------------------------------------------------
    Protected Sub OpenConnection() Implements IGetSQLData.OpenConnection
        If Me.m_connection_str = "" Then
            Exit Sub
        End If
        OpenConnection(Me.m_connection_str)
    End Sub

    Protected Sub OpenConnection(connString As String) Implements IGetSQLData.OpenConnection
        Dim retryCount As Integer = 3
        If m_DBCn Is Nothing Then
            m_DBCn = New SqlConnection(connString)
        End If
        If m_DBCn.State <> ConnectionState.Open Then
            While retryCount > 0
                Try
                    m_DBCn.Open()
                    retryCount = 0
                Catch ex As SqlException
                    retryCount -= 1
                    If retryCount = 0 Then
                        Throw New Exception("could not open database connection after three tries using " & connString & ": " & ex.Message)
                    End If
                    System.Threading.Thread.Sleep(3000)
                    m_DBCn.Close()
                End Try
            End While
        End If
    End Sub

    Protected Sub CloseConnection() Implements IGetSQLData.CloseConnection
        If Not m_DBCn Is Nothing Then
            m_DBCn.Close()
            m_DBCn = Nothing
        End If
    End Sub

    Protected ReadOnly Property Connected() As Boolean Implements IGetSQLData.Connected
        Get
            If m_DBCn Is Nothing Then
                Return False
            Else
                If Me.m_DBCn.State = ConnectionState.Open Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Get
    End Property

    Protected Property ConnectionString() As String Implements IGetSQLData.ConnectionString
        Get
            Return Me.m_connection_str
        End Get
        Set(Value As String)
            Me.m_connection_str = Value
        End Set
    End Property

    Protected ReadOnly Property Connection() As SqlConnection Implements IGetSQLData.Connection
        Get
            If Me.Connected Then
                Return Me.m_DBCn
            Else
                Me.OpenConnection()
                Return Me.m_DBCn
            End If
        End Get
    End Property

    Protected Function GetTableTemplate(
        tableName As String,
        ByRef SQLDataAdapter As SqlDataAdapter,
        ByRef SQLCommandBuilder As SqlCommandBuilder) As DataTable Implements IGetSQLData.GetTableTemplate

        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql, SQLDataAdapter, SQLCommandBuilder)

    End Function

    Protected Function GetTableTemplate(tableName As String) As DataTable Implements IGetSQLData.GetTableTemplate
        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql)
    End Function

    Protected Function GetTable(
        SelectSQL As String,
        ByRef SQLDataAdapter As SqlDataAdapter,
        ByRef SQLCommandBuilder As SqlCommandBuilder) As DataTable Implements IGetSQLData.GetTable

        Dim tmpIDTable As New DataTable
        Dim GetID_CMD As SqlCommand = New SqlCommand(SelectSQL)

        Dim numTries As Integer = 3

        If Not Me.m_PersistConnection Then Me.OpenConnection()

        GetID_CMD.CommandTimeout = 600
        GetID_CMD.Connection = Me.m_DBCn

        If Me.Connected = True Then

            SQLDataAdapter = New SqlDataAdapter
            SQLCommandBuilder = New SqlCommandBuilder(SQLDataAdapter)
            SQLDataAdapter.SelectCommand = GetID_CMD

            While numTries > 0
                Try
                    SQLDataAdapter.Fill(tmpIDTable)
                    Exit While
                Catch ex As Exception
                    numTries -= 1
                    If numTries = 0 Then
                        Throw New Exception("could not get records after three tries: " & ex.Message)
                    End If
                    System.Threading.Thread.Sleep(3000)
                End Try

            End While

            'SQLDataAdapter.Dispose()
            'SQLDataAdapter = Nothing

            If Not Me.m_PersistConnection Then Me.CloseConnection()
        Else
            tmpIDTable = Nothing
        End If

        Return tmpIDTable

    End Function

    Protected Function GetTable(SelectSQL As String) As DataTable Implements IGetSQLData.GetTable
        Dim tmpDA As SqlDataAdapter = Nothing
        Dim tmpCB As SqlCommandBuilder = Nothing

        Dim tmpTable As DataTable = GetTable(SelectSQL, tmpDA, tmpCB)
        tmpDA.Dispose()
        tmpDA = Nothing

        tmpCB.Dispose()
        tmpCB = Nothing

        Return tmpTable

    End Function

    Protected Sub CreateRelationship(
        ds As DataSet,
        dt1 As DataTable,
        dt1_keyFieldName As String,
        dt2 As DataTable,
        dt2_keyFieldName As String)

        Dim dc_dt1_keyField As DataColumn = dt1.Columns(dt1_keyFieldName)
        Dim dc_dt2_keyField As DataColumn = dt2.Columns(dt2_keyFieldName)
        ds.Relations.Add(dc_dt1_keyField, dc_dt2_keyField)

    End Sub

    Protected Sub SetPrimaryKey(
        keyColumnIndex As Integer,
        dt As DataTable)

        Dim pKey(0) As DataColumn
        pKey(0) = dt.Columns(keyColumnIndex)
        dt.PrimaryKey = pKey

    End Sub

    Protected Function DataTableToHashTable(
    ByRef dt As DataTable,
    keyFieldName As String,
    valueFieldName As String,
    Optional filterString As String = "") As Hashtable Implements IGetSQLData.DataTableToHashtable

        Dim dr As DataRow
        Dim foundRows() As DataRow = dt.Select(filterString)
        Dim ht As New Hashtable(foundRows.Length)
        'Dim al As ArrayList
        Dim key As String

        For Each dr In foundRows
            key = dr.Item(keyFieldName).ToString
            If Not ht.Contains(key) Then
                ht.Add(key, dr.Item(valueFieldName).ToString)
            End If
            'If ht.Contains(key) Then
            '    al = DirectCast(ht(key), ArrayList)
            'Else
            '    al = New ArrayList
            'End If
            'al.Add(dr.Item(valueFieldName).ToString)
            'ht(key) = al
        Next

        Return ht

    End Function


    Protected Function DataTableToComplexHashTable(
        ByRef dt As DataTable,
        keyFieldName As String,
        valueFieldName As String,
        Optional filterString As String = "") As Hashtable Implements IGetSQLData.DataTableToComplexHashtable

        Dim dr As DataRow
        Dim foundRows() As DataRow = dt.Select(filterString)
        Dim ht As New Hashtable(foundRows.Length)
        Dim al As ArrayList
        Dim key As String

        For Each dr In foundRows
            key = dr.Item(keyFieldName).ToString
            If ht.Contains(key) Then
                al = DirectCast(ht(key), ArrayList)
            Else
                al = New ArrayList
            End If
            al.Add(dr.Item(valueFieldName).ToString)
            ht(key) = al
        Next

        Return ht

    End Function


End Class
