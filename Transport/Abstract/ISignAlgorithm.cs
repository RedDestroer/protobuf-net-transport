using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    public interface ISignAlgorithm
    {
        byte[] GetSign(Stream stream);
        bool VerifySign(Stream stream, byte[] signBytes);
        bool VerifySign(Stream stream, Stream signStream);
        bool VerifySign(string md5Hash, Stream signStream);
    }
}