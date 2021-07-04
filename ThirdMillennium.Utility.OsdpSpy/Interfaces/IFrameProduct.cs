using System;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IFrameProduct
    {
        DateTime Timestamp { get; }
        Frame Frame { get; }
        IPayload Payload { get; }
    }
}