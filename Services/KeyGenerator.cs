using System;
using System.Security.Cryptography;
using System.Text;

namespace B6CRM.Services
{
    public class KeyGenerator
    {
        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GetUniqueKey(int size = 20)
        {
            /* byte[] data = new byte[4 * size];
             using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
             {
                 crypto.GetBytes(data);
             }*/
            byte[] data = RandomNumberGenerator.GetBytes(80);
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string GetUniqueKeyOriginal_BIASED(int size = 20)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = RandomNumberGenerator.GetBytes(size);
            
            /*using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }*/
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % chars.Length]);
            }
            return result.ToString();
        }
    }

}
