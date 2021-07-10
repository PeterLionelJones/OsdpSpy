using ThirdMillennium.Annotations;

namespace ThirdMillennium.Utility.OSDP
{
    public interface IDecoder
    {
        void Decode(byte[] input, IAnnotation output);
    }
}