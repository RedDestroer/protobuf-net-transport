using System;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    public sealed class TempFileStreamGetter
        : IStreamGetter, IDisposable
    {
        private TempFile _tempFile;

        public TempFileStreamGetter(Stream stream)
        {
            _tempFile = TempFile.Create(stream);
        }

        ~TempFileStreamGetter()
        {
            Dispose();
        }

        public Stream CreateStream()
        {
            return _tempFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
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