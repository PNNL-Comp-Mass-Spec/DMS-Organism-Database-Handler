using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.ProteinStorage;

namespace AppUI_OrfDBHandler
{
    public class FilePreviewHandler
    {
        private ProteinStorage mProteins;
        private FASTAReader mLoader;
        private string mcurrentFilePath;
        private frmFilePreview mfrmPreview;

        public event FormStatusEventHandler FormStatus;

        public delegate void FormStatusEventHandler(bool visible);

        public FilePreviewHandler()
        {
            mfrmPreview = new frmFilePreview();
            mfrmPreview.RefreshRequest += FillPreview;
            mfrmPreview.FormClosing += OnFormClose;
        }

        private void GetProteins(
            string filePath,
            int lineCount)
        {
            if (mLoader == null)
            {
                mLoader = new FASTAReader();
            }

            mProteins = mLoader.GetProteinEntries(filePath, lineCount);

            var newPreviewContents = mProteins.GetEntriesIEnumerable().Select(protein =>
            {
                var li = new ListViewItem(protein.Reference);
                li.SubItems.Add(protein.Description);
                return li;
            }).ToArray();
            mfrmPreview.SetPreviewItems(newPreviewContents);
        }

        private void FillPreview(int lineCount)
        {
            GetProteins(mcurrentFilePath, lineCount);
        }

        public void ShowPreview(string filePath, int horizontalPos, int verticalPos, int height)
        {
            mcurrentFilePath = filePath;
            if (mfrmPreview == null)
            {
                mfrmPreview = new frmFilePreview();
                mfrmPreview.RefreshRequest += FillPreview;
                mfrmPreview.FormClosing += OnFormClose;
            }

            mfrmPreview.DesktopLocation = new Point(horizontalPos, verticalPos);
            mfrmPreview.Height = height;
            mfrmPreview.WindowName = "Preview of: " + Path.GetFileName(filePath);
            if (mfrmPreview.Visible == false)
            {
                mfrmPreview.Show();
            }
            else
            {
                FillPreview(mfrmPreview.GetLineCount());
            }

            FormStatus?.Invoke(true);
        }

        public void CloseForm()
        {
            mfrmPreview.Close();
        }

        ~FilePreviewHandler()
        {
            if (mfrmPreview != null)
            {
                mfrmPreview.RefreshRequest -= FillPreview;
                mfrmPreview.FormClosing -= OnFormClose;
            }

            mProteins = null;
            mLoader = null;
            mfrmPreview = null;
        }

        public void OnFormClose()
        {
            FormStatus?.Invoke(false);

            if (mfrmPreview != null)
            {
                mfrmPreview.RefreshRequest -= FillPreview;
                mfrmPreview.FormClosing -= OnFormClose;
            }
            mfrmPreview = null;
        }
    }
}