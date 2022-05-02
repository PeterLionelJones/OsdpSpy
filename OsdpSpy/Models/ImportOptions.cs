using OsdpSpy.Abstractions;

namespace OsdpSpy.Models
{
    public class ImportOptions : IImportOptions
    {
        public ImportOptions(IFileTransferOptions file)
            => _file = file;

        private readonly IFileTransferOptions _file;
        
        public string InputFileName { get; set; }
        public bool FilterPollAck { get; set; }

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
    }
}