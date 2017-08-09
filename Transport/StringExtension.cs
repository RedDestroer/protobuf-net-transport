using System;

namespace ProtoBuf.Transport
{
#if NET20 || NET30 || NET35
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Is string null or whitespace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }
    }
#endif
}