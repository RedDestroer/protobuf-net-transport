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

        private const byte SignSize = 128;

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

                byte isSignedByte = br.ReadByte();
                switch (isSignedByte)
                {
                    case 0: // No sign. Nothing to do
                        break;
                    case 1:
                        wrapper.Seek(SignSize + 4, SeekOrigin.Current); // Sign size plus size of int of protected data size
                        break;
                    default:
                        throw new InvalidOperationException("Unknown information about file sign.");
                }

                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("Headers header section not found.");

                uint size = br.ReadUInt32();
                ushort headersCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size - 2))
                {
                    for (ushort i = 0; i < headersCount; i++)
                    {
                        dataPack.Headers.Add(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                    }
                }
                
                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("Properties header section not found.");

                size = br.ReadUInt32();
                ushort propertiesCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size - 2))
                {
                    for (ushort i = 0; i < propertiesCount; i++)
                    {
                        dataPack.Properties.AddOrReplace(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                    }
                }
                
                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("AddInfo's header section not found.");

                size = br.ReadUInt32();
                ushort addInfosCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size - 2))
                {
                    for (ushort i = 0; i < addInfosCount; i++)
                    {
                        dataPack.AddInfos.Add(Serializer.DeserializeWithLengthPrefix<AddInfo>(filter, PrefixStyle.Base128));
                    }
                }
                
                if (br.ReadByte() != HeaderSection)
                    throw new InvalidOperationException("DataPart's header section not found.");

                // ReSharper disable once RedundantAssignment
                size = br.ReadUInt32();
                ushort dataPartsCount = br.ReadUInt16();
                var dataPartInfos = new List<DataPartInfo>();

                for (ushort i = 0; i < dataPartsCount; i++)
                {
                    dataPartInfos.Add(new DataPartInfo
                        {
                            PropertiesAddress = br.ReadUInt32(),
                            PropertiesCount = br.ReadUInt16(),
                            DataAddress = br.ReadUInt32(),
                            DataSize = br.ReadUInt32()
                        });
                }

                ReadDataParts(dataPack, br, dataPartInfos);
            }

            return dataPack;
        }

        protected abstract void ReadDataParts(DataPack dataPack, BinaryReader br, List<DataPartInfo> dataPartInfos);

        protected class DataPartInfo
        {
            public uint PropertiesAddress { get; set; }

            public ushort PropertiesCount { get; set; }

            public uint DataAddress { get; set; }

            public uint DataSize { get; set; }
        }
    }
}