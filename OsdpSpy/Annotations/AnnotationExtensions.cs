namespace OsdpSpy.Annotations;

public static class AnnotationExtensions
{
    public static IAnnotation Append(
        this IAnnotation output, 
        string logMessage,
        object logObject)
    {
        return output.Append(logMessage, [logObject]);
    }

    public static IAnnotation Append(this IAnnotation output, string logMessage) 
        => output.Append(logMessage, null);

    public static IAnnotation AppendNewLine(this IAnnotation output) 
        => output.Append("\n");
        
    public static IAnnotation AppendItem(
        this IAnnotation output, 
        string name, 
        object logObject, 
        string suffix = null)
    {
        var logMsg = $"\n    {name, Annotator.FirstColumnWidth} {{{name}}}";
        if (suffix != null) logMsg += $" {suffix}";
        return output.Append(logMsg, logObject);
    }

    public static IAnnotation AppendItem(
        this IAnnotation output, 
        bool predicate, 
        string name, 
        object logObject,
        string suffix = null)
    {
        return predicate 
            ? output.AppendItem(name, logObject, suffix) 
            : output;
    }
        
    public static IAnnotation Annotate(this IAnnotation a, string m, object p1)
        => a.Append(m, [p1]);

    public static IAnnotation Annotate(this IAnnotation a, string m, object p1, object p2)
        => a.Append(m, [p1, p2]);

    public static IAnnotation Annotate(
        this IAnnotation a, string m, object p1, object p2, object p3)
        => a.Append(m, [p1, p2, p3]);

    public static IAnnotation Annotate(
        this IAnnotation a, string m, object p1, object p2, object p3, object p4)
        => a.Append(m, [p1, p2, p3, p4]);

    public static IAnnotation Annotate(
        this IAnnotation a, string m, object p1, object p2, object p3, object p4, object p5)
        => a.Append(m, [p1, p2, p3, p4, p5]);
        
    public static void AndLogTo<T>(
        this IAnnotation annotation,
        IAlertingAnnotator<T> annotator)
    {
        annotator.LogAlert(annotation);
    }
}