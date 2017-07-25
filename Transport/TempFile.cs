using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    /// <summary>
    /// Temp file for temp operations
    /// </summary>
    public sealed class TempFile
        : IDisposable
    {
        private readonly string _fullFileName;
        private bool _disposed;

        /// <summary>
        /// Creates <see cref="TempFile"/> instance. Creates new temp file on filesystem.
        /// </summary>
        public TempFile()
        {
            _fullFileName = TempProvider.Current.GetTempFullFileName();
            CreateIfNotExists(_fullFileName);
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> instance. Creates new temp file on filesystem at given directory
        /// </summary>
        /// <param name="tempFullDirName">Full directory name</param>
        public TempFile(string tempFullDirName)
        {
            if (tempFullDirName == null) throw new ArgumentNullException("tempFullDirName");

            _fullFileName = TempProvider.Current.GetTempFullFileName(tempFullDirName);
            CreateIfNotExists(_fullFileName);
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> instance. Creates new temp file on filesystem at given directory with given name prefix
        /// </summary>
        /// <param name="tempFullDirName">Full directory name</param>
        /// <param name="fileNamePrefix">Prefix of file</param>
        public TempFile(string tempFullDirName, string fileNamePrefix)
        {
            if (tempFullDirName == null) throw new ArgumentNullException("tempFullDirName");
            if (fileNamePrefix == null) throw new ArgumentNullException("fileNamePrefix");

            _fullFileName = TempProvider.Current.GetTempFullFileName(tempFullDirName, fileNamePrefix);
            CreateIfNotExists(_fullFileName);
        }

        private TempFile(string fullFileName, bool isFile) // isFile needs only to separate signatures
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");

            _fullFileName = fullFileName;
            CreateIfNotExists(_fullFileName);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~TempFile()
        {
            Dispose();
        }

        /// <summary>
        /// Full file name of temp file
        /// </summary>
        public string FullFileName { get { return _fullFileName; } }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get { return Path.GetFileName(_fullFileName); } }

        /// <summary>
        /// File information
        /// </summary>
        public FileInfo FileInfo { get { return new FileInfo(FullFileName); } }

        /// <summary>
        /// If true - file exists; if false - not
        /// </summary>
        public bool IsExists { get { return FileInfo.Exists; } }

        /// <summary>
        /// Is file is read only
        /// </summary>
        public bool IsReadOnly { get { return FileInfo.IsReadOnly; } }

        /// <summary>
        /// Length of file
        /// </summary>
        public long Length { get { return FileInfo.Length; } }

        /// <summary>
        /// Creates <see cref="TempFile"/> from given tempFullDirName
        /// </summary>
        /// <param name="stream">A stream of data written to a temporary file</param>
        /// <returns></returns>
        public static TempFile Create(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return Create(new TempFile(), stream);
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> from given stream at given directory
        /// </summary>
        /// <param name="stream">A stream of data written to a temporary file</param>
        /// <param name="tempFullDirName">Full directory name</param>
        /// <returns></returns>
        public static TempFile Create(Stream stream, string tempFullDirName)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (tempFullDirName == null) throw new ArgumentNullException("tempFullDirName");

            return Create(new TempFile(tempFullDirName), stream);
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> from given stream at given directory with given file name prefix
        /// </summary>
        /// <param name="stream">A stream of data written to a temporary file</param>
        /// <param name="tempFullDirName">Full directory name</param>
        /// <param name="fileNamePrefix">Prefix of file</param>
        /// <returns></returns>
        public static TempFile Create(Stream stream, string tempFullDirName, string fileNamePrefix)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (tempFullDirName == null) throw new ArgumentNullException("tempFullDirName");
            if (fileNamePrefix == null) throw new ArgumentNullException("fileNamePrefix");

            return Create(new TempFile(tempFullDirName, fileNamePrefix), stream);
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> from given file
        /// </summary>
        /// <param name="fullFileName">Source file full name</param>
        /// <returns></returns>
        public static TempFile Create(string fullFileName)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");

            using (var stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Create(stream);
            }
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> from given file at given directory
        /// </summary>
        /// <param name="fullFileName">Source file full name</param>
        /// <param name="tempFullDirName"></param>
        /// <returns></returns>
        public static TempFile Create(string fullFileName, string tempFullDirName)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");
            if (tempFullDirName == null) throw new ArgumentNullException("tempFullDirName");

            using (var stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Create(stream, tempFullDirName);
            }
        }

        /// <summary>
        /// Creates <see cref="TempFile"/> from given file at given directory with given file name prefix
        /// </summary>
        /// <param name="fullFileName">Source file full name</param>
        /// <param name="tempFullDirName"></param>
        /// <param name="fileNamePrefix"></param>
        /// <returns></returns>
        public static TempFile Create(string fullFileName, string tempFullDirName, string fileNamePrefix)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");
            if (tempFullDirName == null) throw new ArgumentNullException("tempFullDirName");
            if (fileNamePrefix == null) throw new ArgumentNullException("fileNamePrefix");

            using (var stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Create(stream, tempFullDirName, fileNamePrefix);
            }
        }

        /// <summary>
        /// Sets given file as <see cref="TempFile"/>, but not changes it's name
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static TempFile CreateAsTempFile(string fullFileName)
        {
            return new TempFile(fullFileName, true);
        }

        /// <summary>
        /// Opens temp file for read
        /// </summary>
        /// <param name="fileMode">File mode</param>
        /// <returns>Returns open stream of temp file</returns>
        public Stream Open(FileMode fileMode)
        {
            return new FileStream(FullFileName, fileMode);
        }

        /// <summary>
        /// Opens temp file for read
        /// </summary>
        /// <param name="fileMode">File mode</param>
        /// <param name="fileAccess">File access</param>
        /// <returns>Returns open stream of temp file</returns>
        public Stream Open(FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(FullFileName, fileMode, fileAccess);
        }

        /// <summary>
        /// Opens temp file for read
        /// </summary>
        /// <param name="fileMode">File mode</param>
        /// <param name="fileAccess">File access</param>
        /// <param name="fileShare">File share</param>
        /// <returns>Returns open stream of temp file</returns>
        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new FileStream(FullFileName, fileMode, fileAccess, fileShare);
        }

        /// <summary>
        /// Opens temp file for read
        /// </summary>
        /// <param name="fileMode">File mode</param>
        /// <param name="fileAccess">File access</param>
        /// <param name="fileShare">File share</param>
        /// <param name="buffer">Buffer size</param>
        /// <returns>Returns open stream of temp file</returns>
        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int buffer)
        {
            return new FileStream(FullFileName, fileMode, fileAccess, fileShare, buffer);
        }

        /// <summary>
        /// Opens temp file for read
        /// </summary>
        /// <param name="fileMode">File mode</param>
        /// <param name="fileAccess">File access</param>
        /// <param name="fileShare">File share</param>
        /// <param name="buffer">Buffer size</param>
        /// <param name="fileOptions">File options</param>
        /// <returns>Returns open stream of temp file</returns>
        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int buffer, FileOptions fileOptions)
        {
            return new FileStream(FullFileName, fileMode, fileAccess, fileShare, buffer, fileOptions);
        }

        /// <summary>
        /// Opens temp file as UTF8 <see cref="StreamReader"/>
        /// </summary>
        /// <returns></returns>
        public StreamReader OpenText()
        {
            return File.OpenText(FileName);
        }

        /// <summary>
        /// Open temp file for write
        /// </summary>
        /// <returns></returns>
        public Stream OpenWrite()
        {
            return File.OpenWrite(FullFileName);
        }

        /// <summary>
        /// Dispose temp file, and delete it from filesystem
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            bool deleted = false;
            try
            {
                if (File.Exists(_fullFileName))
                    File.Delete(_fullFileName);

                deleted = true;
            }
            catch
            {
                Thread.Sleep(500);
            }

            if (!deleted)
                Trace.Write(string.Format("Can't delete temporary file '{0}' because it blocked.", _fullFileName));

            GC.SuppressFinalize(this);
            _disposed = true;
        }

        private static TempFile Create(TempFile tempFile, Stream stream)
        {
            using (var output = new FileStream(tempFile.FullFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
#if NET20 || NET30 || NET35
                var buffer = BufferProvider.Current.TakeBuffer();
                try
                {
                    int byteCount;
                    while ((byteCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, byteCount);
                    }
                }
                finally
                {
                    BufferProvider.Current.ReturnBuffer(buffer);
                }

                output.Flush();
#endif

#if NET40 || NET45
                stream.CopyTo(output);
                output.Flush(true);
#endif
            }

            return tempFile;
        }

        private void CreateIfNotExists(string fullFileName)
        {
            if (!File.Exists(fullFileName))
            {
                var directoryName = Path.GetDirectoryName(fullFileName);
                if (directoryName != null && !Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                File.WriteAllBytes(fullFileName, new byte[] { });
            }
        }
    }
}