using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NoobCoins
{
    public class Transaction
    {

        public String transactionId; // this is also the hash of the transaction.
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter sender; // senders address/public key.
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter reciepient; // Recipients address/public key.
        public float value;
        public byte[] signature; // this is to prevent anybody else from spending funds in our wallet.

        public List<TransactionInput> inputs = new List<TransactionInput>();
        public List<TransactionOutput> outputs = new List<TransactionOutput>();

        private static int sequence = 0; // a rough count of how many transactions have been generated. 

        // Constructor: 
        public Transaction(Org.BouncyCastle.Crypto.AsymmetricKeyParameter from, Org.BouncyCastle.Crypto.AsymmetricKeyParameter to, float value, List<TransactionInput> inputs)
        {
            this.sender = from;
            this.reciepient = to;
            this.value = value;
            this.inputs = inputs;
        }

        // This Calculates the transaction hash (which will be used as its Id)
        private String calulateHash()
        {
            sequence++; //increase the sequence to avoid 2 identical transactions having the same hash
            return StringUtil.applySha256(
                    StringUtil.getStringFromKey(sender) +
                    StringUtil.getStringFromKey(reciepient) +
                    Convert.ToString(value) + sequence
                    );
        }

        //Signs all the data we dont wish to be tampered with.
        public void generateSignature(Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey)
        {
            String data = StringUtil.getStringFromKey(sender) + StringUtil.getStringFromKey(reciepient) + Convert.ToString(value);
            signature = StringUtil.applyECDSASig(privateKey, data);
        }
        //Verifies the data we signed hasnt been tampered with
        public bool verifiySignature()
        {
            String data = StringUtil.getStringFromKey(sender) + StringUtil.getStringFromKey(reciepient) + Convert.ToString(value);
            return StringUtil.verifyECDSASig(sender, data, signature);
        }
    }
}
