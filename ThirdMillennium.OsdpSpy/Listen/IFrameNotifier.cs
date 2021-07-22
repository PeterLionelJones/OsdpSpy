using System;

namespace ThirdMillennium.OsdpSpy
{
    public interface IFrameNotifier : IThreadService
    {
        EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}