using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Collections;

namespace NoobCoins
{
    public class wallet
    {
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey;
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey;
        public static Dictionary<string, TransactionOutput> UTXOs = new Dictionary<string, TransactionOutput>();  //only UTXOs owned by this wallet.

        public wallet()
        {
            generateKeyPair();
        }

        public void generateKeyPair()
        {
            try
            {
                ECKeyPairGenerator gen = new ECKeyPairGenerator("ECDSA");
                SecureRandom secureRandom = new SecureRandom();
                Org.BouncyCastle.Asn1.X9.X9ECParameters ecp = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp224k1");
                ECDomainParameters ecSpec = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                ECKeyGenerationParameters ecgp = new ECKeyGenerationParameters(ecSpec, secureRandom);
                gen.Init(ecgp);
                AsymmetricCipherKeyPair eckp = gen.GenerateKeyPair();
                this.privateKey = eckp.Private;
                this.publicKey = eckp.Public;
            }
            catch (Exception e)
            {
                throw new Exception("", e);
            }

        }
        //returns balance and stores the UTXO's owned by this wallet in this.UTXOs
        public float getBalance()
        {
            float total = 0;
            foreach (KeyValuePair<string, TransactionOutput> item in Program.UTXOs)
            {
                TransactionOutput UTXO = item.Value;
                if (UTXO.isMine(publicKey))
                { //if output belongs to me ( if coins belong to me )
                    if(!UTXOs.ContainsKey(UTXO.id))
                        UTXOs.Add(UTXO.id, UTXO); //add it to our list of unspent transactions.
                    total += UTXO.value;
                }
            }
            return total;
        }
        //Generates and returns a new transaction from this wallet.
        public Transaction sendFunds(Org.BouncyCastle.Crypto.AsymmetricKeyParameter _recipient, float value)
        {
            if (getBalance() < value)
            { //gather balance and check funds.
                Console.WriteLine("#Not Enough funds to send transaction. Transaction Discarded.");
                return null;
            }
            //create array list of inputs
            List<TransactionInput> inputs = new List<TransactionInput>();

            float total = 0;
            foreach (KeyValuePair<string, TransactionOutput> item in UTXOs)
            {
                TransactionOutput UTXO = item.Value;
                total += UTXO.value;
                inputs.Add(new TransactionInput(UTXO.id));
                if (total > value) break;
            }

            Transaction newTransaction = new Transaction(publicKey, _recipient, value, inputs);
            newTransaction.generateSignature(privateKey);

            foreach (TransactionInput input in inputs)
            {
                UTXOs.Remove(input.transactionOutputId);
            }
            return newTransaction;
        }
    }
}
