using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AnnotationTests
{
    [Test]
    [TestCase("{Param1}", new object[] { 1 })]
    [TestCase("{Param1}", new object[] { "1" })]
    [TestCase("{Param1} {Param2}", new object[] { "1", "2" })]
    [TestCase("{Param1} {Param2}", new object[] { "1", 2 })]
    [TestCase("{Param1} {Param2}", new object[] { 1, 2 })]
    public void Append_SingleCall_VerifyMessageAndParameters(string message, object[] parameters)
    {
        var logger = new Mock<ILogger<Annotation>>();
        var annotation = new Annotation(logger.Object);

        annotation.Append(message, parameters);

        Assert.That(annotation.Message.Contains(message));
        Assert.That(annotation.Parameters.SequenceEqual(parameters));
    }
    
    [Test]
    [TestCase(
        "{Param1}", 
        new object[] { 1 }, 
        "{Param2}", 
        new object[] { "2" })]
    
    [TestCase(
        "{Param1}", 
        new object[] { "1" }, 
        "{Param2}", 
        new object[] { 2 })]
    
    [TestCase(
        "{Param1} {Param2}", 
        new object[] { "1", "2" }, 
        "{Param3} {Param4}", 
        new object[] { 3, 4 })]
    
    public void Append_DoubleCall_VerifyMessageAndParameters(
        string message1, 
        object[] parameters1,
        string message2,
        object[] parameters2)
    {
        var logger = new Mock<ILogger<Annotation>>();
        var annotation = new Annotation(logger.Object);

        annotation.Append(message1, parameters1);
        annotation.Append(message2, parameters2);

        Assert.That(annotation.Message.Contains(message1));
        Assert.That(annotation.Message.Contains(message2));
        Assert.That(annotation.Parameters.Length == parameters1.Length + parameters2.Length);
    }
    
    [Test]
    
    [TestCase(
        "{Param1} {Param2}", 
        new object[] { "1", 2 }, 
        "Param1", 
        true)]
    
    [TestCase(
        "{Param1} {Param2}", 
        new object[] { "1", 2 }, 
        "Param2", 
        true)]
    
    [TestCase(
        "{Param1} {Param2}", 
        new object[] { "1", 2 }, 
        "Param3", 
        false)]
    
    public void Contains_SpecifyTag_ReturnsExpected(
        string message, 
        object[] parameters, 
        string tag, 
        bool expected)
    {
        var logger = new Mock<ILogger<Annotation>>();
        var annotation = new Annotation(logger.Object);
        annotation.Append(message, parameters);

        var found = annotation.Contains(tag);

        Assert.That(found == expected);
    }

    [Test]

    [TestCase(
        "{Param1}", 
        new object[] { 1 }, 
        "{Param2}", 
        new object[] { "2" })]
    
    [TestCase(
        "{Param1}", 
        new object[] { "1" }, 
        "{Param2}", 
        new object[] { 2 })]
    
    [TestCase(
        "{Param1} {Param2}", 
        new object[] { "1", "2" }, 
        "{Param3} {Param4}", 
        new object[] { 3, 4 })]
    
    public void Log_AppendAnnotations_CheckLog(string m1, object[] p1, string m2, object[] p2)
    {
        var logger = new Mock<ILogger<Annotation>>();
        var annotation = new Annotation(logger.Object);

        annotation.Append(m1, p1);
        annotation.Append(m2, p2);
        annotation.Log();

        Assert.Pass();
    }
}