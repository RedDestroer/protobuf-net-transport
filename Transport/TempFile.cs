using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ProtoBuf.Transport.Ambient;

namespace ProtoBuf.Transport
{
    public sealed class TempFile
        : IDisposable
    {
        private readonly string _fullFileName;
        private bool _disposed;

        public TempFile()
        {
            _fullFileName = TempProvider.Current.GetTempFullFileName();
            CreateIfNotExists(_fullFileName);
        }

        public TempFile(string tempDirName)
        {
            if (tempDirName == null) throw new ArgumentNullException("tempDirName");

            _fullFileName = TempProvider.Current.GetTempFullFileName(tempDirName);
            CreateIfNotExists(_fullFileName);
        }

        public TempFile(string tempDirName, string fileNamePrefix)
        {
            if (tempDirName == null) throw new ArgumentNullException("tempDirName");
            if (fileNamePrefix == null) throw new ArgumentNullException("fileNamePrefix");

            _fullFileName = TempProvider.Current.GetTempFullFileName(tempDirName, fileNamePrefix);
            CreateIfNotExists(_fullFileName);
        }

        private TempFile(string fullFileName, bool isFile) // isFile нужен только чтобы разнести сигнатуры
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");

            _fullFileName = fullFileName;
            CreateIfNotExists(_fullFileName);
        }

        ~TempFile()
        {
            Dispose();
        }

        public string FullFileName { get { return _fullFileName; } }
        public string FileName { get { return Path.GetFileName(_fullFileName); } }
        public FileInfo FileInfo { get { return new FileInfo(FullFileName); } }
        public bool IsExists { get { return FileInfo.Exists; } }
        public bool IsReadOnly { get { return FileInfo.IsReadOnly; } }
        public long Length { get { return FileInfo.Length; } }

        public static TempFile Create(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            return Create(new TempFile(), stream);
        }

        public static TempFile Create(Stream stream, string tempDirName)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (tempDirName == null) throw new ArgumentNullException("tempDirName");

            return Create(new TempFile(tempDirName), stream);
        }

        public static TempFile Create(Stream stream, string tempDirName, string fileNamePrefix)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            if (tempDirName == null) throw new ArgumentNullException("tempDirName");
            if (fileNamePrefix == null) throw new ArgumentNullException("fileNamePrefix");

            return Create(new TempFile(tempDirName, fileNamePrefix), stream);
        }

        public static TempFile Create(string fullFileName)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");

            using (var stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Create(stream);
            }
        }

        public static TempFile Create(string fullFileName, string tempDirName)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");
            if (tempDirName == null) throw new ArgumentNullException("tempDirName");

            using (var stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Create(stream, tempDirName);
            }
        }

        public static TempFile Create(string fullFileName, string tempDirName, string fileNamePrefix)
        {
            if (fullFileName == null) throw new ArgumentNullException("fullFileName");
            if (tempDirName == null) throw new ArgumentNullException("tempDirName");
            if (fileNamePrefix == null) throw new ArgumentNullException("fileNamePrefix");

            using (var stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Create(stream, tempDirName, fileNamePrefix);
            }
        }

        /// <summary>
        /// Оборачивает существуещий файл, делая его временным, но не меняя его имени
        /// </summary>
        /// <param name="fullFileName"></param>
        /// <returns></returns>
        public static TempFile CreateAsTempFile(string fullFileName)
        {
            return new TempFile(fullFileName, true);
        }

        public Stream Open(FileMode fileMode)
        {
            return new FileStream(FullFileName, fileMode);
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(FullFileName, fileMode, fileAccess);
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            return new FileStream(FullFileName, fileMode, fileAccess, fileShare);
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int buffer)
        {
            return new FileStream(FullFileName, fileMode, fileAccess, fileShare, buffer);
        }

        public Stream Open(FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int buffer, FileOptions fileOptions)
        {
            return new FileStream(FullFileName, fileMode, fileAccess, fileShare, buffer, fileOptions);
        }

        public StreamReader OpenText()
        {
            return File.OpenText(FileName);
        }

        public Stream OpenWrite()
        {
            return File.OpenWrite(FullFileName);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            bool deleted = false;
            int i = 0;
            while (i < 5)
            {
                try
                {
                    if (File.Exists(_fullFileName))
                        File.Delete(_fullFileName);

                    deleted = true;
                    break;
                }
                catch
                {
                    i++;
                    Thread.Sleep(500);
                }
            }

            if (!deleted)
                Trace.Write(string.Format("Не удалось удалить временный файл '{0}' так как он всё ещё заблокирован.", _fullFileName));

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