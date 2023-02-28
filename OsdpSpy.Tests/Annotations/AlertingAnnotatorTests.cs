using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AlertingAnnotatorTests
{
    private class TestAnnotator : AlertingAnnotator<object>
    {
        public TestAnnotator(IFactory<IAnnotation> factory) : base(factory) {}
    }
    
    [Test]
    public void Constructor_ConstructAlertingAnnotator_ConstructedSuccessfully()
    {
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;

        var annotator = new TestAnnotator(factory);

        Assert.IsNotNull(annotator);
        Assert.That(annotator.InternalQueue.IsEmpty);
    }
    
    [Test]
    public void CreateAlert_CreateAnAlert_CreatedSuccessfully()
    {
        var loggerMock = new Mock<ILogger<Annotation>>();
        var logger = loggerMock.Object;
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;
        factoryMock.Setup(v => v.Create()).Returns(new Annotation(logger));
        
        var annotator = new TestAnnotator(factory);
        var alert = annotator.CreateAlert(
            "Name", 
            "Message", 
            "Heading");

        Assert.IsNotNull(annotator);
        Assert.That(annotator.InternalQueue.IsEmpty);
        Assert.IsNotNull(alert);
    }
    
    [Test]
    public void LogAlert_LogAnAlert_QueuedSuccessfully()
    {
        var loggerMock = new Mock<ILogger<Annotation>>();
        var logger = loggerMock.Object;
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;
        factoryMock.Setup(v => v.Create()).Returns(new Annotation(logger));
        
        var annotator = new TestAnnotator(factory);
        var alert = annotator.CreateAlert(
            "Name", 
            "Message", 
            "Heading");
        annotator.LogAlert(alert);

        Assert.IsNotNull(annotator);
        Assert.IsNotNull(alert);
        Assert.That(annotator.InternalQueue.Count == 1);
    }
    
    [Test]
    public void ReportState_FlushTheCachedMessages_DequeuedSuccessfully()
    {
        var loggerMock = new Mock<ILogger<Annotation>>();
        var logger = loggerMock.Object;
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;
        factoryMock.Setup(v => v.Create()).Returns(new Annotation(logger));
        
        var annotator = new TestAnnotator(factory);
        var alert = annotator.CreateAlert(
            "Name", 
            "Message", 
            "Heading");
        annotator.LogAlert(alert);
        annotator.ReportState();

        Assert.IsNotNull(annotator);
        Assert.IsNotNull(alert);
        Assert.That(annotator.InternalQueue.IsEmpty);
    }
    
    [Test]
    public void ReportState_FlushEmptyCache_NoErrors()
    {
        var loggerMock = new Mock<ILogger<Annotation>>();
        var logger = loggerMock.Object;
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;
        factoryMock.Setup(v => v.Create()).Returns(new Annotation(logger));
        
        var annotator = new TestAnnotator(factory);
        annotator.ReportState();

        Assert.IsNotNull(annotator);
        Assert.That(annotator.InternalQueue.IsEmpty);
    }
}