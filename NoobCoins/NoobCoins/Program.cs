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
        public static Dictionary<string, TransactionOutput> UTXOs = new Dictionary<string, TransactionOutput>(); //list of all unspent transactions.
        public static wallet walletA;
        public static wallet walletB;
        internal static float minimumTransaction = 0.1f;
        public static Transaction genesisTransaction;

        static void Main(string[] args)
        {


            walletA = new wallet();
            walletB = new wallet();
            wallet coinbase = new wallet();

            //create genesis transaction, which sends 100 NoobCoin to walletA: 
            genesisTransaction = new Transaction(coinbase.publicKey, walletA.publicKey, 100f, null);
            genesisTransaction.generateSignature(coinbase.privateKey);   //manually sign the genesis transaction	
            genesisTransaction.transactionId = "0"; //manually set the transaction id
            genesisTransaction.outputs.Add(new TransactionOutput(genesisTransaction.reciepient, genesisTransaction.value, genesisTransaction.transactionId)); //manually add the Transactions Output
            UTXOs.Add(genesisTransaction.outputs[0].id, genesisTransaction.outputs[0]); //its important to store our first transaction in the UTXOs list.

            Console.WriteLine("Creating and Mining Genesis block... ");
            Block genesis = new Block("0");
            genesis.addTransaction(genesisTransaction);
            addBlock(genesis);

            //testing
            Block block1 = new Block(genesis.hash);
            Console.WriteLine("\nWalletA's balance is: " + walletA.getBalance());
            Console.WriteLine("\nWalletA is Attempting to send funds (40) to WalletB...");
            block1.addTransaction(walletA.sendFunds(walletB.publicKey, 40f));
            addBlock(block1);
            Console.WriteLine("\nWalletA's balance is: " + walletA.getBalance());
            Console.WriteLine("WalletB's balance is: " + walletB.getBalance());

            Block block2 = new Block(block1.hash);
            Console.WriteLine("\nWalletA Attempting to send more funds (1000) than it has...");
            block2.addTransaction(walletA.sendFunds(walletB.publicKey, 1000f));
            addBlock(block2);
            Console.WriteLine("\nWalletA's balance is: " + walletA.getBalance());
            Console.WriteLine("WalletB's balance is: " + walletB.getBalance());

            Block block3 = new Block(block2.hash);
            Console.WriteLine("\nWalletB is Attempting to send funds (20) to WalletA...");
            block3.addTransaction(walletB.sendFunds(walletA.publicKey, 20));
            Console.WriteLine("\nWalletA's balance is: " + walletA.getBalance());
            Console.WriteLine("WalletB's balance is: " + walletB.getBalance());

            isChainValid();

            Console.ReadKey();

        }

        public static Boolean isChainValid()
        {
            Block currentBlock;
            Block previousBlock;
            String hashTarget = new String(new char[difficulty]).Replace('\0', '0');
            Dictionary<string, TransactionOutput> tempUTXOs = new Dictionary<string, TransactionOutput>(); //a temporary working list of unspent transactions at a given block state.
            tempUTXOs.Add(genesisTransaction.outputs[(0)].id, genesisTransaction.outputs[(0)]);

            //loop through blockchain to check hashes:
            for (int i = 1; i < blockchain.Count; i++)
            {

                currentBlock = blockchain[i];
                previousBlock = blockchain[i - 1];
                //compare registered hash and calculated hash:
                if (!currentBlock.hash.Equals(currentBlock.calculateHash()))
                {
                    Console.WriteLine("#Current Hashes not equal");
                    return false;
                }
                //compare previous hash and registered previous hash
                if (!previousBlock.hash.Equals(currentBlock.previoushash))
                {
                    Console.WriteLine("#Previous Hashes not equal");
                    return false;
                }
                //check if hash is solved
                if (!currentBlock.hash.Substring(0, difficulty).Equals(hashTarget))
                {
                    Console.WriteLine("#This block hasn't been mined");
                    return false;
                }

                //loop thru blockchains transactions:
                TransactionOutput tempOutput;
                for (int t = 0; t < currentBlock.transactions.Count; t++)
                {
                    Transaction currentTransaction = currentBlock.transactions[t];

                    if (!currentTransaction.verifiySignature())
                    {
                        Console.WriteLine("#Signature on Transaction(" + t + ") is Invalid");
                        return false;
                    }
                    if (currentTransaction.getInputsValue() != currentTransaction.getOutputsValue())
                    {
                        Console.WriteLine("#Inputs are note equal to outputs on Transaction(" + t + ")");
                        return false;
                    }

                    foreach (TransactionInput input in currentTransaction.inputs)
                    {
                        tempOutput = tempUTXOs[input.transactionOutputId];

                        if (tempOutput == null)
                        {
                            Console.WriteLine("#Referenced input on Transaction(" + t + ") is Missing");
                            return false;
                        }

                        if (input.UTXO.value != tempOutput.value)
                        {
                            Console.WriteLine("#Referenced input Transaction(" + t + ") value is Invalid");
                            return false;
                        }

                        tempUTXOs.Remove(input.transactionOutputId);
                    }

                    foreach (TransactionOutput output in currentTransaction.outputs)
                    {
                        tempUTXOs.Add(output.id, output);
                    }

                    if (currentTransaction.outputs[0].reciepient != currentTransaction.reciepient)
                    {
                        Console.WriteLine("#Transaction(" + t + ") output reciepient is not who it should be");
                        return false;
                    }
                    if (currentTransaction.outputs[1].reciepient != currentTransaction.sender)
                    {
                        Console.WriteLine("#Transaction(" + t + ") output 'change' is not sender.");
                        return false;
                    }

                }

            }
            Console.WriteLine("Blockchain is valid");
            return true;
        }

        public static void addBlock(Block newBlock)
        {
            newBlock.mineBlock(difficulty);
            blockchain.Add(newBlock);
        }
    }
}
