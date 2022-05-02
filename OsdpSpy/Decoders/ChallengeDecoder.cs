using OsdpSpy.Annotations;
using OsdpSpy.Osdp;

namespace OsdpSpy.Decoders
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