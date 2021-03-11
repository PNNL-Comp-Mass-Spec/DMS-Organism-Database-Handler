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
        private readonly string mPsConnectionString;
        private AddUpdateEntries mUploader;
        private Dictionary<string, int> mProteinIdLookup;

        private int mMaxProteinNameLength = 32;

        // AuthorityLookupHash key = AuthorityID, value = AuthorityName
        public ExtractFromFlatFile(Dictionary<string, string> authorityList, string psConnectionString)
        {
            mAuthorities = authorityList;
            mPsConnectionString = psConnectionString;
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

            for (var columnNumber = 1; columnNumber <= valuesByColumnId.Count; columnNumber++)
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

            for (var columnId = 1; columnId <= lineEntries.Length; columnId++)
            {
                var lineEntry = lineEntries[columnId - 1];

                if (lineEntry.Trim(' ').Length > 0)
                {
                    valuesByColumnId.Add(columnId, lineEntry);
                }
                else
                {
                    valuesByColumnId.Add(columnId, "---");
                }
            }

            return valuesByColumnId;
        }

        public System.Windows.Forms.ListViewItem DataLineToListViewItem(
            Dictionary<int, string> dataLine,
            int lineCount)
        {
            var columnCount = dataLine.Count;

            var maxColumnCount = ColumnNames.Count;

            var lvItem = new System.Windows.Forms.ListViewItem(dataLine[1]);
            for (var columnNumber = 2; columnNumber <= columnCount; columnNumber++)
            {
                var dataValue = dataLine[columnNumber];
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
                for (var columnNumber = 1; columnNumber <= blankColumnCount; columnNumber++)
                    lvItem.SubItems.Add("---");
            }

            return lvItem;
        }

        public int LoadGroups(
            string delimiter,
            bool useHeaderLineInfo)
        {
            ExtractGroupsFromLine(mfirstLine, delimiter, useHeaderLineInfo);
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
                var firstLineStored = false;
                while (!reader.EndOfStream)
                {
                    var entryLine = reader.ReadLine();
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
        /// <param name="primaryReferenceNameColumnId">The number of the column with the name to use as primary</param>
        /// <param name="authorityHash">Dictionary with columnID (number), and authority name for that column</param>
        public void ParseLoadedFile(
            int primaryReferenceNameColumnId,
            Dictionary<string, string> authorityHash)
        {
            foreach (var dataLine in mFileContents)
            {
                var primaryRef = dataLine[primaryReferenceNameColumnId];

                for (var columnNumber = 1; columnNumber <= dataLine.Count; columnNumber++)
                {
                    if (!columnNumber.Equals(primaryReferenceNameColumnId) &&
                        !dataLine[columnNumber].Equals("---"))
                    {
                        mAnnotationStorage.AddAnnotation(
                            columnNumber, primaryRef,
                            dataLine[columnNumber]);
                    }
                }
            }
        }

        public string LookupAuthorityName(int authorityId)
        {
            return mAuthorities[authorityId.ToString()];
        }

        public System.Windows.Forms.ListViewItem GetListViewItemForGroup(
            int groupId)
        {
            var li = new System.Windows.Forms.ListViewItem(groupId.ToString());
            li.SubItems.Add(mAnnotationStorage.GetGroupName(groupId));
            if (mAnnotationStorage.GetAnnotationAuthorityId(groupId) > 0)
            {
                li.SubItems.Add(mAuthorities[mAnnotationStorage.GetAnnotationAuthorityId(groupId).ToString()].ToString());
            }
            else
            {
                li.SubItems.Add("-- None Selected --");
            }

            if (mAnnotationStorage.GetDelimiter(groupId) != null)
            {
                li.SubItems.Add(mAnnotationStorage.GetDelimiter(groupId).ToString());
            }
            else
            {
                li.SubItems.Add(" ");
            }

            return li;
        }

        public void ChangeAuthorityIDforGroup(int groupId, int authorityId)
        {
            mAnnotationStorage.SetAnnotationAuthorityId(groupId, authorityId);
        }

        public void UploadNewNames(int primaryReferenceNameColumnId)
        {
            ParseLoadedFile(primaryReferenceNameColumnId, mAuthorities);
            if (mUploader == null)
            {
                mUploader = new AddUpdateEntries(mPsConnectionString);
            }

            var groupCount = mAnnotationStorage.GroupCount;

            mProteinIdLookup = GetProteinIDsForPrimaryReferences(mAnnotationStorage.GetAllPrimaryReferences());

            for (var columnCount = 1; columnCount <= groupCount; columnCount++)
            {
                if (!columnCount.Equals(primaryReferenceNameColumnId))
                {
                    var ag = mAnnotationStorage.GetGroup(columnCount);
                    var referenceLookup = ag.GetAllXRefs();
                    foreach (var proteinName in referenceLookup.Keys)
                        mUploader.AddProteinReference(
                            proteinName,
                            string.Empty,
                            0,
                            ag.AnnotationAuthorityId,
                            mProteinIdLookup[proteinName],
                            mMaxProteinNameLength);
                }
            }
        }

        private Dictionary<string, int> GetProteinIDsForPrimaryReferences(IReadOnlyCollection<string> primaryReferences)
        {
            var ht = new Dictionary<string, int>(primaryReferences.Count);

            if (mUploader == null)
            {
                mUploader = new AddUpdateEntries(mPsConnectionString);
            }

            foreach (var name in primaryReferences)
            {
                if (!ht.ContainsKey(name))
                {
                    int id;
                    if (mProteinIdLookup.ContainsKey(name))
                    {
                        id = mProteinIdLookup[name];
                    }
                    else
                    {
                        id = mUploader.GetProteinIdFromName(name);
                    }

                    ht.Add(name, id);
                }
            }

            return ht;
        }
    }
}