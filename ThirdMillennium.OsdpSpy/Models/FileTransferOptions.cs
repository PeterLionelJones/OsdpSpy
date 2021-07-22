namespace ThirdMillennium.OsdpSpy
{
    public class FileTransferOptions : IFileTransferOptions
    {
        public bool CaptureOsdpFileTransfer { get; set; }
        public string OsdpFileTransferDirectory { get; set; }
    }
}