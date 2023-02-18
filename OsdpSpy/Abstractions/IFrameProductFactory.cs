using System;
using OsdpSpy.Abstractions;
using OsdpSpy.Osdp;

namespace OsdpSpy
{
    public interface IFrameProductFactory
    {
        IFrameProduct Create(Frame frame);
        IFrameProduct Create(DateTime timestamp, Frame frame);
    }
}