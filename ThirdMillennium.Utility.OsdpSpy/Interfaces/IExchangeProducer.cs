using System;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IExchangeProducer : IFrameConsumer
    {
        EventHandler<IExchange> ExchangeHandler { get; set; }
    }
}