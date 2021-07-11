using System;
using System.Threading;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class BusFrameProducer : IBusFrameProducer
    {
        public BusFrameProducer(IFrameReceiver receiver, IFrameNotifier notifier)
        {
            _receiver = receiver;
            _notifier = notifier;
        }

        private readonly IFrameReceiver _receiver;
        private readonly IFrameNotifier _notifier;

        private CancellationTokenSource _source;
        private CancellationToken _token;

        public bool IsRunning { get; private set; }

        public void SetRate(int rate) => _receiver.SetRate(rate);

        public void Start()
        {
            IsRunning = true;
            _source  = new CancellationTokenSource();
            _token = _source.Token;
            _notifier.Start(_token);
            _receiver.Start(_token);
        }

        public void Stop()
        {
            _source?.Cancel();
            IsRunning = false;
        }

        public EventHandler<IFrameProduct> FrameHandler
        {
            get => _notifier.FrameHandler;
            set => _notifier.FrameHandler = value;
        }

        public EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
    }
}