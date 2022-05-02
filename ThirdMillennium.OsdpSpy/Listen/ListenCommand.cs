using System;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;

namespace ThirdMillennium.OsdpSpy
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
            IExchangeLoggerOptions loggerOptions,
            IKeyStore keyStore)
        {
            _frames = frames;
            _logger = logger;
            _exchanges = exchanges;
            _listenOptions = listenOptions;
            _consumer = consumer;
            _loggerOptions = loggerOptions;
            _keyStore = keyStore;
        }

        private readonly IBusFrameProducer _frames;
        private readonly IFrameLogger _logger;
        private readonly IExchangeProducer _exchanges;
        private readonly IListenOptions _listenOptions;
        private readonly IExchangeConsumer _consumer;
        private readonly IExchangeLoggerOptions _loggerOptions;
        private readonly IKeyStore _keyStore;

        [Option(
            "-c|--capture", 
            "Log to osdpcap",
            CommandOptionType.SingleOrNoValue)]
        private (bool hasValue, string value) CaptureToOsdpCap
        {
            set
            {
                _listenOptions.CaptureToOsdpCap = value.hasValue;
                _listenOptions.OsdpCapDirectory = value.hasValue ? value.value : null;
            }

            get => (_listenOptions.CaptureToOsdpCap, _listenOptions.OsdpCapDirectory);
        }

        [Option(
            "-t|--filetransfer",
            "Capture OSDP files",
            CommandOptionType.SingleOrNoValue)]
        private (bool hasValue, string value) OsdpFileCaptureDirectory
        {
            set
            {
                _listenOptions.CaptureOsdpFileTransfer = value.hasValue;
                _listenOptions.OsdpFileTransferDirectory = value.hasValue ? value.value : null;
            }
            
            get => (_listenOptions.CaptureOsdpFileTransfer, _listenOptions.OsdpFileTransferDirectory);
        }

        [Option(
            template: "-e|--elasticsearch",
            description: "Elasticsearch event sink URL",
            optionType: CommandOptionType.SingleValue)]
        private string ElasticSearchUrl
        {
            set => _listenOptions.ElasticSearchUrl = value;
            get => _listenOptions.ElasticSearchUrl;
        }

        [Option(
            template: "-f|--filter",
            description: "Filter out POLL/ACK pairs",
            optionType: CommandOptionType.NoValue)]
        private bool Filter
        {
            set => _listenOptions.FilterPollAck = value;
            get => _listenOptions.FilterPollAck;
        }

        [Option(
            template: "-p|--port",
            description: "Port name (COMn: | /dev/tty.usbserial* | /dev/ttyusb*)",
            optionType: CommandOptionType.SingleValue)]
        private string PortName
        {
            set => _listenOptions.PortName = value;
            get => _listenOptions.PortName;
        }

        [Option(
            template: "-r|--rate",
            description: "Baud rate (auto | 9600 | 19200 | 38400 | 57600 | 115200 | 230400)",
            optionType: CommandOptionType.SingleValue)]
        [IsValidBaudRate]
        private string BaudRate
        {
            set => _listenOptions.BaudRate = value;
            get => _listenOptions.BaudRate;
        }

        [Option(
            template: "-s|--seq",
            description: "SEQ event sink URL",
            optionType: CommandOptionType.SingleValue)]
        private string SeqUrl
        {
            set => _listenOptions.SeqUrl = value;
            get => _listenOptions.SeqUrl;
        }

        // ReSharper disable once UnusedMember.Local
        private int OnExecute(IConsole console)
        {
            // Tell the user what we are up to.
            console.LogParameters(_listenOptions, "Starting Listen Command");
            _keyStore.List();
            
            // Feed frames into the exchange producer and on to the exchange consumer.
            _loggerOptions.FilterPollAck = _listenOptions.FilterPollAck;
            _exchanges.Subscribe(_frames);
            _consumer.Subscribe(_exchanges);
            
            // Are we logging to an osdpcap file?
            if (_listenOptions.CaptureToOsdpCap)
            {
                _logger.Subscribe(_frames);
            }
          
            // Attempt to start the listener thread.
            _frames.Start();

            // Tell the user how to end it . . .
            console.WriteLine("Press any key to stop listening\n");

            // Listen to the OSDP channel for as long as the user wants.
            for (var cancelled = false; !cancelled && _frames.IsRunning; )
            {
                Thread.Sleep(100);

                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    cancelled = true;
                }
            }
            
            // Summarise the traffic.
            _consumer.Summarise();
            
            // Stop listening to OSDP frames.
            _frames.Stop();
            _exchanges.Unsubscribe();
            _consumer.Unsubscribe();
            
            return 1;
        }
    }
}