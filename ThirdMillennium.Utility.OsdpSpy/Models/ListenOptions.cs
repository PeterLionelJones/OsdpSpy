namespace ThirdMillennium.Utility.OSDP
{
    public class ListenOptions : IListenOptions, IExchangeLoggerOptions
    {
        public ListenOptions(IFileTransferOptions file, IFrameLoggerOptions frame)
        {
            _file = file;
            _frame = frame;
        }

        private readonly IFileTransferOptions _file;
        private readonly IFrameLoggerOptions _frame;
        
        public string ElasticSearchUrl { get; set;  }
        public bool FilterPollAck { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public string SeqUrl { get; set; }

        public bool CaptureOsdpFileTransfer
        {
            get => _file.CaptureOsdpFileTransfer; 
            set => _file.CaptureOsdpFileTransfer = value;
        }

        public string OsdpFileTransferDirectory
        {
            get => _file.OsdpFileTransferDirectory; 
            set => _file.OsdpFileTransferDirectory = value;
        }

        public bool CaptureToOsdpCap
        {
            get; 
            set;
        }
        public string OsdpCapDirectory 
        { 
            get => _frame.OsdpCapDirectory; 
            set => _frame.OsdpCapDirectory = value; 
        }
    }

    public class FrameLoggerOptions : IFrameLoggerOptions
    {
        public bool CaptureToOsdpCap { get; set; }
        public string OsdpCapDirectory { get; set; }
    }
}