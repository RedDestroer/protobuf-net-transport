using System;
using System.IO;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Creates <see cref="DelegateDataContainer"/> instance
        /// </summary>
        /// <param name="streamAction"></param>
        public DelegateDataContainer(Action<Stream> streamAction)
        {
            if (streamAction == null) throw new ArgumentNullException("streamAction");

            _streamAction = streamAction;
        }

        /// <summary>
        /// Returns stream, contained within
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            var stream = new MemoryStream();
            _streamAction(stream);
            stream.Position = 0;

            return stream;
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