using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    public interface IDataPackReader
    {
        DataPack Read(byte[] prefix, Stream stream);
    }
}