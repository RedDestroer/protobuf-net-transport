using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Data transport container
    /// </summary>
    public class DataPack
    {
        private readonly byte[] _dataPrefix;
        private readonly byte _prefixSize;
        
        /// <summary>
        /// Creates <see cref="DataPack"/> instance.
        /// </summary>
        public DataPack()
        {
            Headers = new Headers();
            Properties = new Properties();
            DataParts = new List<DataPart>();
            DateCreate = TimeProvider.Current.Now;
            FileId = Guid.NewGuid();
            _dataPrefix = null;
            _prefixSize = 0;
        }

        /// <summary>
        /// Creates <see cref="DataPack"/> instance with prefix bytes.
        /// </summary>
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

        /// <summary>
        /// Creates <see cref="DataPack"/> instance with prefix bytes (Encoding.UTF8.GetBytes(dataPrefix)).
        /// </summary>
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

        /// <summary>
        /// Headers of container
        /// </summary>
        public Headers Headers { get; private set; }

        /// <summary>
        /// Properties of container
        /// </summary>
        public Properties Properties { get; private set; }

        /// <summary>
        /// All transported data
        /// </summary>
        public IList<DataPart> DataParts { get; private set; }

        /// <summary>
        /// Size of prefix in bytes
        /// </summary>
        public byte PrefixSize { get { return _prefixSize; } }

        /// <summary>
        /// File identifier (for uniqueness)
        /// </summary>
        public Guid? FileId { get; set; }

        /// <summary>
        /// File create date
        /// </summary>
        public DateTime? DateCreate { get; set; }

        /// <summary>
        /// File description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get prefix as byte array
        /// </summary>
        /// <returns></returns>
        public byte[] GetPrefix()
        {
            if (PrefixSize < 1)
                return new byte[0];

            var dataPrefix = new byte[PrefixSize];

            Array.Copy(_dataPrefix, 0, dataPrefix, 0, PrefixSize);

            return dataPrefix;
        }

        /// <summary>
        /// Checks if first bytes of stream matches prefix
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks if byte array matches prefix
        /// </summary>
        /// <param name="dataPrefix"></param>
        /// <returns></returns>
        public bool IsPrefixMatch(byte[] dataPrefix)
        {
            if (dataPrefix == null && PrefixSize <= 0)
                return true;
            if (dataPrefix == null && PrefixSize > 0)
                return false;

            // ReSharper disable once PossibleNullReferenceException
            if (dataPrefix.Length != PrefixSize)
                return false;
            if (PrefixSize <= 0)
                return true;

            for (int i = 0; i < PrefixSize; i++)
            {
                if (_dataPrefix[i] != dataPrefix[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Adds header to transport container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataPair AddHeader(string name, string value = null)
        {
            var dataPair = new DataPair(name, value);

            Headers.Add(dataPair);

            return dataPair;
        }

        /// <summary>
        /// Adds property to transport container
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DataPair AddProperty(string name, string value = null)
        {
            var dataPair = new DataPair(name, value);

            Properties.AddOrReplace(dataPair);

            return dataPair;
        }

        /// <summary>
        /// Adds <see cref="IDataContainer"/> as part of data to transport container
        /// </summary>
        /// <param name="dataContainer"></param>
        /// <returns></returns>
        public DataPart AddDataPart(IDataContainer dataContainer)
        {
            if (dataContainer == null) throw new ArgumentNullException("dataContainer");

            var dataPart = new DataPart(dataContainer);

            DataParts.Add(dataPart);

            return dataPart;
        }

        /// <summary>
        /// Adds <see cref="DataPart"/> to transport container
        /// </summary>
        /// <param name="dataPart"></param>
        /// <returns></returns>
        public DataPart AddDataPart(DataPart dataPart)
        {
            if (dataPart == null) throw new ArgumentNullException("dataPart");

            DataParts.Add(dataPart);

            return dataPart;
        }
    }
}