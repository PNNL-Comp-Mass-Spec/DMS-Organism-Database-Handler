using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class AddAnnotationTypeType
    {
        private readonly string mConnectionString;
        private AddUpdateEntries mSpRunner;

        private string mTypeName;
        private string mDescription;
        private string mExample;
        private int mAuthId;
        private bool mEntryExists = false;
        private AddNamingAuthorityType mAuthAdd;
        private readonly DataTable mAuthorities;
        private Point mFormLocation;

        public string TypeName => mTypeName;

        public string Description => mDescription;

        public string AnnotationExample => mExample;

        public int AuthorityID => mAuthId;

        public string DisplayName => GetDisplayName(mAuthId, mTypeName);

        public bool EntryExists => mEntryExists;

        public Point FormLocation
        {
            set => mFormLocation = value;
        }

        public AddAnnotationTypeType(string psConnectionString)
        {
            mConnectionString = psConnectionString;

            mAuthAdd = new AddNamingAuthorityType(mConnectionString);
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
            if (mSpRunner == null)
            {
                mSpRunner = new AddUpdateEntries(mConnectionString);
            }

            if (mAuthAdd == null)
            {
                mAuthAdd = new AddNamingAuthorityType(mConnectionString);
            }

            frmAnn.AuthorityTable = mAuthorities;
            frmAnn.ConnectionString = mConnectionString;
            frmAnn.DesktopLocation = mFormLocation;
            var r = frmAnn.ShowDialog();

            if (r == DialogResult.OK)
            {
                mTypeName = frmAnn.TypeName;
                mDescription = frmAnn.Description;
                mExample = frmAnn.Example;
                mAuthId = frmAnn.AuthorityID;

                annTypeId = mSpRunner.AddAnnotationType(
                    mTypeName, mDescription, mExample, mAuthId);

                if (annTypeId < 0)
                {
                    var authNames = mAuthorities.Select("Authority_ID = " + mAuthId);
                    var authName = authNames[0]["Name"].ToString();
                    MessageBox.Show(
                        "An entry called '" + mTypeName + "' for '" + authName + "' already exists in the Annotation Types table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    mEntryExists = true;
                    annTypeId = -annTypeId;
                }
                else
                {
                    mEntryExists = false;
                }
            }
            else
            {
                annTypeId = 0;
                mEntryExists = true;
            }

            mSpRunner = null;

            return annTypeId;
        }
    }
}