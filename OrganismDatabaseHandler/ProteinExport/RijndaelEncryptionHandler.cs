using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OrganismDatabaseHandler.ProteinExport
{
    public class RijndaelEncryptionHandler
    {
        // <summary>
        // Encrypts specified plaintext using Rijndael symmetric key algorithm
        // and returns a base64-encoded result.
        // </summary>
        // <param name="plainText">
        // Plaintext value to be encrypted.
        // </param>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived. The
        // derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that this
        // passphrase is an ASCII string.
        // </param>
        // <param name="saltValue">
        // Salt value used along with passphrase to generate password. Salt can
        // be any string. In this example we assume that salt is an ASCII string.
        // </param>
        // <param name="hashAlgorithm">
        // Hash algorithm used to generate password. Allowed values are: "MD5" and
        // "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        // </param>
        // <param name="passwordIterations">
        // Number of iterations used to generate password. One or two iterations
        // should be enough.
        // </param>
        // <param name="initVector">
        // Initialization vector (or IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long.
        // </param>
        // <param name="keySize">
        // Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        // Longer keys are more secure than shorter keys.
        // </param>
        // <returns>
        // Encrypted value formatted as a base64-encoded string.
        // </returns>

        private const int NUM_PW_ITERATIONS = 1;
        private const string SALT_VALUE = "pRi5m533kRu135";
        private const string INIT_VECTOR = "@3k8573j4083j410";
        private const int KEY_SIZE = 192;

        private readonly Rfc2898DeriveBytes m_Password;

        private readonly RijndaelManaged m_SymmetricKey;
        private SHA1Managed m_SHA1Provider;
        private readonly ICryptoTransform m_Encryptor;
        private readonly ICryptoTransform m_Decryptor;

        private readonly byte[] m_KeyBytes;
        private readonly byte[] m_saltValueBytes;
        private readonly byte[] m_initVectorBytes;

        public RijndaelEncryptionHandler(string passPhrase)
        {

            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            m_initVectorBytes = Encoding.ASCII.GetBytes(INIT_VECTOR);

            m_saltValueBytes = Encoding.ASCII.GetBytes(SALT_VALUE);

            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and
            // salt value. The password will be created using the specified hash
            // algorithm. Password creation can be done in several iterations.
            m_Password = new Rfc2898DeriveBytes(passPhrase, m_saltValueBytes, NUM_PW_ITERATIONS);

            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            m_KeyBytes = m_Password.GetBytes((int)Math.Round(KEY_SIZE / 8d));

            // Create uninitialized Rijndael encryption object.
            m_SymmetricKey = new RijndaelManaged();

            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            m_SymmetricKey.Mode = CipherMode.CBC;

            // Generate encryptor from the existing key bytes and initialization
            // vector. Key size will be defined based on the number of the key
            // bytes.
            m_Encryptor = m_SymmetricKey.CreateEncryptor(m_KeyBytes, m_initVectorBytes);

            m_Decryptor = m_SymmetricKey.CreateDecryptor(m_KeyBytes, m_initVectorBytes);
        }

        public string Encrypt(string plainText)
        {
            // Convert our plaintext into a byte array.
            // Let us assume that plaintext contains UTF8-encoded characters.
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            // Define memory stream which will be used to hold encrypted data.
            var memoryStream = new MemoryStream();

            // Define cryptographic stream (always use Write mode for encryption).
            var cryptoStream = new CryptoStream(memoryStream,
                m_Encryptor,
                CryptoStreamMode.Write);
            // Start encrypting.
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            // Finish encrypting.
            cryptoStream.FlushFinalBlock();

            // Convert our encrypted data from a memory stream into a byte array.
            var cipherTextBytes = memoryStream.ToArray();

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert encrypted data into a base64-encoded string.
            var cipherText = Convert.ToBase64String(cipherTextBytes);

            // Return encrypted string.
            return cipherText;
        }

        public string MakeArbitraryHash(string Sourcetext)
        {
            if (m_SHA1Provider == null)
            {
                m_SHA1Provider = new SHA1Managed();
            }

            // Create an encoding object to ensure the encoding standard for the source text
            var Ue = new ASCIIEncoding();

            // Retrieve a byte array based on the source text
            var ByteSourceText = Ue.GetBytes(Sourcetext);

            // Compute the hash value from the source
            var SHA1_hash = m_SHA1Provider.ComputeHash(ByteSourceText);

            // And convert it to String format for return
            string SHA1string = ToHexString(SHA1_hash);

            return SHA1string;
        }

        // <summary>
        // Decrypts specified ciphertext using Rijndael symmetric key algorithm.
        // </summary>
        // <param name="cipherText">
        // Base64-formatted ciphertext value.
        // </param>
        // <param name="passPhrase">
        // Passphrase from which a pseudo-random password will be derived. The
        // derived password will be used to generate the encryption key.
        // Passphrase can be any string. In this example we assume that this
        // passphrase is an ASCII string.
        // </param>
        // <param name="saltValue">
        // Salt value used along with passphrase to generate password. Salt can
        // be any string. In this example we assume that salt is an ASCII string.
        // </param>
        // <param name="hashAlgorithm">
        // Hash algorithm used to generate password. Allowed values are: "MD5" and
        // "SHA1". SHA1 hashes are a bit slower, but more secure than MD5 hashes.
        // </param>
        // <param name="passwordIterations">
        // Number of iterations used to generate password. One or two iterations
        // should be enough.
        // </param>
        // <param name="initVector">
        // Initialization vector (or IV). This value is required to encrypt the
        // first block of plaintext data. For RijndaelManaged class IV must be
        // exactly 16 ASCII characters long.
        // </param>
        // <param name="keySize">
        // Size of encryption key in bits. Allowed values are: 128, 192, and 256.
        // Longer keys are more secure than shorter keys.
        // </param>
        // <returns>
        // Decrypted string value.
        // </returns>
        // <remarks>
        // Most of the logic in this function is similar to the Encrypt
        // logic. In order for decryption to work, all parameters of this function
        // - except cipherText value - must match the corresponding parameters of
        // the Encrypt function which was called to generate the
        // ciphertext.
        // </remarks>

        public string Decrypt(string cipherText)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            // Dim initVectorBytes As Byte()
            // initVectorBytes = Encoding.ASCII.GetBytes(initVector)

            // Dim saltValueBytes As Byte()
            // saltValueBytes = Encoding.ASCII.GetBytes(saltValue)

            // Convert our ciphertext into a byte array.
            var cipherTextBytes = Convert.FromBase64String(cipherText);

            // Define memory stream which will be used to hold encrypted data.
            var memoryStream = new MemoryStream(cipherTextBytes);

            // Define memory stream which will be used to hold encrypted data.
            var cryptoStream = new CryptoStream(memoryStream,
                m_Decryptor,
                CryptoStreamMode.Read);

            // Since at this point we don't know what the size of decrypted data
            // will be, allocate the buffer long enough to hold ciphertext;
            // plaintext is never longer than ciphertext.
            var plainTextBytes = new byte[cipherTextBytes.Length];

            // Start decrypting.
            var decryptedByteCount = cryptoStream.Read(plainTextBytes,
                0,
                plainTextBytes.Length);

            // Close both streams.
            memoryStream.Close();
            cryptoStream.Close();

            // Convert decrypted data into a string.
            // Let us assume that the original plaintext string was UTF8-encoded.
            var plainText = Encoding.UTF8.GetString(plainTextBytes,
                0,
                decryptedByteCount);

            // Return decrypted string.
            return plainText;
        }

        public static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();

            foreach (var b in bytes)
                sb.Append(string.Format("{0:X2}", b));

            return sb.ToString();
        }
    }
}