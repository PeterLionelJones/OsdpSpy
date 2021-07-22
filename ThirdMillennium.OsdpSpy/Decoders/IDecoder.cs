using ThirdMillennium.Annotations;

namespace ThirdMillennium.OsdpSpy
{
    public interface IDecoder
    {
        void Decode(byte[] input, IAnnotation output);
    }
}