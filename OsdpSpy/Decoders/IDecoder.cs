using OsdpSpy.Annotations;

namespace OsdpSpy.Decoders
{
    public interface IDecoder
    {
        void Decode(byte[] input, IAnnotation output);
    }
}