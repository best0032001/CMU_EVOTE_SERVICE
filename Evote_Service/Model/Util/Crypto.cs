using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Evote_Service.Model.Util
{
    public class Crypto
    {
        private ICryptoTransform rijndaelDecryptor;
        // Replace me with a 16-byte key, share between Java and C#
        private byte[] rawSecretKey;

        /// <summary>
        /// rawSecretKey values
        /// </summary>

        private byte[] passwordKey;
        /// <summary>
        /// variable field
        /// </summary>


        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptDecrypt"/> class.
        /// </summary>
        /// <param name="passphrase">pass phrase</param>
        public Crypto(string passphrase,String _rawSecretKey)
        {
            rawSecretKey= Encoding.ASCII.GetBytes(_rawSecretKey);
            passwordKey = EncodeDigest(passphrase);
            RijndaelManaged rijndael = new RijndaelManaged();
            //rijndael.Padding = PaddingMode.None;
            this.rijndaelDecryptor = rijndael.CreateDecryptor(passwordKey, rawSecretKey);
        }


        private byte[] EncodeDigest(string text)
        {
            SHA256CryptoServiceProvider x = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            //MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(text);
            return x.ComputeHash(data);
        }


        public string Encrypt(string plainText)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {

                byte[] encrypted = EncryptStringToBytes(plainText, passwordKey, rawSecretKey);
                return Convert.ToBase64String(encrypted);
            }
        }


        public byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iV)
        {
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            if (iV == null || iV.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            byte[] encrypted;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = key;
                rijAlg.IV = iV;
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                using (MemoryStream memorystreamEncrypt = new MemoryStream())
                {
                    using (CryptoStream cryptostreamEncrypt = new CryptoStream(memorystreamEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamwriterEncrypt = new StreamWriter(cryptostreamEncrypt))
                        {
                            streamwriterEncrypt.Write(plainText);
                        }

                        encrypted = memorystreamEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }


        public string Decrypt(byte[] encryptedData)
        {
            byte[] newClearData = this.rijndaelDecryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.ASCII.GetString(newClearData);
        }


        public string DecryptFromBase64(string encryptedBase64)
        {
            return this.Decrypt(Convert.FromBase64String(encryptedBase64));
        }
    }
}
