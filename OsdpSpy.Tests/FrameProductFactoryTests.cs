using System;
using NUnit.Framework;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests;

[TestFixture]
public class FrameProductFactoryTests
{
    [Test]
    public void Create_FromFrame_ObjectCreatedSuccessFully()
    {
        var frame = new Frame();
        var testObject = new FrameProductFactory();
        
        var frameProduct = testObject.Create(frame);
        
        Assert.That(frameProduct, Is.Not.Null);
        Assert.That(frameProduct.Frame == frame);
        Assert.That(frameProduct.Timestamp, Is.Not.Null);
        Assert.That(frameProduct.Payload, Is.Not.Null);
    }
    
    [Test]
    public void Create_FromFrameAndTimeStamp_ObjectCreatedSuccessFully()
    {
        var frame = new Frame();
        var testObject = new FrameProductFactory();
        
        var frameProduct = testObject.Create(DateTime.Now, frame);
        
        Assert.That(frameProduct, Is.Not.Null);
        Assert.That(frameProduct.Frame == frame);
        Assert.That(frameProduct.Timestamp, Is.Not.Null);
        Assert.That(frameProduct.Payload, Is.Not.Null);
    }

    [Test]
    public void Create_NullFrame_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var testObject = new FrameProductFactory();
            testObject.Create(null);
        });
    }

    [Test]
    public void Create_NullTimestamp_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var testObject = new FrameProductFactory();
            testObject.Create(DateTime.Now, null);
        });
    }
}