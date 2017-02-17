using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    public interface IDataPackWriter
    {
        void Write(Stream stream, DataPack dataPack, ISignAlgorithm signAlgorithm = null);
    }
}