using System;
using System.Collections;
using System.Data;
using System.Windows.Forms;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class CollectionStatePickerHandler
    {
        private bool mForceReload = false;
        private DataTable mListViewData;
        private readonly DBTask mGetTables;
        private readonly AddUpdateEntries mSpAccess;

        public CollectionStatePickerHandler(string psConnectionString)
        {
            mGetTables = new DBTask(psConnectionString);
            mSpAccess = new AddUpdateEntries(psConnectionString);
            mForceReload = true;
        }

        public bool ForceIdTableReload
        {
            set => mForceReload = value;
        }

        public void ChangeSelectedCollectionStates(int newStateId, ArrayList selectedCollectionIdList)
        {
            foreach (int id in selectedCollectionIdList)
                mSpAccess.UpdateProteinCollectionState(id, newStateId);
        }

        private void SetupPickerListView(ListView lvw, DataTable dt, string filterCriteria)
        {
            filterCriteria = filterCriteria.Trim(' ');

            var criteriaCollection = filterCriteria.Split(' ');

            var filterString = string.Empty;

            if (criteriaCollection.Length > 0 && filterCriteria.Length > 0)
            {
                foreach (var filterElement in criteriaCollection)
                    filterString += "[Name] LIKE '%" + filterElement + "%' OR [State] LIKE '%" + filterElement + "%' OR ";
                // Trim off final " OR "
                filterString = filterString.Substring(0, filterString.Length - 4);
            }
            else
            {
                filterString = "";
            }

            var collectionRows = dt.Select(filterString);

            lvw.BeginUpdate();
            foreach (var cRow in collectionRows)
            {
                var tmpCreated = Convert.ToDateTime(cRow["Created"]);
                var tmpMod = Convert.ToDateTime(cRow["Modified"]);
                var item = new ListViewItem {Text = cRow["Name"].ToString(), Tag = cRow["ID"]};
                item.SubItems.Add(tmpCreated.ToString("yyyy-MM-dd"));
                item.SubItems.Add(tmpMod.ToString("yyyy-MM-dd"));
                item.SubItems.Add(cRow["State"].ToString());
                lvw.Items.Add(item);
            }

            lvw.EndUpdate();
        }

        public void FillListView(ListView listViewToFill)
        {
            FillFilteredListView(listViewToFill, "");
        }

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

        public DataTable GetCollectionTable()
        {
            var SQL = "SELECT * FROM V_Collection_State_Picker ORDER BY [Name]";
            var cTable = mGetTables.GetTable(SQL);
            return cTable;
        }

        public DataTable GetStates()
        {
            var SQL = "SELECT State, Collection_State_ID as ID " + "FROM T_Protein_Collection_States ORDER BY Collection_State_ID";
            var sTable = mGetTables.GetTable(SQL);

            return sTable;
        }
    }
}