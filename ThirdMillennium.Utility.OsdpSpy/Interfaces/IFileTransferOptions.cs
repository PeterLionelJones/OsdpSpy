namespace ThirdMillennium.Utility.OSDP
{
    public interface IFileTransferOptions
    {
        bool CaptureOsdpFileTransfer { get; set; }
        string OsdpFileTransferDirectory { get; set; }
    }
}