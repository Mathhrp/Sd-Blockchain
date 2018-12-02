using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCoins
{
    public class Block
    {
        public string hash { get; set; }
        public string previoushash { get; set; }
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
    }
}
