using System.Threading;
using McMaster.Extensions.CommandLineUtils;

namespace ThirdMillennium.Utility.OSDP
{
    // TODO: Add auto as the default -r. Use -1 as the baud rate.
    // TODO: Add logging to Elasticsearch.

    [Command(Description = "Listen to OSDP communication")]
    public class ListenCommand
    {
        public ListenCommand(
            IBusFrameProducer frames,
            IFrameLogger logger,
            IExchangeProducer exchanges,
            IListenOptions listenOptions, 
            IExchangeConsumer consumer,
            IExchangeLoggerOptions loggerOptions)
        {
            _frames = frames;
            _logger = logger;
            _exchanges = exchanges;
            _listenOptions = listenOptions;
            _consumer = consumer;
            _loggerOptions = loggerOptions;
        }

        private readonly IBusFrameProducer _frames;
        private readonly IFrameLogger _logger;
        private readonly IExchangeProducer _exchanges;
        private readonly IListenOptions _listenOptions;
        private readonly IExchangeConsumer _consumer;
        private readonly IExchangeLoggerOptions _loggerOptions;

        [Option(
            template: "-c|--capture", 
            description: "Capture to osdpcap file", 
            optionType: CommandOptionType.NoValue)]
        public bool OutputFileName
        {
            get => _listenOptions.Capture;
            set => _listenOptions.Capture = value;
        }

        [Option(
            template: "-e|--elasticsearch",
            description: "Elasticsearch event sink URL",
            optionType: CommandOptionType.SingleValue)]
        public string ElasticSearchUrl
        {
            get => _listenOptions.ElasticSearchUrl; 
            set => _listenOptions.ElasticSearchUrl = value;
        }

        [Option(
            template: "-f|--filter",
            description: "Filter out POLL/ACK pairs",
            optionType: CommandOptionType.NoValue)]
        public bool Filter
        {
            get => _listenOptions.FilterPollAck;
            set => _listenOptions.FilterPollAck = value;
        }

        [Option(
            template: "-p|--port",
            description: "Port name (COMn: | /dev/tty.usbserial* | /dev/ttyusb*)",
            optionType: CommandOptionType.SingleValue)]
        public string PortName
        {
            get => _listenOptions.PortName; 
            set => _listenOptions.PortName = value;
        }

        [Option(
            template: "-r|--rate",
            description: "Baud rate (9600 | 19200 | 38400 | 57600 | 115200 | 230400)",
            optionType: CommandOptionType.SingleValue)]
        [IsValidBaudRate]
        public int BaudRate
        {
            get => _listenOptions.BaudRate; 
            set => _listenOptions.BaudRate = value;
        }

        [Option(
            template: "-s|--seq",
            description: "SEQ event sink URL",
            optionType: CommandOptionType.SingleValue)]
        public string SeqUrl
        {
            get => _listenOptions.SeqUrl; 
            set => _listenOptions.SeqUrl = value;
        }

        // ReSharper disable once UnusedMember.Local
        private int OnExecute(IConsole console)
        {
            // Tell the user what we are up to.
            console.LogParameters(_listenOptions, "Starting Listen Command");
            
            // Feed frames into the exchange producer and on to the exchange consumer.
            _loggerOptions.FilterPollAck = _listenOptions.FilterPollAck;
            _exchanges.Subscribe(_frames);
            _consumer.Subscribe(_exchanges);
            
            // Are we logging to an osdpcap file?
            if (_listenOptions.Capture)
            {
                _logger.Subscribe(_frames);
            }
          
            // Attempt to start the listener thread.
            if (!_frames.Start())
            {
                console.WriteLine("Failed to start OSDP listener");
                return - 1;
            }

            // Tell the user how to end it . . .
            console.WriteLine("Press Ctrl+C to stop listening\n");

            // Set the Ctrl+C handler.
            var cancelled = false;
            console.CancelKeyPress += (_, _) => cancelled = true;

            // Listen to the OSDP channel for as long as the user wants.
            while (!cancelled) Thread.Sleep(100);
            
            // Stop listening to OSDP frames.
            _frames.Stop();
            _exchanges.Unsubscribe();
            _consumer.Unsubscribe();
            return 1;
        }
    }
}