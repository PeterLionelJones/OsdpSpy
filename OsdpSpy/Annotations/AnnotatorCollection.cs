using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

[assembly:InternalsVisibleTo("OsdpSpy.Tests.Annotations")]

namespace OsdpSpy.Annotations;

public class AnnotatorCollection<T> : Collection<IAnnotator<T>>, IAnnotatorCollection<T>
{
    protected AnnotatorCollection(IFactory<IAnnotation> factory)
    {
        _factory = factory;
    }

    private readonly IFactory<IAnnotation> _factory;

    protected internal void AddRange(IEnumerable<IAnnotator<T>> annotators)
    {
        foreach (var annotator in annotators)
        {
            Add(annotator);
        }
    }

    private static void RunAnnotator(IAnnotator<T> annotator, T input, IAnnotation annotation)
    {
        try
        {
            annotator.Annotate(input, annotation);
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }

    public void Annotate(T input)
    {
        var annotation = _factory.Create();
            
        foreach (var annotator in this)
        {
            RunAnnotator(annotator, input, annotation);
        }

        annotation.AppendNewLine();
                
        if (IncludeInput(input, annotation)) annotation.Log();
    }

    public virtual bool IncludeInput(T input, IAnnotation annotation)
    {
        return this.All(annotator => annotator.IncludeInput(input));
    }

    public void ReportState()
    {
        foreach (var annotator in this)
        {
            annotator.ReportState();
        }
    }

    public void Summarise()
    {
        foreach (var annotator in this)
        {
            annotator.Summarise();
        }
    }
}