using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class BusFrameProducer : IBusFrameProducer
    {
        public BusFrameProducer(
            ISerialDeviceManager mgr, 
            IListenOptions options,
            IFrameProductFactory factory)
        {
            _mgr = mgr;
            _options = options;
            _factory = factory;
        }

        private readonly ISerialDeviceManager _mgr;
        private readonly IListenOptions _options;
        private readonly IFrameProductFactory _factory;
        
        private IChannel _channel;

        private CancellationTokenSource _source;
        private CancellationToken _token;

        private bool OpenChannel()
        {
            var rx = new ResponseFrame();
            var dev = _mgr.FromPortName(_options.PortName);
            _channel = _mgr.CreateChannel();
            var connected = 
                _channel.Open(dev, _options.BaudRate) && 
                _channel.Read(rx, 200);

            return connected;
        }
        
        /// <summary>
        ///     The BackgroundService method starts up the circuit using the OnStart method, polls
        ///     the readers in the for transactions and processes circuit requests using OnService.
        ///     When the service is cancelled, the circuit is shutdown gracefully on the OnStop
        ///     method. The service is cancelled by calling the Stop method.
        /// </summary>
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

        private void OnService()
        {
            try
            {
                var rx = new ResponseFrame();
                
                // Poll until told otherwise!
                while (true)
                {
                    try
                    {
                        // Is it time to stop?
                        _token.ThrowIfCancellationRequested();

                        // Are there any bytes to process?
                        var available = _channel.BytesToRead;
                        if (available == 0) continue;
                        
                        // Allocate the buffer and read the bytes.
                        var buffer = new byte[available];
                        _channel.Read(buffer, 0, buffer.Length);
                        
                        // Add the bytes to the current frame.
                        foreach (var inch in buffer)
                        {
                            var complete = rx.AddByte(inch);

                            if (complete)
                            {
                                // Decode the received frame.
                                rx.Disassemble();
                                
                                // Pump the frame out to interested parties.
                                FrameHandler?.Invoke(this, _factory.Create(rx));
                                
                                // Start a new frame before it is too late!
                                rx = new ResponseFrame();
                            }
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
                Debug.WriteLine("CircuitThread.BackgroundService was cancelled");
            }
        }

        private void OnStop() {}


        public bool Start()
        {
            _source  = new CancellationTokenSource();
            _token = _source.Token;
            Task.Run(BackgroundService, _token);
            return true;
        }

        public void Stop()
        {
            // Let the listener stop itself.
            _source?.Cancel();
        }
    
        /// <summary>
        ///     The ConnectionStateEventHandler notifies subscribes of a change of state of the
        ///     connection to the circuit. 
        /// </summary>
        public EventHandler<ConnectionState> ConnectionStateEventHandler { get; set; }
        
        public EventHandler<IFrameProduct> FrameHandler { get; set; }
    }
}