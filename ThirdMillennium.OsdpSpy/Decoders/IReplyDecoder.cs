using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public interface IReplyDecoder : IDecoder
    {
        Reply Reply { get; }
    }
}