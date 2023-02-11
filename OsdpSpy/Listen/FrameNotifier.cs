using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Listen
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