using NUnit.Framework;
using OsdpSpy.Models;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests;

[TestFixture]
public class ExchangeFactoryTests
{
    [Test]
    public void Create_ValidParameters_CreatedSuccessfully()
    {
        var frame = new Frame();
        const long sequence = 1234;
        var frameProduct = FrameProduct.Create(frame);
        
        var testObject = new ExchangeFactory();
        var exchange = testObject.Create(sequence, frameProduct);
        
        Assert.That(sequence == exchange.Sequence);
    }
}