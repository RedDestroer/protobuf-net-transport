using System;

#if NET30 || NET35 || NET40 || NET45
using System.Diagnostics.CodeAnalysis;
#endif

namespace ProtoBuf.Transport.Ambient
{
    /// <summary>
    /// Provider of time
    /// </summary>
#if NET40 || NET45
    [ExcludeFromCodeCoverage]
#endif
    public abstract class TimeProvider
    {
        private static TimeProvider _current;

        /// <summary>
        /// Static constructor
        /// </summary>
        static TimeProvider()
        {
            ResetToDefault();
        }

        /// <summary>
        /// Current <see cref="TimeProvider"/> instance
        /// </summary>
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

        /// <summary>
        /// Current <see cref="DateTime"/> in UTC
        /// </summary>
        public abstract DateTime UtcNow { get; }

        /// <summary>
        /// Current <see cref="DateTime"/>
        /// </summary>
        public abstract DateTime Now { get; }

        /// <summary>
        /// Resets <see cref="TimeProvider"/> to default behaviour
        /// </summary>
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