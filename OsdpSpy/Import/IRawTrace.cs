namespace OsdpSpy.Import
{
    public interface IRawTrace
    {
        long Seconds { get; }
        long Nanoseconds { get; }
        string Io { get; }
        string Data { get; }
        string TraceVersion { get; }
        string Source { get; }
    }
}