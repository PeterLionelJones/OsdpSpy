using System;
using OsdpSpy.Osdp;

namespace OsdpSpy.Listen
{
    public interface IFrameReceiver : IThreadService
    {
        void SetRate(int rate);
        EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
    }
}