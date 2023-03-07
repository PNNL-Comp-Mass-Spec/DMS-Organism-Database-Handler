using System;
using System.Data;
using OrganismDatabaseHandler.DatabaseTools;
using PRISM;

namespace PRISMSeq_Uploader.ExtractAdditionalAnnotations
{
    internal interface IGetAnnotationsFromDb
    {
    }

    [Obsolete("Unused")]
    internal class GetAnnotationsFromDb
    {
        private readonly string mConnectionString;
        private DBTask mDatabaseHelper;

        public GetAnnotationsFromDb(string psConnectionString)
        {
            mConnectionString = psConnectionString;
        }

        public AnnotationInfo GetAnnotationDetails(int proteinCollectionId)
        {
            mDatabaseHelper = new DBTask(mConnectionString);

            // FYI: The constructor for AnnotationInfo() doesn't use collectionName or proteinCollectionId at present
            var info = new AnnotationInfo();

            // Get Protein Collection Name
            var sqlQuery1 = "SELECT Name FROM V_Collection_Picker " +
                            "WHERE ID = " + proteinCollectionId;
            var nameLookupTable = mDatabaseHelper.GetTable(sqlQuery1);

            // ReSharper disable once UnusedVariable
            var collectionName = nameLookupTable.Rows[0]["Name"].ToString();

            // Get Naming Authority Lookup

            var sqlQuery2 = "SELECT Authority_ID, Name FROM T_Naming_Authorities";
            var authorityLookupTable = mDatabaseHelper.GetTable(sqlQuery2);

            var authorityLookupRows = authorityLookupTable.Select("");
            foreach (var dr in authorityLookupRows)
            {
                info.AddAuthorityNameToLookup(
                    DBTools.GetInteger(dr["Authority_ID"]),
                    DBTools.GetString(dr["Name"]));
            }

            // Get Annotation Group Information

            var sqlQuery3 = "SELECT Annotation_Group, Authority_ID " +
                            "FROM T_Annotation_Groups " +
                            "WHERE Protein_Collection_ID = " + proteinCollectionId;

            var annotationGroupLookup = mDatabaseHelper.GetTable(sqlQuery3);

            foreach (DataRow dr in annotationGroupLookup.Rows)
            {
                info.AddAnnotationGroupLookup(
                    DBTools.GetInteger(dr["Annotation_Group"]),
                    DBTools.GetInteger(dr["Authority_ID"]));
            }

            // Get Collection Member Primary Information

            // Get Reference_ID, Name, Description, Protein_ID, Protein_Collection_ID,
            // Authority_ID, Annotation_Group_ID
            var sqlQuery4 = "SELECT * FROM V_Protein_Collection_Members " +
                "WHERE Protein_Collection_ID = " + proteinCollectionId +
                " AND Annotation_Group_ID = 0";
            var annotationTableLookup = mDatabaseHelper.GetTable(sqlQuery4);

            var annotationTableRows = annotationTableLookup.Select("");

            foreach (var dr in annotationTableRows)
            {
                var tmpRefId = DBTools.GetInteger(dr["Reference_ID"]);
                var tmpName = DBTools.GetString(dr["Name"]);
                var tmpDesc = DBTools.GetString(dr["Description"]);
                var tmpProtId = DBTools.GetInteger(dr["Protein_ID"]);
                var tmpNameAuthId = DBTools.GetInteger(dr["Annotation_Type_ID"]);

                info.AddPrimaryAnnotation(
                    tmpProtId, tmpName, tmpDesc, tmpRefId, tmpNameAuthId);
            }

            return info;
        }
    }
}