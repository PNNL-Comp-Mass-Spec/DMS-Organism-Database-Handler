﻿using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinImport;
using PRISM;
using PRISMDatabaseUtils;

namespace AppUI_OrfDBHandler
{
    public class CollectionStatePickerHandler : EventNotifier
    {
        // Ignore Spelling: yyyy-MM-dd

        private bool mForceReload;
        private DataTable mListViewData;
        private readonly DBTask mGetTables;
        private readonly AddUpdateEntries mSpAccess;

        public CollectionStatePickerHandler(string psConnectionString)
        {
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(psConnectionString, "PRISMSeq_Uploader");

            mGetTables = new DBTask(connectionStringToUse);
            RegisterEvents(mGetTables);

            mSpAccess = new AddUpdateEntries(connectionStringToUse);
            RegisterEvents(mSpAccess);

            mForceReload = true;
        }

        public bool ForceIdTableReload
        {
            set => mForceReload = value;
        }

        public void ChangeSelectedCollectionStates(int newStateId, ArrayList selectedCollectionIdList)
        {
            foreach (int id in selectedCollectionIdList)
            {
                mSpAccess.UpdateProteinCollectionState(id, newStateId);
            }
        }

        private void SetupPickerListView(ListView lvw, DataTable dt, string filterCriteria)
        {
            filterCriteria = filterCriteria.Trim(' ');

            var criteriaCollection = filterCriteria.Split(' ');

            var filterString = string.Empty;

            if (criteriaCollection.Length > 0 && filterCriteria.Length > 0)
            {
                foreach (var filterElement in criteriaCollection)
                {
                    filterString += "[Name] LIKE '%" + filterElement + "%' OR [State] LIKE '%" + filterElement + "%' OR ";
                }

                // Trim off final " OR "
                filterString = filterString.Substring(0, filterString.Length - 4);
            }
            else
            {
                filterString = string.Empty;
            }

            var collectionRows = dt.Select(filterString);

            lvw.BeginUpdate();
            foreach (var cRow in collectionRows)
            {
                var created = Convert.ToDateTime(cRow["Created"]);
                var modified = Convert.ToDateTime(cRow["Modified"]);
                var item = new ListViewItem {Text = cRow["Name"].ToString(), Tag = cRow["ID"]};
                item.SubItems.Add(created.ToString("yyyy-MM-dd"));
                item.SubItems.Add(modified.ToString("yyyy-MM-dd"));
                item.SubItems.Add(cRow["State"].ToString());
                lvw.Items.Add(item);
            }

            lvw.EndUpdate();
        }

        [Obsolete("View retired in 2022")]
        public void FillListView(ListView listViewToFill)
        {
            FillFilteredListView(listViewToFill, "");
        }

        [Obsolete("View retired in 2022")]
        public void FillFilteredListView(ListView listViewToFill, string filterString)
        {
            listViewToFill.Items.Clear();
            if (mForceReload)
            {
                mListViewData = GetCollectionTable();
                mForceReload = false;
            }

            SetupPickerListView(listViewToFill, mListViewData, filterString);
        }

        [Obsolete("View retired in 2022")]
        public DataTable GetCollectionTable()
        {
            const string SQL = "SELECT * FROM V_Collection_State_Picker ORDER BY [Name]";
            var cTable = mGetTables.GetTable(SQL);
            return cTable;
        }

        public DataTable GetStates()
        {
            const string SQL = "SELECT State, Collection_State_ID as ID " +
                      "FROM T_Protein_Collection_States ORDER BY Collection_State_ID";
            var sTable = mGetTables.GetTable(SQL);

            return sTable;
        }
    }
}