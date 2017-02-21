using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    /// <summary>
    /// Reader for files, written in protobuf-net-transport format
    /// </summary>
    public interface IDataPackReader
    {
        /// <summary>
        /// Reads <see cref="DataPack"/> from stream, using prefix if provided
        /// </summary>
        /// <param name="stream">Stream with data to deserialize</param>
        /// <param name="prefix">Byte array with bytes to check before deserialize</param>
        /// <returns></returns>
        DataPack Read(Stream stream, byte[] prefix = null);

        /// <summary>
        /// Reads <see cref="DataPack"/> from stream, using prefix if provided
        /// </summary>
        /// <param name="stream">Stream with data to deserialize</param>
        /// <param name="prefix">Byte array with bytes to check before deserialize</param>
        /// <returns></returns>
        DataPack Read(Stream stream, string prefix = null);
    }
}