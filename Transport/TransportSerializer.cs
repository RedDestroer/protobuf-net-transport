using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Serializer to serialize and deserealize DataPack files
    /// </summary>
    public static class TransportSerializer
    {
        private static IDataPackReader _dataPackReader;
        private static IDataPackWriter _dataPackWriter;

        /// <summary>
        /// Type constructor
        /// </summary>
        static TransportSerializer()
        {
            DataPackReader = new OfflineDataPackReader();
            DataPackWriter = new OfflineDataPackWriter();
        }

        /// <summary>
        /// DataPackReader used to deserealize stream into DataPack
        /// </summary>
        public static IDataPackReader DataPackReader
        {
            get { return _dataPackReader; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _dataPackReader = value;
            }
        }

        /// <summary>
        /// DataPackWriter used to serialize DataPack into strream
        /// </summary>
        public static IDataPackWriter DataPackWriter
        {
            get { return _dataPackWriter; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _dataPackWriter = value;
            }
        }

        /// <summary>
        /// Writes DataPack into stream using signing if needed
        /// </summary>
        /// <param name="dataPack">DataPack object to write</param>
        /// <param name="stream">Stream, where DataPack will be serialized</param>
        /// <param name="signAlgorithm">Signing algorithm to use if needed</param>
        public static void Serialize(DataPack dataPack, Stream stream, ISignAlgorithm signAlgorithm = null)
        {
            if (dataPack == null) throw new ArgumentNullException("dataPack");
            if (stream == null) throw new ArgumentNullException("stream");

            DataPackWriter.Write(dataPack, stream, signAlgorithm);
        }

        /// <summary>
        /// Deserializes DataPack from stream
        /// </summary>
        /// <param name="stream">Stream, where DataPack is contained</param>
        /// <returns></returns>
        public static DataPack Deserialize(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return Deserialize(stream, (string)null);
        }

        /// <summary>
        /// Deserializes DataPack from stream and checks if prefix match
        /// </summary>
        /// <param name="stream">Stream, where DataPack is contained</param>
        /// <param name="prefix">Prefix of data</param>
        /// <returns></returns>
        public static DataPack Deserialize(Stream stream, byte[] prefix)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return DataPackReader.Read(stream, prefix);
        }

        /// <summary>
        /// Deserializes DataPack from stream and checks if prefix match
        /// </summary>
        /// <param name="stream">Stream, where DataPack is contained</param>
        /// <param name="prefix">Prefix of data</param>
        /// <returns></returns>
        public static DataPack Deserialize(Stream stream, string prefix)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return DataPackReader.Read(stream, prefix);
        }
    }
}