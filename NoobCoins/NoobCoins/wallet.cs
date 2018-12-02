using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace NoobCoins
{
    public class wallet
    {
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter privateKey;
        public Org.BouncyCastle.Crypto.AsymmetricKeyParameter publicKey;

        public wallet()
        {
            generateKeyPair();
        }

        public void generateKeyPair()
        {
            try
            {
                ECKeyPairGenerator gen = new ECKeyPairGenerator("ECDSA");
                SecureRandom secureRandom = new SecureRandom();
                Org.BouncyCastle.Asn1.X9.X9ECParameters ecp = Org.BouncyCastle.Asn1.Sec.SecNamedCurves.GetByName("secp224k1");
                ECDomainParameters ecSpec = new ECDomainParameters(ecp.Curve, ecp.G, ecp.N, ecp.H, ecp.GetSeed());
                ECKeyGenerationParameters ecgp = new ECKeyGenerationParameters(ecSpec, secureRandom);
                gen.Init(ecgp);
                AsymmetricCipherKeyPair eckp = gen.GenerateKeyPair();
                this.privateKey = eckp.Private;
                this.publicKey = eckp.Public;
            }
            catch (Exception e)
            {
                throw new Exception("",e);
            }
        }
    }
}
