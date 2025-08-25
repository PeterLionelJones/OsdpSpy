using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Import;

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
    private bool Filter
    {
        get => _importOptions.FilterPollAck;
        set => _importOptions.FilterPollAck = value;
    }

    [Option(
        template: "-i|--input",
        description: "Input osdpcap file name",
        optionType: CommandOptionType.SingleValue)]
    private string InputFileName
    {
        get => _importOptions.InputFileName; 
        set => _importOptions.InputFileName = value;
    }

    // ReSharper disable once UnusedMember.Local
    private int OnExecute(IConsole console)
    {
        // Make sure a file was specified.
        if (String.IsNullOrEmpty(InputFileName))
        {
            console.WriteLine("Input file not specified");
            return -1;
        }

        // Make sure we have a valid file to process.
        if (!File.Exists(InputFileName))
        {
            console.WriteLine($"{InputFileName} does not exist");
            return -1;
        }
            
        // Tell the user what we are up to.
        console.WriteLine($"\nImporting {InputFileName}\n");
            
        // Feed frames into the exchange producer and on to the exchange consumer.
        _loggerOptions.FilterPollAck = Filter;
        _exchanges.Subscribe(_frames);
        _consumer.Subscribe(_exchanges);

        // Attempt to process the input file.
        var result =_frames.Process(InputFileName);

        // Summarise the findings.
        _consumer.Summarise();

        // Disconnect the feeds.
        _exchanges.Unsubscribe();
        _consumer.Unsubscribe();
        return result ? 1 : -1;
    }
}