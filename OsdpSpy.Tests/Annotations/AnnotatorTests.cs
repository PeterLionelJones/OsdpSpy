using Moq;
using NUnit.Framework;
using OsdpSpy.Annotations;

namespace OsdpSpy.Tests.Annotations;

[TestFixture]
public class AnnotatorTests
{
    private class TestAnnotator : Annotator<object>
    {
        public TestAnnotator() {}
    }
    
    [Test]
    public void Constructor_ConstructTestAnnotator_Succeeds()
    {
        var annotator = new TestAnnotator();
        Assert.IsNotNull(annotator);
    }
    
    [Test]
    public void Annotate_AnnotateInput_Succeeds()
    {
        var annotationMock = new Mock<IAnnotation>();
        var annotation = annotationMock.Object;
        var input = new object();
        
        var annotator = new TestAnnotator();
        annotator.Annotate(input, annotation);

        Assert.IsNotNull(annotator);
    }
    
    [Test]
    public void IncludeInput_Verify_IsCorrect()
    {
        var input = new object();
        
        var annotator = new TestAnnotator();
        var includeInput = annotator.IncludeInput(input);

        Assert.IsNotNull(annotator);
        Assert.IsTrue(includeInput);
    }
    
    [Test]
    public void ReportState_Execute_Succeeds()
    {
        var annotator = new TestAnnotator();
        annotator.ReportState();
        Assert.IsNotNull(annotator);
    }
    
    [Test]
    public void Summarise_Execute_Succeeds()
    {
        var annotator = new TestAnnotator();
        annotator.Summarise();
        Assert.IsNotNull(annotator);
    }
}