using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OrganismDatabaseHandler.ProteinImport;
using OrganismDatabaseHandler.ProteinStorage;
using PRISM;

namespace PRISMSeq_Uploader
{
    public class FilePreviewHandler : EventNotifier
    {
        private ProteinStorage mProteins;
        private FASTAReader mLoader;
        private string mCurrentFilePath;
        private frmFilePreview mFrmPreview;

        public event FormStatusEventHandler FormStatus;

        public FilePreviewHandler()
        {
            mFrmPreview = new frmFilePreview();
            mFrmPreview.RefreshRequest += FillPreview;
            mFrmPreview.FormClosed += OnFormClose;
        }

        private void GetProteins(
            string filePath,
            int lineCount)
        {
            if (mLoader == null)
            {
                mLoader = new FASTAReader();
                RegisterEvents(mLoader);
            }

            mProteins = mLoader.GetProteinEntries(filePath, lineCount);

            var newPreviewContents = mProteins.GetEntriesIEnumerable().Select(protein =>
            {
                var li = new ListViewItem(protein.Reference);
                li.SubItems.Add(protein.Description);
                return li;
            }).ToArray();
            mFrmPreview.SetPreviewItems(newPreviewContents);
        }

        private void FillPreview(int lineCount)
        {
            GetProteins(mCurrentFilePath, lineCount);
        }

        public void ShowPreview(string filePath, int horizontalPos, int verticalPos, int height)
        {
            mCurrentFilePath = filePath;
            if (mFrmPreview == null)
            {
                mFrmPreview = new frmFilePreview();
                mFrmPreview.RefreshRequest += FillPreview;
                mFrmPreview.FormClosed += OnFormClose;
            }

            mFrmPreview.DesktopLocation = new Point(horizontalPos, verticalPos);
            mFrmPreview.Height = height;
            mFrmPreview.WindowName = "Preview of: " + Path.GetFileName(filePath);
            if (!mFrmPreview.Visible)
            {
                mFrmPreview.Show();
            }
            else
            {
                FillPreview(mFrmPreview.GetLineCount());
            }

            FormStatus?.Invoke(true);
        }

        public void CloseForm()
        {
            mFrmPreview.Close();
        }

        ~FilePreviewHandler()
        {
            if (mFrmPreview != null)
            {
                mFrmPreview.RefreshRequest -= FillPreview;
                mFrmPreview.FormClosed -= OnFormClose;
            }

            mProteins = null;
            mLoader = null;
            mFrmPreview = null;
        }

        public void OnFormClose(object sender, FormClosedEventArgs e)
        {
            FormStatus?.Invoke(false);

            if (mFrmPreview != null)
            {
                mFrmPreview.RefreshRequest -= FillPreview;
                mFrmPreview.FormClosed -= OnFormClose;
            }
            mFrmPreview = null;
        }
    }
}