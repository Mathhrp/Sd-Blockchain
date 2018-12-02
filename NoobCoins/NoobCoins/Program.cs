using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NoobCoins
{
    class Program
    {
        public static List<Block> blockchain = new List<Block>();
        public static int difficulty = 2;
        public static wallet walletA;
        public static wallet walletB;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            walletA = new wallet();
            walletB = new wallet();
            //Test public and private keys
            Console.WriteLine("Private and public keys:");
            Console.WriteLine(StringUtil.getStringFromKey(walletA.privateKey));
            Console.WriteLine(StringUtil.getStringFromKey(walletA.publicKey));
            //Create a test transaction from WalletA to walletB 
            Transaction transaction = new Transaction(walletA.publicKey, walletB.publicKey, 5, null);
            transaction.generateSignature(walletA.privateKey);
            //Verify the signature works and verify it from the public key
            Console.WriteLine("Is signature verified");
            Console.WriteLine(transaction.verifiySignature());

            /* blockchain.Add(new Block("Hi im the first block", "0"));
             Console.WriteLine("Trying to Mine block 1... ");
             blockchain[0].mineBlock(difficulty);
             //for (int i = 0; ; i++)
             //{

             //    blockchain.Add(new Block("Yo im the second block", blockchain[(blockchain.Count - 1)].hash));
             //    Console.WriteLine($"Trying to Mine block {i}... ");
             //    blockchain[i].mineBlock(difficulty);

             //    Console.WriteLine("\nBlockchain is Valid: " + isChainValid());

             //    string blockchainJson = JsonConvert.SerializeObject(blockchain);
             //    Console.WriteLine(blockchainJson);
             //}*/
            Console.ReadKey();

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
