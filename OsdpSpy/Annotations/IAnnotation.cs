namespace OsdpSpy.Annotations
{
    public interface IAnnotation
    {
        IAnnotation Append(string logMessage, object[] logObjects);
        bool Contains(string tag);
        void Log();
    }
}