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
        private bool m_forceReload = false;
        private DataTable m_ListViewData;
        private readonly DBTask m_GetTables;
        private readonly AddUpdateEntries m_SPAccess;

        public CollectionStatePickerHandler(string psConnectionString)
        {
            m_GetTables = new DBTask(psConnectionString);
            m_SPAccess = new AddUpdateEntries(psConnectionString);
            m_forceReload = true;
        }

        public bool ForceIDTableReload
        {
            set => m_forceReload = value;
        }

        public void ChangeSelectedCollectionStates(int newStateID, ArrayList selectedCollectionIDList)
        {
            foreach (int ID in selectedCollectionIDList)
                m_SPAccess.UpdateProteinCollectionState(ID, newStateID);
        }

        private void SetupPickerListView(ListView lvw, DataTable dt, string filterCriteria)
        {
            DateTime tmpCreated;
            DateTime tmpMod;

            filterCriteria = filterCriteria.Trim(' ');

            var criteriaCollection = filterCriteria.Split(' ');
            DataRow[] collectionRows;

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

            collectionRows = dt.Select(filterString);
            ListViewItem item;

            lvw.BeginUpdate();
            foreach (var cRow in collectionRows)
            {
                tmpCreated = Convert.ToDateTime(cRow["Created"]);
                tmpMod = Convert.ToDateTime(cRow["Modified"]);
                item = new ListViewItem();
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
            if (m_forceReload)
            {
                m_ListViewData = GetCollectionTable();
                m_forceReload = false;
            }

            SetupPickerListView(listViewToFill, m_ListViewData, FilterString);
        }

        public DataTable GetCollectionTable()
        {
            string SQL = "SELECT * FROM V_Collection_State_Picker ORDER BY [Name]";
            var cTable = m_GetTables.GetTable(SQL);
            return cTable;
        }

        public DataTable GetStates()
        {
            string SQL = "SELECT State, Collection_State_ID as ID " + "FROM T_Protein_Collection_States ORDER BY Collection_State_ID";
            var sTable = m_GetTables.GetTable(SQL);

            return sTable;
        }
    }
}