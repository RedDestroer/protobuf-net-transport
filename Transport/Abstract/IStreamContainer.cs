using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    public interface IStreamContainer
    {
        Stream GetStream();

        void CopyToStream(Stream stream);
    }
}
