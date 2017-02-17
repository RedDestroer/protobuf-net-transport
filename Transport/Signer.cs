using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public static class Signer
    {
        public static void Sign(uint prefixSize, ISignAlgorithm signAlgorithm, Stream sourceStream, Stream destinationStream)
        {
            if (signAlgorithm == null) throw new ArgumentNullException("signAlgorithm");
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

                    uint signInfoAddress = GetAddress(bw);
                    bw.Write(0); // Protected data size
                    bw.Write(0); // Sign size

                    uint dataSizeAddress = GetAddress(bw) - 8;

                    while ((byteCount = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        wrapper.Write(buffer, 0, byteCount);
                    }

                    wrapper.Flush();
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

        public static bool IsSignMatch(uint prefixSize, ISignAlgorithm signAlgorithm, Stream stream)
        {
            if (signAlgorithm == null) throw new ArgumentNullException("signAlgorithm");
            if (stream == null) throw new ArgumentNullException("stream");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream doesn't support reading.");

            long position = stream.Position;

            stream.Seek(prefixSize, SeekOrigin.Current);
            int isSignedByte = stream.ReadByte();
            if (isSignedByte == 0)
                throw new InvalidOperationException("Stream is not signed.");

            using (var wrapper = new NonClosingStreamWrapper(stream)) // To prevent stream from closing by BinaryReader
            using (var br = new BinaryReader(wrapper))
            {
                uint protectedDataSize = br.ReadUInt32(); // Protected data size
                uint signSize = br.ReadUInt32(); // Sign size

                using (var signStream = new FilteredStream(wrapper, wrapper.Position + protectedDataSize, signSize))
                {
                    using (var dataStream = new FilteredStream(wrapper, wrapper.Position, protectedDataSize - 8))
                    {
                        var result = signAlgorithm.VerifySign(dataStream, signStream);

                        wrapper.Position = position;

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
                using (var wrapper = new NonClosingStreamWrapper(sourceStream)) // To prevent stream from closing by BinaryReader
                using (var br = new BinaryReader(wrapper))
                {
                    int byteCount;
                    uint prefixBytesLeft = prefixSize;
                    while ((byteCount = wrapper.Read(buffer, 0, (int)Math.Min(buffer.Length, prefixBytesLeft))) > 0)
                    {
                        destinationStream.Write(buffer, 0, byteCount);
                        prefixBytesLeft -= (uint)byteCount;
                        if (prefixBytesLeft == 0)
                            break;
                    }

                    uint protectedDataSize = br.ReadUInt32(); // Protected data size

                    // ReSharper disable once UnusedVariable
                    uint signSize = br.ReadUInt32(); // Sign size

                    destinationStream.WriteByte(0);
                    wrapper.Seek(4 + 4, SeekOrigin.Current);

                    while ((byteCount = wrapper.Read(buffer, 0, (int)Math.Min(buffer.Length, protectedDataSize))) > 0)
                    {
                        destinationStream.Write(buffer, 0, byteCount);
                        protectedDataSize -= (uint)byteCount;
                        if (protectedDataSize == 0)
                            break;
                    }
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