namespace OsdpSpy.Annotations
{
    public static class AnnotationExtensions
    {
        public static IAnnotation Append(
            this IAnnotation output, 
            string logMessage,
            object logObject)
        {
            return output.Append(logMessage, new [] {logObject});
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
            const int nameWidth = -24;
            var logMsg = $"\n    {name, nameWidth} {{{name}}}";
            if (suffix != null) logMsg += $" {suffix}";
            return output.Append(logMsg, logObject);
        }
        
        public static IAnnotation Annotate(this IAnnotation a, string m, object p1)
            => a.Append(m, new[] { p1 });

        public static IAnnotation Annotate(this IAnnotation a, string m, object p1, object p2)
            => a.Append(m, new[] { p1, p2 });

        public static IAnnotation Annotate(
            this IAnnotation a, string m, object p1, object p2, object p3)
            => a.Append(m, new[] { p1, p2, p3 });

        public static IAnnotation Annotate(
            this IAnnotation a, string m, object p1, object p2, object p3, object p4)
            => a.Append(m, new[] { p1, p2, p3, p4 });

        public static IAnnotation Annotate(
            this IAnnotation a, string m, object p1, object p2, object p3, object p4, object p5)
            => a.Append(m, new[] { p1, p2, p3, p4, p5 });
        
        public static void AndLogTo<T>(
            this IAnnotation annotation,
            IAlertingAnnotator<T> annotator)
        {
            annotator.LogAlert(annotation);
        }
    }
}