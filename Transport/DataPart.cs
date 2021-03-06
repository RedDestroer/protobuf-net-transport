﻿using System;
using System.Diagnostics;
using System.IO;
using ProtoBuf.Transport.Abstract;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Holds information about transported piece of data
    /// </summary>
    [DebuggerDisplay("Headers = {Headers.Count}, Properties = {Properties.Count}")]
    public class DataPart
    {
        private readonly IDataContainer _dataContainer;

        /// <summary>
        /// Creates <see cref="DataPart"/> instance
        /// </summary>
        /// <param name="dataContainer">Container with data stream</param>
        public DataPart(IDataContainer dataContainer)
        {
            if (dataContainer == null) throw new ArgumentNullException("dataContainer");

            _dataContainer = dataContainer;
            Headers = new Headers();
            Properties = new Properties();
        }

        /// <summary>
        /// List of headers
        /// </summary>
        public Headers Headers { get; private set; }

        /// <summary>
        /// List of properties
        /// </summary>
        public Properties Properties { get; private set; }

        /// <summary>
        /// Returns holded data stream
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
            return _dataContainer.GetStream();
        }

        /// <summary>
        /// Writes holded data stream into another stream
        /// </summary>
        /// <param name="stream"></param>
        public void CopyToStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            _dataContainer.CopyToStream(stream);
        }

#if NET35 || NET40 || NET45
        /// <summary>
        /// Deserialize data from stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deserializeFunc"></param>
        /// <returns></returns>
        public T Deserialize<T>(Func<Stream, T> deserializeFunc)
        {
            if (deserializeFunc == null) throw new ArgumentNullException("deserializeFunc");

            T t;
            using (var stream = GetStream())
            {
                t = deserializeFunc(stream);
            }

            return t;
        }
#endif

#if NET20 || NET30
        /// <summary>
        /// Deserialize data from stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deserializeFunc"></param>
        /// <returns></returns>
        public T Deserialize<T>(Func<Stream, T> deserializeFunc)
        {
            if (deserializeFunc == null) throw new ArgumentNullException("deserializeFunc");

            T t;
            using (var stream = GetStream())
            {
                t = deserializeFunc(stream);
            }

            return t;
        }
#endif
    }
}