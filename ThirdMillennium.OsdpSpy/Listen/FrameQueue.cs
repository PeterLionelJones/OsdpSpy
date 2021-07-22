using System;
using System.Collections.Concurrent;

namespace ThirdMillennium.OsdpSpy
{
    public class FrameQueue : IFrameQueue
    {
        public FrameQueue()
        {
            _queue = new ConcurrentQueue<IFrameProduct>();
        }
        
        private readonly ConcurrentQueue<IFrameProduct> _queue;
        
        public void Add(IFrameProduct product)
        {
            _queue.Enqueue(product);
        }

        public void Flush()
        {
            while (_queue.TryDequeue(out var product))
            {
                FrameHandler?.Invoke(this, product);
            }
        }

        public EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}