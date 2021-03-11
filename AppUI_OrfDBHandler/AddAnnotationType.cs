using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;

namespace AppUI_OrfDBHandler
{
    public class AddAnnotationTypeType
    {
        private string m_ConnectionString;
        private AddUpdateEntries m_SPRunner;

        private string m_TypeName;
        private string m_Description;
        private string m_Example;
        private int m_AuthID;
        private bool m_EntryExists = false;
        private AddNamingAuthorityType m_AuthAdd;
        private DataTable m_Authorities;
        private Point m_FormLocation;

        public string TypeName => m_TypeName;

        public string Description => m_Description;

        public string AnnotationExample => m_Example;

        public int AuthorityID => m_AuthID;

        public string DisplayName => GetDisplayName(m_AuthID, m_TypeName);

        public bool EntryExists => m_EntryExists;

        public Point FormLocation
        {
            set => m_FormLocation = value;
        }

        public AddAnnotationTypeType(string psConnectionString)
        {
            m_ConnectionString = psConnectionString;

            m_AuthAdd = new AddNamingAuthorityType(m_ConnectionString);
            m_Authorities = m_AuthAdd.AuthoritiesTable;
        }

        private string GetDisplayName(int authID, string authTypeName)
        {
            string authName;
            var foundRows = m_Authorities.Select("ID = " + authID).ToList();

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
            if (m_SPRunner == null)
            {
                m_SPRunner = new AddUpdateEntries(m_ConnectionString);
            }

            if (m_AuthAdd == null)
            {
                m_AuthAdd = new AddNamingAuthorityType(m_ConnectionString);
            }

            frmAnn.AuthorityTable = m_Authorities;
            frmAnn.ConnectionString = m_ConnectionString;
            frmAnn.DesktopLocation = m_FormLocation;
            var r = frmAnn.ShowDialog();

            if (r == DialogResult.OK)
            {
                m_TypeName = frmAnn.TypeName;
                m_Description = frmAnn.Description;
                m_Example = frmAnn.Example;
                m_AuthID = frmAnn.AuthorityID;

                annTypeID = m_SPRunner.AddAnnotationType(
                    m_TypeName, m_Description, m_Example, m_AuthID);

                if (annTypeID < 0)
                {
                    var authNames = m_Authorities.Select("Authority_ID = " + m_AuthID.ToString());
                    var authName = authNames[0]["Name"].ToString();
                    MessageBox.Show(
                        "An entry called '" + m_TypeName + "' for '" + authName + "' already exists in the Annotation Types table",
                        "Entry already exists!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                    m_EntryExists = true;
                    annTypeID = -annTypeID;
                }
                else
                {
                    m_EntryExists = false;
                }
            }
            else
            {
                annTypeID = 0;
                m_EntryExists = true;
            }

            m_SPRunner = null;

            return annTypeID;
        }
    }
}