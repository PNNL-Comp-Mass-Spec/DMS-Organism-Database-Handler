Imports System.Collections.Specialized
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb

Public Interface IGetSQLData
    Function GetTable(ByVal SelectSQL As String) As DataTable
    Function GetTable( _
        ByVal SelectSQL As String, _
        ByRef SQLDataAdapter As SqlClient.SqlDataAdapter, _
        ByRef SQLCommandBuilder As SqlClient.SqlCommandBuilder) As DataTable

    Function GetTableTemplate(ByVal TableName As String) As DataTable
    Function GetTableTemplate( _
        ByVal TableName As String, _
        ByRef SQLDataAdapter As SqlClient.SqlDataAdapter, _
        ByRef SQLCommandBuilder As SqlClient.SqlCommandBuilder) As DataTable
    Function DataTableToHashtable( _
        ByRef dt As DataTable, _
        ByVal keyFieldName As String, _
        ByVal valueFieldName As String, _
        Optional ByVal filterString As String = "") As Hashtable
    Function DataTableToComplexHashtable( _
        ByRef dt As DataTable, _
        ByVal keyFieldName As String, _
        ByVal valueFieldName As String, _
        Optional ByVal filterString As String = "") As Hashtable

    Sub OpenConnection()
    Sub OpenConnection(ByVal connString As String)
    Sub CloseConnection()

    Property ConnectionString() As String
    ReadOnly Property Connected() As Boolean
    ReadOnly Property Connection() As SqlClient.SqlConnection

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
    Public Sub New(ByVal connString As String, Optional ByVal persistConnection As Boolean = False)
        Me.m_connection_str = connString
        Me.SetupNew(persistConnection)
    End Sub

    Public Sub New(Optional ByVal persistConnection As Boolean = False)
        Me.SetupNew(persistConnection)
    End Sub

    Private Sub SetupNew(ByVal persistConnection As Boolean)
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

    Protected Sub OpenConnection(ByVal connString As String) Implements IGetSQLData.OpenConnection
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
        Set(ByVal Value As String)
            Me.m_connection_str = Value
        End Set
    End Property

    Protected ReadOnly Property Connection() As SqlClient.SqlConnection Implements IGetSQLData.Connection
        Get
            If Me.Connected Then
                Return Me.m_DBCn
            Else
                Me.OpenConnection()
                Return Me.m_DBCn
            End If
        End Get
    End Property

    Protected Function GetTableTemplate( _
        ByVal tableName As String, _
        ByRef SQLDataAdapter As SqlClient.SqlDataAdapter, _
        ByRef SQLCommandBuilder As SqlClient.SqlCommandBuilder) As DataTable Implements IGetSQLData.GetTableTemplate

        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql, SQLDataAdapter, SQLCommandBuilder)

    End Function

    Protected Function GetTableTemplate(ByVal tableName As String) As DataTable Implements IGetSQLData.GetTableTemplate
        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql)
    End Function

    Protected Function GetTable( _
        ByVal SelectSQL As String, _
        ByRef SQLDataAdapter As SqlClient.SqlDataAdapter, _
        ByRef SQLCommandBuilder As SqlClient.SqlCommandBuilder) As DataTable Implements IGetSQLData.GetTable

        Dim tmpIDTable As New DataTable
        Dim GetID_CMD As SqlClient.SqlCommand = New SqlClient.SqlCommand(SelectSQL)

        Dim numTries As Integer = 3

        If Not Me.m_PersistConnection Then Me.OpenConnection()

        GetID_CMD.CommandTimeout = 600
        GetID_CMD.Connection = Me.m_DBCn

        If Me.Connected = True Then

            SQLDataAdapter = New SqlClient.SqlDataAdapter
            SQLCommandBuilder = New SqlClient.SqlCommandBuilder(SQLDataAdapter)
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

    Protected Function GetTable(ByVal SelectSQL As String) As DataTable Implements IGetSQLData.GetTable
		Dim tmpDA As SqlClient.SqlDataAdapter = Nothing
		Dim tmpCB As SqlClient.SqlCommandBuilder = Nothing

        Dim tmpTable As DataTable = GetTable(SelectSQL, tmpDA, tmpCB)
        tmpDA.Dispose()
        tmpDA = Nothing

        tmpCB.Dispose()
        tmpCB = Nothing

        Return tmpTable

    End Function

    Protected Sub CreateRelationship( _
        ByVal ds As DataSet, _
        ByVal dt1 As DataTable, _
        ByVal dt1_keyFieldName As String, _
        ByVal dt2 As DataTable, _
        ByVal dt2_keyFieldName As String)

        Dim dc_dt1_keyField As DataColumn = dt1.Columns(dt1_keyFieldName)
        Dim dc_dt2_keyField As DataColumn = dt2.Columns(dt2_keyFieldName)
        ds.Relations.Add(dc_dt1_keyField, dc_dt2_keyField)

    End Sub

    Protected Sub SetPrimaryKey( _
        ByVal keyColumnIndex As Integer, _
        ByVal dt As DataTable)

        Dim pKey(0) As DataColumn
        pKey(0) = dt.Columns(keyColumnIndex)
        dt.PrimaryKey = pKey

    End Sub

    Protected Function DataTableToHashTable( _
    ByRef dt As DataTable, _
    ByVal keyFieldName As String, _
    ByVal valueFieldName As String, _
    Optional ByVal filterString As String = "") As Hashtable Implements IGetSQLData.DataTableToHashtable

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


    Protected Function DataTableToComplexHashTable( _
        ByRef dt As DataTable, _
        ByVal keyFieldName As String, _
        ByVal valueFieldName As String, _
        Optional ByVal filterString As String = "") As Hashtable Implements IGetSQLData.DataTableToComplexHashtable

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
