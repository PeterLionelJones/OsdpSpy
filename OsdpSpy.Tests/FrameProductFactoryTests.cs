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
        
        Assert.IsNotNull(frameProduct);
        Assert.That(frameProduct.Frame == frame);
        Assert.IsNotNull(frameProduct.Timestamp);
        Assert.IsNotNull(frameProduct.Payload);
    }
    
    [Test]
    public void Create_FromFrameAndTimeStamp_ObjectCreatedSuccessFully()
    {
        var frame = new Frame();
        var testObject = new FrameProductFactory();
        
        var frameProduct = testObject.Create(DateTime.Now, frame);
        
        Assert.IsNotNull(frameProduct);
        Assert.That(frameProduct.Frame == frame);
        Assert.IsNotNull(frameProduct.Timestamp);
        Assert.IsNotNull(frameProduct.Payload);
    }

    [Test]
    public void Create_NullFrame_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var testObject = new FrameProductFactory();
            var frameProduct = testObject.Create(null);
        });
    }

    [Test]
    public void Create_NullTimestamp_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var testObject = new FrameProductFactory();
            var frameProduct = testObject.Create(DateTime.Now, null);
        });
    }
}