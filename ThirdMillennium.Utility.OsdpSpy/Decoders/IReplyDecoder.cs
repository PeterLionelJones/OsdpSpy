using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IReplyDecoder : IDecoder
    {
        Reply Reply { get; }
    }
}