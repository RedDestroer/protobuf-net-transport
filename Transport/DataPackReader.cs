﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Reader for files, written in protobuf-net-transport format
    /// </summary>
    public abstract class DataPackReader
        : IDataPackReader
    {
        /// <summary>
        /// Mark byte for information sections
        /// </summary>
        public const byte InfoSection = 1;

        /// <summary>
        /// Mark byte for data sections
        /// </summary>
        public const byte DataSection = 2;

        /// <summary>
        /// Reads <see cref="DataPack"/> from stream, using prefix if provided
        /// </summary>
        /// <param name="stream">Stream with data to deserialize</param>
        /// <param name="prefix">Byte array with bytes to check before deserialize</param>
        /// <returns></returns>
        public DataPack Read(Stream stream, byte[] prefix = null)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream doesn't support reading.");

            DataPack dataPack;

            using (var wrapper = new NonClosingStreamWrapper(stream)) // To prevent source stream from closing by BinaryReader
            using (var br = new BinaryReader(wrapper))
            {
                byte prefixSize = br.ReadByte();
                byte[] dataPrefix = null;
                if (prefixSize > 0)
                {
                    dataPrefix = br.ReadBytes(prefixSize);
                }

                dataPack = new DataPack(dataPrefix)
                    {
                        DateCreate = null,
                        Description = null,
                        FileId = null
                    };

                if (!dataPack.IsPrefixMatch(prefix))
                    throw new InvalidOperationException("Data prefix is wrong.");

                byte isSignedByte = br.ReadByte();
                switch (isSignedByte)
                {
                    case 0: // No sign. Nothing to do
                        break;
                    case 1:
                        // ReSharper disable once UnusedVariable
                        uint protectedDataSize = br.ReadUInt32(); // Protected data size

                        // ReSharper disable once UnusedVariable
                        uint signSize = br.ReadUInt32(); // Sign size

                        break;
                    default:
                        throw new InvalidOperationException("Unknown information about file sign.");
                }

                if (br.ReadByte() != InfoSection)
                    throw new InvalidOperationException("Implicit properties info section not found.");

                uint size = br.ReadUInt32();
                ushort propertiesCount = br.ReadUInt16();

                var implicitProperties = new Properties();
                using (var filter = new FilteredStream(wrapper, wrapper.Position, size - 2))
                {
                    for (ushort i = 0; i < propertiesCount; i++)
                    {
                        implicitProperties.AddOrReplace(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                    }
                }

                var dateCreateString = implicitProperties.TryGetPropertyValue("DateCreate", null);
                if (dateCreateString != null)
                {
                    dataPack.DateCreate = DateTime.ParseExact(dateCreateString, Consts.DateTimeFormat, CultureInfo.InvariantCulture);
                }

                var fileIdString = implicitProperties.TryGetPropertyValue("FileId", null);
                if (fileIdString != null)
                {
#if NET20 || NET30 || NET35
                    dataPack.FileId = new Guid(fileIdString);
#endif

#if NET40 || NET45
                    dataPack.FileId = Guid.Parse(fileIdString);
#endif
                }

                var descriptionString = implicitProperties.TryGetPropertyValue("Description", null);
                if (descriptionString != null)
                {
                    dataPack.Description = descriptionString;
                }

                if (br.ReadByte() != InfoSection)
                    throw new InvalidOperationException("Headers info section not found.");

                size = br.ReadUInt32();
                ushort headersCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size - 2))
                {
                    for (ushort i = 0; i < headersCount; i++)
                    {
                        dataPack.Headers.Add(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                    }
                }

                if (br.ReadByte() != InfoSection)
                    throw new InvalidOperationException("Properties info section not found.");

                size = br.ReadUInt32();
                propertiesCount = br.ReadUInt16();

                using (var filter = new FilteredStream(wrapper, br.BaseStream.Position, size - 2))
                {
                    for (ushort i = 0; i < propertiesCount; i++)
                    {
                        dataPack.Properties.AddOrReplace(Serializer.DeserializeWithLengthPrefix<DataPair>(filter, PrefixStyle.Base128));
                    }
                }

                if (br.ReadByte() != InfoSection)
                    throw new InvalidOperationException("DataPart's info section not found.");

                // ReSharper disable once RedundantAssignment
                size = br.ReadUInt32();
                ushort dataPartsCount = br.ReadUInt16();
                var dataPartInfos = new List<DataPartInfo>();

                for (ushort i = 0; i < dataPartsCount; i++)
                {
                    dataPartInfos.Add(new DataPartInfo
                        {
                            HeadersAddress = br.ReadUInt32(),
                            HeadersCount = br.ReadUInt16(),
                            HeadersSize = br.ReadUInt32(),
                            PropertiesAddress = br.ReadUInt32(),
                            PropertiesCount = br.ReadUInt16(),
                            PropertiesSize = br.ReadUInt32(),
                            DataAddress = br.ReadUInt32(),
                            DataSize = br.ReadUInt32()
                        });
                }

                ReadDataParts(dataPack, br, dataPartInfos, stream);
            }
            
            return dataPack;
        }

        /// <summary>
        /// Reads <see cref="DataPack"/> from stream, using prefix if provided
        /// </summary>
        /// <param name="stream">Stream with data to deserialize</param>
        /// <param name="prefix">Byte array with bytes to check before deserialize</param>
        /// <returns></returns>
        public DataPack Read(Stream stream, string prefix = null)
        {
            if (prefix == null)
                return Read(stream, (byte[])null);

            var bytes = Encoding.UTF8.GetBytes(prefix);

            return Read(stream, bytes);
        }

        /// <summary>
        /// Read all data parts from <see cref="DataPack"/>
        /// </summary>
        /// <param name="dataPack"></param>
        /// <param name="br">Binary reader</param>
        /// <param name="dataPartInfos">List of datapart information</param>
        /// <param name="stream">Stream of transport container</param>
        protected abstract void ReadDataParts(DataPack dataPack, BinaryReader br, List<DataPartInfo> dataPartInfos, Stream stream);
        
        /// <summary>
        /// Information about data part
        /// </summary>
        protected class DataPartInfo
        {
            /// <summary>
            /// Address of headers
            /// </summary>
            public uint HeadersAddress { get; set; }

            /// <summary>
            /// Count of headers
            /// </summary>
            public ushort HeadersCount { get; set; }
            
            /// <summary>
            /// Size of all headers
            /// </summary>
            public uint HeadersSize { get; set; }

            /// <summary>
            /// Address of properties
            /// </summary>
            public uint PropertiesAddress { get; set; }

            /// <summary>
            /// Properties count
            /// </summary>
            public ushort PropertiesCount { get; set; }

            /// <summary>
            /// Size of all properties
            /// </summary>
            public uint PropertiesSize { get; set; }

            /// <summary>
            /// Address of data
            /// </summary>
            public uint DataAddress { get; set; }

            /// <summary>
            /// Size of data
            /// </summary>
            public uint DataSize { get; set; }
        }
    }
}