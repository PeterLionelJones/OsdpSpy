namespace ThirdMillennium.Utility.OSDP
{
    public class ListenOptions : IListenOptions, IExchangeLoggerOptions
    {
        public bool AutoConfigure { get; set;  }
        public bool Capture { get; set; }
        public string ElasticSearchUrl { get; set;  }
        public bool FilterPollAck { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public string SeqUrl { get; set; }
    }
}