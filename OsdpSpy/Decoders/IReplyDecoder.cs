using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public interface IReplyDecoder : IDecoder
{
    Reply Reply { get; }
}