using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public class OnlineStreamGetter
        : IStreamGetter
    {
        private readonly Stream _stream;

        public OnlineStreamGetter(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _stream = stream;
        }

        public Stream CreateStream()
        {
            _stream.Position = 0;

            return _stream;
        }
    }
}