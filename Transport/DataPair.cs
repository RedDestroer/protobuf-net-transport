using System;
using System.Diagnostics;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Key value storage for one pair of data
    /// </summary>
    [DebuggerDisplay(@"\{ ""Name"": {Name}, ""Value"": {Value} \}")]
    [ProtoContract(SkipConstructor = true)]
    public class DataPair
    {
        /// <summary>
        /// Initializes new instance of <see cref="DataPair"/>. Defaul constructo used only for deserialization purposes.
        /// </summary>
        private DataPair()
        {
        }

        /// <summary>
        /// Initializes new instance of <see cref="DataPair"/>
        /// </summary>
        /// <param name="name">Name of this pair</param>
        /// <param name="value">Value of this pair</param>
        public DataPair(string name, string value = null)
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

        /// <summary>
        /// Key of this pair
        /// </summary>
        [ProtoMember(1, IsRequired = true)]
        public string Name { get; private set; }

        /// <summary>
        /// Value of this pair
        /// </summary>
        [ProtoMember(2, IsRequired = false)]
        public string Value { get; private set; }

        /// <summary>
        /// Creates a copy of DataPair
        /// </summary>
        /// <returns></returns>
        public DataPair Clone()
        {
            return new DataPair(Name, Value);
        }
    }
}