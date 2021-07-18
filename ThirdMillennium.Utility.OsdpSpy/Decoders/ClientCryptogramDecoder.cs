using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class ClientCryptogramDecoder : IReplyDecoder
    {
        public Reply Reply => Reply.CCRYPT;

        public void Decode(byte[] input, IAnnotation output)
        {
            output
                .AppendByteArray("ClientId", input, 0, 8)
                .AppendByteArray("RndB", input, 8, 8)
                .AppendByteArray("ClientCryptogram", input, 16, 16);

            if (input.Length != 32)
            {
                output.AppendItem("NonCompliance", "Invalid Payload");
            }
        }
    }
}