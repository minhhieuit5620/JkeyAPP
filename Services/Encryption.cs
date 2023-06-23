using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace JKeyApp.Services
{
    public static class Encryption
    {
        public static string keyDescryptDefault = "JITS!@#123";

        #region AES
        static string salt = "ZGgIddJOLDv7l1fH7BCKplG2j4iAnAQ3";
        static readonly byte[] saltAES = Encoding.ASCII.GetBytes("yO3NV1mhssvqWNMbG3atWie5V6PcLlTc");
        static string defaultAESPass = "bNDPWqKCrTFjgyOiDJ47v7Babl4vhpE3oGBrUxqog625mb8vX7p3oOE6G7JLHpMt";
        public static string GenaratePassword(int len, bool onlyDigit = true)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, len)
                .Select(s => s[(new Random()).Next(s.Length)]).ToArray());
        }
        public static string EncryptPasswordPlantext(string loginId, string passwordPlantext)
        {
            return EncryptPassword(loginId, Sha256(passwordPlantext));
        }
        public static string EncryptPassword(string loginId, string password)
        {
            if (password.Length != 64)
                throw new Exception("Password must be encrypted as SHA256");
            string mixSatl = string.Concat(salt.Zip(password.Substring(20, 32), (a, b) => new[] { a, b }).SelectMany(c => c));
            return Sha256((loginId + mixSatl + password).ToLower());
        }
        public static string AESEncrypt(string input)
        {
            return AESEncryptWithPassword(input, defaultAESPass);
        }
        public static string AESEncryptWithPassword(string input, string password)
        {
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.

            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltAES, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedBytes);
        }
        public static string AESDecrypt(string input)
        {
            return AESDecryptWithPassword(input, defaultAESPass);
        }
        public static string AESDecryptWithPassword(string input, string password)
        {
            byte[] bytesToBeDecrypted = Convert.FromBase64String(input);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.

            using (MemoryStream ms = new MemoryStream())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltAES, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }
        #endregion

        #region SHA
        public static string Sha256(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion

        #region smart otp
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GetSmartOTPSecret(string deviceId, string secretKey)
        {
            for (int i = 0; i < deviceId.Length; i++)
            {
                int pos = Convert.ToInt32(i * 3.14);
                while (pos > 256)
                {
                    pos = pos - 256;
                }
                secretKey = secretKey.Insert(pos, deviceId[i].ToString());
            }
            return Sha256(secretKey);
        }
        #endregion
    }
}
