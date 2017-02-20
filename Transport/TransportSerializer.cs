using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public static class TransportSerializer
    {
        private static IDataPackReader _dataPackReader;
        private static IDataPackWriter _dataPackWriter;

        static TransportSerializer()
        {
            DataPackReader = new OfflineDataPackReader();
            DataPackWriter = new OfflineDataPackWriter();
        }

        public static IDataPackReader DataPackReader
        {
            get { return _dataPackReader; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _dataPackReader = value;
            }
        }

        public static IDataPackWriter DataPackWriter
        {
            get { return _dataPackWriter; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                _dataPackWriter = value;
            }
        }

        public static void Serialize(DataPack dataPack, Stream stream, ISignAlgorithm signAlgorithm = null)
        {
            if (dataPack == null) throw new ArgumentNullException("dataPack");
            if (stream == null) throw new ArgumentNullException("stream");

            DataPackWriter.Write(dataPack, stream, signAlgorithm);
        }

        public static DataPack Deserialize(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return Deserialize(stream, (string)null);
        }

        public static DataPack Deserialize(Stream stream, byte[] prefix)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return DataPackReader.Read(stream, prefix);
        }

        public static DataPack Deserialize(Stream stream, string prefix)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return DataPackReader.Read(stream, prefix);
        }
    }
}