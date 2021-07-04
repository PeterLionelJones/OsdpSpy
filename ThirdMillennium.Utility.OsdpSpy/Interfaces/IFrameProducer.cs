using System;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameProducer
    {
        EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}