using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public interface IMultipartMessageDecoder : IDecoder
{
    Reply Reply { get; }
}