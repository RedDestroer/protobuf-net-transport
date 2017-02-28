using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Holds reference to delegate, which is called at CopyToStream method.
    /// </summary>
    public class DelegateDataContainer
        : IDataContainer
    {
        private readonly Action<Stream> _streamAction;

        public DelegateDataContainer(Action<Stream> streamAction)
        {
            if (streamAction == null) throw new ArgumentNullException("streamAction");

            _streamAction = streamAction;
        }

        /// <summary>
        /// Copy contained inner data into the output stream.
        /// </summary>
        /// <param name="stream"></param>
        public void CopyToStream(Stream stream)
        {
            _streamAction(stream);
        }
    }
}