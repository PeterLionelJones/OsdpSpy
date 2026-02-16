using OsdpSpy.Abstractions;

namespace OsdpSpy.Models;

public class FileTransferOptions : IFileTransferOptions
{
    public bool CaptureOsdpFileTransfer { get; set; }
    public string OsdpFileTransferDirectory { get; set; }
}