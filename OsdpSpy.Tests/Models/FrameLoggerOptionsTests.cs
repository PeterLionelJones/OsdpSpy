using NUnit.Framework;
using OsdpSpy.Models;

namespace OsdpSpy.Tests.Models;

[TestFixture]
public class FrameLoggerOptionsTests
{
    [Test]
    public void Constructor_Construct_FieldsLoaded()
    {
        const bool capture = true;
        const string directory = "./osdp";
        
        var fileTransferOptions = new FrameLoggerOptions
        {
            CaptureToOsdpCap = capture, 
            OsdpCapDirectory = directory
        };

        Assert.That(fileTransferOptions.CaptureToOsdpCap == capture);
        Assert.That(fileTransferOptions.OsdpCapDirectory == directory);
    }

}