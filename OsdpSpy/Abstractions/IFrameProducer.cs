using System;

namespace OsdpSpy.Abstractions
{
    public interface IFrameProducer
    {
        EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}