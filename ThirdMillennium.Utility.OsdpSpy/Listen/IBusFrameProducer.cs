using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IBusFrameProducer : IFrameProducer
    {
        bool Start();
        void Stop();
        EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
    }
}