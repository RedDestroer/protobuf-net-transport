using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public abstract class DataPackReader
        : IDataPackReader
    {
        public const byte HeaderSection = 0;
        public const byte DataSection = 1;

        public DataPack Read(byte[] prefix, Stream stream)
        {
            if (prefix == null) throw new ArgumentNullException("prefix");
            if (stream == null) throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream doesn't support reading.");

            var dataPack = new DataPack(prefix);

            using (var wrapper = new NonClosingStreamWrapper(stream)) // To prevent source stream from closing by BinaryReader
            using (var br = new BinaryReader(wrapper))
            {
                var dataPrefix = br.ReadBytes(dataPack.PrefixSize);
                if (!dataPack.IsPrefixMatch(dataPrefix))
                    throw new InvalidOperationException("Data prefix is wrong.");

                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("Headers header section not found.");

                br.BaseStream.Seek(4, SeekOrigin.Current);
                int headersCount = br.ReadInt32();

                for (int i = 0; i < headersCount; i++)
                {
                    dataPack.Headers.Add(Serializer.Deserialize<DataPair>(wrapper));
                }

                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("Properties header section not found.");

                br.BaseStream.Seek(4, SeekOrigin.Current);
                int propertiesCount = br.ReadInt32();

                for (int i = 0; i < propertiesCount; i++)
                {
                    dataPack.Properties.AddOrReplace(Serializer.Deserialize<DataPair>(wrapper));
                }

                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("AddInfo's header section not found.");

                br.BaseStream.Seek(4, SeekOrigin.Current);
                int addInfosCount = br.ReadInt32();

                for (int i = 0; i < addInfosCount; i++)
                {
                    dataPack.AddInfos.Add(Serializer.Deserialize<AddInfo>(wrapper));
                }

                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("DataPart's header section not found.");

                br.BaseStream.Seek(4, SeekOrigin.Current);
                int dataPartsCount = br.ReadInt32();
                var dataPartInfos = new List<DataPartInfo>();

                for (int i = 0; i < dataPartsCount; i++)
                {
                    dataPartInfos.Add(new DataPartInfo
                    {
                        PropertiesAddress = br.ReadInt32(),
                        PropertiesCount = br.ReadInt32(),
                        DataAddress = br.ReadInt32(),
                        DataSize = br.ReadInt32()
                    });
                }

                ReadDataParts(dataPack, br, dataPartInfos);
            }

            return dataPack;
        }

        protected abstract void ReadDataParts(DataPack dataPack, BinaryReader br, List<DataPartInfo> dataPartInfos);

        protected class DataPartInfo
        {
            public int PropertiesAddress { get; set; }

            public int PropertiesCount { get; set; }

            public int DataAddress { get; set; }

            public int DataSize { get; set; }
        }
    }
}