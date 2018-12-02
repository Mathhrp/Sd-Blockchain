using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NoobCoins
{
    public class StringUtil
    {

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

        //Applies ECDSA Signature and returns the result ( as bytes ).
        public static byte[] applyECDSASig(Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey, string input)
        {
            byte[] output = new byte[0];
            try
            {

                byte[] msgBytes = Encoding.UTF8.GetBytes(input);

                ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
                signer.Init(true, privateKey);
                signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
                byte[] sigBytes = signer.GenerateSignature();

                output= sigBytes;
            }
            catch (Exception e)
            {
                throw new Exception("",e);
            }
            return output;
        }

        //Verifies a String signature 
        public static bool verifyECDSASig(Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey, string data, byte[] signature)
        {
            try
            {
               
                byte[] msgBytes = Encoding.UTF8.GetBytes(data);
                byte[] sigBytes = signature;

                ISigner signer = SignerUtilities.GetSigner("SHA-256withECDSA");
                signer.Init(false, publicKey);
                signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
                return signer.VerifySignature(sigBytes);
            }
            catch (Exception e)
            {
                throw new Exception("",e);
            }
        }

        public static string getStringFromKey(Org.BouncyCastle.Crypto.AsymmetricKeyParameter key)
        {
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Convert.ToString(key.GetHashCode())));
        }

        //Tacks in array of transactions and returns a merkle root.
        public static string getMerkleRoot(List<Transaction> transactions)
        {
            int count = transactions.Count;
            List<String> previousTreeLayer = new List<String>();
            foreach (Transaction transaction in transactions)
            {
                previousTreeLayer.Add(transaction.transactionId);
            }
            List<String> treeLayer = previousTreeLayer;
            while (count > 1)
            {
                treeLayer = new List<String>();
                for (int i = 1; i < previousTreeLayer.Count; i++)
                {
                    treeLayer.Add(applySha256(previousTreeLayer[(i - 1)] + previousTreeLayer[i]));
                }
                count = treeLayer.Count;
                previousTreeLayer = treeLayer;
            }
            String merkleRoot = (treeLayer.Count == 1) ? treeLayer[0] : "";
            return merkleRoot;
        }
    }


}
