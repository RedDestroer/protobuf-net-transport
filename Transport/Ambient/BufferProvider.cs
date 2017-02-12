using System;

#if NET30 || NET35 || NET40 || NET45
using System.Diagnostics.CodeAnalysis;
#endif

namespace ProtoBuf.Transport.Ambient
{
#if NET30 || NET35 || NET40 || NET45
    [ExcludeFromCodeCoverage]
#endif

    public abstract class BufferProvider
    {
        private static BufferProvider _current;

        static BufferProvider()
        {
            ResetToDefault();
        }

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

        public int DefaultBufferSize { get; protected set; }

        public static void ResetToDefault()
        {
            if (_current != null)
                _current.Clear();
            _current = new DefaultBufferProvider();
        }

        public abstract byte[] TakeBuffer();
        public abstract byte[] TakeBuffer(int bufferSize);
        public abstract void ReturnBuffer(byte[] buffer);
        public abstract void Clear();

#if NET20
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

#if NET30 || NET35 || NET40 || NET45
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