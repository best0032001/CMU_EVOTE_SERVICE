using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Evote_Service.Model.Util
{
    public class VoteEnCrypt
    {
        public async Task<String> EnCrypt(String bodyText, RSA rsa)
        {

            byte[] data = Encoding.UTF8.GetBytes(bodyText);
            byte[] cipherText = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
            String converText = Convert.ToBase64String(cipherText);
            String cypherText = converText;
            return cypherText;
        }

        public async Task<String> DeCrypt(String cypherText, RSA rsa)
        {
            byte[] data = Convert.FromBase64String(cypherText);
            byte[] cipherText = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
            String decodeText = Encoding.UTF8.GetString(cipherText);
            String deCryptText = decodeText;
            return deCryptText;
        }
    }
}
