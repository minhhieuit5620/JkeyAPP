using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Services
{
    public class InMemoryKey : IKeyProvider
    {
        static readonly object platformSupportSync = new object();

        readonly object stateSync = new object();
        readonly byte[] KeyData;
        readonly int keyLength;

        /// <summary>
        /// Creates an instance of a key.
        /// </summary>
        /// <param name="key">Plaintext key data</param>
        public InMemoryKey(byte[] key)
        {
            if (!(key != null))
                throw new ArgumentNullException("key");
            if (!(key.Length > 0))
                throw new ArgumentException("The key must not be empty");

            this.keyLength = key.Length;
            int paddedKeyLength = (int)Math.Ceiling((decimal)key.Length / (decimal)16) * 16;
            this.KeyData = new byte[paddedKeyLength];
            Array.Copy(key, this.KeyData, key.Length);
        }

        /// <summary>
        /// Gets a copy of the plaintext key
        /// </summary>
        /// <remarks>
        /// This is internal rather than protected so that the tests can use this method
        /// </remarks>
        /// <returns>Plaintext Key</returns>
        internal byte[] GetCopyOfKey()
        {
            var plainKey = new byte[this.keyLength];
            lock (this.stateSync)
            {
                Array.Copy(this.KeyData, plainKey, this.keyLength);
            }
            return plainKey;
        }

        /// <summary>
        /// Uses the key to get an HMAC using the specified algorithm and data
        /// </summary>
        /// <param name="mode">The HMAC algorithm to use</param>
        /// <param name="data">The data used to compute the HMAC</param>
        /// <returns>HMAC of the key and data</returns>
        public byte[] ComputeHmac(OtpHashMode mode, byte[] data)
        {
            byte[] hashedValue = null;
            using (HMAC hmac = CreateHmacHash(mode))
            {
                byte[] key = this.GetCopyOfKey();
                try
                {
                    hmac.Key = key;
                    hashedValue = hmac.ComputeHash(data);
                }
                finally
                {
                    KeyUtilities.Destroy(key);
                }
            }

            return hashedValue;
        }

        /// <summary>
        /// Create an HMAC object for the specified algorithm
        /// </summary>
        private static HMAC CreateHmacHash(OtpHashMode otpHashMode)
        {
            HMAC hmacAlgorithm = null;
            switch (otpHashMode)
            {
                case OtpHashMode.Sha256:
                    hmacAlgorithm = new HMACSHA256();
                    break;
                case OtpHashMode.Sha512:
                    hmacAlgorithm = new HMACSHA512();
                    break;
                default: //case OtpHashMode.Sha1:
                    hmacAlgorithm = new HMACSHA1();
                    break;
            }
            return hmacAlgorithm;
        }
    }
}
