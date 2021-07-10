using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class BusFrameProducer : IBusFrameProducer
    {
        public BusFrameProducer(
            ISerialDeviceManager mgr, 
            IListenOptions options,
            IFrameProductFactory factory,
            ILogger<BusFrameProducer> logger)
        {
            _mgr = mgr;
            _options = options;
            _factory = factory;
            _logger = logger;
        }

        private const int OfflineTimeout = 1000;
        private const int OnlineTimeout = 10000;
        
        private readonly ISerialDeviceManager _mgr;
        private readonly IListenOptions _options;
        private readonly IFrameProductFactory _factory;
        private readonly ILogger<BusFrameProducer> _logger;
        
        private IChannel _channel;
        private CancellationTokenSource _source;
        private CancellationToken _token;
        
        private DateTime _lastFrame = DateTime.UtcNow;
        private ResponseFrame _rx = new ResponseFrame();
        private bool _online;

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
            SetRate(_channel.BaudRate.NextBaudRate());
        }
        
        private bool OpenChannel()
        {
            var dev = _mgr.FromPortName(_options.PortName);
            _channel = _mgr.CreateChannel();
            IsRunning = _channel.Open(dev, _options.BaudRate);
            return IsRunning;
        }
        
        private void BackgroundService()
        {
            // Attempt to start the background service.
            if (OnStart())
            {
                // Provide service until the service is cancelled.
                OnService();
                
                // Stop the service.
                OnStop();
            }
        }

        private bool OnStart()
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
            FrameHandler?.Invoke(this, product);
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

        private void OnService()
        {
            try
            {
                // Poll until told otherwise!
                while (true)
                {
                    try
                    {
                        _token.ThrowIfCancellationRequested();
                        
                        ReadFrame();

                        if (NoActivity)
                        {
                            OnLostSynchronisation();
                            SwitchBaudRate();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("BackgroundService was cancelled");
            }
        }

        private void OnStop() {}

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

        public bool IsRunning { get; private set; } = true;

        public void SetRate(int rate)
        {
            if (!rate.IsValidBaudRate())
                throw new ArgumentException("Invalid baud rate");

            if (!IsRunning) return;

            _online = false;
            _lastFrame = DateTime.UtcNow;
            _rx = new ResponseFrame();
            _channel.Reopen(rate);
        }

        public void Start()
        {
            _source  = new CancellationTokenSource();
            _token = _source.Token;
            Task.Run(BackgroundService, _token);
        }

        public void Stop()
        {
            _source?.Cancel();
            IsRunning = false;
        }
    
        public EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
        public EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}