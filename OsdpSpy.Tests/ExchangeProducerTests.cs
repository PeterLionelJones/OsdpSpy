using System;
using Moq;
using NUnit.Framework;
using OsdpSpy.Abstractions;
using OsdpSpy.Models;
using OsdpSpy.Osdp;

namespace OsdpSpy.Tests;

[TestFixture]
public class ExchangeProducerTests
{
    private readonly byte[] _tx =
    {
        0x53, 0x00, 0x09, 0x00, 0x07, 0x61, 0x00, 0x90, 0x3F
    };
    
    private readonly byte[] _rx =
    {
        0x53, 0x80, 0x14, 0x00, 0x07, 0x45, 0x6C, 0x4E, 
        0x86, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 
        0x00, 0x01, 0x02, 0x08
    };

    [Test]
    public void Subscribe_ValidInput_SetsFrameHandler()
    {
        var factory = new Mock<IExchangeFactory>();
        var frameProducer = new Mock<IFrameProducer>();
        frameProducer.SetupProperty(f => f.FrameHandler);

        var testObject = new ExchangeProducer(factory.Object);
        testObject.Subscribe(frameProducer.Object);
        
        Assert.IsNotNull(frameProducer.Object.FrameHandler);
    }

    [Test]
    public void Subscribe_NullInput_ThrowsNullArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            var factory = new Mock<IExchangeFactory>();
            var testObject = new ExchangeProducer(factory.Object);
            testObject.Subscribe(null);
        });
    }

    [Test]
    public void Unsubscribe_ExistingSubscription_ExhangeHandlerIsNull()
    {
        var factory = new Mock<IExchangeFactory>();
        var frameProducer = new Mock<IFrameProducer>();
        frameProducer.SetupProperty(f => f.FrameHandler);

        var testObject = new ExchangeProducer(factory.Object);
        testObject.Subscribe(frameProducer.Object);
        testObject.Unsubscribe();
        
        Assert.IsNull(frameProducer.Object.FrameHandler);
    }

    [Test]
    public void OnFrame_FireIndirectlyTxFrameOnce_ExchangeHandlerNotCalled()
    {
        Frame txFrame = null;
        Frame rxFrame = null;
        var factory = new Mock<IExchangeFactory>();
        var frameProducer = new Mock<IFrameProducer>();
        frameProducer.SetupProperty(f => f.FrameHandler);
        var txProduct = FrameProduct.Create(_tx.ToFrame());
        

        var testObject = new ExchangeProducer(factory.Object);
        testObject.ExchangeHandler += (_, exchange) =>
        {
            txFrame = exchange.Acu.Frame;
            rxFrame = exchange.Pd.Frame;
        };
        testObject.Subscribe(frameProducer.Object);
        frameProducer.Object.FrameHandler.Invoke(this, txProduct);
        
        Assert.IsNull(txFrame);
        Assert.IsNull(rxFrame);
    }

    [Test]
    public void OnFrame_FireIndirectlyRxFrameOnce_ExchangeHandlerNotCalled()
    {
        Frame txFrame = null;
        Frame rxFrame = null;
        var factory = new Mock<IExchangeFactory>();
        var frameProducer = new Mock<IFrameProducer>();
        frameProducer.SetupProperty(f => f.FrameHandler);
        var rxProduct = FrameProduct.Create(_rx.ToFrame());
        

        var testObject = new ExchangeProducer(factory.Object);
        testObject.ExchangeHandler += (_, exchange) =>
        {
            txFrame = exchange.Acu.Frame;
            rxFrame = exchange.Pd.Frame;
        };
        testObject.Subscribe(frameProducer.Object);
        frameProducer.Object.FrameHandler.Invoke(this, rxProduct);
        
        Assert.IsNull(txFrame);
        Assert.IsNull(rxFrame);
    }

    [Test]
    public void OnFrame_FireIndirectlyTxFrameTwice_ExchangeHandlerCalledWithTimedOutExchange()
    {
        Frame txFrame = null;
        Frame rxFrame = null;
        var factory = new ExchangeFactory();
        var frameProducer = new Mock<IFrameProducer>();
        frameProducer.SetupProperty(f => f.FrameHandler);
        var txProduct = FrameProduct.Create(_tx.ToFrame());
        var testObject = new ExchangeProducer(factory);
        testObject.ExchangeHandler += (_, exchange) =>
        {
            txFrame = exchange.Acu.Frame;
            rxFrame = exchange.Pd?.Frame;
        };
        
        testObject.Subscribe(frameProducer.Object);
        frameProducer.Object.FrameHandler.Invoke(this, txProduct);
        frameProducer.Object.FrameHandler.Invoke(this, txProduct);
        
        Assert.That(txFrame == txProduct.Frame);
        Assert.IsNull(rxFrame);
    }

    [Test]
    public void OnFrame_FireIndirectlyTxFollowsByRx_ExchangeHandlerCalledWithValidExchange()
    {
        Frame txFrame = null;
        Frame rxFrame = null;
        var factory = new ExchangeFactory();
        var frameProducer = new Mock<IFrameProducer>();
        frameProducer.SetupProperty(f => f.FrameHandler);
        var txProduct = FrameProduct.Create(_tx.ToFrame());
        var rxProduct = FrameProduct.Create(_rx.ToFrame());
        var testObject = new ExchangeProducer(factory);
        testObject.ExchangeHandler += (_, exchange) =>
        {
            txFrame = exchange.Acu.Frame;
            rxFrame = exchange.Pd?.Frame;
        };
        
        testObject.Subscribe(frameProducer.Object);
        frameProducer.Object.FrameHandler.Invoke(this, txProduct);
        frameProducer.Object.FrameHandler.Invoke(this, rxProduct);
        
        Assert.That(txFrame == txProduct.Frame);
        Assert.That(rxFrame == rxProduct.Frame);
    }
}