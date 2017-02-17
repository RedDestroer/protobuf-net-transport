using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public class RsaSignAlgorithm
        : ISignAlgorithm
    {
        private const string HashAlgName = "SHA256";
        private readonly string _keyPairOrPublicKey;

        public RsaSignAlgorithm(string keyPairOrPublicKey)
        {
            if (keyPairOrPublicKey == null) throw new ArgumentNullException("keyPairOrPublicKey");

            _keyPairOrPublicKey = keyPairOrPublicKey;
        }

        public static RSACryptoServiceProvider GetProviderFromKey(string keyPair)
        {
            if (keyPair == null) throw new ArgumentNullException("keyPair");

            var rsaAlg = new RSACryptoServiceProvider();
            rsaAlg.FromXmlString(keyPair);

            return rsaAlg;
        }

        public byte[] GetSign(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            string hashString = ComputeHashMd5Hash(stream);

            return GetSignedHash(hashString);
        }

        public bool VerifySign(Stream stream, byte[] signBytes)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (signBytes == null) throw new ArgumentNullException("signBytes");

            string md5Hash = ComputeHashMd5Hash(stream);
            byte[] hashBytes = Encoding.UTF8.GetBytes(md5Hash);

            return VerifySign(hashBytes, signBytes);
        }

        public bool VerifySign(Stream stream, Stream signStream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (signStream == null) throw new ArgumentNullException("signStream");

            string md5Hash = ComputeHashMd5Hash(stream);

            return VerifySign(md5Hash, signStream);
        }

        public bool VerifySign(string md5Hash, Stream signStream)
        {
            if (md5Hash == null) throw new ArgumentNullException("md5Hash");
            if (signStream == null) throw new ArgumentNullException("signStream");

            byte[] hashBytes = Encoding.UTF8.GetBytes(md5Hash);
            byte[] signBytes = new byte[signStream.Length];
            signStream.Seek(0, SeekOrigin.Begin);
            
            signStream.Read(signBytes, 0, (int)signStream.Length);

            return VerifySign(hashBytes, signBytes);
        }

        private bool VerifySign(byte[] hashBytes, byte[] signBytes)
        {
            using (RSACryptoServiceProvider rsaAlg = GetProviderFromKey(_keyPairOrPublicKey))
            {
                var deformatter = new RSAPKCS1SignatureDeformatter(rsaAlg);
                deformatter.SetHashAlgorithm(HashAlgName);

                return deformatter.VerifySignature(hashBytes, signBytes);
            }
        }

        private string ComputeHashMd5Hash(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            
            byte[] md5Hash;
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                md5Hash = md5.ComputeHash(stream);
            }

            var answer = new StringBuilder();
            for (int i = 0; i < md5Hash.Length; i++)
            {
                answer.Append(md5Hash[i].ToString("x2"));
            }

            return answer.ToString();
        }

        private byte[] GetSignedHash(string md5Hash)
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(md5Hash);

            using (RSACryptoServiceProvider rsaAlg = GetProviderFromKey(_keyPairOrPublicKey))
            {
                var formatter = new RSAPKCS1SignatureFormatter(rsaAlg);
                formatter.SetHashAlgorithm(HashAlgName);
                byte[] signedHashBytes = formatter.CreateSignature(hashBytes);

                return signedHashBytes;
            }
        }
    }
}