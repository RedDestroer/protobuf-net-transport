using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public class FuncStreamGetter
        : IStreamGetter
    {
        private readonly Func<Stream> _streamFunc;

        public FuncStreamGetter(Func<Stream> streamFunc)
        {
            if (streamFunc == null) throw new ArgumentNullException("streamFunc");

            _streamFunc = streamFunc;
        }

        public Stream CreateStream()
        {
            return _streamFunc();
        }
    }
}