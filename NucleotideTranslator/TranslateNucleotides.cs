using System.Collections;
using System.Data;

namespace NucleotideTranslator
{
    public class TranslateNucleotides
    {
        private ArrayList m_TranslationMatrix;
        private TableManipulationBase.DBTask m_GetSQLData;

        private string m_TranTableListName = "T_DNA_Translation_Tables";
        private string m_TransTableMembersName = "T_DNA_Translation_Table_Members";

        public TranslateNucleotides(string DMSConnectionString)
        {
            m_GetSQLData = new TableManipulationBase.DBTask(DMSConnectionString);
        }

        public ArrayList LoadTransMatrix(int TranslationTableID)
        {
            var BaseArray = "ATGC".ToCharArray();

            string selectSQL =
                "SELECT * FROM " + m_TransTableMembersName +
                " WHERE DNA_Translation_Table_ID = " + TranslationTableID;

            var members = m_GetSQLData.GetTable(selectSQL);

            DataRow dr;
            string tertSelect;
            DataRow[] TertiaryRows;

            var PrimaryList = new ArrayList();
            var SecondaryList = new ArrayList();
            var TertiaryList = new ArrayList();

            foreach (var base_1 in BaseArray)
            {
                foreach (var base_2 in BaseArray)
                {
                    foreach (var base_3 in BaseArray)
                    {
                        tertSelect = "Base_1 = '" + base_1.ToString() +
                            "' AND Base_2 = '" + base_2.ToString() +
                            "' AND Base_3 = '" + base_3.ToString() + "'";
                        TertiaryRows = members.Select(tertSelect);

                        dr = TertiaryRows[0];

                        TertiaryList.Add(new TranslationEntry(base_3.ToString(), dr["Coded_AA"].ToString()));
                    }

                    SecondaryList.Add(new TranslationEntry(base_2.ToString(), TertiaryList));
                    TertiaryList = new ArrayList();
                }

                PrimaryList.Add(new TranslationEntry(base_1.ToString(), SecondaryList));
                SecondaryList = new ArrayList();
            }

            return PrimaryList;
        }

        protected int LoadNucPositions(string filePath)
        {
            return default;
        }
    }
}