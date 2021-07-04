using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    public class ReplyAnnotator : IReplyAnnotator
    {
        public void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Pd?.Payload == null) return;
            
            if (input.Pd.Payload.Cipher != null)
            {
                output.Annotate(
                    "  PD Cipher:       {PdCipher}\n", 
                    input.Pd.Payload.Cipher.ToHexString());
            }
            
            if (input.Pd.Payload.Plain != null)
            {
                output.Annotate(
                    "  PD Plain:        {PdPlain}\n", 
                    input.Pd.Payload.Plain.ToHexString());
            }
        }
    }
}