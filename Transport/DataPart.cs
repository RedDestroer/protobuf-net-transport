using System;
using System.Diagnostics;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    [DebuggerDisplay("Headers = {Headers.Count}, Properties = {Properties.Count}")]
    public class DataPart
    {
        private readonly IDataContainer _dataContainer;

        public DataPart(IDataContainer dataContainer)
        {
            if (dataContainer == null) throw new ArgumentNullException("dataContainer");

            _dataContainer = dataContainer;
            Headers = new Headers();
            Properties = new Properties();
        }

        public Headers Headers { get; private set; }

        public Properties Properties { get; private set; }

        public void CopyToStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _dataContainer.CopyToStream(stream);
        }
    }
}