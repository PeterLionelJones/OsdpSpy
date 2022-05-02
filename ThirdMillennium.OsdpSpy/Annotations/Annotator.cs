namespace OsdpSpy.Annotations
{
    public abstract class Annotator<T> : IAnnotator<T>
    {
        public virtual void Annotate(T input, IAnnotation output) {}
        public virtual bool IncludeInput(T input) => true;
        public virtual void ReportState() {}
        public virtual void Summarise() {}
    }
}