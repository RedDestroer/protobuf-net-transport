using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    /// <summary>
    /// Holds information about data, which is ready to be copied into given stream by command
    /// </summary>
    public interface IDataContainer
    {
        /// <summary>
        /// Copy contained inner data into the output stream.
        /// </summary>
        /// <param name="stream"></param>
        void CopyToStream(Stream stream);
    }
}
