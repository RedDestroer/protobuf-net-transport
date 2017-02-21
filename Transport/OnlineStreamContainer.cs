using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public class OnlineStreamContainer
        : IStreamContainer
    {
        private readonly Stream _stream;

        public OnlineStreamContainer(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _stream = stream;
        }

        public Stream GetStream()
        {
            _stream.Position = 0;

            return _stream;
        }

        public void CopyToStream(Stream output)
        {
            var buffer = BufferProvider.Current.TakeBuffer();
            try
            {
                _stream.Position = 0;
                int byteCount;
                while ((byteCount = _stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, byteCount);
                }
            }
            finally
            {
                BufferProvider.Current.ReturnBuffer(buffer);
                _stream.Position = 0;
            }
        }
    }
}