using System;
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
            "-c|--capture",
            "Capture OSDP files",
            CommandOptionType.SingleOrNoValue)]
        private (bool hasValue, string value) OsdpFileCaptureDirectory
        {
            set
            {
                _listenOptions.CaptureOsdpFiles = value.hasValue;
                _listenOptions.OsdpFileCaptureDirectory = value.hasValue ? value.value : null;
            }
        }

        [Option(
            template: "-e|--elasticsearch",
            description: "Elasticsearch event sink URL",
            optionType: CommandOptionType.SingleValue)]
        public string ElasticSearchUrl
        {
            set => _listenOptions.ElasticSearchUrl = value;
        }

        [Option(
            template: "-f|--filter",
            description: "Filter out POLL/ACK pairs",
            optionType: CommandOptionType.NoValue)]
        public bool Filter
        {
            set => _listenOptions.FilterPollAck = value;
        }

        [Option(
            template: "-p|--port",
            description: "Port name (COMn: | /dev/tty.usbserial* | /dev/ttyusb*)",
            optionType: CommandOptionType.SingleValue)]
        public string PortName
        {
            set => _listenOptions.PortName = value;
        }

        [Option(
            template: "-r|--rate",
            description: "Baud rate (9600 | 19200 | 38400 | 57600 | 115200 | 230400)",
            optionType: CommandOptionType.SingleValue)]
        [IsValidBaudRate]
        public int BaudRate
        {
            set => _listenOptions.BaudRate = value;
        }

        [Option(
            template: "-s|--seq",
            description: "SEQ event sink URL",
            optionType: CommandOptionType.SingleValue)]
        public string SeqUrl
        {
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
            if (_listenOptions.CaptureOsdpFiles)
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