using System.Data;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public class DataListViewHandler
    {
        public DataListViewHandler(ListView listViewToFill)
        {
            m_LVW = listViewToFill;
        }

        private readonly ListView m_LVW;

        public void Load(DataTable listTable)
        {
            FillListView(m_LVW, listTable);
        }

        public void Load(DataTable listTable, string quickFilterCriteria)
        {
            FillFilteredListView(m_LVW, listTable, quickFilterCriteria);
        }

        private void SetupPickerListView(
            ListView lvw,
            DataTable dt,
            string filterCriteria = "")
        {
            string filterString = string.Empty;

            if (filterCriteria.Length != 0)
            {
                filterString = "[Name] LIKE '%" + filterCriteria + "%' " +
                    "OR [Description] LIKE '%" + filterCriteria + "%'";
            }

            lvw.BeginUpdate();

            var itemRows = dt.Select(filterString, "Name");

            //NumberLoadedStatus?.Invoke(itemRows.Length, dt.Rows.Count);

            // proteinCount = Convert.ToInt32(itemRows.Length);

            //LoadStart?.Invoke("Filling List...");

            foreach (var itemRow in itemRows)
            {
                var item = new ListViewItem();
                item.Text = itemRow[0].ToString();
                item.SubItems.Add(itemRow[1].ToString());
                lvw.Items.Add(item);
            }

            lvw.EndUpdate();
        }

        protected void FillListView(
            ListView listViewToFill,
            DataTable listData)
        {
            listViewToFill.Items.Clear();
            SetupPickerListView(listViewToFill, listData);
        }

        protected void FillFilteredListView(
            ListView listViewToFill,
            DataTable listData,
            string filterString)
        {
            listViewToFill.Items.Clear();
            SetupPickerListView(listViewToFill, listData, filterString);
        }
    }
}