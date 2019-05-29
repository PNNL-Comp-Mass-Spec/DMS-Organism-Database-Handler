Imports System.Data.SqlClient
Imports System.Runtime.InteropServices

Public Interface IGetSQLData

    Function GetTable(selectSQL As String) As DataTable

    Function GetTable(
        selectSQL As String,
        <Out> ByRef SQLDataAdapter As SqlDataAdapter) As DataTable

    Function GetTableTemplate(tableName As String) As DataTable

    Function DataTableToHashtable(
        dt As DataTable,
        keyFieldName As String,
        valueFieldName As String,
        Optional filterString As String = "") As Hashtable

    Function DataTableToComplexHashtable(
        dt As DataTable,
        keyFieldName As String,
        valueFieldName As String,
        Optional filterString As String = "") As Hashtable

    Sub OpenConnection()
    Sub OpenConnection(connString As String)
    Sub CloseConnection()

    ReadOnly Property ConnectionString As String
    ReadOnly Property Connected As Boolean
    ReadOnly Property Connection As SqlConnection

End Interface

Public Class clsDBTask
    Implements IGetSQLData

#Region "Member Variables"

    ' DB access
    Protected m_DBCn As SqlConnection
    Protected ReadOnly m_PersistConnection As Boolean

#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connString"></param>
    ''' <param name="persistConnection"></param>
    Public Sub New(connString As String, Optional persistConnection As Boolean = False)
        ConnectionString = connString
        m_PersistConnection = persistConnection

        If m_PersistConnection Then
            OpenConnection(ConnectionString)
        Else
            ' Nothing to do; the connection will be opened later
        End If
    End Sub


    '------[for DB access]-----------------------------------------------------------
    Protected Sub OpenConnection() Implements IGetSQLData.OpenConnection
        If String.IsNullOrWhiteSpace(ConnectionString) Then
            Exit Sub
        End If
        OpenConnection(ConnectionString)
    End Sub

    Protected Sub OpenConnection(connString As String) Implements IGetSQLData.OpenConnection
        Const MAX_ATTEMPTS = 6

        If m_DBCn Is Nothing Then
            m_DBCn = New SqlConnection(connString)
        End If

        If m_DBCn.State <> ConnectionState.Open Then

            Dim connectionAttempt = 1
            While connectionAttempt <= MAX_ATTEMPTS
                Try
                    m_DBCn.Open()
                    Exit While
                Catch ex As Exception
                    connectionAttempt += 1
                    If connectionAttempt > 6 Then
                        Throw New Exception(String.Format(
                            "Could not open database connection after {0} tries using {1}: {2}",
                            MAX_ATTEMPTS, connString, ex.Message))
                    End If

                    Threading.Thread.Sleep(3000 * connectionAttempt)
                    m_DBCn.Close()
                End Try
            End While
        End If
    End Sub

    Protected Sub CloseConnection() Implements IGetSQLData.CloseConnection
        Try
            If Not m_DBCn Is Nothing AndAlso m_DBCn.State = ConnectionState.Open Then
                m_DBCn.Close()
                m_DBCn = Nothing
            End If
        Catch ex As Exception
            ' Ignore errors here
            Console.WriteLine("Warning, exception closing the database connection: " & ex.Message)
        End Try
    End Sub

    Protected ReadOnly Property Connected As Boolean Implements IGetSQLData.Connected
        Get
            If m_DBCn Is Nothing Then
                Return False
            Else
                If m_DBCn.State = ConnectionState.Open Then
                    Return True
                Else
                    Return False
                End If
            End If
        End Get
    End Property

    Protected ReadOnly Property ConnectionString As String Implements IGetSQLData.ConnectionString

    Protected ReadOnly Property Connection As SqlConnection Implements IGetSQLData.Connection
        Get
            If Connected Then
                Return m_DBCn
            Else
                OpenConnection()
                Return m_DBCn
            End If
        End Get
    End Property

    Protected Function GetTableTemplate(tableName As String) As DataTable Implements IGetSQLData.GetTableTemplate
        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql)
    End Function

    Protected Function GetTable(
        selectSQL As String,
        <Out> ByRef SQLDataAdapter As SqlDataAdapter) As DataTable Implements IGetSQLData.GetTable

        Const MAX_ATTEMPTS = 6
        Const COMMAND_TIMEOUT_SECONDS = 600

        Dim tmpIDTable As New DataTable

        If Not m_PersistConnection Then OpenConnection()

        Dim cmd = New SqlCommand(selectSQL) With {
            .CommandTimeout = COMMAND_TIMEOUT_SECONDS,
            .Connection = m_DBCn
        }

        If Connected = True Then

            SQLDataAdapter = New SqlDataAdapter With {
                .SelectCommand = cmd
            }

            Dim connectionAttempt = 1
            While connectionAttempt <= MAX_ATTEMPTS

                Try
                    SQLDataAdapter.Fill(tmpIDTable)
                    Exit While
                Catch ex As Exception
                    connectionAttempt += 1
                    If connectionAttempt > MAX_ATTEMPTS Then
                        Throw New Exception(String.Format("Could not get records after {0} attempts for query {1}: {2}", MAX_ATTEMPTS, selectSQL, ex.Message))
                    End If
                    Threading.Thread.Sleep(3000 * connectionAttempt)
                End Try

            End While

            If Not m_PersistConnection Then CloseConnection()
        Else
            SQLDataAdapter = Nothing
            tmpIDTable = Nothing
        End If

        Return tmpIDTable

    End Function

    Protected Function GetTable(selectSQL As String) As DataTable Implements IGetSQLData.GetTable
        Dim tmpDA As SqlDataAdapter = Nothing

        Dim tmpTable As DataTable = GetTable(selectSQL, tmpDA)

        tmpDA.Dispose()

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
      dt As DataTable,
      keyFieldName As String,
      valueFieldName As String,
      Optional filterString As String = "") As Hashtable Implements IGetSQLData.DataTableToHashtable

        Dim foundRows() As DataRow = dt.Select(filterString)
        Dim ht As New Hashtable(foundRows.Length)

        For Each dr In foundRows
            Dim key = dr.Item(keyFieldName).ToString()
            If Not ht.Contains(key) Then
                ht.Add(key, dr.Item(valueFieldName).ToString())
            End If
        Next

        Return ht

    End Function


    Protected Function DataTableToComplexHashTable(
        dt As DataTable,
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
