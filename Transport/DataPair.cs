using System;

namespace ProtoBuf.Transport
{
    [ProtoContract]
    public class DataPair
    {
        public DataPair(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
#if NET20 || NET30 || NET35
            if (StringExtension.IsNullOrWhiteSpace(name)) throw new ArgumentOutOfRangeException("name");
#endif

#if NET40 || NET45
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentOutOfRangeException("name");
#endif
            
            Name = name;
            Value = value;
        }

        [ProtoMember(1, IsRequired = true)]
        public string Name { get; private set; }

        [ProtoMember(2, IsRequired = false)]
        public string Value { get; private set; }

        public DataPair Clone()
        {
            return new DataPair(Name, Value);
        }
    }
}