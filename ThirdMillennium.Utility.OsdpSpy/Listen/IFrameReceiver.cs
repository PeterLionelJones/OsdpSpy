using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameReceiver : IThreadService
    {
        void SetRate(int rate);
        EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
    }
}