namespace ProtoBuf.Transport
{
#if NET20 || NET30
    /// <summary>
    /// Function delegate
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public delegate TResult Func<out TResult>();

    /// <summary>
    /// Function delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg"></param>
    /// <returns></returns>
    public delegate TResult Func<in T, out TResult>(T arg);
#endif
}