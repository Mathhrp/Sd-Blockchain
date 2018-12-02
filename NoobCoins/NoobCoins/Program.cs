﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NoobCoins
{
    class Program
    {
        public static List<Block> blockchain = new List<Block>();
        public static int difficulty = 5;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            blockchain.Add(new Block("Hi im the first block", "0"));
            Console.WriteLine("Trying to Mine block 1... ");
            blockchain[0].mineBlock(difficulty);
            blockchain.Add(new Block("Yo im the second block", blockchain[(blockchain.Count - 1)].hash));
            Console.WriteLine("Trying to Mine block 2... ");
            blockchain[1].mineBlock(difficulty);
            blockchain.Add(new Block("Hey im the third block", blockchain[(blockchain.Count - 1)].hash));
            Console.WriteLine("Trying to Mine block 2... ");
            blockchain[2].mineBlock(difficulty);

            Console.WriteLine("\nBlockchain is Valid: " + isChainValid());

            string blockchainJson = JsonConvert.SerializeObject(blockchain);
            Console.WriteLine(blockchainJson);

            Console.Read();
        }

        public static Boolean isChainValid()
        {
            Block currentBlock;
            Block previousBlock;

            //loop through blockchain to check hashes:
            for (int i = 1; i < blockchain.Count; i++)
            {
                currentBlock = blockchain[i];
                previousBlock = blockchain[i - 1];
                //compare registered hash and calculated hash:
                if (!currentBlock.hash.Equals(currentBlock.calculateHash()))
                {
                    Console.Write("Current Hashes not equal");
                    return false;
                }
                //compare previous hash and registered previous hash
                if (!previousBlock.hash.Equals(currentBlock.previoushash))
                {
                    Console.Write("Previous Hashes not equal");
                    return false;
                }
            }
            return true;
        }
    }
}
