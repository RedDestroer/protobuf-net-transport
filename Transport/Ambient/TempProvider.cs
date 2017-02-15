using System;
using System.IO;
using System.Text.RegularExpressions;

#if NET30 || NET35 || NET40 || NET45
using System.Diagnostics.CodeAnalysis;
#endif

namespace ProtoBuf.Transport.Ambient
{
#if NET40 || NET45
    [ExcludeFromCodeCoverage]
#endif
    public abstract class TempProvider
    {
        private static TempProvider _current;

        static TempProvider()
        {
            ResetToDefault();
        }

        public static TempProvider Current
        {
            get
            {
                return _current;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _current = value;
            }
        }

        public static void ResetToDefault()
        {
            _current = new DefaultTempProvider();
        }

        public abstract string GetTempFullDirName();
        public abstract string GetTempFullFileName();
        public abstract string GetTempFullFileName(string fullDirName);
        public abstract string GetTempFullFileName(string fullDirName, string fileNamePrefix);

        internal class DefaultTempProvider
            : TempProvider
        {
            private readonly Regex _cleanerRegex;

            public DefaultTempProvider()
            {
                string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                _cleanerRegex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            }

            public override string GetTempFullDirName()
            {
                string answer = Path.GetTempPath();

                if (string.IsNullOrEmpty(answer))
                    answer = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (string.IsNullOrEmpty(answer))
                    answer = @"C:\";

                return answer;
            }

            public override string GetTempFullFileName()
            {
                return Path.GetTempFileName();
            }

            public override string GetTempFullFileName(string fullDirName)
            {
                return GetTempFullFileName(fullDirName, string.Empty);
            }

            public override string GetTempFullFileName(string fullDirName, string fileNamePrefix)
            {
                string fileName = Escape(fileNamePrefix) + "." + Guid.NewGuid().ToString().Replace("-", string.Empty);
                return Path.Combine(fullDirName, fileName);
            }

            private string Escape(string s)
            {
                return _cleanerRegex.Replace(s, string.Empty);
            }
        }
    }
}