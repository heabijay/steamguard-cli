using System;
using System.Security.Cryptography;

namespace SteamGuard.Helpers
{
    public static class CryptographyHelper
    {
        private const int PBKDF2_ITERATIONS = 50000; //Set to 50k to make program not unbearably slow. May increase in future.
        private const int SALT_LENGTH = 8;
        private const int KEY_SIZE_BYTES = 32;
        private const int IV_LENGTH = 16;

        /// <summary>
        /// Returns an 8-byte cryptographically random salt in base64 encoding
        /// </summary>
        /// <returns></returns>
        public static string GetRandomSalt()
        {
            byte[] salt = new byte[SALT_LENGTH];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Returns a 16-byte cryptographically random initialization vector (IV) in base64 encoding
        /// </summary>
        /// <returns></returns>
        public static string GetInitializationVector()
        {
            byte[] IV = new byte[IV_LENGTH];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(IV);
            }
            return Convert.ToBase64String(IV);
        }

        /// <summary>
        /// Generates an encryption key derived using a password, a random salt, and specified number of rounds of PBKDF2
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static byte[] GetEncryptionKey(string password, string salt)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException(nameof(password));
            }
            if (string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException(nameof(salt));
            }
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), PBKDF2_ITERATIONS))
            {
                return pbkdf2.GetBytes(KEY_SIZE_BYTES);
            }
        }
    }
}
