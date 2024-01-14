using System;
using OsdpSpy.Abstractions;
using OsdpSpy.Models;
using OsdpSpy.Osdp;

namespace OsdpSpy;

public class FrameProductFactory : IFrameProductFactory
{
    public IFrameProduct Create(Frame frame)
        => FrameProduct.Create(frame);

    public IFrameProduct Create(DateTime timestamp, Frame frame)
        => FrameProduct.Create(timestamp, frame);
}