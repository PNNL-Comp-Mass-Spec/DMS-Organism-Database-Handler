using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;
using PRISM;
using PRISMDatabaseUtils;

namespace PRISMSeq_Uploader
{
    public class AddAnnotationTypeType : EventNotifier
    {
        private readonly string mConnectionString;
        private AddUpdateEntries mSpRunner;

        private AddNamingAuthorityType mAuthAdd;
        private readonly DataTable mAuthorities;
        private Point mFormLocation;

        public string TypeName { get; private set; }

        public string Description { get; private set; }

        public string AnnotationExample { get; private set; }

        public int AuthorityID { get; private set; }

        public string DisplayName => GetDisplayName(AuthorityID, TypeName);

        public bool EntryExists { get; private set; }

        public Point FormLocation
        {
            set => mFormLocation = value;
        }

        public AddAnnotationTypeType(string dbConnectionString)
        {
            mConnectionString = dbConnectionString;

            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mConnectionString, "PRISMSeq_Uploader");

            mAuthAdd = new AddNamingAuthorityType(connectionStringToUse);
            RegisterEvents(mAuthAdd);

            mAuthorities = mAuthAdd.AuthoritiesTable;
        }

        private string GetDisplayName(int authId, string authTypeName)
        {
            string authName;
            var foundRows = mAuthorities.Select("ID = " + authId).ToList();

            if (foundRows.Count > 0)
            {
                authName = foundRows[0]["Display_Name"].ToString();
            }
            else
            {
                authName = "UnknownAuth";
            }

            return authName + " - " + authTypeName;
        }

        public int AddAnnotationType()
        {
            var frmAnn = new frmAddAnnotationType();
            int annTypeId;

            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mConnectionString, "PRISMSeq_Uploader");

            if (mSpRunner == null)
            {
                mSpRunner = new AddUpdateEntries(connectionStringToUse);
                RegisterEvents(mSpRunner);
            }

            if (mAuthAdd == null)
            {
                mAuthAdd = new AddNamingAuthorityType(connectionStringToUse);
                RegisterEvents(mAuthAdd);
            }

            frmAnn.AuthorityTable = mAuthorities;
            frmAnn.ConnectionString = connectionStringToUse;
            frmAnn.DesktopLocation = mFormLocation;
            var r = frmAnn.ShowDialog();

            if (r == DialogResult.OK)
            {
                TypeName = frmAnn.TypeName;
                Description = frmAnn.Description;
                AnnotationExample = frmAnn.Example;
                AuthorityID = frmAnn.AuthorityID;

                annTypeId = mSpRunner.AddAnnotationType(
                    TypeName, Description, AnnotationExample, AuthorityID);

                if (annTypeId < 0)
                {
                    var authNames = mAuthorities.Select("Authority_ID = " + AuthorityID);
                    var authName = authNames[0]["Name"].ToString();
                    MessageBox.Show(
                        "An entry called '" + TypeName + "' for '" + authName + "' already exists in the Annotation Types table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    EntryExists = true;
                    annTypeId = -annTypeId;
                }
                else
                {
                    EntryExists = false;
                }
            }
            else
            {
                annTypeId = 0;
                EntryExists = true;
            }

            mSpRunner = null;

            return annTypeId;
        }
    }
}