using System;
using System.Collections.Generic;
using System.Data;
using PRISM;
using PRISMDatabaseUtils;

namespace TableManipulationBase
{
    public class DBTask : EventNotifier
    {
        #region "Member Variables"

#pragma warning disable CS3003 // Type is not CLS-compliant
        private readonly IDBTools mDBTools;
#pragma warning restore CS3003 // Type is not CLS-compliant

        #endregion

        #region "Properties"

        /// <summary>
        /// Database connection string
        /// </summary>
        /// <returns></returns>
        public string ConnectionString => mDBTools.ConnectStr;

        /// <summary>
        /// Database connection string
        /// </summary>
        /// <returns></returns>
#pragma warning disable CS3003 // Type is not CLS-compliant
        public IDBTools DBTools => mDBTools;
#pragma warning restore CS3003 // Type is not CLS-compliant

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public DBTask(string connectionString)
        {
            mDBTools = DbToolsFactory.GetDBTools(connectionString);
            RegisterEvents(mDBTools);
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public DBTask(IDBTools existingDbTools)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            mDBTools = existingDbTools;
        }

        public DataTable GetTableTemplate(string tableName)
        {
            string sql = "SELECT * FROM " + tableName + " WHERE 1=0";
            return GetTable(sql);
        }

        public DataTable GetTable(string selectSQL)
        {
            int retryCount = 6;
            int retryDelaySeconds = 5;
            int timeoutSeconds = 600;
            DataTable queryResults = null;
            bool success = mDBTools.GetQueryResultsDataTable(selectSQL, out queryResults, retryCount, retryDelaySeconds, timeoutSeconds);

            if (!success)
            {
                string errorMessage = "Could not get records after three tries; query: " + selectSQL;
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

            foreach (var dr in foundRows)
            {
                string key = dr[keyFieldName].ToString();
                if (!dataDictionary.ContainsKey(key))
                {
                    dataDictionary.Add(key, dr[valueFieldName].ToString());
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

            foreach (var dr in foundRows)
            {
                string key = dr[keyFieldName].ToString();
                int keyValue;
                if (!int.TryParse(key, out keyValue))
                {
                    continue;
                }

                if (!dataDictionary.ContainsKey(keyValue))
                {
                    dataDictionary.Add(keyValue, dr[valueFieldName].ToString());
                }
            }

            return dataDictionary;
        }

        private void ShowTrace(string message)
        {
            if (!ShowTraceMessages)
                return;
            Console.WriteLine("  " + message);
        }

        public bool ShowTraceMessages { get; set; }
    }
}