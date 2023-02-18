namespace OsdpSpy.Abstractions;

public interface IFrameLoggerOptions
{
    bool CaptureToOsdpCap { get; set; }
    string OsdpCapDirectory { get; set; }
}