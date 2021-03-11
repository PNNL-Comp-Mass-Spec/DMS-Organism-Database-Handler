using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class AddNamingAuthorityType
    {
        private readonly string mConnectionString;
        private AddUpdateEntries mSpRunner;

        private string mShortName;
        private string mFullName;
        private string mWebAddress;
        private bool mEntryExists = false;
        private ImportHandler mImporter;
        private readonly DataTable mAuthorityTable;
        private Point mFormLocation;

        public string ShortName => mShortName;

        public string FullName => mFullName;

        public string WebAddress => mWebAddress;

        public bool EntryExists => mEntryExists;

        public DataTable AuthoritiesTable => mAuthorityTable;

        public Point FormLocation
        {
            set => mFormLocation = value;
        }

        public AddNamingAuthorityType(string psConnectionString)
        {
            mConnectionString = psConnectionString;
            mAuthorityTable = GetAuthoritiesList();
        }

        public int AddNamingAuthority()
        {
            var frmAuth = new frmAddNamingAuthority {DesktopLocation = mFormLocation};
            int authId;
            if (mSpRunner == null)
            {
                mSpRunner = new AddUpdateEntries(mConnectionString);
            }

            var r = frmAuth.ShowDialog();

            if (r == DialogResult.OK)
            {
                mShortName = frmAuth.ShortName;
                mFullName = frmAuth.FullName;
                mWebAddress = frmAuth.WebAddress;
                authId = mSpRunner.AddNamingAuthority(mShortName, mFullName, mWebAddress);
                if (authId < 0)
                {
                    MessageBox.Show(
                        "An entry for '" + mShortName + "' already exists in the Authorities table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    mEntryExists = true;
                    authId = -authId;
                }
                else
                {
                    mEntryExists = false;
                }
            }
            else
            {
                authId = -1;
            }

            mSpRunner = null;

            return authId;
        }

        private DataTable GetAuthoritiesList()
        {
            if (mImporter == null)
            {
                mImporter = new ImportHandler(mConnectionString);
            }

            var tmpAuthTable = mImporter.LoadAuthorities();

            return tmpAuthTable;
        }
    }
}