using System.Collections.Generic;

namespace OsdpSpy.Annotations
{
    public interface IAnnotatorCollection<T> : ICollection<IAnnotator<T>>
    {
        void Annotate(T input);
        bool IncludeInput(T input, IAnnotation annotation);
        void ReportState();
        void Summarise();
    }
}