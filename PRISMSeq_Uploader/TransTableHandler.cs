using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OrganismDatabaseHandler.DatabaseTools;
using PRISMDatabaseUtils;

namespace PRISMSeq_Uploader
{
    // * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
    //
    // Handles importation/searching/etc. of ASN.1 Formatted Genetic Code Tables from NCBI
    //
    // Example entry from ASN.1 file (between the - - - )
    //
    // - - - - - - - -
    //
    // Genetic-code-table ::= {
    // {
    // name "Standard" ,
    // name "SGC0" ,
    // id 1 ,
    // ncbieaa  "FFLLSSSSYY**CC*WLLLLPPPPHHQQRRRRIIIMTTTTNNKKSSRRVVVVAAAADDEEGGGG",
    // sncbieaa "---M---------------M---------------M----------------------------"
    // -- Base1  TTTTTTTTTTTTTTTTCCCCCCCCCCCCCCCCAAAAAAAAAAAAAAAAGGGGGGGGGGGGGGGG
    // -- Base2  TTTTCCCCAAAAGGGGTTTTCCCCAAAAGGGGTTTTCCCCAAAAGGGGTTTTCCCCAAAAGGGG
    // -- Base3  TCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAGTCAG
    // }
    // }
    //
    // - - - - - - - --

    public class TransTableHandler
    {
        private DataTable mTranslationEntries;
        private DataTable mTranslationTables;
        private readonly string mConnectionString;
        private const string EntriesTableName = "T_DNA_Translation_Table_Members";
        private const string IdTableName = "T_DNA_Translation_Tables";

        public TransTableHandler(string pisConnectionString)
        {
            mConnectionString = pisConnectionString;
        }

        public DataTable GetAllTranslationTableEntries(string filePath)
        {
            ScanFileForEntries(filePath);

            // Not implemented
            return new DataTable();
        }

        /// <summary>
        /// Look through the given ASN.1 file and scan for translation table entries
        /// </summary>
        /// <param name="filePath"></param>
        private void ScanFileForEntries(string filePath)
        {
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mConnectionString, "PRISMSeq_Uploader");

            var dba = new DBTask(connectionStringToUse);

            const string sqlQuery1 = "SELECT * FROM " + EntriesTableName;
            mTranslationEntries = dba.GetTable(sqlQuery1);

            const string sqlQuery2 = "SELECT * FROM " + IdTableName;
            mTranslationTables = dba.GetTable(sqlQuery2);

            var inputFile = new System.IO.FileInfo(filePath);

            if (inputFile.Exists)
            {
                using var reader = new StreamReader(new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

                // Get table format
                while (!reader.EndOfStream)
                {
                    var dataLine = reader.ReadLine();

                    if (string.IsNullOrWhiteSpace(dataLine))
                        continue;

                    var checkString = dataLine.Substring(0, 2);

                    if (checkString != "--")      // not a comment line. Process further
                    {
                        if (checkString == " {")   // Beginning of an entry block
                        {
                            var rawEntry = new List<string>();
                            var entryLine = reader.ReadLine();

                            while (entryLine?.Substring(0, 2) != " }")
                            {
                                rawEntry.Add(entryLine);
                                entryLine = reader.ReadLine();
                            }

                            ProcessTranslationEntry(rawEntry);

                            // These two statements used a DataAdapter object to synchronize data in mTranslation_Entries and mTranslation_Tables with tables in the database
                            // With the update of DBTask to use DbToolsFactory in February 2020, the DataAdapter functionality is no longer enabled

                            Console.WriteLine("Skipping: entryDA.Update(mTranslation_Entries)");
                            Console.WriteLine("Skipping: idDA.Update(mTranslation_Tables)");
                        }
                    }
                }
            }
        }

        private void ProcessTranslationEntry(IEnumerable<string> rawEntryCollection)
        {
            var id = 0;

            var aaList = string.Empty;
            var startList = string.Empty;
            var base1List = string.Empty;
            var base2List = string.Empty;
            var base3List = string.Empty;
            var nameList = new List<string>();

            var startPos = 0;

            const string trimString = " ,\"";
            var trimChars = trimString.ToCharArray();

            foreach (var str in rawEntryCollection)
            {
                var s = str.Trim();
                string tmp;
                switch (s.Substring(0, 3))
                {
                    case "nam":
                        tmp = s.TrimStart();
                        startPos = tmp.IndexOf(" ", StringComparison.Ordinal) + 1;
                        tmp = tmp.Substring(startPos);
                        tmp = tmp.Trim(trimChars);

                        foreach (var name in tmp.Split(";".ToCharArray()))
                        {
                            nameList.Add(name);
                        }

                        break;

                    case "id ":
                        tmp = s.TrimStart();
                        tmp = tmp.Substring(tmp.IndexOf(" ", StringComparison.Ordinal) + 1);
                        tmp = tmp.TrimEnd(trimChars);
                        id = Convert.ToInt32(tmp);
                        break;

                    case "ncb":
                        tmp = s.TrimStart();
                        startPos = tmp.IndexOf("\"", StringComparison.Ordinal) + 1;
                        tmp = tmp.Substring(startPos);
                        tmp = tmp.TrimEnd(trimChars);
                        aaList = tmp;
                        break;

                    case "snc":
                        tmp = s.TrimStart();
                        tmp = tmp.Substring(startPos); // TODO: This either does nothing, or undesirable things.
                        tmp = tmp.TrimEnd(trimChars);
                        startList = tmp;
                        break;

                    case "-- ":
                        switch (s.Substring(0, 8))
                        {
                            case "-- Base1":
                                base1List = ProcessBaseString(s);
                                break;

                            case "-- Base2":
                                base2List = ProcessBaseString(s);
                                break;

                            case "-- Base3":
                                base3List = ProcessBaseString(s);
                                break;

                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }

            SplitCodonEntries(aaList, startList, base1List, base2List, base3List, nameList, id);
        }

        private string ProcessBaseString(string rawBaseString)
        {
            var trimmedString = rawBaseString.TrimStart();

            return trimmedString.Substring(10);
        }

        private bool SplitCodonEntries(
            string aaString,
            string startString,
            string base1List,
            string base2List,
            string base3List,
            List<string> nameList,
            int id)
        {
            // Check for length consistency
            var baseLength = aaString.Length;

            if (baseLength != startString.Length ||
                baseLength != base1List.Length ||
                baseLength != base2List.Length ||
                baseLength != base3List.Length)
            {
                return false;
            }

            var counter = 0;
            DataRow dataRow;

            foreach (var name in nameList)
            {
                dataRow = mTranslationTables.NewRow();
                dataRow["Translation_Table_Name"] = name.Trim() + " (ID = " + id + ")";
                dataRow["DNA_Translation_Table_ID"] = id;
                mTranslationTables.Rows.Add(dataRow);
            }

            foreach (var aminoAcid in aaString.ToCharArray())
            {
                dataRow = mTranslationEntries.NewRow();
                var startSequence = startString.Substring(counter, 1);

                var base1 = base1List.Substring(counter, 1);
                var base2 = base2List.Substring(counter, 1);
                var base3 = base3List.Substring(counter, 1);

                dataRow["Coded_AA"] = aminoAcid;
                dataRow["Start_Sequence"] = startSequence;
                dataRow["Base_1"] = base1;
                dataRow["Base_2"] = base2;
                dataRow["Base_3"] = base3;
                dataRow["DNA_Translation_Table_ID"] = id;

                mTranslationEntries.Rows.Add(dataRow);
                counter++;
            }

            return default;
        }
    }
}