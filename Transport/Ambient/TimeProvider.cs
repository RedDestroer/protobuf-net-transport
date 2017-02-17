using System;

#if NET30 || NET35 || NET40 || NET45
using System.Diagnostics.CodeAnalysis;
#endif

namespace ProtoBuf.Transport.Ambient
{
#if NET40 || NET45
    [ExcludeFromCodeCoverage]
#endif
    public abstract class TimeProvider
    {
        private static TimeProvider _current;

        static TimeProvider()
        {
            ResetToDefault();
        }

        public static TimeProvider Current
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

        public abstract DateTime UtcNow { get; }
        public abstract DateTime Now { get; }

        public static void ResetToDefault()
        {
            _current = new DefaultTimeProvider();
        }

        internal class DefaultTimeProvider
            : TimeProvider
        {
            public override DateTime UtcNow
            {
                get { return DateTime.UtcNow; }
            }

            public override DateTime Now
            {
                get { return DateTime.Now; }
            }
        }
    }
}