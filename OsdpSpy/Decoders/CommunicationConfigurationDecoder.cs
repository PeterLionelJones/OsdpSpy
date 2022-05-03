using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders
{
    public class CommunicationConfigurationDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.COMSET;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendItem("NewAddress", input.ToNewAddress());
            output.AppendItem("NewBaudRate", input.ToNewBaudRate());
        }
    }

    internal static class CommunicationConfigurationDecoderExtensions
    {
        internal static int ToNewAddress(this byte[] payload)
            => payload[0];
        
        internal static int ToNewBaudRate(this byte[] payload)
            => payload[1] | payload[2] << 8 | payload[3] << 16 | payload[4] << 24;
    }
}