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

        public Block(string data,string previousHash)
        {
            this.data = data;
            this.previoushash = previousHash;
            this.timeStamp = Convert.ToInt64(DateTime.Now.ToString("yyyyMMssHHmmssffff"));
        }
    }
}
