using OsdpSpy.Abstractions;
using OsdpSpy.Models;

namespace OsdpSpy
{
    public class ExchangeFactory : IExchangeFactory
    {
        public IExchange Create(long seq, IFrameProduct tx)
            => Exchange.Create(seq, tx);
    }
}