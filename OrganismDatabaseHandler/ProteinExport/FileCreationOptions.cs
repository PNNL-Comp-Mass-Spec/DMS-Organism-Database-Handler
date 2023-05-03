using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    internal class FileCreationOptions
    {
        // Ignore Spelling: filetype

        private readonly DBTask mDatabaseAccessor;
        private DataTable mCreationValuesTable;
        private DataTable mKeywordTable;

        public FileCreationOptions(DBTask databaseAccessor)
        {
            mDatabaseAccessor = databaseAccessor;
        }

        public GetFASTAFromDMS.SequenceTypes SequenceDirection { get; private set; }

        // Options string looks like... "seq_direction=forward;filetype=fasta"
        public string ExtractOptions(string optionsString)
        {
            var optionsHash = new Dictionary<string, string>();

            DataRow[] foundRows;

            var validKeyword = default(bool);
            var validValue = default(bool);

            var cleanOptionsString = new StringBuilder();

            const string keywordTableSQL = "SELECT keyword_id, keyword, default_value FROM t_creation_option_keywords";
            //var valuesTableSQL = "SELECT value_id, value_string, keyword_id FROM t_creation_option_values";
            const string creationValuesSQL = "SELECT keyword, value_string, string_element FROM v_creation_string_lookup";

            mKeywordTable ??= mDatabaseAccessor.GetTable(keywordTableSQL);

            // If mValuesTable Is Nothing Then
            // mValuesTable = mDatabaseAccessor.GetTable(valuesTableSQL)
            // End If

            mCreationValuesTable ??= mDatabaseAccessor.GetTable(creationValuesSQL);

            //var optionsStringParser = new System.Text.RegularExpressions.Regex(
            //    "(?<keyword>\S+)\s*=\s*(?<value>\S+),*?")
            var optionsStringParser = new Regex(
                @"(?<keyword>[^,\s]*)\s*=\s*(?<value>[^,\s]+)");

            foreach (Match m in optionsStringParser.Matches(optionsString))
            {
                // optionsHash.Add(m.Groups("keyword").Value, m.Groups("value").Value)
                var keyword = m.Groups["keyword"].Value;
                var value = m.Groups["value"].Value;

                // Check for valid keyword/value pair
                foundRows = mCreationValuesTable.Select("keyword = '" + keyword + "' AND value_String = '" + value + "'");

                if (foundRows.Length < 1)
                {
                    // check if keyword or value is bad
                    var errorString = new StringBuilder();

                    var checkRows = mCreationValuesTable.Select("keyword = '" + keyword);

                    if (checkRows.Length > 0)
                        validKeyword = true;

                    checkRows = mCreationValuesTable.Select("value_string = '" + value + "'");

                    if (checkRows.Length > 0)
                        validValue = true;

                    if (!validKeyword)
                    {
                        errorString.AppendFormat("Keyword: {0} is not valid", keyword);
                    }

                    if (!validValue)
                    {
                        if (errorString.ToString().Length > 0)
                            errorString.Append(", ");
                        errorString.AppendFormat("Value: {0} is not a valid option", value);
                    }

                    throw new Exception(errorString.ToString());
                }

                if (optionsHash.ContainsKey(keyword))
                {
                    throw new Exception(keyword + " is a duplicate keyword");
                }

                optionsHash.Add(keyword, value);
            }

            // Parse dictionary into canonical options string for return
            foundRows = mKeywordTable.Select(string.Empty, "keyword_id ASC");

            foreach (var dataRow in foundRows)
            {
                if (cleanOptionsString.ToString().Length > 0)
                {
                    cleanOptionsString.Append(",");
                }

                var keyword = dataRow["keyword"].ToString();
                var value = optionsHash.TryGetValue(keyword, out var definedValue)
                    ? definedValue
                    : dataRow["default_value"].ToString();

                switch (keyword)
                {
                    case "seq_direction":
                        // Convert from string to an enum (case insensitive matching to enum value names)
                        SequenceDirection = (GetFASTAFromDMS.SequenceTypes)Enum.Parse(typeof(GetFASTAFromDMS.SequenceTypes), value, true);
                        break;

                    case "filetype":
                        // Deprecated; ignore
                        break;
                }

                cleanOptionsString.Append(keyword);
                cleanOptionsString.Append("=");
                cleanOptionsString.Append(value);
            }

            return cleanOptionsString.ToString();
        }

        public string MakeCreationOptionsString(
            GetFASTAFromDMS.SequenceTypes seqDirection)
        {
            var creationOptionsSb = new StringBuilder();

            creationOptionsSb.Append("seq_direction=");
            creationOptionsSb.Append(seqDirection.ToString());
            creationOptionsSb.Append(",");
            creationOptionsSb.Append("filetype=fasta");

            return creationOptionsSb.ToString();
        }
    }
}