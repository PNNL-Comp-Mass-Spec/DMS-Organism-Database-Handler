using System;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler.ExtractAdditionalAnnotations
{
    public partial class frmRegexSpecifyFromDescription : Form
    {
        public frmRegexSpecifyFromDescription()
        {
            base.Load += frmRegexSpecify_Load;

            InitializeComponent();
        }

        private ExtractionSources m_ExtractionSource;

        public enum ExtractionSources
        {
            Name,
            Description,
        }

        private void frmRegexSpecify_Load(object sender, EventArgs e)
        {
        }

        private void lblNewNames_Click(object sender, EventArgs e)
        {
        }

        private void lbxNewNames_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmdMatch_Click(object sender, EventArgs e)
        {
        }

        private void rdbSourceSelect_CheckedChanged(object sender, EventArgs e)
        {
            System.Windows.Forms.RadioButton rdb = (System.Windows.Forms.RadioButton)sender;

            if (rdb.Checked == true)
            {
                if ((rdb.Name ?? "") == (rdbNameSelect.Name ?? ""))
                {
                    m_ExtractionSource = ExtractionSources.Name;
                }
                else if ((rdb.Name ?? "") == (rdbDescriptionSelect.Name ?? ""))
                {
                    m_ExtractionSource = ExtractionSources.Description;
                }
            }
        }
    }
}
