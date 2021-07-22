using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class FrameProductFactory : IFrameProductFactory
    {
        public IFrameProduct Create(Frame frame)
            => new FrameProduct(frame);

        public IFrameProduct Create(DateTime timestamp, Frame frame)
            => new FrameProduct(timestamp, frame);
    }
}