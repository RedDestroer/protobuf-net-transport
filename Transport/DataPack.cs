using System;
using System.Collections.Generic;
using System.IO;

namespace ProtoBuf.Transport
{
    public class DataPack
    {
        private readonly byte[] _dataPrefix;
        private readonly byte _prefixSize;

        public DataPack(byte[] dataPrefix)
        {
            if (dataPrefix == null) throw new ArgumentNullException("dataPrefix");
            if (dataPrefix.Length > 255)
                throw new InvalidDataException("Length of dataPrefix must be in range from 0 to 255");

            _dataPrefix = dataPrefix;
            _prefixSize = (byte)_dataPrefix.Length;
            Headers = new Headers();
            Properties = new Properties();
            AddInfos = new List<AddInfo>();
            DataParts = new List<DataPart>();
        }

        public Headers Headers { get; private set; }

        public Properties Properties { get; private set; }

        public IList<AddInfo> AddInfos { get; private set; }

        public IList<DataPart> DataParts { get; private set; }

        public byte PrefixSize { get { return _prefixSize; } }

        public byte[] GetPrefix()
        {
            var dataPrefix = new byte[PrefixSize];

            Array.Copy(_dataPrefix, 0, dataPrefix, 0, PrefixSize);

            return dataPrefix;
        }

        public bool IsPrefixMatch(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

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

            for (int i = 0; i < PrefixSize; i++)
            {
                if (_dataPrefix[i] != dataPrefix[i])
                    return false;
            }

            return true;
        }
    }
}