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
        private bool mforceReload = false;
        private DataTable mListViewData;
        private readonly DBTask mGetTables;
        private readonly AddUpdateEntries mSPAccess;

        public CollectionStatePickerHandler(string psConnectionString)
        {
            mGetTables = new DBTask(psConnectionString);
            mSPAccess = new AddUpdateEntries(psConnectionString);
            mforceReload = true;
        }

        public bool ForceIDTableReload
        {
            set => mforceReload = value;
        }

        public void ChangeSelectedCollectionStates(int newStateID, ArrayList selectedCollectionIDList)
        {
            foreach (int ID in selectedCollectionIDList)
                mSPAccess.UpdateProteinCollectionState(ID, newStateID);
        }

        private void SetupPickerListView(ListView lvw, DataTable dt, string filterCriteria)
        {
            filterCriteria = filterCriteria.Trim(' ');

            var criteriaCollection = filterCriteria.Split(' ');

            string filterString = string.Empty;

            if (criteriaCollection.Length > 0 & filterCriteria.Length > 0)
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
                var item = new ListViewItem();
                item.Text = cRow["Name"].ToString();
                item.Tag = cRow["ID"];
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

        public void FillFilteredListView(ListView listViewToFill, string FilterString)
        {
            listViewToFill.Items.Clear();
            if (mforceReload)
            {
                mListViewData = GetCollectionTable();
                mforceReload = false;
            }

            SetupPickerListView(listViewToFill, mListViewData, FilterString);
        }

        public DataTable GetCollectionTable()
        {
            string SQL = "SELECT * FROM V_Collection_State_Picker ORDER BY [Name]";
            var cTable = mGetTables.GetTable(SQL);
            return cTable;
        }

        public DataTable GetStates()
        {
            string SQL = "SELECT State, Collection_State_ID as ID " + "FROM T_Protein_Collection_States ORDER BY Collection_State_ID";
            var sTable = mGetTables.GetTable(SQL);

            return sTable;
        }
    }
}