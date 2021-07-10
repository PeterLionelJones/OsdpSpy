using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThirdMillennium.Utility.OSDP
{
    internal class FrameNotifier : ThreadService, IFrameNotifier
    {
        public FrameNotifier(IFrameQueue queue)
        {
            _queue = queue;
        }
        
        private readonly IFrameQueue _queue;

        protected override async Task OnServiceAsync()
        {
            _queue.Flush();
            await base.OnServiceAsync();
        }

        public EventHandler<IFrameProduct> FrameHandler
        {
            get => _queue.FrameHandler; 
            set => _queue.FrameHandler = value;
        }
    }
}