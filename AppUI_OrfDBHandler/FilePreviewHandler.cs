using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Protein_Importer;
using Protein_Storage;

namespace AppUI_OrfDBHandler
{
    public class FilePreviewHandler
    {
        private ProteinStorage m_Proteins;
        private FASTAReader m_Loader;
        private string m_currentFilePath;
        private frmFilePreview m_frmPreview;

        public event FormStatusEventHandler FormStatus;

        public delegate void FormStatusEventHandler(bool visible);

        public FilePreviewHandler()
        {
            m_frmPreview = new frmFilePreview();
            m_frmPreview.RefreshRequest += FillPreview;
            m_frmPreview.FormClosing += OnFormClose;
        }

        private void GetProteins(
            string filePath,
            int lineCount)
        {
            if (m_Loader == null)
            {
                m_Loader = new FASTAReader();
            }

            m_Proteins = m_Loader.GetProteinEntries(filePath, lineCount);

            var newPreviewContents = m_Proteins.GetEntriesIEnumerable().Select(protein =>
            {
                var li = new ListViewItem(protein.Reference);
                li.SubItems.Add(protein.Description);
                return li;
            }).ToArray();
            m_frmPreview.SetPreviewItems(newPreviewContents);
        }

        private void FillPreview(int lineCount)
        {
            GetProteins(m_currentFilePath, lineCount);
        }

        public void ShowPreview(string filePath, int horizontalPos, int verticalPos, int height)
        {
            m_currentFilePath = filePath;
            if (m_frmPreview == null)
            {
                m_frmPreview = new frmFilePreview();
                m_frmPreview.RefreshRequest += FillPreview;
                m_frmPreview.FormClosing += OnFormClose;
            }

            m_frmPreview.DesktopLocation = new Point(horizontalPos, verticalPos);
            m_frmPreview.Height = height;
            m_frmPreview.WindowName = "Preview of: " + Path.GetFileName(filePath);
            if (m_frmPreview.Visible == false)
            {
                m_frmPreview.Show();
            }
            else
            {
                FillPreview(m_frmPreview.GetLineCount());
            }

            FormStatus?.Invoke(true);
        }

        public void CloseForm()
        {
            m_frmPreview.Close();
        }

        ~FilePreviewHandler()
        {
            if (m_frmPreview != null)
            {
                m_frmPreview.RefreshRequest -= FillPreview;
                m_frmPreview.FormClosing -= OnFormClose;
            }

            m_Proteins = null;
            m_Loader = null;
            m_frmPreview = null;
        }

        public void OnFormClose()
        {
            FormStatus?.Invoke(false);

            if (m_frmPreview != null)
            {
                m_frmPreview.RefreshRequest -= FillPreview;
                m_frmPreview.FormClosing -= OnFormClose;
            }
            m_frmPreview = null;
        }
    }
}