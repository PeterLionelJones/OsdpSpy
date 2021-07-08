using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    public class CommandAnnotator : ExchangeAnnotator
    {
        public CommandAnnotator() : base(3) {}
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            if (input.Acu.Payload.Cipher != null)
            {
                output.Annotate(
                    "  ACU Cipher:      {AcuCipher}\n", 
                    input.Acu.Payload.Cipher.ToHexString());
            }
            
            if (input.Acu.Payload.Plain != null)
            {
                output.Annotate(
                    "  ACU Plain:       {AcuPlain}\n", 
                    input.Acu.Payload.Plain.ToHexString());
            }
        }
    }
}