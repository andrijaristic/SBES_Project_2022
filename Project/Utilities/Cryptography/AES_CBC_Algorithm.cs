using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Cryptography
{
    public class AES_CBC_Algorithm
    {
        public static byte[] EncryptString(string inputString, string secretKey)
        {           
            byte[] encryptedString = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(secretKey);

                aesAlg.GenerateIV();
                
                aesAlg.Mode = CipherMode.CBC;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
        
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {                            
                            swEncrypt.Write(inputString);
                        }
                        encryptedString = aesAlg.IV.Concat(msEncrypt.ToArray()).ToArray();
                        /*
                        var combinedIvCt = new byte[IV.Length + encrypted.Length];
                        Array.Copy(IV, 0, combinedIvCt, 0, IV.Length);
                        Array.Copy(encrypted, 0, combinedIvCt, IV.Length, encrypted.Length); 
                         */
                    }
                }
            }
            return encryptedString;
        }

        public static string DecryptString(byte[] inputString, string secretKey)
        {                           
            string decryptedString = null;           

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(secretKey);

                byte[] IV = new byte[aesAlg.BlockSize / 8];
                byte[] cipherText = new byte[inputString.Length - IV.Length];

                Array.Copy(inputString, IV, IV.Length);
                Array.Copy(inputString, IV.Length, cipherText, 0, cipherText.Length);

                aesAlg.IV = IV;

                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {                        
                            decryptedString = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return decryptedString;
        }
    }
}
