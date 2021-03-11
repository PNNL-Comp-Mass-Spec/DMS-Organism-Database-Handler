using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class AddAnnotationTypeType
    {
        private string mConnectionString;
        private AddUpdateEntries mSPRunner;

        private string mTypeName;
        private string mDescription;
        private string mExample;
        private int mAuthID;
        private bool mEntryExists = false;
        private AddNamingAuthorityType mAuthAdd;
        private DataTable mAuthorities;
        private Point mFormLocation;

        public string TypeName => mTypeName;

        public string Description => mDescription;

        public string AnnotationExample => mExample;

        public int AuthorityID => mAuthID;

        public string DisplayName => GetDisplayName(mAuthID, mTypeName);

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

        private string GetDisplayName(int authID, string authTypeName)
        {
            string authName;
            var foundRows = mAuthorities.Select("ID = " + authID).ToList();

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
            int annTypeID;
            if (mSPRunner == null)
            {
                mSPRunner = new AddUpdateEntries(mConnectionString);
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
                mAuthID = frmAnn.AuthorityID;

                annTypeID = mSPRunner.AddAnnotationType(
                    mTypeName, mDescription, mExample, mAuthID);

                if (annTypeID < 0)
                {
                    var authNames = mAuthorities.Select("Authority_ID = " + mAuthID.ToString());
                    var authName = authNames[0]["Name"].ToString();
                    MessageBox.Show(
                        "An entry called '" + mTypeName + "' for '" + authName + "' already exists in the Annotation Types table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    mEntryExists = true;
                    annTypeID = -annTypeID;
                }
                else
                {
                    mEntryExists = false;
                }
            }
            else
            {
                annTypeID = 0;
                mEntryExists = true;
            }

            mSPRunner = null;

            return annTypeID;
        }
    }
}