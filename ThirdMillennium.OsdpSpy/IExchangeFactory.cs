namespace ThirdMillennium.OsdpSpy
{
    public interface IExchangeFactory
    {
        IExchange Create(long seq, IFrameProduct tx);
    }
}