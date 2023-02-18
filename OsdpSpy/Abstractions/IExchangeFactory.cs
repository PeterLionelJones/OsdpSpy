using OsdpSpy.Abstractions;

namespace OsdpSpy
{
    public interface IExchangeFactory
    {
        IExchange Create(long seq, IFrameProduct tx);
    }
}