using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public interface IReplyDecoderCollection
    {
        void Decode(Reply reply, byte[] input, IAnnotation output);
    }

    public interface ICommandDecoderCollection
    {
        void Decode(Command command, byte[] input, IAnnotation output);
    }
}