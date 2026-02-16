namespace OsdpSpy.Annotations;

public static class Annotator
{
    public const int FirstColumnWidth = -24;
    public const int HexBlockBytes = 32;
}

public abstract class Annotator<T> : IAnnotator<T>
{
    public virtual void Annotate(T input, IAnnotation output)
    {
    }

    public virtual bool IncludeInput(T input) => true;

    public virtual void ReportState()
    {
    }

    public virtual void Summarise()
    {
    }
}