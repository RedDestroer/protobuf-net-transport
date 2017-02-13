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

                int size = br.ReadInt32();
                ushort headersCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size))
                {
                    for (ushort i = 0; i < headersCount; i++)
                    {
                        dataPack.Headers.Add(Serializer.Deserialize<DataPair>(filter));
                    }
                }
                
                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("Properties header section not found.");

                size = br.ReadInt32();
                ushort propertiesCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size))
                {
                    for (ushort i = 0; i < propertiesCount; i++)
                    {
                        dataPack.Properties.AddOrReplace(Serializer.Deserialize<DataPair>(filter));
                    }
                }
                
                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("AddInfo's header section not found.");

                size = br.ReadInt32();
                ushort addInfosCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size))
                {
                    for (ushort i = 0; i < addInfosCount; i++)
                    {
                        dataPack.AddInfos.Add(Serializer.Deserialize<AddInfo>(filter));
                    }
                }
                
                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("DataPart's header section not found.");

                br.BaseStream.Seek(4, SeekOrigin.Current);
                ushort dataPartsCount = br.ReadUInt16();
                var dataPartInfos = new List<DataPartInfo>();

                for (ushort i = 0; i < dataPartsCount; i++)
                {
                    dataPartInfos.Add(new DataPartInfo
                        {
                            PropertiesAddress = br.ReadInt32(),
                            PropertiesCount = br.ReadUInt16(),
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

            public ushort PropertiesCount { get; set; }

            public int DataAddress { get; set; }

            public int DataSize { get; set; }
        }
    }
}