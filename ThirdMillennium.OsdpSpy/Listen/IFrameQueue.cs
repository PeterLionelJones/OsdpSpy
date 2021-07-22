using System;

namespace ThirdMillennium.OsdpSpy
{
    public interface IFrameQueue
    {
        void Add(IFrameProduct product);
        void Flush();
        public EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}