using System;
using System.Security.Cryptography;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Generator for RSA keys
    /// </summary>
    public static class RsaKeyGenerator
    {
        /// <summary>
        /// Generates next RSA key pair
        /// </summary>
        /// <returns></returns>
        public static string Generate()
        {
            using (var rsaAlg = new RSACryptoServiceProvider())
            {
                return rsaAlg.ToXmlString(true);
            }
        }

        /// <summary>
        /// Gets public key from key pair string
        /// </summary>
        /// <param name="keyPair">Key pair</param>
        /// <returns>Returns only public part of key pair</returns>
        public static string GetPublicKey(string keyPair)
        {
            if (keyPair == null) throw new ArgumentNullException("keyPair");

            using (var rsaAlg = RsaSignAlgorithm.GetProviderFromKey(keyPair))
            {
                return rsaAlg.ToXmlString(false);
            }
        }
    }
}