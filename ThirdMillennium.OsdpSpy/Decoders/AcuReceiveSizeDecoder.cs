using OsdpSpy.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class AcuReceiveSizeDecoder : ICommandDecoder
    {
        public Command Command => Command.ACURXSIZE;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendItem("AcuReceiveBufferSize", input.ToAcuReceiveBufferSize());
        }
    }

    internal static class AcuReceiveSizeDecoderExtensions
    {
        internal static int ToAcuReceiveBufferSize(this byte[] payload)
            => payload[0] | (payload[1] << 8);
    }
}