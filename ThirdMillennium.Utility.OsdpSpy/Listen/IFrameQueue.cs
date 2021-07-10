using System;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameQueue
    {
        void Add(IFrameProduct product);
        void Flush();
        public EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}