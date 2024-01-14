using Newtonsoft.Json;

namespace OsdpSpy.Import;

public class RawTrace : IRawTrace
{
    [JsonProperty("timeSec")]
    public long Seconds { get; set; }
        
    [JsonProperty("timeNano")]
    public long Nanoseconds { get; set; }

    [JsonProperty("io")]
    public string Io { get; set; }
        
    [JsonProperty("data")]
    public string Data { get; set; }

    [JsonProperty("osdpTraceVersion")]
    public string TraceVersion { get; set; }
        
    [JsonProperty("osdpSource")]
    public string Source { get; set; }
}