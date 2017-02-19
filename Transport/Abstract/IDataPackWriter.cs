using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    /// <summary>
    /// Writer for DataPack
    /// </summary>
    public interface IDataPackWriter
    {
        /// <summary>
        /// Writes <see cref="DataPack"/> to stream
        /// </summary>
        /// <param name="stream">Stream where <see cref="DataPack"/> is written to</param>
        /// <param name="dataPack">Data</param>
        /// <param name="signAlgorithm">Sign algorithm if needed</param>
        void Write(Stream stream, DataPack dataPack, ISignAlgorithm signAlgorithm = null);
    }
}