using OsdpSpy.Abstractions;

namespace OsdpSpy.Models;

public class FrameLoggerOptions : IFrameLoggerOptions
{
    public bool CaptureToOsdpCap { get; set; }
    public string OsdpCapDirectory { get; set; }
}