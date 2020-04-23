using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MTC.JMICS.Utility.Utils
{
    public class EncryptorDecryptorEngine
    {
        public static string password = @"OfCourage,BlessedByGod,TreadOnTheWaves";
        //public static string EncryptString(string Message, string Passphrase)


        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="stringToEncrypt">string that is required to encrypt.</param>
        /// <param name="encryptedString">Encrypted string</param>
        /// <returns>True if Sucessful else False</returns>
        public static bool EncryptString(string stringToEncrypt, out string encryptedString, out string errMsg)
        {
            bool returnVal = false;
            encryptedString = "";
            errMsg = "";
            string Passphrase = password;

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();


            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            try
            {

                // Step 4. Convert the input string to a byte[]
                byte[] DataToEncrypt = UTF8.GetBytes(stringToEncrypt);

                // Step 5. Attempt to encrypt the string

                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);

                encryptedString = Convert.ToBase64String(Results);
                returnVal = true;
            }
            catch (Exception Ex)
            {
                errMsg = "Can't Encrypt a string " + Ex.Message;
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            return returnVal;

        }

        public static string EncryptString(string stringToEncrypt, String strKey)
        {
            //bool returnVal = false;
            string encryptedString = string.Empty;

            //string Passphrase = password;
            string Passphrase = strKey;

            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();


            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            try
            {

                // Step 4. Convert the input string to a byte[]
                byte[] DataToEncrypt = UTF8.GetBytes(stringToEncrypt);

                // Step 5. Attempt to encrypt the string
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);

                encryptedString = Convert.ToBase64String(Results);
                //returnVal = true;
            }
            catch (Exception Ex)
            {
                //errMsg = "Can't Encrypt a string " + Ex.Message;
                throw Ex;
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            return encryptedString;

        }

        /// <summary>
        /// Decrypt a string
        /// </summary>
        /// <param name="encryptedString">Encrypted String that is required to decrypt</param>
        /// <param name="originalString">Decrypted String</param>
        /// <returns>True if Sucessful else False</returns>
        public static bool DecryptString(string encryptedString, out string originalString, out string errMsg)
        {

            bool returnVal = false;
            originalString = "";
            errMsg = "";
            string Passphrase = password;
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            try
            {

                // Step 4. Convert the input string to a byte[]
                byte[] DataToDecrypt = Convert.FromBase64String(encryptedString);

                // Step 5. Attempt to decrypt the string

                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);

                // Step 6. Return the decrypted string in UTF8 format
                originalString = UTF8.GetString(Results);
                returnVal = true;

            }
            catch (Exception Ex)
            {
                errMsg = "Can't Decrypt a string " + Ex.Message;
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }


            return returnVal;
        }

        public static string DecryptString(string encryptedString, String strKey)
        {

            //bool returnVal = false;
            string originalString = string.Empty;

            //string Passphrase = password;
            string Passphrase = strKey;


            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the decoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            try
            {

                // Step 4. Convert the input string to a byte[]
                byte[] DataToDecrypt = Convert.FromBase64String(encryptedString);

                // Step 5. Attempt to decrypt the string

                ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);

                // Step 6. Return the decrypted string in UTF8 format
                originalString = UTF8.GetString(Results);
                //returnVal = true;

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }


            return originalString;
        }

        public static string EncodeBase64(string inputString)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(inputString);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string DecodeBase64(string encodedString)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedString);
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }


        public static string CreateSalt()
        {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] number = new byte[32];
            rng.GetBytes(number);
            return Convert.ToBase64String(number);
        }

        public static string CreateHash(string value, string salt, HashType hashType)
        {
            string hashString = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                HashAlgorithm hashAlg = HashAlgorithm.Create(hashType.ToString());
                byte[] pwordData = Encoding.Default.GetBytes(salt + value);
                byte[] hash = hashAlg.ComputeHash(pwordData);
                hashString = Convert.ToBase64String(hash);
            }
            return hashString;
        }

        public static bool ConfirmHash(string plainValue, string hashedValue, string salt, HashType hashType)
        {
            string passwordHash = CreateHash(plainValue, salt, hashType);
            return Convert.ToBoolean(String.Compare(hashedValue, passwordHash, true) == 0 ? "true" : "false");
        }
    }
}
