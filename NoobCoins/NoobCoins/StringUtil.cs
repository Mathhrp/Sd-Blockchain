using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NoobCoins
{
    public class StringUtil
    {
        public static object Enconding { get; private set; }

        public static string applySha256(string imput)
        {
            try
            {
                HashAlgorithm sha = new SHA1CryptoServiceProvider();
                byte[] has = sha.ComputeHash(Encoding.ASCII.GetBytes(imput));
                StringBuilder hexString = new StringBuilder();
                for(int i=0;i< has.Length;i++)
                {
                    string hex = has[i].ToString("X4");
                    if (hex.Length == 1) hexString.Append('0');
                    hexString.Append(hex);
                }
                return hexString.ToString();
            }
            catch (Exception e)
            {
                throw new Exception("erro",e);
            }
        }
    }
}
