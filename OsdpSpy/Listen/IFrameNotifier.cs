using System;
using OsdpSpy.Abstractions;

namespace OsdpSpy.Listen;

public interface IFrameNotifier : IThreadService
{
    EventHandler<IFrameProduct> FrameHandler { get; set; }
}