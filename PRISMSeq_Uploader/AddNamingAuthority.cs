using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;
using PRISM;
using PRISMDatabaseUtils;

namespace PRISMSeq_Uploader
{
    public class AddNamingAuthorityType : EventNotifier
    {
        private readonly string mConnectionString;
        private AddUpdateEntries mSpRunner;

        private string mWebAddress;
        private ImportHandler mImporter;
        private Point mFormLocation;

        public string ShortName { get; private set; }

        public string FullName { get; private set; }

        public bool EntryExists { get; private set; }

        public DataTable AuthoritiesTable { get; }

        public Point FormLocation
        {
            set => mFormLocation = value;
        }

        public AddNamingAuthorityType(string dbConnectionString)
        {
            mConnectionString = dbConnectionString;
            AuthoritiesTable = GetAuthoritiesList();
        }

        public int AddNamingAuthority()
        {
            var frmAuth = new frmAddNamingAuthority {DesktopLocation = mFormLocation};
            int authId;

            if (mSpRunner == null)
            {
                var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mConnectionString, "PRISMSeq_Uploader");

                mSpRunner = new AddUpdateEntries(connectionStringToUse);
                RegisterEvents(mSpRunner);
            }

            var r = frmAuth.ShowDialog();

            if (r == DialogResult.OK)
            {
                ShortName = frmAuth.ShortName;
                FullName = frmAuth.FullName;
                mWebAddress = frmAuth.WebAddress;
                authId = mSpRunner.AddNamingAuthority(ShortName, FullName, mWebAddress);

                if (authId < 0)
                {
                    MessageBox.Show(
                        "An entry for '" + ShortName + "' already exists in the Authorities table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    EntryExists = true;
                    authId = -authId;
                }
                else
                {
                    EntryExists = false;
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
            var connectionStringToUse = DbToolsFactory.AddApplicationNameToConnectionString(mConnectionString, "PRISMSeq_Uploader");

            mImporter ??= new ImportHandler(connectionStringToUse);

            return mImporter.LoadAuthorities();
        }
    }
}