namespace OsdpSpy.Annotations
{
    public interface IAlertingAnnotator<in T> : IAnnotator<T>
    {
        IAnnotation CreateAlert(string alertName, string alertMessage, string alertHeading = null);
        void LogAlert(IAnnotation alert);
    }
}