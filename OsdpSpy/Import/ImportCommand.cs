using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Import
{
    [Command(Description = "Import an osdpcap file")]
    public class ImportCommand
    {
        public ImportCommand(
            IFileFrameProducer frames, 
            IExchangeProducer exchanges,
            IImportOptions importOptions,
            IExchangeConsumer consumer,
            IExchangeLoggerOptions loggerOptions)
        {
            _frames = frames;
            _exchanges = exchanges;
            _importOptions = importOptions;
            _consumer = consumer;
            _loggerOptions = loggerOptions;
        }

        private readonly IFileFrameProducer _frames;
        private readonly IExchangeProducer _exchanges;
        private readonly IImportOptions _importOptions;
        private readonly IExchangeConsumer _consumer;
        private readonly IExchangeLoggerOptions _loggerOptions;
        
        [Option(
            template: "-f|--filter",
            description: "Filter out POLL/ACK pairs",
            optionType: CommandOptionType.NoValue)]
        public bool Filter
        {
            get => _importOptions.FilterPollAck;
            set => _importOptions.FilterPollAck = value;
        }

        [Option(
            template: "-i|--input",
            description: "Input osdppcap file name",
            optionType: CommandOptionType.SingleValue)]
        public string InputFileName
        {
            get => _importOptions.InputFileName; 
            set => _importOptions.InputFileName = value;
        }

        // ReSharper disable once UnusedMember.Local
        private int OnExecute(IConsole console)
        {
            // Tell the user what we are up to.
            console.WriteLine($"\nImporting {_importOptions.InputFileName}\n");
            
            // Feed frames into the exchange producer and on to the exchange consumer.
            _loggerOptions.FilterPollAck = _importOptions.FilterPollAck;
            _exchanges.Subscribe(_frames);
            _consumer.Subscribe(_exchanges);

            // Attempt to process the input file.
            var result =_frames.Process(_importOptions.InputFileName);
            
            // Summarise the findings.
            //_summariser.Summarise();

            // Disconnect the feeds.
            _exchanges.Unsubscribe();
            _consumer.Unsubscribe();
            return result ? 1 : -1;
        }
    }
}