namespace ThirdMillennium.Utility.OSDP
{
    public class ImportOptions : IImportOptions
    {
        public ImportOptions(IFileTransferOptions file)
            => _file = file;

        private readonly IFileTransferOptions _file;
        
        public string InputFileName { get; set; }
        public bool FilterPollAck { get; set; }

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