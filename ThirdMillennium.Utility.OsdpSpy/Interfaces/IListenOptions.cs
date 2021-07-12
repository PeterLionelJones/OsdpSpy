namespace ThirdMillennium.Utility.OSDP
{
    public interface IListenOptions : IFileTransferOptions, IFrameLoggerOptions
    {
        string ElasticSearchUrl { get; set; }
        bool FilterPollAck { get; set; }
        string PortName { get; set; }
        int BaudRate { get; set; }
        string SeqUrl { get; set; }
    }

    public interface IFrameLoggerOptions
    {
        bool CaptureToOsdpCap { get; set; }
        string OsdpCapDirectory { get; set; }
    }

}