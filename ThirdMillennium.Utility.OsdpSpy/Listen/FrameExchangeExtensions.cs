using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public static class FrameExchangeExtensions
    {
        public static bool IsPollAckPair(this IExchange exchange)
            => exchange.Pd != null &&
               exchange.Acu.Frame.Command == Command.POLL &&
               exchange.Pd.Frame.Reply == Reply.ACK &&
               exchange.Acu.Frame.Address == exchange.Pd.Frame.Address &&
               exchange.Acu.Frame.Sequence == exchange.Pd.Frame.Sequence;

        public static bool IsNoResponse(this IExchange exchange)
            => exchange.Pd == null;
    }
}