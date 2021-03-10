using System.Data;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

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
            DataRow[] itemRows;
            string filterString = string.Empty;

            if (Strings.Len(filterCriteria) != 0)
            {
                filterString = "[Name] LIKE '%" + filterCriteria + "%' " +
                    "OR [Description] LIKE '%" + filterCriteria + "%'";
            }

            lvw.BeginUpdate();

            itemRows = dt.Select(filterString, "Name");

            //NumberLoadedStatus?.Invoke(itemRows.Length, dt.Rows.Count);

            // proteinCount = Conversions.ToInteger(itemRows.Length);

            //LoadStart?.Invoke("Filling List...");

            foreach (var itemRow in itemRows)
            {
                var item = new ListViewItem();
                item.Text = Conversions.ToString(itemRow[0]);
                item.SubItems.Add(Conversions.ToString(itemRow[1]));
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