using NUnit.Framework;
using OsdpSpy.Models;

namespace OsdpSpy.Tests.Models;

[TestFixture]
public class ListenOptionsTests
{
    [Test]
    public void Constructor_Construct_FieldsLoaded()
    {
        const string elasticsearchUrl = "https//elastic";
        const string seqUrl = "https://seq";
        const bool filter = true;
        const string portName = "/dev/usb";
        const int baudRate = 9600;
        const bool capture = true;
        const string directory = "./osdp";

        var fileTransferOptions = new FileTransferOptions();
        var frameLoggerOptions = new FrameLoggerOptions();

        var testObject = new ListenOptions(fileTransferOptions, frameLoggerOptions)
        {
            ElasticSearchUrl = elasticsearchUrl,
            FilterPollAck = filter,
            PortName = portName,
            BaudRate = baudRate.ToString(),
            SeqUrl = seqUrl,
            CaptureOsdpFileTransfer = capture, 
            OsdpFileTransferDirectory = directory,
            CaptureToOsdpCap = capture,
            OsdpCapDirectory = directory
        };

        Assert.That(testObject.ElasticSearchUrl == elasticsearchUrl);
        Assert.That(testObject.FilterPollAck == filter);
        Assert.That(testObject.PortName == portName);
        Assert.That(testObject.BaudRate == baudRate.ToString());
        Assert.That(testObject.SeqUrl == seqUrl);
        Assert.That(testObject.CaptureOsdpFileTransfer == capture);
        Assert.That(testObject.OsdpFileTransferDirectory == directory);
        Assert.That(testObject.CaptureToOsdpCap == capture);
        Assert.That(testObject.OsdpCapDirectory == directory);
    }
}