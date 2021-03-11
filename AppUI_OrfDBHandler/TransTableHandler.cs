using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using OrganismDatabaseHandler.DatabaseTools;

namespace AppUI_OrfDBHandler
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
        private DataTable m_Translation_Entries;
        private DataTable m_Translation_Tables;
        private string m_ConnectionString;
        private const string EntriesTableName = "T_DNA_Translation_Table_Members";
        private const string IDTableName = "T_DNA_Translation_Tables";

        public TransTableHandler(string PIS_ConnectionString)
        {
            m_ConnectionString = PIS_ConnectionString;
        }

        public DataTable GetAllTranslationTableEntries(string FilePath)
        {
            ScanFileForEntries(FilePath);

            // Not implemented
            return new DataTable();
        }

        private void ScanFileForEntries(string filePath)
        {
            // Look through a given ASN.1 file and scan for translation table entries
            var dba = new DBTask(m_ConnectionString);

            string sqlQuery1 = "SELECT * FROM " + EntriesTableName;
            m_Translation_Entries = dba.GetTable(sqlQuery1);

            string sqlQuery2 = "SELECT * FROM " + IDTableName;
            m_Translation_Tables = dba.GetTable(sqlQuery2);

            var fi = new System.IO.FileInfo(filePath);

            if (fi.Exists)
            {
                System.IO.TextReader tr = fi.OpenText();
                var tmpLineCache = tr.ReadLine();

                // Get table format

                while (tmpLineCache != null)
                {
                    var checkString = tmpLineCache.Substring(0, 2);
                    if (checkString != "--")      // not a comment line. Process further
                    {
                        if (checkString == " {")   // Beginning of an entry block
                        {
                            var rawEntry = new List<string>();
                            var entryLine = tr.ReadLine();
                            while (entryLine?.Substring(0, 2) != " }")
                            {
                                rawEntry.Add(entryLine);
                                entryLine = tr.ReadLine();
                            }

                            ProcessTranslationEntry(rawEntry);

                            // These two statements used a DataAdapter object to synchronize data in m_Translation_Entries and m_Translation_Tables with tables in the database
                            // With the update of DBTask to use DbToolsFactory in February 2020, the DataAdapter functionality is no longer enabled

                            Console.WriteLine("Skipping: entryDA.Update(m_Translation_Entries)");
                            Console.WriteLine("Skipping: idDA.Update(m_Translation_Tables)");
                        }

                        tmpLineCache = tr.ReadLine();
                    }
                    else
                    {
                        tmpLineCache = tr.ReadLine();           // comment line. Ignore
                    }
                }
            }
        }

        [Obsolete("Unused")]
        private void SyncLocalToDMS()
        {
            var dba = new DBTask(m_ConnectionString);

            string sqlQuery = "SELECT * FROM " + EntriesTableName;

            var entriesTable = dba.GetTable(sqlQuery);
        }

        private void ProcessTranslationEntry(IEnumerable<string> rawEntryCollection)
        {
            var id = default(int);

            string AAList = string.Empty;
            string StartList = string.Empty;
            string Base1List = string.Empty;
            string Base2List = string.Empty;
            string Base3List = string.Empty;
            var nameList = new List<string>();

            var tmpStartPos = default(int);

            string trimString = " ,\"";
            var trimChars = trimString.ToCharArray();

            foreach (var str in rawEntryCollection)
            {
                var s = str.Trim();
                string tmp;
                switch (s.Substring(0, 3) ?? "")
                {
                    case "nam":
                        tmp = s.TrimStart();
                        tmpStartPos = tmp.IndexOf(" ") + 1;
                        tmp = tmp.Substring(tmpStartPos);
                        tmp = tmp.Trim(trimChars);
                        var tmpNameList = tmp.Split(";".ToCharArray());
                        foreach (var tmpName in tmpNameList)
                            nameList.Add(tmpName);
                        break;

                    case "id ":
                        tmp = s.TrimStart();
                        tmp = tmp.Substring(tmp.IndexOf(" ") + 1);
                        tmp = tmp.TrimEnd(trimChars);
                        id = Convert.ToInt32(tmp);
                        break;

                    case "ncb":
                        tmp = s.TrimStart();
                        tmpStartPos = tmp.IndexOf("\"") + 1;
                        tmp = tmp.Substring(tmpStartPos);
                        tmp = tmp.TrimEnd(trimChars);
                        AAList = tmp;
                        break;

                    case "snc":
                        tmp = s.TrimStart();
                        tmp = tmp.Substring(tmpStartPos);
                        tmp = tmp.TrimEnd(trimChars);
                        StartList = tmp;
                        break;

                    case "-- ":
                        switch (s.Substring(0, 8) ?? "")
                        {
                            case "-- Base1":
                                Base1List = ProcessBaseString(s);
                                break;

                            case "-- Base2":
                                Base2List = ProcessBaseString(s);
                                break;

                            case "-- Base3":
                                Base3List = ProcessBaseString(s);
                                break;

                            default:
                                break;
                        }

                        break;

                    default:
                        break;
                }
            }

            bool success = SplitCodonEntries(AAList, StartList, Base1List, Base2List, Base3List, nameList, id);
        }

        private string ProcessBaseString(string rawBaseString)
        {
            var tmpString = rawBaseString.TrimStart();

            tmpString = tmpString.Substring(11);

            return tmpString;
        }

        private bool SplitCodonEntries(
            string AAString,
            string StartString,
            string Base1List,
            string Base2List,
            string Base3List,
            List<string> NameList,
            int ID)
        {
            // Check for length consistency
            int baseLength = AAString.Length;

            if (baseLength != StartString.Length ||
                baseLength != Base1List.Length ||
                baseLength != Base2List.Length ||
                baseLength != Base3List.Length)
            {
                return false;
            }

            bool tmpStart;

            var counter = default(int);
            DataRow dr;

            foreach (var tmpName in NameList)
            {
                dr = m_Translation_Tables.NewRow();
                dr["Translation_Table_Name"] = tmpName.Trim() + " (ID = " + ID.ToString() + ")";
                dr["DNA_Translation_Table_ID"] = ID;
                m_Translation_Tables.Rows.Add(dr);
            }

            var arrAA = AAString.ToCharArray();

            foreach (var tmpAA in arrAA)
            {
                dr = m_Translation_Entries.NewRow();
                counter += 1;
                var tmpStartString = StartString.Substring(counter, 1);
                if (tmpStartString == "M")
                {
                    tmpStart = true;
                }
                else
                {
                    tmpStart = false;
                }

                var Base1 = Base1List.Substring(counter, 1);
                var Base2 = Base2List.Substring(counter, 1);
                var Base3 = Base3List.Substring(counter, 1);

                dr["Coded_AA"] = tmpAA;
                dr["Start_Sequence"] = tmpStartString;
                dr["Base_1"] = Base1;
                dr["Base_2"] = Base2;
                dr["Base_3"] = Base3;
                dr["DNA_Translation_Table_ID"] = ID;

                m_Translation_Entries.Rows.Add(dr);
            }

            return default;
        }
    }
}