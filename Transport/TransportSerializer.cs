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

        public static void Serialize(Stream stream, DataPack dataPack)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (dataPack == null) throw new ArgumentNullException("dataPack");

            DataPackWriter.Write(stream, dataPack);
        }

        public static DataPack Deserialize(byte[] prefix, Stream stream)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            if (stream == null) throw new ArgumentNullException("stream");

            return DataPackReader.Read(prefix, stream);
        }
    }
}