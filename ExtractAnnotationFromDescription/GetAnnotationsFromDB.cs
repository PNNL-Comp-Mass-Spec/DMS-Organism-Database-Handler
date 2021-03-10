using System;
using System.Data;
using PRISM;

namespace ExtractAnnotationFromDescription
{
    internal interface IGetAnnotationsFromDB
    {
    }

    [Obsolete("Unused")]
    internal class GetAnnotationsFromDB
    {
        private readonly string m_ConnectionString;
        private TableManipulationBase.DBTask m_DatabaseHelper;

        public GetAnnotationsFromDB(string psConnectionString)
        {
            m_ConnectionString = psConnectionString;
        }

        public AnnotationInfo GetAnnotationDetails(int ProteinCollectionID)
        {
            m_DatabaseHelper = new TableManipulationBase.DBTask(m_ConnectionString);

            // FYI: The constructor for AnnotationInfo() doesn't use CollectionName or ProteinCollectionID at present
            var info = new AnnotationInfo();

            // Get Protein Collection Name
            var sqlQuery1 = "SELECT TOP 1 Name FROM V_Collection_Picker " +
                            "WHERE ID = " + ProteinCollectionID;
            var nameLookupTable = m_DatabaseHelper.GetTable(sqlQuery1);

            // ReSharper disable once UnusedVariable
            string collectionName = nameLookupTable.Rows[0]["Name"].ToString();

            // Get Naming Authority Lookup

            string sqlQuery2 = "SELECT Authority_ID, Name FROM T_Naming_Authorities";
            var authorityLookupTable = m_DatabaseHelper.GetTable(sqlQuery2);

            var authorityLookupRows = authorityLookupTable.Select("");
            foreach (DataRow dr in authorityLookupRows)
                info.AddAuthorityNameToLookup(
                    DBTools.GetInteger(dr["Authority_ID"]),
                    DBTools.GetString(dr["Name"]));


            // Get Annotation Group Information

            var sqlQuery3 = "SELECT Annotation_Group, Authority_ID " +
                            "FROM T_Annotation_Groups " +
                            "WHERE Protein_Collection_ID = " + ProteinCollectionID.ToString();

            var annotationGroupLookup = m_DatabaseHelper.GetTable(sqlQuery3);

            foreach (DataRow dr in annotationGroupLookup.Rows)
                info.AddAnnotationGroupLookup(
                    DBTools.GetInteger(dr["Annotation_Group"]),
                    DBTools.GetInteger(dr["Authority_ID"]));


            // Get Collection Member Primary Information

            // Get Reference_ID, Name, Description, Protein_ID, Protein_Collection_ID,
            // Authority_ID, Annotation_Group_ID
            var sqlQuery4 = "SELECT * FROM V_Protein_Collection_Members " +
                "WHERE Protein_Collection_ID = " + ProteinCollectionID.ToString() +
                " AND Annotation_Group_ID = 0";
            var annotationTableLookup = m_DatabaseHelper.GetTable(sqlQuery4);

            var annotationTableRows = annotationTableLookup.Select("");

            foreach (DataRow dr in annotationTableRows)
            {
                int tmpRefID = DBTools.GetInteger(dr["Reference_ID"]);
                string tmpName = DBTools.GetString(dr["Name"]);
                string tmpDesc = DBTools.GetString(dr["Description"]);
                int tmpProtID = DBTools.GetInteger(dr["Protein_ID"]);
                int tmpNameAuthID = DBTools.GetInteger(dr["Annotation_Type_ID"]);

                info.AddPrimaryAnnotation(
                    tmpProtID, tmpName, tmpDesc, tmpRefID, tmpNameAuthID);
            }

            return info;
        }
    }
}