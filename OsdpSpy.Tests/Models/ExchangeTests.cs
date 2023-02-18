using System;
using NUnit.Framework;
using OsdpSpy.Models;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests.Models;

[TestFixture]
public class ExchangeTests
{
    [Test]
    public void AddReceived_AddTwice_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            long sequence = 1234;
            var frame = new Frame();
            var frameProduct = FrameProduct.Create(frame);

            var testObject = Exchange.Create(sequence, frameProduct);
            testObject.AddReceived(frameProduct);
            testObject.AddReceived(frameProduct);
        });
    }
}