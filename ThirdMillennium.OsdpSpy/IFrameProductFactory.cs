using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public interface IFrameProductFactory
    {
        IFrameProduct Create(Frame frame);
        IFrameProduct Create(DateTime timestamp, Frame frame);
    }
}