using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;
using ThirdMillennium.Protocol.OSDP;

namespace ThirdMillennium.Utility.OSDP
{
    public class ServerCryptogramDecoder : ICommandDecoder
    {
        public Command Command => Command.SCRYPT;

        public void Decode(byte[] input, IAnnotation output)
        {
            output.AppendByteArray("ServerCryptogram", input);

            if (input.Length != 16)
            {
                output.AppendItem("NonCompliance", "Invalid Payload");
            }
        }
    }
}