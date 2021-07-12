namespace ThirdMillennium.Utility.OSDP
{
    public interface IFileTransferOptions
    {
        bool CaptureOsdpFiles { get; set; }
        string OsdpFileCaptureDirectory { get; set; }
    }
}