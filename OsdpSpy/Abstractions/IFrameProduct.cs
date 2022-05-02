using System;
using OsdpSpy.Osdp;

namespace OsdpSpy.Abstractions
{
    public interface IFrameProduct
    {
        DateTime Timestamp { get; }
        Frame Frame { get; }
        IPayload Payload { get; }
    }
}