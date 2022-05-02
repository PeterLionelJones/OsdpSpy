namespace OsdpSpy.Annotations
{
    public interface IAnnotator<in T>
    {
        void Annotate(T input, IAnnotation output);
        bool IncludeInput(T input);
        void ReportState();
        void Summarise();
    }
}