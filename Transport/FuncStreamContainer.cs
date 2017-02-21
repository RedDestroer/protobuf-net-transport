using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public class FuncStreamContainer
        : IStreamContainer
    {
        private readonly Func<Stream> _streamFunc;

        public FuncStreamContainer(Func<Stream> streamFunc)
        {
            if (streamFunc == null) throw new ArgumentNullException("streamFunc");

            _streamFunc = streamFunc;
        }

        public Stream GetStream()
        {
            return _streamFunc();
        }

        public void CopyToStream(Stream output)
        {
            var buffer = BufferProvider.Current.TakeBuffer();
            try
            {
                using (var source = GetStream())
                {
                    int byteCount;
                    while ((byteCount = source.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, byteCount);
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