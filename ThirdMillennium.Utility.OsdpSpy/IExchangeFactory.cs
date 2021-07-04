namespace ThirdMillennium.Utility.OSDP
{
    public interface IExchangeFactory
    {
        IExchange Create(long seq, IFrameProduct tx);
    }
}