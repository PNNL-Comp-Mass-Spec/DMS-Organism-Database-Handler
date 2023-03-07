namespace PRISMSeq_Uploader
{
    public delegate void FormStatusEventHandler(bool visible);
    public delegate void RefreshRequestEventHandler(int lineCount);
    public delegate void LoadProgressEventHandler(double fractionDone);
    public delegate void TaskChangeEventHandler(string currentTaskTitle);
    public delegate void SyncStartEventHandler(string statusMsg);
    public delegate void SyncProgressEventHandler(string statusMsg, double fractionDone);
    public delegate void SyncCompleteEventHandler();
}
