using ThirdMillennium.Annotations;
using ThirdMillennium.Protocol;

namespace ThirdMillennium.Utility.OSDP
{
    public class CommandAnnotator : ExchangeAnnotator
    {
        public CommandAnnotator(ICommandDecoderCollection decoder)
            : base(3)
        {
            _decoder = decoder;
        }

        private readonly ICommandDecoderCollection _decoder;
        
        public override void Annotate(IExchange input, IAnnotation output)
        {
            output.AppendNewLine();
            
            output.AppendItem("ACUCommand", input.Acu.Frame.Command.ToString());

            if (input.Acu.Payload.Cipher != null)
            {
                output.AppendItem("ACUCipher", input.Acu.Payload.Cipher.ToHexString());
            }
            
            if (input.Acu.Payload.Plain != null)
            {
                output.AppendItem("ACUPlain",input.Acu.Payload.Plain.ToHexString());
                _decoder.Decode(input.Acu.Frame.Command, input.Acu.Payload.Plain, output);
            }
        }
    }
}