using System;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameNotifier : IThreadService
    {
        EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}