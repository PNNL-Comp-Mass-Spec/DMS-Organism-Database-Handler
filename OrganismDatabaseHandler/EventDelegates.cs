using System.Collections.Generic;
using System.Data;
using OrganismDatabaseHandler.ProteinUpload;
using ValidateFastaFile;

namespace OrganismDatabaseHandler
{
    public delegate void LoadStartEventHandler(string taskTitle);

    public delegate void LoadEndEventHandler();

    public delegate void LoadProgressEventHandler(double fractionDone);

    public delegate void FileGenerationCompletedEventHandler(string outputPath);

    public delegate void FileGenerationProgressEventHandler(string statusMsg, double fractionDone);

    public delegate void FileGenerationStartedEventHandler(string taskMsg);

    public delegate void ExportStartEventHandler(string taskTitle);

    public delegate void ExportProgressEventHandler(string statusMsg, double fractionDone);

    public delegate void ExportEndEventHandler();

    public delegate void CollectionLoadCompleteEventHandler(DataTable collectionsTable);

    public delegate void EncryptionStartEventHandler(string taskMsg);

    public delegate void EncryptionProgressEventHandler(string statusMsg, double fractionDone);

    public delegate void EncryptionCompleteEventHandler();

    public delegate void BatchProgressEventHandler(string status);

    public delegate void ValidationProgressEventHandler(string taskTitle, double fractionDone);

    public delegate void ValidFASTAFileLoadedEventHandler(string fastaFilePath, PSUploadHandler.UploadInfo uploadData);

    public delegate void InvalidFASTAFileEventHandler(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> errorCollection);

    public delegate void FASTAFileWarningsEventHandler(string fastaFilePath, List<CustomFastaValidator.ErrorInfoExtended> warningCollection);

    public delegate void WroteLineEndNormalizedFASTAEventHandler(string newFilePath);
}
