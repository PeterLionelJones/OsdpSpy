using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public interface IBusFrameProducer : IFrameProducer
    {
        bool IsRunning { get; }
        void SetRate(int rate);
        void Start();
        void Stop();
        EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
    }
}