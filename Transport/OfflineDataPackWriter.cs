using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public class OfflineDataPackWriter
        : IDataPackWriter
    {
        public const byte HeaderSection = 0;
        public const byte DataSection = 1;

        public void Write(Stream stream, DataPack dataPack)
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

                // Header section with data headers info
                bw.Write(HeaderSection);
                var address = GetAddress(bw);
                bw.Write(0);
                bw.Write(dataPack.Headers.Count);

                foreach (var header in dataPack.Headers)
                {
                    Serializer.Serialize(wrapper, header);
                }

                WriteSize(bw, address);

                // Header section with data properties info
                var properties = dataPack.Properties.GetProperties();
                bw.Write(HeaderSection);
                address = GetAddress(bw);
                bw.Write(0);
                bw.Write(properties.Count);

                foreach (var property in properties)
                {
                    Serializer.Serialize(wrapper, property);
                }

                WriteSize(bw, address);

                // Header section with data addInfo's info
                bw.Write(HeaderSection);
                address = GetAddress(bw);
                bw.Write(0);
                bw.Write(dataPack.AddInfos.Count);

                foreach (var addInfo in dataPack.AddInfos)
                {
                    Serializer.Serialize(wrapper, addInfo);
                }

                WriteSize(bw, address);

                // Header section with data dataPart's info
                bw.Write(HeaderSection);
                var dataPartAddress = GetAddress(bw);
                bw.Write(0);
                bw.Write(dataPack.DataParts.Count);

                for (int i = 0; i < dataPack.DataParts.Count; i++)
                {
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                }

                WriteSize(bw, dataPartAddress);

                bw.Write(DataSection);
                address = GetAddress(bw);
                bw.Write(0);

                for (int i = 0; i < dataPack.DataParts.Count; i++)
                {
                    WriteDataPart(bw, dataPartAddress, i, dataPack);
                }

                WriteSize(bw, address);

                bw.Seek(0, SeekOrigin.End);
                bw.Flush();
            }
        }

        private void WriteDataPart(BinaryWriter bw, int dataPartAddress, int index, DataPack dataPack)
        {
            int propertiesAddress = GetAddress(bw);
            
            var dataPart = dataPack.DataParts[index];
            var properties = dataPart.Properties.GetProperties();

            bw.Write(properties.Count);
            foreach (var property in properties)
            {
                Serializer.Serialize(bw.BaseStream, property);
            }

            int dataAddress = GetAddress(bw);

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

            int dataSize = GetAddress(bw) - dataAddress;

            bw.BaseStream.Seek(dataPartAddress, SeekOrigin.Begin);
            bw.Seek(index * (4 * 4), SeekOrigin.Current);

            bw.Write(propertiesAddress);
            bw.Write(properties.Count);
            bw.Write(dataAddress);
            bw.Write(dataSize);
            bw.Flush();

            bw.Seek(0, SeekOrigin.End);
        }

        private void WriteSize(BinaryWriter bw, int address)
        {
            var newAddress = GetAddress(bw);
            bw.BaseStream.Seek(address, SeekOrigin.Begin);
            bw.Write(newAddress - address);
            bw.BaseStream.Seek(0, SeekOrigin.End);
        }

        private int GetAddress(BinaryWriter bw)
        {
            bw.Flush();
            int position = (int)bw.BaseStream.Position;

            return position;
        }
    }
}