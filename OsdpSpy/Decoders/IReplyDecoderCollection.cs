using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public interface IReplyDecoderCollection
{
    void Decode(Reply reply, byte[] input, IAnnotation output);
}

public interface ICommandDecoderCollection
{
    void Decode(Command command, byte[] input, IAnnotation output);
}