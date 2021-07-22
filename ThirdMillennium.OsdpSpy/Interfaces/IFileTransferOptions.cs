namespace ThirdMillennium.OsdpSpy
{
    public interface IFileTransferOptions
    {
        bool CaptureOsdpFileTransfer { get; set; }
        string OsdpFileTransferDirectory { get; set; }
    }
}