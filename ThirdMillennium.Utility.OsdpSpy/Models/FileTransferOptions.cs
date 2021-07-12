namespace ThirdMillennium.Utility.OSDP
{
    public class FileTransferOptions : IFileTransferOptions
    {
        public bool CaptureOsdpFileTransfer { get; set; }
        public string OsdpFileTransferDirectory { get; set; }
    }
}