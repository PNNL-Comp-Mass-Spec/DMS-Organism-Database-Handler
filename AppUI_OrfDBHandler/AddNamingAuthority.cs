using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class AddNamingAuthorityType
    {
        private readonly string m_ConnectionString;
        private AddUpdateEntries m_SPRunner;

        private string m_shortName;
        private string m_fullName;
        private string m_webAddress;
        private bool m_EntryExists = false;
        private ImportHandler m_Importer;
        private readonly DataTable m_AuthorityTable;
        private Point m_FormLocation;

        public string ShortName => m_shortName;

        public string FullName => m_fullName;

        public string WebAddress => m_webAddress;

        public bool EntryExists => m_EntryExists;

        public DataTable AuthoritiesTable => m_AuthorityTable;

        public Point FormLocation
        {
            set => m_FormLocation = value;
        }

        public AddNamingAuthorityType(string psConnectionString)
        {
            m_ConnectionString = psConnectionString;
            m_AuthorityTable = GetAuthoritiesList();
        }

        public int AddNamingAuthority()
        {
            var frmAuth = new frmAddNamingAuthority();
            frmAuth.DesktopLocation = m_FormLocation;
            int authID;
            if (m_SPRunner == null)
            {
                m_SPRunner = new AddUpdateEntries(m_ConnectionString);
            }

            var r = frmAuth.ShowDialog();

            if (r == DialogResult.OK)
            {
                m_shortName = frmAuth.ShortName;
                m_fullName = frmAuth.FullName;
                m_webAddress = frmAuth.WebAddress;
                authID = m_SPRunner.AddNamingAuthority(m_shortName, m_fullName, m_webAddress);
                if (authID < 0)
                {
                    MessageBox.Show(
                        "An entry for '" + m_shortName + "' already exists in the Authorities table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    m_EntryExists = true;
                    authID = -authID;
                }
                else
                {
                    m_EntryExists = false;
                }
            }
            else
            {
                authID = -1;
            }

            m_SPRunner = null;

            return authID;
        }

        private DataTable GetAuthoritiesList()
        {
            if (m_Importer == null)
            {
                m_Importer = new ImportHandler(m_ConnectionString);
            }

            var tmpAuthTable = m_Importer.LoadAuthorities();

            return tmpAuthTable;
        }
    }
}