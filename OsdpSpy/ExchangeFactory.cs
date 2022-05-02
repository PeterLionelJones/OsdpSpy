using OsdpSpy.Abstractions;
using OsdpSpy.Models;

namespace OsdpSpy
{
    public class ExchangeFactory : IExchangeFactory
    {
        public IExchange Create(long seq, IFrameProduct tx)
            => new Exchange { Sequence = seq, Acu = tx };
    }
}