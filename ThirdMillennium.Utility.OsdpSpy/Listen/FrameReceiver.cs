using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    internal class FrameReceiver : ThreadService, IFrameReceiver
    {
        public FrameReceiver(
            ISerialDeviceManager mgr, 
            IListenOptions options,
            IFrameQueue queue,
            IFrameProductFactory factory,
            ILogger<BusFrameProducer> logger)
        {
            _mgr = mgr;
            _options = options;
            _queue = queue;
            _factory = factory;
            _logger = logger;
        }

        private const int OfflineTimeout = 1000;
        private const int OnlineTimeout = 10000;
        
        private readonly ISerialDeviceManager _mgr;
        private readonly IListenOptions _options;
        private readonly IFrameQueue _queue;
        private readonly IFrameProductFactory _factory;
        private readonly ILogger<BusFrameProducer> _logger;
        
        private IChannel _channel;
        
        private DateTime _lastFrame = DateTime.UtcNow;
        private ResponseFrame _rx = new ResponseFrame();
        private bool _online;
        private int _newRate;
        
        private bool IsSwitchPending { get; set; }
        private bool NoActivity
        {
            get
            {
                var timeout = 
                    TimeSpan.FromMilliseconds(_online ? OnlineTimeout : OfflineTimeout);
                
                return DateTime.UtcNow - _lastFrame > timeout;
            }
        }

        private void SwitchBaudRate()
        {
            Switch(_channel.BaudRate.NextBaudRate());
        }
        
        private void Switch(int rate)
        {
            _online = false;
            _lastFrame = DateTime.UtcNow;
            _rx = new ResponseFrame();
            _channel.Reopen(rate);
        }

        private void Switch()
        {
            if (IsSwitchPending)
            {
                IsSwitchPending = false;
                Switch(_newRate);
            }
        }
        
        private void OnSynchronised()
        {
            if (!_online)
            {
                _online = true;
            
                _logger.LogInformation(
                    "Synchronised with OSDP Bus at {BaudRate} Baud\n", 
                    _channel.BaudRate);
            }
        }

        private void OnLostSynchronisation()
        {
            if (_online)
            {
                _online = false;
                _logger.LogInformation("Lost Synchronisation with OSDP Bus\n");
            }
        }

        private bool OpenChannel()
        {
            var dev = _mgr.FromPortName(_options.PortName);
            _channel = _mgr.CreateChannel();
            IsRunning = _channel.Open(dev, _options.BaudRate);
            return IsRunning;
        }

        protected override bool OnStart()
        {
            try
            {
                // Notify consumers that we are trying to connect to the channel.
                ConnectionStateEventHandler?.Invoke(this, ConnectionState.Connecting);
            
                // Attempt to open the specified channel and make sure there is OSDP activity.
                var connected = OpenChannel();
            
                // Notify consumers the result of attempting to open the channel.
                ConnectionStateEventHandler?.Invoke(this, connected 
                    ? ConnectionState.Connected 
                    : ConnectionState.ConnectFailed);
            
                // Are we open for business?
                return connected;
            }
            catch (Exception)
            {
                return false;
            }
        }
    
        private void Notify(Frame frame)
        {
            // Decode the received frame.
            frame.Disassemble();

            // Create the frame product and update the timestamp.
            var product = _factory.Create(frame);
            _lastFrame = product.Timestamp;
                            
            // Pump the frame product out to interested parties.
            //FrameHandler?.Invoke(this, product);
            _queue.Add(product);
        }

        private void ReadFrame()
        {
            var available = _channel.BytesToRead;
            if (available == 0) return;

            if (available > 100)
            {
                Debug.WriteLine($"{available} Bytes in Buffer");
            }
                        
            // Allocate the buffer and read the bytes.
            var buffer = new byte[available];
            _channel.Read(buffer, 0, buffer.Length);
                        
            // Add the bytes to the current frame.
            foreach (var inch in buffer)
            {
                var complete = _rx.AddByte(inch);

                if (complete)
                {
                    Notify(_rx);
                    OnSynchronised();
                    _rx = new ResponseFrame();
                }
            }
        }

        protected override void OnService()
        {
            ReadFrame();

            if (IsSwitchPending)
            {
                IsSwitchPending = false;
                Switch(_newRate);
            }
    
            if (NoActivity)
            {
                OnLostSynchronisation();
                SwitchBaudRate();
            }
        }
        
        public void SetRate(int rate)
        {
            if (!rate.IsValidBaudRate())
                throw new ArgumentException("Invalid baud rate");

            IsSwitchPending = true;
            _newRate = rate;
        }

        public EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
    }
}