namespace ProtoBuf.Transport
{
#if NET20 || NET30
    public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T arg);
#endif
}