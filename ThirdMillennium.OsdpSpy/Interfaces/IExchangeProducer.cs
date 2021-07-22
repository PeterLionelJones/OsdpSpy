using System;

namespace ThirdMillennium.OsdpSpy
{
    public interface IExchangeProducer : IFrameConsumer
    {
        EventHandler<IExchange> ExchangeHandler { get; set; }
    }
}