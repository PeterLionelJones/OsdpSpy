using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AnnotatorCollectionTests
{
    private class TestAnnotator : Annotator<object>
    {
        public TestAnnotator() {}
    }
    
    private class TestCollection : AnnotatorCollection<object>
    {
        public TestCollection(IFactory<IAnnotation> factory) : base(factory) {}
    }

    private static IEnumerable<IAnnotator<object>> NewRange(int quantity)
    {
        var annotators = new TestAnnotator[quantity];

        for (var i = 0; i < quantity; ++i)
        {
            annotators[i] = new TestAnnotator();
        }

        return annotators.AsEnumerable();
    }
    
    [Test]
    public void Constructor_Construct_Succeeds()
    {
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;

        var annotatorCollection = new TestCollection(factory);
        
        Assert.IsNotNull(annotatorCollection);
        Assert.That(annotatorCollection.Count == 0);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    public void AddRange_AddQuantityOfAnnotators_VerifyNumberOfAnnotators(int count)
    {
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        var factory = factoryMock.Object;
        var annotatorCollection = new TestCollection(factory);
        var annotatorRange = NewRange(count);

        annotatorCollection.AddRange(annotatorRange);
        
        Assert.IsNotNull(annotatorCollection);
        Assert.That(annotatorCollection.Count == count);
    }
    
    [Test]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    public void Annotate_RunAnntotors_VerifyOutput(int count)
    {
        var annotationMock = new Mock<IAnnotation>();
        var annotation = annotationMock.Object;
        
        var annotatorMock = new Mock<IAnnotator<object>>();
        var annotator = annotatorMock.Object;
        
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        factoryMock.Setup(factory => factory.Create()).Returns(annotation);
        var factory = factoryMock.Object;

        var annotatorCollection = new TestCollection(factory);
        for (var i = 0; i < count; ++i)
        {
            annotatorCollection.Add(annotator);
        }
        
        annotatorCollection.Annotate(new object());

        Assert.IsNotNull(annotatorCollection);
        Assert.That(annotatorCollection.Count == count);
    }
    
    [Test]
    public void Annotate_RunAnntotorsOnNullInput_VerifyOutput()
    {
        var annotationMock = new Mock<IAnnotation>();
        var annotation = annotationMock.Object;
        
        var annotatorMock = new Mock<IAnnotator<object>>();
        annotatorMock.Setup(foo => foo
                .Annotate(It.IsAny<object>(), It.IsAny<IAnnotation>()))
            .Throws<Exception>();
        var annotator = annotatorMock.Object;
        
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        factoryMock.Setup(factory => factory.Create()).Returns(annotation);
        var factory = factoryMock.Object;

        var annotatorCollection = new TestCollection(factory)
        {
            annotator,
            annotator
        };

        annotatorCollection.Annotate(new object());

        Assert.IsNotNull(annotatorCollection);
        Assert.That(annotatorCollection.Count == 2);
    }
    
    [Test]
    public void ReportState_ReportState_Succeeds()
    {
        var annotationMock = new Mock<IAnnotation>();
        var annotation = annotationMock.Object;
        
        var annotatorMock = new Mock<IAnnotator<object>>();
        annotatorMock.Setup(foo => foo
                .Annotate(It.IsAny<object>(), It.IsAny<IAnnotation>()))
            .Throws<Exception>();
        var annotator = annotatorMock.Object;
        
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        factoryMock.Setup(factory => factory.Create()).Returns(annotation);
        var factory = factoryMock.Object;

        var annotatorCollection = new TestCollection(factory)
        {
            annotator,
            annotator
        };

        annotatorCollection.Annotate(new object());
        annotatorCollection.ReportState();

        Assert.IsNotNull(annotatorCollection);
        Assert.That(annotatorCollection.Count == 2);
        annotatorMock.Verify(foo => foo.ReportState(), Times.Exactly(2));
    }
    
    [Test]
    public void Summarise_Summarise_Succeeds()
    {
        var annotationMock = new Mock<IAnnotation>();
        var annotation = annotationMock.Object;
        
        var annotatorMock = new Mock<IAnnotator<object>>();
        annotatorMock.Setup(foo => foo
                .Annotate(It.IsAny<object>(), It.IsAny<IAnnotation>()))
            .Throws<Exception>();
        var annotator = annotatorMock.Object;
        
        var factoryMock = new Mock<IFactory<IAnnotation>>();
        factoryMock.Setup(factory => factory.Create()).Returns(annotation);
        var factory = factoryMock.Object;

        var annotatorCollection = new TestCollection(factory)
        {
            annotator,
            annotator
        };

        annotatorCollection.Annotate(new object());
        annotatorCollection.Summarise();

        Assert.IsNotNull(annotatorCollection);
        Assert.That(annotatorCollection.Count == 2);
        annotatorMock.Verify(foo => foo.Summarise(), Times.Exactly(2));
    }
}