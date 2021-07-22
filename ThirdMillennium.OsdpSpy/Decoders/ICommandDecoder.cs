using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public interface ICommandDecoder : IDecoder
    {
        Command Command { get; }
    }
}