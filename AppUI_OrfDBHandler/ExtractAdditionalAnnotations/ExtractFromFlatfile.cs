using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler.ExtractAdditionalAnnotations
{
    internal class ExtractFromFlatFile
    {
        private string mFilePath;

        /// <summary>
        /// Each entry in this list is a dictionary where keys are column name (1-based) and values are the value for that column
        /// </summary>
        private List<Dictionary<int, string>> mFileContents;

        /// <summary>
        /// Keys are column number (starting at 1)
        /// Values are column names
        /// </summary>
        private Dictionary<int, string> mColumnNameLookup;

        private readonly Dictionary<string, string> mAuthorities;
        private AnnotationStorage mAnnotationStorage;
        private string mfirstLine;
        private readonly string mPSConnectionString;
        private AddUpdateEntries mUploader;
        private Dictionary<string, int> mProteinIDLookup;

        private int mMaxProteinNameLength = 32;

        // AuthorityLookupHash key = AuthorityID, value = AuthorityName
        public ExtractFromFlatFile(Dictionary<string, string> AuthorityList, string psConnectionString)
        {
            mAuthorities = AuthorityList;
            mPSConnectionString = psConnectionString;
        }

        public List<Dictionary<int, string>> FileContents => mFileContents;

        public AnnotationStorage Annotations => mAnnotationStorage;

        /// <summary>
        /// Keys are column number (starting at 1)
        /// Values are column names
        /// </summary>
        public Dictionary<int, string> ColumnNames => mColumnNameLookup;

        private void ExtractGroupsFromLine(
            string entryLine,
            string delimiter,
            bool useContentsAsColumnNames)
        {
            mAnnotationStorage = new AnnotationStorage();

            // In dictionary valuesByColumnId:
            // Keys are column number (starting at 1)
            // Values are column names
            var valuesByColumnId = GetLineValuesByColumnId(entryLine, delimiter);

            if (mColumnNameLookup == null)
            {
                mColumnNameLookup = new Dictionary<int, string>(valuesByColumnId.Count);
            }
            else
            {
                mColumnNameLookup.Clear();
            }

            mAnnotationStorage.ClearAnnotationGroups();

            for (int columnNumber = 1; columnNumber <= valuesByColumnId.Count; columnNumber++)
            {
                string columnName;
                if (useContentsAsColumnNames)
                {
                    columnName = valuesByColumnId[columnNumber];
                }
                else
                {
                    columnName = "Column_" + columnNumber.ToString("00");
                }

                mColumnNameLookup.Add(columnNumber, columnName);
                mAnnotationStorage.AddAnnotationGroup(columnNumber, columnName);
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
            int columnCount = dataLine.Count;

            int maxColumnCount = ColumnNames.Count;

            var lvItem = new System.Windows.Forms.ListViewItem(dataLine[1]);
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

            var blankColumnCount = maxColumnCount - columnCount;
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
            ExtractGroupsFromLine(mfirstLine, delimiter, UseHeaderLineInfo);
            return default;
        }

        // Returns number of lines loaded
        public int LoadFile(
            string filePath,
            string delimiter,
            bool useHeaderLineInfo)
        {
            var inputFile = new FileInfo(filePath);

            mFileContents = new List<Dictionary<int, string>>();

            using (var reader = new StreamReader(new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                bool firstLineStored = false;
                while (!reader.EndOfStream)
                {
                    string entryLine = reader.ReadLine();
                    if (!firstLineStored)
                    {
                        mfirstLine = entryLine;
                        firstLineStored = true;
                    }

                    var valuesByColumnId = GetLineValuesByColumnId(entryLine, delimiter);
                    mFileContents.Add(valuesByColumnId);
                }
            }

            // Get Column names if possible
            ExtractGroupsFromLine(mfirstLine, delimiter, useHeaderLineInfo);

            return mFileContents.Count;
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
            foreach (var dataLine in mFileContents)
            {
                string primaryRef = dataLine[primaryReferenceNameColumnID];

                for (int columnNumber = 1; columnNumber <= dataLine.Count; columnNumber++)
                {
                    if (!columnNumber.Equals(primaryReferenceNameColumnID) &&
                        !dataLine[columnNumber].Equals("---"))
                    {
                        mAnnotationStorage.AddAnnotation(
                            columnNumber, primaryRef,
                            dataLine[columnNumber]);
                    }
                }
            }
        }

        public string LookupAuthorityName(int AuthorityID)
        {
            return mAuthorities[AuthorityID.ToString()];
        }

        public System.Windows.Forms.ListViewItem GetListViewItemForGroup(
            int GroupID)
        {
            var li = new System.Windows.Forms.ListViewItem(GroupID.ToString());
            li.SubItems.Add(mAnnotationStorage.GetGroupName(GroupID));
            if (mAnnotationStorage.GetAnnotationAuthorityID(GroupID) > 0)
            {
                li.SubItems.Add(mAuthorities[mAnnotationStorage.GetAnnotationAuthorityID(GroupID).ToString()].ToString());
            }
            else
            {
                li.SubItems.Add("-- None Selected --");
            }

            if (mAnnotationStorage.GetDelimiter(GroupID) != null)
            {
                li.SubItems.Add(mAnnotationStorage.GetDelimiter(GroupID).ToString());
            }
            else
            {
                li.SubItems.Add(" ");
            }

            return li;
        }

        public void ChangeAuthorityIDforGroup(int GroupID, int AuthorityID)
        {
            mAnnotationStorage.SetAnnotationAuthorityID(GroupID, AuthorityID);
        }

        public void UploadNewNames(int PrimaryReferenceNameColumnID)
        {
            ParseLoadedFile(PrimaryReferenceNameColumnID, mAuthorities);
            if (mUploader == null)
            {
                mUploader = new AddUpdateEntries(mPSConnectionString);
            }

            int groupCount = mAnnotationStorage.GroupCount;

            mProteinIDLookup = GetProteinIDsForPrimaryReferences(mAnnotationStorage.GetAllPrimaryReferences());

            for (int columnCount = 1; columnCount <= groupCount; columnCount++)
            {
                if (!columnCount.Equals(PrimaryReferenceNameColumnID))
                {
                    var ag = mAnnotationStorage.GetGroup(columnCount);
                    var referenceLookup = ag.GetAllXRefs();
                    foreach (var proteinName in referenceLookup.Keys)
                        mUploader.AddProteinReference(
                            proteinName,
                            string.Empty,
                            0,
                            ag.AnnotationAuthorityID,
                            mProteinIDLookup[proteinName],
                            mMaxProteinNameLength);
                }
            }
        }

        private Dictionary<string, int> GetProteinIDsForPrimaryReferences(IReadOnlyCollection<string> PrimaryReferences)
        {
            var ht = new Dictionary<string, int>(PrimaryReferences.Count);

            if (mUploader == null)
            {
                mUploader = new AddUpdateEntries(mPSConnectionString);
            }

            foreach (var name in PrimaryReferences)
            {
                if (!ht.ContainsKey(name))
                {
                    int id;
                    if (mProteinIDLookup.ContainsKey(name))
                    {
                        id = mProteinIDLookup[name];
                    }
                    else
                    {
                        id = mUploader.GetProteinIDFromName(name);
                    }

                    ht.Add(name, id);
                }
            }

            return ht;
        }
    }
}