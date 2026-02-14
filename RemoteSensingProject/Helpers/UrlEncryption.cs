using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RemoteSensingProject.Helpers
{
    public static class UrlEncryption
    {
        private static readonly string key = "MacreelInf784280";
        private static readonly string iv = "082487fnIleercaM";

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encrypted)
                        .Replace("+", "-")
                        .Replace("/", "_")
                        .Replace("=", "");
                }
            }
        }


        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace("-", "+").Replace("_", "/");
            switch(cipherText.Length % 4)
            {
                case 2: cipherText += "=="; break;
                case 3: cipherText += "="; break;
            };

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes (iv);

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    byte[] decypted = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                    return Encoding.UTF8.GetString(decypted);
                }
            }
        }
    }
}