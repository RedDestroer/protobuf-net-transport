using System;
using System.Diagnostics;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    [DebuggerDisplay("Headers = {Headers.Count}, Properties = {Properties.Count}")]
    public class DataPart
    {
        private readonly IStreamContainer _streamContainer;

        public DataPart(IStreamContainer streamContainer)
        {
            if (streamContainer == null) throw new ArgumentNullException("streamContainer");

            _streamContainer = streamContainer;
            Headers = new Headers();
            Properties = new Properties();
        }

        public Headers Headers { get; private set; }

        public Properties Properties { get; private set; }

        public Stream GetStream()
        {
            return _streamContainer.GetStream();
        }

        public void CopyToStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _streamContainer.CopyToStream(stream);
        }
    }
}