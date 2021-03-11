using System;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinExport;

namespace OrganismDatabaseHandler.ProteinUpload
{
    internal class CollectionEncryptor
    {
        private RijndaelEncryptionHandler mRijndaelEncryptor;
        private readonly DBTask mDatabaseAccessor;

        public event EncryptionStartEventHandler EncryptionStart;

        public delegate void EncryptionStartEventHandler(string taskMsg);

        public event EncryptionProgressEventHandler EncryptionProgress;

        public delegate void EncryptionProgressEventHandler(string statusMsg, double fractionDone);

        public event EncryptionCompleteEventHandler EncryptionComplete;

        public delegate void EncryptionCompleteEventHandler();

        public CollectionEncryptor(string passPhrase, DBTask databaseAccessor)
        {
            mRijndaelEncryptor = new RijndaelEncryptionHandler(passPhrase);
            mDatabaseAccessor = databaseAccessor;
        }

        public void EncryptStorageCollectionSequences(ProteinStorage.ProteinStorage storageCollection)
        {
            var e = storageCollection.GetEnumerator();

            OnEncryptionStart("Encrypting Sequences");
            var counter = 0;
            var counterMax = storageCollection.ProteinCount;
            int eventTriggerThresh;

            if (counterMax <= 50)
            {
                eventTriggerThresh = 1;
            }
            else
            {
                eventTriggerThresh = (int)Math.Round(counterMax / 50d);
            }

            while (e.MoveNext())
            {
                if (counter % eventTriggerThresh == 0)
                {
                    OnEncryptionProgressUpdate(counter / (double)counterMax);
                }

                var ce = e.Current.Value;
                ce.Sequence = mRijndaelEncryptor.Encrypt(ce.Sequence);
                ce.SHA1Hash = mRijndaelEncryptor.MakeArbitraryHash(ce.Sequence);
                ce.IsEncrypted = true;
                counter += 1;
            }
        }

        public void AddUpdateEncryptionMetadata()
        {
        }

        private void OnEncryptionStart(string taskMsg)
        {
            EncryptionStart?.Invoke(taskMsg);
        }

        private void OnEncryptionProgressUpdate(double fractionDone)
        {
            EncryptionProgress?.Invoke("", fractionDone);
        }

        private void OnEncryptionComplete()
        {
            EncryptionComplete?.Invoke();
        }
    }
}