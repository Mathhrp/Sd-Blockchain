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
        //Returns true if new transaction could be created.	
        public bool processTransaction()
        {

            if (verifiySignature() == false)
            {
                Console.WriteLine("#Transaction Signature failed to verify");
                return false;
            }

            //gather transaction inputs (Make sure they are unspent):
            foreach (TransactionInput i in inputs)
            {
                Program.UTXOs.TryGetValue(i.transactionOutputId,out i.UTXO);
            }

            //check if transaction is valid:
            if (getInputsValue() < Program.minimumTransaction)
            {
                Console.WriteLine("#Transaction Inputs to small: " + getInputsValue());
                return false;
            }

            //generate transaction outputs:
            float leftOver = getInputsValue() - value; //get value of inputs then the left over change:
            transactionId = calulateHash();
            outputs.Add(new TransactionOutput(this.reciepient, value, transactionId)); //send value to recipient
            outputs.Add(new TransactionOutput(this.sender, leftOver, transactionId)); //send the left over 'change' back to sender		

            //add outputs to Unspent list
            foreach (TransactionOutput o in outputs)
            {
                Program.UTXOs.Add(o.id, o);
            }

            //remove transaction inputs from UTXO lists as spent:
            foreach (TransactionInput i in inputs)
            {
                if (i.UTXO == null) continue; //if Transaction can't be found skip it 
                Program.UTXOs.Remove(i.UTXO.id);
            }

            return true;
        }

        //returns sum of inputs(UTXOs) values
        public float getInputsValue()
        {
            float total = 0;
            foreach (TransactionInput i in inputs)
            {
                if (i.UTXO == null) continue; //if Transaction can't be found skip it 
                total += i.UTXO.value;
            }
            return total;
        }

        //returns sum of outputs:
        public float getOutputsValue()
        {
            float total = 0;
            foreach (TransactionOutput o in outputs)
            {
                total += o.value;
            }
            return total;
        }

    }
}
