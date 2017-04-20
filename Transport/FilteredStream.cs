using System;
using System.IO;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Декоратор для потока, позволяющий выделить из потока только какой-то фрагмент
    /// на своём диспозе не диспозит underlying поток, который декорирует.
    /// Это сделано осознано и менять это поведение НЕЛЬЗЯ, если нужен фильтрующий поток, 
    /// который диспозит свой подчинённый поток, то используйте DisposableFilteredStream
    /// </summary>
    public class FilteredStream
        : Stream
    {
        private readonly long _length;
        private readonly long _start;

        private long _position;

        public FilteredStream(Stream stream, long start, long length)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (start < 0) throw new ArgumentOutOfRangeException("start", start, "Начало не может быть отрицательным");
            if (length < 0) throw new ArgumentOutOfRangeException("length", length, "Длина не может быть отрицательной");
            if (start > stream.Length) throw new ArgumentOutOfRangeException("start", start, "Начало не может выходить за пределы декорируемого потока");
            if (start + length > stream.Length) throw new InvalidOperationException("Начало плюс длина не могут выходить за пределы декорируемого потока");

            Stream = stream;
            _start = start;
            _length = length;

            if (Stream.Position < start)
                Stream.Position = start;
            if (Stream.Position > start + length)
                Stream.Position = start + length;

            _position = Stream.Position - start;
        }

        public override bool CanRead
        {
            get { return Stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return Stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _position; }
            set
            {
                if (value < 0)
                {
                    _position = 0;
                }
                else if (_length < value)
                {
                    _position = _length;
                }
                else
                {
                    _position = value;
                }

                Stream.Seek(_position + _start, SeekOrigin.Begin);
            }
        }

        protected Stream Stream { get; private set; }

        public override void Flush()
        {
            Stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset < 0)
                throw new InvalidOperationException("Смещение должно быть больше нуля.");

            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset <= _length)
                        _position = offset;
                    else
                        _position = _length;

                    return Stream.Seek(_position + _start, origin);
                case SeekOrigin.Current:
                    if (_position + offset > _length)
                        _position = _length;
                    else
                        _position += offset;

                    return Stream.Seek(offset, origin);
                case SeekOrigin.End:
                    if (_position - offset < 0)
                        _position = 0;
                    else
                        _position -= offset;

                    return Stream.Seek(Stream.Length - (_start + _length) + offset, origin);
            }

            return 0;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");

            int bytesToRead = Math.Min(count, (int)(_length - _position));
            if (bytesToRead == 0)
                return 0;

            int bytesRead = Stream.Read(buffer, offset, bytesToRead);
            _position += bytesRead;

            return bytesRead;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
