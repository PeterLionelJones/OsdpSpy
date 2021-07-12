namespace ThirdMillennium.Utility.OSDP
{
    public class ListenOptions : IListenOptions, IExchangeLoggerOptions
    {
        public ListenOptions(IFileTransferOptions file)
            => _file = file;

        private readonly IFileTransferOptions _file;
        
        public bool AutoConfigure { get; set;  }
        public string ElasticSearchUrl { get; set;  }
        public bool FilterPollAck { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public string SeqUrl { get; set; }

        public bool CaptureOsdpFiles
        {
            get => _file.CaptureOsdpFiles; 
            set => _file.CaptureOsdpFiles = value;
        }

        public string OsdpFileCaptureDirectory
        {
            get => _file.OsdpFileCaptureDirectory; 
            set => _file.OsdpFileCaptureDirectory = value;
        }
    }
}