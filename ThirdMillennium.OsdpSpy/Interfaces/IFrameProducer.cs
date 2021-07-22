using System;

namespace ThirdMillennium.OsdpSpy
{
    public interface IFrameProducer
    {
        EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}