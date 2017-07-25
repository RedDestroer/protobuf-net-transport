using System;
using System.IO;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Stream decorator. Allows get only set fragment from base stream
    /// </summary>
    public class FilteredStream
        : Stream
    {
        private readonly long _length;
        private readonly long _start;

        private long _position;

        /// <summary>
        /// Creates <see cref="FilteredStream"/> instance
        /// </summary>
        /// <param name="stream">Base stream</param>
        /// <param name="start">Start of fragment</param>
        /// <param name="length">Length of fragment</param>
        public FilteredStream(Stream stream, long start, long length)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (start < 0) throw new ArgumentOutOfRangeException("start", start, "Start can not be nagative.");
            if (length < 0) throw new ArgumentOutOfRangeException("length", length, "Length can not be negative.");
            if (start > stream.Length) throw new ArgumentOutOfRangeException("start", start, "Start can not lay out of bounds of decorated stream.");
            if (start + length > stream.Length) throw new InvalidOperationException("Start plus length can not lay out of bounds of decorated stream.");

            Stream = stream;
            _start = start;
            _length = length;

            if (Stream.Position < start)
                Stream.Position = start;
            if (Stream.Position > start + length)
                Stream.Position = start + length;

            _position = Stream.Position - start;
        }

        /// <summary>
        /// Can read from stream
        /// </summary>
        public override bool CanRead
        {
            get { return Stream.CanRead; }
        }

        /// <summary>
        /// Can seek at stream
        /// </summary>
        public override bool CanSeek
        {
            get { return Stream.CanSeek; }
        }

        /// <summary>
        /// Can wite to stream
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Length of stream
        /// </summary>
        public override long Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Position at stream
        /// </summary>
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

        /// <summary>
        /// Underlying stream
        /// </summary>
        protected Stream Stream { get; private set; }

        /// <summary>
        /// Flush stream
        /// </summary>
        public override void Flush()
        {
            Stream.Flush();
        }

        /// <summary>
        /// Seek at stream
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Set lenght of stream
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Read bytes from stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Write bytes to stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
