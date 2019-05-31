Imports System.Data.SqlClient
Imports System.Runtime.InteropServices

Public Class clsDBTask

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
    Public Sub New(connString As String, Optional persistConnection As Boolean = False, Optional enableTraceMessages As Boolean = False)
        ConnectionString = connString
        m_PersistConnection = persistConnection
        ShowTraceMessages = enableTraceMessages

        If m_PersistConnection Then
            OpenConnection(ConnectionString)
        Else
            ' Nothing to do; the connection will be opened later
        End If
    End Sub


    '------[for DB access]-----------------------------------------------------------
    Protected Sub OpenConnection()
        If String.IsNullOrWhiteSpace(ConnectionString) Then
            Exit Sub
        End If
        OpenConnection(ConnectionString)
    End Sub

    Protected Sub OpenConnection(connString As String)
        Const MAX_ATTEMPTS = 6

        ShowTrace("Opening a database connection, connection string: " & connString)

        If m_DBCn Is Nothing Then
            ShowTrace("Instantiating a new instance of SqlConnection")
            m_DBCn = New SqlConnection(connString)
        End If

        If m_DBCn.State <> ConnectionState.Open Then

            Dim connectionAttempt = 1
            While connectionAttempt <= MAX_ATTEMPTS
                Try
                    ShowTrace("Calling m_DBCn.Open")
                    m_DBCn.Open()
                    Exit While
                Catch ex As Exception
                    connectionAttempt += 1
                    If connectionAttempt > 6 Then
                        Throw New Exception(String.Format(
                            "Could not open database connection after {0} tries using {1}: {2}",
                            MAX_ATTEMPTS, connString, ex.Message))
                    End If

                    ShowTrace("Exception opening the connection: " & ex.Message)

                    Threading.Thread.Sleep(3000 * connectionAttempt)

                    ShowTrace("Closing connection and trying again")
                    m_DBCn.Close()
                End Try
            End While
        Else
            ShowTrace("The database connection is already open")

        End If
    End Sub

    Public Sub CloseConnection()
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

    Public ReadOnly Property Connected As Boolean
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

    Public ReadOnly Property ConnectionString As String

    Public ReadOnly Property Connection As SqlConnection
        Get
            If Connected Then
                Return m_DBCn
            Else
                OpenConnection()
                Return m_DBCn
            End If
        End Get
    End Property

    Public Function GetTableTemplate(tableName As String) As DataTable
        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql)
    End Function

    Public Function GetTable(
        selectSQL As String,
        <Out> ByRef dataAdapter As SqlDataAdapter) As DataTable

        Const MAX_ATTEMPTS = 6
        Const COMMAND_TIMEOUT_SECONDS = 600

        Dim tmpIDTable As New DataTable

        If Not m_PersistConnection Then OpenConnection()

        Dim cmd = New SqlCommand(selectSQL) With {
            .CommandTimeout = COMMAND_TIMEOUT_SECONDS,
            .Connection = m_DBCn
        }

        If Connected = True Then

            dataAdapter = New SqlDataAdapter With {
                .SelectCommand = cmd
            }

            Dim connectionAttempt = 1
            While connectionAttempt <= MAX_ATTEMPTS

                Try
                    dataAdapter.Fill(tmpIDTable)
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
            dataAdapter = Nothing
            tmpIDTable = Nothing
        End If

        Return tmpIDTable

    End Function

    Public Function GetTable(selectSQL As String) As DataTable
        Dim dataAdapter As SqlDataAdapter = Nothing

        Dim tmpTable As DataTable = GetTable(selectSQL, dataAdapter)

        dataAdapter.Dispose()

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

    Public Function DataTableToHashTable(
      dt As DataTable,
      keyFieldName As String,
      valueFieldName As String,
      Optional filterString As String = "") As Hashtable

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
        Optional filterString As String = "") As Hashtable

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

    Private Sub ShowTrace(message As String)
        If Not ShowTraceMessages Then Exit Sub

        Console.WriteLine("  " & message)
    End Sub

    Public Property ShowTraceMessages As Boolean

End Class
