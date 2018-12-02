using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCoins
{
    public class Block
    {
        public string hash { get; set; }
        public string previoushash { get; set; }
        public string merkleRoot;
        public List<Transaction> transactions = new List<Transaction>(); //our data will be a simple message.
        private string data { get; set; }
        public long timeStamp { get; set; }
        private int nonce;

        public Block(string data,string previousHash)
        {
            this.data = data;
            this.previoushash = previousHash;
            this.timeStamp = Convert.ToInt64(DateTime.Now.ToString("yyyyMMssHHmmssffff"));
            this.hash = calculateHash();
        }

        public Block(string previousHash)
        {
            
            this.previoushash = previousHash;
            this.timeStamp = Convert.ToInt64(DateTime.Now.ToString("yyyyMMssHHmmssffff"));
            this.hash = calculateHash();
        }

        public string calculateHash()
        {
            String calculatehash = StringUtil.applySha256(previoushash + timeStamp.ToString() + data);
            return calculatehash;
        }

        public void mineBlock(int difficulty)
        {
            String target = new String(new char[difficulty]).Replace('\0', '0'); //Create a string with difficulty * "0" 
            while (!hash.Substring(0, difficulty).Equals(target))
            {
                nonce++;
                hash = calculateHash();
            }
           Console.Write("Block Mined!!! : " + hash);
        }

        //Add transactions to this block
        public bool addTransaction(Transaction transaction)
        {
            //process transaction and check if valid, unless block is genesis block then ignore.
            if (transaction == null) return false;
            if ((this.previoushash != "0"))
            {
                if ((transaction.processTransaction() != true))
                {
                    Console.WriteLine("Transaction failed to process. Discarded.");
                    return false;
                }
            }
            transactions.Add(transaction);
            Console.WriteLine("Transaction Successfully added to Block");
            return true;
        }
    }
}
