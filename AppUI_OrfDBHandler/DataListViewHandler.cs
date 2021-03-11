using System.Data;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler
{
    public class DataListViewHandler
    {
        public DataListViewHandler(ListView listViewToFill)
        {
            mLVW = listViewToFill;
        }

        private readonly ListView mLVW;

        public void Load(DataTable listTable)
        {
            FillListView(mLVW, listTable);
        }

        public void Load(DataTable listTable, string quickFilterCriteria)
        {
            FillFilteredListView(mLVW, listTable, quickFilterCriteria);
        }

        private void SetupPickerListView(
            ListView lvw,
            DataTable dt,
            string filterCriteria = "")
        {
            var filterString = string.Empty;

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