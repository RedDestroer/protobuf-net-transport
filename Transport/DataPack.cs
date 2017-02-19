using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public class DataPack
    {
        private readonly byte[] _dataPrefix;
        private readonly byte _prefixSize;

        public DataPack(byte[] dataPrefix = null)
            : this()
        {
            if (dataPrefix != null && dataPrefix.Length > 255)
                throw new InvalidDataException("Length of dataPrefix must be in range from 0 to 255");

            _dataPrefix = dataPrefix;
            _prefixSize = _dataPrefix == null
                ? (byte)0
                : (byte)_dataPrefix.Length;
        }

        public DataPack(string dataPrefix = null)
            : this()
        {
            if (dataPrefix != null)
            {
                var bytes = Encoding.UTF8.GetBytes(dataPrefix);

                if (bytes.Length > 255)
                    throw new InvalidDataException("Length of dataPrefix must be in range from 0 to 255");

                _dataPrefix = bytes;
            }
            else
            {
                _dataPrefix = null;
            }

            _prefixSize = _dataPrefix == null
                ? (byte)0
                : (byte)_dataPrefix.Length;
        }

        private DataPack()
        {
            Headers = new Headers();
            Properties = new Properties();
            DataParts = new List<DataPart>();
            DateCreate = TimeProvider.Current.Now;
        }

        public Headers Headers { get; private set; }

        public Properties Properties { get; private set; }

        public IList<DataPart> DataParts { get; private set; }

        public byte PrefixSize { get { return _prefixSize; } }

        public DateTime? DateCreate { get; set; }

        public string Description { get; set; }

        public byte[] GetPrefix()
        {
            if (PrefixSize < 1)
                return new byte[0];

            var dataPrefix = new byte[PrefixSize];

            Array.Copy(_dataPrefix, 0, dataPrefix, 0, PrefixSize);

            return dataPrefix;
        }

        public bool IsPrefixMatch(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (PrefixSize < 1)
                return true;

            var dataPrefix = new byte[PrefixSize];
            var byteCount = stream.Read(dataPrefix, 0, PrefixSize);

            if (byteCount != PrefixSize)
                return false;

            return IsPrefixMatch(dataPrefix);
        }

        public bool IsPrefixMatch(byte[] dataPrefix)
        {
            if (dataPrefix == null) throw new ArgumentNullException("dataPrefix");

            if (dataPrefix.Length != PrefixSize)
                return false;
            if (PrefixSize < 1)
                return true;

            for (int i = 0; i < PrefixSize; i++)
            {
                if (_dataPrefix[i] != dataPrefix[i])
                    return false;
            }

            return true;
        }
    }
}