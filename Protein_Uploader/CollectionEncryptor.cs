using System;

namespace Protein_Uploader
{
    internal class CollectionEncryptor
    {
        protected Protein_Exporter.RijndaelEncryptionHandler m_RijndaelEncryptor;
        private readonly TableManipulationBase.DBTask m_DatabaseAccessor;

        public event EncryptionStartEventHandler EncryptionStart;

        public delegate void EncryptionStartEventHandler(string taskMsg);

        public event EncryptionProgressEventHandler EncryptionProgress;

        public delegate void EncryptionProgressEventHandler(string statusMsg, double fractionDone);

        public event EncryptionCompleteEventHandler EncryptionComplete;

        public delegate void EncryptionCompleteEventHandler();

        public CollectionEncryptor(string PassPhrase, TableManipulationBase.DBTask databaseAccessor)
        {
            m_RijndaelEncryptor = new Protein_Exporter.RijndaelEncryptionHandler(PassPhrase);
            m_DatabaseAccessor = databaseAccessor;
        }

        public void EncryptStorageCollectionSequences(Protein_Storage.ProteinStorage storageCollection)
        {
            var e = storageCollection.GetEnumerator();

            OnEncryptionStart("Encrypting Sequences");
            int counter = 0;
            int counterMax = storageCollection.ProteinCount;
            int EventTriggerThresh;

            if (counterMax <= 50)
            {
                EventTriggerThresh = 1;
            }
            else
            {
                EventTriggerThresh = (int)Math.Round(counterMax / 50d);
            }

            while (e.MoveNext())
            {
                if (counter % EventTriggerThresh == 0)
                {
                    OnEncryptionProgressUpdate(counter / (double)counterMax);
                }

                var ce = e.Current.Value;
                ce.Sequence = m_RijndaelEncryptor.Encrypt(ce.Sequence);
                ce.SHA1Hash = m_RijndaelEncryptor.MakeArbitraryHash(ce.Sequence);
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