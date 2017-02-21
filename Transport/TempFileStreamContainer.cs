using System;
using System.IO;
using ProtoBuf.Transport.Abstract;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public sealed class TempFileStreamContainer
        : IStreamContainer, IDisposable
    {
        private TempFile _tempFile;

        public TempFileStreamContainer(Stream stream)
        {
            _tempFile = TempFile.Create(stream);
        }

        ~TempFileStreamContainer()
        {
            Dispose();
        }

        public Stream GetStream()
        {
            return _tempFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
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