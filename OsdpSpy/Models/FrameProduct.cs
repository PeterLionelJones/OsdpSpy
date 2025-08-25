using System;
using OsdpSpy.Abstractions;
using OsdpSpy.Osdp;

namespace OsdpSpy.Models;

public class FrameProduct : IFrameProduct
{
    private FrameProduct() {}
        
    public static IFrameProduct Create(DateTime timestamp, Frame frame)
    {
        if (frame == null)
            throw new ArgumentNullException();
            
        return new FrameProduct
        {
            Timestamp = timestamp,
            Frame = frame,
            Payload = new Payload()
        };
    }

    public static IFrameProduct Create(Frame frame)
        => Create(DateTime.UtcNow, frame);
        
    public DateTime Timestamp { get; private init; }
    public Frame Frame { get; private init; }
    public IPayload Payload { get; private init; }
}