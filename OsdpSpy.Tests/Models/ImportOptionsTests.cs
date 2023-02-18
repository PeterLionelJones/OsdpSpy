using NUnit.Framework;
using OsdpSpy.Models;

namespace OsdpSpy.Tests.Models;

[TestFixture]
public class ImportOptionsTests
{
    [Test]
    public void Constructor_Construct_FieldsLoaded()
    {
        const bool capture = true;
        const string directory = "./osdp";
        const string inputFileName = "Input File Name";
        const bool filter = true;

        var testObject = new ImportOptions(new FileTransferOptions())
        {
            CaptureOsdpFileTransfer = capture, 
            OsdpFileTransferDirectory = directory,
            InputFileName = inputFileName,
            FilterPollAck = filter
        };

        Assert.That(testObject.CaptureOsdpFileTransfer == capture);
        Assert.That(testObject.OsdpFileTransferDirectory == directory);
        Assert.That(testObject.InputFileName == inputFileName);
        Assert.That(testObject.FilterPollAck == filter);
    }
}