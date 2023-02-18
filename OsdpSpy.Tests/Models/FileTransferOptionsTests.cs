using NUnit.Framework;
using OsdpSpy.Models;

namespace OsdpSpy.Tests.Models;

[TestFixture]
public class FileTransferOptionsTests
{
    [Test]
    public void Constructor_Construct_FieldsLoaded()
    {
        const bool capture = true;
        const string directory = "./osdp";
        
        var testObject = new FileTransferOptions
        {
            CaptureOsdpFileTransfer = capture, 
            OsdpFileTransferDirectory = directory
        };

        Assert.That(testObject.CaptureOsdpFileTransfer == capture);
        Assert.That(testObject.OsdpFileTransferDirectory == directory);
    }
}