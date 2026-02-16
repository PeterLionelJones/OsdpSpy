namespace OsdpSpy.Abstractions;

public interface IListenOptions : IFileTransferOptions, IFrameLoggerOptions
{
    string ElasticSearchUrl { get; set; }
    bool FilterPollAck { get; set; }
    string PortName { get; set; }
    string BaudRate { get; set; }
    string SeqUrl { get; set; }
}