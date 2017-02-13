namespace ProtoBuf.Transport
{
#if NET20 || NET30
    public delegate TResult Func<out TResult>();
#endif
}