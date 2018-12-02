namespace NoobCoins
{
    public class TransactionInput
    {
        public string transactionOutputId; //Reference to TransactionOutputs -> transactionId
        public TransactionOutput UTXO; //Contains the Unspent transaction output

        public TransactionInput(string transactionOutputId)
        {
            this.transactionOutputId = transactionOutputId;
        }
    }
}