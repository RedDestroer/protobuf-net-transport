using System;

#if NET30 || NET35 || NET40 || NET45
using System.Diagnostics.CodeAnalysis;
#endif

namespace ProtoBuf.Transport.Ambient
{
    /// <summary>
    /// Buffer provider
    /// </summary>
#if NET40 || NET45
    [ExcludeFromCodeCoverage]
#endif

    public abstract class BufferProvider
    {
        private static BufferProvider _current;

        /// <summary>
        /// Static constructor
        /// </summary>
        static BufferProvider()
        {
            ResetToDefault();
        }

        /// <summary>
        /// Current buffer provider
        /// </summary>
        public static BufferProvider Current
        {
            get { return _current; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _current = value;
            }
        }

        /// <summary>
        /// Default size of buffers
        /// </summary>
        public int DefaultBufferSize { get; protected set; }

        /// <summary>
        /// Reset buffer provider to defaul behaviour
        /// </summary>
        public static void ResetToDefault()
        {
            if (_current != null)
                _current.Clear();
            _current = new DefaultBufferProvider();
        }

        /// <summary>
        /// Gets buffer from buffer manager
        /// </summary>
        /// <returns></returns>
        public abstract byte[] TakeBuffer();

        /// <summary>
        /// Gets buffer from buffer manager with given size
        /// </summary>
        /// <param name="bufferSize">Size of buffer</param>
        /// <returns></returns>
        public abstract byte[] TakeBuffer(int bufferSize);

        /// <summary>
        /// Returns buffer back to buffer manager
        /// </summary>
        /// <param name="buffer">Buffer to return</param>
        public abstract void ReturnBuffer(byte[] buffer);

        /// <summary>
        /// Clears buffer manager
        /// </summary>
        public abstract void Clear();

#if NET20 || !FEAT_SERVICEMODEL
        internal class DefaultBufferProvider
            : BufferProvider
        {
            private readonly TempBufferManager _bufferManager;

            public DefaultBufferProvider()
            {
                DefaultBufferSize = 65 * 1024;
                _bufferManager = new TempBufferManager();
            }

            public override byte[] TakeBuffer()
            {
                return TakeBuffer(DefaultBufferSize);
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                return _bufferManager.TakeBuffer(bufferSize);
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                _bufferManager.ReturnBuffer(buffer);
            }

            public override void Clear()
            {
                _bufferManager.Clear();
            }
        }

        internal class TempBufferManager
        {
            public byte[] TakeBuffer(int bufferSize)
            {
                return new byte[bufferSize];
            }

            public void ReturnBuffer(byte[] buffer)
            {
            }

            public void Clear()
            {
            }
        }
#endif

#if (NET30 || NET35 || NET40 || NET45) && FEAT_SERVICEMODEL
        internal class DefaultBufferProvider
            : BufferProvider
        {
            private readonly System.ServiceModel.Channels.BufferManager _bufferManager;

            public DefaultBufferProvider()
            {
                DefaultBufferSize = 65 * 1024;
                _bufferManager = System.ServiceModel.Channels.BufferManager.CreateBufferManager(1024, DefaultBufferSize);
            }

            public override byte[] TakeBuffer()
            {
                return TakeBuffer(DefaultBufferSize);
            }

            public override byte[] TakeBuffer(int bufferSize)
            {
                return _bufferManager.TakeBuffer(bufferSize);
            }

            public override void ReturnBuffer(byte[] buffer)
            {
                _bufferManager.ReturnBuffer(buffer);
            }

            public override void Clear()
            {
                _bufferManager.Clear();
            }
        }
#endif
    }
}