using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public class DataPart
    {
        private readonly IStreamGetter _streamGetter;

        public DataPart(IStreamGetter streamGetter)
        {
            if (streamGetter == null) throw new ArgumentNullException("streamGetter");

            _streamGetter = streamGetter;
            Headers = new Headers();
            Properties = new Properties();
        }

        public Headers Headers { get; private set; }

        public Properties Properties { get; private set; }

        public Stream CreateStream()
        {
            return _streamGetter.CreateStream();
        }
    }
}