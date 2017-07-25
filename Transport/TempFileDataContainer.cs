using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// It copies given stream into temporary file and then given stream can be closed without harm to the data. CopyToStream takes data from that temporary file. Temporary file will be deleted on Dispose().
    /// </summary>
    public sealed class TempFileDataContainer
        : IDataContainer, IDisposable
    {
        private TempFile _tempFile;

        /// <summary>
        /// Creates instance of <see cref="TempFileDataContainer"/>
        /// </summary>
        /// <param name="stream"></param>
        public TempFileDataContainer(Stream stream)
        {
            _tempFile = TempFile.Create(stream);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~TempFileDataContainer()
        {
            Dispose();
        }

        /// <summary>
        /// Returns stream, contained within
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            return _tempFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
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
                using (var source = _tempFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var tf = _tempFile;
            if (tf != null)
            {
                tf.Dispose();
            }

            _tempFile = null;
        }
    }
}