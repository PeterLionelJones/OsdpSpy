using NUnit.Framework;
using OsdpSpy.Extensions;
using OsdpSpy.Models;

namespace OsdpSpy.Tests.Extensions;

[TestFixture]
public class ListenOptionsExtensionsTests
{
    [Test]
    [TestCase(null, 9600)]
    [TestCase("AUTO", 9600)]
    [TestCase("auto", 9600)]
    [TestCase("Auto", 9600)]
    [TestCase("9600", 9600)]
    [TestCase("19200", 19200)]
    [TestCase("38400", 38400)]
    [TestCase("57600", 57600)]
    [TestCase("115200", 115200)]
    [TestCase("230400", 230400)]
    [TestCase("any-thing-else", 9600)]
    public void ToBaudRate_FromListenOptions_ReturnsExpectedInteger(string rateString, int expectedBaudRate)
    {
        var options = new ListenOptions(new FileTransferOptions(), new FrameLoggerOptions()) { BaudRate = rateString };

        var baudRate = options.ToBaudRate();
        
        Assert.That(baudRate.Equals(expectedBaudRate));
    }

    [Test]
    [TestCase(null, false)]
    [TestCase("AUTO", true)]
    [TestCase("auto", true)]
    [TestCase("Auto", true)]
    [TestCase("9600", false)]
    [TestCase("19200", false)]
    [TestCase("38400", false)]
    [TestCase("57600", false)]
    [TestCase("115200", false)]
    [TestCase("230400", false)]
    [TestCase("any-thing-else", false)]
    public void CanScanBaudRates_FromListenOptions_ReturnsExpectedValue(string rateString, bool expected)
    {
        var options = new ListenOptions(new FileTransferOptions(), new FrameLoggerOptions()) { BaudRate = rateString };

        var canScan = options.CanScanBaudRates();

        Assert.That(canScan.Equals(expected));
    }
}