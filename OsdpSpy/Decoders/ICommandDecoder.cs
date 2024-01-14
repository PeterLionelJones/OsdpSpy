using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public interface ICommandDecoder : IDecoder
{
    Command Command { get; }
}