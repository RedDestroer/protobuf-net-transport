using System;
using System.IO;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public static class RsaSigner
    {
        public static void Sign(uint prefixSize, string keyPair, Stream sourceStream, Stream destinationStream)
        {
            if (keyPair == null) throw new ArgumentNullException("keyPair");
            if (sourceStream == null) throw new ArgumentNullException("sourceStream");
            if (destinationStream == null) throw new ArgumentNullException("destinationStream");

            var position = sourceStream.Position;

            if (!sourceStream.CanSeek)
                throw new InvalidOperationException("Source stream doesn't support seeking.");
            if (!sourceStream.CanRead)
                throw new InvalidOperationException("Source stream doesn't support reading.");
            if (!destinationStream.CanSeek)
                throw new InvalidOperationException("Destination stream doesn't support seeking.");
            if (!destinationStream.CanRead)
                throw new InvalidOperationException("Destination stream doesn't support reading.");
            if (!destinationStream.CanWrite)
                throw new InvalidOperationException("Destination stream doesn't support writing.");

            var keyAlgorithm = new RsaSignAlgorithm(keyPair);

            sourceStream.Seek(prefixSize, SeekOrigin.Current);
            int isSignedByte = sourceStream.ReadByte();
            if (isSignedByte == 1)
                throw new InvalidOperationException("Source stream already signed.");

            sourceStream.Position = position;

            var buffer = BufferProvider.Current.TakeBuffer();
            try
            {
                int byteCount;
                uint prefixBytesLeft = prefixSize;
                while ((byteCount = sourceStream.Read(buffer, 0, (int)Math.Min(buffer.Length, prefixBytesLeft))) > 0)
                {
                    destinationStream.Write(buffer, 0, byteCount);
                    prefixBytesLeft -= (uint)byteCount;
                    if (prefixBytesLeft == 0)
                        break;
                }

                using (var wrapper = new NonClosingStreamWrapper(destinationStream)) // To prevent stream from closing by BinaryWriter
                using (var bw = new BinaryWriter(wrapper))
                {
                    bw.Write((byte)1);

                    uint signAddress = GetAddress(bw);

                    for (int i = 0; i < 33; i++) // 128 bytes for sign and 4 byte for data size
                    {
                        bw.Write(0);
                    }

                    uint dataSizeAddress = GetAddress(bw) - 4;

                    while ((byteCount = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destinationStream.Write(buffer, 0, byteCount);
                    }

                    destinationStream.Flush();
                    uint dataSize = GetAddress(bw) - dataSizeAddress - 4;

                    destinationStream.Position = dataSizeAddress;
                    bw.Write(dataSize);
                    bw.Flush();

                    byte[] sign;
                    using (var filter = new FilteredStream(destinationStream, dataSizeAddress, dataSize + 4))
                    {
                        sign = keyAlgorithm.GetSign(filter);
                    }

                    destinationStream.Position = signAddress;
                    bw.Write(sign, 0, sign.Length);
                    bw.Flush();
                }
            }
            finally
            {
                BufferProvider.Current.ReturnBuffer(buffer);
            }
        }

        public static bool IsSigned(uint prefixSize, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream doesn't support reading.");

            stream.Seek(prefixSize, SeekOrigin.Current);
            int isSignedByte = stream.ReadByte();
            if (isSignedByte == 1)
                return true;

            return false;
        }

        public static bool IsSignMatch(uint prefixSize, string publicKey, Stream stream)
        {
            if (publicKey == null) throw new ArgumentNullException("publicKey");
            if (stream == null) throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream doesn't support reading.");

            long position = stream.Position;
            var keyAlgorithm = new RsaSignAlgorithm(publicKey);

            stream.Seek(prefixSize, SeekOrigin.Current);
            int isSignedByte = stream.ReadByte();
            if (isSignedByte == 0)
                throw new InvalidOperationException("Stream is not signed.");

            using (var wrapper = new NonClosingStreamWrapper(stream)) // To prevent stream from closing by BinaryReader
            using (var br = new BinaryReader(wrapper))
            {
                using (var signStream = new FilteredStream(stream, stream.Position, 128))
                {
                    stream.Seek(128, SeekOrigin.Current);
                    uint dataSize = br.ReadUInt32();

                    using (var dataStream = new FilteredStream(stream, stream.Position - 4, dataSize + 4))
                    {
                        var result = keyAlgorithm.VerifySign(dataStream, signStream);

                        stream.Position = position;

                        return result;
                    }
                }
            }
        }

        public static void RemoveSign(uint prefixSize, Stream sourceStream, Stream destinationStream)
        {
            if (sourceStream == null) throw new ArgumentNullException("sourceStream");
            if (destinationStream == null) throw new ArgumentNullException("destinationStream");

            var buffer = BufferProvider.Current.TakeBuffer();
            try
            {
                int byteCount;
                uint prefixBytesLeft = prefixSize;
                while ((byteCount = sourceStream.Read(buffer, 0, (int)Math.Min(buffer.Length, prefixBytesLeft))) > 0)
                {
                    destinationStream.Write(buffer, 0, byteCount);
                    prefixBytesLeft -= (uint)byteCount;
                    if (prefixBytesLeft == 0)
                        break;
                }

                sourceStream.WriteByte(0);
                sourceStream.Seek(128 + 4, SeekOrigin.Current);

                while ((byteCount = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, byteCount);
                }
            }
            finally
            {
                BufferProvider.Current.ReturnBuffer(buffer);
            }
        }

        private static uint GetAddress(BinaryWriter bw)
        {
            bw.Flush();
            uint position = (uint)bw.BaseStream.Position;

            return position;
        }
    }
}