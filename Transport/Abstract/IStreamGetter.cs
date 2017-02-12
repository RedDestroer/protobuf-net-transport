using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    public interface IStreamGetter
    {
        Stream CreateStream();
    }
}
