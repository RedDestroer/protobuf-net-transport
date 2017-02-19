using System;
using System.Globalization;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public class OfflineDataPackWriter
        : IDataPackWriter
    {
        public const byte InfoSection = 1;
        public const byte DataSection = 2;

        public void Write(Stream stream, DataPack dataPack, ISignAlgorithm signAlgorithm = null)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (dataPack == null) throw new ArgumentNullException("dataPack");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanWrite)
                throw new InvalidOperationException("Stream doesn't support writing.");

            using (var wrapper = new NonClosingStreamWrapper(stream)) // To prevent source stream from closing by BinaryWriter
            using (var bw = new BinaryWriter(wrapper))
            {
                // Prefix of data
                bw.Write(dataPack.GetPrefix(), 0, dataPack.PrefixSize);

                bw.Write((byte)255);

                uint signInfoAddress = 0;
                uint dataSizeAddress = 0;
                if (signAlgorithm == null)
                {
                    // Stream not signed
                    bw.Write((byte)0);
                }
                else
                {
                    // Stream is signed
                    bw.Write((byte)1);
                    signInfoAddress = GetAddress(bw);
                    bw.Write(0); // Protected data size
                    bw.Write(0); // Sign size
                    dataSizeAddress = GetAddress(bw) - 8;
                }

                // Create implicit properties
                var implicitProperties = new Properties();
                if (dataPack.DateCreate != null)
                    implicitProperties["DateCreate"] = dataPack.DateCreate.Value.ToString(Consts.DateTimeFormat, CultureInfo.InvariantCulture);
                if (dataPack.Description != null)
                    implicitProperties["Description"] = dataPack.Description;

                // Info section implicit properties
                var properties = implicitProperties.GetPropertiesList();
                bw.Write(InfoSection);
                uint address = GetAddress(bw);
                bw.Write(0u);
                ushort cnt = (ushort)properties.Count;
                bw.Write(cnt);

                for (ushort i = 0; i < cnt; i++)
                {
                    Serializer.SerializeWithLengthPrefix(wrapper, properties[i], PrefixStyle.Base128);
                }

                WriteSize(bw, address, address + 4);

                // Info section with data headers info
                bw.Write(InfoSection);
                address = GetAddress(bw);
                bw.Write(0u);
                cnt = (ushort)dataPack.Headers.Count;
                bw.Write(cnt);

                for (ushort i = 0; i < cnt; i++)
                {
                    Serializer.SerializeWithLengthPrefix(wrapper, dataPack.Headers[i], PrefixStyle.Base128);
                }

                WriteSize(bw, address, address + 4);

                // Header section with data properties info
                properties = dataPack.Properties.GetPropertiesList();
                bw.Write(InfoSection);
                address = GetAddress(bw);
                bw.Write(0u);
                cnt = (ushort)properties.Count;
                bw.Write(cnt);

                for (ushort i = 0; i < cnt; i++)
                {
                    Serializer.SerializeWithLengthPrefix(wrapper, properties[i], PrefixStyle.Base128);
                }

                WriteSize(bw, address, address + 4);

                // Header section with data dataPart's info
                bw.Write(InfoSection);
                var dataPartAddress = GetAddress(bw);
                bw.Write(0u);
                cnt = (ushort)dataPack.DataParts.Count;
                bw.Write(cnt);

                for (ushort i = 0; i < cnt; i++)
                {
                    bw.Write(0u);
                    bw.Write((ushort)0);
                    bw.Write(0u);
                    bw.Write(0u);
                    bw.Write((ushort)0);
                    bw.Write(0u);
                    bw.Write(0u);
                    bw.Write(0u);
                }

                WriteSize(bw, dataPartAddress, dataPartAddress + 4);

                bw.Write(DataSection);
                address = GetAddress(bw);
                bw.Write(0u);

                for (ushort i = 0; i < cnt; i++)
                {
                    WriteDataPart(bw, dataPartAddress + 6, i, dataPack);
                }

                WriteSize(bw, address, address + 4);

                bw.Seek(0, SeekOrigin.End);
                bw.Flush();

                if (signAlgorithm != null)
                {
                    uint protectedDataSize = GetAddress(bw) - dataSizeAddress;

                    wrapper.Position = dataSizeAddress;
                    bw.Write(protectedDataSize);
                    bw.Flush();

                    byte[] sign;
                    using (var filter = new FilteredStream(wrapper, dataSizeAddress, protectedDataSize))
                    {
                        sign = signAlgorithm.GetSign(filter);
                    }

                    wrapper.Seek(0, SeekOrigin.End);

                    bw.Write(sign, 0, sign.Length);
                    bw.Flush();

                    wrapper.Position = signInfoAddress;
                    bw.Write((uint)sign.Length);
                    bw.Flush();

                    wrapper.Seek(0, SeekOrigin.End);
                }
            }
        }

        private void WriteDataPart(BinaryWriter bw, uint dataPartAddress, int index, DataPack dataPack)
        {
            var dataPart = dataPack.DataParts[index];

            uint headersAddress = GetAddress(bw);
            var headers = dataPart.Headers;
            ushort headersCount = (ushort)headers.Count;
            for (ushort i = 0; i < headersCount; i++)
            {
                Serializer.SerializeWithLengthPrefix(bw.BaseStream, headers[i], PrefixStyle.Base128);
            }
            uint headersEndAddress = GetAddress(bw);
            uint headersSize = headersEndAddress - headersAddress;
            if (headersCount == 0)
            {
                headersAddress = 0;
                headersSize = 0;
            }

            uint propertiesAddress = GetAddress(bw);
            var properties = dataPart.Properties.GetPropertiesList();
            var propertiesCount = (ushort)properties.Count;
            for (ushort i = 0; i < propertiesCount; i++)
            {
                Serializer.SerializeWithLengthPrefix(bw.BaseStream, properties[i], PrefixStyle.Base128);
            }
            uint propertiesEndAddress = GetAddress(bw);
            uint propertiesSize = propertiesEndAddress - propertiesAddress;
            if (propertiesCount == 0)
            {
                propertiesAddress = 0;
                propertiesSize = 0;
            }

            uint dataAddress = GetAddress(bw);
            using (var sourceStream = dataPart.CreateStream())
            {
                var buffer = BufferProvider.Current.TakeBuffer();
                try
                {
                    int byteCount;
                    while ((byteCount = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, byteCount);
                    }
                }
                finally
                {
                    BufferProvider.Current.ReturnBuffer(buffer);
                }
            }

            uint dataSize = GetAddress(bw) - dataAddress;
            if (dataSize == 0)
                dataAddress = 0;

            bw.BaseStream.Seek(dataPartAddress, SeekOrigin.Begin);
            bw.Seek(index * ((6 * 4) + 2 * 2), SeekOrigin.Current);

            bw.Write(headersAddress);
            bw.Write(headersCount);
            bw.Write(headersSize);
            bw.Write(propertiesAddress);
            bw.Write(propertiesCount);
            bw.Write(propertiesSize);
            bw.Write(dataAddress);
            bw.Write(dataSize);
            bw.Flush();

            bw.Seek(0, SeekOrigin.End);
        }

        private void WriteSize(BinaryWriter bw, uint destAddress, uint startAddress)
        {
            var newAddress = GetAddress(bw);
            bw.BaseStream.Seek(destAddress, SeekOrigin.Begin);
            bw.Write(newAddress - startAddress);
            bw.BaseStream.Seek(0, SeekOrigin.End);
        }

        private uint GetAddress(BinaryWriter bw)
        {
            bw.Flush();
            uint position = (uint)bw.BaseStream.Position;

            return position;
        }
    }
}