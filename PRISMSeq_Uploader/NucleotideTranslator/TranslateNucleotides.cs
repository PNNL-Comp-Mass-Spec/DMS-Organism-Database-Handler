using System;
using System.Collections;
using OrganismDatabaseHandler.DatabaseTools;

namespace AppUI_OrfDBHandler.NucleotideTranslator
{
    [Obsolete("Uses old table name")]
    public class TranslateNucleotides
    {
        private ArrayList mTranslationMatrix;
        private readonly DBTask mGetSQLData;

        private string mTranTableListName = "T_DNA_Translation_Tables";
        private string mTransTableMembersName = "T_DNA_Translation_Table_Members";

        public TranslateNucleotides(string dmsConnectionString)
        {
            mGetSQLData = new DBTask(dmsConnectionString);
        }

        [Obsolete("Uses old table name")]
        public ArrayList LoadTransMatrix(int translationTableId)
        {
            var baseArray = "ATGC".ToCharArray();

            var selectSql =
                "SELECT * FROM " + mTransTableMembersName +
                " WHERE DNA_Translation_Table_ID = " + translationTableId;

            var members = mGetSQLData.GetTable(selectSql);

            var primaryList = new ArrayList();
            var secondaryList = new ArrayList();
            var tertiaryList = new ArrayList();

            foreach (var base1 in baseArray)
            {
                foreach (var base2 in baseArray)
                {
                    foreach (var base3 in baseArray)
                    {
                        var tertSelect = "Base_1 = '" + base1 +
                                         "' AND Base_2 = '" + base2 +
                                         "' AND Base_3 = '" + base3 + "'";
                        var tertiaryRows = members.Select(tertSelect);

                        var dr = tertiaryRows[0];

                        tertiaryList.Add(new TranslationEntry(base3.ToString(), dr["Coded_AA"].ToString()));
                    }

                    secondaryList.Add(new TranslationEntry(base2.ToString(), tertiaryList));
                    tertiaryList = new ArrayList();
                }

                primaryList.Add(new TranslationEntry(base1.ToString(), secondaryList));
                secondaryList = new ArrayList();
            }

            return primaryList;
        }

        protected int LoadNucPositions(string filePath)
        {
            return default;
        }
    }
}