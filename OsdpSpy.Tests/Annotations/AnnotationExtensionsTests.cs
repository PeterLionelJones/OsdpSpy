using Moq;
using NUnit.Framework;
using OsdpSpy.Abstractions;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AnnotationExtensionsTests
{
    [Test]
    public void Append_SingleObject_CallsAppendWithSingleItemArray()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Append("{Param1}", 1);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v == "{Param1}"),
            It.Is<object[]>(v => (int) v[0] == 1)));
    }

    [Test]
    public void Append_MessageOnly_CallsAppendWithNullItemArray()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Append("A message");
        
        annotation.Verify(foo => foo.Append(
            It.IsAny<string>(),
            It.Is<object[]>(v => v == null)));
    }

    [Test]
    public void AppendNewLine_CallsAppend_VerifyMessage()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.AppendNewLine();
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v == "\n"),
            It.Is<object[]>(v => v == null)));
    }

    [Test]
    public void AppendItem_NoSuffix_VerifyMessage()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.AppendItem("Item", 1);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("Item")),
            It.Is<object[]>(v => (int) v[0] == 1)));
    }

    [Test]
    public void AppendItem_WithSuffix_VerifyMessage()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.AppendItem("Item", 1, "Suffix");
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("Item") && v.Contains("Suffix")),
            It.Is<object[]>(v => (int) v[0] == 1)));
    }

    [Test]
    public void Annotate_WithOneParameter_VerifyMessageAndParameter()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Annotate("{p1}", 1);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("{p1}")),
            It.Is<object[]>(v => (int) v[0] == 1)));
    }

    [Test]
    public void Annotate_WithTwoParameters_VerifyMessageAndParameters()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Annotate("{p1}{p2}", 1, 2);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("{p1}") && v.Contains("{p2}")),
            It.Is<object[]>(v => (int) v[0] == 1 && (int) v[1] == 2)));
    }

    [Test]
    public void Annotate_WithThreeParameters_VerifyMessageAndParameters()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Annotate("{p1}{p2}{p3}", 1, 2, 3);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("{p1}") && v.Contains("{p2}") && v.Contains("{p3}")),
            It.Is<object[]>(v => (int) v[0] == 1 && (int) v[1] == 2 && (int) v[2] == 3)));
    }

    [Test]
    public void Annotate_WithFourParameters_VerifyMessageAndParameters()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Annotate("{p1}{p2}{p3}{p4}", 1, 2, 3, 4);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("{p1}") && v.Contains("{p2}") && v.Contains("{p3}") && v.Contains("{p4}")),
            It.Is<object[]>(v => (int) v[0] == 1 && (int) v[1] == 2 && (int) v[2] == 3 && (int) v[3] == 4)));
    }

    [Test]
    public void Annotate_WithFiveParameters_VerifyMessageAndParameters()
    {
        var annotation = new Mock<IAnnotation>();

        annotation.Object.Annotate("{p1}{p2}{p3}{p4}{p5}", 1, 2, 3, 4, 5);
        
        annotation.Verify(foo => foo.Append(
            It.Is<string>(v => v.Contains("{p1}") && v.Contains("{p2}") && v.Contains("{p3}") && v.Contains("{p4}") && v.Contains("{p5}")),
            It.Is<object[]>(v => (int) v[0] == 1 && (int) v[1] == 2 && (int) v[2] == 3 && (int) v[3] == 4 && (int) v[4] == 5)));
    }

    [Test]
    public void AndLogTo_LogAlert_VerifyAlertLogged()
    {
        var annotator = new Mock<IAlertingAnnotator<IExchange>>();
        var annotation = new Mock<IAnnotation>();

        annotation.Object.AndLogTo(annotator.Object);

        annotator.Verify(v => v.LogAlert(It.IsAny<IAnnotation>()), Times.Once);
    }
}