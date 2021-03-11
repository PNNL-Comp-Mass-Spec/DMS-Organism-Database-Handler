using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class AddNamingAuthorityType
    {
        private readonly string mConnectionString;
        private AddUpdateEntries mSPRunner;

        private string mshortName;
        private string mfullName;
        private string mwebAddress;
        private bool mEntryExists = false;
        private ImportHandler mImporter;
        private readonly DataTable mAuthorityTable;
        private Point mFormLocation;

        public string ShortName => mshortName;

        public string FullName => mfullName;

        public string WebAddress => mwebAddress;

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
            var frmAuth = new frmAddNamingAuthority();
            frmAuth.DesktopLocation = mFormLocation;
            int authID;
            if (mSPRunner == null)
            {
                mSPRunner = new AddUpdateEntries(mConnectionString);
            }

            var r = frmAuth.ShowDialog();

            if (r == DialogResult.OK)
            {
                mshortName = frmAuth.ShortName;
                mfullName = frmAuth.FullName;
                mwebAddress = frmAuth.WebAddress;
                authID = mSPRunner.AddNamingAuthority(mshortName, mfullName, mwebAddress);
                if (authID < 0)
                {
                    MessageBox.Show(
                        "An entry for '" + mshortName + "' already exists in the Authorities table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    mEntryExists = true;
                    authID = -authID;
                }
                else
                {
                    mEntryExists = false;
                }
            }
            else
            {
                authID = -1;
            }

            mSPRunner = null;

            return authID;
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