namespace ThirdMillennium.Utility.OSDP
{
    public interface IListenOptions : IFileTransferOptions
    {
        bool AutoConfigure { get; set; }
        string ElasticSearchUrl { get; set; }
        bool FilterPollAck { get; set; }
        string PortName { get; set; }
        int BaudRate { get; set; }
        string SeqUrl { get; set; }
    }
}