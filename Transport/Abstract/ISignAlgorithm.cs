using System.IO;

namespace ProtoBuf.Transport.Abstract
{
    /// <summary>
    /// Sign algorithm wrapper interface
    /// </summary>
    public interface ISignAlgorithm
    {
        /// <summary>
        /// Returns sign of given stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        byte[] GetSign(Stream stream);

        /// <summary>
        /// Verifies sign against given stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="signBytes"></param>
        /// <returns>Returns true if matches; false otherwise</returns>
        bool VerifySign(Stream stream, byte[] signBytes);

        /// <summary>
        /// Verifies sign against given stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="signStream"></param>
        /// <returns>Returns true if matches; false otherwise</returns>
        bool VerifySign(Stream stream, Stream signStream);

        /// <summary>
        /// Verifies if hash matches sign stream
        /// </summary>
        /// <param name="md5Hash">MD5 hash string</param>
        /// <param name="signStream">Sign stream</param>
        /// <returns>Returns true if matches; false otherwise</returns>
        bool VerifySign(string md5Hash, Stream signStream);
    }
}