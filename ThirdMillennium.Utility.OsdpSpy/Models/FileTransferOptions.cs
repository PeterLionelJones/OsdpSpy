namespace ThirdMillennium.Utility.OSDP
{
    public class FileTransferOptions : IFileTransferOptions
    {
        public bool CaptureOsdpFiles { get; set; }
        public string OsdpFileCaptureDirectory { get; set; }
    }
}