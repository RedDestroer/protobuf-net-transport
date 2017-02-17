using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    public interface IDataPackReader
    {
        DataPack Read(Stream stream, byte[] prefix = null);
    }
}