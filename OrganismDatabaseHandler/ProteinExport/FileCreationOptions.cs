﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using OrganismDatabaseHandler.DatabaseTools;

namespace OrganismDatabaseHandler.ProteinExport
{
    internal class FileCreationOptions
    {
        private readonly DBTask mDatabaseAccessor;
        private GetFASTAFromDMS.SequenceTypes mSeqDirection;
        private GetFASTAFromDMS.DatabaseFormatTypes mFileType;
        private DataTable mCreationValuesTable;
        private DataTable mKeywordTable;

        public FileCreationOptions(DBTask databaseAccessor)
        {
            mDatabaseAccessor = databaseAccessor;
        }

        public GetFASTAFromDMS.SequenceTypes SequenceDirection => mSeqDirection;

        public GetFASTAFromDMS.DatabaseFormatTypes FileFormatType => mFileType;

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

            if (mKeywordTable == null)
            {
                mKeywordTable = mDatabaseAccessor.GetTable(keywordTableSQL);
            }

            // If mValuesTable Is Nothing Then
            // mValuesTable = mDatabaseAccessor.GetTable(valuesTableSQL)
            // End If

            if (mCreationValuesTable == null)
            {
                mCreationValuesTable = mDatabaseAccessor.GetTable(creationValuesSQL);
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
                foundRows = mCreationValuesTable.Select("Keyword = '" + tmpKeyword + "' AND Value_String = '" + tmpValue + "'");
                if (foundRows.Length < 1)
                {
                    // check if keyword or value is bad
                    var errorString = new StringBuilder();
                    var checkRows = mCreationValuesTable.Select("Keyword = '" + tmpKeyword);
                    if (checkRows.Length > 0)
                        validKeyword = true;
                    checkRows = mCreationValuesTable.Select("Value_String = '" + tmpValue + "'");
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
            foundRows = mKeywordTable.Select("", "Keyword_ID ASC");
            foreach (var dr in foundRows)
            {
                if (cleanOptionsString.ToString().Length > 0)
                {
                    cleanOptionsString.Append(",");
                }

                tmpKeyword = dr["Keyword"].ToString();
                if (optionsHash.ContainsKey(tmpKeyword))
                {
                    tmpValue = optionsHash[tmpKeyword];
                }
                else
                {
                    tmpValue = dr["Default_Value"].ToString();
                }

                switch (tmpKeyword)
                {
                    case "seq_direction":
                        mSeqDirection = (GetFASTAFromDMS.SequenceTypes)Enum.Parse(typeof(GetFASTAFromDMS.SequenceTypes), tmpValue);
                        break;

                    case "filetype":
                        mFileType = (GetFASTAFromDMS.DatabaseFormatTypes)Enum.Parse(typeof(GetFASTAFromDMS.DatabaseFormatTypes), tmpValue);
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
            var creationOptionsSb = new StringBuilder();

            creationOptionsSb.Append("seq_direction=");
            creationOptionsSb.Append(seqDirection.ToString());
            creationOptionsSb.Append(",");
            creationOptionsSb.Append("filetype=");
            creationOptionsSb.Append(databaseFormatType.ToString());

            return creationOptionsSb.ToString();
        }
    }
}