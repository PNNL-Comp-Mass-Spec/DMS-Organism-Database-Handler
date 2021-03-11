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
        private readonly DBTask m_DatabaseAccessor;
        private GetFASTAFromDMS.SequenceTypes m_SeqDirection;
        private GetFASTAFromDMS.DatabaseFormatTypes m_FileType;
        private DataTable m_CreationValuesTable;
        private DataTable m_KeywordTable;

        public FileCreationOptions(DBTask databaseAccessor)
        {
            m_DatabaseAccessor = databaseAccessor;
        }

        public GetFASTAFromDMS.SequenceTypes SequenceDirection => m_SeqDirection;

        public GetFASTAFromDMS.DatabaseFormatTypes FileFormatType => m_FileType;

        // Options string looks like... "seq_direction=forward;filetype=fasta"
        public string ExtractOptions(string optionsString)
        {
            var optionsHash = new Dictionary<string, string>();

            DataRow[] foundRows;

            string tmpKeyword;
            string tmpValue;

            var validKeyword = default(bool);
            var validValue = default(bool);

            var cleanOptionsString = new StringBuilder();

            var keywordTableSQL = "SELECT Keyword_ID, Keyword, Default_Value FROM T_Creation_Option_Keywords";
            //var valuesTableSQL = "SELECT Value_ID, Value_String, Keyword_ID FROM T_Creation_Option_Values";
            var creationValuesSQL = "SELECT Keyword, Value_String, String_Element FROM V_Creation_String_Lookup";

            if (m_KeywordTable == null)
            {
                m_KeywordTable = m_DatabaseAccessor.GetTable(keywordTableSQL);
            }

            // If m_ValuesTable Is Nothing Then
            // m_ValuesTable = m_DatabaseAccessor.GetTable(valuesTableSQL)
            // End If

            if (m_CreationValuesTable == null)
            {
                m_CreationValuesTable = m_DatabaseAccessor.GetTable(creationValuesSQL);
            }

            //var optionsStringParser = new System.Text.RegularExpressions.Regex(
            //    "(?<keyword>\S+)\s*=\s*(?<value>\S+),*?")
            var optionsStringParser = new Regex(
                @"(?<keyword>[^,\s]*)\s*=\s*(?<value>[^,\s]+)");

            var mCollection = optionsStringParser.Matches(optionsString);

            foreach (Match m in mCollection)
            {
                // optionsHash.Add(m.Groups("keyword").Value, m.Groups("value").Value)
                tmpKeyword = m.Groups["keyword"].Value;
                tmpValue = m.Groups["value"].Value;

                // Check for valid keyword/value pair
                foundRows = m_CreationValuesTable.Select("Keyword = '" + tmpKeyword + "' AND Value_String = '" + tmpValue + "'");
                if (foundRows.Length < 1)
                {
                    // check if keyword or value is bad
                    var errorString = new StringBuilder();
                    var checkRows = m_CreationValuesTable.Select("Keyword = '" + tmpKeyword);
                    if (checkRows.Length > 0)
                        validKeyword = true;
                    checkRows = m_CreationValuesTable.Select("Value_String = '" + tmpValue + "'");
                    if (checkRows.Length > 0)
                        validValue = true;
                    if (!validKeyword)
                    {
                        errorString.Append("Keyword: " + tmpKeyword + " is not valid");
                    }

                    if (!validValue)
                    {
                        if (errorString.ToString().Length > 0)
                            errorString.Append(", ");
                        errorString.Append("Value: " + tmpValue + "is not a valid option");
                    }

                    throw new Exception(errorString.ToString());
                }

                if (optionsHash.ContainsKey(tmpKeyword))
                {
                    throw new Exception(tmpKeyword + " is a duplicate keyword");
                }
                else
                {
                    optionsHash.Add(tmpKeyword, tmpValue);
                }
            }

            // Parse dictionary into canonical options string for return
            foundRows = m_KeywordTable.Select("", "Keyword_ID ASC");
            foreach (var dr in foundRows)
            {
                if (cleanOptionsString.ToString().Length > 0)
                {
                    cleanOptionsString.Append(",");
                }

                tmpKeyword = dr["Keyword"].ToString();
                if (optionsHash.ContainsKey(tmpKeyword))
                {
                    tmpValue = optionsHash[tmpKeyword].ToString();
                }
                else
                {
                    tmpValue = dr["Default_Value"].ToString();
                }

                switch (tmpKeyword ?? "")
                {
                    case "seq_direction":
                        m_SeqDirection = (GetFASTAFromDMS.SequenceTypes)Enum.Parse(typeof(GetFASTAFromDMS.SequenceTypes), tmpValue);
                        break;

                    case "filetype":
                        m_FileType = (GetFASTAFromDMS.DatabaseFormatTypes)Enum.Parse(typeof(GetFASTAFromDMS.DatabaseFormatTypes), tmpValue);
                        break;
                }

                cleanOptionsString.Append(tmpKeyword);
                cleanOptionsString.Append("=");
                cleanOptionsString.Append(tmpValue);
            }

            return cleanOptionsString.ToString();
        }

        public string MakeCreationOptionsString(
            GetFASTAFromDMS.SequenceTypes seqDirection,
            GetFASTAFromDMS.DatabaseFormatTypes databaseFormatType)
        {
            var creationOptionsSB = new StringBuilder();

            creationOptionsSB.Append("seq_direction=");
            creationOptionsSB.Append(seqDirection.ToString());
            creationOptionsSB.Append(",");
            creationOptionsSB.Append("filetype=");
            creationOptionsSB.Append(databaseFormatType.ToString());

            return creationOptionsSB.ToString();
        }
    }
}