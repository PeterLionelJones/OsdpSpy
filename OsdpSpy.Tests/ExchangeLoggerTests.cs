using System;
using Moq;
using NUnit.Framework;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;
using OsdpSpy.Import;
using OsdpSpy.Models;

namespace OsdpSpy.Tests;

[TestFixture]
public class ExchangeLoggerTests
{
    [Test]
    public void Summarise_Executes_CallCorrectAnnotationMethods()
    {
        var annotators = new Mock<IAnnotatorCollection<IExchange>>();
        var logger = new ExchangeLogger(annotators.Object);
        
        logger.Summarise();
        
        annotators.Verify(foo => foo.Summarise(), Times.Once);
    }
    
    [Test]
    public void Subscribe_ValidInput_SetsExchangeHandler()
    {
        var annotators = new Mock<IAnnotatorCollection<IExchange>>();
        var input = new Mock<IExchangeProducer>();
        input.SetupProperty(f => f.ExchangeHandler);
        
        var testObject = new ExchangeLogger(annotators.Object);
        testObject.Subscribe(input.Object);

        Assert.NotNull(input.Object.ExchangeHandler);
    }
    
    [Test]
    public void Subscribe_NullInput_ThrowsNullArgumentException()
    {
        var annotators = new Mock<IAnnotatorCollection<IExchange>>();
        var input = new Mock<IExchangeProducer>();
        input.SetupProperty(f => f.ExchangeHandler);

        Assert.Throws<ArgumentNullException>(() =>
        {
            var testObject = new ExchangeLogger(annotators.Object);
            testObject.Subscribe(null);
        });
    }

    [Test]
    public void Unsubscribe_ExistingSubscription_ExhangeHandlerIsNull()
    {
        var annotators = new Mock<IAnnotatorCollection<IExchange>>();
        var input = new Mock<IExchangeProducer>();
        input.SetupProperty(f => f.ExchangeHandler);
        
        var testObject = new ExchangeLogger(annotators.Object);
        testObject.Subscribe(input.Object);
        testObject.Unsubscribe();

        Assert.IsNull(input.Object.ExchangeHandler);
    }

    [Test]
    public void OnExchange_FireIndirectly_CallCorrectAnnotationMethods()
    {
        var annotators = new Mock<IAnnotatorCollection<IExchange>>();
        var exchange = new Mock<IExchange>();
        var input = new Mock<IExchangeProducer>();
        input.SetupProperty(f => f.ExchangeHandler);
        
        var testObject = new ExchangeLogger(annotators.Object);
        testObject.Subscribe(input.Object);
        input.Object.ExchangeHandler.Invoke(this, exchange.Object);

        Assert.IsNotNull(input.Object.ExchangeHandler);
        annotators.Verify(foo => foo.Annotate(It.IsAny<IExchange>()), Times.Once);
        annotators.Verify(foo => foo.ReportState(), Times.Once);
    }
}