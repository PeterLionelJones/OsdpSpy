namespace ThirdMillennium.Utility.OSDP
{
    public interface IListenOptions
    {
        bool AutoConfigure { get; set; }
        bool Capture { get; set; }
        string ElasticSearchUrl { get; set; }
        bool FilterPollAck { get; set; }
        string PortName { get; set; }
        int BaudRate { get; set; }
        string SeqUrl { get; set; }
    }
}