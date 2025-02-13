using System;
using System.Collections.Generic;
using System.Data;
using PRISM;
using PRISMDatabaseUtils;

namespace OrganismDatabaseHandler.DatabaseTools
{
    public class DBTask : EventNotifier
    {

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString => DbTools.ConnectStr;

        /// <summary>
        /// Database connection string
        /// </summary>
#pragma warning disable CS3003 // Type is not CLS-compliant
        public IDBTools DbTools { get; }
#pragma warning restore CS3003 // Type is not CLS-compliant

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        public DBTask(string connectionString)
        {
            DbTools = DbToolsFactory.GetDBTools(connectionString);
            RegisterEvents(DbTools);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public DBTask(IDBTools existingDbTools)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            DbTools = existingDbTools;
        }

        public DataTable GetTableTemplate(string tableName)
        {
            var sql = "SELECT * FROM " + tableName + " WHERE 1=0";
            return GetTable(sql);
        }

        public DataTable GetTable(string selectSql)
        {
            const int retryCount = 6;
            const int retryDelaySeconds = 5;
            const int timeoutSeconds = 600;
            var success = DbTools.GetQueryResultsDataTable(selectSql, out var queryResults, retryCount, retryDelaySeconds, timeoutSeconds);

            if (!success)
            {
                var errorMessage = "Could not get records after three tries; query: " + selectSql;
                OnErrorEvent(errorMessage);
                throw new Exception(errorMessage);
            }

            return queryResults;
        }

        public Dictionary<string, string> DataTableToDictionary(
            DataTable dt,
            string keyFieldName,
            string valueFieldName,
            string filterString = "")
        {
            var foundRows = dt.Select(filterString);
            var dataDictionary = new Dictionary<string, string>(foundRows.Length);

            foreach (var dataRow in foundRows)
            {
                var key = dataRow[keyFieldName].ToString();

                if (!dataDictionary.ContainsKey(key))
                {
                    dataDictionary.Add(key, dataRow[valueFieldName].ToString());
                }
            }

            return dataDictionary;
        }

        public Dictionary<int, string> DataTableToDictionaryIntegerKeys(
            DataTable dt,
            string keyFieldName,
            string valueFieldName,
            string filterString = "")
        {
            var foundRows = dt.Select(filterString);
            var dataDictionary = new Dictionary<int, string>(foundRows.Length);

            foreach (var dataRow in foundRows)
            {
                var key = dataRow[keyFieldName].ToString();

                if (!int.TryParse(key, out var keyValue))
                {
                    continue;
                }

                if (!dataDictionary.ContainsKey(keyValue))
                {
                    dataDictionary.Add(keyValue, dataRow[valueFieldName].ToString());
                }
            }

            return dataDictionary;
        }
    }
}