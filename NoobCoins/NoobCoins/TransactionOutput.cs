using System;

namespace NoobCoins
{
    public class TransactionOutput
    {
        public string id;
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter reciepient; //also known as the new owner of these coins.
        public float value; //the amount of coins they own
        public string parentTransactionId; //the id of the transaction this output was created in

        //Constructor
        public TransactionOutput(Org.BouncyCastle.Crypto.AsymmetricKeyParameter reciepient, float value, string parentTransactionId)
        {
            this.reciepient = reciepient;
            this.value = value;
            this.parentTransactionId = parentTransactionId;
            this.id = StringUtil.applySha256(StringUtil.getStringFromKey(reciepient) + Convert.ToString(value) + parentTransactionId);
        }

        //Check if coin belongs to you
        public bool isMine(Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey)
        {
            return (publicKey == reciepient);
        }

    }
}