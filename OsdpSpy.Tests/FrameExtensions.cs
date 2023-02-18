using OsdpSpy.Osdp;

namespace OsdpSpy.Tests;

internal static class FrameExtensions
{
    internal static Frame ToFrame(this byte[] data)
    {
        var frame = new ResponseFrame();
        
        foreach (var inch in data)
        {
            frame.AddByte(inch);
        }

        frame.Disassemble();

        return frame;
    }
}