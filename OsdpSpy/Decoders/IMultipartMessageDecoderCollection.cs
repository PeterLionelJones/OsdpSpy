using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders;

public interface IMultipartMessageDecoderCollection
{
    void Decode(Reply reply, byte[] input, IAnnotation output);
}