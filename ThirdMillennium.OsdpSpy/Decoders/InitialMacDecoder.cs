using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.OsdpSpy
{
    public class InitialMacDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.RMAC_I;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendByteArray("InitialMac", input);

            if (input.Length != 16)
            {
                output.AppendItem("NonCompliance", "Invalid Payload");
            }
        }
    }
}