using System;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Listen
{
    public interface IFrameQueue
    {
        void Add(IFrameProduct product);
        void Flush();
        public EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}