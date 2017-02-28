using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// It contains a reference to the function, which creates a stream
    /// </summary>
    public class FuncDataContainer
        : IDataContainer
    {
        private readonly Func<Stream> _streamFunc;

        /// <summary>
        /// Creates instance of <see cref="FuncDataContainer"/>
        /// </summary>
        /// <param name="streamFunc"></param>
        public FuncDataContainer(Func<Stream> streamFunc)
        {
            if (streamFunc == null) throw new ArgumentNullException("streamFunc");

            _streamFunc = streamFunc;
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
                using (var source = _streamFunc())
                {
                    int byteCount;
                    while ((byteCount = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, byteCount);
                    }
                }
            }
            finally
            {
                BufferProvider.Current.ReturnBuffer(buffer);
            }
        }
    }
}