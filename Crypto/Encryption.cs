using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Crypto
{
    /// <summary>
    /// Class to manage encryption and decryption of string.
    /// </summary>
    public static class Encryption
    {
        /// <summary>
        /// Encryption level.
        /// </summary>
        private const uint EncryptionLevel = 1;

        /// <summary>
        /// Method to encrypt string.
        /// </summary>
        /// <param name="stringToEncode">String to encrypt.</param>
        /// <param name="encryptionLevel">A positive integer which is used to encode the string, default value is 1.</param>
        /// <returns>Encrypted string.</returns>
        public static string Encode(string stringToEncode, uint encryptionLevel = EncryptionLevel)
        {
            string encodedString = stringToEncode;
            for (int i = 0; i < encryptionLevel; i++)
            {
                encodedString = _Encode(encodedString);
            }
            return encodedString;
        }

        /// <summary>
        /// Method to decrypt string
        /// </summary>
        /// <param name="stringToDecode">String to decrypt.</param>
        /// <param name="encryptionLevel">A positive integer which is used to decode the string, default value is 1.</param>
        /// <returns>Decrypted string.</returns>
        public static string Decode(string stringToDecode, uint encryptionLevel = EncryptionLevel)
        {
            string decodedString = stringToDecode;
            for (int i = 0; i < encryptionLevel; i++)
            {
                decodedString = _Decode(decodedString);
            }
            return decodedString;
        }

        /// <summary>
        /// Method to perform encryption.
        /// </summary>
        /// <param name="stringToEncode">String to encrypt.</param>
        /// <returns>Encrypted string.</returns>
        private static string _Encode(string stringToEncode)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(stringToEncode);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        /// <summary>
        /// Method to perform decryption.
        /// </summary>
        /// <param name="stringToDecode">String to decrypt.</param>
        /// <returns>Decrypted string.</returns>
        private static string _Decode(string stringToDecode)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(stringToDecode);
            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }
    }
}