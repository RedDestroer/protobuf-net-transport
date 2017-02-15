using System;
using System.Security.Cryptography;

namespace ProtoBuf.Transport
{
    public class RsaKeyGenerator
    {
        public string Generate()
        {
            using (var rsaAlg = new RSACryptoServiceProvider())
            {
                return rsaAlg.ToXmlString(true);
            }
        }

        public string GetPublicKey(string keyPair)
        {
            if (keyPair == null) throw new ArgumentNullException("keyPair");

            using (var rsaAlg = RsaSignAlgorithm.GetProviderFromKey(keyPair))
            {
                return rsaAlg.ToXmlString(false);
            }
        }
    }
}