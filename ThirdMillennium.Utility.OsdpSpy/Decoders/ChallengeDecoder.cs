using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class ChallengeDecoder : ICommandDecoder
    {
        public Command Command => Command.CHLNG;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendByteArray("RndA", input);

            if (input.Length != 8)
            {
                output.AppendItem("NonCompliance", "Invalid Payload");
            }
        }
    }
}