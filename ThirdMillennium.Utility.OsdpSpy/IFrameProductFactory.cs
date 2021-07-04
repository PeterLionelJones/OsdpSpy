using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameProductFactory
    {
        IFrameProduct Create(Frame frame);
        IFrameProduct Create(DateTime timestamp, Frame frame);
    }
}