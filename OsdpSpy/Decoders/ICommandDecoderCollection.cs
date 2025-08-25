using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public interface ICommandDecoderCollection
{
    void Decode(Command command, byte[] input, IAnnotation output);
}