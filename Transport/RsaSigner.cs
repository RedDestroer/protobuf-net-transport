using System;
using System.IO;

namespace ProtoBuf.Transport
{
    public static class RsaSigner
    {
        public static void Sign(uint prefixSize, Stream sourceStream, string keyPair, Stream destinationStream)
        {
            if (sourceStream == null) throw new ArgumentNullException("sourceStream");
            if (keyPair == null) throw new ArgumentNullException("keyPair");
            if (destinationStream == null) throw new ArgumentNullException("destinationStream");

            if (!sourceStream.CanSeek)
                throw new InvalidOperationException("Source stream doesn't support seeking.");
            if (!sourceStream.CanRead)
                throw new InvalidOperationException("Source stream doesn't support reading.");
            if (!destinationStream.CanSeek)
                throw new InvalidOperationException("Destination stream doesn't support seeking.");
            if (!destinationStream.CanWrite)
                throw new InvalidOperationException("Destination stream doesn't support writing.");

            sourceStream.Seek(prefixSize, SeekOrigin.Current);
            int isSignedByte = sourceStream.ReadByte();
            if (isSignedByte == 1)
                throw new InvalidOperationException("Source stream already signed.");

            sourceStream.Seek(-1*(prefixSize + 1), SeekOrigin.Current);

            throw new NotImplementedException();
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

        public static bool IsSignMatch(uint prefixSize, Stream stream, string publicKey)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (publicKey == null) throw new ArgumentNullException("publicKey");

            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream doesn't support seeking.");
            if (!stream.CanRead)
                throw new InvalidOperationException("Stream doesn't support reading.");

            stream.Seek(prefixSize, SeekOrigin.Current);
            int isSignedByte = stream.ReadByte();
            if (isSignedByte == 0)
                throw new InvalidOperationException("Stream is not signed.");

            throw new NotImplementedException();
        }

        public static void RemoveSign(uint prefixSize, Stream sourceStream, Stream destinationStream)
        {
            throw new NotImplementedException();
        }
    }
}