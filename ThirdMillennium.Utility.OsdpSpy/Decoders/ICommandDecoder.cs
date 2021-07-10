using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public interface ICommandDecoder : IDecoder
    {
        Command Command { get; }
    }
}