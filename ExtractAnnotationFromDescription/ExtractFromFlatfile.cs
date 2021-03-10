using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic;

namespace ExtractAnnotationFromDescription
{
    internal class ExtractFromFlatFile
    {
        private string m_FilePath;

        /// <summary>
        /// Each entry in this list is a dictionary where keys are column name (1-based) and values are the value for that column
        /// </summary>
        private List<Dictionary<int, string>> m_FileContents;

        /// <summary>
        /// Keys are column number (starting at 1)
        /// Values are column names
        /// </summary>
        private Dictionary<int, string> m_ColumnNameLookup;

        private readonly Dictionary<string, string> m_Authorities;
        private AnnotationStorage m_AnnotationStorage;
        private string m_firstLine;
        private readonly string m_PSConnectionString;
        private Protein_Importer.AddUpdateEntries m_Uploader;
        private Dictionary<string, int> m_ProteinIDLookup;

        private int m_MaxProteinNameLength = 32;

        // AuthorityLookupHash key = AuthorityID, value = AuthorityName
        public ExtractFromFlatFile(Dictionary<string, string> AuthorityList, string psConnectionString)
        {
            m_Authorities = AuthorityList;
            m_PSConnectionString = psConnectionString;
        }

        public List<Dictionary<int, string>> FileContents
        {
            get
            {
                return m_FileContents;
            }
        }

        public AnnotationStorage Annotations
        {
            get
            {
                return m_AnnotationStorage;
            }
        }

        /// <summary>
        /// Keys are column number (starting at 1)
        /// Values are column names
        /// </summary>
        public Dictionary<int, string> ColumnNames
        {
            get
            {
                return m_ColumnNameLookup;
            }
        }

        private void ExtractGroupsFromLine(
            string entryLine,
            string delimiter,
            bool useContentsAsColumnNames)
        {
            m_AnnotationStorage = new AnnotationStorage();

            // In dictionary valuesByColumnId:
            // Keys are column number (starting at 1)
            // Values are column names
            var valuesByColumnId = GetLineValuesByColumnId(entryLine, delimiter);

            if (m_ColumnNameLookup == null)
            {
                m_ColumnNameLookup = new Dictionary<int, string>(valuesByColumnId.Count);
            }
            else
            {
                m_ColumnNameLookup.Clear();
            }

            m_AnnotationStorage.ClearAnnotationGroups();

            for (int columnNumber = 1; columnNumber <= valuesByColumnId.Count; columnNumber++)
            {
                string columnName;
                if (useContentsAsColumnNames)
                {
                    columnName = valuesByColumnId[columnNumber];
                }
                else
                {
                    columnName = "Column_" + Strings.Format(columnNumber, "00");
                }

                m_ColumnNameLookup.Add(columnNumber, columnName);
                m_AnnotationStorage.AddAnnotationGroup(columnNumber, columnName);
            }
        }

        private Dictionary<int, string> GetLineValuesByColumnId(
            string entryLine,
            string delimiter)
        {
            var lineEntries = entryLine.Split(delimiter.ToCharArray());
            var valuesByColumnId = new Dictionary<int, string>(lineEntries.Length);

            for (int columnID = 1; columnID <= lineEntries.Length; columnID++)
            {
                string lineEntry = lineEntries[columnID - 1];

                if (lineEntry.Trim(' ').Length > 0)
                {
                    valuesByColumnId.Add(columnID, lineEntry);
                }
                else
                {
                    valuesByColumnId.Add(columnID, "---");
                }
            }

            return valuesByColumnId;
        }

        public System.Windows.Forms.ListViewItem DataLineToListViewItem(
            Dictionary<int, string> dataLine,
            int lineCount)
        {
            System.Windows.Forms.ListViewItem lvItem;
            int columnCount = dataLine.Count;

            int maxColumnCount = ColumnNames.Count;

            int blankColumnCount;

            lvItem = new System.Windows.Forms.ListViewItem(dataLine[1]);
            for (int columnNumber = 2; columnNumber <= columnCount; columnNumber++)
            {
                string dataValue = dataLine[columnNumber];
                if (dataValue.Length > 0)
                {
                    lvItem.SubItems.Add(dataValue);
                }
                else
                {
                    lvItem.SubItems.Add("---");
                }
            }

            blankColumnCount = maxColumnCount - columnCount;
            if (blankColumnCount > 0)
            {
                for (int columnNumber = 1; columnNumber <= blankColumnCount; columnNumber++)
                    lvItem.SubItems.Add("---");
            }

            return lvItem;
        }

        public int LoadGroups(
            string delimiter,
            bool UseHeaderLineInfo)
        {
            ExtractGroupsFromLine(m_firstLine, delimiter, UseHeaderLineInfo);
            return default;
        }

        // Returns number of lines loaded
        public int LoadFile(
            string filePath,
            string delimiter,
            bool useHeaderLineInfo)
        {
            var inputFile = new FileInfo(filePath);

            m_FileContents = new List<Dictionary<int, string>>();

            using (var reader = new StreamReader(new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                bool firstLineStored = false;
                while (!reader.EndOfStream)
                {
                    string entryLine = reader.ReadLine();
                    if (!firstLineStored)
                    {
                        m_firstLine = entryLine;
                        firstLineStored = true;
                    }

                    var valuesByColumnId = GetLineValuesByColumnId(entryLine, delimiter);
                    m_FileContents.Add(valuesByColumnId);
                }
            }

            // Get Column names if possible
            ExtractGroupsFromLine(m_firstLine, delimiter, useHeaderLineInfo);

            return m_FileContents.Count;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="primaryReferenceNameColumnID">The number of the column with the name to use as primary</param>
        /// <param name="authorityHash">Dictionary with columnID (number), and authority name for that column</param>
        public void ParseLoadedFile(
            int primaryReferenceNameColumnID,
            Dictionary<string, string> authorityHash)
        {
            foreach (var dataLine in m_FileContents)
            {
                string primaryRef = dataLine[primaryReferenceNameColumnID];

                for (int columnNumber = 1; columnNumber <= dataLine.Count; columnNumber++)
                {
                    if (!columnNumber.Equals(primaryReferenceNameColumnID) &&
                        !dataLine[columnNumber].Equals("---"))
                    {
                        m_AnnotationStorage.AddAnnotation(
                            columnNumber, primaryRef,
                            dataLine[columnNumber]);
                    }
                }
            }
        }

        public string LookupAuthorityName(int AuthorityID)
        {
            return m_Authorities[AuthorityID.ToString()];
        }

        public System.Windows.Forms.ListViewItem GetListViewItemForGroup(
            int GroupID)
        {
            var li = new System.Windows.Forms.ListViewItem(GroupID.ToString());
            li.SubItems.Add(m_AnnotationStorage.get_GroupName(GroupID));
            if (m_AnnotationStorage.get_AnnotationAuthorityID(GroupID) > 0)
            {
                li.SubItems.Add(m_Authorities[m_AnnotationStorage.get_AnnotationAuthorityID(GroupID).ToString()].ToString());
            }
            else
            {
                li.SubItems.Add("-- None Selected --");
            }

            if (m_AnnotationStorage.get_Delimiter(GroupID) != null)
            {
                li.SubItems.Add(m_AnnotationStorage.get_Delimiter(GroupID).ToString());
            }
            else
            {
                li.SubItems.Add(" ");
            }

            return li;
        }

        public void ChangeAuthorityIDforGroup(int GroupID, int AuthorityID)
        {
            m_AnnotationStorage.set_AnnotationAuthorityID(GroupID, AuthorityID);
        }

        public void UploadNewNames(int PrimaryReferenceNameColumnID)
        {
            ParseLoadedFile(PrimaryReferenceNameColumnID, m_Authorities);
            if (m_Uploader == null)
            {
                m_Uploader = new Protein_Importer.AddUpdateEntries(m_PSConnectionString);
            }

            int groupCount = m_AnnotationStorage.GroupCount;

            m_ProteinIDLookup = GetProteinIDsForPrimaryReferences(m_AnnotationStorage.GetAllPrimaryReferences());

            for (int columnCount = 1; columnCount <= groupCount; columnCount++)
            {
                if (!columnCount.Equals(PrimaryReferenceNameColumnID))
                {
                    var ag = m_AnnotationStorage.GetGroup(columnCount);
                    var referenceLookup = ag.GetAllXRefs();
                    foreach (var proteinName in referenceLookup.Keys)
                        m_Uploader.AddProteinReference(
                            proteinName,
                            string.Empty,
                            0,
                            ag.AnnotationAuthorityID,
                            m_ProteinIDLookup[proteinName],
                            m_MaxProteinNameLength);
                }
            }
        }

        private Dictionary<string, int> GetProteinIDsForPrimaryReferences(IReadOnlyCollection<string> PrimaryReferences)
        {
            var ht = new Dictionary<string, int>(PrimaryReferences.Count);
            int id;

            if (m_Uploader == null)
            {
                m_Uploader = new Protein_Importer.AddUpdateEntries(m_PSConnectionString);
            }

            foreach (var name in PrimaryReferences)
            {
                if (!ht.ContainsKey(name))
                {
                    if (m_ProteinIDLookup.ContainsKey(name))
                    {
                        id = m_ProteinIDLookup[name];
                    }
                    else
                    {
                        id = m_Uploader.GetProteinIDFromName(name);
                    }

                    ht.Add(name, id);
                }
            }

            return ht;
        }
    }
}