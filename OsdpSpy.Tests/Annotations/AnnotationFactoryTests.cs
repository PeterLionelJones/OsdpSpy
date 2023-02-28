using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AnnotationFactoryTests
{
    [Test]
    public void Constructor_CreateAnnotation_VerifyAnnotationCreated()
    {
        var logger = new Mock<ILogger<Annotation>>();
        var provider = new Mock<IServiceProvider>();
        var factory = new AnnotationFactory(provider.Object);
        provider
            .Setup(o => o.GetService(typeof(IAnnotation)))
            .Returns(new Annotation(logger.Object));

        var returnedObj = factory.Create();

        Assert.IsNotNull(returnedObj);
    }
}