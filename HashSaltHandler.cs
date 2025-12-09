using System;
using System.Text;
using System.Security.Cryptography;

namespace UserLogin
{
    public class HashSaltUtil
    {
        private const int _keySize = 64;//hash and salt are typically stored in base64 encoded string
        private const int _iterations = 350000;
        private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512; //non primitive data types need readonly to remain constant

        ///<summary>
        ///Create hash and salt for the user password input
        ///</summary>
        ///<param name="password">string for the password input</param>
        ///<returns>tuple of strings for the hash and salt</returns>
        public static (string hash, string salt) HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(_keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, _iterations, _hashAlgorithm, _keySize);

            return (Convert.ToHexString(hash), Convert.ToHexString(salt));//returns salt and hash as a string
        }

        ///<summary>
        ///Compares the stored hash and salt with the password input
        ///</summary>
        ///<param name="password">string for the password input</param>
        ///<param name="storedHash">string of the stored hash</param>
        ///<param name="storedSalt">string for the stored salt</param>
        ///<returns>boolean if the password hash input matches with the stored hash and salt returns true else returns false</returns>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromHexString(storedSalt);//reverts the stored salt to bytes
            var hashToVerify = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), saltBytes, _iterations, _hashAlgorithm, _keySize);
            var storedHashBytes = Convert.FromHexString(storedHash);//reverts the stored hash to bytes

            return CryptographicOperations.FixedTimeEquals(hashToVerify, storedHashBytes);
        }   
    }
}