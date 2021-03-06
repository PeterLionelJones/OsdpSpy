using System;
using OsdpSpy.Abstractions;
using OsdpSpy.Osdp;

namespace OsdpSpy.Models
{
    public class FrameProduct : IFrameProduct
    {
        public FrameProduct(DateTime timestamp, Frame frame)
        {
            Timestamp = timestamp;
            Frame = frame;
            Payload = new Payload();
        }

        public FrameProduct(Frame frame) : this(DateTime.UtcNow, frame) {}

        public DateTime Timestamp { get; }
        public Frame Frame { get; }
        public IPayload Payload { get; }
    }
}