namespace OsdpSpy.Abstractions;

public interface IFileTransferOptions
{
    bool CaptureOsdpFileTransfer { get; set; }
    string OsdpFileTransferDirectory { get; set; }
}