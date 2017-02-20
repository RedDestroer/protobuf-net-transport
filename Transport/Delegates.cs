using System.IO;

namespace ProtoBuf.Transport
{
    public delegate void SerializationMethod(Stream stream, object obj);
    public delegate object DeserializationMethod(Stream stream);
}