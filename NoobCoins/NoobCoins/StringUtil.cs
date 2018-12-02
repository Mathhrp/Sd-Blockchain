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
            }
            catch { }
            return "";
        }
    }
}
