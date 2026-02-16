using System;
using OsdpSpy.Osdp;

namespace OsdpSpy.Abstractions;

public interface IFrameProductFactory
{
    IFrameProduct Create(Frame frame);
    IFrameProduct Create(DateTime timestamp, Frame frame);
}