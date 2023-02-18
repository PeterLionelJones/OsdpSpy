using System;

namespace OsdpSpy.Abstractions;

public interface IExchangeProducer : IFrameConsumer
{
    EventHandler<IExchange> ExchangeHandler { get; set; }
}