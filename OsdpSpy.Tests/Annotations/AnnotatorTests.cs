using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AnnotatorTests
{
    private class TestAnnotator : Annotator<object>;
    
    [Test]
    public void Constructor_ConstructTestAnnotator_Succeeds()
    {
        var annotator = new TestAnnotator();
        Assert.That(annotator, Is.Not.Null);
    }
    
    [Test]
    public void Annotate_AnnotateInput_Succeeds()
    {
        var annotationMock = new Mock<IAnnotation>();
        var annotation = annotationMock.Object;
        var input = new object();
        
        var annotator = new TestAnnotator();
        annotator.Annotate(input, annotation);

        Assert.That(annotator, Is.Not.Null);
    }
    
    [Test]
    public void IncludeInput_Verify_IsCorrect()
    {
        var input = new object();
        
        var annotator = new TestAnnotator();
        var includeInput = annotator.IncludeInput(input);

        Assert.That(annotator, Is.Not.Null);
        Assert.That(includeInput, Is.True);
    }
    
    [Test]
    public void ReportState_Execute_Succeeds()
    {
        var annotator = new TestAnnotator();
        annotator.ReportState();
        Assert.That(annotator, Is.Not.Null);
    }
    
    [Test]
    public void Summarise_Execute_Succeeds()
    {
        var annotator = new TestAnnotator();
        annotator.Summarise();
        Assert.That(annotator, Is.Not.Null);
    }
}