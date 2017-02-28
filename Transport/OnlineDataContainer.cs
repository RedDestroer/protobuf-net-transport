using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// It contains a reference to the stream, which will be copied to output. Assumes that contained stream is not closed before CopyStream has called. Assumes that stream is seekable.
    /// </summary>
    public class OnlineDataContainer
        : IDataContainer
    {
        private readonly Stream _stream;

        /// <summary>
        /// Creates instance of <see cref="OnlineDataContainer"/>
        /// </summary>
        /// <param name="stream"></param>
        public OnlineDataContainer(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _stream = stream;
        }

        /// <summary>
        /// Copy contained inner data into the output stream.
        /// </summary>
        /// <param name="stream"></param>
        public void CopyToStream(Stream stream)
        {
            var buffer = BufferProvider.Current.TakeBuffer();
            try
            {
                _stream.Position = 0;
                int byteCount;
                while ((byteCount = _stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, byteCount);
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