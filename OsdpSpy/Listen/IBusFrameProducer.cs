using System;
using OsdpSpy.Abstractions;
using OsdpSpy.Osdp;

namespace OsdpSpy.Listen
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