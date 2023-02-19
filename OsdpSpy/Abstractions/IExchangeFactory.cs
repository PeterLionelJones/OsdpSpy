namespace OsdpSpy.Abstractions;

public interface IExchangeFactory
{
    IExchange Create(long seq, IFrameProduct tx);
}