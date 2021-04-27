using System;
using OrganismDatabaseHandler.DatabaseTools;
using OrganismDatabaseHandler.ProteinExport;

namespace OrganismDatabaseHandler.ProteinUpload
{
    internal class CollectionEncryptor
    {
        private readonly RijndaelEncryptionHandler mRijndaelEncryptor;
        private readonly DBTask mDatabaseAccessor;

        public event EncryptionStartEventHandler EncryptionStart;
        public event EncryptionProgressEventHandler EncryptionProgress;
        public event EncryptionCompleteEventHandler EncryptionComplete;

        public CollectionEncryptor(string passPhrase, DBTask databaseAccessor)
        {
            mRijndaelEncryptor = new RijndaelEncryptionHandler(passPhrase);
            mDatabaseAccessor = databaseAccessor;
        }

        public void EncryptStorageCollectionSequences(ProteinStorage.ProteinStorage storageCollection)
        {
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

            foreach (var ce in storageCollection.GetEntriesIEnumerable())
            {
                if (counter % eventTriggerThresh == 0)
                {
                    OnEncryptionProgressUpdate(counter / (double)counterMax);
                }

                ce.Sequence = mRijndaelEncryptor.Encrypt(ce.Sequence);
                ce.SHA1Hash = mRijndaelEncryptor.MakeArbitraryHash(ce.Sequence);
                ce.IsEncrypted = true;
                counter++;
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