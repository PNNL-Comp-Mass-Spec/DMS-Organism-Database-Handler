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
            int authId;
            if (mSpRunner == null)
            {
                mSpRunner = new AddUpdateEntries(mConnectionString);
            }

            var r = frmAuth.ShowDialog();

            if (r == DialogResult.OK)
            {
                mshortName = frmAuth.ShortName;
                mfullName = frmAuth.FullName;
                mwebAddress = frmAuth.WebAddress;
                authId = mSpRunner.AddNamingAuthority(mshortName, mfullName, mwebAddress);
                if (authId < 0)
                {
                    MessageBox.Show(
                        "An entry for '" + mshortName + "' already exists in the Authorities table",
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