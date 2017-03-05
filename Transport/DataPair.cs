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
        : IEquatable<DataPair>
    {
        /// <summary>
        /// Initializes new instance of <see cref="DataPair"/>. Default constructor used only for deserialization purposes.
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

        public static bool operator ==(DataPair left, DataPair right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (((object)left == null) || ((object)right == null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(DataPair left, DataPair right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Creates a copy of DataPair
        /// </summary>
        /// <returns></returns>
        public DataPair Clone()
        {
            return new DataPair(Name, Value);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(DataPair other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return string.Equals(Name, other.Name) && string.Equals(Value, other.Value);
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
        /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;

            return Equals((DataPair)obj);
        }

        /// <summary>Serves as a hash function for a particular type. </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null
                            ? Name.GetHashCode()
                            : 0) * 397) ^ (Value != null
                           ? Value.GetHashCode()
                           : 0);
            }
        }
    }
}