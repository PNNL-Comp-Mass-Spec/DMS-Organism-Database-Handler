using System.Collections;
using System.Data;
using OrganismDatabaseHandler.DatabaseTools;

namespace AppUI_OrfDBHandler.NucleotideTranslator
{
    public class TranslateNucleotides
    {
        private ArrayList mTranslationMatrix;
        private DBTask mGetSQLData;

        private string mTranTableListName = "T_DNA_Translation_Tables";
        private string mTransTableMembersName = "T_DNA_Translation_Table_Members";

        public TranslateNucleotides(string DMSConnectionString)
        {
            mGetSQLData = new DBTask(DMSConnectionString);
        }

        public ArrayList LoadTransMatrix(int TranslationTableID)
        {
            var BaseArray = "ATGC".ToCharArray();

            string selectSQL =
                "SELECT * FROM " + mTransTableMembersName +
                " WHERE DNA_Translation_Table_ID = " + TranslationTableID;

            var members = mGetSQLData.GetTable(selectSQL);

            var PrimaryList = new ArrayList();
            var SecondaryList = new ArrayList();
            var TertiaryList = new ArrayList();

            foreach (var base_1 in BaseArray)
            {
                foreach (var base_2 in BaseArray)
                {
                    foreach (var base_3 in BaseArray)
                    {
                        var tertSelect = "Base_1 = '" + base_1.ToString() +
                                         "' AND Base_2 = '" + base_2.ToString() +
                                         "' AND Base_3 = '" + base_3.ToString() + "'";
                        var TertiaryRows = members.Select(tertSelect);

                        var dr = TertiaryRows[0];

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