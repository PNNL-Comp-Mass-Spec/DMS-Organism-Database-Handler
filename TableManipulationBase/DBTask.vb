Imports System.Collections.Generic
Imports PRISM
Imports PRISMDatabaseUtils

Public Class DBTask
    Inherits EventNotifier

#Region "Member Variables"

#Disable Warning BC40025 ' Type of member is not CLS-compliant
    Protected ReadOnly mDBTools As IDBTools
#Enable Warning BC40025 ' Type of member is not CLS-compliant

#End Region

#Region "Properties"

    ''' <summary>
    ''' Database connection string
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ConnectionString As String
        Get
            Return mDBTools.ConnectStr
        End Get
    End Property

#Disable Warning BC40027 ' Return type of function is not CLS-compliant

    ''' <summary>
    ''' Database connection string
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property DBTools As IDBTools
        Get
            Return mDBTools
        End Get
    End Property

#Enable Warning BC40027 ' Return type of function is not CLS-compliant

#End Region
    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="connectionString"></param>
    Public Sub New(connectionString As String)
        mDBTools = DbToolsFactory.GetDBTools(connectionString)
        RegisterEvents(mDBTools)
    End Sub

#Disable Warning BC40028 ' Type of parameter is not CLS-compliant
    Public Sub New(existingDbTools As IDBTools)
        mDBTools = existingDbTools
    End Sub
#Enable Warning BC40028 ' Type of parameter is not CLS-compliant

    Public Function GetTableTemplate(tableName As String) As DataTable
        Dim sql As String = "SELECT * FROM " & tableName & " WHERE 1=0"
        Return GetTable(sql)
    End Function

    Public Function GetTable(selectSQL As String) As DataTable

        Dim retryCount = 6
        Dim retryDelaySeconds = 5
        Dim timeoutSeconds = 600

        Dim queryResults As DataTable = Nothing
        Dim success = mDBTools.GetQueryResultsDataTable(selectSQL, queryResults, retryCount, retryDelaySeconds, timeoutSeconds)

        If Not success Then
            Dim errorMessage = "Could not get records after three tries; query: " & selectSQL
            OnErrorEvent(errorMessage)
            Throw New Exception(errorMessage)
        End If

        Return queryResults

    End Function

    Public Function DataTableToDictionary(
      dt As DataTable,
      keyFieldName As String,
      valueFieldName As String,
      Optional filterString As String = "") As Dictionary(Of String, String)

        Dim foundRows() As DataRow = dt.Select(filterString)
        Dim dataDictionary = New Dictionary(Of String, String)(foundRows.Length)

        For Each dr In foundRows
            Dim key = dr.Item(keyFieldName).ToString()
            If Not dataDictionary.ContainsKey(key) Then
                dataDictionary.Add(key, dr.Item(valueFieldName).ToString())
            End If
        Next

        Return dataDictionary

    End Function

    Public Function DataTableToDictionaryIntegerKeys(
      dt As DataTable,
      keyFieldName As String,
      valueFieldName As String,
      Optional filterString As String = "") As Dictionary(Of Integer, String)

        Dim foundRows() As DataRow = dt.Select(filterString)
        Dim dataDictionary = New Dictionary(Of Integer, String)(foundRows.Length)

        For Each dr In foundRows
            Dim key = dr.Item(keyFieldName).ToString()
            Dim keyValue As Integer
            If Not Integer.TryParse(key, keyValue) Then
                Continue For
            End If

            If Not dataDictionary.ContainsKey(keyValue) Then
                dataDictionary.Add(keyValue, dr.Item(valueFieldName).ToString())
            End If
        Next

        Return dataDictionary

    End Function

    Private Sub ShowTrace(message As String)
        If Not ShowTraceMessages Then Exit Sub

        Console.WriteLine("  " & message)
    End Sub

    Public Property ShowTraceMessages As Boolean

End Class
